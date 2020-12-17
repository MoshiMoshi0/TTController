using System.Collections.Generic;
using System.ComponentModel;
using TTController.Common;
using TTController.Common.Plugin;

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
        [DefaultValue(null)] public IEffectBase Effect { get; internal set; } = null;
    }
}
