using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using System.Collections;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigESCCalibration : UserControl, IActivate
    {
        public ConfigESCCalibration()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            bool RcCalmin = true;
            bool RcCalmax = true;
            var copy = new Hashtable((Hashtable) MainV2.comPort.MAV.param);

            foreach (string item in copy.Keys)
            {
                if (item == "RC3_MIN" && MainV2.comPort.MAV.param[item].ToString() == "1100")
                {
                    RcCalmin = false;
                }
                else if (item == "RC3_MAX" && MainV2.comPort.MAV.param[item].ToString() == "1900")
                {
                    RcCalmax = false;
                }
                else if (item == "ESC_CALIBRATION")
                {
                    if (MainV2.comPort.MAV.param[item].ToString() == "2")
                    {
                        buttonStart.Text = "Ready";
                        buttonStart.Enabled = false;
                    }
                    else
                    {
                        buttonStart.Text = "Start";
                    }
                }
            }

            if (!(RcCalmax || RcCalmin))
            {
                labelRC.Visible = true;
                buttonStart.Enabled = false;
            }
            else
            {
                labelRC.Visible = false;
                buttonStart.Enabled = true;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (!MainV2.comPort.setParam("ESC_CALIBRATION", 2))
                {
                    CustomMessageBox.Show("Set param error. Please ensure your version is AC3.3+.");
                    return;
                }
            }
            catch
            {
                CustomMessageBox.Show("Set param error. Please ensure your version is AC3.3+.");
                    return;
            }

            buttonStart.Text = "Ready";
            buttonStart.Enabled = false;
        }
    }
}