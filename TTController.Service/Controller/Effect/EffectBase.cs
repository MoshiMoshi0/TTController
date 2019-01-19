using System;
using System.Collections.Generic;
using TTController.Common;
using TTController.Service.Manager;
using TTController.Service.Trigger;

namespace TTController.Service.Controller.Effect
{
    public enum EffectType
    {
        Flow = 0x00,
        Spectrum = 0x04,
        Ripple = 0x08,
        Blink = 0xc,
        Pulse = 0x10,
        Wave = 0x14,
        ByLed = 0x18,
        Full = 0x19
    }

    public enum EffectSpeed
    {
        Slow = 0x03,
        Normal = 0x02,
        Fast = 0x01,
        Extreme = 0x00
    }

    public interface IEffectBase : IDisposable
    {
        bool Enabled { get; }
        bool HandlesLedTransformation { get; }
        byte EffectByte { get; }
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

        protected EffectBase(T config)
        {
            Config = config;
        }

        public virtual void Dispose() { }

        public abstract byte EffectByte { get; } 
        public abstract IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);

    }
}
