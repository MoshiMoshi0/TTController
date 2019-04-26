using System.Collections.Concurrent;
using NLog;
using OpenHardwareMonitor.Hardware;
using TTController.Common;

namespace TTController.Service.Utils
{
    public interface IDataProvider
    {
        void Accept(ICacheCollector collector);
    }

    public interface ICacheCollector
    {
        void StoreTemperature(Identifier sensor, float temperature);
        void StorePortData(PortIdentifier port, PortData data);
        void StorePortConfig(PortIdentifier port, PortConfig config);
    }

    public class DataCache : ICacheCollector, ICacheProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CacheProviderProxy _providerProxy;
        private readonly CacheCollectorProxy _collectorProxy;
        private readonly ConcurrentDictionary<PortIdentifier, PortData> _portDataCache;
        private readonly ConcurrentDictionary<PortIdentifier, PortConfig> _portConfigCache;
        private readonly ConcurrentDictionary<Identifier, float> _temperatureCache;

        public DataCache()
        {
            Logger.Info("Creating DataCache...");

            _providerProxy = new CacheProviderProxy(this);
            _collectorProxy = new CacheCollectorProxy(this);

            _portDataCache = new ConcurrentDictionary<PortIdentifier, PortData>();
            _portConfigCache = new ConcurrentDictionary<PortIdentifier, PortConfig>();
            _temperatureCache = new ConcurrentDictionary<Identifier, float>();
        }

        public ICacheProvider AsReadOnly() => _providerProxy;
        public ICacheCollector AsWriteOnly() => _collectorProxy;

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

        #region Proxy
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

        public class CacheCollectorProxy : ICacheCollector
        {
            private readonly ICacheCollector _collector;

            public CacheCollectorProxy(ICacheCollector collector)
            {
                _collector = collector;
            }

            public void StoreTemperature(Identifier sensor, float temperature) => _collector.StoreTemperature(sensor, temperature);
            public void StorePortData(PortIdentifier port, PortData data) => _collector.StorePortData(port, data);
            public void StorePortConfig(PortIdentifier port, PortConfig config) => _collector.StorePortConfig(port, config);
        }
        #endregion
    }
}
