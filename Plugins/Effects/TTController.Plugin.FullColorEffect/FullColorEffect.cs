using System.Collections.Generic;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.FullColorEffect
{
    public class FullColorEffectConfig : EffectConfigBase
    {
        public LedColor Color { get; private set; } = new LedColor(0, 0, 0);
    }

    public class FullColorEffect : EffectBase<FullColorEffectConfig>
    {
        public FullColorEffect(FullColorEffectConfig config) : base(config) { }

        public override string EffectType => "Full";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            return ports.ToDictionary(p => p, _ => new List<LedColor>(){ Config.Color });
        }
    }
}