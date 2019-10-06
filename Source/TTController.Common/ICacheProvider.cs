using LibreHardwareMonitor.Hardware;
using System.Collections.Generic;

namespace TTController.Common
{
    public interface ICacheProvider
    {
        float GetSensorValue(Identifier sensor);
        SensorConfig GetSensorConfig(Identifier sensor);
        PortData GetPortData(PortIdentifier port);
        PortConfig GetPortConfig(PortIdentifier port);
        byte? GetPortSpeed(PortIdentifier port);
        List<LedColor> GetPortColors(PortIdentifier port);
        DeviceConfig GetDeviceConfig(PortIdentifier port);
    }
}
