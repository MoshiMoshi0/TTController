using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.PulseEffect
{
    public class PulseEffectConfig : EffectConfigBase
    {
        public List<LedColor> Colors { get; private set; } = new List<LedColor>();
        [DefaultValue(0.025)] public double BrightnessStep { get; private set; } = 0.025;
    }

    public class PulseEffect : EffectBase<PulseEffectConfig>
    {
        private double _t;
        private int _direction;
        private readonly double[] _maxBrightness;

        public PulseEffect(PulseEffectConfig config) : base(config)
        {
            _direction = -1;
            _t = 1d;
            _maxBrightness = config.Colors.Select(c =>
            {
                var (h, s, v) = LedColor.ToHsv(c);
                return v;
            }).ToArray();
        }

        public override string EffectType => "ByLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            _t += Config.BrightnessStep * _direction;
            if (_t < 0)
            {
                _direction = 1;
                _t = 0;
            }
            else if (_t > 1)
            {
                _direction = -1;
                _t = 1;
            }

            var colors = new List<LedColor>();
            for (var i = 0; i < Config.Colors.Count; i++)
            {
                var (h, s, v) = LedColor.ToHsv(Config.Colors[i]);
                colors.Add(LedColor.FromHsv(h, s, _maxBrightness[i] * _t));
            }

            return ports.ToDictionary(p => p, _ => colors.ToList());
        }
    }
}
