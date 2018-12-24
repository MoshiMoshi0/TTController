using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Hardware.Temperature
{
    public class TemperatureProvider : ITemperatureProvider
    {
        public ISensor Sensor { get; }
        
        public TemperatureProvider(ISensor sensor)
        {
            Sensor = sensor;
        }

        public virtual float Value()
        {
            return ValueOrDefault(float.NaN);
        }

        public virtual float ValueOrDefault(float defaultValue)
        {
            if (Sensor == null)
                return defaultValue;

            return Sensor.Value.GetValueOrDefault(defaultValue);
        }
    }
}
