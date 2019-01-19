using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Service.Manager;

namespace TTController.Service.Controller.Speed
{
    public class StaticSpeedControllerConfig : SpeedControllerConfigBase
    {
        public byte Speed { get; set; } = 50;
    }

    public class StaticSpeedController : SpeedControllerBase<StaticSpeedControllerConfig>
    {
        public StaticSpeedController(StaticSpeedControllerConfig config) : base(config) {}

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache)
        {
            return ports.ToDictionary(p => p, p => Config.Speed);
        }
    }
}
