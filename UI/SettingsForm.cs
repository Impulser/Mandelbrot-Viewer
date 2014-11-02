using System.ComponentModel;
using System.Windows.Forms;

namespace MandelbrotSet
{
    public partial class SettingsForm : Form
    {
        public SettingsForm(Viewer viewerForm)
        {
            ViewerForm = viewerForm;
            InitializeComponent();
            Settings = new MandelbrotSettings();
            propertyGrid1.SelectedObject = Settings;
            Settings.UpdateSettings += Settings_UpdateSettings;
            //Load += (sender, args) => Application.AddMessageFilter(propertyGrid1);
        }

        public MandelbrotSettings Settings { get; private set; }

        public Viewer ViewerForm { get; private set; }

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
