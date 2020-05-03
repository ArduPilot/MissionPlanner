namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigHWAirspeed
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigHWAirspeed));
            this.CHK_enableairspeed = new MissionPlanner.Controls.MavlinkCheckBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.CHK_airspeeduse = new MissionPlanner.Controls.MavlinkCheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mavlinkCheckBoxAirspeed_pin = new MissionPlanner.Controls.MavlinkComboBox();
            this.lbl_airspeed_pin = new System.Windows.Forms.Label();
            this.mavlinkComboBoxARSPD_TYPE = new MissionPlanner.Controls.MavlinkComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // CHK_enableairspeed
            // 
            resources.ApplyResources(this.CHK_enableairspeed, "CHK_enableairspeed");
            this.CHK_enableairspeed.Name = "CHK_enableairspeed";
            this.CHK_enableairspeed.OffValue = 0D;
            this.CHK_enableairspeed.OnValue = 1D;
            this.CHK_enableairspeed.ParamName = null;
            this.CHK_enableairspeed.UseVisualStyleBackColor = true;
            this.CHK_enableairspeed.CheckedChanged += new System.EventHandler(this.CHK_enableairspeed_CheckedChanged);
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.White;
            this.pictureBox4.BackgroundImage = global::MissionPlanner.Properties.Resources.airspeed;
            resources.ApplyResources(this.pictureBox4, "pictureBox4");
            this.pictureBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.TabStop = false;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // CHK_airspeeduse
            // 
            resources.ApplyResources(this.CHK_airspeeduse, "CHK_airspeeduse");
            this.CHK_airspeeduse.Name = "CHK_airspeeduse";
            this.CHK_airspeeduse.OffValue = 0D;
            this.CHK_airspeeduse.OnValue = 1D;
            this.CHK_airspeeduse.ParamName = null;
            this.CHK_airspeeduse.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // mavlinkCheckBoxAirspeed_pin
            // 
            this.mavlinkCheckBoxAirspeed_pin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mavlinkCheckBoxAirspeed_pin.DropDownWidth = 200;
            resources.ApplyResources(this.mavlinkCheckBoxAirspeed_pin, "mavlinkCheckBoxAirspeed_pin");
            this.mavlinkCheckBoxAirspeed_pin.Name = "mavlinkCheckBoxAirspeed_pin";
            this.mavlinkCheckBoxAirspeed_pin.ParamName = null;
            this.mavlinkCheckBoxAirspeed_pin.SubControl = null;
            // 
            // lbl_airspeed_pin
            // 
            resources.ApplyResources(this.lbl_airspeed_pin, "lbl_airspeed_pin");
            this.lbl_airspeed_pin.Name = "lbl_airspeed_pin";
            // 
            // mavlinkComboBoxARSPD_TYPE
            // 
            this.mavlinkComboBoxARSPD_TYPE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.mavlinkComboBoxARSPD_TYPE, "mavlinkComboBoxARSPD_TYPE");
            this.mavlinkComboBoxARSPD_TYPE.FormattingEnabled = true;
            this.mavlinkComboBoxARSPD_TYPE.Name = "mavlinkComboBoxARSPD_TYPE";
            this.mavlinkComboBoxARSPD_TYPE.ParamName = null;
            this.mavlinkComboBoxARSPD_TYPE.SubControl = null;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ConfigHWAirspeed
            // 
            
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mavlinkComboBoxARSPD_TYPE);
            this.Controls.Add(this.lbl_airspeed_pin);
            this.Controls.Add(this.mavlinkCheckBoxAirspeed_pin);
            this.Controls.Add(this.CHK_airspeeduse);
            this.Controls.Add(this.CHK_enableairspeed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.pictureBox4);
            this.Name = "ConfigHWAirspeed";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MavlinkCheckBox CHK_enableairspeed;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private Controls.MavlinkCheckBox CHK_airspeeduse;
        private System.Windows.Forms.Label label2;
        private Controls.MavlinkComboBox mavlinkCheckBoxAirspeed_pin;
        private System.Windows.Forms.Label lbl_airspeed_pin;
        private Controls.MavlinkComboBox mavlinkComboBoxARSPD_TYPE;
        private System.Windows.Forms.Label label1;
    }
}
