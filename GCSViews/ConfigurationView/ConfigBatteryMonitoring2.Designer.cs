namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigBatteryMonitoring2
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
            _startup = true;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigBatteryMonitoring2));
            this.label47 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.TXT_battcapacity = new System.Windows.Forms.TextBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CHK_speechbattery = new System.Windows.Forms.CheckBox();
            this.TXT_measuredvoltage = new System.Windows.Forms.TextBox();
            this.TXT_voltage = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.TXT_divider = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.TXT_ampspervolt = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txt_current = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_meascurrent = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mavlinkComboBox1 = new MissionPlanner.Controls.MavlinkComboBox();
            this.mavlinkComboBox2 = new MissionPlanner.Controls.MavlinkComboBox();
            this.mavlinkComboBox3 = new MissionPlanner.Controls.MavlinkComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label47
            // 
            resources.ApplyResources(this.label47, "label47");
            this.label47.Name = "label47";
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // TXT_battcapacity
            // 
            resources.ApplyResources(this.TXT_battcapacity, "TXT_battcapacity");
            this.TXT_battcapacity.Name = "TXT_battcapacity";
            this.TXT_battcapacity.Validated += new System.EventHandler(this.TXT_battcapacity_Validated);
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.White;
            this.pictureBox5.BackgroundImage = global::MissionPlanner.Properties.Resources.BR_APMPWRDEAN_2;
            resources.ApplyResources(this.pictureBox5, "pictureBox5");
            this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
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
            // CHK_speechbattery
            // 
            resources.ApplyResources(this.CHK_speechbattery, "CHK_speechbattery");
            this.CHK_speechbattery.Name = "CHK_speechbattery";
            this.CHK_speechbattery.UseVisualStyleBackColor = true;
            this.CHK_speechbattery.CheckedChanged += new System.EventHandler(this.CHK_speechbattery_CheckedChanged);
            // 
            // TXT_measuredvoltage
            // 
            resources.ApplyResources(this.TXT_measuredvoltage, "TXT_measuredvoltage");
            this.TXT_measuredvoltage.Name = "TXT_measuredvoltage";
            this.TXT_measuredvoltage.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TXT_measuredvoltage_PreviewKeyDown);
            this.TXT_measuredvoltage.Validated += new System.EventHandler(this.TXT_measuredvoltage_Validated);
            // 
            // TXT_voltage
            // 
            resources.ApplyResources(this.TXT_voltage, "TXT_voltage");
            this.TXT_voltage.Name = "TXT_voltage";
            this.TXT_voltage.ReadOnly = true;
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // TXT_divider
            // 
            resources.ApplyResources(this.TXT_divider, "TXT_divider");
            this.TXT_divider.Name = "TXT_divider";
            this.TXT_divider.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TXT_divider_PreviewKeyDown);
            this.TXT_divider.Validated += new System.EventHandler(this.TXT_divider_Validated);
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // TXT_ampspervolt
            // 
            resources.ApplyResources(this.TXT_ampspervolt, "TXT_ampspervolt");
            this.TXT_ampspervolt.Name = "TXT_ampspervolt";
            this.TXT_ampspervolt.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TXT_ampspervolt_PreviewKeyDown);
            this.TXT_ampspervolt.Validated += new System.EventHandler(this.TXT_ampspervolt_Validated);
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.Name = "label32";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txt_current);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.txt_meascurrent);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label32);
            this.groupBox4.Controls.Add(this.label33);
            this.groupBox4.Controls.Add(this.TXT_ampspervolt);
            this.groupBox4.Controls.Add(this.label34);
            this.groupBox4.Controls.Add(this.TXT_divider);
            this.groupBox4.Controls.Add(this.label35);
            this.groupBox4.Controls.Add(this.TXT_voltage);
            this.groupBox4.Controls.Add(this.TXT_measuredvoltage);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // txt_current
            // 
            resources.ApplyResources(this.txt_current, "txt_current");
            this.txt_current.Name = "txt_current";
            this.txt_current.ReadOnly = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txt_meascurrent
            // 
            resources.ApplyResources(this.txt_meascurrent, "txt_meascurrent");
            this.txt_meascurrent.Name = "txt_meascurrent";
            this.txt_meascurrent.Validated += new System.EventHandler(this.txt_meascurrent_Validated);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // mavlinkComboBox1
            // 
            this.mavlinkComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.mavlinkComboBox1, "mavlinkComboBox1");
            this.mavlinkComboBox1.FormattingEnabled = true;
            this.mavlinkComboBox1.Name = "mavlinkComboBox1";
            this.mavlinkComboBox1.ParamName = null;
            this.mavlinkComboBox1.SubControl = null;
            // 
            // mavlinkComboBox2
            // 
            this.mavlinkComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.mavlinkComboBox2, "mavlinkComboBox2");
            this.mavlinkComboBox2.FormattingEnabled = true;
            this.mavlinkComboBox2.Name = "mavlinkComboBox2";
            this.mavlinkComboBox2.ParamName = null;
            this.mavlinkComboBox2.SubControl = null;
            // 
            // mavlinkComboBox3
            // 
            this.mavlinkComboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.mavlinkComboBox3, "mavlinkComboBox3");
            this.mavlinkComboBox3.FormattingEnabled = true;
            this.mavlinkComboBox3.Name = "mavlinkComboBox3";
            this.mavlinkComboBox3.ParamName = null;
            this.mavlinkComboBox3.SubControl = null;
            // 
            // ConfigBatteryMonitoring2
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.mavlinkComboBox3);
            this.Controls.Add(this.mavlinkComboBox2);
            this.Controls.Add(this.mavlinkComboBox1);
            this.Controls.Add(this.CHK_speechbattery);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label47);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.TXT_battcapacity);
            this.Controls.Add(this.pictureBox5);
            this.Name = "ConfigBatteryMonitoring2";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox TXT_battcapacity;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox CHK_speechbattery;
        private System.Windows.Forms.TextBox TXT_measuredvoltage;
        private System.Windows.Forms.TextBox TXT_voltage;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox TXT_divider;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox TXT_ampspervolt;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txt_current;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_meascurrent;
        private System.Windows.Forms.Label label3;
        private Controls.MavlinkComboBox mavlinkComboBox1;
        private Controls.MavlinkComboBox mavlinkComboBox2;
        private Controls.MavlinkComboBox mavlinkComboBox3;
    }
}
