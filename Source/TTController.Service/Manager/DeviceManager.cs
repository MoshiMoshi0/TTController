using System;
using System.Collections.Generic;
using System.Linq;
using HidLibrary;
using NLog;
using TTController.Common;
using TTController.Common.Plugin;
using TTController.Service.Hardware;
using TTController.Service.Utils;

namespace TTController.Service.Manager
{
    public sealed class DeviceManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IReadOnlyList<HidDevice> _devices;
        private readonly IReadOnlyList<IControllerProxy> _controllers;

        public IReadOnlyList<IControllerProxy> Controllers => _controllers;

        public DeviceManager()
        {
            Logger.Info("Creating Device Manager...");
            _devices = new List<HidDevice>();
            _controllers = new List<IControllerProxy>();

            var definitions = typeof(IControllerDefinition).FindInAssemblies()
                .Select(t => (IControllerDefinition)Activator.CreateInstance(t))
                .ToList();

            var devices = new List<HidDevice>();
            var controllers = new List<IControllerProxy>();
            foreach (var definition in definitions)
            {
                Logger.Debug("Searching for \"{0}\" controllers", definition.Name);
                var detectedDevices = HidDevices.Enumerate(definition.VendorId, definition.ProductIds.ToArray());
                var detectedCount = detectedDevices.Count();

                if (detectedCount == 0)
                    continue;

                if(detectedCount == 1)
                    Logger.Trace("Found 1 controller [{vid}, {pid}]", definition.VendorId, detectedDevices.Select(d => d.Attributes.ProductId).First());
                else
                    Logger.Trace("Found {count} controllers [{vid}, [{pids}]]", detectedCount, definition.VendorId, detectedDevices.Select(d => d.Attributes.ProductId));

                foreach (var device in detectedDevices)
                {
                    var controller = (IControllerProxy) Activator.CreateInstance(definition.ControllerProxyType, new HidDeviceProxy(device), definition);
                    if (!controller.Init())
                    {
                        Logger.Warn("Failed to initialize \"{0}\" controller! [{1}, {2}]", definition.Name, device.Attributes.VendorHexId, device.Attributes.ProductHexId);
                        continue;
                    }

                    Logger.Info("Initialized \"{0}\" controller [{1}, {2}]", definition.Name, device.Attributes.VendorHexId, device.Attributes.ProductHexId);

                    devices.Add(device);
                    controllers.Add(controller);
                }
            }

            _devices = devices;
            _controllers = controllers;
        }

        public IControllerProxy GetController(PortIdentifier port) =>
            _controllers.FirstOrDefault(c => c.IsValidPort(port));

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing Device Manager...");

            var count = _devices.Count;
            foreach (var device in _devices)
                device.Dispose();

            Logger.Debug("Disposed devices: {0}", count);
        }
    }
}
