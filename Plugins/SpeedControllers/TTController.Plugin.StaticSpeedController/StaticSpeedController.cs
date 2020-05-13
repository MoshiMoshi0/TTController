using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.StaticSpeedController
{
    public class StaticSpeedControllerConfig : SpeedControllerConfigBase
    {
        [DefaultValue(50)] public byte Speed { get; internal set; } = 50;
    }

    public class StaticSpeedController : SpeedControllerBase<StaticSpeedControllerConfig>
    {
        public StaticSpeedController(StaticSpeedControllerConfig config) : base(config)
        {
            if(Config.Speed != 0 && Config.Speed < 20)
                Config.Speed = 20;
        }

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache)
            => ports.ToDictionary(p => p, _ => Config.Speed);
    }
}
