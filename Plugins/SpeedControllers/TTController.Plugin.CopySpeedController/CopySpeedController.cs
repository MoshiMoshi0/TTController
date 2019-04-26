using System.Collections.Generic;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.CopySpeedController
{
    public class CopySpeedControllerConfig : SpeedControllerConfigBase
    {
        public PortIdentifier Target { get; private set; }
    }

    public class CopySpeedController : SpeedControllerBase<CopySpeedControllerConfig>
    {
        public CopySpeedController(CopySpeedControllerConfig config) : base(config) { }

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var data = cache.GetPortData(Config.Target);
            if (data?.Speed == null)
                return null;

            return ports.ToDictionary(p => p, _ => data.Speed.Value);
        }
    }
}
