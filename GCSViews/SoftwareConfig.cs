﻿using System;
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

        private BackstageView.BackstageViewPage AddBackstageViewPage(UserControl userControl, string headerText, BackstageView.BackstageViewPage Parent = null)
        {
            try
            {
                return backstageView.AddPage(userControl, headerText, Parent);
            }
            catch (Exception ex) { log.Error(ex); return null; }
        }

        private void SoftwareConfig_Load(object sender, EventArgs e)
        {
            try
            {
                BackstageView.BackstageViewPage start = null;

                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    AddBackstageViewPage(new ConfigFlightModes(), "Flight Modes");

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                        AddBackstageViewPage(new ConfigAC_Fence(), "GeoFence");

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                    {
                        start = AddBackstageViewPage(new ConfigSimplePids(), "Basic Pids");

                        //     AddBackstageViewPage(new ConfigSimplePidsV2(), "Basic Pids V2");
                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
                    {
                        start = AddBackstageViewPage(new ConfigArduplane(), "APM:Plane Pids");

                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover)
                    {
                        start = AddBackstageViewPage(new ConfigArdurover(), "APM:Rover Pids");

                    }

                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                    {
                        // var configpanel = new Controls.ConfigPanel(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "ArduCopterConfig.xml");
                        // AddBackstageViewPage(configpanel, "ArduCopter Pids");

                        AddBackstageViewPage(new ConfigArducopter(), "APM:Copter Pids");
                    }

                    AddBackstageViewPage(new ConfigFriendlyParams { ParameterMode = ParameterMetaDataConstants.Standard }, "Standard Params");
                    AddBackstageViewPage(new ConfigFriendlyParams { ParameterMode = ParameterMetaDataConstants.Advanced }, "Advanced Params");
                    AddBackstageViewPage(new ConfigRawParams(), "Full Parameter List");



                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx)
                    {
                        start = AddBackstageViewPage(new ConfigFlightModes(), "Flight Modes");
                        AddBackstageViewPage(new ConfigAteryxSensors(), "Ateryx Zero Sensors");
                        AddBackstageViewPage(new ConfigAteryx(), "Ateryx Pids");
                    }

                    AddBackstageViewPage(new ConfigPlanner(), "Planner");
                }
                else
                {
                    start = AddBackstageViewPage(new ConfigPlanner(), "Planner");
                }

                AddBackstageViewPage(new ConfigHelp(), "Help");

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
