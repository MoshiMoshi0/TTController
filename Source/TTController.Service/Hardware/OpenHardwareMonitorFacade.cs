using NLog;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TTController.Service.Hardware
{
    public sealed class OpenHardwareMonitorFacade : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Computer _computer;
        private readonly List<ISensor> _sensors;

        public IReadOnlyList<ISensor> Sensors => _sensors.AsReadOnly();

        public OpenHardwareMonitorFacade()
        {
            Logger.Info("Creating Open Hardware Monitor Proxy...");

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

                Logger.Trace("Valid sensor identifier: {0}", sensor.Identifier);
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
            Logger.Info("Disposing Open Hardware Monitor Proxy...");

            _computer?.Close();
            _sensors.Clear();
        }
    }
}
