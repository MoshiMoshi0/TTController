using LibreHardwareMonitor.Hardware;
using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Config.Converters
{
    public class SensorConfigConverter : AbstractNamedValueTupleConverter<List<Identifier>, SensorConfig>
    {
        protected override string KeyName() => "Sensors";
        protected override string ValueName() => "Config";
    }
}
