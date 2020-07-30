using fastJSON;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml;

namespace MissionPlanner.Controls
{
    public partial class SerialOutputCoT : Form
    {
        static TcpListener listener;
        static ICommsSerial CoTStream = new SerialPort();
        static double updaterate = 5;
        System.Threading.Thread t12;
        static bool threadrun = false;
        static internal PointLatLngAlt HomeLoc = new PointLatLngAlt(0, 0, 0, "Home");

        public SerialOutputCoT()
        {
            InitializeComponent();

            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            CMB_serialport.Items.Add("TCP Host - 14551");
            CMB_serialport.Items.Add("TCP Client");
            CMB_serialport.Items.Add("UDP Host - 14551");
            CMB_serialport.Items.Add("UDP Client");

            CMB_updaterate.Text = updaterate + "hz";

            if (threadrun)
            {
                BUT_connect.Text = Strings.Stop;
            }

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void CMB_updaterate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {

            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }

            if (CoTStream.IsOpen)
            {
                threadrun = false;
                CoTStream.Close();
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
                            CoTStream = new TcpSerial();
                            CMB_baudrate.SelectedIndex = 0;
                            listener = new TcpListener(System.Net.IPAddress.Any, 14551);
                            listener.Start(0);
                            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
                            BUT_connect.Text = Strings.Stop;
                            break;
                        case "TCP Client":
                            CoTStream = new TcpSerial() { retrys = 999999, autoReconnect = true };
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "UDP Host - 14551":
                            CoTStream = new UdpSerial();
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
                    CoTStream.BaudRate = int.Parse(CMB_baudrate.Text);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.InvalidBaudRate);
                    return;
                }
                try
                {
                    if (listener == null)
                        CoTStream.Open();
                }
                catch
                {
                    CustomMessageBox.Show("Error Connecting\nif using com0com please rename the ports to COM??");
                    return;
                }

                t12 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
                {
                    IsBackground = true,
                    Name = "CoT output"
                };
                t12.Start();

