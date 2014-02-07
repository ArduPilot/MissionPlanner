using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.Utilities;
using MissionPlanner.GCSViews.ConfigurationView;
using log4net;
using System.Reflection;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews
{
    public partial class InitialSetup : MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static string lastpagename = "";

        public bool isConnected { get { return MainV2.comPort.BaseStream.IsOpen; } }

        public bool isDisConnected { get { return !MainV2.comPort.BaseStream.IsOpen; } }

        public bool isCopter { get { return isConnected && MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2; } }

        public bool isHeli { get { return isConnected && MainV2.comPort.MAV.param["H_SWASH_TYPE"] != null; } }

        public bool isPlane { get { return isConnected && (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane || MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx); } }

        public bool isRover { get { return isConnected && MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover; } }

        public InitialSetup()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            initialSetupBindingSource.DataSource = this;
        }

        private BackstageViewPage AddBackstageViewPage(UserControl userControl, string headerText, BackstageViewPage Parent = null, bool advanced = false)
        {
            try
            {
                log.Debug("adding page "+ headerText);
                var obj = backstageView.AddPage(userControl, headerText, Parent, advanced);
                log.Debug("done page " + headerText);
                return obj;
            }
            catch (Exception ex) { log.Error(ex); return null; }
        }

        private void HardwareConfig_Load(object sender, EventArgs e)
        {
            // remeber last page accessed
            foreach (BackstageViewPage page in backstageView.Pages)
            {
                if (page.LinkText == lastpagename && page.Show)
                {
                    this.backstageView.ActivatePage(page);
                    break;
                }
            }

            ThemeManager.ApplyThemeTo(this);

            return;

            backstageView.Pages.Clear();

            try
            {
                BackstageViewPage start;

                

                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    AddBackstageViewPage(new MissionPlanner.GCSViews.ConfigurationView.ConfigFirmwareDisabled(), "Install Firmware");

                    AddBackstageViewPage(new MissionPlanner.GCSViews.ConfigurationView.ConfigWizard(), "Wizard");

                    BackstageViewPage mandatoryhardware = AddBackstageViewPage(new ConfigMandatory(), "Mandatory Hardware", null);
                    BackstageViewPage optionalhardware = AddBackstageViewPage(new ConfigOptional(), "Optional Hardware", null);

                    start = mandatoryhardware;

                    if (MainV2.comPort.MAV.param["H_SWASH_TYPE"] != null)
                    {
                        AddBackstageViewPage(new ConfigTradHeli(), "Heli Setup", mandatoryhardware);
                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                    {
                        AddBackstageViewPage(new ConfigDefaultSettings(), "Load default params\nfor standard frames", mandatoryhardware);

                        AddBackstageViewPage(new ConfigFrameType(), "Frame Type", mandatoryhardware);
                    }

                    AddBackstageViewPage(new ConfigHWCompass(), "Compass", mandatoryhardware);

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                    {
                        AddBackstageViewPage(new ConfigAccelerometerCalibrationQuad(), "Accel Calibration", mandatoryhardware);
                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
                    {
                        AddBackstageViewPage(new ConfigAccelerometerCalibrationPlane(), "Accel Calibration", mandatoryhardware);
                    }

                    AddBackstageViewPage(new ConfigRadioInput(), "Radio Calibration", mandatoryhardware);

                    AddBackstageViewPage(new ConfigFlightModes(), "Flight Modes", mandatoryhardware);

                    AddBackstageViewPage(new ConfigFailSafe(), "FailSafe", mandatoryhardware);

                    AddBackstageViewPage(new MissionPlanner._3DRradio(), "3DR Radio", optionalhardware);
                    AddBackstageViewPage(new ConfigBatteryMonitoring(), "Battery Monitor", optionalhardware);
                    AddBackstageViewPage(new ConfigHWSonar(), "Sonar", optionalhardware);
                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane || MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx)
                        AddBackstageViewPage(new ConfigHWAirspeed(), "Airspeed", optionalhardware);
                    AddBackstageViewPage(new ConfigHWOptFlow(), "Optical Flow", optionalhardware);
                    AddBackstageViewPage(new ConfigHWOSD(), "OSD", optionalhardware);
                    // opt flow
                    // osd
                    AddBackstageViewPage(new ConfigMount(), "Camera Gimbal", optionalhardware);

                    AddBackstageViewPage(new MissionPlanner.Antenna.Tracker(), "Antenna Tracker", optionalhardware);


                }
                else
                {
                    AddBackstageViewPage(new MissionPlanner.GCSViews.ConfigurationView.ConfigFirmware(), "Install Firmware");
                    AddBackstageViewPage(new MissionPlanner.GCSViews.ConfigurationView.ConfigWizard(), "Wizard");
                    AddBackstageViewPage(new MissionPlanner._3DRradio(), "3DR Radio");
                    AddBackstageViewPage(new MissionPlanner.Antenna.Tracker(), "Antenna Tracker");
                }

  

                log.Debug("Draw Menu");
                if (backstageView.SelectedPage == null)
                    backstageView.DrawMenu(null, true);

                ThemeManager.ApplyThemeTo(this);
            }
            catch (Exception ex) { log.Error(ex); }
        }

        private void HardwareConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backstageView.SelectedPage != null)
                lastpagename = backstageView.SelectedPage.LinkText;

            backstageView.Close();
        }
    }
}
