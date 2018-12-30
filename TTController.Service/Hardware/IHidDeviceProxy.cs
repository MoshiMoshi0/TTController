using System.Collections.Generic;

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
