using System.Collections.Generic;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Service.Config
{
    public class ProfileConfig
    {
        public string Name { get; internal set; } = "Default";
        public List<PortIdentifier> Ports { get; internal set; } = new List<PortIdentifier>();

        public List<ISpeedControllerBase> SpeedControllers { get; internal set; } = new List<ISpeedControllerBase>();
        public List<IEffectBase> Effects { get; internal set; } = new List<IEffectBase>();
    }
}
