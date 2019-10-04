using System.Collections.Concurrent;
using System.Collections.Generic;
using NLog;
using LibreHardwareMonitor.Hardware;
using TTController.Common;

namespace TTController.Service.Utils
{
    public interface IDataProvider
    {
        void Accept(ICacheCollector collector);
    }

    public interface ICacheCollector
    {
        void StoreSensorValue(Identifier sensor, float value);
        void StoreSensorConfig(Identifier sensor, SensorConfig config);
        void StorePortData(PortIdentifier port, PortData data);
        void StorePortConfig(PortIdentifier port, PortConfig config);
        void StorePortSpeed(PortIdentifier port, byte speed);
        void StorePortColors(PortIdentifier port, List<LedColor> colors);
    }

    public class DataCache : ICacheCollector, ICacheProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CacheProviderProxy _providerProxy;
        private readonly CacheCollectorProxy _collectorProxy;

        private readonly ConcurrentDictionary<PortIdentifier, PortData> _portDataCache;
        private readonly ConcurrentDictionary<PortIdentifier, PortConfig> _portConfigCache;
        private readonly ConcurrentDictionary<PortIdentifier, byte> _portSpeedCache;
        private readonly ConcurrentDictionary<PortIdentifier, List<LedColor>> _portColorCache;
        private readonly ConcurrentDictionary<Identifier, float> _sensorValueCache;
        private readonly ConcurrentDictionary<Identifier, SensorConfig> _sensorConfigCache;

        public IReadOnlyDictionary<PortIdentifier, PortData> PortDataCache => _portDataCache;
        public IReadOnlyDictionary<PortIdentifier, PortConfig> PortConfigCache => _portConfigCache;
        public IReadOnlyDictionary<Identifier, float> SensorValueCache => _sensorValueCache;
        public IReadOnlyDictionary<Identifier, SensorConfig> SensorConfigCache => _sensorConfigCache;

        public DataCache()
        {
            Logger.Info("Creating DataCache...");

            _providerProxy = new CacheProviderProxy(this);
            _collectorProxy = new CacheCollectorProxy(this);

            _portDataCache = new ConcurrentDictionary<PortIdentifier, PortData>();
            _portConfigCache = new ConcurrentDictionary<PortIdentifier, PortConfig>();
            _portSpeedCache = new ConcurrentDictionary<PortIdentifier, byte>();
            _portColorCache = new ConcurrentDictionary<PortIdentifier, List<LedColor>>();
            _sensorValueCache = new ConcurrentDictionary<Identifier, float>();
            _sensorConfigCache = new ConcurrentDictionary<Identifier, SensorConfig>();
        }

        public ICacheProvider AsReadOnly() => _providerProxy;
        public ICacheCollector AsWriteOnly() => _collectorProxy;

        public float GetSensorValue(Identifier sensor) => _sensorValueCache.TryGetValue(sensor, out var value) ? value : float.NaN;
        public void StoreSensorValue(Identifier sensor, float value) => _sensorValueCache[sensor] = value;

        public SensorConfig GetSensorConfig(Identifier sensor) => _sensorConfigCache.TryGetValue(sensor, out var config) ? config : null;
        public void StoreSensorConfig(Identifier sensor, SensorConfig config) => _sensorConfigCache[sensor] = config;

        public PortData GetPortData(PortIdentifier port) => _portDataCache.TryGetValue(port, out var data) ? data : null;
        public void StorePortData(PortIdentifier port, PortData data) => _portDataCache[port] = data;

        public PortConfig GetPortConfig(PortIdentifier port) => _portConfigCache.TryGetValue(port, out var config) ? config : null;
        public void StorePortConfig(PortIdentifier port, PortConfig config) => _portConfigCache[port] = config;

        public byte? GetPortSpeed(PortIdentifier port) => _portSpeedCache.TryGetValue(port, out var speed) ? speed : (byte?)null;
        public void StorePortSpeed(PortIdentifier port, byte speed) => _portSpeedCache[port] = speed;

        public List<LedColor> GetPortColors(PortIdentifier port) => _portColorCache.TryGetValue(port, out var colors) ? colors : null;
        public void StorePortColors(PortIdentifier port, List<LedColor> colors) => _portColorCache[port] = colors;

        public void Clear()
        {
            _portDataCache.Clear();
            _portConfigCache.Clear();
            _sensorValueCache.Clear();
        }

        #region Proxy
        public class CacheProviderProxy : ICacheProvider
        {
            private readonly ICacheProvider _provider;

            public CacheProviderProxy(ICacheProvider provider)
            {
                _provider = provider;
            }

            public float GetSensorValue(Identifier sensor) => _provider.GetSensorValue(sensor);
            public SensorConfig GetSensorConfig(Identifier sensor) => _provider.GetSensorConfig(sensor);
            public PortData GetPortData(PortIdentifier port) => _provider.GetPortData(port);
            public PortConfig GetPortConfig(PortIdentifier port) => _provider.GetPortConfig(port);
            public byte? GetPortSpeed(PortIdentifier port) => _provider.GetPortSpeed(port);
            public List<LedColor> GetPortColors(PortIdentifier port) => _provider.GetPortColors(port);
        }

        public class CacheCollectorProxy : ICacheCollector
        {
            private readonly ICacheCollector _collector;

            public CacheCollectorProxy(ICacheCollector collector)
            {
                _collector = collector;
            }

            public void StoreSensorValue(Identifier sensor, float value) => _collector.StoreSensorValue(sensor, value);
            public void StoreSensorConfig(Identifier sensor, SensorConfig config) => _collector.StoreSensorConfig(sensor, config);
            public void StorePortData(PortIdentifier port, PortData data) => _collector.StorePortData(port, data);
            public void StorePortConfig(PortIdentifier port, PortConfig config) => _collector.StorePortConfig(port, config);
            public void StorePortSpeed(PortIdentifier port, byte speed) => _collector.StorePortSpeed(port, speed);
            public void StorePortColors(PortIdentifier port, List<LedColor> colors) => _collector.StorePortColors(port, colors);
        }
        #endregion
    }
}
