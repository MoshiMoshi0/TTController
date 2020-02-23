using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.PulseEffect
{
    public class PulseEffectConfig : EffectConfigBase
    {
        [DefaultValue(0.025)] public double BrightnessStep { get; internal set; } = 0.025;
        public LedColorProvider Color { get; internal set; } = new LedColorProvider();
    }

    public class PulseEffect : EffectBase<PulseEffectConfig>
    {
        private double _t;
        private int _direction;

        public PulseEffect(PulseEffectConfig config) : base(config)
        {
            _direction = -1;
            _t = 1d;
        }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            _t += Config.BrightnessStep * _direction;
            if (_t < 0 || _t > 1)
                _direction = -_direction;

            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();

                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;
                    result.Add(port, Config.Color.Get(ledCount).Select(c => c.SetValue(c.GetValue() * _t)).ToList());
                }

                return result;
            }

            if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();
                var totalLedCount = ports.Sum(p => cache.GetDeviceConfig(p).LedCount);
                var colors = Config.Color.Get(totalLedCount);

                var offset = 0;
                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;
                    result.Add(port, colors.Skip(offset).Take(ledCount).Select(c => c.SetValue(c.GetValue() * _t)).ToList());
                    offset += ledCount;
                }

                return result;
            }

            return null;
        }
    }
}
