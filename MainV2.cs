﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Threading;
using System.Net.Sockets;
using MissionPlanner.Utilities;
using IronPython.Hosting;
using log4net;
using MissionPlanner.Controls;
using System.Security.Cryptography;
using MissionPlanner.Comms;
using MissionPlanner.Arduino;
using Transitions;
using System.Web.Script.Serialization;
using System.Speech.Synthesis;
using MissionPlanner;

namespace MissionPlanner
{
    public partial class MainV2 : Form
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static class NativeMethods
        {
            // used to hide/show console window
            [DllImport("user32.dll")]
            public static extern int FindWindow(string szClass, string szTitle);
            [DllImport("user32.dll")]
            public static extern int ShowWindow(int Handle, int showState);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr RegisterDeviceNotification
            (IntPtr hRecipient,
            IntPtr NotificationFilter,
            Int32 Flags);

            static public int SW_SHOWNORMAL = 1;
            static public int SW_HIDE = 0;
        }

        public class menuicons
        {
            public static Image fd = global::MissionPlanner.Properties.Resources.light_flightdata_icon;
            public static Image fp = global::MissionPlanner.Properties.Resources.light_flightplan_icon;
            public static Image initsetup = global::MissionPlanner.Properties.Resources.light_initialsetup_icon;
            public static Image config_tuning = global::MissionPlanner.Properties.Resources.light_tuningconfig_icon;
            public static Image sim = global::MissionPlanner.Properties.Resources.light_simulation_icon;
            public static Image terminal = global::MissionPlanner.Properties.Resources.light_terminal_icon;
            public static Image help = global::MissionPlanner.Properties.Resources.light_help_icon;
            public static Image donate = global::MissionPlanner.Properties.Resources.donate;
            public static Image connect = global::MissionPlanner.Properties.Resources.light_connect_icon;
            public static Image disconnect = global::MissionPlanner.Properties.Resources.light_disconnect_icon;
            public static Image bg = global::MissionPlanner.Properties.Resources.bgdark;
        }

        public class menuicons2 : menuicons
        {
            public new static Image fd = global::MissionPlanner.Properties.Resources.dark_flightdata_icon;
            public new static Image fp = global::MissionPlanner.Properties.Resources.dark_flightplan_icon;
            public new static Image initsetup = global::MissionPlanner.Properties.Resources.dark_initialsetup_icon;
            public new static Image config_tuning = global::MissionPlanner.Properties.Resources.dark_tuningconfig_icon;
            public new static Image sim = global::MissionPlanner.Properties.Resources.dark_simulation_icon;
            public new static Image terminal = global::MissionPlanner.Properties.Resources.dark_terminal_icon;
            public new static Image help = global::MissionPlanner.Properties.Resources.dark_help_icon;
            public new static Image donate = global::MissionPlanner.Properties.Resources.donate;
            public new static Image connect = global::MissionPlanner.Properties.Resources.dark_connect_icon;
            public new static Image disconnect = global::MissionPlanner.Properties.Resources.dark_disconnect_icon;
            public new static Image bg = global::MissionPlanner.Properties.Resources.bgdark;
        }

        Controls.MainSwitcher MyView;

        /// <summary>
        /// Active Comport interface
        /// </summary>
        public static MAVLinkInterface comPort = new MAVLinkInterface();

/// <summary>
/// passive comports
/// </summary>
        public static List<MAVLinkInterface> Comports = new List<MAVLinkInterface>();

        public delegate void WMDeviceChangeEventHandler(WM_DEVICECHANGE_enum cause);
        public event WMDeviceChangeEventHandler DeviceChanged;

        /// <summary>
        /// Comport name
        /// </summary>
        public static string comPortName = "";
        /// <summary>
        /// use to store all internal config
        /// </summary>
        public static Hashtable config = new Hashtable();
        /// <summary>
        /// mono detection
        /// </summary>
        public static bool MONO = false;
        /// <summary>
        /// speech engine enable
        /// </summary>
        public static bool speechEnable = false;
        /// <summary>
        /// spech engine static class
        /// </summary>
        public static Speech speechEngine = null;
        /// <summary>
        /// joystick static class
        /// </summary>
        public static Joystick joystick = null;
        /// <summary>
        /// track last joystick packet sent. used to control rate
        /// </summary>
        DateTime lastjoystick = DateTime.Now;
        /// <summary>
        /// hud background image grabber from a video stream - not realy that efficent. ie no hardware overlays etc.
        /// </summary>
        public static WebCamService.Capture cam = null;
        /// <summary>
        /// controls the main serial reader thread
        /// </summary>
        bool serialThread = false;

        bool pluginthreadrun = false;

        Thread httpthread;
        Thread joystickthread;
        Thread serialreaderthread;
        Thread pluginthread;

        /// <summary>
        /// track the last heartbeat sent
        /// </summary>
        private DateTime heatbeatSend = DateTime.Now;
        /// <summary>
        /// used to call anything as needed.
        /// </summary>
        public static MainV2 instance = null;

        public static string LogDir { 
            get { 
                if (config["logdirectory"] == null) 
                    return _logdir; 
                return config["logdirectory"].ToString(); 
            } 
            set
            { 
                _logdir = value;
                config["logdirectory"] = value; 
            } 
        }
        static string _logdir =  Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"logs";

        public static MainSwitcher View;

        /// <summary>
        /// store the time we first connect
        /// </summary>
        DateTime connecttime = DateTime.Now;
        DateTime nodatawarning = DateTime.Now;
        DateTime OpenTime = DateTime.Now;

        /// <summary>
        /// enum of firmwares
        /// </summary>
        public enum Firmwares
        {
            ArduPlane,
            ArduCopter2,
            //ArduHeli,
            ArduRover,
            Ateryx
        }

        DateTime connectButtonUpdate = DateTime.Now;
        /// <summary>
        /// declared here if i want a "single" instance of the form
        /// ie configuration gets reloaded on every click
        /// </summary>
        public GCSViews.FlightData FlightData;
        public GCSViews.FlightPlanner FlightPlanner;
        GCSViews.Simulation Simulation;

        private Form connectionStatsForm;
        private ConnectionStats _connectionStats;

        /// <summary>
        /// This 'Control' is the toolstrip control that holds the comport combo, baudrate combo etc
        /// Otiginally seperate controls, each hosted in a toolstip sqaure, combined into this custom
        /// control for layout reasons.
        /// </summary>
        static internal ConnectionControl _connectionControl;

