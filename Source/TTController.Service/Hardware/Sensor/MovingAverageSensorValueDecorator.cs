namespace TTController.Service.Hardware.Sensor
{
    public class MovingAverageSensorValueDecorator : AbstractSensorValueDecorator
    {
        private readonly double _alpha;

        public MovingAverageSensorValueDecorator(ISensorValueProvider sensorValueProvider, double alpha)
            : base(sensorValueProvider)
        {
            _alpha = alpha;
        }

        public override void Update()
        {
            SensorValueProvider.Update();
            var newValue = SensorValueProvider.ValueOrDefault(float.NaN);
            if (float.IsNaN(newValue))
                return;

            if (CurrentValue == null)
            {
                CurrentValue = newValue;
                return;
            }

            CurrentValue = (float)((1 - _alpha) * newValue + _alpha * CurrentValue);
        }
    }
}
