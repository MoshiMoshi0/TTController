using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Service.Controller.Definition;
using TTController.Service.Hardware;
using TTController.Service.Utils;

namespace TTController.Service.Controller.Proxy
{
    public class DefaultControllerProxy : AbstractControllerProxy
    {
        private readonly IReadOnlyDictionary<string, byte> _availableEffects;
        public DefaultControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
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
            foreach (var (e, eb) in effectModes)
            foreach (var (s, sb) in effectSpeeds)
                result.Add($"{e}_{s}", (byte)(eb + sb));

            result.Add("ByLed", 0x18);
            result.Add("Full", 0x19);

            _availableEffects = result;
        }
        
        public override IEnumerable<PortIdentifier> Ports => Enumerable.Range(1, Definition.PortCount)
            .Select(x => new PortIdentifier(Device.VendorId, Device.ProductId, (byte) x));

        public override IEnumerable<string> EffectTypes => _availableEffects.Keys;

        public override bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors)
        {
            var bytes = new List<byte> { 0x32, 0x52, port, mode };
            foreach (var color in colors)
            {
                bytes.Add(color.G);
                bytes.Add(color.R);
                bytes.Add(color.B);
            }
            
            var result = Device.WriteReadBytes(bytes);
            return result[3] == 0xfc;
        }

        public override bool SetSpeed(byte port, byte speed)
        {
            var result = Device.WriteReadBytes(0x32, 0x51, port, 0x01, speed);
            return result[3] == 0xfc;
        }

        public override PortData GetPortData(byte port)
        {
            var result = Device.WriteReadBytes(0x33, 0x51, port);

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
            return _availableEffects.TryGetValue(effectType, out var value) ? value : (byte?) null;
        }

        public override void SaveProfile()
        {
            var result = Device.WriteReadBytes(0x32, 0x53);
        }

        public override bool Init()
        {
            var result = Device.WriteReadBytes(0xfe, 0x33);
            return result != null && result.Length >= 3 && result[3] == 0xfc;
        }

        public override bool IsValidPort(PortIdentifier port) =>
            port.ControllerProductId == Device.ProductId &&
           port.ControllerVendorId == Device.VendorId &&
           port.Id >= 1 && port.Id <= Definition.PortCount;
    }
}
