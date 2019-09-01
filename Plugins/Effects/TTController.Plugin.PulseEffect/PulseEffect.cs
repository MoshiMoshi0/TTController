using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.PulseEffect
{
    public class PulseEffectConfig : EffectConfigBase
    {
        public List<LedColor> Colors { get; private set; } = new List<LedColor>();
        [DefaultValue(0.025)] public double BrightnessStep { get; private set; } = 0.025;
    }

    public class PulseEffect : EffectBase<PulseEffectConfig>
    {
        private readonly LedColor[] _colors;

        private double _t;
        private int _direction;

        public PulseEffect(PulseEffectConfig config) : base(config)
        {
            _direction = -1;
            _t = 1d;
            _colors = new LedColor[Config.Colors.Count];
        }

        public override string EffectType => "PerLed";

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

            for (var i = 0; i < Config.Colors.Count; i++)
            {
                var (h, s, v) = LedColor.ToHsv(Config.Colors[i]);
                _colors[i] = LedColor.FromHsv(h, s, v * _t);
            }

            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                return ports.ToDictionary(p => p, _ => _colors.ToList());
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();

                var offset = 0;
                foreach (var port in ports)
                {
                    var config = cache.GetPortConfig(port);
                    if (config == null)
                        continue;

                    result.Add(port, _colors.Skip(offset).Take(config.LedCount).ToList());
                    offset += config.LedCount;
                }

                return result;
            }

            return null;

        }
    }
}
