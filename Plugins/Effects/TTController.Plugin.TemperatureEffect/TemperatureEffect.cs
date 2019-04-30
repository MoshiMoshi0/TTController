using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OpenHardwareMonitor.Hardware;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.TemperatureEffect
{
    public class TemperatureEffectConfig : EffectConfigBase
    {
        public List<Identifier> Sensors { get; private set; } = new List<Identifier>();
        [DefaultValue(SensorMixFunction.Maximum)] public SensorMixFunction SensorMixFunction { get; private set; } = SensorMixFunction.Maximum;
        public LedColorGradient ColorGradient { get; private set; } = new LedColorGradient();
    }

    public class TemperatureEffect : EffectBase<TemperatureEffectConfig>
    {
        private double _r, _g, _b;
        private readonly float _minTemperature, _maxTemperature;

        public override string EffectType => "Full";

        public TemperatureEffect(TemperatureEffectConfig config) : base(config, config.Sensors)
        {
            _r = config.ColorGradient.Start.Color.R;
            _g = config.ColorGradient.Start.Color.G;
            _b = config.ColorGradient.Start.Color.B;

            _minTemperature = (float) config.ColorGradient.Start.Location;
            _maxTemperature = (float) config.ColorGradient.End.Location;
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

            if (temperature < _minTemperature)
                temperature = _minTemperature;
            if (temperature > _maxTemperature)
                temperature = _maxTemperature;

            if (float.IsNaN(temperature))
            {
                _r = Config.ColorGradient.Start.Color.R;
                _g = Config.ColorGradient.Start.Color.G;
                _b = Config.ColorGradient.Start.Color.B;
            }
            else
            {
                var (rr, gg, bb) = Config.ColorGradient.ColorAtDeconstruct(temperature);

                const float t = 0.05f;
                _r = _r * (1 - t) + rr * t;
                _g = _g * (1 - t) + gg * t;
                _b = _b * (1 - t) + bb * t;
            }

            var color = new LedColor((byte) _r, (byte) _g, (byte) _b);
            return ports.ToDictionary(p => p, _ => new List<LedColor>{ color });
        }
    }
}
