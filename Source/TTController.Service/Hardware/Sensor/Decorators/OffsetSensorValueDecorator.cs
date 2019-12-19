namespace TTController.Service.Hardware.Sensor.Decorators
{
    public class OffsetSensorValueDecorator : AbstractSensorValueDecorator
    {
        private readonly float _offset;

        public OffsetSensorValueDecorator(ISensorValueProvider sensorValueProvider, float offset)
            : base(sensorValueProvider)
        {
            _offset = offset;

            CurrentValue = null;
        }

        public override void Update()
        {
            SensorValueProvider.Update();
            var newValue = SensorValueProvider.ValueOrDefault(float.NaN);
            if (float.IsNaN(newValue))
                return;

            CurrentValue = newValue + _offset;
        }
    }
}
