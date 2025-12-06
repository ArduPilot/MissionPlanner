using MissionPlanner.Controls;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class BatteryMonitorGB : MyUserControl
    {
        public BatteryMonitorGB()
        {
            InitializeComponent();
        }

        public Control InnerControl
        {
            get => contentHost.Controls.Count > 0 ? contentHost.Controls[0] : null;
            set
            {
                contentHost.Controls.Clear();
                if (value != null)
                {
                    value.Dock = DockStyle.Fill;
                    contentHost.Controls.Add(value);
                }
            }
        }

        public string GroupTitle
        {
            get => groupBox1.Text;
            set => groupBox1.Text = value;
        }
    }
}

