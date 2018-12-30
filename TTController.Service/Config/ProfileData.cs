using System;
using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Config
{
    public class ProfileData
    {
        public string Name { set; get; }
        public Guid Guid { set; get; } = Guid.NewGuid();
        public List<PortIdentifier> Ports { get; set; } = new List<PortIdentifier>();

        public List<SpeedControllerData> SpeedControllers { get; set; } = new List<SpeedControllerData>();
        public List<EffectData> Effects { get; set; } = new List<EffectData>();

        public ProfileData(string name)
        {
            Name = name;
        }
    }
}
