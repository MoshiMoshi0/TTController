using System;
using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Config
{
    public class ProfileData
    {
        public string Name { get; private set; }
        public Guid Guid { get; private set; } = Guid.NewGuid();
        public List<PortIdentifier> Ports { get; private set; } = new List<PortIdentifier>();

        public List<SpeedControllerData> SpeedControllers { get; private set; } = new List<SpeedControllerData>();
        public List<EffectData> Effects { get; private set; } = new List<EffectData>();

        public ProfileData(string name)
        {
            Name = name;
        }
    }
}
