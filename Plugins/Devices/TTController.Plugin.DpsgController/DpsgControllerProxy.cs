using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.DpsgController
{
    public class DpsgControllerProxy : AbstractControllerProxy
    {
        private readonly IReadOnlyDictionary<string, byte> _availableEffects;

        public DpsgControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
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

            result.Add("ByLed", 0x18);
            result.Add("Full", 0x19);

            _availableEffects = result;
        }

        public override IEnumerable<PortIdentifier> Ports
        {
            get
            {
                yield return new PortIdentifier(Device.VendorId, Device.ProductId, 0);
            }
        }

        public override IEnumerable<string> EffectTypes => _availableEffects.Keys;

        public override bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors)
        {
            var bytes = new List<byte> { 0x30, 0x42, mode };
            foreach (var color in colors)
            {
                bytes.Add(color.G);
                bytes.Add(color.R);
                bytes.Add(color.B);
            }

            return Device.WriteReadBytes(bytes)?[3] == 0xfc;
        }

        public override bool SetSpeed(byte port, byte speed)
        {
            if (speed == 0)
                return Device.WriteReadBytes(0x30, 0x41, 0x03)?[3] == 0xfc;
            if (speed == 1) // silent
                return Device.WriteReadBytes(0x30, 0x41, 0x01)?[3] == 0xfc;
            if (speed == 2) // performance
                return Device.WriteReadBytes(0x30, 0x41, 0x02)?[3] == 0xfc;

            return Device.WriteReadBytes(0x30, 0x41, 0x04, speed)?[3] == 0xfc;
        }

        public override PortData GetPortData(byte port)
        {
            float GetData(byte b)
            {
                var bytes = Device.WriteReadBytes(0x31, b)?.Skip(3).Take(2).ToArray();
                if (bytes == null || bytes.Length == 0)
                    return float.NaN;

                var value = bytes[1] << 8 | bytes[0];
                var exponent = (value & 0x7800) >> 11;
                var sign = (value & 0x8000) >> 15;
                var fraction = (value & 0x7ff);

                if (sign == 1)
                    exponent -= 16;

                return (float)Math.Pow(2.0, exponent) * fraction;
            }

            var vin = GetData(0x33);
            var vvOut12 = GetData(0x34);
            var vvOut5 = GetData(0x35);
            var vvOut33 = GetData(0x36);
            var viOut12 = GetData(0x37);
            var viOut5 = GetData(0x38);
            var viOut33 = GetData(0x39);
            var temp = GetData(0x3a);
            var fanRpm = GetData(0x3b);

            var watts = vvOut12 * viOut12 + vvOut5 * viOut5 + vvOut33 * viOut33;
            //var efficiency = lut[(int)(watts / 10.0)];

            var data = new PortData()
            {
                Temperature = temp,
                Rpm = (int)fanRpm,
                ["VIN"] = vin,
                ["VVOut12"] = vvOut12,
                ["VVOut5"] = vvOut5,
                ["VVOut33"] = vvOut33,
                ["VIOut12"] = viOut12,
                ["VIOut5"] = viOut5,
                ["VIOut33"] = viOut33,
                ["Watts"] = watts,
            };

            return data;
        }

        public override byte? GetEffectByte(string effectType)
        {
            if (effectType == null)
                return null;
            return _availableEffects.TryGetValue(effectType, out var value) ? value : (byte?)null;
        }

        public override void SaveProfile()
        {
        }

        public override bool Init()
        {
            var result = Device.WriteReadBytes(0xfe, 0x31);
            if (result == null)
                return false;

            try
            {
                var model = Encoding.ASCII.GetString(result, 0, result.Length);
                return !string.IsNullOrEmpty(model);
            }
            catch { return false; }
        }

        public override bool IsValidPort(PortIdentifier port) =>
            port.ControllerProductId == Device.ProductId
            && port.ControllerVendorId == Device.VendorId
            && port.Id == 0;
    }
}
