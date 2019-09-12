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

        private readonly List<IHidDeviceProxy> _deviceProxies;
        private readonly List<IControllerProxy> _controllers;

        public IReadOnlyList<IControllerProxy> Controllers => _controllers;

        public DeviceManager()
        {
            Logger.Info("Creating Device Manager...");

            var definitions = typeof(IControllerDefinition).FindImplementations()
                .Select(t => (IControllerDefinition)Activator.CreateInstance(t))
                .ToList();

            var devices = new List<IHidDeviceProxy>();
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
                    var deviceProxy = new HidDeviceProxy(device);
                    var controller = (IControllerProxy) Activator.CreateInstance(definition.ControllerProxyType, deviceProxy, definition);
                    if (!controller.Init())
                    {
                        Logger.Warn("Failed to initialize \"{0}\" controller! [{1}, {2}]", definition.Name, device.Attributes.VendorHexId, device.Attributes.ProductHexId);

                        deviceProxy.Dispose();
                        continue;
                    }

                    Logger.Info("Initialized \"{0}\" controller [{1}, {2}]", definition.Name, device.Attributes.VendorHexId, device.Attributes.ProductHexId);

                    devices.Add(deviceProxy);
                    controllers.Add(controller);
                }
            }

            _deviceProxies = devices;
            _controllers = controllers;
        }

        public IControllerProxy GetController(PortIdentifier port) =>
            Controllers.FirstOrDefault(c => c.IsValidPort(port));

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing Device Manager...");

            var count = _deviceProxies.Count;
            foreach (var deviceProxy in _deviceProxies)
                deviceProxy.Dispose();

            Logger.Debug("Disposed devices: {0}", count);
        }
    }
}
