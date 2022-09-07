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
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace MissionPlanner
{

    public partial class NMEA_GPS_Connection : UserControl
    {
        public class PointNMEA
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
            public double Alt { get; set; }
            public double Alt_WGS84 { get; set; }
            public float hdop { get; set; }
            public int sats { get; set; }
            public int fix_type { get; set; }
        }

        static TcpListener listener;
        static ICommsSerial comPort = null;
        //static internal PointLatLngAlt lastgotolocation = new PointLatLngAlt(0, 0, 0, "Goto last");
        //static internal PointLatLngAlt gotolocation = new PointLatLngAlt(0, 0, 0, "Goto");

        private PointNMEA _thisData { get; set; } = new PointNMEA();

        public Stopwatch last_gps_msg = new Stopwatch();
        private bool portsAreLoaded = false;
        static NMEA_GPS_Connection Instance;

        private NMEA_Viewer _nmeaViewer;

        System.Threading.Thread _thread;
        static bool threadrun = false;
        DateTime _last_time_1 = DateTime.Now;
        DateTime _last_time_2 = DateTime.Now;
        DateTime _startup_time = DateTime.Now;
        float _update_rate_hz_1 = 10.0f; // 10 hz
        float _update_rate_hz_2 = 1.0f; // 1 hz 

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
                Console.WriteLine("Couldn't Init Open NMEA Form.");
            }
            //timer2.Start();

            if ((LicenseManager.UsageMode != LicenseUsageMode.Designtime) && (!String.IsNullOrEmpty(Settings.Instance["moving_gps_com"])))
                start();

        }

        private void start()
        {
/*            try
            {
                if (_thread != null)
                {
                    _thread.Abort();

                }
            }
            catch { }*/

            threadrun = true;
            _thread = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
            {
                IsBackground = true,
                Name = "NMEA_Thread"
            };
            _thread.Start();
        }

        public PointNMEA getPointNMEA()
        {
            return _thisData;
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


        private void CMB_serialport_Enter(object sender, EventArgs e)
        {
            init_com_port_list();
        }

        private void autoConnectGPS()
        {
            // TODO quick autoconnect after 2 minutes

            if (portsAreLoaded == false || CMB_serialport.SelectedIndex > 0) return;
            init_com_port_list();
            
            
            try
            {
                // Preload Serial port from settings
                if (!String.IsNullOrEmpty(Settings.Instance["moving_gps_com"]) && CMB_serialport.Items.Contains(Settings.Instance["moving_gps_com"]))
                {
                    CMB_serialport.SelectedIndex = CMB_serialport.Items.IndexOf(Settings.Instance["moving_gps_com"]);
                    //Console.Write("COM: " + CMB_serialport.Text);
                }
                else
                    return;

                //Preload Baud Rate from Settings
                if (!String.IsNullOrEmpty(Settings.Instance["moving_gps_baud"]) && CMB_baudrate.Items.Contains(Settings.Instance["moving_gps_baud"]))
                {
                    CMB_baudrate.SelectedIndex = CMB_baudrate.Items.IndexOf(Settings.Instance["moving_gps_baud"]);
                    //Console.Write(" BAUD: " + CMB_baudrate.Text);
                }              

                //Preload Auto-Connect from Settings
                if (Settings.Instance.GetBoolean("moving_gps_auto"))
                {
                    if (CB_auto_connect.Checked == true)
                        doGPSConnect();
                    else
                        CB_auto_connect.Checked = true; // will auto try to connect
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Auto Connect Setup Failed.");
                Console.WriteLine(ex.Message);
            }

        }

        private void CB_auto_connect_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_auto_connect.Checked == true && CMB_serialport.SelectedIndex > 0) doGPSConnect();

            Settings.Instance["moving_gps_auto"] = CB_auto_connect.Checked.ToString();

        }

        private void doGPSConnect()
        {
            if (comPort != null && comPort.IsOpen)
            {
                comPort.Close();
                BUT_connect.Text = Strings.Connect;
                threadrun = false;
                LBL_gpsStatus.Text = "Disconnected";
                _thisData = new PointNMEA();
                _thread.Abort();
            }
            else
            {
                LBL_gpsStatus.Text = "Connecting to " + CMB_serialport.Text;
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
                    //CustomMessageBox.Show(Strings.ErrorConnecting + "\n" + ex.ToString(), Strings.ERROR);
                    LBL_gpsStatus.Text = "Error Connecting to " + CMB_serialport.Text + ". Try again.";
                    return;
                }

                if (comPort != null && comPort.IsOpen)
                {
                    Console.WriteLine("Moving Base COM Port Opened at port " + comPort.PortName);
                    LBL_gpsStatus.Text = "Connected to " + comPort.PortName + ". Waiting for fix";

                    start();

                }

                Settings.Instance["moving_gps_com"] = CMB_serialport.Text;
                Settings.Instance["moving_gps_baud"] = CMB_baudrate.Text;
                Settings.Instance["moving_gps_auto"] = CB_auto_connect.Checked.ToString();

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
                                LBL_gpsStatus.Text = "Connected, No Fix";
                                continue;
                            }

                            _thisData.Lat = double.Parse(items[2], CultureInfo.InvariantCulture) / 100.0;

                            _thisData.Lat = (int)_thisData.Lat + ((_thisData.Lat - (int)_thisData.Lat) / 0.60);

                            if (items[3] == "S")
                                _thisData.Lat *= -1;

                            _thisData.Lng = double.Parse(items[4], CultureInfo.InvariantCulture) / 100.0;

                            _thisData.Lng = (int)_thisData.Lng + ((_thisData.Lng - (int)_thisData.Lng) / 0.60);

                            if (items[5] == "W")
                                _thisData.Lng *= -1;

                            _thisData.Alt = double.Parse(items[9], CultureInfo.InvariantCulture);

                            if (!String.IsNullOrEmpty(items[11]))
                            {
                                _thisData.Alt_WGS84 = _thisData.Alt + double.Parse(items[11], CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                _thisData.Alt_WGS84 = -1.0;
                            }


                            _thisData.fix_type = (int.Parse(items[6]));

                            _thisData.sats = (int.Parse(items[7]));
                            _thisData.hdop = (float.Parse(items[8]));

                            last_gps_msg.Restart();
                            udpate_gps_text();
                            updateNMEAViewer(true, line);
                        } else
                        {
                            updateNMEAViewer(false, line);
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

        private void updateNMEAViewer(bool inUse, string line)
        {
            try
            {
                if (_nmeaViewer != null)
                {
                    _nmeaViewer.update_NMEA_String(((inUse==true)?"> ":"X ") + line);
                }
            }
            catch { }
        }

        private void udpate_gps_text()
            {
                if (!Instance.IsDisposed)
                {
                    Instance.BeginInvoke(
                        (MethodInvoker)
                            delegate
                            {
                                Instance.LBL_gpsStatus.Text = String.Format("{0:0.00000}", _thisData.Lat) + " " + String.Format("{0:0.00000}", _thisData.Lng) + " " +
                                                             String.Format("{0:0.002} m", _thisData.Alt) + Environment.NewLine + "WGS84: " + String.Format("{0:0.002} m", _thisData.Alt_WGS84) + 
                                                             " Sats: " + _thisData.sats + " HDOP: " + String.Format("{0:0.02}", _thisData.hdop) + " DGPS: " + ((_thisData.fix_type > 1) ? "Yes":"No");
                            }
                        );
                }
            }



        private void LBL_gpsStatus_DoubleClick(object sender, EventArgs e)
        {
            _nmeaViewer = new NMEA_Viewer(); 
            _nmeaViewer.Show();
            _nmeaViewer.setLabel("Showing Feed from: " + comPort.PortName);
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

        private void mainloop()
        {
            threadrun = true;
            while (threadrun)
            {
                DateTime _now = DateTime.Now;
                if ((comPort != null && comPort.IsOpen))
                {
                    try
                    {
                        if (_now > _last_time_1.AddSeconds(1.0 / _update_rate_hz_1))
                        {
                            // Check GPS
                            readNMEAGPS();
                            _last_time_1 = DateTime.Now;
                        }
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep((int)(1000 / _update_rate_hz_1));
                    }
                }
                else
                {
                    try
                    {
                        if (_now.AddSeconds(-300) < _startup_time && _now > _last_time_2.AddSeconds(1.0 / _update_rate_hz_2))
                        {
                            // Check GPS
                            autoConnectGPS();
                            _last_time_2 = DateTime.Now;
                        }
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep((int)(1000 / _update_rate_hz_2));
                    }


                }
                System.Threading.Thread.Sleep((int)(10));
            }
        }

    }

}
