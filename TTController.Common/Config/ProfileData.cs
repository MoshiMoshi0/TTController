using System;
using System.Collections.Generic;

namespace TTController.Common.Config
{
    public class ProfileData
    {
        public Guid Guid { set; get; }
        public string Name { set; get; }
        public List<PortIdentifier> Ports { set; get; }
        public List<LedColor> LedColors { set; get; }

        public ProfileData(string name)
        {
            Name = name;

            Guid = Guid.NewGuid();
            Ports = new List<PortIdentifier>();
        }
    }
}
