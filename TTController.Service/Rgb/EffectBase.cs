using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TTController.Common;
using TTController.Service.Config;
using TTController.Service.Manager;

namespace TTController.Service.Rgb
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
        byte EffectByte { get; }
        IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);
    }

    public abstract class EffectBase<T> : IEffectBase where T : EffectConfigBase
    {
        public T Config { get; }
        public virtual bool Enabled => Config.Trigger?.Value() ?? false;

        protected EffectBase(T config)
        {
            Config = config;
        }

        public virtual void Dispose() { }

        public abstract byte EffectByte { get; } 
        public abstract IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache);

    }
}
