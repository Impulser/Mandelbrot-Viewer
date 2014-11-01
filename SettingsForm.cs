using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotSet
{
    public partial class SettingsForm : Form
    {
        public MandelbrotSettings Settings { get; } = new MandelbrotSettings();
        public Viewer ViewerForm { get; private set; }

        public SettingsForm(Viewer viewerForm)
        {
            ViewerForm = viewerForm;
            InitializeComponent();
            propertyGrid1.SelectedObject = Settings;
            Settings.UpdateSettings += Settings_UpdateSettings;
        }

        private void Settings_UpdateSettings(object sender, PropertyChangedEventArgs e)
        {
            propertyGrid1.Refresh();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Settings.OnPropertyChanged(new PropertyChangedEventArgs(e.ChangedItem.Label));
        }
    }
}
