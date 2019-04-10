using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigGPSInject : MyUserControl, IActivate, IDeactivate
    {
        public ConfigGPSInject()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            gps.Activate();
        }

        public void Deactivate()
        {
            gps.Deactivate();
        }
    }
}