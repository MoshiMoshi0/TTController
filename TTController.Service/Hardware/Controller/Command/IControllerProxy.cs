using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Hardware.Controller.Command
{
    public interface IControllerProxy
    {
        string Name { get; }
        int VendorId { get; }
        int ProductId { get; }
        IEnumerable<PortIdentifier> Ports { get; }

        bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors);
        bool SetSpeed(byte port, byte speed);
        bool SetPwm(byte port, int rpm);
        PortData GetPortData(byte port);
        void SaveProfile();
        bool Init();
        bool IsValidPort(PortIdentifier port);
    }
}
