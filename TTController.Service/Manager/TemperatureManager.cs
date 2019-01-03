using System;
using System.Collections.Generic;
using System.Linq;
using OpenHardwareMonitor.Hardware;
using TTController.Service.Hardware.Temperature;
using TTController.Service.Utils;

namespace TTController.Service.Manager
{
    public class TemperatureManager : IDataProvider, IDisposable
    {
        private readonly Computer _computer;
        private readonly List<ISensor> _sensors;
        private readonly Dictionary<Identifier, ITemperatureProvider> _providerMap;
        private readonly ITemperatureProviderFactory _providerFactory;

        public TemperatureManager(ITemperatureProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;

            _sensors = new List<ISensor>();
            _providerMap = new Dictionary<Identifier, ITemperatureProvider>();
            
            _computer = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = true,
                HDDEnabled = true
            };

            _computer.Open();
            _computer.Accept(new SensorVisitor(sensor =>
            {
                if (sensor.SensorType == SensorType.Temperature)
                    _sensors.Add(sensor);
            }));
        }

        public void Update()
        {
            //TODO: should we cache this in Enable/Disable sensor?
            var hardwareList = _providerMap
                .Select(kv => _sensors.FirstOrDefault(s => s.Identifier == kv.Key)?.Hardware)
                .Where(h => h != null)
                .Distinct();

            foreach (var hardware in hardwareList)
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

            _providerMap.Add(identifier, _providerFactory.Create(sensor));
        }

        public void DisableSensor(Identifier identifier)
        {
            if (!_providerMap.ContainsKey(identifier))
                return;

            _providerMap.Remove(identifier);
        }

        public void Visit(ICacheCollector collector)
        {
            foreach (var (sensor, provider) in _providerMap)
                collector.StoreTemperature(sensor, provider.Value());
        }

        public void Dispose()
        {
            _computer?.Close();
        }
    }
}
