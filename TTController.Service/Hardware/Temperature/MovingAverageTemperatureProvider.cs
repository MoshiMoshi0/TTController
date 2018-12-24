using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Hardware.Temperature
{
    public class MovingAverageTemperatureProvider : TemperatureProvider
    {
        private readonly double _alpha;
        private volatile float _averageTemperature;

        public MovingAverageTemperatureProvider(ISensor sensor, double alpha) : base(sensor)
        {
            _alpha = alpha;
            _averageTemperature = base.ValueOrDefault(float.NaN);
        }

        public override float Value() => Update(base.Value());
        public override float ValueOrDefault(float defalutValue) => Update(base.ValueOrDefault(defalutValue));

        private float Update(float temperature)
        {
            _averageTemperature = (float)((1 - _alpha) * temperature + _alpha * _averageTemperature);
            return _averageTemperature;
        }
    }
}
