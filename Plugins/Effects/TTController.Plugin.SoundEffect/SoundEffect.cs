using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using TTController.Common;

namespace TTController.Plugin.SoundEffect
{
    public class SoundEffectConfig : EffectConfigBase
    {
        public bool UseAverage { get; private set; } = true;
        public int MinimumFrequency { get; private set; } = 100;
        public int MaximumFrequency { get; private set; } = 10000;
        public ScalingStrategy ScalingStrategy { get; private set; } = ScalingStrategy.Sqrt;
        public double ScalingFactor { get; private set; } = 2;
        public ColorGenerationMethod ColorGenerationMethod { get; private set; } = ColorGenerationMethod.SpanPorts;
        public LedColor FromColor { get; private set; }
        public LedColor ToColor { get; private set; }
    }

    public class SoundEffect : EffectBase<SoundEffectConfig>
    {
        private readonly FftSize _fftSize;
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

            _fftSize = FftSize.Fft1024;
            _fftBuffer = new float[(int)_fftSize];
            _spectrumProvider = new SpectrumProvider(sampleSource.WaveFormat.Channels, sampleSource.WaveFormat.SampleRate, _fftSize);

            var notificationSource = new DataNotificationSource(sampleSource);
            notificationSource.DataRead += (s, e) => { _spectrumProvider.Add(e.Data, e.Data.Length); };

            var waveSource = notificationSource.ToWaveSource(16);
            var buffer = new byte[waveSource.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, e) => { while (waveSource.Read(buffer, 0, buffer.Length) > 0) ; };

            _spectrum = new LedSpectrum(GenerateColor)
            {
                FftSize = _fftSize,
                SpectrumProvider = _spectrumProvider,
                UseAverage = Config.UseAverage,
                MinimumFrequency = Config.MinimumFrequency,
                MaximumFrequency = Config.MaximumFrequency,
                ScalingStrategy = Config.ScalingStrategy,
                ScalingFactor = Config.ScalingFactor,
                IsXLogScale = false,
                SpectrumResolution = (int) _fftSize
            };

            _spectrum.UpdateFrequencyMapping();
            _soundIn.Start();
        }

        public override byte EffectByte => (byte) EffectType.ByLed;
        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            if (!_spectrumProvider.GetFftData(_fftBuffer, this))
                return null;
            
            return _spectrum.GenerateColors(Config.ColorGenerationMethod, ports, cache, _fftBuffer);
        }

        public LedColor GenerateColor(double fftValue)
        {
            var rr = Config.FromColor.R * (1 - fftValue) + Config.ToColor.R * fftValue;
            var gg = Config.FromColor.G * (1 - fftValue) + Config.ToColor.G * fftValue;
            var bb = Config.FromColor.B * (1 - fftValue) + Config.ToColor.B * fftValue;
            return new LedColor((byte)rr, (byte)gg, (byte)bb);
        }

        public override void Dispose()
        {
            _soundIn.Dispose();
            base.Dispose();
        }
    }
}
