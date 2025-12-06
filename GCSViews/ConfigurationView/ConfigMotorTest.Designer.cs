namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigMotorTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigMotorTest));
            this.NUM_thr_percent = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.NUM_duration = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanelMotors = new System.Windows.Forms.FlowLayoutPanel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnTestAll = new MissionPlanner.Controls.MyButton();
            this.btnStopAll = new MissionPlanner.Controls.MyButton();
            this.btnTestSequence = new MissionPlanner.Controls.MyButton();
            this.webViewFrame = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.FrameClass = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.NUM_mot_spin_min = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.NUM_mot_spin_arm = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_thr_percent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_duration)).BeginInit();
            this.panel1.SuspendLayout();
            this.panelButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webViewFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_mot_spin_min)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_mot_spin_arm)).BeginInit();
            this.SuspendLayout();
            //
            // NUM_thr_percent
            //
            this.NUM_thr_percent.Location = new System.Drawing.Point(66, 14);
            this.NUM_thr_percent.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.NUM_thr_percent.Name = "NUM_thr_percent";
            this.NUM_thr_percent.Size = new System.Drawing.Size(54, 20);
            this.NUM_thr_percent.TabIndex = 0;
            this.NUM_thr_percent.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Throttle %";
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 529);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Note: Remove props!";
            //
            // linkLabel1
            //
            this.linkLabel1.Location = new System.Drawing.Point(6, 545);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(145, 17);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Documentation";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            //
            // NUM_duration
            //
            this.NUM_duration.Location = new System.Drawing.Point(201, 14);
            this.NUM_duration.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.NUM_duration.Name = "NUM_duration";
            this.NUM_duration.Size = new System.Drawing.Size(54, 20);
            this.NUM_duration.TabIndex = 4;
            this.NUM_duration.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(129, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Duration (s)";
            //
            // panel1
            //
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.flowLayoutPanelMotors);
            this.panel1.Controls.Add(this.panelButtons);
            this.panel1.Controls.Add(this.webViewFrame);
            this.panel1.Controls.Add(this.FrameClass);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.NUM_mot_spin_min);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.NUM_mot_spin_arm);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.NUM_thr_percent);
            this.panel1.Controls.Add(this.NUM_duration);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1194, 583);
            this.panel1.TabIndex = 6;
            //
            // flowLayoutPanelMotors
            //
            this.flowLayoutPanelMotors.AutoScroll = true;
            this.flowLayoutPanelMotors.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelMotors.Location = new System.Drawing.Point(6, 75);
            this.flowLayoutPanelMotors.Name = "flowLayoutPanelMotors";
            this.flowLayoutPanelMotors.Size = new System.Drawing.Size(320, 400);
            this.flowLayoutPanelMotors.TabIndex = 12;
            this.flowLayoutPanelMotors.WrapContents = false;
            //
            // panelButtons
            //
            this.panelButtons.Controls.Add(this.btnTestAll);
            this.panelButtons.Controls.Add(this.btnStopAll);
            this.panelButtons.Controls.Add(this.btnTestSequence);
            this.panelButtons.Location = new System.Drawing.Point(6, 481);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(320, 45);
            this.panelButtons.TabIndex = 13;
            //
            // btnTestAll
            //
            this.btnTestAll.Location = new System.Drawing.Point(0, 3);
            this.btnTestAll.Name = "btnTestAll";
            this.btnTestAll.Size = new System.Drawing.Size(100, 37);
            this.btnTestAll.TabIndex = 0;
            this.btnTestAll.Text = "Test All Motors";
            this.btnTestAll.UseVisualStyleBackColor = true;
            this.btnTestAll.Click += new System.EventHandler(this.but_TestAll);
            //
            // btnStopAll
            //
            this.btnStopAll.Location = new System.Drawing.Point(106, 3);
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.Size = new System.Drawing.Size(100, 37);
            this.btnStopAll.TabIndex = 1;
            this.btnStopAll.Text = "Stop All Motors";
            this.btnStopAll.UseVisualStyleBackColor = true;
            this.btnStopAll.Click += new System.EventHandler(this.but_StopAll);
            //
            // btnTestSequence
            //
            this.btnTestSequence.Location = new System.Drawing.Point(212, 3);
            this.btnTestSequence.Name = "btnTestSequence";
            this.btnTestSequence.Size = new System.Drawing.Size(100, 37);
            this.btnTestSequence.TabIndex = 2;
            this.btnTestSequence.Text = "Test Sequence";
            this.btnTestSequence.UseVisualStyleBackColor = true;
            this.btnTestSequence.Click += new System.EventHandler(this.but_TestAllSeq);
            //
            // webViewFrame
            //
            this.webViewFrame.AllowExternalDrop = true;
            this.webViewFrame.CreationProperties = null;
            this.webViewFrame.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webViewFrame.Location = new System.Drawing.Point(508, 14);
            this.webViewFrame.Name = "webViewFrame";
            this.webViewFrame.Size = new System.Drawing.Size(400, 400);
            this.webViewFrame.TabIndex = 7;
            this.webViewFrame.TabStop = false;
            this.webViewFrame.ZoomFactor = 1D;
            //
            // FrameClass
            //
            this.FrameClass.AutoSize = true;
            this.FrameClass.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.FrameClass.Location = new System.Drawing.Point(6, 45);
            this.FrameClass.Name = "FrameClass";
            this.FrameClass.Size = new System.Drawing.Size(200, 13);
            this.FrameClass.TabIndex = 10;
            this.FrameClass.Text = "Class: unknown, Type: unknown";
            //
            // label5
            //
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(363, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(159, 18);
            this.label5.TabIndex = 9;
            this.label5.Text = "Motor Spin Min (0.00-1.00):";
            //
            // NUM_mot_spin_min
            //
            this.NUM_mot_spin_min.DecimalPlaces = 2;
            this.NUM_mot_spin_min.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NUM_mot_spin_min.Location = new System.Drawing.Point(282, 43);
            this.NUM_mot_spin_min.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUM_mot_spin_min.Name = "NUM_mot_spin_min";
            this.NUM_mot_spin_min.Size = new System.Drawing.Size(75, 20);
            this.NUM_mot_spin_min.TabIndex = 8;
            this.NUM_mot_spin_min.ValueChanged += new System.EventHandler(this.NUM_mot_spin_min_ValueChanged);
            //
            // label4
            //
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(363, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(159, 18);
            this.label4.TabIndex = 7;
            this.label4.Text = "Motor Spin Arm (0.00-1.00):";
            //
            // NUM_mot_spin_arm
            //
            this.NUM_mot_spin_arm.DecimalPlaces = 2;
            this.NUM_mot_spin_arm.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NUM_mot_spin_arm.Location = new System.Drawing.Point(282, 14);
            this.NUM_mot_spin_arm.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUM_mot_spin_arm.Name = "NUM_mot_spin_arm";
            this.NUM_mot_spin_arm.Size = new System.Drawing.Size(75, 20);
            this.NUM_mot_spin_arm.TabIndex = 6;
            this.NUM_mot_spin_arm.ValueChanged += new System.EventHandler(this.NUM_mot_spin_arm_ValueChanged);
            //
            // ConfigMotorTest
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.panel1);
            this.Name = "ConfigMotorTest";
            this.Size = new System.Drawing.Size(1194, 583);
            ((System.ComponentModel.ISupportInitialize)(this.NUM_thr_percent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_duration)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webViewFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_mot_spin_min)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_mot_spin_arm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown NUM_thr_percent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.NumericUpDown NUM_duration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown NUM_mot_spin_arm;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown NUM_mot_spin_min;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label FrameClass;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewFrame;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMotors;
        private System.Windows.Forms.Panel panelButtons;
        private MissionPlanner.Controls.MyButton btnTestAll;
        private MissionPlanner.Controls.MyButton btnStopAll;
        private MissionPlanner.Controls.MyButton btnTestSequence;
    }
}
