using System;
using System.Collections.Generic;
using TTController.Common.Plugin;

namespace TTController.Plugin.DpsgController
{
    public class DpsgControllerDefinition : IControllerDefinition
    {
        public string Name => "Dpsg";
        public int VendorId => 0x264a;
        public IEnumerable<int> ProductIds { get { yield return 0x2329; } }
        public int PortCount => 0;
        public Type ControllerProxyType => typeof(DpsgControllerProxy);
    }
}
