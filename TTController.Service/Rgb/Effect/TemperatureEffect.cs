using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
using TTController.Common;
using TTController.Service.Manager;
using TTController.Service.Speed.Controller;

namespace TTController.Service.Rgb.Effect
{
    public class TemperatureEffectConfig : EffectConfigBase
    {
        public List<Identifier> Sensors { get; set; }
        public SensorMixFunction SensorMixFunction { get; set; } = SensorMixFunction.Maximum;
        public LedColor StartColor { get; set; }
        public LedColor EndColor { get; set; }
        public int StartTemperature { get; set; }
        public int EndTemperature { get; set; }
    }

    public class TemperatureEffect : EffectBase<TemperatureEffectConfig>
    {
        private float _r, _g, _b;

        public override byte EffectByte => (byte) EffectType.Full;

        public TemperatureEffect(TemperatureEffectConfig config) : base(config)
        {
            _r = Config.StartColor.R;
            _g = Config.StartColor.G;
            _b = Config.StartColor.B;
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var temperatures = Config.Sensors.Select(cache.GetTemperature);
            var temperature = float.NaN;
            if (Config.SensorMixFunction == SensorMixFunction.Average)
                temperature = temperatures.Average();
            else if (Config.SensorMixFunction == SensorMixFunction.Minimum)
                temperature = temperatures.Min();
            else if (Config.SensorMixFunction == SensorMixFunction.Maximum)
                temperature = temperatures.Max();

            if (temperature < Config.StartTemperature)
                temperature = Config.StartTemperature;
            if (temperature > Config.EndTemperature)
                temperature = Config.EndTemperature;

            if (float.IsNaN(temperature))
            {
                _r = Config.EndColor.R;
                _g = Config.EndColor.G;
                _b = Config.EndColor.B;
            }
            else
            {
                var t = (temperature - Config.StartTemperature) /
                        (Config.EndTemperature - Config.StartTemperature);

                var rr = Config.StartColor.R * (1 - t) + Config.EndColor.R * t;
                var gg = Config.StartColor.G * (1 - t) + Config.EndColor.G * t;
                var bb = Config.StartColor.B * (1 - t) + Config.EndColor.B * t;

                t = 0.05f;
                _r = _r * (1 - t) + rr * t;
                _g = _g * (1 - t) + gg * t;
                _b = _b * (1 - t) + bb * t;
            }

            var color = new LedColor((byte) _r, (byte) _g, (byte) _b);
            return ports.ToDictionary(p => p, p => new List<LedColor>{ color });
        }
    }
}
