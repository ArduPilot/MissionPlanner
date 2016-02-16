using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections; // hashs
using System.Diagnostics; // stopwatch
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Drawing;
using System.Threading;
using MissionPlanner.Controls;
using System.ComponentModel;
using System.Linq;
using log4net;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System.Windows.Forms;
using MissionPlanner.HIL;
using MissionPlanner.Mavlink;

namespace MissionPlanner
{
    public class MAVLinkInterface : MAVLink, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ICommsSerial BaseStream { get; set; }

        public ICommsSerial MirrorStream { get; set; }
        public bool MirrorStreamWrite { get; set; }

        public event EventHandler ParamListChanged;

        public event EventHandler MavChanged;

        public event EventHandler CommsClose;

        const int gcssysid = 255;

        /// <summary>
        /// used to prevent comport access for exclusive use
        /// </summary>
        public bool giveComport
        {
            get { return _giveComport; }
            set { _giveComport = value; }
        }

        volatile bool _giveComport = false;

        DateTime lastparamset = DateTime.MinValue;

        internal string plaintxtline = "";
        string buildplaintxtline = "";

        public bool ReadOnly = false;

        public TerrainFollow Terrain;

        public event ProgressEventHandler Progress;

        int _sysidcurrent = 0;

        public int sysidcurrent
        {
            get { return _sysidcurrent; }
            set
            {
                if (_sysidcurrent == value)
                    return;
                _sysidcurrent = value;
                if (MavChanged != null) MavChanged(this, null);
            }
        }

        int _compidcurrent = 0;

        public int compidcurrent
        {
            get { return _compidcurrent; }
            set
            {
                if (_compidcurrent == value)
                    return;
                _compidcurrent = value;
                if (MavChanged != null) MavChanged(this, null);
            }
        }

        public MAVList MAVlist = new MAVList();

        public MAVState MAV
        {
            get { return MAVlist[sysidcurrent, compidcurrent]; }
            set { MAVlist[sysidcurrent, compidcurrent] = value; }
        }

        public double CONNECT_TIMEOUT_SECONDS = 30;

        /// <summary>
        /// progress form to handle connect and param requests
        /// </summary>
        ProgressReporterDialogue frmProgressReporter;

        /// <summary>
        /// used for outbound packet sending
        /// </summary>
        internal int packetcount = 0;

        private readonly Subject<int> _bytesReceivedSubj = new Subject<int>();
        private readonly Subject<int> _bytesSentSubj = new Subject<int>();

        /// <summary>
        /// Observable of the count of bytes received, notified when the bytes themselves are received
        /// </summary>
        public IObservable<int> BytesReceived
        {
            get { return _bytesReceivedSubj; }
        }

        /// <summary>
        /// Observable of the count of bytes sent, notified when the bytes themselves are received
        /// </summary>
        public IObservable<int> BytesSent
        {
            get { return _bytesSentSubj; }
        }

        /// <summary>
        /// Observable of the count of packets skipped (on reception), 
        /// calculated from periods where received packet sequence is not
        /// contiguous
        /// </summary>
        public Subject<int> WhenPacketLost { get; set; }

        public Subject<int> WhenPacketReceived { get; set; }

        /// <summary>
        /// used as a serial port write lock
        /// </summary>
        volatile object objlock = new object();

        /// <summary>
        /// used for a readlock on readpacket
        /// </summary>
        volatile object readlock = new object();

        /// <summary>
        /// mavlink version
        /// </summary>
        byte mavlinkversion = 0;

        private MavlinkParse mavparse = new MavlinkParse();

        /// <summary>
        /// turns on console packet display
        /// </summary>
        public bool debugmavlink { get; set; }

        /// <summary>
        /// enabled read from file mode
        /// </summary>
        public bool logreadmode { get; set; }

        public DateTime lastlogread { get; set; }
        public BinaryReader logplaybackfile { get; set; }
        public BufferedStream logfile { get; set; }
        public BufferedStream rawlogfile { get; set; }

        int bps1 = 0;
        int bps2 = 0;
        public DateTime bpstime { get; set; }


        public MAVLinkInterface()
        {
            // init fields
            this.BaseStream = new SerialPort();
            this.packetcount = 0;
            this._bytesReceivedSubj = new Subject<int>();
            this._bytesSentSubj = new Subject<int>();
            this.WhenPacketLost = new Subject<int>();
            this.WhenPacketReceived = new Subject<int>();
            this.readlock = new object();

            this.mavlinkversion = 0;

            this.debugmavlink = false;
            this.logreadmode = false;
            this.lastlogread = DateTime.MinValue;
            this.logplaybackfile = null;
            this.logfile = null;
            this.rawlogfile = null;
            this.bps1 = 0;
            this.bps2 = 0;
            this.bpstime = DateTime.MinValue;


            this.lastbad = new byte[2];
        }

        public MAVLinkInterface(Stream logfileStream)
            : this()
        {
            logplaybackfile = new BinaryReader(logfileStream);
            logreadmode = true;
        }

        public void Close()
        {
            try
            {
                if (logfile != null)
                    logfile.Close();
            }
            catch
            {
            }
            try
            {
                if (rawlogfile != null)
                    rawlogfile.Close();
            }
            catch
            {
            }
            try
            {
                if (logplaybackfile != null)
                    logplaybackfile.Close();
            }
            catch
            {
            }

            try
            {
                if (BaseStream.IsOpen)
                    BaseStream.Close();
            }
            catch
            {
            }

            try
            {
                if (CommsClose != null)
                    CommsClose(this, null);
            }
            catch
            {
            }
        }

        public void Open()
        {
            Open(false);
        }

        public void Open(bool getparams, bool skipconnectedcheck = false)
        {
            if (BaseStream.IsOpen && !skipconnectedcheck)
                return;

            MAVlist.Clear();

            frmProgressReporter = new ProgressReporterDialogue
            {
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
                Text = Strings.ConnectingMavlink
            };

            if (getparams)
            {
                frmProgressReporter.DoWork += FrmProgressReporterDoWorkAndParams;
            }
            else
            {
                frmProgressReporter.DoWork += FrmProgressReporterDoWorkNOParams;
            }
            frmProgressReporter.UpdateProgressAndStatus(-1, Strings.MavlinkConnecting);
            ThemeManager.ApplyThemeTo(frmProgressReporter);

            frmProgressReporter.RunBackgroundOperationAsync();

            frmProgressReporter.Dispose();

            if (ParamListChanged != null)
            {
                ParamListChanged(this, null);
            }
        }

        void FrmProgressReporterDoWorkAndParams(object sender, ProgressWorkerEventArgs e, object passdata = null)
        {
            OpenBg(sender, true, e);
        }

        void FrmProgressReporterDoWorkNOParams(object sender, ProgressWorkerEventArgs e, object passdata = null)
        {
            OpenBg(sender, false, e);
        }

        private void OpenBg(object PRsender, bool getparams, ProgressWorkerEventArgs progressWorkerEventArgs)
        {
            frmProgressReporter.UpdateProgressAndStatus(-1, Strings.MavlinkConnecting);

            giveComport = true;

            // allow settings to settle - previous dtr 
            System.Threading.Thread.Sleep(1000);

            Terrain = new TerrainFollow(this);

            bool hbseen = false;

            try
            {
                BaseStream.ReadBufferSize = 16*1024;

                lock (objlock) // so we dont have random traffic
                {
                    log.Info("Open port with " + BaseStream.PortName + " " + BaseStream.BaudRate);

                    BaseStream.Open();

                    BaseStream.DiscardInBuffer();

                    // other boards seem to have issues if there is no delay? posible bootloader timeout issue
                    Thread.Sleep(1000);
                }

                byte[] buffer = new byte[0];
                byte[] buffer1 = new byte[0];

                DateTime start = DateTime.Now;
                DateTime deadline = start.AddSeconds(CONNECT_TIMEOUT_SECONDS);

                var countDown = new System.Timers.Timer {Interval = 1000, AutoReset = false};
                countDown.Elapsed += (sender, e) =>
                {
                    int secondsRemaining = (deadline - e.SignalTime).Seconds;
                    frmProgressReporter.UpdateProgressAndStatus(-1, string.Format(Strings.Trying, secondsRemaining));
                    if (secondsRemaining > 0) countDown.Start();
                };
                countDown.Start();

                // px4 native
                BaseStream.WriteLine("sh /etc/init.d/rc.usb");

                int count = 0;

                while (true)
                {
                    if (progressWorkerEventArgs.CancelRequested)
                    {
                        progressWorkerEventArgs.CancelAcknowledged = true;
                        countDown.Stop();
                        if (BaseStream.IsOpen)
                            BaseStream.Close();
                        giveComport = false;
                        return;
                    }

                    log.Info(DateTime.Now.Millisecond + " Start connect loop ");

                    if (DateTime.Now > deadline)
                    {
                        //if (Progress != null)
                        //    Progress(-1, "No Heartbeat Packets");
                        countDown.Stop();
                        this.Close();

                        if (hbseen)
                        {
                            progressWorkerEventArgs.ErrorMessage = Strings.Only1Hb;
                            throw new Exception(Strings.Only1HbD);
                        }
                        else
                        {
                            progressWorkerEventArgs.ErrorMessage = "No Heartbeat Packets Received";
                            throw new Exception(@"Can not establish a connection\n
Please check the following
1. You have firmware loaded
2. You have the correct serial port selected
3. PX4 - You have the microsd card installed
4. Try a diffrent usb port\n\n" +
                                                "No Mavlink Heartbeat Packets where read from this port - Verify Baud Rate and setup\nMission Planner waits for 2 valid heartbeat packets before connecting");
                        }
                    }

                    System.Threading.Thread.Sleep(1);

                    // can see 2 heartbeat packets at any time, and will connect - was one after the other

                    if (buffer.Length == 0)
                        buffer = getHeartBeat();

                    System.Threading.Thread.Sleep(1);

                    if (buffer1.Length == 0)
                        buffer1 = getHeartBeat();


                    if (buffer.Length > 0 || buffer1.Length > 0)
                        hbseen = true;

                    count++;

                    // 2 hbs that match
                    if (buffer.Length > 5 && buffer1.Length > 5 && buffer[3] == buffer1[3] && buffer[4] == buffer1[4])
                    {
                        mavlink_heartbeat_t hb = buffer.ByteArrayToStructure<mavlink_heartbeat_t>(6);

                        if (hb.type != (byte) MAVLink.MAV_TYPE.GCS)
                        {
                            SetupMavConnect(buffer[3], buffer[4], buffer[2], hb);
                            break;
                        }
                    }

                    // 2 hb's that dont match. more than one sysid here
                    if (buffer.Length > 5 && buffer1.Length > 5 && (buffer[3] != buffer1[3] || buffer[4] != buffer1[4]))
                    {
                        mavlink_heartbeat_t hb = buffer.ByteArrayToStructure<mavlink_heartbeat_t>(6);

                        if (hb.type != (byte) MAVLink.MAV_TYPE.ANTENNA_TRACKER && hb.type != (byte) MAVLink.MAV_TYPE.GCS)
                        {
                            SetupMavConnect(buffer[3], buffer[4], buffer[2], hb);
                            break;
                        }

                        hb = buffer1.ByteArrayToStructure<mavlink_heartbeat_t>(6);

                        if (hb.type != (byte) MAVLink.MAV_TYPE.ANTENNA_TRACKER && hb.type != (byte) MAVLink.MAV_TYPE.GCS)
                        {
                            SetupMavConnect(buffer1[3], buffer1[4], buffer1[2], hb);
                            break;
                        }
                    }
                }

                countDown.Stop();

                getVersion();

                frmProgressReporter.UpdateProgressAndStatus(0,
                    "Getting Params.. (sysid " + MAV.sysid + " compid " + MAV.compid + ") ");

                if (getparams)
                {
                    getParamListBG();
                }

                byte[] temp = ASCIIEncoding.ASCII.GetBytes("Mission Planner " + Application.ProductVersion + "\0");
                Array.Resize(ref temp, 50);
                generatePacket((byte) MAVLINK_MSG_ID.STATUSTEXT,
                    new mavlink_statustext_t() {severity = (byte) MAV_SEVERITY.INFO, text = temp});

                if (frmProgressReporter.doWorkArgs.CancelAcknowledged == true)
                {
                    giveComport = false;
                    if (BaseStream.IsOpen)
                        BaseStream.Close();
                    return;
                }
            }
            catch (Exception e)
            {
                try
                {
                    BaseStream.Close();
                }
                catch
                {
                }
                giveComport = false;
                if (string.IsNullOrEmpty(progressWorkerEventArgs.ErrorMessage))
                    progressWorkerEventArgs.ErrorMessage = Strings.ConnectFailed;
                log.Error(e);
                throw;
            }
            //frmProgressReporter.Close();
            giveComport = false;
            frmProgressReporter.UpdateProgressAndStatus(100, Strings.Done);
            log.Info("Done open " + MAV.sysid + " " + MAV.compid);
            MAV.packetslost = 0;
            MAV.synclost = 0;
        }

