using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;
using NLog;

namespace TTController.Common.Plugin
{
    public abstract class EffectBase<T> : IEffectBase where T : EffectConfigBase
    {
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

        public IDictionary<PortIdentifier, List<LedColor>> GetColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var result = GenerateColors(ports, cache);

            if (result != null)
            {
                foreach (var port in result.Keys)
                {
                    var colors = result[port];
                    PostProcess(ref colors, cache);
                }
            }

            return result;
        }

        public List<LedColor> GetColors(int count, ICacheProvider cache)
        {
            var colors = GenerateColors(count, cache);
            PostProcess(ref colors, cache);
            return colors;
        }

        protected virtual void PostProcess(ref List<LedColor> colors, ICacheProvider cache)
        {
            if (Config.Modifiers != null)
                foreach (var modifier in Config.Modifiers)
                    modifier.Apply(ref colors, cache);
        }

        protected abstract IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);
        protected abstract List<LedColor> GenerateColors(int count, ICacheProvider cache);
    }
}
