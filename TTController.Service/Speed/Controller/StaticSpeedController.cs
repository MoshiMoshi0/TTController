using System.Collections.Generic;
using System.Linq;
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
        public StaticSpeedController(TemperatureManager temperatureManager, StaticSpeedControllerConfig config) : base(temperatureManager, config) {}

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(IDictionary<PortIdentifier, PortData> portDataMap)
        {
            return portDataMap.ToDictionary(kv => kv.Key, kv => Config.Speed);
        }
    }
}
