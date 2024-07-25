using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Comms;
using MissionPlanner.Mavlink;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace MissionPlanner
{
    public class MAVLinkInterface : MAVLink, IDisposable, IMAVLinkInterface, IMAVLinkInterfaceLogRead
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ICommsSerial _baseStream;

        public ICommsSerial BaseStream
        {
            get { return _baseStream; }
            set
            {
                // This is called every time user changes the port selection, so we need to make sure we cleanup
                // any previous objects so we don't leave the cleanup of system resources to the garbage collector.
                if (_baseStream != null)
                {
                    try
                    {
                        if (_baseStream.IsOpen)
                        {
                            _baseStream.Close();
                        }
                    }
                    catch
                    {
                    }

                    IDisposable dsp = _baseStream as IDisposable;
                    if (dsp != null)
                    {
                        try
                        {
                            dsp.Dispose();
                        }
                        catch
                        {
                        }
                    }
                }

                _baseStream = value;
            }
        }

        public event EventHandler<MAVLinkMessage> OnPacketReceived
        {
            add
            {
                log.Debug("Subscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value?.Method?.Name +
                         " " + value?.Target?.GetType()?.Name +
                         " " + value);
                _OnPacketReceived += value;
            }

            remove
            {
                log.Debug("UnSubscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " +
                         value?.Method?.Name + " " + value?.Target?.GetType()?.Name +
                         " " + value);
                _OnPacketReceived -= value;
            }
        }

        public event EventHandler<(byte, byte)> MAVDetected
        {
            add
            {
                log.Debug("Subscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value?.Method?.Name +
                         " " + value?.Target?.GetType()?.Name +
                         " " + value);
                _MAVDetected += value;
            }

            remove
            {
                log.Debug("UnSubscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " +
                         value?.Method?.Name + " " + value?.Target?.GetType()?.Name +
                         " " + value);
                _MAVDetected -= value;
            }
        }

        private EventHandler<(byte, byte)> _MAVDetected;

        public void DoOnPacketSent(MAVLinkMessage pkt)
        {
            _OnPacketSent?.Invoke(this, pkt);
        }

        public event EventHandler<MAVLinkMessage> OnPacketSent
        {
            add
            {
                log.Debug("Subscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _OnPacketSent += value;
            }
            remove
            {
                log.Debug("UnSubscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _OnPacketSent -= value;
            }
        }

        public static event EventHandler<adsb.PointLatLngAltHdg> UpdateADSBPlanePosition
        {
            add
            {
                log.Info("Subscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _UpdateADSBPlanePosition += value;
            }
            remove
            {
                log.Info("UnSubscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _UpdateADSBPlanePosition -= value;
            }
        }

        public static event EventHandler<(string id, MAV_COLLISION_THREAT_LEVEL threat_level)> UpdateADSBCollision
        {
            add
            {
                log.Info("Subscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _UpdateADSBCollision += value;
            }
            remove
            {
                log.Info("UnSubscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _UpdateADSBCollision -= value;
            }
        }

        public ICommsSerial MirrorStream { get; set; }
        public bool MirrorStreamWrite { get; set; }

        public event EventHandler ParamListChanged
        {
            add
            {
                log.Debug("Subscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _ParamListChanged += value;
            }
            remove
            {
                log.Debug("UnSubscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _ParamListChanged -= value;
            }
        }

        public event EventHandler MavChanged
        {
            add
            {
                log.Info("Subscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _MavChanged += value;
            }
            remove
            {
                log.Info("UnSubscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _MavChanged -= value;
            }
        }

        public event EventHandler CommsClose
        {
            add
            {
                log.Info("Subscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _CommsClose += value;
            }
            remove
            {
                log.Info("UnSubscribed " + new StackTrace(1, true)?.GetFrame(0)?.ToString() + " " + value.Method.Name +
                         " " + value.Target.GetType().Name +
                         " " + value);
                _CommsClose -= value;
            }
        }

        public static byte gcssysid { get; set; } = 255;

        private string lastset = "";

        /// <summary>
        /// used to prevent comport access for exclusive use
        /// </summary>
        public bool giveComport
        {
            get
            {
                //Console.WriteLine("get giveComport {0} current {1}", Thread.CurrentThread.Name, _giveComport);
                return _giveComport;
            }
            set
            {
                log.InfoFormat("set giveComport {0} current {1} new {2}", Thread.CurrentThread.Name, _giveComport,
                    value);
                if (_giveComport && value)
                {
                    // trying to set it true twice
                    log.Error(new System.Diagnostics.StackTrace().ToString());
                }

                if (value == true)
                {
                    // wait for current read to end before continue
                    lock (readlock)
                    {
                    }
                }

                lastset = Thread.CurrentThread.Name;
                _giveComport = value;
            }
        }

        private volatile bool _giveComport = false;

        private DateTime lastparamset = DateTime.MinValue;

        /// <summary>
        /// stores a list of strings during connect for possible debug
        /// </summary>
        internal List<string> plaintxtlinebuffer = new List<string>(30);

        /// <summary>
        /// stores a single string during connect for possible debug
        /// </summary>
        internal string plaintxtline = "";
        private string buildplaintxtline = "";

        public bool ReadOnly = false;

        public TerrainFollow Terrain;

        public event ProgressEventHandler Progress;

        private int _sysidcurrent = 0;

        public int sysidcurrent
        {
            get { return _sysidcurrent; }
            set
            {
                if (_sysidcurrent == value)
                    return;
                _sysidcurrent = value;
                if (_MavChanged != null) _MavChanged(this, null);
            }
        }

        private int _compidcurrent = 0;

        public int compidcurrent
        {
            get { return _compidcurrent; }
            set
            {
                if (_compidcurrent == value)
                    return;
                _compidcurrent = value;
                if (_MavChanged != null) _MavChanged(this, null);
            }
        }

        public MAVList MAVlist;

        public MAVState MAV
        {
            get { return MAVlist[sysidcurrent, compidcurrent]; }
            set { MAVlist[sysidcurrent, compidcurrent] = value; }
        }

        public double CONNECT_TIMEOUT_SECONDS = Settings.Instance.GetDouble("CONNECT_TIMEOUT_SECONDS", 30.0);

        /// <summary>
        /// progress form to handle connect and param requests
        /// </summary>
        public IProgressReporterDialogue frmProgressReporter;

        /// <summary>
        /// used for outbound packet sending
        /// </summary>
        public int packetcount { get; internal set; } = 0;

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
        public volatile object writelock = new object();

        /// <summary>
        /// mavlink version
        /// </summary>
        private byte mavlinkversion = 0;

        /// <summary>
        /// turns on console packet display
        /// </summary>
        public bool debugmavlink { get; set; }

        /// <summary>
        /// enabled read from file mode
        /// </summary>
        public bool logreadmode
        {
            get { return _logreadmode; }
            set { _logreadmode = value; }
        }

        /// <summary>
        /// used to disable all speech originating from this module
        /// </summary>
        public bool speechenabled { get; set; } = true;

        private bool _logreadmode = false;

        private BinaryReader _logplaybackfile;

        public DateTime lastlogread { get; set; }
        public DateTime lastlogreadwall { get; set; }

        public BinaryReader logplaybackfile
        {
            get { return _logplaybackfile; }
            set
            {
                _logplaybackfile = value;
                if (_logplaybackfile != null && _logplaybackfile.BaseStream is FileStream)
                    log.Info("Logplaybackfile set " + ((FileStream) _logplaybackfile.BaseStream).Name);
                MAVlist.Clear();
            }
        }

        public BufferedStream logfile { get; set; }
        public BufferedStream rawlogfile { get; set; }

        private int _mavlink1count = 0;
        private int _mavlink2count = 0;
        private int _mavlink2signed = 0;
        private int _bps1 = 0;
        private int _bps2 = 0;
        private DateTime _bpstime { get; set; }

        public static ISpeech Speech;

        ~MAVLinkInterface()
        {
            this.Dispose();
        }

        public MAVLinkInterface()
        {
            // init fields
            MAVlist = new MAVList(this);
            this.packetcount = 0;
            this._bytesReceivedSubj = new Subject<int>();
            this._bytesSentSubj = new Subject<int>();
            this.WhenPacketLost = new Subject<int>();
            this.WhenPacketReceived = new Subject<int>();

            this.mavlinkversion = 0;

            this.debugmavlink = false;
            this.logreadmode = false;
            this.lastlogread = DateTime.MinValue;
            this._logplaybackfile = null;
            this.logfile = null;
            this.rawlogfile = null;
            this._bps1 = 0;
            this._bps2 = 0;
            this._bpstime = DateTime.MinValue;
            _mavlink1count = 0;
            _mavlink2count = 0;
            _mavlink2signed = 0;

            AIS.Start(this);

            // new hearbeat detected
            MAVDetected += OnMAVDetected;
        }

        private void OnMAVDetected(object sender, (byte, byte) tuple)
        {

            // check for a camera
            if (tuple.Item2 >= (byte) MAVLink.MAV_COMPONENT.MAV_COMP_ID_CAMERA &&
                tuple.Item2 <= (byte) MAV_COMPONENT.MAV_COMP_ID_CAMERA6)
            {
                MAVlist[tuple.Item1, tuple.Item2].Camera = new CameraProtocol();
                Task.Run(async () =>
                {
                    try
                    {
                        // Open holds this
                        while (!_openComplete)
                        {
                            await Task.Delay(1000);
                        }

                        await Task.Delay(2000);

                        if (MAVlist[tuple.Item1, tuple.Item2].Camera == null)
                            return;

                        await MAVlist[tuple.Item1, tuple.Item2]
                            .Camera.StartID(MAVlist[tuple.Item1, tuple.Item2])
                            .ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }
                });
            }
            // gimbals
            if (tuple.Item2 >= (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_GIMBAL &&
                tuple.Item2 <= (byte)MAV_COMPONENT.MAV_COMP_ID_GIMBAL6)
            {
                MAVlist[tuple.Item1, tuple.Item2].Gimbal = new GimbalProtocol();
                Task.Run(async () =>
                {
                    try
                    {
                        // Open holds this
                        while (!_openComplete)
                        {
                            await Task.Delay(1000);
                        }

                        await Task.Delay(2000);

                        MAVlist[tuple.Item1, tuple.Item2]
                            .Gimbal.Discover(this);
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }
                });
            }
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
                Terrain?.UnSub();
                Terrain = null;
            }
            catch
            {

            }

            try
            {
                giveComport = false;

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
                if (BaseStream != null && BaseStream.IsOpen)
                    BaseStream.Close();
            }
            catch
            {
            }

            try
            {
                if (_CommsClose != null)
                    _CommsClose(this, null);
            }
            catch
            {
            }
        }

        public delegate IProgressReporterDialogue ProgressEventHandle(string title);

        public static event ProgressEventHandle CreateIProgressReporterDialogue;

        public void Open()
        {
            Open(false);
        }

        public void Open(bool getparams, bool skipconnectedcheck = false, bool showui = true)
        {
            if (BaseStream == null || BaseStream.IsOpen && !skipconnectedcheck)
                return;

            MAVlist.Clear();

            if (showui)
                frmProgressReporter = CreateIProgressReporterDialogue(Strings.ConnectingMavlink);
            else
                frmProgressReporter = new NoUIReporter();

            if (getparams)
            {
                frmProgressReporter.DoWork += FrmProgressReporterDoWorkAndParams;
            }
            else
            {
                frmProgressReporter.DoWork += FrmProgressReporterDoWorkNOParams;
            }

            frmProgressReporter.UpdateProgressAndStatus(-1, Strings.MavlinkConnecting);

            frmProgressReporter.RunBackgroundOperationAsync();

            frmProgressReporter.Dispose();

            if (_ParamListChanged != null)
            {
                _ParamListChanged(this, null);
            }
        }

        private void FrmProgressReporterDoWorkAndParams(IProgressReporterDialogue sender)
        {
            OpenBg(sender, true);
        }

        private void FrmProgressReporterDoWorkNOParams(IProgressReporterDialogue sender)
        {
            OpenBg(sender, false);
        }

        private void OpenBg(IProgressReporterDialogue PRsender, bool getparams)
        {
            _openComplete = false;

            frmProgressReporter.UpdateProgressAndStatus(-1, Strings.MavlinkConnecting);

            if (BaseStream is SerialPort)
            {
                // allow settings to settle - previous dtr
                log.Debug("SerialPort Sleep 1");
                Thread.Sleep(1000);
            }

            Terrain?.UnSub();
            Terrain = new TerrainFollow(this);

            bool hbseen = false;

            try
            {
                BaseStream.ReadBufferSize = 16 * 1024;

                lock (writelock) // so we dont have random traffic
                {
                    log.Info("Open port with " + BaseStream.PortName + " " + BaseStream.BaudRate);

                    if (BaseStream is UdpSerial)
                    {
                        PRsender.doWorkArgs.CancelRequestChanged += (o, e) =>
                        {
                            ((UdpSerial) BaseStream).CancelConnect = true;
                            ((ProgressWorkerEventArgs) o)
                                .CancelAcknowledged = true;
                        };
                    }

                    BaseStream.Open();

                    BaseStream.DiscardInBuffer();

                    // other boards seem to have issues if there is no delay? posible bootloader timeout issue
                    if (BaseStream is SerialPort)
                    {
                        log.Debug("SerialPort Sleep 2");
                        Thread.Sleep(1000);
                    }
                }

                List<MAVLinkMessage> hbhistory = new List<MAVLinkMessage>();

                DateTime start = DateTime.Now;
                DateTime deadline = start.AddSeconds(CONNECT_TIMEOUT_SECONDS);

                var countDown = new Timer {Interval = 1000, AutoReset = false};
                countDown.Elapsed += (sender, e) =>
                {
                    int secondsRemaining = (deadline - e.SignalTime).Seconds;
                    frmProgressReporter.UpdateProgressAndStatus(-1, string.Format(Strings.Trying, secondsRemaining));
                    if (secondsRemaining > 0) countDown.Start();
                };
                countDown.Start();

                int count = 0;

                plaintxtline = "";

                while (true)
                {
                    if (PRsender.doWorkArgs.CancelRequested)
                    {
                        PRsender.doWorkArgs.CancelAcknowledged = true;
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
                            PRsender.doWorkArgs.ErrorMessage = Strings.Only1Hb;
                            throw new Exception(Strings.Only1HbD + plaintxtlinebuffer.Aggregate((a, b) => a + "\r\n" + b));
                        }
                        else
                        {
                            PRsender.doWorkArgs.ErrorMessage = "No Heartbeat Packets Received";
                            throw new TimeoutException(@"Can not establish a connection

No Mavlink Heartbeat Packets where read from this port - Verify Baud Rate and setup
Mission Planner waits for 2 valid heartbeat packets before connecting
" + plaintxtlinebuffer.Aggregate((a, b) => a + "\r\n" + b));
                        }
                    }

                    Thread.Sleep(1);

                    var buffer = getHeartBeat();
                    if (buffer.Length > 0)
                    {
                        if (buffer.msgid == (ulong)MAVLINK_MSG_ID.HEARTBEAT)
                        {
                            mavlink_heartbeat_t hb = buffer.ToStructure<mavlink_heartbeat_t>();

                            // no GCS's and no broadcast compid's (ping adsb)
                            if (hb.type != (byte)MAV_TYPE.GCS && buffer.compid != 0)
                            {
                                hbhistory.Add(buffer);
                            }
                        }
                        else if (buffer.msgid == (ulong)MAVLINK_MSG_ID.HIGH_LATENCY2)
                        {
                            mavlink_high_latency2_t hl = buffer.ToStructure<mavlink_high_latency2_t>();
                            // no GCS's and no broadcast compid's (ping adsb)
                            if (hl.type != (byte)MAV_TYPE.GCS && buffer.compid != 0)
                            {
                                hbhistory.Add(buffer);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (hbhistory.Count > 0)
                        hbseen = true;

                    count++;

                    // if we get no data, try enableing rts/cts
                    if (buffer.Length == 0 && BaseStream is SerialPort && start.AddSeconds(20) < DateTime.Now)
                    {
                        try
                        {
                            log.Debug("about to set RTS to " + !BaseStream.RtsEnable);
                            BaseStream.RtsEnable = !BaseStream.RtsEnable;
                        }
                        catch
                        {
                        }
                    }

                    // SLCAN check
                    if (Regex.IsMatch(plaintxtline, @"\rT[0-9A-Z]{9,32}$", RegexOptions.Multiline))
                    {
                        PRsender.doWorkArgs.ErrorMessage =
                            "This appears to be a CAN port, please use the DroneCAN screen";
                        throw new Exception(@"This appears to be a CAN port, please use the DroneCAN screen");
                    }

                    // check we have hb's
                    if (hbhistory.Count > 0)
                    {
                        bool exit = false;
                        // get most seen hbs
                        var mostseenlist = hbhistory.GroupBy(s => MAVList.GetID(s.sysid, s.compid))
                            .OrderByDescending(s => s.Count()).Where(s => s.Count() >= 2);
                        foreach (var mostseen in mostseenlist)
                        {
                            // get count on most seen
                            var seentimes = mostseen.Count();
                            // get the most seen mavlinkmessage
                            var msg = mostseen.First();

                            // preference compid of 1, failover to anything that we have seen 4 times
                            if (seentimes >= 2 && msg.compid == 1 || seentimes >= 4)
                            {
                                if (msg.msgid == (ulong)MAVLINK_MSG_ID.HEARTBEAT)
                                    SetupMavConnect(msg, (mavlink_heartbeat_t) msg.data);
                                if (msg.msgid == (ulong)MAVLINK_MSG_ID.HIGH_LATENCY2)
                                    SetupMavConnect(msg, (mavlink_high_latency2_t)msg.data);
                                sysidcurrent = msg.sysid;
                                compidcurrent = msg.compid;
                                log.Info($"HB Selection {sysidcurrent}-{compidcurrent} seen {seentimes}");
                                exit = true;
                                break;
                            }
                        }

                        if (exit)
                            break;
                    }
                }

                countDown.Stop();

                char[] temp = ("Mission Planner " + getAppVersion() + "\0").ToCharArray();
                Array.Resize(ref temp, 50);

                //
                generatePacket((byte) MAVLINK_MSG_ID.STATUSTEXT,
                    new mavlink_statustext_t() {severity = (byte) MAV_SEVERITY.INFO, text = temp.ToByteArray()});

                // mavlink2
                generatePacket((byte) MAVLINK_MSG_ID.STATUSTEXT,
                    new mavlink_statustext_t() {severity = (byte) MAV_SEVERITY.INFO, text = temp.ToByteArray()},
                    sysidcurrent,
                    compidcurrent, true, false);

                // mavlink2 - signed
                generatePacket((byte) MAVLINK_MSG_ID.STATUSTEXT,
                    new mavlink_statustext_t() {severity = (byte) MAV_SEVERITY.INFO, text = temp.ToByteArray()},
                    sysidcurrent,
                    compidcurrent, true, true);

                // this ensures a mavlink2 change has been noticed
                getHeartBeat();

                getVersion((byte) sysidcurrent, (byte) compidcurrent);

                doCommand((byte) sysidcurrent, (byte) compidcurrent, MAV_CMD.DO_SEND_BANNER, 0, 0, 0, 0, 0, 0, 0,
                    false);

                if (getparams)
                {
                    frmProgressReporter.UpdateProgressAndStatus(0,
                        "Getting Params.. (sysid " + MAV.sysid + " compid " + MAV.compid + ") ");

                    getParamListMavftp(MAV.sysid, MAV.compid);
                }

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
                if (string.IsNullOrEmpty(PRsender.doWorkArgs.ErrorMessage))
                    PRsender.doWorkArgs.ErrorMessage = Strings.ConnectFailed;
                log.Error(e);
                throw;
            }

            //frmProgressReporter.Close();
            giveComport = false;
            frmProgressReporter.UpdateProgressAndStatus(100, Strings.Done);
            log.Info("Done open " + MAV.sysid + " " + MAV.compid);
            MAV.packetslost = 0;
            MAV.synclost = 0;
            _openComplete = true;
        }

        private string getAppVersion()
        {
            try
            {
                Assembly entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly != null)
                {
                    object[] customAttributes =
                        entryAssembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
                    if (customAttributes != null && customAttributes.Length != 0)
                    {
                        return ((AssemblyFileVersionAttribute) customAttributes[0]).Version;
                    }
                }
            }
            catch
            {
            }

            return "0.0";
        }

        private uint request_id = 0;

        /// <summary>
        /// devop read spi mpu6000 0 0 0xf5 1
        ///STABILIZE> Operation 1 OK: 1 bytes
        ///0  1  2  3  4  5  6  7  8  9  a b  c d  e f
        ///f0: -- -- -- -- -- 71 -- -- -- -- -- -- -- -- -- --
        /// </summary>
        /// <param name="bustype">spi/i2c</param>
        /// <param name="name">spi</param>
        /// <param name="bus">i2c</param>
        /// <param name="address">i2c</param>
        /// <param name="regstart"></param>
        /// <param name="count"></param>
        /// <param name="writebytes"></param>
        public byte device_op(byte sysid, byte compid, out byte[] outputbuffer, MAVLink.DEVICE_OP_BUSTYPE bustype,
            string name, byte bus,
            byte address, byte regstart, byte count, byte[] writebytes = null)
        {
            var responce = false;
            var buffer = new byte[0];
            byte result = 0;

            var sub = SubscribeToPacketType(MAVLINK_MSG_ID.DEVICE_OP_READ_REPLY, (m) =>
            {
                var mtype = (MAVLINK_MSG_ID)m.msgid;
                if (mtype == MAVLINK_MSG_ID.DEVICE_OP_READ_REPLY)
                {
                    var msg = (mavlink_device_op_read_reply_t)m.data;
                    if (msg.result != 0)
                        log.InfoFormat(name + " Operation {0} failed: {1}", msg.request_id, msg.result);
                    else
                        log.InfoFormat(name + " Operation {0} OK: {1} bytes", msg.request_id, msg.count);
                    for (var i = 0; i < msg.count; i++)
                    {
                        var reg = i + msg.regstart;
                        log.InfoFormat("{0,2:X}:{1,2:X} ", reg, msg.data[i]);
                        if ((i + 1) % 16 == 0)
                            Console.WriteLine();
                        if (msg.count % 16 != 0)
                            Console.WriteLine();
                    }

                    buffer = msg.data.Take(msg.count).ToArray();
                    responce = true;
                    result = msg.result;
                }

                return true;
            }, sysid, compid);

            var sub2 = SubscribeToPacketType(MAVLINK_MSG_ID.DEVICE_OP_WRITE_REPLY, (m) =>
            {
                var mtype = (MAVLINK_MSG_ID) m.msgid;
                if (mtype == MAVLINK_MSG_ID.DEVICE_OP_WRITE_REPLY)
                {
                    var msg = (mavlink_device_op_write_reply_t) m.data;
                    if (msg.result != 0)
                        log.InfoFormat(name + " Operation {0} failed: {1}", msg.request_id, msg.result);
                    else
                        log.InfoFormat(name + " Operation {0} OK", msg.request_id);

                    responce = true;
                    result = msg.result;
                }

                return true;
            }, sysid, compid);

            if (writebytes != null)
            {
                var write = new mavlink_device_op_write_t()
                {
                    target_system = (byte) sysid,
                    target_component = (byte) compid,
                    request_id = request_id++,
                    bustype = (byte) bustype,
                    bus = bus,
                    busname = name.MakeBytesSize(40),
                    address = address,
                    regstart = regstart,
                    count = (byte) writebytes.Length,
                    data = writebytes.MakeSize(128)
                };

                generatePacket(MAVLINK_MSG_ID.DEVICE_OP_WRITE, write);
            }
            else
            {
                var read = new MAVLink.mavlink_device_op_read_t()
                {
                    target_system = (byte) sysid,
                    target_component = (byte) compid,
                    request_id = request_id++,
                    bustype = (byte) bustype,
                    bus = bus,
                    busname = name.MakeBytesSize(40),
                    address = address,
                    regstart = regstart,
                    count = count
                };
                generatePacket(MAVLINK_MSG_ID.DEVICE_OP_READ, read);
            }

            log.InfoFormat("bustype {0} name {1} bus {2} address {3}", bustype, name, bus, address);
            var start = DateTime.Now;
            while (!responce && start.AddSeconds(1) > DateTime.Now)
            {
                Thread.Sleep(1);
            }

            UnSubscribeToPacketType(sub);
            UnSubscribeToPacketType(sub2);

            outputbuffer = buffer;
            return result;
        }

        private void SetupMavConnect(MAVLinkMessage message, mavlink_heartbeat_t hb)
        {
            mavlinkversion = hb.mavlink_version;
            MAVlist[message.sysid, message.compid].aptype = (MAV_TYPE) hb.type;
            MAVlist[message.sysid, message.compid].apname = (MAV_AUTOPILOT) hb.autopilot;

            setAPType(message.sysid, message.compid);

            MAVlist[message.sysid, message.compid].sysid = message.sysid;
            MAVlist[message.sysid, message.compid].compid = message.compid;
            MAVlist[message.sysid, message.compid].recvpacketcount = message.seq;
            log.InfoFormat("ID sys {0} comp {1} ver{2} type {3} name {4}", message.sysid, message.compid,
                mavlinkversion,
                MAVlist[message.sysid, message.compid].aptype.ToString(), MAVlist[message.sysid, message.compid].apname.ToString());
        }

        private void SetupMavConnect(MAVLinkMessage message, mavlink_high_latency2_t hl)
        {
            mavlinkversion = 0;
            MAVlist[message.sysid, message.compid].aptype = (MAV_TYPE)hl.type;
            MAVlist[message.sysid, message.compid].apname = (MAV_AUTOPILOT)hl.autopilot;
            MAVlist[message.sysid, message.compid].isHighLatency = true;

            setAPType(message.sysid, message.compid);

            MAVlist[message.sysid, message.compid].sysid = message.sysid;
            MAVlist[message.sysid, message.compid].compid = message.compid;
            MAVlist[message.sysid, message.compid].recvpacketcount = message.seq;
            log.InfoFormat("ID HL sys {0} comp {1} ver{2} type {3} name {4}", message.sysid, message.compid,
                mavlinkversion,
                MAVlist[message.sysid, message.compid].aptype.ToString(), MAVlist[message.sysid, message.compid].apname.ToString());
        }

        public MAVLinkMessage getHeartBeat()
        {
            return getHeartBeatAsync().AwaitSync();
        }

        public async Task<MAVLinkMessage> getHeartBeatAsync()
        {
            giveComport = true;

            sendPacket(new MAVLink.mavlink_heartbeat_t()
            {
                type = (byte) MAVLink.MAV_TYPE.GCS,
                autopilot = (byte) MAVLink.MAV_AUTOPILOT.INVALID,
                mavlink_version = 3 // MAVLink.MAVLINK_VERSION
            }, gcssysid, 190);

            DateTime start = DateTime.Now;
            int readcount = 0;
            while (true)
            {
                MAVLinkMessage buffer = await readPacketAsync().ConfigureAwait(false);
                readcount++;
                if (buffer.Length > 5)
                {
                    //log.Info("getHB packet received: " + buffer.Length + " btr " + BaseStream.BytesToRead + " type " + buffer.msgid );
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.HEARTBEAT)
                    {
                        mavlink_heartbeat_t hb = buffer.ToStructure<mavlink_heartbeat_t>();

                        if (hb.type != (byte) MAV_TYPE.GCS)
                        {
                            SetupMavConnect(buffer, hb);

                            giveComport = false;
                            return buffer;
                        }
                    }
                    if (buffer.msgid == (byte)MAVLINK_MSG_ID.HIGH_LATENCY2)
                    {
                        mavlink_high_latency2_t hl = buffer.ToStructure<mavlink_high_latency2_t>();

                        if (hl.type != (byte)MAV_TYPE.GCS)
                        {
                            SetupMavConnect(buffer, hl);

                            giveComport = false;
                            return buffer;
                        }
                    }
                }

                if (DateTime.Now > start.AddMilliseconds(2200) || readcount > 200) // was 1200 , now 2.2 sec
                {
                    giveComport = false;
                    return MAVLinkMessage.Invalid;
                }
            }
        }

        /// <summary>
        /// Send a mavlink packet
        /// </summary>
        /// <param name="indata">data</param>
        /// <param name="sysid">target sysid</param>
        /// <param name="compid">target compid</param>
        public void sendPacket(object indata, int sysid, int compid)
        {
            bool validPacket = false;

            if (BaseStream == null || !BaseStream.IsOpen)
                return;

            foreach (var ty in MAVLINK_MESSAGE_INFOS)
            {
                if (ty.type == indata.GetType())
                {
                    validPacket = true;
                    generatePacket((int) ty.msgid, indata, sysid, compid);
                    return;
                }
            }

            if (!validPacket)
            {
                log.Info("Mavlink : NOT VALID PACKET sendPacket() " + indata.GetType().ToString());
            }
        }

        private void generatePacket(MAVLINK_MSG_ID messageType, object indata)
        {
            generatePacket((int) messageType, indata);
        }

        private void generatePacket(int messageType, object indata)
        {
            //uses currently targeted mavs sysid and compid
            generatePacket(messageType, indata, MAV.sysid, MAV.compid);
        }

        public void generatePacket(MAVLINK_MSG_ID messageType, object indata, int sysid, int compid,
            bool forcemavlink2 = false,
            bool forcesigning = false)
        {
            generatePacket((int) messageType, indata, sysid, compid, forcemavlink2, forcesigning);
        }

        /// <summary>
        /// Generate a Mavlink Packet and write to serial
        /// </summary>
        /// <param name="messageType">type number = MAVLINK_MSG_ID</param>
        /// <param name="indata">struct of data</param>
        public void generatePacket(int messageType, object indata, int sysid, int compid, bool forcemavlink2 = false,
            bool forcesigning = false)
        {
            if (BaseStream == null || !BaseStream.IsOpen)
            {
                return;
            }

            if (ReadOnly)
            {
                // allow these messages
                if (messageType == (byte) MAVLINK_MSG_ID.MISSION_REQUEST_LIST ||
                    messageType == (byte) MAVLINK_MSG_ID.MISSION_REQUEST_PARTIAL_LIST ||
                    messageType == (byte) MAVLINK_MSG_ID.MISSION_REQUEST ||
                    messageType == (byte) MAVLINK_MSG_ID.MISSION_REQUEST_INT ||
                    messageType == (byte) MAVLINK_MSG_ID.PARAM_REQUEST_LIST ||
                    messageType == (byte) MAVLINK_MSG_ID.PARAM_REQUEST_READ ||
                    messageType == (byte) MAVLINK_MSG_ID.RALLY_FETCH_POINT ||
                    messageType == (byte) MAVLINK_MSG_ID.FENCE_FETCH_POINT
                )
                {
                }
                else
                {
                    return;
                }
            }

            byte[] packet = new byte[0];
            lock (writelock)
            {
                byte[] data = MavlinkUtil.StructureToByteArray(indata);
                
                int i = 0;

                // are we mavlink2 enabled for this sysid/compid
                if (!MAVlist[sysid, compid].mavlinkv2 && messageType < 256 && !forcemavlink2)
                {
                    var info = MAVLINK_MESSAGE_INFOS.SingleOrDefault(p => p.msgid == messageType);
                    if (data.Length != info.minlength)
                    {
                        Array.Resize(ref data, (int) info.minlength);
                    }

                    //Console.WriteLine(DateTime.Now + " PC Doing req "+ messageType + " " + this.BytesToRead);
                    packet = new byte[data.Length + 6 + 2];

                    packet[0] = MAVLINK_STX_MAVLINK1;
                    packet[1] = (byte) data.Length;
                    packet[2] = (byte) packetcount;

                    packetcount++;

                    packet[3] = gcssysid;
                    packet[4] = (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
                    packet[5] = (byte) messageType;

                    i = 6;
                    foreach (byte b in data)
                    {
                        packet[i] = b;
                        i++;
                    }

                    ushort checksum = MavlinkCRC.crc_calculate(packet, packet[1] + 6);

                    checksum = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_INFOS.GetMessageInfo((uint) messageType).crc,
                        checksum);

                    byte ck_a = (byte) (checksum & 0xFF); ///< High byte
                    byte ck_b = (byte) (checksum >> 8); ///< Low byte

                    packet[i] = ck_a;
                    i += 1;
                    packet[i] = ck_b;
                    i += 1;
                }
                else
                {
                    // trim packet for mavlink2
                    MavlinkUtil.trim_payload(ref data);

                    // space for signing if needed
                    if (MAVlist[sysid, compid].signing || forcesigning)
                        packet = new byte[data.Length + MAVLINK_NUM_HEADER_BYTES + MAVLINK_NUM_CHECKSUM_BYTES +
                                          MAVLINK_SIGNATURE_BLOCK_LEN];
                    else
                        packet = new byte[data.Length + MAVLINK_NUM_HEADER_BYTES + MAVLINK_NUM_CHECKSUM_BYTES];

                    packet[0] = MAVLINK_STX;
                    packet[1] = (byte) data.Length;
                    packet[2] = 0; // incompat
                    if (MAVlist[sysid, compid].signing || forcesigning) // current mav
                        packet[2] |= MAVLINK_IFLAG_SIGNED;
                    packet[3] = 0; // compat
                    packet[4] = (byte) packetcount;

                    packetcount++;

                    packet[5] = gcssysid;
                    packet[6] = (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
                    packet[7] = (byte) (messageType & 0xff);
                    packet[8] = (byte) ((messageType >> 8) & 0xff);
                    packet[9] = (byte) ((messageType >> 16) & 0xff);

                    i = 10;
                    foreach (byte b in data)
                    {
                        packet[i] = b;
                        i++;
                    }

                    ushort checksum = MavlinkCRC.crc_calculate(packet, packet[1] + MAVLINK_NUM_HEADER_BYTES);

                    checksum = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_INFOS.GetMessageInfo((uint) messageType).crc,
                        checksum);

                    byte ck_a = (byte) (checksum & 0xFF); ///< High byte
                    byte ck_b = (byte) (checksum >> 8); ///< Low byte

                    packet[i] = ck_a;
                    i += 1;
                    packet[i] = ck_b;
                    i += 1;

                    if (MAVlist[sysid, compid].signing || forcesigning)
                    {
                        //https://docs.google.com/document/d/1ETle6qQRcaNWAmpG2wz0oOpFKSF_bcTmYMQvtTGI8ns/edit

                        /*
                        8 bits of link ID
                        48 bits of timestamp
                        48 bits of signature
                        */

                        // signature = sha256_48(secret_key + header + payload + CRC + link-ID + timestamp)

                        var timestamp = (UInt64) ((DateTime.UtcNow - new DateTime(2015, 1, 1)).TotalMilliseconds * 100);

                        if (timestamp == MAVlist[sysid, compid].timestamp)
                            timestamp++;

                        MAVlist[sysid, compid].timestamp = timestamp;

                        var timebytes = BitConverter.GetBytes(timestamp);

                        var sig = new byte[7]; // 13 includes the outgoing hash
                        sig[0] = MAVlist[sysid, compid].sendlinkid;
                        Array.Copy(timebytes, 0, sig, 1, 6); // timestamp

                        //Console.WriteLine("gen linkid {0}, time {1} {2} {3} {4} {5} {6} {7}", sig[0], sig[1], sig[2], sig[3], sig[4], sig[5], sig[6], timestamp);

                        var signingKey = MAVlist[sysid, compid].signingKey;

                        if (signingKey == null || signingKey.Length != 32)
                        {
                            signingKey = new byte[32];
                        }

                        using (SHA256CryptoServiceProvider signit = new SHA256CryptoServiceProvider())
                        {
                            signit.TransformBlock(signingKey, 0, signingKey.Length, null, 0);
                            signit.TransformBlock(packet, 0, i, null, 0);
                            signit.TransformFinalBlock(sig, 0, sig.Length);
                            var ctx = signit.Hash;
                            // trim to 48
                            Array.Resize(ref ctx, 6);

                            foreach (byte b in sig)
                            {
                                packet[i] = b;
                                i++;
                            }

                            foreach (byte b in ctx)
                            {
                                packet[i] = b;
                                i++;
                            }
                        }
                    }
                }

                if (BaseStream.IsOpen)
                {
                    BaseStream.Write(packet, 0, i);
                    _bytesSentSubj.OnNext(i);
                }

                try
                {
                    SaveToTlog(new Span<byte>(packet, 0, i));
                }
                catch
                {
                }
            }
            try
            {
                // callback outside of lock
                if(packet.Length > 0)
                    _OnPacketSent?.Invoke(this, new MAVLinkMessage(packet));
            }
            catch
            {
            }
        }

        public void SaveToTlog(Span<byte> packet)
        {
            try
            {
                if (logfile != null && logfile.CanWrite && !logreadmode)
                {
                    lock (logfile)
                    {
                        byte[] datearray =
                            BitConverter.GetBytes(
                                (UInt64) ((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000));
                        Array.Reverse(datearray);
                        logfile.Write(datearray, 0, datearray.Length);
                        logfile.Write(packet.ToArray(), 0, packet.Length);
                    }
                }
            }
            catch
            {
            }
        }

        public bool Write(byte[] packet)
        {
            lock (writelock)
            {
                if (BaseStream.IsOpen)
                {
                    BaseStream.Write(packet, 0, packet.Length);
                    _bytesSentSubj.OnNext(packet.Length);
                }

                try
                {
                    SaveToTlog(new Span<byte>(packet, 0, packet.Length));
                }
                catch
                {
                }
            }
            try
            {
                // callback outside of lock
                _OnPacketSent?.Invoke(this, new MAVLinkMessage(packet));
            }
            catch
            {
            }
            return true;
        }

        public bool Write(string line)
        {
            lock (writelock)
            {
                BaseStream.Write(line);
            }

            _bytesSentSubj.OnNext(line.Length);
            return true;
        }

        public bool setupSigning(byte sysid, byte compid, string userseed, byte[] key = null)
        {
            byte[] shauser;
            bool clearkey = false;

            if (key == null)
            {
                clearkey = String.IsNullOrEmpty(userseed);

                // sha the user input string
                using (SHA256CryptoServiceProvider signit = new SHA256CryptoServiceProvider())
                {
                    shauser = signit.ComputeHash(Encoding.UTF8.GetBytes(userseed));
                    Array.Resize(ref shauser, 32);
                }
            }
            else
            {
                shauser = key;
                Array.Resize(ref shauser, 32);
            }

            mavlink_setup_signing_t sign = new mavlink_setup_signing_t();
            if (!clearkey)
            {
                sign.initial_timestamp =
                    (UInt64) ((DateTime.UtcNow - new DateTime(2015, 1, 1)).TotalMilliseconds * 100);
                sign.secret_key = shauser;
            }
            else
            {
                sign.initial_timestamp = 0;
                sign.secret_key = new byte[32];
            }

            sign.target_component = (byte) compid;
            sign.target_system = (byte) sysid;

            generatePacket((int) MAVLINK_MSG_ID.SETUP_SIGNING, sign, sysid, compid);

            generatePacket((int) MAVLINK_MSG_ID.SETUP_SIGNING, sign, sysid, compid);

            if (!clearkey)
            {
                // we will auto adapt to this key - so no need to assign it
                //MAVlist[sysid, compid].signingKey = shauser;
            }
            else
            {
                // clear the key
                MAVlist[sysid, compid].signingKey = null;
                return disableSigning(sysid, compid);
            }

            return enableSigning(sysid, compid);
        }

        public bool enableSigning(int sysid, int compid)
        {
            MAVlist[sysid, compid].signing = true;
            MAVlist[sysid, compid].mavlinkv2 = true;

            return MAVlist[sysid, compid].signing;
        }

        public bool disableSigning(int sysid, int compid)
        {
            MAVlist[sysid, compid].signing = false;
            MAVlist[sysid, compid].mavlinkv2 = false;

            return MAVlist[sysid, compid].signing;
        }

        [Obsolete]
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

        [Obsolete]
        public bool setParam(string paramname, double value, bool force = false)
        {
            return setParam((byte) sysidcurrent, (byte) compidcurrent, paramname, value, force);
        }

        public bool setParam(byte sysid, byte compid, string paramname, double value, bool force = false)
        {
            return setParamAsync(sysid, compid, paramname, value, force).AwaitSync();
        }

        /// <summary>
        /// Set parameter on apm
        /// </summary>
        /// <param name="paramname">name as a string</param>
        /// <param name="value"></param>
        public async Task<bool> setParamAsync(byte sysid, byte compid, string paramname, double value,
            bool force = false)
        {
            if (!MAVlist[sysid, compid].param.ContainsKey(paramname))
            {
                log.Warn("Trying to set Param that doesnt exist " + paramname + "=" + value);
                return false;
            }

            MAVLinkParam param = MAVlist[sysid, compid].param[paramname];
            if (param.Value == value && !force)
            {
                log.Warn("setParam " + paramname + " not modified as same");
                return true;
            }

            giveComport = true;

            // param type is set here, however it is always sent over the air as a float 100int = 100f.
            var req = new mavlink_param_set_t
            {
                target_system = sysid,
                target_component = compid,
                param_type = (byte) MAVlist[sysid, compid].param_types[paramname]
            };

            char[] temp = paramname.ToCharArray();

            Array.Resize(ref temp, 16);
            req.param_id = temp.ToByteArray();
            if ((MAVlist[sysid, compid].cs.capabilities & (uint) MAV_PROTOCOL_CAPABILITY.PARAM_FLOAT) > 0 ||
                MAVlist[sysid, compid].apname == MAV_AUTOPILOT.ARDUPILOTMEGA)
            {
                req.param_value = new MAVLinkParam(paramname, value, (MAV_PARAM_TYPE.REAL32)).float_value;
            }
            else
            {
                req.param_value = new MAVLinkParam(paramname, value,
                    (MAV_PARAM_TYPE) MAVlist[sysid, compid].param_types[paramname]).float_value;
            }

            int currentparamcount = MAVlist[sysid, compid].param.Count;

            bool complete = false;

            var sub1 = SubscribeToPacketType(MAVLINK_MSG_ID.PARAM_VALUE, buffer =>
            {
                if (buffer.msgid == (byte) MAVLINK_MSG_ID.PARAM_VALUE && buffer.sysid == req.target_system &&
                    buffer.compid == req.target_component)
                {
                    mavlink_param_value_t par = buffer.ToStructure<mavlink_param_value_t>();

                    string st = Encoding.UTF8.GetString(par.param_id);

                    int pos = st.IndexOf('\0');

                    if (pos != -1)
                    {
                        st = st.Substring(0, pos);
                    }

                    if (st != paramname)
                    {
                        log.InfoFormat("MAVLINK bad param response - {0} vs {1}", paramname, st);
                        return true;
                    }

                    if (MAVlist[sysid, compid].apname == MAV_AUTOPILOT.ARDUPILOTMEGA)
                    {
                        var offset = Marshal.OffsetOf(typeof(mavlink_param_value_t), "param_value");
                        MAVlist[sysid, compid].param[st] = new MAVLinkParam(st, BitConverter.GetBytes(par.param_value),
                            MAV_PARAM_TYPE.REAL32, (MAV_PARAM_TYPE) par.param_type, param.default_value);
                    }
                    else
                    {
                        var offset = Marshal.OffsetOf(typeof(mavlink_param_value_t), "param_value");
                        MAVlist[sysid, compid].param[st] = new MAVLinkParam(st, BitConverter.GetBytes(par.param_value),
                            (MAV_PARAM_TYPE) par.param_type, (MAV_PARAM_TYPE) par.param_type);
                    }

                    log.Info("setParam gotback " + st + " : " + MAVlist[sysid, compid].param[st]);

                    lastparamset = DateTime.UtcNow;

                    // check if enabeling this param has added subparams, queue on gui thread
                    if (currentparamcount < par.param_count)
                    {
                        /*
                        MainV2.instance.BeginInvoke((Action) delegate
                        {
                            Loading.ShowLoading(String.Format(Strings.ParamRefreshRequired, currentparamcount,
                                par.param_count));
                        });
                        */
                    }

                    complete = true;
                }

                return true;
            }, sysid, compid);

            try
            {
                generatePacket((byte) MAVLINK_MSG_ID.PARAM_SET, req, sysid, compid);

                log.InfoFormat("setParam '{0}' = '{1}' sysid {2} compid {3}", paramname, value, sysid,
                    compid);

                DateTime start = DateTime.Now;
                int retrys = 3;
                while (true)
                {
                    if (complete)
                        return true;

                    if (!(start.AddMilliseconds(700) > DateTime.Now))
                    {
                        if (retrys > 0)
                        {
                            log.Info("setParam Retry " + retrys);
                            generatePacket((byte) MAVLINK_MSG_ID.PARAM_SET, req, sysid, compid);
                            start = DateTime.Now;
                            retrys--;
                            continue;
                        }

                        throw new TimeoutException("Timeout on read - setParam " + paramname);
                    }

                    await readPacketAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                giveComport = false;
                UnSubscribeToPacketType(sub1);
            }
        }

        /// <summary>
        /// With GUI
        /// </summary>
        public void getParamList()
        {
            log.InfoFormat("getParamList {0} {1}", sysidcurrent, compidcurrent);

            frmProgressReporter = CreateIProgressReporterDialogue(Strings.GettingParams + " " + sysidcurrent);

            frmProgressReporter.DoWork += FrmProgressReporterGetParams;
            frmProgressReporter.UpdateProgressAndStatus(-1, Strings.GettingParamsD);

            frmProgressReporter.RunBackgroundOperationAsync();

            frmProgressReporter.Dispose();

            _ParamListChanged?.Invoke(this, null);
        }

        private void FrmProgressReporterGetParams(IProgressReporterDialogue sender)
        {
            getParamListMavftp(MAV.sysid, MAV.compid);
        }

        /// <summary>
        /// Get param list via mavftp with old style fallback
        /// </summary>
        /// <param name="sysid"></param>
        /// <param name="compid"></param>
        /// <returns>MAVLinkParamList</returns>
        public MAVLinkParamList getParamListMavftp(byte sysid, byte compid)
        {
            return getParamListMavftpAsync(sysid, compid).AwaitSync();
        }

        public async Task<MAVLinkParamList> getParamListMavftpAsync(byte sysid, byte compid)
        {
            var sub2 = SubscribeToPacketType(MAVLINK_MSG_ID.STATUSTEXT, buffer =>
            {
                if (buffer.msgid == (byte) MAVLINK_MSG_ID.STATUSTEXT)
                {
                    var msg = buffer.ToStructure<mavlink_statustext_t>();

                    string logdata = Encoding.UTF8.GetString(msg.text);

                    int ind = logdata.IndexOf('\0');
                    if (ind != -1)
                        logdata = logdata.Substring(0, ind);

                    if (logdata.ToLower().Contains("copter") || logdata.ToLower().Contains("rover") ||
                        logdata.ToLower().Contains("plane"))
                    {
                        MAVlist[sysid, compid].VersionString = logdata;
                        // Version string may allow to better identify type
                        setAPType(sysid, compid);
                    }
                    else if (logdata.ToLower().Contains("nuttx") || logdata.ToLower().Contains("chibios"))
                    {
                        MAVlist[sysid, compid].SoftwareVersions = logdata;
                    }
                    else if (logdata.ToLower().Contains("px4v2") ||
                             Regex.IsMatch(logdata, @"\s[0-9A-F]+\s[0-9A-F]+\s[0-9A-F]+"))
                    {
                        MAVlist[sysid, compid].SerialString = logdata;
                    }
                    else if (logdata.ToLower().Contains("frame"))
                    {
                        MAVlist[sysid, compid].FrameString = logdata;
                    }
                    else
                    {
                    }
                }

                return true;
            }, (byte)sysid, (byte)compid);

            // get the banner
            doCommand((byte) sysid, (byte) compid, MAV_CMD.DO_SEND_BANNER, 0, 0, 0, 0, 0, 0, 0,
                false);

            try
            {
                if (((MAVlist[sysid,compid].cs.capabilities & (int) MAV_PROTOCOL_CAPABILITY.FTP) > 0) && Settings.Instance.GetBoolean("UseMavFtpParams", true))
                {
                    if (frmProgressReporter != null)
                        frmProgressReporter.UpdateProgressAndStatus(-1, $"Checking for Param MAVFTP {sysid}-{compid}");
                    var cancel = new CancellationTokenSource();
                    var paramfileTask = Task.Run<MemoryStream>(() =>
                    {
                        var ftp = new MAVFtp(this, sysid, compid);
                        ftp.Progress += (s,i) =>
                        {
                            if (frmProgressReporter != null)
                                frmProgressReporter.UpdateProgressAndStatus(i,
                                    $"Getting Param MAVFTP {sysid}-{compid} : {s}");
                        };
                        return ftp.GetFile(
                            // always try to get defualts, AP will send orginal format if not avalable
                            "@PARAM/param.pck?withdefaults=1", cancel, true, 110);
                    });
                    while (!paramfileTask.IsCompleted)
                    {
                        if (frmProgressReporter != null)
                            if (frmProgressReporter.doWorkArgs.CancelRequested)
                            {
                                cancel.Cancel();
                                frmProgressReporter.doWorkArgs.CancelAcknowledged = true;
                                return MAVlist[sysid, compid].param;
                            }

                        await Task.Delay(10).ConfigureAwait(false);
                    }

                    if (frmProgressReporter != null)
                        if (frmProgressReporter.doWorkArgs.CancelRequested)
                            return MAVlist[sysid, compid].param;

                    var paramfile = await paramfileTask;
                    if (paramfile != null && paramfile.Length > 0)
                    {
                        var mavlist = parampck.unpack(paramfile.ToArray());
                        if (mavlist != null)
                        {
                            MAVlist[sysid, compid].param.Clear();
                            MAVlist[sysid, compid].param.TotalReported =
                                mavlist.Count;
                            MAVlist[sysid, compid].param.AddRange(mavlist);
                            var gen = new MavlinkParse();
                            mavlist.ForEach(a =>
                            {
                                MAVlist[sysid, compid].param_types[a.Name] =
                                    a.Type;
                                SaveToTlog(gen.GenerateMAVLinkPacket10(
                                    MAVLINK_MSG_ID.PARAM_VALUE,
                                    new mavlink_param_value_t((float) a.Value, (ushort) mavlist.Count,
                                        0,
                                        a.Name.MakeBytesSize(16), (byte) a.Type), sysid, compid));
                            });

                            UnSubscribeToPacketType(sub2);
                            return MAVlist[sysid, compid].param;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            UnSubscribeToPacketType(sub2);
            return await getParamListAsync(sysid, compid);
        }

        /// <summary>
        /// Without GUI
        /// </summary>
        /// <param name="sysid"></param>
        /// <param name="compid"></param>
        /// <returns></returns>
        public MAVLinkParamList getParamList(byte sysid, byte compid)
        {
            return getParamListAsync(sysid, compid).AwaitSync();
        }

        /// <summary>
        /// Get param list from apm
        /// </summary>
        /// <returns></returns>
        public async Task<MAVLinkParamList> getParamListAsync(byte sysid, byte compid)
        {
            List<int> indexsreceived = new List<int>();

            // create new list so if canceled we use the old list
            MAVLinkParamList newparamlist = new MAVLinkParamList();

            int param_total = 1;

            mavlink_param_request_list_t req = new mavlink_param_request_list_t
            {
                target_system = sysid,
                target_component = compid
            };

            DateTime start = DateTime.Now;

            DateTime lastmessage = DateTime.MinValue;

            //hires.Stopwatch stopwatch = new hires.Stopwatch();
            int retry = 0;
            bool onebyone = false;
            bool missing_params = false;
            DateTime lastonebyone = DateTime.MinValue;

            var sub2 = SubscribeToPacketType(MAVLINK_MSG_ID.STATUSTEXT, buffer =>
            {
                if (buffer.msgid == (byte) MAVLINK_MSG_ID.STATUSTEXT)
                {
                    var msg = buffer.ToStructure<mavlink_statustext_t>();

                    string logdata = Encoding.UTF8.GetString(msg.text);

                    int ind = logdata.IndexOf('\0');
                    if (ind != -1)
                        logdata = logdata.Substring(0, ind);

                    if (logdata.ToLower().Contains("copter") || logdata.ToLower().Contains("rover") ||
                        logdata.ToLower().Contains("plane"))
                    {
                        MAVlist[sysid, compid].VersionString = logdata;
                    }
                    else if (logdata.ToLower().Contains("nuttx") || logdata.ToLower().Contains("chibios"))
                    {
                        MAVlist[sysid, compid].SoftwareVersions = logdata;
                    }
                    else if (logdata.ToLower().Contains("px4v2") ||
                             Regex.IsMatch(logdata, @"\s[0-9A-F]+\s[0-9A-F]+\s[0-9A-F]+"))
                    {
                        MAVlist[sysid, compid].SerialString = logdata;
                    }
                    else if (logdata.ToLower().Contains("frame"))
                    {
                        MAVlist[sysid, compid].FrameString = logdata;
                    }
                    else
                    {
                    }
                }

                return true;
            }, (byte)sysid, (byte)compid);

            int seenvalid = 0;

            var sub1 = SubscribeToPacketType(MAVLINK_MSG_ID.PARAM_VALUE, buffer =>
            {
                if (buffer.msgid == (byte) MAVLINK_MSG_ID.PARAM_VALUE && buffer.sysid == req.target_system &&
                    buffer.compid == req.target_component)
                {
                    // if we are doing one by one dont update start time
                    if (!onebyone)
                        start = DateTime.Now;

                    mavlink_param_value_t par = buffer.ToStructure<mavlink_param_value_t>();

                    // set new target
                    param_total = (par.param_count);
                    newparamlist.TotalReported = param_total;

                    if (param_total == 0)
                        return true;

                    string paramID = Encoding.UTF8.GetString(par.param_id);

                    int pos = paramID.IndexOf('\0');
                    if (pos != -1)
                    {
                        paramID = paramID.Substring(0, pos);
                    }

                    // check if we already have it
                    if (indexsreceived.Contains(par.param_index))
                    {
                        log.Info("Already got " + (par.param_index) + " '" + paramID + "' " +
                                 (indexsreceived.Count * 100) / param_total);
                        if (frmProgressReporter != null)
                            this.frmProgressReporter.UpdateProgressAndStatus(
                                (indexsreceived.Count * 100) / param_total, "Already Got param " + paramID);
                        return true;
                    }

                    seenvalid++;
                    //Console.WriteLine(DateTime.Now.Millisecond + " gp2 ");

                    log.Info(DateTime.Now.Millisecond + " got param " + (par.param_index) + " of " +
                             (par.param_count) + " name: " + paramID + " " + par.param_value);

                    //Console.WriteLine(DateTime.Now.Millisecond + " gp2a ");

                    // item uses float based param system
                    if ((MAVlist[sysid, compid].cs.capabilities & (uint) MAV_PROTOCOL_CAPABILITY.PARAM_FLOAT) > 0 ||
                        MAVlist[sysid, compid].apname == MAV_AUTOPILOT.ARDUPILOTMEGA)
                    {
                        var offset = Marshal.OffsetOf(typeof(mavlink_param_value_t), "param_value");
                        newparamlist[paramID] = new MAVLinkParam(paramID, BitConverter.GetBytes(par.param_value),
                            MAV_PARAM_TYPE.REAL32, (MAV_PARAM_TYPE) par.param_type);
                    }
                    else
                    {
                        var offset = Marshal.OffsetOf(typeof(mavlink_param_value_t), "param_value");
                        newparamlist[paramID] = new MAVLinkParam(paramID, BitConverter.GetBytes(par.param_value),
                            (MAV_PARAM_TYPE) par.param_type, (MAV_PARAM_TYPE) par.param_type);
                    }

                    //Console.WriteLine(DateTime.Now.Millisecond + " gp2b ");

                    // exclude index of 65535
                    if (par.param_index != 65535)
                        indexsreceived.Add(par.param_index);

                    lock(MAVlist[sysid, compid].param_types)
                        MAVlist[sysid, compid].param_types[paramID] = (MAV_PARAM_TYPE) par.param_type;

                    //Console.WriteLine(DateTime.Now.Millisecond + " gp3 ");

                    if (frmProgressReporter != null)
                        this.frmProgressReporter.UpdateProgressAndStatus((indexsreceived.Count * 100) / param_total,
                            Strings.Gotparam + paramID);

                    // we hit the last param, jump striaght to retry if params missing
                    if (par.param_index == (param_total - 1))
                        missing_params |= indexsreceived.Count < param_total;
                }

                return true;
            }, (byte)sysid, (byte)compid);


            generatePacket((byte)MAVLINK_MSG_ID.PARAM_REQUEST_LIST, req);

            short tenbytenindex = 0;

            do
            {
                if (frmProgressReporter != null && frmProgressReporter.doWorkArgs.CancelRequested)
                {
                    frmProgressReporter.doWorkArgs.CancelAcknowledged = true;

                    UnSubscribeToPacketType(sub1);
                    UnSubscribeToPacketType(sub2);
                    frmProgressReporter.doWorkArgs.ErrorMessage = "User Canceled";
                    return MAVlist[sysid, compid].param;
                }

                // 4 seconds between valid packets or got to last param and have some missing
                if ((missing_params || !(start.AddMilliseconds(4000) > DateTime.Now)) && !logreadmode)
                {
                    // if we have less than 75% of the total use full list pull
                    if (!onebyone && (retry < 2) && ((param_total <= 1) || (indexsreceived.Count < ((param_total / 4) * 3))))
                    {
                        if (param_total > 1)
                        {
                            // only increment retry counter if we have a valid total
                            // otherwise keep requesting full list
                            retry++;
                        }
                        log.InfoFormat("Get Param whole list retry {0} got {1} 75%={2} count {3}", retry,
                            indexsreceived.Count, ((param_total / 4) * 3), param_total);
                        generatePacket((byte) MAVLINK_MSG_ID.PARAM_REQUEST_LIST, req);
                        start = DateTime.Now;
                        missing_params = false;
                        continue;
                    }

                    onebyone = true;

                    if (lastonebyone.AddMilliseconds(1000) < DateTime.Now)
                    {
                        int queued = 0;
                        short startindex = 0;
                        var backupseenvalid = seenvalid;
                        if (param_total > 1)
                            startindex = (short) (tenbytenindex % (param_total - 1));
                        
                        seenvalid = 0;

                        // seeing more than what we requested... skip this round - maybe full param list is still inflight
                        if(backupseenvalid > 10)
                            continue;

                        log.Info("Get param 10 by 10 - got " + indexsreceived.Count + " of " + param_total +
                                 " request from startindex " + startindex + " seen since last requests " + backupseenvalid);

                        // try getting individual params
                        bool found_missing_param = false;
                        for (short i = startindex; i <= (param_total - 1); i++)
                        {
                            if (!indexsreceived.Contains(i))
                            {
                                found_missing_param = true;

                                if (frmProgressReporter != null && frmProgressReporter.doWorkArgs.CancelRequested)
                                {
                                    frmProgressReporter.doWorkArgs.CancelAcknowledged = true;
                                    UnSubscribeToPacketType(sub1);
                                    UnSubscribeToPacketType(sub2);
                                    frmProgressReporter.doWorkArgs.ErrorMessage = "User Canceled";
                                    return MAVlist[sysid, compid].param;
                                }

                                // prevent dropping out of this get params loop
                                try
                                {
                                    queued++;
                                    tenbytenindex = (short)(i + 1);

                                    mavlink_param_request_read_t req2 = new mavlink_param_request_read_t
                                    {
                                        target_system = sysid,
                                        target_component = compid,
                                        param_index = i,
                                        param_id = new byte[] {0}
                                    };

                                    Array.Resize(ref req2.param_id, 16);

                                    generatePacket((byte) MAVLINK_MSG_ID.PARAM_REQUEST_READ, req2);

                                    if (queued >= 10)
                                    {
                                        break;
                                    }
                                }
                                catch (Exception excp)
                                {
                                    log.Info("GetParam Failed index: " + i + " " + excp);
                                    UnSubscribeToPacketType(sub1);
                                    UnSubscribeToPacketType(sub2);
                                    throw excp;
                                }
                            }
                        }
                        if (!found_missing_param)
                        {
                            // no missing param after start index, start from the begining
                            tenbytenindex = 0;
                        }

                        lastonebyone = DateTime.Now;
                    }
                }

                await readPacketAsync();

                if (logreadmode && logplaybackfile.BaseStream.Position >= logplaybackfile.BaseStream.Length)
                {
                    break;
                }

                if (!logreadmode && !BaseStream.IsOpen)
                {
                    var exp = new Exception("Not Connected");
                    frmProgressReporter.doWorkArgs.ErrorMessage = exp.Message;
                    UnSubscribeToPacketType(sub1);
                    UnSubscribeToPacketType(sub2);
                    throw exp;
                }
            } while (indexsreceived.Count < param_total);

            if (indexsreceived.Count != param_total)
            {
                var exp = new Exception("Missing Params " + indexsreceived.Count + " vs " + param_total);
                frmProgressReporter.doWorkArgs.ErrorMessage = exp.Message;
                UnSubscribeToPacketType(sub1);
                UnSubscribeToPacketType(sub2);
                throw exp;
            }

            UnSubscribeToPacketType(sub1);
            UnSubscribeToPacketType(sub2);

            log.InfoFormat("list count {0}, rcvd total {1} indexseen count {2}", newparamlist.Count, param_total,
                indexsreceived.Count);

            MAVlist[sysid, compid].param.Clear();
            MAVlist[sysid, compid].param.TotalReported = param_total;
            MAVlist[sysid, compid].param.AddRange(newparamlist);
            return MAVlist[sysid, compid].param;
        }

        private int _parampoll = 0;

        public void getParamPoll()
        {
            // check if we have all
            if (MAV.param.TotalReceived >= MAV.param.TotalReported)
            {
                return;
            }

            // if we are connected as primary to a vehicle where we dont have all the params, poll for them
            short i = (short) (_parampoll % MAV.param.TotalReported);

            GetParam((byte) sysidcurrent, (byte) compidcurrent, "", i, false);

            _parampoll++;
        }

        [Obsolete]
        public float GetParam(string name)
        {
            return GetParam(name, -1);
        }

        [Obsolete]
        public float GetParam(short index)
        {
            return GetParam("", index);
        }

        [Obsolete]
        public float GetParam(string name = "", short index = -1, bool requireresponce = true)
        {
            return GetParam(MAV.sysid, MAV.compid, name, index, requireresponce);
        }

        public float GetParam(byte sysid, byte compid, string name = "", short index = -1, bool requireresponce = true)
        {
            return GetParamAsync(sysid, compid, name, index, requireresponce).AwaitSync();
        }

        /// <summary>
        /// Get param by either index or name
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<float> GetParamAsync(byte sysid, byte compid, string name = "", short index = -1,
            bool requireresponce = true)
        {
            if (name == "" && index == -1)
                return 0;

            log.Info("GetParam name: '" + name + "' or index: " + index + " " + sysid + ":" + compid);

            MAVLinkMessage buffer;
            giveComport = true;
            mavlink_param_request_read_t req = new mavlink_param_request_read_t
            {
                target_system = sysid,
                target_component = compid,
                param_index = index,
                param_id = new byte[] {0}
            };
            if (index == -1)
            {
                req.param_id = name.MakeBytes();
            }

            Array.Resize(ref req.param_id, 16);

            generatePacket((byte) MAVLINK_MSG_ID.PARAM_REQUEST_READ, req, sysid, compid);

            if (!requireresponce)
            {
                giveComport = false;
                return 0f;
            }

            DateTime start = DateTime.Now;
            int retrys = 3;

            while (true)
            {
                if (!(start.AddMilliseconds(700) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("GetParam Retry " + retrys);
                        generatePacket((byte) MAVLINK_MSG_ID.PARAM_REQUEST_READ, req, sysid, compid);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }

                    giveComport = false;
                    throw new TimeoutException("Timeout on read - GetParam");
                }

                buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.PARAM_VALUE && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        mavlink_param_value_t par = buffer.ToStructure<mavlink_param_value_t>();

                        string st = Encoding.UTF8.GetString(par.param_id);

                        int pos = st.IndexOf('\0');

                        if (pos != -1)
                        {
                            st = st.Substring(0, pos);
                        }

                        // not the correct id
                        if (!(par.param_index == index || st == name))
                        {
                            log.ErrorFormat("Wrong Answer {0} - {1} - {2}    --- '{3}' vs '{4}'", par.param_index,
                                Encoding.UTF8.GetString(par.param_id), par.param_value,
                                Encoding.UTF8.GetString(req.param_id).TrimEnd(), st);
                            continue;
                        }

                        // update table
                        if (MAVlist[sysid, compid].apname == MAV_AUTOPILOT.ARDUPILOTMEGA)
                        {
                            var offset = Marshal.OffsetOf(typeof(mavlink_param_value_t), "param_value");
                            MAVlist[sysid, compid].param[st] = new MAVLinkParam(st,
                                BitConverter.GetBytes(par.param_value), MAV_PARAM_TYPE.REAL32,
                                (MAV_PARAM_TYPE) par.param_type);
                        }
                        else
                        {
                            var offset = Marshal.OffsetOf(typeof(mavlink_param_value_t), "param_value");
                            MAVlist[sysid, compid].param[st] = new MAVLinkParam(st,
                                BitConverter.GetBytes(par.param_value), (MAV_PARAM_TYPE) par.param_type,
                                (MAV_PARAM_TYPE) par.param_type);
                        }

                        lock(MAVlist[sysid, compid].param_types)
                            MAVlist[sysid, compid].param_types[st] = (MAV_PARAM_TYPE) par.param_type;

                        log.Info(DateTime.Now.Millisecond + " got param " + (par.param_index) + " of " +
                                 (par.param_count) + " name: " + st);

                        giveComport = false;

                        return par.param_value;
                    }
                }
            }
        }

        /// <summary>
        /// Stops all requested data packets.
        /// </summary>
        public void stopall(bool forget)
        {
            mavlink_request_data_stream_t req = new mavlink_request_data_stream_t
            {
                target_system = MAV.sysid,
                target_component = MAV.compid,
                req_message_rate = 10,
                start_stop = 0,
                req_stream_id = 0
            };

            // no error on bad
            try
            {
                generatePacket((byte) MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
                Thread.Sleep(20);
                generatePacket((byte) MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
                Thread.Sleep(20);
                generatePacket((byte) MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
                log.Info("Stopall Done");
            }
            catch
            {
            }
        }

        [Obsolete]
        public void setWPACK(MAVLink.MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            setWPACK(MAV.sysid, MAV.compid, type);
        }

        public void setWPACK(byte sysid, byte compid, MAVLink.MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            mavlink_mission_ack_t req = new mavlink_mission_ack_t
            {
                target_system = sysid,
                target_component = compid,
                type = 0,
                mission_type = (byte) type
            };

            generatePacket((byte) MAVLINK_MSG_ID.MISSION_ACK, req);
        }

        public bool setWPCurrent(byte sysid, byte compid, ushort index)
        {
            return setWPCurrentAsync(sysid, compid, index).AwaitSync();
        }

        public async Task<bool> setWPCurrentAsync(byte sysid, byte compid, ushort index)
        {
            giveComport = true;
            MAVLinkMessage buffer;

            mavlink_mission_set_current_t req = new mavlink_mission_set_current_t
            {
                target_system = sysid,
                target_component = compid,
                seq = index
            };

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
                    throw new TimeoutException("Timeout on read - setWPCurrent");
                }

                buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_CURRENT && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
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

        public bool doDFUBoot(byte sysid, byte compid)
        {
            doCommand(sysid, compid, MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN,
                42, 24, 71, 99, 0, 0, 0, false);

            return true;
        }

        /// <summary>
        /// Control high latency mode
        /// </summary>
        /// <param name="onoff">true to enable, false to disable</param>
        /// <returns></returns>
        public bool doHighLatency(bool onoff= false)
        {
            int param1 = onoff ? 1 : 0;

            var ans1 = doCommand((byte)sysidcurrent, (byte)compidcurrent, MAV_CMD.CONTROL_HIGH_LATENCY,
                    param1, 0, 0, 0, 0, 0, 0);

            return ans1;
        }

        /// <summary>
        /// reboot the vehicle
        /// </summary>
        /// <param name="bootloadermode">reboot into bootloader mode?</param>
        /// <param name="currentvehicle">use current sysid/compid or scan for it</param>
        /// <returns></returns>
        public bool doReboot(bool bootloadermode = false, bool currentvehicle = true)
        {
            int param1 = 1;
            if (bootloadermode)
            {
                param1 = 3;
            }

            // reboot the current selected mav
            if (currentvehicle)
            {
                var ans1 = doCommand((byte) sysidcurrent, (byte) compidcurrent, MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN,
                    param1, 0, 0, 0, 0, 0, 0);
                var ans2 = false;
                if (!ans1)
                {
                    ans2 = doCommand((byte)sysidcurrent, (byte)compidcurrent, MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN, 1, 0,
                    0, 0, 0, 0, 0);
                }
                // Successful reboot
                if (ans1 || ans2)
                {
                    // Not going into bootloader and on a serial connection
                    if (!bootloadermode && (BaseStream is SerialPort))
                    {
                        // Direct USB will disconnect after a reboot, wait and see if we should re-connect
                        Thread.Sleep(500);
                        if (!BaseStream.IsOpen)
                        {
                            Open(true);
                        }
                    }
                    return true;
                }

                giveComport = false;
                return false;
            }
            else
            {
                // scan for hb on unknown mav
                MAVLinkMessage buffer = getHeartBeat();

                if (buffer.Length > 5)
                {
                    mavlink_heartbeat_t hb = buffer.ToStructure<mavlink_heartbeat_t>();

                    mavlinkversion = hb.mavlink_version;
                    MAV.aptype = (MAV_TYPE) hb.type;
                    MAV.apname = (MAV_AUTOPILOT) hb.autopilot;
                    MAV.sysid = buffer.sysid;
                    MAV.compid = buffer.compid;
                }

                // reboot if we have seen hb
                if (MAV.sysid != 0 && MAV.compid != 0)
                {
                    doCommand((byte) sysidcurrent, (byte) compidcurrent, MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN, param1, 0,
                        0, 0, 0, 0, 0);
                    doCommand((byte) sysidcurrent, (byte) compidcurrent, MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN, 1, 0, 0, 0,
                        0, 0, 0);
                }
            }

            giveComport = false;
            return true;
        }

        [Obsolete]
        public bool doARM(bool armit, bool force = false)
        {
            return doARM(MAV.sysid, MAV.compid, armit, force);
        }

        public bool doARM(byte sysid, byte compid, bool armit, bool force = false)
        {
            return doARMAsync(sysid, compid, armit, force).AwaitSync();
        }

        public async Task<bool> doARMAsync(byte sysid, byte compid, bool armit, bool force = false)
        {
            const float magic_force_arm_value = 2989.0f;
            const float magic_force_disarm_value = 21196.0f;

            if (force)
            {
                if (armit)
                    return await doCommandAsync(sysid, compid, MAV_CMD.COMPONENT_ARM_DISARM, 1, magic_force_arm_value,
                        0, 0, 0, 0,
                        0).ConfigureAwait(false);
                else
                    return await doCommandAsync(sysid, compid, MAV_CMD.COMPONENT_ARM_DISARM, 0,
                        magic_force_disarm_value, 0, 0, 0,
                        0, 0).ConfigureAwait(false);
            }
            else
            {
                if (armit)
                    return await doCommandAsync(sysid, compid, MAV_CMD.COMPONENT_ARM_DISARM, 1, 0, 0, 0, 0, 0, 0)
                        .ConfigureAwait(false);
                else
                    return await doCommandAsync(sysid, compid, MAV_CMD.COMPONENT_ARM_DISARM, 0, 0, 0, 0, 0,
                        0, 0).ConfigureAwait(false);
            }
        }

        [Obsolete]
        public bool doAbortLand()
        {
            return doCommand(MAV_CMD.DO_GO_AROUND, 0, 0, 0, 0, 0, 0, 0);
        }

        [Obsolete]
        public bool doMotorTest(int motor, MOTOR_TEST_THROTTLE_TYPE thr_type, int throttle, int timeout,
            int motorcount = 0)
        {
            return doCommand(MAV_CMD.DO_MOTOR_TEST, (float) motor, (float) (byte) thr_type,
                (float) throttle, (float) timeout, (float) motorcount, 0, 0);
        }

        [Obsolete]
        public bool doCommand(MAV_CMD actionid, float p1, float p2, float p3, float p4, float p5, float p6, float p7,
            bool requireack = true)
        {
            return doCommand(MAV.sysid, MAV.compid, actionid, p1, p2, p3, p4, p5, p6, p7, requireack, null);
        }

        public bool doCommand(byte sysid, byte compid, MAV_CMD actionid, float p1, float p2, float p3,
            float p4,
            float p5, float p6, float p7, bool requireack = true, Action uicallback = null)
        {
            return doCommandAsync(sysid, compid, actionid, p1, p2, p3, p4, p5, p6, p7, requireack, uicallback)
                .AwaitSync();
        }

        public async Task<bool> doCommandAsync(byte sysid, byte compid, MAV_CMD actionid, float p1, float p2, float p3,
            float p4,
            float p5, float p6, float p7, bool requireack = true, Action uicallback = null)
        {
            if (BaseStream == null || BaseStream.IsOpen == false)
                return false;

            MAVLinkMessage buffer;

            mavlink_command_long_t req = new mavlink_command_long_t
            {
                target_system = sysid,
                target_component = compid,
                command = (ushort) actionid,
                confirmation = 0,
                param1 = p1,
                param2 = p2,
                param3 = p3,
                param4 = p4,
                param5 = p5,
                param6 = p6,
                param7 = p7
            };

            log.InfoFormat("doCommand cmd {0} {1} {2} {3} {4} {5} {6} {7}", actionid.ToString(), p1, p2, p3, p4, p5, p6,
                p7);

            if (requireack)
                giveComport = true;

            generatePacket((byte) MAVLINK_MSG_ID.COMMAND_LONG, req, sysid, compid);

            if (!requireack)
            {
                giveComport = false;
                return true;
            }

            DateTime GUI = DateTime.Now;

            DateTime start = DateTime.Now;
            int retrys = 3;

            int timeout = 2000;

            // imu calib take a little while
            if (actionid == MAV_CMD.PREFLIGHT_CALIBRATION && p5 == 1)
            {
                // this is for advanced accel offsets, and blocks execution
                giveComport = false;
                return true;
            }
            else if (actionid == MAV_CMD.PREFLIGHT_CALIBRATION && p6 == 1)
            {
                // compassmot
                // send again just incase
                generatePacket((byte) MAVLINK_MSG_ID.COMMAND_LONG, req, sysid, compid);
                giveComport = false;
                return true;
            }
            else if (actionid == MAV_CMD.PREFLIGHT_CALIBRATION)
            {
                retrys = 1;
                timeout = 25000;
            }
            else if (actionid == MAV_CMD.FLASH_BOOTLOADER)
            {
                retrys = 1;
                timeout = 25000;
            }
            else if (actionid == MAV_CMD.PREFLIGHT_REBOOT_SHUTDOWN)
            {
                generatePacket((byte) MAVLINK_MSG_ID.COMMAND_LONG, req, sysid, compid);
                giveComport = false;
                return true;
            }
            else if (actionid == MAV_CMD.COMPONENT_ARM_DISARM)
            {
                // 10 seconds as may need an imu calib
                timeout = 10000;
            }
            else if (actionid == MAV_CMD.GET_HOME_POSITION)
            {
                giveComport = false;
                return true;
            }

            while (true)
            {
                if (DateTime.Now > GUI.AddMilliseconds(100))
                {
                    GUI = DateTime.Now;

                    uicallback?.Invoke();
                }

                if (!(start.AddMilliseconds(timeout) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("doCommand Retry " + retrys);
                        req.confirmation++;
                        generatePacket((byte) MAVLINK_MSG_ID.COMMAND_LONG, req, sysid, compid);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }

                    giveComport = false;
                    throw new TimeoutException("Timeout on read - doCommand");
                }

                buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.COMMAND_ACK && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        var ack = buffer.ToStructure<mavlink_command_ack_t>();

                        if (ack.command != req.command)
                        {
                            log.InfoFormat("doCommand cmd resp {0} - {1} - Commands dont match", (MAV_CMD) ack.command,
                                (MAV_RESULT) ack.result);
                            continue;
                        }

                        log.InfoFormat("doCommand cmd resp {0} - {1}", (MAV_CMD) ack.command, (MAV_RESULT) ack.result);


                        if (ack.result == (byte)MAV_RESULT.IN_PROGRESS)
                        {
                            start = DateTime.Now;
                            retrys = 0;
                            continue;
                        } 
                        else if (ack.result == (byte) MAV_RESULT.ACCEPTED)
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

        public bool doCommandInt(byte sysid, byte compid, MAV_CMD actionid, float p1, float p2, float p3, float p4,
            int p5, int p6, float p7, bool requireack = true, Action uicallback = null,
            MAV_FRAME frame = MAV_FRAME.GLOBAL)
        {
            return doCommandIntAsync(sysid, compid, actionid, p1, p2, p3, p4, p5, p6, p7, requireack, uicallback, frame)
                .AwaitSync();
        }

        public async Task<bool> doCommandIntAsync(byte sysid, byte compid, MAV_CMD actionid, float p1, float p2,
            float p3, float p4,
            int p5, int p6, float p7, bool requireack = true, Action uicallback = null,
            MAV_FRAME frame = MAV_FRAME.GLOBAL)
        {
            if (BaseStream == null || BaseStream.IsOpen == false)
                return false;

            MAVLinkMessage buffer;

            mavlink_command_int_t req = new mavlink_command_int_t()
            {
                target_system = sysid,
                target_component = compid,

                command = (ushort) actionid,

                param1 = p1,
                param2 = p2,
                param3 = p3,
                param4 = p4,
                x = p5,
                y = p6,
                z = p7,
                autocontinue = 0,
                current = 0,
                frame = (byte) frame
            };

            log.InfoFormat("doCommandIntAsync cmd {0} {1} {2} {3} {4} {5} {6} {7}", actionid.ToString(), p1, p2, p3, p4,
                p5, p6,
                p7);

            if (requireack)
                giveComport = true;

            generatePacket((byte) MAVLINK_MSG_ID.COMMAND_INT, req, sysid, compid);

            if (!requireack)
            {
                giveComport = false;
                return true;
            }

            DateTime GUI = DateTime.Now;

            DateTime start = DateTime.Now;
            int retrys = 3;

            int timeout = 2000;

            while (true)
            {
                if (DateTime.Now > GUI.AddMilliseconds(100))
                {
                    GUI = DateTime.Now;

                    uicallback?.Invoke();
                }

                if (!(start.AddMilliseconds(timeout) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("doCommandIntAsync Retry " + retrys);
                        generatePacket((byte) MAVLINK_MSG_ID.COMMAND_INT, req, sysid, compid);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }

                    giveComport = false;
                    throw new TimeoutException("Timeout on read - doCommand");
                }

                buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.COMMAND_ACK && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        var ack = buffer.ToStructure<mavlink_command_ack_t>();

                        if (ack.command != req.command)
                        {
                            log.InfoFormat("doCommandIntAsync cmd resp {0} - {1} - Commands dont match",
                                (MAV_CMD) ack.command,
                                (MAV_RESULT) ack.result);
                            continue;
                        }

                        log.InfoFormat("doCommandIntAsync cmd resp {0} - {1}", (MAV_CMD) ack.command,
                            (MAV_RESULT) ack.result);

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
            ack.command = (ushort) MAV_CMD.PREFLIGHT_CALIBRATION;
            ack.result = 0;

            // send twice
            generatePacket(MAVLINK_MSG_ID.COMMAND_ACK, ack);
            Thread.Sleep(20);
            generatePacket(MAVLINK_MSG_ID.COMMAND_ACK, ack);
        }

        public void SendSerialControl(SERIAL_CONTROL_DEV port, ushort timeoutms, byte[] data, uint baudrate = 0,
            bool close = false)
        {
            mavlink_serial_control_t ctl = new mavlink_serial_control_t
            {
                baudrate = baudrate,
                device = (byte) port,
                timeout = timeoutms,
                data = new byte[70],
                count = 0
            };

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
                int packets = (data.Length / 70) + 1;
                int len = data.Length;
                while (len > 0)
                {
                    if (packets == 1)
                        ctl.flags |= (byte) SERIAL_CONTROL_FLAG.RESPOND;

                    byte n = (byte) Math.Min(70, len);

                    ctl.count = n;
                    Array.Copy(data, data.Length - len, ctl.data, 0, n);

                    // dont flood the port
                    Thread.Sleep(10);

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

        public void SendRCOverride(byte sysid, byte compid, UInt16 rc1, UInt16 rc2, UInt16 rc3, UInt16 rc4, UInt16 rc5,
            UInt16 rc6, UInt16 rc7, UInt16 rc8)
        {
            mavlink_rc_channels_override_t rc = new mavlink_rc_channels_override_t
            {
                target_component = compid,
                target_system = sysid,
                chan1_raw = rc1,
                chan2_raw = rc2,
                chan3_raw = rc3,
                chan4_raw = rc4,
                chan5_raw = rc5,
                chan6_raw = rc6,
                chan7_raw = rc7,
                chan8_raw = rc8
            };

            sendPacket(rc, rc.target_system, rc.target_component);
        }

        public void SendManualControl(byte sysid, byte compid, Int16 x, Int16 y, Int16 z, Int16 r, UInt16 buttons)
        {
            mavlink_manual_control_t mc = new mavlink_manual_control_t
            {
                target = sysid,
                x = x,
                y = y,
                z = z,
                r = r,
                buttons = buttons
            };

            sendPacket(mc, sysid, compid);
        }

        public void requestDatastream(MAV_DATA_STREAM id, byte hzrate, int sysid = -1, int compid = -1)
        {
            requestDatastream(id, (int) hzrate, sysid, compid);
        }

        public void requestDatastream(MAV_DATA_STREAM id, int hzrate, int sysid = -1, int compid = -1)
        {
            if (sysid == -1)
                sysid = sysidcurrent;

            if (compid == -1)
                compid = compidcurrent;

            if (hzrate == -1)
                return;

            double pps = 0;

            switch (id)
            {
                case MAV_DATA_STREAM.ALL:

                    break;

                case MAV_DATA_STREAM.EXTENDED_STATUS:
                    if (MAVlist[sysid, compid].packetspersecondbuild.ContainsKey((byte) MAVLINK_MSG_ID.SYS_STATUS))
                    {
                        if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.SYS_STATUS] <
                            DateTime.UtcNow.AddSeconds(-2))
                            break;
                        pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.SYS_STATUS];
                    }

                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }

                    break;

                case MAV_DATA_STREAM.EXTRA1:
                    if (MAVlist[sysid, compid].packetspersecondbuild.ContainsKey((byte) MAVLINK_MSG_ID.ATTITUDE))
                    {
                        if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.ATTITUDE] <
                            DateTime.UtcNow.AddSeconds(-2))
                            break;
                        pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.ATTITUDE];
                    }

                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }

                    break;

                case MAV_DATA_STREAM.EXTRA2:
                    if (MAVlist[sysid, compid].packetspersecondbuild.ContainsKey((byte) MAVLINK_MSG_ID.VFR_HUD))
                    {
                        if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.VFR_HUD] <
                            DateTime.UtcNow.AddSeconds(-2))
                            break;
                        pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.VFR_HUD];
                    }

                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }

                    break;

                case MAV_DATA_STREAM.EXTRA3:
                    if (MAVlist[sysid, compid].packetspersecondbuild.ContainsKey((byte) MAVLINK_MSG_ID.AHRS))
                    {
                        if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.AHRS] <
                            DateTime.UtcNow.AddSeconds(-2))
                            break;
                        pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.AHRS];
                    }

                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }

                    break;

                case MAV_DATA_STREAM.POSITION:
                    if (MAVlist[sysid, compid]
                        .packetspersecondbuild.ContainsKey((byte) MAVLINK_MSG_ID.GLOBAL_POSITION_INT))
                    {
                        if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.GLOBAL_POSITION_INT] <
                            DateTime.UtcNow.AddSeconds(-2))
                            break;
                        pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.GLOBAL_POSITION_INT];
                    }

                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }

                    break;

                case MAV_DATA_STREAM.RAW_CONTROLLER:
                    if (MAVlist[sysid, compid]
                        .packetspersecondbuild.ContainsKey((byte) MAVLINK_MSG_ID.RC_CHANNELS_SCALED))
                    {
                        if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.RC_CHANNELS_SCALED] <
                            DateTime.UtcNow.AddSeconds(-2))
                            break;
                        pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.RC_CHANNELS_SCALED];
                    }

                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }

                    break;

                case MAV_DATA_STREAM.RAW_SENSORS:
                    if (MAVlist[sysid, compid].packetspersecondbuild.ContainsKey((byte) MAVLINK_MSG_ID.RAW_IMU))
                    {
                        if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.RAW_IMU] <
                            DateTime.UtcNow.AddSeconds(-2))
                            break;
                        pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.RAW_IMU];
                    }

                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }

                    break;

                case MAV_DATA_STREAM.RC_CHANNELS:
                    if (MAVlist[sysid, compid].packetspersecondbuild.ContainsKey((byte) MAVLINK_MSG_ID.RC_CHANNELS_RAW))
                    {
                        if (MAVlist[sysid, compid].packetspersecondbuild[(byte) MAVLINK_MSG_ID.RC_CHANNELS_RAW] <
                            DateTime.UtcNow.AddSeconds(-2))
                            break;
                        pps = MAVlist[sysid, compid].packetspersecond[(byte) MAVLINK_MSG_ID.RC_CHANNELS_RAW];
                    }

                    if (hzratecheck(pps, hzrate))
                    {
                        return;
                    }

                    break;
            }

            if (pps == 0 && hzrate == 0)
            {
                return;
            }

            log.InfoFormat("Request stream {0} at {1} hz for {2}:{3}",
                Enum.Parse(typeof(MAV_DATA_STREAM), id.ToString()), hzrate, sysid, compid);
            getDatastream((byte) sysid, (byte) compid, id, hzrate);
        }

        // returns true for ok
        private bool hzratecheck(double pps, int hzrate)
        {
            if (double.IsInfinity(pps))
            {
                return false;
            }
            // 0 request
            else if (hzrate == 0 && pps == 0)
            {
                return true;
            }

            // range check pps, include packetloss
            if (pps > hzrate - 1 && pps < hzrate + 0.1)
                return true;

            return false;
        }

        [Obsolete]
        private void getDatastream(MAV_DATA_STREAM id, int hzrate)
        {
            getDatastream(MAV.sysid, MAV.compid, id, hzrate);
        }

        private void getDatastream(byte sysid, byte compid, MAV_DATA_STREAM id, int hzrate)
        {
            if (hzrate == -1)
                return;

            mavlink_request_data_stream_t req = new mavlink_request_data_stream_t
            {
                target_system = sysid,
                target_component = compid,
                req_message_rate = (byte) hzrate,
                start_stop = 1,
                req_stream_id = (byte) id
            };

            // send each one twice.
            generatePacket((byte) MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req, sysid, compid);
            generatePacket((byte) MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req, sysid, compid);
        }

        [Obsolete]
        public ushort getWPCount(MAVLink.MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            return getWPCountAsync(MAV.sysid, MAV.compid, type).AwaitSync();
        }

        public ushort getWPCount(byte sysid, byte compid, MAVLink.MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            return getWPCountAsync(sysid, compid, type).AwaitSync();
        }

        /// <summary>
        /// Returns WP count
        /// </summary>
        /// <returns></returns>
        public async Task<ushort> getWPCountAsync(byte sysid, byte compid,
            MAVLink.MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            giveComport = true;
            MAVLinkMessage buffer;
            mavlink_mission_request_list_t req = new mavlink_mission_request_list_t
            {
                target_system = sysid,
                target_component = compid,
                mission_type = (byte) type
            };

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
                    throw new TimeoutException("Timeout on read - getWPCount");
                }

                buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_COUNT && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        var count = buffer.ToStructure<mavlink_mission_count_t>();
                        // check this gcs sent it
                        if (count.target_system != gcssysid ||
                            count.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        log.Info("wpcount: " + count.count);
                        giveComport = false;
                        return count.count; // should be ushort, but apm has limited wp count < byte
                    }
                }
            }
        }

        [Obsolete]
        public Locationwp getHomePosition()
        {
            return getHomePositionAsync((byte) sysidcurrent, (byte) compidcurrent).AwaitSync();
        }

        public async Task<Locationwp> getHomePositionAsync(byte sysid, byte compid)
        {
            Locationwp? ans = null;

            var sub1 = SubscribeToPacketType(MAVLINK_MSG_ID.HOME_POSITION, buffer =>
            {
                if (buffer.msgid == (byte) MAVLINK_MSG_ID.HOME_POSITION)
                {
                    var home = buffer.ToStructure<mavlink_home_position_t>();

                    ans = new Locationwp().Set(home.latitude / 1.0e7, home.longitude / 1.0e7, home.altitude / 1000.0,
                        (byte) MAV_CMD.WAYPOINT);
                }

                return true;
            }, (byte)sysid, (byte)compid);

            try
            {
                doCommand(sysid, compid, MAV_CMD.GET_HOME_POSITION, 0, 0, 0, 0, 0, 0, 0, false);

                giveComport = true;
                DateTime start = DateTime.Now;
                int retrys = 3;

                while (true)
                {
                    if (ans != null)
                        return ans.Value;

                    if (!(start.AddMilliseconds(700) > DateTime.Now))
                    {
                        if (retrys > 0)
                        {
                            log.Info("getHomePosition Retry " + retrys + " - giv com " + giveComport);
                            doCommand(sysid, compid, MAV_CMD.GET_HOME_POSITION, 0, 0, 0, 0, 0, 0, 0, false);
                            start = DateTime.Now;
                            retrys--;
                            continue;
                        }

                        throw new TimeoutException("Timeout on read - getHomePosition");
                    }

                    await readPacketAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                giveComport = false;
                UnSubscribeToPacketType(sub1);
            }
        }

        [Obsolete]
        public Locationwp getWP(ushort index, MAVLink.MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            return getWP(MAV.sysid, MAV.compid, index, type);
        }

        public Locationwp getWP(byte sysid, byte compid, ushort index,
            MAVLink.MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            return getWPAsync(sysid, compid, index, type).AwaitSync();
        }

        /// <summary>
        /// Gets specfied WP
        /// </summary>
        /// <param name="index"></param>
        /// <returns>WP</returns>
        public async Task<Locationwp> getWPAsync(byte sysid, byte compid, ushort index,
            MAVLink.MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            while (giveComport == true)
                Thread.Sleep(10);

            bool use_int = (MAVlist[sysid, compid].cs.capabilities & (uint) MAV_PROTOCOL_CAPABILITY.MISSION_INT) > 0;

            object req;

            if (use_int)
            {
                mavlink_mission_request_int_t reqi = new mavlink_mission_request_int_t
                {
                    target_system = sysid,
                    target_component = compid,
                    mission_type = (byte) type,
                    seq = index
                };

                // request
                generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST_INT, reqi);

                req = reqi;
            }
            else
            {
                mavlink_mission_request_t reqf = new mavlink_mission_request_t
                {
                    target_system = sysid,
                    target_component = compid,
                    mission_type = (byte) type,
                    seq = index
                };

                // request
                generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST, reqf);

                req = reqf;
            }

            giveComport = true;
            Locationwp loc = new Locationwp();

            DateTime start = DateTime.Now;
            int retrys = 5;

            while (true)
            {
                if (!(start.AddMilliseconds(2500) > DateTime.Now)) // apm times out after 5000ms
                {
                    if (retrys > 0)
                    {
                        log.Info("getWP Retry " + retrys);
                        if (use_int)
                            generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST_INT, req);
                        else
                            generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }

                    giveComport = false;
                    throw new TimeoutException("Timeout on read - getWP");
                }

                //Console.WriteLine("getwp read " + DateTime.Now.Millisecond);
                MAVLinkMessage buffer = await readPacketAsync().ConfigureAwait(false);
                //Console.WriteLine("getwp readend " + DateTime.Now.Millisecond);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_ITEM && buffer.sysid == sysid &&
                        buffer.compid == compid)
                    {
                        //Console.WriteLine("getwp ans " + DateTime.Now.Millisecond);

                        var wp = buffer.ToStructure<mavlink_mission_item_t>();
                        if (wp.target_system != gcssysid ||
                            wp.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        // received a packet, but not what we requested
                        if (index != wp.seq)
                        {
                            generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST, req);
                            continue;
                        }

                        loc.frame = wp.frame;
                        loc.id = (ushort) (wp.command);
                        loc.p1 = (wp.param1);
                        loc.p2 = (wp.param2);
                        loc.p3 = (wp.param3);
                        loc.p4 = (wp.param4);

                        loc.alt = ((wp.z));
                        loc.lat = ((wp.x));
                        loc.lng = ((wp.y));

                        log.InfoFormat("getWP {0} {1} {2} {3} {4} opt {5}", loc.id, loc.p1, loc.alt, loc.lat, loc.lng,
                            loc.frame);

                        break;
                    }
                    else if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_ITEM_INT && buffer.sysid == sysid &&
                             buffer.compid == compid)
                    {
                        //Console.WriteLine("getwp ans " + DateTime.Now.Millisecond);

                        var wp = buffer.ToStructure<mavlink_mission_item_int_t>();
                        // check this gcs sent it
                        if (wp.target_system != gcssysid ||
                            wp.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        // received a packet, but not what we requested
                        if (index != wp.seq)
                        {
                            generatePacket((byte) MAVLINK_MSG_ID.MISSION_REQUEST_INT, req);
                            continue;
                        }

                        loc.frame = wp.frame;
                        loc.id = (ushort) (wp.command);
                        loc.p1 = (wp.param1);
                        loc.p2 = (wp.param2);
                        loc.p3 = (wp.param3);
                        loc.p4 = (wp.param4);

                        loc.alt = ((wp.z));
                        loc.lat = ((wp.x / 1.0e7));
                        loc.lng = ((wp.y / 1.0e7));

                        if (!Locationwp.isLocationCommand(loc.id))
                        {
                            loc.lat = wp.x;
                            loc.lng = wp.y;
                        }

                        log.InfoFormat("getWPint {0} {1} {2} {3} {4} opt {5}", loc.id, loc.p1, loc.alt, loc.lat,
                            loc.lng,
                            loc.frame);

                        break;
                    }
                    else
                    {
                        //log.Info(DateTime.Now + " PC getwp " + buffer.msgid);
                    }
                }
            }

            giveComport = false;
            return loc;
        }

        public object DebugPacket(MAVLinkMessage datin)
        {
            string text = "";
            return DebugPacket(datin, ref text, true);
        }

        public object DebugPacket(MAVLinkMessage datin, bool PrintToConsole)
        {
            string text = "";
            return DebugPacket(datin, ref text, PrintToConsole);
        }

        public object DebugPacket(MAVLinkMessage datin, ref string text)
        {
            return DebugPacket(datin, ref text, true);
        }

        /// <summary>
        /// Print entire decoded packet to console
        /// </summary>
        /// <param name="datin">packet byte array</param>
        /// <returns>struct of data</returns>
        public object DebugPacket(MAVLinkMessage datin, ref string text, bool PrintToConsole, string delimeter = " ")
        {
            string textoutput = "";
            try
            {
                if (datin.Length > 5)
                {
                    textoutput =
                        string.Format(
                            "{0,2:X}{8}{1,2:X}{8}{2,2:X}{8}{3,2:X}{8}{4,2:X}{8}{5,2:X}{8}{6,2:X}{8}{7,6:X}{8}",
                            datin.header,
                            datin.payloadlength, datin.incompat_flags, datin.compat_flags, datin.seq, datin.sysid,
                            datin.compid, datin.msgid, delimeter);

                    object data = datin.data;

                    Type test = data.GetType();

                    {
                        textoutput = textoutput + test.Name + delimeter;

                        foreach (var field in test.GetFields())
                        {
                            // field.Name has the field's name.

                            object fieldValue = field.GetValue(data); // Get value

                            if (field.FieldType.IsArray)
                            {
                                textoutput = textoutput + field.Name + delimeter;
                                if (fieldValue.GetType() == typeof(byte[]))
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

                                if (fieldValue.GetType() == typeof(char[]))
                                {
                                    try
                                    {
                                        char[] crap = (char[]) fieldValue;

                                        foreach (char fiel in crap)
                                        {
                                            if (fiel == 0)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                textoutput = textoutput + fiel;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }

                                if (fieldValue.GetType() == typeof(short[]))
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

                        var sig = "";
                        if (datin.sig != null)
                            sig = Convert.ToBase64String(datin.sig);

                        textoutput = textoutput + delimeter + "sig " + sig + delimeter + "Len" + delimeter +
                                     datin.Length + delimeter + "crc16" + delimeter + datin.crc16 + "\r\n";
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
                textoutput = textoutput + "\r\n";
            }

            return null;
        }

        [Obsolete]
        public void setWPPartialUpdate(ushort startwp, ushort endwp,
            MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            setWPPartialUpdateAsync(MAV.sysid, MAV.compid, startwp, endwp, type).ConfigureAwait(false).GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Set start and finish for partial wp upload.
        /// </summary>
        /// <param name="startwp"></param>
        /// <param name="endwp"></param>
        public async Task setWPPartialUpdateAsync(byte sysid, byte compid, ushort startwp, ushort endwp,
            MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            mavlink_mission_write_partial_list_t req = new mavlink_mission_write_partial_list_t
            {
                target_system = sysid,
                target_component = compid,
                start_index = (short) startwp,
                end_index = (short) endwp,
                mission_type = (byte) type
            };

            generatePacket((byte) MAVLINK_MSG_ID.MISSION_WRITE_PARTIAL_LIST, req);
        }

        [Obsolete]
        public void setWPTotal(ushort wp_total, MAVLink.MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            setWPTotalAsync(MAV.sysid, MAV.compid, wp_total, type).ConfigureAwait(false).GetAwaiter().GetResult();
            return;
        }

        /// <summary>
        /// Sets wp total count
        /// </summary>
        /// <param name="wp_total"></param>
        public async Task setWPTotalAsync(byte sysid, byte compid, ushort wp_total,
            MAVLink.MAV_MISSION_TYPE type = MAV_MISSION_TYPE.MISSION)
        {
            giveComport = true;
            mavlink_mission_count_t req = new mavlink_mission_count_t
            {
                target_system = sysid,
                target_component = compid,
                count = wp_total,
                mission_type = (byte) type
            };

            log.Info("setWPTotal req MISSION_COUNT " + req.ToJSON(Formatting.None));
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
                    throw new TimeoutException("Timeout on read - setWPTotal");
                }

                MAVLinkMessage buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 9)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_REQUEST && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        var request = buffer.ToStructure<mavlink_mission_request_t>();
                        // check this gcs sent it
                        if (request.target_system != gcssysid ||
                            request.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        if (request.seq == 0 || request.seq == 1)
                        {
                            if (req.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                            {
                                if (MAV.param["WP_TOTAL"] != null)
                                    MAV.param["WP_TOTAL"].Value = wp_total - 1;
                                if (MAV.param["CMD_TOTAL"] != null)
                                    MAV.param["CMD_TOTAL"].Value = wp_total - 1;
                                if (MAV.param["MIS_TOTAL"] != null)
                                    MAV.param["MIS_TOTAL"].Value = wp_total - 1;
                                MAVlist[req.target_system, req.target_component].wps.Clear();
                            }

                            if (req.mission_type == (byte) MAV_MISSION_TYPE.FENCE)
                                MAVlist[req.target_system, req.target_component].fencepoints.Clear();
                            if (req.mission_type == (byte) MAV_MISSION_TYPE.RALLY)
                                MAVlist[req.target_system, req.target_component].rallypoints.Clear();

                            giveComport = false;
                            return;
                        }
                    }
                    else if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_REQUEST_INT &&
                             buffer.sysid == req.target_system && buffer.compid == req.target_component)
                    {
                        var request = buffer.ToStructure<mavlink_mission_request_int_t>();
                        // check this gcs sent it
                        if (request.target_system != gcssysid ||
                            request.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        if (request.seq == 0 || request.seq == 1)
                        {
                            if (req.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                            {
                                if (MAV.param["WP_TOTAL"] != null)
                                    MAV.param["WP_TOTAL"].Value = wp_total - 1;
                                if (MAV.param["CMD_TOTAL"] != null)
                                    MAV.param["CMD_TOTAL"].Value = wp_total - 1;
                                if (MAV.param["MIS_TOTAL"] != null)
                                    MAV.param["MIS_TOTAL"].Value = wp_total - 1;
                                MAVlist[req.target_system, req.target_component].wps.Clear();
                            }

                            if (req.mission_type == (byte) MAV_MISSION_TYPE.FENCE)
                                MAVlist[req.target_system, req.target_component].fencepoints.Clear();
                            if (req.mission_type == (byte) MAV_MISSION_TYPE.RALLY)
                                MAVlist[req.target_system, req.target_component].rallypoints.Clear();

                            giveComport = false;
                            return;
                        }
                    }
                    else if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_ACK && buffer.sysid == req.target_system &&
                             buffer.compid == req.target_component)
                    {
                        var ans = buffer.ToStructure<mavlink_mission_ack_t>();
                        log.Info("setWPTotal ACK 47 : " + buffer.msgid + " ans " +
                                 Enum.Parse(typeof(MAV_MISSION_RESULT), ans.type.ToString()));
                        // check this gcs sent it
                        if (ans.target_system != gcssysid ||
                            ans.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        giveComport = false;
                        return; // (MAV_MISSION_RESULT)ans.type;
                    }
                    else
                    {
                        //Console.WriteLine(DateTime.Now + " PC getwp " + buffer.msgid);
                    }
                }
            }
        }

        [Obsolete]
        public void InjectGpsData(byte[] data, ushort length)
        {
            InjectGpsData(MAV.sysid, MAV.compid, data, length);
        }

        private int inject_seq_no = 0;

        /// <summary>
        /// used to inject data into the gps ie rtcm/sbp/ubx
        /// </summary>
        /// <param name="data"></param>
        public void InjectGpsData(byte sysid, byte compid, byte[] data, ushort length, bool rtcm_message = true)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            // new message
            if (rtcm_message)
            {
                mavlink_gps_rtcm_data_t gps = new mavlink_gps_rtcm_data_t();
                var msglen = 180;

                if (length > msglen * 4)
                {
                    log.Error("Message too large " + length);
                    return;
                }

                // number of packets we need, including a termination packet if needed
                var nopackets = (length % msglen) == 0 ? length / msglen + 1 : (length / msglen) + 1;

                if (nopackets >= 4)
                    nopackets = 4;

                // flags = isfrag(1)/frag(2)/seq(5)

                for (int a = 0; a < nopackets; a++)
                {
                    // check if its a fragment
                    if (nopackets > 1)
                        gps.flags = 1;
                    else
                        gps.flags = 0;

                    // add fragment number
                    gps.flags += (byte) ((a & 0x3) << 1);

                    // add seq number
                    gps.flags += (byte) ((inject_seq_no & 0x1f) << 3);

                    // create the empty buffer
                    gps.data = new byte[msglen];

                    // calc how much data we are copying
                    int copy = Math.Min(length - a * msglen, msglen);

                    // copy the data
                    Array.Copy(data, a * msglen, gps.data, 0, copy);

                    // set the length
                    gps.len = (byte) copy;

                    generatePacket((byte) MAVLINK_MSG_ID.GPS_RTCM_DATA, gps, sysid, compid);
                }

                inject_seq_no++;
            }
            else
            {
                mavlink_gps_inject_data_t gps = new mavlink_gps_inject_data_t();
                var msglen = 110;

                var len = (length % msglen) == 0 ? length / msglen : (length / msglen) + 1;

                for (int a = 0; a < len; a++)
                {
                    gps.data = new byte[msglen];

                    int copy = Math.Min(length - a * msglen, msglen);

                    Array.Copy(data, a * msglen, gps.data, 0, copy);
                    gps.len = (byte) copy;
                    gps.target_component = compid;
                    gps.target_system = sysid;

                    generatePacket((byte) MAVLINK_MSG_ID.GPS_INJECT_DATA, gps, sysid, compid);
                }
            }
        }

        [Obsolete]
        public MAV_MISSION_RESULT setWP(Locationwp loc, ushort index, MAV_FRAME frame, byte current = 0,
            byte autocontinue = 1, bool use_int = false, MAV_MISSION_TYPE mission_type = MAV_MISSION_TYPE.MISSION)
        {
            return setWP(MAV.sysid, MAV.compid, loc, index, frame, current, autocontinue, use_int, mission_type);
        }

        public MAV_MISSION_RESULT setWP(byte sysid, byte compid, Locationwp loc, ushort index, MAV_FRAME frame,
            byte current = 0,
            byte autocontinue = 1, bool use_int = false, MAV_MISSION_TYPE mission_type = MAV_MISSION_TYPE.MISSION)
        {
            return setWPAsync(sysid, compid, loc, index, frame, current, autocontinue, use_int, mission_type)
                .AwaitSync();
        }

        /// <summary>
        /// Save wp to eeprom
        /// </summary>
        /// <param name="loc">location struct</param>
        /// <param name="index">wp no</param>
        /// <param name="frame">global or relative</param>
        /// <param name="current">0 = no , 2 = guided mode</param>
        public async Task<MAV_MISSION_RESULT> setWPAsync(byte sysid, byte compid, Locationwp loc, ushort index,
            MAV_FRAME frame, byte current = 0,
            byte autocontinue = 1, bool use_int = false, MAV_MISSION_TYPE mission_type = MAV_MISSION_TYPE.MISSION)
        {
            if (use_int)
            {
                mavlink_mission_item_int_t req = new mavlink_mission_item_int_t
                {
                    target_system = sysid,
                    target_component = compid,
                    command = loc.id,
                    current = current,
                    autocontinue = autocontinue,
                    mission_type = (byte) mission_type,
                    frame = (byte) frame
                };

                if (!Locationwp.isLocationCommand(loc.id))
                {
                    req.y = (int) (loc.lng);
                    req.x = (int) (loc.lat);
                }
                else
                {
                    req.y = (int) (loc.lng * 1.0e7);
                    req.x = (int) (loc.lat * 1.0e7);
                }

                req.z = (float) (loc.alt);

                req.param1 = loc.p1;
                req.param2 = loc.p2;
                req.param3 = loc.p3;
                req.param4 = loc.p4;

                req.seq = index;

                return await setWPAsync(req).ConfigureAwait(false);
            }
            else
            {
                mavlink_mission_item_t req = new mavlink_mission_item_t
                {
                    target_system = sysid,
                    target_component = compid,
                    command = loc.id,
                    current = current,
                    autocontinue = autocontinue,
                    mission_type = (byte) mission_type,
                    frame = (byte) frame,
                    y = (float) (loc.lng),
                    x = (float) (loc.lat),
                    z = (float) (loc.alt),
                    param1 = loc.p1,
                    param2 = loc.p2,
                    param3 = loc.p3,
                    param4 = loc.p4,
                    seq = index
                };

                return await setWPAsync(req).ConfigureAwait(false);
            }
        }

        [Obsolete]
        public async Task<MAV_MISSION_RESULT> setWPAsync(mavlink_mission_item_t req)
        {
            giveComport = true;

            ushort index = req.seq;

            log.InfoFormat("setWP {7}:{8} {6} frame {0} cmd {1} p1 {2} x {3} y {4} z {5}", req.frame, req.command,
                req.param1,
                req.x, req.y, req.z, index, req.target_system, req.target_component);

            // request
            generatePacket((byte) MAVLINK_MSG_ID.MISSION_ITEM, req);

            DateTime start = DateTime.Now;
            int retrys = 10;

            while (true)
            {
                if (!(start.AddMilliseconds(450) > DateTime.Now))
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
                    throw new TimeoutException("Timeout on read - setWP");
                }

                MAVLinkMessage buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_ACK && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        var ans = buffer.ToStructure<mavlink_mission_ack_t>();
                        log.Info("set wp " + index + " ACK 47 : " + buffer.msgid + " ans " +
                                 Enum.Parse(typeof(MAV_MISSION_RESULT), ans.type.ToString()));
                        // check this gcs sent it
                        if (ans.target_system != gcssysid ||
                            ans.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        if (req.current == 2)
                        {
                            MAVlist[req.target_system, req.target_component].GuidedMode = (Locationwp) req;
                        }
                        else if (req.current == 3)
                        {
                        }
                        else
                        {
                            if (req.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                                MAVlist[req.target_system, req.target_component].wps[req.seq] = (Locationwp) req;
                            if (req.mission_type == (byte) MAV_MISSION_TYPE.FENCE)
                                MAVlist[req.target_system, req.target_component].fencepoints[req.seq] =
                                    (Locationwp) req;
                            if (req.mission_type == (byte) MAV_MISSION_TYPE.RALLY)
                                MAVlist[req.target_system, req.target_component].rallypoints[req.seq] =
                                    (Locationwp) req;
                        }

                        //if (ans.target_system == req.target_system && ans.target_component == req.target_component)
                        {
                            giveComport = false;
                            return (MAV_MISSION_RESULT) ans.type;
                        }
                    }
                    else if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_REQUEST &&
                             buffer.sysid == req.target_system && buffer.compid == req.target_component)
                    {
                        var ans = buffer.ToStructure<mavlink_mission_request_t>();

                        if (ans.target_system != gcssysid ||
                            ans.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        if (ans.seq == (index + 1))
                        {
                            log.Info("set wp doing " + index + " req " + ans.seq + " REQ 40 : " + buffer.msgid);
                            giveComport = false;

                            if (req.current == 2)
                            {
                                MAVlist[req.target_system, req.target_component].GuidedMode = (Locationwp) req;
                            }
                            else if (req.current == 3)
                            {
                            }
                            else
                            {
                                if (req.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                                    MAVlist[req.target_system, req.target_component].wps[req.seq] = (Locationwp) req;
                                if (req.mission_type == (byte) MAV_MISSION_TYPE.FENCE)
                                    MAVlist[req.target_system, req.target_component].fencepoints[req.seq] =
                                        (Locationwp) req;
                                if (req.mission_type == (byte) MAV_MISSION_TYPE.RALLY)
                                    MAVlist[req.target_system, req.target_component].rallypoints[req.seq] =
                                        (Locationwp) req;
                            }

                            //if (ans.target_system == req.target_system && ans.target_component == req.target_component)
                            {
                                giveComport = false;
                                return MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED;
                            }
                        }
                        else
                        {
                            log.InfoFormat(
                                "set wp fail doing " + index + " req " + ans.seq + " ACK 47 or REQ 40 : " +
                                buffer.msgid +
                                " seq {0} ts {1} tc {2}", req.seq, req.target_system, req.target_component);
                            // resend point now
                            start = DateTime.MinValue;
                        }
                    }
                    else if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_REQUEST_INT &&
                             buffer.sysid == req.target_system && buffer.compid == req.target_component)
                    {
                        var ans = buffer.ToStructure<mavlink_mission_request_int_t>();

                        if (ans.target_system != gcssysid ||
                            ans.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        if (ans.seq == (index + 1))
                        {
                            log.Info("set wp doing " + index + " req " + ans.seq + " REQ 40 : " + buffer.msgid);
                            giveComport = false;

                            if (req.current == 2)
                            {
                                MAVlist[req.target_system, req.target_component].GuidedMode = (Locationwp) req;
                            }
                            else if (req.current == 3)
                            {
                            }
                            else
                            {
                                MAVlist[req.target_system, req.target_component].wps[req.seq] = (Locationwp) req;
                            }

                            //if (ans.target_system == req.target_system && ans.target_component == req.target_component)
                            {
                                giveComport = false;
                                return MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED;
                            }
                        }
                        else
                        {
                            log.InfoFormat(
                                "set wp fail doing " + index + " req " + ans.seq + " ACK 47 or REQ 40 : " +
                                buffer.msgid +
                                " seq {0} ts {1} tc {2}", req.seq, req.target_system, req.target_component);
                            // resend point now
                            start = DateTime.MinValue;
                        }
                    }
                    else
                    {
                        //Console.WriteLine(DateTime.Now + " PC setwp " + buffer.msgid);
                    }
                }
            }

            // return MAV_MISSION_RESULT.MAV_MISSION_INVALID;
        }

        [Obsolete]
        public async Task<MAV_MISSION_RESULT> setWPAsync(mavlink_mission_item_int_t req)
        {
            giveComport = true;

            ushort index = req.seq;

            log.Info("setWPint req MISSION_ITEM_INT " + req.ToJSON(Formatting.None));
            //log.InfoFormat("setWPint {7}:{8} {6} frame {0} cmd {1} p1 {2} x {3} y {4} z {5}", req.frame, req.command, req.param1,
            //  req.x / 1.0e7, req.y /1.0e7 , req.z, index, req.target_system, req.target_component);

            // request
            generatePacket((byte) MAVLINK_MSG_ID.MISSION_ITEM_INT, req);

            DateTime start = DateTime.Now;
            int retrys = 10;

            while (true)
            {
                if (!(start.AddMilliseconds(450) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("setWP Retry " + retrys);
                        generatePacket((byte) MAVLINK_MSG_ID.MISSION_ITEM_INT, req);

                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }

                    giveComport = false;
                    throw new TimeoutException("Timeout on read - setWP");
                }

                MAVLinkMessage buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_ACK && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        var ans = buffer.ToStructure<mavlink_mission_ack_t>();
                        log.Info("setWPint resp MISSION_ACK " + buffer.ToJSON(Formatting.None));
                        //log.Info("set wp " + index + " ACK 47 : " + buffer.msgid + " ans " +
                        //       Enum.Parse(typeof(MAV_MISSION_RESULT), ans.type.ToString()));
                        // check this gcs sent it
                        if (ans.target_system != gcssysid ||
                            ans.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        if (req.current == 2)
                        {
                            MAVlist[req.target_system, req.target_component].GuidedMode = (Locationwp) req;
                        }
                        else if (req.current == 3)
                        {
                        }
                        else
                        {
                            if (req.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                                MAVlist[req.target_system, req.target_component].wps[req.seq] = (Locationwp) req;
                            if (req.mission_type == (byte) MAV_MISSION_TYPE.FENCE)
                                MAVlist[req.target_system, req.target_component].fencepoints[req.seq] =
                                    (Locationwp) req;
                            if (req.mission_type == (byte) MAV_MISSION_TYPE.RALLY)
                                MAVlist[req.target_system, req.target_component].rallypoints[req.seq] =
                                    (Locationwp) req;
                        }

                        //if (ans.target_system == req.target_system && ans.target_component == req.target_component)
                        {
                            giveComport = false;
                            return (MAV_MISSION_RESULT) ans.type;
                        }
                    }
                    else if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_REQUEST &&
                             buffer.sysid == req.target_system && buffer.compid == req.target_component)
                    {
                        var ans = buffer.ToStructure<mavlink_mission_request_t>();
                        if (ans.target_system != gcssysid ||
                            ans.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        if (ans.seq == (index + 1))
                        {
                            log.Info("setWPint resp MISSION_REQUEST" + buffer.ToJSON(Formatting.None));
                            //log.Info("set wp doing " + index + " req " + ans.seq + " REQ 40 : " + buffer.msgid);

                            if (req.current == 2)
                            {
                                MAVlist[req.target_system, req.target_component].GuidedMode = (Locationwp) req;
                            }
                            else if (req.current == 3)
                            {
                            }
                            else
                            {
                                if (req.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                                    MAVlist[req.target_system, req.target_component].wps[req.seq] = (Locationwp) req;
                                if (req.mission_type == (byte) MAV_MISSION_TYPE.FENCE)
                                    MAVlist[req.target_system, req.target_component].fencepoints[req.seq] =
                                        (Locationwp) req;
                                if (req.mission_type == (byte) MAV_MISSION_TYPE.RALLY)
                                    MAVlist[req.target_system, req.target_component].rallypoints[req.seq] =
                                        (Locationwp) req;
                            }

                            //if (ans.target_system == req.target_system && ans.target_component == req.target_component)
                            {
                                giveComport = false;
                                return MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED;
                            }
                        }
                        else
                        {
                            log.InfoFormat(
                                "set wp fail doing " + index + " req " + ans.seq + " ACK 47 or REQ 40 : " +
                                buffer.msgid +
                                " seq {0} ts {1} tc {2}", req.seq, req.target_system, req.target_component);
                            // resend point now
                            start = DateTime.MinValue;
                        }
                    }
                    else
                    {
                        //Console.WriteLine(DateTime.Now + " PC setwp " + buffer.msgid);
                    }
                }
            }

            // return MAV_MISSION_RESULT.MAV_MISSION_INVALID;
        }

        public int getRequestedWPNo(byte sysid, byte compid)
        {
            return getRequestedWPNoAsync(sysid, compid).AwaitSync();
        }

        public async Task<int> getRequestedWPNoAsync(byte sysid, byte compid)
        {
            giveComport = true;
            DateTime start = DateTime.Now;

            while (true)
            {
                if (!(start.AddMilliseconds(1500) > DateTime.Now))
                {
                    giveComport = false;
                    throw new TimeoutException("Timeout on read - getRequestedWPNo");
                }

                MAVLinkMessage buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte)MAVLINK_MSG_ID.MISSION_REQUEST && buffer.sysid == sysid &&
                        buffer.compid == compid)
                    {
                        var ans = buffer.ToStructure<mavlink_mission_request_t>();

                        log.InfoFormat("getRequestedWPNo seq {0} ts {1} tc {2}", ans.seq, ans.target_system,
                            ans.target_component);

                        giveComport = false;

                        return ans.seq;
                    }
                    if (buffer.msgid == (byte)MAVLINK_MSG_ID.MISSION_REQUEST_INT && buffer.sysid == sysid &&
                        buffer.compid == compid)
                    {
                        var ans = buffer.ToStructure<mavlink_mission_request_int_t>();

                        log.InfoFormat("getRequestedWPNo INT seq {0} ts {1} tc {2}", ans.seq, ans.target_system,
                            ans.target_component);

                        giveComport = false;

                        return ans.seq;
                    }
                }
            }
        }

        [Obsolete]
        public void setGuidedModeWP(Locationwp gotohere, bool setguidedmode = true)
        {
            setGuidedModeWP(MAV.sysid, MAV.compid, gotohere, setguidedmode);
        }

        public void setGuidedModeWP(byte sysid, byte compid, Locationwp gotohere, bool setguidedmode = true)
        {
            if (gotohere.alt == 0 || gotohere.lat == 0 || gotohere.lng == 0)
                return;

            giveComport = true;

            try
            {
                gotohere.id = (ushort) MAV_CMD.WAYPOINT;

                if (setguidedmode)
                {
                    // fix for followme change
                    if (MAVlist[sysid, compid].cs.mode.ToUpper() != "GUIDED")
                        setMode(sysid, compid, "GUIDED");
                }

                log.InfoFormat("setGuidedModeWP {0}:{1} lat {2} lng {3} alt {4}", sysid, compid, gotohere.lat,
                    gotohere.lng, gotohere.alt);

                if (MAVlist[sysid, compid].cs.firmware == Firmwares.ArduPlane)
                {
                    MAV_MISSION_RESULT ans = setWP(sysid, compid, gotohere, 0, MAV_FRAME.GLOBAL_RELATIVE_ALT, (byte) 2);

                    if (ans != MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                    {
                        giveComport = false;
                        throw new Exception("Guided Mode Failed");
                    }
                }
                else
                {
                    setPositionTargetGlobalInt((byte) sysid, (byte) compid,
                        true, false, false, false, MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT,
                        gotohere.lat, gotohere.lng, gotohere.alt, 0, 0, 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            giveComport = false;
        }

        [Obsolete]
        public void setNewWPAlt(Locationwp gotohere)
        {
            setNewWPAlt((byte) sysidcurrent, (byte) compidcurrent, gotohere);
        }

        public void setNewWPAlt(byte sysid, byte compid, Locationwp gotohere)
        {
            try
            {
                gotohere.id = (ushort) MAV_CMD.WAYPOINT;

                log.InfoFormat("setNewWPAlt {0}:{1} lat {2} lng {3} alt {4}", sysid, compid, gotohere.lat, gotohere.lng,
                    gotohere.alt);

                MAV_MISSION_RESULT ans = setWP(sysid, compid, gotohere, 0, MAV_FRAME.GLOBAL_RELATIVE_ALT, (byte) 3);

                if (ans != MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                {
                    giveComport = false;
                    throw new Exception("Alt Change Failed");
                }

                /*
                // currently plane supports just an alt change, copter requires all lat/lng/alt
                setPositionTargetGlobalInt((byte)sysid, (byte)compid,
                    true, false, false, false, MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT,
                    gotohere.lat, gotohere.lng, gotohere.alt, 0, 0, 0, 0, 0);
                    */
            }
            catch (Exception ex)
            {
                giveComport = false;
                log.Error(ex);
                throw;
            }

            giveComport = false;
        }

        public void setPositionTargetGlobalInt(byte sysid, byte compid, bool pos, bool vel, bool acc, bool yaw,
            MAV_FRAME frame, double lat, double lng, double alt, double vx, double vy, double vz, double yawangle,
            double yawrate)
        {
            // for mavlink SET_POSITION_TARGET messages
            const ushort MAVLINK_SET_POS_TYPE_MASK_POS_IGNORE = ((1 << 0) | (1 << 1) | (1 << 2));
            const ushort MAVLINK_SET_POS_TYPE_MASK_ALT_IGNORE = ((0 << 0) | (0 << 1) | (1 << 2));
            const ushort MAVLINK_SET_POS_TYPE_MASK_VEL_IGNORE = ((1 << 3) | (1 << 4) | (1 << 5));
            const ushort MAVLINK_SET_POS_TYPE_MASK_ACC_IGNORE = ((1 << 6) | (1 << 7) | (1 << 8));
            const ushort MAVLINK_SET_POS_TYPE_MASK_FORCE = ((1 << 9));
            const ushort MAVLINK_SET_POS_TYPE_MASK_YAW_IGNORE = ((1 << 10) | (1 << 11));

            mavlink_set_position_target_global_int_t target = new mavlink_set_position_target_global_int_t()
            {
                target_system = sysid,
                target_component = compid,
                alt = (float) alt,
                lat_int = (int) (lat * 1e7),
                lon_int = (int) (lng * 1e7),
                coordinate_frame = (byte) frame,
                vx = (float) vx,
                vy = (float) vy,
                vz = (float) vz,
                yaw = (float) yawangle,
                yaw_rate = (float) yawrate
            };

            target.type_mask = ushort.MaxValue;
            target.type_mask -= MAVLINK_SET_POS_TYPE_MASK_FORCE;

            if (pos && lat != 0 && lng != 0)
                target.type_mask -= MAVLINK_SET_POS_TYPE_MASK_POS_IGNORE;
            if (pos && lat == 0 && lng == 0)
                target.type_mask -= MAVLINK_SET_POS_TYPE_MASK_ALT_IGNORE;
            if (vel)
                target.type_mask -= MAVLINK_SET_POS_TYPE_MASK_VEL_IGNORE;
            if (acc)
                target.type_mask -= MAVLINK_SET_POS_TYPE_MASK_ACC_IGNORE;
            if (yaw)
                target.type_mask -= MAVLINK_SET_POS_TYPE_MASK_YAW_IGNORE;

            if (pos)
            {
                if (lat != 0)
                    MAVlist[sysid, compid].GuidedMode.x = (int) (lat * 1e7);
                if (lng != 0)
                    MAVlist[sysid, compid].GuidedMode.y = (int) (lng * 1e7);
                MAVlist[sysid, compid].GuidedMode.z = (float) alt;
            }

            bool pos_ignore = (target.type_mask & MAVLINK_SET_POS_TYPE_MASK_POS_IGNORE) > 0;
            bool vel_ignore = (target.type_mask & MAVLINK_SET_POS_TYPE_MASK_VEL_IGNORE) > 0;
            bool acc_ignore = (target.type_mask & MAVLINK_SET_POS_TYPE_MASK_ACC_IGNORE) > 0;

            generatePacket((byte) MAVLINK_MSG_ID.SET_POSITION_TARGET_GLOBAL_INT, target, sysid, compid);
        }

        [Obsolete]
        public void setDigicamControl(bool shot)
        {
            mavlink_digicam_control_t req = new mavlink_digicam_control_t();

            req.target_system = MAV.sysid;
            req.target_component = MAV.compid;
            req.shot = (shot == true) ? (byte) 1 : (byte) 0;

            if (!doCommand((byte) sysidcurrent, (byte) compidcurrent, MAV_CMD.DO_DIGICAM_CONTROL, 0, 0, 0, 0, 1, 0, 0))
            {
                generatePacket((byte) MAVLINK_MSG_ID.DIGICAM_CONTROL, req);
            }
        }

        [Obsolete]
        public void setMountConfigure(MAV_MOUNT_MODE mountmode, bool stabroll, bool stabpitch, bool stabyaw)
        {
            byte stab_roll = (stabroll == true) ? (byte) 1 : (byte) 0;
            byte stab_pitch = (stabpitch == true) ? (byte) 1 : (byte) 0;
            byte stab_yaw = (stabyaw == true) ? (byte) 1 : (byte) 0;

            // p1 : mount mode
            // p2, p3, p4 : stabilize roll, pitch, yaw
            // p5, p6, p7 : empty
            // no ack required
            doCommand(MAV.sysid, MAV.compid, MAV_CMD.DO_MOUNT_CONFIGURE, (byte)mountmode, stab_roll, stab_pitch, stab_yaw, 0, 0, 0, false);
        }

        [Obsolete]
        public void setMountControl(double pa, double pb, double pc, bool islatlng)
        {
            setMountControl(MAV.sysid, MAV.compid, pa, pb, pc, islatlng);
        }

        public void setMountControl(byte sysid, byte compid, double pa, double pb, double pc, bool islatlng)
        {
            if (!islatlng)
            {
                // pa is pitch, pb is roll, pc is yaw in centi-degrees.  convert to degrees
                doCommand(sysid, compid, MAV_CMD.DO_MOUNT_CONTROL, (float)(pa * 0.01), (float)(pb * 0.01), (float)(pc * 0.01), 0, 0, 0, (float)MAV_MOUNT_MODE.MAVLINK_TARGETING, false);
            }
            else
            {
                // pa is lat in degrees, placed in param5 in deg * 10e7
                // pb is lon in degrees, placed in param6 in deg * 10e7
                // pc is alt in meters, placed in param5 in meters
                doCommand(sysid, compid, MAV_CMD.DO_MOUNT_CONTROL, 0, 0, 0, (float)pc, (float)(pa * 10000000.0f), (float)(pb * 10000000.0f), (float)MAV_MOUNT_MODE.MAVLINK_TARGETING, false);
            }
        }

        [Obsolete]
        public void setMode(string modein)
        {
            setMode(MAV.sysid, MAV.compid, modein);
        }

        public void setMode(byte sysid, byte compid, string modein)
        {
            mavlink_set_mode_t mode = new mavlink_set_mode_t();

            if (translateMode(sysid, compid, modein, ref mode))
            {
                log.Info("setMode " + modein + " (" + mode.custom_mode + ")");
                setMode(sysid, compid, mode);
            }
        }

        [Obsolete]
        public void setMode(mavlink_set_mode_t mode, MAV_MODE_FLAG base_mode = 0)
        {
            setMode(MAV.sysid, MAV.compid, mode, base_mode);
        }

        public void setMode(byte sysid, byte compid, mavlink_set_mode_t mode, MAV_MODE_FLAG base_mode = 0)
        {
            mode.base_mode |= (byte) base_mode;

            // new
            doCommand(sysid, compid, MAV_CMD.DO_SET_MODE, mode.base_mode, mode.custom_mode, 0, 0, 0, 0, 0, false);

            // old
            generatePacket((byte) (byte) MAVLINK_MSG_ID.SET_MODE, mode, sysid, compid);
            Thread.Sleep(10);
            generatePacket((byte) (byte) MAVLINK_MSG_ID.SET_MODE, mode, sysid, compid);
        }

        private double t7 = 1.0e7;

        public bool debug = false;

        private static bool ByteArrayCompare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }

        /// <summary>
        /// used for a readlock on readpacket
        /// </summary>
        private SemaphoreSlim readlock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Serial Reader to read mavlink packets. POLL method
        /// </summary>
        /// <returns></returns>
        public MAVLinkMessage readPacket()
        {
            return readPacketAsync().AwaitSync();
        }

        public async Task<MAVLinkMessage> readPacketAsync()
        {
            byte[] buffer = new byte[MAVLINK_MAX_PACKET_LEN + 25];
            int count = 0;
            int length = 0;
            int readcount = 0;
            MAVLinkMessage message = null;
            bool skipheader = false;

            //These need to be function wide, because of the extension of ReadLock try/catch
            byte sysid;
            byte compid;
            byte packetSeqNo;
            uint msgid;
            bool packetSeemValid = false;

            DateTime start = DateTime.Now;

            if (debug)
                Console.WriteLine(DateTime.Now.Millisecond + " SR0 " + BaseStream?.BytesToRead);

            await readlock.WaitAsync().ConfigureAwait(false);
            
            try
            {
                if (debug)
                    Console.WriteLine(DateTime.Now.Millisecond + " SR1 " + BaseStream?.BytesToRead);
                
                readloop:

                while ((BaseStream != null && BaseStream.IsOpen) || logreadmode)
                {
                    try
                    {
                        if (readcount > 300)
                        {
                            break;
                        }

                        readcount++;
                        if (logreadmode)
                        {
                            message = readlogPacketMavlink();
                            buffer = message.buffer;
                            if (buffer == null || buffer.Length == 0)
                                return MAVLinkMessage.Invalid;
                        }
                        else
                        {
                            if (BaseStream.ReadTimeout != 1200)
                                BaseStream.ReadTimeout =
                                    1200; // 1200 ms between chars - the gps detection requires this.

                            // time updated for internal reference
                            MAV.cs.datetime = DateTime.Now;

                            if (!skipheader)
                            {
                                DateTime to = DateTime.UtcNow.AddMilliseconds(BaseStream.ReadTimeout);

                                if (debug)
                                    Console.WriteLine(DateTime.Now.Millisecond + " SR1a " + BaseStream?.BytesToRead);

                                while (BaseStream.IsOpen && BaseStream.BytesToRead <= 0)
                                {
                                    if (DateTime.UtcNow > to)
                                    {
                                        log.InfoFormat("MAVLINK: 1 wait time out btr {0} len {1}",
                                            BaseStream?.BytesToRead,
                                            length);
                                        throw new TimeoutException("Timeout");
                                    }

                                    await Task.Delay(1).ConfigureAwait(false);
                                    if (debug)
                                        Console.WriteLine(DateTime.Now.Millisecond + " SR0b " +
                                                          BaseStream?.BytesToRead);
                                }

                                if (debug)
                                    Console.WriteLine(DateTime.Now.Millisecond + " SR1a " + BaseStream?.BytesToRead);
                                if (BaseStream.IsOpen)
                                {
                                    BaseStream.Read(buffer, count, 1);
                                    if (rawlogfile != null && rawlogfile.CanWrite)
                                        rawlogfile.WriteByte(buffer[count]);
                                }

                                if (debug)
                                    Console.WriteLine(DateTime.Now.Millisecond + " SR1b " + BaseStream?.BytesToRead);
                            }

                            skipheader = false;
                        }
                    }
                    catch (Exception e)
                    {
                        skipheader = false;
                        log.Info("MAVLink readpacket read error: " + e.ToString());
                        break;
                    }

                    // check if looks like a mavlink packet and check for exclusions and write to console
                    if (buffer[0] != MAVLINK_STX_MAVLINK1 && buffer[0] != 'U' && buffer[0] != MAVLINK_STX)
                    {
                        if (buffer[0] >= 0x20 && buffer[0] <= 127 || buffer[0] == '\n' || buffer[0] == '\r')
                        {
                            // check for line termination
                            if (buffer[0] == '\r' || buffer[0] == '\n')
                            {
                                // check new line is valid
                                if (buildplaintxtline.Length > 3)
                                {
                                    plaintxtline = buildplaintxtline;
                                    plaintxtlinebuffer.Insert(0, plaintxtline);
                                    while (plaintxtlinebuffer.Count >= 30)
                                        plaintxtlinebuffer.RemoveAt(plaintxtlinebuffer.Count - 1);
                                }

                                log.Info(plaintxtline);
                                // reset for next line
                                buildplaintxtline = "";
                            }

                            //TCPConsole.Write(buffer[0]);
                            Console.Write((char)buffer[0]);
                            buildplaintxtline += (char)buffer[0];
                        }

                        _bytesReceivedSubj.OnNext(1);
                        count = 0;
                        buffer[1] = 0;
                        continue;
                    }

                    // reset count on valid packet
                    readcount = 0;

                    if (debug)
                        Console.WriteLine(DateTime.Now.Millisecond + " SR2 " + BaseStream?.BytesToRead);

                    // check for a header
                    if (buffer[0] == MAVLINK_STX_MAVLINK1 || buffer[0] == MAVLINK_STX || buffer[0] == 'U')
                    {
                        var mavlinkv2 = buffer[0] == MAVLINK_STX ? true : false;

                        int headerlength = mavlinkv2 ? MAVLINK_CORE_HEADER_LEN : MAVLINK_CORE_HEADER_MAVLINK1_LEN;
                        int headerlengthstx = headerlength + 1;

                        // if we have the header, and no other chars, get the length and packet identifiers
                        if (count < headerlength && !logreadmode)
                        {
                            DateTime to = DateTime.UtcNow.AddMilliseconds(BaseStream.ReadTimeout);

                            while (BaseStream.IsOpen && BaseStream.BytesToRead < headerlength - count)
                            {
                                if (DateTime.UtcNow > to)
                                {
                                    log.InfoFormat("MAVLINK: 2 wait time out btr {0} len {1}", BaseStream.BytesToRead,
                                        length);
                                    throw new TimeoutException("Timeout");
                                }

                                await Task.Delay(1).ConfigureAwait(false);
                            }

                            if (debug)
                                Console.WriteLine(DateTime.Now.Millisecond + " SR2a " + BaseStream?.BytesToRead);
                            //var start1 = DateTime.Now;
                            int read = BaseStream.Read(buffer, count + 1, headerlength - count);
                            //var end = DateTime.Now - start1;
                            //var lapse = end.TotalMilliseconds;
                            //Console.WriteLine("read: " + lapse);
                            if (debug)
                                Console.WriteLine(DateTime.Now.Millisecond + " SR2b " + BaseStream?.BytesToRead);
                            if (rawlogfile != null && rawlogfile.CanWrite)
                                rawlogfile.Write(buffer, count + 1, read);
                            count += read;
                        }

                        // packet length
                        if (buffer[0] == MAVLINK_STX)
                        {
                            length = buffer[1] + headerlengthstx +
                                     MAVLINK_NUM_CHECKSUM_BYTES; // data + header + checksum - magic - length
                            if ((buffer[2] & MAVLINK_IFLAG_SIGNED) > 0)
                            {
                                length += MAVLINK_SIGNATURE_BLOCK_LEN;
                            }
                        }
                        else
                        {
                            length = buffer[1] + headerlengthstx +
                                     MAVLINK_NUM_CHECKSUM_BYTES; // data + header + checksum - U - length
                        }

                        if (debug)
                            Console.WriteLine(DateTime.Now.Millisecond + " SR3 " + BaseStream?.BytesToRead);

                        if (count >= headerlength || logreadmode)
                        {
                            try
                            {
                                if (logreadmode)
                                {
                                }
                                else
                                {
                                    DateTime to = DateTime.UtcNow.AddMilliseconds(BaseStream.ReadTimeout);

                                    while (BaseStream.IsOpen && BaseStream.BytesToRead < (length - (headerlengthstx)))
                                    {
                                        if (DateTime.UtcNow > to)
                                        {
                                            log.InfoFormat("MAVLINK: 3 wait time out btr {0} len {1}",
                                                BaseStream.BytesToRead, length);
                                            break;
                                        }

                                        await Task.Delay(1).ConfigureAwait(false);
                                    }

                                    if (BaseStream.IsOpen)
                                    {
                                        //var start1 = DateTime.UtcNow;
                                        int read = BaseStream.Read(buffer, headerlengthstx, length - (headerlengthstx));
                                        //var end = DateTime.UtcNow - start1;
                                        //var lapse = end.TotalMilliseconds;
                                        //Console.WriteLine("read: " + lapse);

                                        if (read != (length - headerlengthstx))
                                            log.InfoFormat("MAVLINK: bad read {0}, {1}, {2}", headerlengthstx, length,
                                                count);
                                        if (rawlogfile != null && rawlogfile.CanWrite)
                                        {
                                            // write only what we read, temp is the whole packet, so 6-end
                                            rawlogfile.Write(buffer, headerlengthstx, read);
                                        }
                                    }
                                }

                                count = length;
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

                if (debug)
                    Console.WriteLine(DateTime.Now.Millisecond + " SR4 " + BaseStream?.BytesToRead);
                //ReadLock ended here

                // resize the packet to the correct length
                Array.Resize<byte>(ref buffer, count);

                // add byte count
                _bytesReceivedSubj.OnNext(buffer.Length);

                // update bps statistics
                if (_bpstime.Second != DateTime.UtcNow.Second)
                {
                    long btr = 0;
                    if (BaseStream != null && BaseStream.IsOpen)
                    {
                        btr = BaseStream.BytesToRead;
                    }
                    else if (logreadmode)
                    {
                        btr = logplaybackfile.BaseStream.Length - logplaybackfile.BaseStream.Position;
                    }

                    if (printbps)
                        Console.Write(
                            "bps {0} loss {1} left {2} mem {3} mav2 {4} sign {5} mav1 {6} mav2 {7} signed {8}      \n",
                            _bps1,
                            MAV.synclost, btr,
                            GC.GetTotalMemory(false) / 1024 / 1024.0, MAV.mavlinkv2, MAV.signing,
                            _mavlink1count,
                            _mavlink2count, _mavlink2signed);
                    _bps2 = _bps1; // prev sec
                    _bps1 = 0; // current sec
                    _bpstime = DateTime.UtcNow;
                    _mavlink1count = 0;
                    _mavlink2count = 0;
                    _mavlink2signed = 0;
                }

                _bps1 += buffer.Length;

                if (buffer.Length == 0)
                    return MAVLinkMessage.Invalid;

                if (message == null)
                    message = new MAVLinkMessage(buffer, DateTime.UtcNow);

                msgid = message.msgid;

                message_info msginfo = MAVLINK_MESSAGE_INFOS.GetMessageInfo(msgid);

                // calc crc
                var sigsize = (message.sig != null) ? MAVLINK_SIGNATURE_BLOCK_LEN : 0;
                ushort crc = MavlinkCRC.crc_calculate(buffer,
                    message.Length - sigsize - MAVLINK_NUM_CHECKSUM_BYTES);

                // calc extra bit of crc for mavlink 1.0/2.0
                if (message.header == MAVLINK_STX_MAVLINK1 || message.header == MAVLINK_STX)
                {
                    crc = MavlinkCRC.crc_accumulate(msginfo.crc, crc);
                }

                // check message length size vs table (mavlink1 explicit size check | mavlink2 allow all, undersize 0 trimmed, and oversize unknown extension)
                if (!message.ismavlink2 && message.payloadlength != msginfo.minlength)
                {
                    if (msginfo.length == 0) // pass for unknown packets
                    {
                        log.InfoFormat("unknown packet type {0}", message.msgid);
                    }
                    else
                    {
                        log.InfoFormat("Mavlink Bad Packet (Len Fail) len {0} pkno {1}", buffer.Length,
                            message.msgid);
                        return MAVLinkMessage.Invalid;
                    }
                }

                // check crc
                if ((message.crc16 >> 8) != (crc >> 8) ||
                    (message.crc16 & 0xff) != (crc & 0xff))
                {
                    if (buffer.Length > 5 && msginfo.name != null)
                        log.InfoFormat("Mavlink Bad Packet (crc fail) len {0} crc {1} vs {4} pkno {2} {3}",
                            buffer.Length,
                            crc, message.msgid, msginfo.name.ToString(),
                            message.crc16);
                    if (logreadmode)
                        log.InfoFormat("bad packet pos {0} ", logplaybackfile.BaseStream.Position);
                    // unknown packet
                    if (buffer.Length > 5 && String.IsNullOrEmpty(msginfo.name))
                        ProcessMirrorStream(buffer);

                    // look within the last 20 bytes
                    int offset = buffer.Search(new byte[] { MAVLINK_STX }, Math.Max(1, buffer.Length - 20));
                    if (offset > 0)
                    {
                        var newbuffer = new byte[MAVLINK_MAX_PACKET_LEN + 25];
                        Array.ConstrainedCopy(buffer, offset, newbuffer, 0, buffer.Length - offset);
                        count = buffer.Length - offset - 1;
                        log.InfoFormat($"rewind oldlen {buffer.Length} offset {offset} cnt {count}");
                        buffer = newbuffer;
                        message = null;
                        skipheader = true;
                        goto readloop;
                    }

                    return MAVLinkMessage.Invalid;
                }

                sysid = message.sysid;
                compid = message.compid;
                packetSeqNo = message.seq;

                // create a state for any sysid/compid includes gcs on log playback
                if (!MAVlist.Contains(sysid, compid))
                {
                    // create an item - hidden
                    MAVlist.AddHiddenList(sysid, compid);
                    // prevent packetloss counter on connect
                    MAVlist[sysid, compid].recvpacketcount = unchecked(packetSeqNo - (byte)1);
                }

                // once set it cannot be reverted
                if (!MAVlist[sysid, compid].mavlinkv2)
                    MAVlist[sysid, compid].mavlinkv2 = message.buffer[0] == MAVLINK_STX ? true : false;

                // stat count
                if (message.buffer[0] == MAVLINK_STX)
                    _mavlink2count++;
                else if (message.buffer[0] == MAVLINK_STX_MAVLINK1)
                    _mavlink1count++;

                //check if sig was included in packet, and we are not ignoring the signature (signing isnt checked else we wont enable signing)
                //logreadmode we always ignore signing as they would not be in the log if they failed the signature
                if (message.sig != null && !MAVlist[sysid, compid].signingignore && !logreadmode)
                {
                    _mavlink2signed++;

                    bool valid = false;

                    // check existing key
                    if (MAVlist[sysid, compid].signingKey != null)
                        valid = CheckSignature(MAVlist[sysid, compid].signingKey, message, sysid, compid);

                    if (!valid)
                    {
                        foreach (var AuthKey in MAVAuthKeys.Keys.Values)
                        {
                            valid = CheckSignature(AuthKey.Key, message, sysid, compid);
                            if (valid)
                                break;
                        }

                        if (!valid)
                        {
                            log.InfoFormat("Packet failed signature but passed crc");
                            return MAVLinkMessage.Invalid;
                        }
                    }
                }

                // packet is now verified

                // extract wp's/rally/fence/camera feedback/params from stream, including gcs packets on playback
                if (buffer.Length >= 5)
                {
                    getInfoFromStream(ref message, sysid, compid);
                }

                // if its a gcs packet - dont process further
                if (buffer.Length >= 5 && (sysid == gcssysid || sysid == 253) && logreadmode) // gcs packet
                {
                    return message;
                }

                // update packet loss statistics
                if (!logreadmode && MAVlist[sysid, compid].packetlosttimer.AddSeconds(5) < DateTime.UtcNow)
                {
                    MAVlist[sysid, compid].packetlosttimer = DateTime.UtcNow;
                    MAVlist[sysid, compid].packetslost = (MAVlist[sysid, compid].packetslost * 0.8f);
                    MAVlist[sysid, compid].packetsnotlost = (MAVlist[sysid, compid].packetsnotlost * 0.8f);
                }
                else if (logreadmode && MAVlist[sysid, compid].packetlosttimer.AddSeconds(5) < lastlogread)
                {
                    MAVlist[sysid, compid].packetlosttimer = lastlogread.ToUniversalTime();
                    MAVlist[sysid, compid].packetslost = (MAVlist[sysid, compid].packetslost * 0.8f);
                    MAVlist[sysid, compid].packetsnotlost = (MAVlist[sysid, compid].packetsnotlost * 0.8f);
                }

                packetSeemValid = false;
                //The following IF had to be splitted to accomodate the new place of Readlock end.
                //packetSeemValid helps it.
                if ((message.header == 'U' || message.header == MAVLINK_STX_MAVLINK1 ||
                     message.header == MAVLINK_STX) &&
                    buffer.Length >= message.payloadlength)
                {
                    packetSeemValid = true;
                    // check if we lost pacakets based on seqno
                    int expectedPacketSeqNo = ((MAVlist[sysid, compid].recvpacketcount + 1) % 0x100);

                    {
                        // the second part is to work around a 3dr radio bug sending dup seqno's
                        if (packetSeqNo != expectedPacketSeqNo &&
                            packetSeqNo != MAVlist[sysid, compid].recvpacketcount)
                        {
                            MAVlist[sysid, compid].synclost++; // actual sync loss's
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

                            if (!logreadmode)
                                log.InfoFormat("mav {2}-{4} seqno {0} exp {3} pkts lost {1}", packetSeqNo,
                                    numLost,
                                    sysid,
                                    expectedPacketSeqNo, compid);
                        }

                        MAVlist[sysid, compid].packetsnotlost++;

                        //Console.WriteLine("{0} {1}", sysid, packetSeqNo);

                        MAVlist[sysid, compid].recvpacketcount = packetSeqNo;
                    }
                    WhenPacketReceived.OnNext(1);

                    // packet stats per mav
                    if (!MAVlist[sysid, compid].packetspersecond.ContainsKey(msgid) ||
                        double.IsInfinity(MAVlist[sysid, compid].packetspersecond[msgid]))
                        MAVlist[sysid, compid].packetspersecond[msgid] = 0;
                    if (!MAVlist[sysid, compid].packetspersecondbuild.ContainsKey(msgid))
                        MAVlist[sysid, compid].packetspersecondbuild[msgid] = DateTime.UtcNow;

                    MAVlist[sysid, compid].packetspersecond[msgid] = (((1000 /
                                                                        ((DateTime.UtcNow -
                                                                          MAVlist[sysid, compid]
                                                                              .packetspersecondbuild[msgid])
                                                                            .TotalMilliseconds) +
                                                                        MAVlist[sysid, compid].packetspersecond[
                                                                            msgid]) / 2));

                    MAVlist[sysid, compid].packetspersecondbuild[msgid] = DateTime.UtcNow;
                }
                //Console.WriteLine("Packet {0}",temp[5]);
                // store packet history


                //ReadLock Release moved here to include packet seqence checking.
                //Releasing the lock earlier casued packet sequence errors, when readPacketAsync() was called repeatedly.
                //Especially during retries.

            } // end readlock
            finally
            {
                readlock.Release();
            }

            try
            {
                //The packet header checking IF had to be splitted to allow moving the Readlock try/catch.
                if (packetSeemValid)
                {
                    MAVlist[sysid, compid].addPacket(message);

                    // adsb packets are forwarded and can be from any sysid/compid
                    if (msgid == (byte)MAVLINK_MSG_ID.ADSB_VEHICLE)
                    {
                        var adsb = message.ToStructure<mavlink_adsb_vehicle_t>();

                        var id = adsb.ICAO_address.ToString("X5");

                        if (_UpdateADSBPlanePosition != null)
                            _UpdateADSBPlanePosition(this, new adsb.PointLatLngAltHdg(
                                    adsb.lat / 1e7,
                                    adsb.lon / 1e7,
                                    adsb.altitude / 1000,
                                    adsb.heading * 0.01f,
                                    adsb.hor_velocity,
                                    id,
                                    DateTime.Now
                            )
                            {
                                CallSign = Encoding.UTF8.GetString(adsb.callsign),
                                Squawk = adsb.squawk,
                                Raw = adsb,
                                VerticalSpeed = adsb.ver_velocity,
                            }
                            );
                    }

                    if (msgid == (byte)MAVLINK_MSG_ID.COLLISION)
                    {
                        var coll = message.ToStructure<mavlink_collision_t>();

                        var id = coll.id.ToString("X5");

                        var coll_type = (MAV_COLLISION_SRC)coll.src;

                        var action = (MAV_COLLISION_ACTION)coll.action;

                        if (action > MAV_COLLISION_ACTION.REPORT)
                        {
                            // we are reacting to a threat
                        }

                        var threat_level = (MAV_COLLISION_THREAT_LEVEL)coll.threat_level;

                        _UpdateADSBCollision?.Invoke(this, (id, threat_level));
                    }

                    // set seens sysid's based on hb packet - this will hide 3dr radio packets
                    if (msgid == (uint)MAVLINK_MSG_ID.UAVCAN_NODE_STATUS)
                    {
                        var cannode = message.ToStructure<mavlink_uavcan_node_status_t>();

                        // add a seen sysid
                        if (!MAVlist.Contains(sysid, compid, false))
                        {
                            // ensure its set from connect or log playback
                            MAVlist.Create(sysid, compid);
                            MAVlist[sysid, compid].aptype = MAV_TYPE.ONBOARD_CONTROLLER;
                            MAVlist[sysid, compid].apname = MAV_AUTOPILOT.INVALID;
                            MAVlist[sysid, compid].CANNode = true;
                            setAPType(sysid, compid);

                            // new device, so request node info
                            doCommand(sysid, compid, MAV_CMD.UAVCAN_GET_NODE_INFO, 0, 0, 0, 0, 0, 0, 0, false);
                        }
                    }

                    if (msgid == (uint)MAVLINK_MSG_ID.UAVCAN_NODE_INFO)
                    {
                        var cannode = message.ToStructure<mavlink_uavcan_node_info_t>();

                        var name = Encoding.UTF8.GetString(cannode.name);

                        MAVlist[sysid, compid].VersionString = name;

                        MAVlist[sysid, compid].SoftwareVersions =
                            cannode.sw_version_major + "." + cannode.sw_version_minor;
                    }

                    // set seens sysid's based on hb packet - this will hide 3dr radio packets ( which send a RADIO_STATUS, but not a HEARTBEAT )
                    if (msgid == (byte)MAVLINK_MSG_ID.HEARTBEAT)
                    {
                        mavlink_heartbeat_t hb = message.ToStructure<mavlink_heartbeat_t>();

                        // not a gcs
                        //if (hb.type != (byte)MAV_TYPE.GCS)
                        {
                            // add a seen sysid
                            if (!MAVlist.Contains(sysid, compid, false))
                            {
                                // ensure its set from connect or log playback
                                MAVlist.Create(sysid, compid);
                                MAVlist[sysid, compid].aptype = (MAV_TYPE)hb.type;
                                MAVlist[sysid, compid].apname = (MAV_AUTOPILOT)hb.autopilot;
                                setAPType(sysid, compid);
                                _MAVDetected?.Invoke(this, (sysid, compid));
                            }

                            // attach to the only remote device. / default to first device seen
                            if (MAVlist.Count == 1)
                            {
                                // set it private as compidset will trigger new mavstate
                                _sysidcurrent = sysid;
                                compidcurrent = compid;
                            }
                        }
                    }

                    //treat a high latency packet as a HB
                    if (msgid == (byte)MAVLINK_MSG_ID.HIGH_LATENCY2)
                    {
                        mavlink_high_latency2_t hl = message.ToStructure<mavlink_high_latency2_t>();

                        // not a gcs
                        //if (hb.type != (byte)MAV_TYPE.GCS)
                        {
                            // add a seen sysid
                            if (!MAVlist.Contains(sysid, compid, false))
                            {
                                // ensure its set from connect or log playback
                                MAVlist.Create(sysid, compid);
                                MAVlist[sysid, compid].aptype = (MAV_TYPE)hl.type;
                                MAVlist[sysid, compid].apname = (MAV_AUTOPILOT)hl.autopilot;
                                MAVlist[sysid, compid].isHighLatency = true;
                                setAPType(sysid, compid);
                                _MAVDetected?.Invoke(this, (sysid, compid));
                            }

                            // attach to the only remote device. / default to first device seen
                            if (MAVlist.Count == 1)
                            {
                                // set it private as compidset will trigger new mavstate
                                _sysidcurrent = sysid;
                                compidcurrent = compid;
                            }
                        }
                    }

                    try
                    {
                        // this is to ensure the log is in packet order, as the events on Received may send a packet. (ie mavftp)
                        SaveToTlog(new Span<byte>(buffer));

                        if (logfile != null)
                        {
                            lock (logfile)
                            {
                                if (msgid == 0)
                                {
                                    // flush on heartbeat - 1 seconds
                                    if (logfile != null)
                                        logfile.Flush();
                                    if (rawlogfile != null)
                                        rawlogfile.Flush();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    // process for all mavs, filtering done inside - subscription handler
                    PacketReceived(message);

                    _OnPacketReceived?.Invoke(this, message);

                    if (debugmavlink)
                        DebugPacket(message);

                    if (msgid == (byte)MAVLINK_MSG_ID.STATUSTEXT) // status text
                    {
                        var msg = message.ToStructure<mavlink_statustext_t>();

                        byte sev = msg.severity;

                        string logdata = Encoding.UTF8.GetString(msg.text);
                        int ind = logdata.IndexOf('\0');
                        if (ind != -1)
                            logdata = logdata.Substring(0, ind);
                        log.Info(DateTime.Now + " " + sev + " " + logdata);

                        MAVlist[sysid, compid].cs.messages.Add((DateTime.Now, logdata));

                        // cap list at 1000 element
                        while (MAVlist[sysid, compid].cs.messages.Count > 1000)
                            MAVlist[sysid, compid].cs.messages.RemoveAt(0);

                        // gymbals etc are a child/slave to the main sysid, this displays the children messages under the current displayed vehicle
                        if (sysid == sysidcurrent && compid != compidcurrent)
                            MAVlist[sysidcurrent, compidcurrent].cs.messages
                                .Add((DateTime.Now, compid + " : " + logdata));

                        bool printit = false;

                        MAV_SEVERITY mavsev = MAV_SEVERITY.EMERGENCY;
                        if (sev <= (byte)Settings.Instance.GetInt32("severity", 4))
                        {
                            printit = true;
                            mavsev = (MAV_SEVERITY)sev;
                        }

                        if (logdata.StartsWith("Tuning:") || logdata.StartsWith("PreArm:") ||
                            logdata.StartsWith("Arm:"))
                            printit = true;

                        if (printit)
                        {
                            // high severity msg publish to current selected mav
                            if (sysid == sysidcurrent && compid != compidcurrent)
                            {
                                MAVlist[sysid, compidcurrent].cs.messageHigh = compid + " : " + logdata;
                                MAVlist[sysid, compidcurrent].cs.messageHighSeverity = mavsev;
                            }

                            MAVlist[sysid, compid].cs.messageHigh = logdata;
                            MAVlist[sysid, compid].cs.messageHighSeverity = mavsev;

                            if (Speech != null &&
                                Speech.IsReady &&
                                Settings.Instance["speechenable"] != null &&
                                Settings.Instance["speechenable"].ToString() == "True")
                            {
                                if (speechenabled && !logdata.StartsWith("PreArm:")) // Suppress audible PreArm messages
                                    Speech.SpeakAsync(logdata);
                            }
                        }
                    }

                    if (Settings.Instance["autoParamCommit"] == null ||
                        Settings.Instance.GetBoolean("autoParamCommit") == true)
                    {
                        if (lastparamset != DateTime.MinValue && lastparamset.AddSeconds(10) < DateTime.UtcNow)
                        {
                            lastparamset = DateTime.MinValue;

                            if (BaseStream.IsOpen)
                            {
                                doCommand((byte)sysidcurrent, (byte)compidcurrent, MAV_CMD.PREFLIGHT_STORAGE, 1, 0, 0,
                                    0, 0, 0, 0,
                                    false);
                            }
                        }
                    }

                    ProcessMirrorStream(buffer);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            // update last valid packet receive time
            MAVlist[sysid, compid].lastvalidpacket = DateTime.UtcNow;

            return message;
        }

        private void ProcessMirrorStream(byte[] buffer)
        {
            try
            {
                // full rw from mirror stream
                if (MirrorStream != null && MirrorStream.IsOpen)
                {
                    MirrorStream.Write(buffer, 0, buffer.Length);

                    while (MirrorStream.BytesToRead > 0)
                    {
                        var len = MirrorStream.BytesToRead;

                        byte[] buf = new byte[len];

                        len = MirrorStream.Read(buf, 0, len);

                        if (MirrorStreamWrite)
                            lock (writelock)
                            {
                                BaseStream.Write(buf, 0, len);

                                if (rawlogfile != null && rawlogfile.CanWrite)
                                    rawlogfile.Write(buf, 0, len);
                            }
                    }
                }
            }
            catch
            {
            }
        }

        private bool CheckSignature(byte[] AuthKey, MAVLinkMessage message, byte sysid, byte compid)
        {
            bool valid;
            using (SHA256CryptoServiceProvider signit = new SHA256CryptoServiceProvider())
            {
                signit.TransformBlock(AuthKey, 0, AuthKey.Length, null, 0);
                signit.TransformFinalBlock(message.buffer, 0, message.Length - MAVLINK_SIGNATURE_BLOCK_LEN + 7);
                var ctx = signit.Hash;

                //Console.WriteLine("rec linkid {0}, time {1} {2} {3} {4} {5} {6} {7}", message.sig[0], message.sig[1], message.sig[2], message.sig[3], message.sig[4], message.sig[5], message.sig[6], message.sigTimestamp);

                valid = ByteArrayCompare(new ReadOnlySpan<byte>(ctx, 0, 6), new ReadOnlySpan<byte>(message.sig, 7, 6));

                if (!valid)
                    return valid;

                // got valid key
                MAVlist[sysid, compid].linkid = message.sig[0];

                MAVlist[sysid, compid].signingKey = AuthKey;

                enableSigning(sysid, compid);

                return valid;
            }
        }

        private void PacketReceived(MAVLinkMessage buffer)
        {
            MAVLINK_MSG_ID type = (MAVLINK_MSG_ID) buffer.msgid;
            (MAVLINK_MSG_ID msgId, Func<MAVLinkMessage, bool> function, bool exclusive, byte sysid, byte compid)[] list;

            // lock to grab the list, because this is public, to prevent a recursion
            lock (Subscriptions)
            {
                list = Subscriptions.ToArray();
            }

            foreach (var item in list)
            {
                //exact match, and 0,0 = current primary
                if (item.msgId == type && buffer.sysid == item.sysid && buffer.compid == item.compid ||
                    item.msgId == type && item.sysid == 0 && item.compid == 0 && buffer.sysid == sysidcurrent && buffer.compid == compidcurrent)
                {
                    try
                    {
                        item.function(buffer);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
        }

        readonly private List<(MAVLINK_MSG_ID msgId, Func<MAVLinkMessage, bool> function, bool exclusive, byte sysid, byte compid)> Subscriptions =
            new List<(MAVLINK_MSG_ID, Func<MAVLinkMessage, bool>, bool, byte, byte)>();

        /// <summary>
        /// Subscribe to a packet on the current target MAV. use OnPacketReceived to get all MAVs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="function"></param>
        /// <param name="exclusive"></param>
        /// <returns></returns>
        public int SubscribeToPacketType(MAVLINK_MSG_ID msgid,
            Func<MAVLinkMessage, bool> function, byte sysid, byte compid, bool exclusive = false)
        {
            log.Info($"SubscribeToPacketType {msgid} {function} {exclusive} {sysid} {compid}");

            var item = (msgid, function, exclusive, sysid, compid);

            lock (Subscriptions)
            {
                if (exclusive)
                {
                    foreach (var subitem in Subscriptions)
                    {
                        if (subitem.msgId == item.msgid)
                        {
                            Subscriptions.Remove(subitem);
                            break;
                        }
                    }
                }

                log.Debug("SubscribeToPacketType " + item.msgid + " " + item.function);

                Subscriptions.Add(item);
            }

            return item.GetHashCode();
        }

        public void UnSubscribeToPacketType(int id)
        {
            lock (Subscriptions)
            {
                log.Debug("UnSubscribeToPacketType " + id);
                var sub = Subscriptions.ToArray();

                foreach (var valueTuple in sub)
                {
                    if (id == valueTuple.GetHashCode())
                    {
                        Subscriptions.Remove(valueTuple);
                        break;
                    }
                }
            }
        }



        /// <summary>
        /// Used to extract mission from log file - both sent or received
        /// </summary>
        /// <param name="buffer">packet</param>
        private void getInfoFromStream(ref MAVLinkMessage buffer, byte sysid, byte compid)
        {
            if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_COUNT)
            {
                // clear old
                mavlink_mission_count_t wp = buffer.ToStructure<mavlink_mission_count_t>();

                if (wp.target_system == gcssysid)
                {
                    wp.target_system = sysid;
                    wp.target_component = compid;
                }

                if (wp.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                    MAVlist[wp.target_system, wp.target_component].wps.Clear();
                if (wp.mission_type == (byte) MAV_MISSION_TYPE.FENCE)
                    MAVlist[wp.target_system, wp.target_component].fencepoints.Clear();
                if (wp.mission_type == (byte) MAV_MISSION_TYPE.RALLY)
                    MAVlist[wp.target_system, wp.target_component].rallypoints.Clear();
            }
            else if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_ITEM)
            {
                mavlink_mission_item_t wp = buffer.ToStructure<mavlink_mission_item_t>();

                if (wp.target_system == gcssysid)
                {
                    wp.target_system = sysid;
                    wp.target_component = compid;
                }

                if (wp.current == 2)
                {
                    // guide mode wp
                    if (wp.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                        MAVlist[wp.target_system, wp.target_component].GuidedMode = (Locationwp) wp;
                }
                else
                {
                    if (wp.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                        MAVlist[wp.target_system, wp.target_component].wps[wp.seq] = (Locationwp) wp;
                    if (wp.mission_type == (byte) MAV_MISSION_TYPE.FENCE)
                        MAVlist[wp.target_system, wp.target_component].fencepoints[wp.seq] = (Locationwp) wp;
                    if (wp.mission_type == (byte) MAV_MISSION_TYPE.RALLY)
                        MAVlist[wp.target_system, wp.target_component].rallypoints[wp.seq] = (Locationwp) wp;
                }

                //Console.WriteLine("WP # {7} cmd {8} p1 {0} p2 {1} p3 {2} p4 {3} x {4} y {5} z {6}", wp.param1, wp.param2, wp.param3, wp.param4, wp.x, wp.y, wp.z, wp.seq, wp.command);
            }
            else if (buffer.msgid == (byte) MAVLINK_MSG_ID.MISSION_ITEM_INT)
            {
                mavlink_mission_item_int_t wp = buffer.ToStructure<mavlink_mission_item_int_t>();

                if (wp.target_system == gcssysid)
                {
                    wp.target_system = sysid;
                    wp.target_component = compid;
                }

                if (wp.current == 2)
                {
                    // guide mode wp
                    if (wp.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                        MAVlist[wp.target_system, wp.target_component].GuidedMode = (Locationwp) wp;
                }
                else
                {
                    if (wp.mission_type == (byte) MAV_MISSION_TYPE.MISSION)
                        MAVlist[wp.target_system, wp.target_component].wps[wp.seq] = wp;
                    if (wp.mission_type == (byte) MAV_MISSION_TYPE.FENCE)
                        MAVlist[wp.target_system, wp.target_component].fencepoints[wp.seq] = wp;
                    if (wp.mission_type == (byte) MAV_MISSION_TYPE.RALLY)
                        MAVlist[wp.target_system, wp.target_component].rallypoints[wp.seq] = wp;
                }

                //Console.WriteLine("WP INT # {7} cmd {8} p1 {0} p2 {1} p3 {2} p4 {3} x {4} y {5} z {6}", wp.param1, wp.param2, wp.param3, wp.param4, wp.x, wp.y, wp.z, wp.seq, wp.command);
            }
            else if (buffer.msgid == (byte) MAVLINK_MSG_ID.HOME_POSITION)
            {
                var home = buffer.ToStructure<mavlink_home_position_t>();

                var loc = new PointLatLngAlt(home.latitude / 1.0e7, home.longitude / 1.0e7, home.altitude / 1000.0);

                MAVlist[buffer.sysid, buffer.compid].cs.HomeLocation = loc;
            }
            else if (buffer.msgid == (byte) MAVLINK_MSG_ID.SET_POSITION_TARGET_GLOBAL_INT)
            {
                mavlink_set_position_target_global_int_t setpos =
                    buffer.ToStructure<mavlink_set_position_target_global_int_t>();

                MAVlist[setpos.target_system, setpos.target_component].GuidedMode = (Locationwp) setpos;

                //Console.WriteLine("SET_POSITION_TARGET_GLOBAL_INT x {0} y {1} z {2} frame {3}", setpos.lat_int/1e7, setpos.lon_int/1e7, setpos.alt, setpos.coordinate_frame);
            }
            else if (buffer.msgid == (byte) MAVLINK_MSG_ID.RALLY_POINT)
            {
                mavlink_rally_point_t rallypt = buffer.ToStructure<mavlink_rally_point_t>();

                if (rallypt.target_system == gcssysid)
                {
                    rallypt.target_system = sysid;
                    rallypt.target_component = compid;
                }

                MAVlist[rallypt.target_system, rallypt.target_component].rallypoints[rallypt.idx] =
                    new mavlink_mission_item_int_t(0, 0, 0, 0, rallypt.lat, rallypt.lng, rallypt.alt, rallypt.idx,
                        (ushort) MAV_CMD.RALLY_POINT, rallypt.target_system, rallypt.target_component,
                        (byte) MAV_FRAME.GLOBAL_RELATIVE_ALT, 0, 0, (byte) MAV_MISSION_TYPE.RALLY);

                //Console.WriteLine("RP # {0} {1} {2} {3} {4}", rallypt.idx, rallypt.lat, rallypt.lng, rallypt.alt, rallypt.break_alt);
            }
            else if (buffer.msgid == (byte) MAVLINK_MSG_ID.CAMERA_FEEDBACK)
            {
                mavlink_camera_feedback_t camerapt = buffer.ToStructure<mavlink_camera_feedback_t>();

                if (MAVlist[sysid, compid].camerapoints.Count == 0 ||
                    MAVlist[sysid, compid].camerapoints.Last().time_usec != camerapt.time_usec)
                {
                    MAVlist[sysid, compid].camerapoints.RemoveAll(a =>
                        a.cam_idx * 256 + a.img_idx == camerapt.cam_idx * 256 + camerapt.img_idx);
                    MAVlist[sysid, compid].camerapoints.Add(camerapt);
                }
            }
            else if (buffer.msgid == (byte) MAVLINK_MSG_ID.FENCE_POINT)
            {
                mavlink_fence_point_t fencept = buffer.ToStructure<mavlink_fence_point_t>();

                if (fencept.target_system == gcssysid)
                {
                    fencept.target_system = sysid;
                    fencept.target_component = compid;
                }

                MAVlist[fencept.target_system, fencept.target_component].fencepoints[fencept.idx] =
                    new mavlink_mission_item_int_t(fencept.count - 1, 0, 0, 0, (int) (fencept.lat * 1e7),
                        (int) (fencept.lng * 1e7), 0, fencept.idx,
                        fencept.idx == 0
                            ? (ushort) MAV_CMD.FENCE_RETURN_POINT
                            : (ushort) MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION,
                        fencept.target_system, fencept.target_component, (byte) MAV_FRAME.GLOBAL_RELATIVE_ALT, 0, 0,
                        (byte) MAV_MISSION_TYPE.FENCE);
            }
            else if (buffer.msgid == (byte) MAVLINK_MSG_ID.PARAM_VALUE)
            {
                mavlink_param_value_t value = buffer.ToStructure<mavlink_param_value_t>();

                string st = Encoding.UTF8.GetString(value.param_id);

                int pos = st.IndexOf('\0');

                if (pos != -1)
                {
                    st = st.Substring(0, pos);
                }

                lock(MAVlist[sysid, compid].param_types)
                    MAVlist[sysid, compid].param_types[st] = (MAV_PARAM_TYPE) value.param_type;

                if (MAVlist[sysid, compid].apname == MAV_AUTOPILOT.ARDUPILOTMEGA &&
                    buffer.compid != (byte) MAV_COMPONENT.MAV_COMP_ID_UDP_BRIDGE)
                {
                    var offset = Marshal.OffsetOf(typeof(mavlink_param_value_t), "param_value");
                    MAVlist[sysid, compid].param[st] = new MAVLinkParam(st, BitConverter.GetBytes(value.param_value),
                        MAV_PARAM_TYPE.REAL32, (MAV_PARAM_TYPE) value.param_type);
                }
                else
                {
                    var offset = Marshal.OffsetOf(typeof(mavlink_param_value_t), "param_value");
                    MAVlist[sysid, compid].param[st] = new MAVLinkParam(st, BitConverter.GetBytes(value.param_value),
                        (MAV_PARAM_TYPE) value.param_type, (MAV_PARAM_TYPE) value.param_type);
                }

                if (value.param_count != ushort.MaxValue)
                    MAVlist[sysid, compid].param.TotalReported = value.param_count;
            }
            else if (buffer.msgid == (byte) MAVLINK_MSG_ID.TIMESYNC)
            {
                Int64 now_ns =
                    (Int64) ((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds *
                             1000000);

                mavlink_timesync_t tsync = buffer.ToStructure<mavlink_timesync_t>();
                // tc1 - client
                // ts1 - server

                // system does not know the time
                if (tsync.tc1 == 0)
                {
                    tsync.tc1 = now_ns;
                    if (BaseStream != null && BaseStream.IsOpen)
                        sendPacket(tsync, buffer.sysid, buffer.compid);
                } // system knows the time
                else if (tsync.tc1 > 0)
                {
                    Int64 offset_ns = (tsync.ts1 + now_ns - tsync.tc1 * 2) / 2;
                    Int64 dt = MAVlist[buffer.sysid, buffer.compid].time_offset_ns - offset_ns;

                    if (Math.Abs(dt) > 10000000) // 10 millisecond skew
                    {
                        MAVlist[buffer.sysid, buffer.compid].time_offset_ns = offset_ns; // hard-set it.
                    }
                    else
                    {
                        var offset_avg_alpha = 0.6;
                        var avg = (offset_avg_alpha * offset_ns) +
                                  (1.0 - offset_avg_alpha) * MAVlist[buffer.sysid, buffer.compid].time_offset_ns;
                        MAVlist[buffer.sysid, buffer.compid].time_offset_ns = (long) avg;
                    }
                }
            }
        }

        [Obsolete]
        public bool getVersion(bool responcerequired = true)
        {
            return getVersion(MAV.sysid, MAV.compid, responcerequired);
        }

        public bool getVersion(byte sysid, byte compid, bool responcerequired = true)
        {
            return getVersionAsync(sysid, compid, responcerequired).AwaitSync();
        }

        public async Task<bool> getVersionAsync(byte sysid, byte compid, bool responcerequired = true)
        {
            mavlink_autopilot_version_request_t req = new mavlink_autopilot_version_request_t();

            req.target_component = compid;
            req.target_system = sysid;

            // use *all three* methods
            doCommand(MAVLink.MAV_CMD.REQUEST_MESSAGE,
                (float)MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION,
                0, 0, 0, 0, 0, 0, false);

	    // MAV_CMD.REQUEST_AUTOPILOT_CAPABILITIES is deprecated
            doCommand(MAV_CMD.REQUEST_AUTOPILOT_CAPABILITIES, 0, 0, 0, 0, 0, 0, 0, false);

	    // AUTOPILOT_VERSION_REQUEST is deprecated
            generatePacket((byte) MAVLINK_MSG_ID.AUTOPILOT_VERSION_REQUEST, req);

            if (!responcerequired)
                return true;

            var responce = false;

            var sub = SubscribeToPacketType(MAVLINK_MSG_ID.AUTOPILOT_VERSION, (buffer) =>
            {
                if (buffer.msgid == (byte)MAVLINK_MSG_ID.AUTOPILOT_VERSION && buffer.sysid == req.target_system &&
                    buffer.compid == req.target_component)
                {
                    responce = true;
                    return true;
                }
                return true;
            }, (byte)sysid, (byte)compid);

            DateTime start = DateTime.Now;
            int retrys = 3;

            while (!responce)
            {
                if (!(start.AddMilliseconds(500) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        log.Info("getVersion Retry " + retrys);
                        generatePacket((byte)MAVLINK_MSG_ID.AUTOPILOT_VERSION_REQUEST, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }

                    break;
                }

                await Task.Delay((100));
            }

            UnSubscribeToPacketType(sub);

            return responce;
        }

        [Obsolete]
        public async Task<(PointLatLngAlt plla, int total)> getFencePoint(int no)
        {
            MAVLinkMessage buffer;

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
                    throw new TimeoutException("Timeout on read - getFencePoint");
                }

                buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.FENCE_POINT && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        mavlink_fence_point_t fp = buffer.ToStructure<mavlink_fence_point_t>();

                        // check this gcs sent it
                        if (fp.target_system != gcssysid ||
                            fp.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER ||
                            fp.idx != no)
                            continue;

                        giveComport = false;

                        plla.Lat = fp.lat;
                        plla.Lng = fp.lng;
                        plla.Tag = fp.idx.ToString();

                        return (plla, fp.count);
                    }
                }
            }
        }

        [Obsolete]
        public async Task<FileStream> GetLog(ushort no)
        {
            return new FileStream(await GetLog(MAV.sysid, MAV.compid, no), FileMode.Open, FileAccess.ReadWrite);
        }

        public async Task<string> GetLog(byte sysid, byte compid, ushort no)
        {
            var filename = Path.GetTempFileName();
            using (FileStream ms = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                Hashtable set = new Hashtable();

                giveComport = true;
                MAVLinkMessage buffer;

                if (Progress != null)
                {
                    Progress((int) 0, "");
                }

                uint totallength = 0;
                uint ofs = 0;
                uint bps = 0;
                DateTime bpstimer = DateTime.Now;

                mavlink_log_request_data_t req = new mavlink_log_request_data_t();

                req.target_component = compid;
                req.target_system = sysid;
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
                    if (!(start.AddMilliseconds(3000) > DateTime.Now))
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
                        throw new TimeoutException("Timeout on read - GetLog");
                    }

                    var start1 = DateTime.Now;
                    buffer = await readPacketAsync().ConfigureAwait(false);
                    var end = DateTime.Now - start1;
                    var lapse = end.TotalMilliseconds;
                    //Console.WriteLine("readPacketAsync: " + lapse);
                    if (buffer.Length > 5)
                    {
                        if (buffer.msgid == (byte) MAVLINK_MSG_ID.LOG_DATA && buffer.sysid == req.target_system &&
                            buffer.compid == req.target_component)
                        {
                            var data = buffer.ToStructure<mavlink_log_data_t>();

                            if (data.id != no)
                                continue;

                            // reset retrys
                            retrys = 3;
                            start = DateTime.Now;

                            bps += data.count;

                            // record what we have received
                            set[(data.ofs / 90).ToString()] = 1;

                            if (ms.Position != data.ofs)
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
                log.Info("count total " + ((totallength) / 90 + 1));
                log.Info("totallength " + totallength);
                log.Info("current length " + ms.Length);

                while (true && ((BaseStream != null && BaseStream.IsOpen) || logreadmode))
                {
                    if (totallength == ms.Length && set.Count >= ((totallength) / 90 + 1))
                    {
                        giveComport = false;
                        return filename;
                    }

                    if (!(start.AddMilliseconds(500) > DateTime.Now))
                    {
                        for (int a = 0; a < ((totallength) / 90 + 1); a++)
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

                                req.ofs = (uint) (a * 90);
                                req.count = bytereq;
                                log.Info("req missing " + req.ofs + " bytes " + req.count + " got " + set.Count + "/" +
                                         ((totallength) / 90 + 1));
                                generatePacket((byte) MAVLINK_MSG_ID.LOG_REQUEST_DATA, req);
                                start = DateTime.Now;
                                break;
                            }
                        }
                    }

                    buffer = await readPacketAsync().ConfigureAwait(false);
                    if (buffer.Length > 5)
                    {
                        if (buffer.msgid == (byte) MAVLINK_MSG_ID.LOG_DATA && buffer.sysid == req.target_system &&
                            buffer.compid == req.target_component)
                        {
                            var data = buffer.ToStructure<mavlink_log_data_t>();

                            if (data.id != no)
                                continue;

                            // reset retrys
                            retrys = 3;
                            start = DateTime.Now;

                            bps += data.count;

                            // record what we have received
                            set[(data.ofs / 90).ToString()] = 1;

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

                            // check if we have next set and invalidate to request next packets
                            if (set.ContainsKey(((data.ofs / 90) + 1).ToString()))
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

                throw new Exception("Failed to get log");
            }
        }

        [Obsolete]
        public List<mavlink_log_entry_t> GetLogList()
        {
            List<mavlink_log_entry_t> ans = new List<mavlink_log_entry_t>();

            return GetLogEntry(0, ushort.MaxValue).AwaitSync().Values
                .OrderBy(a => a.id).ToList();
        }

        [Obsolete]
        public void GetMountStatus()
        {
            mavlink_mount_status_t req = new mavlink_mount_status_t();
            req.target_component = MAV.compid;
            req.target_system = MAV.sysid;

            generatePacket((byte) MAVLINK_MSG_ID.MOUNT_STATUS, req);
        }

        [Obsolete]
        public async Task<Dictionary<ushort, mavlink_log_entry_t>> GetLogEntry(ushort startno = 0,
            ushort endno = ushort.MaxValue)
        {
            giveComport = true;
            MAVLinkMessage buffer;

            Dictionary<ushort, mavlink_log_entry_t> ans = new Dictionary<ushort, mavlink_log_entry_t>();

            mavlink_log_request_list_t req = new mavlink_log_request_list_t();

            req.target_component = MAV.compid;
            req.target_system = MAV.sysid;
            req.start = startno;
            req.end = endno;

            log.Info("GetLogEntry " + startno + "-" + endno);

            // request point
            generatePacket((byte) MAVLINK_MSG_ID.LOG_REQUEST_LIST, req);

            DateTime start = DateTime.Now;
            int retrys = 4;

            while (true)
            {
                if (!(start.AddMilliseconds(5000) > DateTime.Now))
                {
                    if (retrys > 0)
                    {
                        req.start = startno;
                        req.end = endno;
                        log.Info("GetLogEntry Retry " + retrys + " - giv com " + giveComport);
                        generatePacket((byte) MAVLINK_MSG_ID.LOG_REQUEST_LIST, req);
                        start = DateTime.Now;
                        retrys--;
                        continue;
                    }

                    giveComport = false;
                    throw new TimeoutException("Timeout on read - GetLogEntry");
                }

                buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.LOG_ENTRY && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        var loge = buffer.ToStructure<mavlink_log_entry_t>();

                        if (loge.id >= startno && loge.id <= endno)
                        {
                            if (loge.num_logs == 0 && loge.last_log_num == 0)
                            {
                                giveComport = false;
                                return ans;
                            }

                            // reset timeout
                            start = DateTime.Now;
                            // add the log to our answer
                            ans[loge.id] = loge;
                            // set the startno to our new min
                            startno = (ushort) Math.Min(ans.Keys.Min() + 1, loge.id);
                            // set the end number to logmax
                            endno = loge.last_log_num;
                            if (ans.Count >= loge.num_logs)
                            {
                                giveComport = false;
                                return ans;
                            }
                        }
                    }

                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.LOG_DATA && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        giveComport = false;
                        throw new Exception("Existing log download already in progress.");
                    }
                }
            }
        }

        [Obsolete]
        public void EraseLog()
        {
            mavlink_log_erase_t req = new mavlink_log_erase_t();

            req.target_component = MAV.compid;
            req.target_system = MAV.sysid;

            // send twice - we have no feedback on this
            generatePacket((byte) MAVLINK_MSG_ID.LOG_ERASE, req);
            generatePacket((byte) MAVLINK_MSG_ID.LOG_ERASE, req);
        }

        [Obsolete]
        public async Task<List<PointLatLngAlt>> getRallyPoints()
        {
            List<PointLatLngAlt> points = new List<PointLatLngAlt>();

            if (!MAV.param.ContainsKey("RALLY_TOTAL"))
                return points;

            int count = int.Parse(MAV.param["RALLY_TOTAL"].ToString());

            for (int a = 0; a < (count - 1); a++)
            {
                try
                {
                    var plla = await getRallyPoint(a).ConfigureAwait(false);
                    points.Add(plla.plla);
                }
                catch
                {
                    return points;
                }
            }

            return points;
        }

        [Obsolete]
        public async Task<(PointLatLngAlt plla, int total)> getRallyPoint(int no)
        {
            MAVLinkMessage buffer;

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
                    throw new TimeoutException("Timeout on read - getRallyPoint");
                }

                buffer = await readPacketAsync().ConfigureAwait(false);
                if (buffer.Length > 5)
                {
                    if (buffer.msgid == (byte) MAVLINK_MSG_ID.RALLY_POINT && buffer.sysid == req.target_system &&
                        buffer.compid == req.target_component)
                    {
                        mavlink_rally_point_t fp = buffer.ToStructure<mavlink_rally_point_t>();

                        // check this gcs sent it
                        if (fp.target_system != gcssysid ||
                            fp.target_component != (byte) MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                            continue;

                        if (req.idx != fp.idx)
                        {
                            generatePacket((byte) MAVLINK_MSG_ID.FENCE_FETCH_POINT, req);
                            continue;
                        }

                        plla.Lat = fp.lat / t7;
                        plla.Lng = fp.lng / t7;
                        plla.Tag = fp.idx.ToString();
                        plla.Alt = fp.alt;

                        giveComport = false;

                        return (plla, fp.count);
                    }
                }
            }
        }

        [Obsolete]
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

            while (retry > 0)
            {
                generatePacket((byte) MAVLINK_MSG_ID.FENCE_POINT, fp);
                var newfp = getFencePoint(fp.idx).AwaitSync();

                if (newfp.plla.GetDistance(plla) < 5)
                    return true;
                retry--;
            }

            throw new Exception("Could not verify GeoFence Point");
        }

        [Obsolete]
        public bool setRallyPoint(byte index, PointLatLngAlt plla, short break_alt, UInt16 land_dir_cd, byte flags,
            byte rallypointcount)
        {
            mavlink_rally_point_t rp = new mavlink_rally_point_t();

            rp.idx = index;
            rp.count = rallypointcount;
            rp.lat = (int) (plla.Lat * t7);
            rp.lng = (int) (plla.Lng * t7);
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

                var newfp = getRallyPoint(rp.idx).GetAwaiter().GetResult();

                if (newfp.plla.Lat == plla.Lat && newfp.plla.Lng == rp.lng)
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

        public bool SetSensorOffsets(byte sysid, byte compid, sensoroffsetsenum sensor, float x, float y, float z)
        {
            return doCommand(sysid, compid, MAV_CMD.PREFLIGHT_SET_SENSOR_OFFSETS, (int) sensor, x, y, z, 0, 0, 0);
        }

        public void send_text(byte severity, string txt)
        {
            var temp = Encoding.ASCII.GetBytes(txt);
            generatePacket((byte) MAVLINK_MSG_ID.STATUSTEXT,
                new mavlink_statustext_t() {severity = (byte) severity, text = temp});
        }

        private Dictionary<Stream, Tuple<string, long>> streamfncache = new Dictionary<Stream, Tuple<string, long>>();
        private EventHandler<MAVLinkMessage> _OnPacketReceived;
        private EventHandler<MAVLinkMessage> _OnPacketSent;
        private static EventHandler<adsb.PointLatLngAltHdg> _UpdateADSBPlanePosition;
        private static EventHandler<(string id, MAV_COLLISION_THREAT_LEVEL threat_level)> _UpdateADSBCollision;
        private EventHandler _ParamListChanged;
        private EventHandler _MavChanged;
        private EventHandler _CommsClose;
        public bool printbps = true;
        private bool _openComplete = true;

        private MAVLinkMessage readlogPacketMavlink()
        {
            byte[] datearray = new byte[8];

            bool missingtimestamp = false;

            if (logplaybackfile.BaseStream is FileStream)
            {
                if (!streamfncache.ContainsKey(_logplaybackfile.BaseStream))
                    streamfncache[_logplaybackfile.BaseStream] = new Tuple<string, long>(
                        ((FileStream) _logplaybackfile.BaseStream).Name.ToLower(), logplaybackfile.BaseStream.Length);

                if (streamfncache[_logplaybackfile.BaseStream].Item1.EndsWith(".rlog"))
                    missingtimestamp = true;
            }
            else
            {
                if (!streamfncache.ContainsKey(_logplaybackfile.BaseStream))
                    streamfncache[_logplaybackfile.BaseStream] =
                        new Tuple<string, long>("", logplaybackfile.BaseStream.Length);
            }

            if (!missingtimestamp)
            {
                int tem = logplaybackfile.BaseStream.Read(datearray, 0, datearray.Length);

                Array.Reverse(datearray);

                DateTime date1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                UInt64 dateint = BitConverter.ToUInt64(datearray, 0);

                try
                {
                    // array is reversed above
                    if (datearray[7] == 254 || datearray[7] == 253)
                    {
                        //rewind 8bytes
                        logplaybackfile.BaseStream.Seek(-8, SeekOrigin.Current);
                    }
                    else
                    {
                        if ((dateint / 1000 / 1000 / 60 / 60) < 9999999)
                        {
                            date1 = date1.AddMilliseconds(dateint / 1000);

                            lastlogread = date1.ToLocalTime();
                            lastlogreadwall = DateTime.Now;
                        }
                    }
                }
                catch
                {
                }
            }

            byte[] temp = new byte[0];

            byte byte0 = 0;
            byte byte1 = 0;
            byte byte2 = 0;

            var filelength = streamfncache[_logplaybackfile.BaseStream].Item2;
            var filepos = logplaybackfile.BaseStream.Position;

            if (filelength == filepos)
                return MAVLinkMessage.Invalid;

            int length = 5;
            int a = 0;
            while (a < length)
            {
                if (filelength == filepos)
                    return MAVLinkMessage.Invalid;

                var tempb = (byte) logplaybackfile.ReadByte();
                filepos++;

                switch (a)
                {
                    case 0:
                        byte0 = tempb;
                        if (byte0 != 'U' && byte0 != MAVLINK_STX_MAVLINK1 && byte0 != MAVLINK_STX)
                        {
                            log.DebugFormat("logread - lost sync byte {0} pos {1}", byte0,
                                logplaybackfile.BaseStream.Position);
                            // seek to next valid
                            do
                            {
                                byte0 = logplaybackfile.ReadByte();
                            } while (byte0 != 'U' && byte0 != MAVLINK_STX_MAVLINK1 && byte0 != MAVLINK_STX);

                            a = 1;
                            continue;
                        }

                        break;

                    case 1:
                        byte1 = tempb;
                        // handle length
                    {
                        int headerlength = byte0 == MAVLINK_STX ? 9 : 5;
                        int headerlengthstx = headerlength + 1;

                        length = byte1 + headerlengthstx + 2; // header + 2 checksum
                    }
                        break;

                    case 2:
                        byte2 = tempb;
                        // handle signing and mavlink2
                        if (byte0 == MAVLINK_STX)
                        {
                            if ((byte2 & MAVLINK_IFLAG_SIGNED) > 0)
                                length += MAVLINK_SIGNATURE_BLOCK_LEN;
                        }

                        // handle rest
                    {
                        temp = new byte[length];
                        temp[0] = byte0;
                        temp[1] = byte1;
                        temp[2] = byte2;

                        var readto = a + 1;
                        var readlength = length - (a + 1);
                        logplaybackfile.Read(temp, readto, readlength);
                        a = length;
                    }
                        break;
                }

                a++;
            }

            MAVLinkMessage tmp = new MAVLinkMessage(temp, lastlogread);

            MAVlist[tmp.sysid, tmp.compid].cs.datetime = lastlogread;

            return tmp;
        }

        [Obsolete]
        public bool translateMode(string modein, ref mavlink_set_mode_t mode)
        {
            return translateMode(MAV.sysid, MAV.compid, modein, ref mode);
        }

        public bool translateMode(byte sysid, byte compid, string modein, ref mavlink_set_mode_t mode)
        {
            mode.target_system = sysid;

            if (modein == null || modein == "")
                return false;

            try
            {
                List<KeyValuePair<int, string>> modelist = Common.getModesList(MAVlist[sysid, compid].cs.firmware);

                foreach (KeyValuePair<int, string> pair in modelist)
                {
                    if (pair.Value.ToLower() == modein.ToLower())
                    {
                        mode.base_mode = (byte) MAV_MODE_FLAG.CUSTOM_MODE_ENABLED;
                        mode.custom_mode = (uint) pair.Key;
                    }
                }

                if (mode.base_mode == 0)
                {
                    log.Error("No Mode Changed " + modein);
                    return false;
                }
            }
            catch
            {
                log.Error("Failed to find Mode");
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
                    // try and pull vehicle type from version string
                    if (MAVlist[sysid, compid].VersionString != null && MAVlist[sysid, compid].VersionString.Length > 1)
                    {
                        var firmware_lookup = new Dictionary<string, Firmwares>()
                                {
                                    {"ArduPlane V", Firmwares.ArduPlane},
                                    {"ArduCopter V", Firmwares.ArduCopter2},
                                    {"Blimp V", Firmwares.ArduCopter2},
                                    {"ArduRover V",Firmwares.ArduRover},
                                    {"ArduSub V",Firmwares.ArduSub},
                                    {"AntennaTracker V", Firmwares.ArduTracker}
                                };
                        var match = firmware_lookup.Where(d => MAVlist[sysid, compid].VersionString.StartsWith(d.Key)).ToArray();
                        if (match.Count() == 1)
                        {
                            MAVlist[sysid, compid].cs.firmware = match[0].Value;
                            break;
                        }
                    }

                    switch (MAVlist[sysid, compid].aptype)
                    {
                        case MAV_TYPE.FIXED_WING:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduPlane;
                            break;

                        case MAV_TYPE n when (n >= MAV_TYPE.VTOL_DUOROTOR && n <= MAV_TYPE.VTOL_RESERVED5):
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduPlane;
                            break;

                        case MAV_TYPE.FLAPPING_WING:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduPlane;
                            break;

                        case MAV_TYPE.QUADROTOR:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduCopter2;
                            break;

                        case MAV_TYPE.TRICOPTER:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduCopter2;
                            break;

                        case MAV_TYPE.HEXAROTOR:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduCopter2;
                            break;

                        case MAV_TYPE.OCTOROTOR:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduCopter2;
                            break;

                        case MAV_TYPE.HELICOPTER:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduCopter2;
                            break;

                        case MAV_TYPE.DODECAROTOR:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduCopter2;
                            break;

                        case MAV_TYPE.GROUND_ROVER:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduRover;
                            break;

                        case MAV_TYPE.SURFACE_BOAT:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduRover;
                            break;

                        case MAV_TYPE.SUBMARINE:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduSub;
                            break;

                        case MAV_TYPE.ANTENNA_TRACKER:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduTracker;
                            break;
                        case MAV_TYPE.COAXIAL:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduCopter2;
                            break;
                        default:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.Other;
                            log.Error(MAVlist[sysid, compid].aptype + " not registered as valid type");
                            break;
                    }

                    break;

                case MAV_AUTOPILOT.UDB:
                    switch (MAVlist[sysid, compid].aptype)
                    {
                        case MAV_TYPE.FIXED_WING:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.ArduPlane;
                            break;
                    }

                    break;

                case MAV_AUTOPILOT.GENERIC:
                    switch (MAVlist[sysid, compid].aptype)
                    {
                        case MAV_TYPE.FIXED_WING:
                            MAVlist[sysid, compid].cs.firmware = Firmwares.Ateryx;
                            break;
                    }

                    break;

                case MAV_AUTOPILOT.PX4:
                    MAVlist[sysid, compid].cs.firmware = Firmwares.PX4;
                    break;

                default:
                    switch (MAVlist[sysid, compid].aptype)
                    {
                        case MAV_TYPE.GIMBAL: // storm32 - name 83
                            MAVlist[sysid, compid].cs.firmware = Firmwares.Gimbal;
                            break;
                    }

                    break;
            }
        }

        public override string ToString()
        {
            if (BaseStream != null && BaseStream.IsOpen)
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
            if (MAVlist != null)
                MAVlist.Dispose();

            this.Close();

            Terrain?.UnSub();
            Terrain = null;

            MirrorStream = null;

            logreadmode = false;
            logplaybackfile = null;
        }

        public void uAvionixADSBControl(int baroAltMSL,ushort squawk,/*UAVIONIX_ADSB_OUT_CONTROL_STATE*/byte state,/*UAVIONIX_ADSB_EMERGENCY_STATUS*/byte emergencyStatus,byte[] flight_id,byte x_bit)
        {
            mavlink_uavionix_adsb_out_control_t ctrl = new mavlink_uavionix_adsb_out_control_t();
            ctrl.state = state;
            ctrl.baroAltMSL = baroAltMSL;
            ctrl.squawk = squawk;
            ctrl.emergencyStatus = emergencyStatus;
            ctrl.flight_id = flight_id;
            ctrl.x_bit = x_bit;
            generatePacket(MAVLINK_MSG_ID.UAVIONIX_ADSB_OUT_CONTROL, ctrl);
        }

        private class NoUIReporter : IProgressReporterDialogue
        {
            private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            public ProgressWorkerEventArgs doWorkArgs { get; set; } = new ProgressWorkerEventArgs();

            public event DoWorkEventHandler DoWork;

            public void BeginInvoke(Delegate method)
            {
                method?.DynamicInvoke();
            }

            public void Dispose()
            {

            }

            public void RunBackgroundOperationAsync()
            {
                Task.Run(() =>
                {
                    try
                    {
                        log.Info("DoWork");
                        if (this.DoWork != null) this.DoWork(this);
                        log.Info("DoWork Done");
                    }
                    catch (Exception e)
                    {
                    }
                }).Wait();
            }

            public void UpdateProgressAndStatus(int progress, string status)
            {
                log.Info(progress + "% " + status);
            }
        }
    }
}
