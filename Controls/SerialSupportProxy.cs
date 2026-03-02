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

            // Load radio buttons from settings
            rad_udp.Checked = Settings.Instance.GetBoolean("SerialSupportProxy_UDP", true);
            rad_tcp.Checked = !rad_udp.Checked;

            // Load saved host name and port from settings
            var settings_prefix = rad_udp.Checked ? "UDP" : "TCP";
            TXT_host.Text = Settings.Instance[settings_prefix + "_host_SerialSupportProxy"] ?? "support.ardupilot.org";
            NUM_port.Value = int.Parse(Settings.Instance[settings_prefix + "_port_SerialSupportProxy"] ?? "1");

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
                ICommsSerial serial;
                if (rad_udp.Checked)
                {
                    serial = new UdpSerialConnect() { ConfigRef = "_SerialSupportProxy" };
                }
                else
                {
                    serial = new TcpSerial()
                    {
                        retrys = 999999,
                        autoReconnect = true,
                        Host = TXT_host.Text,
                        Port = NUM_port.Value.ToString("0"),
                        ConfigRef = "_SerialSupportProxy"
                    };
                }
                MainV2.comPort.MirrorStream = serial;
                MainV2.comPort.MirrorStreamWrite = true;
                try
                {
                    if (rad_udp.Checked)
                    {
                        (serial as UdpSerialConnect).Open(TXT_host.Text, NUM_port.Value.ToString("0"));
                    }
                    else
                    {
                        serial.Open();
                    }
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

                // Save the UDP/TCP selection
                Settings.Instance["SerialSupportProxy_UDP"] = rad_udp.Checked.ToString();
            }
        }
    }
}
