using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigFirmwareDisabled : UserControl, IActivate
    {
        public ConfigFirmwareDisabled()
        {
            InitializeComponent();
            Utilities.ThemeManager.ApplyThemeTo(this);
        }

        public void Activate()
        {
        }
    }
}