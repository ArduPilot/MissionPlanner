namespace ArdupilotMega.GCSViews.ConfigurationView
{
    partial class ConfigHWSonar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigHWSonar));
            this.CMB_sonartype = new System.Windows.Forms.ComboBox();
            this.CHK_enablesonar = new ArdupilotMega.Controls.MavlinkCheckBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // CMB_sonartype
            // 
            this.CMB_sonartype.FormattingEnabled = true;
            this.CMB_sonartype.Items.AddRange(new object[] {
            resources.GetString("CMB_sonartype.Items"),
            resources.GetString("CMB_sonartype.Items1"),
            resources.GetString("CMB_sonartype.Items2"),
            resources.GetString("CMB_sonartype.Items3")});
            resources.ApplyResources(this.CMB_sonartype, "CMB_sonartype");
            this.CMB_sonartype.Name = "CMB_sonartype";
            this.CMB_sonartype.SelectedIndexChanged += new System.EventHandler(this.CMB_sonartype_SelectedIndexChanged);
            // 
            // CHK_enablesonar
            // 
            resources.ApplyResources(this.CHK_enablesonar, "CHK_enablesonar");
            this.CHK_enablesonar.Name = "CHK_enablesonar";
            this.CHK_enablesonar.OffValue = 0F;
            this.CHK_enablesonar.OnValue = 1F;
            this.CHK_enablesonar.param = null;
            this.CHK_enablesonar.ParamName = null;
            this.CHK_enablesonar.UseVisualStyleBackColor = true;
            this.CHK_enablesonar.CheckedChanged += new System.EventHandler(this.CHK_enablesonar_CheckedChanged);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.White;
            this.pictureBox3.BackgroundImage = global::ArdupilotMega.Properties.Resources.sonar;
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // ConfigHWSonar
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CHK_enablesonar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CMB_sonartype);
            this.Controls.Add(this.pictureBox3);
            this.Name = "ConfigHWSonar";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_sonartype;
        private ArdupilotMega.Controls.MavlinkCheckBox CHK_enablesonar;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
