using System;
using System.ComponentModel;

namespace TTController.Common
{
    public interface ITriggerBase : IDisposable
    {
        bool Value();
    }

    public abstract class TriggerConfigBase
    {
    }

    public abstract class TriggerBase<T> : ITriggerBase where T : TriggerConfigBase
    {
        public T Config { get; }

        protected TriggerBase(T config)
        {
            Config = config;
        }

        public virtual void Dispose() {}

        public abstract bool Value();
    }
}
