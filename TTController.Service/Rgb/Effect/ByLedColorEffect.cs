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
        private bool _needsUpdate;

        public override byte EffectByte => (byte) EffectType.ByLed;
        public override bool NeedsUpdate() => _needsUpdate;

        public ByLedColorEffect(dynamic config) : base((object)config)
        {
            _needsUpdate = true;
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(IDictionary<PortIdentifier, PortConfigData> portConfigMap)
        {
            _needsUpdate = false;
            return portConfigMap.ToDictionary(p => p.Key, p => Config.Colors);
        }
    }
}
