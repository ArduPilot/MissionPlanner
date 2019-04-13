using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Linq;
using System.Threading;
using MissionPlanner.Utilities;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Comms;
using MissionPlanner.Log;
using Transitions;
using MissionPlanner.Warnings;
using System.Collections.Concurrent;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities.AltitudeAngel;
using System.Threading.Tasks;
using GMap.NET.WindowsForms;
using SkiaSharp;

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

            // Import SetThreadExecutionState Win32 API and necessary flags

            [DllImport("kernel32.dll")]
            public static extern uint SetThreadExecutionState(uint esFlags);

            public const uint ES_CONTINUOUS = 0x80000000;
            public const uint ES_SYSTEM_REQUIRED = 0x00000001;

            static public int SW_SHOWNORMAL = 1;
            static public int SW_HIDE = 0;

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

            [System.Flags]
            public enum LoadLibraryFlags : uint
            {
                DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
                LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
                LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
                LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
                LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
                LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
                LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
                LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
                LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
                LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
                LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
            }
        }

        public static menuicons displayicons = new burntkermitmenuicons();

        public abstract class menuicons
        {
            public abstract Image fd { get; }
            public abstract Image fp { get; }
            public abstract Image initsetup { get; }
            public abstract Image config_tuning { get; }
            public abstract Image sim { get; }
            public abstract Image terminal { get; }
            public abstract Image help { get; }
            public abstract Image donate { get; }
            public abstract Image connect { get; }
            public abstract Image disconnect { get; }
            public abstract Image bg { get; }
            public abstract Image wizard { get; }
        }

        public class burntkermitmenuicons : menuicons
        {
            public override Image fd
            {
                get { return global::MissionPlanner.Properties.Resources.light_flightdata_icon; }
            }

            public override Image fp
            {
                get { return global::MissionPlanner.Properties.Resources.light_flightplan_icon; }
            }

            public override Image initsetup
            {
                get { return global::MissionPlanner.Properties.Resources.light_initialsetup_icon; }
            }

            public override Image config_tuning
            {
                get { return global::MissionPlanner.Properties.Resources.light_tuningconfig_icon; }
            }

            public override Image sim
            {
                get { return global::MissionPlanner.Properties.Resources.light_simulation_icon; }
            }

            public override Image terminal
            {
                get { return global::MissionPlanner.Properties.Resources.light_terminal_icon; }
            }

            public override Image help
            {
                get { return global::MissionPlanner.Properties.Resources.light_help_icon; }
            }

            public override Image donate
            {
                get { return global::MissionPlanner.Properties.Resources.donate; }
            }

            public override Image connect
            {
                get { return global::MissionPlanner.Properties.Resources.light_connect_icon; }
            }

            public override Image disconnect
            {
                get { return global::MissionPlanner.Properties.Resources.light_disconnect_icon; }
            }

            public override Image bg
            {
                get { return global::MissionPlanner.Properties.Resources.bgdark; }
            }
            public override Image wizard
            {
                get { return global::MissionPlanner.Properties.Resources.wizardicon; }
            }
        }

        public class highcontrastmenuicons : menuicons
        {
            public override Image fd
            {
                get { return global::MissionPlanner.Properties.Resources.dark_flightdata_icon; }
            }

            public override Image fp
            {
                get { return global::MissionPlanner.Properties.Resources.dark_flightplan_icon; }
            }

            public override Image initsetup
            {
                get { return global::MissionPlanner.Properties.Resources.dark_initialsetup_icon; }
            }

            public override Image config_tuning
            {
                get { return global::MissionPlanner.Properties.Resources.dark_tuningconfig_icon; }
            }

            public override Image sim
            {
                get { return global::MissionPlanner.Properties.Resources.dark_simulation_icon; }
            }

            public override Image terminal
            {
                get { return global::MissionPlanner.Properties.Resources.dark_terminal_icon; }
            }

            public override Image help
            {
                get { return global::MissionPlanner.Properties.Resources.dark_help_icon; }
            }

            public override Image donate
            {
                get { return global::MissionPlanner.Properties.Resources.donate; }
            }

            public override Image connect
            {
                get { return global::MissionPlanner.Properties.Resources.dark_connect_icon; }
            }

            public override Image disconnect
            {
                get { return global::MissionPlanner.Properties.Resources.dark_disconnect_icon; }
            }

            public override Image bg
            {
                get { return null; }
            }
            public override Image wizard
            {
                get { return global::MissionPlanner.Properties.Resources.wizardicon; }
            }
        }

        Controls.MainSwitcher MyView;

        private static DisplayView _displayConfiguration = new DisplayView().Advanced();

        public static event EventHandler LayoutChanged;

        public static DisplayView DisplayConfiguration
        {
            get { return _displayConfiguration; }
            set
            {
                _displayConfiguration = value;
                Settings.Instance["displayview"] = _displayConfiguration.ConvertToString();
                LayoutChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        
        public static bool ShowAirports { get; set; }
        public static bool ShowTFR { get; set; }

        private Utilities.adsb _adsb;

        public bool EnableADSB
        {
            get { return _adsb != null; }
            set
            {
                if (value == true)
                {
                    _adsb = new Utilities.adsb();

                    if (Settings.Instance["adsbserver"] != null)
                        Utilities.adsb.server = Settings.Instance["adsbserver"];
                    if (Settings.Instance["adsbport"] != null)
                        Utilities.adsb.serverport = int.Parse(Settings.Instance["adsbport"].ToString());
                }
                else
                {
                    Utilities.adsb.Stop();
                    _adsb = null;
                }
            }
        }

        //public static event EventHandler LayoutChanged;

        /// <summary>
        /// Active Comport interface
        /// </summary>
        public static MAVLinkInterface comPort
        {
            get
            {
                return _comPort;
            }
            set {
                if (_comPort == value)
                    return;
                _comPort = value;
                _comPort.MavChanged -= instance.comPort_MavChanged;
                _comPort.MavChanged += instance.comPort_MavChanged;
                instance.comPort_MavChanged(null, null);
            }
        }

        static MAVLinkInterface _comPort = new MAVLinkInterface();

        /// <summary>
        /// passive comports
        /// </summary>
        public static List<MAVLinkInterface> Comports = new List<MAVLinkInterface>();

        public delegate void WMDeviceChangeEventHandler(WM_DEVICECHANGE_enum cause);

        public event WMDeviceChangeEventHandler DeviceChanged;

        /// <summary>
        /// other planes in the area from adsb
        /// </summary>
        public object adsblock = new object();

        public ConcurrentDictionary<string,adsb.PointLatLngAltHdg> adsbPlanes = new ConcurrentDictionary<string, adsb.PointLatLngAltHdg>();

        string titlebar;

        /// <summary>
        /// Comport name
        /// </summary>
        public static string comPortName = "";

        public static int comPortBaud = 115200;

        /// <summary>
        /// mono detection
        /// </summary>
        public static bool MONO = false;

        /// <summary>
        /// speech engine enable
        /// </summary>
        public static bool speechEnable
        {
            get { return speechEngine == null ? false : speechEngine.speechEnable; }
            set
            {
                if (speechEngine != null) speechEngine.speechEnable = value;
            }
        }

        /// <summary>
        /// spech engine static class
        /// </summary>
        public static ISpeech speechEngine { get; set; }

        /// <summary>
        /// joystick static class
        /// </summary>
        public static Joystick.Joystick joystick { get; set; }

        /// <summary>
        /// track last joystick packet sent. used to control rate
        /// </summary>
        DateTime lastjoystick = DateTime.Now;

        /// <summary>
        /// determine if we are running sitl
        /// </summary>
        public static bool sitl
        {
            get
            {
                if (MissionPlanner.Controls.SITL.SITLSEND == null) return false;
                if (MissionPlanner.Controls.SITL.SITLSEND.Client.Connected) return true;
                return false;
            }
        }

        /// <summary>
        /// hud background image grabber from a video stream - not realy that efficent. ie no hardware overlays etc.
        /// </summary>
        public static WebCamService.Capture cam { get; set; }

        /// <summary>
        /// controls the main serial reader thread
        /// </summary>
        bool serialThread = false;

        bool pluginthreadrun = false;

        bool joystickthreadrun = false;

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


        public static MainSwitcher View;

        /// <summary>
        /// store the time we first connect
        /// </summary>
        DateTime connecttime = DateTime.Now;

        DateTime nodatawarning = DateTime.Now;
        DateTime OpenTime = DateTime.Now;


        DateTime connectButtonUpdate = DateTime.Now;

        /// <summary>
        /// declared here if i want a "single" instance of the form
        /// ie configuration gets reloaded on every click
        /// </summary>
        public GCSViews.FlightData FlightData;

        public GCSViews.FlightPlanner FlightPlanner;
        Controls.SITL Simulation;

        private Form connectionStatsForm;
        private ConnectionStats _connectionStats;

        /// <summary>
        /// This 'Control' is the toolstrip control that holds the comport combo, baudrate combo etc
        /// Otiginally seperate controls, each hosted in a toolstip sqaure, combined into this custom
        /// control for layout reasons.
        /// </summary>
        public static ConnectionControl _connectionControl;

        public static bool TerminalTheming = true;

        public void updateLayout(object sender, EventArgs e)
        {
            MenuSimulation.Visible = DisplayConfiguration.displaySimulation;
            MenuTerminal.Visible = DisplayConfiguration.displayTerminal;
            MenuHelp.Visible = DisplayConfiguration.displayHelp;
            MenuDonate.Visible = DisplayConfiguration.displayDonate;
            MissionPlanner.Controls.BackstageView.BackstageView.Advanced = DisplayConfiguration.isAdvancedMode;

            if (Settings.Instance.GetBoolean("menu_autohide") != DisplayConfiguration.autoHideMenuForce)
            {
                AutoHideMenu(DisplayConfiguration.autoHideMenuForce);
                Settings.Instance["menu_autohide"] = DisplayConfiguration.autoHideMenuForce.ToString();
            }

            autoHideToolStripMenuItem.Visible = !DisplayConfiguration.autoHideMenuForce;

            //Flight data page
            if (MainV2.instance.FlightData != null)
            {
                TabControl t = MainV2.instance.FlightData.tabControlactions;
                if (DisplayConfiguration.displayQuickTab && !t.TabPages.Contains(FlightData.tabQuick))
                {
                    t.TabPages.Add(FlightData.tabQuick);
                }
                else if (!DisplayConfiguration.displayQuickTab && t.TabPages.Contains(FlightData.tabQuick))
                {
                    t.TabPages.Remove(FlightData.tabQuick);
                }
                if (DisplayConfiguration.displayPreFlightTab && !t.TabPages.Contains(FlightData.tabPagePreFlight))
                {
                    t.TabPages.Add(FlightData.tabPagePreFlight);
                }
                else if (!DisplayConfiguration.displayPreFlightTab && t.TabPages.Contains(FlightData.tabPagePreFlight))
                {
                    t.TabPages.Remove(FlightData.tabPagePreFlight);
                }
                if (DisplayConfiguration.displayAdvActionsTab && !t.TabPages.Contains(FlightData.tabActions))
                {
                    t.TabPages.Add(FlightData.tabActions);
                }
                else if (!DisplayConfiguration.displayAdvActionsTab && t.TabPages.Contains(FlightData.tabActions))
                {
                    t.TabPages.Remove(FlightData.tabActions);
                }
                if (DisplayConfiguration.displaySimpleActionsTab && !t.TabPages.Contains(FlightData.tabActionsSimple))
                {
                    t.TabPages.Add(FlightData.tabActionsSimple);
                }
                else if (!DisplayConfiguration.displaySimpleActionsTab && t.TabPages.Contains(FlightData.tabActionsSimple))
                {
                    t.TabPages.Remove(FlightData.tabActionsSimple);
                }
                if (DisplayConfiguration.displayGaugesTab && !t.TabPages.Contains(FlightData.tabGauges))
                {
                    t.TabPages.Add(FlightData.tabGauges);
                }
                else if (!DisplayConfiguration.displayGaugesTab && t.TabPages.Contains(FlightData.tabGauges))
                {
                    t.TabPages.Remove(FlightData.tabGauges);
                }
                if (DisplayConfiguration.displayStatusTab && !t.TabPages.Contains(FlightData.tabStatus))
                {
                    t.TabPages.Add(FlightData.tabStatus);
                }
                else if (!DisplayConfiguration.displayStatusTab && t.TabPages.Contains(FlightData.tabStatus))
                {
                    t.TabPages.Remove(FlightData.tabStatus);
                }
                if (DisplayConfiguration.displayServoTab && !t.TabPages.Contains(FlightData.tabServo))
                {
                    t.TabPages.Add(FlightData.tabServo);
                }
                else if (!DisplayConfiguration.displayServoTab && t.TabPages.Contains(FlightData.tabServo))
                {
                    t.TabPages.Remove(FlightData.tabServo);
                }
                if (DisplayConfiguration.displayScriptsTab && !t.TabPages.Contains(FlightData.tabScripts))
                {
                    t.TabPages.Add(FlightData.tabScripts);
                }
                else if (!DisplayConfiguration.displayScriptsTab && t.TabPages.Contains(FlightData.tabScripts))
                {
                    t.TabPages.Remove(FlightData.tabScripts);
                }
                if (DisplayConfiguration.displayTelemetryTab && !t.TabPages.Contains(FlightData.tabTLogs))
                {
                    t.TabPages.Add(FlightData.tabTLogs);
                }
                else if (!DisplayConfiguration.displayTelemetryTab && t.TabPages.Contains(FlightData.tabTLogs))
                {
                    t.TabPages.Remove(FlightData.tabTLogs);
                }
                if (DisplayConfiguration.displayDataflashTab && !t.TabPages.Contains(FlightData.tablogbrowse))
                {
                    t.TabPages.Add(FlightData.tablogbrowse);
                }
                else if (!DisplayConfiguration.displayDataflashTab && t.TabPages.Contains(FlightData.tablogbrowse))
                {
                    t.TabPages.Remove(FlightData.tablogbrowse);
                }
                if (DisplayConfiguration.displayMessagesTab && !t.TabPages.Contains(FlightData.tabPagemessages))
                {
                    t.TabPages.Add(FlightData.tabPagemessages);
                }
                else if (!DisplayConfiguration.displayMessagesTab && t.TabPages.Contains(FlightData.tabPagemessages))
                {
                    t.TabPages.Remove(FlightData.tabPagemessages);
                }
                t.SelectedIndex = 0;

                MainV2.instance.FlightData.loadTabControlActions();
            }

            if (MainV2.instance.FlightPlanner != null)
            {
                //hide menu items 
                MainV2.instance.FlightPlanner.updateDisplayView();
            }
        }
        

        public MainV2()
        {
            log.Info("Mainv2 ctor");

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // set this before we reset it
            Settings.Instance["NUM_tracklength"] = "200";

            // create one here - but override on load
            Settings.Instance["guid"] = Guid.NewGuid().ToString();

            // load config
            LoadConfig();

            // force language to be loaded
            L10N.GetConfigLang();

            ShowAirports = true;

            // setup adsb
            Utilities.adsb.UpdatePlanePosition += adsb_UpdatePlanePosition;

            MAVLinkInterface.UpdateADSBPlanePosition += adsb_UpdatePlanePosition;

            MAVLinkInterface.gcssysid = (byte) Settings.Instance.GetByte("gcsid", MAVLinkInterface.gcssysid);

            Form splash = Program.Splash;

            splash?.Refresh();

            Application.DoEvents();

            instance = this;

            InitializeComponent();
            try
            {
                if(Settings.Instance["theme"] != null)
                    ThemeManager.SetTheme((ThemeManager.Themes)Enum.Parse(typeof(ThemeManager.Themes), Settings.Instance["theme"]));
            }
            catch
            {
            }
            Utilities.ThemeManager.ApplyThemeTo(this);
            MyView = new MainSwitcher(this);

            View = MyView;

            //startup console
            TCPConsole.Write((byte) 'S');

            // define default basestream
            comPort.BaseStream = new SerialPort();
            comPort.BaseStream.BaudRate = 115200;

            _connectionControl = toolStripConnectionControl.ConnectionControl;
            _connectionControl.CMB_baudrate.TextChanged += this.CMB_baudrate_TextChanged;
            _connectionControl.CMB_serialport.SelectedIndexChanged += this.CMB_serialport_SelectedIndexChanged;
            _connectionControl.CMB_serialport.Click += this.CMB_serialport_Click;
            _connectionControl.cmb_sysid.Click += cmb_sysid_Click;

            _connectionControl.ShowLinkStats += (sender, e) => ShowConnectionStatsForm();
            srtm.datadirectory = Settings.GetDataDirectory() +
                                 "srtm";

            var t = Type.GetType("Mono.Runtime");
            MONO = (t != null);

            try
            {
                speechEngine = new Speech();
                MAVLinkInterface.Speech = speechEngine;
                CurrentState.Speech = speechEngine;
            } catch { }

            Warnings.CustomWarning.defaultsrc = comPort.MAV.cs;
            Warnings.WarningEngine.Start();

            // proxy loader - dll load now instead of on config form load
            new Transition(new TransitionType_EaseInEaseOut(2000));

            foreach (object obj in Enum.GetValues(typeof (Firmwares)))
            {
                _connectionControl.TOOL_APMFirmware.Items.Add(obj);
            }

            if (_connectionControl.TOOL_APMFirmware.Items.Count > 0)
                _connectionControl.TOOL_APMFirmware.SelectedIndex = 0;

            PopulateSerialportList();
            if (_connectionControl.CMB_serialport.Items.Count > 0)
            {
                _connectionControl.CMB_baudrate.SelectedIndex = 8;
                _connectionControl.CMB_serialport.SelectedIndex = 0;
            }
            // ** Done

            splash?.Refresh();
            Application.DoEvents();

            // load last saved connection settings
            string temp = Settings.Instance.ComPort;
            if (!string.IsNullOrEmpty(temp))
            {
                _connectionControl.CMB_serialport.SelectedIndex = _connectionControl.CMB_serialport.FindString(temp);
                if (_connectionControl.CMB_serialport.SelectedIndex == -1)
                {
                    _connectionControl.CMB_serialport.Text = temp; // allows ports that dont exist - yet
                }
                comPort.BaseStream.PortName = temp;
                comPortName = temp;
            }
            string temp2 = Settings.Instance.BaudRate;
            if (!string.IsNullOrEmpty(temp2))
            {
                var idx =  _connectionControl.CMB_baudrate.FindString(temp2);
                if (idx == -1)
                {
                    _connectionControl.CMB_baudrate.Text = temp2;
                }
                else
                {
                    _connectionControl.CMB_baudrate.SelectedIndex = idx;
                }

                comPortBaud = int.Parse(temp2);
            }
            string temp3 = Settings.Instance.APMFirmware;
            if (!string.IsNullOrEmpty(temp3))
            {
                _connectionControl.TOOL_APMFirmware.SelectedIndex =
                    _connectionControl.TOOL_APMFirmware.FindStringExact(temp3);
                if (_connectionControl.TOOL_APMFirmware.SelectedIndex == -1)
                    _connectionControl.TOOL_APMFirmware.SelectedIndex = 0;
                MainV2.comPort.MAV.cs.firmware =
                    (Firmwares) Enum.Parse(typeof (Firmwares), _connectionControl.TOOL_APMFirmware.Text);
            }

            MissionPlanner.Utilities.Tracking.cid = new Guid(Settings.Instance["guid"].ToString());

            // setup guids for droneshare
            if (!Settings.Instance.ContainsKey("plane_guid"))
                Settings.Instance["plane_guid"] = Guid.NewGuid().ToString();

            if (!Settings.Instance.ContainsKey("copter_guid"))
                Settings.Instance["copter_guid"] = Guid.NewGuid().ToString();

            if (!Settings.Instance.ContainsKey("rover_guid"))
                Settings.Instance["rover_guid"] = Guid.NewGuid().ToString();

            if (Settings.Instance.ContainsKey("language") && !string.IsNullOrEmpty(Settings.Instance["language"]))
            {
                changelanguage(CultureInfoEx.GetCultureInfo(Settings.Instance["language"]));
            }

            if (splash != null)
            {
                this.Text = splash?.Text;
                titlebar = splash?.Text;
            }

            if (!MONO) // windows only
            {
                if (Settings.Instance["showconsole"] != null && Settings.Instance["showconsole"].ToString() == "True")
                {
                }
                else
                {
                    int win = NativeMethods.FindWindow("ConsoleWindowClass", null);
                    NativeMethods.ShowWindow(win, NativeMethods.SW_HIDE); // hide window
                }

                // prevent system from sleeping while mp open
                var previousExecutionState =
                    NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);
            }

            ChangeUnits();

            if (Settings.Instance["theme"] != null)
            {
                try
                {
                    ThemeManager.SetTheme(
                        (ThemeManager.Themes)
                            Enum.Parse(typeof (ThemeManager.Themes), Settings.Instance["theme"].ToString()));
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                }

                if (ThemeManager.CurrentTheme == ThemeManager.Themes.Custom)
                {
                    try
                    {
                        ThemeManager.BGColor = Color.FromArgb(int.Parse(Settings.Instance["theme_bg"].ToString()));
                        ThemeManager.ControlBGColor = Color.FromArgb(int.Parse(Settings.Instance["theme_ctlbg"].ToString()));
                        ThemeManager.TextColor = Color.FromArgb(int.Parse(Settings.Instance["theme_text"].ToString()));
                        ThemeManager.ButBG = Color.FromArgb(int.Parse(Settings.Instance["theme_butbg"].ToString()));
                        ThemeManager.ButBorder = Color.FromArgb(int.Parse(Settings.Instance["theme_butbord"].ToString()));
                    }
                    catch
                    {
                        log.Error("Bad Custom theme - reset to standard");
                        ThemeManager.SetTheme(ThemeManager.Themes.BurntKermit);
                    }
                }

                if (ThemeManager.CurrentTheme == ThemeManager.Themes.HighContrast)
                {
                    switchicons(new highcontrastmenuicons());
                }
            }

            if (Settings.Instance["showairports"] != null)
            {
                MainV2.ShowAirports = bool.Parse(Settings.Instance["showairports"]);
            }

            // set default
            ShowTFR = true;
            // load saved
            if (Settings.Instance["showtfr"] != null)
            {
                MainV2.ShowTFR = Settings.Instance.GetBoolean("showtfr");
            }

            if (Settings.Instance["enableadsb"] != null)
            {
                MainV2.instance.EnableADSB = Settings.Instance.GetBoolean("enableadsb");
            }

            try
            {
                log.Info("Create FD");
                FlightData = new GCSViews.FlightData();
                log.Info("Create FP");
                FlightPlanner = new GCSViews.FlightPlanner();
                //Configuration = new GCSViews.ConfigurationView.Setup();
                log.Info("Create SIM");
                Simulation = new SITL();
                //Firmware = new GCSViews.Firmware();
                //Terminal = new GCSViews.Terminal();

                FlightData.Width = MyView.Width;
                FlightPlanner.Width = MyView.Width;
                Simulation.Width = MyView.Width;
            }
            catch (ArgumentException e)
            {
                //http://www.microsoft.com/en-us/download/details.aspx?id=16083
                //System.ArgumentException: Font 'Arial' does not support style 'Regular'.

                log.Fatal(e);
                CustomMessageBox.Show(e.ToString() +
                                      "\n\n Font Issues? Please install this http://www.microsoft.com/en-us/download/details.aspx?id=16083");
                //splash.Close();
                //this.Close();
                Application.Exit();
            }
            catch (Exception e)
            {
                log.Fatal(e);
                CustomMessageBox.Show("A Major error has occured : " + e.ToString());
                Application.Exit();
            }

            //set first instance display configuration
            if (DisplayConfiguration == null)
            {
                DisplayConfiguration = DisplayConfiguration.Advanced();
            }

            // load old config
            if (Settings.Instance["advancedview"] != null)
            {
                if (Settings.Instance.GetBoolean("advancedview") == true)
                {
                    DisplayConfiguration = new DisplayView().Advanced();
                }
                // remove old config
                Settings.Instance.Remove("advancedview");
            }            //// load this before the other screens get loaded
            if (Settings.Instance["displayview"] != null)
            {
                try
                {
                    DisplayConfiguration = Settings.Instance.GetDisplayView("displayview");
                }
                catch
                {
                    DisplayConfiguration = DisplayConfiguration.Advanced();
                }
            }

            LayoutChanged += updateLayout;
            LayoutChanged(null, EventArgs.Empty);

            if (Settings.Instance["CHK_GDIPlus"] != null)
                GCSViews.FlightData.myhud.opengl = !bool.Parse(Settings.Instance["CHK_GDIPlus"].ToString());

            if (Settings.Instance["CHK_hudshow"] != null)
                GCSViews.FlightData.myhud.hudon = bool.Parse(Settings.Instance["CHK_hudshow"].ToString());

            try
            {
                if (Settings.Instance["MainLocX"] != null && Settings.Instance["MainLocY"] != null)
                {
                    this.StartPosition = FormStartPosition.Manual;
                    Point startpos = new Point(Settings.Instance.GetInt32("MainLocX"),
                        Settings.Instance.GetInt32("MainLocY"));

                    // fix common bug which happens when user removes a monitor, the app shows up
                    // offscreen and it is very hard to move it onscreen.  Also happens with 
                    // remote desktop a lot.  So this only restores position if the position
                    // is visible.
                    foreach (Screen s in Screen.AllScreens)
                    {
                        if (s.WorkingArea.Contains(startpos))
                        {
                            this.Location = startpos;
                            break;
                        }
                    }

                }

                if (Settings.Instance["MainMaximised"] != null)
                {
                    this.WindowState =
                        (FormWindowState) Enum.Parse(typeof (FormWindowState), Settings.Instance["MainMaximised"]);
                    // dont allow minimised start state
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.Location = new Point(100, 100);
                    }
                }

                if (Settings.Instance["MainHeight"] != null)
                    this.Height = Settings.Instance.GetInt32("MainHeight");
                if (Settings.Instance["MainWidth"] != null)
                    this.Width = Settings.Instance.GetInt32("MainWidth");

                // set presaved default telem rates
                if (Settings.Instance["CMB_rateattitude"] != null)
                    CurrentState.rateattitudebackup = Settings.Instance.GetInt32("CMB_rateattitude");
                if (Settings.Instance["CMB_rateposition"] != null)
                    CurrentState.ratepositionbackup = Settings.Instance.GetInt32("CMB_rateposition");
                if (Settings.Instance["CMB_ratestatus"] != null)
                    CurrentState.ratestatusbackup = Settings.Instance.GetInt32("CMB_ratestatus");
                if (Settings.Instance["CMB_raterc"] != null)
                    CurrentState.ratercbackup = Settings.Instance.GetInt32("CMB_raterc");
                if (Settings.Instance["CMB_ratesensors"] != null)
                    CurrentState.ratesensorsbackup = Settings.Instance.GetInt32("CMB_ratesensors");

                // make sure rates propogate
                MainV2.comPort.MAV.cs.ResetInternals();

                if (Settings.Instance["speechenable"] != null)
                    MainV2.speechEnable = Settings.Instance.GetBoolean("speechenable");

                if (Settings.Instance["analyticsoptout"] != null)
                    MissionPlanner.Utilities.Tracking.OptOut = Settings.Instance.GetBoolean("analyticsoptout");

                try
                {
                    if (Settings.Instance["TXT_homelat"] != null)
                        MainV2.comPort.MAV.cs.HomeLocation.Lat = Settings.Instance.GetDouble("TXT_homelat");

                    if (Settings.Instance["TXT_homelng"] != null)
                        MainV2.comPort.MAV.cs.HomeLocation.Lng = Settings.Instance.GetDouble("TXT_homelng");

                    if (Settings.Instance["TXT_homealt"] != null)
                        MainV2.comPort.MAV.cs.HomeLocation.Alt = Settings.Instance.GetDouble("TXT_homealt");

                    // remove invalid entrys
                    if (Math.Abs(MainV2.comPort.MAV.cs.HomeLocation.Lat) > 90 ||
                        Math.Abs(MainV2.comPort.MAV.cs.HomeLocation.Lng) > 180)
                        MainV2.comPort.MAV.cs.HomeLocation = new PointLatLngAlt();
                }
                catch
                {
                }
            }
            catch
            {
            }

            if (CurrentState.rateattitudebackup == 0) // initilised to 10, configured above from save
            {
                CustomMessageBox.Show(
                    "NOTE: your attitude rate is 0, the hud will not work\nChange in Configuration > Planner > Telemetry Rates");
            }
            
            // create log dir if it doesnt exist
            try
            {
                if (!Directory.Exists(Settings.Instance.LogDir))
                    Directory.CreateDirectory(Settings.Instance.LogDir);
            }
            catch (Exception ex) { log.Error(ex); }
#if !NETSTANDARD2_0
#if !NETCOREAPP2_0
            Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
#endif
#endif

            // make sure new enough .net framework is installed
            if (!MONO)
            {
                Microsoft.Win32.RegistryKey installed_versions =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
                string[] version_names = installed_versions.GetSubKeyNames();
                //version names start with 'v', eg, 'v3.5' which needs to be trimmed off before conversion
                double Framework = Convert.ToDouble(version_names[version_names.Length - 1].Remove(0, 1),
                    CultureInfo.InvariantCulture);
                int SP =
                    Convert.ToInt32(installed_versions.OpenSubKey(version_names[version_names.Length - 1])
                        .GetValue("SP", 0));

                if (Framework < 4.0)
                {
                    CustomMessageBox.Show("This program requires .NET Framework 4.0. You currently have " + Framework);
                }
            }

            if (Program.IconFile != null)
            {
                this.Icon = Icon.FromHandle(((Bitmap)Program.IconFile).GetHicon());
            }

            if (Program.Logo2 != null)
                MenuArduPilot.Image = Program.Logo2;

            if (Program.Logo != null && Program.name == "VVVVZ")
            {
                MenuDonate.Click -= this.toolStripMenuItem1_Click;
                MenuDonate.Text = "";
                MenuDonate.Image = Program.Logo;

                MenuDonate.Click += MenuCustom_Click;

                MenuFlightData.Visible = false;
                MenuFlightPlanner.Visible = true;
                MenuConfigTune.Visible = false;
                MenuHelp.Visible = false;
                MenuInitConfig.Visible = false;
                MenuSimulation.Visible = false;
                MenuTerminal.Visible = false;
            }
            else if (Program.Logo != null && Program.names.Contains(Program.name))
            {
                MenuDonate.Click -= this.toolStripMenuItem1_Click;
                MenuDonate.Text = "";
                MenuDonate.Image = Program.Logo;
            }



            Application.DoEvents();

            Comports.Add(comPort);

            MainV2.comPort.MavChanged += comPort_MavChanged;

            // save config to test we have write access
            SaveConfig();
        }

        void cmb_sysid_Click(object sender, EventArgs e)
        {
            MainV2._connectionControl.UpdateSysIDS();
        }

        void comPort_MavChanged(object sender, EventArgs e)
        {
            log.Info("Mav Changed " + MainV2.comPort.MAV.sysid);

            HUD.Custom.src = MainV2.comPort.MAV.cs;

            CustomWarning.defaultsrc = MainV2.comPort.MAV.cs;

            MissionPlanner.Controls.PreFlight.CheckListItem.defaultsrc = MainV2.comPort.MAV.cs;

            // when uploading a firmware we dont want to reload this screen.
            if (instance.MyView.current.Control != null && instance.MyView.current.Control.GetType() == typeof(GCSViews.InitialSetup))
            {
                var page = ((GCSViews.InitialSetup)instance.MyView.current.Control).backstageView.SelectedPage;
                if (page != null && page.Text == "Install Firmware")
                {
                    return;
                }
            }

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker) delegate
                {
                    //enable the payload control page if a mavlink gimbal is detected
                    if (instance.FlightData != null)
                    {
                        instance.FlightData.updatePayloadTabVisible();
                    }

                    instance.MyView.Reload();
                });
            }
            else
            {
                //enable the payload control page if a mavlink gimbal is detected
                if (instance.FlightData != null)
                {
                    instance.FlightData.updatePayloadTabVisible();
                }

                instance.MyView.Reload();
            }
        }
