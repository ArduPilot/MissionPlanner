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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigHWCompass));
            this.BUT_MagCalibrationLive = new MissionPlanner.Controls.MyButton();
            this.linkLabelmagdec = new System.Windows.Forms.LinkLabel();
            this.label100 = new System.Windows.Forms.Label();
            this.TXT_declination_deg = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.CHK_autodec = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lbl_obmagresult = new System.Windows.Forms.TextBox();
            this.BUT_OBmagcalaccept = new MissionPlanner.Controls.MyButton();
            this.BUT_OBmagcalcancel = new MissionPlanner.Controls.MyButton();
            this.BUT_OBmagcalstart = new MissionPlanner.Controls.MyButton();
            this.CHK_enablecompass = new MissionPlanner.Controls.MavlinkCheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
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
            this.CMB_compass_orient.ParamName = null;
            this.CMB_compass_orient.SubControl = null;
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lbl_obmagresult);
            this.groupBox4.Controls.Add(this.BUT_OBmagcalaccept);
            this.groupBox4.Controls.Add(this.BUT_OBmagcalcancel);
            this.groupBox4.Controls.Add(this.BUT_OBmagcalstart);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // lbl_obmagresult
            // 
            resources.ApplyResources(this.lbl_obmagresult, "lbl_obmagresult");
            this.lbl_obmagresult.Name = "lbl_obmagresult";
            this.lbl_obmagresult.ReadOnly = true;
            // 
            // BUT_OBmagcalaccept
            // 
            resources.ApplyResources(this.BUT_OBmagcalaccept, "BUT_OBmagcalaccept");
            this.BUT_OBmagcalaccept.Name = "BUT_OBmagcalaccept";
            this.BUT_OBmagcalaccept.UseVisualStyleBackColor = true;
            this.BUT_OBmagcalaccept.Click += new System.EventHandler(this.BUT_OBmagcalaccept_Click);
            // 
            // BUT_OBmagcalcancel
            // 
            resources.ApplyResources(this.BUT_OBmagcalcancel, "BUT_OBmagcalcancel");
            this.BUT_OBmagcalcancel.Name = "BUT_OBmagcalcancel";
            this.BUT_OBmagcalcancel.UseVisualStyleBackColor = true;
            this.BUT_OBmagcalcancel.Click += new System.EventHandler(this.BUT_OBmagcalcancel_Click);
            // 
            // BUT_OBmagcalstart
            // 
            resources.ApplyResources(this.BUT_OBmagcalstart, "BUT_OBmagcalstart");
            this.BUT_OBmagcalstart.Name = "BUT_OBmagcalstart";
            this.BUT_OBmagcalstart.UseVisualStyleBackColor = true;
            this.BUT_OBmagcalstart.Click += new System.EventHandler(this.BUT_OBmagcalstart_Click);
            // 
            // CHK_enablecompass
            // 
            resources.ApplyResources(this.CHK_enablecompass, "CHK_enablecompass");
            this.CHK_enablecompass.Name = "CHK_enablecompass";
            this.CHK_enablecompass.OffValue = 0D;
            this.CHK_enablecompass.OnValue = 1D;
            this.CHK_enablecompass.ParamName = null;
            this.CHK_enablecompass.UseVisualStyleBackColor = true;
            this.CHK_enablecompass.CheckedChanged += new System.EventHandler(this.CHK_enablecompass_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.BUT_MagCalibrationLive);
            this.groupBox5.Controls.Add(this.linkLabel1);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // ConfigHWCompass
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TXT_declination_min);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.imageLabel1);
            this.Controls.Add(this.CHK_enablecompass);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.CHK_autodec);
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
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
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
        private System.Windows.Forms.CheckBox CHK_autodec;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private Controls.MavlinkComboBox CMB_compass_orient;
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
        private System.Windows.Forms.GroupBox groupBox4;
        private Controls.MyButton BUT_OBmagcalaccept;
        private Controls.MyButton BUT_OBmagcalcancel;
        private Controls.MyButton BUT_OBmagcalstart;
        private System.Windows.Forms.TextBox lbl_obmagresult;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label4;
    }
}
