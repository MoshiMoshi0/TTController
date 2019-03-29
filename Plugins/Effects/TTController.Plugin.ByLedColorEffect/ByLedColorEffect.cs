using System.Collections.Generic;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.ByLedColorEffect
{
    public class ByLedColorEffectConfig : EffectConfigBase
    {
        public List<LedColor> Colors { get; private set; } = new List<LedColor>();
    }

    public class ByLedColorEffect : EffectBase<ByLedColorEffectConfig>
    {
        public ByLedColorEffect(ByLedColorEffectConfig config) : base(config) { }

        public override string EffectType => "ByLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache) => 
            ports.ToDictionary(p => p, p => Config.Colors.ToList());
    }
}
