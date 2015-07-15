using System.ComponentModel;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigPlannerAdv : UserControl, IActivate
    {
        public ConfigPlannerAdv()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            Params.Rows.Clear();

            foreach (var item in MainV2.config.Keys)
            {
                var rowno = Params.Rows.Add();

                Params.Rows[rowno].Cells[0].Value = item.ToString();
                Params.Rows[rowno].Cells[1].Value = MainV2.config[item].ToString();
            }

            Params.Sort(Params.Columns[0], ListSortDirection.Ascending);
        }
    }
}