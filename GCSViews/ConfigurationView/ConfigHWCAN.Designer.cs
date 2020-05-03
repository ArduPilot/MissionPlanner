namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigHWCAN
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigHWCAN));
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.but_startenum = new MissionPlanner.Controls.MyButton();
            this.but_stopenum = new MissionPlanner.Controls.MyButton();
            this.but_saveconfig = new MissionPlanner.Controls.MyButton();
            this.but_factoryreset = new MissionPlanner.Controls.MyButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.mavlinkComboBox_can = new MissionPlanner.Controls.MavlinkComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.pictureBox5, "pictureBox5");
            this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.TabStop = false;
            // 
            // but_startenum
            // 
            resources.ApplyResources(this.but_startenum, "but_startenum");
            this.but_startenum.Name = "but_startenum";
            this.but_startenum.UseVisualStyleBackColor = true;
            this.but_startenum.Click += new System.EventHandler(this.but_startenum_Click);
            // 
            // but_stopenum
            // 
            resources.ApplyResources(this.but_stopenum, "but_stopenum");
            this.but_stopenum.Name = "but_stopenum";
            this.but_stopenum.UseVisualStyleBackColor = true;
            this.but_stopenum.Click += new System.EventHandler(this.but_stopenum_Click);
            // 
            // but_saveconfig
            // 
            resources.ApplyResources(this.but_saveconfig, "but_saveconfig");
            this.but_saveconfig.Name = "but_saveconfig";
            this.but_saveconfig.UseVisualStyleBackColor = true;
            this.but_saveconfig.Click += new System.EventHandler(this.but_saveconfig_Click);
            // 
            // but_factoryreset
            // 
            resources.ApplyResources(this.but_factoryreset, "but_factoryreset");
            this.but_factoryreset.Name = "but_factoryreset";
            this.but_factoryreset.UseVisualStyleBackColor = true;
            this.but_factoryreset.Click += new System.EventHandler(this.but_factoryreset_Click);
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // mavlinkComboBox_can
            // 
            this.mavlinkComboBox_can.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.mavlinkComboBox_can, "mavlinkComboBox_can");
            this.mavlinkComboBox_can.FormattingEnabled = true;
            this.mavlinkComboBox_can.Name = "mavlinkComboBox_can";
            this.mavlinkComboBox_can.ParamName = null;
            this.mavlinkComboBox_can.SubControl = null;
            this.mavlinkComboBox_can.SelectedIndexChanged += new System.EventHandler(this.mavlinkComboBox_can_SelectedIndexChanged);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.but_startenum);
            this.groupBox1.Controls.Add(this.but_stopenum);
            this.groupBox1.Controls.Add(this.but_saveconfig);
            this.groupBox1.Controls.Add(this.but_factoryreset);
            this.groupBox1.Controls.Add(this.checkBox1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // ConfigHWCAN
            // 
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mavlinkComboBox_can);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.pictureBox5);
            this.Name = "ConfigHWCAN";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.PictureBox pictureBox5;
        private Controls.MyButton but_startenum;
        private Controls.MyButton but_stopenum;
        private Controls.MyButton but_saveconfig;
        private Controls.MyButton but_factoryreset;
        private System.Windows.Forms.CheckBox checkBox1;
        private Controls.MavlinkComboBox mavlinkComboBox_can;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
