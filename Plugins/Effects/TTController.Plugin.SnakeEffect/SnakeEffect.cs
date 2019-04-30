using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.SnakeEffect
{
    public class SnakeEffectConfig : EffectConfigBase
    {
        [DefaultValue(5)] public int Length { get; private set; } = 5;
        [DefaultValue(2)] public int TickCount { get; private set; } = 2;
        public LedColor SnakeColor { get; private set; } = new LedColor(0, 0, 0);
        public LedColor BackgroundColor { get; private set; } = new LedColor(0, 0, 0);
    }

    public class SnakeEffect : EffectBase<SnakeEffectConfig>
    {
        private int _head;
        private int _tick;

        public SnakeEffect(SnakeEffectConfig config) : base(config)
        {
            _head = 0;
            _tick = 0;
        }

        public override string EffectType => "ByLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            int Wrap(int a, int b) => (a % b + b) % b;

            if (_tick++ < Config.TickCount)
                return null;
            _tick = 0;

            var ledCount = ports.Select(p => cache.GetPortConfig(p).LedCount).Sum();
            var colors = Enumerable.Range(0, ledCount).Select(_ => Config.BackgroundColor).ToList();
            for (var i = 0; i < Config.Length; i++)
                colors[Wrap(_head - i, ledCount)] = Config.SnakeColor;

            var sliceOffset = 0;
            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            foreach (var port in ports)
            {
                var config = cache.GetPortConfig(port);
                result.Add(port, colors.GetRange(sliceOffset, config.LedCount));
                sliceOffset += config.LedCount;
            }

            _head = ++_head % ledCount;
            return result;
        }
    }
}
