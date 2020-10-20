using System;
using System.Collections.Generic;
using LibreHardwareMonitor.Hardware;

namespace TTController.Common.Plugin
{
    public interface IEffectBase : IPlugin, IDisposable
    {
        bool IsEnabled(ICacheProvider cache);
        string EffectType { get; }

        void Update(ICacheProvider cache);
        IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);
        List<LedColor> GenerateColors(int count, ICacheProvider cache);
    }
}
