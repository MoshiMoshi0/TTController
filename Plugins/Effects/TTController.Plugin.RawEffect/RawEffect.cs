using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.RawEffect
{
    public class RawEffectConfig : EffectConfigBase
    {
        [DefaultValue(null)] public string EffectType { get; set; } = null;
        public List<LedColor> Colors { get; private set; } = new List<LedColor>();
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
