using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;
using TTController.Service.Manager;

namespace TTController.Service.Speed.Controller
{
    public class StaticSpeedControllerConfig : SpeedControllerConfigBase
    {
        public byte Speed { get; set; } = 50;
    }

    public class StaticSpeedController : SpeedControllerBase<StaticSpeedControllerConfig>
    {
        public StaticSpeedController(TemperatureManager temperatureManager, dynamic config) : base(temperatureManager, (object)config) {}

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(IDictionary<PortIdentifier, PortData> portDataMap)
        {
            return portDataMap.ToDictionary(kv => kv.Key, kv => Config.Speed);
        }
    }
}
