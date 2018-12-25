using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool WriteBytes(IEnumerable<byte> bytes)
        {
            lock (_device)
            {
                return _device.Write(bytes.Prepend((byte)0).ToArray());
            }
        }

        public IEnumerable<byte> ReadBytes()
        {
            lock (_device)
            {
                var data = _device.Read();
                if (data.Status != HidDeviceData.ReadStatus.Success)
                    return Enumerable.Empty<byte>();

                return data.Data.Skip(3);
            }
        }

        public IEnumerable<byte> WriteReadBytes(IEnumerable<byte> bytes)
        {
            if (!WriteBytes(bytes))
                return Enumerable.Empty<byte>();
            return ReadBytes();
        }
    }
}
