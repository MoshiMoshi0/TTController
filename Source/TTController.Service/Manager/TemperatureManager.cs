using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using OpenHardwareMonitor.Hardware;
using TTController.Service.Hardware.Temperature;
using TTController.Service.Utils;

namespace TTController.Service.Manager
{
    public sealed class TemperatureManager : IDataProvider, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly ITemperatureProviderFactory _providerFactory;
        private readonly Dictionary<Identifier, ITemperatureProvider> _providerMap;
        private readonly HashSet<IHardware> _hardware;
        private readonly List<ISensor> _sensors;

        public TemperatureManager(List<ISensor> sensors, ITemperatureProviderFactory providerFactory)
        {
            if(sensors.Any(s => s.SensorType != SensorType.Temperature))
                throw new ArgumentException($"{nameof(sensors)} list can only contain temperature sensors");

            Logger.Info("Creating Temperature Manager...");

            _sensors = sensors;
            _providerFactory = providerFactory;

            _providerMap = new Dictionary<Identifier, ITemperatureProvider>();
            _hardware = new HashSet<IHardware>();
            
            _sensors.ForEach(s => Logger.Info("Valid sensor identifier: {0}", s.Identifier));
        }

        public void Update()
        {
            foreach (var hardware in _hardware)
                hardware.Update();
            
            foreach (var provider in _providerMap.Values)
                provider.Update();
        }

        public float GetSensorValue(Identifier identifier)
        {
            if (!_providerMap.ContainsKey(identifier))
                return float.NaN;

            return _providerMap[identifier].ValueOrDefault(float.NaN);
        }

        public void EnableSensor(Identifier identifier)
        {
            if (_providerMap.ContainsKey(identifier))
                return;

            var sensor = _sensors.FirstOrDefault(s => s.Identifier == identifier);
            if (sensor == null)
                return;

            Logger.Info("Enabling sensor: {0}", sensor.Identifier);
            _providerMap.Add(identifier, _providerFactory.Create(sensor));
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
            if (!_providerMap.ContainsKey(identifier))
                return;

            Logger.Info("Disabling sensor: {0}", identifier);
            _providerMap.Remove(identifier);

            var sensor = _sensors.FirstOrDefault(s => s.Identifier == identifier);
            if (sensor != null && !_sensors.Where(s => s != sensor).Any(s => s.Hardware.Equals(sensor.Hardware)))
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
            foreach (var (sensor, provider) in _providerMap)
                collector.StoreTemperature(sensor, provider.Value());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing TemperatureManager...");
        }
    }
}
