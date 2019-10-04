using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Config.Converters
{
    public class PortConfigConverter : AbstractNamedValueTupleConverter<List<PortIdentifier>, PortConfig>
    {
        protected override string KeyName() => "Ports";
        protected override string ValueName() => "Config";
    }
}
