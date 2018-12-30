using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Hardware.Controller.Command
{
    public interface IControllerProxy
    {
        bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors);
        bool SetSpeed(byte port, byte speed);
        bool SetPwm(byte port, int rpm);
        PortData GetPortData(byte port);
        bool Init();
        bool IsValidPort(PortIdentifier port);
    }
}
