using System.Collections.Generic;
using System.ComponentModel;
using CSCore;
using CSCore.DSP;
using CSCore.Streams;
using TTController.Common;

namespace TTController.Plugin.SoundEffect
{
    public class SpectrumSoundEffectConfigBase : SoundEffectConfigBase
    {
        [DefaultValue(false)] public bool UseAverage { get; internal set; } = false;
        [DefaultValue(50)] public int MinimumFrequency { get; internal set; } = 50;
        [DefaultValue(5000)] public int MaximumFrequency { get; internal set; } = 5000;
        [DefaultValue(ScalingStrategy.Sqrt)] public ScalingStrategy ScalingStrategy { get; internal set; } = ScalingStrategy.Sqrt;
        [DefaultValue(2.0f)] public float ScalingFactor { get; internal set; } = 2.0f;
        [DefaultValue(true)] public bool IsXLogScale { get; internal set; } = true;
    }

    public abstract class SpectrumSoundEffectBase<T> : SoundEffectBase<T> where T : SpectrumSoundEffectConfigBase
    {
        private float[] _fftBuffer;
        private SpectrumProvider _spectrumProvider;
        private IWaveSource _waveSource;
        protected SpectrumBase Spectrum { get; private set; }

        public SpectrumSoundEffectBase(T config) : base(config) { }

        public abstract IDictionary<PortIdentifier, List<LedColor>> GenerateColors(ColorGenerationMethod generationMethod,
            List<PortIdentifier> ports, ICacheProvider cache, float[] fftBuffer);
        public abstract List<LedColor> GenerateColors(int count, ICacheProvider cache, float[] fftBuffer);

        protected override void Initialize()
        {
            var soundInSource = new SoundInSource(SoundIn);
            var sampleSource = soundInSource.ToSampleSource();

            const FftSize fftSize = FftSize.Fft1024;
            _fftBuffer = new float[(int)fftSize];
            _spectrumProvider = new SpectrumProvider(sampleSource.WaveFormat.Channels, sampleSource.WaveFormat.SampleRate, fftSize);

            var notificationSource = new NotificationSource(sampleSource)
            {
                Interval = 10
            };

            notificationSource.BlockRead += (s, e) => _spectrumProvider.Add(e.Data, e.Length);
            _waveSource = notificationSource.ToWaveSource(16);

            var buffer = new byte[_waveSource.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, e) => { while (_waveSource.Read(buffer, 0, buffer.Length) > 0) ; };

            var spectrumConfig = new SpectrumConfig()
            {
                FftSize = fftSize,
                UseAverage = Config.UseAverage,
                MinimumFrequency = Config.MinimumFrequency,
                MaximumFrequency = Config.MaximumFrequency,
                ScalingStrategy = Config.ScalingStrategy,
                ScalingFactor = Config.ScalingFactor,
                IsXLogScale = Config.IsXLogScale
            };

            Spectrum = new SpectrumBase(_spectrumProvider, spectrumConfig);
        }

        protected override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            _ = _spectrumProvider.GetFftData(_fftBuffer, this);
            return GenerateColors(Config.ColorGenerationMethod, ports, cache, _fftBuffer);
        }

        protected override List<LedColor> GenerateColors(int count, ICacheProvider cache)
        {
            _ = _spectrumProvider.GetFftData(_fftBuffer, this);
            return GenerateColors(count, cache, _fftBuffer);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _waveSource?.Dispose();
        }
    }
}
