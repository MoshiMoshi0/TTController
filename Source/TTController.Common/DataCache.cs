using OpenHardwareMonitor.Hardware;

namespace TTController.Common
{
    public interface IDataProvider
    {
        void Visit(ICacheCollector collector);
    }

    public interface ICacheProvider
    {
        float GetTemperature(Identifier sensor);
        PortData GetPortData(PortIdentifier port);
        PortConfig GetPortConfig(PortIdentifier port);
    }

    public interface ICacheCollector
    {
        void StoreTemperature(Identifier sensor, float temperature);
        void StorePortData(PortIdentifier port, PortData data);
        void StorePortConfig(PortIdentifier port, PortConfig config);
        void Clear();
    }
}
