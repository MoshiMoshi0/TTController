using System.Collections.Generic;
using TTController.Common;
using TTController.Service.Controller.Definition;
using TTController.Service.Hardware;

namespace TTController.Service.Controller.Proxy
{
    public interface IControllerProxy
    {
        string Name { get; }
        int VendorId { get; }
        int ProductId { get; }
        IEnumerable<PortIdentifier> Ports { get; }
        IEnumerable<string> EffectTypes { get; }

        bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors);
        bool SetSpeed(byte port, byte speed);
        PortData GetPortData(byte port);
        byte? GetEffectByte(string effectType);
        void SaveProfile();
        bool Init();
        bool IsValidPort(PortIdentifier port);
    }
}
