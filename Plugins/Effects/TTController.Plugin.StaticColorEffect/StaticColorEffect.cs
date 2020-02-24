using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.StaticColorEffect
{
    public class StaticColorEffectConfig : EffectConfigBase
    {
        public LedColorProvider Color { get; internal set; } = new LedColorProvider();
    }

    public class StaticColorEffect : EffectBase<StaticColorEffectConfig>
    {
        public StaticColorEffect(StaticColorEffectConfig config) : base(config) { }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                return ports.ToDictionary(p => p, p => Config.Color.Get(cache.GetDeviceConfig(p).LedCount).ToList()); ;
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();
                var colors = Config.Color.Get(totalLedCount);

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
