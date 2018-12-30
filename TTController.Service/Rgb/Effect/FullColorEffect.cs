using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Service.Config;

namespace TTController.Service.Rgb.Effect
{
    public class FullColorEffectConfig : EffectConfigBase
    {
        public LedColor Color { get; set; }
    }

    public class FullColorEffect : EffectBase<FullColorEffectConfig>
    {
        public FullColorEffect(FullColorEffectConfig config) : base(config){}

        public override byte EffectByte => (byte) EffectType.Full;

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(IDictionary<PortIdentifier, PortConfigData> portConfigMap)
        {
            return portConfigMap.ToDictionary(p => p.Key, p => new List<LedColor> {Config.Color});
        }
    }
}
