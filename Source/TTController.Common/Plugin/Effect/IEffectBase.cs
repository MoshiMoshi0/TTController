using System;
using System.Collections.Generic;
using LibreHardwareMonitor.Hardware;

namespace TTController.Common.Plugin
{
    public interface IEffectBase : IPlugin, IDisposable
    {
        bool IsEnabled(ICacheProvider cache);
        string EffectType { get; }
        IEnumerable<Identifier> UsedSensors { get; }
        IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);
    }
}
