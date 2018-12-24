using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Hardware.Temperature
{
    public interface ITemperatureProviderFactory
    {
        ITemperatureProvider Create(ISensor sensor);
    }
}
