using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using NColorSpace.Converter.Spaces;

namespace NColorSpace.Converter
{
    public class Utility
    {
        public bool IsBackgroundSameColor(string imagePath)
        {
            var image = Image.FromFile(imagePath);
            return ProcessImage(new Bitmap(image));
        }

        public bool IsBackgroundSameColor(Image image)
        {
            return ProcessImage(new Bitmap(image));
        }

        public bool IsBackgroundSameColor(Stream stream)
        {
            return ProcessImage(new Bitmap(Image.FromStream(stream)));
        }

        public bool IsWhitishColor(Color? color)
        {
            if (color == null)
                return true;
            return ((Color)color).R >= 200 && ((Color)color).G >= 200 && ((Color)color).B >= 200
                   && Math.Abs(((Color)color).R - ((Color)color).G) <= 5 && Math.Abs(((Color)color).R - ((Color)color).B) <= 5;
        }

        private static bool ProcessImage(Bitmap image)
        {
            var bitmap = new Bitmap(image);

            CieLab prevCieLab = null;

            for (int i = 0; i < (int)(bitmap.Width * 0.05); i++)
            {
                for (int j = 0; j < (int)(bitmap.Height * 0.40); j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    var xyz = pixel.ToXyz();
                    var cieLab = xyz.ToCieLab();
                    if (prevCieLab == null)
                        prevCieLab = cieLab;
                    else
                    {
                        var diff = GetDeltaE(prevCieLab, cieLab);
                        if (diff > 5)
                        {
                            return false;
                        }

                        prevCieLab = cieLab;
                    }
                }
            }

            for (int i = bitmap.Width - (int)(bitmap.Width * 0.05); i < bitmap.Width; i++)
            {
                for (int j = 0; j < (int)(bitmap.Height * 0.40); j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    var xyz = pixel.ToXyz();
                    var cieLab = xyz.ToCieLab();
                    if (prevCieLab == null)
                        prevCieLab = cieLab;
                    else
                    {
                        var diff = GetDeltaE(prevCieLab, cieLab);
                        if (diff > 5)
                        {
                            return false;
                        }

                        prevCieLab = cieLab;
                    }
                }
            }

            return true;
        }

        private static double GetDeltaE(CieLab c1, CieLab c2)
        {
            return Math.Sqrt(Math.Pow(c1.L - c2.L, 2) + Math.Pow(c1.a - c2.a, 2) + Math.Pow(c1.b - c2.b, 2));
        }
    }
}
