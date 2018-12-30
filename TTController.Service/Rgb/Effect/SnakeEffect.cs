using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Service.Config;

namespace TTController.Service.Rgb.Effect
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

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(IDictionary<PortIdentifier, PortConfigData> portConfigMap)
        {
            int Wrap(int a, int b) => (a % b + b) % b;

            var ledCount = portConfigMap.Select(kv => kv.Value.LedCount).Sum();
            var colors = Enumerable.Range(0, ledCount).Select(x => Config.BackgroundColor).ToList(); 
            for (var i = 0; i < Config.Length; i++)
                colors[Wrap(_head - i, ledCount)] = Config.SnakeColor;

            var sliceOffset = 0;
            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            foreach (var kv in portConfigMap)
            {
                var port = kv.Key;
                var portConfig = kv.Value;

                var slice = colors.GetRange(sliceOffset, portConfig.LedCount);
                if (portConfig.LedRotation > 0)
                    slice = slice.Skip(portConfig.LedRotation).Concat(slice.Take(portConfig.LedRotation)).ToList();
                if (portConfig.LedReverse)
                    slice.Reverse();

                sliceOffset += portConfig.LedCount;
                result.Add(port, slice);
            }

            _head = ++_head % ledCount;
            return result;
        }
    }
}
