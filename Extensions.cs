using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelbrotSet
{
    public static class ColorExtensions
    {
        public static Color FromArgb<T>(T R, T G, T B)
            where T : IConvertible
        {
            return FromArgb(default(T), R, G, B, true);
        }

        public static Color FromArgb<T>(T A, T R, T G, T B, bool ignoreAlpha = true)
            where T : IConvertible
        {
            return Color.FromArgb(ignoreAlpha ? 255 : A.ToInt32(null).NormalizeColourComponent(), R.ToInt32(null).NormalizeColourComponent(), G.ToInt32(null).NormalizeColourComponent(), B.ToInt32(null).NormalizeColourComponent());
        }

        public static Color LinearInterpolate(this Color start, Color end, double ratio, bool ignoreAlpha = true)
        {
            double ratioNorm = 1.0 - ratio,
            A = (ratioNorm * start.A) + (ratio * end.A),
            R = (ratioNorm * start.R) + (ratio * end.R),
            G = (ratioNorm * start.G) + (ratio * end.G),
            B = (ratioNorm * start.B) + (ratio * end.B);
            return FromArgb(A, R, G, B, ignoreAlpha);
        }

        public static int NormalizeColourComponent(this int value)
        {
            if (value < 0)
            {
                value = -((-value) % 510);
                return NormalizeColourComponent(255 + value);
            }

            if (value > 255)
            {
                value %= 510;
                return NormalizeColourComponent(value - 255);
            }
            return value;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value).NormalizeColourComponent();
            int p = Convert.ToInt32(value * (1 - saturation)).NormalizeColourComponent();
            int q = Convert.ToInt32(value * (1 - f * saturation)).NormalizeColourComponent();
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation)).NormalizeColourComponent();

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}
