using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.DelayPortModifier
{
    public class DelayPortModifierConfig : ModifierConfigBase
    {
        [DefaultValue(32)] public int FrameCount { get; internal set; } = 32;
        public LedColorProvider FallbackColor { get; internal set; } = new LedColorProvider();
    }

    public class DelayPortModifier : PortModifierBase<DelayPortModifierConfig>
    {
        private readonly Dictionary<PortIdentifier, Queue<List<LedColor>>> _portQueues;

        public DelayPortModifier(DelayPortModifierConfig config) : base(config)
        {
            _portQueues = new Dictionary<PortIdentifier, Queue<List<LedColor>>>();
        }

        public override void Apply(ref List<LedColor> colors, PortIdentifier port, ICacheProvider cache)
        {
            if (!_portQueues.ContainsKey(port))
                _portQueues.Add(port, new Queue<List<LedColor>>());

            var queue = _portQueues[port];
            queue.Enqueue(colors);

            if (queue.Count < Config.FrameCount)
            {
                colors = Config.FallbackColor.Get(colors.Count).ToList();
                return;
            }

            while (queue.Count >= Config.FrameCount)
                colors = queue.Dequeue();
        }
    }
}
