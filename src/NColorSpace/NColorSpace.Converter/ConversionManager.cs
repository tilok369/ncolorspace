using System;
using System.Drawing;
using NColorSpace.Converter.Spaces;

namespace NColorSpace.Converter
{
    public static class ConversionManager
    {
        public static Xyz ToXyz(this Color color)
        {
            var r = (double)color.R / 255;
            var g = (double)color.G / 255;
            var b = (double)color.B / 255;

            if (r > 0.04045)
                r = Math.Pow(((r + 0.055) / 1.055), 2.4);
            else r = r / 12.92;

            if (g > 0.04045)
                g = Math.Pow(((g + 0.055) / 1.055), 2.4);
            else g = g / 12.92;

            if (b > 0.04045)
                b = Math.Pow(((b + 0.055) / 1.055), 2.4);
            else b = b / 12.92;

            r *= 100;
            g *= 100;
            b *= 100;

            return new Xyz
            {
                X = r * 0.4124 + g * 0.3576 + b * 0.1805,
                Y = r * 0.2126 + g * 0.7152 + b * 0.0722,
                Z = r * 0.0193 + g * 0.1192 + b * 0.9505
            };
        }

        public static CieLab ToCieLab(this Xyz xyz)
        {
            var x = xyz.X / Globals.Constants.D65X2;
            var y = xyz.Y / Globals.Constants.D65Y2;
            var z = xyz.Z / Globals.Constants.D65Z2;

            if (x > 0.008856) x = Math.Pow(x, ((double)1 / 3));
            else x = (7.787 * x) + ((double)16 / 116);

            if (y > 0.008856) y = Math.Pow(y, ((double)1 / 3));
            else y = (7.787 * y) + ((double)16 / 116);

            if (z > 0.008856) z = Math.Pow(z, ((double)1 / 3));
            else z = (7.787 * z) + ((double)16 / 116);

            return new CieLab
            {
                L = (116 * y) - 16,
                a = 500 * (x - y),
                b = 200 * (y - z)
            };
        }
    }
}
