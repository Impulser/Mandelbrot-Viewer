using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace MandelbrotSet
{
    public static class Generator
    {
        public static async Task<Bitmap> MandelbrotSet(Bitmap source, MandelbrotSettings settings)
        {
            return await Task.Run(() =>
            {
                var bitmap = new LockBitmap(source).LockBits();
                var width = bitmap.Width;
                var height = bitmap.Height;
                var xStep = ((settings.MaxReal - settings.MinReal) / width);
                var yStep = ((settings.MaxImaginary - settings.MinImaginary) / height);
                var maximumIterations = (int)Math.Min((200.0 * settings.ZoomLevel), 1 << 16);
                var numPixels = width * height;
                var absReal = Math.Abs(settings.MinReal);
                var absImaginary = Math.Abs(settings.MinImaginary);
                Parallel.ForEach(RangePartition.CreatePartitionedRange(0, numPixels, numPixels / width), pixelIndexRange =>
                {
                    foreach (var pixelIndex in pixelIndexRange)
                    {
                        var pX = pixelIndex % width;
                        var pY = pixelIndex / width;
                        var x = 0.0;
                        var y = 0.0;
                        var xScale = (xStep * pX) - absReal;
                        var yScale = (yStep * pY) - absImaginary;
                        var iteration = 0.0;

                        //var p = Math.Sqrt()
                        while (x * x + y * y < 4 && iteration < maximumIterations)
                        {
                            var temp = (x * x) - (y * y) + xScale;
                            y = 2 * x * y + yScale;
                            x = temp;
                            iteration += 1.0;
                        }

                        var pixelColour = Color.Black;
                        if (iteration < maximumIterations)
                        {
                            //var colourA = ColourFromIteration(Math.Floor(iteration), zoomLevel);
                            //var colourB = ColourFromIteration(Math.Floor(iteration + 1), zoomLevel);
                            if(settings.LerpColours)
                            {
                                var colourA = ColourFromIteration(Math.Floor(iteration), x * x, y * y, settings);
                                var colourB = ColourFromIteration(Math.Floor(iteration + settings.LerpStep), x * x, y * y, settings);
                                pixelColour = colourA.LinearInterpolate(colourB, iteration % settings.LerpStep, true);
                            }
                            else
                            {
                                pixelColour = ColourFromIteration(iteration, x * x, y * y, settings);
                            }
                        }

                        bitmap.SetPixel(pX, pY, pixelColour);
                    }
                });
                return bitmap.UnlockBits();
            });
        }


        public static Color ColourFromIteration(double iteration, double r, double c, MandelbrotSettings settings)
        {
            double zn;
            var red = 0;
            var green = 0;
            var blue = 0;
            switch (settings.ColourAlgorithm)
            {
                case 1:
                    double hue = iteration;  // 2 is escape radius
                    if (settings.LerpColours)
                    {
                        zn = Math.Sqrt(r + c);
                        hue = iteration + 1.0 - Math.Log(Math.Log(Math.Abs(zn))) / Math.Log(2.0);
                    }
                    hue = 0.95 + 20.0 * hue; // adjust to make it prettier
                                             // the hsv function expects values from 0 to 360
                    while (hue > 360.0)
                        hue -= 360.0;
                    while (hue < 0.0)
                        hue += 360.0;

                    return ColorExtensions.ColorFromHSV(hue, settings.Saturation, settings.Value);

                case 2:
                    if (settings.LerpColours)
                    {
                        zn = Math.Sqrt(r + c);
                        iteration = iteration + 1.0 - Math.Log(Math.Log(Math.Abs(zn))) / Math.Log(2.0);
                    }
                    red = Adjust((Math.Sin(iteration / 300.0 * settings.ZoomLevel) + 1) * 127.0f);
                    green = Adjust((Math.Sin(iteration / 200.0 * settings.ZoomLevel) + 1) * 127.0f);
                    blue = Adjust((Math.Sin(iteration / 100.0 * settings.ZoomLevel) + 1) * 127.0f);
                    return ColorExtensions.FromArgb(red, green, blue);

                case 3:
                    if (settings.LerpColours)
                    {
                        zn = Math.Sqrt(r + c);
                        iteration = iteration + 1.0 - Math.Log(Math.Log(Math.Abs(zn))) / Math.Log(2.0);
                    }
                    red = Adjust((Math.Cos(iteration / 300.0 * settings.ZoomLevel) + 1) * 127.0f);
                    green = Adjust((Math.Cos(iteration / 200.0 * settings.ZoomLevel) + 1) * 127.0f);
                    blue = Adjust((Math.Cos(iteration / 100.0 * settings.ZoomLevel) + 1) * 127.0f);
                    return ColorExtensions.FromArgb(red, green, blue);

                case 4:
                    if (settings.LerpColours)
                    {
                        zn = Math.Sqrt(r + c);
                        iteration = iteration + 1.0 - Math.Log(Math.Log(Math.Abs(zn))) / Math.Log(2.0);
                    }
                    red = Adjust((Math.Sin(iteration / 100.0 * settings.ZoomLevel) + 1) * 127.0f);
                    green = Adjust((Math.Cos(iteration / 100.0 * settings.ZoomLevel) + 1) * 127.0f);
                    blue = Adjust((Math.Tan(iteration / 100.0 * settings.ZoomLevel) + 1) * 127.0f);
                    return ColorExtensions.FromArgb(red, green, blue);

                case 5:
                    if (settings.LerpColours)
                    {
                        zn = Math.Sqrt(r + c);
                        iteration = iteration + 1.0 - Math.Log(Math.Log(Math.Abs(zn))) / Math.Log(2.0);
                    }
                    return ColorExtensions.FromArgb(Adjust(iteration * 2), Adjust(iteration * 4), Adjust(iteration * 8));

                case 6:
                    if (settings.LerpColours)
                    {
                        zn = Math.Sqrt(r + c);
                        iteration = iteration + 1.0 - Math.Log(Math.Log(Math.Abs(zn))) / Math.Log(2.0);
                    }
                    return ColorExtensions.FromArgb(Adjust(iteration * 8), Adjust(iteration * 4), Adjust(iteration * 2));

                case 7:
                    if (settings.LerpColours)
                    {
                        zn = Math.Sqrt(r + c);
                        iteration = iteration + 1.0 - Math.Log(Math.Log(Math.Abs(zn))) / Math.Log(2.0);
                    }
                    return ColorExtensions.FromArgb(Adjust(iteration * 2 * settings.ZoomLevel), Adjust(iteration * 4 * settings.ZoomLevel), Adjust(iteration * 8 * settings.ZoomLevel));

                case 8:
                    if (settings.LerpColours)
                    {
                        zn = Math.Sqrt(r + c);
                        iteration = iteration + 1.0 - Math.Log(Math.Log(Math.Abs(zn))) / Math.Log(2.0);
                    }
                    return ColorExtensions.FromArgb(Adjust(iteration * 8 * settings.ZoomLevel), Adjust(iteration * 4 * settings.ZoomLevel), Adjust(iteration * 2 * settings.ZoomLevel));

                default:
                    return ColorExtensions.FromArgb(Adjust(iteration), Adjust(iteration), Adjust(iteration));
            }
        }

        private static int Adjust(double input)
        {
            return ColorExtensions.NormalizeColourComponent((int)Math.Round(input));
        }

        

        public class RangePartition : List<int>
        {
            public RangePartition(int from, int size)
                    : base(Enumerable.Range(from, size))
            { }

            public static IEnumerable<RangePartition> CreatePartitionedRange(int from, int to, int cardinality)
            {
                while (from < to)
                {
                    var partitionLen = Math.Min(to - from, cardinality);
                    yield return new RangePartition(from, partitionLen);
                    from += partitionLen;
                }
            }
        }
    }
}
