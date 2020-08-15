using System;
using System.Collections.Generic;
using System.Linq;
using HidSharp;
using NLog;
using TTController.Common;

namespace TTController.Service.Hardware
{
    public class HidDeviceProxy : IHidDeviceProxy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly HidDevice _device;
        private readonly HidStream _stream;
        private bool _opened;

        public int VendorId => _device.VendorID;
        public int ProductId => _device.ProductID;

        public HidDeviceProxy(HidDevice device)
        {
            _device = device;

            _stream = device.Open();
            _stream.ReadTimeout = 1000;
            _stream.WriteTimeout = 1000;
        }

        public bool WriteBytes(params byte[] bytes)
        {
            if (!_opened || bytes.Length == 0)
                return false;

            var data = new byte[_device.GetMaxOutputReportLength()];
            Array.Copy(bytes, 0, data, 1, Math.Min(bytes.Length, _device.GetMaxOutputReportLength() - 1));

            try
            {
                _stream.Write(data);
                Logger.Trace("W[{vid}, {pid}] {data:X2}", VendorId, ProductId, data);
                return true;
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Failed to write to [{0}, {1}]!", VendorId, ProductId);
                return false;
            }
        }

        public bool WriteBytes(IEnumerable<byte> bytes) =>
            WriteBytes(bytes.ToArray());

        public byte[] ReadBytes()
        {
            if (!_opened)
                return null;

            try
            {
                var data = _stream.Read();
                Logger.Trace("R[{vid}, {pid}] {data:X2}", VendorId, ProductId, data);
                return data;
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Failed to read from [{0}, {1}]!", VendorId, ProductId);
                return null;
            }
        }

        public byte[] WriteReadBytes(params byte[] bytes)
        {
            if (!WriteBytes(bytes))
                return null;
            return ReadBytes();
        }

        public byte[] WriteReadBytes(IEnumerable<byte> bytes) =>
            WriteReadBytes(bytes.ToArray());

        protected virtual void Dispose(bool disposing)
        {
            _stream?.Dispose();
            _opened = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
