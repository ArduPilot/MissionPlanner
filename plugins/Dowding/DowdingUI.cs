using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace Dowding
{
    public partial class DowdingUI : MyUserControl, IActivate
    {
        private Crypto crypto = new Crypto();

        public DowdingUI()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            txt_username.Text = Settings.Instance["Dowding_username"];
            txt_password.Text = crypto.DecryptString(Settings.Instance["Dowding_password"]);
            cmb_server.Text = Settings.Instance["Dowding_server"];
            chk_enable.Checked = Settings.Instance.GetBoolean("Dowding_enabled", false);


            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            CMB_serialport.Items.Add("TCP Host - 14551");
            CMB_serialport.Items.Add("TCP Client");
            CMB_serialport.Items.Add("UDP Host - 14551");
            CMB_serialport.Items.Add("UDP Client");

            if (threadrun)
            {
                BUT_connect.Text = Strings.Stop;
            }

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private async void but_verify_Click(object sender, EventArgs e)
        {
            try
            {
                await new MissionPlanner.WebAPIs.Dowding().Auth(txt_username.Text, txt_password.Text, cmb_server.Text);

                Settings.Instance["Dowding_username"] = txt_username.Text;
                Settings.Instance["Dowding_password"] = crypto.EncryptString(txt_password.Text);
                Settings.Instance["Dowding_server"] = cmb_server.Text;
                Settings.Instance.Save();

                CustomMessageBox.Show("Verified!");
            }
            catch
            {
                CustomMessageBox.Show("Username or password invalid");
            }
        }

        private void chk_enable_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["Dowding_enabled"] = chk_enable.Checked.ToString();
        }

        private void but_token_Click(object sender, EventArgs e)
        {
            var token = "";
            if (InputBox.Show("Token", "Enter your token", ref token) == DialogResult.OK)
            {
                Settings.Instance["Dowding_token"] = token;
            }
        }

        private void but_start_Click(object sender, EventArgs e)
        {
            DowdingPlugin.Start();
        }

        static TcpListener listener;
        static ICommsSerial ATStream = new TcpSerial();
        System.Threading.Thread t12;
        static bool threadrun = false;
        static internal PointLatLngAlt HomeLoc = new PointLatLngAlt(0, 0, 0, "Home");
        private MAVLink.MavlinkParse mavlink;
        private int sequence = 0;
        private DateTime starttime = DateTime.Now;

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }

            if (ATStream.IsOpen)
            {
                threadrun = false;
                ATStream.Close();
                BUT_connect.Text = Strings.Connect;
            }
            else
            {
                try
                {
                    switch (CMB_serialport.Text)
                    {
                        case "TCP Host - 14551":
                        case "TCP Host":
                            ATStream = new TcpSerial();
                            CMB_baudrate.SelectedIndex = 0;
                            listener = new TcpListener(System.Net.IPAddress.Any, 14551);
                            listener.Start(0);
                            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
                            BUT_connect.Text = Strings.Stop;
                            break;
                        case "TCP Client":
                            ATStream = new TcpSerial() { retrys = 999999, autoReconnect = true };
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "UDP Host - 14551":
                            ATStream = new UdpSerial();
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "UDP Client":
                            ATStream = new UdpSerialConnect();
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        default:
                            ATStream = new SerialPort();
                            ATStream.PortName = CMB_serialport.Text;
                            break;
                    }
                }
                catch
                {
                    CustomMessageBox.Show(Strings.InvalidPortName);
                    return;
                }
                try
                {
                    ATStream.BaudRate = int.Parse(CMB_baudrate.Text);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.InvalidBaudRate);
                    return;
                }
                try
                {
                    if (listener == null)
                        ATStream.Open();
                }
                catch
                {
                    CustomMessageBox.Show("Error Connecting\nif using com0com please rename the ports to COM??");
                    return;
                }

                t12 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
                {
                    IsBackground = true,
                    Name = "AT output"
                };
                t12.Start();

                BUT_connect.Text = Strings.Stop;
            }
        }

        void mainloop()
        {
            threadrun = true;

            DowdingPlugin.UpdateOutput += DowdingPlugin_UpdateOutput;

            mavlink = new MAVLink.MavlinkParse();
            starttime = DateTime.Now;

            SendHome();

            while (threadrun)
            {
                Thread.Sleep(500);
            }

            DowdingPlugin.UpdateOutput -= DowdingPlugin_UpdateOutput;
        }

        private void SendHome()
        {
            var cl = new MAVLink.mavlink_command_long_t(0, 0, 0, 0, (float) (HomeLoc.Lat * 1e7),
                (float) (HomeLoc.Lng * 1e7),
                (float) (HomeLoc.Alt * 1e2),
                (ushort) MAVLink.MAV_CMD.DO_SET_HOME, 1, 1, 1);

            var home = mavlink.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cl, 255, 190, sequence++);

            ATStream.Write(home, 0, home.Length);
        }

        private void DowdingPlugin_UpdateOutput(object sender, PointLatLngAlt e)
        {
            var gpi = new MAVLink.mavlink_global_position_int_t((uint)(DateTime.Now - starttime).TotalMilliseconds, (int)(e.Lat * 1e7), (int)(e.Lng * 1e7), (int)(e.Alt * 1e2), 0, 0, 0, 0, 0);
            var packet = mavlink.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT, gpi, 255, 190, sequence++);
            ATStream.Write(packet, 0, packet.Length);

            if (sequence % 10 == 0)
                SendHome();
        }

        void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;

            try
            {
                // End the operation and display the received data on  
                // the console.
                TcpClient client = listener.EndAcceptTcpClient(ar);

                ((TcpSerial)ATStream).client = client;

                listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
            }
            catch { }
        }
    }
}
