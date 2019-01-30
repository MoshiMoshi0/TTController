using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Hardware.Temperature
{
    public class MovingAverageTemperatureProvider : TemperatureProvider
    {
        private readonly double _alpha;

        public MovingAverageTemperatureProvider(ISensor sensor, double alpha) : base(sensor)
        {
            _alpha = alpha;
        }

        public override void Update()
        {
            if (!Sensor.Value.HasValue)
            {
                CurrentValue = float.NaN;
                return;
            }

            if (!CurrentValue.HasValue || float.IsNaN(CurrentValue.Value))
                CurrentValue = Sensor.Value;
            else
                CurrentValue = (float) ((1 - _alpha) * Sensor.Value.Value + _alpha * CurrentValue);
        }
    }
}
