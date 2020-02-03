using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;

namespace TTController.Common.Plugin
{
    public interface ITriggerBase : IPlugin, IDisposable
    {
        bool Value(ICacheProvider cache);
        IEnumerable<Identifier> UsedSensors { get; }
    }
}
