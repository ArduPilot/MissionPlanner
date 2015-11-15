using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class SysidSelector : Form
    {
        public SysidSelector()
        {
            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            cmb_sysid.DataSource = MainV2.comPort.MAVlist.GetRawIDS();
        }

        private void cmb_sysid_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainV2.comPort.sysidcurrent = (int) cmb_sysid.SelectedValue/256;
            MainV2.comPort.compidcurrent = (int) cmb_sysid.SelectedValue%256;
        }

        private void myButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmb_sysid_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = MainV2.comPort.MAVlist[(int) e.Value/256, (int) e.Value%256].aptype.ToString() + "-" +
                      ((int) e.Value%256);
        }
    }
}