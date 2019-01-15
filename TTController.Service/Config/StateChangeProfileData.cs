using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;
using TTController.Service.Rgb;

namespace TTController.Service.Config
{
    public enum StateChangeType
    {
        Boot,
        Shutdown,
        Suspend
    }

    public class StateChangeProfileData
    {
        public StateChangeType StateChangeType { get; private set; }
        public List<PortIdentifier> Ports { get; private set; } = new List<PortIdentifier>();
        public byte Speed { get; private set; } = 50;
        public EffectType EffectType { get; private set; } = EffectType.Spectrum;
        public EffectSpeed EffectSpeed { get; private set; } = EffectSpeed.Normal;
        public List<LedColor> EffectColors { get; private set; } = new List<LedColor>();
    }
}
