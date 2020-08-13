using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.RiingController
{
    public class RiingControllerProxy : AbstractControllerProxy
    {
        private readonly IReadOnlyDictionary<string, byte> _availableEffects;

        public RiingControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
            : base(device, definition)
        {
            _availableEffects = new Dictionary<string, byte>()
            {
                ["Flow"] = 0x01,
                ["Full"] = 0x00
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

        public override IEnumerable<PortIdentifier> Ports => Enumerable.Range(1, Definition.PortCount)
            .Select(x => new PortIdentifier(Device.VendorId, Device.ProductId, (byte)x));

        public override IEnumerable<string> EffectTypes => _availableEffects.Keys;

        public override bool SetRgb(byte port, string effectType, IEnumerable<LedColor> colors)
        {
            if (!_availableEffects.TryGetValue(effectType, out var mode))
                return false;

            var bytes = new List<byte> { 0x32, 0x52, port, mode };
            foreach (var color in colors)
            {
                bytes.Add(color.R);
                bytes.Add(color.G);
                bytes.Add(color.B);
            }

            var result = Device.WriteReadBytes(bytes)?[3] ?? -1;
            return result == 0xfe || result == 0x00;
        }

        public override bool SetSpeed(byte port, byte speed) =>
            Device.WriteReadBytes(0x32, 0x51, port, 0x03, speed)?[3] == 0xfc;

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
