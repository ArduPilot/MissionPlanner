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

        public OpenDroneID_Map_Status()
        {
            Instance = this;

            InitializeComponent();
        }

        public void setStatusOK()
        {
            LED_ODID_Status.Color = Color.Green;
            LBL_ODID_OK.Text = "Remote ID OK";
            LBL_ODID_reason.Text = "";
        }

        public void setStatusAlert(string alertReason)
        {
            LED_ODID_Status.Color = Color.Red;
            LBL_ODID_OK.Text = "Remote ID Fail";
            LBL_ODID_reason.Text = alertReason;
        }


    }

    
}
