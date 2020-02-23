using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.CopySpeedController
{
    public class CopySpeedControllerConfig : SpeedControllerConfigBase
    {
        public PortIdentifier Target { get; internal set; }
    }

    public class CopySpeedController : SpeedControllerBase<CopySpeedControllerConfig>
    {
        public CopySpeedController(CopySpeedControllerConfig config) : base(config) { }

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var speed = cache.GetPortSpeed(Config.Target);
            if (!speed.HasValue)
                return null;

            return ports.ToDictionary(p => p, _ => speed.Value);
        }
    }
}
