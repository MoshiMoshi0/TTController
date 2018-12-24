using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HidLibrary;
using TTController.Service.Hardware.Device;

namespace TTController.Service.Managers
{
    public class DeviceManager
    {
        private readonly IReadOnlyList<IDeviceDefinition> _definitions;
        private readonly IReadOnlyList<HidDevice> _devices;

        public DeviceManager()
        {
            _definitions = Assembly.GetAssembly(typeof(IDeviceDefinition))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IDeviceDefinition).IsAssignableFrom(t))
                .Select(t => (IDeviceDefinition)Activator.CreateInstance(t))
                .ToList();
            
            _devices = _definitions.SelectMany(d => HidDevices.Enumerate(d.VendorId, d.ProductIds.ToArray())).ToList();
        }

        public bool WriteBytes(HidDevice device, IEnumerable<byte> bytes)
        {
            lock (device)
            {
                return device.Write(bytes.ToArray());
            }
        }

        public IEnumerable<byte> ReadBytes(HidDevice device)
        {
            lock (device)
            {
                var data = device.Read();
                if (data.Status != HidDeviceData.ReadStatus.Success)
                    return Enumerable.Empty<byte>();

                return data.Data;
            }
        }

        public IEnumerable<byte> WriteReadBytes(HidDevice device, IEnumerable<byte> bytes)
        {
            if (!WriteBytes(device, bytes))
                return Enumerable.Empty<byte>();
            return ReadBytes(device);
        }
    }
}
