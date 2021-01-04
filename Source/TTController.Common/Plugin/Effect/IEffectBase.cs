using System;
using System.Collections.Generic;

namespace TTController.Common.Plugin
{
    public interface IEffectBase : IPlugin, IDisposable
    {
        string EffectType { get; }

        bool IsEnabled(ICacheProvider cache);
        void Update(ICacheProvider cache);
        IDictionary<PortIdentifier, List<LedColor>> GetColors(List<PortIdentifier> ports, ICacheProvider cache);
        List<LedColor> GetColors(int count, ICacheProvider cache);
    }
}
