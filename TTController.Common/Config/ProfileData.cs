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
        public string RgbEffect { set; get; }
        public dynamic RgbEffectConfig { set; get; }

        public ProfileData(string name)
        {
            Name = name;
        }
    }
}
