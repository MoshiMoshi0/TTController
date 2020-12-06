using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.BlendEffect
{
    public enum BlendMode
    {
        Darken,
        Multiply,
        ColorBurn,
        LinearBurn,
        DarkerColor,
        Lighten,
        Screen,
        ColorDodge,
        LinearDodge,
        LighterColor,
        Overlay,
        SoftLight,
        HardLight,
        VividLight,
        LinearLight,
        PinLight,
        HardMix,
        Difference,
        Exclusion,
        Subtract,
        Divide,
        Hue,
        Color,
        Saturation,
        Luminosity,
    }

    public class BlendEffectConfig : EffectConfigBase
    {
        [DefaultValue(null)] public List<IEffectBase> Effects { get; internal set; } = null;
        [DefaultValue(BlendMode.Multiply)] public BlendMode BlendMode { get; internal set; } = BlendMode.Multiply;
    }

    public class BlendEffect : EffectBase<BlendEffectConfig>
    {
        public BlendEffect(BlendEffectConfig config) : base(config) { }

        public override string EffectType => "PerLed";

        public override void Update(ICacheProvider cache)
        {
            foreach (var effect in Config.Effects)
                effect.Update(cache);
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            foreach (var port in ports)
            {
                var device = cache.GetDeviceConfig(port);
                List<LedColor> destinationColors = null;

                foreach(var effect in Config.Effects)
                {
                    var colors = effect.GenerateColors(device.LedCount, cache);
                    if (destinationColors == null)
                    {
                        destinationColors = colors;
                        continue;
                    }

                    destinationColors = Blend(destinationColors, colors, Config.BlendMode);
                }

                result.Add(port, destinationColors);
            }

            return result;
        }

        public override List<LedColor> GenerateColors(int count, ICacheProvider cache)
            => throw new NotImplementedException();

        private List<LedColor> Blend(List<LedColor> destination, List<LedColor> source, BlendMode mode)
        {
            void EnsureCount(List<LedColor> colors, int desiredCount)
            {
                while (colors.Count < desiredCount)
                    colors.Add(new LedColor());
            }

            var count = Math.Max(destination.Count, source.Count);
            EnsureCount(destination, count);
            EnsureCount(source, count);

            var result = new List<LedColor>();
            for(var i = 0; i < count; i++)
                result.Add(Blend(destination[i], source[i], mode));

            return result;
        }

        private LedColor Blend(LedColor destination, LedColor source, BlendMode mode)
        {
            float Clamp01(float x) => Math.Max(0, Math.Min(1, x));
            float MakeFinite01(float x)
            {
                if (float.IsPositiveInfinity(x)) return 1;
                if (float.IsNegativeInfinity(x)) return 0;
                if (float.IsNaN(x)) return 0;
                return x;
            }

            LedColor SafeCreate01(float r, float g, float b)
            {
                var rr = (byte)Math.Round(Clamp01(MakeFinite01(r)) * 255);
                var gg = (byte)Math.Round(Clamp01(MakeFinite01(g)) * 255);
                var bb = (byte)Math.Round(Clamp01(MakeFinite01(b)) * 255);
                return new LedColor(rr, gg, bb);
            }

            LedColor Apply(Func<float, float, float> func)
                => SafeCreate01(func(destination.R / 255.0f, source.R / 255.0f),
                                func(destination.G / 255.0f, source.G / 255.0f),
                                func(destination.B / 255.0f, source.B / 255.0f));

            // based on https://www.shadertoy.com/view/XdS3RW
            switch (mode)
            {
                case BlendMode.Darken: return Apply((d, s) => Math.Min(d, s));
                case BlendMode.Multiply: return Apply((d, s) => d*s);
                case BlendMode.ColorBurn: return Apply((d, s) => 1 - (1 - d) / s);
                case BlendMode.LinearBurn: return Apply((d, s) => s + d - 1);
                case BlendMode.Lighten: return Apply((d, s) => Math.Max(d, s));
                case BlendMode.Screen: return Apply((d, s) => s + d - s * d);
                case BlendMode.ColorDodge: return Apply((d, s) => d / (1 - s));
                case BlendMode.LinearDodge: return Apply((d, s) => s + d);
                case BlendMode.Overlay: return Apply((d, s) => d < 0.5f ? 2 * s * d : 1 - 2 * (1 - s) * (1 - d));
                case BlendMode.HardLight: return Apply((d, s) => s < 0.5f ? 2 * s * d : 1 - 2 * (1 - s) * (1 - d));
                case BlendMode.VividLight: return Apply((d, s) => s < 0.5f ? 1 - (1 - d) / (2 * s) : d / (2 * (1 - s)));
                case BlendMode.LinearLight: return Apply((d, s) => 2 * s + d - 1);
                case BlendMode.PinLight: return Apply((d, s) => (2 * s - 1 > d) ? 2 * s - 1 : (s < 0.5f * d) ? 2 * s : d);
                case BlendMode.HardMix: return Apply((d, s) => (float)Math.Floor(s + d));
                case BlendMode.Difference: return Apply((d, s) => Math.Abs(d - s));
                case BlendMode.Exclusion: return Apply((d, s) => s + d - 2 * s * d);
                case BlendMode.Subtract: return Apply((d, s) => s - d);
                case BlendMode.Divide: return Apply((d, s) => s / d);
                case BlendMode.Hue: return LedColor.ChangeHue(destination, source.GetHue());
                case BlendMode.Color: return LedColor.ChangeValue(destination, source.GetValue());
                case BlendMode.Saturation: return LedColor.ChangeSaturation(destination, source.GetSaturation());
                case BlendMode.DarkerColor: return (source.R + source.G + source.B < destination.R + destination.G + destination.B) ? source : destination;
                case BlendMode.LighterColor: return (source.R + source.G + source.B > destination.R + destination.G + destination.B) ? source : destination;
                case BlendMode.SoftLight:
                    return Apply((d, s) =>
                    {
                        if (s < 0.5f)
                            return d - (1 - 2 * s) * d * (1 - d);
                        if (d < 0.25f)
                            return d + (2 * s - 1) * d * ((16 * d - 12) * d + 3);
                        return d + (2 * s - 1) * ((float)Math.Sqrt(d) - d);
                    });
                case BlendMode.Luminosity:
                    {
                        var dl = (float)destination.GetLuminance();
                        var sl = (float)source.GetLuminance();
                        var lum = sl - dl;

                        var cr = destination.R / 255.0f + lum;
                        var cg = destination.G / 255.0f + lum;
                        var cb = destination.B / 255.0f + lum;

                        var minC = (float)Math.Min(Math.Min(cr, cg), cb);
                        var maxC = (float)Math.Max(Math.Max(cr, cg), cb);

                        LedColor Create(Func<float, float> func) => SafeCreate01(func(cr), func(cg), func(cb));

                        if (minC < 0) return Create(x => sl + ((x - sl) * sl) / (sl - minC));
                        if (maxC > 1) return Create(x => sl + ((x - sl) * (1 - sl)) / (maxC - sl));
                        return Create(x => x);
                    }
                default: return new LedColor();
            }
        }
    }
}
