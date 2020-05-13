using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TTController.Common
{
    public class LedColorProvider
    {
        [DefaultValue(null)] public LedColor? Full { get; private set; } = null;
        [DefaultValue(null)] public List<LedColor> PerLed { get; private set; } = null;
        [DefaultValue(null)] public LedColorGradient Gradient { get; private set; } = null;

        public LedColor Get(int index, int size)
        {
            if (Full != null) return Full.Value;
            if (PerLed != null && index < PerLed.Count) return PerLed[index];
            if (Gradient != null) return Gradient.GetColor(index / (size - 1d));

            return new LedColor(0, 0, 0);
        }

        public IEnumerable<LedColor> Get(int size)
        {
            if (Full != null) return Enumerable.Repeat(Full.Value, size);
            if (PerLed != null) return size <= PerLed.Count ? PerLed : PerLed.Concat(Enumerable.Repeat(new LedColor(0, 0, 0), size - PerLed.Count));
            if (Gradient != null) return Enumerable.Range(0, size).Select(x => Gradient.GetColor(x / (size - 1d)));

            return Enumerable.Repeat(new LedColor(0, 0, 0), size);
        }
    }
}
