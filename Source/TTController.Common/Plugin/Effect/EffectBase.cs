using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;
using NLog;

namespace TTController.Common.Plugin
{
    public abstract class EffectBase<T> : IEffectBase where T : EffectConfigBase
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected T Config { get; private set; }

        protected EffectBase(T config)
        {
            Config = config;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual bool IsEnabled(ICacheProvider cache) => Config.Trigger?.Value(cache) ?? false;
        public virtual void Update(ICacheProvider cache) { }

        protected virtual void Dispose(bool disposing)
        {
            Config = null;
        }

        public abstract string EffectType { get; }
        public abstract IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);
        public abstract List<LedColor> GenerateColors(int count, ICacheProvider cache);
    }
}
