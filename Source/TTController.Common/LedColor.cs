using System;

namespace TTController.Common
{
    public struct LedColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public LedColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public override string ToString() => $"[{R}, {G}, {B}]";

        public void Deconstruct(out object r, out object g, out object b)
        {
            r = R;
            g = G;
            b = B;
        }

        public static LedColor Lerp(double t, LedColor from, LedColor to)
        {
            var (r, g, b) = LerpDeconstruct(t, from, to);
            return new LedColor((byte)r, (byte)g, (byte)b);
        }

        public static (double, double, double) LerpDeconstruct(double t, LedColor from, LedColor to)
        {
            t = Math.Max(Math.Min(t, 1), 0);
            var r = from.R * (1 - t) + to.R * t;
            var g = from.G * (1 - t) + to.G * t;
            var b = from.B * (1 - t) + to.B * t;
            return (r, g, b);
        }

        public static (double, double, double) ToHsv(LedColor color)
        {
            var max = Math.Max(color.R, Math.Max(color.G, color.B));
            var min = Math.Min(color.R, Math.Min(color.G, color.B));

            var delta = max - min;

            var hue = 0d;
            if (delta != 0)
            {
                if (color.R == max) hue = (color.G - color.B) / (double)delta;
                else if (color.G == max) hue = 2d + (color.B - color.R) / (double)delta;
                else if (color.B == max) hue = 4d + (color.R - color.G) / (double)delta;
            }

            hue *= 60;
            if (hue < 0.0) hue += 360;

            var saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            var value = max / 255d;
            return (hue, saturation, value);
        }

        public static LedColor FromHsv(double hue, double saturation, double value)
        {
            saturation = Math.Max(Math.Min(saturation, 1), 0);
            value = Math.Max(Math.Min(value, 1), 0);

            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            var v = (byte) (value);
            var p = (byte) (value * (1 - saturation));
            var q = (byte) (value * (1 - f * saturation));
            var t = (byte) (value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0: return new LedColor(v, t, p);
                case 1: return new LedColor(q, v, p);
                case 2: return new LedColor(p, v, t);
                case 3: return new LedColor(p, q, v);
                case 4: return new LedColor(t, p, v);
                default: return new LedColor(v, p, q);
            }
        }
    }
}
