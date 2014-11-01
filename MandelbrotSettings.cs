using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelbrotSet
{
    public class MandelbrotSettings : INotifyPropertyChanged
    {
        private double _MaxReal = 0.5;
        private double _MinReal = -2;
        private double _MaxImaginary  = 1;
        private double _MinImaginary  = -1;
        private double _ZoomLevel = 1.0;
        private double _Saturation = 0.8;
        private double _Value = 1.0;
        private int _ColourAlgorithm = 1;
        private bool _LerpColours = true;
        private double _LerpStep = 1.0;
        public double MaxReal
        {
            get { return _MaxReal; }
            set { _MaxReal = FirePropertyChanged("MaxReal", value); }
        }
        public double MinReal
        {
            get { return _MinReal; }
            set { _MinReal = FirePropertyChanged("MinReal", value); }
        }
        public double MaxImaginary
        {
            get { return _MaxImaginary; }
            set { _MaxImaginary = FirePropertyChanged("MaxImaginary", value); }
        }
        public double MinImaginary
        {
            get { return _MinImaginary; }
            set { _MinImaginary = FirePropertyChanged("MinImaginary", value); }
        }
        public double ZoomLevel
        {
            get { return _ZoomLevel; }
            set { _ZoomLevel = FirePropertyChanged("ZoomLevel", value); }
        }

        public double Saturation
        {
            get { return _Saturation; }
            set { _Saturation = FirePropertyChanged("Saturation", value); }
        }

        public double Value
        {
            get { return _Value; }
            set { _Value = FirePropertyChanged("Value", value); }
        }

        public bool LerpColours
        {
            get { return _LerpColours; }
            set { _LerpColours = FirePropertyChanged("LerpColours", value); }
        }

        public double LerpStep
        {
            get { return _LerpStep; }
            set { _LerpStep = FirePropertyChanged("LerpStep", value); }
        }

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
