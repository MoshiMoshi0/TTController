using System;
using System.Collections.Generic;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Service.Config.Data
{
    public class ProfileData
    {
        public string Name { get; private set; } = "Default";
        public Guid Guid { get; private set; } = Guid.NewGuid();
        public List<PortIdentifier> Ports { get; private set; } = new List<PortIdentifier>();

        public List<ISpeedControllerBase> SpeedControllers { get; private set; } = new List<ISpeedControllerBase>();
        public List<IEffectBase> Effects { get; private set; } = new List<IEffectBase>();
    }
}
