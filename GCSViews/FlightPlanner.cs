using MissionPlanner.Controls;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews
{
    public partial class FlightPlanner : MyUserControl, IDeactivate, IActivate
    {

        private readonly FlightPlannerBase _flightPlannerBase;

        public FlightPlanner()
        {
            InitializeComponent();

            _flightPlannerBase = new FlightPlannerBase(this);
        }

        public FlightPlannerBase FlightPlannerBase
        {
            get { return _flightPlannerBase; }
        }

        public void Activate()
        {
            _flightPlannerBase.Activate();
        }

        public void Deactivate()
        {
            _flightPlannerBase.Deactivate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var ans =  _flightPlannerBase.ProcessCmdKey(ref msg, keyData);

            if (ans == false)
                ans = base.ProcessCmdKey(ref msg, keyData);

            return ans;
        }

     
    }
}