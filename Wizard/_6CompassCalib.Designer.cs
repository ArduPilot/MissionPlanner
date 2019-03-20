using System.Windows.Forms;
using MissionPlanner;

namespace MissionPlanner.Wizard
{
    partial class _6CompassCalib
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_6CompassCalib));
            this.panel1 = new System.Windows.Forms.Panel();
            this.BUT_MagCalibrationLive = new MissionPlanner.Controls.MyButton();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radialGradientBG1 = new MissionPlanner.Controls.GradientBG();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BUT_compassorient = new MissionPlanner.Controls.MyButton();
            this.pictureBoxMouseOver3 = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxMouseOver2 = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxMouseOver1 = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOver3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOver2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOver1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.BUT_MagCalibrationLive);
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Name = "panel1";
            // 
            // BUT_MagCalibrationLive
            // 
            resources.ApplyResources(this.BUT_MagCalibrationLive, "BUT_MagCalibrationLive");
            this.BUT_MagCalibrationLive.Name = "BUT_MagCalibrationLive";
            this.BUT_MagCalibrationLive.UseVisualStyleBackColor = true;
            this.BUT_MagCalibrationLive.Click += new System.EventHandler(this.BUT_MagCalibration_Click);
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.LinkColor = System.Drawing.Color.White;
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
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
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.BUT_compassorient);
            this.panel2.Controls.Add(this.pictureBoxMouseOver3);
            this.panel2.Controls.Add(this.pictureBoxMouseOver2);
            this.panel2.Controls.Add(this.pictureBoxMouseOver1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Name = "panel2";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // BUT_compassorient
            // 
            resources.ApplyResources(this.BUT_compassorient, "BUT_compassorient");
            this.BUT_compassorient.Name = "BUT_compassorient";
            this.BUT_compassorient.UseVisualStyleBackColor = true;
            this.BUT_compassorient.Click += new System.EventHandler(this.BUT_compassorient_Click);
            // 
            // pictureBoxMouseOver3
            // 
            resources.ApplyResources(this.pictureBoxMouseOver3, "pictureBoxMouseOver3");
            this.pictureBoxMouseOver3.Image = global::MissionPlanner.Properties.Resources.apmp2;
            this.pictureBoxMouseOver3.ImageNormal = global::MissionPlanner.Properties.Resources.apmp2;
            this.pictureBoxMouseOver3.ImageOver = global::MissionPlanner.Properties.Resources.apmp2;
            this.pictureBoxMouseOver3.Name = "pictureBoxMouseOver3";
            this.pictureBoxMouseOver3.selected = false;
            this.pictureBoxMouseOver3.TabStop = false;
            this.pictureBoxMouseOver3.Tag = "apm2";
            this.pictureBoxMouseOver3.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // pictureBoxMouseOver2
            // 
            resources.ApplyResources(this.pictureBoxMouseOver2, "pictureBoxMouseOver2");
            this.pictureBoxMouseOver2.Image = global::MissionPlanner.Properties.Resources.px4;
            this.pictureBoxMouseOver2.ImageNormal = global::MissionPlanner.Properties.Resources.px4;
            this.pictureBoxMouseOver2.ImageOver = global::MissionPlanner.Properties.Resources.px4;
            this.pictureBoxMouseOver2.Name = "pictureBoxMouseOver2";
            this.pictureBoxMouseOver2.selected = false;
            this.pictureBoxMouseOver2.TabStop = false;
            this.pictureBoxMouseOver2.Tag = "px4";
            this.pictureBoxMouseOver2.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // pictureBoxMouseOver1
            // 
            resources.ApplyResources(this.pictureBoxMouseOver1, "pictureBoxMouseOver1");
            this.pictureBoxMouseOver1.Image = global::MissionPlanner.Properties.Resources.maggps;
            this.pictureBoxMouseOver1.ImageNormal = global::MissionPlanner.Properties.Resources.maggps;
            this.pictureBoxMouseOver1.ImageOver = global::MissionPlanner.Properties.Resources.maggps;
            this.pictureBoxMouseOver1.Name = "pictureBoxMouseOver1";
            this.pictureBoxMouseOver1.selected = false;
            this.pictureBoxMouseOver1.TabStop = false;
            this.pictureBoxMouseOver1.Tag = "external";
            this.pictureBoxMouseOver1.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // _6CompassCalib
            // 
            resources.ApplyResources(this, "$this");
            
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.planebackground;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.radialGradientBG1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "_6CompassCalib";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOver3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOver2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOver1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Controls.GradientBG radialGradientBG1;
        private Label label2;
        private Panel panel2;
        private Label label3;
        private Label label4;
        private Controls.PictureBoxMouseOver pictureBoxMouseOver1;
        private Controls.PictureBoxMouseOver pictureBoxMouseOver2;
        private LinkLabel linkLabel1;
        private Controls.MyButton BUT_MagCalibrationLive;
        private Controls.PictureBoxMouseOver pictureBoxMouseOver3;
        private Label label5;
        private Controls.MyButton BUT_compassorient;
        private Timer timer1;
        private Label label6;

    }
}
