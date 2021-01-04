using System;
using System.Collections.Generic;
using TTController.Common.Plugin;

namespace TTController.Plugin.Level20DeskController
{
    public class Level20DeskControllerDefinition : IControllerDefinition
    {
        public string Name => "Level 20 Desk";
        public int VendorId => 0x264a;
        public IEnumerable<int> ProductIds => new[] { 0x07d1 };
        public int PortCount => 0;
        public Type ControllerProxyType => typeof(Level20DeskControllerProxy);
    }
}
