using System.Windows.Forms;
namespace MissionPlanner.Wizard
{
    partial class _1Intro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_1Intro));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxwizard = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBoxheli = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxselectvehicle = new System.Windows.Forms.PictureBox();
            this.pictureBoxquad = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxrover = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxplane = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.radialGradientBG1 = new MissionPlanner.Controls.GradientBG();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxwizard)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxheli)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxselectvehicle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxquad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxrover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxplane)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pictureBoxwizard);
            this.panel1.Name = "panel1";
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
            // pictureBoxwizard
            // 
            resources.ApplyResources(this.pictureBoxwizard, "pictureBoxwizard");
            this.pictureBoxwizard.Image = global::MissionPlanner.Properties.Resources.wizardicon1;
            this.pictureBoxwizard.Name = "pictureBoxwizard";
            this.pictureBoxwizard.TabStop = false;
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.pictureBoxheli);
            this.panel2.Controls.Add(this.pictureBoxselectvehicle);
            this.panel2.Controls.Add(this.pictureBoxquad);
            this.panel2.Controls.Add(this.pictureBoxrover);
            this.panel2.Controls.Add(this.pictureBoxplane);
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
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // pictureBoxheli
            // 
            resources.ApplyResources(this.pictureBoxheli, "pictureBoxheli");
            this.pictureBoxheli.Image = global::MissionPlanner.Properties.Resources.light_06;
            this.pictureBoxheli.ImageNormal = global::MissionPlanner.Properties.Resources.light_06;
            this.pictureBoxheli.ImageOver = global::MissionPlanner.Properties.Resources._01_06;
            this.pictureBoxheli.Name = "pictureBoxheli";
            this.pictureBoxheli.selected = false;
            this.pictureBoxheli.TabStop = false;
            this.pictureBoxheli.Tag = "heli";
            this.pictureBoxheli.Click += new System.EventHandler(this.pictureBoxheli_Click);
            // 
            // pictureBoxselectvehicle
            // 
            resources.ApplyResources(this.pictureBoxselectvehicle, "pictureBoxselectvehicle");
            this.pictureBoxselectvehicle.Image = global::MissionPlanner.Properties.Resources.selectvehicle;
            this.pictureBoxselectvehicle.Name = "pictureBoxselectvehicle";
            this.pictureBoxselectvehicle.TabStop = false;
            // 
            // pictureBoxquad
            // 
            resources.ApplyResources(this.pictureBoxquad, "pictureBoxquad");
            this.pictureBoxquad.Image = global::MissionPlanner.Properties.Resources.light_05;
            this.pictureBoxquad.ImageNormal = global::MissionPlanner.Properties.Resources.light_05;
            this.pictureBoxquad.ImageOver = global::MissionPlanner.Properties.Resources._01_05;
            this.pictureBoxquad.Name = "pictureBoxquad";
            this.pictureBoxquad.selected = false;
            this.pictureBoxquad.TabStop = false;
            this.pictureBoxquad.Tag = "copter";
            this.pictureBoxquad.Click += new System.EventHandler(this.pictureBoxquad_Click);
            // 
            // pictureBoxrover
            // 
            resources.ApplyResources(this.pictureBoxrover, "pictureBoxrover");
            this.pictureBoxrover.Image = global::MissionPlanner.Properties.Resources.light_03;
            this.pictureBoxrover.ImageNormal = global::MissionPlanner.Properties.Resources.light_03;
            this.pictureBoxrover.ImageOver = global::MissionPlanner.Properties.Resources._01_03;
            this.pictureBoxrover.Name = "pictureBoxrover";
            this.pictureBoxrover.selected = false;
            this.pictureBoxrover.TabStop = false;
            this.pictureBoxrover.Tag = "rover";
            this.pictureBoxrover.Click += new System.EventHandler(this.pictureBoxrover_Click);
            // 
            // pictureBoxplane
            // 
            resources.ApplyResources(this.pictureBoxplane, "pictureBoxplane");
            this.pictureBoxplane.Image = global::MissionPlanner.Properties.Resources.light_01;
            this.pictureBoxplane.ImageNormal = global::MissionPlanner.Properties.Resources.light_01;
            this.pictureBoxplane.ImageOver = global::MissionPlanner.Properties.Resources._01_01;
            this.pictureBoxplane.Name = "pictureBoxplane";
            this.pictureBoxplane.selected = false;
            this.pictureBoxplane.TabStop = false;
            this.pictureBoxplane.Tag = "plane";
            this.pictureBoxplane.Click += new System.EventHandler(this.pictureBoxplane_Click);
            // 
            // radialGradientBG1
            // 
            resources.ApplyResources(this.radialGradientBG1, "radialGradientBG1");
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
            this.radialGradientBG1.Image.Image = global::MissionPlanner.Properties.Resources.missionplannerlogodark;
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
            this.radialGradientBG1.Label.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radialGradientBG1.Label.ImageAlign")));
            this.radialGradientBG1.Label.ImageIndex = ((int)(resources.GetObject("radialGradientBG1.Label.ImageIndex")));
            this.radialGradientBG1.Label.ImageKey = resources.GetString("radialGradientBG1.Label.ImageKey");
            this.radialGradientBG1.Label.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radialGradientBG1.Label.ImeMode")));
            this.radialGradientBG1.Label.Location = ((System.Drawing.Point)(resources.GetObject("radialGradientBG1.Label.Location")));
            this.radialGradientBG1.Label.MaximumSize = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Label.MaximumSize")));
            this.radialGradientBG1.Label.Name = "_Label";
            this.radialGradientBG1.Label.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("radialGradientBG1.Label.RightToLeft")));
            this.radialGradientBG1.Label.TabIndex = ((int)(resources.GetObject("radialGradientBG1.Label.TabIndex")));
            this.radialGradientBG1.Label.Text = resources.GetString("radialGradientBG1.Label.Text");
            this.radialGradientBG1.Label.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radialGradientBG1.Label.TextAlign")));
            this.radialGradientBG1.Label.Visible = ((bool)(resources.GetObject("radialGradientBG1.Label.Visible")));
            this.radialGradientBG1.Name = "radialGradientBG1";
            this.radialGradientBG1.OutsideColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(10)))));
            // 
            // _1Intro
            // 
            resources.ApplyResources(this, "$this");
            
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.planebackground;
            this.Controls.Add(this.radialGradientBG1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "_1Intro";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxwizard)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxheli)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxselectvehicle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxquad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxrover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxplane)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private PictureBox pictureBoxwizard;
        private Label label2;
        private Label label1;
        private Panel panel2;
        private Controls.PictureBoxMouseOver pictureBoxplane;
        private Controls.PictureBoxMouseOver pictureBoxquad;
        private Controls.PictureBoxMouseOver pictureBoxrover;
        private Controls.GradientBG radialGradientBG1;
        private Controls.PictureBoxMouseOver pictureBoxheli;
        private PictureBox pictureBoxselectvehicle;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;

    }
}
