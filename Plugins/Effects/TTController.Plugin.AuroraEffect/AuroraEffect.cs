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

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            _rotation += Config.Step;
            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                return EffectUtils.GenerateColorsPerPort(ports, cache, (port, ledCount) => GenerateColors(ledCount, 0));
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();

                var offset = 0;
                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;
                    var colors = GenerateColors(ledCount, offset);
                    result.Add(port, colors);
                    offset += Config.Mirror ? ledCount / 2 : ledCount;
                }

                return result;
            }

            return null;
        }

        private List<LedColor> GenerateColors(int ledCount, int offset, bool oddDivide = true)
        {
            float Wrap(float a, float b) => (a % b + b) % b;
            LedColor GetColor(int i) => Config.Gradient.GetColor(Wrap((float)(offset + i) / Config.Length + _rotation, 1));

            if (Config.Mirror)
            {
                var colors = Enumerable.Range(0, ledCount).Select(_ => new LedColor()).ToList();
                var isOdd = ledCount % 2 != 0;
                var halfCount = ledCount / 2 + (oddDivide || isOdd ? 0 : -1);
                for (var i = 0; i <= halfCount; i++)
                {
                    var color = GetColor(i);
                    colors[i] = color;
                    if (!oddDivide && !isOdd)
                        colors[ledCount - i - 1] = color;
                    else if (i != 0 && (i != ledCount / 2 || isOdd))
                        colors[ledCount - i] = color;
                }

                return colors;
            }
            else
            {
                return Enumerable.Range(0, ledCount).Select(GetColor).ToList();
            }
        }
    }
}
