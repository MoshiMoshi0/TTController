using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Common.Plugin;

namespace TTController.Plugin.ToughfanController
{
    public class ToughfanControllerDefinition : IControllerDefinition
    {
        public string Name => "Toughfan";

        public int VendorId => 0x264a;

        public IEnumerable<int> ProductIds => Enumerable.Range(0, 16).Select(x => 0x232b + x);
        public int PortCount => 5;
        public Type ControllerProxyType => typeof(ToughfanControllerProxy);
    }
}