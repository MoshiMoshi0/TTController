using System.Collections.Generic;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.RawEffect
{
    public class RawEffectConfig : EffectConfigBase
    {
        public string EffectType { get; set; }
        public List<LedColor> Colors { get; set; }
    }

    public class RawEffect : EffectBase<RawEffectConfig>
    {
        public override string EffectType => Config.EffectType;

        public RawEffect(RawEffectConfig config) : base(config) { }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            return ports.ToDictionary(p => p, p => Config.Colors.ToList());
        }
    }
}
