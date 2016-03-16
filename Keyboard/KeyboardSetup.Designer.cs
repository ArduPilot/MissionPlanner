namespace MissionPlanner.Keyboard
{
    partial class KeyboardSetup
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.setModeComboBox = new System.Windows.Forms.ComboBox();
            this.setModeBox = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.desarmBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.armBox = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.steerRightBox = new System.Windows.Forms.TextBox();
            this.steerLeftBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.pitchBackwardBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.rollRightBox = new System.Windows.Forms.TextBox();
            this.pitchForwardBox = new System.Windows.Forms.TextBox();
            this.rollLeftBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.decelerateBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.progressBarYaw = new MissionPlanner.Controls.HorizontalProgressBar();
            this.progressBarThrottle = new MissionPlanner.Controls.HorizontalProgressBar();
            this.BUT_enable = new MissionPlanner.Controls.MyButton();
            this.progressBarPitch = new MissionPlanner.Controls.HorizontalProgressBar();
            this.progressBarRoll = new MissionPlanner.Controls.HorizontalProgressBar();
            this.rollTrackBar = new MissionPlanner.Controls.MyTrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.yawTrackBarMaxValue = new System.Windows.Forms.Label();
            this.pitchTrackBarMaxValue = new System.Windows.Forms.Label();
            this.rollTrackBarMaxValue = new System.Windows.Forms.Label();
            this.yawTrackBarMinValue = new System.Windows.Forms.Label();
            this.pitchTrackBarMinValue = new System.Windows.Forms.Label();
            this.rollTrackBarMinValue = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.yawTrackBar = new MissionPlanner.Controls.MyTrackBar();
            this.pitchTrackBar = new MissionPlanner.Controls.MyTrackBar();
            this.BUT_help = new MissionPlanner.Controls.MyButton();
            this.accelerateBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rollTrackBar)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yawTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Roll";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(10, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Pitch";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(8, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Throttle";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(10, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Yaw";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.accelerateBox);
            this.groupBox1.Controls.Add(this.setModeComboBox);
            this.groupBox1.Controls.Add(this.setModeBox);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.desarmBox);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.armBox);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.steerRightBox);
            this.groupBox1.Controls.Add(this.steerLeftBox);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.pitchBackwardBox);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.rollRightBox);
            this.groupBox1.Controls.Add(this.pitchForwardBox);
            this.groupBox1.Controls.Add(this.rollLeftBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.decelerateBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(296, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 218);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // setModeComboBox
            // 
            this.setModeComboBox.FormattingEnabled = true;
            this.setModeComboBox.Location = new System.Drawing.Point(171, 174);
            this.setModeComboBox.Name = "setModeComboBox";
            this.setModeComboBox.Size = new System.Drawing.Size(121, 21);
            this.setModeComboBox.TabIndex = 52;
            // 
            // setModeBox
            // 
            this.setModeBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.setModeBox.Location = new System.Drawing.Point(96, 175);
            this.setModeBox.Name = "setModeBox";
            this.setModeBox.ReadOnly = true;
            this.setModeBox.Size = new System.Drawing.Size(60, 20);
            this.setModeBox.TabIndex = 51;
            this.setModeBox.Text = "M";
            this.setModeBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.setModeBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.setModeBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.setModeBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.setModeBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label20.Location = new System.Drawing.Point(5, 178);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(53, 13);
            this.label20.TabIndex = 50;
            this.label20.Text = "Set Mode";
            // 
            // desarmBox
            // 
            this.desarmBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.desarmBox.Location = new System.Drawing.Point(248, 149);
            this.desarmBox.Name = "desarmBox";
            this.desarmBox.ReadOnly = true;
            this.desarmBox.Size = new System.Drawing.Size(60, 20);
            this.desarmBox.TabIndex = 49;
            this.desarmBox.Text = "PageDown";
            this.desarmBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.desarmBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.desarmBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.desarmBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.desarmBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(178, 152);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(39, 13);
            this.label16.TabIndex = 48;
            this.label16.Text = "Disarm";
            // 
            // armBox
            // 
            this.armBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.armBox.Location = new System.Drawing.Point(96, 149);
            this.armBox.Name = "armBox";
            this.armBox.ReadOnly = true;
            this.armBox.Size = new System.Drawing.Size(60, 20);
            this.armBox.TabIndex = 47;
            this.armBox.Text = "PageUp";
            this.armBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.armBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.armBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.armBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.armBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(5, 152);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(25, 13);
            this.label15.TabIndex = 46;
            this.label15.Text = "Arm";
            // 
            // steerRightBox
            // 
            this.steerRightBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.steerRightBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.steerRightBox.Location = new System.Drawing.Point(248, 54);
            this.steerRightBox.Name = "steerRightBox";
            this.steerRightBox.ReadOnly = true;
            this.steerRightBox.Size = new System.Drawing.Size(60, 20);
            this.steerRightBox.TabIndex = 44;
            this.steerRightBox.Text = "D";
            this.steerRightBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.steerRightBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.steerRightBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.steerRightBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.steerRightBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // steerLeftBox
            // 
            this.steerLeftBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.steerLeftBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.steerLeftBox.Location = new System.Drawing.Point(248, 28);
            this.steerLeftBox.Name = "steerLeftBox";
            this.steerLeftBox.ReadOnly = true;
            this.steerLeftBox.Size = new System.Drawing.Size(60, 20);
            this.steerLeftBox.TabIndex = 43;
            this.steerLeftBox.Text = "A";
            this.steerLeftBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.steerLeftBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.steerLeftBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.steerLeftBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.steerLeftBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(178, 56);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 13);
            this.label12.TabIndex = 42;
            this.label12.Text = "Steer Right";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(178, 30);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 13);
            this.label11.TabIndex = 41;
            this.label11.Text = "Steer Left";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(5, 109);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 40;
            this.label10.Text = "Pitch Backward";
            // 
            // pitchBackwardBox
            // 
            this.pitchBackwardBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.pitchBackwardBox.Location = new System.Drawing.Point(96, 106);
            this.pitchBackwardBox.Name = "pitchBackwardBox";
            this.pitchBackwardBox.ReadOnly = true;
            this.pitchBackwardBox.Size = new System.Drawing.Size(60, 20);
            this.pitchBackwardBox.TabIndex = 39;
            this.pitchBackwardBox.Text = "Down";
            this.pitchBackwardBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pitchBackwardBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.pitchBackwardBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.pitchBackwardBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.pitchBackwardBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(178, 108);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 38;
            this.label9.Text = "Roll Right";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(5, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "Pitch Forward";
            // 
            // rollRightBox
            // 
            this.rollRightBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.rollRightBox.Location = new System.Drawing.Point(248, 106);
            this.rollRightBox.Name = "rollRightBox";
            this.rollRightBox.ReadOnly = true;
            this.rollRightBox.Size = new System.Drawing.Size(60, 20);
            this.rollRightBox.TabIndex = 36;
            this.rollRightBox.Text = "Right";
            this.rollRightBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.rollRightBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.rollRightBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.rollRightBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.rollRightBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // pitchForwardBox
            // 
            this.pitchForwardBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.pitchForwardBox.Location = new System.Drawing.Point(96, 80);
            this.pitchForwardBox.Name = "pitchForwardBox";
            this.pitchForwardBox.ReadOnly = true;
            this.pitchForwardBox.Size = new System.Drawing.Size(60, 20);
            this.pitchForwardBox.TabIndex = 35;
            this.pitchForwardBox.Text = "Up";
            this.pitchForwardBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pitchForwardBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.pitchForwardBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.pitchForwardBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.pitchForwardBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // rollLeftBox
            // 
            this.rollLeftBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.rollLeftBox.Location = new System.Drawing.Point(248, 80);
            this.rollLeftBox.Name = "rollLeftBox";
            this.rollLeftBox.ReadOnly = true;
            this.rollLeftBox.Size = new System.Drawing.Size(60, 20);
            this.rollLeftBox.TabIndex = 34;
            this.rollLeftBox.Text = "Left";
            this.rollLeftBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.rollLeftBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.rollLeftBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.rollLeftBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.rollLeftBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(178, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 33;
            this.label7.Text = "Roll Left";
            // 
            // decelerateBox
            // 
            this.decelerateBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.decelerateBox.Location = new System.Drawing.Point(96, 54);
            this.decelerateBox.Name = "decelerateBox";
            this.decelerateBox.ReadOnly = true;
            this.decelerateBox.Size = new System.Drawing.Size(60, 20);
            this.decelerateBox.TabIndex = 32;
            this.decelerateBox.Text = "S";
            this.decelerateBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.decelerateBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.decelerateBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.decelerateBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.decelerateBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(5, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Decelerate";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(5, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Accelerate";
            // 
            // progressBarYaw
            // 
            this.progressBarYaw.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.progressBarYaw.Label = null;
            this.progressBarYaw.Location = new System.Drawing.Point(55, 99);
            this.progressBarYaw.Maximum = 2000;
            this.progressBarYaw.maxline = 0;
            this.progressBarYaw.minline = 0;
            this.progressBarYaw.Name = "progressBarYaw";
            this.progressBarYaw.Size = new System.Drawing.Size(100, 23);
            this.progressBarYaw.TabIndex = 26;
            // 
            // progressBarThrottle
            // 
            this.progressBarThrottle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.progressBarThrottle.Label = null;
            this.progressBarThrottle.Location = new System.Drawing.Point(55, 70);
            this.progressBarThrottle.Maximum = 2000;
            this.progressBarThrottle.maxline = 0;
            this.progressBarThrottle.minline = 0;
            this.progressBarThrottle.Name = "progressBarThrottle";
            this.progressBarThrottle.Size = new System.Drawing.Size(100, 23);
            this.progressBarThrottle.TabIndex = 25;
            // 
            // BUT_enable
            // 
            this.BUT_enable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_enable.Location = new System.Drawing.Point(181, 12);
            this.BUT_enable.Name = "BUT_enable";
            this.BUT_enable.Size = new System.Drawing.Size(75, 23);
            this.BUT_enable.TabIndex = 23;
            this.BUT_enable.Text = "Enable";
            this.BUT_enable.UseVisualStyleBackColor = true;
            this.BUT_enable.Click += new System.EventHandler(this.BUT_enable_Click);
            // 
            // progressBarPitch
            // 
            this.progressBarPitch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.progressBarPitch.Label = null;
            this.progressBarPitch.Location = new System.Drawing.Point(55, 41);
            this.progressBarPitch.Maximum = 2000;
            this.progressBarPitch.maxline = 0;
            this.progressBarPitch.Minimum = 1000;
            this.progressBarPitch.minline = 0;
            this.progressBarPitch.Name = "progressBarPitch";
            this.progressBarPitch.Size = new System.Drawing.Size(100, 23);
            this.progressBarPitch.TabIndex = 15;
            // 
            // progressBarRoll
            // 
            this.progressBarRoll.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.progressBarRoll.Label = null;
            this.progressBarRoll.Location = new System.Drawing.Point(55, 12);
            this.progressBarRoll.Maximum = 2000;
            this.progressBarRoll.maxline = 0;
            this.progressBarRoll.Minimum = 1000;
            this.progressBarRoll.minline = 0;
            this.progressBarRoll.Name = "progressBarRoll";
            this.progressBarRoll.Size = new System.Drawing.Size(100, 23);
            this.progressBarRoll.TabIndex = 6;
            // 
            // rollTrackBar
            // 
            this.rollTrackBar.LargeChange = 0.005F;
            this.rollTrackBar.Location = new System.Drawing.Point(86, 27);
            this.rollTrackBar.Maximum = 500F;
            this.rollTrackBar.Minimum = 100F;
            this.rollTrackBar.Name = "rollTrackBar";
            this.rollTrackBar.Size = new System.Drawing.Size(104, 45);
            this.rollTrackBar.SmallChange = 0F;
            this.rollTrackBar.TabIndex = 29;
            this.rollTrackBar.TickFrequency = 100F;
            this.rollTrackBar.Value = 100F;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.yawTrackBarMaxValue);
            this.groupBox2.Controls.Add(this.pitchTrackBarMaxValue);
            this.groupBox2.Controls.Add(this.rollTrackBarMaxValue);
            this.groupBox2.Controls.Add(this.yawTrackBarMinValue);
            this.groupBox2.Controls.Add(this.pitchTrackBarMinValue);
            this.groupBox2.Controls.Add(this.rollTrackBarMinValue);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.yawTrackBar);
            this.groupBox2.Controls.Add(this.pitchTrackBar);
            this.groupBox2.Controls.Add(this.rollTrackBar);
            this.groupBox2.Location = new System.Drawing.Point(13, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(243, 180);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Factors";
            // 
            // yawTrackBarMaxValue
            // 
            this.yawTrackBarMaxValue.AutoSize = true;
            this.yawTrackBarMaxValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.yawTrackBarMaxValue.Location = new System.Drawing.Point(170, 161);
            this.yawTrackBarMaxValue.Name = "yawTrackBarMaxValue";
            this.yawTrackBarMaxValue.Size = new System.Drawing.Size(13, 13);
            this.yawTrackBarMaxValue.TabIndex = 39;
            this.yawTrackBarMaxValue.Text = "0";
            // 
            // pitchTrackBarMaxValue
            // 
            this.pitchTrackBarMaxValue.AutoSize = true;
            this.pitchTrackBarMaxValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pitchTrackBarMaxValue.Location = new System.Drawing.Point(170, 110);
            this.pitchTrackBarMaxValue.Name = "pitchTrackBarMaxValue";
            this.pitchTrackBarMaxValue.Size = new System.Drawing.Size(13, 13);
            this.pitchTrackBarMaxValue.TabIndex = 38;
            this.pitchTrackBarMaxValue.Text = "0";
            // 
            // rollTrackBarMaxValue
            // 
            this.rollTrackBarMaxValue.AutoSize = true;
            this.rollTrackBarMaxValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rollTrackBarMaxValue.Location = new System.Drawing.Point(170, 59);
            this.rollTrackBarMaxValue.Name = "rollTrackBarMaxValue";
            this.rollTrackBarMaxValue.Size = new System.Drawing.Size(13, 13);
            this.rollTrackBarMaxValue.TabIndex = 37;
            this.rollTrackBarMaxValue.Text = "0";
            // 
            // yawTrackBarMinValue
            // 
            this.yawTrackBarMinValue.AutoSize = true;
            this.yawTrackBarMinValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.yawTrackBarMinValue.Location = new System.Drawing.Point(93, 161);
            this.yawTrackBarMinValue.Name = "yawTrackBarMinValue";
            this.yawTrackBarMinValue.Size = new System.Drawing.Size(13, 13);
            this.yawTrackBarMinValue.TabIndex = 36;
            this.yawTrackBarMinValue.Text = "0";
            // 
            // pitchTrackBarMinValue
            // 
            this.pitchTrackBarMinValue.AutoSize = true;
            this.pitchTrackBarMinValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pitchTrackBarMinValue.Location = new System.Drawing.Point(93, 110);
            this.pitchTrackBarMinValue.Name = "pitchTrackBarMinValue";
            this.pitchTrackBarMinValue.Size = new System.Drawing.Size(13, 13);
            this.pitchTrackBarMinValue.TabIndex = 35;
            this.pitchTrackBarMinValue.Text = "0";
            // 
            // rollTrackBarMinValue
            // 
            this.rollTrackBarMinValue.AutoSize = true;
            this.rollTrackBarMinValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rollTrackBarMinValue.Location = new System.Drawing.Point(93, 59);
            this.rollTrackBarMinValue.Name = "rollTrackBarMinValue";
            this.rollTrackBarMinValue.Size = new System.Drawing.Size(13, 13);
            this.rollTrackBarMinValue.TabIndex = 34;
            this.rollTrackBarMinValue.Text = "0";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label19.Location = new System.Drawing.Point(6, 129);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(61, 13);
            this.label19.TabIndex = 33;
            this.label19.Text = "Yaw Factor";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label18.Location = new System.Drawing.Point(6, 78);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(64, 13);
            this.label18.TabIndex = 32;
            this.label18.Text = "Pitch Factor";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(6, 27);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(58, 13);
            this.label17.TabIndex = 31;
            this.label17.Text = "Roll Factor";
            // 
            // yawTrackBar
            // 
            this.yawTrackBar.LargeChange = 0.005F;
            this.yawTrackBar.Location = new System.Drawing.Point(86, 129);
            this.yawTrackBar.Maximum = 500F;
            this.yawTrackBar.Minimum = 100F;
            this.yawTrackBar.Name = "yawTrackBar";
            this.yawTrackBar.Size = new System.Drawing.Size(104, 45);
            this.yawTrackBar.SmallChange = 0F;
            this.yawTrackBar.TabIndex = 31;
            this.yawTrackBar.TickFrequency = 100F;
            this.yawTrackBar.Value = 100F;
            // 
            // pitchTrackBar
            // 
            this.pitchTrackBar.LargeChange = 0.005F;
            this.pitchTrackBar.Location = new System.Drawing.Point(86, 78);
            this.pitchTrackBar.Maximum = 500F;
            this.pitchTrackBar.Minimum = 100F;
            this.pitchTrackBar.Name = "pitchTrackBar";
            this.pitchTrackBar.Size = new System.Drawing.Size(104, 45);
            this.pitchTrackBar.SmallChange = 0F;
            this.pitchTrackBar.TabIndex = 30;
            this.pitchTrackBar.TickFrequency = 100F;
            this.pitchTrackBar.Value = 100F;
            // 
            // BUT_help
            // 
            this.BUT_help.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_help.Location = new System.Drawing.Point(181, 41);
            this.BUT_help.Name = "BUT_help";
            this.BUT_help.Size = new System.Drawing.Size(75, 23);
            this.BUT_help.TabIndex = 31;
            this.BUT_help.Text = "Help";
            this.BUT_help.UseVisualStyleBackColor = true;
            this.BUT_help.Click += new System.EventHandler(this.BUT_help_Click);
            // 
            // accelerateBox
            // 
            this.accelerateBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.accelerateBox.Location = new System.Drawing.Point(96, 28);
            this.accelerateBox.Name = "accelerateBox";
            this.accelerateBox.ReadOnly = true;
            this.accelerateBox.Size = new System.Drawing.Size(60, 20);
            this.accelerateBox.TabIndex = 53;
            this.accelerateBox.Text = "W";
            this.accelerateBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.accelerateBox.Click += new System.EventHandler(this.allTextBox_Click);
            this.accelerateBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.allTextBox_KeyDown);
            this.accelerateBox.Leave += new System.EventHandler(this.allTextBox_Leave);
            this.accelerateBox.MouseEnter += new System.EventHandler(this.allTextBox_MouseEnter);
            // 
            // KeyboardSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 320);
            this.Controls.Add(this.BUT_help);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.progressBarYaw);
            this.Controls.Add(this.progressBarThrottle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BUT_enable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBarPitch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBarRoll);
            this.Name = "KeyboardSetup";
            this.Text = "Keyboard Controller";
            this.Load += new System.EventHandler(this.Keyboard_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rollTrackBar)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yawTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.HorizontalProgressBar progressBarRoll;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Controls.MyButton BUT_enable;
        private Controls.HorizontalProgressBar progressBarPitch;
        private System.Windows.Forms.Label label3;
        private Controls.HorizontalProgressBar progressBarThrottle;
        private Controls.HorizontalProgressBar progressBarYaw;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox decelerateBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox pitchBackwardBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox rollRightBox;
        private System.Windows.Forms.TextBox pitchForwardBox;
        private System.Windows.Forms.TextBox rollLeftBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox steerLeftBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox steerRightBox;
        private System.Windows.Forms.TextBox desarmBox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox armBox;
        private System.Windows.Forms.Label label15;
        private Controls.MyTrackBar rollTrackBar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private Controls.MyTrackBar yawTrackBar;
        private Controls.MyTrackBar pitchTrackBar;
        private System.Windows.Forms.Label yawTrackBarMaxValue;
        private System.Windows.Forms.Label pitchTrackBarMaxValue;
        private System.Windows.Forms.Label rollTrackBarMaxValue;
        private System.Windows.Forms.Label yawTrackBarMinValue;
        private System.Windows.Forms.Label pitchTrackBarMinValue;
        private System.Windows.Forms.Label rollTrackBarMinValue;
        private System.Windows.Forms.TextBox setModeBox;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox setModeComboBox;
        private Controls.MyButton BUT_help;
        private System.Windows.Forms.TextBox accelerateBox;
    }
}