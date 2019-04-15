using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Manager
{
    public sealed class SensorManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Computer _computer;
        private readonly List<ISensor> _sensors;

        public IEnumerable<ISensor> Sensors => _sensors;

        public IEnumerable<ISensor> TemperatureSensors =>
            _sensors.Where(s => s.SensorType == SensorType.Temperature);

        public SensorManager()
        {
            Logger.Info("Creating Sensor Manager...");
            _sensors = new List<ISensor>();
            _computer = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = true,
                HDDEnabled = true
            };

            _computer.Open();
            _computer.Accept(new SensorVisitor(sensor =>
            {
                _sensors.Add(sensor);
                sensor.ValuesTimeWindow = TimeSpan.Zero;
            }));

            Logger.Info("Detected sensors: {0}", _sensors.Count);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing SensorManager...");
            _computer?.Close();

            _sensors.Clear();
        }
    }
}