        public MainV2()
        {
            log.Info("Mainv2 ctor");

            Form splash = Program.Splash;




            splash.Refresh();

            Application.DoEvents();

            instance = this;

            InitializeComponent();

            MyView = new MainSwitcher(this);

            View = MyView;

            _connectionControl = toolStripConnectionControl.ConnectionControl;
            _connectionControl.CMB_baudrate.TextChanged += this.CMB_baudrate_TextChanged;
            _connectionControl.CMB_serialport.SelectedIndexChanged += this.CMB_serialport_SelectedIndexChanged;
            _connectionControl.CMB_serialport.Enter += this.CMB_serialport_Enter;
            _connectionControl.CMB_serialport.Click += this.CMB_serialport_Click;
            _connectionControl.TOOL_APMFirmware.SelectedIndexChanged += this.TOOL_APMFirmware_SelectedIndexChanged;

            _connectionControl.ShowLinkStats += (sender, e) => ShowConnectionStatsForm();
            srtm.datadirectory = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "srtm";

            var t = Type.GetType("Mono.Runtime");
            MONO = (t != null);

            speechEngine = new Speech();

            // proxy loader - dll load now instead of on config form load
            new Transition(new TransitionType_EaseInEaseOut(2000));

            //MyRenderer.currentpressed = MenuFlightData;

            //MainMenu.Renderer = new MyRenderer();

            foreach (object obj in Enum.GetValues(typeof(Firmwares)))
            {
                _connectionControl.TOOL_APMFirmware.Items.Add(obj);
            }

            if (_connectionControl.TOOL_APMFirmware.Items.Count > 0)
                _connectionControl.TOOL_APMFirmware.SelectedIndex = 0;

            this.Text = splash.Text;

            comPort.BaseStream.BaudRate = 115200;

            // ** Old
            //            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            //            CMB_serialport.Items.Add("TCP");
            //            CMB_serialport.Items.Add("UDP");
            //            if (CMB_serialport.Items.Count > 0)
            //            {
            //                CMB_baudrate.SelectedIndex = 7;
            //                CMB_serialport.SelectedIndex = 0;
            //            }
            // ** new
            _connectionControl.CMB_serialport.Items.Add("AUTO");
            _connectionControl.CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            _connectionControl.CMB_serialport.Items.Add("TCP");
            _connectionControl.CMB_serialport.Items.Add("UDP");
            if (_connectionControl.CMB_serialport.Items.Count > 0)
            {
                _connectionControl.CMB_baudrate.SelectedIndex = 8;
                _connectionControl.CMB_serialport.SelectedIndex = 0;
            }
            // ** Done

            splash.Refresh();
            Application.DoEvents();

            // set this before we reset it
            MainV2.config["NUM_tracklength"] = "200";

            // create one here - but override on load
            MainV2.config["guid"] = Guid.NewGuid().ToString();

            // load config
            xmlconfig(false);

            MissionPlanner.Utilities.Tracking.cid = new Guid(MainV2.config["guid"].ToString());

            if (config.ContainsKey("language") && !string.IsNullOrEmpty((string)config["language"]))
                changelanguage(CultureInfoEx.GetCultureInfo((string)config["language"]));

            if (!MONO) // windows only
            {
                if (MainV2.config["showconsole"] != null && MainV2.config["showconsole"].ToString() == "True")
                {
                }
                else
                {
                    int win = NativeMethods.FindWindow("ConsoleWindowClass", null);
                    NativeMethods.ShowWindow(win, NativeMethods.SW_HIDE); // hide window
                }
            }

            ChangeUnits();

            if (config["theme"] != null)
            {
                ThemeManager.SetTheme((ThemeManager.Themes)Enum.Parse(typeof(ThemeManager.Themes), MainV2.config["theme"].ToString()));

                if (ThemeManager.CurrentTheme == ThemeManager.Themes.Custom)
                {
                    try
                    {
                        ThemeManager.BGColor = Color.FromArgb(int.Parse(MainV2.config["theme_bg"].ToString()));
                        ThemeManager.ControlBGColor = Color.FromArgb(int.Parse(MainV2.config["theme_ctlbg"].ToString()));
                        ThemeManager.TextColor = Color.FromArgb(int.Parse(MainV2.config["theme_text"].ToString()));
                        ThemeManager.ButBG = Color.FromArgb(int.Parse(MainV2.config["theme_butbg"].ToString()));
                        ThemeManager.ButBorder = Color.FromArgb(int.Parse(MainV2.config["theme_butbord"].ToString()));
                    }
                    catch { log.Error("Bad Custom theme - reset to standard"); ThemeManager.SetTheme(ThemeManager.Themes.BurntKermit); }
                }
            }

            try
            {
                log.Info("Create FD");
                FlightData = new GCSViews.FlightData();
                log.Info("Create FP");
                FlightPlanner = new GCSViews.FlightPlanner();
                //Configuration = new GCSViews.ConfigurationView.Setup();
                log.Info("Create SIM");
                Simulation = new GCSViews.Simulation();
                //Firmware = new GCSViews.Firmware();
                //Terminal = new GCSViews.Terminal();

                // preload
                log.Info("Create Python");
                Python.CreateEngine();

                FlightData.Width = MyView.Width;
                FlightPlanner.Width = MyView.Width;
                Simulation.Width = MyView.Width;
            }
            catch (ArgumentException e)
            {
                //http://www.microsoft.com/en-us/download/details.aspx?id=16083
                //System.ArgumentException: Font 'Arial' does not support style 'Regular'.

                log.Fatal(e);
                CustomMessageBox.Show(e.ToString() + "\n\n Font Issues? Please install this http://www.microsoft.com/en-us/download/details.aspx?id=16083");
                //splash.Close();
                //this.Close();
                Application.Exit();
            }
            catch (Exception e) { log.Fatal(e); CustomMessageBox.Show("A Major error has occured : " + e.ToString()); Application.Exit(); }

            if (MainV2.config["CHK_GDIPlus"] != null)
                GCSViews.FlightData.myhud.UseOpenGL = !bool.Parse(MainV2.config["CHK_GDIPlus"].ToString());

            try
            {
                if (config["MainLocX"] != null && config["MainLocY"] != null)
                {
                    this.StartPosition = FormStartPosition.Manual;
                    Point startpos = new Point(int.Parse(config["MainLocX"].ToString()), int.Parse(config["MainLocY"].ToString()));
                    this.Location = startpos;
                }

                if (config["MainMaximised"] != null)
                {
                    this.WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), config["MainMaximised"].ToString());
                    // dont allow minimised start state
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.Location = new Point(100, 100);
                    }
                }

                if (config["MainHeight"] != null)
                    this.Height = int.Parse(config["MainHeight"].ToString());
                if (config["MainWidth"] != null)
                    this.Width = int.Parse(config["MainWidth"].ToString());

                if (config["CMB_rateattitude"] != null)
                    MainV2.comPort.MAV.cs.rateattitude = byte.Parse(config["CMB_rateattitude"].ToString());
                if (config["CMB_rateposition"] != null)
                    MainV2.comPort.MAV.cs.rateposition = byte.Parse(config["CMB_rateposition"].ToString());
                if (config["CMB_ratestatus"] != null)
                    MainV2.comPort.MAV.cs.ratestatus = byte.Parse(config["CMB_ratestatus"].ToString());
                if (config["CMB_raterc"] != null)
                    MainV2.comPort.MAV.cs.raterc = byte.Parse(config["CMB_raterc"].ToString());
                if (config["CMB_ratesensors"] != null)
                    MainV2.comPort.MAV.cs.ratesensors = byte.Parse(config["CMB_ratesensors"].ToString());

                if (config["speechenable"] != null)
                    MainV2.speechEnable = bool.Parse(config["speechenable"].ToString());

                if (MainV2.config["analyticsoptout"] != null)
                    MissionPlanner.Utilities.Tracking.OptOut = bool.Parse(config["analyticsoptout"].ToString());

                try
                {
                    if (config["TXT_homelat"] != null)
                        MainV2.comPort.MAV.cs.HomeLocation.Lat = double.Parse(config["TXT_homelat"].ToString());

                    if (config["TXT_homelng"] != null)
                        MainV2.comPort.MAV.cs.HomeLocation.Lng = double.Parse(config["TXT_homelng"].ToString());

                    if (config["TXT_homealt"] != null)
                        MainV2.comPort.MAV.cs.HomeLocation.Alt = double.Parse(config["TXT_homealt"].ToString());
                }
                catch { }
            }
            catch { }

            if (MainV2.comPort.MAV.cs.rateattitude == 0) // initilised to 10, configured above from save
            {
                CustomMessageBox.Show("NOTE: your attitude rate is 0, the hud will not work\nChange in Configuration > Planner > Telemetry Rates");
            }

            // log dir

            if (config["logdirectory"] != null)
                MainV2.LogDir = config["logdirectory"].ToString();

            //System.Threading.Thread.Sleep(2000);

            // make sure new enough .net framework is installed
            if (!MONO)
            {
                Microsoft.Win32.RegistryKey installed_versions = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
                string[] version_names = installed_versions.GetSubKeyNames();
                //version names start with 'v', eg, 'v3.5' which needs to be trimmed off before conversion
                double Framework = Convert.ToDouble(version_names[version_names.Length - 1].Remove(0, 1), CultureInfo.InvariantCulture);
                int SP = Convert.ToInt32(installed_versions.OpenSubKey(version_names[version_names.Length - 1]).GetValue("SP", 0));

                if (Framework < 4.0)
                {
                    CustomMessageBox.Show("This program requires .NET Framework 4.0. You currently have " + Framework);
                }
            }

            Application.DoEvents();

            Comports.Add(comPort);


            //int fixmenextrelease;
           // if (MainV2.getConfig("fixparams") == "")
            {
            //    Utilities.ParameterMetaDataParser.GetParameterInformation();
            //    MainV2.config["fixparams"] = 1;
            }


           
        }


        private void ResetConnectionStats()
        {
            // If the form has been closed, or never shown before, we need do nothing, as 
            // connection stats will be reset when shown
            if (this.connectionStatsForm != null && connectionStatsForm.Visible)
            {
                // else the form is already showing.  reset the stats
                this.connectionStatsForm.Controls.Clear();
                _connectionStats = new ConnectionStats(comPort);
                this.connectionStatsForm.Controls.Add(_connectionStats);
                ThemeManager.ApplyThemeTo(this.connectionStatsForm);
            }
        }

        private void ShowConnectionStatsForm()
        {
            if (this.connectionStatsForm == null || this.connectionStatsForm.IsDisposed)
            {
                // If the form has been closed, or never shown before, we need all new stuff
                this.connectionStatsForm = new Form
                {
                    Width = 430,
                    Height = 180,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = "Link Stats"
                };
                // Change the connection stats control, so that when/if the connection stats form is showing,
                // there will be something to see
                this.connectionStatsForm.Controls.Clear();
                _connectionStats = new ConnectionStats(comPort);
                this.connectionStatsForm.Controls.Add(_connectionStats);
                this.connectionStatsForm.Width = _connectionStats.Width;
            }

            this.connectionStatsForm.Show();
            ThemeManager.ApplyThemeTo(this.connectionStatsForm);
        }

        private void CMB_serialport_Click(object sender, EventArgs e)
        {
            string oldport = _connectionControl.CMB_serialport.Text;
            _connectionControl.CMB_serialport.Items.Clear();
            _connectionControl.CMB_serialport.Items.Add("AUTO");
            _connectionControl.CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            _connectionControl.CMB_serialport.Items.Add("TCP");
            _connectionControl.CMB_serialport.Items.Add("UDP");
            if (_connectionControl.CMB_serialport.Items.Contains(oldport))
                _connectionControl.CMB_serialport.Text = oldport;
        }


        private void MenuFlightData_Click(object sender, EventArgs e)
        {
            MyView.ShowScreen("FlightData");
        }

        private void MenuFlightPlanner_Click(object sender, EventArgs e)
        {
            // refresh ap/ac specific items
            FlightPlanner.updateCMDParams();

            MyView.ShowScreen("FlightPlanner");
        }

        public void MenuSetup_Click(object sender, EventArgs e)
        {
            if (getConfig("password_protect") == "" || bool.Parse(getConfig("password_protect")) == false)
            {
                MyView.ShowScreen("HWConfig");
            }
            else
            {
                if (Password.VerifyPassword())
                {
                    MyView.ShowScreen("HWConfig");
                }
            }
        }

        private void MenuSimulation_Click(object sender, EventArgs e)
        {
            MyView.ShowScreen("Simulation");
        }

        private void MenuTuning_Click(object sender, EventArgs e)
        {
            if (getConfig("password_protect") == "" || bool.Parse(getConfig("password_protect")) == false)
            {
                MyView.ShowScreen("SWConfig");
            }
            else
            {
                if (Password.VerifyPassword())
                {
                    MyView.ShowScreen("SWConfig");
                }
            }
        }

        private void MenuTerminal_Click(object sender, EventArgs e)
        {
            MyView.ShowScreen("Terminal");
        }

        private void MenuConnect_Click(object sender, EventArgs e)
        {
            comPort.giveComport = false;

            // sanity check
            if (comPort.BaseStream.IsOpen && MainV2.comPort.MAV.cs.groundspeed > 4)
            {
                if (DialogResult.No == CustomMessageBox.Show("Your model is still moving are you sure you want to disconnect?", "Disconnect", MessageBoxButtons.YesNo))
                {
                    return;
                }
            }

            try
            {
                // cleanup from any previous sessions
                if (comPort.logfile != null)
                    comPort.logfile.Close();

                if (comPort.rawlogfile != null)
                    comPort.rawlogfile.Close();
            }
            catch (Exception ex) 
            { 
                CustomMessageBox.Show("Error closing log files (Out of disk space?)\n"+ex.Message,"Error");
            }

            comPort.logfile = null;
            comPort.rawlogfile = null;

            // decide if this is a connect or disconnect
            if (comPort.BaseStream.IsOpen)
            {
                try
                {
                    if (speechEngine != null) // cancel all pending speech
                        speechEngine.SpeakAsyncCancelAll();

                    comPort.BaseStream.DtrEnable = false;
                    comPort.Close();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                // now that we have closed the connection, cancel the connection stats
                // so that the 'time connected' etc does not grow, but the user can still
                // look at the now frozen stats on the still open form
                try
                {
                    // if terminal is used, then closed using this button.... exception
                    if (this.connectionStatsForm != null)
                        ((ConnectionStats)this.connectionStatsForm.Controls[0]).StopUpdates();
                }
                catch { }

                // refresh config window if needed
                if (MyView.current != null)
                {
                    if (MyView.current.Name == "HWConfig")
                        MyView.ShowScreen("HWConfig");
                    if (MyView.current.Name == "SWConfig")
                        MyView.ShowScreen("SWConfig");
                }

            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    try
                    {
                        MissionPlanner.Log.LogSort.SortLogs(Directory.GetFiles(MainV2.LogDir, "*.tlog"));
                    }
                    catch { }
                }
                );
            }
            catch { }
			
                this.MenuConnect.Image = global::MissionPlanner.Properties.Resources.light_connect_icon;
            }
            else
            {
                switch (_connectionControl.CMB_serialport.Text)
                {
                    case "TCP":
                        comPort.BaseStream = new TcpSerial();
                        break;
                    case "UDP":
                        comPort.BaseStream = new UdpSerial();
                        break;
                    case "AUTO":
                    default:
                        comPort.BaseStream = new SerialPort();
                        break;
                }

                // Tell the connection UI that we are now connected.
                _connectionControl.IsConnected(true);

                // Here we want to reset the connection stats counter etc.
                this.ResetConnectionStats();

                MainV2.comPort.MAV.cs.ResetInternals();

                //cleanup any log being played
                comPort.logreadmode = false;
                if (comPort.logplaybackfile != null)
                    comPort.logplaybackfile.Close();
                comPort.logplaybackfile = null;

                try
                {
                    // do autoscan
                    if (_connectionControl.CMB_serialport.Text == "AUTO")
                    {
                        Comms.CommsSerialScan.Scan(false);

                        DateTime deadline = DateTime.Now.AddSeconds(50);

                        while (Comms.CommsSerialScan.foundport == false)
                        {
                            System.Threading.Thread.Sleep(100);

                            if (DateTime.Now > deadline) {
                                CustomMessageBox.Show("Timeout waiting for autoscan/no mavlink device connected");
                                _connectionControl.IsConnected(false);
                                return;
                            }
                        }

                        _connectionControl.CMB_serialport.Text = Comms.CommsSerialScan.portinterface.PortName;
                        _connectionControl.CMB_baudrate.Text = Comms.CommsSerialScan.portinterface.BaudRate.ToString();
                    }

                    // set port, then options
                    comPort.BaseStream.PortName = _connectionControl.CMB_serialport.Text;

                    try
                    {
                        comPort.BaseStream.BaudRate = int.Parse(_connectionControl.CMB_baudrate.Text);
                    }
                    catch (Exception exp) { log.Error(exp); }

                    // false here
                    comPort.BaseStream.DtrEnable = false;
                    comPort.BaseStream.RtsEnable = false;

                    // prevent serialreader from doing anything
                    comPort.giveComport = true;

                        // reset on connect logic.
                        if (config["CHK_resetapmonconnect"] == null || bool.Parse(config["CHK_resetapmonconnect"].ToString()) == true)
                            comPort.BaseStream.toggleDTR();

                        comPort.giveComport = false;

                    // setup to record new logs
                    try
                    {
                        Directory.CreateDirectory(MainV2.LogDir);
                        comPort.logfile = new BufferedStream(File.Open(MainV2.LogDir + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".tlog", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None));

                        comPort.rawlogfile = new BufferedStream(File.Open(MainV2.LogDir + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".rlog", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None));
                    }
                    catch (Exception exp2) { log.Error(exp2); CustomMessageBox.Show("Failed to create log - wont log this session"); } // soft fail

                    // reset connect time - for timeout functions
                    connecttime = DateTime.Now;

                    // do the connect
                    comPort.Open(true);

                    // detect firmware we are conected to.
                    if (comPort.MAV.param["SYSID_SW_TYPE"] != null)
                    {
                        if (float.Parse(comPort.MAV.param["SYSID_SW_TYPE"].ToString()) == 10)
                        {
                            _connectionControl.TOOL_APMFirmware.SelectedIndex = _connectionControl.TOOL_APMFirmware.Items.IndexOf(Firmwares.ArduCopter2);
                        }
                        else if (float.Parse(comPort.MAV.param["SYSID_SW_TYPE"].ToString()) == 7)
                        {
                            _connectionControl.TOOL_APMFirmware.SelectedIndex = _connectionControl.TOOL_APMFirmware.Items.IndexOf(Firmwares.Ateryx);
                        }
                        else if (float.Parse(comPort.MAV.param["SYSID_SW_TYPE"].ToString()) == 20)
                        {
                            _connectionControl.TOOL_APMFirmware.SelectedIndex = _connectionControl.TOOL_APMFirmware.Items.IndexOf(Firmwares.ArduRover);
                        }
                        else if (float.Parse(comPort.MAV.param["SYSID_SW_TYPE"].ToString()) == 0)
                        {
                            _connectionControl.TOOL_APMFirmware.SelectedIndex = _connectionControl.TOOL_APMFirmware.Items.IndexOf(Firmwares.ArduPlane);
                        }
                    }

                    MissionPlanner.Utilities.Tracking.AddEvent("Connect", "Connect", comPort.MAV.cs.firmware.ToString(), comPort.MAV.param.Count.ToString());
                    MissionPlanner.Utilities.Tracking.AddTiming("Connect", "Connect Time", (DateTime.Now - connecttime).TotalMilliseconds,"");

                    MissionPlanner.Utilities.Tracking.AddEvent("Connect", "Baud", comPort.BaseStream.BaudRate.ToString(),"");

                    // save the baudrate for this port
                    config[_connectionControl.CMB_serialport.Text + "_BAUD"] = _connectionControl.CMB_baudrate.Text;

                    // refresh config window if needed
                    if (MyView.current != null) 
                    {
                        if (MyView.current.Name == "HWConfig")
                            MyView.ShowScreen("HWConfig");
                        if (MyView.current.Name == "SWConfig")
                            MyView.ShowScreen("SWConfig");
                    }


                    // load wps on connect option.
                    if (config["loadwpsonconnect"] != null && bool.Parse(config["loadwpsonconnect"].ToString()) == true)
                    {
                        MenuFlightPlanner_Click(null, null);
                        FlightPlanner.BUT_read_Click(null, null);
                    }

                    // set connected icon
                    this.MenuConnect.Image = menuicons.disconnect;
                }
                catch (Exception ex)
                {
                    log.Warn(ex);
                    try
                    {
                        _connectionControl.IsConnected(false);
                        UpdateConnectIcon();
                        comPort.Close();
                    }
                    catch { }
                    CustomMessageBox.Show(@"Can not establish a connection\n\n" + ex.Message);
                    return;
                }
            }
        }

        private void CMB_serialport_SelectedIndexChanged(object sender, EventArgs e)
        {
            comPortName = _connectionControl.CMB_serialport.Text;
            if (comPortName == "UDP" || comPortName == "TCP" || comPortName == "AUTO")
            {
                _connectionControl.CMB_baudrate.Enabled = false;
                if (comPortName == "TCP")
                    MainV2.comPort.BaseStream = new TcpSerial();
                if (comPortName == "UDP")
                    MainV2.comPort.BaseStream = new UdpSerial();
                if (comPortName == "AUTO")
                {
                    MainV2.comPort.BaseStream = new SerialPort();
                    return;
                }
            }
            else
            {
                _connectionControl.CMB_baudrate.Enabled = true;
                MainV2.comPort.BaseStream = new SerialPort();
            }

            try
            {
                comPort.BaseStream.PortName = _connectionControl.CMB_serialport.Text;

                MainV2.comPort.BaseStream.BaudRate = int.Parse(_connectionControl.CMB_baudrate.Text);

                // check for saved baud rate and restore
                if (config[_connectionControl.CMB_serialport.Text + "_BAUD"] != null)
                {
                    _connectionControl.CMB_baudrate.Text = config[_connectionControl.CMB_serialport.Text + "_BAUD"].ToString();
                }
            }
            catch { }
        }

        private void MainV2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Console.WriteLine("MainV2_FormClosed");
        }


        private void xmlconfig(bool write)
        {
            // copy config across is it exists - but dont overwrite
            if (File.Exists(@"C:\Program Files (x86)\APM Planner\config.xml") && !File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"config.xml"))
            {
                File.Copy(@"C:\Program Files (x86)\APM Planner\config.xml", Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"config.xml");
            }

            if (write || !File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"config.xml"))
            {
                try
                {
                    XmlTextWriter xmlwriter = new XmlTextWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"config.xml", Encoding.ASCII);
                    xmlwriter.Formatting = Formatting.Indented;

                    xmlwriter.WriteStartDocument();

                    xmlwriter.WriteStartElement("Config");

                    xmlwriter.WriteElementString("comport", comPortName);

                    xmlwriter.WriteElementString("baudrate", _connectionControl.CMB_baudrate.Text);

                    xmlwriter.WriteElementString("APMFirmware", MainV2.comPort.MAV.cs.firmware.ToString());

                    foreach (string key in config.Keys)
                    {
                        try
                        {
                            if (key == "" || key.Contains("/")) // "/dev/blah"
                                continue;
                            xmlwriter.WriteElementString(key, config[key].ToString());
                        }
                        catch { }
                    }

                    xmlwriter.WriteEndElement();

                    xmlwriter.WriteEndDocument();
                    xmlwriter.Close();
                }
                catch (Exception ex) { CustomMessageBox.Show(ex.ToString()); }
            }
            else
            {
                try
                {
                    using (XmlTextReader xmlreader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"config.xml"))
                    {
                        while (xmlreader.Read())
                        {
                            xmlreader.MoveToElement();
                            try
                            {
                                switch (xmlreader.Name)
                                {
                                    case "comport":
                                        string temp = xmlreader.ReadString();

                                        _connectionControl.CMB_serialport.SelectedIndex = _connectionControl.CMB_serialport.FindString(temp);
                                        if (_connectionControl.CMB_serialport.SelectedIndex == -1)
                                        {
                                            _connectionControl.CMB_serialport.Text = temp; // allows ports that dont exist - yet
                                        }
                                        comPort.BaseStream.PortName = temp;
                                        comPortName = temp;
                                        break;
                                    case "baudrate":
                                        string temp2 = xmlreader.ReadString();

                                        _connectionControl.CMB_baudrate.SelectedIndex = _connectionControl.CMB_baudrate.FindString(temp2);
                                        if (_connectionControl.CMB_baudrate.SelectedIndex == -1)
                                        {
                                            _connectionControl.CMB_baudrate.Text = temp2;
                                            //CMB_baudrate.SelectedIndex = CMB_baudrate.FindString("57600"); ; // must exist
                                        }
                                        //bau = int.Parse(CMB_baudrate.Text);
                                        break;
                                    case "APMFirmware":
                                        string temp3 = xmlreader.ReadString();
                                        _connectionControl.TOOL_APMFirmware.SelectedIndex = _connectionControl.TOOL_APMFirmware.FindStringExact(temp3);
                                        if (_connectionControl.TOOL_APMFirmware.SelectedIndex == -1)
                                            _connectionControl.TOOL_APMFirmware.SelectedIndex = 0;
                                        MainV2.comPort.MAV.cs.firmware = (MainV2.Firmwares)Enum.Parse(typeof(MainV2.Firmwares), _connectionControl.TOOL_APMFirmware.Text);
                                        break;
                                    case "Config":
                                        break;
                                    case "xml":
                                        break;
                                    default:
                                        if (xmlreader.Name == "") // line feeds
                                            break;
                                        config[xmlreader.Name] = xmlreader.ReadString();
                                        break;
                                }
                            }
                            // silent fail on bad entry
                            catch (Exception ee)
                            {
                                log.Error(ee);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Bad Config File", ex);
                }
            }
        }

        /// <summary>
        /// thread used to send joystick packets to the MAV
        /// </summary>
        private void joysticksend()
        {
            float rate = 50; // 1000 / 50 = 20 hz
            int count = 0;

            DateTime lastratechange = DateTime.Now;

            while (true)
            {
                try
                {
                    if (MONO)
                    {
                        log.Error("Mono: closing joystick thread");
                        break;
                    }

                    if (!MONO)
                    {
                        //joystick stuff

                        if (joystick != null && joystick.enabled)
                        {
                            MAVLink.mavlink_rc_channels_override_t rc = new MAVLink.mavlink_rc_channels_override_t();

                            rc.target_component = comPort.MAV.compid;
                            rc.target_system = comPort.MAV.sysid;

                            if (joystick.getJoystickAxis(1) != Joystick.joystickaxis.None)
                                rc.chan1_raw = MainV2.comPort.MAV.cs.rcoverridech1;//(ushort)(((int)state.Rz / 65.535) + 1000);
                            if (joystick.getJoystickAxis(2) != Joystick.joystickaxis.None)
                                rc.chan2_raw = MainV2.comPort.MAV.cs.rcoverridech2;//(ushort)(((int)state.Y / 65.535) + 1000);
                            if (joystick.getJoystickAxis(3) != Joystick.joystickaxis.None)
                                rc.chan3_raw = MainV2.comPort.MAV.cs.rcoverridech3;//(ushort)(1000 - ((int)slider[0] / 65.535 ) + 1000);
                            if (joystick.getJoystickAxis(4) != Joystick.joystickaxis.None)
                                rc.chan4_raw = MainV2.comPort.MAV.cs.rcoverridech4;//(ushort)(((int)state.X / 65.535) + 1000);
                            if (joystick.getJoystickAxis(5) != Joystick.joystickaxis.None)
                                rc.chan5_raw = MainV2.comPort.MAV.cs.rcoverridech5;
                            if (joystick.getJoystickAxis(6) != Joystick.joystickaxis.None)
                                rc.chan6_raw = MainV2.comPort.MAV.cs.rcoverridech6;
                            if (joystick.getJoystickAxis(7) != Joystick.joystickaxis.None)
                                rc.chan7_raw = MainV2.comPort.MAV.cs.rcoverridech7;
                            if (joystick.getJoystickAxis(8) != Joystick.joystickaxis.None)
                                rc.chan8_raw = MainV2.comPort.MAV.cs.rcoverridech8;

                            if (lastjoystick.AddMilliseconds(rate) < DateTime.Now)
                            {
                                /*
                                if (MainV2.comPort.MAV.cs.rssi > 0 && MainV2.comPort.MAV.cs.remrssi > 0)
                                {
                                    if (lastratechange.Second != DateTime.Now.Second)
                                    {
                                        if (MainV2.comPort.MAV.cs.txbuffer > 90)
                                        {
                                            if (rate < 20)
                                                rate = 21;
                                            rate--;

                                            if (MainV2.comPort.MAV.cs.linkqualitygcs < 70)
                                                rate = 50;
                                        }
                                        else
                                        {
                                            if (rate > 100)
                                                rate = 100;
                                            rate++;
                                        }

                                        lastratechange = DateTime.Now;
                                    }
                                 
                                }
                                 */
                                //                                Console.WriteLine(DateTime.Now.Millisecond + " {0} {1} {2} {3} {4}", rc.chan1_raw, rc.chan2_raw, rc.chan3_raw, rc.chan4_raw,rate);
                                comPort.sendPacket(rc);
                                count++;
                                lastjoystick = DateTime.Now;
                            }

                        }
                    }
                    Thread.Sleep(20);
                }
                catch
                {

                } // cant fall out
            }
        }

        /// <summary>
        /// Used to fix the icon status for unexpected unplugs etc...
        /// </summary>
        private void UpdateConnectIcon()
        {
            if ((DateTime.Now - connectButtonUpdate).Milliseconds > 500)
            {
                //                        Console.WriteLine(DateTime.Now.Millisecond);
                if (comPort.BaseStream.IsOpen)
                {
                    if ((string)this.MenuConnect.Image.Tag != "Disconnect")
                    {
                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            this.MenuConnect.Image = menuicons.disconnect;
                            this.MenuConnect.Image.Tag = "Disconnect";
                            this.MenuConnect.Text = "DISCONNECT";
                            _connectionControl.IsConnected(true);
                        });
                    }
                }
                else
                {
                    if (this.MenuConnect.Image != null && (string)this.MenuConnect.Image.Tag != "Connect")
                    {
                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            this.MenuConnect.Image = menuicons.connect;
                            this.MenuConnect.Image.Tag = "Connect";
                            this.MenuConnect.Text = "CONNECT";
                            _connectionControl.IsConnected(false);
                            if (_connectionStats != null)
                            {
                                _connectionStats.StopUpdates();
                            }
                        });
                    }

                    if (comPort.logreadmode)
                    {
                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            _connectionControl.IsConnected(true);
                        });
                    }
                }
                connectButtonUpdate = DateTime.Now;
            }
        }

        ManualResetEvent PluginThreadrunner = new ManualResetEvent(false);

        private void PluginThread()
        {
            Hashtable nextrun = new Hashtable();

            pluginthreadrun = true;

            PluginThreadrunner.Reset();

            while (pluginthreadrun)
            {
                try
                {
                    lock (Plugin.PluginLoader.Plugins)
                    {
                        foreach (var plugin in Plugin.PluginLoader.Plugins)
                        {
                            if (!nextrun.ContainsKey(plugin))
                                nextrun[plugin] = DateTime.MinValue;

                            if (DateTime.Now > plugin.NextRun)
                            {
                                // get ms till next run
                                int msnext = (int)(1000 / plugin.loopratehz);
                                // allow the plug to modify this, if needed
                                plugin.NextRun = DateTime.Now.AddMilliseconds(msnext);

                                try
                                {
                                    bool ans = plugin.Loop();

                                }
                                catch (Exception ex) { log.Error(ex); }
                            }
                        }
                    }
                }
                catch { }

                // max rate is 100 hz - prevent massive cpu usage
                System.Threading.Thread.Sleep(10);
            }

            while (Plugin.PluginLoader.Plugins.Count > 0)
            {
                var plugin = Plugin.PluginLoader.Plugins[0];
                try
                {
                    plugin.Exit();
                }
                catch (Exception ex) { log.Error(ex); }
                Plugin.PluginLoader.Plugins.Remove(plugin);
            }

            PluginThreadrunner.Set();

            return;
        }

        ManualResetEvent SerialThreadrunner = new ManualResetEvent(false);

        /// <summary>
        /// main serial reader thread
        /// controls
        /// serial reading
        /// link quality stats
        /// speech voltage - custom - alt warning - data lost
        /// heartbeat packet sending
        /// 
        /// and can't fall out
        /// </summary>
        private void SerialReader()
        {
            if (serialThread == true)
                return;
            serialThread = true;

            SerialThreadrunner.Reset();

            int minbytes = 0;

            bool armedstatus = false;

            string lastmessagehigh = "";

            DateTime speechcustomtime = DateTime.Now;

            DateTime speechbatterytime = DateTime.Now;
            DateTime speechlowspeedtime = DateTime.Now;

            DateTime linkqualitytime = DateTime.Now;

            while (serialThread)
            {
                try
                {
                    Thread.Sleep(1); // was 5

                    try
                    {
                        if (GCSViews.Terminal.comPort != null && GCSViews.Terminal.comPort.IsOpen)
                            continue;
                    }
                    catch { }

                    // update connect/disconnect button and info stats
                    UpdateConnectIcon();

                    // 30 seconds interval speech options
                    if (speechEnable && speechEngine != null && (DateTime.Now - speechcustomtime).TotalSeconds > 30 && (MainV2.comPort.logreadmode || comPort.BaseStream.IsOpen))
                    {
                        if (MainV2.speechEngine.State == SynthesizerState.Ready)
                        {
                            if (MainV2.getConfig("speechcustomenabled") == "True")
                            {
                                MainV2.speechEngine.SpeakAsync(Common.speechConversion(MainV2.getConfig("speechcustom")));
                            }

                            speechcustomtime = DateTime.Now;
                        }
                    }

                    // speech for battery alerts
                    if (speechEnable && speechEngine != null && (DateTime.Now - speechbatterytime).TotalSeconds > 30 && (MainV2.comPort.logreadmode || comPort.BaseStream.IsOpen))
                    {
                        //speechbatteryvolt
                        float warnvolt = 0;
                        float.TryParse(MainV2.getConfig("speechbatteryvolt"), out warnvolt);
                        float warnpercent = 0;
                        float.TryParse(MainV2.getConfig("speechbatterypercent"), out warnpercent);

                        if (MainV2.getConfig("speechbatteryenabled") == "True" && MainV2.comPort.MAV.cs.battery_voltage <= warnvolt && MainV2.comPort.MAV.cs.battery_voltage >= 5.0)
                        {
                            if (MainV2.speechEngine.State == SynthesizerState.Ready)
                            {
                                MainV2.speechEngine.SpeakAsync(Common.speechConversion(MainV2.getConfig("speechbattery")));
                                speechbatterytime = DateTime.Now;
                            }
                        }
                        else if (MainV2.getConfig("speechbatteryenabled") == "True" && (MainV2.comPort.MAV.cs.battery_remaining) < warnpercent && MainV2.comPort.MAV.cs.battery_voltage >= 5.0 && MainV2.comPort.MAV.cs.battery_remaining != 0.0)
                        {
                            if (MainV2.speechEngine.State == SynthesizerState.Ready)
                            {
                                MainV2.speechEngine.SpeakAsync(Common.speechConversion(MainV2.getConfig("speechbattery")));
                                speechbatterytime = DateTime.Now;
                            }
                        }
                    }

                    // speech for airspeed alerts
                    if (speechEnable && speechEngine != null && (DateTime.Now - speechlowspeedtime).TotalSeconds > 10 && (MainV2.comPort.logreadmode || comPort.BaseStream.IsOpen))
                    {
                        if (MainV2.getConfig("speechlowspeedenabled") == "True")
                        {
                            float warngroundspeed = 0;
                            float.TryParse(MainV2.getConfig("speechlowgroundspeedtrigger"), out warngroundspeed);
                            float warnairspeed = 0;
                            float.TryParse(MainV2.getConfig("speechlowairspeedtrigger"), out warnairspeed);

                            if (MainV2.comPort.MAV.cs.airspeed < warnairspeed)
                            {
                                if (MainV2.speechEngine.State == SynthesizerState.Ready)
                                {
                                    MainV2.speechEngine.SpeakAsync(Common.speechConversion(MainV2.getConfig("speechlowairspeed")));
                                    speechlowspeedtime = DateTime.Now;
                                }
                            }
                            else if (MainV2.comPort.MAV.cs.groundspeed < warngroundspeed)
                            {
                                if (MainV2.speechEngine.State == SynthesizerState.Ready)
                                {
                                    MainV2.speechEngine.SpeakAsync(Common.speechConversion(MainV2.getConfig("speechlowgroundspeed")));
                                    speechlowspeedtime = DateTime.Now;
                                }
                            }
                            else
                            {
                                speechlowspeedtime = DateTime.Now;
                            }
                        }
                    }

                    // speech altitude warning - message high warning
                    if (speechEnable && speechEngine != null && (MainV2.comPort.logreadmode || comPort.BaseStream.IsOpen))
                    {
                        float warnalt = float.MaxValue;
                        float.TryParse(MainV2.getConfig("speechaltheight"), out warnalt);
                        try
                        {
                            if (MainV2.getConfig("speechaltenabled") == "True" && MainV2.comPort.MAV.cs.alt != 0.00 && (MainV2.comPort.MAV.cs.altasl - (int)double.Parse(MainV2.getConfig("TXT_homealt"))) <= warnalt)
                            {
                                if (MainV2.speechEngine.State == SynthesizerState.Ready)
                                    MainV2.speechEngine.SpeakAsync(Common.speechConversion(MainV2.getConfig("speechalt")));
                            }
                        }
                        catch { } // silent fail


                        try
                        {
                            if (MainV2.speechEngine.State == SynthesizerState.Ready && lastmessagehigh != MainV2.comPort.MAV.cs.messageHigh)
                            {
                                MainV2.speechEngine.SpeakAsync(MainV2.comPort.MAV.cs.messageHigh);
                                lastmessagehigh = MainV2.comPort.MAV.cs.messageHigh;
                            }
                        }
                        catch { }
                    }

                    // make sure we attenuate the link quality if we dont see any valid packets
                    if ((DateTime.Now - comPort.lastvalidpacket).TotalSeconds > 10)
                    {
                        MainV2.comPort.MAV.cs.linkqualitygcs = 0;
                    }

                    // attenuate the link qualty over time
                    if ((DateTime.Now - comPort.lastvalidpacket).TotalSeconds >= 1)
                    {
                        if (linkqualitytime.Second != DateTime.Now.Second)
                        {
                            MainV2.comPort.MAV.cs.linkqualitygcs = (ushort)(MainV2.comPort.MAV.cs.linkqualitygcs * 0.8f);
                            linkqualitytime = DateTime.Now;

                            // force redraw is no other packets are being read
                            GCSViews.FlightData.myhud.Invalidate();
                        }
                    }

                    // data loss warning - wait min of 10 seconds, ignore first 30 seconds of connect, repeat at 5 seconds interval
                    if ((DateTime.Now - comPort.lastvalidpacket).TotalSeconds > 10
                        && (DateTime.Now - connecttime).TotalSeconds > 30 
                        && (DateTime.Now - nodatawarning).TotalSeconds > 5 
                        && (MainV2.comPort.logreadmode || comPort.BaseStream.IsOpen)
                        && MainV2.comPort.MAV.cs.armed)
                    {
                        if (speechEnable && speechEngine != null)
                        {
                            if (MainV2.speechEngine.State == SynthesizerState.Ready)
                            {
                                MainV2.speechEngine.SpeakAsync("WARNING No Data for " + (int)(DateTime.Now - comPort.lastvalidpacket).TotalSeconds + " Seconds");
                                nodatawarning = DateTime.Now;
                            }
                        }
                    }

                    // get home point on armed status change.
                    if (armedstatus != MainV2.comPort.MAV.cs.armed && comPort.BaseStream.IsOpen)
                    {
                        armedstatus = MainV2.comPort.MAV.cs.armed;
                        // status just changed to armed
                        if (MainV2.comPort.MAV.cs.armed == true)
                        {
                            try
                            {
                                MainV2.comPort.MAV.cs.HomeLocation = new PointLatLngAlt(MainV2.comPort.getWP(0));
                                if (MyView.current != null && MyView.current.Name == "FlightPlanner")
                                {
                                    // update home if we are on flight data tab
                                    FlightPlanner.updateHome();
                                }
                            }
                            catch
                            {
                                // dont hang this loop
                                this.BeginInvoke((MethodInvoker)delegate { CustomMessageBox.Show("Failed to update home location"); });
                            }
                        }

                        if (speechEnable && speechEngine != null)
                        {
                            if (MainV2.getConfig("speecharmenabled") == "True")
                            {
                                if (armedstatus)
                                    MainV2.speechEngine.SpeakAsync(Common.speechConversion(MainV2.getConfig("speecharm")));
                                else
                                    MainV2.speechEngine.SpeakAsync(Common.speechConversion(MainV2.getConfig("speechdisarm")));
                            }
                        }
                    }

                    // send a hb every seconds from gcs to ap
                    if (heatbeatSend.Second != DateTime.Now.Second)
                    {
                        MAVLink.mavlink_heartbeat_t htb = new MAVLink.mavlink_heartbeat_t()
                        {
                            type = (byte)MAVLink.MAV_TYPE.GCS,
                            autopilot = (byte)MAVLink.MAV_AUTOPILOT.INVALID,
                            mavlink_version = 3,
                        };

                        comPort.sendPacket(htb);

                        foreach (var port in MainV2.Comports)
                        {
                            if (port == MainV2.comPort)
                                continue;
                            try
                            {
                                port.sendPacket(htb);
                            }
                            catch { }
                        }

                        heatbeatSend = DateTime.Now;
                    }

                    // if not connected or busy, sleep and loop
                    if (!comPort.BaseStream.IsOpen || comPort.giveComport == true)
                    {
                        if (!comPort.BaseStream.IsOpen)
                        {
                            // check if other ports are still open
                            foreach (var port in Comports)
                            {
                                if (port.BaseStream.IsOpen)
                                {
                                    Console.WriteLine("Main comport shut, swapping to other mav");
                                    comPort = port;
                                    break;
                                }
                            }
                        }

                        System.Threading.Thread.Sleep(100);
                        continue;
                    }

                    // actualy read the packets
                    while (comPort.BaseStream.IsOpen && comPort.BaseStream.BytesToRead > minbytes && comPort.giveComport == false)
                    {
                        try
                        {
                            comPort.readPacket();
                        }
                        catch { }
                    }

                    // update currentstate of main port
                    try
                    {
                        comPort.MAV.cs.UpdateCurrentSettings(null, false, comPort);
                    }
                    catch { }

                    // read the other interfaces
                    foreach (var port in Comports)
                    {
                        if (!port.BaseStream.IsOpen)
                        {
                            // modify array and drop out
                            Comports.Remove(port);
                            break;
                        }
                        // skip primary interface
                        if (port == comPort)
                            continue;
                        while (port.BaseStream.IsOpen && port.BaseStream.BytesToRead > minbytes)
                        {
                            try
                            {
                                port.readPacket();
                            }
                            catch { }
                        }
                        // update currentstate of port
                        try
                        {
                            port.MAV.cs.UpdateCurrentSettings(null, false, port);
                        }
                        catch { }
                    }
                }
                catch (Exception e)
                {
                    log.Error("Serial Reader fail :" + e.ToString());
                    try
                    {
                        comPort.Close();
                    }
                    catch { }
                }
            }

            Console.WriteLine("SerialReader Done");
            SerialThreadrunner.Set();
        }

        private void MainV2_Load(object sender, EventArgs e)
        {
            // check if its defined, and force to show it if not known about
            if (config["menu_autohide"] == null)
            {
                config["menu_autohide"] = "false";
            }

            try
            {
                AutoHideMenu(bool.Parse(config["menu_autohide"].ToString()));
            }
            catch { }

            MyView.AddScreen(new MainSwitcher.Screen("FlightData", FlightData, true));
            MyView.AddScreen(new MainSwitcher.Screen("FlightPlanner", FlightPlanner, true));
            MyView.AddScreen(new MainSwitcher.Screen("HWConfig", new GCSViews.InitialSetup(), false));
            MyView.AddScreen(new MainSwitcher.Screen("SWConfig", new GCSViews.SoftwareConfig(), false));
            MyView.AddScreen(new MainSwitcher.Screen("Simulation", Simulation, true));
            MyView.AddScreen(new MainSwitcher.Screen("Terminal", new GCSViews.Terminal(), false));
            MyView.AddScreen(new MainSwitcher.Screen("Help", new GCSViews.Help(), false));

            // init button depressed - ensures correct action
            //int fixme;

            this.SuspendLayout();

            MenuFlightData_Click(sender, e);
            MainMenu_ItemClicked(sender, new ToolStripItemClickedEventArgs(MenuFlightData));

            this.ResumeLayout();

            // for long running tasks using own threads.
            // for short use threadpool

            // setup http server
            try
            {

                httpthread = new Thread(new httpserver().listernforclients)
                {
                    Name = "motion jpg stream-network kml",
                    IsBackground = true
                }; 
                httpthread.Start();
            }
            catch (Exception ex)
            {
                log.Error("Error starting TCP listener thread: ", ex);
                CustomMessageBox.Show(ex.ToString());
            }

            /// setup joystick packet sender
            joystickthread = new Thread(new ThreadStart(joysticksend))
             {
                 IsBackground = true,
                 Priority = ThreadPriority.AboveNormal,
                 Name = "Main joystick sender"
             }; 
            joystickthread.Start();

            // setup main serial reader
            serialreaderthread = new Thread(SerialReader)
            {
                IsBackground = true,
                Name = "Main Serial reader",
                Priority = ThreadPriority.AboveNormal
            }; 
            serialreaderthread.Start();

            // setup main plugin thread
            pluginthread = new Thread(PluginThread)
            {
                IsBackground = true,
                Name = "plugin runner thread",
                Priority = ThreadPriority.BelowNormal
            }; 
            pluginthread.Start();


            try
            {
                // if (!System.Diagnostics.Debugger.IsAttached)
                {
                    // single update check per day - in a seperate thread
                    if (getConfig("update_check") != DateTime.Now.ToShortDateString())
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(checkupdate);
                        config["update_check"] = DateTime.Now.ToShortDateString();
                    }
                    else if (getConfig("beta_updates") == "True")
                    {
                        MissionPlanner.Utilities.Update.dobeta = true;
                        System.Threading.ThreadPool.QueueUserWorkItem(checkupdate);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Update check failed", ex);
            }

            try
            {
                Plugin.PluginLoader.LoadAll();
            }
            catch (Exception ex) { log.Error(ex); }

            Program.Splash.Close();

            MissionPlanner.Utilities.Tracking.AddTiming("AppLoad", "Load Time", (DateTime.Now - Program.starttime).TotalMilliseconds, "");

            // play a tlog that was passed to the program
            if (Program.args.Length > 0)
            {
                if (File.Exists(Program.args[0]) && Program.args[0].ToLower().Contains(".tlog"))
                {
                    FlightData.LoadLogFile(Program.args[0]);
                    FlightData.BUT_playlog_Click(null,null);
                }
            }


            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    try
                    {
                        MissionPlanner.Log.LogSort.SortLogs(Directory.GetFiles(MainV2.LogDir, "*.tlog"));
                    }
                    catch { }
                }
                );
            }
            catch { }