        void SetupMavConnect(byte sysid, byte compid, byte mgsno, mavlink_heartbeat_t hb)
        {
            sysidcurrent = sysid;
            compidcurrent = compid;

            mavlinkversion = hb.mavlink_version;
            MAV.aptype = (MAV_TYPE) hb.type;
            MAV.apname = (MAV_AUTOPILOT) hb.autopilot;

            setAPType(sysid, compid);

            MAV.sysid = sysid;
            MAV.compid = compid;
            MAV.recvpacketcount = mgsno;
            log.InfoFormat("ID sys {0} comp {1} ver{2} type {3} name {4}", MAV.sysid, MAV.compid, mavlinkversion,
                MAV.aptype.ToString(), MAV.apname.ToString());
        }

        public byte[] getHeartBeat()
        {
            giveComport = true;
            DateTime start = DateTime.Now;
            int readcount = 0;
            while (true)
            {
                byte[] buffer = readPacket();
                readcount++;
                if (buffer.Length > 5)
                {
                    //log.Info("getHB packet received: " + buffer.Length + " btr " + BaseStream.BytesToRead + " type " + buffer[5] );
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.HEARTBEAT)
                    {
                        mavlink_heartbeat_t hb = buffer.ByteArrayToStructure<mavlink_heartbeat_t>(6);

                        if (hb.type != (byte) MAVLink.MAV_TYPE.GCS)
                        {
                            SetupMavConnect(buffer[3], buffer[4], buffer[2], hb);

                            giveComport = false;
                            return buffer;
                        }
                    }
                }
                if (DateTime.Now > start.AddMilliseconds(2200) || readcount > 200) // was 1200 , now 2.2 sec
                {
                    giveComport = false;
                    return new byte[0];
                }
            }
        }

        public void sendPacket(object indata)
        {
            bool validPacket = false;
            byte a = 0;
            foreach (Type ty in MAVLink.MAVLINK_MESSAGE_INFO)
            {
                if (ty == indata.GetType())
                {
                    validPacket = true;
                    generatePacket(a, indata);
                    return;
                }
                a++;
            }
            if (!validPacket)
            {
                log.Info("Mavlink : NOT VALID PACKET sendPacket() " + indata.GetType().ToString());
            }
        }

        /// <summary>
        /// Generate a Mavlink Packet and write to serial
        /// </summary>
        /// <param name="messageType">type number = MAVLINK_MSG_ID</param>
        /// <param name="indata">struct of data</param>
        void generatePacket(byte messageType, object indata)
        {
            if (!BaseStream.IsOpen)
            {
                return;
            }

            if (ReadOnly)
            {
                // allow these messages
                if (messageType == (byte) MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_LIST ||
                    messageType == (byte) MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_PARTIAL_LIST ||
                    messageType == (byte) MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST ||
                    messageType == (byte) MAVLink.MAVLINK_MSG_ID.PARAM_REQUEST_LIST ||
                    messageType == (byte) MAVLink.MAVLINK_MSG_ID.PARAM_REQUEST_READ ||
                    messageType == (byte) MAVLink.MAVLINK_MSG_ID.RALLY_FETCH_POINT ||
                    messageType == (byte) MAVLink.MAVLINK_MSG_ID.FENCE_FETCH_POINT
                    )
                {
                }
                else
                {
                    return;
                }
            }

            lock (objlock)
            {
                byte[] data;

                data = MavlinkUtil.StructureToByteArray(indata);

                //Console.WriteLine(DateTime.Now + " PC Doing req "+ messageType + " " + this.BytesToRead);
                byte[] packet = new byte[data.Length + 6 + 2];

                packet[0] = 254;
                packet[1] = (byte) data.Length;
                packet[2] = (byte) packetcount;

                packetcount++;

                packet[3] = gcssysid; // this is always 255 - MYGCS
                packet[4] = (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
                packet[5] = messageType;

                int i = 6;
                foreach (byte b in data)
                {
                    packet[i] = b;
                    i++;
                }

                ushort checksum = MavlinkCRC.crc_calculate(packet, packet[1] + 6);

                checksum = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[messageType], checksum);

                byte ck_a = (byte) (checksum & 0xFF); ///< High byte
                byte ck_b = (byte) (checksum >> 8); ///< Low byte

                packet[i] = ck_a;
                i += 1;
                packet[i] = ck_b;
                i += 1;

                if (BaseStream.IsOpen)
                {
                    BaseStream.Write(packet, 0, i);
                    _bytesSentSubj.OnNext(i);
                }

                try
                {
                    if (logfile != null && logfile.CanWrite)
                    {
                        lock (logfile)
                        {
                            byte[] datearray =
                                BitConverter.GetBytes(
                                    (UInt64) ((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds*1000));
                            Array.Reverse(datearray);
                            logfile.Write(datearray, 0, datearray.Length);
                            logfile.Write(packet, 0, i);
                        }
                    }
                }
                catch
                {
                }
                /*
                if (messageType == (byte)MAVLink.MSG_NAMES.REQUEST_DATA_STREAM)
                {
                    try
                    {
                        BinaryWriter bw = new BinaryWriter(File.OpenWrite("serialsent.raw"));
                        bw.Seek(0, SeekOrigin.End);
                        bw.Write(packet, 0, i);
                        bw.Write((byte)'\n');
                        bw.Close();
                    }
                    catch { } // been getting errors from this. people must have it open twice.
                }*/
            }
        }

        public bool Write(string line)
        {
            lock (objlock)
            {
                BaseStream.Write(line);
            }
            _bytesSentSubj.OnNext(line.Length);
            return true;
        }

        /// <summary>
        /// set param on apm, used for param rename
        /// </summary>
        /// <param name="paramname"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool setParam(string[] paramnames, double value)
        {
            foreach (string paramname in paramnames)
            {
                if (setParam(paramname, value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Set parameter on apm
        /// </summary>
        /// <param name="paramname">name as a string</param>
        /// <param name="value"></param>
        public bool setParam(string paramname, double value, bool force = false)
        {
            if (!MAV.param.ContainsKey(paramname))
            {
                log.Warn("Trying to set Param that doesnt exist " + paramname + "=" + value);
                return false;
            }

            if (MAV.param[paramname].Value == value && !force)
            {
                log.Warn("setParam " + paramname + " not modified as same");
                return true;
            }

            giveComport = true;

            // param type is set here, however it is always sent over the air as a float 100int = 100f.
            var req = new mavlink_param_set_t
            {
                target_system = MAV.sysid,
                target_component = MAV.compid,
                param_type = (byte) MAV.param_types[paramname]
            };

            byte[] temp = Encoding.ASCII.GetBytes(paramname);

            Array.Resize(ref temp, 16);
            req.param_id = temp;
            if (MAV.apname == MAV_AUTOPILOT.ARDUPILOTMEGA)
            {
                req.param_value = new MAVLinkParam(paramname, value, (MAV_PARAM_TYPE.REAL32)).float_value;
            }
            else
            {
                req.param_value =
                    new MAVLinkParam(paramname, value, (MAV_PARAM_TYPE) MAV.param_types[paramname]).float_value;
            }

            generatePacket((byte) MAVLINK_MSG_ID.PARAM_SET, req);

            log.InfoFormat("setParam '{0}' = '{1}' sysid {2} compid {3}", paramname, req.param_value, MAV.sysid,
                MAV.compid);

            DateTime start = DateTime.Now;
            int retrys = 3;

            while (true)
            {
                if (!(start.AddMilliseconds(700) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("setParam Retry " + retrys);
                        generatePacket((byte) MAVLINK_MSG_ID.PARAM_SET, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - setParam " + paramname);
                }

                byte[] buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.PARAM_VALUE)
                    {
                        mavlink_param_value_t par = buffer.ByteArrayToStructure<mavlink_param_value_t>(6);

                        string st = System.Text.ASCIIEncoding.ASCII.GetString(par.param_id);

                        int pos = st.IndexOf('\0');

                        if (pos != -1)
                        {
                            st = st.Substring(0, pos);
                        }

                        if (st != paramname)
                        {
                            log.InfoFormat("MAVLINK bad param response - {0} vs {1}", paramname, st);
                            continue;
                        }

                        log.Info("setParam gotback " + st + " : " + par.param_value);

                        if (MAV.apname == MAV_AUTOPILOT.ARDUPILOTMEGA)
                        {
                            MAV.param[st] = new MAVLinkParam(st, par.param_value, MAV_PARAM_TYPE.REAL32);
                        }
                        else
                        {
                            MAV.param[st] = new MAVLinkParam(st, par.param_value, (MAV_PARAM_TYPE) par.param_type);
                        }

                        lastparamset = DateTime.Now;

                        giveComport = false;
                        //System.Threading.Thread.Sleep(100);//(int)(8.5 * 5)); // 8.5ms per byte
                        return true;
                    }
                }
            }
        }

        /*
        public Bitmap getImage()
        {
            MemoryStream ms = new MemoryStream();

        }
        */

        public void getParamList()
        {
            frmProgressReporter = new ProgressReporterDialogue
            {
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
                Text = Strings.GettingParams + " " + sysidcurrent
            };

            frmProgressReporter.DoWork += FrmProgressReporterGetParams;
            frmProgressReporter.UpdateProgressAndStatus(-1, Strings.GettingParamsD);
            ThemeManager.ApplyThemeTo(frmProgressReporter);

            frmProgressReporter.RunBackgroundOperationAsync();

            frmProgressReporter.Dispose();

            if (ParamListChanged != null)
            {
                ParamListChanged(this, null);
            }
        }

        void FrmProgressReporterGetParams(object sender, ProgressWorkerEventArgs e, object passdata = null)
        {
            getParamListBG();
        }

        /// <summary>
        /// Get param list from apm
        /// </summary>
        /// <returns></returns>
        private Hashtable getParamListBG()
        {
            giveComport = true;
            List<int> indexsreceived = new List<int>();

            // create new list so if canceled we use the old list
            MAVLinkParamList newparamlist = new MAVLinkParamList();

            int param_count = 0;
            int param_total = 1;

            mavlink_param_request_list_t req = new mavlink_param_request_list_t();
            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;

            generatePacket((byte) MAVLINK_MSG_ID.PARAM_REQUEST_LIST, req);

            DateTime start = DateTime.Now;
            DateTime restart = DateTime.Now;

            DateTime lastmessage = DateTime.MinValue;

            //hires.Stopwatch stopwatch = new hires.Stopwatch();
            int packets = 0;
            bool onebyone = false;
            DateTime lastonebyone = DateTime.MinValue;

            do
            {
                if (frmProgressReporter.doWorkArgs.CancelRequested)
                {
                    frmProgressReporter.doWorkArgs.CancelAcknowledged = true;
                    giveComport = false;
                    frmProgressReporter.doWorkArgs.ErrorMessage = "User Canceled";
                    return MAV.param;
                }

                // 4 seconds between valid packets
                if (!(start.AddMilliseconds(4000) > DateTime.Now) && !logreadmode)
                {
                    onebyone = true;

                    if (lastonebyone.AddMilliseconds(600) < DateTime.Now)
                    {
                        log.Info("Get param 1 by 1 - got " + indexsreceived.Count + " of " + param_total);

                        int queued = 0;
                        // try getting individual params
                        for (short i = 0; i <= (param_total - 1); i++)
                        {
                            if (!indexsreceived.Contains(i))
                            {
                                if (frmProgressReporter.doWorkArgs.CancelRequested)
                                {
                                    frmProgressReporter.doWorkArgs.CancelAcknowledged = true;
                                    giveComport = false;
                                    frmProgressReporter.doWorkArgs.ErrorMessage = "User Canceled";
                                    return MAV.param;
                                }

                                // prevent dropping out of this get params loop
                                try
                                {
                                    queued++;

                                    mavlink_param_request_read_t req2 = new mavlink_param_request_read_t();
                                    req2.target_system = MAV.sysid;
                                    req2.target_component = MAV.compid;
                                    req2.param_index = i;
                                    req2.param_id = new byte[] {0x0};

                                    Array.Resize(ref req2.param_id, 16);

                                    generatePacket((byte) MAVLINK_MSG_ID.PARAM_REQUEST_READ, req2);

                                    if (queued >= 10)
                                    {
                                        lastonebyone = DateTime.Now;
                                        break;
                                    }
                                }
                                catch (Exception excp)
                                {
                                    log.Info("GetParam Failed index: " + i + " " + excp);
                                    throw excp;
                                }
                            }
                        }
                    }
                }

                //Console.WriteLine(DateTime.Now.Millisecond + " gp0 ");

                byte[] buffer = readPacket();
                //Console.WriteLine(DateTime.Now.Millisecond + " gp1 ");
                if (buffer.Length > 5)
                {
                    packets++;
                    // stopwatch.Start();
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.PARAM_VALUE)
                    {
                        restart = DateTime.Now;
                        // if we are doing one by one dont update start time
                        if (!onebyone)
                            start = DateTime.Now;

                        mavlink_param_value_t par = buffer.ByteArrayToStructure<mavlink_param_value_t>(6);

                        // set new target
                        param_total = (par.param_count);

                        string paramID = System.Text.ASCIIEncoding.ASCII.GetString(par.param_id);

                        int pos = paramID.IndexOf('\0');
                        if (pos != -1)
                        {
                            paramID = paramID.Substring(0, pos);
                        }

                        // check if we already have it
                        if (indexsreceived.Contains(par.param_index))
                        {
                            log.Info("Already got " + (par.param_index) + " '" + paramID + "'");
                            this.frmProgressReporter.UpdateProgressAndStatus((indexsreceived.Count*100)/param_total,
                                "Already Got param " + paramID);
                            continue;
                        }

                        //Console.WriteLine(DateTime.Now.Millisecond + " gp2 ");

                        if (!MainV2.MONO)
                            log.Info(DateTime.Now.Millisecond + " got param " + (par.param_index) + " of " +
                                     (par.param_count) + " name: " + paramID);

                        //Console.WriteLine(DateTime.Now.Millisecond + " gp2a ");

                        if (MAV.apname == MAV_AUTOPILOT.ARDUPILOTMEGA)
                        {
                            newparamlist[paramID] = new MAVLinkParam(paramID, par.param_value, MAV_PARAM_TYPE.REAL32);
                        }
                        else
                        {
                            newparamlist[paramID] = new MAVLinkParam(paramID, par.param_value,
                                (MAV_PARAM_TYPE) par.param_type);
                        }

                        //Console.WriteLine(DateTime.Now.Millisecond + " gp2b ");

                        param_count++;
                        indexsreceived.Add(par.param_index);

                        MAV.param_types[paramID] = (MAV_PARAM_TYPE) par.param_type;

                        //Console.WriteLine(DateTime.Now.Millisecond + " gp3 ");

                        this.frmProgressReporter.UpdateProgressAndStatus((indexsreceived.Count*100)/param_total,
                            Strings.Gotparam + paramID);

                        // we hit the last param - lets escape eq total = 176 index = 0-175
                        if (par.param_index == (param_total - 1))
                            start = DateTime.MinValue;
                    }
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.STATUSTEXT)
                    {
                        var msg = buffer.ByteArrayToStructure<MAVLink.mavlink_statustext_t>(6);

                        string logdata = Encoding.ASCII.GetString(msg.text);

                        int ind = logdata.IndexOf('\0');
                        if (ind != -1)
                            logdata = logdata.Substring(0, ind);

                        if (logdata.ToLower().Contains("copter") || logdata.ToLower().Contains("rover") ||
                            logdata.ToLower().Contains("plane"))
                        {
                            MAV.VersionString = logdata;
                        }
                        else if (logdata.ToLower().Contains("nuttx"))
                        {
                            MAV.SoftwareVersions = logdata;
                        }
                        else if (logdata.ToLower().Contains("px4v2"))
                        {
                            MAV.SerialString = logdata;
                        }
                        else if (logdata.ToLower().Contains("frame"))
                        {
                            MAV.FrameString = logdata;
                        }
                    }
                    //stopwatch.Stop();
                    // Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
                    // Console.WriteLine(DateTime.Now.Millisecond + " gp4 " + BaseStream.BytesToRead);
                }
                if (logreadmode && logplaybackfile.BaseStream.Position >= logplaybackfile.BaseStream.Length)
                {
                    break;
                }
                if (!logreadmode && !BaseStream.IsOpen)
                    throw new Exception("Not Connected");
            } while (indexsreceived.Count < param_total);

            if (indexsreceived.Count != param_total)
            {
                throw new Exception("Missing Params");
            }
            giveComport = false;

            MAV.param.Clear();
            MAV.param.AddRange(newparamlist);
            return MAV.param;
        }

        public float GetParam(string name)
        {
            return GetParam(name, -1);
        }

        public float GetParam(short index)
        {
            return GetParam("", index);
        }

        /// <summary>
        /// Get param by either index or name
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal float GetParam(string name = "", short index = -1)
        {
            if (name == "" && index == -1)
                return 0;

            log.Info("GetParam name: '" + name + "' or index: " + index);

            giveComport = true;
            byte[] buffer;

            mavlink_param_request_read_t req = new mavlink_param_request_read_t();
            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;
            req.param_index = index;
            req.param_id = new byte[] {0x0};
            if (index == -1)
            {
                req.param_id = System.Text.ASCIIEncoding.ASCII.GetBytes(name);
            }

            Array.Resize(ref req.param_id, 16);

            generatePacket((byte) MAVLINK_MSG_ID.PARAM_REQUEST_READ, req);

            DateTime start = DateTime.Now;
            int retrys = 3;

            while (true)
            {
                if (!(start.AddMilliseconds(700) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("GetParam Retry " + retrys);
                        generatePacket((byte) MAVLINK_MSG_ID.PARAM_REQUEST_READ, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - GetParam");
                }

                buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.PARAM_VALUE)
                    {
                        giveComport = false;

                        mavlink_param_value_t par = buffer.ByteArrayToStructure<mavlink_param_value_t>(6);

                        string st = System.Text.ASCIIEncoding.ASCII.GetString(par.param_id);

                        int pos = st.IndexOf('\0');

                        if (pos != -1)
                        {
                            st = st.Substring(0, pos);
                        }

                        // not the correct id
                        if (!(par.param_index == index || st == name))
                        {
                            log.ErrorFormat("Wrong Answer {0} - {1} - {2}    --- '{3}' vs '{4}'", par.param_index,
                                ASCIIEncoding.ASCII.GetString(par.param_id), par.param_value,
                                ASCIIEncoding.ASCII.GetString(req.param_id).TrimEnd(), st);
                            continue;
                        }

                        // update table
                        if (MAV.apname == MAV_AUTOPILOT.ARDUPILOTMEGA)
                        {
                            MAV.param[st] = new MAVLinkParam(st, par.param_value, MAV_PARAM_TYPE.REAL32);
                        }
                        else
                        {
                            MAV.param[st] = new MAVLinkParam(st, par.param_value, (MAV_PARAM_TYPE) par.param_type);
                        }

                        MAV.param_types[st] = (MAV_PARAM_TYPE) par.param_type;

                        log.Info(DateTime.Now.Millisecond + " got param " + (par.param_index) + " of " +
                                 (par.param_count) + " name: " + st);

                        return par.param_value;
                    }
                }
            }
        }

        public static void modifyParamForDisplay(bool fromapm, string paramname, ref float value)
        {
            int planforremoval;

            if (paramname.ToUpper().EndsWith("_IMAX") || paramname.ToUpper().EndsWith("ALT_HOLD_RTL") ||
                paramname.ToUpper().EndsWith("APPROACH_ALT") || paramname.ToUpper().EndsWith("TRIM_ARSPD_CM") ||
                paramname.ToUpper().EndsWith("MIN_GNDSPD_CM")
                || paramname.ToUpper().EndsWith("XTRK_ANGLE_CD") || paramname.ToUpper().EndsWith("LIM_PITCH_MAX") ||
                paramname.ToUpper().EndsWith("LIM_PITCH_MIN")
                || paramname.ToUpper().EndsWith("LIM_ROLL_CD") || paramname.ToUpper().EndsWith("PITCH_MAX") ||
                paramname.ToUpper().EndsWith("WP_SPEED_MAX"))
            {
                if (paramname.ToUpper().EndsWith("THR_RATE_IMAX") || paramname.ToUpper().EndsWith("THR_HOLD_IMAX")
                    || paramname.ToUpper().EndsWith("RATE_RLL_IMAX") || paramname.ToUpper().EndsWith("RATE_PIT_IMAX"))
                    return;

                if (fromapm)
                {
                    value /= 100.0f;
                }
                else
                {
                    value *= 100.0f;
                }
            }
            else if (paramname.ToUpper().StartsWith("TUNE_"))
            {
                if (fromapm)
                {
                    value /= 1000.0f;
                }
                else
                {
                    value *= 1000.0f;
                }
            }
        }

        /// <summary>
        /// Stops all requested data packets.
        /// </summary>
        public void stopall(bool forget)
        {
            mavlink_request_data_stream_t req = new mavlink_request_data_stream_t();
            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;

            req.req_message_rate = 10;
            req.start_stop = 0; // stop
            req.req_stream_id = 0; // all

            // no error on bad
            try
            {
                generatePacket((byte) MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
                System.Threading.Thread.Sleep(20);
                generatePacket((byte) MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
                System.Threading.Thread.Sleep(20);
                generatePacket((byte) MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
                log.Info("Stopall Done");
            }
            catch
            {
            }
        }

        public void setWPACK()
        {
            MAVLink.mavlink_mission_ack_t req = new MAVLink.mavlink_mission_ack_t();
            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;
            req.type = 0;

            generatePacket((byte) MAVLINK_MSG_ID.MISSION_ACK, req);
        }

        public bool setWPCurrent(ushort index)
        {
            giveComport = true;
            byte[] buffer;

            mavlink_mission_set_current_t req = new mavlink_mission_set_current_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;
            req.seq = index;

            generatePacket((byte) MAVLINK_MSG_ID.MISSION_SET_CURRENT, req);

            DateTime start = DateTime.Now;
            int retrys = 5;

            while (true)
            {
                if (!(start.AddMilliseconds(2000) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("setWPCurrent Retry " + retrys);
                        generatePacket((byte) MAVLINK_MSG_ID.MISSION_SET_CURRENT, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - setWPCurrent");
                }

                buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.MISSION_CURRENT)
                    {
                        giveComport = false;
                        return true;
                    }
                }
            }
        }

        [Obsolete("Mavlink 09 - use doCommand", true)]
        public bool doAction(object actionid)
        {
            // mavlink 09
            throw new NotImplementedException();
        }

        /// <summary>
        /// send lots of packets to reset the hardware, this asumes we dont know the sysid in the first place
        /// </summary>
        /// <returns></returns>
        public bool doReboot(bool bootloadermode = false)
        {
            byte[] buffer = getHeartBeat();

            if (buffer.Length > 5)
            {
                mavlink_heartbeat_t hb = buffer.ByteArrayToStructure<mavlink_heartbeat_t>(6);

                mavlinkversion = hb.mavlink_version;
                MAV.aptype = (MAV_TYPE) hb.type;
                MAV.apname = (MAV_AUTOPILOT) hb.autopilot;
                MAV.sysid = buffer[3];
                MAV.compid = buffer[4];
            }

            int param1 = 1;
            if (bootloadermode)
            {
                param1 = 3;
            }

            if (MAV.sysid != 0 && MAV.compid != 0)
            {
                doCommand(MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN, param1, 0, 0, 0, 0, 0, 0);
                doCommand(MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN, 1, 0, 0, 0, 0, 0, 0);
            }
            else
            {
                for (byte a = byte.MinValue; a < byte.MaxValue; a++)
                {
                    giveComport = true;
                    MAV.sysid = a;
                    doCommand(MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN, param1, 0, 0, 0, 0, 0, 0);
                }
            }
            giveComport = false;
            return true;
        }

        public bool doARM(bool armit)
        {
            return doCommand(MAV_CMD.COMPONENT_ARM_DISARM, armit ? 1 : 0, 21196, 0, 0, 0, 0, 0);
        }

        public bool doAbortLand()
        {
            return doCommand(MAV_CMD.DO_GO_AROUND, 0, 0, 0, 0, 0, 0, 0);
        }

        public bool doMotorTest(int motor, MAVLink.MOTOR_TEST_THROTTLE_TYPE thr_type, int throttle, int timeout)
        {
            return MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_MOTOR_TEST, (float) motor, (float) (byte) thr_type,
                (float) throttle, (float) timeout, 0, 0, 0);
        }

        public bool doCommand(MAV_CMD actionid, float p1, float p2, float p3, float p4, float p5, float p6, float p7)
        {
            giveComport = true;
            byte[] buffer;

            mavlink_command_long_t req = new mavlink_command_long_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;

            req.command = (ushort) actionid;

            req.param1 = p1;
            req.param2 = p2;
            req.param3 = p3;
            req.param4 = p4;
            req.param5 = p5;
            req.param6 = p6;
            req.param7 = p7;

            log.InfoFormat("doCommand cmd {0} {1} {2} {3} {4} {5} {6} {7}", actionid.ToString(), p1, p2, p3, p4, p5, p6,
                p7);

            generatePacket((byte) MAVLINK_MSG_ID.COMMAND_LONG, req);

            DateTime start = DateTime.Now;
            int retrys = 3;

            int timeout = 2000;

            // imu calib take a little while
            if (actionid == MAV_CMD.PREFLIGHT_CALIBRATION && p5 == 1)
            {
                // this is for advanced accel offsets, and blocks execution
                return true;
            }
            else if (actionid == MAV_CMD.PREFLIGHT_CALIBRATION)
            {
                retrys = 1;
                timeout = 25000;
            }
            else if (actionid == MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN)
            {
                generatePacket((byte) MAVLINK_MSG_ID.COMMAND_LONG, req);
                giveComport = false;
                return true;
            }
            else if (actionid == MAV_CMD.COMPONENT_ARM_DISARM)
            {
                // 10 seconds as may need an imu calib
                timeout = 10000;
            }
            else if (actionid == MAV_CMD.PREFLIGHT_CALIBRATION && p6 == 1)
            {
                // compassmot
                // send again just incase
                generatePacket((byte) MAVLINK_MSG_ID.COMMAND_LONG, req);
                giveComport = false;
                return true;
            }

            while (true)
            {
                if (!(start.AddMilliseconds(timeout) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("doAction Retry " + retrys);
                        generatePacket((byte) MAVLINK_MSG_ID.COMMAND_LONG, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - doAction");
                }

                buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.COMMAND_ACK)
                    {
                        var ack = buffer.ByteArrayToStructure<mavlink_command_ack_t>(6);


                        if (ack.result == (byte) MAV_RESULT.ACCEPTED)
                        {
                            giveComport = false;
                            return true;
                        }
                        else
                        {
                            giveComport = false;
                            return false;
                        }
                    }
                }
            }
        }

        public void SendAck()
        {
            mavlink_command_ack_t ack = new mavlink_command_ack_t();
            ack.command = (byte) MAV_CMD.PREFLIGHT_CALIBRATION;
            ack.result = 0;

            // send twice
            generatePacket((byte) MAVLINK_MSG_ID.COMMAND_ACK, ack);
            System.Threading.Thread.Sleep(20);
            generatePacket((byte) MAVLINK_MSG_ID.COMMAND_ACK, ack);
        }

        public void SendSerialControl(SERIAL_CONTROL_DEV port, ushort timeoutms, byte[] data, uint baudrate = 0,
            bool close = false)
        {
            mavlink_serial_control_t ctl = new mavlink_serial_control_t();

            ctl.baudrate = baudrate; // no change
            ctl.device = (byte) port;
            ctl.timeout = timeoutms;
            ctl.data = new byte[70];
            ctl.count = 0;
            if (close)
            {
                ctl.flags = 0;
            }
            else
            {
                ctl.flags = (byte) SERIAL_CONTROL_FLAG.EXCLUSIVE; // | SERIAL_CONTROL_FLAG.MULTI);
            }

            if (data != null && data.Length != 0)
            {
                int packets = (data.Length/70) + 1;
                int len = data.Length;
                while (len > 0)
                {
                    if (packets == 1)
                        ctl.flags |= (byte) SERIAL_CONTROL_FLAG.RESPOND;

                    byte n = (byte) Math.Min(70, len);

                    ctl.count = n;
                    Array.Copy(data, data.Length - len, ctl.data, 0, n);

                    // dont flood the port
                    System.Threading.Thread.Sleep(10);

                    generatePacket((byte) MAVLINK_MSG_ID.SERIAL_CONTROL, ctl);

                    len -= n;
                    packets--;
                }
            }
            else
            {
                if (!close)
                    ctl.flags |= (byte) SERIAL_CONTROL_FLAG.RESPOND | (byte) SERIAL_CONTROL_FLAG.MULTI;

                generatePacket((byte) MAVLINK_MSG_ID.SERIAL_CONTROL, ctl);
            }
        }

        public void requestDatastream(MAVLink.MAV_DATA_STREAM id, byte hzrate, int sysid = -1, int compid = -1)
        {
            if (sysid == -1)
                sysid = sysidcurrent;

            if (compid == -1)
                compid = compidcurrent;

            double pps = 0;

            switch (id)
            {
                case MAVLink.MAV_DATA_STREAM.ALL:

                    break;
                case MAVLink.MAV_DATA_STREAM.EXTENDED_STATUS:
                    if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.SYS_STATUS] <
                        DateTime.Now.AddSeconds(-2))
                        break;
                    pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.SYS_STATUS];
                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }
                    break;
                case MAVLink.MAV_DATA_STREAM.EXTRA1:
                    if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.ATTITUDE] <
                        DateTime.Now.AddSeconds(-2))
                        break;
                    pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.ATTITUDE];
                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }
                    break;
                case MAVLink.MAV_DATA_STREAM.EXTRA2:
                    if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.VFR_HUD] <
                        DateTime.Now.AddSeconds(-2))
                        break;
                    pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.VFR_HUD];
                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }
                    break;
                case MAVLink.MAV_DATA_STREAM.EXTRA3:
                    if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.AHRS] <
                        DateTime.Now.AddSeconds(-2))
                        break;
                    pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.AHRS];
                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }
                    break;
                case MAVLink.MAV_DATA_STREAM.POSITION:
                    if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.GLOBAL_POSITION_INT] <
                        DateTime.Now.AddSeconds(-2))
                        break;
                    pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.GLOBAL_POSITION_INT];
                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }
                    break;
                case MAVLink.MAV_DATA_STREAM.RAW_CONTROLLER:
                    if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.RC_CHANNELS_SCALED] <
                        DateTime.Now.AddSeconds(-2))
                        break;
                    pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.RC_CHANNELS_SCALED];
                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }
                    break;
                case MAVLink.MAV_DATA_STREAM.RAW_SENSORS:
                    if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.RAW_IMU] <
                        DateTime.Now.AddSeconds(-2))
                        break;
                    pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.RAW_IMU];
                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }
                    break;
                case MAVLink.MAV_DATA_STREAM.RC_CHANNELS:
                    if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.RC_CHANNELS_RAW] <
                        DateTime.Now.AddSeconds(-2))
                        break;
                    pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.RC_CHANNELS_RAW];
                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }
                    break;
            }

            //packetspersecond[temp[5]];

            if (pps == 0 && hzrate == 0)
            {
                return;
            }


            log.InfoFormat("Request stream {0} at {1} hz for {2}:{3}",
                Enum.Parse(typeof (MAV_DATA_STREAM), id.ToString()), hzrate, sysid, compid);
            getDatastream(id, hzrate);
        }

        // returns true for ok
        bool hzratecheck(double pps, int hzrate)
        {
            if (double.IsInfinity(pps))
            {
                return false;
            }
            else if (hzrate == 0 && pps == 0)
            {
                return true;
            }
            else if (hzrate == 1 && pps >= 0.5 && pps <= 2)
            {
                return true;
            }
            else if (hzrate == 3 && pps >= 2 && hzrate < 5)
            {
                return true;
            }
            else if (hzrate == 10 && pps > 5 && hzrate < 15)
            {
                return true;
            }
            else if (hzrate > 15 && pps > 15)
            {
                return true;
            }

            return false;
        }

        void getDatastream(MAVLink.MAV_DATA_STREAM id, byte hzrate)
        {
            mavlink_request_data_stream_t req = new mavlink_request_data_stream_t();
            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;

            req.req_message_rate = hzrate;
            req.start_stop = 1; // start
            req.req_stream_id = (byte) id; // id

            // send each one twice.
            generatePacket((byte) MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
            generatePacket((byte) MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
        }

        /// <summary>
        /// Returns WP count
        /// </summary>
        /// <returns></returns>
        public ushort getWPCount()
        {
            giveComport = true;
            byte[] buffer;
            mavlink_mission_request_list_t req = new mavlink_mission_request_list_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;

            // request list
            generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST_LIST, req);

            DateTime start = DateTime.Now;
            int retrys = 6;

            while (true)
            {
                if (!(start.AddMilliseconds(700) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("getWPCount Retry " + retrys + " - giv com " + giveComport);
                        generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST_LIST, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    //return (byte)int.Parse(param["WP_TOTAL"].ToString());
                    throw new Exception("Timeout on read - getWPCount");
                }

                buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.MISSION_COUNT)
                    {
                        var count = buffer.ByteArrayToStructure<mavlink_mission_count_t>(6);


                        log.Info("wpcount: " + count.count);
                        giveComport = false;
                        return count.count; // should be ushort, but apm has limited wp count < byte
                    }
                    else
                    {
                        log.Info(DateTime.Now + " PC wpcount " + buffer[5] + " need " + MAVLINK_MSG_ID.MISSION_COUNT);
                    }
                }
            }
        }

        /// <summary>
        /// Gets specfied WP
        /// </summary>
        /// <param name="index"></param>
        /// <returns>WP</returns>
        public Locationwp getWP(ushort index)
        {
            while (giveComport)
                System.Threading.Thread.Sleep(100);

            giveComport = true;
            Locationwp loc = new Locationwp();
            mavlink_mission_request_t req = new mavlink_mission_request_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;

            req.seq = index;

            //Console.WriteLine("getwp req "+ DateTime.Now.Millisecond);

            // request
            generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST, req);

            DateTime start = DateTime.Now;
            int retrys = 5;

            while (true)
            {
                if (!(start.AddMilliseconds(3500) > DateTime.Now)) // apm times out after 5000ms
                {
                    if (retrys > 0)
                    {
                        log.Info("getWP Retry " + retrys);
                        generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - getWP");
                }
                //Console.WriteLine("getwp read " + DateTime.Now.Millisecond);
                byte[] buffer = readPacket();
                //Console.WriteLine("getwp readend " + DateTime.Now.Millisecond);
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.MISSION_ITEM)
                    {
                        //Console.WriteLine("getwp ans " + DateTime.Now.Millisecond);

                        var wp = buffer.ByteArrayToStructure<mavlink_mission_item_t>(6);

                        // received a packet, but not what we requested
                        if (req.seq != wp.seq)
                        {
                            generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST, req);
                            continue;
                        }

                        loc.options = (byte) (wp.frame);
                        loc.id = (byte) (wp.command);
                        loc.p1 = (wp.param1);
                        loc.p2 = (wp.param2);
                        loc.p3 = (wp.param3);
                        loc.p4 = (wp.param4);

                        loc.alt = ((wp.z));
                        loc.lat = ((wp.x));
                        loc.lng = ((wp.y));

                        log.InfoFormat("getWP {0} {1} {2} {3} {4} opt {5}", loc.id, loc.p1, loc.alt, loc.lat, loc.lng,
                            loc.options);

                        break;
                    }
                    else
                    {
                        log.Info(DateTime.Now + " PC getwp " + buffer[5]);
                    }
                }
            }
            giveComport = false;
            return loc;
        }

        public object DebugPacket(byte[] datin)
        {
            string text = "";
            return DebugPacket(datin, ref text, true);
        }

        public object DebugPacket(byte[] datin, bool PrintToConsole)
        {
            string text = "";
            return DebugPacket(datin, ref text, PrintToConsole);
        }

        public object DebugPacket(byte[] datin, ref string text)
        {
            return DebugPacket(datin, ref text, true);
        }

        /// <summary>
        /// Print entire decoded packet to console
        /// </summary>
        /// <param name="datin">packet byte array</param>
        /// <returns>struct of data</returns>
        public object DebugPacket(byte[] datin, ref string text, bool PrintToConsole, string delimeter = " ")
        {
            string textoutput;
            try
            {
                if (datin.Length > 5)
                {
                    byte header = datin[0];
                    byte length = datin[1];
                    byte seq = datin[2];
                    byte sysid = datin[3];
                    byte compid = datin[4];
                    byte messid = datin[5];

                    textoutput = string.Format("{0,2:X}{6}{1,2:X}{6}{2,2:X}{6}{3,2:X}{6}{4,2:X}{6}{5,2:X}{6}", header,
                        length, seq, sysid, compid, messid, delimeter);

                    object data = Activator.CreateInstance(MAVLINK_MESSAGE_INFO[messid]);

                    MavlinkUtil.ByteArrayToStructure(datin, ref data, 6);

                    Type test = data.GetType();

                    if (PrintToConsole)
                    {
                        textoutput = textoutput + test.Name + delimeter;

                        foreach (var field in test.GetFields())
                        {
                            // field.Name has the field's name.

                            object fieldValue = field.GetValue(data); // Get value

                            if (field.FieldType.IsArray)
                            {
                                textoutput = textoutput + field.Name + delimeter;
                                if (fieldValue.GetType() == typeof (byte[]))
                                {
                                    try
                                    {
                                        byte[] crap = (byte[]) fieldValue;

                                        foreach (byte fiel in crap)
                                        {
                                            if (fiel == 0)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                textoutput = textoutput + (char) fiel;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                                if (fieldValue.GetType() == typeof (short[]))
                                {
                                    try
                                    {
                                        short[] crap = (short[]) fieldValue;

                                        foreach (short fiel in crap)
                                        {
                                            if (fiel == 0)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                textoutput = textoutput + Convert.ToString(fiel, 16) + "|";
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                                textoutput = textoutput + delimeter;
                            }
                            else
                            {
                                textoutput = textoutput + field.Name + delimeter + fieldValue.ToString() + delimeter;
                            }
                        }
                        textoutput = textoutput + delimeter + "Len" + delimeter + datin.Length + "\r\n";
                        if (PrintToConsole)
                            Console.Write(textoutput);

                        if (text != null)
                            text = textoutput;
                    }

                    return data;
                }
            }
            catch
            {
            }

            return null;
        }

        public object GetPacket(byte[] datin)
        {
            if (datin.Length > 5)
            {
                byte header = datin[0];
                byte length = datin[1];
                byte seq = datin[2];
                byte sysid = datin[3];
                byte compid = datin[4];
                byte messid = datin[5];

                try
                {
                    object data = Activator.CreateInstance(MAVLINK_MESSAGE_INFO[messid]);

                    MavlinkUtil.ByteArrayToStructure(datin, ref data, 6);

                    return data;
                }
                catch
                {
                }
            }

            return null;
        }

        /// <summary>
        /// Set start and finish for partial wp upload.
        /// </summary>
        /// <param name="startwp"></param>
        /// <param name="endwp"></param>
        public void setWPPartialUpdate(ushort startwp, ushort endwp)
        {
            mavlink_mission_write_partial_list_t req = new mavlink_mission_write_partial_list_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;

            req.start_index = (short) startwp;
            req.end_index = (short) endwp;

            generatePacket((byte) MAVLINK_MSG_ID.MISSION_WRITE_PARTIAL_LIST, req);
        }

        /// <summary>
        /// Sets wp total count
        /// </summary>
        /// <param name="wp_total"></param>
        public void setWPTotal(ushort wp_total)
        {
            giveComport = true;
            mavlink_mission_count_t req = new mavlink_mission_count_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid; // MSG_NAMES.MISSION_COUNT

            req.count = wp_total;

            generatePacket((byte) MAVLINK_MSG_ID.MISSION_COUNT, req);

            DateTime start = DateTime.Now;
            int retrys = 3;

            while (true)
            {
                if (!(start.AddMilliseconds(700) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("setWPTotal Retry " + retrys);
                        generatePacket((byte) MAVLINK_MSG_ID.MISSION_COUNT, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - setWPTotal");
                }
                byte[] buffer = readPacket();
                if (buffer.Length > 9)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.MISSION_REQUEST)
                    {
                        var request = buffer.ByteArrayToStructure<mavlink_mission_request_t>(6);

                        if (request.seq == 0)
                        {
                            if (MAV.param["WP_TOTAL"] != null)
                                MAV.param["WP_TOTAL"].Value = wp_total - 1;
                            if (MAV.param["CMD_TOTAL"] != null)
                                MAV.param["CMD_TOTAL"].Value = wp_total - 1;
                            if (MAV.param["MIS_TOTAL"] != null)
                                MAV.param["MIS_TOTAL"].Value = wp_total - 1;

                            MAV.wps.Clear();

                            giveComport = false;
                            return;
                        }
                    }
                    else
                    {
                        //Console.WriteLine(DateTime.Now + " PC getwp " + buffer[5]);
                    }
                }
            }
        }

        /// <summary>
        /// used to injecy data into the gps ie rtcm/sbp/ubx
        /// </summary>
        /// <param name="data"></param>
        public void InjectGpsData(byte[] data, byte length)
        {
            mavlink_gps_inject_data_t gps = new mavlink_gps_inject_data_t();

            gps.data = new byte[110];
            Array.Copy(data, gps.data, length);
            gps.len = length;
            gps.target_component = MAV.compid;
            gps.target_system = MAV.sysid;

            generatePacket((byte) MAVLINK_MSG_ID.GPS_INJECT_DATA, gps);
        }

        /// <summary>
        /// Save wp to eeprom
        /// </summary>
        /// <param name="loc">location struct</param>
        /// <param name="index">wp no</param>
        /// <param name="frame">global or relative</param>
        /// <param name="current">0 = no , 2 = guided mode</param>
        public MAV_MISSION_RESULT setWP(Locationwp loc, ushort index, MAV_FRAME frame, byte current = 0,
            byte autocontinue = 1)
        {
            mavlink_mission_item_t req = new mavlink_mission_item_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid; // MSG_NAMES.MISSION_ITEM

            req.command = loc.id;

            req.current = current;
            req.autocontinue = autocontinue;

            req.frame = (byte) frame;
            req.y = (float) (loc.lng);
            req.x = (float) (loc.lat);
            req.z = (float) (loc.alt);

            req.param1 = loc.p1;
            req.param2 = loc.p2;
            req.param3 = loc.p3;
            req.param4 = loc.p4;

            req.seq = index;

            return setWP(req);
        }

        public MAV_MISSION_RESULT setWP(mavlink_mission_item_t req)
        {
            giveComport = true;

            ushort index = req.seq;

            log.InfoFormat("setWP {6} frame {0} cmd {1} p1 {2} x {3} y {4} z {5}", req.frame, req.command, req.param1,
                req.x, req.y, req.z, index);

            // request
            generatePacket((byte) MAVLINK_MSG_ID.MISSION_ITEM, req);


            DateTime start = DateTime.Now;
            int retrys = 10;

            while (true)
            {
                if (!(start.AddMilliseconds(400) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("setWP Retry " + retrys);
                        generatePacket((byte) MAVLINK_MSG_ID.MISSION_ITEM, req);

                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - setWP");
                }
                byte[] buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.MISSION_ACK)
                    {
                        var ans = buffer.ByteArrayToStructure<mavlink_mission_ack_t>(6);
                        log.Info("set wp " + index + " ACK 47 : " + buffer[5] + " ans " +
                                 Enum.Parse(typeof (MAV_MISSION_RESULT), ans.type.ToString()));
                        giveComport = false;

                        if (req.current == 2)
                        {
                            MAV.GuidedMode = req;
                        }
                        else if (req.current == 3)
                        {
                        }
                        else
                        {
                            MAV.wps[req.seq] = req;
                        }

                        return (MAV_MISSION_RESULT) ans.type;
                    }
                    else if (buffer[5] == (byte) MAVLINK_MSG_ID.MISSION_REQUEST)
                    {
                        var ans = buffer.ByteArrayToStructure<mavlink_mission_request_t>(6);
                        if (ans.seq == (index + 1))
                        {
                            log.Info("set wp doing " + index + " req " + ans.seq + " REQ 40 : " + buffer[5]);
                            giveComport = false;

                            if (req.current == 2)
                            {
                                MAV.GuidedMode = req;
                            }
                            else if (req.current == 3)
                            {
                            }
                            else
                            {
                                MAV.wps[req.seq] = req;
                            }

                            return MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED;
                        }
                        else
                        {
                            log.InfoFormat(
                                "set wp fail doing " + index + " req " + ans.seq + " ACK 47 or REQ 40 : " + buffer[5] +
                                " seq {0} ts {1} tc {2}", req.seq, req.target_system, req.target_component);
                            // resend point now
                            start = DateTime.MinValue;
                        }
                    }
                    else
                    {
                        //Console.WriteLine(DateTime.Now + " PC setwp " + buffer[5]);
                    }
                }
            }

            // return MAV_MISSION_RESULT.MAV_MISSION_INVALID;
        }

        public void setNextWPTargetAlt(ushort wpno, float alt)
        {
            // get the existing wp
            Locationwp current = getWP(wpno);

            mavlink_mission_write_partial_list_t req = new mavlink_mission_write_partial_list_t();
            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;

            req.start_index = (short) wpno;
            req.end_index = (short) wpno;

            // change the alt
            current.alt = alt;

            // send a request to update single point
            generatePacket((byte) MAVLINK_MSG_ID.MISSION_WRITE_PARTIAL_LIST, req);
            Thread.Sleep(10);
            generatePacket((byte) MAVLINK_MSG_ID.MISSION_WRITE_PARTIAL_LIST, req);

            MAV_FRAME frame = (current.options & 0x1) == 0 ? MAV_FRAME.GLOBAL : MAV_FRAME.GLOBAL_RELATIVE_ALT;

            //send the point with new alt
            setWP(current, wpno, MAV_FRAME.GLOBAL_RELATIVE_ALT, 0);

            // set the point as current to reload the modified command
            setWPCurrent(wpno);
        }

        public void setGuidedModeWP(Locationwp gotohere)
        {
            if (gotohere.alt == 0 || gotohere.lat == 0 || gotohere.lng == 0)
                return;

            giveComport = true;

            try
            {
                gotohere.id = (byte) MAV_CMD.WAYPOINT;

                // fix for followme change
                if (MAV.cs.mode.ToUpper() != "GUIDED")
                    setMode("GUIDED");

                MAV_MISSION_RESULT ans = setWP(gotohere, 0, MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT, (byte) 2);

                if (ans != MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                    throw new Exception("Guided Mode Failed");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            giveComport = false;
        }

        public void setNewWPAlt(Locationwp gotohere)
        {
            giveComport = true;

            try
            {
                gotohere.id = (byte) MAV_CMD.WAYPOINT;

                MAV_MISSION_RESULT ans = setWP(gotohere, 0, MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT, (byte) 3);

                if (ans != MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                    throw new Exception("Alt Change Failed");
            }
            catch (Exception ex)
            {
                giveComport = false;
                log.Error(ex);
                throw;
            }

            giveComport = false;
        }

        public void setDigicamConfigure()
        {
            // not implmented
        }

        public void setDigicamControl(bool shot)
        {
            mavlink_digicam_control_t req = new mavlink_digicam_control_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;
            req.shot = (shot == true) ? (byte) 1 : (byte) 0;

            generatePacket((byte) MAVLINK_MSG_ID.DIGICAM_CONTROL, req);

            MainV2.comPort.doCommand(MAV_CMD.DO_DIGICAM_CONTROL, 0, 0, 0, 0, 1, 0, 0);

            //MAVLINK_MSG_ID.CAMERA_FEEDBACK;

            //mavlink_camera_feedback_t
        }

        public void setMountConfigure(MAV_MOUNT_MODE mountmode, bool stabroll, bool stabpitch, bool stabyaw)
        {
            mavlink_mount_configure_t req = new mavlink_mount_configure_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;
            req.mount_mode = (byte) mountmode;
            req.stab_pitch = (stabpitch == true) ? (byte) 1 : (byte) 0;
            req.stab_roll = (stabroll == true) ? (byte) 1 : (byte) 0;
            req.stab_yaw = (stabyaw == true) ? (byte) 1 : (byte) 0;

            generatePacket((byte) MAVLINK_MSG_ID.MOUNT_CONFIGURE, req);
            System.Threading.Thread.Sleep(20);
            generatePacket((byte) MAVLINK_MSG_ID.MOUNT_CONFIGURE, req);
        }

        public void setMountControl(double pa, double pb, double pc, bool islatlng)
        {
            mavlink_mount_control_t req = new mavlink_mount_control_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;
            if (!islatlng)
            {
                req.input_a = (int) pa;
                req.input_b = (int) pb;
                req.input_c = (int) pc;
            }
            else
            {
                req.input_a = (int) (pa*10000000.0);
                req.input_b = (int) (pb*10000000.0);
                req.input_c = (int) (pc*100.0);
            }

            generatePacket((byte) MAVLINK_MSG_ID.MOUNT_CONTROL, req);
            System.Threading.Thread.Sleep(20);
            generatePacket((byte) MAVLINK_MSG_ID.MOUNT_CONTROL, req);
        }

        public void setMode(string modein)
        {
            try
            {
                MAVLink.mavlink_set_mode_t mode = new MAVLink.mavlink_set_mode_t();

                if (translateMode(modein, ref mode))
                {
                    setMode(mode);
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Failed to change Modes");
            }
        }

        public void setMode(mavlink_set_mode_t mode, MAV_MODE_FLAG base_mode = 0)
        {
            mode.base_mode |= (byte) base_mode;

            generatePacket((byte) (byte) MAVLink.MAVLINK_MSG_ID.SET_MODE, mode);
            System.Threading.Thread.Sleep(10);
            generatePacket((byte) (byte) MAVLink.MAVLINK_MSG_ID.SET_MODE, mode);
        }

        /// <summary>
        /// used for last bad serial characters
        /// </summary>
        byte[] lastbad = new byte[2];

        private double t7 = 1.0e7;

        /// <summary>
        /// Serial Reader to read mavlink packets. POLL method
        /// </summary>
        /// <returns></returns>
        public byte[] readPacket()
        {
            /*
            var buffer =  mavparse.ReadPacket(BaseStream.BaseStream);
            
            if (buffer == null)
                return new byte[0];
            */

            byte[] buffer = new byte[270];
            int count = 0;
            int length = 0;
            int readcount = 0;
            lastbad = new byte[2];

            BaseStream.ReadTimeout = 1200; // 1200 ms between chars - the gps detection requires this.

            DateTime start = DateTime.Now;

            //Console.WriteLine(DateTime.Now.Millisecond + " SR0 " + BaseStream.BytesToRead);

            lock (readlock)
            {
                //Console.WriteLine(DateTime.Now.Millisecond + " SR1 " + BaseStream.BytesToRead);

                while (BaseStream.IsOpen || logreadmode)
                {
                    try
                    {
                        if (readcount > 300)
                        {
                            log.Info("MAVLink readpacket No valid mavlink packets");
                            break;
                        }
                        readcount++;
                        if (logreadmode)
                        {
                            buffer = readlogPacketMavlink();
                            if (buffer.Length == 0)
                                return new byte[0];
                        }
                        else
                        {
                            // time updated for internal reference
                            MAV.cs.datetime = DateTime.Now;

                            DateTime to = DateTime.Now.AddMilliseconds(BaseStream.ReadTimeout);

                            // Console.WriteLine(DateTime.Now.Millisecond + " SR1a " + BaseStream.BytesToRead);

                            while (BaseStream.IsOpen && BaseStream.BytesToRead <= 0)
                            {
                                if (DateTime.Now > to)
                                {
                                    log.InfoFormat("MAVLINK: 1 wait time out btr {0} len {1}", BaseStream.BytesToRead,
                                        length);
                                    throw new TimeoutException("Timeout");
                                }
                                System.Threading.Thread.Sleep(1);
                                //Console.WriteLine(DateTime.Now.Millisecond + " SR0b " + BaseStream.BytesToRead);
                            }
                            //Console.WriteLine(DateTime.Now.Millisecond + " SR1a " + BaseStream.BytesToRead);
                            if (BaseStream.IsOpen)
                            {
                                BaseStream.Read(buffer, count, 1);
                                if (rawlogfile != null && rawlogfile.CanWrite)
                                    rawlogfile.WriteByte(buffer[count]);
                            }
                            //Console.WriteLine(DateTime.Now.Millisecond + " SR1b " + BaseStream.BytesToRead);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Info("MAVLink readpacket read error: " + e.ToString());
                        break;
                    }

                    // check if looks like a mavlink packet and check for exclusions and write to console
                    if (buffer[0] != 254 && buffer[0] != 'U')
                    {
                        if (buffer[0] >= 0x20 && buffer[0] <= 127 || buffer[0] == '\n' || buffer[0] == '\r')
                        {
                            // check for line termination
                            if (buffer[0] == '\r' || buffer[0] == '\n')
                            {
                                // check new line is valid
                                if (buildplaintxtline.Length > 3)
                                    plaintxtline = buildplaintxtline;

                                // reset for next line
                                buildplaintxtline = "";
                            }

                            TCPConsole.Write(buffer[0]);
                            Console.Write((char) buffer[0]);
                            buildplaintxtline += (char) buffer[0];
                        }
                        _bytesReceivedSubj.OnNext(1);
                        count = 0;
                        lastbad[0] = lastbad[1];
                        lastbad[1] = buffer[0];
                        buffer[1] = 0;
                        continue;
                    }
                    // reset count on valid packet
                    readcount = 0;

                    //Console.WriteLine(DateTime.Now.Millisecond + " SR2 " + BaseStream.BytesToRead);

                    // check for a header
                    if (buffer[0] == 254 || buffer[0] == 'U')
                    {
                        // if we have the header, and no other chars, get the length and packet identifiers
                        if (count == 0 && !logreadmode)
                        {
                            DateTime to = DateTime.Now.AddMilliseconds(BaseStream.ReadTimeout);

                            while (BaseStream.IsOpen && BaseStream.BytesToRead < 5)
                            {
                                if (DateTime.Now > to)
                                {
                                    log.InfoFormat("MAVLINK: 2 wait time out btr {0} len {1}", BaseStream.BytesToRead,
                                        length);
                                    throw new Exception("Timeout");
                                }
                                System.Threading.Thread.Sleep(1);
                                //Console.WriteLine(DateTime.Now.Millisecond + " SR0b " + BaseStream.BytesToRead);
                            }
                            int read = BaseStream.Read(buffer, 1, 5);
                            count = read;
                            if (rawlogfile != null && rawlogfile.CanWrite)
                                rawlogfile.Write(buffer, 1, read);
                        }

                        // packet length
                        length = buffer[1] + 6 + 2 - 2; // data + header + checksum - U - length
                        if (count >= 5 || logreadmode)
                        {
                            try
                            {
                                if (logreadmode)
                                {
                                }
                                else
                                {
                                    DateTime to = DateTime.Now.AddMilliseconds(BaseStream.ReadTimeout);

                                    while (BaseStream.IsOpen && BaseStream.BytesToRead < (length - 4))
                                    {
                                        if (DateTime.Now > to)
                                        {
                                            log.InfoFormat("MAVLINK: 3 wait time out btr {0} len {1}",
                                                BaseStream.BytesToRead, length);
                                            break;
                                        }
                                        System.Threading.Thread.Sleep(1);
                                    }
                                    if (BaseStream.IsOpen)
                                    {
                                        int read = BaseStream.Read(buffer, 6, length - 4);
                                        if (rawlogfile != null && rawlogfile.CanWrite)
                                        {
                                            // write only what we read, temp is the whole packet, so 6-end
                                            rawlogfile.Write(buffer, 6, read);
                                        }
                                    }
                                }
                                count = length + 2;
                            }
                            catch
                            {
                                break;
                            }
                            break;
                        }
                    }

                    count++;
                    if (count == 299)
                        break;
                }

                //Console.WriteLine(DateTime.Now.Millisecond + " SR3 " + BaseStream.BytesToRead);
            } // end readlock

            // resize the packet to the correct length
            Array.Resize<byte>(ref buffer, count);

            // add byte count
            _bytesReceivedSubj.OnNext(buffer.Length);

            // update bps statistics
            if (bpstime.Second != DateTime.Now.Second && !logreadmode && BaseStream.IsOpen)
            {
                Console.Write("bps {0} loss {1} left {2} mem {3}      \n", bps1, MAV.synclost, BaseStream.BytesToRead,
                    System.GC.GetTotalMemory(false)/1024/1024.0);
                bps2 = bps1; // prev sec
                bps1 = 0; // current sec
                bpstime = DateTime.Now;
            }

            bps1 += buffer.Length;

            // calc crc
            ushort crc = MavlinkCRC.crc_calculate(buffer, buffer.Length - 2);

            // calc extra bit of crc for mavlink 1.0
            if (buffer.Length > 5 && buffer[0] == 254)
            {
                crc = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[buffer[5]], crc);
            }

            // check message length vs table
            if (buffer.Length > 5 && buffer[1] != MAVLINK_MESSAGE_LENGTHS[buffer[5]])
            {
                if (MAVLINK_MESSAGE_LENGTHS[buffer[5]] == 0) // pass for unknown packets
                {
                    log.InfoFormat("unknown packet type {0}", buffer[5]);
                }
                else
                {
                    log.InfoFormat("Mavlink Bad Packet (Len Fail) len {0} pkno {1}", buffer.Length, buffer[5]);
                    if (buffer.Length == 11 && buffer[0] == 'U' && buffer[5] == 0) // check for 0.9 hb packet
                    {
                        string message =
                            "Mavlink 0.9 Heartbeat, Please upgrade your AP, This planner is for Mavlink 1.0\n\n";
                        Console.WriteLine(message);
                        if (logreadmode)
                            logplaybackfile.BaseStream.Seek(0, SeekOrigin.End);
                        throw new Exception(message);
                    }
                    return new byte[0];
                }
            }

            // check crc
            if (buffer.Length < 5 || buffer[buffer.Length - 1] != (crc >> 8) ||
                buffer[buffer.Length - 2] != (crc & 0xff))
            {
                int packetno = -1;
                if (buffer.Length > 5)
                {
                    packetno = buffer[5];
                }
                if (packetno != -1 && buffer.Length > 5 && MAVLINK_MESSAGE_INFO[packetno] != null)
                    log.InfoFormat("Mavlink Bad Packet (crc fail) len {0} crc {1} vs {4} pkno {2} {3}", buffer.Length,
                        crc, packetno, MAVLINK_MESSAGE_INFO[packetno].ToString(),
                        BitConverter.ToUInt16(buffer, buffer.Length - 2));
                if (logreadmode)
                    log.InfoFormat("bad packet pos {0} ", logplaybackfile.BaseStream.Position);
                return new byte[0];
            }

            // packet is now verified

            byte sysid = buffer[3];
            byte compid = buffer[4];

            // update packet loss statistics
            if (!logreadmode && MAVlist[sysid, compid].packetlosttimer.AddSeconds(5) < DateTime.Now)
            {
                MAVlist[sysid, compid].packetlosttimer = DateTime.Now;
                MAVlist[sysid, compid].packetslost = (MAVlist[sysid, compid].packetslost*0.8f);
                MAVlist[sysid, compid].packetsnotlost = (MAVlist[sysid, compid].packetsnotlost*0.8f);
            }
            else if (logreadmode && MAVlist[sysid, compid].packetlosttimer.AddSeconds(5) < lastlogread)
            {
                MAVlist[sysid, compid].packetlosttimer = lastlogread;
                MAVlist[sysid, compid].packetslost = (MAVlist[sysid, compid].packetslost*0.8f);
                MAVlist[sysid, compid].packetsnotlost = (MAVlist[sysid, compid].packetsnotlost*0.8f);
            }

            // extract wp's from stream
            if (buffer.Length >= 5)
            {
                getWPsfromstream(ref buffer, sysid, compid);
            }

            // if its a gcs packet - dont process further
            if (buffer.Length >= 5 && (sysid == 255 || sysid == 253) && logreadmode) // gcs packet
            {
                return buffer; // new byte[0];
            }

            // 3dr radios dont send a hb, so no mavstate is ever created, this overrides that behavior
            if (sysid == 51 && compid == 68 && !MAVlist.Contains(51,68))
            {
                // create an item
                MAVlist[sysid, compid] = MAVlist[sysid, compid];
                MAVlist[sysid, compid].sysid = sysid;
                MAVlist[sysid, compid].compid = compid;
            }

            try
            {
                if ((buffer[0] == 'U' || buffer[0] == 254) && buffer.Length >= buffer[1])
                {
                    // check if we lost pacakets based on seqno
                    byte packetSeqNo = buffer[2];
                    int expectedPacketSeqNo = ((MAVlist[sysid, compid].recvpacketcount + 1)%0x100);

                    {
                        // the seconds part is to work around a 3dr radio bug sending dup seqno's
                        if (packetSeqNo != expectedPacketSeqNo && packetSeqNo != MAVlist[sysid, compid].recvpacketcount)
                        {
                            MAVlist[sysid, compid].synclost++; // actualy sync loss's
                            int numLost = 0;

                            if (packetSeqNo < ((MAVlist[sysid, compid].recvpacketcount + 1)))
                                // recvpacketcount = 255 then   10 < 256 = true if was % 0x100 this would fail
                            {
                                numLost = 0x100 - expectedPacketSeqNo + packetSeqNo;
                            }
                            else
                            {
                                numLost = packetSeqNo - expectedPacketSeqNo;
                            }

                            MAVlist[sysid, compid].packetslost += numLost;
                            WhenPacketLost.OnNext(numLost);

                            log.InfoFormat("mav {2} seqno {0} exp {3} pkts lost {1}", packetSeqNo, numLost, sysid,
                                expectedPacketSeqNo);
                        }

                        MAVlist[sysid, compid].packetsnotlost++;

                        //Console.WriteLine("{0} {1}", sysid, packetSeqNo);

                        MAVlist[sysid, compid].recvpacketcount = packetSeqNo;
                    }
                    WhenPacketReceived.OnNext(1);

                    // packet stats per mav
                    if (double.IsInfinity(MAVlist[sysid, compid].packetspersecond[buffer[5]]))
                        MAVlist[sysid, compid].packetspersecond[buffer[5]] = 0;

                    MAVlist[sysid, compid].packetspersecond[buffer[5]] = (((1000/
                                                                            ((DateTime.Now -
                                                                              MAVlist[sysid, compid]
                                                                                  .packetspersecondbuild[buffer[5]])
                                                                                .TotalMilliseconds) +
                                                                            MAVlist[sysid, compid].packetspersecond[
                                                                                buffer[5]])/2));

                    MAVlist[sysid, compid].packetspersecondbuild[buffer[5]] = DateTime.Now;

                    //Console.WriteLine("Packet {0}",temp[5]);
                    // store packet history
                    lock (objlock)
                    {
                        MAVlist[sysid, compid].packets[buffer[5]] = buffer;
                        MAVlist[sysid, compid].packetseencount[buffer[5]]++;

                        // 3dr radio status packet are injected into the current mav
                        if (buffer[5] == (byte) MAVLink.MAVLINK_MSG_ID.RADIO_STATUS ||
                            buffer[5] == (byte) MAVLink.MAVLINK_MSG_ID.RADIO)
                        {
                            MAVlist[sysidcurrent, compidcurrent].packets[buffer[5]] = buffer;
                            MAVlist[sysidcurrent, compidcurrent].packetseencount[buffer[5]]++;
                        }

                        // adsb packets are forwarded and can be from any sysid/compid
                        if (buffer[5] == (byte)MAVLink.MAVLINK_MSG_ID.ADSB_VEHICLE)
                        {
                            var adsb = buffer.ByteArrayToStructure<MAVLink.mavlink_adsb_vehicle_t>(6);

                            MainV2.instance.adsbPlanes[adsb.ICAO_address.ToString("X5")] = new MissionPlanner.Utilities.adsb.PointLatLngAltHdg(adsb.lat / 1e7, adsb.lon / 1e7, adsb.altitude / 1000, adsb.heading, adsb.ICAO_address.ToString("X5"));
                            MainV2.instance.adsbPlaneAge[adsb.ICAO_address.ToString("X5")] = DateTime.Now;
                        }
                    }

                    // set seens sysid's based on hb packet - this will hide 3dr radio packets
                    if (buffer[5] == (byte) MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                    {
                        mavlink_heartbeat_t hb = buffer.ByteArrayToStructure<mavlink_heartbeat_t>(6);

                        // not a gcs
                        if (hb.type != (byte) MAV_TYPE.GCS)
                        {
                            // add a seen sysid
                            if (!MAVlist.Contains(sysid, compid))
                            {
                                // ensure its set from connect or log playback
                                MAVlist.Create(sysid, compid);
                                MAVlist[sysid, compid].aptype = (MAV_TYPE) hb.type;
                                MAVlist[sysid, compid].apname = (MAV_AUTOPILOT) hb.autopilot;
                                setAPType(sysid, compid);
                            }

                            // attach to the only remote device. / default to first device seen
                            if (MAVlist.Count == 1)
                            {
                                sysidcurrent = sysid;
                                compidcurrent = compid;
                            }
                        }
                    }

                    // only process for active mav
                    if (sysidcurrent == sysid && compidcurrent == compid)
                        PacketReceived(buffer);

                    if (debugmavlink)
                        DebugPacket(buffer);

                    if (buffer[5] == (byte) MAVLink.MAVLINK_MSG_ID.STATUSTEXT) // status text
                    {
                        var msg =
                            MAVlist[sysid, compid].packets[(byte) MAVLink.MAVLINK_MSG_ID.STATUSTEXT]
                                .ByteArrayToStructure<MAVLink.mavlink_statustext_t>(6);

                        byte sev = msg.severity;

                        string logdata = Encoding.ASCII.GetString(msg.text);
                        int ind = logdata.IndexOf('\0');
                        if (ind != -1)
                            logdata = logdata.Substring(0, ind);
                        log.Info(DateTime.Now + " " + sev + " " + logdata);

                        MAVlist[sysid, compid].cs.messages.Add(logdata);

                        bool printit = false;

                        // the change of severity and the autopilot version where introduced at the same time, so any version non 0 can be used
                        // copter 3.4+
                        // plane 3.4+
                        if (MAVlist[sysid, compid].cs.version.Major > 0 || MAVlist[sysid, compid].cs.version.Minor >= 4)
                        {
                            if (sev <= (byte) MAV_SEVERITY.WARNING)
                            {
                                printit = true;
                            }
                        }
                        else
                        {
                            if (sev >= 3)
                            {
                                printit = true;
                            }
                        }

                        if (printit)
                        {
                            MAVlist[sysid, compid].cs.messageHigh = logdata;
                            MAVlist[sysid, compid].cs.messageHighTime = DateTime.Now;

                            if (MainV2.speechEngine != null &&
                                MainV2.speechEngine.State == System.Speech.Synthesis.SynthesizerState.Ready &&
                                MainV2.config["speechenable"] != null &&
                                MainV2.config["speechenable"].ToString() == "True")
                            {
                                MainV2.speechEngine.SpeakAsync(logdata);
                            }
                        }
                    }

                    if (lastparamset != DateTime.MinValue && lastparamset.AddSeconds(10) < DateTime.Now)
                    {
                        lastparamset = DateTime.MinValue;

                        if (BaseStream.IsOpen)
                        {
                            doCommand(MAV_CMD.PREFLIGHT_STORAGE, 0, 0, 0, 0, 0, 0, 0);
                        }
                    }

                    getWPsfromstream(ref buffer, sysid, compid);

                    try
                    {
                        if (logfile != null && logfile.CanWrite && !logreadmode)
                        {
                            lock (logfile)
                            {
                                byte[] datearray =
                                    BitConverter.GetBytes(
                                        (UInt64) ((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds*1000));
                                Array.Reverse(datearray);
                                logfile.Write(datearray, 0, datearray.Length);
                                logfile.Write(buffer, 0, buffer.Length);

                                if (buffer[5] == 0)
                                {
// flush on heartbeat - 1 seconds
                                    logfile.Flush();
                                    rawlogfile.Flush();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    try
                    {
                        // full rw from mirror stream
                        if (MirrorStream != null && MirrorStream.IsOpen)
                        {
                            MirrorStream.Write(buffer, 0, buffer.Length);

                            while (MirrorStream.BytesToRead > 0)
                            {
                                byte[] buf = new byte[1024];

                                int len = MirrorStream.Read(buf, 0, buf.Length);

                                if (MirrorStreamWrite)
                                    BaseStream.Write(buf, 0, len);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            // update last valid packet receive time
            MAVlist[sysid, compid].lastvalidpacket = DateTime.Now;

            return buffer;
        }

        private void PacketReceived(byte[] buffer)
        {
            MAVLINK_MSG_ID type = (MAVLINK_MSG_ID) buffer[5];

            lock (Subscriptions)
            {
                foreach (var item in Subscriptions)
                {
                    if (item.Key == type)
                    {
                        try
                        {
                            item.Value(buffer);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                }
            }
        }

        List<KeyValuePair<MAVLINK_MSG_ID, Func<byte[], bool>>> Subscriptions =
            new List<KeyValuePair<MAVLINK_MSG_ID, Func<byte[], bool>>>();

        public KeyValuePair<MAVLINK_MSG_ID, Func<byte[], bool>> SubscribeToPacketType(MAVLINK_MSG_ID type,
            Func<byte[], bool> function, bool exclusive = false)
        {
            var item = new KeyValuePair<MAVLINK_MSG_ID, Func<byte[], bool>>(type, function);

            lock (Subscriptions)
            {
                if (exclusive)
                {
                    foreach (var subitem in Subscriptions)
                    {
                        if (subitem.Key == item.Key)
                        {
                            Subscriptions.Remove(subitem);
                            break;
                        }
                    }
                }

                log.Info("SubscribeToPacketType " + item.Key + " " + item.Value);

                Subscriptions.Add(item);
            }

            return item;
        }

        public void UnSubscribeToPacketType(KeyValuePair<MAVLINK_MSG_ID, Func<byte[], bool>> item)
        {
            lock (Subscriptions)
            {
                log.Info("UnSubscribeToPacketType " + item.Key + " " + item.Value);
                Subscriptions.Remove(item);
            }
        }

        /// <summary>
        /// Used to extract mission from log file - both sent or received
        /// </summary>
        /// <param name="buffer">packet</param>
        void getWPsfromstream(ref byte[] buffer, byte sysid, byte compid)
        {
            if (buffer[5] == (byte) MAVLINK_MSG_ID.MISSION_COUNT)
            {
                // clear old
                mavlink_mission_count_t wp = buffer.ByteArrayToStructure<mavlink_mission_count_t>(6);

                if (wp.target_system == gcssysid)
                {
                    wp.target_system = sysid;
                    wp.target_component = compid;
                }

                MAVlist[wp.target_system, wp.target_component].wps.Clear();
            }

            if (buffer[5] == (byte) MAVLink.MAVLINK_MSG_ID.MISSION_ITEM)
            {
                mavlink_mission_item_t wp = buffer.ByteArrayToStructure<mavlink_mission_item_t>(6);

                if (wp.target_system == gcssysid)
                {
                    wp.target_system = sysid;
                    wp.target_component = compid;
                }

                if (wp.current == 2)
                {
                    // guide mode wp
                    MAVlist[wp.target_system, wp.target_component].GuidedMode = wp;
                }
                else
                {
                    MAVlist[wp.target_system, wp.target_component].wps[wp.seq] = wp;
                }

                Console.WriteLine("WP # {7} cmd {8} p1 {0} p2 {1} p3 {2} p4 {3} x {4} y {5} z {6}", wp.param1, wp.param2,
                    wp.param3, wp.param4, wp.x, wp.y, wp.z, wp.seq, wp.command);
            }

            if (buffer[5] == (byte) MAVLINK_MSG_ID.RALLY_POINT)
            {
                mavlink_rally_point_t rallypt = buffer.ByteArrayToStructure<mavlink_rally_point_t>(6);

                if (rallypt.target_system == gcssysid)
                {
                    rallypt.target_system = sysid;
                    rallypt.target_component = compid;
                }

                MAVlist[rallypt.target_system, rallypt.target_component].rallypoints[rallypt.idx] = rallypt;

                Console.WriteLine("RP # {0} {1} {2} {3} {4}", rallypt.idx, rallypt.lat, rallypt.lng, rallypt.alt,
                    rallypt.break_alt);
            }

            if (buffer[5] == (byte) MAVLINK_MSG_ID.FENCE_POINT)
            {
                mavlink_fence_point_t fencept = buffer.ByteArrayToStructure<mavlink_fence_point_t>(6);

                if (fencept.target_system == gcssysid)
                {
                    fencept.target_system = sysid;
                    fencept.target_component = compid;
                }

                MAVlist[fencept.target_system, fencept.target_component].fencepoints[fencept.idx] = fencept;
            }

            if (buffer[5] == (byte) MAVLINK_MSG_ID.PARAM_VALUE)
            {
                mavlink_param_value_t value = buffer.ByteArrayToStructure<mavlink_param_value_t>(6);

                string st = System.Text.ASCIIEncoding.ASCII.GetString(value.param_id);

                int pos = st.IndexOf('\0');

                if (pos != -1)
                {
                    st = st.Substring(0, pos);
                }

                if (MAV.apname == MAV_AUTOPILOT.ARDUPILOTMEGA)
                {
                    MAVlist[sysid, compid].param[st] = new MAVLinkParam(st, value.param_value, MAV_PARAM_TYPE.REAL32);
                }
                else
                {
                    MAVlist[sysid, compid].param[st] = new MAVLinkParam(st, value.param_value,
                        (MAV_PARAM_TYPE) value.param_type);
                }
            }
        }

        public bool getVersion()
        {
            MAVLink.mavlink_autopilot_version_request_t req = new mavlink_autopilot_version_request_t();

            req.target_component = MAV.compid;
            req.target_system = MAV.sysid;

            // request point
            generatePacket((byte) MAVLINK_MSG_ID.AUTOPILOT_VERSION_REQUEST, req);

            DateTime start = DateTime.Now;
            int retrys = 3;

            while (true)
            {
                if (!(start.AddMilliseconds(200) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("getVersion Retry " + retrys + " - giv com " + giveComport);
                        generatePacket((byte) MAVLINK_MSG_ID.AUTOPILOT_VERSION_REQUEST, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    return false;
                }

                byte[] buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.AUTOPILOT_VERSION)
                    {
                        giveComport = false;

                        return true;
                    }
                }
            }
        }

        public PointLatLngAlt getFencePoint(int no, ref int total)
        {
            byte[] buffer;

            giveComport = true;

            PointLatLngAlt plla = new PointLatLngAlt();
            mavlink_fence_fetch_point_t req = new mavlink_fence_fetch_point_t();

            req.idx = (byte) no;
            req.target_component = MAV.compid;
            req.target_system = MAV.sysid;

            // request point
            generatePacket((byte) MAVLINK_MSG_ID.FENCE_FETCH_POINT, req);

            DateTime start = DateTime.Now;
            int retrys = 3;

            while (true)
            {
                if (!(start.AddMilliseconds(700) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("getFencePoint Retry " + retrys + " - giv com " + giveComport);
                        generatePacket((byte) MAVLINK_MSG_ID.FENCE_FETCH_POINT, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - getFencePoint");
                }

                buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.FENCE_POINT)
                    {
                        giveComport = false;

                        mavlink_fence_point_t fp = buffer.ByteArrayToStructure<mavlink_fence_point_t>(6);

                        plla.Lat = fp.lat;
                        plla.Lng = fp.lng;
                        plla.Tag = fp.idx.ToString();

                        total = fp.count;

                        return plla;
                    }
                }
            }
        }

        public FileStream GetLog(ushort no)
        {
            FileStream ms = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite);
            Hashtable set = new Hashtable();

            giveComport = true;
            byte[] buffer;

            if (Progress != null)
            {
                Progress((int) 0, "");
            }

            uint totallength = 0;
            uint ofs = 0;
            uint bps = 0;
            DateTime bpstimer = DateTime.Now;

            mavlink_log_request_data_t req = new mavlink_log_request_data_t();

            req.target_component = MAV.compid;
            req.target_system = MAV.sysid;
            req.id = no;
            req.ofs = ofs;
            // entire log
            req.count = 0xFFFFFFFF;

            // request point
            generatePacket((byte) MAVLINK_MSG_ID.LOG_REQUEST_DATA, req);

            DateTime start = DateTime.Now;
            int retrys = 3;

            while (true)
            {
                if (!(start.AddMilliseconds(1000) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("GetLog Retry " + retrys + " - giv com " + giveComport);
                        generatePacket((byte) MAVLINK_MSG_ID.LOG_REQUEST_DATA, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - GetLog");
                }

                buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.LOG_DATA)
                    {
                        var data = buffer.ByteArrayToStructure<mavlink_log_data_t>();

                        if (data.id != no)
                            continue;

                        // reset retrys
                        retrys = 3;
                        start = DateTime.Now;

                        bps += data.count;

                        // record what we have received
                        set[(data.ofs/90).ToString()] = 1;

                        ms.Seek((long) data.ofs, SeekOrigin.Begin);
                        ms.Write(data.data, 0, data.count);

                        // update new start point
                        req.ofs = data.ofs + data.count;

                        if (bpstimer.Second != DateTime.Now.Second)
                        {
                            if (Progress != null)
                            {
                                Progress((int) req.ofs, "");
                            }

                            //Console.WriteLine("log dl bps: " + bps.ToString());
                            bpstimer = DateTime.Now;
                            bps = 0;
                        }

                        // if data is less than max packet size or 0 > exit
                        if (data.count < 90 || data.count == 0)
                        {
                            totallength = data.ofs + data.count;
                            log.Info("start fillin len " + totallength + " count " + set.Count + " datalen " +
                                     data.count);
                            break;
                        }
                    }
                }
            }

            log.Info("set count " + set.Count);
            log.Info("count total " + ((totallength)/90 + 1));
            log.Info("totallength " + totallength);
            log.Info("current length " + ms.Length);

            while (true)
            {
                if (totallength == ms.Length && set.Count >= ((totallength) / 90 + 1))
                {
                    giveComport = false;
                    return ms;
                }

                if (!(start.AddMilliseconds(200) > DateTime.Now))
                {
                    for (int a = 0; a < ((totallength)/90 + 1); a++)
                    {
                        if (!set.ContainsKey(a.ToString()))
                        {
                            // request large chunk if they are back to back
                            uint bytereq = 90;
                            int b = a + 1;
                            while (!set.ContainsKey(b.ToString()))
                            {
                                bytereq += 90;
                                b++;
                            }

                            req.ofs = (uint) (a*90);
                            req.count = bytereq;
                            log.Info("req missing " + req.ofs + " " + req.count);
                            generatePacket((byte) MAVLINK_MSG_ID.LOG_REQUEST_DATA, req);
                            start = DateTime.Now;
                            break;
                        }
                    }
                }

                buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.LOG_DATA)
                    {
                        var data = buffer.ByteArrayToStructure<mavlink_log_data_t>();

                        if (data.id != no)
                            continue;

                        // reset retrys
                        retrys = 3;
                        start = DateTime.Now;

                        bps += data.count;

                        // record what we have received
                        set[(data.ofs/90).ToString()] = 1;

                        ms.Seek((long)data.ofs, SeekOrigin.Begin);
                        ms.Write(data.data, 0, data.count);

                        // update new start point
                        req.ofs = data.ofs + data.count;

                        if (bpstimer.Second != DateTime.Now.Second)
                        {
                            if (Progress != null)
                            {
                                Progress((int) req.ofs, "");
                            }

                            //Console.WriteLine("log dl bps: " + bps.ToString());
                            bpstimer = DateTime.Now;
                            bps = 0;
                        }

                        // check if we have next set and invalidate to request next packets
                        if (set.ContainsKey(((data.ofs/90) + 1).ToString()))
                        {
                            start = DateTime.MinValue;
                        }

                        // if data is less than max packet size or 0 > exit
                        if (data.count < 90 || data.count == 0)
                        {
                            continue;
                        }
                    }
                }
            }
        }

        public List<mavlink_log_entry_t> GetLogList()
        {
            List<mavlink_log_entry_t> ans = new List<mavlink_log_entry_t>();

            mavlink_log_entry_t entry1 = GetLogEntry(0, ushort.MaxValue);

            log.Info("id " + entry1.id + " lastllogno " + entry1.last_log_num + " #logs " + entry1.num_logs + " size " +
                     entry1.size);
            //ans.Add(entry1);

            for (ushort a = (ushort) (entry1.last_log_num - entry1.num_logs + 1); a <= entry1.last_log_num; a++)
            {
                mavlink_log_entry_t entry = GetLogEntry(a, a);
                ans.Add(entry);
            }

            return ans;
        }

        public void GetMountStatus()
        {
            mavlink_mount_status_t req = new MAVLink.mavlink_mount_status_t();
            req.target_component = MAV.compid;
            req.target_system = MAV.sysid;

            generatePacket((byte) MAVLINK_MSG_ID.MOUNT_STATUS, req);
        }

        public mavlink_log_entry_t GetLogEntry(ushort startno = 0, ushort endno = ushort.MaxValue)
        {
            giveComport = true;
            byte[] buffer;

            mavlink_log_request_list_t req = new mavlink_log_request_list_t();

            req.target_component = MAV.compid;
            req.target_system = MAV.sysid;
            req.start = startno;
            req.end = endno;

            log.Info("GetLogEntry " + startno + "-" + endno);

            // request point
            generatePacket((byte) MAVLINK_MSG_ID.LOG_REQUEST_LIST, req);

            DateTime start = DateTime.Now;
            int retrys = 5;

            while (true)
            {
                if (!(start.AddMilliseconds(1000) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("GetLogEntry Retry " + retrys + " - giv com " + giveComport);
                        generatePacket((byte) MAVLINK_MSG_ID.LOG_REQUEST_LIST, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - GetLogEntry");
                }

                buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.LOG_ENTRY)
                    {
                        var ans = buffer.ByteArrayToStructure<mavlink_log_entry_t>();

                        if (ans.id >= startno && ans.id <= endno)
                        {
                            giveComport = false;
                            return ans;
                        }
                    }
                }
            }
        }

        public void EraseLog()
        {
            mavlink_log_erase_t req = new mavlink_log_erase_t();

            req.target_component = MAV.compid;
            req.target_system = MAV.sysid;

            // send twice - we have no feedback on this
            generatePacket((byte) MAVLINK_MSG_ID.LOG_ERASE, req);
            generatePacket((byte) MAVLINK_MSG_ID.LOG_ERASE, req);
        }

        public List<PointLatLngAlt> getRallyPoints()
        {
            List<PointLatLngAlt> points = new List<PointLatLngAlt>();

            if (!MAV.param.ContainsKey("RALLY_TOTAL"))
                return points;

            int count = int.Parse(MAV.param["RALLY_TOTAL"].ToString());

            for (int a = 0; a < (count - 1); a++)
            {
                try
                {
                    PointLatLngAlt plla = getRallyPoint(a, ref count);
                    points.Add(plla);
                }
                catch
                {
                    return points;
                }
            }

            return points;
        }

        public PointLatLngAlt getRallyPoint(int no, ref int total)
        {
            byte[] buffer;

            giveComport = true;

            PointLatLngAlt plla = new PointLatLngAlt();
            mavlink_rally_fetch_point_t req = new mavlink_rally_fetch_point_t();

            req.idx = (byte) no;
            req.target_component = MAV.compid;
            req.target_system = MAV.sysid;

            // request point
            generatePacket((byte) MAVLINK_MSG_ID.RALLY_FETCH_POINT, req);

            DateTime start = DateTime.Now;
            int retrys = 3;

            while (true)
            {
                if (!(start.AddMilliseconds(700) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("getRallyPoint Retry " + retrys + " - giv com " + giveComport);
                        generatePacket((byte) MAVLINK_MSG_ID.FENCE_FETCH_POINT, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }
                    giveComport = false;
                    throw new Exception("Timeout on read - getRallyPoint");
                }

                buffer = readPacket();
                if (buffer.Length > 5)
                {
                    if (buffer[5] == (byte) MAVLINK_MSG_ID.RALLY_POINT)
                    {
                        mavlink_rally_point_t fp = buffer.ByteArrayToStructure<mavlink_rally_point_t>(6);

                        if (req.idx != fp.idx)
                        {
                            generatePacket((byte) MAVLINK_MSG_ID.FENCE_FETCH_POINT, req);
                            continue;
                        }

                        plla.Lat = fp.lat/t7;
                        plla.Lng = fp.lng/t7;
                        plla.Tag = fp.idx.ToString();
                        plla.Alt = fp.alt;

                        total = fp.count;

                        giveComport = false;

                        return plla;
                    }
                }
            }
        }

        public bool setFencePoint(byte index, PointLatLngAlt plla, byte fencepointcount)
        {
            mavlink_fence_point_t fp = new mavlink_fence_point_t();

            fp.idx = index;
            fp.count = fencepointcount;
            fp.lat = (float) plla.Lat;
            fp.lng = (float) plla.Lng;
            fp.target_component = MAV.compid;
            fp.target_system = MAV.sysid;

            int retry = 3;

            PointLatLngAlt newfp;

            while (retry > 0)
            {
                generatePacket((byte) MAVLINK_MSG_ID.FENCE_POINT, fp);
                int counttemp = 0;
                newfp = getFencePoint(fp.idx, ref counttemp);

                if (newfp.GetDistance(plla) < 5)
                    return true;
                retry--;
            }

            throw new Exception("Could not verify GeoFence Point");
        }

        public bool setRallyPoint(byte index, PointLatLngAlt plla, short break_alt, UInt16 land_dir_cd, byte flags,
            byte rallypointcount)
        {
            mavlink_rally_point_t rp = new mavlink_rally_point_t();

            rp.idx = index;
            rp.count = rallypointcount;
            rp.lat = (int) (plla.Lat*t7);
            rp.lng = (int) (plla.Lng*t7);
            rp.alt = (short) plla.Alt;
            rp.break_alt = break_alt;
            rp.land_dir = land_dir_cd;
            rp.flags = (byte) flags;
            rp.target_component = MAV.compid;
            rp.target_system = MAV.sysid;

            int retry = 3;

            while (retry > 0)
            {
                generatePacket((byte) MAVLINK_MSG_ID.RALLY_POINT, rp);
                int counttemp = 0;
                PointLatLngAlt newfp = getRallyPoint(rp.idx, ref counttemp);

                if (newfp.Lat == plla.Lat && newfp.Lng == rp.lng)
                {
                    Console.WriteLine("Rally Set");
                    return true;
                }
                retry--;
            }

            return false;
        }

        public enum sensoroffsetsenum
        {
            gyro = 0,
            accelerometer = 1,
            magnetometer = 2,
            barometer = 3,
            optical_flow = 4,
            second_magnetometer = 5
        }

        public bool SetSensorOffsets(sensoroffsetsenum sensor, float x, float y, float z)
        {
            return doCommand(MAV_CMD.PREFLIGHT_SET_SENSOR_OFFSETS, (int) sensor, x, y, z, 0, 0, 0);
        }

        byte[] readlogPacketMavlink()
        {
            byte[] temp = new byte[300];

            //byte[] datearray = BitConverter.GetBytes((ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);

            byte[] datearray = new byte[8];

            int tem = logplaybackfile.BaseStream.Read(datearray, 0, datearray.Length);

            Array.Reverse(datearray);

            DateTime date1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            UInt64 dateint = BitConverter.ToUInt64(datearray, 0);

            try
            {
                // array is reversed above
                if (datearray[7] == 254)
                {
                    //rewind 8bytes
                    logplaybackfile.BaseStream.Seek(-8, SeekOrigin.Current);
                }
                else
                {
                    date1 = date1.AddMilliseconds(dateint/1000);

                    lastlogread = date1.ToLocalTime();
                }
            }
            catch
            {
            }


            int length = 5;
            int a = 0;
            while (a < length)
            {
                if (logplaybackfile.BaseStream.Position == logplaybackfile.BaseStream.Length)
                    break;
                temp[a] = (byte) logplaybackfile.ReadByte();
                if (temp[0] != 'U' && temp[0] != 254)
                {
                    log.InfoFormat("logread - lost sync byte {0} pos {1}", temp[0], logplaybackfile.BaseStream.Position);
                    a = 0;
                    continue;
                }
                if (a == 1)
                {
                    length = temp[1] + 6 + 2; // 6 header + 2 checksum
                }
                a++;
            }

            MAVlist[temp[3], temp[4]].cs.datetime = lastlogread;

            return temp;
        }

        public bool translateMode(string modein, ref MAVLink.mavlink_set_mode_t mode)
        {
            mode.target_system = MAV.sysid;

            if (modein == null || modein == "")
                return false;

            try
            {
                List<KeyValuePair<int, string>> modelist = Common.getModesList(MAV.cs);

                foreach (KeyValuePair<int, string> pair in modelist)
                {
                    if (pair.Value.ToLower() == modein.ToLower())
                    {
                        mode.base_mode = (byte) MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED;
                        mode.custom_mode = (uint) pair.Key;
                    }
                }

                if (mode.base_mode == 0)
                {
                    MessageBox.Show("No Mode Changed " + modein);
                    return false;
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Failed to find Mode");
                return false;
            }

            return true;
        }

        public void setAPType(byte sysid, byte compid)
        {
            MAVlist[sysid, compid].sysid = sysid;
            MAVlist[sysid, compid].compid = compid;

            switch (MAVlist[sysid, compid].apname)
            {
                case MAV_AUTOPILOT.ARDUPILOTMEGA:
                    switch (MAVlist[sysid, compid].aptype)
                    {
                        case MAVLink.MAV_TYPE.FIXED_WING:
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.ArduPlane;
                            break;
                        case MAVLink.MAV_TYPE.QUADROTOR:
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.ArduCopter2;
                            break;
                        case MAVLink.MAV_TYPE.TRICOPTER:
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.ArduCopter2;
                            break;
                        case MAVLink.MAV_TYPE.HEXAROTOR:
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.ArduCopter2;
                            break;
                        case MAVLink.MAV_TYPE.OCTOROTOR:
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.ArduCopter2;
                            break;
                        case MAVLink.MAV_TYPE.HELICOPTER:
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.ArduCopter2;
                            break;
                        case MAVLink.MAV_TYPE.GROUND_ROVER:
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.ArduRover;
                            break;
                        case MAV_TYPE.ANTENNA_TRACKER:
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.ArduTracker;
                            break;
                        default:
                            break;
                    }
                    break;
                case MAV_AUTOPILOT.UDB:
                    switch (MAVlist[sysid, compid].aptype)
                    {
                        case MAVLink.MAV_TYPE.FIXED_WING:
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.ArduPlane;
                            break;
                    }
                    break;
                case MAV_AUTOPILOT.GENERIC:
                    switch (MAVlist[sysid, compid].aptype)
                    {
                        case MAVLink.MAV_TYPE.FIXED_WING:
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.Ateryx;
                            break;
                    }
                    break;
                default:
                    switch (MAVlist[sysid, compid].aptype)
                    {
                        case MAV_TYPE.GIMBAL: // storm32 - name 83
                            MAVlist[sysid, compid].cs.firmware = MainV2.Firmwares.Gymbal;
                            break;
                    }
                    break;
            }

            switch (MAVlist[sysid, compid].cs.firmware)
            {
                case MainV2.Firmwares.ArduCopter2:
                    MAVlist[sysid, compid].Guid = MainV2.config["copter_guid"].ToString();
                    break;
                case MainV2.Firmwares.ArduPlane:
                    MAVlist[sysid, compid].Guid = MainV2.config["plane_guid"].ToString();
                    break;
                case MainV2.Firmwares.ArduRover:
                    MAVlist[sysid, compid].Guid = MainV2.config["rover_guid"].ToString();
                    break;
            }
        }

        public override string ToString()
        {
            if (BaseStream.IsOpen)
                return "MAV " + MAV.sysid + " on " + BaseStream.PortName;

            if (logreadmode)
                return "MAV " + MAV.sysid + " on LogFile";

            return "MAV " + MAV.sysid + " on Ice";
        }


        public void Dispose()
        {
            if (_bytesReceivedSubj != null)
                _bytesReceivedSubj.Dispose();
            if (_bytesSentSubj != null)
                _bytesSentSubj.Dispose();
            this.Close();

            Terrain = null;

            MirrorStream = null;

            logreadmode = false;
            logplaybackfile = null;
        }
    }
}