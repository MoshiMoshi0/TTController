using System;
using System.Collections.Generic;
using System.Linq;
using HidLibrary;
using NLog;
using TTController.Common;

namespace TTController.Service.Hardware
{
    public class HidDeviceProxy : IHidDeviceProxy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly HidDevice _device;

        public int VendorId => _device.Attributes.VendorId;
        public int ProductId => _device.Attributes.ProductId;

        public HidDeviceProxy(HidDevice device)
        {
            _device = device;
        }

        public bool WriteBytes(params byte[] bytes)
        {
            if (bytes.Length == 0)
                return false;

            var data = new byte[_device.Capabilities.OutputReportByteLength];
            Array.Copy(bytes, 0, data, 1, Math.Min(bytes.Length, _device.Capabilities.OutputReportByteLength - 1));

            Logger.Trace("W[{vid}, {pid}] {data}", _device.Attributes.VendorId, _device.Attributes.ProductId, data);

            return _device.Write(data, 1000);
        }

        public bool WriteBytes(IEnumerable<byte> bytes) =>
            WriteBytes(bytes.ToArray());

        public byte[] ReadBytes()
        {
            var data = _device.Read(1000);
            if (data.Status != HidDeviceData.ReadStatus.Success) {
                Logger.Warn("Read from [{0}, {1}] failed with status \"{2}\"!", _device.Attributes.VendorId, _device.Attributes.ProductId, data.Status);
                return null;
            }

            Logger.Trace("R[{vid}, {pid}] {data}", _device.Attributes.VendorId, _device.Attributes.ProductId, data.Data);

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
