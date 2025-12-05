using System.Windows.Forms;

namespace MissionPlanner.GCSViews
{
    partial class FlightData
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlightData));
            this.MainH = new System.Windows.Forms.SplitContainer();
            this.SubMainLeft = new System.Windows.Forms.SplitContainer();
            this.hud1 = new MissionPlanner.Controls.HUD();
            this.contextMenuStripHud = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.videoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recordHudToAVIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setMJPEGSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setGStreamerSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hereLinkVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gStreamerStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAspectRatioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.russianHudToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.swapWithMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setBatteryCellCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bindingSourceHud = new System.Windows.Forms.BindingSource(this.components);
            this.contextMenuStripactionstab = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlactions = new System.Windows.Forms.TabControl();
            this.tabQuick = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelQuick = new System.Windows.Forms.TableLayoutPanel();
            this.quickView6 = new MissionPlanner.Controls.QuickView();
            this.contextMenuStripQuickView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editQuickViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setViewCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorMove = new System.Windows.Forms.ToolStripSeparator();
            this.moveLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorReset = new System.Windows.Forms.ToolStripSeparator();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bindingSourceQuickTab = new System.Windows.Forms.BindingSource(this.components);
            this.quickView5 = new MissionPlanner.Controls.QuickView();
            this.quickView4 = new MissionPlanner.Controls.QuickView();
            this.quickView3 = new MissionPlanner.Controls.QuickView();
            this.quickView2 = new MissionPlanner.Controls.QuickView();
            this.quickView1 = new MissionPlanner.Controls.QuickView();
            this.tabActions = new System.Windows.Forms.TabPage();
            this.flightDataActions1 = new MissionPlanner.Controls.FlightDataActions();
            this.panelActionsSpacer = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BUT_SendMSG = new MissionPlanner.Controls.MyButton();
            this.BUT_abortland = new MissionPlanner.Controls.MyButton();
            this.modifyandSetLoiterRad = new MissionPlanner.Controls.ModifyandSet();
            this.BUT_clear_track = new MissionPlanner.Controls.MyButton();
            this.CMB_action = new System.Windows.Forms.ComboBox();
            this.BUTactiondo = new MissionPlanner.Controls.MyButton();
            this.BUT_resumemis = new MissionPlanner.Controls.MyButton();
            this.modifyandSetAlt = new MissionPlanner.Controls.ModifyandSet();
            this.modifyandSetSpeed = new MissionPlanner.Controls.ModifyandSet();
            this.CMB_setwp = new System.Windows.Forms.ComboBox();
            this.BUT_ARM = new MissionPlanner.Controls.MyButton();
            this.BUT_mountmode = new MissionPlanner.Controls.MyButton();
            this.BUT_Reboot = new MissionPlanner.Controls.MyButton();
            this.BUT_joystick = new MissionPlanner.Controls.MyButton();
            this.BUT_RAWSensor = new MissionPlanner.Controls.MyButton();
            this.BUT_Homealt = new MissionPlanner.Controls.MyButton();
            this.BUTrestartmission = new MissionPlanner.Controls.MyButton();
            this.CMB_mountmode = new System.Windows.Forms.ComboBox();
            this.BUT_quickrtl = new MissionPlanner.Controls.MyButton();
            this.BUT_quickmanual = new MissionPlanner.Controls.MyButton();
            this.BUT_setwp = new MissionPlanner.Controls.MyButton();
            this.CMB_modes = new System.Windows.Forms.ComboBox();
            this.BUT_quickauto = new MissionPlanner.Controls.MyButton();
            this.BUT_setmode = new MissionPlanner.Controls.MyButton();
            this.tabPagemessages = new System.Windows.Forms.TabPage();
            this.messagesList1 = new MissionPlanner.Controls.MessagesList();
            this.txt_messagebox = new System.Windows.Forms.TextBox();
            this.tabParams = new System.Windows.Forms.TabPage();
            this.configRawParams1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigRawParams();
            this.tabVideo = new System.Windows.Forms.TabPage();
            this.flightPlannerVideoOptions1 = new MissionPlanner.Controls.FlightPlannerVideoOptions();
            this.tabTuning = new System.Windows.Forms.TabPage();
            this.configArduplane1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigArduplane();
            this.configArducopter1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigArducopter();
            this.configArdurover1 = new MissionPlanner.GCSViews.ConfigurationView.ConfigArdurover();
            this.tabInspector = new System.Windows.Forms.TabPage();
            this.mavlinkInspectorControl1 = new MissionPlanner.Controls.MAVLinkInspectorControl();
            this.tabActionsSimple = new System.Windows.Forms.TabPage();
            this.myButton1 = new MissionPlanner.Controls.MyButton();
            this.myButton2 = new MissionPlanner.Controls.MyButton();
            this.myButton3 = new MissionPlanner.Controls.MyButton();
            this.tabPagePreFlight = new System.Windows.Forms.TabPage();
            this.checkListControl1 = new MissionPlanner.Controls.PreFlight.CheckListControl();
            this.tabGauges = new System.Windows.Forms.TabPage();
            this.Gvspeed = new AGaugeApp.AGauge();
            this.bindingSourceGaugesTab = new System.Windows.Forms.BindingSource(this.components);
            this.Gheading = new MissionPlanner.Controls.HSI();
            this.Galt = new AGaugeApp.AGauge();
            this.Gspeed = new AGaugeApp.AGauge();
            this.tabTransponder = new System.Windows.Forms.TabPage();
            this.NACp_tb = new System.Windows.Forms.TextBox();
            this.NIC_tb = new System.Windows.Forms.TextBox();
            this.NACp_lbl = new System.Windows.Forms.Label();
            this.NIC_lbl = new System.Windows.Forms.Label();
            this.Squawk_nud = new System.Windows.Forms.NumericUpDown();
            this.FlightID_tb = new System.Windows.Forms.TextBox();
            this.fault_clb = new System.Windows.Forms.CheckedListBox();
            this.XPDRConnect_btn = new System.Windows.Forms.Button();
            this.Squawk_label = new System.Windows.Forms.Label();
            this.FlightID_label = new System.Windows.Forms.Label();
            this.IDENT_btn = new System.Windows.Forms.Button();
            this.ALT_btn = new System.Windows.Forms.Button();
            this.STBY_btn = new System.Windows.Forms.Button();
            this.ON_btn = new System.Windows.Forms.Button();
            this.Mode_clb = new System.Windows.Forms.CheckedListBox();
            this.tabStatus = new System.Windows.Forms.TabPage();
            this.tabServo = new System.Windows.Forms.TabPage();
            this.flowLayoutPanelServos = new System.Windows.Forms.FlowLayoutPanel();
            this.servoOptions1 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions2 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions3 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions4 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions5 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions6 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions7 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions8 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions9 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions10 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions11 = new MissionPlanner.Controls.ServoOptions();
            this.servoOptions12 = new MissionPlanner.Controls.ServoOptions();
            this.relayOptions1 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions2 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions3 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions4 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions5 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions6 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions7 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions8 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions9 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions10 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions11 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions12 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions13 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions14 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions15 = new MissionPlanner.Controls.RelayOptions();
            this.relayOptions16 = new MissionPlanner.Controls.RelayOptions();
            this.tabAuxFunction = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.auxOptions1 = new MissionPlanner.Controls.AuxOptions();
            this.auxOptions2 = new MissionPlanner.Controls.AuxOptions();
            this.auxOptions3 = new MissionPlanner.Controls.AuxOptions();
            this.auxOptions4 = new MissionPlanner.Controls.AuxOptions();
            this.auxOptions5 = new MissionPlanner.Controls.AuxOptions();
            this.auxOptions6 = new MissionPlanner.Controls.AuxOptions();
            this.auxOptions7 = new MissionPlanner.Controls.AuxOptions();
            this.tabScripts = new System.Windows.Forms.TabPage();
            this.checkBoxRedirectOutput = new System.Windows.Forms.CheckBox();
            this.BUT_edit_selected = new MissionPlanner.Controls.MyButton();
            this.labelSelectedScript = new System.Windows.Forms.Label();
            this.BUT_run_script = new MissionPlanner.Controls.MyButton();
            this.BUT_abort_script = new MissionPlanner.Controls.MyButton();
            this.labelScriptStatus = new System.Windows.Forms.Label();
            this.BUT_select_script = new MissionPlanner.Controls.MyButton();
            this.tabTLogs = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelLogs = new System.Windows.Forms.TableLayoutPanel();
            this.grpDataflash = new System.Windows.Forms.GroupBox();
            this.grpTelemetry = new System.Windows.Forms.GroupBox();
            this.tableLayoutPaneltlogs = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanelTLogControls = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanelTrackRow = new System.Windows.Forms.TableLayoutPanel();
            this.tracklog = new System.Windows.Forms.TrackBar();
            this.lbl_logpercent = new System.Windows.Forms.Label();
            this.flowLayoutPanelSpeed = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.comboPlaybackSpeed = new System.Windows.Forms.ComboBox();
            this.BUT_loadtelem = new MissionPlanner.Controls.MyButton();
            this.lbl_playbackspeed = new System.Windows.Forms.Label();
            this.LBL_logfn = new System.Windows.Forms.Label();
            this.BUT_log2kml = new MissionPlanner.Controls.MyButton();
            this.BUT_playlog = new MissionPlanner.Controls.MyButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.BUT_DFMavlink = new MissionPlanner.Controls.MyButton();
            this.BUT_georefimage = new MissionPlanner.Controls.MyButton();
            this.BUT_logbrowse = new MissionPlanner.Controls.MyButton();
            this.BUT_matlab = new MissionPlanner.Controls.MyButton();
            this.but_bintolog = new MissionPlanner.Controls.MyButton();
            this.but_dflogtokml = new MissionPlanner.Controls.MyButton();
            this.BUT_loganalysis = new MissionPlanner.Controls.MyButton();
            this.panel_persistent = new System.Windows.Forms.Panel();
            this.tableMap = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.zg1 = new ZedGraph.ZedGraphControl();
            this.configRawParams2 = new MissionPlanner.GCSViews.ConfigurationView.ConfigRawParams();
            this.contextMenuStripMap = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.goHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flyToHereAltToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flyToCoordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPoiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.poiatcoordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pointCameraHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PointCameraCoordsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.triggerCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flightPlannerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setHomeHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setEKFHomeHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setHomeHereToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.takeOffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onOffCameraOverlapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jumpToTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gimbalVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gimbalVideoFullSizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gimbalVideoMiniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gimbalVideoPopOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.but_disablejoystick = new MissionPlanner.Controls.MyButton();
            this.Zoomlevel = new System.Windows.Forms.NumericUpDown();
            this.distanceBar1 = new MissionPlanner.Controls.DistanceBar();
            this.TRK_zoom = new MissionPlanner.Controls.MyTrackBar();
            this.windDir1 = new MissionPlanner.Controls.WindDir();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_hdop = new MissionPlanner.Controls.MyLabel();
            this.lbl_sats = new MissionPlanner.Controls.MyLabel();
            this.gMapControl1 = new MissionPlanner.Controls.myGMAP();
            this.panel1 = new System.Windows.Forms.Panel();
            this.coords1 = new MissionPlanner.Controls.Coords();
            this.CHK_autopan = new System.Windows.Forms.CheckBox();
            this.CB_tuning = new System.Windows.Forms.CheckBox();
            this.CB_params = new System.Windows.Forms.CheckBox();
            this.ZedGraphTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openScriptDialog = new System.Windows.Forms.OpenFileDialog();
            this.scriptChecker = new System.Windows.Forms.Timer(this.components);
            this.Messagetabtimer = new System.Windows.Forms.Timer(this.components);
            this.bindingSourceStatusTab = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.MainH)).BeginInit();
            this.MainH.Panel1.SuspendLayout();
            this.MainH.Panel2.SuspendLayout();
            this.MainH.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SubMainLeft)).BeginInit();
            this.SubMainLeft.Panel1.SuspendLayout();
            this.SubMainLeft.Panel2.SuspendLayout();
            this.SubMainLeft.SuspendLayout();
            this.contextMenuStripHud.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceHud)).BeginInit();
            this.contextMenuStripactionstab.SuspendLayout();
            this.tabControlactions.SuspendLayout();
            this.tabQuick.SuspendLayout();
            this.tableLayoutPanelQuick.SuspendLayout();
            this.contextMenuStripQuickView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceQuickTab)).BeginInit();
            this.tabActions.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPagemessages.SuspendLayout();
            this.tabParams.SuspendLayout();
            this.tabVideo.SuspendLayout();
            this.tabTuning.SuspendLayout();
            this.tabInspector.SuspendLayout();
            this.tabActionsSimple.SuspendLayout();
            this.tabPagePreFlight.SuspendLayout();
            this.tabGauges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceGaugesTab)).BeginInit();
            this.tabTransponder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Squawk_nud)).BeginInit();
            this.tabServo.SuspendLayout();
            this.flowLayoutPanelServos.SuspendLayout();
            this.tabAuxFunction.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabScripts.SuspendLayout();
            this.tabTLogs.SuspendLayout();
            this.tableLayoutPanelLogs.SuspendLayout();
            this.grpDataflash.SuspendLayout();
            this.grpTelemetry.SuspendLayout();
            this.tableLayoutPaneltlogs.SuspendLayout();
            this.flowLayoutPanelTLogControls.SuspendLayout();
            this.tableLayoutPanelTrackRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tracklog)).BeginInit();
            this.flowLayoutPanelSpeed.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStripMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Zoomlevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TRK_zoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceStatusTab)).BeginInit();
            this.SuspendLayout();
            // 
            // MainH
            // 
            this.MainH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.MainH, "MainH");
            this.MainH.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.MainH.Name = "MainH";
            // 
            // MainH.Panel1
            // 
            this.MainH.Panel1.Controls.Add(this.SubMainLeft);
            // 
            // MainH.Panel2
            // 
            this.MainH.Panel2.Controls.Add(this.tableMap);
            // 
            // SubMainLeft
            // 
            this.SubMainLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.SubMainLeft, "SubMainLeft");
            this.SubMainLeft.Name = "SubMainLeft";
            // 
            // SubMainLeft.Panel1
            // 
            this.SubMainLeft.Panel1.Controls.Add(this.hud1);
            // 
            // SubMainLeft.Panel2
            // 
            this.SubMainLeft.Panel2.ContextMenuStrip = this.contextMenuStripactionstab;
            this.SubMainLeft.Panel2.Controls.Add(this.tabControlactions);
            this.SubMainLeft.Panel2.Controls.Add(this.panel_persistent);
            // 
            // hud1
            // 
            this.hud1.airspeed = 0F;
            this.hud1.alt = 0F;
            this.hud1.altunit = null;
            this.hud1.AOA = 0F;
            this.hud1.BackColor = System.Drawing.Color.Black;
            this.hud1.batterycellcount = 4;
            this.hud1.batterylevel = 0F;
            this.hud1.batterylevel2 = 0F;
            this.hud1.batteryon2 = true;
            this.hud1.batteryremaining = 0F;
            this.hud1.batteryremaining2 = 0F;
            this.hud1.bgimage = null;
            this.hud1.connected = false;
            this.hud1.ContextMenuStrip = this.contextMenuStripHud;
            this.hud1.critAOA = 25F;
            this.hud1.criticalvoltagealert = false;
            this.hud1.critSSA = 30F;
            this.hud1.current = 0F;
            this.hud1.current2 = 0F;
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("airspeed", this.bindingSourceHud, "airspeed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("alt", this.bindingSourceHud, "alt", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("load", this.bindingSourceHud, "load", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("batterylevel", this.bindingSourceHud, "battery_voltage", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("batteryremaining", this.bindingSourceHud, "battery_remaining", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("connected", this.bindingSourceHud, "connected", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("current", this.bindingSourceHud, "current", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("batterylevel2", this.bindingSourceHud, "battery_voltage2", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("batteryremaining2", this.bindingSourceHud, "battery_remaining2", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("current2", this.bindingSourceHud, "current2", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("datetime", this.bindingSourceHud, "datetime", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("disttowp", this.bindingSourceHud, "wp_dist", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("ekfstatus", this.bindingSourceHud, "ekfstatus", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("failsafe", this.bindingSourceHud, "failsafe", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("gpsfix", this.bindingSourceHud, "gpsstatus", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("gpsfix2", this.bindingSourceHud, "gpsstatus2", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("gpshdop", this.bindingSourceHud, "gpshdop", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("gpshdop2", this.bindingSourceHud, "gpshdop2", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("groundalt", this.bindingSourceHud, "HomeAlt", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("groundcourse", this.bindingSourceHud, "groundcourse", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("groundspeed", this.bindingSourceHud, "groundspeed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("heading", this.bindingSourceHud, "yaw", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("linkqualitygcs", this.bindingSourceHud, "linkqualitygcs", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("message", this.bindingSourceHud, "messageHigh", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("messageSeverity", this.bindingSourceHud, "messageHighSeverity", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("mode", this.bindingSourceHud, "mode", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("navpitch", this.bindingSourceHud, "nav_pitch", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("navroll", this.bindingSourceHud, "nav_roll", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("pitch", this.bindingSourceHud, "pitch", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("prearmstatus", this.bindingSourceHud, "prearmstatus", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("roll", this.bindingSourceHud, "roll", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("safetyactive", this.bindingSourceHud, "safetyactive", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("status", this.bindingSourceHud, "armed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("targetalt", this.bindingSourceHud, "targetalt", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("targetheading", this.bindingSourceHud, "nav_bearing", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("targetspeed", this.bindingSourceHud, "targetairspeed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("turnrate", this.bindingSourceHud, "turnrate", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("verticalspeed", this.bindingSourceHud, "verticalspeed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("vibex", this.bindingSourceHud, "vibex", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("vibey", this.bindingSourceHud, "vibey", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("vibez", this.bindingSourceHud, "vibez", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("wpno", this.bindingSourceHud, "wpno", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("xtrack_error", this.bindingSourceHud, "xtrack_error", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("AOA", this.bindingSourceHud, "AOA", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("SSA", this.bindingSourceHud, "SSA", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("critAOA", this.bindingSourceHud, "crit_AOA", true));
            this.hud1.datetime = new System.DateTime(((long)(0)));
            this.hud1.displayAOASSA = false;
            this.hud1.displayCellVoltage = false;
            this.hud1.displayicons = false;
            this.hud1.disttowp = 0F;
            this.hud1.distunit = null;
            resources.ApplyResources(this.hud1, "hud1");
            this.hud1.ekfstatus = 0F;
            this.hud1.failsafe = false;
            this.hud1.gpsfix = 0F;
            this.hud1.gpsfix2 = 0F;
            this.hud1.gpshdop = 0F;
            this.hud1.gpshdop2 = 0F;
            this.hud1.groundalt = 0F;
            this.hud1.groundcourse = 0F;
            this.hud1.groundspeed = 0F;
            this.hud1.heading = 0F;
            this.hud1.hudcolor = System.Drawing.Color.LightGray;
            this.hud1.linkqualitygcs = 0F;
            this.hud1.load = 0F;
            this.hud1.lowairspeed = false;
            this.hud1.lowgroundspeed = false;
            this.hud1.lowvoltagealert = false;
            this.hud1.message = "";
            this.hud1.messageSeverity = MAVLink.MAV_SEVERITY.EMERGENCY;
            this.hud1.mode = "Unknown";
            this.hud1.Name = "hud1";
            this.hud1.navpitch = 0F;
            this.hud1.navroll = 0F;
            this.hud1.pitch = 0F;
            this.hud1.prearmstatus = false;
            this.hud1.roll = 0F;
            this.hud1.Russian = false;
            this.hud1.safetyactive = false;
            this.hud1.skyColor1 = System.Drawing.Color.Blue;
            this.hud1.skyColor2 = System.Drawing.Color.LightBlue;
            this.hud1.speedunit = null;
            this.hud1.SSA = 0F;
            this.hud1.status = false;
            this.hud1.targetalt = 0F;
            this.hud1.targetheading = 0F;
            this.hud1.targetspeed = 0F;
            this.hud1.turnrate = 0F;
            this.hud1.verticalspeed = 0F;
            this.hud1.vibex = 0F;
            this.hud1.vibey = 0F;
            this.hud1.vibez = 0F;
            this.hud1.VSync = false;
            this.hud1.wpno = 0;
            this.hud1.xtrack_error = 0F;
            this.hud1.ekfclick += new System.EventHandler(this.hud1_ekfclick);
            this.hud1.vibeclick += new System.EventHandler(this.hud1_vibeclick);
            this.hud1.prearmclick += new System.EventHandler(this.hud1_prearmclick);
            this.hud1.Load += new System.EventHandler(this.hud1_Load);
            this.hud1.DoubleClick += new System.EventHandler(this.hud1_DoubleClick);
            this.hud1.Resize += new System.EventHandler(this.hud1_Resize);
            // 
            // contextMenuStripHud
            // 
            this.contextMenuStripHud.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.videoToolStripMenuItem,
            this.setAspectRatioToolStripMenuItem,
            this.userItemsToolStripMenuItem,
            this.russianHudToolStripMenuItem,
            this.swapWithMapToolStripMenuItem,
            this.groundColorToolStripMenuItem,
            this.setBatteryCellCountToolStripMenuItem,
            this.showIconsToolStripMenuItem});
            this.contextMenuStripHud.Name = "contextMenuStrip2";
            resources.ApplyResources(this.contextMenuStripHud, "contextMenuStripHud");
            // 
            // videoToolStripMenuItem
            // 
            this.videoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recordHudToAVIToolStripMenuItem,
            this.stopRecordToolStripMenuItem,
            this.setMJPEGSourceToolStripMenuItem,
            this.startCameraToolStripMenuItem,
            this.setGStreamerSourceToolStripMenuItem,
            this.hereLinkVideoToolStripMenuItem,
            this.gStreamerStopToolStripMenuItem});
            this.videoToolStripMenuItem.Name = "videoToolStripMenuItem";
            resources.ApplyResources(this.videoToolStripMenuItem, "videoToolStripMenuItem");
            // 
            // recordHudToAVIToolStripMenuItem
            // 
            this.recordHudToAVIToolStripMenuItem.Name = "recordHudToAVIToolStripMenuItem";
            resources.ApplyResources(this.recordHudToAVIToolStripMenuItem, "recordHudToAVIToolStripMenuItem");
            this.recordHudToAVIToolStripMenuItem.Click += new System.EventHandler(this.recordHudToAVIToolStripMenuItem_Click);
            // 
            // stopRecordToolStripMenuItem
            // 
            this.stopRecordToolStripMenuItem.Name = "stopRecordToolStripMenuItem";
            resources.ApplyResources(this.stopRecordToolStripMenuItem, "stopRecordToolStripMenuItem");
            this.stopRecordToolStripMenuItem.Click += new System.EventHandler(this.stopRecordToolStripMenuItem_Click);
            // 
            // setMJPEGSourceToolStripMenuItem
            // 
            this.setMJPEGSourceToolStripMenuItem.Name = "setMJPEGSourceToolStripMenuItem";
            resources.ApplyResources(this.setMJPEGSourceToolStripMenuItem, "setMJPEGSourceToolStripMenuItem");
            this.setMJPEGSourceToolStripMenuItem.Click += new System.EventHandler(this.setMJPEGSourceToolStripMenuItem_Click);
            // 
            // startCameraToolStripMenuItem
            // 
            this.startCameraToolStripMenuItem.Name = "startCameraToolStripMenuItem";
            resources.ApplyResources(this.startCameraToolStripMenuItem, "startCameraToolStripMenuItem");
            this.startCameraToolStripMenuItem.Click += new System.EventHandler(this.startCameraToolStripMenuItem_Click);
            // 
            // setGStreamerSourceToolStripMenuItem
            // 
            this.setGStreamerSourceToolStripMenuItem.Name = "setGStreamerSourceToolStripMenuItem";
            resources.ApplyResources(this.setGStreamerSourceToolStripMenuItem, "setGStreamerSourceToolStripMenuItem");
            this.setGStreamerSourceToolStripMenuItem.Click += new System.EventHandler(this.setGStreamerSourceToolStripMenuItem_Click);
            // 
            // hereLinkVideoToolStripMenuItem
            // 
            this.hereLinkVideoToolStripMenuItem.Name = "hereLinkVideoToolStripMenuItem";
            resources.ApplyResources(this.hereLinkVideoToolStripMenuItem, "hereLinkVideoToolStripMenuItem");
            this.hereLinkVideoToolStripMenuItem.Click += new System.EventHandler(this.HereLinkVideoToolStripMenuItem_Click);
            // 
            // gStreamerStopToolStripMenuItem
            // 
            this.gStreamerStopToolStripMenuItem.Name = "gStreamerStopToolStripMenuItem";
            resources.ApplyResources(this.gStreamerStopToolStripMenuItem, "gStreamerStopToolStripMenuItem");
            this.gStreamerStopToolStripMenuItem.Click += new System.EventHandler(this.GStreamerStopToolStripMenuItem_Click);
            // 
            // setAspectRatioToolStripMenuItem
            // 
            this.setAspectRatioToolStripMenuItem.Name = "setAspectRatioToolStripMenuItem";
            resources.ApplyResources(this.setAspectRatioToolStripMenuItem, "setAspectRatioToolStripMenuItem");
            this.setAspectRatioToolStripMenuItem.Click += new System.EventHandler(this.setAspectRatioToolStripMenuItem_Click);
            // 
            // userItemsToolStripMenuItem
            // 
            this.userItemsToolStripMenuItem.Name = "userItemsToolStripMenuItem";
            resources.ApplyResources(this.userItemsToolStripMenuItem, "userItemsToolStripMenuItem");
            this.userItemsToolStripMenuItem.Click += new System.EventHandler(this.hud_UserItem);
            // 
            // russianHudToolStripMenuItem
            // 
            this.russianHudToolStripMenuItem.Name = "russianHudToolStripMenuItem";
            resources.ApplyResources(this.russianHudToolStripMenuItem, "russianHudToolStripMenuItem");
            this.russianHudToolStripMenuItem.Click += new System.EventHandler(this.russianHudToolStripMenuItem_Click);
            // 
            // swapWithMapToolStripMenuItem
            // 
            this.swapWithMapToolStripMenuItem.Name = "swapWithMapToolStripMenuItem";
            resources.ApplyResources(this.swapWithMapToolStripMenuItem, "swapWithMapToolStripMenuItem");
            this.swapWithMapToolStripMenuItem.Click += new System.EventHandler(this.swapWithMapToolStripMenuItem_Click);
            // 
            // groundColorToolStripMenuItem
            // 
            this.groundColorToolStripMenuItem.CheckOnClick = true;
            this.groundColorToolStripMenuItem.Name = "groundColorToolStripMenuItem";
            resources.ApplyResources(this.groundColorToolStripMenuItem, "groundColorToolStripMenuItem");
            this.groundColorToolStripMenuItem.Click += new System.EventHandler(this.groundColorToolStripMenuItem_Click);
            // 
            // setBatteryCellCountToolStripMenuItem
            // 
            this.setBatteryCellCountToolStripMenuItem.Name = "setBatteryCellCountToolStripMenuItem";
            resources.ApplyResources(this.setBatteryCellCountToolStripMenuItem, "setBatteryCellCountToolStripMenuItem");
            this.setBatteryCellCountToolStripMenuItem.Click += new System.EventHandler(this.setBatteryCellCountToolStripMenuItem_Click);
            // 
            // showIconsToolStripMenuItem
            // 
            this.showIconsToolStripMenuItem.Name = "showIconsToolStripMenuItem";
            resources.ApplyResources(this.showIconsToolStripMenuItem, "showIconsToolStripMenuItem");
            this.showIconsToolStripMenuItem.Click += new System.EventHandler(this.showIconsToolStripMenuItem_Click);
            // 
            // bindingSourceHud
            // 
            this.bindingSourceHud.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // contextMenuStripactionstab
            // 
            this.contextMenuStripactionstab.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customizeToolStripMenuItem});
            this.contextMenuStripactionstab.Name = "contextMenuStripactionstab";
            resources.ApplyResources(this.contextMenuStripactionstab, "contextMenuStripactionstab");
            //
            // customizeToolStripMenuItem
            //
            this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            resources.ApplyResources(this.customizeToolStripMenuItem, "customizeToolStripMenuItem");
            this.customizeToolStripMenuItem.Click += new System.EventHandler(this.customizeToolStripMenuItem_Click);
            // 
            // tabControlactions
            // 
            this.tabControlactions.ContextMenuStrip = this.contextMenuStripactionstab;
            this.tabControlactions.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlactions.Controls.Add(this.tabQuick);
            this.tabControlactions.Controls.Add(this.tabActions);
            this.tabControlactions.Controls.Add(this.tabPagemessages);
            this.tabControlactions.Controls.Add(this.tabParams);
            this.tabControlactions.Controls.Add(this.tabVideo);
            this.tabControlactions.Controls.Add(this.tabTuning);
            this.tabControlactions.Controls.Add(this.tabInspector);
            this.tabControlactions.Controls.Add(this.tabActionsSimple);
            this.tabControlactions.Controls.Add(this.tabPagePreFlight);
            this.tabControlactions.Controls.Add(this.tabGauges);
            this.tabControlactions.Controls.Add(this.tabTransponder);
            this.tabControlactions.Controls.Add(this.tabStatus);
            this.tabControlactions.Controls.Add(this.tabServo);
            this.tabControlactions.Controls.Add(this.tabAuxFunction);
            this.tabControlactions.Controls.Add(this.tabScripts);
            this.tabControlactions.Controls.Add(this.tabTLogs);
            resources.ApplyResources(this.tabControlactions, "tabControlactions");
            this.tabControlactions.Name = "tabControlactions";
            this.tabControlactions.SelectedIndex = 0;
            this.tabControlactions.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControlactions.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabQuick
            // 
            resources.ApplyResources(this.tabQuick, "tabQuick");
            this.tabQuick.Controls.Add(this.tableLayoutPanelQuick);
            this.tabQuick.Name = "tabQuick";
            this.tabQuick.UseVisualStyleBackColor = true;
            this.tabQuick.Resize += new System.EventHandler(this.tabQuick_Resize);
            // 
            // tableLayoutPanelQuick
            // 
            resources.ApplyResources(this.tableLayoutPanelQuick, "tableLayoutPanelQuick");
            this.tableLayoutPanelQuick.Controls.Add(this.quickView6, 1, 2);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView5, 0, 2);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView4, 1, 1);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView3, 0, 1);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView2, 1, 0);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView1, 0, 0);
            this.tableLayoutPanelQuick.Name = "tableLayoutPanelQuick";
            // 
            // quickView6
            // 
            this.quickView6.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView6.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "DistToHome", true));
            this.quickView6.desc = "DistToMAV";
            resources.ApplyResources(this.quickView6, "quickView6");
            this.quickView6.Name = "quickView6";
            this.quickView6.number = 0D;
            this.quickView6.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(252)))));
            this.quickView6.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView6.numberformat = "0.00";
            this.quickView6.DoubleClick += new System.EventHandler(this.quickView_DoubleClick);
            // 
            // contextMenuStripQuickView
            //
            this.contextMenuStripQuickView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editQuickViewToolStripMenuItem,
            this.setViewCountToolStripMenuItem,
            this.undockToolStripMenuItem,
            this.toolStripSeparatorMove,
            this.moveLeftToolStripMenuItem,
            this.moveRightToolStripMenuItem,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripSeparatorReset,
            this.resetToolStripMenuItem,
            this.resetAllToolStripMenuItem});
            this.contextMenuStripQuickView.Name = "contextMenuStripQuickView";
            resources.ApplyResources(this.contextMenuStripQuickView, "contextMenuStripQuickView");
            this.contextMenuStripQuickView.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripQuickView_Opening);
            //
            // editQuickViewToolStripMenuItem
            //
            this.editQuickViewToolStripMenuItem.Name = "editQuickViewToolStripMenuItem";
            this.editQuickViewToolStripMenuItem.Text = "Edit";
            this.editQuickViewToolStripMenuItem.Click += new System.EventHandler(this.editQuickViewToolStripMenuItem_Click);
            //
            // setViewCountToolStripMenuItem
            //
            this.setViewCountToolStripMenuItem.Name = "setViewCountToolStripMenuItem";
            resources.ApplyResources(this.setViewCountToolStripMenuItem, "setViewCountToolStripMenuItem");
            this.setViewCountToolStripMenuItem.Click += new System.EventHandler(this.setViewCountToolStripMenuItem_Click);
            //
            // undockToolStripMenuItem
            //
            this.undockToolStripMenuItem.Name = "undockToolStripMenuItem";
            resources.ApplyResources(this.undockToolStripMenuItem, "undockToolStripMenuItem");
            this.undockToolStripMenuItem.Click += new System.EventHandler(this.undockDockToolStripMenuItem_Click);
            //
            // toolStripSeparatorMove
            //
            this.toolStripSeparatorMove.Name = "toolStripSeparatorMove";
            resources.ApplyResources(this.toolStripSeparatorMove, "toolStripSeparatorMove");
            //
            // moveLeftToolStripMenuItem
            //
            this.moveLeftToolStripMenuItem.Name = "moveLeftToolStripMenuItem";
            this.moveLeftToolStripMenuItem.Text = "Move Left";
            resources.ApplyResources(this.moveLeftToolStripMenuItem, "moveLeftToolStripMenuItem");
            this.moveLeftToolStripMenuItem.Click += new System.EventHandler(this.moveLeftToolStripMenuItem_Click);
            //
            // moveRightToolStripMenuItem
            //
            this.moveRightToolStripMenuItem.Name = "moveRightToolStripMenuItem";
            this.moveRightToolStripMenuItem.Text = "Move Right";
            resources.ApplyResources(this.moveRightToolStripMenuItem, "moveRightToolStripMenuItem");
            this.moveRightToolStripMenuItem.Click += new System.EventHandler(this.moveRightToolStripMenuItem_Click);
            //
            // moveUpToolStripMenuItem
            //
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Text = "Move Up";
            resources.ApplyResources(this.moveUpToolStripMenuItem, "moveUpToolStripMenuItem");
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            //
            // moveDownToolStripMenuItem
            //
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Text = "Move Down";
            resources.ApplyResources(this.moveDownToolStripMenuItem, "moveDownToolStripMenuItem");
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            //
            // toolStripSeparatorReset
            //
            this.toolStripSeparatorReset.Name = "toolStripSeparatorReset";
            resources.ApplyResources(this.toolStripSeparatorReset, "toolStripSeparatorReset");
            //
            // resetQuickViewToolStripMenuItem
            //
            this.resetToolStripMenuItem.Name = "resetQuickViewToolStripMenuItem";
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetQuickViewToolStripMenuItem_Click);
            //
            // resetAllQuickViewToolStripMenuItem
            //
            this.resetAllToolStripMenuItem.Name = "resetAllQuickViewToolStripMenuItem";
            this.resetAllToolStripMenuItem.Text = "Reset All";
            this.resetAllToolStripMenuItem.Click += new System.EventHandler(this.resetAllQuickViewToolStripMenuItem_Click);
            //
            // bindingSourceQuickTab
            // 
            this.bindingSourceQuickTab.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // quickView5
            // 
            this.quickView5.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView5.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "verticalspeed", true));
            this.quickView5.desc = "verticalspeed";
            resources.ApplyResources(this.quickView5, "quickView5");
            this.quickView5.Name = "quickView5";
            this.quickView5.number = 0D;
            this.quickView5.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(254)))), ((int)(((byte)(86)))));
            this.quickView5.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView5.numberformat = "0.00";
            this.quickView5.DoubleClick += new System.EventHandler(this.quickView_DoubleClick);
            // 
            // quickView4
            // 
            this.quickView4.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView4.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "yaw", true));
            this.quickView4.desc = "yaw";
            resources.ApplyResources(this.quickView4, "quickView4");
            this.quickView4.Name = "quickView4";
            this.quickView4.number = 0D;
            this.quickView4.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(83)))));
            this.quickView4.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView4.numberformat = "0.00";
            this.quickView4.DoubleClick += new System.EventHandler(this.quickView_DoubleClick);
            // 
            // quickView3
            // 
            this.quickView3.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView3.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "wp_dist", true));
            this.quickView3.desc = "wp_dist";
            resources.ApplyResources(this.quickView3, "quickView3");
            this.quickView3.Name = "quickView3";
            this.quickView3.number = 0D;
            this.quickView3.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(91)))));
            this.quickView3.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView3.numberformat = "0.00";
            this.quickView3.DoubleClick += new System.EventHandler(this.quickView_DoubleClick);
            // 
            // quickView2
            // 
            this.quickView2.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView2.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "groundspeed", true));
            this.quickView2.desc = "groundspeed";
            resources.ApplyResources(this.quickView2, "quickView2");
            this.quickView2.Name = "quickView2";
            this.quickView2.number = 0D;
            this.quickView2.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(132)))), ((int)(((byte)(46)))));
            this.quickView2.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView2.numberformat = "0.00";
            this.quickView2.DoubleClick += new System.EventHandler(this.quickView_DoubleClick);
            // 
            // quickView1
            // 
            this.quickView1.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView1.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "alt", true));
            this.quickView1.desc = "alt";
            resources.ApplyResources(this.quickView1, "quickView1");
            this.quickView1.Name = "quickView1";
            this.quickView1.number = 0D;
            this.quickView1.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(151)))), ((int)(((byte)(248)))));
            this.quickView1.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView1.numberformat = "0.00";
            this.toolTip1.SetToolTip(this.quickView1, resources.GetString("quickView1.ToolTip"));
            this.quickView1.DoubleClick += new System.EventHandler(this.quickView_DoubleClick);
            //
            // tabActions
            //
            this.tabActions.Controls.Add(this.flightDataActions1);
            resources.ApplyResources(this.tabActions, "tabActions");
            this.tabActions.Name = "tabActions";
            this.tabActions.Padding = new System.Windows.Forms.Padding(4);
            this.tabActions.UseVisualStyleBackColor = true;
            //
            // flightDataActions1
            //
            this.flightDataActions1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flightDataActions1.Location = new System.Drawing.Point(4, 4);
            this.flightDataActions1.Name = "flightDataActions1";
            this.flightDataActions1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnStyles.Clear();
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Controls.Add(this.BUT_SendMSG, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.BUT_abortland, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.modifyandSetLoiterRad, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.BUT_clear_track, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.CMB_action, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.BUTactiondo, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.BUT_resumemis, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.modifyandSetAlt, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.modifyandSetSpeed, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.CMB_setwp, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.BUT_ARM, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.BUT_mountmode, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.BUT_Reboot, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.BUT_joystick, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.BUT_RAWSensor, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.BUT_Homealt, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.BUTrestartmission, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.CMB_mountmode, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.BUT_quickrtl, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.BUT_quickmanual, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.BUT_setwp, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.CMB_modes, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.BUT_quickauto, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.BUT_setmode, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
            // 
            // panelActionsSpacer
            // 
            this.panelActionsSpacer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelActionsSpacer.Location = new System.Drawing.Point(4, 4);
            this.panelActionsSpacer.Name = "panelActionsSpacer";
            this.panelActionsSpacer.Size = new System.Drawing.Size(0, 0);
            this.panelActionsSpacer.TabIndex = 79;
            // 
            // BUT_SendMSG
            // 
            this.BUT_SendMSG.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_SendMSG.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_SendMSG.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_SendMSG, "BUT_SendMSG");
            this.BUT_SendMSG.Name = "BUT_SendMSG";
            this.BUT_SendMSG.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_SendMSG, resources.GetString("BUT_SendMSG.ToolTip"));
            this.BUT_SendMSG.UseVisualStyleBackColor = true;
            this.BUT_SendMSG.Click += new System.EventHandler(this.BUT_SendMSG_Click);
            // 
            // BUT_abortland
            // 
            this.BUT_abortland.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_abortland.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_abortland.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_abortland, "BUT_abortland");
            this.BUT_abortland.Name = "BUT_abortland";
            this.BUT_abortland.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_abortland, resources.GetString("BUT_abortland.ToolTip"));
            this.BUT_abortland.UseVisualStyleBackColor = true;
            this.BUT_abortland.Click += new System.EventHandler(this.BUT_abortland_Click);
            // 
            // modifyandSetLoiterRad
            //
            resources.ApplyResources(this.modifyandSetLoiterRad, "modifyandSetLoiterRad");
            this.modifyandSetLoiterRad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.modifyandSetLoiterRad.Margin = new System.Windows.Forms.Padding(4);
            this.modifyandSetLoiterRad.ButtonText = "Set Loit Rad";
            this.modifyandSetLoiterRad.DecimalPlaces = 0;
            this.modifyandSetLoiterRad.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.modifyandSetLoiterRad.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.modifyandSetLoiterRad.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.modifyandSetLoiterRad.Name = "modifyandSetLoiterRad";
            this.modifyandSetLoiterRad.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.modifyandSetLoiterRad.Click += new System.EventHandler(this.modifyandSetLoiterRad_Click);
            // 
            // BUT_clear_track
            // 
            this.BUT_clear_track.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_clear_track.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_clear_track.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_clear_track, "BUT_clear_track");
            this.BUT_clear_track.Name = "BUT_clear_track";
            this.BUT_clear_track.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_clear_track, resources.GetString("BUT_clear_track.ToolTip"));
            this.BUT_clear_track.UseVisualStyleBackColor = true;
            this.BUT_clear_track.Click += new System.EventHandler(this.BUT_clear_track_Click);
            // 
            // CMB_action
            // 
            resources.ApplyResources(this.CMB_action, "CMB_action");
            this.CMB_action.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_action.DropDownWidth = 250;
            this.CMB_action.FormattingEnabled = true;
            this.CMB_action.Name = "CMB_action";
            // 
            // BUTactiondo
            // 
            this.BUTactiondo.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUTactiondo.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUTactiondo.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUTactiondo, "BUTactiondo");
            this.BUTactiondo.Name = "BUTactiondo";
            this.BUTactiondo.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUTactiondo, resources.GetString("BUTactiondo.ToolTip"));
            this.BUTactiondo.UseVisualStyleBackColor = true;
            this.BUTactiondo.Click += new System.EventHandler(this.BUTactiondo_Click);
            // 
            // BUT_resumemis
            // 
            this.BUT_resumemis.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_resumemis.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_resumemis.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_resumemis, "BUT_resumemis");
            this.BUT_resumemis.Name = "BUT_resumemis";
            this.BUT_resumemis.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_resumemis.UseVisualStyleBackColor = true;
            this.BUT_resumemis.Click += new System.EventHandler(this.BUT_resumemis_Click);
            // 
            // modifyandSetAlt
            //
            resources.ApplyResources(this.modifyandSetAlt, "modifyandSetAlt");
            this.modifyandSetAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.modifyandSetAlt.Margin = new System.Windows.Forms.Padding(4);
            this.modifyandSetAlt.ButtonText = "Set Alt";
            this.modifyandSetAlt.DecimalPlaces = 1;
            this.modifyandSetAlt.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.modifyandSetAlt.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.modifyandSetAlt.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modifyandSetAlt.Name = "modifyandSetAlt";
            this.modifyandSetAlt.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.modifyandSetAlt.Click += new System.EventHandler(this.modifyandSetAlt_Click);
            // 
            // modifyandSetSpeed
            //
            resources.ApplyResources(this.modifyandSetSpeed, "modifyandSetSpeed");
            this.modifyandSetSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.modifyandSetSpeed.Margin = new System.Windows.Forms.Padding(4);
            this.modifyandSetSpeed.ButtonText = "Set Speed";
            this.modifyandSetSpeed.DecimalPlaces = 1;
            this.modifyandSetSpeed.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.modifyandSetSpeed.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.modifyandSetSpeed.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modifyandSetSpeed.Name = "modifyandSetSpeed";
            this.modifyandSetSpeed.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.modifyandSetSpeed.Click += new System.EventHandler(this.modifyandSetSpeed_Click);
            this.modifyandSetSpeed.ParentChanged += new System.EventHandler(this.modifyandSetSpeed_ParentChanged);
            // 
            // CMB_setwp
            // 
            resources.ApplyResources(this.CMB_setwp, "CMB_setwp");
            this.CMB_setwp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_setwp.DropDownWidth = 150;
            this.CMB_setwp.FormattingEnabled = true;
            this.CMB_setwp.Items.AddRange(new object[] {
            resources.GetString("CMB_setwp.Items")});
            this.CMB_setwp.Name = "CMB_setwp";
            this.CMB_setwp.Click += new System.EventHandler(this.CMB_setwp_Click);
            // 
            // BUT_ARM
            // 
            this.BUT_ARM.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_ARM.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_ARM.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_ARM, "BUT_ARM");
            this.BUT_ARM.Name = "BUT_ARM";
            this.BUT_ARM.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_ARM, resources.GetString("BUT_ARM.ToolTip"));
            this.BUT_ARM.UseVisualStyleBackColor = true;
            this.BUT_ARM.Click += new System.EventHandler(this.BUT_ARM_Click);
            // 
            // BUT_mountmode
            // 
            this.BUT_mountmode.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_mountmode.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_mountmode.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_mountmode, "BUT_mountmode");
            this.BUT_mountmode.Name = "BUT_mountmode";
            this.BUT_mountmode.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_mountmode, resources.GetString("BUT_mountmode.ToolTip"));
            this.BUT_mountmode.UseVisualStyleBackColor = true;
            this.BUT_mountmode.Click += new System.EventHandler(this.BUT_mountmode_Click);
            //
            // BUT_Reboot
            //
            this.BUT_Reboot.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_Reboot.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_Reboot.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_Reboot, "BUT_Reboot");
            this.BUT_Reboot.Name = "BUT_Reboot";
            this.BUT_Reboot.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Reboot.UseVisualStyleBackColor = true;
            this.BUT_Reboot.Click += new System.EventHandler(this.BUT_Reboot_Click);
            //
            // BUT_joystick
            // 
            this.BUT_joystick.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_joystick.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_joystick.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_joystick, "BUT_joystick");
            this.BUT_joystick.Name = "BUT_joystick";
            this.BUT_joystick.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_joystick, resources.GetString("BUT_joystick.ToolTip"));
            this.BUT_joystick.UseVisualStyleBackColor = true;
            this.BUT_joystick.Click += new System.EventHandler(this.BUT_joystick_Click);
            // 
            // BUT_RAWSensor
            // 
            this.BUT_RAWSensor.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_RAWSensor.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_RAWSensor.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_RAWSensor, "BUT_RAWSensor");
            this.BUT_RAWSensor.Name = "BUT_RAWSensor";
            this.BUT_RAWSensor.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_RAWSensor, resources.GetString("BUT_RAWSensor.ToolTip"));
            this.BUT_RAWSensor.UseVisualStyleBackColor = true;
            this.BUT_RAWSensor.Click += new System.EventHandler(this.BUT_RAWSensor_Click);
            // 
            // BUT_Homealt
            // 
            this.BUT_Homealt.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_Homealt.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_Homealt.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_Homealt, "BUT_Homealt");
            this.BUT_Homealt.Name = "BUT_Homealt";
            this.BUT_Homealt.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_Homealt, resources.GetString("BUT_Homealt.ToolTip"));
            this.BUT_Homealt.UseVisualStyleBackColor = true;
            this.BUT_Homealt.Click += new System.EventHandler(this.BUT_Homealt_Click);
            // 
            // BUTrestartmission
            // 
            this.BUTrestartmission.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUTrestartmission.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUTrestartmission.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUTrestartmission, "BUTrestartmission");
            this.BUTrestartmission.Name = "BUTrestartmission";
            this.BUTrestartmission.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUTrestartmission, resources.GetString("BUTrestartmission.ToolTip"));
            this.BUTrestartmission.UseVisualStyleBackColor = true;
            this.BUTrestartmission.Click += new System.EventHandler(this.BUTrestartmission_Click);
            // 
            // CMB_mountmode
            // 
            resources.ApplyResources(this.CMB_mountmode, "CMB_mountmode");
            this.CMB_mountmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_mountmode.DropDownWidth = 150;
            this.CMB_mountmode.FormattingEnabled = true;
            this.CMB_mountmode.Name = "CMB_mountmode";
            // 
            // BUT_quickrtl
            // 
            this.BUT_quickrtl.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_quickrtl.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_quickrtl.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_quickrtl, "BUT_quickrtl");
            this.BUT_quickrtl.Name = "BUT_quickrtl";
            this.BUT_quickrtl.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_quickrtl, resources.GetString("BUT_quickrtl.ToolTip"));
            this.BUT_quickrtl.UseVisualStyleBackColor = true;
            this.BUT_quickrtl.Click += new System.EventHandler(this.BUT_quickrtl_Click);
            // 
            // BUT_quickmanual
            // 
            this.BUT_quickmanual.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_quickmanual.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_quickmanual.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_quickmanual, "BUT_quickmanual");
            this.BUT_quickmanual.Name = "BUT_quickmanual";
            this.BUT_quickmanual.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_quickmanual, resources.GetString("BUT_quickmanual.ToolTip"));
            this.BUT_quickmanual.UseVisualStyleBackColor = true;
            this.BUT_quickmanual.Click += new System.EventHandler(this.BUT_quickmanual_Click);
            // 
            // BUT_setwp
            // 
            this.BUT_setwp.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_setwp.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_setwp.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_setwp, "BUT_setwp");
            this.BUT_setwp.Name = "BUT_setwp";
            this.BUT_setwp.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_setwp, resources.GetString("BUT_setwp.ToolTip"));
            this.BUT_setwp.UseVisualStyleBackColor = true;
            this.BUT_setwp.Click += new System.EventHandler(this.BUT_setwp_Click);
            // 
            // CMB_modes
            // 
            resources.ApplyResources(this.CMB_modes, "CMB_modes");
            this.CMB_modes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_modes.DropDownWidth = 150;
            this.CMB_modes.FormattingEnabled = true;
            this.CMB_modes.Name = "CMB_modes";
            this.CMB_modes.Click += new System.EventHandler(this.CMB_modes_Click);
            // 
            // BUT_quickauto
            // 
            this.BUT_quickauto.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_quickauto.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_quickauto.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_quickauto, "BUT_quickauto");
            this.BUT_quickauto.Name = "BUT_quickauto";
            this.BUT_quickauto.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_quickauto, resources.GetString("BUT_quickauto.ToolTip"));
            this.BUT_quickauto.UseVisualStyleBackColor = true;
            this.BUT_quickauto.Click += new System.EventHandler(this.BUT_quickauto_Click);
            // 
            // BUT_setmode
            // 
            this.BUT_setmode.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_setmode.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_setmode.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_setmode, "BUT_setmode");
            this.BUT_setmode.Name = "BUT_setmode";
            this.BUT_setmode.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.BUT_setmode, resources.GetString("BUT_setmode.ToolTip"));
            this.BUT_setmode.UseVisualStyleBackColor = true;
            this.BUT_setmode.Click += new System.EventHandler(this.BUT_setmode_Click);
            //
            // tabPagemessages
            //
            this.tabPagemessages.Controls.Add(this.messagesList1);
            resources.ApplyResources(this.tabPagemessages, "tabPagemessages");
            this.tabPagemessages.Name = "tabPagemessages";
            this.tabPagemessages.UseVisualStyleBackColor = true;
            //
            // messagesList1
            //
            this.messagesList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesList1.Name = "messagesList1";
            //
            // txt_messagebox
            //
            resources.ApplyResources(this.txt_messagebox, "txt_messagebox");
            this.txt_messagebox.Name = "txt_messagebox";
            //
            // tabParams
            //
            this.tabParams.Controls.Add(this.configRawParams1);
            resources.ApplyResources(this.tabParams, "tabParams");
            this.tabParams.Name = "tabParams";
            this.tabParams.UseVisualStyleBackColor = true;
            //
            // configRawParams1
            //
            resources.ApplyResources(this.configRawParams1, "configRawParams1");
            this.configRawParams1.Name = "configRawParams1";
            //
            // tabVideo
            //
            this.tabVideo.Controls.Add(this.flightPlannerVideoOptions1);
            resources.ApplyResources(this.tabVideo, "tabVideo");
            this.tabVideo.Name = "tabVideo";
            this.tabVideo.UseVisualStyleBackColor = true;
            //
            // flightPlannerVideoOptions1
            //
            resources.ApplyResources(this.flightPlannerVideoOptions1, "flightPlannerVideoOptions1");
            this.flightPlannerVideoOptions1.Name = "flightPlannerVideoOptions1";
            //
            // tabTuning
            //
            this.tabTuning.Controls.Add(this.configArduplane1);
            this.tabTuning.Controls.Add(this.configArducopter1);
            this.tabTuning.Controls.Add(this.configArdurover1);
            resources.ApplyResources(this.tabTuning, "tabTuning");
            this.tabTuning.Name = "tabTuning";
            this.tabTuning.UseVisualStyleBackColor = true;
            //
            // configArduplane1
            //
            this.configArduplane1.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.configArduplane1, "configArduplane1");
            this.configArduplane1.Name = "configArduplane1";
            this.configArduplane1.Visible = false;
            //
            // configArducopter1
            //
            this.configArducopter1.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.configArducopter1, "configArducopter1");
            this.configArducopter1.Name = "configArducopter1";
            this.configArducopter1.Visible = false;
            //
            // configArdurover1
            //
            this.configArdurover1.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.configArdurover1, "configArdurover1");
            this.configArdurover1.Name = "configArdurover1";
            this.configArdurover1.Visible = false;
            //
            // tabInspector
            //
            this.tabInspector.Controls.Add(this.mavlinkInspectorControl1);
            resources.ApplyResources(this.tabInspector, "tabInspector");
            this.tabInspector.Name = "tabInspector";
            this.tabInspector.UseVisualStyleBackColor = true;
            //
            // mavlinkInspectorControl1
            //
            this.mavlinkInspectorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.mavlinkInspectorControl1, "mavlinkInspectorControl1");
            this.mavlinkInspectorControl1.Name = "mavlinkInspectorControl1";
            //
            // tabActionsSimple
            // 
            this.tabActionsSimple.Controls.Add(this.myButton1);
            this.tabActionsSimple.Controls.Add(this.myButton2);
            this.tabActionsSimple.Controls.Add(this.myButton3);
            resources.ApplyResources(this.tabActionsSimple, "tabActionsSimple");
            this.tabActionsSimple.Name = "tabActionsSimple";
            this.tabActionsSimple.UseVisualStyleBackColor = true;
            // 
            // myButton1
            // 
            this.myButton1.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton1.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton1.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton1, "myButton1");
            this.myButton1.Name = "myButton1";
            this.myButton1.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.myButton1, resources.GetString("myButton1.ToolTip"));
            this.myButton1.UseVisualStyleBackColor = true;
            this.myButton1.Click += new System.EventHandler(this.BUT_quickmanual_Click);
            // 
            // myButton2
            // 
            this.myButton2.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton2.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton2.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton2, "myButton2");
            this.myButton2.Name = "myButton2";
            this.myButton2.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.myButton2, resources.GetString("myButton2.ToolTip"));
            this.myButton2.UseVisualStyleBackColor = true;
            this.myButton2.Click += new System.EventHandler(this.BUT_quickrtl_Click);
            // 
            // myButton3
            // 
            this.myButton3.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton3.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton3.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton3, "myButton3");
            this.myButton3.Name = "myButton3";
            this.myButton3.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.myButton3, resources.GetString("myButton3.ToolTip"));
            this.myButton3.UseVisualStyleBackColor = true;
            this.myButton3.Click += new System.EventHandler(this.BUT_quickauto_Click);
            // 
            // tabPagePreFlight
            // 
            this.tabPagePreFlight.Controls.Add(this.checkListControl1);
            resources.ApplyResources(this.tabPagePreFlight, "tabPagePreFlight");
            this.tabPagePreFlight.Name = "tabPagePreFlight";
            this.tabPagePreFlight.UseVisualStyleBackColor = true;
            // 
            // checkListControl1
            // 
            resources.ApplyResources(this.checkListControl1, "checkListControl1");
            this.checkListControl1.Name = "checkListControl1";
            // 
            // tabGauges
            // 
            this.tabGauges.Controls.Add(this.Gvspeed);
            this.tabGauges.Controls.Add(this.Gheading);
            this.tabGauges.Controls.Add(this.Galt);
            this.tabGauges.Controls.Add(this.Gspeed);
            resources.ApplyResources(this.tabGauges, "tabGauges");
            this.tabGauges.Name = "tabGauges";
            this.tabGauges.UseVisualStyleBackColor = true;
            this.tabGauges.Resize += new System.EventHandler(this.tabPage1_Resize);
            // 
            // Gvspeed
            // 
            this.Gvspeed.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.Gvspeed, "Gvspeed");
            this.Gvspeed.BaseArcColor = System.Drawing.Color.Transparent;
            this.Gvspeed.BaseArcRadius = 60;
            this.Gvspeed.BaseArcStart = 20;
            this.Gvspeed.BaseArcSweep = 320;
            this.Gvspeed.BaseArcWidth = 2;
            this.Gvspeed.Cap_Idx = ((byte)(0));
            this.Gvspeed.CapColor = System.Drawing.Color.White;
            this.Gvspeed.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.Gvspeed.CapPosition = new System.Drawing.Point(65, 85);
            this.Gvspeed.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(65, 85),
        new System.Drawing.Point(30, 55),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.Gvspeed.CapsText = new string[] {
        "VSI",
        "",
        "",
        "",
        ""};
            this.Gvspeed.CapText = "VSI";
            this.Gvspeed.Center = new System.Drawing.Point(75, 75);
            this.Gvspeed.DataBindings.Add(new System.Windows.Forms.Binding("Value0", this.bindingSourceGaugesTab, "verticalspeed", true));
            this.Gvspeed.MaxValue = 10F;
            this.Gvspeed.MinValue = -10F;
            this.Gvspeed.Name = "Gvspeed";
            this.Gvspeed.Need_Idx = ((byte)(3));
            this.Gvspeed.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.Gvspeed.NeedleColor2 = System.Drawing.Color.White;
            this.Gvspeed.NeedleEnabled = false;
            this.Gvspeed.NeedleRadius = 80;
            this.Gvspeed.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.Gvspeed.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White};
            this.Gvspeed.NeedlesEnabled = new bool[] {
        true,
        false,
        false,
        false};
            this.Gvspeed.NeedlesRadius = new int[] {
        50,
        30,
        50,
        80};
            this.Gvspeed.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.Gvspeed.NeedlesWidth = new int[] {
        2,
        2,
        2,
        2};
            this.Gvspeed.NeedleType = 0;
            this.Gvspeed.NeedleWidth = 2;
            this.Gvspeed.Range_Idx = ((byte)(0));
            this.Gvspeed.RangeColor = System.Drawing.Color.LightGreen;
            this.Gvspeed.RangeEnabled = false;
            this.Gvspeed.RangeEndValue = 360F;
            this.Gvspeed.RangeInnerRadius = 1;
            this.Gvspeed.RangeOuterRadius = 60;
            this.Gvspeed.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.LightGreen,
        System.Drawing.Color.Red,
        System.Drawing.Color.Orange,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.Gvspeed.RangesEnabled = new bool[] {
        false,
        false,
        false,
        false,
        false};
            this.Gvspeed.RangesEndValue = new float[] {
        360F,
        200F,
        150F,
        0F,
        0F};
            this.Gvspeed.RangesInnerRadius = new int[] {
        1,
        1,
        1,
        70,
        70};
            this.Gvspeed.RangesOuterRadius = new int[] {
        60,
        60,
        60,
        80,
        80};
            this.Gvspeed.RangesStartValue = new float[] {
        0F,
        150F,
        75F,
        0F,
        0F};
            this.Gvspeed.RangeStartValue = 0F;
            this.Gvspeed.ScaleLinesInterColor = System.Drawing.Color.White;
            this.Gvspeed.ScaleLinesInterInnerRadius = 52;
            this.Gvspeed.ScaleLinesInterOuterRadius = 60;
            this.Gvspeed.ScaleLinesInterWidth = 1;
            this.Gvspeed.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.Gvspeed.ScaleLinesMajorInnerRadius = 50;
            this.Gvspeed.ScaleLinesMajorOuterRadius = 60;
            this.Gvspeed.ScaleLinesMajorStepValue = 2F;
            this.Gvspeed.ScaleLinesMajorWidth = 2;
            this.Gvspeed.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.Gvspeed.ScaleLinesMinorInnerRadius = 55;
            this.Gvspeed.ScaleLinesMinorNumOf = 9;
            this.Gvspeed.ScaleLinesMinorOuterRadius = 60;
            this.Gvspeed.ScaleLinesMinorWidth = 1;
            this.Gvspeed.ScaleNumbersColor = System.Drawing.Color.White;
            this.Gvspeed.ScaleNumbersFormat = "";
            this.Gvspeed.ScaleNumbersRadius = 42;
            this.Gvspeed.ScaleNumbersRotation = 0;
            this.Gvspeed.ScaleNumbersStartScaleLine = 1;
            this.Gvspeed.ScaleNumbersStepScaleLines = 1;
            this.Gvspeed.Value = 0F;
            this.Gvspeed.Value0 = 0F;
            this.Gvspeed.Value1 = 0F;
            this.Gvspeed.Value2 = 0F;
            this.Gvspeed.Value3 = 0F;
            // 
            // bindingSourceGaugesTab
            // 
            this.bindingSourceGaugesTab.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // Gheading
            // 
            this.Gheading.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.Gheading, "Gheading");
            this.Gheading.DataBindings.Add(new System.Windows.Forms.Binding("Heading", this.bindingSourceGaugesTab, "yaw", true));
            this.Gheading.DataBindings.Add(new System.Windows.Forms.Binding("NavHeading", this.bindingSourceGaugesTab, "nav_bearing", true));
            this.Gheading.Heading = 0;
            this.Gheading.Name = "Gheading";
            this.Gheading.NavHeading = 0;
            // 
            // Galt
            // 
            this.Galt.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.Galt, "Galt");
            this.Galt.BaseArcColor = System.Drawing.Color.Transparent;
            this.Galt.BaseArcRadius = 60;
            this.Galt.BaseArcStart = 270;
            this.Galt.BaseArcSweep = 360;
            this.Galt.BaseArcWidth = 2;
            this.Galt.Cap_Idx = ((byte)(0));
            this.Galt.CapColor = System.Drawing.Color.White;
            this.Galt.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.Galt.CapPosition = new System.Drawing.Point(68, 85);
            this.Galt.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(68, 85),
        new System.Drawing.Point(30, 55),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.Galt.CapsText = new string[] {
        "Alt",
        "",
        "",
        "",
        ""};
            this.Galt.CapText = "Alt";
            this.Galt.Center = new System.Drawing.Point(75, 75);
            this.Galt.DataBindings.Add(new System.Windows.Forms.Binding("Value0", this.bindingSourceGaugesTab, "altd100", true));
            this.Galt.DataBindings.Add(new System.Windows.Forms.Binding("Value1", this.bindingSourceGaugesTab, "altd1000", true));
            this.Galt.DataBindings.Add(new System.Windows.Forms.Binding("Value2", this.bindingSourceGaugesTab, "targetaltd100", true));
            this.Galt.MaxValue = 9.99F;
            this.Galt.MinValue = 0F;
            this.Galt.Name = "Galt";
            this.Galt.Need_Idx = ((byte)(3));
            this.Galt.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.Galt.NeedleColor2 = System.Drawing.Color.White;
            this.Galt.NeedleEnabled = false;
            this.Galt.NeedleRadius = 80;
            this.Galt.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Red,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.Galt.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White};
            this.Galt.NeedlesEnabled = new bool[] {
        true,
        true,
        true,
        false};
            this.Galt.NeedlesRadius = new int[] {
        50,
        30,
        50,
        80};
            this.Galt.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.Galt.NeedlesWidth = new int[] {
        2,
        2,
        2,
        2};
            this.Galt.NeedleType = 0;
            this.Galt.NeedleWidth = 2;
            this.Galt.Range_Idx = ((byte)(0));
            this.Galt.RangeColor = System.Drawing.Color.LightGreen;
            this.Galt.RangeEnabled = false;
            this.Galt.RangeEndValue = 360F;
            this.Galt.RangeInnerRadius = 1;
            this.Galt.RangeOuterRadius = 60;
            this.Galt.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.LightGreen,
        System.Drawing.Color.Red,
        System.Drawing.Color.Orange,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.Galt.RangesEnabled = new bool[] {
        false,
        false,
        false,
        false,
        false};
            this.Galt.RangesEndValue = new float[] {
        360F,
        200F,
        150F,
        0F,
        0F};
            this.Galt.RangesInnerRadius = new int[] {
        1,
        1,
        1,
        70,
        70};
            this.Galt.RangesOuterRadius = new int[] {
        60,
        60,
        60,
        80,
        80};
            this.Galt.RangesStartValue = new float[] {
        0F,
        150F,
        75F,
        0F,
        0F};
            this.Galt.RangeStartValue = 0F;
            this.Galt.ScaleLinesInterColor = System.Drawing.Color.White;
            this.Galt.ScaleLinesInterInnerRadius = 52;
            this.Galt.ScaleLinesInterOuterRadius = 60;
            this.Galt.ScaleLinesInterWidth = 1;
            this.Galt.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.Galt.ScaleLinesMajorInnerRadius = 50;
            this.Galt.ScaleLinesMajorOuterRadius = 60;
            this.Galt.ScaleLinesMajorStepValue = 1F;
            this.Galt.ScaleLinesMajorWidth = 2;
            this.Galt.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.Galt.ScaleLinesMinorInnerRadius = 55;
            this.Galt.ScaleLinesMinorNumOf = 9;
            this.Galt.ScaleLinesMinorOuterRadius = 60;
            this.Galt.ScaleLinesMinorWidth = 1;
            this.Galt.ScaleNumbersColor = System.Drawing.Color.White;
            this.Galt.ScaleNumbersFormat = "";
            this.Galt.ScaleNumbersRadius = 42;
            this.Galt.ScaleNumbersRotation = 0;
            this.Galt.ScaleNumbersStartScaleLine = 1;
            this.Galt.ScaleNumbersStepScaleLines = 1;
            this.Galt.Value = 0F;
            this.Galt.Value0 = 0F;
            this.Galt.Value1 = 0F;
            this.Galt.Value2 = 0F;
            this.Galt.Value3 = 0F;
            // 
            // Gspeed
            // 
            this.Gspeed.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.Gspeed, "Gspeed");
            this.Gspeed.BaseArcColor = System.Drawing.Color.Transparent;
            this.Gspeed.BaseArcRadius = 70;
            this.Gspeed.BaseArcStart = 135;
            this.Gspeed.BaseArcSweep = 270;
            this.Gspeed.BaseArcWidth = 2;
            this.Gspeed.Cap_Idx = ((byte)(0));
            this.Gspeed.CapColor = System.Drawing.Color.White;
            this.Gspeed.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.Gspeed.CapPosition = new System.Drawing.Point(58, 85);
            this.Gspeed.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(58, 85),
        new System.Drawing.Point(50, 110),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.Gspeed.CapsText = new string[] {
        "Speed",
        "",
        "",
        "",
        ""};
            this.Gspeed.CapText = "Speed";
            this.Gspeed.Center = new System.Drawing.Point(75, 75);
            this.Gspeed.DataBindings.Add(new System.Windows.Forms.Binding("Value0", this.bindingSourceGaugesTab, "airspeed", true));
            this.Gspeed.DataBindings.Add(new System.Windows.Forms.Binding("Value1", this.bindingSourceGaugesTab, "groundspeed", true));
            this.Gspeed.MaxValue = 60F;
            this.Gspeed.MinValue = 0F;
            this.Gspeed.Name = "Gspeed";
            this.Gspeed.Need_Idx = ((byte)(3));
            this.Gspeed.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.Gspeed.NeedleColor2 = System.Drawing.Color.Brown;
            this.Gspeed.NeedleEnabled = false;
            this.Gspeed.NeedleRadius = 70;
            this.Gspeed.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Red,
        AGaugeApp.AGauge.NeedleColorEnum.Blue,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.Gspeed.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Brown};
            this.Gspeed.NeedlesEnabled = new bool[] {
        true,
        true,
        false,
        false};
            this.Gspeed.NeedlesRadius = new int[] {
        50,
        50,
        70,
        70};
            this.Gspeed.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.Gspeed.NeedlesWidth = new int[] {
        2,
        1,
        2,
        2};
            this.Gspeed.NeedleType = 0;
            this.Gspeed.NeedleWidth = 2;
            this.Gspeed.Range_Idx = ((byte)(2));
            this.Gspeed.RangeColor = System.Drawing.Color.Orange;
            this.Gspeed.RangeEnabled = false;
            this.Gspeed.RangeEndValue = 50F;
            this.Gspeed.RangeInnerRadius = 1;
            this.Gspeed.RangeOuterRadius = 70;
            this.Gspeed.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.LightGreen,
        System.Drawing.Color.Red,
        System.Drawing.Color.Orange,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.Gspeed.RangesEnabled = new bool[] {
        false,
        false,
        false,
        false,
        false};
            this.Gspeed.RangesEndValue = new float[] {
        35F,
        60F,
        50F,
        0F,
        0F};
            this.Gspeed.RangesInnerRadius = new int[] {
        1,
        1,
        1,
        70,
        70};
            this.Gspeed.RangesOuterRadius = new int[] {
        70,
        70,
        70,
        80,
        80};
            this.Gspeed.RangesStartValue = new float[] {
        0F,
        50F,
        35F,
        0F,
        0F};
            this.Gspeed.RangeStartValue = 35F;
            this.Gspeed.ScaleLinesInterColor = System.Drawing.Color.White;
            this.Gspeed.ScaleLinesInterInnerRadius = 52;
            this.Gspeed.ScaleLinesInterOuterRadius = 60;
            this.Gspeed.ScaleLinesInterWidth = 1;
            this.Gspeed.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.Gspeed.ScaleLinesMajorInnerRadius = 50;
            this.Gspeed.ScaleLinesMajorOuterRadius = 60;
            this.Gspeed.ScaleLinesMajorStepValue = 10F;
            this.Gspeed.ScaleLinesMajorWidth = 2;
            this.Gspeed.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.Gspeed.ScaleLinesMinorInnerRadius = 55;
            this.Gspeed.ScaleLinesMinorNumOf = 9;
            this.Gspeed.ScaleLinesMinorOuterRadius = 60;
            this.Gspeed.ScaleLinesMinorWidth = 1;
            this.Gspeed.ScaleNumbersColor = System.Drawing.Color.White;
            this.Gspeed.ScaleNumbersFormat = null;
            this.Gspeed.ScaleNumbersRadius = 42;
            this.Gspeed.ScaleNumbersRotation = 0;
            this.Gspeed.ScaleNumbersStartScaleLine = 1;
            this.Gspeed.ScaleNumbersStepScaleLines = 1;
            this.toolTip1.SetToolTip(this.Gspeed, resources.GetString("Gspeed.ToolTip"));
            this.Gspeed.Value = 0F;
            this.Gspeed.Value0 = 0F;
            this.Gspeed.Value1 = 0F;
            this.Gspeed.Value2 = 0F;
            this.Gspeed.Value3 = 0F;
            this.Gspeed.DoubleClick += new System.EventHandler(this.Gspeed_DoubleClick);
            // 
            // tabTransponder
            // 
            resources.ApplyResources(this.tabTransponder, "tabTransponder");
            this.tabTransponder.Controls.Add(this.NACp_tb);
            this.tabTransponder.Controls.Add(this.NIC_tb);
            this.tabTransponder.Controls.Add(this.NACp_lbl);
            this.tabTransponder.Controls.Add(this.NIC_lbl);
            this.tabTransponder.Controls.Add(this.Squawk_nud);
            this.tabTransponder.Controls.Add(this.FlightID_tb);
            this.tabTransponder.Controls.Add(this.fault_clb);
            this.tabTransponder.Controls.Add(this.XPDRConnect_btn);
            this.tabTransponder.Controls.Add(this.Squawk_label);
            this.tabTransponder.Controls.Add(this.FlightID_label);
            this.tabTransponder.Controls.Add(this.IDENT_btn);
            this.tabTransponder.Controls.Add(this.ALT_btn);
            this.tabTransponder.Controls.Add(this.STBY_btn);
            this.tabTransponder.Controls.Add(this.ON_btn);
            this.tabTransponder.Controls.Add(this.Mode_clb);
            this.tabTransponder.Name = "tabTransponder";
            this.tabTransponder.UseVisualStyleBackColor = true;
            // 
            // NACp_tb
            // 
            resources.ApplyResources(this.NACp_tb, "NACp_tb");
            this.NACp_tb.Name = "NACp_tb";
            this.NACp_tb.ReadOnly = true;
            // 
            // NIC_tb
            // 
            resources.ApplyResources(this.NIC_tb, "NIC_tb");
            this.NIC_tb.Name = "NIC_tb";
            this.NIC_tb.ReadOnly = true;
            // 
            // NACp_lbl
            // 
            resources.ApplyResources(this.NACp_lbl, "NACp_lbl");
            this.NACp_lbl.Name = "NACp_lbl";
            // 
            // NIC_lbl
            // 
            resources.ApplyResources(this.NIC_lbl, "NIC_lbl");
            this.NIC_lbl.Name = "NIC_lbl";
            // 
            // Squawk_nud
            // 
            resources.ApplyResources(this.Squawk_nud, "Squawk_nud");
            this.Squawk_nud.Maximum = new decimal(new int[] {
            7777,
            0,
            0,
            0});
            this.Squawk_nud.Name = "Squawk_nud";
            this.Squawk_nud.Value = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.Squawk_nud.ValueChanged += new System.EventHandler(this.Squawk_nud_ValueChanged);
            this.Squawk_nud.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Squawk_nud_MouseWheel);
            // 
            // FlightID_tb
            // 
            this.FlightID_tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            resources.ApplyResources(this.FlightID_tb, "FlightID_tb");
            this.FlightID_tb.Name = "FlightID_tb";
            this.FlightID_tb.TextChanged += new System.EventHandler(this.FlightID_tb_TextChanged);
            // 
            // fault_clb
            // 
            resources.ApplyResources(this.fault_clb, "fault_clb");
            this.fault_clb.FormattingEnabled = true;
            this.fault_clb.Items.AddRange(new object[] {
            resources.GetString("fault_clb.Items"),
            resources.GetString("fault_clb.Items1"),
            resources.GetString("fault_clb.Items2"),
            resources.GetString("fault_clb.Items3"),
            resources.GetString("fault_clb.Items4")});
            this.fault_clb.Name = "fault_clb";
            // 
            // XPDRConnect_btn
            // 
            resources.ApplyResources(this.XPDRConnect_btn, "XPDRConnect_btn");
            this.XPDRConnect_btn.Name = "XPDRConnect_btn";
            this.XPDRConnect_btn.UseVisualStyleBackColor = true;
            this.XPDRConnect_btn.Click += new System.EventHandler(this.XPDRConnect_btn_Click);
            // 
            // Squawk_label
            // 
            resources.ApplyResources(this.Squawk_label, "Squawk_label");
            this.Squawk_label.Name = "Squawk_label";
            // 
            // FlightID_label
            // 
            resources.ApplyResources(this.FlightID_label, "FlightID_label");
            this.FlightID_label.Name = "FlightID_label";
            // 
            // IDENT_btn
            // 
            resources.ApplyResources(this.IDENT_btn, "IDENT_btn");
            this.IDENT_btn.Name = "IDENT_btn";
            this.IDENT_btn.UseVisualStyleBackColor = true;
            this.IDENT_btn.Click += new System.EventHandler(this.IDENT_btn_Click);
            // 
            // ALT_btn
            // 
            resources.ApplyResources(this.ALT_btn, "ALT_btn");
            this.ALT_btn.Name = "ALT_btn";
            this.ALT_btn.UseVisualStyleBackColor = true;
            this.ALT_btn.Click += new System.EventHandler(this.ALT_btn_Click);
            // 
            // STBY_btn
            // 
            resources.ApplyResources(this.STBY_btn, "STBY_btn");
            this.STBY_btn.Name = "STBY_btn";
            this.STBY_btn.UseVisualStyleBackColor = true;
            this.STBY_btn.Click += new System.EventHandler(this.STBY_btn_Click);
            // 
            // ON_btn
            // 
            resources.ApplyResources(this.ON_btn, "ON_btn");
            this.ON_btn.Name = "ON_btn";
            this.ON_btn.UseVisualStyleBackColor = true;
            this.ON_btn.Click += new System.EventHandler(this.ON_btn_Click);
            // 
            // Mode_clb
            // 
            this.Mode_clb.CheckOnClick = true;
            resources.ApplyResources(this.Mode_clb, "Mode_clb");
            this.Mode_clb.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Mode_clb.FormattingEnabled = true;
            this.Mode_clb.Items.AddRange(new object[] {
            resources.GetString("Mode_clb.Items"),
            resources.GetString("Mode_clb.Items1"),
            resources.GetString("Mode_clb.Items2"),
            resources.GetString("Mode_clb.Items3")});
            this.Mode_clb.Name = "Mode_clb";
            //
            // tabStatus
            //
            resources.ApplyResources(this.tabStatus, "tabStatus");
            this.tabStatus.Name = "tabStatus";
            // 
            // tabServo
            // 
            this.tabServo.Controls.Add(this.flowLayoutPanelServos);
            resources.ApplyResources(this.tabServo, "tabServo");
            this.tabServo.Name = "tabServo";
            this.tabServo.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelServos
            // 
            resources.ApplyResources(this.flowLayoutPanelServos, "flowLayoutPanelServos");
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions1);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions2);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions3);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions4);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions5);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions6);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions7);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions8);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions9);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions10);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions11);
            this.flowLayoutPanelServos.Controls.Add(this.servoOptions12);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions1);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions2);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions3);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions4);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions5);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions6);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions7);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions8);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions9);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions10);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions11);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions12);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions13);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions14);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions15);
            this.flowLayoutPanelServos.Controls.Add(this.relayOptions16);
            this.flowLayoutPanelServos.Name = "flowLayoutPanelServos";
            // 
            // servoOptions1
            // 
            resources.ApplyResources(this.servoOptions1, "servoOptions1");
            this.servoOptions1.Name = "servoOptions1";
            this.servoOptions1.thisservo = 5;
            // 
            // servoOptions2
            // 
            resources.ApplyResources(this.servoOptions2, "servoOptions2");
            this.servoOptions2.Name = "servoOptions2";
            this.servoOptions2.thisservo = 6;
            // 
            // servoOptions3
            // 
            resources.ApplyResources(this.servoOptions3, "servoOptions3");
            this.servoOptions3.Name = "servoOptions3";
            this.servoOptions3.thisservo = 7;
            // 
            // servoOptions4
            // 
            resources.ApplyResources(this.servoOptions4, "servoOptions4");
            this.servoOptions4.Name = "servoOptions4";
            this.servoOptions4.thisservo = 8;
            // 
            // servoOptions5
            // 
            resources.ApplyResources(this.servoOptions5, "servoOptions5");
            this.servoOptions5.Name = "servoOptions5";
            this.servoOptions5.thisservo = 9;
            // 
            // servoOptions6
            // 
            resources.ApplyResources(this.servoOptions6, "servoOptions6");
            this.servoOptions6.Name = "servoOptions6";
            this.servoOptions6.thisservo = 10;
            // 
            // servoOptions7
            // 
            resources.ApplyResources(this.servoOptions7, "servoOptions7");
            this.servoOptions7.Name = "servoOptions7";
            this.servoOptions7.thisservo = 11;
            // 
            // servoOptions8
            // 
            resources.ApplyResources(this.servoOptions8, "servoOptions8");
            this.servoOptions8.Name = "servoOptions8";
            this.servoOptions8.thisservo = 12;
            // 
            // servoOptions9
            // 
            resources.ApplyResources(this.servoOptions9, "servoOptions9");
            this.servoOptions9.Name = "servoOptions9";
            this.servoOptions9.thisservo = 13;
            // 
            // servoOptions10
            // 
            resources.ApplyResources(this.servoOptions10, "servoOptions10");
            this.servoOptions10.Name = "servoOptions10";
            this.servoOptions10.thisservo = 14;
            // 
            // servoOptions11
            // 
            resources.ApplyResources(this.servoOptions11, "servoOptions11");
            this.servoOptions11.Name = "servoOptions11";
            this.servoOptions11.thisservo = 15;
            // 
            // servoOptions12
            // 
            resources.ApplyResources(this.servoOptions12, "servoOptions12");
            this.servoOptions12.Name = "servoOptions12";
            this.servoOptions12.thisservo = 16;
            // 
            // relayOptions1
            // 
            resources.ApplyResources(this.relayOptions1, "relayOptions1");
            this.relayOptions1.Name = "relayOptions1";
            this.relayOptions1.thisrelay = 0;
            // 
            // relayOptions2
            // 
            resources.ApplyResources(this.relayOptions2, "relayOptions2");
            this.relayOptions2.Name = "relayOptions2";
            this.relayOptions2.thisrelay = 1;
            // 
            // relayOptions3
            // 
            resources.ApplyResources(this.relayOptions3, "relayOptions3");
            this.relayOptions3.Name = "relayOptions3";
            this.relayOptions3.thisrelay = 2;
            // 
            // relayOptions4
            // 
            resources.ApplyResources(this.relayOptions4, "relayOptions4");
            this.relayOptions4.Name = "relayOptions4";
            this.relayOptions4.thisrelay = 3;
            // 
            // relayOptions5
            // 
            resources.ApplyResources(this.relayOptions5, "relayOptions5");
            this.relayOptions5.Name = "relayOptions5";
            this.relayOptions5.thisrelay = 4;
            // 
            // relayOptions6
            // 
            resources.ApplyResources(this.relayOptions6, "relayOptions6");
            this.relayOptions6.Name = "relayOptions6";
            this.relayOptions6.thisrelay = 5;
            // 
            // relayOptions7
            // 
            resources.ApplyResources(this.relayOptions7, "relayOptions7");
            this.relayOptions7.Name = "relayOptions7";
            this.relayOptions7.thisrelay = 6;
            // 
            // relayOptions8
            // 
            resources.ApplyResources(this.relayOptions8, "relayOptions8");
            this.relayOptions8.Name = "relayOptions8";
            this.relayOptions8.thisrelay = 7;
            // 
            // relayOptions9
            // 
            resources.ApplyResources(this.relayOptions9, "relayOptions9");
            this.relayOptions9.Name = "relayOptions9";
            this.relayOptions9.thisrelay = 8;
            // 
            // relayOptions10
            // 
            resources.ApplyResources(this.relayOptions10, "relayOptions10");
            this.relayOptions10.Name = "relayOptions10";
            this.relayOptions10.thisrelay = 9;
            // 
            // relayOptions11
            // 
            resources.ApplyResources(this.relayOptions11, "relayOptions11");
            this.relayOptions11.Name = "relayOptions11";
            this.relayOptions11.thisrelay = 10;
            // 
            // relayOptions12
            // 
            resources.ApplyResources(this.relayOptions12, "relayOptions12");
            this.relayOptions12.Name = "relayOptions12";
            this.relayOptions12.thisrelay = 11;
            // 
            // relayOptions13
            // 
            resources.ApplyResources(this.relayOptions13, "relayOptions13");
            this.relayOptions13.Name = "relayOptions13";
            this.relayOptions13.thisrelay = 12;
            // 
            // relayOptions14
            // 
            resources.ApplyResources(this.relayOptions14, "relayOptions14");
            this.relayOptions14.Name = "relayOptions14";
            this.relayOptions14.thisrelay = 13;
            // 
            // relayOptions15
            // 
            resources.ApplyResources(this.relayOptions15, "relayOptions15");
            this.relayOptions15.Name = "relayOptions15";
            this.relayOptions15.thisrelay = 14;
            // 
            // relayOptions16
            // 
            resources.ApplyResources(this.relayOptions16, "relayOptions16");
            this.relayOptions16.Name = "relayOptions16";
            this.relayOptions16.thisrelay = 15;
            // 
            // tabAuxFunction
            // 
            this.tabAuxFunction.Controls.Add(this.flowLayoutPanel1);
            resources.ApplyResources(this.tabAuxFunction, "tabAuxFunction");
            this.tabAuxFunction.Name = "tabAuxFunction";
            this.tabAuxFunction.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.auxOptions1);
            this.flowLayoutPanel1.Controls.Add(this.auxOptions2);
            this.flowLayoutPanel1.Controls.Add(this.auxOptions3);
            this.flowLayoutPanel1.Controls.Add(this.auxOptions4);
            this.flowLayoutPanel1.Controls.Add(this.auxOptions5);
            this.flowLayoutPanel1.Controls.Add(this.auxOptions6);
            this.flowLayoutPanel1.Controls.Add(this.auxOptions7);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // auxOptions1
            // 
            resources.ApplyResources(this.auxOptions1, "auxOptions1");
            this.auxOptions1.Name = "auxOptions1";
            // 
            // auxOptions2
            // 
            resources.ApplyResources(this.auxOptions2, "auxOptions2");
            this.auxOptions2.Name = "auxOptions2";
            // 
            // auxOptions3
            // 
            resources.ApplyResources(this.auxOptions3, "auxOptions3");
            this.auxOptions3.Name = "auxOptions3";
            // 
            // auxOptions4
            // 
            resources.ApplyResources(this.auxOptions4, "auxOptions4");
            this.auxOptions4.Name = "auxOptions4";
            // 
            // auxOptions5
            // 
            resources.ApplyResources(this.auxOptions5, "auxOptions5");
            this.auxOptions5.Name = "auxOptions5";
            // 
            // auxOptions6
            // 
            resources.ApplyResources(this.auxOptions6, "auxOptions6");
            this.auxOptions6.Name = "auxOptions6";
            // 
            // auxOptions7
            // 
            resources.ApplyResources(this.auxOptions7, "auxOptions7");
            this.auxOptions7.Name = "auxOptions7";
            // 
            // tabScripts
            // 
            this.tabScripts.Controls.Add(this.checkBoxRedirectOutput);
            this.tabScripts.Controls.Add(this.BUT_edit_selected);
            this.tabScripts.Controls.Add(this.labelSelectedScript);
            this.tabScripts.Controls.Add(this.BUT_run_script);
            this.tabScripts.Controls.Add(this.BUT_abort_script);
            this.tabScripts.Controls.Add(this.labelScriptStatus);
            this.tabScripts.Controls.Add(this.BUT_select_script);
            resources.ApplyResources(this.tabScripts, "tabScripts");
            this.tabScripts.Name = "tabScripts";
            this.tabScripts.UseVisualStyleBackColor = true;
            // 
            // checkBoxRedirectOutput
            // 
            resources.ApplyResources(this.checkBoxRedirectOutput, "checkBoxRedirectOutput");
            this.checkBoxRedirectOutput.Checked = true;
            this.checkBoxRedirectOutput.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRedirectOutput.Name = "checkBoxRedirectOutput";
            this.checkBoxRedirectOutput.UseVisualStyleBackColor = true;
            // 
            // BUT_edit_selected
            // 
            this.BUT_edit_selected.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_edit_selected.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_edit_selected.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_edit_selected, "BUT_edit_selected");
            this.BUT_edit_selected.Name = "BUT_edit_selected";
            this.BUT_edit_selected.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_edit_selected.UseVisualStyleBackColor = true;
            this.BUT_edit_selected.Click += new System.EventHandler(this.BUT_edit_selected_Click);
            // 
            // labelSelectedScript
            // 
            resources.ApplyResources(this.labelSelectedScript, "labelSelectedScript");
            this.labelSelectedScript.Name = "labelSelectedScript";
            // 
            // BUT_run_script
            // 
            this.BUT_run_script.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_run_script.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_run_script.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_run_script, "BUT_run_script");
            this.BUT_run_script.Name = "BUT_run_script";
            this.BUT_run_script.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_run_script.UseVisualStyleBackColor = true;
            this.BUT_run_script.Click += new System.EventHandler(this.BUT_run_script_Click);
            // 
            // BUT_abort_script
            // 
            this.BUT_abort_script.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_abort_script.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_abort_script.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_abort_script, "BUT_abort_script");
            this.BUT_abort_script.Name = "BUT_abort_script";
            this.BUT_abort_script.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_abort_script.UseVisualStyleBackColor = true;
            this.BUT_abort_script.Click += new System.EventHandler(this.BUT_abort_script_Click);
            // 
            // labelScriptStatus
            // 
            resources.ApplyResources(this.labelScriptStatus, "labelScriptStatus");
            this.labelScriptStatus.Name = "labelScriptStatus";
            // 
            // BUT_select_script
            // 
            this.BUT_select_script.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_select_script.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_select_script.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_select_script, "BUT_select_script");
            this.BUT_select_script.Name = "BUT_select_script";
            this.BUT_select_script.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_select_script.UseVisualStyleBackColor = true;
            this.BUT_select_script.Click += new System.EventHandler(this.BUT_select_script_Click);
            //
            // tabTLogs
            //
            this.tabTLogs.Controls.Add(this.tableLayoutPanelLogs);
            resources.ApplyResources(this.tabTLogs, "tabTLogs");
            this.tabTLogs.Name = "tabTLogs";
            this.tabTLogs.UseVisualStyleBackColor = true;
            //
            // tableLayoutPanelLogs
            //
            this.tableLayoutPanelLogs.ColumnCount = 2;
            this.tableLayoutPanelLogs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanelLogs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanelLogs.Controls.Add(this.grpDataflash, 0, 0);
            this.tableLayoutPanelLogs.Controls.Add(this.grpTelemetry, 1, 0);
            this.tableLayoutPanelLogs.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelLogs.AutoSize = true;
            this.tableLayoutPanelLogs.Name = "tableLayoutPanelLogs";
            this.tableLayoutPanelLogs.Padding = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanelLogs.RowCount = 1;
            this.tableLayoutPanelLogs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            //
            // grpDataflash
            //
            this.grpDataflash.Controls.Add(this.tableLayoutPanel2);
            this.grpDataflash.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpDataflash.AutoSize = true;
            this.grpDataflash.MinimumSize = new System.Drawing.Size(0, 150);
            this.grpDataflash.Name = "grpDataflash";
            this.grpDataflash.Padding = new System.Windows.Forms.Padding(6);
            this.grpDataflash.TabIndex = 0;
            this.grpDataflash.TabStop = false;
            this.grpDataflash.Text = "Dataflash Logs";
            //
            // grpTelemetry
            //
            this.grpTelemetry.Controls.Add(this.tableLayoutPaneltlogs);
            this.grpTelemetry.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTelemetry.AutoSize = true;
            this.grpTelemetry.MinimumSize = new System.Drawing.Size(0, 150);
            this.grpTelemetry.Name = "grpTelemetry";
            this.grpTelemetry.Padding = new System.Windows.Forms.Padding(6);
            this.grpTelemetry.TabIndex = 1;
            this.grpTelemetry.TabStop = false;
            this.grpTelemetry.Text = "Telemetry Logs";
            //
            // tableLayoutPaneltlogs
            //
            resources.ApplyResources(this.tableLayoutPaneltlogs, "tableLayoutPaneltlogs");
            this.tableLayoutPaneltlogs.ColumnCount = 1;
            this.tableLayoutPaneltlogs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPaneltlogs.Controls.Add(this.flowLayoutPanelTLogControls, 0, 0);
            this.tableLayoutPaneltlogs.Controls.Add(this.tableLayoutPanelTrackRow, 0, 1);
            this.tableLayoutPaneltlogs.Controls.Add(this.flowLayoutPanelSpeed, 0, 2);
            this.tableLayoutPaneltlogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPaneltlogs.MinimumSize = new System.Drawing.Size(300, 0);
            this.tableLayoutPaneltlogs.Name = "tableLayoutPaneltlogs";
            this.tableLayoutPaneltlogs.RowCount = 3;
            this.tableLayoutPaneltlogs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPaneltlogs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPaneltlogs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            //
            // flowLayoutPanelTLogControls
            //
            resources.ApplyResources(this.flowLayoutPanelTLogControls, "flowLayoutPanelTLogControls");
            this.flowLayoutPanelTLogControls.AutoSize = true;
            this.flowLayoutPanelTLogControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelTLogControls.Controls.Add(this.BUT_loadtelem);
            this.flowLayoutPanelTLogControls.Controls.Add(this.BUT_log2kml);
            this.flowLayoutPanelTLogControls.Controls.Add(this.LBL_logfn);
            this.flowLayoutPanelTLogControls.Name = "flowLayoutPanelTLogControls";
            this.flowLayoutPanelTLogControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanelTLogControls.WrapContents = false;
            //
            // tableLayoutPanelTrackRow
            //
            resources.ApplyResources(this.tableLayoutPanelTrackRow, "tableLayoutPanelTrackRow");
            this.tableLayoutPanelTrackRow.AutoSize = true;
            this.tableLayoutPanelTrackRow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelTrackRow.ColumnCount = 3;
            this.tableLayoutPanelTrackRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTrackRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTrackRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTrackRow.Controls.Add(this.BUT_playlog, 0, 0);
            this.tableLayoutPanelTrackRow.Controls.Add(this.tracklog, 1, 0);
            this.tableLayoutPanelTrackRow.Controls.Add(this.lbl_logpercent, 2, 0);
            this.tableLayoutPanelTrackRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelTrackRow.Name = "tableLayoutPanelTrackRow";
            this.tableLayoutPanelTrackRow.RowCount = 1;
            this.tableLayoutPanelTrackRow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // 
            // tracklog
            // 
            resources.ApplyResources(this.tracklog, "tracklog");
            this.tracklog.Maximum = 100;
            this.tracklog.Name = "tracklog";
            this.tracklog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tracklog.TickFrequency = 5;
            this.tracklog.Scroll += new System.EventHandler(this.tracklog_Scroll);
            //
            // lbl_logpercent
            //
            resources.ApplyResources(this.lbl_logpercent, "lbl_logpercent");
            this.lbl_logpercent.Name = "lbl_logpercent";
            this.lbl_logpercent.MinimumSize = new System.Drawing.Size(46, 0);
            this.lbl_logpercent.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            // 
            // flowLayoutPanelSpeed
            // 
            resources.ApplyResources(this.flowLayoutPanelSpeed, "flowLayoutPanelSpeed");
            this.flowLayoutPanelSpeed.AutoSize = true;
            this.flowLayoutPanelSpeed.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelSpeed.Controls.Add(this.label2);
            this.flowLayoutPanelSpeed.Controls.Add(this.comboPlaybackSpeed);
            this.flowLayoutPanelSpeed.Controls.Add(this.lbl_playbackspeed);
            this.flowLayoutPanelSpeed.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanelSpeed.Name = "flowLayoutPanelSpeed";
            this.flowLayoutPanelSpeed.WrapContents = false;
            //
            // label2
            //
            resources.ApplyResources(this.label2, "label2");
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.label2.Name = "label2";
            //
            // comboPlaybackSpeed
            //
            resources.ApplyResources(this.comboPlaybackSpeed, "comboPlaybackSpeed");
            this.comboPlaybackSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPlaybackSpeed.FormattingEnabled = true;
            this.comboPlaybackSpeed.Margin = new System.Windows.Forms.Padding(3, 16, 3, 0);
            this.comboPlaybackSpeed.Name = "comboPlaybackSpeed";
            this.comboPlaybackSpeed.SelectedIndexChanged += new System.EventHandler(this.comboPlaybackSpeed_SelectedIndexChanged);
            // 
            // BUT_loadtelem
            // 
            this.BUT_loadtelem.AutoSize = true;
            this.BUT_loadtelem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BUT_loadtelem.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_loadtelem.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_loadtelem.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_loadtelem, "BUT_loadtelem");
            this.BUT_loadtelem.Name = "BUT_loadtelem";
            this.BUT_loadtelem.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_loadtelem.UseVisualStyleBackColor = true;
            this.BUT_loadtelem.Click += new System.EventHandler(this.BUT_loadtelem_Click);
            //
            // lbl_playbackspeed
            //
            resources.ApplyResources(this.lbl_playbackspeed, "lbl_playbackspeed");
            this.lbl_playbackspeed.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.lbl_playbackspeed.Name = "lbl_playbackspeed";
            // 
            // LBL_logfn
            // 
            resources.ApplyResources(this.LBL_logfn, "LBL_logfn");
            this.LBL_logfn.Name = "LBL_logfn";
            // 
            // BUT_log2kml
            // 
            this.BUT_log2kml.AutoSize = true;
            this.BUT_log2kml.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BUT_log2kml.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_log2kml.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_log2kml.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_log2kml, "BUT_log2kml");
            this.BUT_log2kml.Name = "BUT_log2kml";
            this.BUT_log2kml.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_log2kml.UseVisualStyleBackColor = true;
            this.BUT_log2kml.Click += new System.EventHandler(this.BUT_log2kml_Click);
            // 
            // BUT_playlog
            // 
            this.BUT_playlog.AutoSize = true;
            this.BUT_playlog.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BUT_playlog.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_playlog.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_playlog.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_playlog, "BUT_playlog");
            this.BUT_playlog.Name = "BUT_playlog";
            this.BUT_playlog.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_playlog.UseVisualStyleBackColor = true;
            this.BUT_playlog.Click += new System.EventHandler(this.BUT_playlog_Click);
            //
            // tableLayoutPanel2
            //
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel2.Controls.Add(this.BUT_DFMavlink, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.BUT_logbrowse, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.BUT_loganalysis, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.but_dflogtokml, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.but_bintolog, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.BUT_matlab, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.BUT_georefimage, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            //
            // BUT_DFMavlink
            //
            this.BUT_DFMavlink.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_DFMavlink.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_DFMavlink.ColorNotEnabled = System.Drawing.Color.Empty;
            this.BUT_DFMavlink.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.BUT_DFMavlink, "BUT_DFMavlink");
            this.BUT_DFMavlink.Name = "BUT_DFMavlink";
            this.BUT_DFMavlink.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_DFMavlink.UseVisualStyleBackColor = true;
            this.BUT_DFMavlink.Click += new System.EventHandler(this.BUT_DFMavlink_Click);
            //
            // BUT_georefimage
            //
            this.BUT_georefimage.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.BUT_georefimage, "BUT_georefimage");
            this.BUT_georefimage.Name = "BUT_georefimage";
            this.BUT_georefimage.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_georefimage.Click += new System.EventHandler(this.BUT_georefimage_Click);
            //
            // BUT_logbrowse
            //
            this.BUT_logbrowse.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_logbrowse.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_logbrowse.ColorNotEnabled = System.Drawing.Color.Empty;
            this.BUT_logbrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.BUT_logbrowse, "BUT_logbrowse");
            this.BUT_logbrowse.Name = "BUT_logbrowse";
            this.BUT_logbrowse.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_logbrowse.UseVisualStyleBackColor = true;
            this.BUT_logbrowse.Click += new System.EventHandler(this.BUT_logbrowse_Click);
            //
            // BUT_matlab
            //
            this.BUT_matlab.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_matlab.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_matlab.ColorNotEnabled = System.Drawing.Color.Empty;
            this.BUT_matlab.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.BUT_matlab, "BUT_matlab");
            this.BUT_matlab.Name = "BUT_matlab";
            this.BUT_matlab.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_matlab.UseVisualStyleBackColor = true;
            this.BUT_matlab.Click += new System.EventHandler(this.BUT_matlab_Click);
            //
            // but_bintolog
            //
            this.but_bintolog.ColorMouseDown = System.Drawing.Color.Empty;
            this.but_bintolog.ColorMouseOver = System.Drawing.Color.Empty;
            this.but_bintolog.ColorNotEnabled = System.Drawing.Color.Empty;
            this.but_bintolog.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.but_bintolog, "but_bintolog");
            this.but_bintolog.Name = "but_bintolog";
            this.but_bintolog.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_bintolog.UseVisualStyleBackColor = true;
            this.but_bintolog.Click += new System.EventHandler(this.but_bintolog_Click);
            //
            // but_dflogtokml
            //
            this.but_dflogtokml.ColorMouseDown = System.Drawing.Color.Empty;
            this.but_dflogtokml.ColorMouseOver = System.Drawing.Color.Empty;
            this.but_dflogtokml.ColorNotEnabled = System.Drawing.Color.Empty;
            this.but_dflogtokml.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.but_dflogtokml, "but_dflogtokml");
            this.but_dflogtokml.Name = "but_dflogtokml";
            this.but_dflogtokml.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_dflogtokml.UseVisualStyleBackColor = true;
            this.but_dflogtokml.Click += new System.EventHandler(this.but_dflogtokml_Click);
            //
            // BUT_loganalysis
            //
            this.BUT_loganalysis.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_loganalysis.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_loganalysis.ColorNotEnabled = System.Drawing.Color.Empty;
            this.BUT_loganalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            resources.ApplyResources(this.BUT_loganalysis, "BUT_loganalysis");
            this.BUT_loganalysis.Name = "BUT_loganalysis";
            this.BUT_loganalysis.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_loganalysis.UseVisualStyleBackColor = true;
            this.BUT_loganalysis.Click += new System.EventHandler(this.BUT_loganalysis_Click);
            // 
            // panel_persistent
            // 
            resources.ApplyResources(this.panel_persistent, "panel_persistent");
            this.panel_persistent.Name = "panel_persistent";
            // 
            // tableMap
            // 
            resources.ApplyResources(this.tableMap, "tableMap");
            this.tableMap.Controls.Add(this.splitContainer1, 0, 0);
            this.tableMap.Controls.Add(this.panel1, 0, 1);
            this.tableMap.Name = "tableMap";
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            //
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            //
            // splitContainer1.Panel2
            //
            this.splitContainer1.Panel2.ContextMenuStrip = this.contextMenuStripMap;
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.but_disablejoystick);
            this.splitContainer1.Panel2.Controls.Add(this.Zoomlevel);
            this.splitContainer1.Panel2.Controls.Add(this.distanceBar1);
            this.splitContainer1.Panel2.Controls.Add(this.TRK_zoom);
            this.splitContainer1.Panel2.Controls.Add(this.windDir1);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.gMapControl1);
            this.splitContainer1.Panel2.Resize += new System.EventHandler(this.splitContainer1_Panel2_Resize);
            //
            // splitContainer2
            //
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Name = "splitContainer2";
            //
            // splitContainer2.Panel1
            //
            this.splitContainer2.Panel1.Controls.Add(this.zg1);
            //
            // splitContainer2.Panel2
            //
            this.splitContainer2.Panel2.Controls.Add(this.configRawParams2);
            //
            // zg1
            //
            resources.ApplyResources(this.zg1, "zg1");
            this.zg1.Name = "zg1";
            this.zg1.ScrollGrace = 0D;
            this.zg1.ScrollMaxX = 0D;
            this.zg1.ScrollMaxY = 0D;
            this.zg1.ScrollMaxY2 = 0D;
            this.zg1.ScrollMinX = 0D;
            this.zg1.ScrollMinY = 0D;
            this.zg1.ScrollMinY2 = 0D;
            this.zg1.DoubleClick += new System.EventHandler(this.zg1_DoubleClick);
            //
            // configRawParams2
            //
            resources.ApplyResources(this.configRawParams2, "configRawParams2");
            this.configRawParams2.Name = "configRawParams2";
            //
            // contextMenuStripMap
            // 
            this.contextMenuStripMap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goHereToolStripMenuItem,
            this.flyToHereAltToolStripMenuItem,
            this.flyToCoordsToolStripMenuItem,
            this.addPoiToolStripMenuItem,
            this.pointCameraHereToolStripMenuItem,
            this.PointCameraCoordsToolStripMenuItem1,
            this.triggerCameraToolStripMenuItem,
            this.flightPlannerToolStripMenuItem,
            this.setHomeHereToolStripMenuItem,
            this.takeOffToolStripMenuItem,
            this.onOffCameraOverlapToolStripMenuItem,
            this.jumpToTagToolStripMenuItem,
            this.gimbalVideoToolStripMenuItem});
            this.contextMenuStripMap.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStripMap, "contextMenuStripMap");
            // 
            // goHereToolStripMenuItem
            // 
            this.goHereToolStripMenuItem.Name = "goHereToolStripMenuItem";
            resources.ApplyResources(this.goHereToolStripMenuItem, "goHereToolStripMenuItem");
            this.goHereToolStripMenuItem.Click += new System.EventHandler(this.goHereToolStripMenuItem_Click);
            // 
            // flyToHereAltToolStripMenuItem
            // 
            this.flyToHereAltToolStripMenuItem.Name = "flyToHereAltToolStripMenuItem";
            resources.ApplyResources(this.flyToHereAltToolStripMenuItem, "flyToHereAltToolStripMenuItem");
            this.flyToHereAltToolStripMenuItem.Click += new System.EventHandler(this.flyToHereAltToolStripMenuItem_Click);
            // 
            // flyToCoordsToolStripMenuItem
            // 
            this.flyToCoordsToolStripMenuItem.Name = "flyToCoordsToolStripMenuItem";
            resources.ApplyResources(this.flyToCoordsToolStripMenuItem, "flyToCoordsToolStripMenuItem");
            this.flyToCoordsToolStripMenuItem.Click += new System.EventHandler(this.flyToCoordsToolStripMenuItem_Click);
            // 
            // addPoiToolStripMenuItem
            // 
            this.addPoiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.saveFileToolStripMenuItem,
            this.loadFileToolStripMenuItem,
            this.poiatcoordsToolStripMenuItem});
            this.addPoiToolStripMenuItem.Name = "addPoiToolStripMenuItem";
            resources.ApplyResources(this.addPoiToolStripMenuItem, "addPoiToolStripMenuItem");
            this.addPoiToolStripMenuItem.Click += new System.EventHandler(this.addPoiToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // saveFileToolStripMenuItem
            // 
            this.saveFileToolStripMenuItem.Name = "saveFileToolStripMenuItem";
            resources.ApplyResources(this.saveFileToolStripMenuItem, "saveFileToolStripMenuItem");
            this.saveFileToolStripMenuItem.Click += new System.EventHandler(this.saveFileToolStripMenuItem_Click);
            // 
            // loadFileToolStripMenuItem
            // 
            this.loadFileToolStripMenuItem.Name = "loadFileToolStripMenuItem";
            resources.ApplyResources(this.loadFileToolStripMenuItem, "loadFileToolStripMenuItem");
            this.loadFileToolStripMenuItem.Click += new System.EventHandler(this.loadFileToolStripMenuItem_Click);
            // 
            // poiatcoordsToolStripMenuItem
            // 
            this.poiatcoordsToolStripMenuItem.Name = "poiatcoordsToolStripMenuItem";
            resources.ApplyResources(this.poiatcoordsToolStripMenuItem, "poiatcoordsToolStripMenuItem");
            this.poiatcoordsToolStripMenuItem.Click += new System.EventHandler(this.poiatcoordsToolStripMenuItem_Click);
            // 
            // pointCameraHereToolStripMenuItem
            // 
            this.pointCameraHereToolStripMenuItem.Name = "pointCameraHereToolStripMenuItem";
            resources.ApplyResources(this.pointCameraHereToolStripMenuItem, "pointCameraHereToolStripMenuItem");
            this.pointCameraHereToolStripMenuItem.Click += new System.EventHandler(this.pointCameraHereToolStripMenuItem_Click);
            // 
            // PointCameraCoordsToolStripMenuItem1
            // 
            this.PointCameraCoordsToolStripMenuItem1.Name = "PointCameraCoordsToolStripMenuItem1";
            resources.ApplyResources(this.PointCameraCoordsToolStripMenuItem1, "PointCameraCoordsToolStripMenuItem1");
            this.PointCameraCoordsToolStripMenuItem1.Click += new System.EventHandler(this.PointCameraCoordsToolStripMenuItem1_Click);
            // 
            // triggerCameraToolStripMenuItem
            // 
            this.triggerCameraToolStripMenuItem.Name = "triggerCameraToolStripMenuItem";
            resources.ApplyResources(this.triggerCameraToolStripMenuItem, "triggerCameraToolStripMenuItem");
            this.triggerCameraToolStripMenuItem.Click += new System.EventHandler(this.triggerCameraToolStripMenuItem_Click);
            // 
            // flightPlannerToolStripMenuItem
            // 
            this.flightPlannerToolStripMenuItem.Name = "flightPlannerToolStripMenuItem";
            resources.ApplyResources(this.flightPlannerToolStripMenuItem, "flightPlannerToolStripMenuItem");
            this.flightPlannerToolStripMenuItem.Click += new System.EventHandler(this.flightPlannerToolStripMenuItem_Click);
            // 
            // setHomeHereToolStripMenuItem
            // 
            this.setHomeHereToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setEKFHomeHereToolStripMenuItem,
            this.setHomeHereToolStripMenuItem1});
            this.setHomeHereToolStripMenuItem.Name = "setHomeHereToolStripMenuItem";
            resources.ApplyResources(this.setHomeHereToolStripMenuItem, "setHomeHereToolStripMenuItem");
            // 
            // setEKFHomeHereToolStripMenuItem
            // 
            this.setEKFHomeHereToolStripMenuItem.Name = "setEKFHomeHereToolStripMenuItem";
            resources.ApplyResources(this.setEKFHomeHereToolStripMenuItem, "setEKFHomeHereToolStripMenuItem");
            this.setEKFHomeHereToolStripMenuItem.Click += new System.EventHandler(this.setEKFHomeHereToolStripMenuItem_Click);
            // 
            // setHomeHereToolStripMenuItem1
            // 
            this.setHomeHereToolStripMenuItem1.Name = "setHomeHereToolStripMenuItem1";
            resources.ApplyResources(this.setHomeHereToolStripMenuItem1, "setHomeHereToolStripMenuItem1");
            this.setHomeHereToolStripMenuItem1.Click += new System.EventHandler(this.setHomeHereToolStripMenuItem_Click);
            // 
            // takeOffToolStripMenuItem
            // 
            this.takeOffToolStripMenuItem.Name = "takeOffToolStripMenuItem";
            resources.ApplyResources(this.takeOffToolStripMenuItem, "takeOffToolStripMenuItem");
            this.takeOffToolStripMenuItem.Click += new System.EventHandler(this.takeOffToolStripMenuItem_Click);
            // 
            // onOffCameraOverlapToolStripMenuItem
            // 
            this.onOffCameraOverlapToolStripMenuItem.CheckOnClick = true;
            this.onOffCameraOverlapToolStripMenuItem.Name = "onOffCameraOverlapToolStripMenuItem";
            resources.ApplyResources(this.onOffCameraOverlapToolStripMenuItem, "onOffCameraOverlapToolStripMenuItem");
            this.onOffCameraOverlapToolStripMenuItem.Click += new System.EventHandler(this.onOffCameraOverlapToolStripMenuItem_Click);
            // 
            // jumpToTagToolStripMenuItem
            // 
            this.jumpToTagToolStripMenuItem.Name = "jumpToTagToolStripMenuItem";
            resources.ApplyResources(this.jumpToTagToolStripMenuItem, "jumpToTagToolStripMenuItem");
            this.jumpToTagToolStripMenuItem.Click += new System.EventHandler(this.jumpToTagToolStripMenuItem_Click);
            // 
            // gimbalVideoToolStripMenuItem
            // 
            this.gimbalVideoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gimbalVideoFullSizedToolStripMenuItem,
            this.gimbalVideoMiniToolStripMenuItem,
            this.gimbalVideoPopOutToolStripMenuItem});
            this.gimbalVideoToolStripMenuItem.Name = "gimbalVideoToolStripMenuItem";
            resources.ApplyResources(this.gimbalVideoToolStripMenuItem, "gimbalVideoToolStripMenuItem");
            // 
            // gimbalVideoFullSizedToolStripMenuItem
            // 
            this.gimbalVideoFullSizedToolStripMenuItem.Name = "gimbalVideoFullSizedToolStripMenuItem";
            resources.ApplyResources(this.gimbalVideoFullSizedToolStripMenuItem, "gimbalVideoFullSizedToolStripMenuItem");
            this.gimbalVideoFullSizedToolStripMenuItem.Click += new System.EventHandler(this.gimbalVideoFullSizedToolStripMenuItem_Click);
            // 
            // gimbalVideoMiniToolStripMenuItem
            // 
            this.gimbalVideoMiniToolStripMenuItem.Name = "gimbalVideoMiniToolStripMenuItem";
            resources.ApplyResources(this.gimbalVideoMiniToolStripMenuItem, "gimbalVideoMiniToolStripMenuItem");
            this.gimbalVideoMiniToolStripMenuItem.Click += new System.EventHandler(this.gimbalVideoMiniToolStripMenuItem_Click);
            // 
            // gimbalVideoPopOutToolStripMenuItem
            // 
            this.gimbalVideoPopOutToolStripMenuItem.Name = "gimbalVideoPopOutToolStripMenuItem";
            resources.ApplyResources(this.gimbalVideoPopOutToolStripMenuItem, "gimbalVideoPopOutToolStripMenuItem");
            this.gimbalVideoPopOutToolStripMenuItem.Click += new System.EventHandler(this.gimbalVideoPopOutToolStripMenuItem_Click);
            //
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // but_disablejoystick
            // 
            this.but_disablejoystick.ColorMouseDown = System.Drawing.Color.Empty;
            this.but_disablejoystick.ColorMouseOver = System.Drawing.Color.Empty;
            this.but_disablejoystick.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.but_disablejoystick, "but_disablejoystick");
            this.but_disablejoystick.Name = "but_disablejoystick";
            this.but_disablejoystick.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_disablejoystick.UseVisualStyleBackColor = true;
            this.but_disablejoystick.Click += new System.EventHandler(this.but_disablejoystick_Click);
            // 
            // Zoomlevel
            // 
            resources.ApplyResources(this.Zoomlevel, "Zoomlevel");
            this.Zoomlevel.DecimalPlaces = 1;
            this.Zoomlevel.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.Zoomlevel.Maximum = new decimal(new int[] {
            18,
            0,
            0,
            0});
            this.Zoomlevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Zoomlevel.Name = "Zoomlevel";
            this.toolTip1.SetToolTip(this.Zoomlevel, resources.GetString("Zoomlevel.ToolTip"));
            this.Zoomlevel.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.Zoomlevel.ValueChanged += new System.EventHandler(this.Zoomlevel_ValueChanged);
            // 
            // distanceBar1
            // 
            resources.ApplyResources(this.distanceBar1, "distanceBar1");
            this.distanceBar1.BackColor = System.Drawing.Color.Transparent;
            this.distanceBar1.Name = "distanceBar1";
            this.distanceBar1.totaldist = 100F;
            this.distanceBar1.traveleddist = 0F;
            // 
            // TRK_zoom
            // 
            resources.ApplyResources(this.TRK_zoom, "TRK_zoom");
            this.TRK_zoom.LargeChange = 1F;
            this.TRK_zoom.Maximum = 24F;
            this.TRK_zoom.Minimum = 1F;
            this.TRK_zoom.Name = "TRK_zoom";
            this.TRK_zoom.SmallChange = 1F;
            this.TRK_zoom.TickFrequency = 1F;
            this.TRK_zoom.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.TRK_zoom.Value = 1F;
            this.TRK_zoom.Scroll += new System.EventHandler(this.TRK_zoom_Scroll);
            // 
            // windDir1
            // 
            this.windDir1.BackColor = System.Drawing.Color.Transparent;
            this.windDir1.DataBindings.Add(new System.Windows.Forms.Binding("Direction", this.bindingSource1, "wind_dir", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.windDir1.DataBindings.Add(new System.Windows.Forms.Binding("Speed", this.bindingSource1, "wind_vel", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.windDir1.Direction = 180D;
            resources.ApplyResources(this.windDir1, "windDir1");
            this.windDir1.Name = "windDir1";
            this.windDir1.Speed = 0D;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.BackColor = System.Drawing.Color.Black;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Name = "label6";
            this.label6.Tag = "custom";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.Green;
            this.label5.Name = "label5";
            this.label5.Tag = "custom";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label3.Name = "label3";
            this.label3.Tag = "custom";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Name = "label4";
            this.label4.Tag = "custom";
            // 
            // lbl_hdop
            //
            resources.ApplyResources(this.lbl_hdop, "lbl_hdop");
            this.lbl_hdop.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource1, "gpshdopvdop", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.lbl_hdop.Name = "lbl_hdop";
            this.lbl_hdop.resize = true;
            this.lbl_hdop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_hdop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lbl_hdop, resources.GetString("lbl_hdop.ToolTip"));
            //
            // lbl_sats
            //
            resources.ApplyResources(this.lbl_sats, "lbl_sats");
            this.lbl_sats.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource1, "satcount", true, System.Windows.Forms.DataSourceUpdateMode.Never, null, "Sats: 0"));
            this.lbl_sats.Name = "lbl_sats";
            this.lbl_sats.resize = true;
            this.lbl_sats.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_sats.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lbl_sats, resources.GetString("lbl_sats.ToolTip"));
            // 
            // gMapControl1
            // 
            this.gMapControl1.BackColor = System.Drawing.Color.Black;
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.ContextMenuStrip = this.contextMenuStripMap;
            resources.ApplyResources(this.gMapControl1, "gMapControl1");
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Gray;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.HoldInvalidation = false;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 24;
            this.gMapControl1.MinZoom = 0;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Fractional;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Zoom = 3D;
            this.gMapControl1.OnPositionChanged += new GMap.NET.PositionChanged(this.gMapControl1_OnPositionChanged);
            this.gMapControl1.Click += new System.EventHandler(this.gMapControl1_Click);
            this.gMapControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseDown);
            this.gMapControl1.MouseLeave += new System.EventHandler(this.gMapControl1_MouseLeave);
            this.gMapControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseMove);
            this.gMapControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseUp);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.coords1);
            this.panel1.Controls.Add(this.CHK_autopan);
            this.panel1.Controls.Add(this.CB_tuning);
            this.panel1.Controls.Add(this.CB_params);
            this.panel1.Name = "panel1";
            // 
            // coords1
            // 
            this.coords1.Alt = 0D;
            this.coords1.AltSource = "";
            this.coords1.AltUnit = "m";
            this.coords1.DataBindings.Add(new System.Windows.Forms.Binding("Alt", this.bindingSource1, "alt", true));
            this.coords1.DataBindings.Add(new System.Windows.Forms.Binding("Lat", this.bindingSource1, "lat", true));
            this.coords1.DataBindings.Add(new System.Windows.Forms.Binding("Lng", this.bindingSource1, "lng", true));
            this.coords1.Lat = 0D;
            this.coords1.Lng = 0D;
            resources.ApplyResources(this.coords1, "coords1");
            this.coords1.Name = "coords1";
            this.coords1.Vertical = false;
            // 
            // CHK_autopan
            // 
            resources.ApplyResources(this.CHK_autopan, "CHK_autopan");
            this.CHK_autopan.Checked = true;
            this.CHK_autopan.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_autopan.Name = "CHK_autopan";
            this.toolTip1.SetToolTip(this.CHK_autopan, resources.GetString("CHK_autopan.ToolTip"));
            this.CHK_autopan.UseVisualStyleBackColor = true;
            this.CHK_autopan.CheckedChanged += new System.EventHandler(this.CHK_autopan_CheckedChanged);
            // 
            // CB_tuning
            // 
            resources.ApplyResources(this.CB_tuning, "CB_tuning");
            this.CB_tuning.Name = "CB_tuning";
            this.toolTip1.SetToolTip(this.CB_tuning, resources.GetString("CB_tuning.ToolTip"));
            this.CB_tuning.UseVisualStyleBackColor = true;
            this.CB_tuning.CheckedChanged += new System.EventHandler(this.CB_tuning_CheckedChanged);
            //
            // CB_params
            //
            resources.ApplyResources(this.CB_params, "CB_params");
            this.CB_params.Name = "CB_params";
            this.toolTip1.SetToolTip(this.CB_params, resources.GetString("CB_params.ToolTip"));
            this.CB_params.UseVisualStyleBackColor = true;
            this.CB_params.CheckedChanged += new System.EventHandler(this.CB_params_CheckedChanged);
            //
            // ZedGraphTimer
            // 
            this.ZedGraphTimer.Tick += new System.EventHandler(this.ZedGraphTimer_Tick);
            // 
            // toolTip1
            // 
            this.toolTip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.toolTip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            // 
            // openScriptDialog
            // 
            resources.ApplyResources(this.openScriptDialog, "openScriptDialog");
            // 
            // scriptChecker
            // 
            this.scriptChecker.Tick += new System.EventHandler(this.scriptChecker_Tick);
            // 
            // Messagetabtimer
            // 
            this.Messagetabtimer.Interval = 200;
            this.Messagetabtimer.Tick += new System.EventHandler(this.Messagetabtimer_Tick);
            // 
            // bindingSourceStatusTab
            // 
            this.bindingSourceStatusTab.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // FlightData
            // 
            this.Controls.Add(this.MainH);
            resources.ApplyResources(this, "$this");
            this.Name = "FlightData";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FlightData_FormClosing);
            this.Load += new System.EventHandler(this.FlightData_Load);
            this.Resize += new System.EventHandler(this.FlightData_Resize);
            this.ParentChanged += new System.EventHandler(this.FlightData_ParentChanged);
            this.MainH.Panel1.ResumeLayout(false);
            this.MainH.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainH)).EndInit();
            this.MainH.ResumeLayout(false);
            this.SubMainLeft.Panel1.ResumeLayout(false);
            this.SubMainLeft.Panel2.ResumeLayout(false);
            this.SubMainLeft.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SubMainLeft)).EndInit();
            this.SubMainLeft.ResumeLayout(false);
            this.contextMenuStripHud.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceHud)).EndInit();
            this.contextMenuStripactionstab.ResumeLayout(false);
            this.tabControlactions.ResumeLayout(false);
            this.tabQuick.ResumeLayout(false);
            this.tableLayoutPanelQuick.ResumeLayout(false);
            this.contextMenuStripQuickView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceQuickTab)).EndInit();
            this.tabActions.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabPagemessages.ResumeLayout(false);
            this.tabPagemessages.PerformLayout();
            this.tabParams.ResumeLayout(false);
            this.tabVideo.ResumeLayout(false);
            this.tabTuning.ResumeLayout(false);
            this.tabInspector.ResumeLayout(false);
            this.tabActionsSimple.ResumeLayout(false);
            this.tabPagePreFlight.ResumeLayout(false);
            this.tabGauges.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceGaugesTab)).EndInit();
            this.tabTransponder.ResumeLayout(false);
            this.tabTransponder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Squawk_nud)).EndInit();
            this.tabServo.ResumeLayout(false);
            this.flowLayoutPanelServos.ResumeLayout(false);
            this.tabAuxFunction.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tabScripts.ResumeLayout(false);
            this.tabScripts.PerformLayout();
            this.tabTLogs.ResumeLayout(false);
            this.tableLayoutPanelLogs.ResumeLayout(false);
            this.grpDataflash.ResumeLayout(false);
            this.grpTelemetry.ResumeLayout(false);
            this.tableLayoutPaneltlogs.ResumeLayout(false);
            this.tableLayoutPaneltlogs.PerformLayout();
            this.flowLayoutPanelTLogControls.ResumeLayout(false);
            this.flowLayoutPanelTLogControls.PerformLayout();
            this.tableLayoutPanelTrackRow.ResumeLayout(false);
            this.tableLayoutPanelTrackRow.PerformLayout();
            this.flowLayoutPanelSpeed.ResumeLayout(false);
            this.flowLayoutPanelSpeed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tracklog)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableMap.ResumeLayout(false);
            this.tableMap.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStripMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Zoomlevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TRK_zoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceStatusTab)).EndInit();
            this.ResumeLayout(false);

        }

  

        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Timer ZedGraphTimer;
        private System.Windows.Forms.SplitContainer MainH;
        private System.Windows.Forms.SplitContainer SubMainLeft;
        private System.Windows.Forms.ToolStripMenuItem goHereToolStripMenuItem;
        private Controls.HUD hud1;
        private Controls.MyButton BUT_clear_track;
        private System.Windows.Forms.CheckBox CB_tuning;
        private System.Windows.Forms.CheckBox CB_params;
        private Controls.MyButton BUT_RAWSensor;
        private Controls.MyButton BUTactiondo;
        private Controls.MyButton BUTrestartmission;
        private System.Windows.Forms.ComboBox CMB_action;
        private Controls.MyButton BUT_Homealt;
        private System.Windows.Forms.TrackBar tracklog;
        private Controls.MyButton BUT_playlog;
        private Controls.MyButton BUT_loadtelem;
        private AGaugeApp.AGauge Galt;
        private AGaugeApp.AGauge Gspeed;
        private AGaugeApp.AGauge Gvspeed;
        private System.Windows.Forms.TableLayoutPanel tableMap;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown Zoomlevel;
        private Label label1;
        private System.Windows.Forms.CheckBox CHK_autopan;
        public Controls.myGMAP gMapControl1;
        private ZedGraph.ZedGraphControl zg1;
        public System.Windows.Forms.TabControl tabControlactions;
        public System.Windows.Forms.TabPage tabGauges;
        public System.Windows.Forms.TabPage tabStatus;
        public System.Windows.Forms.TabPage tabActions;
        public System.Windows.Forms.TabPage tabTLogs;
        private System.Windows.Forms.ComboBox CMB_modes;
        private Controls.MyButton BUT_setmode;
        private System.Windows.Forms.ComboBox CMB_setwp;
        private Controls.MyButton BUT_setwp;
        private Controls.MyButton BUT_quickmanual;
        private Controls.MyButton BUT_quickrtl;
        private Controls.MyButton BUT_quickauto;
        private Controls.MyButton BUT_log2kml;
        private Controls.MyButton BUT_joystick;
        private System.Windows.Forms.ToolTip toolTip1;
        private Label lbl_logpercent;
        private System.Windows.Forms.ToolStripMenuItem pointCameraHereToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ConfigurationView.ConfigRawParams configRawParams2;
        private Controls.MyLabel lbl_hdop;
        private Controls.MyLabel lbl_sats;
        private Controls.HSI Gheading;
        private Label lbl_playbackspeed;
        private System.Windows.Forms.ToolStripMenuItem setAspectRatioToolStripMenuItem;
        public System.Windows.Forms.TabPage tabQuick;
        private Controls.QuickView quickView3;
        private Controls.QuickView quickView2;
        private Controls.QuickView quickView1;
        private Controls.QuickView quickView4;
        private System.Windows.Forms.ToolStripMenuItem flyToHereAltToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flightPlannerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userItemsToolStripMenuItem;
        //private Crom.Controls.Docking.DockContainer dockContainer1;
        private Controls.MyButton BUT_ARM;
        private Controls.ModifyandSet modifyandSetAlt;
        private Controls.ModifyandSet modifyandSetSpeed;
        private System.Windows.Forms.ToolStripMenuItem triggerCameraToolStripMenuItem;
        private Controls.MyTrackBar TRK_zoom;
        private Label LBL_logfn;
        public System.Windows.Forms.TabPage tabServo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelServos;
        private Controls.ServoOptions servoOptions1;
        private Controls.ServoOptions servoOptions2;
        private Controls.ServoOptions servoOptions3;
        private Controls.ServoOptions servoOptions4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPaneltlogs;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLogs;
        private System.Windows.Forms.GroupBox grpDataflash;
        private System.Windows.Forms.GroupBox grpTelemetry;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTLogControls;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTrackRow;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelSpeed;
        private Controls.ServoOptions servoOptions5;
        private Controls.ServoOptions servoOptions6;
        private Controls.ServoOptions servoOptions7;
        private Controls.ServoOptions servoOptions8;
        private Controls.ServoOptions servoOptions9;
        private Controls.ServoOptions servoOptions10;
        private Controls.ServoOptions servoOptions11;
        private Controls.ServoOptions servoOptions12;
        private System.Windows.Forms.BindingSource bindingSourceHud;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelQuick;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboPlaybackSpeed;
        private Controls.MyButton BUT_logbrowse;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TabPage tabScripts;
        private Controls.MyButton BUT_edit_selected;
        private System.Windows.Forms.Label labelSelectedScript;
        private Controls.MyButton BUT_run_script;
        private Controls.MyButton BUT_abort_script;
        private System.Windows.Forms.Label labelScriptStatus;
        private Controls.MyButton BUT_select_script;
        private System.Windows.Forms.OpenFileDialog openScriptDialog;
        private System.Windows.Forms.Timer scriptChecker;
        private System.Windows.Forms.CheckBox checkBoxRedirectOutput;
        private System.Windows.Forms.ToolStripMenuItem russianHudToolStripMenuItem;
        public System.Windows.Forms.ContextMenuStrip contextMenuStripMap;
        public System.Windows.Forms.ContextMenuStrip contextMenuStripHud;
        private System.Windows.Forms.BindingSource bindingSourceQuickTab;
        private System.Windows.Forms.BindingSource bindingSourceStatusTab;
        private System.Windows.Forms.BindingSource bindingSourceGaugesTab;
        private System.Windows.Forms.ToolStripMenuItem setHomeHereToolStripMenuItem;
        private MissionPlanner.Controls.Coords coords1;
        private Controls.MyButton BUT_matlab;
        private System.Windows.Forms.ComboBox CMB_mountmode;
        private Controls.MyButton BUT_mountmode;
        private Controls.MyButton BUT_Reboot;
        public Controls.WindDir windDir1;
        private Controls.MyButton but_bintolog;
        private Controls.MyButton but_dflogtokml;
        private Controls.MyButton BUT_DFMavlink;
        public System.Windows.Forms.TabPage tabPagemessages;
        private Controls.MessagesList messagesList1;
        private System.Windows.Forms.TextBox txt_messagebox;
        private System.Windows.Forms.Timer Messagetabtimer;
        public System.Windows.Forms.TabPage tabParams;
        private ConfigurationView.ConfigRawParams configRawParams1;
        public System.Windows.Forms.TabPage tabVideo;
        private Controls.FlightPlannerVideoOptions flightPlannerVideoOptions1;
        public System.Windows.Forms.TabPage tabTuning;
        private ConfigurationView.ConfigArduplane configArduplane1;
        private ConfigurationView.ConfigArducopter configArducopter1;
        private ConfigurationView.ConfigArdurover configArdurover1;
        public System.Windows.Forms.TabPage tabInspector;
        private Controls.MAVLinkInspectorControl mavlinkInspectorControl1;
        public System.Windows.Forms.TabPage tabActionsSimple;
        private Controls.MyButton myButton1;
        private Controls.MyButton myButton2;
        private Controls.MyButton myButton3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripactionstab;
        private Controls.MyButton BUT_loganalysis;
        private System.Windows.Forms.ToolStripMenuItem addPoiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFileToolStripMenuItem;
        private Controls.DistanceBar distanceBar1;
        private System.Windows.Forms.ToolStripMenuItem takeOffToolStripMenuItem;
        private Controls.MyButton BUT_resumemis;
        public System.Windows.Forms.TabPage tabPagePreFlight;
        private Controls.PreFlight.CheckListControl checkListControl1;
        private System.Windows.Forms.ToolStripMenuItem swapWithMapToolStripMenuItem;
        private Controls.MyButton BUT_abortland;
        private Controls.MyButton but_disablejoystick;
        private System.Windows.Forms.ToolStripMenuItem videoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recordHudToAVIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setMJPEGSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PointCameraCoordsToolStripMenuItem1;
        private Controls.ModifyandSet modifyandSetLoiterRad;
        private System.Windows.Forms.ToolStripMenuItem onOffCameraOverlapToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripQuickView;
        private System.Windows.Forms.ToolStripMenuItem editQuickViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setViewCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setGStreamerSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setEKFHomeHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setHomeHereToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem groundColorToolStripMenuItem;
        private Controls.RelayOptions relayOptions1;
        private Controls.RelayOptions relayOptions2;
        private Controls.RelayOptions relayOptions3;
        private Controls.RelayOptions relayOptions4;
        private Controls.RelayOptions relayOptions5;
        private Controls.RelayOptions relayOptions6;
        private System.Windows.Forms.ToolStripMenuItem hereLinkVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gStreamerStopToolStripMenuItem;
        private Controls.MyButton BUT_georefimage;
        private Controls.QuickView quickView6;
        private Controls.QuickView quickView5;
        private System.Windows.Forms.ToolStripMenuItem poiatcoordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flyToCoordsToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelActionsSpacer;
        private Controls.FlightDataActions flightDataActions1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolStripMenuItem setBatteryCellCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undockToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorMove;
        private System.Windows.Forms.ToolStripMenuItem moveLeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveRightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorReset;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetAllToolStripMenuItem;
        private System.Windows.Forms.Button ALT_btn;
        private System.Windows.Forms.Button STBY_btn;
        private System.Windows.Forms.Button ON_btn;
        private System.Windows.Forms.CheckedListBox Mode_clb;
        private System.Windows.Forms.NumericUpDown Squawk_nud;
        private System.Windows.Forms.Label Squawk_label;
        private System.Windows.Forms.Label FlightID_label;
        private System.Windows.Forms.TextBox FlightID_tb;
        private System.Windows.Forms.Button IDENT_btn;
        public System.Windows.Forms.TabPage tabTransponder;
        private System.Windows.Forms.Button XPDRConnect_btn;
        private System.Windows.Forms.CheckedListBox fault_clb;
        private System.Windows.Forms.Label NACp_lbl;
        private System.Windows.Forms.Label NIC_lbl;
        private System.Windows.Forms.TextBox NACp_tb;
        private System.Windows.Forms.TextBox NIC_tb;
        private ToolStripMenuItem showIconsToolStripMenuItem;
        private Controls.MyButton BUT_SendMSG;
        public Panel panel_persistent;
        public System.Windows.Forms.TabPage tabAuxFunction;
        private FlowLayoutPanel flowLayoutPanel1;
        private Controls.AuxOptions auxOptions1;
        private Controls.AuxOptions auxOptions2;
        private Controls.AuxOptions auxOptions3;
        private Controls.AuxOptions auxOptions4;
        private Controls.AuxOptions auxOptions5;
        private Controls.AuxOptions auxOptions6;
        private Controls.AuxOptions auxOptions7;
        private ToolStripMenuItem jumpToTagToolStripMenuItem;
        private Controls.RelayOptions relayOptions7;
        private Controls.RelayOptions relayOptions8;
        private Controls.RelayOptions relayOptions9;
        private Controls.RelayOptions relayOptions10;
        private Controls.RelayOptions relayOptions11;
        private Controls.RelayOptions relayOptions12;
        private Controls.RelayOptions relayOptions13;
        private Controls.RelayOptions relayOptions14;
        private Controls.RelayOptions relayOptions15;
        private Controls.RelayOptions relayOptions16;
        private ToolStripMenuItem gimbalVideoToolStripMenuItem;
        private ToolStripMenuItem gimbalVideoFullSizedToolStripMenuItem;
        private ToolStripMenuItem gimbalVideoMiniToolStripMenuItem;
        private ToolStripMenuItem gimbalVideoPopOutToolStripMenuItem;
    }
}
