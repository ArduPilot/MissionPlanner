using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigOptional : UserControl, IActivate
    {
        public ConfigOptional()
        {
            InitializeComponent();
            Utilities.ThemeManager.ApplyThemeTo(this);
        }

        public void Activate()
        {
        }
    }
}