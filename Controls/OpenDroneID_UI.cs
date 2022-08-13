using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System.Drawing;


namespace MissionPlanner.Controls
{
    public partial class OpenDroneID_UI : UserControl
    {
        static OpenDroneID_UI Instance;
        static TcpListener listener;
        static ICommsSerial comPort = null;
        static internal PointLatLngAlt lastgotolocation = new PointLatLngAlt(0, 0, 0, "Goto last");
        static internal PointLatLngAlt gotolocation = new PointLatLngAlt(0, 0, 0, "Goto");
        static MAVLink.mavlink_open_drone_id_arm_status_t odid_arm_status; 

        public OpenDroneID_UI()
        {
            Instance = this;

            InitializeComponent();
            try
            {
                CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
                CMB_serialport.Items.Add("TCP Host - 14551");
                CMB_serialport.Items.Add("TCP Client");
                CMB_serialport.Items.Add("UDP Host - 14551");
                CMB_serialport.Items.Add("UDP Client");
            } catch
            {
                Console.WriteLine("Couldn't Init Open DID Form.");
            }
            MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_ARM_STATUS, handleODIDArmMSg, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);
        }


        private bool handleODIDArmMSg(MAVLink.MAVLinkMessage arg)
        {
            odid_arm_status = arg.ToStructure<MAVLink.mavlink_open_drone_id_arm_status_t>();

            //led_ArmedError.Color = ((odid_arm_status.status > 0) ? Color.Red : Color.Green);
            LBL_armed_txt.Text = (odid_arm_status.error).ToString();
            return true; 
        }


        private void BUT_connect_Click(object sender, EventArgs e)
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

                timer1.Start(); 

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

        //TO-DO - we may want to move this to a more centralized spot, or make this 
        //the primary thread for Moving Base GPS read. 
        private void timer1_Tick(object sender, EventArgs e)
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

                            gotolocation.Tag = "Sats " + items[7] + " hdop " + items[8] + (items[6]=="2" ? " - DGPS Fix":" - GPS Fix");

                            udpate_gps_text();
                        }

                        // Sanity Check
                        if (gotolocation.Lat != 0.0 && gotolocation.Lng != 0.0)
                            MainV2.comPort.MAV.cs.MovingBase = gotolocation;
                    }
                } else
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
                            Instance.LBL_gpsStatus.Text = Math.Round(gotolocation.Lat, 6) + " " + Math.Round(gotolocation.Lng, 6) + " " +
                                                         Math.Round(gotolocation.Alt, 2) + gotolocation.Tag;
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
    }



}
