using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Config.Data
{
    public enum ComputerStateType
    {
        Boot,
        Shutdown,
        Suspend
    }

    public class ComputerStateProfileData
    {
        public ComputerStateType StateType { get; private set; }
        public List<PortIdentifier> Ports { get; private set; } = new List<PortIdentifier>();
        public byte Speed { get; private set; } = 50;
        public EffectType EffectType { get; private set; } = EffectType.Spectrum;
        public EffectSpeed EffectSpeed { get; private set; } = EffectSpeed.Normal;
        public List<LedColor> EffectColors { get; private set; } = new List<LedColor>();
    }
}
