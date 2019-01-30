using System;
using System.Collections.Generic;

namespace TTController.Service.Controller.Definition
{
    public interface IControllerDefinition
    {
        string Name { get; }
        int VendorId { get; }
        IEnumerable<int> ProductIds { get; }
        Type CommandFactoryType { get; }
        int PortCount { get; }
    }
}
