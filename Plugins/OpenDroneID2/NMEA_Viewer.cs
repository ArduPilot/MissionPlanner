using LibVLC.NET;
using MissionPlanner.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner
{
    public partial class NMEA_Viewer : Form
    {
        static NMEA_Viewer Instance;

        public NMEA_Viewer()
        {
            Instance = this;

            InitializeComponent();
        }
        public void update_NMEA_String(string msg)
        {
            
            if (!Instance.IsDisposed)
            {
                Instance.BeginInvoke(
                    (MethodInvoker)
                        delegate
                        {
                            TXT_Data.Text = DateTime.Now.ToString("HH:mm:ss") + " - " + msg + Environment.NewLine + Environment.NewLine + TXT_Data.Text;
                        }
                    );
            }

        }

        public void setLabel(string port)
        {
            LBL_port_txt.Text = "Showing Data from: " + port;
        }

    }
}
