using System.Collections.Generic;

namespace TTController.Service.Hardware.Device
{
    public interface IDeviceDefinition
    {
        string DeviceName { get; }
        int VendorId { get; }
        IEnumerable<int> ProductIds { get; }
    }
}
