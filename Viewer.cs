using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MandelbrotSet
{
    public partial class Viewer : Form
    {
        private static bool isRendering;
        public bool fullScreen = false;

        private SettingsForm settingsForm;

        private MandelbrotSettings Settings
        {
            get
            {
                return settingsForm.Settings;
            }
        }

        public Viewer()
        {
            try
            {
                InitializeComponent();
                settingsForm = new SettingsForm(this);
                Settings.PropertyChanged += Settings_PropertyChanged;
                Load += (s, e) => RenderMandelbrot();
                SizeChanged += (s, e) => RenderMandelbrot();
                Shown += (s, e) => settingsForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RenderMandelbrot();
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
            if (isRendering)
            {
                return;
            }
            pictureBox1.SetBounds(0, 0, Width, Height, BoundsSpecified.All);
            Settings.MaxReal = maxr ?? Settings.MaxReal;
            Settings.MinReal = minr ?? Settings.MinReal;
            Settings.MaxImaginary = maxi ?? Settings.MaxImaginary;
            Settings.MinImaginary = mini ?? Settings.MinImaginary;

            lblStatus.Text = "Currently Rendering";
            isRendering = true;

            Program.Benchmark("Mandelbrot", async () =>
            {
                var render = await Generator.MandelbrotSet(new Bitmap(Dimension, Dimension), Settings).ConfigureAwait(false);
                SafeInvoke(() =>
                {
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = render;
                    isRendering = false;
                });
            },
              (tag, benchmark) =>
                  SafeInvoke(() => lblStatus.Text = (string.Format("Rendered {0} in {1}ms | {2} {3} {4} {5}", tag, benchmark.Milliseconds, Settings.MaxReal, Settings.MinReal, Settings.MaxImaginary, Settings.MinImaginary))));
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
            var currentxjump = ((Settings.MaxReal - Settings.MinReal) / Width);
            var currentyjump = ((Settings.MaxImaginary - Settings.MinImaginary) / Height);

            var zoomx = 0.0;
            var zoomy = 0.0;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    zoomx = Width / 5.0;
                    zoomy = Height / 5.0;
                    Settings.ZoomLevel *= 1.1;
                    break;

                case MouseButtons.Right:
                    zoomx = Width;
                    zoomy = Height;
                    Settings.ZoomLevel /= 1.1;
                    break;
            }
            RenderMandelbrot(((ex + zoomx) * currentxjump) - Math.Abs(Settings.MinReal), ((ex - zoomx) * currentxjump) - Math.Abs(Settings.MinReal), ((ey + zoomy) * currentyjump) - Math.Abs(Settings.MinImaginary),
                             ((ey - zoomy) * currentyjump) - Math.Abs(Settings.MinImaginary));
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
                    Settings.MaxImaginary = 1.0;
                    Settings.MinImaginary = -1.0;
                    Settings.MaxReal = 0.5;
                    Settings.MinReal = -2;
                    Settings.ZoomLevel = 1;
                    break;

                case Keys.Enter:
                    Settings.MaxReal -= Settings.MaxReal / 20.0;
                    Settings.MinReal -= Settings.MinReal / 20.0;
                    Settings.MaxImaginary -= Settings.MaxImaginary / 20.0;
                    Settings.MinImaginary -= Settings.MinImaginary / 20.0;
                    Settings.ZoomLevel /= 1.1;
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