                BUT_connect.Text = Strings.Stop;
            }
        }


        void mainloop()
        {
            threadrun = true;
            //CoTStream.NewLine = "\r\n";
            int counter = 0;
            while (threadrun)
            {
                try
                {
                    if (!CoTStream.IsOpen)
                    {
                        Thread.Sleep(10);
                        continue;
                    }


                    //const String xmlStr = getXmlString("")



                    /*
                    //GGA
                    double lat = (int)MainV2.comPort.MAV.cs.lat +
                                    ((MainV2.comPort.MAV.cs.lat - (int)MainV2.comPort.MAV.cs.lat) * .6f);
                    double lng = (int)MainV2.comPort.MAV.cs.lng +
                                    ((MainV2.comPort.MAV.cs.lng - (int)MainV2.comPort.MAV.cs.lng) * .6f);
                    string line = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "$GP{0},{1:HHmmss.fff},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},", "GGA",
                        DateTime.Now.ToUniversalTime(), Math.Abs(lat * 100).ToString("0.00000", CultureInfo.InvariantCulture), MainV2.comPort.MAV.cs.lat < 0 ? "S" : "N",
                        Math.Abs(lng * 100).ToString("0.00000", CultureInfo.InvariantCulture), MainV2.comPort.MAV.cs.lng < 0 ? "W" : "E",
                        MainV2.comPort.MAV.cs.gpsstatus >= 3 ? 1 : 0, MainV2.comPort.MAV.cs.satcount,
                        MainV2.comPort.MAV.cs.gpshdop, MainV2.comPort.MAV.cs.altasl, "M", 0, "M", "");

                    string checksum = GetChecksum(line);
                    CoTStream.WriteLine(line + "*" + checksum);

                    //GLL
                    line = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "$GP{0},{1},{2},{3},{4},{5:HHmmss.fff},{6},{7}", "GLL",
                        Math.Abs(lat * 100).ToString("0.00", CultureInfo.InvariantCulture), MainV2.comPort.MAV.cs.lat < 0 ? "S" : "N",
                        Math.Abs(lng * 100).ToString("0.00", CultureInfo.InvariantCulture), MainV2.comPort.MAV.cs.lng < 0 ? "W" : "E",
                        DateTime.Now.ToUniversalTime(), "A", "A");

                    checksum = GetChecksum(line);
                    CoTStream.WriteLine(line + "*" + checksum);

                    //HDG
                    line = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "$GP{0},{1:0.0},{2},{3},{4},{5}", "HDG",
                        MainV2.comPort.MAV.cs.yaw, 0, "E", 0, "E");

                    checksum = GetChecksum(line);
                    CoTStream.WriteLine(line + "*" + checksum);

                    //VTG
                    line = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "$GP{0},{1},{2},{3},{4}", "VTG",
                        MainV2.comPort.MAV.cs.groundcourse.ToString("000"), MainV2.comPort.MAV.cs.yaw.ToString("000"),
                        (MainV2.comPort.MAV.cs.groundspeed * 1.943844).ToString("00.0", CultureInfo.InvariantCulture),
                        (MainV2.comPort.MAV.cs.groundspeed * 3.6).ToString("00.0", CultureInfo.InvariantCulture));

                    checksum = GetChecksum(line);
                    CoTStream.WriteLine(line + "*" + checksum);

                    //RMC
                    line = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "$GP{0},{1:HHmmss.fff},{2},{3},{4},{5},{6},{7},{8},{9:ddMMyy},{10},{11},{12}", "RMC",
                        DateTime.Now.ToUniversalTime(), "A", Math.Abs(lat * 100).ToString("0.00000", CultureInfo.InvariantCulture),
                        MainV2.comPort.MAV.cs.lat < 0 ? "S" : "N", Math.Abs(lng * 100).ToString("0.00000", CultureInfo.InvariantCulture),
                        MainV2.comPort.MAV.cs.lng < 0 ? "W" : "E", (MainV2.comPort.MAV.cs.groundspeed * 1.943844).ToString("0.0", CultureInfo.InvariantCulture),
                        MainV2.comPort.MAV.cs.groundcourse.ToString("0.0", CultureInfo.InvariantCulture), DateTime.Now, 0, "E", "A");

                    checksum = GetChecksum(line);
                    CoTStream.WriteLine(line + "*" + checksum);

                    if (counter % 20 == 0 && HomeLoc.Lat != 0 && HomeLoc.Lng != 0)
                    {
                        line = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                            "$GP{0},{1:HHmmss.fff},{2},{3},{4},{5},{6},{7},", "HOM", DateTime.Now.ToUniversalTime(),
                            Math.Abs(HomeLoc.Lat * 100).ToString("0.00000", CultureInfo.InvariantCulture), HomeLoc.Lat < 0 ? "S" : "N", Math.Abs(HomeLoc.Lng * 100).ToString("0.00000", CultureInfo.InvariantCulture),
                            HomeLoc.Lng < 0 ? "W" : "E", HomeLoc.Alt, "M");

                        checksum = GetChecksum(line);
                        CoTStream.WriteLine(line + "*" + checksum);
                    }

                    line = string.Format(System.Globalization.CultureInfo.InvariantCulture, "$GP{0},{1},{2},{3},", "RPY",
                        MainV2.comPort.MAV.cs.roll.ToString("0.00000", CultureInfo.InvariantCulture), MainV2.comPort.MAV.cs.pitch.ToString("0.00000", CultureInfo.InvariantCulture), MainV2.comPort.MAV.cs.yaw.ToString("0.00000", CultureInfo.InvariantCulture));

                    checksum = GetChecksum(line);
                    CoTStream.WriteLine(line + "*" + checksum);
                    */

                        
                    var nextsend = DateTime.Now.AddMilliseconds(1000 / updaterate);
                    var sleepfor = Math.Min((int)Math.Abs((nextsend - DateTime.Now).TotalMilliseconds), 4000);
                    System.Threading.Thread.Sleep(sleepfor);
                    counter++;
                        
                }
                catch
                {
                }
            }
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

                ((TcpSerial)CoTStream).client = client;

                listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
            }
            catch { }
        }

        String getXmlString(String uid, String type, String how, double lat, double lng, double alt, double course = -1, double speed = -1)
        {
            // Cursor-on-Target spec
            // https://www.mitre.org/sites/default/files/pdf/09_4937.pdf

            // MIL-STD-2525, needed for event->type
            // https://www.jcs.mil/Portals/36/Documents/Doctrine/Other_Pubs/ms_2525d.pdf

            if (uid == null || uid.Length <= 0) {
                uid = "";
            }
            if (type == null || type.Length <= 0) {
                type = "";
            }

            CultureInfo culture = new CultureInfo("en-US");
            culture.NumberFormat.NumberGroupSeparator = "";

            DateTime time = DateTime.Now;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine  ("<?xml version='1.0' encoding='UTF-8' standalone='yes'?>");
            sb.AppendLine  ("<event version=\"2.0\"");

            sb.AppendFormat("    uid=\"{0}\"", uid); sb.AppendLine();
            sb.AppendFormat("    type=\"{0}\"", type); sb.AppendLine();  // CoT spec section 2.3, additional values by MIL-STD-2525

            sb.AppendFormat("    time=\"{0:yyyy-MM-dd}T{0:HH:mm:ss}.00Z\"", time); sb.AppendLine(); // time stamp: when the event was generated
            sb.AppendFormat("    start=\"{0:yyyy-MM-dd}T{0:HH:mm:ss}.00Z\"", time.AddSeconds(-5)); sb.AppendLine(); // starting time when an event should be considered valid
            sb.AppendFormat("    stale=\"{0:yyyy-MM-dd}T{0:HH:mm:ss}.00Z\"", time.AddSeconds(5)); sb.AppendLine(); // ending time when an event should no longer be considered valid

            // See Appendix A
            // where h- means human and m- means machine
            // m-g      == h.gps
            // h-p      == h.pasted
            // m-f      == h.fused
            // m-n      == h.ins
            // m-g-n    == h.ins-gps
            // m-g-d    == h.dgps
            sb.AppendFormat("    how=\"{0}\">", how); sb.AppendLine(); // Gives a hint about how the coordinates were generated


            sb.AppendLine  ("  <detail>");
            if (course >= 0 && course <= 360 && speed >= 0) {
                sb.AppendFormat(culture, "    <track course=\"{0:N2}\" speed=\"{1:N2}\" />", course, speed); sb.AppendLine();
            }
            sb.AppendLine  ("  </detail>");

            // hae = Height above the WGS ellipsoid in meters
            // ce = Circular 1-sigma or decimal a circular area about the point in meters
            // le = Linear 1-sigma error or decimal an attitude range about the point in meters
            sb.AppendFormat(culture, "  <point lat=\"{0:N7}\" lon=\"{1:N7}\" hae=\"{2,5:N2}\" ce=\"1.0\" le=\"1.0\"", lat, lng, alt); sb.AppendLine();
            sb.AppendLine  ("</event>");

            return sb.ToString();
        }

        private void CB_overrides_enable_CheckedChanged(object sender, EventArgs e)
        {
            // this just changed. Set all internal checkboxes to same value
            bool checkedValue = CB_overrides_enable.Checked;

            CB_override_lat.Checked = checkedValue;
            CB_override_lng.Checked = checkedValue;
            CB_override_alt.Checked = checkedValue;
            CB_override_heading.Checked = checkedValue;
            CB_override_speed.Checked = checkedValue;
        }
    }
}
