using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OpenHardwareMonitor.Hardware;

namespace TTController.Common
{
    public interface IEffectBase : IDisposable
    {
        bool IsEnabled(ICacheProvider cache);
        string EffectType { get; }
        IEnumerable<Identifier> UsedSensors { get; }
        IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);
    }

    public abstract class EffectConfigBase
    {
        [DefaultValue(null)] public ITriggerBase Trigger { get; private set; } = null;
    }

    public abstract class EffectBase<T> : IEffectBase where T : EffectConfigBase
    {
        protected T Config { get; }
        public IEnumerable<Identifier> UsedSensors { get; private set; }

        protected EffectBase(T config) : this(config, Enumerable.Empty<Identifier>()) { }
        protected EffectBase(T config, IEnumerable<Identifier> usedSensors)
        {
            Config = config;
            UsedSensors = usedSensors
                .Union(config?.Trigger?.UsedSensors ?? Enumerable.Empty<Identifier>())
                .ToList();
        }

        public virtual bool IsEnabled(ICacheProvider cache) => Config.Trigger?.Value(cache) ?? false;
        public virtual void Dispose() { }

        public abstract string EffectType { get; }
        public abstract IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);

    }
}
