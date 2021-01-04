using System;

namespace TTController.Common.Plugin
{
    public abstract class TriggerBase<T> : ITriggerBase where T : TriggerConfigBase
    {
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
