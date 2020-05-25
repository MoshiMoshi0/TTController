using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.RiingTrioController
{
    public class RiingTrioControllerProxy : AbstractControllerProxy
    {
        private readonly IReadOnlyDictionary<string, byte> _availableEffects;

        public RiingTrioControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
            : base(device, definition)
        {
            _availableEffects = new Dictionary<string, byte>
            {
                { "PerLed", 0x24 }
            };
        }

        public override IEnumerable<PortIdentifier> Ports => Enumerable.Range(1, Definition.PortCount)
            .Select(x => new PortIdentifier(Device.VendorId, Device.ProductId, (byte)x));

        public override IEnumerable<string> EffectTypes => _availableEffects.Keys;

        public override bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors)
        {
            bool WriteChunk(byte chunkId)
            {
                const byte maxPerChunk = 19;
                var bytes = new List<byte> { 0x32, 0x52, port, mode, 0x03, chunkId, 0x00 };
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

        public override bool SetSpeed(byte port, byte speed) =>
            Device.WriteReadBytes(0x32, 0x51, port, 0x01, speed)?[3] == 0xfc;

        public override PortData GetPortData(byte port)
        {
            var result = Device.WriteReadBytes(0x33, 0x51, port);
            if (result == null)
                return null;

            if (result[3] == 0xfe)
                return null;

            var data = new PortData
            {
                PortId = result[3],
                Speed = result[5],
                Rpm = (result[7] << 8) + result[6],
                ["Unknown"] = result[4]
            };

            return data;
        }

        public override byte? GetEffectByte(string effectType)
        {
            if (effectType == null)
                return null;
            return _availableEffects.TryGetValue(effectType, out var value) ? value : (byte?)null;
        }

        public override void SaveProfile() =>
            Device.WriteReadBytes(0x32, 0x53);

        public override bool Init() =>
            Device.WriteReadBytes(0xfe, 0x33)?[3] == 0xfc;

        public override bool IsValidPort(PortIdentifier port) =>
            port.ControllerProductId == Device.ProductId
            && port.ControllerVendorId == Device.VendorId
            && port.Id >= 1
            && port.Id <= Definition.PortCount;
    }
}
