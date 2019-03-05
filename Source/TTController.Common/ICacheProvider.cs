using OpenHardwareMonitor.Hardware;

namespace TTController.Common
{
    public interface ICacheProvider
    {
        float GetTemperature(Identifier sensor);
        PortData GetPortData(PortIdentifier port);
        PortConfig GetPortConfig(PortIdentifier port);
    }
}
