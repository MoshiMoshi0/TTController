using NLog;
using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Service.Utils;

namespace TTController.Service.Hardware
{
    public sealed class LibreHardwareMonitorFacade : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Computer _computer;
        private readonly List<ISensor> _sensors;

        public IReadOnlyList<ISensor> Sensors => _sensors.AsReadOnly();

        public LibreHardwareMonitorFacade()
        {
            Logger.Info("Initializing Open Hardware Monitor...");

            _sensors = new List<ISensor>();
            _computer = new Computer()
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsStorageEnabled = true
            };

            _computer.Open();
            _computer.Accept(new SensorVisitor(sensor =>
            {
                _sensors.Add(sensor);
                sensor.ValuesTimeWindow = TimeSpan.Zero;

                Logger.Trace("Valid sensor identifier: {0}", sensor.Identifier);
            }));

            Logger.Debug("Detected {0} sensors", _sensors.Count);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Finalizing Open Hardware Monitor...");

            _computer?.Close();
            _sensors.Clear();
        }
    }
}
