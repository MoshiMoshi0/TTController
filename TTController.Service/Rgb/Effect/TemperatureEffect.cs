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
        private LedColor _lastColor;

        public override byte EffectByte => (byte) EffectType.Full;

        public TemperatureEffect(TemperatureEffectConfig config) : base(config)
        {
            _lastColor = Config.StartColor;
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

            var t = (temperature - Config.StartTemperature) /
                               (Config.EndTemperature - Config.StartTemperature);

            var rr = Config.StartColor.R * (1 - t) + Config.EndColor.R * t;
            var gg = Config.StartColor.G * (1 - t) + Config.EndColor.G * t;
            var bb = Config.StartColor.B * (1 - t) + Config.EndColor.B * t;
            var target = new LedColor((byte) rr, (byte) gg, (byte) bb);
            
            t = 0.9f;
            rr = _lastColor.R * (1 - t) + target.R * t;
            gg = _lastColor.G * (1 - t) + target.G * t;
            bb = _lastColor.B * (1 - t) + target.B * t;

            var color = new LedColor((byte) rr, (byte) gg, (byte) bb);
            _lastColor = color;
            return ports.ToDictionary(p => p, p => new List<LedColor>{color});
        }
    }
}