#if !NETSTANDARD2_0
#if !NETCOREAPP2_0
        void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            // try prevent crash on resume
            if (e.Mode == Microsoft.Win32.PowerModes.Suspend)
            {
                doDisconnect(MainV2.comPort);
            }
        }
#endif
#endif
        private void BGLoadAirports(object nothing)
        {
            // read airport list
            try
            {
                Utilities.Airports.ReadOurairports(Settings.GetRunningDirectory() +
                                                   "airports.csv");

                Utilities.Airports.checkdups = true;

                //Utilities.Airports.ReadOpenflights(Application.StartupPath + Path.DirectorySeparatorChar + "airports.dat");

                log.Info("Loaded " + Utilities.Airports.GetAirportCount + " airports");
            }
            catch
            {
            }
        }

        public void switchicons(menuicons icons)
        {
            // dont update if no change
            if (displayicons.GetType() == icons.GetType())
                return;

            displayicons = icons;

            MainMenu.BackColor = SystemColors.MenuBar;

            MainMenu.BackgroundImage = displayicons.bg;

            MenuFlightData.Image = displayicons.fd;
            MenuFlightPlanner.Image = displayicons.fp;
            MenuInitConfig.Image = displayicons.initsetup;
            MenuSimulation.Image = displayicons.sim;
            MenuConfigTune.Image = displayicons.config_tuning;
            MenuTerminal.Image = displayicons.terminal;
            MenuConnect.Image = displayicons.connect;
            MenuHelp.Image = displayicons.help;
            MenuDonate.Image = displayicons.donate;


            MenuFlightData.ForeColor = ThemeManager.TextColor;
            MenuFlightPlanner.ForeColor = ThemeManager.TextColor;
            MenuInitConfig.ForeColor = ThemeManager.TextColor;
            MenuSimulation.ForeColor = ThemeManager.TextColor;
            MenuConfigTune.ForeColor = ThemeManager.TextColor;
            MenuTerminal.ForeColor = ThemeManager.TextColor;
            MenuConnect.ForeColor = ThemeManager.TextColor;
            MenuHelp.ForeColor = ThemeManager.TextColor;
            MenuDonate.ForeColor = ThemeManager.TextColor;
        }

        void MenuCustom_Click(object sender, EventArgs e)
        {
            if (Settings.Instance.GetBoolean("password_protect") == false)
            {
                MenuFlightData.Visible = true;
                MenuFlightPlanner.Visible = true;
                MenuConfigTune.Visible = true;
                MenuHelp.Visible = true;
                MenuInitConfig.Visible = true;
                MenuSimulation.Visible = true;
                MenuTerminal.Visible = true;
            }
            else
            {
                if (Password.VerifyPassword())
                {
                    MenuFlightData.Visible = true;
                    MenuFlightPlanner.Visible = true;
                    MenuConfigTune.Visible = true;
                    MenuHelp.Visible = true;
                    MenuInitConfig.Visible = true;
                    MenuSimulation.Visible = true;
                    MenuTerminal.Visible = true;
                }
            }
        }

        void adsb_UpdatePlanePosition(object sender, MissionPlanner.Utilities.adsb.PointLatLngAltHdg adsb)
        {
            lock (adsblock)
            {
                var id = adsb.Tag;

                if (MainV2.instance.adsbPlanes.ContainsKey(id))
                {
                    // update existing
                    ((adsb.PointLatLngAltHdg) instance.adsbPlanes[id]).Lat = adsb.Lat;
                    ((adsb.PointLatLngAltHdg) instance.adsbPlanes[id]).Lng = adsb.Lng;
                    ((adsb.PointLatLngAltHdg) instance.adsbPlanes[id]).Alt = adsb.Alt;
                    ((adsb.PointLatLngAltHdg) instance.adsbPlanes[id]).Heading = adsb.Heading;
                    ((adsb.PointLatLngAltHdg) instance.adsbPlanes[id]).Time = DateTime.Now;
                    ((adsb.PointLatLngAltHdg) instance.adsbPlanes[id]).CallSign = adsb.CallSign;
                }
                else
                {
                    // create new plane
                    MainV2.instance.adsbPlanes[id] =
                        new adsb.PointLatLngAltHdg(adsb.Lat, adsb.Lng,
                            adsb.Alt, adsb.Heading, adsb.Speed , id,
                            DateTime.Now) {CallSign = adsb.CallSign};
                }

                try
                {
                    // dont rebroadcast something that came from the drone
                    if (sender != null && sender is MAVLinkInterface)
                        return;

                    MAVLink.mavlink_adsb_vehicle_t packet = new MAVLink.mavlink_adsb_vehicle_t();

                    packet.altitude = (int)(MainV2.instance.adsbPlanes[id].Alt * 1000);
                    packet.altitude_type = (byte)MAVLink.ADSB_ALTITUDE_TYPE.GEOMETRIC;
                    packet.callsign = ASCIIEncoding.ASCII.GetBytes(adsb.CallSign);
                    packet.emitter_type = (byte)MAVLink.ADSB_EMITTER_TYPE.NO_INFO;
                    packet.heading = (ushort)(MainV2.instance.adsbPlanes[id].Heading * 100);
                    packet.lat = (int)(MainV2.instance.adsbPlanes[id].Lat * 1e7);
                    packet.lon = (int)(MainV2.instance.adsbPlanes[id].Lng * 1e7);
                    packet.ICAO_address = uint.Parse(id, NumberStyles.HexNumber);

                    packet.flags = (ushort)(MAVLink.ADSB_FLAGS.VALID_ALTITUDE | MAVLink.ADSB_FLAGS.VALID_COORDS |
                        MAVLink.ADSB_FLAGS.VALID_HEADING | MAVLink.ADSB_FLAGS.VALID_CALLSIGN);

                    //send to current connected
                    MainV2.comPort.sendPacket(packet, MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid);
                }
                catch
                {

                }
            }
        }


        private void ResetConnectionStats()
        {
            log.Info("Reset connection stats");
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
                    Text = Strings.LinkStats
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
            PopulateSerialportList();
            if (_connectionControl.CMB_serialport.Items.Contains(oldport))
                _connectionControl.CMB_serialport.Text = oldport;
        }

        private void PopulateSerialportList()
        {
            _connectionControl.CMB_serialport.Items.Clear();
            _connectionControl.CMB_serialport.Items.Add("AUTO");
            _connectionControl.CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            _connectionControl.CMB_serialport.Items.Add("TCP");
            _connectionControl.CMB_serialport.Items.Add("UDP");
            _connectionControl.CMB_serialport.Items.Add("UDPCl");
        }

        private void MenuFlightData_Click(object sender, EventArgs e)
        {
            MyView.ShowScreen("FlightData");
        }

        private void MenuFlightPlanner_Click(object sender, EventArgs e)
        {
            MyView.ShowScreen("FlightPlanner");
        }

        public void MenuSetup_Click(object sender, EventArgs e)
        {
            if (Settings.Instance.GetBoolean("password_protect") == false)
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
            if (Settings.Instance.GetBoolean("password_protect") == false)
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

        public void doDisconnect(MAVLinkInterface comPort)
        {
            log.Info("We are disconnecting");
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
                    ((ConnectionStats) this.connectionStatsForm.Controls[0]).StopUpdates();
            }
            catch
            {
            }

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
                System.Threading.ThreadPool.QueueUserWorkItem((WaitCallback) delegate
                {
                    try
                    {
                        MissionPlanner.Log.LogSort.SortLogs(Directory.GetFiles(Settings.Instance.LogDir, "*.tlog"));
                    }
                    catch
                    {
                    }
                }
                    );
            }
            catch
            {
            }

            this.MenuConnect.Image = global::MissionPlanner.Properties.Resources.light_connect_icon;
        }

        public void doConnect(MAVLinkInterface comPort, string portname, string baud, bool getparams = true)
        {
            bool skipconnectcheck = false;
            log.Info("We are connecting to " + portname + " " + baud );
            switch (portname)
            {
                case "preset":
                    skipconnectcheck = true;
                    if (comPort.BaseStream is TcpSerial)
                        _connectionControl.CMB_serialport.Text = "TCP";
                    if (comPort.BaseStream is UdpSerial)
                        _connectionControl.CMB_serialport.Text = "UDP";
                    if (comPort.BaseStream is UdpSerialConnect)
                        _connectionControl.CMB_serialport.Text = "UDPCl";
                    if (comPort.BaseStream is SerialPort)
                    {
                        _connectionControl.CMB_serialport.Text = comPort.BaseStream.PortName;
                        _connectionControl.CMB_baudrate.Text = comPort.BaseStream.BaudRate.ToString();
                    }
                    break;
                case "TCP":
                    comPort.BaseStream = new TcpSerial();
                    _connectionControl.CMB_serialport.Text = "TCP";
                    break;
                case "UDP":
                    comPort.BaseStream = new UdpSerial();
                    _connectionControl.CMB_serialport.Text = "UDP";
                    break;
                case "UDPCl":
                    comPort.BaseStream = new UdpSerialConnect();
                    _connectionControl.CMB_serialport.Text = "UDPCl";
                    break;
                case "AUTO":
                    // do autoscan
                    Comms.CommsSerialScan.Scan(true);
                    DateTime deadline = DateTime.Now.AddSeconds(50);
                    while (Comms.CommsSerialScan.foundport == false || Comms.CommsSerialScan.run == 1)
                    {
                        System.Threading.Thread.Sleep(100);

                        if (DateTime.Now > deadline)
                        {
                            CustomMessageBox.Show(Strings.Timeout);
                            _connectionControl.IsConnected(false);
                            return;
                        }
                    }
                    return;
                default:
                    comPort.BaseStream = new SerialPort();
                    break;
            }

            // Tell the connection UI that we are now connected.
            _connectionControl.IsConnected(true);

            // Here we want to reset the connection stats counter etc.
            this.ResetConnectionStats();

            comPort.MAV.cs.ResetInternals();

            //cleanup any log being played
            comPort.logreadmode = false;
            if (comPort.logplaybackfile != null)
                comPort.logplaybackfile.Close();
            comPort.logplaybackfile = null;

            try
            {
                log.Info("Set Portname");
                // set port, then options
                if(portname.ToLower() != "preset")
                    comPort.BaseStream.PortName = portname;

                log.Info("Set Baudrate");
                try
                {
                    if(baud != "" && baud != "0")
                        comPort.BaseStream.BaudRate = int.Parse(baud);
                }
                catch (Exception exp)
                {
                    log.Error(exp);
                }

                // prevent serialreader from doing anything
                comPort.giveComport = true;

                log.Info("About to do dtr if needed");
                // reset on connect logic.
                if (Settings.Instance.GetBoolean("CHK_resetapmonconnect") == true)
                {
                    log.Info("set dtr rts to false");
                    comPort.BaseStream.DtrEnable = false;
                    comPort.BaseStream.RtsEnable = false;

                    comPort.BaseStream.toggleDTR();
                }

                comPort.giveComport = false;

                // setup to record new logs
                try
                {
                    Directory.CreateDirectory(Settings.Instance.LogDir);
                    lock (this)
                    {
                        // create log names
                        var dt = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                        var tlog = Settings.Instance.LogDir + Path.DirectorySeparatorChar +
                                   dt + ".tlog";
                        var rlog = Settings.Instance.LogDir + Path.DirectorySeparatorChar +
                                   dt + ".rlog";

                        // check if this logname already exists
                        int a = 1;
                        while (File.Exists(tlog))
                        {
                            Thread.Sleep(1000);
                            // create new names with a as an index
                            dt = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + "-" + a.ToString();
                            tlog = Settings.Instance.LogDir + Path.DirectorySeparatorChar +
                                   dt + ".tlog";
                            rlog = Settings.Instance.LogDir + Path.DirectorySeparatorChar +
                                   dt + ".rlog";
                        }

                        //open the logs for writing
                        comPort.logfile =
                            new BufferedStream(File.Open(tlog, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None));
                        comPort.rawlogfile =
                            new BufferedStream(File.Open(rlog, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None));
                        log.Info("creating logfile " + dt + ".tlog");
                    }
                }
                catch (Exception exp2)
                {
                    log.Error(exp2);
                    CustomMessageBox.Show(Strings.Failclog);
                } // soft fail

                // reset connect time - for timeout functions
                connecttime = DateTime.Now;

                // do the connect
                comPort.Open(false, skipconnectcheck);

                if (!comPort.BaseStream.IsOpen)
                {
                    log.Info("comport is closed. existing connect");
                    try
                    {
                        _connectionControl.IsConnected(false);
                        UpdateConnectIcon();
                        comPort.Close();
                    }
                    catch
                    {
                    }
                    return;
                }

                if (getparams)
                    comPort.getParamList();

                _connectionControl.UpdateSysIDS();

                // detect firmware we are conected to.
                if (comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
                {
                    _connectionControl.TOOL_APMFirmware.SelectedIndex =
                        _connectionControl.TOOL_APMFirmware.Items.IndexOf(Firmwares.ArduCopter2);
                }
                else if (comPort.MAV.cs.firmware == Firmwares.Ateryx)
                {
                    _connectionControl.TOOL_APMFirmware.SelectedIndex =
                        _connectionControl.TOOL_APMFirmware.Items.IndexOf(Firmwares.Ateryx);
                }
                else if (comPort.MAV.cs.firmware == Firmwares.ArduRover)
                {
                    _connectionControl.TOOL_APMFirmware.SelectedIndex =
                        _connectionControl.TOOL_APMFirmware.Items.IndexOf(Firmwares.ArduRover);
                }
                else if (comPort.MAV.cs.firmware == Firmwares.ArduSub)
                {
                    _connectionControl.TOOL_APMFirmware.SelectedIndex =
                        _connectionControl.TOOL_APMFirmware.Items.IndexOf(Firmwares.ArduSub);
                }
                else if (comPort.MAV.cs.firmware == Firmwares.ArduPlane)
                {
                    _connectionControl.TOOL_APMFirmware.SelectedIndex =
                        _connectionControl.TOOL_APMFirmware.Items.IndexOf(Firmwares.ArduPlane);
                }

                // check for newer firmware
                var softwares = Firmware.LoadSoftwares();

                if (softwares.Count > 0)
                {
                    try
                    {
                        string[] fields1 = comPort.MAV.VersionString.Split(' ');

                        foreach (Firmware.software item in softwares)
                        {
                            string[] fields2 = item.name.Split(' ');

                            // check primare firmware type. ie arudplane, arducopter
                            if (fields1[0] == fields2[0])
                            {
                                Version ver1 = VersionDetection.GetVersion(comPort.MAV.VersionString);
                                Version ver2 = VersionDetection.GetVersion(item.name);

                                if (ver2 > ver1)
                                {
                                    Common.MessageShowAgain(Strings.NewFirmware + "-" + item.name,
                                        Strings.NewFirmwareA + item.name + Strings.Pleaseup);
                                    break;
                                }

                                // check the first hit only
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                FlightData.CheckBatteryShow();
                /*
                MissionPlanner.Utilities.Tracking.AddEvent("Connect", "Connect", comPort.MAV.cs.firmware.ToString(),
                    comPort.MAV.param.Count.ToString());
                MissionPlanner.Utilities.Tracking.AddTiming("Connect", "Connect Time",
                    (DateTime.Now - connecttime).TotalMilliseconds, "");

                MissionPlanner.Utilities.Tracking.AddEvent("Connect", "Baud", comPort.BaseStream.BaudRate.ToString(), "");

                if(comPort.MAV.param.ContainsKey("SPRAY_ENABLE"))
                    MissionPlanner.Utilities.Tracking.AddEvent("Param", "Value", "SPRAY_ENABLE",comPort.MAV.param["SPRAY_ENABLE"].ToString());

                if (comPort.MAV.param.ContainsKey("CHUTE_ENABLE"))
                    MissionPlanner.Utilities.Tracking.AddEvent("Param", "Value", "CHUTE_ENABLE", comPort.MAV.param["CHUTE_ENABLE"].ToString());

                if (comPort.MAV.param.ContainsKey("TERRAIN_ENABLE"))
                    MissionPlanner.Utilities.Tracking.AddEvent("Param", "Value", "TERRAIN_ENABLE", comPort.MAV.param["TERRAIN_ENABLE"].ToString());

                if (comPort.MAV.param.ContainsKey("ADSB_ENABLE"))
                    MissionPlanner.Utilities.Tracking.AddEvent("Param", "Value", "ADSB_ENABLE", comPort.MAV.param["ADSB_ENABLE"].ToString());

                if (comPort.MAV.param.ContainsKey("AVD_ENABLE"))
                    MissionPlanner.Utilities.Tracking.AddEvent("Param", "Value", "AVD_ENABLE", comPort.MAV.param["AVD_ENABLE"].ToString());
                */
                // save the baudrate for this port
                Settings.Instance[_connectionControl.CMB_serialport.Text + "_BAUD"] = _connectionControl.CMB_baudrate.Text;

                this.Text = titlebar + " " + comPort.MAV.VersionString;

                // refresh config window if needed
                if (MyView.current != null)
                {
                    if (MyView.current.Name == "HWConfig")
                        MyView.ShowScreen("HWConfig");
                    if (MyView.current.Name == "SWConfig")
                        MyView.ShowScreen("SWConfig");
                }

                // load wps on connect option.
                if (Settings.Instance.GetBoolean("loadwpsonconnect") == true)
                {
                    // only do it if we are connected.
                    if (comPort.BaseStream.IsOpen)
                    {
                        MenuFlightPlanner_Click(null, null);
                        FlightPlanner.BUT_read_Click(null, null);
                    }
                }

                // get any rallypoints
                if (MainV2.comPort.MAV.param.ContainsKey("RALLY_TOTAL") &&
                    int.Parse(MainV2.comPort.MAV.param["RALLY_TOTAL"].ToString()) > 0)
                {
                    FlightPlanner.getRallyPointsToolStripMenuItem_Click(null, null);

                    double maxdist = 0;

                    foreach (var rally in comPort.MAV.rallypoints)
                    {
                        foreach (var rally1 in comPort.MAV.rallypoints)
                        {
                            var pnt1 = new PointLatLngAlt(rally.Value.lat/10000000.0f, rally.Value.lng/10000000.0f);
                            var pnt2 = new PointLatLngAlt(rally1.Value.lat/10000000.0f, rally1.Value.lng/10000000.0f);

                            var dist = pnt1.GetDistance(pnt2);

                            maxdist = Math.Max(maxdist, dist);
                        }
                    }

                    if (comPort.MAV.param.ContainsKey("RALLY_LIMIT_KM") &&
                        (maxdist/1000.0) > (float) comPort.MAV.param["RALLY_LIMIT_KM"])
                    {
                        CustomMessageBox.Show(Strings.Warningrallypointdistance + " " +
                                              (maxdist/1000.0).ToString("0.00") + " > " +
                                              (float) comPort.MAV.param["RALLY_LIMIT_KM"]);
                    }
                }

                // get any fences
                if (MainV2.comPort.MAV.param.ContainsKey("FENCE_TOTAL") &&
                    int.Parse(MainV2.comPort.MAV.param["FENCE_TOTAL"].ToString()) > 1 &&
                    MainV2.comPort.MAV.param.ContainsKey("FENCE_ACTION") )
                {
                    FlightPlanner.GeoFencedownloadToolStripMenuItem_Click(null, null);
                }
                //Add HUD custom items source 
                HUD.Custom.src = MainV2.comPort.MAV.cs;

                // set connected icon
                this.MenuConnect.Image = displayicons.disconnect;
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
                catch (Exception ex2)
                {
                    log.Warn(ex2);
                }
                CustomMessageBox.Show("Can not establish a connection\n\n" + ex.Message);
                return;
            }
        }

        private void MenuConnect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void Connect()
        {
            comPort.giveComport = false;

            log.Info("MenuConnect Start");

            // sanity check
            if (comPort.BaseStream.IsOpen && comPort.MAV.cs.groundspeed > 4)
            {
                if ((int) DialogResult.No ==
                    CustomMessageBox.Show(Strings.Stillmoving, Strings.Disconnect, MessageBoxButtons.YesNo))
                {
                    return;
                }
            }

            try
            {
                log.Info("Cleanup last logfiles");
                // cleanup from any previous sessions
                if (comPort.logfile != null)
                    comPort.logfile.Close();

                if (comPort.rawlogfile != null)
                    comPort.rawlogfile.Close();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ErrorClosingLogFile + ex.Message, Strings.ERROR);
            }

            comPort.logfile = null;
            comPort.rawlogfile = null;

            // decide if this is a connect or disconnect
            if (comPort.BaseStream.IsOpen)
            {
                doDisconnect(comPort);
            }
            else
            {
                doConnect(comPort, _connectionControl.CMB_serialport.Text, _connectionControl.CMB_baudrate.Text);
            }

            _connectionControl.UpdateSysIDS();

            loadph_serial();
        }

        void loadph_serial()
        {
            try
            {
                if (comPort.MAV.SerialString == "")
                    return;

                var serials = File.ReadAllLines("ph2_serial.csv");

                foreach (var serial in serials)
                {
                    if (serial.Contains(comPort.MAV.SerialString.Substring(comPort.MAV.SerialString.Length - 26, 26)) &&
                        !Settings.Instance.ContainsKey(comPort.MAV.SerialString.Replace(" ", "")))
                    {
                        CustomMessageBox.Show(
                            "Your board has a Critical service bulletin please see [link;http://discuss.ardupilot.org/t/sb-0000001-critical-service-bulletin-for-beta-cube-2-1/14711;Click here]",
                            Strings.ERROR);

                        Settings.Instance[comPort.MAV.SerialString.Replace(" ","")] = true.ToString();
                    }
                }
            }
            catch
            {
                
            }
        }

        private void CMB_serialport_SelectedIndexChanged(object sender, EventArgs e)
        {
            comPortName = _connectionControl.CMB_serialport.Text;
            if (comPortName == "UDP" || comPortName == "UDPCl" || comPortName == "TCP" || comPortName == "AUTO")
            {
                _connectionControl.CMB_baudrate.Enabled = false;
            }
            else
            {
                _connectionControl.CMB_baudrate.Enabled = true;
            }

            try
            {
                // check for saved baud rate and restore
                if (Settings.Instance[_connectionControl.CMB_serialport.Text + "_BAUD"] != null)
                {
                    _connectionControl.CMB_baudrate.Text =
                        Settings.Instance[_connectionControl.CMB_serialport.Text + "_BAUD"];
                }
            }
            catch
            {
            }
        }


        /// <summary>
        /// overriding the OnCLosing is a bit cleaner than handling the event, since it 
        /// is this object.
        /// 
        /// This happens before FormClosed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            log.Info("MainV2_FormClosing");

            log.Info("GMaps write cache");
            // speed up tile saving on exit
            GMap.NET.GMaps.Instance.CacheOnIdleRead = false;
            GMap.NET.GMaps.Instance.BoostCacheEngine = true;

            Settings.Instance["MainHeight"] = this.Height.ToString();
            Settings.Instance["MainWidth"] = this.Width.ToString();
            Settings.Instance["MainMaximised"] = this.WindowState.ToString();

            Settings.Instance["MainLocX"] = this.Location.X.ToString();
            Settings.Instance["MainLocY"] = this.Location.Y.ToString();

            log.Info("close logs");
            AltitudeAngel.Dispose();

            // close bases connection
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
            catch
            {
            }

            log.Info("close ports");
            // close all connections
            foreach (var port in Comports)
            {
                try
                {
                    port.logreadmode = false;
                    if (port.logfile != null)
                        port.logfile.Close();

                    if (port.rawlogfile != null)
                        port.rawlogfile.Close();

                    port.logfile = null;
                    port.rawlogfile = null;
                }
                catch
                {
                }
            }

            log.Info("stop adsb");
            Utilities.adsb.Stop();

            log.Info("stop WarningEngine");
            Warnings.WarningEngine.Stop();

            log.Info("stop UDPVideoShim");
            UDPVideoShim.Stop();

            log.Info("stop GStreamer");
            GStreamer.StopAll();

            log.Info("closing vlcrender");
            try
            {
                while (vlcrender.store.Count > 0)
                    vlcrender.store[0].Stop();
            }
            catch
            {
            }

            log.Info("closing pluginthread");

            pluginthreadrun = false;

            if (pluginthread != null)
                pluginthread.Join();

            log.Info("closing serialthread");

            serialThread = false;

            if (serialreaderthread != null)
                serialreaderthread.Join();

            log.Info("closing joystickthread");

            joystickthreadrun = false;

            if (joystickthread != null)
                joystickthread.Join();

            log.Info("closing httpthread");

            // if we are waiting on a socket we need to force an abort
            httpserver.Stop();

            log.Info("sorting tlogs");
            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem((WaitCallback) delegate
                    {
                        try
                        {
                            MissionPlanner.Log.LogSort.SortLogs(Directory.GetFiles(Settings.Instance.LogDir, "*.tlog"));
                        }
                        catch
                        {
                        }
                    }
                );
            }
            catch
            {
            }

            log.Info("closing MyView");

            // close all tabs
            MyView.Dispose();

            log.Info("closing fd");
            try
            {
                FlightData.Dispose();
            }
            catch
            {
            }
            log.Info("closing fp");
            try
            {
                FlightPlanner.Dispose();
            }
            catch
            {
            }
            log.Info("closing sim");
            try
            {
                Simulation.Dispose();
            }
            catch
            {
            }

            try
            {
                if (comPort.BaseStream.IsOpen)
                    comPort.Close();
            }
            catch
            {
            } // i get alot of these errors, the port is still open, but not valid - user has unpluged usb

            // save config
            SaveConfig();

            Console.WriteLine(httpthread?.IsAlive);
            Console.WriteLine(joystickthread?.IsAlive);
            Console.WriteLine(serialreaderthread?.IsAlive);
            Console.WriteLine(pluginthread?.IsAlive);

            log.Info("MainV2_FormClosing done");

            if (MONO)
                this.Dispose();
        }


        /// <summary>
        /// this happens after FormClosing...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            Console.WriteLine("MainV2_FormClosed");

            if (joystick != null)
            {
                while (!joysendThreadExited)
                    Thread.Sleep(10);

                joystick.Dispose(); //proper clean up of joystick.
            }
        }

        private void LoadConfig()
        {
            try
            {
                log.Info("Loading config");

                Settings.Instance.Load();

                comPortName = Settings.Instance.ComPort;
            }
            catch (Exception ex)
            {
                log.Error("Bad Config File", ex);
            }
        }

        private void SaveConfig()
        {
            try
            {
                log.Info("Saving config");
                Settings.Instance.ComPort = comPortName;

                if (_connectionControl != null)
                    Settings.Instance.BaudRate = _connectionControl.CMB_baudrate.Text;

                Settings.Instance.APMFirmware = MainV2.comPort.MAV.cs.firmware.ToString();

                Settings.Instance.Save();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// needs to be true by default so that exits properly if no joystick used.
        /// </summary>
        volatile private bool joysendThreadExited = true;

        /// <summary>
        /// thread used to send joystick packets to the MAV
        /// </summary>
        private void joysticksend()
        {
            float rate = 50; // 1000 / 50 = 20 hz
            int count = 0;

            DateTime lastratechange = DateTime.Now;

            joystickthreadrun = true;

            while (joystickthreadrun)
            {
                joysendThreadExited = false;
                //so we know this thread is stil alive.           
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
                            if (!joystick.manual_control)
                            {
                                MAVLink.mavlink_rc_channels_override_t rc = new MAVLink.mavlink_rc_channels_override_t();

                                rc.target_component = comPort.MAV.compid;
                                rc.target_system = comPort.MAV.sysid;

                                if (joystick.getJoystickAxis(1) != Joystick.Joystick.joystickaxis.None)rc.chan1_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech1;
                                if (joystick.getJoystickAxis(2) != Joystick.Joystick.joystickaxis.None)rc.chan2_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech2;
                                if (joystick.getJoystickAxis(3) != Joystick.Joystick.joystickaxis.None)rc.chan3_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech3;
                                if (joystick.getJoystickAxis(4) != Joystick.Joystick.joystickaxis.None)rc.chan4_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech4;
                                if (joystick.getJoystickAxis(5) != Joystick.Joystick.joystickaxis.None)rc.chan5_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech5;
                                if (joystick.getJoystickAxis(6) != Joystick.Joystick.joystickaxis.None)rc.chan6_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech6;
                                if (joystick.getJoystickAxis(7) != Joystick.Joystick.joystickaxis.None)rc.chan7_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech7;
                                if (joystick.getJoystickAxis(8) != Joystick.Joystick.joystickaxis.None)rc.chan8_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech8;
                                if (joystick.getJoystickAxis(9) != Joystick.Joystick.joystickaxis.None) rc.chan9_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech9;
                                if (joystick.getJoystickAxis(10) != Joystick.Joystick.joystickaxis.None) rc.chan10_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech10;
                                if (joystick.getJoystickAxis(11) != Joystick.Joystick.joystickaxis.None) rc.chan11_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech11;
                                if (joystick.getJoystickAxis(12) != Joystick.Joystick.joystickaxis.None) rc.chan12_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech12;
                                if (joystick.getJoystickAxis(13) != Joystick.Joystick.joystickaxis.None) rc.chan13_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech13;
                                if (joystick.getJoystickAxis(14) != Joystick.Joystick.joystickaxis.None) rc.chan14_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech14;
                                if (joystick.getJoystickAxis(15) != Joystick.Joystick.joystickaxis.None) rc.chan15_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech15;
                                if (joystick.getJoystickAxis(16) != Joystick.Joystick.joystickaxis.None) rc.chan16_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech16;
                                if (joystick.getJoystickAxis(17) != Joystick.Joystick.joystickaxis.None) rc.chan17_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech17;
                                if (joystick.getJoystickAxis(18) != Joystick.Joystick.joystickaxis.None) rc.chan18_raw = (ushort)MainV2.comPort.MAV.cs.rcoverridech18;

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

                                    //Console.WriteLine("Joystick btw " + comPort.BaseStream.BytesToWrite);

                                    if (!comPort.BaseStream.IsOpen)
                                        continue;

                                    if (comPort.BaseStream.BytesToWrite < 50)
                                    {
                                        if (sitl)
                                        {
                                            MissionPlanner.Controls.SITL.rcinput();
                                        }
                                        else
                                        {
                                            comPort.sendPacket(rc, rc.target_system, rc.target_component);
                                        }
                                        count++;
                                        lastjoystick = DateTime.Now;
                                    }
                                }
                            }
                            else
                            {
                                MAVLink.mavlink_manual_control_t rc = new MAVLink.mavlink_manual_control_t();

                                rc.target = comPort.MAV.compid;

                                if (joystick.getJoystickAxis(1) != Joystick.Joystick.joystickaxis.None)
                                    rc.x = MainV2.comPort.MAV.cs.rcoverridech1;
                                if (joystick.getJoystickAxis(2) != Joystick.Joystick.joystickaxis.None)
                                    rc.y = MainV2.comPort.MAV.cs.rcoverridech2;
                                if (joystick.getJoystickAxis(3) != Joystick.Joystick.joystickaxis.None)
                                    rc.z = MainV2.comPort.MAV.cs.rcoverridech3;
                                if (joystick.getJoystickAxis(4) != Joystick.Joystick.joystickaxis.None)
                                    rc.r = MainV2.comPort.MAV.cs.rcoverridech4;

                                if (lastjoystick.AddMilliseconds(rate) < DateTime.Now)
                                {
                                    if (!comPort.BaseStream.IsOpen)
                                        continue;

                                    if (comPort.BaseStream.BytesToWrite < 50)
                                    {
                                        if (sitl)
                                        {
                                            MissionPlanner.Controls.SITL.rcinput();
                                        }
                                        else
                                        {
                                            comPort.sendPacket(rc, comPort.MAV.sysid, comPort.MAV.compid);
                                        }
                                        count++;
                                        lastjoystick = DateTime.Now;
                                    }
                                }
                            }
                        }
                    }
                    Thread.Sleep(20);
                }
                catch
                {
                } // cant fall out
            }
            joysendThreadExited = true; //so we know this thread exited.    
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
                    if ((string) this.MenuConnect.Image.Tag != "Disconnect")
                    {
                        this.BeginInvoke((MethodInvoker) delegate
                        {
                            this.MenuConnect.Image = displayicons.disconnect;
                            this.MenuConnect.Image.Tag = "Disconnect";
                            this.MenuConnect.Text = Strings.DISCONNECTc;
                            _connectionControl.IsConnected(true);
                        });
                    }
                }
                else
                {
                    if (this.MenuConnect.Image != null && (string) this.MenuConnect.Image.Tag != "Connect")
                    {
                        this.BeginInvoke((MethodInvoker) delegate
                        {
                            this.MenuConnect.Image = displayicons.connect;
                            this.MenuConnect.Image.Tag = "Connect";
                            this.MenuConnect.Text = Strings.CONNECTc;
                            _connectionControl.IsConnected(false);
                            if (_connectionStats != null)
                            {
                                _connectionStats.StopUpdates();
                            }
                        });
                    }

                    if (comPort.logreadmode)
                    {
                        this.BeginInvoke((MethodInvoker) delegate { _connectionControl.IsConnected(true); });
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
                                int msnext = (int) (1000/plugin.loopratehz);
                                // allow the plug to modify this, if needed
                                plugin.NextRun = DateTime.Now.AddMilliseconds(msnext);

                                try
                                {
                                    bool ans = plugin.Loop();
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);
                                }
                            }
                        }
                    }
                }
                catch
                {
                }

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
                catch (Exception ex)
                {
                    log.Error(ex);
                }
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

            int altwarningmax = 0;

            bool armedstatus = false;

            string lastmessagehigh = "";

            DateTime speechcustomtime = DateTime.Now;

            DateTime speechlowspeedtime = DateTime.Now;

            DateTime linkqualitytime = DateTime.Now;

            while (serialThread)
            {
                try
                {
                    Thread.Sleep(1); // was 5

                    try
                    {
                        if (GCSViews.Terminal.comPort is MAVLinkSerialPort)
                        {
                        }
                        else
                        {
                            if (GCSViews.Terminal.comPort != null && GCSViews.Terminal.comPort.IsOpen)
                                continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    // update connect/disconnect button and info stats
                    try
                    {
                        UpdateConnectIcon();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    // 30 seconds interval speech options
                    if (speechEnable && speechEngine != null && (DateTime.Now - speechcustomtime).TotalSeconds > 30 &&
                        (MainV2.comPort.logreadmode || comPort.BaseStream.IsOpen))
                    {
                        if (MainV2.speechEngine.IsReady)
                        {
                            if (Settings.Instance.GetBoolean("speechcustomenabled"))
                            {
                                MainV2.speechEngine.SpeakAsync(ArduPilot.Common.speechConversion(comPort.MAV, ""+ Settings.Instance["speechcustom"]));
                            }

                            speechcustomtime = DateTime.Now;
                        }

                        // speech for battery alerts
                        //speechbatteryvolt
                        float warnvolt = Settings.Instance.GetFloat("speechbatteryvolt");
                        float warnpercent = Settings.Instance.GetFloat("speechbatterypercent");

                        if (Settings.Instance.GetBoolean("speechbatteryenabled") == true &&
                            MainV2.comPort.MAV.cs.battery_voltage <= warnvolt &&
                            MainV2.comPort.MAV.cs.battery_voltage >= 5.0)
                        {
                            if (MainV2.speechEngine.IsReady)
                            {
                                MainV2.speechEngine.SpeakAsync(ArduPilot.Common.speechConversion(comPort.MAV, "" + Settings.Instance["speechbattery"]));
                            }
                        }
                        else if (Settings.Instance.GetBoolean("speechbatteryenabled") == true &&
                                 (MainV2.comPort.MAV.cs.battery_remaining) < warnpercent &&
                                 MainV2.comPort.MAV.cs.battery_voltage >= 5.0 &&
                                 MainV2.comPort.MAV.cs.battery_remaining != 0.0)
                        {
                            if (MainV2.speechEngine.IsReady)
                            {
                                MainV2.speechEngine.SpeakAsync(
                                    ArduPilot.Common.speechConversion(comPort.MAV, "" + Settings.Instance["speechbattery"]));
                            }
                        }
                    }

                    // speech for airspeed alerts
                    if (speechEnable && speechEngine != null && (DateTime.Now - speechlowspeedtime).TotalSeconds > 10 &&
                        (MainV2.comPort.logreadmode || comPort.BaseStream.IsOpen))
                    {
                        if (Settings.Instance.GetBoolean("speechlowspeedenabled") == true && MainV2.comPort.MAV.cs.armed)
                        {
                            float warngroundspeed = Settings.Instance.GetFloat("speechlowgroundspeedtrigger");
                            float warnairspeed = Settings.Instance.GetFloat("speechlowairspeedtrigger");

                            if (MainV2.comPort.MAV.cs.airspeed < warnairspeed)
                            {
                                if (MainV2.speechEngine.IsReady)
                                {
                                    MainV2.speechEngine.SpeakAsync(
                                        ArduPilot.Common.speechConversion(comPort.MAV, "" + Settings.Instance["speechlowairspeed"]));
                                    speechlowspeedtime = DateTime.Now;
                                }
                            }
                            else if (MainV2.comPort.MAV.cs.groundspeed < warngroundspeed)
                            {
                                if (MainV2.speechEngine.IsReady)
                                {
                                    MainV2.speechEngine.SpeakAsync(
                                        ArduPilot.Common.speechConversion(comPort.MAV, "" + Settings.Instance["speechlowgroundspeed"]));
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
                    if (speechEnable && speechEngine != null &&
                        (MainV2.comPort.logreadmode || comPort.BaseStream.IsOpen))
                    {
                        float warnalt = float.MaxValue;
                        if (Settings.Instance.ContainsKey("speechaltheight"))
                        {
                            warnalt = Settings.Instance.GetFloat("speechaltheight");
                        }
                        try
                        {
                            altwarningmax = (int) Math.Max(MainV2.comPort.MAV.cs.alt, altwarningmax);

                            if (Settings.Instance.GetBoolean("speechaltenabled") == true && MainV2.comPort.MAV.cs.alt != 0.00 &&
                                (MainV2.comPort.MAV.cs.alt <= warnalt) && MainV2.comPort.MAV.cs.armed)
                            {
                                if (altwarningmax > warnalt)
                                {
                                    if (MainV2.speechEngine.IsReady)
                                        MainV2.speechEngine.SpeakAsync(
                                            ArduPilot.Common.speechConversion(comPort.MAV, "" +Settings.Instance["speechalt"]));
                                }
                            }
                        }
                        catch
                        {
                        } // silent fail


                        try
                        {
                            // say the latest high priority message
                            if (MainV2.speechEngine.IsReady &&
                                lastmessagehigh != MainV2.comPort.MAV.cs.messageHigh && MainV2.comPort.MAV.cs.messageHigh != null)
                            {
                                if (!MainV2.comPort.MAV.cs.messageHigh.StartsWith("PX4v2 "))
                                {
                                    MainV2.speechEngine.SpeakAsync(MainV2.comPort.MAV.cs.messageHigh);
                                    lastmessagehigh = MainV2.comPort.MAV.cs.messageHigh;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    // not doing anything
                    if (!MainV2.comPort.logreadmode && !comPort.BaseStream.IsOpen)
                    {
                        altwarningmax = 0;
                    }

                    // attenuate the link qualty over time
                    if ((DateTime.Now - MainV2.comPort.MAV.lastvalidpacket).TotalSeconds >= 1)
                    {
                        if (linkqualitytime.Second != DateTime.Now.Second)
                        {
                            MainV2.comPort.MAV.cs.linkqualitygcs = (ushort) (MainV2.comPort.MAV.cs.linkqualitygcs*0.8f);
                            linkqualitytime = DateTime.Now;

                            // force redraw if there are no other packets are being read
                            GCSViews.FlightData.myhud.Invalidate();
                        }
                    }

                    // data loss warning - wait min of 10 seconds, ignore first 30 seconds of connect, repeat at 5 seconds interval
                    if ((DateTime.Now - MainV2.comPort.MAV.lastvalidpacket).TotalSeconds > 10
                        && (DateTime.Now - connecttime).TotalSeconds > 30
                        && (DateTime.Now - nodatawarning).TotalSeconds > 5
                        && (MainV2.comPort.logreadmode || comPort.BaseStream.IsOpen)
                        && MainV2.comPort.MAV.cs.armed)
                    {
                        if (speechEnable && speechEngine != null)
                        {
                            if (MainV2.speechEngine.IsReady)
                            {
                                MainV2.speechEngine.SpeakAsync("WARNING No Data for " +
                                                               (int)
                                                                   (DateTime.Now - MainV2.comPort.MAV.lastvalidpacket)
                                                                       .TotalSeconds + " Seconds");
                                nodatawarning = DateTime.Now;
                            }
                        }
                    }

                    // get home point on armed status change.
                    if (armedstatus != MainV2.comPort.MAV.cs.armed && comPort.BaseStream.IsOpen)
                    {
                        armedstatus = MainV2.comPort.MAV.cs.armed;
                        // status just changed to armed
                        if (MainV2.comPort.MAV.cs.armed == true && 
                            MainV2.comPort.MAV.apname != MAVLink.MAV_AUTOPILOT.INVALID &&
                            MainV2.comPort.MAV.aptype != MAVLink.MAV_TYPE.GIMBAL)
                        {
                            System.Threading.ThreadPool.QueueUserWorkItem(state =>
                            {
                                try
                                {
                                    MainV2.comPort.MAV.cs.HomeLocation = new PointLatLngAlt(MainV2.comPort.getWP(0));
                                    if (MyView.current != null && MyView.current.Name == "FlightPlanner")
                                    {
                                        // update home if we are on flight data tab
                                        this.BeginInvoke((Action) delegate { FlightPlanner.updateHome(); });
                                    }

                                }
                                catch
                                {
                                    // dont hang this loop
                                    this.BeginInvoke(
                                        (Action)
                                            delegate
                                            {
                                                CustomMessageBox.Show("Failed to update home location (" +
                                                                      MainV2.comPort.MAV.sysid + ")");
                                            });
                                }
                            });
                        }

                        if (speechEnable && speechEngine != null)
                        {
                            if (Settings.Instance.GetBoolean("speecharmenabled"))
                            {
                                string speech = armedstatus ? Settings.Instance["speecharm"] : Settings.Instance["speechdisarm"];
                                if (!string.IsNullOrEmpty(speech))
                                {
                                    MainV2.speechEngine.SpeakAsync(ArduPilot.Common.speechConversion(comPort.MAV, speech));
                                }
                            }
                        }
                    }

                    // send a hb every seconds from gcs to ap
                    if (heatbeatSend.Second != DateTime.Now.Second)
                    {
                        MAVLink.mavlink_heartbeat_t htb = new MAVLink.mavlink_heartbeat_t()
                        {
                            type = (byte) MAVLink.MAV_TYPE.GCS,
                            autopilot = (byte) MAVLink.MAV_AUTOPILOT.INVALID,
                            mavlink_version = 3 // MAVLink.MAVLINK_VERSION
                        };

                        // enumerate each link
                        foreach (var port in Comports.ToArray())
                        {
                            if (!port.BaseStream.IsOpen)
                                continue;

                            // poll for params at heartbeat interval - primary mav on this port only
                            if (!port.giveComport)
                            {
                                try
                                {
                                    // poll only when not armed
                                    if (!port.MAV.cs.armed)
                                    {
                                        port.getParamPoll();
                                        port.getParamPoll();
                                    }
                                }
                                catch
                                {
                                }
                            }

                            // there are 3 hb types we can send, mavlink1, mavlink2 signed and unsigned
                            bool sentsigned = false;
                            bool sentmavlink1 = false;
                            bool sentmavlink2 = false;

                            // enumerate each mav
                            foreach (var MAV in port.MAVlist)
                            {
                                try
                                {
                                    // poll for version if we dont have it - every mav every port
                                    if (!MAV.cs.armed && (DateTime.Now.Second % 20) == 0 && MAV.cs.version < new Version(0, 1))
                                        port.getVersion(MAV.sysid, MAV.compid, false);

                                    // are we talking to a mavlink2 device
                                    if (MAV.mavlinkv2)
                                    {
                                        // is signing enabled
                                        if (MAV.signing)
                                        {
                                            // check if we have already sent
                                            if (sentsigned)
                                                continue;
                                            sentsigned = true;
                                        }
                                        else
                                        {
                                            // check if we have already sent
                                            if (sentmavlink2)
                                                continue;
                                            sentmavlink2 = true;
                                        }
                                    }
                                    else
                                    {
                                        // check if we have already sent
                                        if (sentmavlink1)
                                            continue;
                                        sentmavlink1 = true;
                                    }

                                    port.sendPacket(htb, MAV.sysid, MAV.compid);
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);
                                    // close the bad port
                                    try
                                    {
                                        port.Close();
                                    }
                                    catch
                                    {
                                    }
                                    // refresh the screen if needed
                                    if (port == MainV2.comPort)
                                    {
                                        // refresh config window if needed
                                        if (MyView.current != null)
                                        {
                                            this.BeginInvoke((MethodInvoker) delegate()
                                            {
                                                if (MyView.current.Name == "HWConfig")
                                                    MyView.ShowScreen("HWConfig");
                                                if (MyView.current.Name == "SWConfig")
                                                    MyView.ShowScreen("SWConfig");
                                            });
                                        }
                                    }
                                }
                            }
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
                    }

                    // read the interfaces
                    foreach (var port in Comports.ToArray())
                    {
                        if (!port.BaseStream.IsOpen)
                        {
                            // skip primary interface
                            if (port == comPort)
                                continue;

                            // modify array and drop out
                            Comports.Remove(port);
                            port.Dispose();
                            break;
                        }

                        while (port.BaseStream.IsOpen && port.BaseStream.BytesToRead > minbytes &&
                               port.giveComport == false && serialThread)
                        {
                            try
                            {
                                port.readPacket();
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                        // update currentstate of sysids on the port
                        foreach (var MAV in port.MAVlist)
                        {
                            try
                            {
                                MAV.cs.UpdateCurrentSettings(null, false, port, MAV);
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Tracking.AddException(e);
                    log.Error("Serial Reader fail :" + e.ToString());
                    try
                    {
                        comPort.Close();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }

            Console.WriteLine("SerialReader Done");
            SerialThreadrunner.Set();
        }

        protected override void OnLoad(EventArgs e)
        {
            // check if its defined, and force to show it if not known about
            if (Settings.Instance["menu_autohide"] == null)
            {
                Settings.Instance["menu_autohide"] = "false";
            }

            try
            {
                AutoHideMenu(Settings.Instance.GetBoolean("menu_autohide"));
            }
            catch
            {
            }

            MyView.AddScreen(new MainSwitcher.Screen("FlightData", FlightData, true));
            MyView.AddScreen(new MainSwitcher.Screen("FlightPlanner", FlightPlanner, true));
            MyView.AddScreen(new MainSwitcher.Screen("HWConfig", typeof(GCSViews.InitialSetup), false));
            MyView.AddScreen(new MainSwitcher.Screen("SWConfig", typeof(GCSViews.SoftwareConfig), false));
            MyView.AddScreen(new MainSwitcher.Screen("Simulation", Simulation, true));
            MyView.AddScreen(new MainSwitcher.Screen("Terminal", typeof(GCSViews.Terminal), false));
            MyView.AddScreen(new MainSwitcher.Screen("Help", typeof(GCSViews.Help), false));

            try
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                }
                else
                {
                    log.Info("Load Pluggins");
                    Plugin.PluginLoader.LoadAll();
                    log.Info("Load Pluggins... Done");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            if (Program.Logo != null && Program.name == "VVVVZ")
            {
                this.PerformLayout();
                MenuFlightPlanner_Click(this, e);
                MainMenu_ItemClicked(this, new ToolStripItemClickedEventArgs(MenuFlightPlanner));
            }
            else
            {
                this.PerformLayout();
                log.Info("show FlightData");
                MenuFlightData_Click(this, e);
                log.Info("show FlightData... Done");
                MainMenu_ItemClicked(this, new ToolStripItemClickedEventArgs(MenuFlightData));
            }

            // for long running tasks using own threads.
            // for short use threadpool

            this.SuspendLayout();

            // setup http server
            try
            {
                log.Info("start http");
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

            log.Info("start joystick");
            // setup joystick packet sender
            joystickthread = new Thread(new ThreadStart(joysticksend))
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal,
                Name = "Main joystick sender"
            };
            joystickthread.Start();

            log.Info("start serialreader");
            // setup main serial reader
            serialreaderthread = new Thread(SerialReader)
            {
                IsBackground = true,
                Name = "Main Serial reader",
                Priority = ThreadPriority.AboveNormal
            };
            serialreaderthread.Start();

            log.Info("start plugin thread");
            // setup main plugin thread
            pluginthread = new Thread(PluginThread)
            {
                IsBackground = true,
                Name = "plugin runner thread",
                Priority = ThreadPriority.BelowNormal
            };
            pluginthread.Start();

            ThreadPool.QueueUserWorkItem(BGLoadAirports);

            ThreadPool.QueueUserWorkItem(BGCreateMaps);

            //ThreadPool.QueueUserWorkItem(BGGetAlmanac);

            ThreadPool.QueueUserWorkItem(BGgetTFR);

            ThreadPool.QueueUserWorkItem(BGNoFly);

            ThreadPool.QueueUserWorkItem(BGGetKIndex);

            // update firmware version list - only once per day
            ThreadPool.QueueUserWorkItem(BGFirmwareCheck);

            log.Info("start udpvideoshim");
            // start listener
            UDPVideoShim.Start();

            log.Info("start udpmavlinkshim");
            UDPMavlinkShim.Start();

            BinaryLog.onFlightMode += (firmware, modeno) =>
            {
                try
                {
                    var modes = Common.getModesList((Firmwares) Enum.Parse(typeof(Firmwares), firmware));
                    string currentmode = null;

                    foreach (var mode in modes)
                    {
                        if (mode.Key == modeno)
                        {
                            currentmode = mode.Value;
                            break;
                        }
                    }

                    return currentmode;
                }
               catch
                {
                    return null;
                }
            };

            GStreamer.onNewImage += (sender, image) =>
            {
                var old = GCSViews.FlightData.myhud.bgimage;
                GCSViews.FlightData.myhud.bgimage = new Bitmap(image.Width, image.Height, 4*image.Width, PixelFormat.Format32bppPArgb,
                    (image as Utilities.Drawing.Bitmap).LockBits(Rectangle.Empty, null,SKColorType.Bgra8888).Scan0);
                if (old != null)
                    old.Dispose();
            };

            vlcrender.onNewImage += (sender, image) =>
            {
                var old = GCSViews.FlightData.myhud.bgimage;
                GCSViews.FlightData.myhud.bgimage = new Bitmap(image.Width, image.Height, 4 * image.Width, PixelFormat.Format32bppPArgb,
                    (image as Utilities.Drawing.Bitmap).LockBits(Rectangle.Empty, null,SKColorType.Bgra8888).Scan0);
                if (old != null)
                    old.Dispose();
                };

            //ZeroConf.EnumerateAllServicesFromAllHosts();

            //ZeroConf.ProbeForRTSP();

            CommsSerialScan.doConnect += port =>
            {
                if (MainV2.instance.InvokeRequired)
                {
                    MainV2.instance.BeginInvoke(
                        (Action) delegate()
                        {
                            MAVLinkInterface mav = new MAVLinkInterface();
                            mav.BaseStream = port;
                            MainV2.instance.doConnect(mav, "preset", "0");
                            MainV2.Comports.Add(mav);
                        });
                }
                else
                {
                    MAVLinkInterface mav = new MAVLinkInterface();
                    mav.BaseStream = port;
                    MainV2.instance.doConnect(mav, "preset", "0");
                    MainV2.Comports.Add(mav);
                }
            };

            try
            {
                if (!MONO)
                {
                    log.Info("Load AltitudeAngel");
                    AltitudeAngel.Configure();
                    AltitudeAngel.Initialize();
                    log.Info("Load AltitudeAngel... Done");
                }
            }
            catch (TypeInitializationException) // windows xp lacking patch level
            {
                //CustomMessageBox.Show("Please update your .net version. kb2468871");
            }
            catch (Exception ex)
            {
                Tracking.AddException(ex);
            }

            this.ResumeLayout();

            Program.Splash?.Close();

            log.Info("appload time");
            MissionPlanner.Utilities.Tracking.AddTiming("AppLoad", "Load Time",
                (DateTime.Now - Program.starttime).TotalMilliseconds, "");

            bool winXp = Environment.OSVersion.Version.Major == 5;
            if (winXp)
            {
                Common.MessageShowAgain("Windows XP",
                    "This is the last version that will support Windows XP, please update your OS");

                // invalidate update url
                System.Configuration.ConfigurationManager.AppSettings["UpdateLocationVersion"] =
                    "http://firmware.ardupilot.org/MissionPlanner/xp/";
                System.Configuration.ConfigurationManager.AppSettings["UpdateLocation"] =
                    "http://firmware.ardupilot.org/MissionPlanner/xp/";
                System.Configuration.ConfigurationManager.AppSettings["UpdateLocationMD5"] =
                    "http://firmware.ardupilot.org/MissionPlanner/xp/checksums.txt";
                System.Configuration.ConfigurationManager.AppSettings["BetaUpdateLocationVersion"] = "";
            }

            try
            {
                // single update check per day - in a seperate thread
                if (Settings.Instance["update_check"] != DateTime.Now.ToShortDateString())
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(checkupdate);
                    Settings.Instance["update_check"] = DateTime.Now.ToShortDateString();
                }
                else if (Settings.Instance.GetBoolean("beta_updates") == true)
                {
                    MissionPlanner.Utilities.Update.dobeta = true;
                    System.Threading.ThreadPool.QueueUserWorkItem(checkupdate);
                }
            }
            catch (Exception ex)
            {
                log.Error("Update check failed", ex);
            }

            // play a tlog that was passed to the program/ load a bin log passed
            if (Program.args.Length > 0)
            {
                var cmds = ProcessCommandLine(Program.args);

                if (cmds.ContainsKey("file") && File.Exists(cmds["file"]) && cmds["file"].ToLower().EndsWith(".tlog"))
                {
                    FlightData.LoadLogFile(Program.args[0]);
                    FlightData.BUT_playlog_Click(null, null);
                }
                else if (cmds.ContainsKey("file") && File.Exists(cmds["file"]) &&
                         (cmds["file"].ToLower().EndsWith(".log") || cmds["file"].ToLower().EndsWith(".bin")))
                {
                    LogBrowse logbrowse = new LogBrowse();
                    ThemeManager.ApplyThemeTo(logbrowse);
                    logbrowse.logfilename = Program.args[0];
                    logbrowse.Show(this);
                    logbrowse.BringToFront();
                }

                if (cmds.ContainsKey("script") && File.Exists(cmds["script"]))
                {
                    // invoke for after onload finished
                    this.BeginInvoke((Action) delegate()
                    {
                        try
                        {
                            FlightData.selectedscript = cmds["script"];

                            FlightData.BUT_run_script_Click(null, null);
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show("Start script failed: "+ex.ToString(), Strings.ERROR);
                        }
                    });
                }

                if (cmds.ContainsKey("joy") && cmds.ContainsKey("type"))
                {
                    if (cmds["type"].ToLower() == "plane")
                    {
                        MainV2.comPort.MAV.cs.firmware = Firmwares.ArduPlane;
                    }
                    else if (cmds["type"].ToLower() == "copter")
                    {
                        MainV2.comPort.MAV.cs.firmware = Firmwares.ArduCopter2;
                    }
                    else if (cmds["type"].ToLower() == "rover")
                    {
                        MainV2.comPort.MAV.cs.firmware = Firmwares.ArduRover;
                    }
                    else if (cmds["type"].ToLower() == "sub")
                    {
                        MainV2.comPort.MAV.cs.firmware = Firmwares.ArduSub;
                    }

                    var joy = new Joystick.Joystick(() => MainV2.comPort);

                    if (joy.start(cmds["joy"]))
                    {
                        MainV2.joystick = joy;
                        MainV2.joystick.enabled = true;
                    }
                    else
                    {
                        CustomMessageBox.Show("Failed to start joystick");
                    }
                }

                if (cmds.ContainsKey("cam"))
                {
                    try
                    {
                        MainV2.cam = new WebCamService.Capture(int.Parse(cmds["cam"]), null);

                        MainV2.cam.Start();
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show(ex.ToString());
                    }
                }

                if (cmds.ContainsKey("gstream"))
                {
                    GStreamer.gstlaunch = GStreamer.LookForGstreamer();

                    if (!File.Exists(GStreamer.gstlaunch))
                    {
                        if (CustomMessageBox.Show(
                                "A video stream has been detected, but gstreamer has not been configured/installed.\nDo you want to install/config it now?",
                                "GStreamer", System.Windows.Forms.MessageBoxButtons.YesNo) ==
                            (int)System.Windows.Forms.DialogResult.Yes)
                        {
                            UDPVideoShim.DownloadGStreamer();
                        }
                    }

                    try
                    {
                        new Thread(delegate()
                        {
                            // 36 retrys
                            for (int i = 0; i < 36; i++)
                            {
                                try
                                {
                                    var st = GStreamer.StartA(cmds["gstream"]);
                                    if (st == null)
                                    {
                                        // prevent spam
                                        Thread.Sleep(5000);
                                    }
                                    else
                                    {
                                        while (st.IsAlive)
                                        {
                                            Thread.Sleep(1000);
                                        }
                                    }
                                }
                                catch (BadImageFormatException ex)
                                {
                                    // not running on x64
                                    log.Error(ex);
                                    return;
                                }
                                catch (DllNotFoundException ex)
                                {
                                    // missing or failed download
                                    log.Error(ex);
                                    return;
                                }
                            }
                        }) {IsBackground = true}.Start();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                if (cmds.ContainsKey("port") && cmds.ContainsKey("baud"))
                {
                    _connectionControl.CMB_serialport.Text = cmds["port"];
                    _connectionControl.CMB_baudrate.Text = cmds["baud"];

                    doConnect(MainV2.comPort, cmds["port"], cmds["baud"]);
                }
            }

            // show wizard on first use
            if (Settings.Instance["newuser"] == null)
            {
                if (CustomMessageBox.Show("This is your first run, Do you wish to use the setup wizard?\nRecomended for new users.", "Wizard", MessageBoxButtons.YesNo) == (int)System.Windows.Forms.DialogResult.Yes)
                {
                    Wizard.Wizard wiz = new Wizard.Wizard();

                    wiz.ShowDialog(this);
                }

                CustomMessageBox.Show("To use the wizard please goto the initial setup screen, and click the wizard icon.", "Wizard");

                Settings.Instance["newuser"] = DateTime.Now.ToShortDateString();
            }
        }

        private Dictionary<string, string> ProcessCommandLine(string[] args)
        {
            Dictionary<string, string> cmdargs = new Dictionary<string, string>();
            string cmd = "";
            foreach (var s in args)
            {
                if (s.StartsWith("-") || s.StartsWith("/") || s.StartsWith("--"))
                {
                    cmd = s.TrimStart(new char[] {'-', '/', '-'}).TrimStart(new char[] { '-', '/', '-' });
                    continue;
                }
                if (cmd != "")
                {
                    cmdargs[cmd] = s;
                    log.Info("ProcessCommandLine: "+ cmd + " = " + s);
                    cmd = "";
                    continue;
                }
                if (File.Exists(s))
                {
                    // we are not a command, and the file exists.
                    cmdargs["file"] = s;
                    log.Info("ProcessCommandLine: " + "file" + " = " + s);
                    continue;
                }

                log.Info("ProcessCommandLine: UnKnown = " + s);
            }

            return cmdargs;
        }

        private void BGFirmwareCheck(object state)
        {
            try
            {
                if (Settings.Instance["fw_check"] != DateTime.Now.ToShortDateString())
                {
                    var fw = new Firmware();
                    var list = fw.getFWList();
                    if (list.Count > 1)
                        Firmware.SaveSoftwares(new Firmware.optionsObject() { softwares = list });

                    Settings.Instance["fw_check"] = DateTime.Now.ToShortDateString();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void BGGetKIndex(object state)
        {
            try
            {
                // check the last kindex date
                if (Settings.Instance["kindexdate"] == DateTime.Now.ToShortDateString())
                {
                    KIndex_KIndex(Settings.Instance.GetInt32("kindex"), null);
                }
                else
                {
                    // get a new kindex
                    KIndex.KIndexEvent += KIndex_KIndex;
                    KIndex.GetKIndex();

                    Settings.Instance["kindexdate"] = DateTime.Now.ToShortDateString();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void BGgetTFR(object state)
        {
            try
            {
                tfr.tfrcache = Settings.GetUserDataDirectory() + "tfr.xml";
                tfr.GetTFRs();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void BGNoFly(object state)
        {
            try
            {
                NoFly.NoFly.Scan();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }


        void KIndex_KIndex(object sender, EventArgs e)
        {
            CurrentState.KIndexstatic = (int) sender;
            Settings.Instance["kindex"] = CurrentState.KIndexstatic.ToString();
        }

        private void BGCreateMaps(object state)
        {
            // sort logs
            try
            {
                MissionPlanner.Log.LogSort.SortLogs(Directory.GetFiles(Settings.Instance.LogDir, "*.tlog"));

                MissionPlanner.Log.LogSort.SortLogs(Directory.GetFiles(Settings.Instance.LogDir, "*.rlog"));
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            try
            {
                // create maps
                Log.LogMap.MapLogs(Directory.GetFiles(Settings.Instance.LogDir, "*.tlog", SearchOption.AllDirectories));
                Log.LogMap.MapLogs(Directory.GetFiles(Settings.Instance.LogDir, "*.bin", SearchOption.AllDirectories));
                Log.LogMap.MapLogs(Directory.GetFiles(Settings.Instance.LogDir, "*.log", SearchOption.AllDirectories));

                if (File.Exists(tlogThumbnailHandler.tlogThumbnailHandler.queuefile))
                {
                    Log.LogMap.MapLogs(File.ReadAllLines(tlogThumbnailHandler.tlogThumbnailHandler.queuefile));

                    File.Delete(tlogThumbnailHandler.tlogThumbnailHandler.queuefile);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            try
            {
                if (File.Exists(tlogThumbnailHandler.tlogThumbnailHandler.queuefile))
                {
                    Log.LogMap.MapLogs(File.ReadAllLines(tlogThumbnailHandler.tlogThumbnailHandler.queuefile));

                    File.Delete(tlogThumbnailHandler.tlogThumbnailHandler.queuefile);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void checkupdate(object stuff)
        {
            if (Program.WindowsStoreApp)
                return;

            try
            {
                MissionPlanner.Utilities.Update.CheckForUpdate();
            }
            catch (Exception ex)
            {
                log.Error("Update check failed", ex);
            }
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
            if (GCSViews.Terminal.SSHTerminal) { return false; }
            if (keyData == Keys.F12)
            {
                MenuConnect_Click(null, null);
                return true;
            }

            if (keyData == Keys.F2)
            {
                MenuFlightData_Click(null, null);
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
            if (keyData == (Keys.Control | Keys.X)) 
            {
                Video v = new Video();
                v.Show();
            }
            if (keyData == (Keys.Control | Keys.L)) // limits
            {
                new GCSViews.ConfigurationView.ConfigUAVCAN().ShowUserControl();

                return true;
            }
            if (keyData == (Keys.Control | Keys.W)) // test ac config
            {
                new PropagationSettings().Show();

                return true;
            }
            if (keyData == (Keys.Control | Keys.Z))
            {
                //WHOAMI
//#define MPUREG_WHOAMI                               0x75
                //INV2REG_WHO_AM_I               INV2REG(REG_BANK0,0x00U)

                foreach (var item in Enum.GetNames(typeof(hal_ins_spi)))
                {
                    log.Info(item);
                    MainV2.comPort.device_op((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.DEVICE_OP_BUSTYPE.SPI, item, 0, 0, 0x00, 1);
                }

                foreach (var item in Enum.GetNames(typeof(hal_ins_i2c)))
                {
                    log.Info(item);
                    var test = (byte)(int)Enum.Parse(typeof(hal_ins_i2c), item);                  
                    MainV2.comPort.device_op((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.DEVICE_OP_BUSTYPE.I2C, "", 0, (byte)test, 0, 1);
                    MainV2.comPort.device_op((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.DEVICE_OP_BUSTYPE.I2C, "", 1, (byte)test, 0, 1);
                    MainV2.comPort.device_op((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.DEVICE_OP_BUSTYPE.I2C, "", 2, (byte)test, 0, 1);
                    MainV2.comPort.device_op((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.DEVICE_OP_BUSTYPE.I2C, "", 3, (byte)test, 0, 1);
                }

                // enable led
                MainV2.comPort.device_op((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.DEVICE_OP_BUSTYPE.I2C, "", 1, (byte)hal_ins_i2c.TOSHIBA_LED_I2C_ADDR, 4, 1, new byte[] { 3 });
                // get register 1 
                MainV2.comPort.device_op((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.DEVICE_OP_BUSTYPE.I2C, "", 1, (byte)hal_ins_i2c.TOSHIBA_LED_I2C_ADDR, 1, 4);
                // write pwm0 reg 1
                byte r = 0 ;
                byte g =0;
                byte b =0;
                for (r = 0; r <= 16; r++)
                {
                    for (g = 0; g <= 16; g++)
                    {
                        for (b = 0; b <= 16; b++)
                        {
                            MainV2.comPort.device_op((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.DEVICE_OP_BUSTYPE.I2C, "", 1, (byte)hal_ins_i2c.TOSHIBA_LED_I2C_ADDR, 1, 3, new byte[] { b, g, r }); // b g r 
                        }
                    }
                }
                // get register 1 
                MainV2.comPort.device_op((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.DEVICE_OP_BUSTYPE.I2C, "", 1, (byte)hal_ins_i2c.TOSHIBA_LED_I2C_ADDR, 1, 4);

                return true;
            }
            if (keyData == (Keys.Control | Keys.T)) // for override connect
            {
                try
                {
                    MainV2.comPort.Open(false);
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(ex.ToString());
                }
                return true;
            }
            if (keyData == (Keys.Control | Keys.Y)) // for ryan beall and ollyw42
            {
                // write
                try
                {
                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_STORAGE, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
                }
                catch
                {
                    CustomMessageBox.Show("Invalid command");
                    return true;
                }
                //read
                ///////MainV2.comPort.doCommand(MAVLink09.MAV_CMD.PREFLIGHT_STORAGE, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
                CustomMessageBox.Show("Done MAV_ACTION_STORAGE_WRITE");
                return true;
            }
            if (keyData == (Keys.Control | Keys.J))
            {
                GMapControl.GDI = !GMapControl.GDI;

                return true;
            }

            if (ProcessCmdKeyCallback != null)
            {
                return ProcessCmdKeyCallback(ref msg, keyData);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public delegate bool ProcessCmdKeyHandler(ref Message msg, Keys keyData);

        public event ProcessCmdKeyHandler ProcessCmdKeyCallback;

        public void changelanguage(CultureInfo ci)
        {
            log.Info("change lang to " + ci.ToString() + " current " + Thread.CurrentThread.CurrentUICulture.ToString());

            if (ci != null && !Thread.CurrentThread.CurrentUICulture.Equals(ci))
            {
                Thread.CurrentThread.CurrentUICulture = ci;
                Settings.Instance["language"] = ci.Name;
                //System.Threading.Thread.CurrentThread.CurrentCulture = ci;

                HashSet<Control> views = new HashSet<Control> {this, FlightData, FlightPlanner, Simulation};

                foreach (Control view in MyView.Controls)
                    views.Add(view);

                foreach (Control view in views)
                {
                    if (view != null)
                    {
                        ComponentResourceManager rm = new ComponentResourceManager(view.GetType());
                        foreach (Control ctrl in view.Controls)
                        {
                            rm.ApplyResource(ctrl);
                        }
                        rm.ApplyResources(view, "$this");
                    }
                }
            }
        }


        public void ChangeUnits()
        {
            try
            {
                // dist
                if (Settings.Instance["distunits"] != null)
                {
                    switch (
                        (distances) Enum.Parse(typeof (distances), Settings.Instance["distunits"].ToString()))
                    {
                        case distances.Meters:
                            CurrentState.multiplierdist = 1;
                            CurrentState.DistanceUnit = "m";
                            break;
                        case distances.Feet:
                            CurrentState.multiplierdist = 3.2808399f;
                            CurrentState.DistanceUnit = "ft";
                            break;
                    }
                }
                else
                {
                    CurrentState.multiplierdist = 1;
                    CurrentState.DistanceUnit = "m";
                }

                // alt
                if (Settings.Instance["altunits"] != null)
                {
                    switch (
                        (distances)Enum.Parse(typeof(altitudes), Settings.Instance["altunits"].ToString()))
                    {
                        case distances.Meters:
                            CurrentState.multiplieralt = 1;
                            CurrentState.AltUnit = "m";
                            break;
                        case distances.Feet:
                            CurrentState.multiplieralt = 3.2808399f;
                            CurrentState.AltUnit = "ft";
                            break;
                    }
                }
                else
                {
                    CurrentState.multiplieralt = 1;
                    CurrentState.AltUnit = "m";
                }

                // speed
                if (Settings.Instance["speedunits"] != null)
                {
                    switch ((speeds) Enum.Parse(typeof (speeds), Settings.Instance["speedunits"].ToString()))
                    {
                        case speeds.meters_per_second:
                            CurrentState.multiplierspeed = 1;
                            CurrentState.SpeedUnit = "m/s";
                            break;
                        case speeds.fps:
                            CurrentState.multiplierdist = 3.2808399f;
                            CurrentState.SpeedUnit = "fps";
                            break;
                        case speeds.kph:
                            CurrentState.multiplierspeed = 3.6f;
                            CurrentState.SpeedUnit = "kph";
                            break;
                        case speeds.mph:
                            CurrentState.multiplierspeed = 2.23693629f;
                            CurrentState.SpeedUnit = "mph";
                            break;
                        case speeds.knots:
                            CurrentState.multiplierspeed = 1.94384449f;
                            CurrentState.SpeedUnit = "kts";
                            break;
                    }
                }
                else
                {
                    CurrentState.multiplierspeed = 1;
                    CurrentState.SpeedUnit = "m/s";
                }
            }
            catch
            {
            }
        }

        private void CMB_baudrate_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(_connectionControl.CMB_baudrate.Text, out comPortBaud))
            {
                CustomMessageBox.Show(Strings.InvalidBaudRate, Strings.ERROR);
                return;
            }
            var sb = new StringBuilder();
            int baud = 0;
            for (int i = 0; i < _connectionControl.CMB_baudrate.Text.Length; i++)
                if (char.IsDigit(_connectionControl.CMB_baudrate.Text[i]))
                {
                    sb.Append(_connectionControl.CMB_baudrate.Text[i]);
                    baud = baud*10 + _connectionControl.CMB_baudrate.Text[i] - '0';
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
            panel1.Location = new Point(0, 0);
            panel1.Width = menu.Width;
            panel1.BringToFront();
            panel1.Visible = true;
            this.ResumeLayout();
        }

        private void autoHideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoHideMenu(autoHideToolStripMenuItem.Checked);

            Settings.Instance["menu_autohide"] = autoHideToolStripMenuItem.Checked.ToString();
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
            Message temp = new Message();
            ProcessCmdKey(ref temp, e.KeyData);
            Console.WriteLine("MainV2_KeyDown " + e.ToString());
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(
                    "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=mich146%40hotmail%2ecom&lc=AU&item_name=Michael%20Oborne&no_note=0&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHostedGuest");
            }
            catch
            {
                CustomMessageBox.Show("Link open failed. check your default webpage association");
            }
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)] public byte[] dbcp_name;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class DEV_BROADCAST_DEVICEINTERFACE
        {
            public Int32 dbcc_size;
            public Int32 dbcc_devicetype;
            public Int32 dbcc_reserved;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)] internal Byte[]
                dbcc_classguid;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)] internal Byte[] dbcc_name;
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


                        deviceNotificationHandle = NativeMethods.RegisterDeviceNotification(this.Handle,
                            devBroadcastDeviceInterfaceBuffer, DEVICE_NOTIFY_WINDOW_HANDLE);
                    }
                    catch
                    {
                    }

                    break;

                case WM_DEVICECHANGE:
                    // The WParam value identifies what is occurring.
                    WM_DEVICECHANGE_enum n = (WM_DEVICECHANGE_enum) m.WParam;
                    var l = m.LParam;
                    if (n == WM_DEVICECHANGE_enum.DBT_DEVICEREMOVEPENDING)
                    {
                        Console.WriteLine("DBT_DEVICEREMOVEPENDING");
                    }
                    if (n == WM_DEVICECHANGE_enum.DBT_DEVNODES_CHANGED)
                    {
                        Console.WriteLine("DBT_DEVNODES_CHANGED");
                    }
                    if (n == WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL ||
                        n == WM_DEVICECHANGE_enum.DBT_DEVICEREMOVECOMPLETE)
                    {
                        Console.WriteLine(((WM_DEVICECHANGE_enum) n).ToString());

                        DEV_BROADCAST_HDR hdr = new DEV_BROADCAST_HDR();
                        Marshal.PtrToStructure(m.LParam, hdr);

                        try
                        {
                            switch (hdr.dbch_devicetype)
                            {
                                case DBT_DEVTYP_DEVICEINTERFACE:
                                    DEV_BROADCAST_DEVICEINTERFACE inter = new DEV_BROADCAST_DEVICEINTERFACE();
                                    Marshal.PtrToStructure(m.LParam, inter);
                                    log.InfoFormat("Interface {0}",
                                        ASCIIEncoding.Unicode.GetString(inter.dbcc_name, 0, inter.dbcc_size - (4*3)));
                                    break;
                                case DBT_DEVTYP_PORT:
                                    DEV_BROADCAST_PORT prt = new DEV_BROADCAST_PORT();
                                    Marshal.PtrToStructure(m.LParam, prt);
                                    log.InfoFormat("port {0}",
                                        ASCIIEncoding.Unicode.GetString(prt.dbcp_name, 0, prt.dbcp_size - (4*3)));
                                    break;
                            }
                        }
                        catch
                        {
                        }

                        //string port = Marshal.PtrToStringAuto((IntPtr)((long)m.LParam + 12));
                        //Console.WriteLine("Added port {0}",port);
                    }
                    log.InfoFormat("Device Change {0} {1} {2}", m.Msg, (WM_DEVICECHANGE_enum) m.WParam, m.LParam);

                    if (DeviceChanged != null)
                    {
                        try
                        {
                            DeviceChanged((WM_DEVICECHANGE_enum) m.WParam);
                        }
                        catch
                        {
                        }
                    }

                    foreach (var item in MissionPlanner.Plugin.PluginLoader.Plugins)
                    {
                        item.Host.ProcessDeviceChanged((WM_DEVICECHANGE_enum) m.WParam);
                    }

                    break;
                case 0x86: // WM_NCACTIVATE
                    //var thing = Control.FromHandle(m.HWnd);

                    var child = Control.FromHandle(m.LParam);

                    if (child is Form)
                    {
                        log.Debug("ApplyThemeTo " + child.Name);
                        ThemeManager.ApplyThemeTo(child);
                    }
                    break;
                default:
                    //Console.WriteLine(m.ToString());
                    break;
            }
            base.WndProc(ref m);
        }

        const int DBT_DEVTYP_PORT = 0x00000003;
        const int WM_CREATE = 0x0001;
        const Int32 DBT_DEVTYP_HANDLE = 6;
        const Int32 DBT_DEVTYP_DEVICEINTERFACE = 5;
        const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        const Int32 DIGCF_PRESENT = 2;
        const Int32 DIGCF_DEVICEINTERFACE = 0X10;
        const Int32 WM_DEVICECHANGE = 0X219;
        public static Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");


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
                    item.BackgroundImage = displayicons.bg; //.BackColor = Color.Black;
                }
            }
            //MainMenu.BackColor = Color.Black;
            //MainMenu.BackgroundImage = MissionPlanner.Properties.Resources.bgdark;
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // full screen
            if (fullScreenToolStripMenuItem.Checked)
            {
                this.TopMost = true;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.WindowState = FormWindowState.Normal;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.TopMost = false;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void readonlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainV2.comPort.ReadOnly = readonlyToolStripMenuItem.Checked;
        }

        private void connectionOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ConnectionOptions().Show(this);
        }

        private void MenuArduPilot_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://ardupilot.org/?utm_source=Menu&utm_campaign=MP");
            }
            catch
            {
                CustomMessageBox.Show("Failed to open url http://ardupilot.org");
            }
        }

        private void connectionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            if (File.Exists(openFileDialog.FileName))
            {
                var lines = File.ReadAllLines(openFileDialog.FileName);

                Regex tcp = new Regex("tcp://(.*):([0-9]+)");
                Regex udp = new Regex("udp://(.*):([0-9]+)");
                Regex udpcl = new Regex("udpcl://(.*):([0-9]+)");
                Regex serial = new Regex("serial:(.*):([0-9]+)");

                ConcurrentBag<MAVLinkInterface> mavs = new ConcurrentBag<MAVLinkInterface>();

                Parallel.ForEach(lines, line =>
                //foreach (var line in lines)
                {
                    try
                    {
                        MAVLinkInterface mav = new MAVLinkInterface();

                        if (tcp.IsMatch(line))
                        {
                            var matches = tcp.Match(line);
                            var tc = new TcpSerial();
                            tc.client = new TcpClient(matches.Groups[1].Value, int.Parse(matches.Groups[2].Value));
                            mav.BaseStream = tc;
                        }
                        else if (udp.IsMatch(line))
                        {
                            var matches = udp.Match(line);
                            var uc = new UdpSerial(new UdpClient(int.Parse(matches.Groups[2].Value)));
                            uc.Port = matches.Groups[2].Value;
                            mav.BaseStream = uc;
                        }
                        else if (udpcl.IsMatch(line))
                        {
                            var matches = udpcl.Match(line);
                            var udc = new UdpSerialConnect();
                            udc.Port = matches.Groups[2].Value;
                            udc.client = new UdpClient(matches.Groups[1].Value, int.Parse(matches.Groups[2].Value));
                            mav.BaseStream = udc;
                        }
                        else if (serial.IsMatch(line))
                        {
                            var matches = serial.Match(line);
                            var port = new Comms.SerialPort();
                            port.PortName = matches.Groups[1].Value;
                            port.BaudRate = int.Parse(matches.Groups[2].Value);
                            mav.BaseStream = port;
                            mav.BaseStream.Open();
                        }
                        else
                        {
                            return;
                        }

                        mavs.Add(mav);
                    }
                    catch
                    {
                    }
                }
                );

                foreach (var mav in mavs)
                {
                    MainV2.instance.BeginInvoke((Action)delegate
                    {
                        doConnect(mav, "preset", "0", false);
                        Comports.Add(mav);
                    });
                }
            }
        }
    }
}