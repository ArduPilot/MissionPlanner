using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls.BackstageView;
using ArdupilotMega.Utilities;
using ArdupilotMega.GCSViews.ConfigurationView;
using log4net;
using System.Reflection;
using ArdupilotMega.Controls;

namespace ArdupilotMega.GCSViews
{
    public partial class InitialSetup : MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static string lastpagename = "";

        public InitialSetup()
        {
            InitializeComponent();

        }

        public void Activate()
        {
            MissionPlanner.Utilities.Tracking.AddPage(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private BackstageView.BackstageViewPage AddBackstageViewPage(UserControl userControl, string headerText, BackstageView.BackstageViewPage Parent = null)
        {
            try
            {
                return backstageView.AddPage(userControl, headerText, Parent);
            }
            catch (Exception ex) { log.Error(ex); return null; }
        }

        private void HardwareConfig_Load(object sender, EventArgs e)
        {
            BackstageView.BackstageViewPage start;
            if (MainV2.comPort.BaseStream.IsOpen)
            {
                AddBackstageViewPage(new ArdupilotMega.GCSViews.ConfigurationView.ConfigFirmwareDisabled(), "Install Firmware");

                BackstageView.BackstageViewPage mandatoryhardware = AddBackstageViewPage(new ConfigMandatory(), "Mandatory Hardware", null);
                BackstageView.BackstageViewPage optionalhardware = AddBackstageViewPage(new ConfigOptional(), "Optional Hardware", null);

                start = mandatoryhardware;

                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                {
                    AddBackstageViewPage(new ConfigFrameType(), "Frame Type", mandatoryhardware);
                }

                AddBackstageViewPage(new ConfigHWCompass(), "Compass",mandatoryhardware);

                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2 || MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduHeli)
                {
                    AddBackstageViewPage(new ConfigAccelerometerCalibrationQuad(), "Accel Calibration", mandatoryhardware);
                }

                AddBackstageViewPage(new ConfigRadioInput(), "Radio Calibration", mandatoryhardware);

                AddBackstageViewPage(new ArdupilotMega._3DRradio(), "3DR Radio", optionalhardware);
                AddBackstageViewPage(new ConfigBatteryMonitoring(), "Battery Monitor", optionalhardware);
                AddBackstageViewPage(new ConfigHWSonar(), "Sonar", optionalhardware);
                AddBackstageViewPage(new ConfigHWAirspeed(), "Airspeed", optionalhardware);
                AddBackstageViewPage(new ConfigHWOptFlow(), "Optical Flow", optionalhardware);
                AddBackstageViewPage(new ConfigHWOSD(), "OSD", optionalhardware);
                // opt flow
                // osd
                AddBackstageViewPage(new ConfigMount(), "Camera Gimbal", optionalhardware);

                AddBackstageViewPage(new ArdupilotMega.Antenna.Tracker(), "Antenna Tracker", optionalhardware);

                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
                {
                    AddBackstageViewPage(new ConfigAccelerometerCalibrationPlane(), "ArduPlane Level",mandatoryhardware);
                }
            }
            else
            {
                AddBackstageViewPage(new ArdupilotMega.GCSViews.ConfigFirmware(), "Install Firmware");
                AddBackstageViewPage(new ArdupilotMega._3DRradio(), "3DR Radio");
                AddBackstageViewPage(new ArdupilotMega.Antenna.Tracker(), "Antenna Tracker");
            }

            // remeber last page accessed
            foreach (BackstageView.BackstageViewPage page in backstageView.Pages)
            {
                if (page.LinkText == lastpagename)
                {
                    this.backstageView.ActivatePage(page);
                    break;
                }
            }

            if (backstageView.SelectedPage == null)
                backstageView.DrawMenu(null, true);

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
