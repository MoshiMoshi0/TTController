using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTController.Common.Plugin
{
    public abstract class TriggerBase<T> : ITriggerBase where T : TriggerConfigBase
    {
        public T Config { get; private set; }
        public IEnumerable<Identifier> UsedSensors { get; private set; }

        protected TriggerBase(T config) : this(config, Enumerable.Empty<Identifier>()) { }

        protected TriggerBase(T config, IEnumerable<Identifier> usedSensors)
        {
            Config = config;
            UsedSensors = usedSensors;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract bool Value(ICacheProvider cache);

        protected virtual void Dispose(bool disposing)
        {
            Config = null;
            UsedSensors = null;
        }
    }
}
