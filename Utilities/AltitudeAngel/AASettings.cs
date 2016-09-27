using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Windows.Forms;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    public partial class AASettings : Form
    {
        public AASettings()
        {
            InitializeComponent();

            // load settings
            chk_grounddata.Checked = Utilities.AltitudeAngel.AltitudeAngel.service.GroundDataDisplay;
            chk_airdata.Checked = Utilities.AltitudeAngel.AltitudeAngel.service.AirDataDisplay;
        }

        private void but_enable_Click(object sender, EventArgs e)
        {
            if (Utilities.AltitudeAngel.AltitudeAngel.service.IsSignedIn)
            {
                CustomMessageBox.Show("You are already signed in", "AltitudeAngel");
                return;
            }

            Utilities.AltitudeAngel.AltitudeAngel.service.SignInAsync();
        }

        private void but_disable_Click(object sender, EventArgs e)
        {
            Utilities.AltitudeAngel.AltitudeAngel.service.DisconnectAsync();
        }

        private void chk_airdata_CheckedChanged(object sender, EventArgs e)
        {
            Utilities.AltitudeAngel.AltitudeAngel.service.AirDataDisplay = chk_airdata.Checked;
        }

        private void chk_grounddata_CheckedChanged(object sender, EventArgs e)
        {
            Utilities.AltitudeAngel.AltitudeAngel.service.GroundDataDisplay = chk_grounddata.Checked;
        }
    }
}
