using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.StaticSpeedController
{
    public class StaticSpeedControllerConfig : SpeedControllerConfigBase
    {
        [DefaultValue(50)] public byte Speed { get; private set; } = 50;
    }

    public class StaticSpeedController : SpeedControllerBase<StaticSpeedControllerConfig>
    {
        public StaticSpeedController(StaticSpeedControllerConfig config) : base(config) {}

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache)
        {
            return ports.ToDictionary(p => p, _ => Config.Speed);
        }
    }
}
