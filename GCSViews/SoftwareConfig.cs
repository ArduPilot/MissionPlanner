using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.GCSViews.ConfigurationView;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews
{
    public partial class SoftwareConfig : MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string lastpagename = "";
        [Flags]
        public enum pageOptions
        {
            none = 0,
            isConnected = 1,
            isDisConnected = 2,
            isTracker = 4,
            isCopter = 8,
            isCopter35plus = 16,
            isHeli = 32,
            isQuadPlane = 64,
            isPlane = 128,
            isRover = 256,
            gotAllParams = 512
        }

        public class pluginPage
        {
            public Type page;
            public string headerText;
            public pageOptions options;

            public pluginPage(Type page, string headerText, pageOptions options)
            {
                this.page = page;
                this.headerText = headerText;
                this.options = options;
            }
        }


        private static List<pluginPage> pluginViewPages = new List<pluginPage>();

        public static void AddPluginViewPage(Type page, string headerText, pageOptions options = pageOptions.none)
        {
            pluginViewPages.Add(new pluginPage(page, headerText, options));
        }
        public bool isConnected
        {
            get { return MainV2.comPort.BaseStream.IsOpen; }
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


        public bool gotAllParams
        {
            get
            {
                log.InfoFormat("TotalReceived {0} TotalReported {1}", MainV2.comPort.MAV.param.TotalReceived,
                    MainV2.comPort.MAV.param.TotalReported);
                if (MainV2.comPort.MAV.param.TotalReceived < MainV2.comPort.MAV.param.TotalReported)
                {
                    return false;
                }

                return true;
            }
        }
        public SoftwareConfig()
        {
            InitializeComponent();
        }

        public void Activate()
        {
        }

        public BackstageViewPage AddBackstageViewPage(Type userControl, string headerText,
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

                if (gotAllParams)
                {
                    if (MainV2.comPort.BaseStream.IsOpen)
                    {
                        if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
                        {
                            if (MainV2.DisplayConfiguration.displayGeoFence)
                            {
                                AddBackstageViewPage(typeof(ConfigAC_Fence), Strings.GeoFence);
                            }
                        }

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
                            if (MainV2.DisplayConfiguration.displayBasicTuning)
                            {
                                start = AddBackstageViewPage(typeof(ConfigArduplane), Strings.BasicTuning);
                            }

                            if (MainV2.DisplayConfiguration.displayExtendedTuning)
                            {
                                AddBackstageViewPage(typeof(ConfigArducopter), "QP " + Strings.ExtendedTuning);
                            }
                        }

                        if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduRover)
                        {
                            start = AddBackstageViewPage(typeof(ConfigArdurover), Strings.BasicTuning);
                        }

                        if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduTracker)
                        {
                            start = AddBackstageViewPage(typeof(ConfigAntennaTracker), Strings.ExtendedTuning);
                        }

                        if (MainV2.DisplayConfiguration.displayStandardParams)
                        {
                            AddBackstageViewPage(typeof(ConfigFriendlyParams), Strings.StandardParams);
                        }

                        if (MainV2.DisplayConfiguration.displayAdvancedParams)
                        {
                            AddBackstageViewPage(typeof(ConfigFriendlyParamsAdv), Strings.AdvancedParams, null, true);
                        }

                        if (!Program.MONO && ConfigOSD.IsApplicable() && MainV2.DisplayConfiguration.displayOSD)
                        {
                            AddBackstageViewPage(typeof(ConfigOSD), Strings.OnboardOSD);
                        }

                        if (MainV2.DisplayConfiguration.displayMavFTP)
                        {
                            if ((MainV2.comPort.MAV.cs.capabilities & (int)MAVLink.MAV_PROTOCOL_CAPABILITY.FTP) > 0)
                            {
                                AddBackstageViewPage(typeof(MavFTPUI), Strings.MAVFtp);
                            }
                        }

                        if (MainV2.DisplayConfiguration.displayUserParam)
                        {
                            AddBackstageViewPage(typeof(ConfigUserDefined), Strings.User_Params);
                        }
                    }
                }

                if (MainV2.DisplayConfiguration.displayFullParamList)
                {
                    if(!MainV2.comPort.BaseStream.IsOpen || gotAllParams)
                        AddBackstageViewPage(typeof(ConfigRawParams), Strings.FullParameterList, null, false);
                }
                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    if (MainV2.comPort.MAV.cs.firmware == Firmwares.Ateryx)
                    {
                        start = AddBackstageViewPage(typeof(ConfigFlightModes), Strings.FlightModes);
                        AddBackstageViewPage(typeof(ConfigAteryxSensors), "Ateryx Zero Sensors");
                        AddBackstageViewPage(typeof(ConfigAteryx), "Ateryx Pids");
                    }

                    if (!gotAllParams)
                    {
                        if (start == null)
                            start = AddBackstageViewPage(typeof(ConfigParamLoading), Strings.Loading);
                        else
                            AddBackstageViewPage(typeof(ConfigParamLoading), Strings.Loading);
                    }

                    if (MainV2.DisplayConfiguration.displayPlannerSettings)
                    {
                        AddBackstageViewPage(typeof(ConfigPlanner), Strings.Planner);
                    }
                }
                else
                {
                    if (MainV2.DisplayConfiguration.displayPlannerSettings)
                    {
                        start = AddBackstageViewPage(typeof(ConfigPlanner), Strings.Planner);
                    }
                }

                // Add custrom pages set up by plugins
                foreach (var item in pluginViewPages)
                {

                    // go through all options expect disconnected since there is no meaning for sw config in disconnected state
                    if (item.options.HasFlag(pageOptions.isConnected) && !isConnected)
                        continue;
                    if (item.options.HasFlag(pageOptions.isTracker) && !isTracker)
                        continue;
                    if (item.options.HasFlag(pageOptions.isCopter) && !isCopter)
                        continue;
                    if (item.options.HasFlag(pageOptions.isCopter35plus) && !isCopter35plus)
                        continue;
                    if (item.options.HasFlag(pageOptions.isHeli) && !isHeli)
                        continue;
                    if (item.options.HasFlag(pageOptions.isQuadPlane) && !isQuadPlane)
                        continue;
                    if (item.options.HasFlag(pageOptions.isPlane) && !isPlane)
                        continue;
                    if (item.options.HasFlag(pageOptions.isRover) && !isRover)
                        continue;
                    if (item.options.HasFlag(pageOptions.gotAllParams) && !gotAllParams)
                        continue;

                    AddBackstageViewPage(item.page, item.headerText);
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
                    this.BeginInvoke((Action) delegate
                    {
                        try
                        {
                            backstageView.ActivatePage(start);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    });
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