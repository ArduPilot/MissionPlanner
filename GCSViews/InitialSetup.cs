using System;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.GCSViews.ConfigurationView;
using MissionPlanner.Utilities;
using System.Resources;
using MissionPlanner.ArduPilot;
using MissionPlanner.Radio;

namespace MissionPlanner.GCSViews
{
    public partial class InitialSetup : MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string lastpagename = "";

        public InitialSetup()
        {
            InitializeComponent();
        }

        public bool isConnected
        {
            get { return MainV2.comPort.BaseStream.IsOpen; }
        }

        public bool isDisConnected
        {
            get { return !MainV2.comPort.BaseStream.IsOpen; }
        }

        public bool isTracker
        {
            get { return isConnected && MainV2.comPort.MAV.cs.firmware == Firmwares.ArduTracker; }
        }

        public bool isCopter
        {
            get { return isConnected && MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2; }
        }

        public bool isCopter35plus
        {
            get { return MainV2.comPort.MAV.cs.version >= Version.Parse("3.5"); }
        }

        public bool isHeli
        {
            get { return isConnected && MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER; }
        }

        public bool isQuadPlane
        {
            get
            {
                return isConnected && isPlane &&
                       MainV2.comPort.MAV.param.ContainsKey("Q_ENABLE") &&
                       (MainV2.comPort.MAV.param["Q_ENABLE"].Value == 1.0);
            }
        }

        public bool isPlane
        {
            get
            {
                return isConnected &&
                       (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane ||
                        MainV2.comPort.MAV.cs.firmware == Firmwares.Ateryx);
            }
        }

        public bool isRover
        {
            get { return isConnected && MainV2.comPort.MAV.cs.firmware == Firmwares.ArduRover; }
        }

        private BackstageViewPage AddBackstageViewPage(Type userControl, string headerText, bool enabled = true,
    BackstageViewPage Parent = null, bool advanced = false)
        {
            try
            {
                if (enabled)
                    return backstageView.AddPage(userControl, headerText, Parent, advanced);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }

            return null;
        }

        public void Activate()
        {
        }

