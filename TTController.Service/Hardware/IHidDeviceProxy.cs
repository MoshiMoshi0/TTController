using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidLibrary;

namespace TTController.Service.Hardware
{
    public interface IHidDeviceProxy
    {
        int VendorId { get; }
        int ProductId { get; }

        bool WriteBytes(IEnumerable<byte> bytes);
        IEnumerable<byte> ReadBytes();
        IEnumerable<byte> WriteReadBytes(IEnumerable<byte> bytes);
    }
}
