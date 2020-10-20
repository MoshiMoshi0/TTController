using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HidSharp;
using NLog;
using TTController.Common;
using TTController.Common.Plugin;
using TTController.Service.Hardware;
using TTController.Service.Utils;

namespace TTController.Service.Managers
{
    public sealed class DeviceManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly object _searchLock = new object();
        private readonly IReadOnlyList<IControllerDefinition> _definitions;
        private List<IControllerProxy> _controllers;

        public IEnumerable<IControllerProxy> Controllers => _controllers;

        public DeviceManager()
        {
            Logger.Info("Creating Device Manager...");

            _controllers = new List<IControllerProxy>();
            _definitions = typeof(IControllerDefinition).FindImplementations()
                .Select(t => (IControllerDefinition)Activator.CreateInstance(t))
                .ToList();

            DeviceList.Local.Changed += DeviceListChanged;

            lock (_searchLock)
                SearchForControllers();
        }

        public IControllerProxy GetController(PortIdentifier port) =>
            Controllers?.FirstOrDefault(c => c.IsValidPort(port)) ?? null;

        private void SearchForControllers()
        {
            var controllersCopy = new List<IControllerProxy>(_controllers);

            Logger.Info("Searching for controller changes");
            foreach (var definition in _definitions)
            {
                Logger.Debug("Searching for \"{0}\" controllers", definition.Name);
                var detectedDevices = DeviceList.Local.GetHidDevices().Where(d => d.VendorID == definition.VendorId && definition.ProductIds.Contains(d.ProductID));
                var detectedCount = detectedDevices.Count();

                _ = controllersCopy.RemoveAll(controller => {
                    if (detectedDevices.Any(d => d.VendorID == controller.VendorId && d.ProductID == controller.ProductId))
                        return false;

                    if (controller.VendorId != definition.VendorId || !definition.ProductIds.Contains(controller.ProductId))
                        return false;

                    Logger.Info("Removing missing \"{0}\" controller [{1}, {2}]", controller.Name, controller.ProductId, controller.VendorId);
                    controller.Dispose();
                    return true;
                });

                if (detectedCount == 0)
                    continue;

                if (detectedCount == 1)
                    Logger.Trace("Found 1 new controller [{vid}, {pid}]", definition.VendorId, detectedDevices.Select(d => d.ProductID).First());
                else
                    Logger.Trace("Found {count} new controllers [{vid}, [{pids}]]", detectedCount, definition.VendorId, detectedDevices.Select(d => d.ProductID));

                foreach (var device in detectedDevices)
                {
                    if (controllersCopy.Any(c => c.ProductId == device.ProductID && c.VendorId == device.VendorID))
                        continue;

                    var deviceProxy = new HidDeviceProxy(device);
                    var controller = (IControllerProxy)Activator.CreateInstance(definition.ControllerProxyType, deviceProxy, definition);
                    if (!controller.Init())
                    {
                        Logger.Warn("Failed to initialize \"{0}\" controller! [{1}, {2}]", definition.Name, device.VendorID, device.ProductID);

                        deviceProxy.Dispose();
                        continue;
                    }

                    Logger.Info("Initialized \"{0}\" controller [{1}, {2}], version: \"{3}\"", definition.Name, device.VendorID, device.ProductID, controller.Version);
                    controllersCopy.Add(controller);
                }
            }

            if(!controllersCopy.ContentsEqual(_controllers))
                Interlocked.Exchange(ref _controllers, controllersCopy);
        }

        private void DeviceListChanged(object sender, DeviceListChangedEventArgs e)
        {
            lock (_searchLock)
            {
                Logger.Info("Device list changed!");
                SearchForControllers();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing Device Manager...");

            DeviceList.Local.Changed -= DeviceListChanged;

            var count = _controllers.Count;
            foreach (var controller in _controllers)
                controller.Dispose();

            Logger.Debug("Disposed controllers: {0}", count);
        }
    }
}
