using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Hardware.Sensor
{
    public class MovingAverageSensorValueProviderFactory : ISensorValueProviderFactory

    {
        private readonly double _alpha;

        public MovingAverageSensorValueProviderFactory() : this(1.0) { }

        public MovingAverageSensorValueProviderFactory(double alpha)
        {
            _alpha = alpha;
        }

        public ISensorValueProvider Create(ISensor sensor)
        {
            return new MovingAverageSensorValueDecorator(new SensorValueProvider(sensor), _alpha);
        }
    }
}
