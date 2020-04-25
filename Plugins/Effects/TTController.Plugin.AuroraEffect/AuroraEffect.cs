using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.AuroraEffect
{
    public class AuroraEffectConfig : EffectConfigBase
    {
        [DefaultValue(3)] public int TickInterval { get; internal set; } = 1;
        [DefaultValue(32)] public int Length { get; internal set; } = 64;
        [DefaultValue(null)] public LedColorGradient Gradient { get; internal set; } = null;
    }

    public class AuroraEffect : EffectBase<AuroraEffectConfig>
    {
        private int _tick;
        private int _rotation;

        public AuroraEffect(AuroraEffectConfig config) : base(config)
        {
            if (Config.Gradient == null)
            {
                LedColor GetColor(int index)
                {
                    var normalizedIndex = index / (float)Config.Length;
                    var component = (int) Math.Floor(normalizedIndex * 6);
                    var value = (normalizedIndex - component / 6f) * 6;

                    if (component % 2 == 1)
                        value = 1 - value;

                    var byteValue = (byte)Math.Round(value * 255);
                    if (component == 0) return new LedColor(255, byteValue, 0);
                    else if (component == 1) return new LedColor(byteValue, 255, 0);
                    else if (component == 2) return new LedColor(0, 255, byteValue);
                    else if (component == 3) return new LedColor(0, byteValue, 255);
                    else if (component == 4) return new LedColor(byteValue, 0, 255);
                    else if (component == 5) return new LedColor(255, 0, byteValue);

                    return new LedColor();
                }

                var colors = Enumerable.Range(0, Config.Length).Select(GetColor);
                Config.Gradient = new LedColorGradient(colors);
            }
        }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            int Wrap(int a, int b) => (a % b + b) % b;
            List<LedColor> CalculateColors(int length) =>
                Enumerable.Range(-length / 2, length)
                        .Select(x => Wrap(x + _rotation, Config.Length))
                        .Select(x => Config.Gradient.GetColor((float)x / Config.Length))
                        .ToList();

            if (_tick++ >= Config.TickInterval)
            {
                _tick = 0;
                _rotation++;
            }

            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();

                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;
                    result.Add(port, CalculateColors(ledCount));
                }

                return result;
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();
                var colors = CalculateColors(totalLedCount);

                var offset = 0;
                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;
                    result.Add(port, colors.Skip(offset).Take(ledCount).ToList());
                    offset += ledCount;
                }

                return result;
            }

            return null;
        }
    }
}
