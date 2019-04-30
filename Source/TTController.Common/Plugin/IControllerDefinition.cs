using System;
using System.Collections.Generic;

namespace TTController.Common.Plugin
{
    public interface IControllerDefinition
    {
        string Name { get; }
        int VendorId { get; }
        IEnumerable<int> ProductIds { get; }
        int PortCount { get; }
        Type ControllerProxyType { get; }
    }
}
