using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Common.Plugin;

namespace TTController.Plugin.RiingController
{
    public class RiingControllerDefinition : IControllerDefinition
    {
        public string Name => "Riing";
        public int VendorId => 0x264a;
        public IEnumerable<int> ProductIds => Enumerable.Range(0, 16).Select(x => 0x1f41 + x);
        public int PortCount => 5;
        public Type ControllerProxyType => typeof(RiingControllerProxy);
    }
}