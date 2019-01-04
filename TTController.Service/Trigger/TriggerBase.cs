using System;

namespace TTController.Service.Trigger
{
    public interface ITriggerBase : IDisposable
    {
        bool Value();
    }

    public abstract class TriggerConfigBase
    {
        protected TriggerConfigBase() { }
    }

    public abstract class TriggerBase<T> : ITriggerBase where T : TriggerConfigBase
    {
        public T Config { set; get; }

        protected TriggerBase(T config)
        {
            Config = config;
        }

        public virtual void Dispose() {}

        public abstract bool Value();
    }
}
