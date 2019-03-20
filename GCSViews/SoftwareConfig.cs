using System;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.GCSViews.ConfigurationView;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews
{
    public partial class SoftwareConfig : MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string lastpagename = "";

        public SoftwareConfig()
        {
            InitializeComponent();
        }

        public void Activate()
        {
        }

        private BackstageViewPage AddBackstageViewPage(Type userControl, string headerText,
            BackstageViewPage Parent = null, bool advanced = false)
        {
            try
            {
                return backstageView.AddPage(userControl, headerText, Parent, advanced);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        private void SoftwareConfig_Load(object sender, EventArgs e)
        {
            try
            {
                BackstageViewPage start = null;

                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    if (MainV2.DisplayConfiguration.displayFlightModes)
                    {
                        start = AddBackstageViewPage(typeof(ConfigFlightModes), Strings.FlightModes);
                    }

                    if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
                        AddBackstageViewPage(typeof(ConfigAC_Fence), Strings.GeoFence);

                    if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
                    {
                        if (MainV2.DisplayConfiguration.displayBasicTuning)
                        { 
                            start = AddBackstageViewPage(typeof(ConfigSimplePids), Strings.BasicTuning);
                        }
                        if (MainV2.DisplayConfiguration.displayExtendedTuning)
                        {
                            AddBackstageViewPage(typeof(ConfigArducopter), Strings.ExtendedTuning);
                        }
                    }

                    if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane)
                    {
                        start = AddBackstageViewPage(typeof(ConfigArduplane), Strings.BasicTuning);
                    }

                    if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduRover)
                    {
                        start = AddBackstageViewPage(typeof(ConfigArdurover), Strings.BasicTuning);
                    }

                    if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduTracker)
                    {
                        start = AddBackstageViewPage(typeof(ConfigAntennaTracker), Strings.ExtendedTuning);
                    }

                    if (MainV2.DisplayConfiguration.displayBasicTuning)
                    {
                        AddBackstageViewPage(typeof(ConfigFriendlyParams), Strings.StandardParams);
                    }

                    if (MainV2.DisplayConfiguration.displayAdvancedParams)
                    {
                        AddBackstageViewPage(typeof(ConfigFriendlyParamsAdv), Strings.AdvancedParams, null, true);
                    }

                    if (!Program.MONO && ConfigOSD.IsApplicable())
                    {
                        AddBackstageViewPage(typeof(ConfigOSD), Strings.OnboardOSD);
                    }
                }

                if (MainV2.DisplayConfiguration.displayFullParamList)
                {
                    AddBackstageViewPage(typeof(ConfigRawParams), Strings.FullParameterList, null, true);
                }
                if (MainV2.DisplayConfiguration.displayFullParamTree && !Program.MONO)
                {
                    AddBackstageViewPage(typeof(ConfigRawParamsTree), Strings.FullParameterTree, null, true);
                }

                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    if (MainV2.comPort.MAV.cs.firmware == Firmwares.Ateryx)
                    {
                        start = AddBackstageViewPage(typeof(ConfigFlightModes), Strings.FlightModes);
                        AddBackstageViewPage(typeof(ConfigAteryxSensors), "Ateryx Zero Sensors");
                        AddBackstageViewPage(typeof(ConfigAteryx), "Ateryx Pids");
                    }

                    AddBackstageViewPage(typeof(ConfigPlanner), "Planner");
                }
                else
                {
                    start = AddBackstageViewPage(typeof(ConfigPlanner), "Planner");
                }

                // apply theme before trying to display it
                ThemeManager.ApplyThemeTo(this);

                // remeber last page accessed
                foreach (BackstageViewPage page in backstageView.Pages)
                {
                    if (page.LinkText == lastpagename)
                    {
                        backstageView.ActivatePage(page);
                        break;
                    }
                }


                if (backstageView.SelectedPage == null && start != null)
                    backstageView.ActivatePage(start);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void SoftwareConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backstageView.SelectedPage != null)
                lastpagename = backstageView.SelectedPage.LinkText;

            backstageView.Close();
        }
    }
}