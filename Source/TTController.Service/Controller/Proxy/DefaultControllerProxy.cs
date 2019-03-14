using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TTController.Common;
using TTController.Service.Controller.Definition;
using TTController.Service.Hardware;

namespace TTController.Service.Controller.Proxy
{
    public class DefaultControllerProxy : AbstractControllerProxy
    {
        public DefaultControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
            : base(device, definition) { }
        
        public override IEnumerable<PortIdentifier> Ports => Enumerable.Range(1, Definition.PortCount)
            .Select(x => new PortIdentifier(Device.VendorId, Device.ProductId, (byte) x));

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
