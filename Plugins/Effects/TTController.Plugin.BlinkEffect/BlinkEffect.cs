using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;

namespace TTController.Plugin.BlinkEffect
{
    public class BlinkEffectConfig : EffectConfigBase
    {
        public int On { get; set; } = 1000;
        public int Off { get; set; } = 1000;
        public List<LedColor> Colors { get; set; }
    }

    public class BlinkEffect : EffectBase<BlinkEffectConfig>
    {
        private int _ticks;
        private bool _state;

        public BlinkEffect(BlinkEffectConfig config) : base(config) { }

        public override byte EffectByte => (byte) EffectType.ByLed;

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var current = Environment.TickCount;
            var diff = current - _ticks;

            if ((_state && diff > Config.On) || (!_state && diff > Config.Off))
            {
                _ticks = current;
                _state = !_state;
            }
            
            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            foreach (var port in ports)
            {
                var config = cache.GetPortConfig(port);
                if (config == null)
                    continue;

                if (_state)
                {
                    result.Add(port, Config.Colors.ToList());
                }
                else
                {
                    var offColor = new LedColor(0, 0, 0);
                    result.Add(port, Enumerable.Range(0, config.LedCount).Select(x => offColor).ToList());
                }
            }

            return result;
        }
    }
}
