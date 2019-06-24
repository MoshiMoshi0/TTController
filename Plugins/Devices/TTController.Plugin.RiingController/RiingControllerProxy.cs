using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.RiingController
{
    public class RiingControllerProxy : DefaultControllerProxy
    {
        public RiingControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
            : base(device, definition)
        { }

        public override bool SetSpeed(byte port, byte speed) =>
            Device.WriteReadBytes(0x32, 0x51, port, 0x03, speed)?[3] == 0xfc;

        protected override Dictionary<string, byte> GenerateAvailableEffects()
        {
            return new Dictionary<string, byte>()
            {
                ["Flow"] = 0x01,
                ["Full"] = 0x00
            };
        }
    }
}
