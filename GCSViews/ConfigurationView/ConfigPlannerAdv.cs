using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using ArdupilotMega.Controls;

namespace ArdupilotMega.GCSViews.ConfigurationView
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
                int rowno = Params.Rows.Add();

                Params.Rows[rowno].Cells[0].Value = item.ToString();
                Params.Rows[rowno].Cells[1].Value = MainV2.config[item].ToString();
            }

            Params.Sort(Params.Columns[0], ListSortDirection.Ascending);
        }
    }
}
