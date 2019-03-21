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
        public ComputerStateType StateType { get; private set; } = ComputerStateType.Shutdown;
        public List<PortIdentifier> Ports { get; private set; } = new List<PortIdentifier>();
        public byte? Speed { get; private set; }
        public string EffectType { get; private set; }
        public List<LedColor> EffectColors { get; private set; } = new List<LedColor>();
    }
}
