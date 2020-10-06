using System;
using System.Collections.Generic;
using System.ComponentModel;
using CSCore;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.SoundEffect
{
    public class SoundEffectConfig : EffectConfigBase
    {
        [DefaultValue(true)] public bool UseAverage { get; internal set; } = true;
        [DefaultValue(100)] public int MinimumFrequency { get; internal set; } = 100;
        [DefaultValue(10000)] public int MaximumFrequency { get; internal set; } = 10000;
        [DefaultValue(ScalingStrategy.Sqrt)] public ScalingStrategy ScalingStrategy { get; internal set; } = ScalingStrategy.Sqrt;
        [DefaultValue(2.0)] public double ScalingFactor { get; internal set; } = 2.0;
        public LedColorGradient ColorGradient { get; internal set; } = new LedColorGradient();
    }

    public class SoundEffect : EffectBase<SoundEffectConfig>
    {
        private float[] _fftBuffer;
        private SpectrumProvider _spectrumProvider;
        private WasapiLoopbackCapture _soundIn;
        private LedSpectrum _spectrum;
        private bool _initialized;
        private int _lastInitializeTickCount;

        public SoundEffect(SoundEffectConfig config) : base(config)
        {
            _initialized = false;
            Initialize();
        }

        public override string EffectType => "PerLed";
        public override bool IsEnabled(ICacheProvider cache) => Initialize() && base.IsEnabled(cache);

        private bool Initialize()
        {
            if (_initialized)
                return true;

            var currentTicks = Environment.TickCount;
            if (currentTicks - _lastInitializeTickCount < 1000)
                return false;
            _lastInitializeTickCount = currentTicks;

            try
            {
                _soundIn = new WasapiLoopbackCapture();
                _soundIn.Initialize();
            }
            catch (Exception e)
            {
                Logger.Debug(e, "Failed to initialize WasapiLoopbackCapture!");
                return false;
            }

            Logger.Debug($"Initialized WasapiLoopbackCapture on \"{_soundIn.Device.FriendlyName}\"");

            var soundInSource = new SoundInSource(_soundIn);
            var sampleSource = soundInSource.ToSampleSource();

            const FftSize fftSize = FftSize.Fft1024;
            _fftBuffer = new float[(int)fftSize];
            _spectrumProvider = new SpectrumProvider(sampleSource.WaveFormat.Channels, sampleSource.WaveFormat.SampleRate, fftSize);

            var notificationSource = new DataNotificationSource(sampleSource);
            notificationSource.DataRead += (s, e) => _spectrumProvider.Add(e.Data, e.Data.Length);

            var waveSource = notificationSource.ToWaveSource(16);
            var buffer = new byte[waveSource.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, e) => { while (waveSource.Read(buffer, 0, buffer.Length) > 0) ; };

            _spectrum = new LedSpectrum(Config.ColorGradient)
            {
                FftSize = fftSize,
                SpectrumProvider = _spectrumProvider,
                UseAverage = Config.UseAverage,
                MinimumFrequency = Config.MinimumFrequency,
                MaximumFrequency = Config.MaximumFrequency,
                ScalingStrategy = Config.ScalingStrategy,
                ScalingFactor = Config.ScalingFactor,
                IsXLogScale = false
            };

            _soundIn.Start();

            _initialized = true;
            return true;
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            _ = _spectrumProvider.GetFftData(_fftBuffer, this);
            return _spectrum.GenerateColors(Config.ColorGenerationMethod, ports, cache, _fftBuffer);
        }

        public override List<LedColor> GenerateColors(int count, ICacheProvider cache)
        {
            _ = _spectrumProvider.GetFftData(_fftBuffer, this);
            return _spectrum.GenerateColors(count, cache, _fftBuffer);
        }

        protected override void Dispose(bool disposing)
        {
            _soundIn.Dispose();
            base.Dispose(disposing);
        }
    }
}
