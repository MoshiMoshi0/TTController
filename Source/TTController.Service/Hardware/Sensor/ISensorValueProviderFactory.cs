using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Hardware.Sensor
{
    public interface ISensorValueProviderFactory
    {
        ISensorValueProvider Create(ISensor sensor);
    }
}
