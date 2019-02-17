using System;
using System.Collections.Generic;
using System.Linq;
using OpenHardwareMonitor.Hardware;

namespace TTController.Common
{
    public interface IEffectBase : IDisposable
    {
        bool Enabled { get; }
        bool HandlesLedTransformation { get; }
        byte EffectByte { get; }
        IEnumerable<Identifier> UsedSensors { get; }
        IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);
    }

    public abstract class EffectConfigBase
    {
        public ITriggerBase Trigger { get; set; }
    }

    public abstract class EffectBase<T> : IEffectBase where T : EffectConfigBase
    {
        public T Config { get; }
        public virtual bool Enabled => Config.Trigger?.Value() ?? false;
        public virtual bool HandlesLedTransformation => false;
        public virtual IEnumerable<Identifier> UsedSensors => Enumerable.Empty<Identifier>();

        protected EffectBase(T config)
        {
            Config = config;
        }

        public virtual void Dispose() { }

        public abstract byte EffectByte { get; } 
        public abstract IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);

    }
}
