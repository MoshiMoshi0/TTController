using LibreHardwareMonitor.Hardware;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.SensorTrigger
{
    public enum ComparsionType
    {
        Equals,
        Greater,
        Less
    }

    public class SensorTriggerConfig : TriggerConfigBase
    {
        public List<Identifier> Sensors { get; private set; } = new List<Identifier>();
        [DefaultValue(SensorMixFunction.Maximum)] public SensorMixFunction SensorMixFunction { get; private set; } = SensorMixFunction.Maximum;
        [DefaultValue(float.NaN)] public float Value { get; private set; } = float.NaN;
        [DefaultValue(ComparsionType.Greater)] public ComparsionType ComparsionType { get; private set; } = ComparsionType.Greater;
    }

    public class SensorTrigger : TriggerBase<SensorTriggerConfig>
    {
        public SensorTrigger(SensorTriggerConfig config) : base(config, config.Sensors) { }

        public override bool Value(ICacheProvider cache)
        {
            var values = Config.Sensors.Select(cache.GetSensorValue);
            var value = float.NaN;
            if (Config.SensorMixFunction == SensorMixFunction.Average)
                value = values.Average();
            else if (Config.SensorMixFunction == SensorMixFunction.Minimum)
                value = values.Min();
            else if (Config.SensorMixFunction == SensorMixFunction.Maximum)
                value = values.Max();

            if (float.IsNaN(value))
                return false;

            if (Config.ComparsionType == ComparsionType.Equals)
                return value == Config.Value;
            if (Config.ComparsionType == ComparsionType.Greater)
                return value > Config.Value;
            else if (Config.ComparsionType == ComparsionType.Less)
                return value < Config.Value;

            return false;
        }
    }
}