        private void HardwareConfig_Load(object sender, EventArgs e)
        {
            ResourceManager rm = new ResourceManager(this.GetType());

            if (MainV2.DisplayConfiguration.displayInstallFirmware)
            {
                if (!Program.WindowsStoreApp)
                {
                    AddBackstageViewPage(typeof(ConfigFirmwareDisabled), rm.GetString("backstageViewPagefw.Text"),
                        isConnected);
                    AddBackstageViewPage(typeof(ConfigFirmware), rm.GetString("backstageViewPagefw.Text"),
                        isDisConnected);
                }
            }

            if (MainV2.DisplayConfiguration.displayWizard)
            {
                AddBackstageViewPage(typeof(ConfigWizard), rm.GetString("backstageViewPagewizard.Text"));
            }

            var mand = AddBackstageViewPage(typeof(ConfigMandatory), rm.GetString("backstageViewPagemand.Text"), isConnected);

            if (MainV2.DisplayConfiguration.displayFrameType)
            {
                AddBackstageViewPage(typeof(ConfigTradHeli), rm.GetString("backstageViewPagetradheli.Text"), isHeli, mand);
                AddBackstageViewPage(typeof(ConfigFrameType), rm.GetString("backstageViewPageframetype.Text"), isCopter && !isCopter35plus, mand);
                AddBackstageViewPage(typeof(ConfigFrameClassType), rm.GetString("backstageViewPageframetype.Text"), isCopter && isCopter35plus, mand);
            }

            if (MainV2.DisplayConfiguration.displayAccelCalibration)
            {
                AddBackstageViewPage(typeof(ConfigAccelerometerCalibration), rm.GetString("backstageViewPageaccel.Text"), isConnected, mand);
            }

            AddBackstageViewPage(typeof(ConfigHWCompass), rm.GetString("backstageViewPagecompass.Text"), isConnected, mand);

            if (MainV2.DisplayConfiguration.displayRadioCalibration)
            {
                AddBackstageViewPage(typeof(ConfigRadioInput), rm.GetString("backstageViewPageradio.Text"), isConnected, mand);
                AddBackstageViewPage(typeof(ConfigRadioOutput), "Servo Output", isConnected, mand);
                
            }
            if (MainV2.DisplayConfiguration.displayEscCalibration)
            {
                AddBackstageViewPage(typeof(ConfigESCCalibration), "ESC Calibration", isConnected, mand);
            }
            if (MainV2.DisplayConfiguration.displayFlightModes)
            {
                AddBackstageViewPage(typeof(ConfigFlightModes), rm.GetString("backstageViewPageflmode.Text"), isConnected, mand);
            }
            if (MainV2.DisplayConfiguration.displayFailSafe)
            {
                AddBackstageViewPage(typeof(ConfigFailSafe), rm.GetString("backstageViewPagefs.Text"), isConnected, mand);
            }

            AddBackstageViewPage(typeof(ConfigHWIDs), "HW ID", isConnected, mand);

            var opt = AddBackstageViewPage(typeof(ConfigOptional), rm.GetString("backstageViewPageopt.Text"));
            AddBackstageViewPage(typeof(ConfigGPSInject), "RTK/GPS Inject", true, opt);
            if (MainV2.DisplayConfiguration.displaySikRadio)
            {
                AddBackstageViewPage(typeof(Sikradio), rm.GetString("backstageViewPageSikradio.Text"), true, opt);
            }
            if (MainV2.DisplayConfiguration.displayBattMonitor)
            {
                AddBackstageViewPage(typeof(ConfigBatteryMonitoring), rm.GetString("backstageViewPagebatmon.Text"), isConnected, opt);
                AddBackstageViewPage(typeof(ConfigBatteryMonitoring2), rm.GetString("backstageViewPageBatt2.Text"), isConnected, opt);
            }
            if (MainV2.DisplayConfiguration.displayCAN)
            {
                AddBackstageViewPage(typeof(ConfigHWCAN), "CAN", isConnected, opt);
            }

            AddBackstageViewPage(typeof(ConfigUAVCAN), "UAVCAN", isConnected, opt);

            if (MainV2.DisplayConfiguration.displayCompassMotorCalib)
            {
                AddBackstageViewPage(typeof(ConfigCompassMot), rm.GetString("backstageViewPagecompassmot.Text"), isConnected, opt);
            }
            if (MainV2.DisplayConfiguration.displayRangeFinder)
            {
                AddBackstageViewPage(typeof(ConfigHWRangeFinder), rm.GetString("backstageViewPagesonar.Text"), isConnected, opt);
            }
            if (MainV2.DisplayConfiguration.displayAirSpeed)
            {
                AddBackstageViewPage(typeof(ConfigHWAirspeed), rm.GetString("backstageViewPageairspeed.Text"), isConnected, opt);
            }
            if (MainV2.DisplayConfiguration.displayPx4Flow)
            {
                AddBackstageViewPage(typeof(ConfigHWPX4Flow), rm.GetString("backstageViewPagePX4Flow.Text"), true, opt);
            }
            if (MainV2.DisplayConfiguration.displayOpticalFlow)
            {
                AddBackstageViewPage(typeof(ConfigHWOptFlow), rm.GetString("backstageViewPageoptflow.Text"), isConnected, opt);
            }
            if (MainV2.DisplayConfiguration.displayOsd)
            {
                AddBackstageViewPage(typeof(ConfigHWOSD), rm.GetString("backstageViewPageosd.Text"), isConnected, opt);
            }
            if (MainV2.DisplayConfiguration.displayCameraGimbal)
            {
                AddBackstageViewPage(typeof(ConfigMount), rm.GetString("backstageViewPagegimbal.Text"), isConnected, opt);
            }
            if (MainV2.DisplayConfiguration.displayAntennaTracker)
            {
                AddBackstageViewPage(typeof(ConfigAntennaTracker), rm.GetString("backstageViewPageAntTrack.Text"), isTracker, opt);
            }
            if (MainV2.DisplayConfiguration.displayMotorTest)
            {
                AddBackstageViewPage(typeof(ConfigMotorTest), rm.GetString("backstageViewPageMotorTest.Text"), isConnected, opt);
            }
            if (MainV2.DisplayConfiguration.displayBluetooth)
            {
                AddBackstageViewPage(typeof(ConfigHWBT), rm.GetString("backstageViewPagehwbt.Text"), true, opt);
            }
            if (MainV2.DisplayConfiguration.displayParachute)
            {
                AddBackstageViewPage(typeof(ConfigHWParachute), rm.GetString("backstageViewPageParachute.Text"), isConnected, opt);
            }
            if (MainV2.DisplayConfiguration.displayEsp)
            {
                AddBackstageViewPage(typeof(ConfigHWESP8266), rm.GetString("backstageViewPageESP.Text"), isConnected, opt);
            }
            if (MainV2.DisplayConfiguration.displayAntennaTracker)
            {
                AddBackstageViewPage(typeof(Antenna.Tracker), "Antenna Tracker", true, opt);
            }

            AddBackstageViewPage(typeof(ConfigFFT), "FFT Setup", isConnected, opt);

            // remeber last page accessed
            foreach (BackstageViewPage page in backstageView.Pages)
            {
                if (page.LinkText == lastpagename && page.Show)
                {
                    backstageView.ActivatePage(page);
                    break;
                }
            }

            ThemeManager.ApplyThemeTo(this);
        }

        private void HardwareConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backstageView.SelectedPage != null)
                lastpagename = backstageView.SelectedPage.LinkText;

            backstageView.Close();
        }
    }
}