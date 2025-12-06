using MissionPlanner.Controls;
namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigRadioInput
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigRadioInput));
            this.groupBoxElevons = new System.Windows.Forms.GroupBox();
            this.CHK_mixmode = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_elevonch2rev = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_elevonrev = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_elevonch1rev = new MissionPlanner.Controls.MavlinkCheckBox();
            this.BUT_BindDSM8 = new MissionPlanner.Controls.MyButton();
            this.BUT_BindDSMX = new MissionPlanner.Controls.MyButton();
            this.BUT_BindDSM2 = new MissionPlanner.Controls.MyButton();
            this.BUT_Calibrateradio = new MissionPlanner.Controls.MyButton();
            this.BAR8 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BAR7 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BAR6 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BAR5 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BARpitch = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BARthrottle = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BARyaw = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BARroll = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BAR9 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BAR16 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BAR15 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BAR14 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BAR13 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BAR12 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BAR11 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.BAR10 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.currentStateBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelSticks = new System.Windows.Forms.Panel();
            this.sticksTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.stickLeft = new MissionPlanner.Controls.RCStickControl();
            this.stickRight = new MissionPlanner.Controls.RCStickControl();
            this.groupBoxChannels = new System.Windows.Forms.GroupBox();
            this.tableLayoutChannels = new System.Windows.Forms.TableLayoutPanel();
            this.CHK_revroll = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev9 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_revpitch = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev10 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_revthr = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev11 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_revyaw = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev12 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev5 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev13 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev6 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev14 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev7 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev15 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev8 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.CHK_rev16 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.panelControls = new System.Windows.Forms.Panel();
            this.controlsTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.scrollPanel = new System.Windows.Forms.Panel();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.groupBoxElevons.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currentStateBindingSource)).BeginInit();
            this.tableLayoutMain.SuspendLayout();
            this.panelSticks.SuspendLayout();
            this.sticksTableLayout.SuspendLayout();
            this.groupBoxChannels.SuspendLayout();
            this.tableLayoutChannels.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.controlsTableLayout.SuspendLayout();
            this.scrollPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxElevons
            // 
            this.groupBoxElevons.Controls.Add(this.CHK_mixmode);
            this.groupBoxElevons.Controls.Add(this.CHK_elevonch2rev);
            this.groupBoxElevons.Controls.Add(this.CHK_elevonrev);
            this.groupBoxElevons.Controls.Add(this.CHK_elevonch1rev);
            resources.ApplyResources(this.groupBoxElevons, "groupBoxElevons");
            this.groupBoxElevons.Name = "groupBoxElevons";
            this.groupBoxElevons.TabStop = false;
            // 
            // CHK_mixmode
            // 
            resources.ApplyResources(this.CHK_mixmode, "CHK_mixmode");
            this.CHK_mixmode.Name = "CHK_mixmode";
            this.CHK_mixmode.OffValue = 0D;
            this.CHK_mixmode.OnValue = 1D;
            this.CHK_mixmode.ParamName = null;
            this.CHK_mixmode.UseVisualStyleBackColor = true;
            // 
            // CHK_elevonch2rev
            // 
            resources.ApplyResources(this.CHK_elevonch2rev, "CHK_elevonch2rev");
            this.CHK_elevonch2rev.Name = "CHK_elevonch2rev";
            this.CHK_elevonch2rev.OffValue = 0D;
            this.CHK_elevonch2rev.OnValue = 1D;
            this.CHK_elevonch2rev.ParamName = null;
            this.CHK_elevonch2rev.UseVisualStyleBackColor = true;
            // 
            // CHK_elevonrev
            // 
            resources.ApplyResources(this.CHK_elevonrev, "CHK_elevonrev");
            this.CHK_elevonrev.Name = "CHK_elevonrev";
            this.CHK_elevonrev.OffValue = 0D;
            this.CHK_elevonrev.OnValue = 1D;
            this.CHK_elevonrev.ParamName = null;
            this.CHK_elevonrev.UseVisualStyleBackColor = true;
            // 
            // CHK_elevonch1rev
            // 
            resources.ApplyResources(this.CHK_elevonch1rev, "CHK_elevonch1rev");
            this.CHK_elevonch1rev.Name = "CHK_elevonch1rev";
            this.CHK_elevonch1rev.OffValue = 0D;
            this.CHK_elevonch1rev.OnValue = 1D;
            this.CHK_elevonch1rev.ParamName = null;
            this.CHK_elevonch1rev.UseVisualStyleBackColor = true;
            // 
            // BUT_BindDSM8
            // 
            resources.ApplyResources(this.BUT_BindDSM8, "BUT_BindDSM8");
            this.BUT_BindDSM8.Name = "BUT_BindDSM8";
            this.BUT_BindDSM8.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_BindDSM8.UseVisualStyleBackColor = true;
            this.BUT_BindDSM8.Click += new System.EventHandler(this.BUT_Bindradiodsm8_Click);
            // 
            // BUT_BindDSMX
            // 
            resources.ApplyResources(this.BUT_BindDSMX, "BUT_BindDSMX");
            this.BUT_BindDSMX.Name = "BUT_BindDSMX";
            this.BUT_BindDSMX.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_BindDSMX.UseVisualStyleBackColor = true;
            this.BUT_BindDSMX.Click += new System.EventHandler(this.BUT_BindradiodsmX_Click);
            // 
            // BUT_BindDSM2
            // 
            resources.ApplyResources(this.BUT_BindDSM2, "BUT_BindDSM2");
            this.BUT_BindDSM2.Name = "BUT_BindDSM2";
            this.BUT_BindDSM2.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_BindDSM2.UseVisualStyleBackColor = true;
            this.BUT_BindDSM2.Click += new System.EventHandler(this.BUT_Bindradiodsm2_Click);
            // 
            // BUT_Calibrateradio
            // 
            resources.ApplyResources(this.BUT_Calibrateradio, "BUT_Calibrateradio");
            this.BUT_Calibrateradio.Name = "BUT_Calibrateradio";
            this.BUT_Calibrateradio.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Calibrateradio.UseVisualStyleBackColor = true;
            this.BUT_Calibrateradio.Click += new System.EventHandler(this.BUT_Calibrateradio_Click);
            // 
            // BAR8
            // 
            this.BAR8.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR8.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR8.DisplayScale = 1F;
            resources.ApplyResources(this.BAR8, "BAR8");
            this.BAR8.DrawLabel = true;
            this.BAR8.Label = "CH 8";
            this.BAR8.Maximum = 2200;
            this.BAR8.maxline = 0;
            this.BAR8.Minimum = 800;
            this.BAR8.minline = 0;
            this.BAR8.Name = "BAR8";
            this.BAR8.Value = 1500;
            this.BAR8.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BAR7
            // 
            this.BAR7.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR7.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR7.DisplayScale = 1F;
            resources.ApplyResources(this.BAR7, "BAR7");
            this.BAR7.DrawLabel = true;
            this.BAR7.Label = "CH 7";
            this.BAR7.Maximum = 2200;
            this.BAR7.maxline = 0;
            this.BAR7.Minimum = 800;
            this.BAR7.minline = 0;
            this.BAR7.Name = "BAR7";
            this.BAR7.Value = 1500;
            this.BAR7.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BAR6
            // 
            this.BAR6.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR6.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR6.DisplayScale = 1F;
            resources.ApplyResources(this.BAR6, "BAR6");
            this.BAR6.DrawLabel = true;
            this.BAR6.Label = "CH 6";
            this.BAR6.Maximum = 2200;
            this.BAR6.maxline = 0;
            this.BAR6.Minimum = 800;
            this.BAR6.minline = 0;
            this.BAR6.Name = "BAR6";
            this.BAR6.Value = 1500;
            this.BAR6.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BAR5
            // 
            this.BAR5.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR5.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR5.DisplayScale = 1F;
            resources.ApplyResources(this.BAR5, "BAR5");
            this.BAR5.DrawLabel = true;
            this.BAR5.Label = "CH 5";
            this.BAR5.Maximum = 2200;
            this.BAR5.maxline = 0;
            this.BAR5.Minimum = 800;
            this.BAR5.minline = 0;
            this.BAR5.Name = "BAR5";
            this.BAR5.Value = 1500;
            this.BAR5.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BARpitch
            // 
            this.BARpitch.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.BARpitch.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BARpitch.DisplayScale = 1F;
            resources.ApplyResources(this.BARpitch, "BARpitch");
            this.BARpitch.DrawLabel = true;
            this.BARpitch.Label = "Pitch";
            this.BARpitch.Maximum = 2200;
            this.BARpitch.maxline = 0;
            this.BARpitch.Minimum = 800;
            this.BARpitch.minline = 0;
            this.BARpitch.Name = "BARpitch";
            this.BARpitch.Value = 1500;
            this.BARpitch.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BARthrottle
            // 
            this.BARthrottle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BARthrottle.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BARthrottle.DisplayScale = 1F;
            resources.ApplyResources(this.BARthrottle, "BARthrottle");
            this.BARthrottle.DrawLabel = true;
            this.BARthrottle.Label = "Throttle";
            this.BARthrottle.Maximum = 2200;
            this.BARthrottle.maxline = 0;
            this.BARthrottle.Minimum = 800;
            this.BARthrottle.minline = 0;
            this.BARthrottle.Name = "BARthrottle";
            this.BARthrottle.Value = 1000;
            this.BARthrottle.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BARyaw
            // 
            this.BARyaw.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.BARyaw.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BARyaw.DisplayScale = 1F;
            resources.ApplyResources(this.BARyaw, "BARyaw");
            this.BARyaw.DrawLabel = true;
            this.BARyaw.Label = "Yaw";
            this.BARyaw.Maximum = 2200;
            this.BARyaw.maxline = 0;
            this.BARyaw.Minimum = 800;
            this.BARyaw.minline = 0;
            this.BARyaw.Name = "BARyaw";
            this.BARyaw.Value = 1500;
            this.BARyaw.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BARroll
            // 
            this.BARroll.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.BARroll.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BARroll.DisplayScale = 1F;
            resources.ApplyResources(this.BARroll, "BARroll");
            this.BARroll.DrawLabel = true;
            this.BARroll.Label = "Roll";
            this.BARroll.Maximum = 2200;
            this.BARroll.maxline = 0;
            this.BARroll.Minimum = 800;
            this.BARroll.minline = 0;
            this.BARroll.Name = "BARroll";
            this.BARroll.Value = 1500;
            this.BARroll.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BUT_BindDSM2);
            this.groupBox1.Controls.Add(this.BUT_BindDSM8);
            this.groupBox1.Controls.Add(this.BUT_BindDSMX);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // BAR9
            // 
            this.BAR9.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR9.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR9.DisplayScale = 1F;
            resources.ApplyResources(this.BAR9, "BAR9");
            this.BAR9.DrawLabel = true;
            this.BAR9.Label = "CH 9";
            this.BAR9.Maximum = 2200;
            this.BAR9.maxline = 0;
            this.BAR9.Minimum = 800;
            this.BAR9.minline = 0;
            this.BAR9.Name = "BAR9";
            this.BAR9.Value = 1500;
            this.BAR9.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BAR16
            // 
            this.BAR16.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR16.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR16.DisplayScale = 1F;
            resources.ApplyResources(this.BAR16, "BAR16");
            this.BAR16.DrawLabel = true;
            this.BAR16.Label = "CH 16";
            this.BAR16.Maximum = 2200;
            this.BAR16.maxline = 0;
            this.BAR16.Minimum = 800;
            this.BAR16.minline = 0;
            this.BAR16.Name = "BAR16";
            this.BAR16.Value = 1500;
            this.BAR16.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BAR15
            // 
            this.BAR15.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR15.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR15.DisplayScale = 1F;
            resources.ApplyResources(this.BAR15, "BAR15");
            this.BAR15.DrawLabel = true;
            this.BAR15.Label = "CH 15";
            this.BAR15.Maximum = 2200;
            this.BAR15.maxline = 0;
            this.BAR15.Minimum = 800;
            this.BAR15.minline = 0;
            this.BAR15.Name = "BAR15";
            this.BAR15.Value = 1500;
            this.BAR15.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BAR14
            // 
            this.BAR14.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR14.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR14.DisplayScale = 1F;
            resources.ApplyResources(this.BAR14, "BAR14");
            this.BAR14.DrawLabel = true;
            this.BAR14.Label = "CH 14";
            this.BAR14.Maximum = 2200;
            this.BAR14.maxline = 0;
            this.BAR14.Minimum = 800;
            this.BAR14.minline = 0;
            this.BAR14.Name = "BAR14";
            this.BAR14.Value = 1500;
            this.BAR14.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BAR13
            // 
            this.BAR13.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR13.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR13.DisplayScale = 1F;
            resources.ApplyResources(this.BAR13, "BAR13");
            this.BAR13.DrawLabel = true;
            this.BAR13.Label = "CH 13";
            this.BAR13.Maximum = 2200;
            this.BAR13.maxline = 0;
            this.BAR13.Minimum = 800;
            this.BAR13.minline = 0;
            this.BAR13.Name = "BAR13";
            this.BAR13.Value = 1500;
            this.BAR13.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BAR12
            // 
            this.BAR12.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR12.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR12.DisplayScale = 1F;
            resources.ApplyResources(this.BAR12, "BAR12");
            this.BAR12.DrawLabel = true;
            this.BAR12.Label = "CH 12";
            this.BAR12.Maximum = 2200;
            this.BAR12.maxline = 0;
            this.BAR12.Minimum = 800;
            this.BAR12.minline = 0;
            this.BAR12.Name = "BAR12";
            this.BAR12.Value = 1500;
            this.BAR12.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BAR11
            // 
            this.BAR11.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR11.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR11.DisplayScale = 1F;
            resources.ApplyResources(this.BAR11, "BAR11");
            this.BAR11.DrawLabel = true;
            this.BAR11.Label = "CH 11";
            this.BAR11.Maximum = 2200;
            this.BAR11.maxline = 0;
            this.BAR11.Minimum = 800;
            this.BAR11.minline = 0;
            this.BAR11.Name = "BAR11";
            this.BAR11.Value = 1500;
            this.BAR11.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // BAR10
            // 
            this.BAR10.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.BAR10.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.BAR10.DisplayScale = 1F;
            resources.ApplyResources(this.BAR10, "BAR10");
            this.BAR10.DrawLabel = true;
            this.BAR10.Label = "CH 10";
            this.BAR10.Maximum = 2200;
            this.BAR10.maxline = 0;
            this.BAR10.Minimum = 800;
            this.BAR10.minline = 0;
            this.BAR10.Name = "BAR10";
            this.BAR10.Value = 1500;
            this.BAR10.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            // 
            // currentStateBindingSource
            // 
            this.currentStateBindingSource.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // tableLayoutMain
            // 
            resources.ApplyResources(this.tableLayoutMain, "tableLayoutMain");
            this.tableLayoutMain.Controls.Add(this.panelSticks, 0, 0);
            this.tableLayoutMain.Controls.Add(this.groupBoxChannels, 0, 1);
            this.tableLayoutMain.Controls.Add(this.panelControls, 0, 2);
            this.tableLayoutMain.Name = "tableLayoutMain";
            // 
            // panelSticks
            // 
            this.panelSticks.Controls.Add(this.sticksTableLayout);
            resources.ApplyResources(this.panelSticks, "panelSticks");
            this.panelSticks.Name = "panelSticks";
            // 
            // sticksTableLayout
            // 
            resources.ApplyResources(this.sticksTableLayout, "sticksTableLayout");
            this.sticksTableLayout.Controls.Add(this.stickLeft, 0, 0);
            this.sticksTableLayout.Controls.Add(this.stickRight, 1, 0);
            this.sticksTableLayout.Name = "sticksTableLayout";
            // 
            // stickLeft
            // 
            resources.ApplyResources(this.stickLeft, "stickLeft");
            this.stickLeft.HorizontalLabel = "Yaw";
            this.stickLeft.HorizontalMaxLine = 0;
            this.stickLeft.HorizontalMinLine = 0;
            this.stickLeft.Name = "stickLeft";
            this.stickLeft.VerticalLabel = "Throttle";
            this.stickLeft.VerticalMaxLine = 0;
            this.stickLeft.VerticalMinLine = 0;
            this.stickLeft.VerticalValue = 1000;
            // 
            // stickRight
            // 
            resources.ApplyResources(this.stickRight, "stickRight");
            this.stickRight.HorizontalLabel = "Roll";
            this.stickRight.HorizontalMaxLine = 0;
            this.stickRight.HorizontalMinLine = 0;
            this.stickRight.Name = "stickRight";
            this.stickRight.VerticalLabel = "Pitch";
            this.stickRight.VerticalMaxLine = 0;
            this.stickRight.VerticalMinLine = 0;
            // 
            // groupBoxChannels
            // 
            resources.ApplyResources(this.groupBoxChannels, "groupBoxChannels");
            this.groupBoxChannels.Controls.Add(this.tableLayoutChannels);
            this.groupBoxChannels.Name = "groupBoxChannels";
            this.groupBoxChannels.TabStop = false;
            // 
            // tableLayoutChannels
            // 
            resources.ApplyResources(this.tableLayoutChannels, "tableLayoutChannels");
            this.tableLayoutChannels.Controls.Add(this.BARroll, 0, 0);
            this.tableLayoutChannels.Controls.Add(this.CHK_revroll, 1, 0);
            this.tableLayoutChannels.Controls.Add(this.BAR9, 2, 0);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev9, 3, 0);
            this.tableLayoutChannels.Controls.Add(this.BARpitch, 0, 1);
            this.tableLayoutChannels.Controls.Add(this.CHK_revpitch, 1, 1);
            this.tableLayoutChannels.Controls.Add(this.BAR10, 2, 1);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev10, 3, 1);
            this.tableLayoutChannels.Controls.Add(this.BARthrottle, 0, 2);
            this.tableLayoutChannels.Controls.Add(this.CHK_revthr, 1, 2);
            this.tableLayoutChannels.Controls.Add(this.BAR11, 2, 2);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev11, 3, 2);
            this.tableLayoutChannels.Controls.Add(this.BARyaw, 0, 3);
            this.tableLayoutChannels.Controls.Add(this.CHK_revyaw, 1, 3);
            this.tableLayoutChannels.Controls.Add(this.BAR12, 2, 3);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev12, 3, 3);
            this.tableLayoutChannels.Controls.Add(this.BAR5, 0, 4);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev5, 1, 4);
            this.tableLayoutChannels.Controls.Add(this.BAR13, 2, 4);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev13, 3, 4);
            this.tableLayoutChannels.Controls.Add(this.BAR6, 0, 5);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev6, 1, 5);
            this.tableLayoutChannels.Controls.Add(this.BAR14, 2, 5);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev14, 3, 5);
            this.tableLayoutChannels.Controls.Add(this.BAR7, 0, 6);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev7, 1, 6);
            this.tableLayoutChannels.Controls.Add(this.BAR15, 2, 6);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev15, 3, 6);
            this.tableLayoutChannels.Controls.Add(this.BAR8, 0, 7);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev8, 1, 7);
            this.tableLayoutChannels.Controls.Add(this.BAR16, 2, 7);
            this.tableLayoutChannels.Controls.Add(this.CHK_rev16, 3, 7);
            this.tableLayoutChannels.Name = "tableLayoutChannels";
            // 
            // CHK_revroll
            // 
            resources.ApplyResources(this.CHK_revroll, "CHK_revroll");
            this.CHK_revroll.Name = "CHK_revroll";
            this.CHK_revroll.OffValue = 0D;
            this.CHK_revroll.OnValue = 1D;
            this.CHK_revroll.ParamName = null;
            this.CHK_revroll.UseVisualStyleBackColor = true;
            // 
            // CHK_rev9
            // 
            resources.ApplyResources(this.CHK_rev9, "CHK_rev9");
            this.CHK_rev9.Name = "CHK_rev9";
            this.CHK_rev9.OffValue = 0D;
            this.CHK_rev9.OnValue = 1D;
            this.CHK_rev9.ParamName = null;
            this.CHK_rev9.UseVisualStyleBackColor = true;
            // 
            // CHK_revpitch
            // 
            resources.ApplyResources(this.CHK_revpitch, "CHK_revpitch");
            this.CHK_revpitch.Name = "CHK_revpitch";
            this.CHK_revpitch.OffValue = 0D;
            this.CHK_revpitch.OnValue = 1D;
            this.CHK_revpitch.ParamName = null;
            this.CHK_revpitch.UseVisualStyleBackColor = true;
            // 
            // CHK_rev10
            // 
            resources.ApplyResources(this.CHK_rev10, "CHK_rev10");
            this.CHK_rev10.Name = "CHK_rev10";
            this.CHK_rev10.OffValue = 0D;
            this.CHK_rev10.OnValue = 1D;
            this.CHK_rev10.ParamName = null;
            this.CHK_rev10.UseVisualStyleBackColor = true;
            // 
            // CHK_revthr
            // 
            resources.ApplyResources(this.CHK_revthr, "CHK_revthr");
            this.CHK_revthr.Name = "CHK_revthr";
            this.CHK_revthr.OffValue = 0D;
            this.CHK_revthr.OnValue = 1D;
            this.CHK_revthr.ParamName = null;
            this.CHK_revthr.UseVisualStyleBackColor = true;
            // 
            // CHK_rev11
            // 
            resources.ApplyResources(this.CHK_rev11, "CHK_rev11");
            this.CHK_rev11.Name = "CHK_rev11";
            this.CHK_rev11.OffValue = 0D;
            this.CHK_rev11.OnValue = 1D;
            this.CHK_rev11.ParamName = null;
            this.CHK_rev11.UseVisualStyleBackColor = true;
            // 
            // CHK_revyaw
            // 
            resources.ApplyResources(this.CHK_revyaw, "CHK_revyaw");
            this.CHK_revyaw.Name = "CHK_revyaw";
            this.CHK_revyaw.OffValue = 0D;
            this.CHK_revyaw.OnValue = 1D;
            this.CHK_revyaw.ParamName = null;
            this.CHK_revyaw.UseVisualStyleBackColor = true;
            // 
            // CHK_rev12
            // 
            resources.ApplyResources(this.CHK_rev12, "CHK_rev12");
            this.CHK_rev12.Name = "CHK_rev12";
            this.CHK_rev12.OffValue = 0D;
            this.CHK_rev12.OnValue = 1D;
            this.CHK_rev12.ParamName = null;
            this.CHK_rev12.UseVisualStyleBackColor = true;
            // 
            // CHK_rev5
            // 
            resources.ApplyResources(this.CHK_rev5, "CHK_rev5");
            this.CHK_rev5.Name = "CHK_rev5";
            this.CHK_rev5.OffValue = 0D;
            this.CHK_rev5.OnValue = 1D;
            this.CHK_rev5.ParamName = null;
            this.CHK_rev5.UseVisualStyleBackColor = true;
            // 
            // CHK_rev13
            // 
            resources.ApplyResources(this.CHK_rev13, "CHK_rev13");
            this.CHK_rev13.Name = "CHK_rev13";
            this.CHK_rev13.OffValue = 0D;
            this.CHK_rev13.OnValue = 1D;
            this.CHK_rev13.ParamName = null;
            this.CHK_rev13.UseVisualStyleBackColor = true;
            // 
            // CHK_rev6
            // 
            resources.ApplyResources(this.CHK_rev6, "CHK_rev6");
            this.CHK_rev6.Name = "CHK_rev6";
            this.CHK_rev6.OffValue = 0D;
            this.CHK_rev6.OnValue = 1D;
            this.CHK_rev6.ParamName = null;
            this.CHK_rev6.UseVisualStyleBackColor = true;
            // 
            // CHK_rev14
            // 
            resources.ApplyResources(this.CHK_rev14, "CHK_rev14");
            this.CHK_rev14.Name = "CHK_rev14";
            this.CHK_rev14.OffValue = 0D;
            this.CHK_rev14.OnValue = 1D;
            this.CHK_rev14.ParamName = null;
            this.CHK_rev14.UseVisualStyleBackColor = true;
            // 
            // CHK_rev7
            // 
            resources.ApplyResources(this.CHK_rev7, "CHK_rev7");
            this.CHK_rev7.Name = "CHK_rev7";
            this.CHK_rev7.OffValue = 0D;
            this.CHK_rev7.OnValue = 1D;
            this.CHK_rev7.ParamName = null;
            this.CHK_rev7.UseVisualStyleBackColor = true;
            // 
            // CHK_rev15
            // 
            resources.ApplyResources(this.CHK_rev15, "CHK_rev15");
            this.CHK_rev15.Name = "CHK_rev15";
            this.CHK_rev15.OffValue = 0D;
            this.CHK_rev15.OnValue = 1D;
            this.CHK_rev15.ParamName = null;
            this.CHK_rev15.UseVisualStyleBackColor = true;
            // 
            // CHK_rev8
            // 
            resources.ApplyResources(this.CHK_rev8, "CHK_rev8");
            this.CHK_rev8.Name = "CHK_rev8";
            this.CHK_rev8.OffValue = 0D;
            this.CHK_rev8.OnValue = 1D;
            this.CHK_rev8.ParamName = null;
            this.CHK_rev8.UseVisualStyleBackColor = true;
            // 
            // CHK_rev16
            // 
            resources.ApplyResources(this.CHK_rev16, "CHK_rev16");
            this.CHK_rev16.Name = "CHK_rev16";
            this.CHK_rev16.OffValue = 0D;
            this.CHK_rev16.OnValue = 1D;
            this.CHK_rev16.ParamName = null;
            this.CHK_rev16.UseVisualStyleBackColor = true;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.controlsTableLayout);
            resources.ApplyResources(this.panelControls, "panelControls");
            this.panelControls.Name = "panelControls";
            // 
            // controlsTableLayout
            // 
            resources.ApplyResources(this.controlsTableLayout, "controlsTableLayout");
            this.controlsTableLayout.Controls.Add(this.groupBox1, 0, 0);
            this.controlsTableLayout.Controls.Add(this.groupBoxElevons, 1, 0);
            this.controlsTableLayout.Controls.Add(this.BUT_Calibrateradio, 2, 0);
            this.controlsTableLayout.Name = "controlsTableLayout";
            // 
            // scrollPanel
            // 
            resources.ApplyResources(this.scrollPanel, "scrollPanel");
            this.scrollPanel.Controls.Add(this.contentPanel);
            this.scrollPanel.Name = "scrollPanel";
            // 
            // contentPanel
            // 
            resources.ApplyResources(this.contentPanel, "contentPanel");
            this.contentPanel.Controls.Add(this.tableLayoutMain);
            this.contentPanel.Name = "contentPanel";
            // 
            // ConfigRadioInput
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.scrollPanel);
            this.Name = "ConfigRadioInput";
            this.groupBoxElevons.ResumeLayout(false);
            this.groupBoxElevons.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.currentStateBindingSource)).EndInit();
            this.tableLayoutMain.ResumeLayout(false);
            this.tableLayoutMain.PerformLayout();
            this.panelSticks.ResumeLayout(false);
            this.sticksTableLayout.ResumeLayout(false);
            this.groupBoxChannels.ResumeLayout(false);
            this.groupBoxChannels.PerformLayout();
            this.tableLayoutChannels.ResumeLayout(false);
            this.tableLayoutChannels.PerformLayout();
            this.panelControls.ResumeLayout(false);
            this.controlsTableLayout.ResumeLayout(false);
            this.controlsTableLayout.PerformLayout();
            this.scrollPanel.ResumeLayout(false);
            this.scrollPanel.PerformLayout();
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxElevons;
        private MavlinkCheckBox CHK_mixmode;
        private MavlinkCheckBox CHK_elevonch2rev;
        private MavlinkCheckBox CHK_elevonrev;
        private MavlinkCheckBox CHK_elevonch1rev;
        private MavlinkCheckBox CHK_revthr;
        private MavlinkCheckBox CHK_revyaw;
        private MavlinkCheckBox CHK_revpitch;
        private MavlinkCheckBox CHK_revroll;
        private Controls.MyButton BUT_Calibrateradio;
        private HorizontalProgressBar2 BAR8;
        private HorizontalProgressBar2 BAR7;
        private HorizontalProgressBar2 BAR6;
        private HorizontalProgressBar2 BAR5;
        private HorizontalProgressBar2 BARpitch;
        private HorizontalProgressBar2 BARthrottle;
        private HorizontalProgressBar2 BARyaw;
        private HorizontalProgressBar2 BARroll;
        private System.Windows.Forms.BindingSource currentStateBindingSource;
        private MyButton BUT_BindDSM2;
        private MyButton BUT_BindDSMX;
        private MyButton BUT_BindDSM8;
        private System.Windows.Forms.GroupBox groupBox1;
        private HorizontalProgressBar2 BAR9;
        private HorizontalProgressBar2 BAR16;
        private HorizontalProgressBar2 BAR15;
        private HorizontalProgressBar2 BAR14;
        private HorizontalProgressBar2 BAR13;
        private HorizontalProgressBar2 BAR12;
        private HorizontalProgressBar2 BAR11;
        private HorizontalProgressBar2 BAR10;
        private MavlinkCheckBox CHK_rev5;
        private MavlinkCheckBox CHK_rev6;
        private MavlinkCheckBox CHK_rev7;
        private MavlinkCheckBox CHK_rev8;
        private MavlinkCheckBox CHK_rev9;
        private MavlinkCheckBox CHK_rev10;
        private MavlinkCheckBox CHK_rev11;
        private MavlinkCheckBox CHK_rev12;
        private MavlinkCheckBox CHK_rev13;
        private MavlinkCheckBox CHK_rev14;
        private MavlinkCheckBox CHK_rev15;
        private MavlinkCheckBox CHK_rev16;
        private System.Windows.Forms.Panel panelSticks;
        private RCStickControl stickLeft;
        private RCStickControl stickRight;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.TableLayoutPanel tableLayoutChannels;
        private System.Windows.Forms.Panel scrollPanel;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.GroupBox groupBoxChannels;
        private System.Windows.Forms.TableLayoutPanel sticksTableLayout;
        private System.Windows.Forms.TableLayoutPanel controlsTableLayout;
    }
}
