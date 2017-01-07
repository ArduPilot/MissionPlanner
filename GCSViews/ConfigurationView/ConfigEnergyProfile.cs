using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigEnergyProfile : UserControl, IActivate
    {
        public ConfigEnergyProfile()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            //throw new NotImplementedException();
        }

        private void TXT_ParamA_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Btn_SaveChanges_Click(object sender, EventArgs e)
        {
            Settings.Instance["EP_Current"] = TXT_IM90Deg.Text + "|" + TXT_IM45Deg.Text + "|" + TXT_I0Deg.Text + "|" + TXT_IP45Deg.Text + "|" + TXT_IP90Deg.Text;   //Settings for I-function
            Settings.Instance["EP_Velocity"] = TXT_VM90Deg.Text + "|" + TXT_VM45Deg.Text + "|" + TXT_V0Deg.Text + "|" + TXT_VP45Deg.Text + "|" + TXT_VP90Deg.Text;  //Settings for V-function
        }
    }
}
