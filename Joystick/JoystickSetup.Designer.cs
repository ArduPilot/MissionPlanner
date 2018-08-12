using MissionPlanner.Controls;
namespace MissionPlanner.Joystick
{
    partial class JoystickSetup
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JoystickSetup));
            this.CMB_joysticks = new System.Windows.Forms.ComboBox();
            this.CMB_CH1 = new System.Windows.Forms.ComboBox();
            this.CMB_CH2 = new System.Windows.Forms.ComboBox();
            this.CMB_CH3 = new System.Windows.Forms.ComboBox();
            this.CMB_CH4 = new System.Windows.Forms.ComboBox();
            this.expo_ch1 = new System.Windows.Forms.TextBox();
            this.expo_ch2 = new System.Windows.Forms.TextBox();
            this.expo_ch3 = new System.Windows.Forms.TextBox();
            this.expo_ch4 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.revCH1 = new System.Windows.Forms.CheckBox();
            this.revCH2 = new System.Windows.Forms.CheckBox();
            this.revCH3 = new System.Windows.Forms.CheckBox();
            this.revCH4 = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.CHK_elevons = new System.Windows.Forms.CheckBox();
            this.revCH5 = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.expo_ch5 = new System.Windows.Forms.TextBox();
            this.CMB_CH5 = new System.Windows.Forms.ComboBox();
            this.revCH6 = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.expo_ch6 = new System.Windows.Forms.TextBox();
            this.CMB_CH6 = new System.Windows.Forms.ComboBox();
            this.revCH7 = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.expo_ch7 = new System.Windows.Forms.TextBox();
            this.CMB_CH7 = new System.Windows.Forms.ComboBox();
            this.revCH8 = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.expo_ch8 = new System.Windows.Forms.TextBox();
            this.CMB_CH8 = new System.Windows.Forms.ComboBox();
            this.BUT_detch8 = new MissionPlanner.Controls.MyButton();
            this.ProgressBarCH8 = new MissionPlanner.Controls.HorizontalProgressBar();
            this.BUT_detch4 = new MissionPlanner.Controls.MyButton();
            this.BUT_detch3 = new MissionPlanner.Controls.MyButton();
            this.BUT_detch2 = new MissionPlanner.Controls.MyButton();
            this.BUT_detch1 = new MissionPlanner.Controls.MyButton();
            this.BUT_enable = new MissionPlanner.Controls.MyButton();
            this.BUT_save = new MissionPlanner.Controls.MyButton();
            this.progressBarRudder = new MissionPlanner.Controls.HorizontalProgressBar();
            this.progressBarThrottle = new MissionPlanner.Controls.HorizontalProgressBar();
            this.progressBarPith = new MissionPlanner.Controls.HorizontalProgressBar();
            this.progressBarRoll = new MissionPlanner.Controls.HorizontalProgressBar();
            this.BUT_detch5 = new MissionPlanner.Controls.MyButton();
            this.ProgressBarCH5 = new MissionPlanner.Controls.HorizontalProgressBar();
            this.BUT_detch6 = new MissionPlanner.Controls.MyButton();
            this.ProgressBarCH6 = new MissionPlanner.Controls.HorizontalProgressBar();
            this.BUT_detch7 = new MissionPlanner.Controls.MyButton();
            this.ProgressBarCH7 = new MissionPlanner.Controls.HorizontalProgressBar();
            this.label14 = new System.Windows.Forms.Label();
            this.chk_manualcontrol = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // CMB_joysticks
            // 
            this.CMB_joysticks.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_joysticks, "CMB_joysticks");
            this.CMB_joysticks.Name = "CMB_joysticks";
            this.CMB_joysticks.SelectedIndexChanged += new System.EventHandler(this.CMB_joysticks_SelectedIndexChanged);
            this.CMB_joysticks.Click += new System.EventHandler(this.CMB_joysticks_Click);
            // 
            // CMB_CH1
            // 
            this.CMB_CH1.FormattingEnabled = true;
            this.CMB_CH1.Items.AddRange(new object[] {
            resources.GetString("CMB_CH1.Items"),
            resources.GetString("CMB_CH1.Items1"),
            resources.GetString("CMB_CH1.Items2"),
            resources.GetString("CMB_CH1.Items3")});
            resources.ApplyResources(this.CMB_CH1, "CMB_CH1");
            this.CMB_CH1.Name = "CMB_CH1";
            this.CMB_CH1.SelectedIndexChanged += new System.EventHandler(this.CMB_CH1_SelectedIndexChanged);
            // 
            // CMB_CH2
            // 
            this.CMB_CH2.FormattingEnabled = true;
            this.CMB_CH2.Items.AddRange(new object[] {
            resources.GetString("CMB_CH2.Items"),
            resources.GetString("CMB_CH2.Items1"),
            resources.GetString("CMB_CH2.Items2"),
            resources.GetString("CMB_CH2.Items3")});
            resources.ApplyResources(this.CMB_CH2, "CMB_CH2");
            this.CMB_CH2.Name = "CMB_CH2";
            this.CMB_CH2.SelectedIndexChanged += new System.EventHandler(this.CMB_CH2_SelectedIndexChanged);
            // 
            // CMB_CH3
            // 
            this.CMB_CH3.FormattingEnabled = true;
            this.CMB_CH3.Items.AddRange(new object[] {
            resources.GetString("CMB_CH3.Items"),
            resources.GetString("CMB_CH3.Items1"),
            resources.GetString("CMB_CH3.Items2"),
            resources.GetString("CMB_CH3.Items3")});
            resources.ApplyResources(this.CMB_CH3, "CMB_CH3");
            this.CMB_CH3.Name = "CMB_CH3";
            this.CMB_CH3.SelectedIndexChanged += new System.EventHandler(this.CMB_CH3_SelectedIndexChanged);
            // 
            // CMB_CH4
            // 
            this.CMB_CH4.FormattingEnabled = true;
            this.CMB_CH4.Items.AddRange(new object[] {
            resources.GetString("CMB_CH4.Items"),
            resources.GetString("CMB_CH4.Items1"),
            resources.GetString("CMB_CH4.Items2"),
            resources.GetString("CMB_CH4.Items3")});
            resources.ApplyResources(this.CMB_CH4, "CMB_CH4");
            this.CMB_CH4.Name = "CMB_CH4";
            this.CMB_CH4.SelectedIndexChanged += new System.EventHandler(this.CMB_CH4_SelectedIndexChanged);
            // 
            // expo_ch1
            // 
            this.expo_ch1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.expo_ch1, "expo_ch1");
            this.expo_ch1.Name = "expo_ch1";
            // 
            // expo_ch2
            // 
            this.expo_ch2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.expo_ch2, "expo_ch2");
            this.expo_ch2.Name = "expo_ch2";
            // 
            // expo_ch3
            // 
            this.expo_ch3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.expo_ch3, "expo_ch3");
            this.expo_ch3.Name = "expo_ch3";
            // 
            // expo_ch4
            // 
            this.expo_ch4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.expo_ch4, "expo_ch4");
            this.expo_ch4.Name = "expo_ch4";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // revCH1
            // 
            resources.ApplyResources(this.revCH1, "revCH1");
            this.revCH1.Name = "revCH1";
            this.revCH1.UseVisualStyleBackColor = true;
            this.revCH1.CheckedChanged += new System.EventHandler(this.revCH1_CheckedChanged);
            // 
            // revCH2
            // 
            resources.ApplyResources(this.revCH2, "revCH2");
            this.revCH2.Name = "revCH2";
            this.revCH2.UseVisualStyleBackColor = true;
            this.revCH2.CheckedChanged += new System.EventHandler(this.revCH2_CheckedChanged);
            // 
            // revCH3
            // 
            resources.ApplyResources(this.revCH3, "revCH3");
            this.revCH3.Name = "revCH3";
            this.revCH3.UseVisualStyleBackColor = true;
            this.revCH3.CheckedChanged += new System.EventHandler(this.revCH3_CheckedChanged);
            // 
            // revCH4
            // 
            resources.ApplyResources(this.revCH4, "revCH4");
            this.revCH4.Name = "revCH4";
            this.revCH4.UseVisualStyleBackColor = true;
            this.revCH4.CheckedChanged += new System.EventHandler(this.revCH4_CheckedChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // CHK_elevons
            // 
            resources.ApplyResources(this.CHK_elevons, "CHK_elevons");
            this.CHK_elevons.Name = "CHK_elevons";
            this.CHK_elevons.UseVisualStyleBackColor = true;
            this.CHK_elevons.CheckedChanged += new System.EventHandler(this.CHK_elevons_CheckedChanged);
            // 
            // revCH5
            // 
            resources.ApplyResources(this.revCH5, "revCH5");
            this.revCH5.Name = "revCH5";
            this.revCH5.UseVisualStyleBackColor = true;
            this.revCH5.CheckedChanged += new System.EventHandler(this.revCH5_CheckedChanged);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // expo_ch5
            // 
            this.expo_ch5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.expo_ch5, "expo_ch5");
            this.expo_ch5.Name = "expo_ch5";
            // 
            // CMB_CH5
            // 
            this.CMB_CH5.FormattingEnabled = true;
            this.CMB_CH5.Items.AddRange(new object[] {
            resources.GetString("CMB_CH5.Items"),
            resources.GetString("CMB_CH5.Items1"),
            resources.GetString("CMB_CH5.Items2"),
            resources.GetString("CMB_CH5.Items3")});
            resources.ApplyResources(this.CMB_CH5, "CMB_CH5");
            this.CMB_CH5.Name = "CMB_CH5";
            this.CMB_CH5.SelectedIndexChanged += new System.EventHandler(this.CMB_CH5_SelectedIndexChanged);
            // 
            // revCH6
            // 
            resources.ApplyResources(this.revCH6, "revCH6");
            this.revCH6.Name = "revCH6";
            this.revCH6.UseVisualStyleBackColor = true;
            this.revCH6.CheckedChanged += new System.EventHandler(this.revCH6_CheckedChanged);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // expo_ch6
            // 
            this.expo_ch6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.expo_ch6, "expo_ch6");
            this.expo_ch6.Name = "expo_ch6";
            // 
            // CMB_CH6
            // 
            this.CMB_CH6.FormattingEnabled = true;
            this.CMB_CH6.Items.AddRange(new object[] {
            resources.GetString("CMB_CH6.Items"),
            resources.GetString("CMB_CH6.Items1"),
            resources.GetString("CMB_CH6.Items2"),
            resources.GetString("CMB_CH6.Items3")});
            resources.ApplyResources(this.CMB_CH6, "CMB_CH6");
            this.CMB_CH6.Name = "CMB_CH6";
            this.CMB_CH6.SelectedIndexChanged += new System.EventHandler(this.CMB_CH6_SelectedIndexChanged);
            // 
            // revCH7
            // 
            resources.ApplyResources(this.revCH7, "revCH7");
            this.revCH7.Name = "revCH7";
            this.revCH7.UseVisualStyleBackColor = true;
            this.revCH7.CheckedChanged += new System.EventHandler(this.revCH7_CheckedChanged);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // expo_ch7
            // 
            this.expo_ch7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.expo_ch7, "expo_ch7");
            this.expo_ch7.Name = "expo_ch7";
            // 
            // CMB_CH7
            // 
            this.CMB_CH7.FormattingEnabled = true;
            this.CMB_CH7.Items.AddRange(new object[] {
            resources.GetString("CMB_CH7.Items"),
            resources.GetString("CMB_CH7.Items1"),
            resources.GetString("CMB_CH7.Items2"),
            resources.GetString("CMB_CH7.Items3")});
            resources.ApplyResources(this.CMB_CH7, "CMB_CH7");
            this.CMB_CH7.Name = "CMB_CH7";
            this.CMB_CH7.SelectedIndexChanged += new System.EventHandler(this.CMB_CH7_SelectedIndexChanged);
            // 
            // revCH8
            // 
            resources.ApplyResources(this.revCH8, "revCH8");
            this.revCH8.Name = "revCH8";
            this.revCH8.UseVisualStyleBackColor = true;
            this.revCH8.CheckedChanged += new System.EventHandler(this.revCH8_CheckedChanged);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // expo_ch8
            // 
            this.expo_ch8.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.expo_ch8, "expo_ch8");
            this.expo_ch8.Name = "expo_ch8";
            // 
            // CMB_CH8
            // 
            this.CMB_CH8.FormattingEnabled = true;
            this.CMB_CH8.Items.AddRange(new object[] {
            resources.GetString("CMB_CH8.Items"),
            resources.GetString("CMB_CH8.Items1"),
            resources.GetString("CMB_CH8.Items2"),
            resources.GetString("CMB_CH8.Items3")});
            resources.ApplyResources(this.CMB_CH8, "CMB_CH8");
            this.CMB_CH8.Name = "CMB_CH8";
            this.CMB_CH8.SelectedIndexChanged += new System.EventHandler(this.CMB_CH8_SelectedIndexChanged);
            // 
            // BUT_detch8
            // 
            resources.ApplyResources(this.BUT_detch8, "BUT_detch8");
            this.BUT_detch8.Name = "BUT_detch8";
            this.BUT_detch8.UseVisualStyleBackColor = true;
            this.BUT_detch8.Click += new System.EventHandler(this.BUT_detch8_Click);
            // 
            // ProgressBarCH8
            // 
            this.ProgressBarCH8.DrawLabel = true;
            resources.ApplyResources(this.ProgressBarCH8, "ProgressBarCH8");
            this.ProgressBarCH8.Label = null;
            this.ProgressBarCH8.Maximum = 2200;
            this.ProgressBarCH8.maxline = 0;
            this.ProgressBarCH8.Minimum = 800;
            this.ProgressBarCH8.minline = 0;
            this.ProgressBarCH8.Name = "ProgressBarCH8";
            this.ProgressBarCH8.Value = 800;
            // 
            // BUT_detch4
            // 
            resources.ApplyResources(this.BUT_detch4, "BUT_detch4");
            this.BUT_detch4.Name = "BUT_detch4";
            this.BUT_detch4.UseVisualStyleBackColor = true;
            this.BUT_detch4.Click += new System.EventHandler(this.BUT_detch4_Click);
            // 
            // BUT_detch3
            // 
            resources.ApplyResources(this.BUT_detch3, "BUT_detch3");
            this.BUT_detch3.Name = "BUT_detch3";
            this.BUT_detch3.UseVisualStyleBackColor = true;
            this.BUT_detch3.Click += new System.EventHandler(this.BUT_detch3_Click);
            // 
            // BUT_detch2
            // 
            resources.ApplyResources(this.BUT_detch2, "BUT_detch2");
            this.BUT_detch2.Name = "BUT_detch2";
            this.BUT_detch2.UseVisualStyleBackColor = true;
            this.BUT_detch2.Click += new System.EventHandler(this.BUT_detch2_Click);
            // 
            // BUT_detch1
            // 
            resources.ApplyResources(this.BUT_detch1, "BUT_detch1");
            this.BUT_detch1.Name = "BUT_detch1";
            this.BUT_detch1.UseVisualStyleBackColor = true;
            this.BUT_detch1.Click += new System.EventHandler(this.BUT_detch1_Click);
            // 
            // BUT_enable
            // 
            resources.ApplyResources(this.BUT_enable, "BUT_enable");
            this.BUT_enable.Name = "BUT_enable";
            this.BUT_enable.UseVisualStyleBackColor = true;
            this.BUT_enable.Click += new System.EventHandler(this.BUT_enable_Click);
            // 
            // BUT_save
            // 
            resources.ApplyResources(this.BUT_save, "BUT_save");
            this.BUT_save.Name = "BUT_save";
            this.BUT_save.UseVisualStyleBackColor = true;
            this.BUT_save.Click += new System.EventHandler(this.BUT_save_Click);
            // 
            // progressBarRudder
            // 
            this.progressBarRudder.DrawLabel = true;
            resources.ApplyResources(this.progressBarRudder, "progressBarRudder");
            this.progressBarRudder.Label = null;
            this.progressBarRudder.Maximum = 2200;
            this.progressBarRudder.maxline = 0;
            this.progressBarRudder.Minimum = 800;
            this.progressBarRudder.minline = 0;
            this.progressBarRudder.Name = "progressBarRudder";
            this.progressBarRudder.Value = 800;
            // 
            // progressBarThrottle
            // 
            this.progressBarThrottle.DrawLabel = true;
            resources.ApplyResources(this.progressBarThrottle, "progressBarThrottle");
            this.progressBarThrottle.Label = null;
            this.progressBarThrottle.Maximum = 2200;
            this.progressBarThrottle.maxline = 0;
            this.progressBarThrottle.Minimum = 800;
            this.progressBarThrottle.minline = 0;
            this.progressBarThrottle.Name = "progressBarThrottle";
            this.progressBarThrottle.Value = 800;
            // 
            // progressBarPith
            // 
            this.progressBarPith.DrawLabel = true;
            resources.ApplyResources(this.progressBarPith, "progressBarPith");
            this.progressBarPith.Label = null;
            this.progressBarPith.Maximum = 2200;
            this.progressBarPith.maxline = 0;
            this.progressBarPith.Minimum = 800;
            this.progressBarPith.minline = 0;
            this.progressBarPith.Name = "progressBarPith";
            this.progressBarPith.Value = 800;
            // 
            // progressBarRoll
            // 
            this.progressBarRoll.DrawLabel = true;
            resources.ApplyResources(this.progressBarRoll, "progressBarRoll");
            this.progressBarRoll.Label = null;
            this.progressBarRoll.Maximum = 2200;
            this.progressBarRoll.maxline = 0;
            this.progressBarRoll.Minimum = 800;
            this.progressBarRoll.minline = 0;
            this.progressBarRoll.Name = "progressBarRoll";
            this.progressBarRoll.Value = 800;
            // 
            // BUT_detch5
            // 
            resources.ApplyResources(this.BUT_detch5, "BUT_detch5");
            this.BUT_detch5.Name = "BUT_detch5";
            this.BUT_detch5.UseVisualStyleBackColor = true;
            this.BUT_detch5.Click += new System.EventHandler(this.BUT_detch5_Click);
            // 
            // ProgressBarCH5
            // 
            this.ProgressBarCH5.DrawLabel = true;
            resources.ApplyResources(this.ProgressBarCH5, "ProgressBarCH5");
            this.ProgressBarCH5.Label = null;
            this.ProgressBarCH5.Maximum = 2200;
            this.ProgressBarCH5.maxline = 0;
            this.ProgressBarCH5.Minimum = 800;
            this.ProgressBarCH5.minline = 0;
            this.ProgressBarCH5.Name = "ProgressBarCH5";
            this.ProgressBarCH5.Value = 800;
            // 
            // BUT_detch6
            // 
            resources.ApplyResources(this.BUT_detch6, "BUT_detch6");
            this.BUT_detch6.Name = "BUT_detch6";
            this.BUT_detch6.UseVisualStyleBackColor = true;
            this.BUT_detch6.Click += new System.EventHandler(this.BUT_detch6_Click);
            // 
            // ProgressBarCH6
            // 
            this.ProgressBarCH6.DrawLabel = true;
            resources.ApplyResources(this.ProgressBarCH6, "ProgressBarCH6");
            this.ProgressBarCH6.Label = null;
            this.ProgressBarCH6.Maximum = 2200;
            this.ProgressBarCH6.maxline = 0;
            this.ProgressBarCH6.Minimum = 800;
            this.ProgressBarCH6.minline = 0;
            this.ProgressBarCH6.Name = "ProgressBarCH6";
            this.ProgressBarCH6.Value = 800;
            // 
            // BUT_detch7
            // 
            resources.ApplyResources(this.BUT_detch7, "BUT_detch7");
            this.BUT_detch7.Name = "BUT_detch7";
            this.BUT_detch7.UseVisualStyleBackColor = true;
            this.BUT_detch7.Click += new System.EventHandler(this.BUT_detch7_Click);
            // 
            // ProgressBarCH7
            // 
            this.ProgressBarCH7.DrawLabel = true;
            resources.ApplyResources(this.ProgressBarCH7, "ProgressBarCH7");
            this.ProgressBarCH7.Label = null;
            this.ProgressBarCH7.Maximum = 2200;
            this.ProgressBarCH7.maxline = 0;
            this.ProgressBarCH7.Minimum = 800;
            this.ProgressBarCH7.minline = 0;
            this.ProgressBarCH7.Name = "ProgressBarCH7";
            this.ProgressBarCH7.Value = 800;
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // chk_manualcontrol
            // 
            resources.ApplyResources(this.chk_manualcontrol, "chk_manualcontrol");
            this.chk_manualcontrol.Name = "chk_manualcontrol";
            this.chk_manualcontrol.UseVisualStyleBackColor = true;
            this.chk_manualcontrol.CheckedChanged += new System.EventHandler(this.chk_manualcontrol_CheckedChanged);
            // 
            // JoystickSetup
            // 
            
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.chk_manualcontrol);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.BUT_detch8);
            this.Controls.Add(this.revCH8);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.expo_ch8);
            this.Controls.Add(this.ProgressBarCH8);
            this.Controls.Add(this.CMB_CH8);
            this.Controls.Add(this.BUT_detch7);
            this.Controls.Add(this.revCH7);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.expo_ch7);
            this.Controls.Add(this.ProgressBarCH7);
            this.Controls.Add(this.CMB_CH7);
            this.Controls.Add(this.BUT_detch6);
            this.Controls.Add(this.revCH6);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.expo_ch6);
            this.Controls.Add(this.ProgressBarCH6);
            this.Controls.Add(this.CMB_CH6);
            this.Controls.Add(this.BUT_detch5);
            this.Controls.Add(this.revCH5);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.expo_ch5);
            this.Controls.Add(this.ProgressBarCH5);
            this.Controls.Add(this.CMB_CH5);
            this.Controls.Add(this.CHK_elevons);
            this.Controls.Add(this.BUT_detch4);
            this.Controls.Add(this.BUT_detch3);
            this.Controls.Add(this.BUT_detch2);
            this.Controls.Add(this.BUT_detch1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.BUT_enable);
            this.Controls.Add(this.BUT_save);
            this.Controls.Add(this.revCH4);
            this.Controls.Add(this.revCH3);
            this.Controls.Add(this.revCH2);
            this.Controls.Add(this.revCH1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.expo_ch4);
            this.Controls.Add(this.expo_ch3);
            this.Controls.Add(this.expo_ch2);
            this.Controls.Add(this.expo_ch1);
            this.Controls.Add(this.progressBarRudder);
            this.Controls.Add(this.progressBarThrottle);
            this.Controls.Add(this.progressBarPith);
            this.Controls.Add(this.progressBarRoll);
            this.Controls.Add(this.CMB_CH4);
            this.Controls.Add(this.CMB_CH3);
            this.Controls.Add(this.CMB_CH2);
            this.Controls.Add(this.CMB_CH1);
            this.Controls.Add(this.CMB_joysticks);
            this.Name = "JoystickSetup";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.JoystickSetup_FormClosed);
            this.Load += new System.EventHandler(this.Joystick_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_joysticks;
        private System.Windows.Forms.ComboBox CMB_CH1;
        private System.Windows.Forms.ComboBox CMB_CH2;
        private System.Windows.Forms.ComboBox CMB_CH3;
        private System.Windows.Forms.ComboBox CMB_CH4;
        private HorizontalProgressBar progressBarRoll;
        private HorizontalProgressBar progressBarPith;
        private HorizontalProgressBar progressBarThrottle;
        private HorizontalProgressBar progressBarRudder;
        private System.Windows.Forms.TextBox expo_ch1;
        private System.Windows.Forms.TextBox expo_ch2;
        private System.Windows.Forms.TextBox expo_ch3;
        private System.Windows.Forms.TextBox expo_ch4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox revCH1;
        private System.Windows.Forms.CheckBox revCH2;
        private System.Windows.Forms.CheckBox revCH3;
        private System.Windows.Forms.CheckBox revCH4;
        private Controls.MyButton BUT_save;
        private Controls.MyButton BUT_enable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Timer timer1;
        private Controls.MyButton BUT_detch1;
        private Controls.MyButton BUT_detch2;
        private Controls.MyButton BUT_detch3;
        private Controls.MyButton BUT_detch4;
        private System.Windows.Forms.CheckBox CHK_elevons;
        private Controls.MyButton BUT_detch5;
        private System.Windows.Forms.CheckBox revCH5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox expo_ch5;
        private HorizontalProgressBar ProgressBarCH5;
        private System.Windows.Forms.ComboBox CMB_CH5;
        private Controls.MyButton BUT_detch6;
        private System.Windows.Forms.CheckBox revCH6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox expo_ch6;
        private HorizontalProgressBar ProgressBarCH6;
        private System.Windows.Forms.ComboBox CMB_CH6;
        private Controls.MyButton BUT_detch7;
        private System.Windows.Forms.CheckBox revCH7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox expo_ch7;
        private HorizontalProgressBar ProgressBarCH7;
        private System.Windows.Forms.ComboBox CMB_CH7;
        private Controls.MyButton BUT_detch8;
        private System.Windows.Forms.CheckBox revCH8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox expo_ch8;
        private HorizontalProgressBar ProgressBarCH8;
        private System.Windows.Forms.ComboBox CMB_CH8;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox chk_manualcontrol;
    }
}