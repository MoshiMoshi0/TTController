using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Config;

namespace TTController.Service.Rgb.Effect
{
    public class ByLedColorEffectConfig : EffectConfigBase
    {
        public List<LedColor> Colors { get; set; }
    }

    public class ByLedColorEffect : EffectBase<ByLedColorEffectConfig>
    {
        public override byte EffectByte => (byte) EffectType.ByLed;

        public ByLedColorEffect(dynamic config) : base((object)config) {}

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(IDictionary<PortIdentifier, PortConfigData> portConfigMap)
        {
            return portConfigMap.ToDictionary(p => p.Key, p => Config.Colors);
        }
    }
}
