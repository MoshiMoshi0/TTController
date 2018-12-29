using System;
using System.Collections.Generic;

namespace TTController.Common.Config
{
    public class ProfileData
    {
        public string Name { set; get; }
        public Guid Guid { set; get; } = Guid.NewGuid();
        public List<PortIdentifier> Ports { get; set; } = new List<PortIdentifier>();

        public Dictionary<string, dynamic> SpeedControllers { get; set; } = new Dictionary<string, dynamic>();
        public Dictionary<string, dynamic> Effects { get; set; } = new Dictionary<string, dynamic>();

        public ProfileData(string name)
        {
            Name = name;
        }
    }
}
