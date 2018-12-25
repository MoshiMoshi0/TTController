using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Hardware.Controller.Command
{
    public interface IControllerCommandFactory
    {
        IEnumerable<byte> SetRgbBytes(byte port, byte mode, IEnumerable<LedColor> colors);
        IEnumerable<byte> SetSpeedBytes(byte port, byte speed);
        IEnumerable<byte> SetPwmBytes(byte port, int rpm);

        IEnumerable<byte> GetPortDataBytes(byte port);
        IEnumerable<byte> InitBytes();
    }
}
