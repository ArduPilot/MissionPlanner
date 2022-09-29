using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class OpenDroneID_Map_Status : UserControl
    {

        public static OpenDroneID_Map_Status Instance;
        public OpenDroneID_UI _parent_ODID { get; set; } = null;

        public OpenDroneID_Map_Status()
        {
            Instance = this;
            

            InitializeComponent();
        }

        public void setStatusOK()
        {
            LED_ODID_Status.Color = Color.Green;
            LBL_ODID_OK.Text = "Remote ID OK";
            LBL_ODID_reason.Text = "DBL CLK -> Emergency";
        }

        public void setStatusAlert(string alertReason)
        {
            LED_ODID_Status.Color = Color.Red;
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
                try
                {
                    if (_parent_ODID != null)
                     _parent_ODID.setEmergencyFromMap();
                } catch
                {
                    Console.WriteLine("ODID - Error setting Emergency from Map");
                }
            }
        }

    }

    
}
