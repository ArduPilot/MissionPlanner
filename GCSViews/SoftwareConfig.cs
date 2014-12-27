using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.GCSViews.ConfigurationView;
using MissionPlanner.Utilities;
using log4net;
using System.Reflection;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews
{
    public partial class SoftwareConfig : MyUserControl, IActivate
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static string lastpagename = "";

        public SoftwareConfig()
        {
            InitializeComponent();
  }

        public void Activate()
        {
        }

        private BackstageViewPage AddBackstageViewPage(UserControl userControl, string headerText, BackstageViewPage Parent = null, bool advanced = false)
        {
            try
            {
                return backstageView.AddPage(userControl, headerText, Parent, advanced);
            }
            catch (Exception ex) { log.Error(ex); return null; }
        }

        private void SoftwareConfig_Load(object sender, EventArgs e)
        {
            try
            {
                BackstageViewPage start = null;

                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    AddBackstageViewPage(new ConfigFlightModes(), Strings.FlightModes);

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                        AddBackstageViewPage(new ConfigAC_Fence(), Strings.GeoFence);

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                    {
                        start = AddBackstageViewPage(new ConfigSimplePids(), Strings.BasicTuning);

                        AddBackstageViewPage(new ConfigArducopter(), Strings.ExtendedTuning);
                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
                    {
                        start = AddBackstageViewPage(new ConfigArduplane(), Strings.BasicTuning);

                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover)
                    {
                        start = AddBackstageViewPage(new ConfigArdurover(), Strings.BasicTuning);
                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduTracker)
                    {
                        start = AddBackstageViewPage(new ConfigAntennaTracker(), Strings.ExtendedTuning);
                    }

                    AddBackstageViewPage(new ConfigFriendlyParams { ParameterMode = ParameterMetaDataConstants.Standard }, Strings.StandardParams);
                    AddBackstageViewPage(new ConfigFriendlyParams { ParameterMode = ParameterMetaDataConstants.Advanced }, Strings.AdvancedParams, null, true);
                    AddBackstageViewPage(new ConfigRawParams(), Strings.FullParameterList, null, true);

                    AddBackstageViewPage(new ConfigRawParamsTree(), Strings.FullParameterTree, null, true);


                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx)
                    {
                        start = AddBackstageViewPage(new ConfigFlightModes(), Strings.FlightModes);
                        AddBackstageViewPage(new ConfigAteryxSensors(), "Ateryx Zero Sensors");
                        AddBackstageViewPage(new ConfigAteryx(), "Ateryx Pids");
                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduTracker)
                    {
                        start = AddBackstageViewPage(new ConfigRawParams(), Strings.FullParameterList, null, true);
                    }

                    AddBackstageViewPage(new ConfigPlanner(), "Planner");
                }
                else
                {
                    start = AddBackstageViewPage(new ConfigPlanner(), "Planner");
                }

                // remeber last page accessed
                foreach (BackstageViewPage page in backstageView.Pages)
                {
                    if (page.LinkText == lastpagename)
                    {
                        this.backstageView.ActivatePage(page);
                        break;
                    }
                }


                if (this.backstageView.SelectedPage == null && start != null)
                    backstageView.ActivatePage(start);

                ThemeManager.ApplyThemeTo(this);
            }
            catch (Exception ex) { log.Error(ex); }
        }

        private void SoftwareConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backstageView.SelectedPage != null)
                lastpagename = backstageView.SelectedPage.LinkText;

            backstageView.Close();
        }
    }
}
