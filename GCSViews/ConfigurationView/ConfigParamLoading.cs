using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigParamLoading : UserControl, IActivate, IDeactivate
    {
        public bool gotAllParams
        {
            get
            {
                if (MainV2.comPort.MAV.param.TotalReceived < MainV2.comPort.MAV.param.TotalReported)
                {
                    return false;
                }

                return true;
            }
        }

        public ConfigParamLoading()
        {
            InitializeComponent();

            // L10N: keep upstream typos ("Paramaters"/"untill") in the key - see strings_zhHans.txt
            label1.Text = MissionPlanner.Utilities.L10NU.GetString(
                "Paramaters are still loading. Many screens will not work untill all Parameters are loaded. ",
                "Paramaters are still loading. Many screens will not work untill all Parameters are loaded. ");
            but_forceparams.Text = MissionPlanner.Utilities.L10NU.GetString("Retry Now", "Retry Now");
        }

        public void Activate()
        {
            timer1.Start();
        }

        public void Deactivate()
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (gotAllParams)
                MainV2.View.Reload();
        }

        private void but_forceparams_Click(object sender, EventArgs e)
        {
            MainV2.comPort.getParamList();
        }
    }
}
