using System.Collections.Generic;
using System.ComponentModel;
using TTController.Common;

namespace TTController.Service.Config
{
    public enum ComputerStateType
    {
        Boot,
        Shutdown,
        Suspend
    }

    public class ComputerStateProfileConfig
    {
        [DefaultValue(ComputerStateType.Shutdown)] public ComputerStateType StateType { get; internal set; } = ComputerStateType.Shutdown;
        public List<PortIdentifier> Ports { get; internal set; } = new List<PortIdentifier>();
        [DefaultValue(null)] public byte? Speed { get; internal set; } = null;
        [DefaultValue(null)] public LedColorProvider Color { get; internal set; } = null;
        [DefaultValue("PerLed")] public string EffectType { get; internal set; } = "PerLed";
    }
}
