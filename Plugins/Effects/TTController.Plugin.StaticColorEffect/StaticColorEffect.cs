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
                return EffectUtils.GenerateColorsPerPort(ports, cache, (port, ledCount) => GenerateColors(ledCount, cache));
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();
                return EffectUtils.SplitColorsPerPort(GenerateColors(totalLedCount, cache), ports, cache);
            }

            return null;
        }

        public override List<LedColor> GenerateColors(int count, ICacheProvider cache)
            => Config.Color.Get(count).ToList();
    }
}
