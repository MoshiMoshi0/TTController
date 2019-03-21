using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;

namespace TTController.Plugin.SpectrumEffect
{
    public class SpectrumEffectConfig : EffectConfigBase
    {
        public float Saturation { get; set; }
        public float Brightness { get; set; }
        public float HueStep { get; set; }
    }

    public class SpectrumEffect : EffectBase<SpectrumEffectConfig>
    {
        private float _hue;
        
        public SpectrumEffect(SpectrumEffectConfig config) : base(config)
        {
            _hue = 0;
        }

        public override string EffectType => "Full";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            _hue = (_hue + Config.HueStep) % 360;
            var color = LedColor.FromHsv(_hue, Config.Saturation, Config.Brightness);
            return ports.ToDictionary(p => p, p => new List<LedColor>() {color});
        }
    }
}
