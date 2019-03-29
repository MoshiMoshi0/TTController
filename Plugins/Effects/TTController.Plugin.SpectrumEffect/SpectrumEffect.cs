using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.SpectrumEffect
{
    public class SpectrumEffectConfig : EffectConfigBase
    {
        [DefaultValue(1.0)] public double Saturation { get; private set; } = 1.0;
        [DefaultValue(1.0)] public double Brightness { get; private set; } = 1.0;
        [DefaultValue(1.0)] public double HueStep { get; private set; } = 1.0;
    }

    public class SpectrumEffect : EffectBase<SpectrumEffectConfig>
    {
        private double _hue;
        
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
