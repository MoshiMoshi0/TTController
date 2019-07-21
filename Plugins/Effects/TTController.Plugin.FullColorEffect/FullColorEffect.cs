using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.FullColorEffect
{
    public class FullColorEffectConfig : EffectConfigBase
    {
        public LedColor Color { get; private set; } = new LedColor(0, 0, 0);
    }

    public class FullColorEffect : EffectBase<FullColorEffectConfig>
    {
        public FullColorEffect(FullColorEffectConfig config) : base(config) { }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            return ports.ToDictionary(p => p, p => Enumerable.Repeat(Config.Color, cache.GetPortConfig(p).LedCount).ToList());
        }
    }
}