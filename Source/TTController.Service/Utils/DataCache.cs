using System.Collections.Concurrent;
using NLog;
using OpenHardwareMonitor.Hardware;
using TTController.Common;

namespace TTController.Service.Utils
{
    public class DataCache : ICacheCollector, ICacheProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CacheProviderProxy _proxy;
        private readonly ConcurrentDictionary<PortIdentifier, PortData> _portDataCache;
        private readonly ConcurrentDictionary<PortIdentifier, PortConfig> _portConfigCache;
        private readonly ConcurrentDictionary<Identifier, float> _temperatureCache;

        public DataCache()
        {
            Logger.Info("Creating DataCache...");

            _proxy = new CacheProviderProxy(this);
            _portDataCache = new ConcurrentDictionary<PortIdentifier, PortData>();
            _portConfigCache = new ConcurrentDictionary<PortIdentifier, PortConfig>();
            _temperatureCache = new ConcurrentDictionary<Identifier, float>();
        }

        public CacheProviderProxy GetProxy() => _proxy;

        public float GetTemperature(Identifier sensor) => _temperatureCache.TryGetValue(sensor, out var temperature) ? temperature : float.NaN;
        public void StoreTemperature(Identifier sensor, float temperature) => _temperatureCache[sensor] = temperature;

        public PortData GetPortData(PortIdentifier port) => _portDataCache.TryGetValue(port, out var data) ? data : null;
        public void StorePortData(PortIdentifier port, PortData data) => _portDataCache[port] = data;

        public PortConfig GetPortConfig(PortIdentifier port) => _portConfigCache.TryGetValue(port, out var config) ? config : null;
        public void StorePortConfig(PortIdentifier port, PortConfig config) => _portConfigCache[port] = config;

        public void Clear()
        {
            _portDataCache.Clear();
            _portConfigCache.Clear();
            _temperatureCache.Clear();
        }
    }

    public class CacheProviderProxy : ICacheProvider
    {
        private readonly ICacheProvider _provider;

        public CacheProviderProxy(ICacheProvider provider)
        {
            _provider = provider;
        }

        public float GetTemperature(Identifier sensor) => _provider.GetTemperature(sensor);
        public PortData GetPortData(PortIdentifier port) => _provider.GetPortData(port);
        public PortConfig GetPortConfig(PortIdentifier port) => _provider.GetPortConfig(port);
    }
}
