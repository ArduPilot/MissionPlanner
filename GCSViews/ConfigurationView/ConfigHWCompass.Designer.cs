namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigHWCompass
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigHWCompass));
            this.BUT_MagCalibrationLive = new MissionPlanner.Controls.MyButton();
            this.linkLabelmagdec = new System.Windows.Forms.LinkLabel();
            this.label100 = new System.Windows.Forms.Label();
            this.TXT_declination_deg = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BUT_MagCalibrationLog = new MissionPlanner.Controls.MyButton();
            this.CHK_autodec = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbl_adv_cfg_only = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.imageLabel1 = new MissionPlanner.Controls.ImageLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rb_px4pixhawk = new System.Windows.Forms.RadioButton();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.radioButtonmanual = new System.Windows.Forms.RadioButton();
            this.radioButton_external = new System.Windows.Forms.RadioButton();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.CMB_compass_orient = new MissionPlanner.Controls.MavlinkComboBox();
            this.radioButton_onboard = new System.Windows.Forms.RadioButton();
            this.TXT_declination_min = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.CHK_enablecompass = new MissionPlanner.Controls.MavlinkCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // BUT_MagCalibrationLive
            // 
            resources.ApplyResources(this.BUT_MagCalibrationLive, "BUT_MagCalibrationLive");
            this.BUT_MagCalibrationLive.Name = "BUT_MagCalibrationLive";
            this.BUT_MagCalibrationLive.UseVisualStyleBackColor = true;
            this.BUT_MagCalibrationLive.Click += new System.EventHandler(this.BUT_MagCalibration_Click);
            // 
            // linkLabelmagdec
            // 
            resources.ApplyResources(this.linkLabelmagdec, "linkLabelmagdec");
            this.linkLabelmagdec.Name = "linkLabelmagdec";
            this.linkLabelmagdec.TabStop = true;
            this.linkLabelmagdec.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label100
            // 
            resources.ApplyResources(this.label100, "label100");
            this.label100.Name = "label100";
            // 
            // TXT_declination_deg
            // 
            resources.ApplyResources(this.TXT_declination_deg, "TXT_declination_deg");
            this.TXT_declination_deg.Name = "TXT_declination_deg";
            this.TXT_declination_deg.Validated += new System.EventHandler(this.TXT_declination_Validated);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::MissionPlanner.Properties.Resources.compass;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // BUT_MagCalibrationLog
            // 
            resources.ApplyResources(this.BUT_MagCalibrationLog, "BUT_MagCalibrationLog");
            this.BUT_MagCalibrationLog.Name = "BUT_MagCalibrationLog";
            this.BUT_MagCalibrationLog.UseVisualStyleBackColor = true;
            this.BUT_MagCalibrationLog.Click += new System.EventHandler(this.BUT_MagCalibrationLog_Click);
            // 
            // CHK_autodec
            // 
            resources.ApplyResources(this.CHK_autodec, "CHK_autodec");
            this.CHK_autodec.Name = "CHK_autodec";
            this.CHK_autodec.UseVisualStyleBackColor = true;
            this.CHK_autodec.CheckedChanged += new System.EventHandler(this.CHK_autodec_CheckedChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // lbl_adv_cfg_only
            // 
            resources.ApplyResources(this.lbl_adv_cfg_only, "lbl_adv_cfg_only");
            this.lbl_adv_cfg_only.Name = "lbl_adv_cfg_only";
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked_1);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // imageLabel1
            // 
            this.imageLabel1.Image = ((System.Drawing.Image)(resources.GetObject("imageLabel1.Image")));
            resources.ApplyResources(this.imageLabel1, "imageLabel1");
            this.imageLabel1.Name = "imageLabel1";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::MissionPlanner.Properties.Resources.apmp2;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rb_px4pixhawk);
            this.groupBox1.Controls.Add(this.pictureBox4);
            this.groupBox1.Controls.Add(this.radioButtonmanual);
            this.groupBox1.Controls.Add(this.radioButton_external);
            this.groupBox1.Controls.Add(this.pictureBox3);
            this.groupBox1.Controls.Add(this.CMB_compass_orient);
            this.groupBox1.Controls.Add(this.radioButton_onboard);
            this.groupBox1.Controls.Add(this.pictureBox2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // rb_px4pixhawk
            // 
            resources.ApplyResources(this.rb_px4pixhawk, "rb_px4pixhawk");
            this.rb_px4pixhawk.Name = "rb_px4pixhawk";
            this.rb_px4pixhawk.UseVisualStyleBackColor = true;
            this.rb_px4pixhawk.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackgroundImage = global::MissionPlanner.Properties.Resources.pixhawk;
            resources.ApplyResources(this.pictureBox4, "pictureBox4");
            this.pictureBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.TabStop = false;
            // 
            // radioButtonmanual
            // 
            resources.ApplyResources(this.radioButtonmanual, "radioButtonmanual");
            this.radioButtonmanual.Checked = true;
            this.radioButtonmanual.Name = "radioButtonmanual";
            this.radioButtonmanual.TabStop = true;
            this.radioButtonmanual.UseVisualStyleBackColor = true;
            this.radioButtonmanual.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton_external
            // 
            resources.ApplyResources(this.radioButton_external, "radioButton_external");
            this.radioButton_external.Name = "radioButton_external";
            this.radioButton_external.UseVisualStyleBackColor = true;
            this.radioButton_external.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackgroundImage = global::MissionPlanner.Properties.Resources.maggps;
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // CMB_compass_orient
            // 
            this.CMB_compass_orient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.CMB_compass_orient, "CMB_compass_orient");
            this.CMB_compass_orient.FormattingEnabled = true;
            this.CMB_compass_orient.Name = "CMB_compass_orient";
            this.CMB_compass_orient.param = null;
            this.CMB_compass_orient.ParamName = null;
            // 
            // radioButton_onboard
            // 
            resources.ApplyResources(this.radioButton_onboard, "radioButton_onboard");
            this.radioButton_onboard.Name = "radioButton_onboard";
            this.radioButton_onboard.UseVisualStyleBackColor = true;
            this.radioButton_onboard.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // TXT_declination_min
            // 
            resources.ApplyResources(this.TXT_declination_min, "TXT_declination_min");
            this.TXT_declination_min.Name = "TXT_declination_min";
            this.TXT_declination_min.Validated += new System.EventHandler(this.TXT_declination_Validated);
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // CHK_enablecompass
            // 
            resources.ApplyResources(this.CHK_enablecompass, "CHK_enablecompass");
            this.CHK_enablecompass.Name = "CHK_enablecompass";
            this.CHK_enablecompass.OffValue = 0F;
            this.CHK_enablecompass.OnValue = 1F;
            this.CHK_enablecompass.param = null;
            this.CHK_enablecompass.ParamName = null;
            this.CHK_enablecompass.UseVisualStyleBackColor = true;
            this.CHK_enablecompass.CheckedChanged += new System.EventHandler(this.CHK_enablecompass_CheckedChanged);
            // 
            // ConfigHWCompass
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TXT_declination_min);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.imageLabel1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.lbl_adv_cfg_only);
            this.Controls.Add(this.CHK_enablecompass);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.CHK_autodec);
            this.Controls.Add(this.BUT_MagCalibrationLog);
            this.Controls.Add(this.BUT_MagCalibrationLive);
            this.Controls.Add(this.linkLabelmagdec);
            this.Controls.Add(this.label100);
            this.Controls.Add(this.TXT_declination_deg);
            this.Controls.Add(this.pictureBox1);
            this.Name = "ConfigHWCompass";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyButton BUT_MagCalibrationLive;
        private System.Windows.Forms.LinkLabel linkLabelmagdec;
        private System.Windows.Forms.Label label100;
        private System.Windows.Forms.TextBox TXT_declination_deg;
        private Controls.MavlinkCheckBox CHK_enablecompass;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Controls.MyButton BUT_MagCalibrationLog;
        private System.Windows.Forms.CheckBox CHK_autodec;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private Controls.MavlinkComboBox CMB_compass_orient;
        private System.Windows.Forms.Label lbl_adv_cfg_only;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label1;
        private Controls.ImageLabel imageLabel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonmanual;
        private System.Windows.Forms.RadioButton radioButton_external;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.RadioButton radioButton_onboard;
        private System.Windows.Forms.TextBox TXT_declination_min;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rb_px4pixhawk;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}
