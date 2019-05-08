using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Hardware.Sensor
{
    public class SensorValueProvider : ISensorValueProvider
    {
        protected float? CurrentValue { set; get; }
        protected ISensor Sensor { get; }

        public SensorValueProvider(ISensor sensor)
        {
            Sensor = sensor;
        }

        public virtual void Update() => CurrentValue = Sensor.Value;
        public virtual float Value() => ValueOrDefault(float.NaN);
        public virtual float ValueOrDefault(float defaultValue) => CurrentValue ?? defaultValue;
    }
}
