using System;
using System.Collections.Generic;

namespace TTController.Common.Plugin
{
    public interface ISpeedControllerBase : IPlugin, IDisposable
    {
        bool IsEnabled(ICacheProvider cache);
        IDictionary<PortIdentifier, byte> GetSpeeds(List<PortIdentifier> ports, ICacheProvider cache);
    }
}
