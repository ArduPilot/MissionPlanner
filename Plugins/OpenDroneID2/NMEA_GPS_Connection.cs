using MissionPlanner.Utilities;
using MissionPlanner.Comms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using LibVLC.NET;
using MissionPlanner.Controls;

namespace MissionPlanner
{
    public partial class NMEA_GPS_Connection : UserControl
    {

        static TcpListener listener;
        static ICommsSerial comPort = null;
        //static internal PointLatLngAlt lastgotolocation = new PointLatLngAlt(0, 0, 0, "Goto last");
        //static internal PointLatLngAlt gotolocation = new PointLatLngAlt(0, 0, 0, "Goto");


        public double Lat, Lng;
        public double Alt, Alt_WGS84;
        public float hdop;
        int Sats;
        int fix_type;
        public Stopwatch last_gps_msg = new Stopwatch();
        private bool portsAreLoaded = false;
        static NMEA_GPS_Connection Instance;

        public NMEA_GPS_Connection()
        {
            InitializeComponent();
            Instance = this;

            try
            {
                init_com_port_list();
            }
            catch
            {
                Console.WriteLine("Couldn't Init Open DID Form.");
            }
            timer2.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Check GPS
            readNMEAGPS();
        }

        private void init_com_port_list()
        {
            CMB_serialport.Items.Clear();
            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            CMB_serialport.Items.Add("TCP Host - 14551");
            CMB_serialport.Items.Add("TCP Client");
            CMB_serialport.Items.Add("UDP Host - 14551");
            CMB_serialport.Items.Add("UDP Client");
            portsAreLoaded = true;

        }

        private void autoConnectGPS()
        {
            // TODO quick autoconnect after 2 minutes

            if (portsAreLoaded == false || CMB_serialport.SelectedIndex > 0) return;
            init_com_port_list();
            Console.Write("Checking Auto Connect {" + Settings.Instance["moving_gps_com"] + "," + Settings.Instance["moving_gps_baud"] + "}... ");
            try
            {
                if (!String.IsNullOrEmpty(Settings.Instance["moving_gps_com"]) && CMB_serialport.Items.Contains(Settings.Instance["moving_gps_com"]))
                {
                    CMB_serialport.SelectedIndex = CMB_serialport.Items.IndexOf(Settings.Instance["moving_gps_com"]);
                    Console.Write("COM: " + CMB_serialport.Text);
                }
                else
                    return;

                if (!String.IsNullOrEmpty(Settings.Instance["moving_gps_baud"]) && CMB_baudrate.Items.Contains(Settings.Instance["moving_gps_baud"]))
                {
                    CMB_baudrate.SelectedIndex = CMB_baudrate.Items.IndexOf(Settings.Instance["moving_gps_baud"]);
                    Console.Write(" BAUD: " + CMB_baudrate.Text);
                }

                Console.WriteLine();
                timer2.Stop();

                //if (!String.IsNullOrEmpty(Settings.Instance["moving_gps_auto"]) && Settings.Instance["moving_gps_auto"]=="True")
                if (Settings.Instance.GetBoolean("moving_gps_auto"))
                {
                    CB_auto_connect.Checked = true;
                    //doGPSConnect();
                }


            }
            catch
            {
                Console.WriteLine("Auto Connect Failed.");
            }

        }

