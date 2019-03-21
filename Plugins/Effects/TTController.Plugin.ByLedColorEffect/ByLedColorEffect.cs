using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;

namespace TTController.Plugin.ByLedColorEffect
{
    public class ByLedColorEffectConfig : EffectConfigBase
    {
        public List<LedColor> Colors { get; set; }
    }

    public class ByLedColorEffect : EffectBase<ByLedColorEffectConfig>
    {
        public ByLedColorEffect(ByLedColorEffectConfig config) : base(config) { }

        public override string EffectType => "ByLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache) => 
            ports.ToDictionary(p => p, p => Config.Colors.ToList());
    }
}
