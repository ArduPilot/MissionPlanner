using System;
using System.Reflection;
using System.Windows.Forms;
using log4net;
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
                    AddBackstageViewPage(typeof(ConfigFlightModes), Strings.FlightModes);

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                        AddBackstageViewPage(typeof( ConfigAC_Fence), Strings.GeoFence);

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                    {
                        start = AddBackstageViewPage(typeof( ConfigSimplePids), Strings.BasicTuning);

                        AddBackstageViewPage(typeof( ConfigArducopter), Strings.ExtendedTuning);
                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
                    {
                        start = AddBackstageViewPage(typeof( ConfigArduplane), Strings.BasicTuning);
                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover)
                    {
                        start = AddBackstageViewPage(typeof( ConfigArdurover), Strings.BasicTuning);
                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduTracker)
                    {
                        start = AddBackstageViewPage(typeof( ConfigAntennaTracker), Strings.ExtendedTuning);
                    }

                    AddBackstageViewPage(
                        typeof( ConfigFriendlyParams), Strings.StandardParams);
                    AddBackstageViewPage(
                        typeof( ConfigFriendlyParamsAdv), Strings.AdvancedParams, null, true);
                    AddBackstageViewPage(typeof( ConfigRawParams), Strings.FullParameterList, null, true);

                    AddBackstageViewPage(typeof( ConfigRawParamsTree), Strings.FullParameterTree, null, true);


                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx)
                    {
                        start = AddBackstageViewPage(typeof( ConfigFlightModes), Strings.FlightModes);
                        AddBackstageViewPage(typeof( ConfigAteryxSensors), "Ateryx Zero Sensors");
                        AddBackstageViewPage(typeof( ConfigAteryx), "Ateryx Pids");
                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduTracker)
                    {
                        start = AddBackstageViewPage(typeof( ConfigRawParams), Strings.FullParameterList, null, true);
                    }

                    AddBackstageViewPage(typeof( ConfigPlanner), "Planner");
                }
                else
                {
                    start = AddBackstageViewPage(typeof( ConfigPlanner), "Planner");
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