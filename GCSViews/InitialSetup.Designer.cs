﻿namespace MissionPlanner.GCSViews
{
    partial class InitialSetup
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitialSetup));
            this.backstageView = new MissionPlanner.Controls.BackstageView.BackstageView();
            this.backstageViewPagefw = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.initialSetupBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.configFirmware1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigFirmware();
            this.backstageViewPagefwdisabled = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configFirmwareDisabled1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigFirmwareDisabled();
            this.backstageViewPagewizard = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configWizard1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigWizard();
            this.backstageViewPagemand = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configMandatory1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigMandatory();
            this.backstageViewPagetradheli = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configTradHeli1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigTradHeli();
            this.backstageViewPageframetype = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configFrameType1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigFrameType();
            this.backstageViewPagecompass = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configHWCompass1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigHWCompass();
            this.backstageViewPageaccelquad = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configAccelerometerCalibrationQuad1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigAccelerometerCalibrationQuad();
            this.backstageViewPageaccelplane = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configAccelerometerCalibrationPlane1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigAccelerometerCalibrationPlane();
            this.backstageViewPageacceltracker = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configAccelerometerCalibrationTracker1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigAccelerometerCalibrationTracker();
            this.backstageViewPageradio = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configRadioInput1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigRadioInput();
            this.backstageViewPageflmode = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configFlightModes1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigFlightModes();
            this.backstageViewPagefs = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configFailSafe1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigFailSafe();
            this.backstageViewPageopt = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configOptional1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigOptional();
            this.backstageViewPage3drradio = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this._3DRradio1 = new MissionPlanner._3DRradio();
            this.backstageViewPagebatmon = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configBatteryMonitoring1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigBatteryMonitoring();
            this.backstageViewPagecompassmot = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configCompassMot1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigCompassMot();
            this.backstageViewPagesonar = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configHWSonar1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigHWSonar();
            this.backstageViewPageairspeed = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configHWAirspeed1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigHWAirspeed();
            this.backstageViewPageoptflow = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configHWOptFlow1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigHWOptFlow();
            this.backstageViewPageosd = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configHWOSD1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigHWOSD();
            this.backstageViewPagegimbal = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configMount1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigMount();
            this.backstageViewPageAntTrack = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.tracker1 = new MissionPlanner.Antenna.Tracker();
            this.backstageViewPageMotorTest = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            this.configMotor1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigMotorTest();
            this.backstageViewPageinstfw = new MissionPlanner.Controls.BackstageView.BackstageViewPage();
            ((System.ComponentModel.ISupportInitialize)(this.initialSetupBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // backstageView
            // 
            resources.ApplyResources(this.backstageView, "backstageView");
            this.backstageView.HighlightColor1 = System.Drawing.SystemColors.Highlight;
            this.backstageView.HighlightColor2 = System.Drawing.SystemColors.MenuHighlight;
            this.backstageView.Name = "backstageView";
            this.backstageView.Pages.Add(this.backstageViewPagefw);
            this.backstageView.Pages.Add(this.backstageViewPagefwdisabled);
            this.backstageView.Pages.Add(this.backstageViewPagewizard);
            this.backstageView.Pages.Add(this.backstageViewPagemand);
            this.backstageView.Pages.Add(this.backstageViewPagetradheli);
            this.backstageView.Pages.Add(this.backstageViewPageframetype);
            this.backstageView.Pages.Add(this.backstageViewPageaccelquad);
            this.backstageView.Pages.Add(this.backstageViewPageaccelplane);
            this.backstageView.Pages.Add(this.backstageViewPageacceltracker);
            this.backstageView.Pages.Add(this.backstageViewPagecompass);
            this.backstageView.Pages.Add(this.backstageViewPageradio);
            this.backstageView.Pages.Add(this.backstageViewPageflmode);
            this.backstageView.Pages.Add(this.backstageViewPagefs);
            this.backstageView.Pages.Add(this.backstageViewPageopt);
            this.backstageView.Pages.Add(this.backstageViewPage3drradio);
            this.backstageView.Pages.Add(this.backstageViewPagebatmon);
            this.backstageView.Pages.Add(this.backstageViewPagecompassmot);
            this.backstageView.Pages.Add(this.backstageViewPagesonar);
            this.backstageView.Pages.Add(this.backstageViewPageairspeed);
            this.backstageView.Pages.Add(this.backstageViewPageoptflow);
            this.backstageView.Pages.Add(this.backstageViewPageosd);
            this.backstageView.Pages.Add(this.backstageViewPagegimbal);
            this.backstageView.Pages.Add(this.backstageViewPageAntTrack);
            this.backstageView.Pages.Add(this.backstageViewPageMotorTest);
            this.backstageView.WidthMenu = 172;
            // 
            // backstageViewPagefw
            // 
            this.backstageViewPagefw.Advanced = false;
            this.backstageViewPagefw.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isDisConnected", true));
            this.backstageViewPagefw.LinkText = "Install Firmware";
            this.backstageViewPagefw.Page = this.configFirmware1;
            this.backstageViewPagefw.Parent = null;
            this.backstageViewPagefw.Show = true;
            this.backstageViewPagefw.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagefw, "backstageViewPagefw");
            // 
            // initialSetupBindingSource
            // 
            this.initialSetupBindingSource.DataSource = typeof(MissionPlanner.GCSViews.InitialSetup);
            // 
            // configFirmware1
            // 
            resources.ApplyResources(this.configFirmware1, "configFirmware1");
            this.configFirmware1.Name = "configFirmware1";
            // 
            // backstageViewPagefwdisabled
            // 
            this.backstageViewPagefwdisabled.Advanced = false;
            this.backstageViewPagefwdisabled.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isConnected", true));
            this.backstageViewPagefwdisabled.LinkText = "Install Firmware";
            this.backstageViewPagefwdisabled.Page = this.configFirmwareDisabled1;
            this.backstageViewPagefwdisabled.Parent = null;
            this.backstageViewPagefwdisabled.Show = true;
            this.backstageViewPagefwdisabled.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagefwdisabled, "backstageViewPagefwdisabled");
            // 
            // configFirmwareDisabled1
            // 
            resources.ApplyResources(this.configFirmwareDisabled1, "configFirmwareDisabled1");
            this.configFirmwareDisabled1.Name = "configFirmwareDisabled1";
            // 
            // backstageViewPagewizard
            // 
            this.backstageViewPagewizard.Advanced = false;
            this.backstageViewPagewizard.LinkText = "Wizard";
            this.backstageViewPagewizard.Page = this.configWizard1;
            this.backstageViewPagewizard.Parent = null;
            this.backstageViewPagewizard.Show = true;
            this.backstageViewPagewizard.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagewizard, "backstageViewPagewizard");
            // 
            // configWizard1
            // 
            resources.ApplyResources(this.configWizard1, "configWizard1");
            this.configWizard1.Name = "configWizard1";
            // 
            // backstageViewPagemand
            // 
            this.backstageViewPagemand.Advanced = false;
            this.backstageViewPagemand.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isConnected", true));
            this.backstageViewPagemand.LinkText = "Mandatory Hardware";
            this.backstageViewPagemand.Page = this.configMandatory1;
            this.backstageViewPagemand.Parent = null;
            this.backstageViewPagemand.Show = true;
            this.backstageViewPagemand.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagemand, "backstageViewPagemand");
            // 
            // configMandatory1
            // 
            resources.ApplyResources(this.configMandatory1, "configMandatory1");
            this.configMandatory1.Name = "configMandatory1";
            // 
            // backstageViewPagetradheli
            // 
            this.backstageViewPagetradheli.Advanced = false;
            this.backstageViewPagetradheli.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isHeli", true));
            this.backstageViewPagetradheli.LinkText = "Heli Setup";
            this.backstageViewPagetradheli.Page = this.configTradHeli1;
            this.backstageViewPagetradheli.Parent = this.backstageViewPagemand;
            this.backstageViewPagetradheli.Show = true;
            this.backstageViewPagetradheli.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagetradheli, "backstageViewPagetradheli");
            // 
            // configTradHeli1
            // 
            resources.ApplyResources(this.configTradHeli1, "configTradHeli1");
            this.configTradHeli1.Name = "configTradHeli1";
            // 
            // backstageViewPageframetype
            // 
            this.backstageViewPageframetype.Advanced = false;
            this.backstageViewPageframetype.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isCopter", true));
            this.backstageViewPageframetype.LinkText = "Frame Type";
            this.backstageViewPageframetype.Page = this.configFrameType1;
            this.backstageViewPageframetype.Parent = this.backstageViewPagemand;
            this.backstageViewPageframetype.Show = true;
            this.backstageViewPageframetype.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageframetype, "backstageViewPageframetype");
            // 
            // configFrameType1
            // 
            resources.ApplyResources(this.configFrameType1, "configFrameType1");
            this.configFrameType1.Name = "configFrameType1";
            // 
            // backstageViewPagecompass
            // 
            this.backstageViewPagecompass.Advanced = false;
            this.backstageViewPagecompass.LinkText = "Compass";
            this.backstageViewPagecompass.Page = this.configHWCompass1;
            this.backstageViewPagecompass.Parent = this.backstageViewPagemand;
            this.backstageViewPagecompass.Show = true;
            this.backstageViewPagecompass.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagecompass, "backstageViewPagecompass");
            // 
            // configHWCompass1
            // 
            resources.ApplyResources(this.configHWCompass1, "configHWCompass1");
            this.configHWCompass1.Name = "configHWCompass1";
            // 
            // backstageViewPageaccelquad
            // 
            this.backstageViewPageaccelquad.Advanced = false;
            this.backstageViewPageaccelquad.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isCopter", true));
            this.backstageViewPageaccelquad.LinkText = "Accel Calibration";
            this.backstageViewPageaccelquad.Page = this.configAccelerometerCalibrationQuad1;
            this.backstageViewPageaccelquad.Parent = this.backstageViewPagemand;
            this.backstageViewPageaccelquad.Show = true;
            this.backstageViewPageaccelquad.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageaccelquad, "backstageViewPageaccelquad");
            // 
            // configAccelerometerCalibrationQuad1
            // 
            resources.ApplyResources(this.configAccelerometerCalibrationQuad1, "configAccelerometerCalibrationQuad1");
            this.configAccelerometerCalibrationQuad1.Name = "configAccelerometerCalibrationQuad1";
            // 
            // backstageViewPageaccelplane
            // 
            this.backstageViewPageaccelplane.Advanced = false;
            this.backstageViewPageaccelplane.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isPlane", true));
            this.backstageViewPageaccelplane.LinkText = "Accel Calibration";
            this.backstageViewPageaccelplane.Page = this.configAccelerometerCalibrationPlane1;
            this.backstageViewPageaccelplane.Parent = this.backstageViewPagemand;
            this.backstageViewPageaccelplane.Show = true;
            this.backstageViewPageaccelplane.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageaccelplane, "backstageViewPageaccelplane");
            // 
            // configAccelerometerCalibrationPlane1
            // 
            resources.ApplyResources(this.configAccelerometerCalibrationPlane1, "configAccelerometerCalibrationPlane1");
            this.configAccelerometerCalibrationPlane1.Name = "configAccelerometerCalibrationPlane1";
            // 
            // backstageViewPageacceltracker
            // 
            this.backstageViewPageacceltracker.Advanced = false;
            this.backstageViewPageacceltracker.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isTracker", true));
            this.backstageViewPageacceltracker.LinkText = "Accel Calibration";
            this.backstageViewPageacceltracker.Page = this.configAccelerometerCalibrationTracker1;
            this.backstageViewPageacceltracker.Parent = this.backstageViewPagemand;
            this.backstageViewPageacceltracker.Show = true;
            this.backstageViewPageacceltracker.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageacceltracker, "backstageViewPageacceltracker");
            // 
            // configAccelerometerCalibrationTracker1
            // 
            resources.ApplyResources(this.configAccelerometerCalibrationTracker1, "configAccelerometerCalibrationTracker1");
            this.configAccelerometerCalibrationTracker1.Name = "configAccelerometerCalibrationTracker1";
            // 
            // backstageViewPageradio
            // 
            this.backstageViewPageradio.Advanced = false;
            this.backstageViewPageradio.LinkText = "Radio Calibration";
            this.backstageViewPageradio.Page = this.configRadioInput1;
            this.backstageViewPageradio.Parent = this.backstageViewPagemand;
            this.backstageViewPageradio.Show = true;
            this.backstageViewPageradio.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageradio, "backstageViewPageradio");
            // 
            // configRadioInput1
            // 
            resources.ApplyResources(this.configRadioInput1, "configRadioInput1");
            this.configRadioInput1.Name = "configRadioInput1";
            // 
            // backstageViewPageflmode
            // 
            this.backstageViewPageflmode.Advanced = false;
            this.backstageViewPageflmode.LinkText = "Flight Modes";
            this.backstageViewPageflmode.Page = this.configFlightModes1;
            this.backstageViewPageflmode.Parent = this.backstageViewPagemand;
            this.backstageViewPageflmode.Show = true;
            this.backstageViewPageflmode.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageflmode, "backstageViewPageflmode");
            // 
            // configFlightModes1
            // 
            resources.ApplyResources(this.configFlightModes1, "configFlightModes1");
            this.configFlightModes1.Name = "configFlightModes1";
            // 
            // backstageViewPagefs
            // 
            this.backstageViewPagefs.Advanced = false;
            this.backstageViewPagefs.LinkText = "FailSafe";
            this.backstageViewPagefs.Page = this.configFailSafe1;
            this.backstageViewPagefs.Parent = this.backstageViewPagemand;
            this.backstageViewPagefs.Show = true;
            this.backstageViewPagefs.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagefs, "backstageViewPagefs");
            // 
            // configFailSafe1
            // 
            resources.ApplyResources(this.configFailSafe1, "configFailSafe1");
            this.configFailSafe1.Name = "configFailSafe1";
            // 
            // backstageViewPageopt
            // 
            this.backstageViewPageopt.Advanced = false;
            this.backstageViewPageopt.LinkText = "Optional Hardware";
            this.backstageViewPageopt.Page = this.configOptional1;
            this.backstageViewPageopt.Parent = null;
            this.backstageViewPageopt.Show = true;
            this.backstageViewPageopt.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageopt, "backstageViewPageopt");
            // 
            // configOptional1
            // 
            resources.ApplyResources(this.configOptional1, "configOptional1");
            this.configOptional1.Name = "configOptional1";
            // 
            // backstageViewPage3drradio
            // 
            this.backstageViewPage3drradio.Advanced = false;
            this.backstageViewPage3drradio.LinkText = "3DR Radio";
            this.backstageViewPage3drradio.Page = this._3DRradio1;
            this.backstageViewPage3drradio.Parent = this.backstageViewPageopt;
            this.backstageViewPage3drradio.Show = true;
            this.backstageViewPage3drradio.Spacing = 30;
            resources.ApplyResources(this.backstageViewPage3drradio, "backstageViewPage3drradio");
            // 
            // _3DRradio1
            // 
            resources.ApplyResources(this._3DRradio1, "_3DRradio1");
            this._3DRradio1.Name = "_3DRradio1";
            // 
            // backstageViewPagebatmon
            // 
            this.backstageViewPagebatmon.Advanced = false;
            this.backstageViewPagebatmon.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isConnected", true));
            this.backstageViewPagebatmon.LinkText = "Battery Monitor";
            this.backstageViewPagebatmon.Page = this.configBatteryMonitoring1;
            this.backstageViewPagebatmon.Parent = this.backstageViewPageopt;
            this.backstageViewPagebatmon.Show = true;
            this.backstageViewPagebatmon.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagebatmon, "backstageViewPagebatmon");
            // 
            // configBatteryMonitoring1
            // 
            resources.ApplyResources(this.configBatteryMonitoring1, "configBatteryMonitoring1");
            this.configBatteryMonitoring1.Name = "configBatteryMonitoring1";
            // 
            // backstageViewPagecompassmot
            // 
            this.backstageViewPagecompassmot.Advanced = false;
            this.backstageViewPagecompassmot.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isCopter", true));
            this.backstageViewPagecompassmot.LinkText = "Compass/Motor Calib";
            this.backstageViewPagecompassmot.Page = this.configCompassMot1;
            this.backstageViewPagecompassmot.Parent = this.backstageViewPageopt;
            this.backstageViewPagecompassmot.Show = true;
            this.backstageViewPagecompassmot.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagecompassmot, "backstageViewPagecompassmot");
            // 
            // configCompassMot1
            // 
            resources.ApplyResources(this.configCompassMot1, "configCompassMot1");
            this.configCompassMot1.Name = "configCompassMot1";
            // 
            // backstageViewPagesonar
            // 
            this.backstageViewPagesonar.Advanced = false;
            this.backstageViewPagesonar.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isConnected", true));
            this.backstageViewPagesonar.LinkText = "Sonar";
            this.backstageViewPagesonar.Page = this.configHWSonar1;
            this.backstageViewPagesonar.Parent = this.backstageViewPageopt;
            this.backstageViewPagesonar.Show = true;
            this.backstageViewPagesonar.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagesonar, "backstageViewPagesonar");
            // 
            // configHWSonar1
            // 
            resources.ApplyResources(this.configHWSonar1, "configHWSonar1");
            this.configHWSonar1.Name = "configHWSonar1";
            // 
            // backstageViewPageairspeed
            // 
            this.backstageViewPageairspeed.Advanced = false;
            this.backstageViewPageairspeed.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isConnected", true));
            this.backstageViewPageairspeed.LinkText = "Airspeed";
            this.backstageViewPageairspeed.Page = this.configHWAirspeed1;
            this.backstageViewPageairspeed.Parent = this.backstageViewPageopt;
            this.backstageViewPageairspeed.Show = true;
            this.backstageViewPageairspeed.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageairspeed, "backstageViewPageairspeed");
            // 
            // configHWAirspeed1
            // 
            resources.ApplyResources(this.configHWAirspeed1, "configHWAirspeed1");
            this.configHWAirspeed1.Name = "configHWAirspeed1";
            // 
            // backstageViewPageoptflow
            // 
            this.backstageViewPageoptflow.Advanced = false;
            this.backstageViewPageoptflow.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isConnected", true));
            this.backstageViewPageoptflow.LinkText = "Optical Flow";
            this.backstageViewPageoptflow.Page = this.configHWOptFlow1;
            this.backstageViewPageoptflow.Parent = this.backstageViewPageopt;
            this.backstageViewPageoptflow.Show = true;
            this.backstageViewPageoptflow.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageoptflow, "backstageViewPageoptflow");
            // 
            // configHWOptFlow1
            // 
            resources.ApplyResources(this.configHWOptFlow1, "configHWOptFlow1");
            this.configHWOptFlow1.Name = "configHWOptFlow1";
            // 
            // backstageViewPageosd
            // 
            this.backstageViewPageosd.Advanced = false;
            this.backstageViewPageosd.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isConnected", true));
            this.backstageViewPageosd.LinkText = "OSD";
            this.backstageViewPageosd.Page = this.configHWOSD1;
            this.backstageViewPageosd.Parent = this.backstageViewPageopt;
            this.backstageViewPageosd.Show = true;
            this.backstageViewPageosd.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageosd, "backstageViewPageosd");
            // 
            // configHWOSD1
            // 
            resources.ApplyResources(this.configHWOSD1, "configHWOSD1");
            this.configHWOSD1.Name = "configHWOSD1";
            // 
            // backstageViewPagegimbal
            // 
            this.backstageViewPagegimbal.Advanced = false;
            this.backstageViewPagegimbal.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isConnected", true));
            this.backstageViewPagegimbal.LinkText = "Camera Gimbal";
            this.backstageViewPagegimbal.Page = this.configMount1;
            this.backstageViewPagegimbal.Parent = this.backstageViewPageopt;
            this.backstageViewPagegimbal.Show = true;
            this.backstageViewPagegimbal.Spacing = 30;
            resources.ApplyResources(this.backstageViewPagegimbal, "backstageViewPagegimbal");
            // 
            // configMount1
            // 
            resources.ApplyResources(this.configMount1, "configMount1");
            this.configMount1.Name = "configMount1";
            // 
            // backstageViewPageAntTrack
            // 
            this.backstageViewPageAntTrack.Advanced = false;
            this.backstageViewPageAntTrack.LinkText = "Antenna tracker";
            this.backstageViewPageAntTrack.Page = this.tracker1;
            this.backstageViewPageAntTrack.Parent = this.backstageViewPageopt;
            this.backstageViewPageAntTrack.Show = true;
            this.backstageViewPageAntTrack.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageAntTrack, "backstageViewPageAntTrack");
            // 
            // tracker1
            // 
            this.tracker1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(39)))), ((int)(((byte)(40)))));
            resources.ApplyResources(this.tracker1, "tracker1");
            this.tracker1.ForeColor = System.Drawing.Color.White;
            this.tracker1.Name = "tracker1";
            // 
            // backstageViewPageMotorTest
            // 
            this.backstageViewPageMotorTest.Advanced = false;
            this.backstageViewPageMotorTest.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isCopter", true));
            this.backstageViewPageMotorTest.LinkText = "Motor Test";
            this.backstageViewPageMotorTest.Page = this.configMotor1;
            this.backstageViewPageMotorTest.Parent = this.backstageViewPageopt;
            this.backstageViewPageMotorTest.Show = true;
            this.backstageViewPageMotorTest.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageMotorTest, "backstageViewPageMotorTest");
            // 
            // configMotor1
            // 
            resources.ApplyResources(this.configMotor1, "configMotor1");
            this.configMotor1.Name = "configMotor1";
            // 
            // backstageViewPageinstfw
            // 
            this.backstageViewPageinstfw.Advanced = false;
            this.backstageViewPageinstfw.DataBindings.Add(new System.Windows.Forms.Binding("Show", this.initialSetupBindingSource, "isDisConnected", true));
            this.backstageViewPageinstfw.LinkText = "Install Firmware";
            this.backstageViewPageinstfw.Page = this.configFirmware1;
            this.backstageViewPageinstfw.Parent = null;
            this.backstageViewPageinstfw.Show = false;
            this.backstageViewPageinstfw.Spacing = 30;
            resources.ApplyResources(this.backstageViewPageinstfw, "backstageViewPageinstfw");
            // 
            // InitialSetup
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.backstageView);
            this.Controls.Add(this.configAccelerometerCalibrationTracker1);
            this.MinimumSize = new System.Drawing.Size(1000, 450);
            this.Name = "InitialSetup";
            resources.ApplyResources(this, "$this");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HardwareConfig_FormClosing);
            this.Load += new System.EventHandler(this.HardwareConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.initialSetupBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.BackstageView.BackstageView backstageView;
        private ConfigurationView.ConfigFirmware configFirmware1;
        private ConfigurationView.ConfigFirmwareDisabled configFirmwareDisabled1;
        private ConfigurationView.ConfigWizard configWizard1;
        private ConfigurationView.ConfigMandatory configMandatory1;
        private ConfigurationView.ConfigOptional configOptional1;
        private ConfigurationView.ConfigTradHeli configTradHeli1;
        private ConfigurationView.ConfigFrameType configFrameType1;
        private ConfigurationView.ConfigHWCompass configHWCompass1;
        private ConfigurationView.ConfigAccelerometerCalibrationQuad configAccelerometerCalibrationQuad1;
        private ConfigurationView.ConfigAccelerometerCalibrationPlane configAccelerometerCalibrationPlane1;
        private ConfigurationView.ConfigRadioInput configRadioInput1;
        private ConfigurationView.ConfigFlightModes configFlightModes1;
        private ConfigurationView.ConfigFailSafe configFailSafe1;
        private _3DRradio _3DRradio1;
        private ConfigurationView.ConfigBatteryMonitoring configBatteryMonitoring1;
        private ConfigurationView.ConfigHWSonar configHWSonar1;
        private ConfigurationView.ConfigHWAirspeed configHWAirspeed1;
        private ConfigurationView.ConfigHWOptFlow configHWOptFlow1;
        private ConfigurationView.ConfigHWOSD configHWOSD1;
        private ConfigurationView.ConfigMount configMount1;
        private ConfigurationView.ConfigMotorTest configMotor1;
        private Antenna.Tracker tracker1;
        private Controls.BackstageView.BackstageViewPage backstageViewPageinstfw;
        private Controls.BackstageView.BackstageViewPage backstageViewPagewizard;
        private Controls.BackstageView.BackstageViewPage backstageViewPagemand;
        private Controls.BackstageView.BackstageViewPage backstageViewPageopt;
        private Controls.BackstageView.BackstageViewPage backstageViewPagetradheli;
        private Controls.BackstageView.BackstageViewPage backstageViewPageframetype;
        private Controls.BackstageView.BackstageViewPage backstageViewPagecompass;
        private Controls.BackstageView.BackstageViewPage backstageViewPageaccelquad;
        private Controls.BackstageView.BackstageViewPage backstageViewPageaccelplane;
        private Controls.BackstageView.BackstageViewPage backstageViewPageradio;
        private Controls.BackstageView.BackstageViewPage backstageViewPageflmode;
        private Controls.BackstageView.BackstageViewPage backstageViewPagefs;
        private Controls.BackstageView.BackstageViewPage backstageViewPage3drradio;
        private Controls.BackstageView.BackstageViewPage backstageViewPagebatmon;
        private Controls.BackstageView.BackstageViewPage backstageViewPagesonar;
        private Controls.BackstageView.BackstageViewPage backstageViewPageairspeed;
        private Controls.BackstageView.BackstageViewPage backstageViewPageoptflow;
        private Controls.BackstageView.BackstageViewPage backstageViewPageosd;
        private Controls.BackstageView.BackstageViewPage backstageViewPagegimbal;
        private Controls.BackstageView.BackstageViewPage backstageViewPageAntTrack;
        private Controls.BackstageView.BackstageViewPage backstageViewPagefwdisabled;
        private Controls.BackstageView.BackstageViewPage backstageViewPagefw;
        private System.Windows.Forms.BindingSource initialSetupBindingSource;
        private Controls.BackstageView.BackstageViewPage backstageViewPagecompassmot;
        private ConfigurationView.ConfigCompassMot configCompassMot1;
        private Controls.BackstageView.BackstageViewPage backstageViewPageMotorTest;
        private Controls.BackstageView.BackstageViewPage backstageViewPageacceltracker;
        private ConfigurationView.ConfigAccelerometerCalibrationTracker configAccelerometerCalibrationTracker1;
    }
}
