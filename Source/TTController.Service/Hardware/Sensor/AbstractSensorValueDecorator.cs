namespace TTController.Service.Hardware.Sensor
{
    public abstract class AbstractSensorValueDecorator : ISensorValueProvider
    {
        protected float? CurrentValue { set; get; }
        protected ISensorValueProvider SensorValueProvider { get; }

        protected AbstractSensorValueDecorator(ISensorValueProvider sensorValueProvider)
        {
            SensorValueProvider = sensorValueProvider;
        }

        public abstract void Update();
        public virtual float Value() => ValueOrDefault(float.NaN);
        public virtual float ValueOrDefault(float defaultValue) => CurrentValue ?? defaultValue;
    }
}
