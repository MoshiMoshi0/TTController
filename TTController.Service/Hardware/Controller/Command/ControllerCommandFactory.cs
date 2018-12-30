using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Hardware.Controller.Command
{
    public class ControllerCommandFactory : IControllerCommandFactory
    {
        public IEnumerable<byte> SetRgbBytes(byte port, byte mode, IEnumerable<LedColor> colors)
        {
            var bytes = new List<byte> { 0x32, 0x52, port, mode };

            foreach (var color in colors)
            {
                bytes.Add(color.G);
                bytes.Add(color.R);
                bytes.Add(color.B);
            }

            return bytes;
        }

        public IEnumerable<byte> SetSpeedBytes(byte port, byte speed) => 
            new List<byte> { 0x32, 0x51, port, 0x01, speed};

        public IEnumerable<byte> SetPwmBytes(byte port, int rpm) => 
            new List<byte> { 0x32, 0x51, port, 0x02, (byte)((rpm >> 8) & 0xff), (byte)(rpm & 0xff) };

        public IEnumerable<byte> GetPortDataBytes(byte port) => 
            new List<byte> { 0x33, 0x51, port };

        public IEnumerable<byte> InitBytes() =>
            new List<byte> { 0xfe, 0x33 };
    }
}
