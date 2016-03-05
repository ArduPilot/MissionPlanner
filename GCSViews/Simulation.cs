using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.HIL;
using ZedGraph;
using MissionPlanner.Utilities;

// config file
// dll imports
// Graphs

// Written by Michael Oborne

namespace MissionPlanner.GCSViews
{
    public partial class Simulation : MyUserControl
    {
        private const int AEROSIMRC_MAX_CHANNELS = 39;
        private const int FG_MAX_ENGINES = 4;
        private const int FG_MAX_WHEELS = 3;
        private const int FG_MAX_TANKS = 4;
        private const float ft2m = (float) (1.0/3.2808399);
        private const float rad2deg = (float) (180/Math.PI);
        private const float deg2rad = (float) (1.0/rad2deg);
        private const float kts2fps = (float) 1.68780986;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MAVLinkInterface comPort = MainV2.comPort;
        private readonly float[][] DATA = new float[113][];
        // for servo graph
        private readonly RollingPointPairList list = new RollingPointPairList(1200);
        private readonly RollingPointPairList list2 = new RollingPointPairList(1200);
        private readonly RollingPointPairList list3 = new RollingPointPairList(1200);
        private readonly RollingPointPairList list4 = new RollingPointPairList(1200);
        private readonly List<string> position = new List<string>();
        // 1000 hz = 1000ms
        private readonly double simstep = 1.0/1000;
        private readonly sitl_fdm[] sitl_fdmbuffer = new sitl_fdm[5];
        private TDataFromAeroSimRC aeroin;
        private bool displayfull;
        private int GPS_rate = 200;
        // gps buffer
        private int gpsbufferindex;
        private int hzcount;
        private int hzcount2;
        private DateTime hzcounttime = DateTime.Now;
        private TcpClient JSBSimSEND;
        private FGNetFDM lastfdmdata;
        private DateTime lastgpsupdate = DateTime.Now;
        private UdpClient MavLink;
        private DateTime now = DateTime.Now;
        private sitl_fdm oldgps;
        private DateTime oldtime = DateTime.Now;
        private int packetcount;
        private int packetssent;
        private int pitchgain = 10000;
        private int recvPort = 49005;
        private EndPoint Remote = new IPEndPoint(IPAddress.Any, 0);
        private int REV_pitch = 1;
        private int REV_roll = 1;
        private int REV_rudder = 1;
        // set defaults
        private int rollgain = 10000;
        private int ruddergain = 10000;
        private string simIP = "127.0.0.1";
        private int simPort = 49000;
        private DateTime simsendtime = DateTime.Now;
        private double simtime = 1;
        private Socket SimulatorRECV;
        private sitl_fdm sitldata_old;
        private string SITLIP = "127.0.0.1";
        private Socket SITLRCRECV;
        public int threadrun;
        private int throttlegain = 10000;
        //string logdata = "";
        private int tickStart;
        private byte[] udpdata = new byte[113*9 + 5]; // 113 types - 9 items per type (index+8) + 5 byte header
        private UdpClient XplanesSEND;

        public Simulation()
        {
            InitializeComponent();
        }

        ~Simulation()
        {
            if (threadrun == 1)
                ConnectComPort_Click(new object(), EventArgs.Empty);

            Dispose(false);

            MavLink = null;
            XplanesSEND = null;
            SimulatorRECV = null;
            SITLRCRECV = null;
        }

        private void Simulation_Load(object sender, EventArgs e)
        {
            timer_servo_graph.Stop();

            GPSrate.SelectedIndex = 2;

            xmlconfig(false);

            CreateChart(zg1);

            CHKdisplayall_CheckedChanged(null, null);
        }

        private void ConnectComPort_Click(object sender, EventArgs e)
        {
            if (threadrun == 0)
            {
                OutputLog.Clear();

                if (MainV2.comPort.BaseStream.IsOpen == false)
                {
                    //   CustomMessageBox.Show("Please connect first");
                    //  return;
                }


                try
                {
                    // reset/create
                    lastfdmdata = new FGNetFDM();

                    if (RAD_JSBSim.Checked)
                    {
                        simPort = 5124;
                        recvPort = 5123;
                    }

                    SetupUDPRecv();

                    if (RAD_softXplanes.Checked)
                    {
                        SetupUDPXplanes();
                        SetupUDPMavLink();
                    }
                    else
                    {
                        SetupUDPXplanes(); // fg udp style
                        SetupUDPMavLink(); // pass traffic - raw
                    }

                    OutputLog.AppendText("Sim Link Started\n");
                }
                catch (Exception ex)
                {
                    OutputLog.AppendText("Socket setup problem. Do you have this open already? " + ex);
                }

                // set to highest to try prevent any timer issues

                var t11 = new Thread(mainloop)
                {
                    Name = "Main simu Serial/UDP listener",
                    IsBackground = true,
                    Priority = ThreadPriority.Lowest
                };
                t11.Start();
                timer_servo_graph.Start();
            }
            else
            {
                timer_servo_graph.Stop();
                threadrun = 0;
                if (JSBSimSEND != null && JSBSimSEND.Connected)
                {
                    try
                    {
                        JSBSimSEND.Client.Send(Encoding.ASCII.GetBytes("\n\nexit\n"));
                    }
                    catch
                    {
                    }
                }
                if (SimulatorRECV != null)
                    SimulatorRECV.Close();
                if (SimulatorRECV != null && SimulatorRECV.Connected)
                    SimulatorRECV.Disconnect(true);
                if (SITLRCRECV != null)
                    SITLRCRECV.Close();
                if (MavLink != null)
                    MavLink.Close();
                position.Clear();

                if (XplanesSEND != null)
                    XplanesSEND.Close();

                //                if (comPort.BaseStream.IsOpen)
                //                    comPort.stopall(true);

                OutputLog.AppendText("Sim Link Stopped\n");

                Thread.Sleep(1000);
                Application.DoEvents();
            }
        }

