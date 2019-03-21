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

    public abstract class AbstractControllerProxy : IControllerProxy
    {
        protected readonly IHidDeviceProxy Device;
        protected readonly IControllerDefinition Definition;

        protected AbstractControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
        {
            Device = device;
            Definition = definition;
        }

        public string Name => Definition.Name;
        public int VendorId => Device.VendorId;
        public int ProductId => Device.ProductId;

        public abstract IEnumerable<PortIdentifier> Ports { get; }
        public abstract IEnumerable<string> EffectTypes { get; }
        public abstract bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors);
        public abstract bool SetSpeed(byte port, byte speed);
        public abstract PortData GetPortData(byte port);
        public abstract byte? GetEffectByte(string effectType);
        public abstract void SaveProfile();
        public abstract bool Init();
        public abstract bool IsValidPort(PortIdentifier port);
    }
}
