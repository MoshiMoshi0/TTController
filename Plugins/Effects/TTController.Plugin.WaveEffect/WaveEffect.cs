using System.Collections.Generic;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.WaveEffect
{
    public class WaveEffectConfig : EffectConfigBase
    {
        public List<LedColor> Colors { get; set; }
        public int TickInterval { get; set; }
    }

    public class WaveEffect : EffectBase<WaveEffectConfig>
    {
        private int _tick;
        private readonly LinkedList<LedColor> _colors;

        public WaveEffect(WaveEffectConfig config) : base(config)
        {
            _tick = Config.TickInterval;
            _colors = new LinkedList<LedColor>(Config.Colors);
        }

        public override string EffectType => "ByLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            if (_tick++ >= Config.TickInterval)
            {
                _tick = 0;
                _colors.AddFirst(_colors.Last.Value);
                _colors.RemoveLast();
            }

            return ports.ToDictionary(p => p, p => _colors.ToList());
        }
    }
}