        private void CB_auto_connect_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_auto_connect.Checked == true && CMB_serialport.SelectedIndex > 0) doGPSConnect();
        }

        private void doGPSConnect()
        {
            if (comPort != null && comPort.IsOpen)
            {
                comPort.Close();
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
                            comPort = new TcpSerial();
                            CMB_baudrate.SelectedIndex = 0;
                            listener = new TcpListener(System.Net.IPAddress.Any, 14551);
                            listener.Start(0);
                            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
                            BUT_connect.Text = Strings.Stop;
                            break;
                        case "TCP Client":
                            comPort = new TcpSerial() { retrys = 999999, autoReconnect = true };
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "UDP Host - 14551":
                            comPort = new UdpSerial();
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "UDP Client":
                            comPort = new UdpSerialConnect();
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        default:
                            comPort = new SerialPort();
                            comPort.PortName = CMB_serialport.Text;
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
                    comPort.BaudRate = int.Parse(CMB_baudrate.Text);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.InvalidBaudRate, Strings.ERROR);
                    return;
                }
                try
                {
                    if (listener == null)
                        comPort.Open();
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(Strings.ErrorConnecting + "\n" + ex.ToString(), Strings.ERROR);
                    return;
                }

                if (comPort != null && comPort.IsOpen)
                    Console.WriteLine("Moving Base COM Port Opened at port " + comPort.PortName);


                Settings.Instance["moving_gps_com"] = CMB_serialport.Text;
                Settings.Instance["moving_gps_baud"] = CMB_baudrate.Text;
                Settings.Instance["moving_gps_auto"] = CB_auto_connect.Checked.ToString();

                timer1.Start();
                last_gps_msg.Start();
                BUT_connect.Text = Strings.Stop;
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

                ((TcpSerial)comPort).client = client;

                listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
            }
            catch { }

        }

        private void readNMEAGPS()
        {
            try // Process Comport Data
            {
                if (comPort != null && comPort.IsOpen)
                {

                    while (comPort.BytesToRead > 0)
                    {
                        string line = comPort.ReadLine();
                        //Console.WriteLine(line); // for debug

                        //string line = string.Format("$GP{0},{1:HHmmss},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},", "GGA", DateTime.Now.ToUniversalTime(), Math.Abs(lat * 100), MainV2.comPort.MAV.cs.lat < 0 ? "S" : "N", Math.Abs(lng * 100), MainV2.comPort.MAV.cs.lng < 0 ? "W" : "E", MainV2.comPort.MAV.cs.gpsstatus, MainV2.comPort.MAV.cs.satcount, MainV2.comPort.MAV.cs.gpshdop, MainV2.comPort.MAV.cs.alt, "M", 0, "M", "");
                        if (line.StartsWith("$GPGGA") || line.StartsWith("$GNGGA")) // 
                        {
                            string[] items = line.Trim().Split(',', '*');

                            if (items[items.Length - 1] != GetChecksum(line.Trim()))
                            {
                                Console.WriteLine("Bad Nmea line " + items[15] + " vs " + GetChecksum(line.Trim()));
                                continue;
                            }

                            if (items[6] == "0")
                            {
                                Console.WriteLine("No Fix");
                                continue;
                            }

                            Lat = double.Parse(items[2], CultureInfo.InvariantCulture) / 100.0;

                            Lat = (int)Lat + ((Lat - (int)Lat) / 0.60);

                            if (items[3] == "S")
                                Lat *= -1;

                            Lng = double.Parse(items[4], CultureInfo.InvariantCulture) / 100.0;

                            Lng = (int)Lng + ((Lng - (int)Lng) / 0.60);

                            if (items[5] == "W")
                                Lng *= -1;

                            Alt = double.Parse(items[9], CultureInfo.InvariantCulture);

                            if (!String.IsNullOrEmpty(items[11]))
                            {
                                Alt_WGS84 = Alt + double.Parse(items[11], CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                Alt_WGS84 = -1.0;
                            }

                            
                            fix_type = (int.Parse(items[6]));

                            last_gps_msg.Restart();
                            udpate_gps_text();
                        }



                    }
                }
                else
                {
                    BUT_connect.Text = Strings.Connect;
                }
            }
            catch
            {
                Console.WriteLine("Error Processing NMEA Data for Moving Base.");
            }
        }

        private void udpate_gps_text()
        {
            if (!Instance.IsDisposed)
            {
                Instance.BeginInvoke(
                    (MethodInvoker)
                        delegate
                        {
                            Instance.LBL_gpsStatus.Text = String.Format("{0:0.00000}", Lat) + " " + String.Format("{0:0.00000}", Lng) + " " +
                                                         String.Format("{0:0.002} m", Alt) + Environment.NewLine + "WGS84: " + String.Format("{0:0.002} m", Alt_WGS84) + 
                                                         " Sats: " + Sats + " HDOP: " + String.Format("{0:0.002} m", hdop) + " DGPS: " + ((fix_type > 1) ? "Yes":"No");
                        }
                    );
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                autoConnectGPS();   
            } catch
            {

            }
        }

        // Calculates the checksum for a sentence
        string GetChecksum(string sentence)
        {
            // Loop through all chars to get a checksum
            int Checksum = 0;
            foreach (char Character in sentence.ToCharArray())
            {
                switch (Character)
                {
                    case '$':
                        // Ignore the dollar sign
                        break;
                    case '*':
                        // Stop processing before the asterisk
                        return Checksum.ToString("X2");
                    default:
                        // Is this the first value for the checksum?
                        if (Checksum == 0)
                        {
                            // Yes. Set the checksum to the value
                            Checksum = Convert.ToByte(Character);
                        }
                        else
                        {
                            // No. XOR the checksum with this character's value
                            Checksum = Checksum ^ Convert.ToByte(Character);
                        }
                        break;
                }
            }
            // Return the checksum formatted as a two-character hexadecimal
            return Checksum.ToString("X2");
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            doGPSConnect();

        }

    }

}
