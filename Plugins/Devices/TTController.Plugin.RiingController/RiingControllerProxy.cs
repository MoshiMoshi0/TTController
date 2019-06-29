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

        public override bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors)
        {
            var bytes = new List<byte> { 0x32, 0x52, port, mode };
            foreach (var color in colors)
            {
                bytes.Add(color.R);
                bytes.Add(color.G);
                bytes.Add(color.B);
            }

            return Device.WriteReadBytes(bytes)?[3] == 0xfc;
        }

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
