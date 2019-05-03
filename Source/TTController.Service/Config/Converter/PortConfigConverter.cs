using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converter
{
    public class PortConfigConverter : AbstractNamedKeyValuePairConverter<List<PortIdentifier>, PortConfig>
    {
        protected override string KeyName() => "Ports";
        protected override string ValueName() => "Config";
    }
}
