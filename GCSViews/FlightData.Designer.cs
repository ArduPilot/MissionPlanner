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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.contextMenuStripactionstab = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripQuickView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setViewCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bindingSourceQuickTab = new System.Windows.Forms.BindingSource(this.components);
            this.bindingSourceGaugesTab = new System.Windows.Forms.BindingSource(this.components);
            this.bindingSourcePayloadTab = new System.Windows.Forms.BindingSource(this.components);
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.myButton10 = new MissionPlanner.Controls.MyButton();
            this.myButton9 = new MissionPlanner.Controls.MyButton();
            this.SelectWP = new System.Windows.Forms.ComboBox();
            this.myButton7 = new MissionPlanner.Controls.MyButton();
            this.myButton6 = new MissionPlanner.Controls.MyButton();
            this.myButton5 = new MissionPlanner.Controls.MyButton();
            this.myButton4 = new MissionPlanner.Controls.MyButton();
            this.tableMap = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.zg1 = new ZedGraph.ZedGraphControl();
            this.contextMenuStripMap = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.goHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pointCameraHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.distanceBar1 = new MissionPlanner.Controls.DistanceBar();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_hdop = new MissionPlanner.Controls.MyLabel();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lbl_sats = new MissionPlanner.Controls.MyLabel();
            this.gMapControl1 = new MissionPlanner.Controls.myGMAP();
            this.TRK_zoom = new MissionPlanner.Controls.MyTrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.coords1 = new MissionPlanner.Controls.Coords();
            this.Zoomlevel = new System.Windows.Forms.NumericUpDown();
            this.label1 = new MissionPlanner.Controls.MyLabel();
            this.CHK_autopan = new System.Windows.Forms.CheckBox();
            this.ZedGraphTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openScriptDialog = new System.Windows.Forms.OpenFileDialog();
            this.scriptChecker = new System.Windows.Forms.Timer(this.components);
            this.Messagetabtimer = new System.Windows.Forms.Timer(this.components);
            this.bindingSourceStatusTab = new System.Windows.Forms.BindingSource(this.components);
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.contextMenu2 = new System.Windows.Forms.ContextMenu();
            this.baseRenderer1 = new BrightIdeasSoftware.BaseRenderer();
            this.tabTLogs = new System.Windows.Forms.TabPage();
            this.tableLayoutPaneltlogs = new System.Windows.Forms.TableLayoutPanel();
            this.tracklog = new System.Windows.Forms.TrackBar();
            this.BUT_playlog = new MissionPlanner.Controls.MyButton();
            this.BUT_log2kml = new MissionPlanner.Controls.MyButton();
            this.LBL_logfn = new System.Windows.Forms.Label();
            this.lbl_logpercent = new System.Windows.Forms.Label();
            this.lbl_playbackspeed = new System.Windows.Forms.Label();
            this.BUT_loadtelem = new MissionPlanner.Controls.MyButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.BUT_speed1_10 = new MissionPlanner.Controls.MyButton();
            this.BUT_speed1_4 = new MissionPlanner.Controls.MyButton();
            this.BUT_speed1_2 = new MissionPlanner.Controls.MyButton();
            this.BUT_speed1 = new MissionPlanner.Controls.MyButton();
            this.BUT_speed2 = new MissionPlanner.Controls.MyButton();
            this.BUT_speed5 = new MissionPlanner.Controls.MyButton();
            this.BUT_speed10 = new MissionPlanner.Controls.MyButton();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPayload = new System.Windows.Forms.TabPage();
            this.groupBoxPitch = new System.Windows.Forms.GroupBox();
            this.TXT_gimbalPitchPos = new System.Windows.Forms.TextBox();
            this.trackBarPitch = new System.Windows.Forms.TrackBar();
            this.BUT_resetGimbalPos = new MissionPlanner.Controls.MyButton();
            this.groupBoxYaw = new System.Windows.Forms.GroupBox();
            this.trackBarYaw = new System.Windows.Forms.TrackBar();
            this.TXT_gimbalYawPos = new System.Windows.Forms.TextBox();
            this.groupBoxRoll = new System.Windows.Forms.GroupBox();
            this.trackBarRoll = new System.Windows.Forms.TrackBar();
            this.TXT_gimbalRollPos = new System.Windows.Forms.TextBox();
            this.BUT_PayloadFolder = new MissionPlanner.Controls.MyButton();
            this.tabActionsSimple = new System.Windows.Forms.TabPage();
            this.myButton3 = new MissionPlanner.Controls.MyButton();
            this.myButton2 = new MissionPlanner.Controls.MyButton();
            this.myButton1 = new MissionPlanner.Controls.MyButton();
            this.tabActions = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BUT_setmode = new MissionPlanner.Controls.MyButton();
            this.BUT_quickauto = new MissionPlanner.Controls.MyButton();
            this.CMB_modes = new System.Windows.Forms.ComboBox();
            this.BUT_setwp = new MissionPlanner.Controls.MyButton();
            this.BUT_quickmanual = new MissionPlanner.Controls.MyButton();
            this.BUT_quickrtl = new MissionPlanner.Controls.MyButton();
            this.CMB_mountmode = new System.Windows.Forms.ComboBox();
            this.BUTrestartmission = new MissionPlanner.Controls.MyButton();
            this.BUT_Homealt = new MissionPlanner.Controls.MyButton();
            this.BUT_RAWSensor = new MissionPlanner.Controls.MyButton();
            this.BUT_joystick = new MissionPlanner.Controls.MyButton();
            this.BUT_mountmode = new MissionPlanner.Controls.MyButton();
            this.BUT_ARM = new MissionPlanner.Controls.MyButton();
            this.CMB_setwp = new System.Windows.Forms.ComboBox();
            this.modifyandSetSpeed = new MissionPlanner.Controls.ModifyandSet();
            this.modifyandSetAlt = new MissionPlanner.Controls.ModifyandSet();
            this.BUT_resumemis = new MissionPlanner.Controls.MyButton();
            this.BUTactiondo = new MissionPlanner.Controls.MyButton();
            this.CMB_action = new System.Windows.Forms.ComboBox();
            this.BUT_clear_track = new MissionPlanner.Controls.MyButton();
            this.modifyandSetLoiterRad = new MissionPlanner.Controls.ModifyandSet();
            this.BUT_abortland = new MissionPlanner.Controls.MyButton();
            this.tabQuick = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelQuick = new System.Windows.Forms.TableLayoutPanel();
            this.quickView1 = new MissionPlanner.Controls.QuickView();
            this.quickView2 = new MissionPlanner.Controls.QuickView();
            this.quickView3 = new MissionPlanner.Controls.QuickView();
            this.quickView4 = new MissionPlanner.Controls.QuickView();
            this.quickView5 = new MissionPlanner.Controls.QuickView();
            this.quickView6 = new MissionPlanner.Controls.QuickView();
            this.quickView8 = new MissionPlanner.Controls.QuickView();
            this.quickView7 = new MissionPlanner.Controls.QuickView();
            this.tabControlactions = new System.Windows.Forms.TabControl();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStripactionstab.SuspendLayout();
            this.contextMenuStripQuickView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceQuickTab)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceGaugesTab)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourcePayloadTab)).BeginInit();
            this.tableMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStripMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TRK_zoom)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Zoomlevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceStatusTab)).BeginInit();
            this.tabTLogs.SuspendLayout();
            this.tableLayoutPaneltlogs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tracklog)).BeginInit();
            this.panel2.SuspendLayout();
            this.tabPayload.SuspendLayout();
            this.groupBoxPitch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPitch)).BeginInit();
            this.groupBoxYaw.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarYaw)).BeginInit();
            this.groupBoxRoll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRoll)).BeginInit();
            this.tabActionsSimple.SuspendLayout();
            this.tabActions.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabQuick.SuspendLayout();
            this.tableLayoutPanelQuick.SuspendLayout();
            this.tabControlactions.SuspendLayout();
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
            this.SubMainLeft.Panel2.Controls.Add(this.splitContainer2);
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
            this.hud1.batteryremaining = 0F;
            this.hud1.bgimage = null;
            this.hud1.connected = false;
            this.hud1.ContextMenuStrip = this.contextMenuStripHud;
            this.hud1.critAOA = 25F;
            this.hud1.criticalvoltagealert = false;
            this.hud1.critSSA = 30F;
            this.hud1.current = 0F;
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("airspeed", this.bindingSourceHud, "airspeed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("alt", this.bindingSourceHud, "alt", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("batterylevel", this.bindingSourceHud, "battery_voltage", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("batteryremaining", this.bindingSourceHud, "battery_remaining", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("connected", this.bindingSourceHud, "connected", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("current", this.bindingSourceHud, "current", true));
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
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("roll", this.bindingSourceHud, "roll", true));
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
            this.hud1.roll = 0F;
            this.hud1.Russian = false;
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
            this.hud1.Load += new System.EventHandler(this.hud1_Load);
            this.hud1.DoubleClick += new System.EventHandler(this.hud1_DoubleClick);
            this.hud1.Resize += new System.EventHandler(this.hud1_Resize);
            // 
            // contextMenuStripHud
            // 
            this.contextMenuStripHud.ImageScalingSize = new System.Drawing.Size(20, 20);
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
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControlactions);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(62)))), ((int)(((byte)(117)))));
            this.splitContainer2.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer2.Panel2.Controls.Add(this.myButton10);
            this.splitContainer2.Panel2.Controls.Add(this.myButton9);
            this.splitContainer2.Panel2.Controls.Add(this.SelectWP);
            this.splitContainer2.Panel2.Controls.Add(this.myButton7);
            this.splitContainer2.Panel2.Controls.Add(this.myButton6);
            this.splitContainer2.Panel2.Controls.Add(this.myButton5);
            this.splitContainer2.Panel2.Controls.Add(this.myButton4);
            // 
            // contextMenuStripactionstab
            // 
            this.contextMenuStripactionstab.ImageScalingSize = new System.Drawing.Size(20, 20);
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
            // contextMenuStripQuickView
            // 
            this.contextMenuStripQuickView.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripQuickView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setViewCountToolStripMenuItem,
            this.undockToolStripMenuItem});
            this.contextMenuStripQuickView.Name = "contextMenuStripQuickView";
            resources.ApplyResources(this.contextMenuStripQuickView, "contextMenuStripQuickView");
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
            // bindingSourceQuickTab
            // 
            this.bindingSourceQuickTab.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // bindingSourceGaugesTab
            // 
            this.bindingSourceGaugesTab.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // bindingSourcePayloadTab
            // 
            this.bindingSourcePayloadTab.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Info;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.Menu;
            this.richTextBox1.Name = "richTextBox1";
            // 
            // myButton10
            // 
            this.myButton10.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton10.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton10.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton10, "myButton10");
            this.myButton10.Name = "myButton10";
            this.toolTip1.SetToolTip(this.myButton10, resources.GetString("myButton10.ToolTip"));
            this.myButton10.UseVisualStyleBackColor = true;
            // 
            // myButton9
            // 
            this.myButton9.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton9.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton9.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton9, "myButton9");
            this.myButton9.Name = "myButton9";
            this.toolTip1.SetToolTip(this.myButton9, resources.GetString("myButton9.ToolTip"));
            this.myButton9.UseVisualStyleBackColor = true;
            this.myButton9.Click += new System.EventHandler(this.myButton9_Click);
            // 
            // SelectWP
            // 
            this.SelectWP.FormattingEnabled = true;
            resources.ApplyResources(this.SelectWP, "SelectWP");
            this.SelectWP.Name = "SelectWP";
            this.SelectWP.Click += new System.EventHandler(this.SelectWP_Click);
            // 
            // myButton7
            // 
            this.myButton7.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton7.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton7.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton7, "myButton7");
            this.myButton7.Name = "myButton7";
            this.toolTip1.SetToolTip(this.myButton7, resources.GetString("myButton7.ToolTip"));
            this.myButton7.UseVisualStyleBackColor = true;
            this.myButton7.Click += new System.EventHandler(this.BUT_quickmanual_Click);
            // 
            // myButton6
            // 
            this.myButton6.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton6.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton6.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton6, "myButton6");
            this.myButton6.Name = "myButton6";
            this.myButton6.UseVisualStyleBackColor = true;
            this.myButton6.Click += new System.EventHandler(this.BUT_resumemis_V2_Click);
            // 
            // myButton5
            // 
            this.myButton5.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton5.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton5.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton5, "myButton5");
            this.myButton5.Name = "myButton5";
            this.toolTip1.SetToolTip(this.myButton5, resources.GetString("myButton5.ToolTip"));
            this.myButton5.UseVisualStyleBackColor = true;
            this.myButton5.Click += new System.EventHandler(this.BUT_ARM_Click);
            // 
            // myButton4
            // 
            this.myButton4.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton4.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton4.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton4, "myButton4");
            this.myButton4.Name = "myButton4";
            this.toolTip1.SetToolTip(this.myButton4, resources.GetString("myButton4.ToolTip"));
            this.myButton4.UseVisualStyleBackColor = true;
            this.myButton4.Click += new System.EventHandler(this.BUT_quickauto_Click);
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
            this.splitContainer1.Panel1.Controls.Add(this.zg1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.ContextMenuStrip = this.contextMenuStripMap;
            this.splitContainer1.Panel2.Controls.Add(this.distanceBar1);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.lbl_hdop);
            this.splitContainer1.Panel2.Controls.Add(this.lbl_sats);
            this.splitContainer1.Panel2.Controls.Add(this.gMapControl1);
            this.splitContainer1.Panel2.Controls.Add(this.TRK_zoom);
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
            // contextMenuStripMap
            // 
            this.contextMenuStripMap.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripMap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goHereToolStripMenuItem,
            this.pointCameraHereToolStripMenuItem,
            this.toolStripMenuItem1});
            this.contextMenuStripMap.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStripMap, "contextMenuStripMap");
            // 
            // goHereToolStripMenuItem
            // 
            this.goHereToolStripMenuItem.Name = "goHereToolStripMenuItem";
            resources.ApplyResources(this.goHereToolStripMenuItem, "goHereToolStripMenuItem");
            this.goHereToolStripMenuItem.Click += new System.EventHandler(this.goHereToolStripMenuItem_Click);
            // 
            // pointCameraHereToolStripMenuItem
            // 
            this.pointCameraHereToolStripMenuItem.Name = "pointCameraHereToolStripMenuItem";
            resources.ApplyResources(this.pointCameraHereToolStripMenuItem, "pointCameraHereToolStripMenuItem");
            this.pointCameraHereToolStripMenuItem.Click += new System.EventHandler(this.pointCameraHereToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Click += new System.EventHandler(this.setHomeHereToolStripMenuItem_Click);
            // 
            // distanceBar1
            // 
            resources.ApplyResources(this.distanceBar1, "distanceBar1");
            this.distanceBar1.BackColor = System.Drawing.Color.Transparent;
            this.distanceBar1.Name = "distanceBar1";
            this.distanceBar1.totaldist = 100F;
            this.distanceBar1.traveleddist = 0F;
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
            this.lbl_hdop.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource1, "gpshdop", true, System.Windows.Forms.DataSourceUpdateMode.Never, null, "hdop: 0.0"));
            this.lbl_hdop.Name = "lbl_hdop";
            this.lbl_hdop.resize = true;
            this.toolTip1.SetToolTip(this.lbl_hdop, resources.GetString("lbl_hdop.ToolTip"));
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // lbl_sats
            // 
            resources.ApplyResources(this.lbl_sats, "lbl_sats");
            this.lbl_sats.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource1, "satcount", true, System.Windows.Forms.DataSourceUpdateMode.Never, null, "Sats: 0"));
            this.lbl_sats.Name = "lbl_sats";
            this.lbl_sats.resize = true;
            this.toolTip1.SetToolTip(this.lbl_sats, resources.GetString("lbl_sats.ToolTip"));
            // 
            // gMapControl1
            // 
            this.gMapControl1.BackColor = System.Drawing.Color.AliceBlue;
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
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            this.gMapControl1.Click += new System.EventHandler(this.gMapControl1_Click);
            this.gMapControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseDown);
            this.gMapControl1.MouseLeave += new System.EventHandler(this.gMapControl1_MouseLeave);
            this.gMapControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseMove);
            this.gMapControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseUp);
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
            this.TRK_zoom.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.TRK_zoom.Value = 1F;
            this.TRK_zoom.Scroll += new System.EventHandler(this.TRK_zoom_Scroll);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.coords1);
            this.panel1.Controls.Add(this.Zoomlevel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.CHK_autopan);
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.resize = false;
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
            // ZedGraphTimer
            // 
            this.ZedGraphTimer.Tick += new System.EventHandler(this.timer1_Tick);
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
            // tabTLogs
            // 
            this.tabTLogs.Controls.Add(this.tableLayoutPaneltlogs);
            resources.ApplyResources(this.tabTLogs, "tabTLogs");
            this.tabTLogs.Name = "tabTLogs";
            this.tabTLogs.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPaneltlogs
            // 
            resources.ApplyResources(this.tableLayoutPaneltlogs, "tableLayoutPaneltlogs");
            this.tableLayoutPaneltlogs.Controls.Add(this.panel2, 1, 2);
            this.tableLayoutPaneltlogs.Controls.Add(this.BUT_loadtelem, 0, 0);
            this.tableLayoutPaneltlogs.Controls.Add(this.lbl_playbackspeed, 2, 2);
            this.tableLayoutPaneltlogs.Controls.Add(this.lbl_logpercent, 2, 1);
            this.tableLayoutPaneltlogs.Controls.Add(this.LBL_logfn, 1, 0);
            this.tableLayoutPaneltlogs.Controls.Add(this.BUT_log2kml, 0, 2);
            this.tableLayoutPaneltlogs.Controls.Add(this.BUT_playlog, 0, 1);
            this.tableLayoutPaneltlogs.Controls.Add(this.tracklog, 1, 1);
            this.tableLayoutPaneltlogs.Name = "tableLayoutPaneltlogs";
            // 
            // tracklog
            // 
            resources.ApplyResources(this.tracklog, "tracklog");
            this.tracklog.Maximum = 100;
            this.tracklog.Name = "tracklog";
            this.tracklog.TickFrequency = 5;
            this.tracklog.Scroll += new System.EventHandler(this.tracklog_Scroll);
            // 
            // BUT_playlog
            // 
            this.BUT_playlog.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_playlog.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_playlog.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_playlog, "BUT_playlog");
            this.BUT_playlog.Name = "BUT_playlog";
            this.BUT_playlog.UseVisualStyleBackColor = true;
            this.BUT_playlog.Click += new System.EventHandler(this.BUT_playlog_Click);
            // 
            // BUT_log2kml
            // 
            this.BUT_log2kml.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_log2kml.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_log2kml.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_log2kml, "BUT_log2kml");
            this.BUT_log2kml.Name = "BUT_log2kml";
            this.BUT_log2kml.UseVisualStyleBackColor = true;
            this.BUT_log2kml.Click += new System.EventHandler(this.BUT_log2kml_Click);
            // 
            // LBL_logfn
            // 
            this.tableLayoutPaneltlogs.SetColumnSpan(this.LBL_logfn, 2);
            resources.ApplyResources(this.LBL_logfn, "LBL_logfn");
            this.LBL_logfn.Name = "LBL_logfn";
            // 
            // lbl_logpercent
            // 
            resources.ApplyResources(this.lbl_logpercent, "lbl_logpercent");
            this.lbl_logpercent.Name = "lbl_logpercent";
            // 
            // lbl_playbackspeed
            // 
            resources.ApplyResources(this.lbl_playbackspeed, "lbl_playbackspeed");
            this.lbl_playbackspeed.Name = "lbl_playbackspeed";
            // 
            // BUT_loadtelem
            // 
            this.BUT_loadtelem.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_loadtelem.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_loadtelem.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_loadtelem, "BUT_loadtelem");
            this.BUT_loadtelem.Name = "BUT_loadtelem";
            this.BUT_loadtelem.UseVisualStyleBackColor = true;
            this.BUT_loadtelem.Click += new System.EventHandler(this.BUT_loadtelem_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.BUT_speed10);
            this.panel2.Controls.Add(this.BUT_speed5);
            this.panel2.Controls.Add(this.BUT_speed2);
            this.panel2.Controls.Add(this.BUT_speed1);
            this.panel2.Controls.Add(this.BUT_speed1_2);
            this.panel2.Controls.Add(this.BUT_speed1_4);
            this.panel2.Controls.Add(this.BUT_speed1_10);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // BUT_speed1_10
            // 
            this.BUT_speed1_10.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_speed1_10.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_speed1_10.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_speed1_10, "BUT_speed1_10");
            this.BUT_speed1_10.Name = "BUT_speed1_10";
            this.BUT_speed1_10.Tag = "0.1";
            this.BUT_speed1_10.UseVisualStyleBackColor = true;
            this.BUT_speed1_10.Click += new System.EventHandler(this.BUT_speed1_Click);
            // 
            // BUT_speed1_4
            // 
            this.BUT_speed1_4.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_speed1_4.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_speed1_4.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_speed1_4, "BUT_speed1_4");
            this.BUT_speed1_4.Name = "BUT_speed1_4";
            this.BUT_speed1_4.Tag = "0.25";
            this.BUT_speed1_4.UseVisualStyleBackColor = true;
            this.BUT_speed1_4.Click += new System.EventHandler(this.BUT_speed1_Click);
            // 
            // BUT_speed1_2
            // 
            this.BUT_speed1_2.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_speed1_2.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_speed1_2.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_speed1_2, "BUT_speed1_2");
            this.BUT_speed1_2.Name = "BUT_speed1_2";
            this.BUT_speed1_2.Tag = "0.5";
            this.BUT_speed1_2.UseVisualStyleBackColor = true;
            this.BUT_speed1_2.Click += new System.EventHandler(this.BUT_speed1_Click);
            // 
            // BUT_speed1
            // 
            this.BUT_speed1.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_speed1.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_speed1.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_speed1, "BUT_speed1");
            this.BUT_speed1.Name = "BUT_speed1";
            this.BUT_speed1.Tag = "1";
            this.BUT_speed1.UseVisualStyleBackColor = true;
            this.BUT_speed1.Click += new System.EventHandler(this.BUT_speed1_Click);
            // 
            // BUT_speed2
            // 
            this.BUT_speed2.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_speed2.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_speed2.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_speed2, "BUT_speed2");
            this.BUT_speed2.Name = "BUT_speed2";
            this.BUT_speed2.Tag = "2";
            this.BUT_speed2.UseVisualStyleBackColor = true;
            this.BUT_speed2.Click += new System.EventHandler(this.BUT_speed1_Click);
            // 
            // BUT_speed5
            // 
            this.BUT_speed5.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_speed5.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_speed5.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_speed5, "BUT_speed5");
            this.BUT_speed5.Name = "BUT_speed5";
            this.BUT_speed5.Tag = "5";
            this.BUT_speed5.UseVisualStyleBackColor = true;
            this.BUT_speed5.Click += new System.EventHandler(this.BUT_speed1_Click);
            // 
            // BUT_speed10
            // 
            this.BUT_speed10.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_speed10.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_speed10.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_speed10, "BUT_speed10");
            this.BUT_speed10.Name = "BUT_speed10";
            this.BUT_speed10.Tag = "10";
            this.BUT_speed10.UseVisualStyleBackColor = true;
            this.BUT_speed10.Click += new System.EventHandler(this.BUT_speed1_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tabPayload
            // 
            this.tabPayload.Controls.Add(this.BUT_PayloadFolder);
            this.tabPayload.Controls.Add(this.groupBoxRoll);
            this.tabPayload.Controls.Add(this.groupBoxYaw);
            this.tabPayload.Controls.Add(this.BUT_resetGimbalPos);
            this.tabPayload.Controls.Add(this.groupBoxPitch);
            resources.ApplyResources(this.tabPayload, "tabPayload");
            this.tabPayload.Name = "tabPayload";
            this.tabPayload.UseVisualStyleBackColor = true;
            // 
            // groupBoxPitch
            // 
            this.groupBoxPitch.Controls.Add(this.trackBarPitch);
            this.groupBoxPitch.Controls.Add(this.TXT_gimbalPitchPos);
            resources.ApplyResources(this.groupBoxPitch, "groupBoxPitch");
            this.groupBoxPitch.Name = "groupBoxPitch";
            this.groupBoxPitch.TabStop = false;
            // 
            // TXT_gimbalPitchPos
            // 
            this.TXT_gimbalPitchPos.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourcePayloadTab, "campointa", true));
            resources.ApplyResources(this.TXT_gimbalPitchPos, "TXT_gimbalPitchPos");
            this.TXT_gimbalPitchPos.Name = "TXT_gimbalPitchPos";
            // 
            // trackBarPitch
            // 
            resources.ApplyResources(this.trackBarPitch, "trackBarPitch");
            this.trackBarPitch.LargeChange = 10;
            this.trackBarPitch.Maximum = 45;
            this.trackBarPitch.Minimum = -45;
            this.trackBarPitch.Name = "trackBarPitch";
            this.trackBarPitch.SmallChange = 5;
            this.trackBarPitch.TickFrequency = 10;
            this.trackBarPitch.Scroll += new System.EventHandler(this.gimbalTrackbar_Scroll);
            // 
            // BUT_resetGimbalPos
            // 
            resources.ApplyResources(this.BUT_resetGimbalPos, "BUT_resetGimbalPos");
            this.BUT_resetGimbalPos.Name = "BUT_resetGimbalPos";
            this.BUT_resetGimbalPos.UseVisualStyleBackColor = true;
            this.BUT_resetGimbalPos.Click += new System.EventHandler(this.BUT_resetGimbalPos_Click);
            // 
            // groupBoxYaw
            // 
            this.groupBoxYaw.Controls.Add(this.TXT_gimbalYawPos);
            this.groupBoxYaw.Controls.Add(this.trackBarYaw);
            resources.ApplyResources(this.groupBoxYaw, "groupBoxYaw");
            this.groupBoxYaw.Name = "groupBoxYaw";
            this.groupBoxYaw.TabStop = false;
            // 
            // trackBarYaw
            // 
            resources.ApplyResources(this.trackBarYaw, "trackBarYaw");
            this.trackBarYaw.LargeChange = 10;
            this.trackBarYaw.Maximum = 45;
            this.trackBarYaw.Minimum = -45;
            this.trackBarYaw.Name = "trackBarYaw";
            this.trackBarYaw.TickFrequency = 10;
            this.trackBarYaw.Scroll += new System.EventHandler(this.gimbalTrackbar_Scroll);
            // 
            // TXT_gimbalYawPos
            // 
            this.TXT_gimbalYawPos.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourcePayloadTab, "campointc", true));
            resources.ApplyResources(this.TXT_gimbalYawPos, "TXT_gimbalYawPos");
            this.TXT_gimbalYawPos.Name = "TXT_gimbalYawPos";
            // 
            // groupBoxRoll
            // 
            this.groupBoxRoll.Controls.Add(this.TXT_gimbalRollPos);
            this.groupBoxRoll.Controls.Add(this.trackBarRoll);
            resources.ApplyResources(this.groupBoxRoll, "groupBoxRoll");
            this.groupBoxRoll.Name = "groupBoxRoll";
            this.groupBoxRoll.TabStop = false;
            // 
            // trackBarRoll
            // 
            resources.ApplyResources(this.trackBarRoll, "trackBarRoll");
            this.trackBarRoll.LargeChange = 10;
            this.trackBarRoll.Maximum = 45;
            this.trackBarRoll.Minimum = -45;
            this.trackBarRoll.Name = "trackBarRoll";
            this.trackBarRoll.TickFrequency = 10;
            this.trackBarRoll.Scroll += new System.EventHandler(this.gimbalTrackbar_Scroll);
            // 
            // TXT_gimbalRollPos
            // 
            this.TXT_gimbalRollPos.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSourcePayloadTab, "campointb", true));
            resources.ApplyResources(this.TXT_gimbalRollPos, "TXT_gimbalRollPos");
            this.TXT_gimbalRollPos.Name = "TXT_gimbalRollPos";
            // 
            // BUT_PayloadFolder
            // 
            resources.ApplyResources(this.BUT_PayloadFolder, "BUT_PayloadFolder");
            this.BUT_PayloadFolder.Name = "BUT_PayloadFolder";
            this.BUT_PayloadFolder.UseVisualStyleBackColor = true;
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
            // myButton3
            // 
            this.myButton3.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton3.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton3.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton3, "myButton3");
            this.myButton3.Name = "myButton3";
            this.toolTip1.SetToolTip(this.myButton3, resources.GetString("myButton3.ToolTip"));
            this.myButton3.UseVisualStyleBackColor = true;
            this.myButton3.Click += new System.EventHandler(this.BUT_quickauto_Click);
            // 
            // myButton2
            // 
            this.myButton2.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton2.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton2.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton2, "myButton2");
            this.myButton2.Name = "myButton2";
            this.toolTip1.SetToolTip(this.myButton2, resources.GetString("myButton2.ToolTip"));
            this.myButton2.UseVisualStyleBackColor = true;
            this.myButton2.Click += new System.EventHandler(this.BUT_quickrtl_Click);
            // 
            // myButton1
            // 
            this.myButton1.ColorMouseDown = System.Drawing.Color.Empty;
            this.myButton1.ColorMouseOver = System.Drawing.Color.Empty;
            this.myButton1.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.myButton1, "myButton1");
            this.myButton1.Name = "myButton1";
            this.toolTip1.SetToolTip(this.myButton1, resources.GetString("myButton1.ToolTip"));
            this.myButton1.UseVisualStyleBackColor = true;
            this.myButton1.Click += new System.EventHandler(this.BUT_quickmanual_Click);
            // 
            // tabActions
            // 
            this.tabActions.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.tabActions, "tabActions");
            this.tabActions.Name = "tabActions";
            this.tabActions.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
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
            // 
            // BUT_setmode
            // 
            this.BUT_setmode.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_setmode.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_setmode.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_setmode, "BUT_setmode");
            this.BUT_setmode.Name = "BUT_setmode";
            this.toolTip1.SetToolTip(this.BUT_setmode, resources.GetString("BUT_setmode.ToolTip"));
            this.BUT_setmode.UseVisualStyleBackColor = true;
            this.BUT_setmode.Click += new System.EventHandler(this.BUT_setmode_Click);
            // 
            // BUT_quickauto
            // 
            this.BUT_quickauto.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_quickauto.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_quickauto.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_quickauto, "BUT_quickauto");
            this.BUT_quickauto.Name = "BUT_quickauto";
            this.toolTip1.SetToolTip(this.BUT_quickauto, resources.GetString("BUT_quickauto.ToolTip"));
            this.BUT_quickauto.UseVisualStyleBackColor = true;
            this.BUT_quickauto.Click += new System.EventHandler(this.BUT_quickauto_Click);
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
            // BUT_setwp
            // 
            this.BUT_setwp.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_setwp.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_setwp.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_setwp, "BUT_setwp");
            this.BUT_setwp.Name = "BUT_setwp";
            this.toolTip1.SetToolTip(this.BUT_setwp, resources.GetString("BUT_setwp.ToolTip"));
            this.BUT_setwp.UseVisualStyleBackColor = true;
            this.BUT_setwp.Click += new System.EventHandler(this.BUT_setwp_Click);
            // 
            // BUT_quickmanual
            // 
            this.BUT_quickmanual.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_quickmanual.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_quickmanual.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_quickmanual, "BUT_quickmanual");
            this.BUT_quickmanual.Name = "BUT_quickmanual";
            this.toolTip1.SetToolTip(this.BUT_quickmanual, resources.GetString("BUT_quickmanual.ToolTip"));
            this.BUT_quickmanual.UseVisualStyleBackColor = true;
            this.BUT_quickmanual.Click += new System.EventHandler(this.BUT_quickmanual_Click);
            // 
            // BUT_quickrtl
            // 
            this.BUT_quickrtl.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_quickrtl.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_quickrtl.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_quickrtl, "BUT_quickrtl");
            this.BUT_quickrtl.Name = "BUT_quickrtl";
            this.toolTip1.SetToolTip(this.BUT_quickrtl, resources.GetString("BUT_quickrtl.ToolTip"));
            this.BUT_quickrtl.UseVisualStyleBackColor = true;
            this.BUT_quickrtl.Click += new System.EventHandler(this.BUT_quickrtl_Click);
            // 
            // CMB_mountmode
            // 
            resources.ApplyResources(this.CMB_mountmode, "CMB_mountmode");
            this.CMB_mountmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_mountmode.DropDownWidth = 150;
            this.CMB_mountmode.FormattingEnabled = true;
            this.CMB_mountmode.Name = "CMB_mountmode";
            // 
            // BUTrestartmission
            // 
            this.BUTrestartmission.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUTrestartmission.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUTrestartmission.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUTrestartmission, "BUTrestartmission");
            this.BUTrestartmission.Name = "BUTrestartmission";
            this.toolTip1.SetToolTip(this.BUTrestartmission, resources.GetString("BUTrestartmission.ToolTip"));
            this.BUTrestartmission.UseVisualStyleBackColor = true;
            this.BUTrestartmission.Click += new System.EventHandler(this.BUTrestartmission_Click);
            // 
            // BUT_Homealt
            // 
            this.BUT_Homealt.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_Homealt.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_Homealt.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_Homealt, "BUT_Homealt");
            this.BUT_Homealt.Name = "BUT_Homealt";
            this.toolTip1.SetToolTip(this.BUT_Homealt, resources.GetString("BUT_Homealt.ToolTip"));
            this.BUT_Homealt.UseVisualStyleBackColor = true;
            this.BUT_Homealt.Click += new System.EventHandler(this.BUT_Homealt_Click);
            // 
            // BUT_RAWSensor
            // 
            this.BUT_RAWSensor.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_RAWSensor.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_RAWSensor.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_RAWSensor, "BUT_RAWSensor");
            this.BUT_RAWSensor.Name = "BUT_RAWSensor";
            this.toolTip1.SetToolTip(this.BUT_RAWSensor, resources.GetString("BUT_RAWSensor.ToolTip"));
            this.BUT_RAWSensor.UseVisualStyleBackColor = true;
            this.BUT_RAWSensor.Click += new System.EventHandler(this.BUT_RAWSensor_Click);
            // 
            // BUT_joystick
            // 
            this.BUT_joystick.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_joystick.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_joystick.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_joystick, "BUT_joystick");
            this.BUT_joystick.Name = "BUT_joystick";
            this.toolTip1.SetToolTip(this.BUT_joystick, resources.GetString("BUT_joystick.ToolTip"));
            this.BUT_joystick.UseVisualStyleBackColor = true;
            this.BUT_joystick.Click += new System.EventHandler(this.BUT_joystick_Click);
            // 
            // BUT_mountmode
            // 
            this.BUT_mountmode.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_mountmode.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_mountmode.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_mountmode, "BUT_mountmode");
            this.BUT_mountmode.Name = "BUT_mountmode";
            this.toolTip1.SetToolTip(this.BUT_mountmode, resources.GetString("BUT_mountmode.ToolTip"));
            this.BUT_mountmode.UseVisualStyleBackColor = true;
            this.BUT_mountmode.Click += new System.EventHandler(this.BUT_mountmode_Click);
            // 
            // BUT_ARM
            // 
            this.BUT_ARM.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_ARM.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_ARM.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_ARM, "BUT_ARM");
            this.BUT_ARM.Name = "BUT_ARM";
            this.toolTip1.SetToolTip(this.BUT_ARM, resources.GetString("BUT_ARM.ToolTip"));
            this.BUT_ARM.UseVisualStyleBackColor = true;
            this.BUT_ARM.Click += new System.EventHandler(this.BUT_ARM_Click);
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
            // modifyandSetSpeed
            // 
            resources.ApplyResources(this.modifyandSetSpeed, "modifyandSetSpeed");
            this.modifyandSetSpeed.ButtonText = "Change Speed";
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
            // modifyandSetAlt
            // 
            resources.ApplyResources(this.modifyandSetAlt, "modifyandSetAlt");
            this.modifyandSetAlt.ButtonText = "Change Alt";
            this.modifyandSetAlt.DecimalPlaces = 1;
            this.modifyandSetAlt.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.modifyandSetAlt.Maximum = new decimal(new int[] {
            1000,
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
            // BUT_resumemis
            // 
            this.BUT_resumemis.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_resumemis.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_resumemis.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_resumemis, "BUT_resumemis");
            this.BUT_resumemis.Name = "BUT_resumemis";
            this.BUT_resumemis.UseVisualStyleBackColor = true;
            this.BUT_resumemis.Click += new System.EventHandler(this.BUT_resumemis_Click);
            // 
            // BUTactiondo
            // 
            this.BUTactiondo.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUTactiondo.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUTactiondo.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUTactiondo, "BUTactiondo");
            this.BUTactiondo.Name = "BUTactiondo";
            this.toolTip1.SetToolTip(this.BUTactiondo, resources.GetString("BUTactiondo.ToolTip"));
            this.BUTactiondo.UseVisualStyleBackColor = true;
            this.BUTactiondo.Click += new System.EventHandler(this.BUTactiondo_Click);
            // 
            // CMB_action
            // 
            resources.ApplyResources(this.CMB_action, "CMB_action");
            this.CMB_action.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_action.DropDownWidth = 150;
            this.CMB_action.FormattingEnabled = true;
            this.CMB_action.Name = "CMB_action";
            // 
            // BUT_clear_track
            // 
            this.BUT_clear_track.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_clear_track.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_clear_track.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_clear_track, "BUT_clear_track");
            this.BUT_clear_track.Name = "BUT_clear_track";
            this.toolTip1.SetToolTip(this.BUT_clear_track, resources.GetString("BUT_clear_track.ToolTip"));
            this.BUT_clear_track.UseVisualStyleBackColor = true;
            this.BUT_clear_track.Click += new System.EventHandler(this.BUT_clear_track_Click);
            // 
            // modifyandSetLoiterRad
            // 
            resources.ApplyResources(this.modifyandSetLoiterRad, "modifyandSetLoiterRad");
            this.modifyandSetLoiterRad.ButtonText = "Set Loiter Rad";
            this.modifyandSetLoiterRad.DecimalPlaces = 0;
            this.modifyandSetLoiterRad.Increment = new decimal(new int[] {
            1,
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
            // BUT_abortland
            // 
            this.BUT_abortland.ColorMouseDown = System.Drawing.Color.Empty;
            this.BUT_abortland.ColorMouseOver = System.Drawing.Color.Empty;
            this.BUT_abortland.ColorNotEnabled = System.Drawing.Color.Empty;
            resources.ApplyResources(this.BUT_abortland, "BUT_abortland");
            this.BUT_abortland.Name = "BUT_abortland";
            this.toolTip1.SetToolTip(this.BUT_abortland, resources.GetString("BUT_abortland.ToolTip"));
            this.BUT_abortland.UseVisualStyleBackColor = true;
            this.BUT_abortland.Click += new System.EventHandler(this.BUT_abortland_Click);
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
            this.tableLayoutPanelQuick.Controls.Add(this.quickView7, 0, 3);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView8, 1, 3);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView6, 1, 2);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView5, 0, 2);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView4, 1, 1);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView3, 0, 1);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView2, 1, 0);
            this.tableLayoutPanelQuick.Controls.Add(this.quickView1, 0, 0);
            this.tableLayoutPanelQuick.Name = "tableLayoutPanelQuick";
            // 
            // quickView1
            // 
            this.quickView1.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView1.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "battery_remaining", true));
            this.quickView1.desc = "Tension Batterie";
            resources.ApplyResources(this.quickView1, "quickView1");
            this.quickView1.Name = "quickView1";
            this.quickView1.number = 0D;
            this.quickView1.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.quickView1.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView1.numberformat = "0";
            // 
            // quickView2
            // 
            this.quickView2.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView2.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "battery_remaining", true));
            this.quickView2.desc = "% Batterie";
            resources.ApplyResources(this.quickView2, "quickView2");
            this.quickView2.Name = "quickView2";
            this.quickView2.number = 0D;
            this.quickView2.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.quickView2.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView2.numberformat = "0";
            // 
            // quickView3
            // 
            this.quickView3.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView3.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "battery_remaining", true));
            this.quickView3.desc = "Courant Bat (A)";
            resources.ApplyResources(this.quickView3, "quickView3");
            this.quickView3.Name = "quickView3";
            this.quickView3.number = 0D;
            this.quickView3.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.quickView3.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView3.numberformat = "0";
            // 
            // quickView4
            // 
            this.quickView4.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView4.desc = "Vitesse (noeuds)";
            resources.ApplyResources(this.quickView4, "quickView4");
            this.quickView4.Name = "quickView4";
            this.quickView4.number = 0D;
            this.quickView4.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.quickView4.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView4.numberformat = "0";
            // 
            // quickView5
            // 
            this.quickView5.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView5.desc = "Hauteur d\'eau";
            resources.ApplyResources(this.quickView5, "quickView5");
            this.quickView5.Name = "quickView5";
            this.quickView5.number = 0D;
            this.quickView5.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.quickView5.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView5.numberformat = "0";
            // 
            // quickView6
            // 
            this.quickView6.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView6.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "battery_remaining", true));
            this.quickView6.desc = "Dist obstacle";
            resources.ApplyResources(this.quickView6, "quickView6");
            this.quickView6.Name = "quickView6";
            this.quickView6.number = 0D;
            this.quickView6.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.quickView6.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView6.numberformat = "0";
            // 
            // quickView8
            // 
            this.quickView8.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView8.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "battery_remaining", true));
            this.quickView8.desc = "temp process (°C)";
            resources.ApplyResources(this.quickView8, "quickView8");
            this.quickView8.Name = "quickView8";
            this.quickView8.number = 0D;
            this.quickView8.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.quickView8.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView8.numberformat = "0";
            this.quickView8.DoubleClick += new System.EventHandler(this.quickView_DoubleClick);
            // 
            // quickView7
            // 
            this.quickView7.ContextMenuStrip = this.contextMenuStripQuickView;
            this.quickView7.DataBindings.Add(new System.Windows.Forms.Binding("number", this.bindingSourceQuickTab, "battery_remaining", true));
            this.quickView7.desc = "Long cable (m)";
            resources.ApplyResources(this.quickView7, "quickView7");
            this.quickView7.Name = "quickView7";
            this.quickView7.number = 0D;
            this.quickView7.numberColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.quickView7.numberColorBackup = System.Drawing.Color.Empty;
            this.quickView7.numberformat = "0";
            this.quickView7.DoubleClick += new System.EventHandler(this.quickView_DoubleClick);
            // 
            // tabControlactions
            // 
            this.tabControlactions.ContextMenuStrip = this.contextMenuStripactionstab;
            this.tabControlactions.Controls.Add(this.tabQuick);
            this.tabControlactions.Controls.Add(this.tabActions);
            this.tabControlactions.Controls.Add(this.tabActionsSimple);
            this.tabControlactions.Controls.Add(this.tabPayload);
            this.tabControlactions.Controls.Add(this.tabTLogs);
            resources.ApplyResources(this.tabControlactions, "tabControlactions");
            this.tabControlactions.Name = "tabControlactions";
            this.tabControlactions.SelectedIndex = 0;
            this.tabControlactions.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControlactions.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
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
            ((System.ComponentModel.ISupportInitialize)(this.SubMainLeft)).EndInit();
            this.SubMainLeft.ResumeLayout(false);
            this.contextMenuStripHud.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceHud)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStripactionstab.ResumeLayout(false);
            this.contextMenuStripQuickView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceQuickTab)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceGaugesTab)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourcePayloadTab)).EndInit();
            this.tableMap.ResumeLayout(false);
            this.tableMap.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStripMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TRK_zoom)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Zoomlevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceStatusTab)).EndInit();
            this.tabTLogs.ResumeLayout(false);
            this.tableLayoutPaneltlogs.ResumeLayout(false);
            this.tableLayoutPaneltlogs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tracklog)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPayload.ResumeLayout(false);
            this.groupBoxPitch.ResumeLayout(false);
            this.groupBoxPitch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPitch)).EndInit();
            this.groupBoxYaw.ResumeLayout(false);
            this.groupBoxYaw.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarYaw)).EndInit();
            this.groupBoxRoll.ResumeLayout(false);
            this.groupBoxRoll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRoll)).EndInit();
            this.tabActionsSimple.ResumeLayout(false);
            this.tabActions.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabQuick.ResumeLayout(false);
            this.tableLayoutPanelQuick.ResumeLayout(false);
            this.tabControlactions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

  

        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Timer ZedGraphTimer;
        private System.Windows.Forms.SplitContainer MainH;
        private System.Windows.Forms.SplitContainer SubMainLeft;
        private System.Windows.Forms.ToolStripMenuItem goHereToolStripMenuItem;
        private Controls.HUD hud1;
        private System.Windows.Forms.TableLayoutPanel tableMap;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown Zoomlevel;
        private Controls.MyLabel label1;
        private System.Windows.Forms.CheckBox CHK_autopan;
        public Controls.myGMAP gMapControl1;
        private ZedGraph.ZedGraphControl zg1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem pointCameraHereToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Controls.MyLabel lbl_hdop;
        private Controls.MyLabel lbl_sats;
        private System.Windows.Forms.ToolStripMenuItem setAspectRatioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userItemsToolStripMenuItem;
        private Controls.MyTrackBar TRK_zoom;
        private System.Windows.Forms.BindingSource bindingSourceHud;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog openScriptDialog;
        private System.Windows.Forms.Timer scriptChecker;
        private System.Windows.Forms.ToolStripMenuItem russianHudToolStripMenuItem;
        public System.Windows.Forms.ContextMenuStrip contextMenuStripMap;
        public System.Windows.Forms.ContextMenuStrip contextMenuStripHud;
        private System.Windows.Forms.BindingSource bindingSourceQuickTab;
        private System.Windows.Forms.BindingSource bindingSourceStatusTab;
        private System.Windows.Forms.BindingSource bindingSourceGaugesTab;
        private MissionPlanner.Controls.Coords coords1;
        private System.Windows.Forms.Timer Messagetabtimer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripactionstab;
        private Controls.DistanceBar distanceBar1;
        private System.Windows.Forms.ToolStripMenuItem swapWithMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem videoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recordHudToAVIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setMJPEGSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripQuickView;
        private System.Windows.Forms.ToolStripMenuItem setViewCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setGStreamerSourceToolStripMenuItem;
        private System.Windows.Forms.BindingSource bindingSourcePayloadTab;
        private System.Windows.Forms.ToolStripMenuItem groundColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hereLinkVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gStreamerStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setBatteryCellCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undockToolStripMenuItem;
        private ToolStripMenuItem showIconsToolStripMenuItem;
        private SplitContainer splitContainer2;
        private Controls.MyButton myButton4;
        private Controls.MyButton myButton7;
        private Controls.MyButton myButton6;
        private Controls.MyButton myButton5;
        private ComboBox SelectWP;
        private Controls.MyButton myButton9;
        private Controls.MyButton myButton10;
        private ContextMenu contextMenu1;
        private RichTextBox richTextBox1;
        private ContextMenu contextMenu2;
        private BrightIdeasSoftware.BaseRenderer baseRenderer1;
        private ToolStripMenuItem toolStripMenuItem1;
        public TabControl tabControlactions;
        public TabPage tabQuick;
        private TableLayoutPanel tableLayoutPanelQuick;
        private Controls.QuickView quickView7;
        private Controls.QuickView quickView8;
        private Controls.QuickView quickView6;
        private Controls.QuickView quickView5;
        private Controls.QuickView quickView4;
        private Controls.QuickView quickView3;
        private Controls.QuickView quickView2;
        private Controls.QuickView quickView1;
        public TabPage tabActions;
        private TableLayoutPanel tableLayoutPanel1;
        private Controls.MyButton BUT_abortland;
        private Controls.ModifyandSet modifyandSetLoiterRad;
        private Controls.MyButton BUT_clear_track;
        private ComboBox CMB_action;
        private Controls.MyButton BUTactiondo;
        private Controls.MyButton BUT_resumemis;
        private Controls.ModifyandSet modifyandSetAlt;
        private Controls.ModifyandSet modifyandSetSpeed;
        private ComboBox CMB_setwp;
        private Controls.MyButton BUT_ARM;
        private Controls.MyButton BUT_mountmode;
        private Controls.MyButton BUT_joystick;
        private Controls.MyButton BUT_RAWSensor;
        private Controls.MyButton BUT_Homealt;
        private Controls.MyButton BUTrestartmission;
        private ComboBox CMB_mountmode;
        private Controls.MyButton BUT_quickrtl;
        private Controls.MyButton BUT_quickmanual;
        private Controls.MyButton BUT_setwp;
        private ComboBox CMB_modes;
        private Controls.MyButton BUT_quickauto;
        private Controls.MyButton BUT_setmode;
        public TabPage tabActionsSimple;
        private Controls.MyButton myButton1;
        private Controls.MyButton myButton2;
        private Controls.MyButton myButton3;
        private TabPage tabPayload;
        private Controls.MyButton BUT_PayloadFolder;
        private GroupBox groupBoxRoll;
        private TextBox TXT_gimbalRollPos;
        private TrackBar trackBarRoll;
        private GroupBox groupBoxYaw;
        private TextBox TXT_gimbalYawPos;
        private TrackBar trackBarYaw;
        private Controls.MyButton BUT_resetGimbalPos;
        private GroupBox groupBoxPitch;
        private TrackBar trackBarPitch;
        private TextBox TXT_gimbalPitchPos;
        public TabPage tabTLogs;
        private TableLayoutPanel tableLayoutPaneltlogs;
        private Panel panel2;
        private Label label2;
        private Controls.MyButton BUT_speed10;
        private Controls.MyButton BUT_speed5;
        private Controls.MyButton BUT_speed2;
        private Controls.MyButton BUT_speed1;
        private Controls.MyButton BUT_speed1_2;
        private Controls.MyButton BUT_speed1_4;
        private Controls.MyButton BUT_speed1_10;
        private Controls.MyButton BUT_loadtelem;
        private Label lbl_playbackspeed;
        private Label lbl_logpercent;
        private Label LBL_logfn;
        private Controls.MyButton BUT_log2kml;
        private Controls.MyButton BUT_playlog;
        private TrackBar tracklog;
    }
}
