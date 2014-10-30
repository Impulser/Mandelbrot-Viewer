using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MandelbrotSet
{
    public static class Generator
    {
        public static async Task<Bitmap> MandelbrotSet(Bitmap source, double maxReal, double minReal, double maxImaginary, double minImaginary, double zoomLevel)
        {
            return await Task.Run(() =>
            {
                var bitmap = new LockBitmap(source).LockBits();
                var width = bitmap.Width;
                var height = bitmap.Height;
                var xStep = ((maxReal - minReal) / width);
                var yStep = ((maxImaginary - minImaginary) / height);
                var maximumIterations = 50 * (1 << (int) zoomLevel);
                var numPixels = width * height;
                var absReal = Math.Abs(minReal);
                var absImaginary = Math.Abs(minImaginary);
                Parallel.ForEach(RangePartition.CreatePartitionedRange(0, numPixels, numPixels / width), pixelIndexRange =>
                {
                    foreach (var pixelIndex in pixelIndexRange)
                    {
                        var x = pixelIndex % width;
                        var y = pixelIndex / width;
                        var zImaginary = 0.0;
                        var zReal = 0.0;
                        var cReal = (xStep * x) - absReal;
                        var cImaginary = (yStep * y) - absImaginary;
                        var iteration = 0;
                        while (zImaginary * zImaginary + zReal * zReal <= 3 && iteration < maximumIterations)
                        {
                            iteration++;
                            var temp = zImaginary;
                            zImaginary = (zImaginary * zImaginary) - (zReal * zReal) + cReal;
                            zReal = (2 * temp * zReal) + cImaginary;
                        }

                        bitmap.SetPixel(x, y, iteration < maximumIterations
                                                      ? ColourFromIteration(iteration, (int) zoomLevel)
                                                      : Color.Black);
                    }
                });
                return bitmap.UnlockBits();
            });
        }

        public static Color ColourFromIteration(int iteration, int zoomLevel)
        {
            iteration *= zoomLevel;
            var levelMod = iteration % zoomLevel;
            var r = (int) ((Math.Tan(iteration / 300.0f + levelMod) + 1) * 127.0f);
            var g = (int) ((Math.Sin(iteration / 200.0f + levelMod) + 1) * 127.0f);
            var b = (int) ((Math.Cos(iteration / 100.0f + levelMod) + 1) * 127.0f);

            return Color.FromArgb(Adjust(r), Adjust(g), Adjust(b));
        }

        private static int Adjust(int value)
        {
            value %= 255;
            return (value < 0
                            ? 255 + value
                            : value);
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
