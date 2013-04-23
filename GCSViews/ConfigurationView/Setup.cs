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
using System.Reflection;
using log4net;

namespace ArdupilotMega.GCSViews.ConfigurationView
{
    public partial class Setup : MyUserControl
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // remember the last page accessed
        static string lastpagename = "";
        public static Controls.FlashMessage flashMessage = new Controls.FlashMessage();

        BackstageView.BackstageViewPage hardware;
        BackstageView.BackstageViewPage standardpage;
        BackstageView.BackstageViewPage advancedpage;

        public Setup()
        {
            InitializeComponent();
            ThemeManager.ApplyThemeTo(this);
            this.Controls.Add(flashMessage);
        }

        private void Setup_Load(object sender, EventArgs e)
        {
            this.SuspendLayout();

            if (MainV2.comPort.BaseStream.IsOpen)
            {
                AddPagesForConnectedState();
			//	backstageView.AddSpacer(20);
            }

            // These pages work when not connected to an APM
            AddBackstageViewPage(new ArdupilotMega._3DRradio(), "3DR Radio", hardware);
            AddBackstageViewPage(new ArdupilotMega.Antenna.Tracker(), "Antenna Tracker");
            backstageView.AddSpacer(10);
            AddBackstageViewPage(new ConfigPlanner(), "Planner");

            // remeber last page accessed
            foreach (BackstageView.BackstageViewPage page in backstageView.Pages) {
                if (page.LinkText == lastpagename)
                {
                    this.backstageView.ActivatePage(page);
                    break;
                }
            }

            //this.backstageView.ActivatePage(backstageView.Pages[0]);

           // AddBackstageViewPage(new ConfigPlannerAdv(), "Planner Adv Config");

            ThemeManager.ApplyThemeTo(this);

            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                Common.MessageShowAgain("Config Connect", @"Please connect (click Connect Button) before using setup.
If you are just setting up 3DR radios, you may continue without connecting.");
            }

            this.ResumeLayout();
        }

        // Add the pages that can only be shown when we are connected to an APM
        private void AddPagesForConnectedState()
        {
            /****************************** Common  **************************/

            if ((MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2) || (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane) || (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover))
            {
                AddBackstageViewPage(new ConfigRadioInput(), "Radio Calibration");
                AddBackstageViewPage(new ConfigFlightModes(), "Flight Modes");
                AddBackstageViewPage(new ConfigFailSafe(), "FailSafe");
                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2 || MainV2.comPort.MAV.param["H_GYR_ENABLE"] != null)
                {
                    AddBackstageViewPage(new ConfigAccelerometerCalibrationQuad(), "ArduCopter Level");
                }
                hardware = AddBackstageViewPage(new ConfigHardwareOptions(), "Hardware Options");
                AddBackstageViewPage(new ConfigBatteryMonitoring(), "Battery Monitor", hardware);
            }

            if ((MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2) || (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane) || (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover) || (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx))
            {
                standardpage = AddBackstageViewPage(new ConfigFriendlyParams { ParameterMode = ParameterMetaDataConstants.Standard }, "Standard Params");
                advancedpage = AddBackstageViewPage(new ConfigFriendlyParams { ParameterMode = ParameterMetaDataConstants.Advanced }, "Advanced Params");
            }
            AddBackstageViewPage(new ConfigRawParams(), "Adv Parameter List", advancedpage);

            /******************************HELI **************************/
            if (MainV2.comPort.MAV.param["H_GYR_ENABLE"] != null) // heli
            {
              //  AddBackstageViewPage(new ConfigSignalization(), "Signalization", hardware);

                AddBackstageViewPage(new ConfigMount(), "Camera Gimbal", hardware);

               

                AddBackstageViewPage(new ConfigTradHeli(), "Heli Setup");

                var configpanel = new Controls.ConfigPanel(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "ArduCopterConfig.xml");
                AddBackstageViewPage(configpanel, "ArduCopter Pids", standardpage);

                AddBackstageViewPage(new ConfigArducopter(), "ArduCopter Config", standardpage);
               // AddBackstageViewPage(new ConfigAP_Limits(), "GeoFence");
            }
                /****************************** ArduCopter **************************/
            else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
            {
              //  AddBackstageViewPage(new ConfigSignalization(), "Signalization", hardware);

                AddBackstageViewPage(new ConfigMount(), "Camera Gimbal", hardware);

                var configpanel = new Controls.ConfigPanel(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "ArduCopterConfig.xml");
                AddBackstageViewPage(configpanel, "ArduCopter Pids", standardpage);

                AddBackstageViewPage(new ConfigArducopter(), "ArduCopter Config", standardpage);
                //AddBackstageViewPage(new ConfigAP_Limits(), "GeoFence");
            }
                /****************************** ArduPlane **************************/
            else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
            {
                AddBackstageViewPage(new ConfigMount(), "Camera Gimbal", hardware);

                AddBackstageViewPage(new ConfigAccelerometerCalibrationPlane(), "ArduPlane Level");
                AddBackstageViewPage(new ConfigArduplane(), "ArduPlane Pids", standardpage);
            }
                /****************************** ArduRover **************************/
            else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover)
            {
                AddBackstageViewPage(new ConfigArdurover(), "ArduRover Pids", standardpage);
            }
            else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx)
            {
                AddBackstageViewPage(new ConfigFlightModes(), "Flight Modes");
                AddBackstageViewPage(new ConfigAteryxSensors(), "Ateryx Zero Sensors");
                AddBackstageViewPage(new ConfigAteryx(), "Ateryx Pids", standardpage);
            }
        }

        private BackstageView.BackstageViewPage AddBackstageViewPage(UserControl userControl, string headerText, BackstageView.BackstageViewPage Parent = null)
        {
            try
            {
                return backstageView.AddPage(userControl, headerText, Parent);
            }
            catch (Exception ex) { log.Error(ex); return null; }
        }

        private void Setup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backstageView.SelectedPage != null)
                lastpagename = backstageView.SelectedPage.LinkText;

            backstageView.Close();
        }
    }
}