using System;
using Newtonsoft.Json;

namespace TTController.Service.Trigger
{
    public interface ITriggerBase : IDisposable
    {
        bool Value();
    }

    public abstract class TriggerBase : ITriggerBase
    {
        public virtual void Dispose() {}

        public abstract bool Value();
    }

    public abstract class ConfigurableTriggerBase<T> : TriggerBase where T : TriggerConfigBase
    {
        protected T Config { get; }

        protected ConfigurableTriggerBase(dynamic rawConfig)
        {
            Config = JsonConvert.DeserializeObject(rawConfig.ToString(), typeof(T));
        }
    }
}
