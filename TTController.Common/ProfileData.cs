using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common
{
    public class ProfileData
    {
        public Guid Guid { protected set; get; }
        public string Name { protected set; get; }
        public List<PortIdentifier> Ports { protected set; get; }

        public ProfileData(string name)
        {
            Name = name;

            Guid = Guid.NewGuid();
            Ports = new List<PortIdentifier>();
        }
    }
}
