using System.ComponentModel;
using System.Drawing.Design;

using MandelbrotSet.UI.Tools;

namespace MandelbrotSet
{
    [DefaultProperty("ColourAlgorithm")]
    public class MandelbrotSettings : INotifyPropertyChanged
    {
        private int _ColourAlgorithm = 1;
        private bool _FlamingShip;
        private bool _LerpColours = true;
        private double _LerpStep = 1.0;
        private double _MaxImaginary = 1;
        private double _MaxReal = 0.5;
        private double _MinImaginary = -1;
        private double _MinReal = -2;
        private double _Saturation = 0.8;
        private double _Value = 1.0;
        private double _ZoomLevel = 1.0;

        [Category("Bounds")]
        [Description("The maximum value for the real component of Complex numbers.")]
        public double MaxReal
        {
            get { return _MaxReal; }
            set { _MaxReal = FirePropertyChanged("MaxReal", value); }
        }

        [Category("Bounds")]
        [Description("The minimum value for the real component of Complex numbers.")]
        public double MinReal
        {
            get { return _MinReal; }
            set { _MinReal = FirePropertyChanged("MinReal", value); }
        }

        [Category("Bounds")]
        [Description("The maximum value for the imaginary component of Complex numbers.")]
        public double MaxImaginary
        {
            get { return _MaxImaginary; }
            set { _MaxImaginary = FirePropertyChanged("MaxImaginary", value); }
        }

        [Category("Bounds")]
        [Description("The minimum value for the imaginary component of Complex numbers.")]
        public double MinImaginary
        {
            get { return _MinImaginary; }
            set { _MinImaginary = FirePropertyChanged("MinImaginary", value); }
        }

        [Category("Bounds")]
        [Description("The current zoom level of the Mandelbrot Set.")]
        [ReadOnly(true)]
        public double ZoomLevel
        {
            get { return _ZoomLevel; }
            set { _ZoomLevel = FirePropertyChanged("ZoomLevel", value); }
        }

        [Category("Configuration")]
        [Description("The rendering Saturation (HSV).")]
        public double Saturation
        {
            get { return _Saturation; }
            set { _Saturation = FirePropertyChanged("Saturation", value); }
        }

        [Category("Configuration")]
        [Description("The rendering Value (HSV).")]
        public double Value
        {
            get { return _Value; }
            set { _Value = FirePropertyChanged("Value", value); }
        }

        [Category("Configuration")]
        [Description("Use linear interpolation while rendering.")]
        public bool LerpColours
        {
            get { return _LerpColours; }
            set { _LerpColours = FirePropertyChanged("LerpColours", value); }
        }

        [Category("Configuration")]
        [Description("Use linear interpolation while rendering.")]
        public bool FlamingShip
        {
            get { return _FlamingShip; }
            set { _FlamingShip = FirePropertyChanged("FlamingShip", value); }
        }

        [Category("Configuration")]
        [Description("The linear interpolation step value to use while rendering.")]
        public double LerpStep
        {
            get { return _LerpStep; }
            set { _LerpStep = FirePropertyChanged("LerpStep", value); }
        }

        [Category("Configuration")]
        [Description("The rendering algorithm to use.")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor))]
        [MinMax(1, 8)]
        public int ColourAlgorithm
        {
            get { return _ColourAlgorithm; }
            set { _ColourAlgorithm = FirePropertyChanged("ColourAlgorithm", value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler UpdateSettings;

        public T FirePropertyChanged<T>(string propertyName, T value)
        {
            var updateHandler = UpdateSettings;
            if (updateHandler != null)
            {
                updateHandler(this, new PropertyChangedEventArgs(propertyName));
            }
            return value;
        }

        public virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
