using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.DpsgSpeedController
{
    public enum FanMode
    {
        Off = 0,
        Silent = 1,
        Performance = 2,
    }

    public class DpsgSpeedControllerConfig : SpeedControllerConfigBase
    {
        [DefaultValue(FanMode.Silent)] public FanMode FanMode { get; internal set; } = FanMode.Silent;
    }

    public class DpsgSpeedController : SpeedControllerBase<DpsgSpeedControllerConfig>
    {
        public DpsgSpeedController(DpsgSpeedControllerConfig config) : base(config) { }

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache)
            => ports.ToDictionary(p => p, _ => (byte)Config.FanMode);
    }
}
