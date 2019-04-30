using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Service.Controller.Proxy;

namespace TTController.Service.Controller.Definition
{
    public class RiingTrioControllerDefinition : IControllerDefinition
    {
        public string Name => "Riing Trio";
        public int VendorId => 0x264a;
        public IEnumerable<int> ProductIds => Enumerable.Range(0, 16).Select(x => 0x2135 + x);
        public int PortCount => 5;
        public Type ControllerProxyType => typeof(RiingTrioControllerProxy);
    }
}