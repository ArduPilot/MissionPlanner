namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigHWBT
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigHWBT));
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.BUT_btsettings = new MissionPlanner.Controls.MyButton();
            this.cmb_baud = new System.Windows.Forms.ComboBox();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.txt_pin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
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
            this.pictureBox5.BackgroundImage = global::MissionPlanner.Properties.Resources.MinimOSD;
            resources.ApplyResources(this.pictureBox5, "pictureBox5");
            this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox5.Image = global::MissionPlanner.Properties.Resources.BT_hc06;
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.TabStop = false;
            // 
            // BUT_btsettings
            // 
            resources.ApplyResources(this.BUT_btsettings, "BUT_btsettings");
            this.BUT_btsettings.Name = "BUT_btsettings";
            this.BUT_btsettings.UseVisualStyleBackColor = true;
            this.BUT_btsettings.Click += new System.EventHandler(this.BUT_btsettings_Click);
            // 
            // cmb_baud
            // 
            this.cmb_baud.FormattingEnabled = true;
            this.cmb_baud.Items.AddRange(new object[] {
            resources.GetString("cmb_baud.Items"),
            resources.GetString("cmb_baud.Items1"),
            resources.GetString("cmb_baud.Items2"),
            resources.GetString("cmb_baud.Items3"),
            resources.GetString("cmb_baud.Items4"),
            resources.GetString("cmb_baud.Items5"),
            resources.GetString("cmb_baud.Items6"),
            resources.GetString("cmb_baud.Items7")});
            resources.ApplyResources(this.cmb_baud, "cmb_baud");
            this.cmb_baud.Name = "cmb_baud";
            // 
            // txt_name
            // 
            resources.ApplyResources(this.txt_name, "txt_name");
            this.txt_name.Name = "txt_name";
            // 
            // txt_pin
            // 
            resources.ApplyResources(this.txt_pin, "txt_pin");
            this.txt_pin.Name = "txt_pin";
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
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // ConfigHWBT
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_pin);
            this.Controls.Add(this.txt_name);
            this.Controls.Add(this.cmb_baud);
            this.Controls.Add(this.BUT_btsettings);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.pictureBox5);
            this.Name = "ConfigHWBT";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.PictureBox pictureBox5;
        private Controls.MyButton BUT_btsettings;
        private System.Windows.Forms.ComboBox cmb_baud;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.TextBox txt_pin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
