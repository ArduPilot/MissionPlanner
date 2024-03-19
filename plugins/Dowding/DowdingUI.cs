using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using com;
using Dowding.Model;
using GMap.NET;
using MissionPlanner;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.CoT;
using Onvif;

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
            try
            {
                txt_password.Text = Settings.Instance["Dowding_password"] == null
                    ? ""
                    : crypto.DecryptString(Settings.Instance["Dowding_password"]);
            }
            catch
            {

            }

            cmb_server.Text = Settings.Instance["Dowding_server"];
            chk_enable.Checked = Settings.Instance.GetBoolean("Dowding_enabled", false);

            txt_onvifip.Text = Settings.Instance["Dowding_onvifipport"];
            txt_onvifuser.Text = Settings.Instance["Dowding_onvifusername"];
            txt_onvifpassword.Text = Settings.Instance["Dowding_onvifpassword"];

            txt_trackerlat.Text = Settings.Instance["Dowding_trackerlat"];
            txt_trackerlong.Text = Settings.Instance["Dowding_trackerlng"];
            txt_trackerhae.Text = Settings.Instance["Dowding_trackerhae"];

            HomeLoc.Lat = Settings.Instance.GetDouble("Dowding_trackerlat");
            HomeLoc.Lng = Settings.Instance.GetDouble("Dowding_trackerlng");
            HomeLoc.Alt = Settings.Instance.GetDouble("Dowding_trackerhae");

            CMB_serialport.Items.Clear();
            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            CMB_serialport.Items.Add("TCP Host - 14551");
            CMB_serialport.Items.Add("TCP Client");
            CMB_serialport.Items.Add("UDP Host - 14551");
            CMB_serialport.Items.Add("UDP Client");


            
            cmb_cotport.Items.Clear();
            cmb_cotport.Items.AddRange(SerialPort.GetPortNames());
            cmb_cotport.Items.Add("TCP Host");
            cmb_cotport.Items.Add("TCP Client");
            cmb_cotport.Items.Add("UDP Host");
            cmb_cotport.Items.Add("UDP Client");


            if (ATthreadrun)
            {
                BUT_connect.Text = Strings.Stop;
            }

            if (DowdingPlugin.IsAlive)
            {
                but_start.Text = Strings.Stop;
            }

            if (onvifthreadrun)
            {
                but_onvif.Text = Strings.Stop;
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

        private async void but_start_Click(object sender, EventArgs e)
        {
            if (DowdingPlugin.IsAlive)
                DowdingPlugin.Stop();
            else
               await DowdingPlugin.Start();

            Activate();
        }

        static TcpListener listener;
        static ICommsSerial ATStream = new TcpSerial();
        static System.Threading.Thread ATThread;
        static bool ATthreadrun = false;
        static internal PointLatLngAlt HomeLoc = new PointLatLngAlt(0, 0, 0, "Home");
        private static MAVLinkInterface mavlink;
        private static DateTime starttime = DateTime.Now;
        static PointLatLngAlt lastplla;
        private static OnvifDevice device;
        static bool onvifthreadrun;

        
        static TcpListener listenerCoT;
        static ICommsSerial CoTStream = new TcpSerial();
        static System.Threading.Thread CoTThread;
        static bool CoTthreadrun = false;

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }

            if (ATStream.IsOpen)
            {
                ATthreadrun = false;
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
                            ATStream = new TcpSerial() {retrys = 999999, autoReconnect = true, ConfigRef = "DowdingATTCP" };
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

                ATThread = new System.Threading.Thread(new System.Threading.ThreadStart(ATmainloop))
                {
                    IsBackground = true,
                    Name = "AT output"
                };
                ATThread.Start();

                BUT_connect.Text = Strings.Stop;
            }
        }

        static void ATmainloop()
        {
            ATthreadrun = true;

            DowdingPlugin.UpdateOutput += DowdingPlugin_UpdateOutput;

            try
            {
                mavlink = new MAVLinkInterface();
                mavlink.BaseStream = ATStream;
                mavlink.getHeartBeat();
            }
            catch
            {
                ATthreadrun = false;
                CustomMessageBox.Show("Failed to detect antenna tracker");
            }

            starttime = DateTime.Now;

            while (ATthreadrun)
            {
                try
                {
                    while (mavlink.giveComport)
                        Thread.Sleep(2);

                    mavlink.readPacket();
                }
                catch
                {
                    Thread.Sleep(2);
                }
            }

            DowdingPlugin.UpdateOutput -= DowdingPlugin_UpdateOutput;
        }

        private async Task SendHome()
        {
            if (!mavlink.BaseStream.IsOpen)
                return;

            await mav_mission.uploadPartial(mavlink, 2, 1, MAVLink.MAV_MISSION_TYPE.MISSION,
                new List<Locationwp>()
                {
                    new Locationwp()
                    {
                        alt = (float)HomeLoc.Alt, frame = (byte)MAVLink.MAV_FRAME.GLOBAL, lat = HomeLoc.Lat, lng = HomeLoc.Lng,
                        id = (ushort)MAVLink.MAV_CMD.WAYPOINT
                    }
                }, 0);
        }

        private static void DowdingPlugin_UpdateOutput(object sender, PointLatLngAlt e)
        {
            if (e == lastplla)
                return;

            lastplla = e;

            if (!mavlink.BaseStream.IsOpen)
                return;

            var gpi = new MAVLink.mavlink_global_position_int_t((uint) (DateTime.Now - starttime).TotalMilliseconds,
                (int) (e.Lat * 1e7), (int) (e.Lng * 1e7), (int) (e.Alt * 1e3), (int) (e.Alt * 1e3 - HomeLoc.Alt), 0, 0,
                0, 0);

            mavlink.generatePacket((int) MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT, gpi, 2, 1);
        }

        void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener) ar.AsyncState;

            try
            {
                // End the operation and display the received data on  
                // the console.
                TcpClient client = listener.EndAcceptTcpClient(ar);

                ((TcpSerial) ATStream).client = client;

                listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
            }
            catch
            {
            }
        }

        private void but_onvif_Click(object sender, EventArgs e)
        {
            if (onvifthreadrun)
            {
                onvifthreadrun = false;
                return;
            }

            var ip = "127.0.0.1";
            var port = 80;
            var ipport = txt_onvifip.Text;
            var user = txt_onvifuser.Text;
            var password = txt_onvifpassword.Text;

            Settings.Instance["Dowding_onvifipport"] = ipport;
            Settings.Instance["Dowding_onvifusername"] = user;
            Settings.Instance["Dowding_onvifpassword"] = password;
            Settings.Instance.Save();

            if (ipport.Contains(":"))
            {
                ip = ipport.Split(':')[0];
                port = int.Parse(ipport.Split(':')[1]);
            }
            else
            {
                ip = ipport;
            }

            Task.Run(async () =>
            {
                try
                {
                    device = await new OnvifDevice(ip, port, user, password).Setup().ConfigureAwait(false);
                    onvifthreadrun = true;

                    DowdingPlugin.UpdateOutput += OnvifUpdate;

                    while (onvifthreadrun)
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show("Failed to connect/send to tracker " + ex.Message, Strings.ERROR);
                    return;
                }
                finally
                {
                    DowdingPlugin.UpdateOutput -= OnvifUpdate;
                }
            });
        }

        private void OnvifUpdate(object sender, PointLatLngAlt e)
        {
            device.YawOffset = Settings.Instance.GetDouble("Dowding_trackeryaw", 0);
            device.SetTrack(HomeLoc,
                e).ConfigureAwait(false);
        }

        private async void but_setathome_Click(object sender, EventArgs e)
        {
            double lat;
            double lng;
            double hae;

            if (!double.TryParse(txt_trackerlat.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out lat))
            {
                CustomMessageBox.Show(Strings.Invalid_Location + " lat", Strings.ERROR);
                return;
            }

            if (!double.TryParse(txt_trackerlong.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out lng))
            {
                CustomMessageBox.Show(Strings.Invalid_Location + " lng", Strings.ERROR);
                return;
            }

            if (!double.TryParse(txt_trackerhae.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out hae))
            {
                CustomMessageBox.Show(Strings.Invalid_Location + " hae", Strings.ERROR);
                return;
            }

            HomeLoc.Lat = lat;
            HomeLoc.Lng = lng;
            HomeLoc.Alt = hae;

            Settings.Instance["Dowding_trackerlat"] = lat.ToString(CultureInfo.InvariantCulture);
            Settings.Instance["Dowding_trackerlng"] = lng.ToString(CultureInfo.InvariantCulture);
            Settings.Instance["Dowding_trackerhae"] = hae.ToString(CultureInfo.InvariantCulture);

            try
            {
                await SendHome();
            }
            catch
            {
                CustomMessageBox.Show("Failed to send home location", Strings.ERROR);
            }
        }

        private void but_cotstart_Click(object sender, EventArgs e)
        {
            if (listenerCoT != null)
            {
                listenerCoT.Stop();
                listenerCoT = null;
            }

            if (CoTStream.IsOpen)
            {
                CoTthreadrun = false;
                CoTStream.Close();
                but_cotstart.Text = Strings.Connect;
            }
            else
            {
                try
                {
                    switch (cmb_cotport.Text)
                    {
                        case "TCP Host - 14551":
                        case "TCP Host":
                            CoTStream = new TcpSerial();
                            int portt = int.Parse(Settings.Instance["cot_tcphostport", "14551"]);
                            InputBox.Show("Enter Port", "Please enter a listen port", ref portt);
                            cmb_cotbaud.SelectedIndex = 0;
                            listenerCoT = new TcpListener(System.Net.IPAddress.Any, portt);
                            listenerCoT.Start(0);
                            listenerCoT.BeginAcceptTcpClient(new AsyncCallback(DoAcceptCoTTcpClientCallback), listenerCoT);
                            break;
                        case "TCP Client":
                            CoTStream = new TcpSerial() {retrys = 999999, autoReconnect = true, ConfigRef = "DowdingCoTTCP" };
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "UDP Host - 14551":
                        case "UDP Host":
                            int portu = int.Parse(Settings.Instance["cot_udphostport", "10011"]);
                            InputBox.Show("Enter Port", "Please enter a listen port", ref portu);
                            Settings.Instance["cot_udphostport"] = portu.ToString();
                            CoTStream = new UdpSerial()
                            {
                                ConfigRef = "cot", Port = portu.ToString(), client = new UdpClient(portu), IsOpen = true
                            };
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "UDP Client":
                            CoTStream = new UdpSerialConnect();
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        default:
                            CoTStream = new SerialPort();
                            CoTStream.PortName = CMB_serialport.Text;
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
                    CoTStream.BaudRate = int.Parse(cmb_cotbaud.Text);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.InvalidBaudRate);
                    return;
                }

                try
                {
                    if (listenerCoT == null)
                        CoTStream.Open();
                }
                catch
                {
                    CustomMessageBox.Show("Error Connecting\nif using com0com please rename the ports to COM??");
                    return;
                }

                CoTThread = new System.Threading.Thread(new System.Threading.ThreadStart(CoTmainloop))
                {
                    IsBackground = true,
                    Name = "CoT output"
                };
                CoTThread.Start();

                but_cotstart.Text = Strings.Stop;
            }
        }

        void DoAcceptCoTTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener) ar.AsyncState;

            try
            {
                // End the operation and display the received data on  
                // the console.
                TcpClient client = listener.EndAcceptTcpClient(ar);

                ((TcpSerial) CoTStream).client = client;

                listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
            }
            catch
            {
            }
        }

        private void CoTmainloop()
        {
            CoTthreadrun = true;

            var stream = new CommsStream(CoTStream, int.MaxValue);

            var lookfor = "<?xml".Select(a=>(byte)a).ToArray();
            var lookforend = "</event>".Select(a=>(byte)a).ToArray();

            while (CoTthreadrun)
            {
                try
                {
                    var sb = new StringBuilder();
                    var buff = new byte[4096];
                    while (CoTthreadrun)
                    {
                        var by = stream.Read(buff, 0, buff.Length);
                        var pos = 0;
                        while (pos < by)
                        {
                            var start = buff.Search(lookfor, pos);
                            var end = buff.Search(lookforend, pos);

                            // entire message
                            if (start >= 0 && end >= 0 && end > start)
                            {
                                var xml = ASCIIEncoding.ASCII.GetString(buff.Skip(start).Take(end + lookforend.Length - start).ToArray());
                                pos = end + lookforend.Length;
                                ProcessCoTMessage(xml);
                            }// start with no end
                            else if (start >= 0 && end == -1)
                            {
                                var partxml = ASCIIEncoding.ASCII.GetString(buff.Skip(start).ToArray());
                                pos = by;
                                sb.Append(partxml);
                            }// no start with end
                            else if (end >= 0)
                            {
                                var partxml = ASCIIEncoding.ASCII.GetString(buff.Take(end + lookforend.Length).ToArray());
                                sb.Append(partxml);
                                ProcessCoTMessage(sb.ToString());
                                sb.Clear();
                                pos = end + lookforend.Length;
                            }// no start or end
                            else
                            {
                                sb.Append(ASCIIEncoding.ASCII.GetString(buff));
                                pos = by;
                            }
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                }
            }
        }

        private XmlSerializer serializer =
            new XmlSerializer(typeof(@event));

        private void ProcessCoTMessage(string xml)
        {
            var data = (@event) serializer.Deserialize(new StringReader(xml));

            var plla = new PointLatLngAlt(
                double.Parse(data.point.lat, CultureInfo.InvariantCulture),
                double.Parse(data.point.lon, CultureInfo.InvariantCulture),
                double.Parse(data.point.hae, CultureInfo.InvariantCulture));

            MissionPlanner.WebAPIs.Dowding.Vehicles[data.uid] = new VehicleTick(Ts:
                ((long)DateTime.Parse(data.time).toUnixTime() * 1000L ), Lat: (decimal) plla.Lat,
                Lon: (decimal) plla.Lng, Hae: (decimal) plla.Alt, CorrelationId: data.uid, AgentId: "",
                ContactId: data.uid, Id: data.uid, Serial: data.uid, Model: data.type, Vendor: "CoT");
        }

        private void num_yawonvif_ValueChanged(object sender, EventArgs e)
        {
            device.YawOffset = (double)num_yawonvif.Value;
            Settings.Instance["Dowding_trackeryaw"] = device.YawOffset.ToString();
        }
    }
}