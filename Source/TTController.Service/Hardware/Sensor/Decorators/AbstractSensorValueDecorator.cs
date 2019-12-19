namespace TTController.Service.Hardware.Sensor.Decorators
{
    public abstract class AbstractSensorValueDecorator : ISensorValueProvider
    {
        protected float? CurrentValue { get; set; }
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