        /// <summary>
        ///     Sets config hash for write on application exit
        /// </summary>
        /// <param name="write">true/false</param>
        private void xmlconfig(bool write)
        {
            if (write)
            {
                Settings.Instance["REV_roll"] = CHKREV_roll.Checked.ToString();
                Settings.Instance["REV_pitch"] = CHKREV_pitch.Checked.ToString();
                Settings.Instance["REV_rudder"] = CHKREV_rudder.Checked.ToString();
                Settings.Instance["GPSrate"] = GPSrate.Text;

                Settings.Instance["MAVrollgain"] = TXT_rollgain.Text;
                Settings.Instance["MAVpitchgain"] = TXT_pitchgain.Text;
                Settings.Instance["MAVruddergain"] = TXT_ruddergain.Text;
                Settings.Instance["MAVthrottlegain"] = TXT_throttlegain.Text;

                Settings.Instance["CHKdisplayall"] = CHKdisplayall.Checked.ToString();

                Settings.Instance["SITLIP"] = SITLIP;
                Settings.Instance["simIP"] = simIP;
                Settings.Instance["recvPort"] = recvPort.ToString();

                Settings.Instance["simPort"] = simPort.ToString();
            }
            else
            {
                foreach (string key in Settings.Instance.Keys)
                {
                    switch (key)
                    {
                        case "simIP":
                            simIP = Settings.Instance[key];
                            break;
                        case "SITLIP":
                            SITLIP = Settings.Instance[key];
                            break;
                        case "simPort":
                            simPort = Settings.Instance.GetInt32(key);
                            break;
                        case "recvPort":
                            recvPort = Settings.Instance.GetInt32(key);
                            break;
                        case "REV_roll":
                            CHKREV_roll.Checked = Settings.Instance.GetBoolean(key);
                            break;
                        case "REV_pitch":
                            CHKREV_pitch.Checked = Settings.Instance.GetBoolean(key);
                            break;
                        case "REV_rudder":
                            CHKREV_rudder.Checked = Settings.Instance.GetBoolean(key);
                            break;
                        case "GPSrate":
                            GPSrate.Text = Settings.Instance[key];
                            break;
                        case "MAVrollgain":
                            TXT_rollgain.Text = Settings.Instance[key];
                            break;
                        case "MAVpitchgain":
                            TXT_pitchgain.Text = Settings.Instance[key];
                            break;
                        case "MAVruddergain":
                            TXT_ruddergain.Text = Settings.Instance[key];
                            break;
                        case "MAVthrottlegain":
                            TXT_throttlegain.Text = Settings.Instance[key];
                            break;
                        case "CHKdisplayall":
                            CHKdisplayall.Checked = Settings.Instance.GetBoolean(key);
                            displayfull = CHKdisplayall.Checked;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void mainloop()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); 
            threadrun = 1;
            Remote = new IPEndPoint(IPAddress.Any, 0);

            var lastdata = DateTime.MinValue;
            try
            {
                // set enable hil status flag - sends base_mode = 0
                MainV2.comPort.setMode(new MAVLink.mavlink_set_mode_t {target_system = MainV2.comPort.MAV.sysid},
                    MAVLink.MAV_MODE_FLAG.HIL_ENABLED);
            }
            catch
            {
            }

            while (threadrun == 1)
            {
                // if (comPort.BaseStream.IsOpen == false) { break; }
                // re-request servo data
                if (!(lastdata.AddSeconds(8) > DateTime.Now))
                {
                    try
                    {
                        comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_CONTROLLER, 50); // request servoout
                    }
                    catch
                    {
                    }
                    lastdata = DateTime.Now; // prevent flooding
                }

                if (hzcounttime.Second != DateTime.Now.Second)
                {
                    Console.WriteLine("SIM recv hz {0} processArduPilot hz {1}", hzcount, hzcount2);
                    hzcount = 0;
                    hzcount2 = 0;
                    hzcounttime = DateTime.Now;
                }

                try
                {
                    if (SimulatorRECV != null && SimulatorRECV.Available > 0)
                    {
                        udpdata = new byte[udpdata.Length];

                        while (SimulatorRECV.Available > 0)
                        {
                            var recv = SimulatorRECV.ReceiveFrom(udpdata, ref Remote);

                            RECVprocess(udpdata, recv, comPort);

                            hzcount++;
                        }
                    }
                }
                catch
                {
                    //OutputLog.AppendText("Xplanes Data Problem - You need DATA IN/OUT 3, 4, 17, 18, 19, 20\n" + ex.Message + "\n");
                }

                try
                {
                    if (SITLRCRECV != null && SITLRCRECV.Available > 0)
                    {
                        var receiveBytes = new byte[28];

                        var remote = (EndPoint) (new IPEndPoint(IPAddress.Any, 5502));

                        var recv = SITLRCRECV.ReceiveFrom(receiveBytes, ref remote);

                        if (recv == 28)
                        {
                            MainV2.comPort.MAV.cs.ch1out = BitConverter.ToUInt16(receiveBytes, 0);
                            MainV2.comPort.MAV.cs.ch2out = BitConverter.ToUInt16(receiveBytes, 2);
                            MainV2.comPort.MAV.cs.ch3out = BitConverter.ToUInt16(receiveBytes, 4);
                            MainV2.comPort.MAV.cs.ch4out = BitConverter.ToUInt16(receiveBytes, 6);
                            MainV2.comPort.MAV.cs.ch5out = BitConverter.ToUInt16(receiveBytes, 8);
                            MainV2.comPort.MAV.cs.ch6out = BitConverter.ToUInt16(receiveBytes, 10);
                            MainV2.comPort.MAV.cs.ch7out = BitConverter.ToUInt16(receiveBytes, 12);
                            MainV2.comPort.MAV.cs.ch8out = BitConverter.ToUInt16(receiveBytes, 14);
                            try
                            {
                                processArduPilot();
                            }
                            catch
                            {
                            }

                            simsendtime = DateTime.Now;

                            hzcount2++;

                            Thread.Sleep(1);

                            continue;
                        }
                    }
                }
                catch
                {
                }
                try
                {
                    if (MavLink != null && MavLink.Client != null && MavLink.Client.Connected && MavLink.Available > 0)
                    {
                        var RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                        var receiveBytes = MavLink.Receive(ref RemoteIpEndPoint);

                        comPort.BaseStream.Write(receiveBytes, 0, receiveBytes.Length);
                    }
                }
                catch
                {
                }
                //if (comPort.BaseStream.IsOpen == false) { break; }
                try
                {
                    MainV2.comPort.MAV.cs.UpdateCurrentSettings(null, true, MainV2.comPort);
                    // when true this uses alot more cpu time

                    if ((DateTime.Now - simsendtime).TotalMilliseconds > 25)
                    {
                        simsendtime = DateTime.Now;

                        if (comPort.BaseStream.IsOpen)
                        {
                            processArduPilot();
                            hzcount2++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Info("SIM Main loop exception " + ex);
                }

                // yield
                Thread.Sleep(0); // this controls send speed  to sim     

                if (Disposing)
                    threadrun = 0;
            }
            comPort.MAV.cs.raterc = 2;
        }

        private void SetupUDPRecv()
        {
            // setup receiver
            var ipep = new IPEndPoint(IPAddress.Any, recvPort);

            try
            {
                // close it to be sure
                if (SimulatorRECV != null)
                    SimulatorRECV.Close();
            }
            catch
            {
            }

            SimulatorRECV = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

            SimulatorRECV.Bind(ipep);

            OutputLog.AppendText("Listerning on port UDP " + recvPort + " (sim->planner)\n");
        }

        private void SetupTcpJSBSim()
        {
            try
            {
                JSBSimSEND = new TcpClient();
                JSBSimSEND.Client.NoDelay = true;
                JSBSimSEND.Connect("127.0.0.1", simPort);
                OutputLog.AppendText("JSB Console port TCP " + simPort + " (planner->sim)\n");

                //JSBSimSEND.Client.Send(System.Text.Encoding.ASCII.GetBytes("set position/h-agl-ft 0\r\n"));

                JSBSimSEND.Client.Send(
                    Encoding.ASCII.GetBytes("set position/lat-gc-deg " + MainV2.comPort.MAV.cs.HomeLocation.Lat + "\r\n"));
                JSBSimSEND.Client.Send(
                    Encoding.ASCII.GetBytes("set position/long-gc-deg " + MainV2.comPort.MAV.cs.HomeLocation.Lng +
                                            "\r\n"));

                JSBSimSEND.Client.Send(Encoding.ASCII.GetBytes("set attitude/phi-rad 0\r\n"));
                JSBSimSEND.Client.Send(Encoding.ASCII.GetBytes("set attitude/theta-rad 0\r\n"));
                JSBSimSEND.Client.Send(Encoding.ASCII.GetBytes("set attitude/psi-rad 0\r\n"));

                JSBSimSEND.Client.Send(Encoding.ASCII.GetBytes("info\r\n"));

                JSBSimSEND.Client.Send(Encoding.ASCII.GetBytes("resume\r\n"));

                Thread.Sleep(3000);

                JSBSimSEND.Client.Send(Encoding.ASCII.GetBytes("step\r\n"));
            }
            catch
            {
                log.Info("JSB console fail");
            }
        }

        private void SetupUDPXplanes()
        {
            // setup sender
            XplanesSEND = new UdpClient(simIP, simPort);

            OutputLog.AppendText("Sending to port UDP " + simPort + " (planner->sim)\n");

            setupXplane();

            OutputLog.AppendText("Sent xplane settings\n");
        }

        private void SetupUDPMavLink()
        {
            // setup sender
            MavLink = new UdpClient("127.0.0.1", 14550);
        }

        /// <summary>
        ///     Recevied UDP packet, process and send required data to serial port.
        /// </summary>
        /// <param name="data">Packet</param>
        /// <param name="receviedbytes">Length</param>
        /// <param name="comPort">Com Port</param>
        private void RECVprocess(byte[] data, int receviedbytes, MAVLinkInterface comPort)
        {
            var sitldata = new sitl_fdm();

            sitldata.timestamp = (ulong) (simtime*1.0e6);

            if (data[0] == 'D' && data[1] == 'A')
            {
                // Xplanes sends
                // 5 byte header
                // 1 int for the index - numbers on left of output
                // 8 floats - might be useful. or 0 if not
                var count = 5;
                while (count < receviedbytes)
                {
                    var index = BitConverter.ToInt32(data, count);

                    DATA[index] = new float[8];

                    DATA[index][0] = BitConverter.ToSingle(data, count + 1*4);
                    ;
                    DATA[index][1] = BitConverter.ToSingle(data, count + 2*4);
                    ;
                    DATA[index][2] = BitConverter.ToSingle(data, count + 3*4);
                    ;
                    DATA[index][3] = BitConverter.ToSingle(data, count + 4*4);
                    ;
                    DATA[index][4] = BitConverter.ToSingle(data, count + 5*4);
                    ;
                    DATA[index][5] = BitConverter.ToSingle(data, count + 6*4);
                    ;
                    DATA[index][6] = BitConverter.ToSingle(data, count + 7*4);
                    ;
                    DATA[index][7] = BitConverter.ToSingle(data, count + 8*4);
                    ;

                    count += 36; // 8 * float
                }

                var xplane9 = !CHK_xplane10.Checked;

                if (xplane9)
                {
                    sitldata.pitchDeg = (DATA[18][0]);
                    sitldata.rollDeg = (DATA[18][1]);
                    sitldata.yawDeg = (DATA[18][2]);
                    sitldata.pitchRate = (DATA[17][0]*rad2deg);
                    sitldata.rollRate = (DATA[17][1]*rad2deg);
                    sitldata.yawRate = (DATA[17][2]*rad2deg);

                    sitldata.heading = DATA[19][2];

                    sitldata.speedN = -DATA[21][5]; // (DATA[3][7] * 0.44704 * Math.Sin(sitldata.heading * deg2rad));
                    sitldata.speedE = DATA[21][3]; // (DATA[3][7] * 0.44704 * Math.Cos(sitldata.heading * deg2rad));
                    sitldata.speedD = -DATA[21][4];
                }
                else
                {
                    sitldata.pitchDeg = (DATA[17][0]);
                    sitldata.rollDeg = (DATA[17][1]);
                    sitldata.yawDeg = (DATA[17][2]);
                    sitldata.pitchRate = (DATA[16][0]*rad2deg);
                    sitldata.rollRate = (DATA[16][1]*rad2deg);
                    sitldata.yawRate = (DATA[16][2]*rad2deg);

                    sitldata.heading = (DATA[18][2]); // 18-2


                    sitldata.speedN = -DATA[21][5]; // (DATA[3][7] * 0.44704 * Math.Sin(sitldata.heading * deg2rad));
                    sitldata.speedE = DATA[21][3]; // (DATA[3][7] * 0.44704 * Math.Cos(sitldata.heading * deg2rad));
                    sitldata.speedD = -DATA[21][4];
                }

                sitldata.airspeed = ((DATA[3][5]*.44704));

                sitldata.latitude = (DATA[20][0]);
                sitldata.longitude = (DATA[20][1]);
                sitldata.altitude = (DATA[20][2]*ft2m);


                //http://en.wikipedia.org/wiki/Banked_turn

                var dcm = new Matrix3();
                dcm.from_euler(sitldata.rollDeg*deg2rad, sitldata.pitchDeg*deg2rad, sitldata.yawDeg*deg2rad);

                // v squared in m/s
                var v_sq = (float) ((DATA[3][7]*0.44704)*(DATA[3][7]*0.44704));

                // rad = tas^2 / (tan(angle) * G)
                var turnrad = v_sq/(float) (9.808f*Math.Tan(sitldata.rollDeg*deg2rad));

                var gload = (float) (1/Math.Cos(sitldata.rollDeg*deg2rad)); // calculated Gs

                // a = v^2/r
                var centripaccel = v_sq/turnrad;

                var accel_body = dcm.transposed()*(new Vector3(0, 0, -9.808));

                var centrip_accel = new Vector3(0, -centripaccel*Math.Cos(sitldata.rollDeg*deg2rad),
                    centripaccel*Math.Sin(sitldata.rollDeg*deg2rad));

                var pitchpart = centrip_accel.y*Math.Cos(sitldata.pitchDeg*deg2rad);
                //centrip_accel.x += pitchpart;
                //centrip_accel.y += pitchpart;


                //centrip_accel.y = -centripaccel;
                accel_body -= centrip_accel;

                var velocitydelta = dcm.transposed()*
                                    (new Vector3((sitldata_old.speedN - sitldata.speedN),
                                        (sitldata_old.speedE - sitldata.speedE), (sitldata_old.speedD - sitldata.speedD)));

                // v = d/t   a = v/t      add linear accel - t is 50 hz
                //accel_body.x += -velocitydelta.x / 0.02;


                //Vector3 velocity = dcm.transposed() * (new Vector3((sitldata.speedN), (sitldata.speedE), (sitldata.speedD)));

                //Console.WriteLine("vel " + velocity.ToString());
                //Console.WriteLine("ved " + velocitydelta.ToString());

                // a = dv / dt

                // 50 hz = 0.02sec
                var accel_mine_body = dcm.transposed()*
                                      (new Vector3((sitldata_old.speedN - sitldata.speedN)/0.02,
                                          (sitldata_old.speedE - sitldata.speedE)/0.02,
                                          (sitldata_old.speedD - sitldata.speedD)/0.02));

                // Console.WriteLine("G"+accel_body.ToString());
                //  Console.WriteLine("M"+accel_mine_body.ToString());

                //sitldata.pitchRate = 0;
                //sitldata.rollRate = 0;
                //sitldata.yawRate = 0;

                sitldata.xAccel = accel_body.x; // DATA[4][5] * 1;
                sitldata.yAccel = accel_body.y; //  DATA[4][6] * 1;
                sitldata.zAccel = accel_body.z; //  (0 - DATA[4][4]) * 9.808;

                //sitldata.xAccel = DATA[4][5] *9.808;
                //sitldata.yAccel = DATA[4][6] *9.808;
                //sitldata.zAccel = -DATA[4][4] *9.808;
            }
            else if (receviedbytes == 0x64) // FG binary udp
            {
                //FlightGear
                /*

                fgIMUData imudata2 = data.ByteArrayToStructureBigEndian<fgIMUData>(0);

                if (imudata2.magic != 0x4c56414d)
                    return;

                if (imudata2.latitude == 0)
                    return;

                chkSensor.Checked = true;

                imu.time_usec = ((ulong)DateTime.Now.ToBinary());

                imu.xacc = ((Int16)(imudata2.accelX * 9808 / 32.2));
                imu.xgyro = ((Int16)(imudata2.rateRoll * 17.453293));
                imu.xmag = 0;
                imu.yacc = ((Int16)(imudata2.accelY * 9808 / 32.2));
                imu.ygyro = ((Int16)(imudata2.ratePitch * 17.453293));
                imu.ymag = 0;
                imu.zacc = ((Int16)(imudata2.accelZ * 9808 / 32.2)); // + 1000
                imu.zgyro = ((Int16)(imudata2.rateYaw * 17.453293));
                imu.zmag = 0;

                gps.alt = ((int)(imudata2.altitude * ft2m * 1000));
                gps.fix_type = 3;
                gps.cog = (ushort)(Math.Atan2(imudata2.velocityE, imudata2.velocityN) * rad2deg * 100);
                gps.lat = (int)(imudata2.latitude * 1.0e7);
                gps.lon = (int)(imudata2.longitude * 1.0e7);
                gps.time_usec = ((ulong)DateTime.Now.Ticks);
                gps.vel = (ushort)(Math.Sqrt((imudata2.velocityN * imudata2.velocityN) + (imudata2.velocityE * imudata2.velocityE)) * ft2m * 100);

                //FileStream stream = File.OpenWrite("fgdata.txt");
                //stream.Write(data, 0, receviedbytes);
                //stream.Close();
                 */
            }
            else if (receviedbytes == 662 || receviedbytes == 658) // 658 = 3.83   662 = 3.91
            {
                /*
            // Model data in body frame coordinates (X=Right, Y=Front, Z=Up)
            public float Model_fVel_Body_X; public float Model_fVel_Body_Y; public float Model_fVel_Body_Z;    // m/s    Model velocity in body coordinates
            public float Model_fAngVel_Body_X; public float Model_fAngVel_Body_Y; public float Model_fAngVel_Body_Z; // rad/s  Model angular velocity in body coordinates
            public float Model_fAccel_Body_X; public float Model_fAccel_Body_Y; public float Model_fAccel_Body_Z;  // m/s/s  Model acceleration in body coordinates
                 */
                var aeroin_last = aeroin;

                aeroin = data.ByteArrayToStructure<TDataFromAeroSimRC>(0);

                sitldata.pitchDeg = (aeroin.Model_fPitch*rad2deg);
                sitldata.rollDeg = (aeroin.Model_fRoll*-1*rad2deg);
                sitldata.yawDeg = ((aeroin.Model_fHeading*rad2deg));

                sitldata.pitchRate = aeroin.Model_fAngVel_Body_X*rad2deg;
                sitldata.rollRate = aeroin.Model_fAngVel_Body_Y*rad2deg;
                sitldata.yawRate = -aeroin.Model_fAngVel_Body_Z*rad2deg;

                // calc gravity
                var dcm = new Matrix3();
                dcm.from_euler(sitldata.rollDeg*deg2rad, sitldata.pitchDeg*deg2rad, sitldata.yawDeg*deg2rad);

                var accel_body = dcm.transposed()*(new Vector3(0, 0, -9.8)); // -9.8

                sitldata.xAccel = aeroin.Model_fAccel_Body_Y/9.808 + accel_body.x; // pitch - back forward-
                sitldata.yAccel = aeroin.Model_fAccel_Body_X/9.808 + accel_body.y; // roll - left right-
                sitldata.zAccel = -aeroin.Model_fAccel_Body_Z/9.808 + accel_body.z;

                //  Console.WriteLine("2 {0,20} {1,20} {2,20}", aeroin.Model_fAccel_Body_X.ToString("0.000"), aeroin.Model_fAccel_Body_Y.ToString("0.000"), aeroin.Model_fAccel_Body_Z.ToString("0.000"));

                sitldata.altitude = aeroin.Model_fPosZ;
                sitldata.latitude = aeroin.Model_fLatitude;
                sitldata.longitude = aeroin.Model_fLongitude;

                sitldata.speedN = aeroin.Model_fVelY;
                sitldata.speedE = aeroin.Model_fVelX;
                sitldata.speedD = aeroin.Model_fVelZ;

                var xvec = aeroin.Model_fVelY - aeroin.Model_fWindVelY;
                var yvec = aeroin.Model_fVelX - aeroin.Model_fWindVelX;

                sitldata.airspeed = ((float) Math.Sqrt((yvec*yvec) + (xvec*xvec)));
            }
            else if (receviedbytes == 408)
            {
                var fdm = data.ByteArrayToStructureBigEndian<FGNetFDM>(0);

                lastfdmdata = fdm;

                sitldata.timestamp = (ulong) (simtime*1.0e6);
                sitldata.altitude = (fdm.altitude);
                sitldata.latitude = (fdm.latitude*rad2deg);
                sitldata.longitude = (fdm.longitude*rad2deg);

                sitldata.rollDeg = fdm.phi*rad2deg;
                sitldata.pitchDeg = fdm.theta*rad2deg;
                sitldata.yawDeg = fdm.psi*rad2deg;

                sitldata.rollRate = fdm.phidot*rad2deg;
                sitldata.pitchRate = fdm.thetadot*rad2deg;
                sitldata.yawRate = fdm.psidot*rad2deg;

                sitldata.speedN = fdm.v_north*ft2m;
                sitldata.speedE = fdm.v_east*ft2m;
                sitldata.speedD = fdm.v_down*ft2m;

                sitldata.xAccel = (fdm.A_X_pilot*9.808/32.2); // pitch
                sitldata.yAccel = (fdm.A_Y_pilot*9.808/32.2); // roll
                sitldata.zAccel = (fdm.A_Z_pilot/32.2*9.808);

                sitldata.airspeed = fdm.vcas*0.5144444f; //  knots to m/s

                //Console.WriteLine("1 {0} {1} {2} {3}",(float)sitldata.rollDeg,MainV2.comPort.MAV.cs.roll,sitldata.pitchDeg,MainV2.comPort.MAV.cs.pitch);

                if (RAD_JSBSim.Checked)
                    sitldata.airspeed = fdm.vcas*ft2m; //  fps to m/s
            }
            else
            {
                log.Info("Bad Udp Packet " + receviedbytes);
                return;
            }

            if (sitldata.altitude < 0)
                sitldata.altitude = 0.00001;

            sitldata_old = sitldata;


            var gpsspan = DateTime.Now - lastgpsupdate;

            // add gps delay
            if (gpsspan.TotalMilliseconds >= GPS_rate)
            {
                lastgpsupdate = DateTime.Now;

                // save current fix = 3
                sitl_fdmbuffer[gpsbufferindex%sitl_fdmbuffer.Length] = sitldata;

                //                Console.WriteLine((gpsbufferindex % gpsbuffer.Length) + " " + ((gpsbufferindex + (gpsbuffer.Length - 1)) % gpsbuffer.Length));

                // return buffer index + 5 = (3 + 5) = 8 % 6 = 2
                oldgps = sitl_fdmbuffer[(gpsbufferindex + (sitl_fdmbuffer.Length - 1))%sitl_fdmbuffer.Length];

                //comPort.sendPacket(oldgps);

                gpsbufferindex++;
            }


            var hilstate = new MAVLink.mavlink_hil_state_t();

            var epochBegin = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            hilstate.time_usec = (ulong) ((DateTime.Now.Ticks - epochBegin.Ticks)/10); // microsec

            hilstate.lat = (int) (oldgps.latitude*1e7); // * 1E7
            hilstate.lon = (int) (oldgps.longitude*1e7); // * 1E7
            hilstate.alt = (int) (oldgps.altitude*1000); // mm

            //   Console.WriteLine(hilstate.alt);

            // Console.WriteLine("{0} {1} {2}", sitldata.rollDeg.ToString("0.0"), sitldata.pitchDeg.ToString("0.0"), sitldata.yawDeg.ToString("0.0"));


            hilstate.pitch = (float) sitldata.pitchDeg*deg2rad; // (rad)
            hilstate.pitchspeed = (float) sitldata.pitchRate*deg2rad; // (rad/s)
            hilstate.roll = (float) sitldata.rollDeg*deg2rad; // (rad)
            hilstate.rollspeed = (float) sitldata.rollRate*deg2rad; // (rad/s)
            hilstate.yaw = (float) sitldata.yawDeg*deg2rad; // (rad)
            hilstate.yawspeed = (float) sitldata.yawRate*deg2rad; // (rad/s)

            hilstate.vx = (short) (sitldata.speedN*100); // m/s * 100 - lat
            hilstate.vy = (short) (sitldata.speedE*100); // m/s * 100 - long
            hilstate.vz = (short) (sitldata.speedD*100); // m/s * 100 - + speed down

            hilstate.xacc = (short) (sitldata.xAccel*101.957); // (mg)
            hilstate.yacc = (short) (sitldata.yAccel*101.957); // (mg)
            hilstate.zacc = (short) (sitldata.zAccel*101.957); // (mg)


            packetcount++;

            if (!comPort.BaseStream.IsOpen)
                return;

            if (comPort.BaseStream.BytesToWrite > 100)
                return;

            //  if (packetcount % 2 == 0) 
            //      return;

            comPort.sendPacket(hilstate);


            //            comPort.sendPacket(oldgps);

            //comPort.sendPacket(new MAVLink.mavlink_vfr_hud_t() { airspeed = (float)sitldata.airspeed } );

            var pres = new MAVLink.mavlink_raw_pressure_t();
            var calc = (101325*Math.Pow(1 - 2.25577*Math.Pow(10, -5)*sitldata.altitude, 5.25588));
            // updated from valid gps
            pres.press_diff1 = (short) (int) (calc - 101325); // 0 alt is 0 pa

            // comPort.sendPacket(pres);
        }

        /// <summary>
        /// </summary>
        /// <param name="lat">rads </param>
        /// <param name="lng">rads </param>
        /// <param name="alt">m</param>
        /// <param name="roll">rads</param>
        /// <param name="pitch">rads</param>
        /// <param name="heading">rads</param>
        /// <param name="yaw">rads</param>
        /// <param name="roll_out">-1 to 1</param>
        /// <param name="pitch_out">-1 to 1</param>
        /// <param name="rudder_out">-1 to 1</param>
        /// <param name="throttle_out">0 to 1</param>
        private void updateScreenDisplay(double lat, double lng, double alt, double roll, double pitch, double heading,
            double yaw, double roll_out, double pitch_out, double rudder_out, double throttle_out)
        {
            try
            {
                // Update Sim stuff
                BeginInvoke((MethodInvoker) delegate
                {
                    TXT_servoroll.Text = roll_out.ToString("0.000");
                    TXT_servopitch.Text = pitch_out.ToString("0.000");
                    TXT_servorudder.Text = rudder_out.ToString("0.000");
                    TXT_servothrottle.Text = throttle_out.ToString("0.000");

                    TXT_lat.Text = (lat*rad2deg).ToString("0.00000");
                    TXT_long.Text = (lng*rad2deg).ToString("0.00000");
                    TXT_alt.Text = (alt).ToString("0.00");

                    TXT_roll.Text = (roll*rad2deg).ToString("0.000");
                    TXT_pitch.Text = (pitch*rad2deg).ToString("0.000");
                    TXT_heading.Text = (heading*rad2deg).ToString("0.000");
                    TXT_yaw.Text = (yaw*rad2deg).ToString("0.000");

                    TXT_wpdist.Text = MainV2.comPort.MAV.cs.wp_dist.ToString();
                    TXT_bererror.Text = MainV2.comPort.MAV.cs.ber_error.ToString();
                    TXT_alterror.Text = MainV2.comPort.MAV.cs.alt_error.ToString();
                    TXT_WP.Text = MainV2.comPort.MAV.cs.wpno.ToString();
                    TXT_control_mode.Text = MainV2.comPort.MAV.cs.mode;
                });
            }
            catch
            {
                Invoke((MethodInvoker) delegate { OutputLog.AppendText("NO SIM data - exep\n"); });
            }
        }

        private void processArduPilot()
        {
            float roll_out, pitch_out, throttle_out, rudder_out;

            roll_out = (MainV2.comPort.MAV.cs.ch1out - 1500)/rollgain;
            pitch_out = (MainV2.comPort.MAV.cs.ch2out - 1500)/pitchgain;
            throttle_out = (MainV2.comPort.MAV.cs.ch3out - 1500)/throttlegain;
            rudder_out = (MainV2.comPort.MAV.cs.ch4out - 1500)/ruddergain;


            // Limit min and max
            roll_out = Constrain(roll_out, -1, 1);
            pitch_out = Constrain(pitch_out, -1, 1);
            rudder_out = Constrain(rudder_out, -1, 1);
            throttle_out = Constrain(throttle_out, 0, 1);

            try
            {
                if (displayfull)
                {
                    // This updates the servo graphs
                    var time = (Environment.TickCount - tickStart)/1000.0;

                    if (CHKgraphroll.Checked)
                    {
                        list.Add(time, roll_out);
                    }
                    else
                    {
                        list.Clear();
                    }
                    if (CHKgraphpitch.Checked)
                    {
                        list2.Add(time, pitch_out);
                    }
                    else
                    {
                        list2.Clear();
                    }
                    if (CHKgraphrudder.Checked)
                    {
                        list3.Add(time, rudder_out);
                    }
                    else
                    {
                        list3.Clear();
                    }
                    if (CHKgraphthrottle.Checked)
                    {
                        list4.Add(time, throttle_out);
                    }
                    else
                    {
                        list4.Clear();
                    }
                }

                if (packetssent%10 == 0) // reduce cpu usage
                {
                    if (RAD_softXplanes.Checked)
                    {
                        if (DATA[20] != null)
                        {
                            var xplane9 = !CHK_xplane10.Checked;
                            if (xplane9)
                            {
                                updateScreenDisplay(DATA[20][0]*deg2rad, DATA[20][1]*deg2rad, DATA[20][2]*.3048,
                                    DATA[18][1]*deg2rad, DATA[18][0]*deg2rad, DATA[19][2]*deg2rad, DATA[18][2]*deg2rad,
                                    roll_out, pitch_out, rudder_out, throttle_out);
                            }
                            else
                            {
                                updateScreenDisplay(DATA[20][0]*deg2rad, DATA[20][1]*deg2rad, DATA[20][2]*.3048,
                                    DATA[17][1]*deg2rad, DATA[17][0]*deg2rad, DATA[18][2]*deg2rad, DATA[17][2]*deg2rad,
                                    roll_out, pitch_out, rudder_out, throttle_out);
                            }
                        }
                    }

                    if (RAD_softFlightGear.Checked || RAD_JSBSim.Checked)
                    {
                        updateScreenDisplay(lastfdmdata.latitude, lastfdmdata.longitude, lastfdmdata.altitude*.3048,
                            lastfdmdata.phi, lastfdmdata.theta, lastfdmdata.psi, lastfdmdata.psi, roll_out, pitch_out,
                            rudder_out, throttle_out);
                    }

                    if (RAD_aerosimrc.Checked)
                    {
                        updateScreenDisplay(aeroin.Model_fLatitude*deg2rad, aeroin.Model_fLongitude*deg2rad,
                            aeroin.Model_fPosZ, aeroin.Model_fRoll, aeroin.Model_fPitch, aeroin.Model_fHeading,
                            aeroin.Model_fHeading, roll_out, pitch_out, rudder_out, throttle_out);
                    }
                }
            }
            catch (Exception e)
            {
                log.Info("Error updateing screen stuff " + e);
            }

            packetssent++;

            if (RAD_aerosimrc.Checked)
            {
                //AeroSimRC
                var AeroSimRC = new byte[4*8]; // StructureToByteArray(fg);

                Array.Copy(BitConverter.GetBytes((double) (roll_out*REV_roll)), 0, AeroSimRC, 0, 8);
                Array.Copy(BitConverter.GetBytes((double) (pitch_out*REV_pitch*-1)), 0, AeroSimRC, 8, 8);
                Array.Copy(BitConverter.GetBytes((double) (rudder_out*REV_rudder)), 0, AeroSimRC, 16, 8);
                Array.Copy(BitConverter.GetBytes((double) ((throttle_out*2) - 1)), 0, AeroSimRC, 24, 8);


                try
                {
                    SimulatorRECV.SendTo(AeroSimRC, Remote);
                }
                catch
                {
                }
            }

            //JSBSim

            if (RAD_JSBSim.Checked)
            {
                roll_out = Constrain(roll_out*REV_roll, -1f, 1f);
                pitch_out = Constrain(-pitch_out*REV_pitch, -1f, 1f);
                rudder_out = Constrain(rudder_out*REV_rudder, -1f, 1f);

                throttle_out = Constrain(throttle_out, -0.0f, 1f);

                var cmd =
                    string.Format(
                        "set fcs/aileron-cmd-norm {0}\r\nset fcs/elevator-cmd-norm {1}\r\nset fcs/rudder-cmd-norm {2}\r\nset fcs/throttle-cmd-norm {3}\r\nstep\n",
                        roll_out, pitch_out, rudder_out, throttle_out);

                simtime += simstep;

                //Console.Write(cmd);
                var data = Encoding.ASCII.GetBytes(cmd);
                JSBSimSEND.Client.Send(data);
            }

            // Flightgear

            if (RAD_softFlightGear.Checked)
            {
                //if (packetssent % 2 == 0) { return; } // short supply buffer.. seems to reduce lag

                var FlightGear = new byte[4*8]; // StructureToByteArray(fg);

                Array.Copy(BitConverter.GetBytes((double) (roll_out*REV_roll)), 0, FlightGear, 0, 8);
                Array.Copy(BitConverter.GetBytes((double) (pitch_out*REV_pitch*-1)), 0, FlightGear, 8, 8);
                Array.Copy(BitConverter.GetBytes((double) (rudder_out*REV_rudder)), 0, FlightGear, 16, 8);
                Array.Copy(BitConverter.GetBytes((double) (throttle_out)), 0, FlightGear, 24, 8);

                Array.Reverse(FlightGear, 0, 8);
                Array.Reverse(FlightGear, 8, 8);
                Array.Reverse(FlightGear, 16, 8);
                Array.Reverse(FlightGear, 24, 8);

                try
                {
                    XplanesSEND.Send(FlightGear, FlightGear.Length);
                }
                catch (Exception)
                {
                    log.Info("Socket Write failed, FG closed?");
                }
            }

            // Xplanes

            if (RAD_softXplanes.Checked)
            {
                // sending only 1 packet instead of many.

                var Xplane = new byte[5 + 36 + 36];


                Xplane[0] = (byte) 'D';
                Xplane[1] = (byte) 'A';
                Xplane[2] = (byte) 'T';
                Xplane[3] = (byte) 'A';
                Xplane[4] = 0;

                Array.Copy(BitConverter.GetBytes(25), 0, Xplane, 5, 4); // packet index

                Array.Copy(BitConverter.GetBytes(throttle_out), 0, Xplane, 9, 4); // start data
                Array.Copy(BitConverter.GetBytes(throttle_out), 0, Xplane, 13, 4);
                Array.Copy(BitConverter.GetBytes(throttle_out), 0, Xplane, 17, 4);
                Array.Copy(BitConverter.GetBytes(throttle_out), 0, Xplane, 21, 4);

                Array.Copy(BitConverter.GetBytes(-999), 0, Xplane, 25, 4);
                Array.Copy(BitConverter.GetBytes(-999), 0, Xplane, 29, 4);
                Array.Copy(BitConverter.GetBytes(-999), 0, Xplane, 33, 4);
                Array.Copy(BitConverter.GetBytes(-999), 0, Xplane, 37, 4);

                // NEXT ONE - control surfaces

                Array.Copy(BitConverter.GetBytes(11), 0, Xplane, 41, 4); // packet index

                Array.Copy(BitConverter.GetBytes(pitch_out*REV_pitch), 0, Xplane, 45, 4); // start data
                Array.Copy(BitConverter.GetBytes(roll_out*REV_roll), 0, Xplane, 49, 4);
                Array.Copy(BitConverter.GetBytes(rudder_out*REV_rudder), 0, Xplane, 53, 4);
                Array.Copy(BitConverter.GetBytes(-999), 0, Xplane, 57, 4);

                Array.Copy(BitConverter.GetBytes((float) (roll_out*REV_roll*0.5)), 0, Xplane, 61, 4);
                Array.Copy(BitConverter.GetBytes(-999), 0, Xplane, 65, 4);
                Array.Copy(BitConverter.GetBytes(-999), 0, Xplane, 69, 4);
                Array.Copy(BitConverter.GetBytes(-999), 0, Xplane, 73, 4);

                try
                {
                    XplanesSEND.Send(Xplane, Xplane.Length);
                }
                catch (Exception e)
                {
                    log.Info("Xplanes udp send error " + e.Message);
                }
            }
        }

        private void setupXplane()
        {
            if (RAD_softXplanes.Checked)
            {
                // sending only 1 packet instead of many.

                var Xplane = new byte[5 + 4*8];

                Xplane[0] = (byte) 'D';
                Xplane[1] = (byte) 'S';
                Xplane[2] = (byte) 'E';
                Xplane[3] = (byte) 'L';
                Xplane[4] = 0;

                if (CHK_xplane10.Checked)
                {
                    var pos = 5;
                    Xplane[pos] = 0x3;
                    pos += 4;
                    Xplane[pos] = 0x4;
                    pos += 4;
                    Xplane[pos] = 0x6;
                    pos += 4;
                    Xplane[pos] = 0x10;
                    pos += 4;
                    Xplane[pos] = 0x11;
                    pos += 4;
                    Xplane[pos] = 0x12;
                    pos += 4;
                    Xplane[pos] = 0x14;
                    pos += 4;
                    Xplane[pos] = 0x15;
                    pos += 4;
                }
                else
                {
                    var pos = 5;
                    Xplane[pos] = 0x3;
                    pos += 4;
                    Xplane[pos] = 0x4;
                    pos += 4;
                    Xplane[pos] = 0x6;
                    pos += 4;
                    Xplane[pos] = 0x11;
                    pos += 4;
                    Xplane[pos] = 0x12;
                    pos += 4;
                    Xplane[pos] = 0x13;
                    pos += 4;
                    Xplane[pos] = 0x14;
                    pos += 4;
                    Xplane[pos] = 0x15;
                    pos += 4;
                }

                try
                {
                    XplanesSEND.Send(Xplane, Xplane.Length);
                }
                catch (Exception e)
                {
                    log.Info("Xplanes udp send error " + e.Message);
                }
            }
        }

        private byte[] StructureToByteArray(object obj)
        {
            var len = Marshal.SizeOf(obj);

            var arr = new byte[len];

            var ptr = Marshal.AllocHGlobal(len);

            Marshal.StructureToPtr(obj, ptr, true);

            Marshal.Copy(ptr, arr, 0, len);

            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        private void RAD_softXplanes_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void RAD_softFlightGear_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void CHKREV_roll_CheckedChanged(object sender, EventArgs e)
        {
            if (CHKREV_roll.Checked)
            {
                REV_roll = -1;
            }
            else
            {
                REV_roll = 1;
            }
        }

        private void CHKREV_pitch_CheckedChanged(object sender, EventArgs e)
        {
            if (CHKREV_pitch.Checked)
            {
                REV_pitch = -1;
            }
            else
            {
                REV_pitch = 1;
            }
        }

        private void CHKREV_rudder_CheckedChanged(object sender, EventArgs e)
        {
            if (CHKREV_rudder.Checked)
            {
                REV_rudder = -1;
            }
            else
            {
                REV_rudder = 1;
            }
        }

        private void GPSrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GPS_rate = int.Parse(GPSrate.Text); //GPSrate.SelectedItem.ToString());
            }
            catch
            {
            }
        }

        private void OutputLog_TextChanged(object sender, EventArgs e)
        {
            if (OutputLog.TextLength >= 10000)
            {
                OutputLog.Text = OutputLog.Text.Substring(OutputLog.TextLength/2);
            }

            // auto scroll
            OutputLog.SelectionStart = OutputLog.Text.Length;

            OutputLog.ScrollToCaret();

            OutputLog.Refresh();
        }

        private float Constrain(float value, float min, float max)
        {
            if (value > max)
            {
                value = max;
            }
            if (value < min)
            {
                value = min;
            }
            return value;
        }

        private short Constrain(double value, double min, double max)
        {
            if (value > max)
            {
                value = max;
            }
            if (value < min)
            {
                value = min;
            }
            return (short) value;
        }

        public void CreateChart(ZedGraphControl zgc)
        {
            Console.WriteLine("sim CreateChart");

            var myPane = zgc.GraphPane;

            zgc.IsAntiAlias = false;

            // Set the titles and axis labels
            myPane.Title.Text = "Servo Output";
            myPane.XAxis.Title.Text = "Time";
            myPane.YAxis.Title.Text = "Output";

            LineItem myCurve;

            myCurve = myPane.AddCurve("Roll", list, Color.Red, SymbolType.None);

            myCurve = myPane.AddCurve("Pitch", list2, Color.Blue, SymbolType.None);

            myCurve = myPane.AddCurve("Rudder", list3, Color.Green, SymbolType.None);

            myCurve = myPane.AddCurve("Throttle", list4, Color.Orange, SymbolType.None);


            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;

            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 5;

            // Make the Y axis scale red
            //myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
            //myPane.YAxis.Title.FontSpec.FontColor = Color.Red;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = true;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;
            // Manually set the axis range
            //myPane.YAxis.Scale.Min = -1;
            //myPane.YAxis.Scale.Max = 1;

            // Fill the axis background with a gradient
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightGray, 45.0f);

            // Sample at 50ms intervals
            timer_servo_graph.Interval = 200;
            //timer_servo_graph.Enabled = true;
            //timer_servo_graph.Start();


            // Calculate the Axis Scale Ranges
            zgc.AxisChange();

            tickStart = Environment.TickCount;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Make sure that the curvelist has at least one curve
            if (zg1.GraphPane.CurveList.Count <= 0)
                return;

            // Get the first CurveItem in the graph
            var curve = zg1.GraphPane.CurveList[0] as LineItem;
            if (curve == null)
                return;

            // Get the PointPairList
            var list = curve.Points as IPointListEdit;
            // If this is null, it means the reference at curve.Points does not
            // support IPointListEdit, so we won't be able to modify it
            if (list == null)
                return;

            // Time is measured in seconds
            var time = (Environment.TickCount - tickStart)/1000.0;

            // Keep the X scale at a rolling 30 second interval, with one
            // major step between the max X value and the end of the axis
            var xScale = zg1.GraphPane.XAxis.Scale;
            if (time > xScale.Max - xScale.MajorStep)
            {
                xScale.Max = time + xScale.MajorStep;
                xScale.Min = xScale.Max - 30.0;
            }
            // Make sure the Y axis is rescaled to accommodate actual data
            try
            {
                zg1.AxisChange();
            }
            catch
            {
            }
            // Force a redraw
            zg1.Invalidate();
        }

        private void SaveSettings_Click(object sender, EventArgs e)
        {
            xmlconfig(true);
        }

        private void GPSrate_Leave(object sender, EventArgs e)
        {
            // user entered values
            GPSrate_SelectedIndexChanged(sender, e);
        }

        private void GPSrate_KeyDown(object sender, KeyEventArgs e)
        {
            // user entered values
            GPSrate_SelectedIndexChanged(sender, e);
        }

        private void but_advsettings_Click(object sender, EventArgs e)
        {
            InputBox.Show("IP", "Enter Sim pc IP (def 127.0.0.1)", ref simIP);

            InputBox.Show("IP", "Enter SITL pc IP (def 127.0.0.1)", ref SITLIP);

            var temp = simPort.ToString();
            InputBox.Show("Port", "Enter Sim pc Port (def 49000)", ref temp);
            simPort = int.Parse(temp);

            temp = recvPort.ToString();
            InputBox.Show("Port", "Enter Planner pc Port (def 49005)", ref temp);
            recvPort = int.Parse(temp);

            xmlconfig(true);
        }

        private void CHK_quad_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void BUT_startfgquad_Click(object sender, EventArgs e)
        {
            var extra = "";
            using (var ofd = new OpenFileDialog
            {
                Filter = "fgfs|*fgfs*"
            })
            {
                if (File.Exists(@"C:\Program Files (x86)\FlightGear\bin\Win32\fgfs.exe"))
                {
                    ofd.InitialDirectory = @"C:\Program Files (x86)\FlightGear\bin\Win32\";
                    extra = " --fg-root=\"C:\\Program Files (x86)\\FlightGear\\data\"";
                }
                else if (File.Exists(@"C:\Program Files\FlightGear\bin\Win32\fgfs.exe"))
                {
                    ofd.InitialDirectory = @"C:\Program Files\FlightGear\bin\Win32\";
                    extra = " --fg-root=\"C:\\Program Files\\FlightGear\\data\"";
                }
                else if (File.Exists(@"C:\Program Files\FlightGear 2.4.0\bin\Win32\fgfs.exe"))
                {
                    ofd.InitialDirectory = @"C:\Program Files\FlightGear 2.4.0\bin\Win32\";
                    extra = " --fg-root=\"C:\\Program Files\\FlightGear 2.4.0\\data\"";
                }
                else if (File.Exists(@"C:\Program Files (x86)\FlightGear 2.4.0\bin\Win32\fgfs.exe"))
                {
                    ofd.InitialDirectory = @"C:\Program Files (x86)\FlightGear 2.4.0\bin\Win32\";
                    extra = " --fg-root=\"C:\\Program Files (x86)\\FlightGear 2.4.0\\data\"";
                }
                else if (File.Exists(@"/usr/games/fgfs"))
                {
                    ofd.InitialDirectory = @"/usr/games";
                }

                if (File.Exists(Settings.Instance["fgexe"]) || ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        Settings.Instance["fgexe"] = ofd.FileName;
                    }
                    else
                    {
                        ofd.FileName = Settings.Instance["fgexe"];
                    }

                    if (!MainV2.MONO)
                    {
                        extra = " --fg-root=\"" +
                                Path.GetDirectoryName(ofd.FileName.ToLower().Replace("bin\\win32\\", "")) + "\\data\"";
                    }

                    var P = new Process();
                    P.StartInfo.FileName = ofd.FileName;
                    P.StartInfo.Arguments = extra +
                                            @" --geometry=400x300      --aircraft=arducopter      --native-fdm=socket,out,50,127.0.0.1,49005,udp 	 --generic=socket,in,50,127.0.0.1,49000,udp,quadhil 	 --fdm=external 	   --roll=0       --pitch=0       --wind=0@0       --turbulence=0.0       --prop:/sim/frame-rate-throttle-hz111111=30       --timeofday=noon       --shading-flat       --fog-disable       --disable-specular-highlight       --disable-skyblend       --disable-random-objects       --disable-panel       --disable-horizon-effect       --disable-clouds       --disable-anti-alias-hud ";
                    try
                    {
                        P.Start();
                    }
                    catch
                    {
                        CustomMessageBox.Show("Failed to start FlightGear");
                    }
                }
            }
        }

