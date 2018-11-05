using System;
using System.Windows.Forms;
using MissionPlanner.Comms;
using System.Globalization;
using MissionPlanner.Utilities;
using System.IO;
using System.Net.Sockets;

namespace MissionPlanner
{
    public partial class MovingBase : Form
    {
        System.Threading.Thread t12;
        static bool threadrun = false;
        static MovingBase Instance;
        static TcpListener listener;
        static ICommsSerial comPort = new SerialPort();
        static internal PointLatLngAlt lastgotolocation = new PointLatLngAlt(0, 0, 0, "Goto last");
        static internal PointLatLngAlt gotolocation = new PointLatLngAlt(0, 0, 0, "Goto");
        static internal int intalt = 100;
        static float updaterate = 0.5f;
        static bool updaterallypnt = false;

        public MovingBase()
        {
            Instance = this;

            InitializeComponent();

            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            CMB_serialport.Items.Add("TCP Host - 14551");
            CMB_serialport.Items.Add("TCP Client");
            CMB_serialport.Items.Add("UDP Host - 14551");
            CMB_serialport.Items.Add("UDP Client");

            CMB_updaterate.SelectedItem = updaterate;

            CHK_updateRallyPnt.Checked = updaterallypnt;

            if (threadrun)
            {
                BUT_connect.Text = Strings.Stop;
                CMB_baudrate.Text = comPort.BaudRate.ToString();
                CMB_serialport.Text = comPort.PortName;
                CMB_updaterate.Text = updaterate.ToString();
            }

            MissionPlanner.Utilities.Tracking.AddPage(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(),
                System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            if (comPort.IsOpen)
            {
                threadrun = false;
                comPort.Close();
                BUT_connect.Text = Strings.Connect;
                MainV2.comPort.MAV.cs.MovingBase = null;
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

                t12 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
                {
                    IsBackground = true,
                    Name = "movingbase Input"
                };
                t12.Start();

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

        void mainloop()
        {
            DateTime nextsend = DateTime.Now;

            DateTime nextrallypntupdate = DateTime.Now;

            StreamWriter sw = new StreamWriter(File.OpenWrite("MovingBase.txt"));

            threadrun = true;
            while (threadrun)
            {
                try
                {
                    string line = comPort.ReadLine();

                    sw.WriteLine(line);

                    //string line = string.Format("$GP{0},{1:HHmmss},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},", "GGA", DateTime.Now.ToUniversalTime(), Math.Abs(lat * 100), MainV2.comPort.MAV.cs.lat < 0 ? "S" : "N", Math.Abs(lng * 100), MainV2.comPort.MAV.cs.lng < 0 ? "W" : "E", MainV2.comPort.MAV.cs.gpsstatus, MainV2.comPort.MAV.cs.satcount, MainV2.comPort.MAV.cs.gpshdop, MainV2.comPort.MAV.cs.alt, "M", 0, "M", "");
                    if (line.StartsWith("$GPGGA") || line.StartsWith("$GNGGA")) // 
                    {
                        string[] items = line.Trim().Split(',', '*');

                        if (items[15] != GetChecksum(line.Trim()))
                        {
                            Console.WriteLine("Bad Nmea line " + items[15] + " vs " + GetChecksum(line.Trim()));
                            continue;
                        }

                        if (items[6] == "0")
                        {
                            Console.WriteLine("No Fix");
                            continue;
                        }

                        gotolocation.Lat = double.Parse(items[2], CultureInfo.InvariantCulture)/100.0;

                        gotolocation.Lat = (int) gotolocation.Lat + ((gotolocation.Lat - (int) gotolocation.Lat)/0.60);

                        if (items[3] == "S")
                            gotolocation.Lat *= -1;

                        gotolocation.Lng = double.Parse(items[4], CultureInfo.InvariantCulture)/100.0;

                        gotolocation.Lng = (int) gotolocation.Lng + ((gotolocation.Lng - (int) gotolocation.Lng)/0.60);

                        if (items[5] == "W")
                            gotolocation.Lng *= -1;

                        gotolocation.Alt = double.Parse(items[9], CultureInfo.InvariantCulture);

                        gotolocation.Tag = "Sats " + items[7] + " hdop " + items[8];
                    }


                    if (DateTime.Now > nextsend && gotolocation.Lat != 0 && gotolocation.Lng != 0 &&
                        gotolocation.Alt != 0) // 200 * 10 = 2 sec /// lastgotolocation != gotolocation && 
                    {
                        nextsend = DateTime.Now.AddMilliseconds(1000/updaterate);
                        Console.WriteLine("new home wp " + DateTime.Now.ToString("h:MM:ss") + " " + gotolocation.Lat +
                                          " " + gotolocation.Lng + " " + gotolocation.Alt);
                        lastgotolocation = new PointLatLngAlt(gotolocation);

                        Locationwp gotohere = new Locationwp();

                        gotohere.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
                        gotohere.alt = (float) (gotolocation.Alt);
                        gotohere.lat = (gotolocation.Lat);
                        gotohere.lng = (gotolocation.Lng);

                        try
                        {
                            updateLocationLabel(gotohere);
                        }
                        catch
                        {
                        }

                        MainV2.comPort.MAV.cs.MovingBase = gotolocation;

                        // plane only
                        if (updaterallypnt && DateTime.Now > nextrallypntupdate)
                        {
                            nextrallypntupdate = DateTime.Now.AddSeconds(5);
                            try
                            {
                                MainV2.comPort.setParam("RALLY_TOTAL", 1);

                                MainV2.comPort.setRallyPoint(0,
                                    new PointLatLngAlt(gotolocation)
                                    {
                                        Alt =
                                            gotolocation.Alt + double.Parse(Settings.Instance["TXT_DefaultAlt"].ToString())
                                    },
                                    0, 0, 0, (byte) (float) MainV2.comPort.MAV.param["RALLY_TOTAL"]);

                                MainV2.comPort.setParam("RALLY_TOTAL", 1);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                    }
                }
                catch
                {
                    System.Threading.Thread.Sleep((int) (1000/updaterate));
                }
            }

            sw.Close();
        }

        private void updateLocationLabel(Locationwp plla)
        {
            if (!Instance.IsDisposed)
            {
                Instance.BeginInvoke(
                    (MethodInvoker)
                        delegate
                        {
                            Instance.LBL_location.Text = gotolocation.Lat + " " + gotolocation.Lng + " " +
                                                         gotolocation.Alt + " " + gotolocation.Tag;
                        }
                    );
            }
        }

        private void SerialOutput_FormClosing(object sender, FormClosingEventArgs e)
        {
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

        private void CMB_updaterate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                updaterate = float.Parse(CMB_updaterate.Text.Replace("hz", ""));
            }
            catch
            {
                CustomMessageBox.Show(Strings.InvalidUpdateRate, Strings.ERROR);
            }
        }

        private void CHK_updateRallyPnt_CheckedChanged(object sender, EventArgs e)
        {
            updaterallypnt = CHK_updateRallyPnt.Checked;
        }
    }
}