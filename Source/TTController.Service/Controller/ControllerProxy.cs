using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Service.Controller.Command;
using TTController.Service.Controller.Definition;
using TTController.Service.Hardware;

namespace TTController.Service.Controller
{
    public class ControllerProxy : IControllerProxy
    {
        private readonly IHidDeviceProxy _device;
        private readonly IControllerDefinition _definition;
        private readonly IControllerCommandFactory _commandFactory;
        
        public ControllerProxy(IHidDeviceProxy device, IControllerDefinition definition)
        {
            _device = device;
            _definition = definition;
            _commandFactory = (IControllerCommandFactory) Activator.CreateInstance(_definition.CommandFactoryType);
        }

        #region IControllerProxy

        public string Name => _definition.Name;
        public int VendorId => _device.VendorId;
        public int ProductId => _device.ProductId;
        public IEnumerable<PortIdentifier> Ports => Enumerable.Range(1, _definition.PortCount)
            .Select(x => new PortIdentifier(_device.VendorId, _device.ProductId, (byte) x));

        public bool SetRgb(byte port, byte mode, IEnumerable<LedColor> colors)
        {
            var result = _device.WriteReadBytes(_commandFactory.SetRgbBytes(port, mode, colors));
            return IsSuccess(result);
        }

        public bool SetSpeed(byte port, byte speed)
        {
            var result = _device.WriteReadBytes(_commandFactory.SetSpeedBytes(port, speed));
            return IsSuccess(result);
        }

        public PortData GetPortData(byte port)
        {
            var result = _device.WriteReadBytes(_commandFactory.GetPortDataBytes(port)).ToList();
            return new PortData(result[0], result[1], result[2], (result[4] << 8) + result[3]);
        }

        public void SaveProfile()
        {
            var result = _device.WriteReadBytes(_commandFactory.SaveProfileBytes());
        }

        public bool Init()
        {
            var result = _device.WriteReadBytes(_commandFactory.InitBytes());
            return IsSuccess(result);
        }

        public bool IsValidPort(PortIdentifier port) =>
            port.ControllerProductId == _device.ProductId &&
           port.ControllerVendorId == _device.VendorId &&
           port.Id >= 1 && port.Id <= _definition.PortCount;
        #endregion

        private bool IsSuccess(IEnumerable<byte> bytes) =>
            bytes.Any() && bytes.ElementAt(0) == 0xfc;
    }
}
