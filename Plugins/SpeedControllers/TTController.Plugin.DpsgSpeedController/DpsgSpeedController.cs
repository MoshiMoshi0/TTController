using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.DpsgSpeedController
{
    public enum FanMode
    {
        Silent,
        Performance
    }

    public class DpsgSpeedControllerConfig : SpeedControllerConfigBase
    {
        [DefaultValue(FanMode.Silent)] public FanMode FanMode { get; private set; } = FanMode.Silent;
    }

    public class DpsgSpeedController : SpeedControllerBase<DpsgSpeedControllerConfig>
    {
        public DpsgSpeedController(DpsgSpeedControllerConfig config) : base(config) { }

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache)
        {
            switch (Config.FanMode)
            {
                default:
                case FanMode.Silent:
                    return ports.ToDictionary(p => p, _ => (byte) 1);
                case FanMode.Performance:
                    return ports.ToDictionary(p => p, _ => (byte) 2);
            }
        }
    }
}
