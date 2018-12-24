using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenHardwareMonitor.Hardware;
using TTController.Service.Hardware.Temperature;

namespace TTController.Service.Managers
{
    public class TemperatureManager
    {
        private readonly Computer _computer;
        private readonly List<ISensor> _sensors;
        private readonly Dictionary<Identifier, ITemperatureProvider> _providers;
        private readonly ITemperatureProviderFactory _providerFactory;

        public TemperatureManager(ITemperatureProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;

            _sensors = new List<ISensor>();
            _providers = new Dictionary<Identifier, ITemperatureProvider>();
            
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
            var hardwareList = _providers
                .Select(kv => _sensors.FirstOrDefault(s => s.Identifier == kv.Key)?.Hardware)
                .Where(h => h != null)
                .Distinct();

            foreach (var hardware in hardwareList)
                hardware.Update();
        }

        public void EnableSensor(Identifier identifier)
        {
            if (_providers.ContainsKey(identifier))
                return;

            var sensor = _sensors.FirstOrDefault(s => s.Identifier == identifier);
            if (sensor == null)
                return;

            _providers.Add(identifier, _providerFactory.Create(sensor));
        }

        public void DisableSensor(Identifier identifier)
        {
            if (!_providers.ContainsKey(identifier))
                return;

            _providers.Remove(identifier);
        }
    }
}
