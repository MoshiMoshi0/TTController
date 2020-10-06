using NLog;
using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;

namespace TTController.Service.Hardware
{
    public sealed class LibreHardwareMonitorFacade : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Computer _computer;
        private readonly List<ISensor> _sensors;

        public IReadOnlyList<ISensor> Sensors => _sensors.AsReadOnly();

        public LibreHardwareMonitorFacade(bool isCpuEnabled = true,
                                          bool isGpuEnabled = true,
                                          bool isStorageEnabled = true,
                                          bool isMotherboardEnabled = true,
                                          bool isMemoryEnabled = true,
                                          bool isNetworkEnabled = true,
                                          bool isControllerEnabled = true)
        {
            Logger.Info("Initializing Libre Hardware Monitor...");

            if (!(isCpuEnabled || isGpuEnabled || isStorageEnabled || isMotherboardEnabled
                || isMemoryEnabled || isNetworkEnabled || isControllerEnabled))
                throw new ArgumentException("At least one sensor group needs to be enabled!");

            _sensors = new List<ISensor>();
            _computer = new Computer()
            {
                IsCpuEnabled = isCpuEnabled,
                IsGpuEnabled = isGpuEnabled,
                IsStorageEnabled = isStorageEnabled,
                IsMotherboardEnabled = isMotherboardEnabled,
                IsMemoryEnabled = isMemoryEnabled,
                IsNetworkEnabled = isNetworkEnabled,
                IsControllerEnabled = isControllerEnabled
            };

            _computer.Open();
            _computer.Accept(new CustomSensorVisitor(sensor =>
            {
                Logger.Trace("Valid sensor identifier: {0}", sensor.Identifier);

                _sensors.Add(sensor);
                sensor.ValuesTimeWindow = TimeSpan.Zero;
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
            Logger.Info("Finalizing Libre Hardware Monitor...");

            _computer?.Close();
            _sensors.Clear();
        }

        private class CustomSensorVisitor : IVisitor
        {
            private readonly Action<ISensor> _callback;

            public CustomSensorVisitor(Action<ISensor> callback)
            {
                _callback = callback;
            }

            public void VisitSensor(ISensor sensor) => _callback?.Invoke(sensor);
            public void VisitComputer(IComputer computer) => computer.Traverse(this);

            public void VisitHardware(IHardware hardware)
            {
                Logger.Trace("Updating hardware: {0}", hardware.Name);

                hardware.Update();
                hardware.Traverse(this);
            }

            public void VisitParameter(IParameter parameter) { }
        }
    }
}