        private void BUT_startfgplane_Click(object sender, EventArgs e)
        {
            var extra = "";
            using (var ofd = new OpenFileDialog
            {
                Filter = "fgfs|*fgfs*"
            })
            {
                if (File.Exists(@"C:\Program Files (x86)\FlightGear\bin\Win32\fgfs.exe"))
                {
                    ofd.InitialDirectory = @"C:\Program Files (x86)\FlightGear\bin\Win32\";
                }
                else if (File.Exists(@"C:\Program Files\FlightGear\bin\Win32\fgfs.exe"))
                {
                    ofd.InitialDirectory = @"C:\Program Files\FlightGear\bin\Win32\";
                }
                else if (File.Exists(@"C:\Program Files\FlightGear 2.4.0\bin\Win32\fgfs.exe"))
                {
                    ofd.InitialDirectory = @"C:\Program Files\FlightGear 2.4.0\bin\Win32\";
                }
                else if (File.Exists(@"C:\Program Files (x86)\FlightGear 2.4.0\bin\Win32\fgfs.exe"))
                {
                    ofd.InitialDirectory = @"C:\Program Files (x86)\FlightGear 2.4.0\bin\Win32\";
                }
                else if (File.Exists(@"/usr/games/fgfs"))
                {
                    ofd.InitialDirectory = @"/usr/games";
                }

                if (File.Exists(""+Settings.Instance["fgexe"]) || ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        Settings.Instance["fgexe"] = ofd.FileName;
                    }
                    else
                    {
                        ofd.FileName = Settings.Instance["fgexe"].ToString();
                    }

                    if (!MainV2.MONO)
                    {
                        extra = " --fg-root=\"" +
                                Path.GetDirectoryName(ofd.FileName.ToLower().Replace("bin\\win32\\", "")) + "\\data\"";
                    }

                    var P = new Process();
                    P.StartInfo.FileName = ofd.FileName;
                    P.StartInfo.Arguments = extra +
                                            @" --geometry=400x300         --native-fdm=socket,out,50,127.0.0.1,49005,udp 	--generic=socket,in,50,127.0.0.1,49000,udp,MAVLink		   --roll=0       --pitch=0       --wind=0@0       --turbulence=0.0       --prop:/sim/frame-rate-throttle-hz=30       --timeofday=noon       --shading-flat       --fog-disable       --disable-specular-highlight       --disable-skyblend       --disable-random-objects       --disable-panel       --disable-horizon-effect       --disable-clouds       --disable-anti-alias-hud ";
                    try
                    {
                        P.Start();
                    }
                    catch
                    {
                        CustomMessageBox.Show("Failed to start FlightGear");
                    }
                }
            }
        }

