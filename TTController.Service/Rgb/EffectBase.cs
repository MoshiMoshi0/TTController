using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;
using TTController.Common.Config;

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

    public interface IEffectBase : IDisposable
    {
        bool Enabled { get; }
        byte EffectByte { get; }
        IDictionary<PortIdentifier, List<LedColor>> GenerateColors(IDictionary<PortIdentifier, PortConfigData> portConfigMap);
    }

    public abstract class EffectBase<T> : IEffectBase where T : EffectConfigBase
    {
        protected T Config { get; }
        public virtual bool Enabled => Config.Trigger.Value();

        protected EffectBase(dynamic rawConfig)
        {
            Config = JsonConvert.DeserializeObject(rawConfig.ToString(), typeof(T));
        }

        public virtual void Dispose() { }

        public abstract byte EffectByte { get; } 
        public abstract IDictionary<PortIdentifier, List<LedColor>> GenerateColors(IDictionary<PortIdentifier, PortConfigData> portConfigMap);

    }
}
