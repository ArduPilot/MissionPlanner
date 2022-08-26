using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Microsoft.Scripting.Hosting.Shell.ConsoleHostOptions;

namespace MissionPlanner.Controls
{
    public partial class OpenDroneID_Map_Status : UserControl
    {

        public static OpenDroneID_Map_Status Instance;

        public OpenDroneID_Map_Status()
        {
            Instance = this;

            InitializeComponent();
        }

        public void setStatusOK()
        {
            LED_ODID_Status.Color = Color.Green;
            //LED_ODID_Status.Blink(0);
            LBL_ODID_OK.Text = "Remote ID OK";
            LBL_ODID_reason.Text = "DBL CLK -> Emergency";
        }

        public void setStatusAlert(string alertReason)
        {
            LED_ODID_Status.Color = Color.Red;
            //LED_ODID_Status.Blink(0);
            LBL_ODID_OK.Text = "Remote ID Fail";
            LBL_ODID_reason.Text = alertReason;
        }

        public void setStatusEmergency(string alertReason)
        {
            LED_ODID_Status.Color = Color.Red;
            //LED_ODID_Status.Blink(200); 
            LBL_ODID_OK.Text = "RID Emergency";
            LBL_ODID_reason.Text = alertReason;
        }

        private void OpenDroneID_Map_Status_DoubleClick(object sender, EventArgs e)
        {
            if (CustomMessageBox.Show("Are you sure you want to declare Remote ID Emergency?", "RID Emergency?", CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
            {
                MainV2.instance.FlightData.openDroneID_UI1.setEmergencyFromMap();
            }
        }

        private void LBL_ODID_reason_Click(object sender, EventArgs e)
        {
            OpenDroneID_Map_Status_DoubleClick(sender, e); 
        }

        private void LBL_ODID_OK_Click(object sender, EventArgs e)
        {
            OpenDroneID_Map_Status_DoubleClick(sender, e);
        }

        private void LED_ODID_Status_Click(object sender, EventArgs e)
        {
            OpenDroneID_Map_Status_DoubleClick(sender, e);
        }
    }

    
}
