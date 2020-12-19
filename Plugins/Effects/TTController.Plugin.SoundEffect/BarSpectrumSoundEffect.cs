using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.SoundEffect
{
    public class BarSpectrumSoundEffectConfig : SpectrumSoundEffectConfigBase
    {
        public LedColorProvider BackgroundColor { get; internal set; } = new LedColorProvider();
        public LedColorProvider BarColor { get; internal set; } = new LedColorProvider();

        [DefaultValue(4)] public int FrequencyPointCount { get; internal set; } = 4;
        [DefaultValue(false)] public bool Mirror { get; internal set; } = false;
    }

    public class BarSpectrumSoundEffect : SpectrumSoundEffectBase<BarSpectrumSoundEffectConfig>
    {
        public BarSpectrumSoundEffect(BarSpectrumSoundEffectConfig config) : base(config)
        {
            Spectrum.UpdateFrequencyMappingIfNecessary(config.FrequencyPointCount);
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(ColorGenerationMethod generationMethod,
            List<PortIdentifier> ports, ICacheProvider cache, float[] fftBuffer)
        {
            var points = Spectrum.CalculateSpectrumPoints(1.0f, fftBuffer);
            if (generationMethod == ColorGenerationMethod.PerPort)
            {
                return EffectUtils.GenerateColorsPerPort(ports, cache, (_, ledCount) => GenerateColors(ledCount, points));
            }
            else if (generationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();
                var colors = GenerateColors(totalLedCount, points);

                if (Config.Mirror)
                    return EffectUtils.SplitMirroredColorsPerPort(colors, ports, cache);
                else
                    return EffectUtils.SplitColorsPerPort(colors, ports, cache);
            }

            return null;
        }

        public override List<LedColor> GenerateColors(int count, ICacheProvider cache, float[] fftBuffer)
        {
            var points = Spectrum.CalculateSpectrumPoints(1.0f, fftBuffer);
            return GenerateColors(count, points);
        }

        private List<LedColor> GenerateColors(int count, List<SpectrumPoint> points)
        {
            var value = Math.Max(0, Math.Min(1, points.Select(p => p.Value).Average()));

            LedColor GetColor(int i, int size)
            {
                var barSize = (int)Math.Round(size * value);
                return i <= barSize ? Config.BarColor.Get(i, size) : Config.BackgroundColor.Get(i, size);
            }

            if (Config.Mirror)
                return EffectUtils.GenerateMirroredColors(count, (index, halfSize) => GetColor(index, halfSize));
            else
                return Enumerable.Range(0, count).Select(x => GetColor(x, count)).ToList();
        }
    }
}
