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
        [DefaultValue(true)] public bool UseAverage { get; private set; } = true;
        [DefaultValue(100)] public int MinimumFrequency { get; private set; } = 100;
        [DefaultValue(10000)] public int MaximumFrequency { get; private set; } = 10000;
        [DefaultValue(ScalingStrategy.Sqrt)] public ScalingStrategy ScalingStrategy { get; private set; } = ScalingStrategy.Sqrt;
        [DefaultValue(2.0)] public double ScalingFactor { get; private set; } = 2.0;
        public LedColorGradient ColorGradient { get; private set; } = new LedColorGradient();
    }

    public class SoundEffect : EffectBase<SoundEffectConfig>
    {
        private readonly float[] _fftBuffer;
        private readonly SpectrumProvider _spectrumProvider;
        private readonly WasapiLoopbackCapture _soundIn;
        private readonly LedSpectrum _spectrum;

        public SoundEffect(SoundEffectConfig config) : base(config)
        {
            _soundIn = new WasapiLoopbackCapture();
            _soundIn.Initialize();

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

            _spectrum = new LedSpectrum(GenerateColor)
            {
                FftSize = fftSize,
                SpectrumProvider = _spectrumProvider,
                UseAverage = Config.UseAverage,
                MinimumFrequency = Config.MinimumFrequency,
                MaximumFrequency = Config.MaximumFrequency,
                ScalingStrategy = Config.ScalingStrategy,
                ScalingFactor = Config.ScalingFactor,
                IsXLogScale = false,
                SpectrumResolution = (int) fftSize
            };

            _spectrum.UpdateFrequencyMapping();
            _soundIn.Start();
        }

        public override string EffectType => "ByLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            if (!_spectrumProvider.GetFftData(_fftBuffer, this))
                return null;

            return _spectrum.GenerateColors(Config.ColorGenerationMethod, ports, cache, _fftBuffer);
        }

        public LedColor GenerateColor(double fftValue) =>
            Config.ColorGradient.GetColor(fftValue);

        protected override void Dispose(bool disposing)
        {
            _soundIn.Dispose();
            base.Dispose(disposing);
        }
    }
}
