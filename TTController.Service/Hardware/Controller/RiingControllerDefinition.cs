using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Service.Hardware.Controller.Command;

namespace TTController.Service.Hardware.Controller
{
    public class RiingControllerDefinition : IControllerDefinition
    {
        public string Name => "Riing Controller";
        public int VendorId => 0x264a;
        public IEnumerable<int> ProductIds => Enumerable.Range(0, 16).Select(x => 0x1f41 + x);
        public Type CommandFactoryType => typeof(ControllerCommandFactory);
        public int PortCount => 5;
    }
}