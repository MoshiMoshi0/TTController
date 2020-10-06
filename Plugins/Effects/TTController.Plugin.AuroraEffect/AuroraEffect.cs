using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.AuroraEffect
{
    public class AuroraEffectConfig : EffectConfigBase
    {
        [DefaultValue(0.003f)] public float Step { get; internal set; } = 0.003f;
        [DefaultValue(64)] public int Length { get; internal set; } = 64;
        [DefaultValue(false)] public bool Mirror { get; internal set; } = false;
        [DefaultValue(1)] public float Brightness { get; internal set; } = 1;
        [DefaultValue(1)] public float Saturation { get; internal set; } = 1;
        [DefaultValue(null)] public LedColorGradient Gradient { get; internal set; } = null;
    }

    public class AuroraEffect : EffectBase<AuroraEffectConfig>
    {
        private float _rotation;

        public AuroraEffect(AuroraEffectConfig config) : base(config)
        {
            if (Config.Gradient == null)
            {
                var colors = Enumerable.Range(0, Config.Length)
                    .Select(x => LedColor.FromHsv(x / (Config.Length - 1f) * 360, Config.Saturation, Config.Brightness));
                Config.Gradient = new LedColorGradient(colors);
            }
        }

        public override string EffectType => "PerLed";

        public override void Update(ICacheProvider cache)
        {
            _rotation += Config.Step;
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                return EffectUtils.GenerateColorsPerPort(ports, cache, (_, ledCount) => GenerateColors(ledCount, cache));
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();
                var colors = GenerateColors(totalLedCount, cache);
                return EffectUtils.SplitColorsPerPort(colors, ports, cache);
            }

            return null;
        }

        public override List<LedColor> GenerateColors(int count, ICacheProvider cache)
        {
            float Wrap(float a, float b) => (a % b + b) % b;
            LedColor GetColor(int i) => Config.Gradient.GetColor(Wrap((float)i / Config.Length + _rotation, 1));

            if (Config.Mirror)
            {
                var colors = Enumerable.Range(0, count).Select(_ => new LedColor()).ToList();
                var isOdd = count % 2 != 0;
                var halfCount = count / 2 + (isOdd ? 0 : -1);
                for (var i = 0; i <= halfCount; i++)
                {
                    var color = GetColor(i);
                    colors[i] = color;
                    if (!isOdd)
                        colors[count - i - 1] = color;
                    else if (i != 0 && (i != count / 2 || isOdd))
                        colors[count - i] = color;
                }

                return colors;
            }
            else
            {
                return Enumerable.Range(0, count).Select(GetColor).ToList();
            }
        }
    }
}
