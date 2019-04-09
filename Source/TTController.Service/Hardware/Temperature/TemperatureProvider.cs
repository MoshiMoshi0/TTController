using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Hardware.Temperature
{
    public class TemperatureProvider : ITemperatureProvider
    {
        protected float? CurrentValue { set; get; }
        protected ISensor Sensor { get; }

        public TemperatureProvider(ISensor sensor)
        {
            Sensor = sensor;
        }

        public virtual void Update() => CurrentValue = Sensor.Value;
        public virtual float Value() => ValueOrDefault(float.NaN);
        public virtual float ValueOrDefault(float defaultValue) => CurrentValue ?? defaultValue;
    }
}
