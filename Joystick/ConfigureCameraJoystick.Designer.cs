namespace MissionPlanner.Joystick {
  partial class ConfigureCameraJoystick {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureCameraJoystick));
      this.label5 = new System.Windows.Forms.Label();
      this.BUT_enable = new MissionPlanner.Controls.MyButton();
      this.BUT_save = new MissionPlanner.Controls.MyButton();
      this.CMB_joysticks = new System.Windows.Forms.ComboBox();
      this.TILT_AUTODET = new MissionPlanner.Controls.MyButton();
      this.PAN_AUTODET = new MissionPlanner.Controls.MyButton();
      this.label9 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.TILT_REV = new System.Windows.Forms.CheckBox();
      this.PAN_REV = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.TILT_OUTPUT = new MissionPlanner.Controls.HorizontalProgressBar();
      this.PAN_OUTPUT = new MissionPlanner.Controls.HorizontalProgressBar();
      this.TILT_AXIS = new System.Windows.Forms.ComboBox();
      this.PAN_AXIS = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.TILT_CH = new System.Windows.Forms.ComboBox();
      this.PAN_CH = new System.Windows.Forms.ComboBox();
      this.ZOOM_CH = new System.Windows.Forms.ComboBox();
      this.ZOOM_AUTODET = new MissionPlanner.Controls.MyButton();
      this.ZOOM_REV = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.ZOOM_OUTPUT = new MissionPlanner.Controls.HorizontalProgressBar();
      this.ZOOM_AXIS = new System.Windows.Forms.ComboBox();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.label8 = new System.Windows.Forms.Label();
      this.ConfigStatusLabel = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.PAN_EXPO = new System.Windows.Forms.NumericUpDown();
      this.TILT_EXPO = new System.Windows.Forms.NumericUpDown();
      this.ZOOM_EXPO = new System.Windows.Forms.NumericUpDown();
      this.PAN_OVERRIDETHRESHOLD = new System.Windows.Forms.NumericUpDown();
      this.TILT_OVERRIDETHRESHOLD = new System.Windows.Forms.NumericUpDown();
      this.ZOOM_OVERRIDETHRESHOLD = new System.Windows.Forms.NumericUpDown();
      this.label11 = new System.Windows.Forms.Label();
      this.TILT_RATECONV = new System.Windows.Forms.CheckBox();
      this.ZOOM_RATECONV = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.PAN_EXPO)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.TILT_EXPO)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ZOOM_EXPO)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PAN_OVERRIDETHRESHOLD)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.TILT_OVERRIDETHRESHOLD)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ZOOM_OVERRIDETHRESHOLD)).BeginInit();
      this.SuspendLayout();
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label5.Location = new System.Drawing.Point(19, 15);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(45, 13);
      this.label5.TabIndex = 27;
      this.label5.Text = "Joystick";
      // 
      // BUT_enable
      // 
      this.BUT_enable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.BUT_enable.Location = new System.Drawing.Point(280, 12);
      this.BUT_enable.Name = "BUT_enable";
      this.BUT_enable.Size = new System.Drawing.Size(75, 23);
      this.BUT_enable.TabIndex = 26;
      this.BUT_enable.Text = "Enable";
      this.BUT_enable.UseVisualStyleBackColor = true;
      this.BUT_enable.Click += new System.EventHandler(this.BUT_enable_Click);
      // 
      // BUT_save
      // 
      this.BUT_save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.BUT_save.Location = new System.Drawing.Point(361, 12);
      this.BUT_save.Name = "BUT_save";
      this.BUT_save.Size = new System.Drawing.Size(75, 23);
      this.BUT_save.TabIndex = 25;
      this.BUT_save.Text = "Save";
      this.BUT_save.UseVisualStyleBackColor = true;
      this.BUT_save.Click += new System.EventHandler(this.BUT_save_Click);
      // 
      // CMB_joysticks
      // 
      this.CMB_joysticks.FormattingEnabled = true;
      this.CMB_joysticks.Location = new System.Drawing.Point(72, 12);
      this.CMB_joysticks.Name = "CMB_joysticks";
      this.CMB_joysticks.Size = new System.Drawing.Size(202, 21);
      this.CMB_joysticks.TabIndex = 24;
      this.CMB_joysticks.SelectedIndexChanged += new System.EventHandler(this.CMB_joysticks_SelectedIndexChanged);
      this.CMB_joysticks.Click += new System.EventHandler(this.CMB_joysticks_Click);
      // 
      // TILT_AUTODET
      // 
      this.TILT_AUTODET.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.TILT_AUTODET.Location = new System.Drawing.Point(148, 93);
      this.TILT_AUTODET.Name = "TILT_AUTODET";
      this.TILT_AUTODET.Size = new System.Drawing.Size(45, 23);
      this.TILT_AUTODET.TabIndex = 45;
      this.TILT_AUTODET.Text = "Auto Detect";
      this.TILT_AUTODET.UseVisualStyleBackColor = true;
      this.TILT_AUTODET.Click += new System.EventHandler(this.TILT_AUTODET_Click);
      // 
      // PAN_AUTODET
      // 
      this.PAN_AUTODET.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.PAN_AUTODET.Location = new System.Drawing.Point(148, 66);
      this.PAN_AUTODET.Name = "PAN_AUTODET";
      this.PAN_AUTODET.Size = new System.Drawing.Size(45, 23);
      this.PAN_AUTODET.TabIndex = 44;
      this.PAN_AUTODET.Text = "Auto Detect";
      this.PAN_AUTODET.UseVisualStyleBackColor = true;
      this.PAN_AUTODET.Click += new System.EventHandler(this.PAN_AUTODET_Click);
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label9.Location = new System.Drawing.Point(491, 47);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(47, 13);
      this.label9.TabIndex = 43;
      this.label9.Text = "Reverse";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label7.Location = new System.Drawing.Point(277, 47);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(39, 13);
      this.label7.TabIndex = 41;
      this.label7.Text = "Output";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label6.Location = new System.Drawing.Point(387, 47);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(31, 13);
      this.label6.TabIndex = 40;
      this.label6.Text = "Expo";
      // 
      // TILT_REV
      // 
      this.TILT_REV.AutoSize = true;
      this.TILT_REV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.TILT_REV.Location = new System.Drawing.Point(493, 96);
      this.TILT_REV.Name = "TILT_REV";
      this.TILT_REV.Size = new System.Drawing.Size(15, 14);
      this.TILT_REV.TabIndex = 39;
      this.TILT_REV.UseVisualStyleBackColor = true;
      this.TILT_REV.CheckedChanged += new System.EventHandler(this.TILT_REV_CheckedChanged);
      // 
      // PAN_REV
      // 
      this.PAN_REV.AutoSize = true;
      this.PAN_REV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.PAN_REV.Location = new System.Drawing.Point(493, 69);
      this.PAN_REV.Name = "PAN_REV";
      this.PAN_REV.Size = new System.Drawing.Size(15, 14);
      this.PAN_REV.TabIndex = 38;
      this.PAN_REV.UseVisualStyleBackColor = true;
      this.PAN_REV.CheckedChanged += new System.EventHandler(this.PAN_REV_CheckedChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label2.Location = new System.Drawing.Point(10, 98);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(21, 13);
      this.label2.TabIndex = 37;
      this.label2.Text = "Tilt";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label1.Location = new System.Drawing.Point(10, 69);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(26, 13);
      this.label1.TabIndex = 36;
      this.label1.Text = "Pan";
      // 
      // TILT_OUTPUT
      // 
      this.TILT_OUTPUT.DrawLabel = true;
      this.TILT_OUTPUT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.TILT_OUTPUT.Label = null;
      this.TILT_OUTPUT.Location = new System.Drawing.Point(279, 92);
      this.TILT_OUTPUT.Maximum = 2200;
      this.TILT_OUTPUT.maxline = 0;
      this.TILT_OUTPUT.Minimum = 800;
      this.TILT_OUTPUT.minline = 0;
      this.TILT_OUTPUT.Name = "TILT_OUTPUT";
      this.TILT_OUTPUT.Size = new System.Drawing.Size(100, 23);
      this.TILT_OUTPUT.TabIndex = 33;
      this.TILT_OUTPUT.Value = 800;
      // 
      // PAN_OUTPUT
      // 
      this.PAN_OUTPUT.DrawLabel = true;
      this.PAN_OUTPUT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.PAN_OUTPUT.Label = null;
      this.PAN_OUTPUT.Location = new System.Drawing.Point(279, 65);
      this.PAN_OUTPUT.Maximum = 2200;
      this.PAN_OUTPUT.maxline = 0;
      this.PAN_OUTPUT.Minimum = 800;
      this.PAN_OUTPUT.minline = 0;
      this.PAN_OUTPUT.Name = "PAN_OUTPUT";
      this.PAN_OUTPUT.Size = new System.Drawing.Size(100, 23);
      this.PAN_OUTPUT.TabIndex = 32;
      this.PAN_OUTPUT.Value = 800;
      // 
      // TILT_AXIS
      // 
      this.TILT_AXIS.FormattingEnabled = true;
      this.TILT_AXIS.Items.AddRange(new object[] {
            "RZ",
            "X",
            "Y",
            "SL1"});
      this.TILT_AXIS.Location = new System.Drawing.Point(72, 93);
      this.TILT_AXIS.Name = "TILT_AXIS";
      this.TILT_AXIS.Size = new System.Drawing.Size(70, 21);
      this.TILT_AXIS.TabIndex = 31;
      this.TILT_AXIS.SelectedIndexChanged += new System.EventHandler(this.TILT_AXIS_SelectedIndexChanged);
      // 
      // PAN_AXIS
      // 
      this.PAN_AXIS.FormattingEnabled = true;
      this.PAN_AXIS.Items.AddRange(new object[] {
            "RZ",
            "X",
            "Y",
            "SL1"});
      this.PAN_AXIS.Location = new System.Drawing.Point(72, 66);
      this.PAN_AXIS.Name = "PAN_AXIS";
      this.PAN_AXIS.Size = new System.Drawing.Size(70, 21);
      this.PAN_AXIS.TabIndex = 30;
      this.PAN_AXIS.SelectedIndexChanged += new System.EventHandler(this.PAN_AXIS_SelectedIndexChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label3.Location = new System.Drawing.Point(197, 47);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(64, 13);
      this.label3.TabIndex = 48;
      this.label3.Text = "RC Channel";
      // 
      // TILT_CH
      // 
      this.TILT_CH.FormattingEnabled = true;
      this.TILT_CH.Items.AddRange(new object[] {
            "RZ",
            "X",
            "Y",
            "SL1"});
      this.TILT_CH.Location = new System.Drawing.Point(200, 93);
      this.TILT_CH.Name = "TILT_CH";
      this.TILT_CH.Size = new System.Drawing.Size(70, 21);
      this.TILT_CH.TabIndex = 47;
      this.TILT_CH.SelectedIndexChanged += new System.EventHandler(this.TILT_AXIS_SelectedIndexChanged);
      // 
      // PAN_CH
      // 
      this.PAN_CH.FormattingEnabled = true;
      this.PAN_CH.Items.AddRange(new object[] {
            "RZ",
            "X",
            "Y",
            "SL1"});
      this.PAN_CH.Location = new System.Drawing.Point(200, 66);
      this.PAN_CH.Name = "PAN_CH";
      this.PAN_CH.Size = new System.Drawing.Size(70, 21);
      this.PAN_CH.TabIndex = 46;
      this.PAN_CH.SelectedIndexChanged += new System.EventHandler(this.PAN_AXIS_SelectedIndexChanged);
      // 
      // ZOOM_CH
      // 
      this.ZOOM_CH.FormattingEnabled = true;
      this.ZOOM_CH.Items.AddRange(new object[] {
            "RZ",
            "X",
            "Y",
            "SL1"});
      this.ZOOM_CH.Location = new System.Drawing.Point(200, 120);
      this.ZOOM_CH.Name = "ZOOM_CH";
      this.ZOOM_CH.Size = new System.Drawing.Size(70, 21);
      this.ZOOM_CH.TabIndex = 55;
      this.ZOOM_CH.SelectedIndexChanged += new System.EventHandler(this.ZOOM_AXIS_SelectedIndexChanged);
      // 
      // ZOOM_AUTODET
      // 
      this.ZOOM_AUTODET.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.ZOOM_AUTODET.Location = new System.Drawing.Point(148, 120);
      this.ZOOM_AUTODET.Name = "ZOOM_AUTODET";
      this.ZOOM_AUTODET.Size = new System.Drawing.Size(45, 23);
      this.ZOOM_AUTODET.TabIndex = 54;
      this.ZOOM_AUTODET.Text = "Auto Detect";
      this.ZOOM_AUTODET.UseVisualStyleBackColor = true;
      this.ZOOM_AUTODET.Click += new System.EventHandler(this.ZOOM_AUTODET_Click);
      // 
      // ZOOM_REV
      // 
      this.ZOOM_REV.AutoSize = true;
      this.ZOOM_REV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.ZOOM_REV.Location = new System.Drawing.Point(493, 123);
      this.ZOOM_REV.Name = "ZOOM_REV";
      this.ZOOM_REV.Size = new System.Drawing.Size(15, 14);
      this.ZOOM_REV.TabIndex = 53;
      this.ZOOM_REV.UseVisualStyleBackColor = true;
      this.ZOOM_REV.CheckedChanged += new System.EventHandler(this.ZOOM_REV_CheckedChanged);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label4.Location = new System.Drawing.Point(10, 125);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(34, 13);
      this.label4.TabIndex = 52;
      this.label4.Text = "Zoom";
      // 
      // ZOOM_OUTPUT
      // 
      this.ZOOM_OUTPUT.DrawLabel = true;
      this.ZOOM_OUTPUT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.ZOOM_OUTPUT.Label = null;
      this.ZOOM_OUTPUT.Location = new System.Drawing.Point(279, 119);
      this.ZOOM_OUTPUT.Maximum = 2200;
      this.ZOOM_OUTPUT.maxline = 0;
      this.ZOOM_OUTPUT.Minimum = 800;
      this.ZOOM_OUTPUT.minline = 0;
      this.ZOOM_OUTPUT.Name = "ZOOM_OUTPUT";
      this.ZOOM_OUTPUT.Size = new System.Drawing.Size(100, 23);
      this.ZOOM_OUTPUT.TabIndex = 50;
      this.ZOOM_OUTPUT.Value = 800;
      // 
      // ZOOM_AXIS
      // 
      this.ZOOM_AXIS.FormattingEnabled = true;
      this.ZOOM_AXIS.Items.AddRange(new object[] {
            "RZ",
            "X",
            "Y",
            "SL1"});
      this.ZOOM_AXIS.Location = new System.Drawing.Point(72, 120);
      this.ZOOM_AXIS.Name = "ZOOM_AXIS";
      this.ZOOM_AXIS.Size = new System.Drawing.Size(70, 21);
      this.ZOOM_AXIS.TabIndex = 49;
      this.ZOOM_AXIS.SelectedIndexChanged += new System.EventHandler(this.ZOOM_AXIS_SelectedIndexChanged);
      // 
      // timer1
      // 
      this.timer1.Enabled = true;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label8.Location = new System.Drawing.Point(69, 47);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(73, 13);
      this.label8.TabIndex = 42;
      this.label8.Text = "Controller Axis";
      // 
      // ConfigStatusLabel
      // 
      this.ConfigStatusLabel.AutoSize = true;
      this.ConfigStatusLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.ConfigStatusLabel.Location = new System.Drawing.Point(442, 17);
      this.ConfigStatusLabel.Name = "ConfigStatusLabel";
      this.ConfigStatusLabel.Size = new System.Drawing.Size(91, 13);
      this.ConfigStatusLabel.TabIndex = 77;
      this.ConfigStatusLabel.Text = "Loaded Config for";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label10.Location = new System.Drawing.Point(544, 47);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(97, 13);
      this.label10.TabIndex = 78;
      this.label10.Text = "Override Threshold";
      // 
      // PAN_EXPO
      // 
      this.PAN_EXPO.Location = new System.Drawing.Point(385, 68);
      this.PAN_EXPO.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
      this.PAN_EXPO.Name = "PAN_EXPO";
      this.PAN_EXPO.Size = new System.Drawing.Size(100, 20);
      this.PAN_EXPO.TabIndex = 82;
      this.PAN_EXPO.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
      // 
      // TILT_EXPO
      // 
      this.TILT_EXPO.Location = new System.Drawing.Point(385, 94);
      this.TILT_EXPO.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
      this.TILT_EXPO.Name = "TILT_EXPO";
      this.TILT_EXPO.Size = new System.Drawing.Size(100, 20);
      this.TILT_EXPO.TabIndex = 83;
      // 
      // ZOOM_EXPO
      // 
      this.ZOOM_EXPO.Location = new System.Drawing.Point(385, 121);
      this.ZOOM_EXPO.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
      this.ZOOM_EXPO.Name = "ZOOM_EXPO";
      this.ZOOM_EXPO.Size = new System.Drawing.Size(100, 20);
      this.ZOOM_EXPO.TabIndex = 84;
      // 
      // PAN_OVERRIDETHRESHOLD
      // 
      this.PAN_OVERRIDETHRESHOLD.Location = new System.Drawing.Point(547, 68);
      this.PAN_OVERRIDETHRESHOLD.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.PAN_OVERRIDETHRESHOLD.Name = "PAN_OVERRIDETHRESHOLD";
      this.PAN_OVERRIDETHRESHOLD.Size = new System.Drawing.Size(100, 20);
      this.PAN_OVERRIDETHRESHOLD.TabIndex = 85;
      this.PAN_OVERRIDETHRESHOLD.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
      // 
      // TILT_OVERRIDETHRESHOLD
      // 
      this.TILT_OVERRIDETHRESHOLD.Location = new System.Drawing.Point(547, 94);
      this.TILT_OVERRIDETHRESHOLD.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.TILT_OVERRIDETHRESHOLD.Name = "TILT_OVERRIDETHRESHOLD";
      this.TILT_OVERRIDETHRESHOLD.Size = new System.Drawing.Size(100, 20);
      this.TILT_OVERRIDETHRESHOLD.TabIndex = 86;
      // 
      // ZOOM_OVERRIDETHRESHOLD
      // 
      this.ZOOM_OVERRIDETHRESHOLD.Location = new System.Drawing.Point(547, 121);
      this.ZOOM_OVERRIDETHRESHOLD.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.ZOOM_OVERRIDETHRESHOLD.Name = "ZOOM_OVERRIDETHRESHOLD";
      this.ZOOM_OVERRIDETHRESHOLD.Size = new System.Drawing.Size(100, 20);
      this.ZOOM_OVERRIDETHRESHOLD.TabIndex = 87;
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label11.Location = new System.Drawing.Point(650, 47);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(61, 13);
      this.label11.TabIndex = 88;
      this.label11.Text = "Rate Conv.";
      // 
      // TILT_RATECONV
      // 
      this.TILT_RATECONV.AutoSize = true;
      this.TILT_RATECONV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.TILT_RATECONV.Location = new System.Drawing.Point(653, 96);
      this.TILT_RATECONV.Name = "TILT_RATECONV";
      this.TILT_RATECONV.Size = new System.Drawing.Size(15, 14);
      this.TILT_RATECONV.TabIndex = 90;
      this.TILT_RATECONV.UseVisualStyleBackColor = true;
      // 
      // ZOOM_RATECONV
      // 
      this.ZOOM_RATECONV.AutoSize = true;
      this.ZOOM_RATECONV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.ZOOM_RATECONV.Location = new System.Drawing.Point(653, 123);
      this.ZOOM_RATECONV.Name = "ZOOM_RATECONV";
      this.ZOOM_RATECONV.Size = new System.Drawing.Size(15, 14);
      this.ZOOM_RATECONV.TabIndex = 91;
      this.ZOOM_RATECONV.UseVisualStyleBackColor = true;
      // 
      // ConfigureCameraJoystick
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(721, 157);
      this.Controls.Add(this.ZOOM_RATECONV);
      this.Controls.Add(this.TILT_RATECONV);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.ZOOM_OVERRIDETHRESHOLD);
      this.Controls.Add(this.TILT_OVERRIDETHRESHOLD);
      this.Controls.Add(this.PAN_OVERRIDETHRESHOLD);
      this.Controls.Add(this.ZOOM_EXPO);
      this.Controls.Add(this.TILT_EXPO);
      this.Controls.Add(this.PAN_EXPO);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.ConfigStatusLabel);
      this.Controls.Add(this.ZOOM_CH);
      this.Controls.Add(this.ZOOM_AUTODET);
      this.Controls.Add(this.ZOOM_REV);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.ZOOM_OUTPUT);
      this.Controls.Add(this.ZOOM_AXIS);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.TILT_CH);
      this.Controls.Add(this.PAN_CH);
      this.Controls.Add(this.TILT_AUTODET);
      this.Controls.Add(this.PAN_AUTODET);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.TILT_REV);
      this.Controls.Add(this.PAN_REV);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.TILT_OUTPUT);
      this.Controls.Add(this.PAN_OUTPUT);
      this.Controls.Add(this.TILT_AXIS);
      this.Controls.Add(this.PAN_AXIS);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.BUT_enable);
      this.Controls.Add(this.BUT_save);
      this.Controls.Add(this.CMB_joysticks);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "ConfigureCameraJoystick";
      this.ShowIcon = false;
      this.Text = "Configure Camera Joystick";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigureCameraJoystick_FormClosed);
      this.Load += new System.EventHandler(this.ConfigureCameraJoystick_Load);
      ((System.ComponentModel.ISupportInitialize)(this.PAN_EXPO)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.TILT_EXPO)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ZOOM_EXPO)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PAN_OVERRIDETHRESHOLD)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.TILT_OVERRIDETHRESHOLD)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ZOOM_OVERRIDETHRESHOLD)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label5;
    private MissionPlanner.Controls.MyButton BUT_enable;
    private MissionPlanner.Controls.MyButton BUT_save;
    private System.Windows.Forms.ComboBox CMB_joysticks;
    private MissionPlanner.Controls.MyButton TILT_AUTODET;
    private MissionPlanner.Controls.MyButton PAN_AUTODET;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.CheckBox TILT_REV;
    private System.Windows.Forms.CheckBox PAN_REV;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private MissionPlanner.Controls.HorizontalProgressBar TILT_OUTPUT;
    private MissionPlanner.Controls.HorizontalProgressBar PAN_OUTPUT;
    private System.Windows.Forms.ComboBox TILT_AXIS;
    private System.Windows.Forms.ComboBox PAN_AXIS;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox TILT_CH;
    private System.Windows.Forms.ComboBox PAN_CH;
    private System.Windows.Forms.ComboBox ZOOM_CH;
    private MissionPlanner.Controls.MyButton ZOOM_AUTODET;
    private System.Windows.Forms.CheckBox ZOOM_REV;
    private System.Windows.Forms.Label label4;
    private MissionPlanner.Controls.HorizontalProgressBar ZOOM_OUTPUT;
    private System.Windows.Forms.ComboBox ZOOM_AXIS;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label ConfigStatusLabel;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.NumericUpDown PAN_OVERRIDETHRESHOLD;
    private System.Windows.Forms.NumericUpDown TILT_OVERRIDETHRESHOLD;
    private System.Windows.Forms.NumericUpDown ZOOM_OVERRIDETHRESHOLD;
    private System.Windows.Forms.NumericUpDown ZOOM_EXPO;
    private System.Windows.Forms.NumericUpDown TILT_EXPO;
    private System.Windows.Forms.NumericUpDown PAN_EXPO;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.CheckBox TILT_RATECONV;
    private System.Windows.Forms.CheckBox ZOOM_RATECONV;
  }
}