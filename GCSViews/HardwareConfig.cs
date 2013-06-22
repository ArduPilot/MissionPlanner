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

namespace ArdupilotMega.GCSViews
{
    public partial class HardwareConfig : MyUserControl
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static string lastpagename = "";

        public HardwareConfig()
        {
            InitializeComponent();
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
                start  = AddBackstageViewPage(new ArdupilotMega.GCSViews.Firmware(), "Install Firmware");

                BackstageView.BackstageViewPage mandatoryhardware = AddBackstageViewPage(new ConfigMandatory(), "Mandatory Hardware", null);
                BackstageView.BackstageViewPage optionalhardware = AddBackstageViewPage(new ConfigOptional(), "Optional Hardware", null);

                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                {
                    AddBackstageViewPage(new ConfigFrameType(), "Frame Type", mandatoryhardware);
                }

                AddBackstageViewPage(new ConfigHWCompass(), "Compass",mandatoryhardware);

                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                {
                    AddBackstageViewPage(new ConfigAccelerometerCalibrationQuad(), "Accel Calibration", mandatoryhardware);
                }

                AddBackstageViewPage(new ConfigRadioInput(), "Radio Calibration", mandatoryhardware);
                AddBackstageViewPage(new ConfigFlightModes(), "Flight Modes", mandatoryhardware);

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

                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2 && MainV2.comPort.MAV.param["H_GYR_ENABLE"] != null)
                {
                    AddBackstageViewPage(new ConfigTradHeli(), "Heli Setup", optionalhardware);
                }

                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
                {
                    AddBackstageViewPage(new ConfigAccelerometerCalibrationPlane(), "ArduPlane Level");
                }
            }
            else
            {
                start  = AddBackstageViewPage(new ArdupilotMega.GCSViews.Firmware(), "Install Firmware");
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

            if (this.backstageView.SelectedPage == null)
                backstageView.ActivatePage(start);

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
