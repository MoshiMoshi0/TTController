using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.PulseEffect
{
    public class PulseEffectConfig : EffectConfigBase
    {
        [DefaultValue(0.025)] public double BrightnessStep { get; private set; } = 0.025;
        [DefaultValue(null)] public List<LedColor> Colors { get; private set; } = null;
        [DefaultValue(null)] public LedColor? Color { get; private set; } = null;
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
            _colors = new LedColor[Config.Color != null ? 1 : Config.Colors.Count];
        }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            void Update(LedColor color, int index)
            {
                var (h, s, v) = LedColor.ToHsv(color);
                _colors[index] = LedColor.FromHsv(h, s, v * _t);
            }

            _t += Config.BrightnessStep * _direction;
            if (_t < 0 || _t > 1)
                _direction = -_direction;

            if (Config.Color != null)
                Update(Config.Color.Value, 0);
            else if (Config.Colors != null)
                for (var i = 0; i < Config.Colors.Count; i++)
                    Update(Config.Colors[i], i);

            if (Config.Color != null)
            {
                return ports.ToDictionary(p => p, p => Enumerable.Repeat(_colors[0], cache.GetDeviceConfig(p).LedCount).ToList());
            }
            else if (Config.Colors != null)
            {
                if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
                    return ports.ToDictionary(p => p, _ => _colors.ToList());

                if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
                {
                    var result = new Dictionary<PortIdentifier, List<LedColor>>();

                    var offset = 0;
                    foreach (var port in ports)
                    {
                        var config = cache.GetPortConfig(port);
                        if (config == null)
                            continue;

                        var ledCount = cache.GetDeviceConfig(port).LedCount;
                        result.Add(port, _colors.Skip(offset).Take(ledCount).ToList());
                        offset += ledCount;
                    }

                    return result;
                }
            }

            return null;
        }
    }
}
