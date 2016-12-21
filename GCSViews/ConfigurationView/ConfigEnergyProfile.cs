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
            Settings.Instance["EP_Current"] = TXT_GradCurNeg.Text + "|" + TXT_BaseHoverCons.Text + "|" + TXT_GradCurPos.Text;
            Settings.Instance["EP_Velocity"] = TXT_SpeedLoss.Text + "|" + TXT_SpeedZeroDeg.Text;
        }
    }
}
