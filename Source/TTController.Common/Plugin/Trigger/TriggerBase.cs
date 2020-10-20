using LibreHardwareMonitor.Hardware;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTController.Common.Plugin
{
    public abstract class TriggerBase<T> : ITriggerBase where T : TriggerConfigBase
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public T Config { get; private set; }

        protected TriggerBase(T config)
        {
            Config = config;
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
        }
    }
}
