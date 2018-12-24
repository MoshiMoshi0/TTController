using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Hardware.Temperature
{
    public class MovingAverageTemperatureProviderFactory : ITemperatureProviderFactory

    {
        private readonly double _alpha;

        public MovingAverageTemperatureProviderFactory() : this(1.0) {}

        public MovingAverageTemperatureProviderFactory(double alpha)
        {
            _alpha = alpha;
        }

        public ITemperatureProvider Create(ISensor sensor)
        {
            return new MovingAverageTemperatureProvider(sensor, _alpha);
        }
    }
}
