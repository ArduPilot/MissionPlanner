using System.ComponentModel;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigPlannerAdv : MyUserControl, IActivate
    {
        public ConfigPlannerAdv()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            Params.Rows.Clear();

            foreach (var item in Settings.Instance.Keys)
            {
                var rowno = Params.Rows.Add();

                Params.Rows[rowno].Cells[0].Value = item.ToString();
                Params.Rows[rowno].Cells[1].Value = Settings.Instance[item];
            }

            Params.Sort(Params.Columns[0], ListSortDirection.Ascending);
        }
    }
}