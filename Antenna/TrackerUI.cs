using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.Antenna
{
    public partial class TrackerUI : UserControl, IDeactivate, IActivate
    {
        public TrackerGeneric TrackerGeneric { get; }

        public TrackerUI()
        {
            InitializeComponent();

            TrackerGeneric = new TrackerGeneric(this);
        }

        public void Deactivate()
        {
            TrackerGeneric.Deactivate();
        }

        public void Activate()
        {
            TrackerGeneric.Activate();
        }
    }
}