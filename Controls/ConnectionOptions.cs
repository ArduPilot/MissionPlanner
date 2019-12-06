using System;
using System.Windows.Forms;
using MissionPlanner.Comms;
using System.Threading;
using MissionPlanner.Utilities;

namespace MissionPlanner
{
    public partial class ConnectionOptions : Form
    {
        // Thread signal. 
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        public ConnectionOptions()
        {
            InitializeComponent();

            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            CMB_serialport.Items.Add("TCP");
            CMB_serialport.Items.Add("UDP");
            CMB_serialport.Items.Add("UDPCl");

            ThemeManager.ApplyThemeTo(this);

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            var mav = new MAVLinkInterface();

            try
            {
                MainV2.instance.doConnect(mav, CMB_serialport.Text, CMB_baudrate.Text);

                MainV2.Comports.Add(mav);

                MainV2._connectionControl.UpdateSysIDS();
            }
            catch (Exception)
            {
            }
        }
    }
}