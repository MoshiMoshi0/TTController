using OpenHardwareMonitor.Hardware;
using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Config.Converter
{
    public class SensorConfigMapConverter : AbstractNamedValueTupleConverter<List<Identifier>, SensorConfig>
    {
        protected override string KeyName() => "Sensors";
        protected override string ValueName() => "Config";
    }
}
