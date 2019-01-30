using System.Collections.Generic;
using System.Linq;
using TTController.Common;

namespace TTController.Effects
{
    public class SnakeEffectConfig : EffectConfigBase
    {
        public int Length { get; set; }
        public LedColor SnakeColor { get; set; }
        public LedColor BackgroundColor { get; set; }
    }

    public class SnakeEffect : EffectBase<SnakeEffectConfig>
    {
        private int _head;

        public SnakeEffect(SnakeEffectConfig config) : base(config)
        {
            _head = 0;
        }

        public override byte EffectByte => (byte) EffectType.ByLed;

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            int Wrap(int a, int b) => (a % b + b) % b;

            var ledCount = ports.Select(p => cache.GetPortConfig(p).LedCount).Sum();
            var colors = Enumerable.Range(0, ledCount).Select(x => Config.BackgroundColor).ToList(); 
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