        private void BUT_startxplane_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog
            {
                Filter = "X-Plane|*X-Plane*"
            })
            {
                try
                {
                    ofd.InitialDirectory = Path.GetDirectoryName(Settings.Instance["xplaneexe"].ToString());
                }
                catch
                {
                }

                if (File.Exists(""+ Settings.Instance["xplaneexe"]) || ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        Settings.Instance["xplaneexe"] = ofd.FileName;
                    }
                    else
                    {
                        ofd.FileName = Settings.Instance["xplaneexe"].ToString();
                    }

                    var P = new Process();
                    P.StartInfo.FileName = ofd.FileName;
                    P.StartInfo.Arguments = "";
                    try
                    {
                        P.Start();
                    }
                    catch
                    {
                        CustomMessageBox.Show("Failed to start XPlanes");
                    }
                }
            }
        }

        private void TXT_rollgain_TextChanged(object sender, EventArgs e)
        {
            updateGains();
        }

        private void TXT_pitchgain_TextChanged(object sender, EventArgs e)
        {
            updateGains();
        }

        private void TXT_ruddergain_TextChanged(object sender, EventArgs e)
        {
            updateGains();
        }

        private void TXT_throttlegain_TextChanged(object sender, EventArgs e)
        {
            updateGains();
        }

        private void updateGains()
        {
            try
            {
                rollgain = int.Parse(TXT_rollgain.Text);
                pitchgain = int.Parse(TXT_pitchgain.Text);
                ruddergain = int.Parse(TXT_ruddergain.Text);
                throttlegain = int.Parse(TXT_throttlegain.Text);
            }
            catch (Exception)
            {
                Invoke((MethodInvoker) delegate { OutputLog.AppendText("Bad Gains!!!\n"); });
            }
        }

        private void CHKdisplayall_CheckedChanged(object sender, EventArgs e)
        {
            displayfull = CHKdisplayall.Checked;

            if (displayfull)
            {
                //this.Width = 651;
                timer_servo_graph.Start();
                zg1.Visible = true;


                CHKgraphpitch.Visible = true;
                CHKgraphroll.Visible = true;
                CHKgraphrudder.Visible = true;
                CHKgraphthrottle.Visible = true;
            }
            else
            {
                //651, 457
                //this.Width = 651;
                //this.Height = 457;

                timer_servo_graph.Stop();
                zg1.Visible = false;

                CHKgraphpitch.Visible = false;
                CHKgraphroll.Visible = false;
                CHKgraphrudder.Visible = false;
                CHKgraphthrottle.Visible = false;
            }
        }

        private void Simulation_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer_servo_graph.Stop();
        }

        private void BUT_sitl_Click(object sender, EventArgs e)
        {
            var form = new SITL();

            form.Show();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct fgIMUData
        {
            // GPS
            public double latitude;
            public double longitude;
            public double altitude;
            public double heading;
            public double velocityN;
            public double velocityE;

            // IMU
            public double accelX;
            public double accelY;
            public double accelZ;
            public double rateRoll;
            public double ratePitch;
            public double rateYaw;

            // trailer
            public uint magic;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct sitl_fdm
        {
            // this is the packet sent by the simulator
            // to the APM executable to update the simulator state
            // All values are little-endian
            public ulong timestamp; // timestamp for lockstep
            public double latitude, longitude; // degrees
            public double altitude; // MSL
            public double heading; // degrees
            public double speedN, speedE, speedD; // m/s
            public double xAccel, yAccel, zAccel; // m/s/s in body frame
            public double rollRate, pitchRate, yawRate; // degrees/s/s in earth frame
            public double rollDeg, pitchDeg, yawDeg; // euler angles, degrees
            public double airspeed; // m/s
            public uint magic; // 0x4c56414f
        };

        //-----------------------------------------------------------------------------
        // Two main data structures are used. This is the first one:
        //
        // This data struct is filled by AeroSIM RC with the simulation data, and sent to the plugin
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TDataFromAeroSimRC
        {
            public ushort nStructSize; // size in bytes of TDataFromAeroSimRC

            //---------------------
            // Integration Time
            //---------------------
            public float Simulation_fIntegrationTimeStep;
            // integration time step in seconds. This is the simulated time since last call to AeroSIMRC_Plugin_Run()

            //---------------------
            // Channels
            //---------------------

            [MarshalAs(
                UnmanagedType.ByValArray,
                SizeConst = AEROSIMRC_MAX_CHANNELS)] public float[] Channel_afValue_TX;

            // [-1, 1] channel positions at TX sticks (i.e. raw stick positions)

            [MarshalAs(
                UnmanagedType.ByValArray,
                SizeConst = AEROSIMRC_MAX_CHANNELS)] public float[] Channel_afValue_RX;

            // [-1, 1] channel positions at RX (i.e. after TX mixes)

            // Use the following constants as indexes for the channel arrays
            // The simulator uses internally the channel numbers for Transmitter Mode 2 (regardless of mode selected by user)
            private const int CH_AILERON = 0;
            private const int CH_ELEVATOR = 1;
            private const int CH_THROTTLE = 2;
            private const int CH_RUDDER = 3;
            private const int CH_5 = 4;
            private const int CH_6 = 5;
            private const int CH_7 = 6;
            private const int CH_PLUGIN_1 = 22; // This channel is mapped by user to any real channel number
            private const int CH_PLUGIN_2 = 23; // This channel is mapped by user to any real channel number

            //---------------------
            // OSD
            //---------------------
            // Video buffer for OSD is a bitmap, 4 bytes per pixel: R G B A; The first 4 bytes are the Top-Left corner pixel
            // The size of the OSD Video Buffer is defined in plugin.txt
            // .OSD_BUFFER_SIZE, in plugin.txt, can be set to one of the following sizes: 512x512, 1024x512 or 1024x1024
            // Set OSD_nWindow_DX and OSD_nWindow_DY in struct TDataToAeroSimRC to the actual size to be displayed
            public IntPtr OSD_pVideoBuffer;

            //---------------------
            // Menu
            //---------------------
            // This variable represent the custom menu status. E.g. 0x000001 means that first menu item is ticked
            // Command  menu item bits are set to 1 when selected, but cleared in the next cycle.
            // Checkbox menu item bits remain 1 until unchecked by user, or cleared in TDataToAeroSimRC::Menu_nFlags_MenuItem_New_CheckBox_Status
            public uint Menu_nFlags_MenuItem_Status;

            //---------------------
            // Model Initial Position in current scenario
            //---------------------
            public float Scenario_fInitialModelPosX;
            public float Scenario_fInitialModelPosY;
            public float Scenario_fInitialModelPosZ; // (m) Model Initial Position on runway
            public float Scenario_fInitialModelHeading;
            public float Scenario_fInitialModelPitch;
            public float Scenario_fInitialModelRoll; // (m) Model Initial Attitude on runway

            //---------------------
            // WayPoints
            // The Description string can be freely used to add more information to the waypoint such as Altitude, WP Type (Overfly, Landing, CAP), Bearing, etc.
            //---------------------
            public float Scenario_fWPHome_X;
            public float Scenario_fWPHome_Y;
            public float Scenario_fWPHome_Lat;
            public float Scenario_fWPHome_Long;
            private readonly IntPtr Scenario_strWPHome_Description; // (m, deg, string)
            public float Scenario_fWPA_X;
            public float Scenario_fWPA_Y;
            public float Scenario_fWPA_Lat;
            public float Scenario_fWPA_Long;
            private readonly IntPtr Scenario_strWPA_Description; // (m, deg, string)
            public float Scenario_fWPB_X;
            public float Scenario_fWPB_Y;
            public float Scenario_fWPB_Lat;
            public float Scenario_fWPB_Long;
            private readonly IntPtr Scenario_strWPB_Description; // (m, deg, string)
            public float Scenario_fWPC_X;
            public float Scenario_fWPC_Y;
            public float Scenario_fWPC_Lat;
            public float Scenario_fWPC_Long;
            private readonly IntPtr Scenario_strWPC_Description; // (m, deg, string)
            public float Scenario_fWPD_X;
            public float Scenario_fWPD_Y;
            public float Scenario_fWPD_Lat;
            public float Scenario_fWPD_Long;
            private readonly IntPtr Scenario_strWPD_Description; // (m, deg, string)

            //---------------------
            // Model data
            //---------------------
            public float Model_fPosX;
            public float Model_fPosY;
            public float Model_fPosZ; // m      Model absolute position in scenario (X=Right, Y=Front, Z=Up)
            public float Model_fVelX;
            public float Model_fVelY;
            public float Model_fVelZ; // m/s    Model velocity
            public float Model_fAngVelX;
            public float Model_fAngVelY;
            public float Model_fAngVelZ; // rad/s  Model angular velocity (useful to implement gyroscopes)
            public float Model_fAccelX;
            public float Model_fAccelY;
            public float Model_fAccelZ; // m/s/s  Model acceleration (useful to implement accelerometers)

            public double Model_fLatitude;
            public double Model_fLongitude; // deg    Model Position in Lat/Long coordinates

            public float Model_fHeightAboveTerrain; // m

            public float Model_fHeading; // rad [-PI,   PI  ] 0 = North, PI/2 = East, PI = South, - PI/2 = West
            public float Model_fPitch; // rad [-PI/2, PI/2] Positive pitch when nose up
            public float Model_fRoll; // rad [-PI,   PI  ] Positive roll when right wing Up

            // Wind
            public float Model_fWindVelX;
            public float Model_fWindVelY;

            public float Model_fWindVelZ;
            // m/s   Velocity of the wind (with gusts) at model position (useful to compute air vel)

            // Engine/Motor Revs per minute
            public float Model_fEngine1_RPM;
            public float Model_fEngine2_RPM;
            public float Model_fEngine3_RPM;
            public float Model_fEngine4_RPM;

            // Battery (electric models)
            public float Model_fBatteryVoltage; // V
            public float Model_fBatteryCurrent; // A
            public float Model_fBatteryConsumedCharge; // Ah
            public float Model_fBatteryCapacity; // Ah

            // Fuel (gas & jet models)
            public float Model_fFuelConsumed; // l
            public float Model_fFuelTankCapacity; // l

            // Ver > 3.81
            // Screen size
            public short Win_nScreenSizeDX;
            public short Win_nScreenSizeDY; // Screen Size, used to resize and reposition simulator window

            // Model Orientation Matrix
            public float Model_fAxisRight_x;
            public float Model_fAxisRight_y;
            public float Model_fAxisRight_z;
            public float Model_fAxisFront_x;
            public float Model_fAxisFront_y;
            public float Model_fAxisFront_z;
            public float Model_fAxisUp_x;
            public float Model_fAxisUp_y;
            public float Model_fAxisUp_z;

            // Model data in body frame coordinates (X=Right, Y=Front, Z=Up)
            public float Model_fVel_Body_X;
            public float Model_fVel_Body_Y;
            public float Model_fVel_Body_Z; // m/s    Model velocity in body coordinates
            public float Model_fAngVel_Body_X;
            public float Model_fAngVel_Body_Y;
            public float Model_fAngVel_Body_Z; // rad/s  Model angular velocity in body coordinates
            public float Model_fAccel_Body_X;
            public float Model_fAccel_Body_Y;
            public float Model_fAccel_Body_Z; // m/s/s  Model acceleration in body coordinates

            // Size in bytes of the allocated OSD buffer (size is defined in plugin.txt in .OSD_BUFFER_SIZE)
            // The buffer size is 4 x .OSD_VIDEO_BUFFER_SIZE (e.g. 4x512x512 = 1048576 bytes), so you should not write outside that memory.
            public uint OSD_nSizeOfVideoBuffer;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FGNetFDM
        {
            public uint version; // increment when data values change
            public uint padding; // padding

            // Positions
            public double longitude; // geodetic (radians)
            public double latitude; // geodetic (radians)
            public double altitude; // above sea level (meters)
            public float agl; // above ground level (meters)
            public float phi; // roll (radians)
            public float theta; // pitch (radians)
            public float psi; // yaw or true heading (radians)
            public float alpha; // angle of attack (radians)
            public float beta; // side slip angle (radians)

            // Velocities
            public float phidot; // roll rate (radians/sec)
            public float thetadot; // pitch rate (radians/sec)
            public float psidot; // yaw rate (radians/sec)
            public float vcas; // calibrated airspeed
            public float climb_rate; // feet per second
            public float v_north; // north velocity in local/body frame, fps
            public float v_east; // east velocity in local/body frame, fps
            public float v_down; // down/vertical velocity in local/body frame, fps
            public float v_wind_body_north; // north velocity in local/body frame
            // relative to local airmass, fps
            public float v_wind_body_east; // east velocity in local/body frame
            // relative to local airmass, fps
            public float v_wind_body_down; // down/vertical velocity in local/body
            // frame relative to local airmass, fps

            // Accelerations
            public float A_X_pilot; // X accel in body frame ft/sec^2
            public float A_Y_pilot; // Y accel in body frame ft/sec^2
            public float A_Z_pilot; // Z accel in body frame ft/sec^2

            // Stall
            public float stall_warning; // 0.0 - 1.0 indicating the amount of stall
            public float slip_deg; // slip ball deflection


            // Pressure

            // Engine status
            private readonly uint num_engines; // Number of valid engines

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_ENGINES)] private readonly uint[] eng_state;
            // Engine state (off, cranking, running)

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_ENGINES)] private readonly float[] rpm;
            // Engine RPM rev/min

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_ENGINES)] private readonly float[] fuel_flow;
            // Fuel flow gallons/hr

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_ENGINES)] private readonly float[] fuel_px;
            // Fuel pressure psi

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_ENGINES)] private readonly float[] egt;
            // Exhuast gas temp deg F

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_ENGINES)] private readonly float[] cht;
            // Cylinder head temp deg F

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_ENGINES)] private readonly float[] mp_osi;
            // Manifold pressure

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_ENGINES)] private readonly float[] tit;
            // Turbine Inlet Temperature

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_ENGINES)] private readonly float[] oil_temp;
            // Oil temp deg F

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_ENGINES)] private readonly float[] oil_px;
            // Oil pressure psi

            // Consumables
            private readonly uint num_tanks; // Max number of fuel tanks
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_TANKS)] private readonly float[] fuel_quantity;

            // Gear status
            private readonly uint num_wheels;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_WHEELS)] private readonly uint[] wow;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_WHEELS)] private readonly float[] gear_pos;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_WHEELS)] private readonly float[] gear_steer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FG_MAX_WHEELS)] private readonly float[] gear_compression;

            // Environment
            private readonly uint cur_time; // current unix time
            // FIXME: make this uint64_t before 2038
            private readonly int warp; // offset in seconds to unix time
            private readonly float visibility; // visibility in meters (for env. effects)

            // Control surface positions (normalized values)
            private readonly float elevator;
            private readonly float elevator_trim_tab;
            private readonly float left_flap;
            private readonly float right_flap;
            private readonly float left_aileron;
            private readonly float right_aileron;
            private readonly float rudder;
            private readonly float nose_wheel;
            private readonly float speedbrake;
            private readonly float spoilers;
        }
    }
}