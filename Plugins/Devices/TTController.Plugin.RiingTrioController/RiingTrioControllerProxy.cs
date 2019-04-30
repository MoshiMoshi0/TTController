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
            if (colorCount <= 20)
                return base.SetRgb(port, mode, colors);

            var result = true;
            const byte maxPerChunk = 19;
            for (byte chunkId = 1, chunkOffset = 0;
                chunkOffset < colorCount;
                chunkId++, chunkOffset += maxPerChunk)
            {
                var bytes = new List<byte> { 0x32, 0x52, port, 0x24, 0x03, chunkId, 0x00 };
                foreach (var color in colors.Skip(chunkOffset).Take(maxPerChunk))
                {
                    bytes.Add(color.G);
                    bytes.Add(color.R);
                    bytes.Add(color.B);
                }

                result &= Device.WriteReadBytes(bytes)?[3] == 0xfc;
            }

            return result;
        }
    }
}
