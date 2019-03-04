using System.Collections.Generic;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.DefaultEffect
{
    public class DefaultEffectConfig : EffectConfigBase
    {
        public EffectType Type { get; set; }
        public EffectSpeed Speed { get; set; }
        public List<LedColor> Colors { get; set; }
    }

    public class DefaultEffect : EffectBase<DefaultEffectConfig>
    {
        public override byte EffectByte => (byte) ((byte) Config.Type + (byte) Config.Speed);

        public DefaultEffect(DefaultEffectConfig config) : base(config) {}
        
        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            return ports.ToDictionary(p => p, p => Config.Colors.ToList());
        }
    }
}
