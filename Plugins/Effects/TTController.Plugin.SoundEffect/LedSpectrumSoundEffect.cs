using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.SoundEffect
{
    public class LedSpectrumSoundEffectConfig : SpectrumSoundEffectConfigBase
    {
        public LedColorGradient ColorGradient { get; internal set; } = new LedColorGradient();
    }

    public class LedSpectrumSoundEffect : SpectrumSoundEffectBase<LedSpectrumSoundEffectConfig>
    {
        public LedSpectrumSoundEffect(LedSpectrumSoundEffectConfig config) : base(config) { }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(ColorGenerationMethod generationMethod,
            List<PortIdentifier> ports, ICacheProvider cache, float[] fftBuffer)
        {
            if (generationMethod == ColorGenerationMethod.PerPort)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();

                var colors = default(List<LedColor>);
                var points = default(List<SpectrumPoint>);
                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;

                    if (Spectrum.UpdateFrequencyMappingIfNecessary(ledCount) || points == null)
                    {
                        points = Spectrum.CalculateSpectrumPoints(1.0f, fftBuffer);
                        colors = GenerateColors(points);
                    }

                    result.Add(port, colors.ToList());
                }

                return result;
            }
            else if (generationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();

                Spectrum.UpdateFrequencyMappingIfNecessary(totalLedCount);
                var points = Spectrum.CalculateSpectrumPoints(1.0f, fftBuffer);
                var colors = GenerateColors(points);
                return EffectUtils.SplitColorsPerPort(colors, ports, cache);
            }

            return null;
        }

        public override List<LedColor> GenerateColors(int count, ICacheProvider cache, float[] fftBuffer)
        {
            Spectrum.UpdateFrequencyMappingIfNecessary(count);
            var points = Spectrum.CalculateSpectrumPoints(1.0f, fftBuffer);
            return GenerateColors(points);
        }

        private List<LedColor> GenerateColors(List<SpectrumPoint> points)
            => points.Select(p => Config.ColorGradient.GetColor(p.Value)).ToList();
    }
}
