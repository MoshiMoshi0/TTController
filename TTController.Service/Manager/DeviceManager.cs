using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HidLibrary;
using TTController.Common;
using TTController.Service.Hardware;
using TTController.Service.Hardware.Controller;
using TTController.Service.Hardware.Controller.Command;

namespace TTController.Service.Manager
{
    public class DeviceManager : IDisposable
    {
        //private readonly List<IControllerDefinition> _definitions;
        private readonly IReadOnlyList<HidDevice> _devices;
        private readonly IReadOnlyList<IControllerProxy> _controllers;

        public DeviceManager()
        {
            _devices = new List<HidDevice>();
            _controllers = new List<IControllerProxy>();

            var definitions = Assembly.GetAssembly(typeof(IControllerDefinition))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IControllerDefinition).IsAssignableFrom(t))
                .Select(t => (IControllerDefinition)Activator.CreateInstance(t))
                .ToList();
            
            var devices = new List<HidDevice>();
            var controllers = new List<IControllerProxy>();
            foreach (var definition in definitions)
            {
                var detectedDevices = HidDevices.Enumerate(definition.VendorId, definition.ProductIds.ToArray());
                foreach (var device in detectedDevices)
                {
                    var controller = new ControllerProxy(new HidDeviceProxy(device), definition);
                    if(!controller.Init())
                        continue;

                    devices.Add(device);
                    controllers.Add(controller);
                }
            }

            _devices = devices;
            _controllers = controllers;
        }
        
        public IControllerProxy GetController(PortIdentifier port)
        {
            return _controllers.FirstOrDefault(c => c.IsValidPort(port));
        }

        public void Dispose()
        {
            foreach (var device in _devices)
                device.Dispose();
        }
    }
}
