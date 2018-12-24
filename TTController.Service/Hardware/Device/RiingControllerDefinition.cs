using System.Collections.Generic;

namespace TTController.Service.Hardware.Device
{
    public class RiingControllerDefinition : IDeviceDefinition
    {
        public string DeviceName => "Riing Controller";
        public int VendorId => 0x264a;

        public IEnumerable<int> ProductIds
        {
            get
            {
                for (var id = 0; id < 16; id++)
                    yield return 0x1f41 + id;
            }
        }
    }
}