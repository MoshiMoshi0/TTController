using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.ColorSequenceEffect
{
    public class ColorSequenceEntry
    {
        [DefaultValue(1000)] public int TransitionTime { get; internal set; } = 1000;
        [DefaultValue(1000)] public int HoldTime { get; internal set; } = 1000;
        [DefaultValue(null)] public LedColorProvider Color { get; internal set; } = null;
    }

    public class ColorSequenceEffectConfig : EffectConfigBase
    {
        [DefaultValue(null)] public List<ColorSequenceEntry> Sequence { get; internal set; } = null;
    }

    public class ColorSequenceEffect : EffectBase<ColorSequenceEffectConfig>
    {
        private enum SequenceState
        {
            Transition,
            Hold
        }

        private SequenceState _state;
        private int _stateStart;
        private int _sequenceIndex;

        public ColorSequenceEffect(ColorSequenceEffectConfig config) : base(config)
        {
            _state = SequenceState.Hold;
            _stateStart = Environment.TickCount;
            _sequenceIndex = 0;
        }

        public override string EffectType => "PerLed";

        public override void Update(ICacheProvider cache) => UpdateState();

        protected override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
            => EffectUtils.GenerateColorsPerPort(ports, cache, (_, ledCount) => GenerateColors(ledCount, cache));

        protected override List<LedColor> GenerateColors(int count, ICacheProvider cache)
        {
            var current = Config.Sequence[_sequenceIndex];
            if (_state == SequenceState.Hold)
            {
                return current.Color.Get(count).ToList();
            }
            else if (_state == SequenceState.Transition)
            {
                int Wrap(int a, int b) => (a % b + b) % b;

                var prev = Config.Sequence[Wrap(_sequenceIndex - 1, Config.Sequence.Count)];
                var t = (Environment.TickCount - _stateStart) / (float)current.TransitionTime;
                return LedColor.Lerp(t, prev.Color.Get(count), current.Color.Get(count)).ToList();
            }

            return null;
        }

        private void UpdateState()
        {
            var current = Config.Sequence[_sequenceIndex];
            var ticks = Environment.TickCount;
            if (_state == SequenceState.Hold && ticks - _stateStart > current.HoldTime)
            {
                _sequenceIndex = (_sequenceIndex + 1) % Config.Sequence.Count;
                _state = SequenceState.Transition;
                _stateStart = ticks;
            }
            else if (_state == SequenceState.Transition && ticks - _stateStart > current.TransitionTime)
            {
                _state = SequenceState.Hold;
                _stateStart = ticks;
            }
        }
    }
}
