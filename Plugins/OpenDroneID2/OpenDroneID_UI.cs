using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System.Drawing;
using System.Diagnostics;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.Controls
{
    public partial class OpenDroneID_UI : UserControl
    {
        static OpenDroneID_UI Instance;
        static TcpListener listener;
        static ICommsSerial comPort = null;
        static internal PointLatLngAlt lastgotolocation = new PointLatLngAlt(0, 0, 0, "Goto last");
        static internal PointLatLngAlt gotolocation = new PointLatLngAlt(0, 0, 0, "Goto");
        //static MAVLink.mavlink_open_drone_id_arm_status_t odid_arm_status;
        private bool hasODID = false;
        private bool portsAreLoaded = false;
        private bool gpsHasSBAS = false;
        private double wgs84_alt;
        private Stopwatch last_odid_msg = new Stopwatch();
        private Stopwatch last_gps_msg = new Stopwatch();

        private bool _odid_arm_msg, _uas_id, _gcs_gps, _odid_arm_status; 

        private const int ODID_ARM_MESSAGE_TIMEOUT = 5000;
        public OpenDroneID myDID = new OpenDroneID();

        public OpenDroneID_UI()
        {
            Instance = this;

            InitializeComponent();
            try
            {
                init_com_port_list();
            }
            catch
            {
                Console.WriteLine("Couldn't Init Open DID Form.");
            }



            start_sub(true);


            timer2.Start();
        }

        private void start_sub(bool force = false)
        {
            if (!force && (MainV2.comPort.BaseStream == null || !MainV2.comPort.BaseStream.IsOpen))
            {
                // pass
            }
            else
            {
                Console.WriteLine("\n\n\n[DRONE ID] Subscribing to OPEN_DRONE_ID_ARM_STATUS for SysId: " + MainV2.comPort.sysidcurrent);
                //MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_ARM_STATUS, handleODIDArmMSg2, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);
                MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_ARM_STATUS, handleODIDArmMSg2, 0, 0);

            }
        }

        private bool handleODIDArmMSg2(MAVLink.MAVLinkMessage arg)
        {
            Console.WriteLine("Got ODID Message!");
            MAVLink.mavlink_open_drone_id_arm_status_t odid_arm_status;
            odid_arm_status = arg.ToStructure<MAVLink.mavlink_open_drone_id_arm_status_t>();

            // TODO: Check timestamp of ODID message and indicate error
            if (hasODID == true)
                last_odid_msg.Restart();
            else
            {
                Console.WriteLine("[DRONE_ID] Detected and Starting on System ID: " + MainV2.comPort.MAV.sysid);
                last_odid_msg.Start();
                myDID.Start(MainV2.comPort, arg.sysid, arg.compid);
            }
            LED_ArmedError.Color = ((odid_arm_status.status > 0) ? Color.Red : Color.Green);
            LBL_armed_invalid.Text = ((odid_arm_status.status > 0) ? "Error: " : "Ready ") + System.Text.Encoding.UTF8.GetString(odid_arm_status.error);
            hasODID = true;

            return true; ;
        }

/*        public bool handleODIDArmMSg(MAVLink.mavlink_open_drone_id_arm_status_t odid_arm_status, byte odid_sys_id, byte odid_comp_id)
        {
            // TODO: Check timestamp of ODID message and indicate error
            if (hasODID == true)
                last_odid_msg.Restart();
            else
            {
                last_odid_msg.Start();
                myDID.Start(MainV2.comPort, odid_sys_id, odid_comp_id);
            }
            LED_ArmedError.Color = ((odid_arm_status.status > 0) ? Color.Red : Color.Green);
            LBL_armed_invalid.Text = ((odid_arm_status.status > 0) ? "Error: " : "Ready ") + System.Text.Encoding.UTF8.GetString(odid_arm_status.error);
            hasODID = true;

            return true;
        }*/

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

            if (portsAreLoaded == false ||  CMB_serialport.SelectedIndex > 0) return;
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


            } catch
            {
                Console.WriteLine("Auto Connect Failed.");
            }

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


        private void BUT_connect_Click(object sender, EventArgs e)
        {
            doGPSConnect();
            
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

        private void timer2_Tick(object sender, EventArgs e)
        {
            try // auto connect GPS
            {
                autoConnectGPS();
            }
            catch
            {
                Console.WriteLine("[MOVING GPS] Auto Connect Failed");
            }
        }




        //TO-DO - we may want to move this to a more centralized spot, or make this 
        //the primary thread for Moving Base GPS read. 
        private void timer1_Tick(object sender, EventArgs e)
        {

            // Check GPS
            readNMEAGPS();

            checkGCSGPS();

            checkODIDMsgs();

            checkODID_OK();

            checkUID();
        }

        private void checkODIDMsgs()
        {
            if (hasODID == false) return;

            // Check Requirements
            _odid_arm_msg = (last_odid_msg.ElapsedMilliseconds < 5000);
           LED_RemoteID_Messages.Color = (_odid_arm_msg==false) ? Color.Red : Color.Green;
        }

        private void checkODID_OK()
        {
            
            if (_gcs_gps == false)
            {
                
                myODID_Status.setStatusAlert("GCS GPS Invalid");
                
            } else if (_odid_arm_msg == false)
            {
                myODID_Status.setStatusAlert("Remote ID Msg Timeout");
                
            } else if (_odid_arm_status == false)
            {
                myODID_Status.setStatusAlert("Remote ID ARM Error");
                
            } else
            {
                myODID_Status.setStatusOK();
                
            }
            
        }

        private void checkUID()
        {

            _uas_id = !String.IsNullOrEmpty(TXT_UAS_ID.Text);
            LED_UAS_ID.Color = _uas_id ? Color.Green : Color.Red;
            
        }

        private void checkGCSGPS()
        {
            // Check GCS GPS
            if (last_gps_msg.ElapsedMilliseconds > 5000)
            {
                LBL_GCS_GPS_Invalid.Text = "GCS Data Timeout.";
                _gcs_gps = false;
            }
            else if (gotolocation.Lat == 0.0 || gotolocation.Lng == 0.0)
            {
                LBL_GCS_GPS_Invalid.Text = "GCS GPS Lock Invalid.";
                LED_gps_valid.Color = Color.Orange;
                _gcs_gps = false;
            }
            else if (gpsHasSBAS == false)
            {
                LED_gps_valid.Color = Color.Yellow;
                LBL_GCS_GPS_Invalid.Text = "GCS No DGPS Corr.";
                _gcs_gps = false;
            }
            else
            {
                LED_gps_valid.Color = Color.Green;
                LBL_GCS_GPS_Invalid.Text = "";
                _gcs_gps = true;
            }

           
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

                            gotolocation.Lat = double.Parse(items[2], CultureInfo.InvariantCulture) / 100.0;

                            gotolocation.Lat = (int)gotolocation.Lat + ((gotolocation.Lat - (int)gotolocation.Lat) / 0.60);

                            if (items[3] == "S")
                                gotolocation.Lat *= -1;

                            gotolocation.Lng = double.Parse(items[4], CultureInfo.InvariantCulture) / 100.0;

                            gotolocation.Lng = (int)gotolocation.Lng + ((gotolocation.Lng - (int)gotolocation.Lng) / 0.60);

                            if (items[5] == "W")
                                gotolocation.Lng *= -1;

                            gotolocation.Alt = double.Parse(items[9], CultureInfo.InvariantCulture);

                            if (!String.IsNullOrEmpty(items[11]))
                            {
                                wgs84_alt = gotolocation.Alt + double.Parse(items[11], CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                wgs84_alt = -1.0;
                            }

                            gotolocation.Tag = "WGS84: " + String.Format("{0:0.0}m", wgs84_alt) + " Sats " + items[7].PadLeft(2) + " hdop " + items[8].PadLeft(4) + (items[6] == "2" ? " - DGPS Fix" : " - GPS Fix");
                            gpsHasSBAS = (items[6] == "2");

                            last_gps_msg.Restart();
                            udpate_gps_text();
                        }

                        // Sanity Check
                        if (gotolocation.Lat != 0.0 && gotolocation.Lng != 0.0)
                        {
                            MainV2.comPort.MAV.cs.MovingBase = gotolocation;

                            myDID.operator_latitude = gotolocation.Lat;
                            myDID.operator_longitude = gotolocation.Lng;
                            myDID.operator_altitude_geo = (float) wgs84_alt;
                            myDID.operator_location_type = MAVLink.MAV_ODID_OPERATOR_LOCATION_TYPE.LIVE_GNSS;


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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Settings.Instance["ODID_UAS_ID"] = TXT_UAS_ID.Text; 
        }

        private void udpate_gps_text()
        {
            if (!Instance.IsDisposed)
            {
                Instance.BeginInvoke(
                    (MethodInvoker)
                        delegate
                        {
                            Instance.LBL_gpsStatus.Text = String.Format("{0:0.00000}",gotolocation.Lat) + " " + String.Format("{0:0.00000}", gotolocation.Lng) + " " +
                                                         String.Format("{0:0.002} m", gotolocation.Alt) + Environment.NewLine + gotolocation.Tag;
                        }
                    );
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

        private void CB_auto_connect_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_auto_connect.Checked == true && CMB_serialport.SelectedIndex > 0) doGPSConnect();
        }
    }



}