/*
            if (getConfig("newuser") == "")
            {
                if (CustomMessageBox.Show("This is your first run, Do you wish to use the setup wizard?\nRecomended for new users.", "Wizard", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    Wizard.Wizard wiz = new Wizard.Wizard();

                    wiz.ShowDialog(this);

                    CustomMessageBox.Show("To use the wizard again please goto the help screen, and click the wizard icon.", "Wizard");
                }
                else
                {
                    CustomMessageBox.Show("To use the wizard please goto the help screen, and click the wizard icon.", "Wizard");
                }

                config["newuser"] = DateTime.Now.ToShortDateString();
            }
*/
        }

        private void checkupdate(object stuff)
        {
            try
            {
                MissionPlanner.Utilities.Update.CheckForUpdate();
            }
            catch (Exception ex)
            {
                log.Error("Update check failed", ex);
            }
        }

        private void TOOL_APMFirmware_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainV2.comPort.MAV.cs.firmware = (MainV2.Firmwares)Enum.Parse(typeof(MainV2.Firmwares), _connectionControl.TOOL_APMFirmware.Text);
        }

        private void MainV2_Resize(object sender, EventArgs e)
        {
            // mono - resize is called before the control is created
            if (MyView != null)
                log.Info("myview width " + MyView.Width + " height " + MyView.Height);

            log.Info("this   width " + this.Width + " height " + this.Height);
        }

        private void MenuHelp_Click(object sender, EventArgs e)
        {
            MyView.ShowScreen("Help");
        }



        /// <summary>
        /// keyboard shortcuts override
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F12)
            {
                MenuConnect_Click(null, null);
                return true;
            }

            if (keyData == Keys.F2)
            {
                MenuFlightData_Click(null,null);
                return true;
            }
            if (keyData == Keys.F3)
            {
                MenuFlightPlanner_Click(null, null);
                return true;
            }
            if (keyData == Keys.F4)
            {
                MenuTuning_Click(null, null);
                return true;
            }

            if (keyData == Keys.F5)
            {
                comPort.getParamList();
                MyView.ShowScreen(MyView.current.Name);
                return true;
            }

            if (keyData == (Keys.Control | Keys.F)) // temp
            {
                Form frm = new temp();
                ThemeManager.ApplyThemeTo(frm);
                frm.Show();
                return true;
            }
            /*if (keyData == (Keys.Control | Keys.S)) // screenshot
            {
                ScreenShot();
                return true;
            }*/
            if (keyData == (Keys.Control | Keys.G)) // nmea out
            {
                Form frm = new SerialOutputNMEA();
                ThemeManager.ApplyThemeTo(frm);
                frm.Show();
                return true;
            }
            if (keyData == (Keys.Control | Keys.L)) // limits
            {
                Form temp = new Form();
                Control frm = new GCSViews.ConfigurationView.ConfigAP_Limits();
                temp.Controls.Add(frm);
                temp.Size = frm.Size;
                frm.Dock = DockStyle.Fill;
                ThemeManager.ApplyThemeTo(temp);
                temp.Show();
                return true;
            }
            if (keyData == (Keys.Control | Keys.W)) // test ac config
            {
                Wizard.Wizard cfg = new Wizard.Wizard();

                cfg.ShowDialog(this);
                
                return true;
            }
            if (keyData == (Keys.Control | Keys.Z)) // test ac config
            {
                MissionPlanner.GenOTP otp = new MissionPlanner.GenOTP();

                otp.ShowDialog(this);

                return true;
            }
            if (keyData == (Keys.Control | Keys.T)) // for override connect
            {
                try
                {
                    MainV2.comPort.Open(false);
                }
                catch (Exception ex) { CustomMessageBox.Show(ex.ToString()); }
                return true;
            }
            if (keyData == (Keys.Control | Keys.Y)) // for ryan beall
            {
                // write
                try
                {
                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_STORAGE, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
                }
                catch { CustomMessageBox.Show("Invalid command"); return true; }
                //read
                ///////MainV2.comPort.doCommand(MAVLink09.MAV_CMD.PREFLIGHT_STORAGE, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
                CustomMessageBox.Show("Done MAV_ACTION_STORAGE_WRITE");
                return true;
            }
            if (keyData == (Keys.Control | Keys.J)) // for jani
            {
                string data = "!!";
                if (System.Windows.Forms.DialogResult.OK == InputBox.Show("inject", "enter data to be written", ref data))
                {
                    MainV2.comPort.Write(data + "\r");
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void changelanguage(CultureInfo ci)
        {
            if (ci != null && !Thread.CurrentThread.CurrentUICulture.Equals(ci))
            {
                Thread.CurrentThread.CurrentUICulture = ci;
                config["language"] = ci.Name;
                //System.Threading.Thread.CurrentThread.CurrentCulture = ci;

                HashSet<Control> views = new HashSet<Control> { FlightData, FlightPlanner, Simulation };

                foreach (Control view in MyView.Controls)
                    views.Add(view);

                foreach (Control view in views)
                {
                    if (view != null)
                    {
                        ComponentResourceManager rm = new ComponentResourceManager(view.GetType());
                        foreach (Control ctrl in view.Controls)
                            rm.ApplyResource(ctrl);
                        rm.ApplyResources(view, "$this");
                    }
                }
            }
        }

        private void MainV2_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.Info("MainV2_FormClosing");

            config["MainHeight"] = this.Height;
            config["MainWidth"] = this.Width;
            config["MainMaximised"] = this.WindowState.ToString();

            config["MainLocX"] = this.Location.X.ToString();
            config["MainLocY"] = this.Location.Y.ToString();

            try
            {
                comPort.logreadmode = false;
                if (comPort.logfile != null)
                    comPort.logfile.Close();

                if (comPort.rawlogfile != null)
                    comPort.rawlogfile.Close();

                comPort.logfile = null;
                comPort.rawlogfile = null;
            }
            catch { }

            // shutdown threads
            GCSViews.FlightData.threadrun = 0;

            log.Info("closing pluginthread");

            pluginthreadrun = false;

            PluginThreadrunner.WaitOne();

            log.Info("closing serialthread");

            // shutdown local thread
            serialThread = false;

            SerialThreadrunner.WaitOne();

            log.Info("sorting tlogs");
            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    try
                    {
                        MissionPlanner.Log.LogSort.SortLogs(Directory.GetFiles(MainV2.LogDir, "*.tlog"));
                    }
                    catch { }
                }
                );
            }
            catch { }

            log.Info("closing MyView");

            // close all tabs
            MyView.Dispose();

            log.Info("closing fd");
            try
            {
                FlightData.Dispose();
            }
            catch { }
            log.Info("closing fp");
            try
            {
                FlightPlanner.Dispose();
            }
            catch { }
            log.Info("closing sim");
            try
            {
                Simulation.Dispose();
            }
            catch { }

            try
            {
                if (comPort.BaseStream.IsOpen)
                    comPort.Close();
            }
            catch { } // i get alot of these errors, the port is still open, but not valid - user has unpluged usb

            log.Info("save config");
            // save config
            xmlconfig(true);
            
            Console.WriteLine(httpthread.IsAlive);
            Console.WriteLine(joystickthread.IsAlive);
            Console.WriteLine(serialreaderthread.IsAlive);
            Console.WriteLine(pluginthread.IsAlive);

            log.Info("MainV2_FormClosing done");

            if (MONO)
                this.Dispose();
        }

        public static string getConfig(string paramname)
        {
            if (config[paramname] != null)
                return config[paramname].ToString();
            return "";
        }

        public void ChangeUnits()
        {
            try
            {
                // dist
                if (MainV2.config["distunits"] != null)
                {
                    switch ((Common.distances)Enum.Parse(typeof(Common.distances), MainV2.config["distunits"].ToString()))
                    {
                        case Common.distances.Meters:
                            MainV2.comPort.MAV.cs.multiplierdist = 1;
                            MainV2.comPort.MAV.cs.DistanceUnit = "m";
                            break;
                        case Common.distances.Feet:
                            MainV2.comPort.MAV.cs.multiplierdist = 3.2808399f;
                            MainV2.comPort.MAV.cs.DistanceUnit = "ft";
                            break;
                    }
                }
                else
                {
                    MainV2.comPort.MAV.cs.multiplierdist = 1;
                    MainV2.comPort.MAV.cs.DistanceUnit = "m";
                }

                // speed
                if (MainV2.config["speedunits"] != null)
                {
                    switch ((Common.speeds)Enum.Parse(typeof(Common.speeds), MainV2.config["speedunits"].ToString()))
                    {
                        case Common.speeds.ms:
                            MainV2.comPort.MAV.cs.multiplierspeed = 1;
                            MainV2.comPort.MAV.cs.SpeedUnit = "m/s";
                            break;
                        case Common.speeds.fps:
                            MainV2.comPort.MAV.cs.multiplierdist = 3.2808399f;
                            MainV2.comPort.MAV.cs.SpeedUnit = "fps";
                            break;
                        case Common.speeds.kph:
                            MainV2.comPort.MAV.cs.multiplierspeed = 3.6f;
                            MainV2.comPort.MAV.cs.SpeedUnit = "kph";
                            break;
                        case Common.speeds.mph:
                            MainV2.comPort.MAV.cs.multiplierspeed = 2.23693629f;
                            MainV2.comPort.MAV.cs.SpeedUnit = "mph";
                            break;
                        case Common.speeds.knots:
                            MainV2.comPort.MAV.cs.multiplierspeed = 1.94384449f;
                            MainV2.comPort.MAV.cs.SpeedUnit = "knots";
                            break;
                    }
                }
                else
                {
                    MainV2.comPort.MAV.cs.multiplierspeed = 1;
                    MainV2.comPort.MAV.cs.SpeedUnit = "m/s";
                }
            }
            catch { }

        }

        private void CMB_baudrate_TextChanged(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            int baud = 0;
            for (int i = 0; i < _connectionControl.CMB_baudrate.Text.Length; i++)
                if (char.IsDigit(_connectionControl.CMB_baudrate.Text[i]))
                {
                    sb.Append(_connectionControl.CMB_baudrate.Text[i]);
                    baud = baud * 10 + _connectionControl.CMB_baudrate.Text[i] - '0';
                }
            if (_connectionControl.CMB_baudrate.Text != sb.ToString())
            {
                _connectionControl.CMB_baudrate.Text = sb.ToString();
            }
            try
            {
                if (baud > 0 && comPort.BaseStream.BaudRate != baud)
                    comPort.BaseStream.BaudRate = baud;
            }
            catch (Exception)
            {
            }
        }

        private void CMB_serialport_Enter(object sender, EventArgs e)
        {
            CMB_serialport_Click(sender, e);
        }

        private void MainMenu_MouseLeave(object sender, EventArgs e)
        {
            if (_connectionControl.PointToClient(Control.MousePosition).Y < MainMenu.Height)
                return;            

            this.SuspendLayout();

            panel1.Visible = false;

            this.ResumeLayout();
        }

        void menu_MouseEnter(object sender, EventArgs e)
        {
            this.SuspendLayout();
            panel1.Location = new Point(0,0);
            panel1.Width = menu.Width;
            panel1.BringToFront();
            panel1.Visible = true;
            this.ResumeLayout();
        }

        private void autoHideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoHideMenu(autoHideToolStripMenuItem.Checked);

            config["menu_autohide"] = autoHideToolStripMenuItem.Checked.ToString();
        }

        void AutoHideMenu(bool hide)
        {
            autoHideToolStripMenuItem.Checked = hide;

            if (!hide)
            {
                this.SuspendLayout();
                panel1.Dock = DockStyle.Top;
                panel1.SendToBack();
                panel1.Visible = true;
                menu.Visible = false;
                MainMenu.MouseLeave -= MainMenu_MouseLeave;
                panel1.MouseLeave -= MainMenu_MouseLeave;
                toolStripConnectionControl.MouseLeave -= MainMenu_MouseLeave;
                this.ResumeLayout();
            }
            else
            {
                this.SuspendLayout();
                panel1.Dock = DockStyle.None;
                panel1.Visible = false;
                MainMenu.MouseLeave += MainMenu_MouseLeave;
                panel1.MouseLeave += MainMenu_MouseLeave;
                toolStripConnectionControl.MouseLeave += MainMenu_MouseLeave;
                menu.Visible = true;
                menu.SendToBack();
                this.ResumeLayout();
            }
        }

        private void MainV2_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=mich146%40hotmail%2ecom&lc=AU&item_name=Michael%20Oborne&no_note=0&currency_code=AUD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHostedGuest");
            }
            catch { CustomMessageBox.Show("Link open failed. check your default webpage association"); }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_HDR
        {
            internal Int32 dbch_size;
            internal Int32 dbch_devicetype;
            internal Int32 dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class DEV_BROADCAST_PORT
        {
            public int dbcp_size;
            public int dbcp_devicetype;
            public int dbcp_reserved; // MSDN say "do not use"
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
            public byte[] dbcp_name;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class DEV_BROADCAST_DEVICEINTERFACE
        {
            public Int32 dbcc_size;
            public Int32 dbcc_devicetype;
            public Int32 dbcc_reserved;
            [MarshalAs(UnmanagedType.ByValArray,            ArraySubType = UnmanagedType.U1,            SizeConst = 16)]
            internal Byte[] dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray,             SizeConst = 255)]
            internal Byte[] dbcc_name;
        }



        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CREATE:
                    try
                    {
                        DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE();
                        IntPtr devBroadcastDeviceInterfaceBuffer;
                        IntPtr deviceNotificationHandle = IntPtr.Zero;
                        Int32 size = 0;

                        // frmMy is the form that will receive device-change messages.


                        size = Marshal.SizeOf(devBroadcastDeviceInterface);
                        devBroadcastDeviceInterface.dbcc_size = size;
                        devBroadcastDeviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
                        devBroadcastDeviceInterface.dbcc_reserved = 0;
                        devBroadcastDeviceInterface.dbcc_classguid = GUID_DEVINTERFACE_USB_DEVICE.ToByteArray();
                        devBroadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal(size);
                        Marshal.StructureToPtr(devBroadcastDeviceInterface, devBroadcastDeviceInterfaceBuffer, true);


                        deviceNotificationHandle = NativeMethods.RegisterDeviceNotification(this.Handle, devBroadcastDeviceInterfaceBuffer, DEVICE_NOTIFY_WINDOW_HANDLE);
                    }
                    catch { }

                    break;

                case WM_DEVICECHANGE:
                    // The WParam value identifies what is occurring.
                    WM_DEVICECHANGE_enum n = (WM_DEVICECHANGE_enum)m.WParam;
                     int l = (int)m.LParam;
                     if (n == WM_DEVICECHANGE_enum.DBT_DEVICEREMOVEPENDING)
                     {
                         Console.WriteLine("DBT_DEVICEREMOVEPENDING");
                     }
                     if (n == WM_DEVICECHANGE_enum.DBT_DEVNODES_CHANGED)
                     {
                         Console.WriteLine("DBT_DEVNODES_CHANGED");
                     }
                     if (n == WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL || n == WM_DEVICECHANGE_enum.DBT_DEVICEREMOVECOMPLETE)
                     {
                         Console.WriteLine(((WM_DEVICECHANGE_enum)n).ToString());

                         DEV_BROADCAST_HDR hdr = new DEV_BROADCAST_HDR();
                         Marshal.PtrToStructure(m.LParam, hdr);

                         try
                         {

                             switch (hdr.dbch_devicetype)
                             {
                                 case DBT_DEVTYP_DEVICEINTERFACE:
                                     DEV_BROADCAST_DEVICEINTERFACE inter = new DEV_BROADCAST_DEVICEINTERFACE();
                                     Marshal.PtrToStructure(m.LParam, inter);
                                     log.InfoFormat("Interface {0}", ASCIIEncoding.Unicode.GetString(inter.dbcc_name, 0, inter.dbcc_size - (4 * 3)));
                                     break;
                                 case DBT_DEVTYP_PORT:
                                     DEV_BROADCAST_PORT prt = new DEV_BROADCAST_PORT();
                                     Marshal.PtrToStructure(m.LParam, prt);
                                     log.InfoFormat("port {0}", ASCIIEncoding.Unicode.GetString(prt.dbcp_name, 0, prt.dbcp_size - (4 * 3)));
                                     break;
                             }

                         }
                         catch { }

                         //string port = Marshal.PtrToStringAuto((IntPtr)((long)m.LParam + 12));
                         //Console.WriteLine("Added port {0}",port);
                     }
                     log.InfoFormat("Device Change {0} {1} {2}", m.Msg, (WM_DEVICECHANGE_enum)m.WParam, m.LParam);

                     if (DeviceChanged != null)
                     {
                         try
                         {
                             DeviceChanged((WM_DEVICECHANGE_enum)m.WParam);


                         }
                         catch { }
                     }

                     foreach (Plugin.Plugin item in MissionPlanner.Plugin.PluginLoader.Plugins)
                     {
                         item.Host.ProcessDeviceChanged((WM_DEVICECHANGE_enum)m.WParam);
                     }

                    break;
                default:

                    break;
            }
            base.WndProc(ref m);
        }

        const int DBT_DEVTYP_PORT = 0x00000003;
        const int WM_CREATE           =            0x0001;
        const Int32 DBT_DEVTYP_HANDLE = 6;
        const Int32 DBT_DEVTYP_DEVICEINTERFACE = 5;
        const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        const Int32 DIGCF_PRESENT = 2;
        const Int32 DIGCF_DEVICEINTERFACE = 0X10;
        const Int32 WM_DEVICECHANGE = 0X219;
        public static Guid        GUID_DEVINTERFACE_USB_DEVICE = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");




        public enum WM_DEVICECHANGE_enum
        {
            DBT_CONFIGCHANGECANCELED = 0x19,
            DBT_CONFIGCHANGED = 0x18,
            DBT_CUSTOMEVENT = 0x8006,
            DBT_DEVICEARRIVAL = 0x8000,
            DBT_DEVICEQUERYREMOVE = 0x8001,
            DBT_DEVICEQUERYREMOVEFAILED = 0x8002,
            DBT_DEVICEREMOVECOMPLETE = 0x8004,
            DBT_DEVICEREMOVEPENDING = 0x8003,
            DBT_DEVICETYPESPECIFIC = 0x8005,
            DBT_DEVNODES_CHANGED = 0x7,
            DBT_QUERYCHANGECONFIG = 0x17,
            DBT_USERDEFINED = 0xFFFF,
        }

        private void MainMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripItem item in MainMenu.Items)
            {
                if (e.ClickedItem == item)
                {
                    item.BackColor = ThemeManager.ControlBGColor;
                }
                else
                {
                    item.BackColor = Color.Transparent;
                    item.BackgroundImage = MissionPlanner.Properties.Resources.bgdark;//.BackColor = Color.Black;
                }
            }
            //MainMenu.BackColor = Color.Black;
            //MainMenu.BackgroundImage = MissionPlanner.Properties.Resources.bgdark;
        }
    }
}