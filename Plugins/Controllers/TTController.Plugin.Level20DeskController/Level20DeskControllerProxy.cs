using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.Level20DeskController
{
    public class Level20DeskControllerProxy : AbstractControllerProxy
    {
        private readonly IReadOnlyDictionary<string, byte> _availableEffects;

        public Level20DeskControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
            : base(device, definition)
        {
            _availableEffects = new Dictionary<string, byte>
            {
                { "PerLed", 0x24 }
            };
        }

        public override Version Version
        {
            get
            {
                var bytes = Device.WriteReadBytes(0x33, 0x50);
                if (bytes == null)
                    return new Version();

                return new Version(bytes[3], bytes[4], bytes[5]);
            }
        }

        public override IEnumerable<PortIdentifier> Ports
            => new[] { new PortIdentifier(Device.VendorId, Device.ProductId, 0) };

        public override IEnumerable<string> EffectTypes => _availableEffects.Keys;

        public override bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors)
        {
            var bytes = new List<byte> { 0x32, 0x52, 0x04, mode };
            foreach (var color in colors)
            {
                bytes.Add(color.G);
                bytes.Add(color.R);
                bytes.Add(color.B);
            }

            return Device.WriteReadBytes(bytes)?[3] == 0xfc;
        }

        public override byte? GetEffectByte(string effectType)
        {
            if (effectType == null)
                return null;
            return _availableEffects.TryGetValue(effectType, out var value) ? value : (byte?)null;
        }

        public override bool SetSpeed(byte port, byte speed) => false;
        public override PortData GetPortData(byte port) => null;
        public override void SaveProfile() { }
        public override bool Init() => Device.WriteReadBytes(0xfe, 0x33)?[3] == 0xfc;

        public override bool IsValidPort(PortIdentifier port) =>
            port.ControllerProductId == Device.ProductId
            && port.ControllerVendorId == Device.VendorId
            && port.Id == 0;
    }
}
