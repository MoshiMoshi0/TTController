using System;
using System.Collections.Generic;

namespace TTController.Common.Plugin
{
    public interface IControllerProxy : IDisposable
    {
        string Name { get; }
        int VendorId { get; }
        int ProductId { get; }
        Version Version { get; }
        IEnumerable<PortIdentifier> Ports { get; }
        IEnumerable<string> EffectTypes { get; }

        bool SetRgb(byte port, string effectType, IEnumerable<LedColor> colors);
        bool SetSpeed(byte port, byte speed);
        PortData GetPortData(byte port);
        void SaveProfile();
        bool Init();
        bool IsValidPort(PortIdentifier port);
    }
}
