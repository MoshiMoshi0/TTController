using System;
using System.Collections.Generic;

namespace TTController.Common.Config
{
    public class ProfileData
    {
        public string Name { set; get; }
        public Guid Guid { set; get; } = Guid.NewGuid();
        public List<PortIdentifier> Ports { set; get; } = new List<PortIdentifier>();
        public List<CurvePoint> CurvePoints { set; get; } = new List<CurvePoint>();
        public Dictionary<string, dynamic> Effects { get; set; } = new Dictionary<string, dynamic>();

        public ProfileData(string name)
        {
            Name = name;
        }
    }
}
