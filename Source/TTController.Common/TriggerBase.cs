using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TTController.Common
{
    public interface ITriggerBase : IDisposable
    {
        bool Value(ICacheProvider cache);
        IEnumerable<Identifier> UsedSensors { get; }
    }

    public abstract class TriggerConfigBase { }

    public abstract class TriggerBase<T> : ITriggerBase where T : TriggerConfigBase
    {
        public T Config { get; }
        public IEnumerable<Identifier> UsedSensors { get; private set; }

        protected TriggerBase(T config) : this(config, Enumerable.Empty<Identifier>()) { }
        protected TriggerBase(T config, IEnumerable<Identifier> usedSensors)
        {
            Config = config;
            UsedSensors = usedSensors;
        }

        public abstract bool Value(ICacheProvider cache);
        public virtual void Dispose() { }
    }
}
