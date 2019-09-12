using System;
using System.Collections.Generic;

namespace TTController.Common.Plugin
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
