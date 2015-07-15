using System.Windows.Forms;
namespace MissionPlanner.Wizard
{
    partial class _7BatteryMonitor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_7BatteryMonitor));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_mah = new System.Windows.Forms.TextBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CMB_sensor = new System.Windows.Forms.ComboBox();
            this.CMB_apmversion = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radialGradientBG1 = new MissionPlanner.Controls.GradientBG();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txt_mah);
            this.panel1.Controls.Add(this.pictureBox5);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.CMB_sensor);
            this.panel1.Controls.Add(this.CMB_apmversion);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Name = "panel1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txt_mah
            // 
            resources.ApplyResources(this.txt_mah, "txt_mah");
            this.txt_mah.Name = "txt_mah";
            // 
            // pictureBox5
            // 
            resources.ApplyResources(this.pictureBox5, "pictureBox5");
            this.pictureBox5.BackColor = System.Drawing.Color.White;
            this.pictureBox5.BackgroundImage = global::MissionPlanner.Properties.Resources.BR_APMPWRDEAN_2;
            this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // CMB_sensor
            // 
            resources.ApplyResources(this.CMB_sensor, "CMB_sensor");
            this.CMB_sensor.FormattingEnabled = true;
            this.CMB_sensor.Items.AddRange(new object[] {
            resources.GetString("CMB_sensor.Items")});
            this.CMB_sensor.Name = "CMB_sensor";
            this.CMB_sensor.SelectedIndexChanged += new System.EventHandler(this.CMB_sensor_SelectedIndexChanged);
            // 
            // CMB_apmversion
            // 
            resources.ApplyResources(this.CMB_apmversion, "CMB_apmversion");
            this.CMB_apmversion.FormattingEnabled = true;
            this.CMB_apmversion.Items.AddRange(new object[] {
            resources.GetString("CMB_apmversion.Items"),
            resources.GetString("CMB_apmversion.Items1"),
            resources.GetString("CMB_apmversion.Items2"),
            resources.GetString("CMB_apmversion.Items3"),
            resources.GetString("CMB_apmversion.Items4")});
            this.CMB_apmversion.Name = "CMB_apmversion";
            this.CMB_apmversion.SelectedIndexChanged += new System.EventHandler(this.CMB_apmversion_SelectedIndexChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // radialGradientBG1
            // 
            resources.ApplyResources(this.radialGradientBG1, "radialGradientBG1");
            this.radialGradientBG1.BackColor = System.Drawing.Color.Black;
            this.radialGradientBG1.CenterColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(164)))), ((int)(((byte)(33)))));
            // 
            // 
            // 
            this.radialGradientBG1.Image.AccessibleDescription = resources.GetString("radialGradientBG1.Image.AccessibleDescription");
            this.radialGradientBG1.Image.AccessibleName = resources.GetString("radialGradientBG1.Image.AccessibleName");
            this.radialGradientBG1.Image.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("radialGradientBG1.Image.Anchor")));
            this.radialGradientBG1.Image.BackColor = System.Drawing.Color.Transparent;
            this.radialGradientBG1.Image.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("radialGradientBG1.Image.BackgroundImage")));
            this.radialGradientBG1.Image.BackgroundImageLayout = ((System.Windows.Forms.ImageLayout)(resources.GetObject("radialGradientBG1.Image.BackgroundImageLayout")));
            this.radialGradientBG1.Image.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("radialGradientBG1.Image.Dock")));
            this.radialGradientBG1.Image.Font = ((System.Drawing.Font)(resources.GetObject("radialGradientBG1.Image.Font")));
            this.radialGradientBG1.Image.ImageLocation = resources.GetString("radialGradientBG1.Image.ImageLocation");
            this.radialGradientBG1.Image.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radialGradientBG1.Image.ImeMode")));
            this.radialGradientBG1.Image.Location = ((System.Drawing.Point)(resources.GetObject("radialGradientBG1.Image.Location")));
            this.radialGradientBG1.Image.MaximumSize = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Image.MaximumSize")));
            this.radialGradientBG1.Image.MinimumSize = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Image.MinimumSize")));
            this.radialGradientBG1.Image.Name = "_Image";
            this.radialGradientBG1.Image.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("radialGradientBG1.Image.RightToLeft")));
            this.radialGradientBG1.Image.Size = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Image.Size")));
            this.radialGradientBG1.Image.SizeMode = ((System.Windows.Forms.PictureBoxSizeMode)(resources.GetObject("radialGradientBG1.Image.SizeMode")));
            this.radialGradientBG1.Image.TabIndex = ((int)(resources.GetObject("radialGradientBG1.Image.TabIndex")));
            this.radialGradientBG1.Image.TabStop = false;
            this.radialGradientBG1.Image.Visible = ((bool)(resources.GetObject("radialGradientBG1.Image.Visible")));
            this.radialGradientBG1.Image.WaitOnLoad = ((bool)(resources.GetObject("radialGradientBG1.Image.WaitOnLoad")));
            // 
            // 
            // 
            this.radialGradientBG1.Label.AccessibleDescription = resources.GetString("radialGradientBG1.Label.AccessibleDescription");
            this.radialGradientBG1.Label.AccessibleName = resources.GetString("radialGradientBG1.Label.AccessibleName");
            this.radialGradientBG1.Label.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("radialGradientBG1.Label.Anchor")));
            this.radialGradientBG1.Label.AutoSize = ((bool)(resources.GetObject("radialGradientBG1.Label.AutoSize")));
            this.radialGradientBG1.Label.BackColor = System.Drawing.Color.Transparent;
            this.radialGradientBG1.Label.BackgroundImageLayout = ((System.Windows.Forms.ImageLayout)(resources.GetObject("radialGradientBG1.Label.BackgroundImageLayout")));
            this.radialGradientBG1.Label.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("radialGradientBG1.Label.Dock")));
            this.radialGradientBG1.Label.Font = ((System.Drawing.Font)(resources.GetObject("radialGradientBG1.Label.Font")));
            this.radialGradientBG1.Label.ForeColor = System.Drawing.Color.Black;
            this.radialGradientBG1.Label.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radialGradientBG1.Label.ImageAlign")));
            this.radialGradientBG1.Label.ImageIndex = ((int)(resources.GetObject("radialGradientBG1.Label.ImageIndex")));
            this.radialGradientBG1.Label.ImageKey = resources.GetString("radialGradientBG1.Label.ImageKey");
            this.radialGradientBG1.Label.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radialGradientBG1.Label.ImeMode")));
            this.radialGradientBG1.Label.Location = ((System.Drawing.Point)(resources.GetObject("radialGradientBG1.Label.Location")));
            this.radialGradientBG1.Label.MaximumSize = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Label.MaximumSize")));
            this.radialGradientBG1.Label.Name = "_Label";
            this.radialGradientBG1.Label.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("radialGradientBG1.Label.RightToLeft")));
            this.radialGradientBG1.Label.Size = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Label.Size")));
            this.radialGradientBG1.Label.TabIndex = ((int)(resources.GetObject("radialGradientBG1.Label.TabIndex")));
            this.radialGradientBG1.Label.Text = resources.GetString("radialGradientBG1.Label.Text");
            this.radialGradientBG1.Label.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radialGradientBG1.Label.TextAlign")));
            this.radialGradientBG1.Name = "radialGradientBG1";
            this.radialGradientBG1.OutsideColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(10)))));
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Name = "panel2";
            // 
            // _7BatteryMonitor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.planebackground;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.radialGradientBG1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "_7BatteryMonitor";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Controls.GradientBG radialGradientBG1;
        private Panel panel2;
        private Label label3;
        private ComboBox CMB_apmversion;
        private Label label2;
        private ComboBox CMB_sensor;
        private PictureBox pictureBox5;
        private Label label4;
        private TextBox txt_mah;

    }
}
