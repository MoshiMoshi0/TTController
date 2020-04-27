using System;

namespace TTController.Common
{
    public readonly struct LedColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public LedColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public LedColor(double hue, double saturation, double value)
        {
            (R, G, B) = FromHsv(hue, saturation, value);
        }

        public override string ToString() => $"[{R}, {G}, {B}]";

        public void Deconstruct(out byte r, out byte g, out byte b)
        {
            r = R;
            g = G;
            b = B;
        }

        public LedColor Lerp(LedColor to, double t) => Lerp(t, this, to);
        public (double, double, double) LerpSmooth(LedColor to, double t) => LerpSmooth(t, this, to);

        public (double, double, double) ToHsv() => ToHsv(this);

        public LedColor SetHue(double hue)
        {
            var (_, s, v) = ToHsv();
            return FromHsv(hue, s, v);
        }

        public LedColor SetSaturation(double saturation)
        {
            var (h, _, v) = ToHsv();
            return FromHsv(h, saturation, v);
        }

        public LedColor SetValue(double value)
        {
            var (h, s, _) = ToHsv();
            return FromHsv(h, s, value);
        }

        public double GetHue() => ToHsv().Item1;
        public double GetSaturation() => ToHsv().Item2;
        public double GetValue() => ToHsv().Item3;

        public static LedColor Lerp(double t, LedColor from, LedColor to)
        {
            var (r, g, b) = LerpSmooth(t, from, to);
            return new LedColor((byte)Math.Round(r), (byte)Math.Round(b), (byte)Math.Round(b));
        }

        public static (double, double, double) LerpSmooth(double t, LedColor from, LedColor to)
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
            var v = (byte) Math.Round((value));
            var p = (byte) Math.Round((value * (1 - saturation)));
            var q = (byte) Math.Round((value * (1 - f * saturation)));
            var t = (byte) Math.Round((value * (1 - (1 - f) * saturation)));

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

        public static LedColor Unpack(int data)
        {
            var r = (byte)((data >> 0) & 0xff);
            var g = (byte)((data >> 8) & 0xff);
            var b = (byte)((data >> 16) & 0xff);

            return new LedColor(r, g, b);
        }

        public static int Pack(LedColor color)
            => (color.R) | (color.G << 8) | (color.B << 16);
    }
}
