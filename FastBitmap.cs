using System.Drawing.Imaging;

namespace MandelbrotSet
{
    //TODO: FINISH THIS SHIT
    public unsafe class FastBitmap
    {
        private int height;
        private PixelFormat pixelFormat;
        private int reserved;
        private int* scan0;
        private int stride;
        private int width;

        public FastBitmap(int width, int height, PixelFormat pixelFormat = PixelFormat.Format32bppRgb)
        {
            this.width = width;
            this.height = height;
            this.pixelFormat = pixelFormat;
        }
    }
}
