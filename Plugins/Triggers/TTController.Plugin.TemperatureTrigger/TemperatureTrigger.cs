using OpenHardwareMonitor.Hardware;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.TemperatureTrigger
{
    public enum IntervalType
    {
        Greater,
        Less
    }

    public class TemperatureTriggerConfig : TriggerConfigBase
    {
        public List<Identifier> Sensors { get; private set; } = new List<Identifier>();
        [DefaultValue(SensorMixFunction.Maximum)] public SensorMixFunction SensorMixFunction { get; private set; } = SensorMixFunction.Maximum;
        [DefaultValue(float.NaN)] public float Temperature { get; private set; } = float.NaN;
        [DefaultValue(IntervalType.Greater)] public IntervalType IntervalType { get; private set; } = IntervalType.Greater;
    }

    public class TemperatureTrigger : TriggerBase<TemperatureTriggerConfig>
    {
        public TemperatureTrigger(TemperatureTriggerConfig config) : base(config, config.Sensors) { }

        public override bool Value(ICacheProvider cache)
        {
            var temperatures = Config.Sensors.Select(cache.GetSensorValue);
            var temperature = float.NaN;
            if (Config.SensorMixFunction == SensorMixFunction.Average)
                temperature = temperatures.Average();
            else if (Config.SensorMixFunction == SensorMixFunction.Minimum)
                temperature = temperatures.Min();
            else if (Config.SensorMixFunction == SensorMixFunction.Maximum)
                temperature = temperatures.Max();

            if (float.IsNaN(temperature))
                return false;

            if (Config.IntervalType == IntervalType.Greater)
                return temperature > Config.Temperature;
            else if (Config.IntervalType == IntervalType.Less)
                return temperature < Config.Temperature;

            return false;
        }
    }
}
