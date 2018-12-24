using System.Collections.Generic;

namespace TTController.Service.Hardware.Device
{
    public class RiingPlusControllerDefinition : IDeviceDefinition
    {
        public string DeviceName => "Riing Plus Controller";
        public int VendorId => 0x264a;

        public IEnumerable<int> ProductIds
        {
            get
            {
                for (var id = 0; id < 16; id++)
                    yield return 0x1fa5 + id;
            }
        }
    }
}