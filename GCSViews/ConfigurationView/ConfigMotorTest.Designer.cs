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
            this.webViewFrame = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.FrameType = new System.Windows.Forms.Label();
            this.FrameClass = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.NUM_mot_spin_min = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.NUM_mot_spin_arm = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_thr_percent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_duration)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webViewFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_mot_spin_min)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_mot_spin_arm)).BeginInit();
            this.SuspendLayout();
            // 
            // NUM_thr_percent
            // 
            resources.ApplyResources(this.NUM_thr_percent, "NUM_thr_percent");
            this.NUM_thr_percent.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.NUM_thr_percent.Name = "NUM_thr_percent";
            this.NUM_thr_percent.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
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
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // NUM_duration
            // 
            resources.ApplyResources(this.NUM_duration, "NUM_duration");
            this.NUM_duration.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.NUM_duration.Name = "NUM_duration";
            this.NUM_duration.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.webViewFrame);
            this.panel1.Controls.Add(this.FrameType);
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
            this.panel1.Name = "panel1";
            // 
            // webViewFrame
            // 
            this.webViewFrame.AllowExternalDrop = true;
            this.webViewFrame.CreationProperties = null;
            this.webViewFrame.DefaultBackgroundColor = System.Drawing.Color.White;
            resources.ApplyResources(this.webViewFrame, "webViewFrame");
            this.webViewFrame.Name = "webViewFrame";
            this.webViewFrame.TabStop = false;
            this.webViewFrame.ZoomFactor = 1D;
            // 
            // FrameType
            // 
            resources.ApplyResources(this.FrameType, "FrameType");
            this.FrameType.Name = "FrameType";
            // 
            // FrameClass
            // 
            resources.ApplyResources(this.FrameClass, "FrameClass");
            this.FrameClass.Name = "FrameClass";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // NUM_mot_spin_min
            // 
            this.NUM_mot_spin_min.DecimalPlaces = 2;
            this.NUM_mot_spin_min.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            resources.ApplyResources(this.NUM_mot_spin_min, "NUM_mot_spin_min");
            this.NUM_mot_spin_min.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUM_mot_spin_min.Name = "NUM_mot_spin_min";
            this.NUM_mot_spin_min.ValueChanged += new System.EventHandler(this.NUM_mot_spin_min_ValueChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // NUM_mot_spin_arm
            // 
            this.NUM_mot_spin_arm.DecimalPlaces = 2;
            this.NUM_mot_spin_arm.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            resources.ApplyResources(this.NUM_mot_spin_arm, "NUM_mot_spin_arm");
            this.NUM_mot_spin_arm.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUM_mot_spin_arm.Name = "NUM_mot_spin_arm";
            this.NUM_mot_spin_arm.ValueChanged += new System.EventHandler(this.NUM_mot_spin_arm_ValueChanged);
            // 
            // ConfigMotorTest
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Name = "ConfigMotorTest";
            ((System.ComponentModel.ISupportInitialize)(this.NUM_thr_percent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_duration)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.Label FrameType;
        private System.Windows.Forms.Label FrameClass;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewFrame;
    }
}
