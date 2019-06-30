using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.RiingTrioController
{
    public class RiingTrioControllerProxy : DefaultControllerProxy
    {
        public RiingTrioControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
            : base(device, definition) { }

        public override bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors)
        {
            var colorCount = colors.Count();
            if (colorCount <= 12)
                return base.SetRgb(port, mode, colors);

            bool WriteChunk(byte chunkId)
            {
                const byte maxPerChunk = 19;
                var bytes = new List<byte> { 0x32, 0x52, port, 0x24, 0x03, chunkId, 0x00 };
                foreach (var color in colors.Skip((chunkId - 1) * maxPerChunk).Take(maxPerChunk))
                {
                    bytes.Add(color.G);
                    bytes.Add(color.R);
                    bytes.Add(color.B);
                }

                return Device.WriteReadBytes(bytes)?[3] == 0xfc;
            }

            var result = true;
            for(byte i = 0x01; i <= 0x02; i++)
                result &= WriteChunk(i);

            return result;
        }
    }
}
