using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.PerLedColorEffect
{
    public class PerLedColorEffectConfig : EffectConfigBase
    {
        public List<LedColor> Colors { get; private set; } = new List<LedColor>();
    }

    public class PerLedColorEffect : EffectBase<PerLedColorEffectConfig>
    {
        public PerLedColorEffect(PerLedColorEffectConfig config) : base(config) { }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache) =>
            ports.ToDictionary(p => p, _ => Config.Colors.ToList());
    }
}
