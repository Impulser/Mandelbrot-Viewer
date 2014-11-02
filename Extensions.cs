using System;
using System.Drawing;

namespace MandelbrotSet
{
    public static class ColorExtensions
    {
        public static Color FromArgb<T>(T r, T g, T b)
                where T : IConvertible
        {
            return FromArgb(default(T), r, g, b, true);
        }

        public static Color FromArgb<T>(T a, T r, T g, T b, bool ignoreAlpha = true)
                where T : IConvertible
        {
            return Color.FromArgb(ignoreAlpha
                                          ? 255
                                          : a.NormColourComp(), r.NormColourComp(), g.NormColourComp(),
                                  b.NormColourComp());
        }

        public static Color LinearInterpolate(this Color start, Color end, double ratio, bool ignoreAlpha = true)
        {
            double ratioNorm = 1.0 - ratio,
                   a = (ratioNorm * start.A) + (ratio * end.A),
                   r = (ratioNorm * start.R) + (ratio * end.R),
                   g = (ratioNorm * start.G) + (ratio * end.G),
                   b = (ratioNorm * start.B) + (ratio * end.B);
            return FromArgb(a, r, g, b, ignoreAlpha);
        }

        public static int NormColourComp(this IConvertible input)
        {
            var value = input.ToInt32(null);
            while (true)
            {
                if (value < 0)
                {
                    value = -((-value) % 510);
                    value = 255 + value;
                    continue;
                }

                if (value > 255)
                {
                    value %= 510;
                    value = value - 255;
                    continue;
                }
                return value;
            }
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            var v = Convert.ToInt32(value).NormColourComp();
            var p = Convert.ToInt32(value * (1 - saturation)).NormColourComp();
            var q = Convert.ToInt32(value * (1 - f * saturation)).NormColourComp();
            var t = Convert.ToInt32(value * (1 - (1 - f) * saturation)).NormColourComp();

            switch (hi)
            {
                case 0:
                    return Color.FromArgb(255, v, t, p);
                case 1:
                    return Color.FromArgb(255, q, v, p);
                case 2:
                    return Color.FromArgb(255, p, v, t);
                case 3:
                    return Color.FromArgb(255, p, q, v);
                case 4:
                    return Color.FromArgb(255, t, p, v);
                default:
                    return Color.FromArgb(255, v, p, q);
            }
        }
    }

    public static class LanguageExtensions
    {
        public static bool Run<TResult>(this TResult value, Action<TResult> runAction)
                where TResult : class
        {
            if (value != null)
            {
                runAction(value);
                return true;
            }
            return false;
        }

        public static TOut Run<TIn, TOut>(this TIn value, Func<TIn, TOut> runAction)
                where TIn : class
        {
            return value != null
                           ? runAction(value)
                           : default(TOut);
        }
    }
}
