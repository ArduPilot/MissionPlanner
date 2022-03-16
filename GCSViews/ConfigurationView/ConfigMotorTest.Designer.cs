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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.but_mot_spin_arm = new MissionPlanner.Controls.MyButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.but_mot_spin_min = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_thr_percent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_duration)).BeginInit();
            this.groupBox1.SuspendLayout();
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
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.but_mot_spin_min);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.but_mot_spin_arm);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.NUM_thr_percent);
            this.groupBox1.Controls.Add(this.NUM_duration);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.linkLabel1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // but_mot_spin_arm
            // 
            resources.ApplyResources(this.but_mot_spin_arm, "but_mot_spin_arm");
            this.but_mot_spin_arm.Name = "but_mot_spin_arm";
            this.but_mot_spin_arm.UseVisualStyleBackColor = true;
            this.but_mot_spin_arm.Click += new System.EventHandler(this.but_mot_spin_arm_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // but_mot_spin_min
            // 
            resources.ApplyResources(this.but_mot_spin_min, "but_mot_spin_min");
            this.but_mot_spin_min.Name = "but_mot_spin_min";
            this.but_mot_spin_min.UseVisualStyleBackColor = true;
            this.but_mot_spin_min.Click += new System.EventHandler(this.but_mot_spin_min_Click);
            // 
            // ConfigMotorTest
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Name = "ConfigMotorTest";
            ((System.ComponentModel.ISupportInitialize)(this.NUM_thr_percent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_duration)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private Controls.MyButton but_mot_spin_arm;
        private System.Windows.Forms.Label label5;
        private Controls.MyButton but_mot_spin_min;
        private System.Windows.Forms.Label label4;
    }
}
