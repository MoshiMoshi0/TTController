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
                return EffectUtils.GenerateColorsPerPort(ports, cache,
                    (port, ledCount) => Config.Color.Get(ledCount).Select(c => LedColor.ChangeValue(c, c.GetValue() * _t)).ToList()
                );
            }

            if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLedCount = ports.Sum(p => cache.GetDeviceConfig(p).LedCount);
                var colors = Config.Color.Get(totalLedCount).Select(c => LedColor.ChangeValue(c, c.GetValue() * _t)).ToList();
                return EffectUtils.SplitColorsPerPort(colors, ports, cache);
            }

            return null;
        }
    }
}
