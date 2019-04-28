using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Service.Controller.Proxy;

namespace TTController.Service.Controller.Definition
{
    public class RiingPlusControllerDefinition : IControllerDefinition
    {
        public string Name => "Riing Plus";
        public int VendorId => 0x264a;
        public IEnumerable<int> ProductIds => Enumerable.Range(0, 16).Select(x => 0x1fa5 + x);
        public int PortCount => 5;
        public Type ControllerProxyType => typeof(DefaultControllerProxy);
    }
}