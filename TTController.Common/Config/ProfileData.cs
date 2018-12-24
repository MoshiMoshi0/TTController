using System;
using System.Collections.Generic;

namespace TTController.Common.Config
{
    public class ProfileData
    {
        public Guid Guid { protected set; get; }
        public string Name { protected set; get; }
        public List<PortIdentifier> Ports { protected set; get; }
        public List<LedColor> LedColors { protected set; get; }

        public ProfileData(string name)
        {
            Name = name;

            Guid = Guid.NewGuid();
            Ports = new List<PortIdentifier>();
        }
    }
}
