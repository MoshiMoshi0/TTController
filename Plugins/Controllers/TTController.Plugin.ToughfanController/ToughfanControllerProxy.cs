using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.ToughfanController
{
    public class ToughfanControllerProxy : AbstractControllerProxy
    {
        private readonly IReadOnlyDictionary<string, byte> _availableEffects;

        public ToughfanControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
            : base(device, definition)
        {
            var effectModes = new Dictionary<string, byte>()
            {
                ["Flow"] = 0x00,
                ["Spectrum"] = 0x04,
                ["Ripple"] = 0x08,
                ["Blink"] = 0x0c,
                ["Pulse"] = 0x10,
                ["Wave"] = 0x14,
            };

            var effectSpeeds = new Dictionary<string, byte>()
            {
                ["Extreme"] = 0x00,
                ["Fast"] = 0x01,
                ["Normal"] = 0x02,
                ["Slow"] = 0x03
            };

            var result = new Dictionary<string, byte>();
            foreach (var mkv in effectModes)
            foreach (var skv in effectSpeeds)
                result.Add($"{mkv.Key}_{skv.Key}", (byte)(mkv.Value + skv.Value));

            result.Add("PerLed", 0x18);
            result.Add("Full", 0x19);

            _availableEffects = result;
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

            var bytes = new List<byte> { 0x32, 0x52, port, 0x24 };
            var color = colors.First();
            for (int i = 0; i < 24; i++)
            {
                bytes.Add(color.G);
                bytes.Add(color.R);
                bytes.Add(color.B);
            }

            // var bytes = new byte[]
            // {
            //     0x32, 0x52, port, 0x24,
            // 0x00, 0x00, 0xff, // 1
            // 0x00, 0x00, 0xff, // 2
            // 0x00, 0x00, 0xff, // 3
            // 0x00, 0x00, 0xff, // 4
            // 0x00, 0x00, 0xff, // 5
            // 0x00, 0x00, 0xff, // 6
            // 0x00, 0x00, 0xff, // 7
            // 0x00, 0x00, 0xff, // 8
            // 0x00, 0x00, 0xff, // 9
            // 0x00, 0x00, 0xff, // 10
            // 0x00, 0x00, 0xff, // 11
            // 0x00, 0x00, 0xff, // 12
            // 0x00, 0x00, 0xff, // 13
            // 0x00, 0x00, 0xff, // 14
            // 0x00, 0x00, 0xff, // 15
            // 0x00, 0x00, 0xff, // 16
            // 0x00, 0x00, 0xff, // 17
            // 0x00, 0x00, 0xff, // 18
            // 0x00, 0x00, 0xff, // 19
            // 0x00, 0x00, 0xff, // 20
            // 0x00, 0x00, 0xff, // 21
            // 0x00, 0x00, 0xff, // 22
            // 0x00, 0x00, 0xff, // 23
            // 0x00, 0x00, 0xff, // 24
            var endBytes = new byte[]
            {
                0x7f, 0x00, 0xff, 0xff, 0x00, 0xff, 0xff, 0x00, 0x7f, 0xff, 0x00, 0x00, 0xff, 0x7f, 0x00, 0xff, 0xff,
                0x00, 0x7f, 0xff, 0x00, 0x00, 0xff, 0x00, 0x00, 0xff, 0x7f, 0x00, 0xff, 0xff, 0x00, 0x7f, 0xff, 0x00,
                0x00, 0xff, 0x7f, 0x00, 0xff, 0xff, 0x00, 0xff, 0xff, 0x00, 0x7f, 0xff, 0x00, 0x00, 0xff, 0x7f, 0x00,
                0xff, 0xff, 0x00, 0x7f, 0xff, 0x00, 0x00, 0xff, 0x00, 0x00, 0xff, 0x7f, 0x00, 0xff, 0xff, 0x00, 0x7f,
                0xff, 0x00, 0x00, 0xff, 0x7f, 0x00, 0xff, 0xff, 0x00, 0xff, 0xff, 0x00, 0x7f, 0xff, 0x00, 0x00, 0xff,
                0x7f, 0x00, 0xff, 0xff, 0x00, 0x7f, 0xff, 0x00, 0x00, 0xff, 0x00, 0x00, 0xff, 0x7f, 0x00, 0xff, 0xff,
                0x00, 0x7f, 0xff, 0x00, 0x00, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            bytes.AddRange(endBytes);
            var result = Device.WriteReadBytes(bytes);
            return result?[3] == 0xfc;
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