using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using OpenHardwareMonitor.Hardware;
using TTController.Common;
using TTController.Service.Hardware;
using TTController.Service.Hardware.Sensor;
using TTController.Service.Utils;

namespace TTController.Service.Manager
{
    public sealed class SensorManager : IDataProvider, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISensorValueProviderFactory _sensorValueProviderFactory;
        private readonly IReadOnlyDictionary<Identifier, SensorConfig> _sensorConfigs;

        private readonly OpenHardwareMonitorFacade _openHardwareMonitorFacade;
        private readonly Dictionary<Identifier, ISensorValueProvider> _sensorValueProviders;
        private readonly HashSet<IHardware> _hardware;

        private bool _cacheInitialized;

        public IEnumerable<Identifier> EnabledSensors => _sensorValueProviders.Keys;

        public SensorManager(ISensorValueProviderFactory sensorValueProviderFactory, IReadOnlyDictionary<Identifier, SensorConfig> sensorConfigs)
        {
            Logger.Info("Creating Sensor Manager...");
            _sensorValueProviderFactory = sensorValueProviderFactory;
            _sensorConfigs = sensorConfigs;

            _openHardwareMonitorFacade = new OpenHardwareMonitorFacade();
            _sensorValueProviders = new Dictionary<Identifier, ISensorValueProvider>();
            _hardware = new HashSet<IHardware>();

            _cacheInitialized = false;

            EnableSensors(sensorConfigs.Keys);
        }

        public void Update()
        {
            foreach (var hardware in _hardware)
                hardware.Update();

            foreach (var provider in _sensorValueProviders.Values)
                provider.Update();
        }

        public float GetSensorValue(Identifier identifier)
        {
            if (!_sensorValueProviders.ContainsKey(identifier))
                return float.NaN;

            return _sensorValueProviders[identifier].ValueOrDefault(float.NaN);
        }

        public void EnableSensor(Identifier identifier)
        {
            if (_sensorValueProviders.ContainsKey(identifier))
                return;

            var sensor = _openHardwareMonitorFacade.Sensors.FirstOrDefault(s => s.Identifier == identifier);
            if (sensor == null)
                return;

            Logger.Info("Enabling sensor: {0}", sensor.Identifier);

            var sensorValueProvider = _sensorValueProviderFactory.Create(sensor);
            if(_sensorConfigs.TryGetValue(identifier, out var config) && config.Offset.HasValue)
                sensorValueProvider = new OffsetSensorValueDecorator(sensorValueProvider, config.Offset.Value);

            _sensorValueProviders.Add(identifier, sensorValueProvider);
            _hardware.Add(sensor.Hardware);
        }

        public void EnableSensors(IEnumerable<Identifier> identifiers)
        {
            if (identifiers == null)
                return;

            foreach (var identifier in identifiers)
                EnableSensor(identifier);
        }

        public void DisableSensor(Identifier identifier)
        {
            if (!_sensorValueProviders.ContainsKey(identifier))
                return;

            var sensor = _openHardwareMonitorFacade.Sensors.FirstOrDefault(s => s.Identifier == identifier);
            if (sensor == null)
                return;

            Logger.Info("Disabling sensor: {0}", identifier);
            _sensorValueProviders.Remove(identifier);

            var removeHardware = _openHardwareMonitorFacade.Sensors
                .Where(s => EnabledSensors.Contains(s.Identifier) && s.Identifier != sensor.Identifier)
                .All(s => s.Hardware != sensor.Hardware);

            if (removeHardware)
                _hardware.Remove(sensor.Hardware);
        }

        public void DisableSensors(IEnumerable<Identifier> identifiers)
        {
            if (identifiers == null)
                return;

            foreach (var identifier in identifiers)
                DisableSensor(identifier);
        }

        public void Accept(ICacheCollector collector)
        {
            if (!_cacheInitialized)
            {
                _cacheInitialized = true;
                foreach (var sensor in EnabledSensors)
                    collector.StoreSensorConfig(sensor, SensorConfig.Default);
            }

            foreach (var (sensor, provider) in _sensorValueProviders)
                collector.StoreSensorValue(sensor, provider.Value());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing SensorManager...");

            _openHardwareMonitorFacade.Dispose();
            _sensorValueProviders.Clear();
            _hardware.Clear();
        }
    }
}
