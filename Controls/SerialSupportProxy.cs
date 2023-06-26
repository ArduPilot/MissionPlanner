using MissionPlanner.Comms;
using MissionPlanner.Utilities;
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
    public partial class SerialSupportProxy : Form
    {
        public SerialSupportProxy()
        {
            InitializeComponent();

            // If the mirror stream is open, change the "Connect" button to "Stop"
            if (MainV2.comPort.MirrorStream != null && MainV2.comPort.MirrorStream.IsOpen)
            {
                BUT_connect.Text = Strings.Stop;
                TXT_host.Enabled = false;
                NUM_port.Enabled = false;
            }

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);

            // Load saved host name and port from settings
            TXT_host.Text = Settings.Instance["UDP_host_SerialSupportProxy"] ?? "support.ardupilot.org";
            NUM_port.Value = int.Parse(Settings.Instance["UDP_port_SerialSupportProxy"] ?? "1");

            // Highlight the port for immediate typing
            NUM_port.Select();
            NUM_port.Select(0, 5);

            // Assign AcceptButton so that hitting "enter" starts the connection
            this.AcceptButton = BUT_connect;

        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            // If the mirror stream is open, then the "Stop" button has been clicked.
            // Close the stream and change to "Connect"
            if (MainV2.comPort.MirrorStream != null && MainV2.comPort.MirrorStream.IsOpen)
            {
                MainV2.comPort.MirrorStream.Close();
                BUT_connect.Text = Strings.Connect;
                TXT_host.Enabled = true;
                NUM_port.Enabled = true;
            }
            else
            {
                // The "Connect" button has been clicked. Create connection
                var udpSerial = new UdpSerialConnect() { ConfigRef = "_SerialSupportProxy" };
                MainV2.comPort.MirrorStream = udpSerial;
                MainV2.comPort.MirrorStreamWrite = true;
                try
                {
                    udpSerial.Open(TXT_host.Text, NUM_port.Value.ToString());
                }
                catch
                {
                    CustomMessageBox.Show("Error Connecting");
                    return;
                }

                // Change the button to "Stop" and disable the host and port fields
                BUT_connect.Text = Strings.Stop;
                TXT_host.Enabled = false;
                NUM_port.Enabled = false;
            }
        }
    }
}
