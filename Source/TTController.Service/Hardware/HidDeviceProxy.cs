using System;
using System.Collections.Generic;
using System.Linq;
using HidLibrary;

namespace TTController.Service.Hardware
{
    public class HidDeviceProxy : IHidDeviceProxy
    {
        private readonly HidDevice _device;

        public int VendorId => _device.Attributes.VendorId;
        public int ProductId => _device.Attributes.ProductId;

        public HidDeviceProxy(HidDevice device)
        {
            _device = device;
        }

        public bool WriteBytes(params byte[] bytes)
        {
            if (!bytes.Any())
                return false;
            
            var data = new byte[_device.Capabilities.OutputReportByteLength];
            Array.Copy(bytes, 0, data, 1, Math.Min(bytes.Length, _device.Capabilities.OutputReportByteLength));
            return _device.Write(data);
        }

        public bool WriteBytes(IEnumerable<byte> bytes) =>
            WriteBytes(bytes.ToArray());

        public byte[] ReadBytes()
        {
            var data = _device.Read();
            if (data.Status != HidDeviceData.ReadStatus.Success)
                return null;
            
            return data.Data;
        }

        public byte[] WriteReadBytes(params byte[] bytes)
        {
            if (!WriteBytes(bytes))
                return null;
            return ReadBytes();
        }

        public byte[] WriteReadBytes(IEnumerable<byte> bytes) =>
            WriteReadBytes(bytes.ToArray());
    }
}
