using System;
using System.Drawing;
using System.Windows.Forms;

namespace MandelbrotSet
{
    public partial class Viewer : Form
    {
        private static double currMaxReal = 2;
        private static double currMinReal = -2;
        private static double currMaxImaginary = 2;
        private static double currMinImaginary = -2;
        private static double zoomLevel = 1.0;
        private static bool isRendering;
        public bool fullScreen = false;

        public Viewer()
        {
            try
            {
                InitializeComponent();
                Load += (s, e) => RenderMandelbrot();
                ResizeEnd += (s, e) => RenderMandelbrot();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public int Dimension
        {
            get { return Math.Min(Width, Height); }
        }

        public double DDimension
        {
            get { return Math.Min(Width, Height); }
        }

        private void RenderMandelbrot(double? maxr = null, double? minr = null, double? maxi = null, double? mini = null)
        {
            if (Width != Height)
            {
                SetBounds(0, 0, Dimension, Dimension, BoundsSpecified.Size);
            }

            if (isRendering)
            {
                return;
            }

            currMaxReal = maxr ?? currMaxReal;
            currMinReal = minr ?? currMinReal;
            currMaxImaginary = maxi ?? currMaxImaginary;
            currMinImaginary = mini ?? currMinImaginary;

            lblStatus.Text = "Currently Rendering";
            isRendering = true;

            Program.Benchmark("Mandelbrot", async () =>
            {
                var render = await Generator.MandelbrotSet(new Bitmap(Dimension, Dimension), currMaxReal, currMinReal, currMaxImaginary, currMinImaginary, zoomLevel).ConfigureAwait(false);
                SafeInvoke(() =>
                {
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = render;
                    isRendering = false;
                });
            },
              (tag, benchmark) =>
                  SafeInvoke(() => lblStatus.Text = (string.Format("Rendered {0} in {1}ms | {2} {3} {4} {5}", tag, benchmark.Milliseconds, currMaxReal, currMinReal, currMaxImaginary, currMinImaginary))));
        }

        public void SafeInvoke(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(action));
            }
            else
            {
                action();
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            var ex = e.X;
            var ey = e.Y;
            var currentxjump = ((currMaxReal - currMinReal) / Dimension);
            var currentyjump = ((currMaxImaginary - currMinImaginary) / Dimension);

            var zoomx = 0.0;
            var zoomy = 0.0;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    zoomx = DDimension / 5.0;
                    zoomy = DDimension / 5.0;
                    zoomLevel++;
                    break;

                case MouseButtons.Right:
                    zoomx = DDimension;
                    zoomy = DDimension;
                    zoomLevel--;
                    break;
            }
            RenderMandelbrot(((ex + zoomx) * currentxjump) - Math.Abs(currMinReal), ((ex - zoomx) * currentxjump) - Math.Abs(currMinReal), ((ey + zoomy) * currentyjump) - Math.Abs(currMinImaginary),
                             ((ey - zoomy) * currentyjump) - Math.Abs(currMinImaginary));
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && e.Clicks == 2)
            {
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        { }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    const double inc = 2.0;
                    currMaxImaginary = inc;
                    currMinImaginary = -inc;
                    currMaxReal = inc;
                    currMinReal = -inc;
                    zoomLevel = 1;
                    break;

                case Keys.Enter:
                    currMaxReal -= currMaxReal / 20.0;
                    currMinReal -= currMinReal / 20.0;
                    currMaxImaginary -= currMaxImaginary / 20.0;
                    currMinImaginary -= currMinImaginary / 20.0;
                    zoomLevel++;
                    break;

                case Keys.F8:
                    if (!fullScreen)
                    {
                        fullScreen = true;
                        FormBorderStyle = FormBorderStyle.None;
                        RenderMandelbrot();
                    }
                    else
                    {
                        fullScreen = false;
                        FormBorderStyle = FormBorderStyle.None;
                        RenderMandelbrot();
                    }
                    return;

                default:
                    return;
            }

            RenderMandelbrot();
        }
    }
}
