using System.Collections.Generic;
using System.Linq;
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
            return IsSuccess(result);
        }

        public override bool SetSpeed(byte port, byte speed)
        {
            var result = Device.WriteReadBytes(new List<byte> { 0x32, 0x51, port, 0x01, speed });
            return IsSuccess(result);
        }

        public override PortData GetPortData(byte port)
        {
            var result = Device.WriteReadBytes(new List<byte> { 0x33, 0x51, port }).ToList();

            var id = result[0];
            var unknown = result[1];
            var speed = result[2];
            var rpm = (result[4] << 8) + result[3];
            return new PortData(id, unknown, speed, rpm);
        }

        public override void SaveProfile()
        {
            var result = Device.WriteReadBytes(new List<byte> { 0x32, 0x53 });
        }

        public override bool Init()
        {
            var result = Device.WriteReadBytes(new List<byte> { 0xfe, 0x33 });
            return IsSuccess(result);
        }

        public override bool IsValidPort(PortIdentifier port) =>
            port.ControllerProductId == Device.ProductId &&
           port.ControllerVendorId == Device.VendorId &&
           port.Id >= 1 && port.Id <= Definition.PortCount;

        private bool IsSuccess(IEnumerable<byte> bytes) =>
            bytes.Any() && bytes.ElementAt(0) == 0xfc;
    }
}
