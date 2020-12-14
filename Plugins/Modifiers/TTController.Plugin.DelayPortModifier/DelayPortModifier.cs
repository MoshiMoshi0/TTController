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
        [DefaultValue(60)] public int FrameCount { get; internal set; } = 60;
        public LedColorProvider FallbackColor { get; internal set; } = new LedColorProvider();
    }

    public class DelayPortModifier : PortModifierBase<DelayPortModifierConfig>
    {
        private readonly Queue<List<LedColor>> _queue;

        public DelayPortModifier(DelayPortModifierConfig config) : base(config)
        {
            _queue = new Queue<List<LedColor>>();
        }

        public override void Apply(ref List<LedColor> colors, PortIdentifier port, ICacheProvider cache)
        {
            _queue.Enqueue(colors);

            if (_queue.Count < Config.FrameCount)
            {
                colors = Config.FallbackColor.Get(colors.Count).ToList();
                return;
            }

            while (_queue.Count >= Config.FrameCount)
                colors = _queue.Dequeue();
        }
    }
}
