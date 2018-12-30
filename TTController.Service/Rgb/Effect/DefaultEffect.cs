using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;
using TTController.Service.Config;

namespace TTController.Service.Rgb.Effect
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
        
        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(IDictionary<PortIdentifier, PortConfigData> portConfigMap)
        {
            return portConfigMap.ToDictionary(p => p.Key, p => Config.Colors);
        }
    }
}
