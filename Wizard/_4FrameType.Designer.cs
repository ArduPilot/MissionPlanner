using System.Windows.Forms;
namespace MissionPlanner.Wizard
{
    partial class _4FrameType
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_4FrameType));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBoxMouseOverY = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxMouseOverH = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxMouseOvertrap = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxMouseOverX = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxMouseOverplus = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radialGradientBG1 = new MissionPlanner.Controls.GradientBG();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOvertrap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverplus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.pictureBoxMouseOverY);
            this.panel1.Controls.Add(this.pictureBoxMouseOverH);
            this.panel1.Controls.Add(this.pictureBoxMouseOvertrap);
            this.panel1.Controls.Add(this.pictureBoxMouseOverX);
            this.panel1.Controls.Add(this.pictureBoxMouseOverplus);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // pictureBoxMouseOverY
            // 
            this.pictureBoxMouseOverY.Image = global::MissionPlanner.Properties.Resources.quadframesnormal_14;
            this.pictureBoxMouseOverY.ImageNormal = global::MissionPlanner.Properties.Resources.quadframesnormal_14;
            this.pictureBoxMouseOverY.ImageOver = global::MissionPlanner.Properties.Resources.quadhover_14;
            resources.ApplyResources(this.pictureBoxMouseOverY, "pictureBoxMouseOverY");
            this.pictureBoxMouseOverY.Name = "pictureBoxMouseOverY";
            this.pictureBoxMouseOverY.selected = false;
            this.pictureBoxMouseOverY.TabStop = false;
            this.pictureBoxMouseOverY.Tag = "y6b";
            this.pictureBoxMouseOverY.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // pictureBoxMouseOverH
            // 
            this.pictureBoxMouseOverH.Image = global::MissionPlanner.Properties.Resources.Hframelight;
            this.pictureBoxMouseOverH.ImageNormal = global::MissionPlanner.Properties.Resources.Hframelight;
            this.pictureBoxMouseOverH.ImageOver = global::MissionPlanner.Properties.Resources.Hframe;
            resources.ApplyResources(this.pictureBoxMouseOverH, "pictureBoxMouseOverH");
            this.pictureBoxMouseOverH.Name = "pictureBoxMouseOverH";
            this.pictureBoxMouseOverH.selected = false;
            this.pictureBoxMouseOverH.TabStop = false;
            this.pictureBoxMouseOverH.Tag = "h";
            this.pictureBoxMouseOverH.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // pictureBoxMouseOvertrap
            // 
            this.pictureBoxMouseOvertrap.Image = global::MissionPlanner.Properties.Resources.quadframesnormal_05;
            this.pictureBoxMouseOvertrap.ImageNormal = global::MissionPlanner.Properties.Resources.quadframesnormal_05;
            this.pictureBoxMouseOvertrap.ImageOver = global::MissionPlanner.Properties.Resources.quadhover_05;
            resources.ApplyResources(this.pictureBoxMouseOvertrap, "pictureBoxMouseOvertrap");
            this.pictureBoxMouseOvertrap.Name = "pictureBoxMouseOvertrap";
            this.pictureBoxMouseOvertrap.selected = false;
            this.pictureBoxMouseOvertrap.TabStop = false;
            this.pictureBoxMouseOvertrap.Tag = "trap";
            this.pictureBoxMouseOvertrap.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // pictureBoxMouseOverX
            // 
            this.pictureBoxMouseOverX.Image = global::MissionPlanner.Properties.Resources.quadframesnormal_03;
            this.pictureBoxMouseOverX.ImageNormal = global::MissionPlanner.Properties.Resources.quadframesnormal_03;
            this.pictureBoxMouseOverX.ImageOver = global::MissionPlanner.Properties.Resources.quadhover_03;
            resources.ApplyResources(this.pictureBoxMouseOverX, "pictureBoxMouseOverX");
            this.pictureBoxMouseOverX.Name = "pictureBoxMouseOverX";
            this.pictureBoxMouseOverX.selected = false;
            this.pictureBoxMouseOverX.TabStop = false;
            this.pictureBoxMouseOverX.Tag = "x";
            this.pictureBoxMouseOverX.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // pictureBoxMouseOverplus
            // 
            this.pictureBoxMouseOverplus.Image = global::MissionPlanner.Properties.Resources.PlusFrames_06;
            this.pictureBoxMouseOverplus.ImageNormal = global::MissionPlanner.Properties.Resources.PlusFrames_06;
            this.pictureBoxMouseOverplus.ImageOver = global::MissionPlanner.Properties.Resources.PlusFramesGreen_06;
            resources.ApplyResources(this.pictureBoxMouseOverplus, "pictureBoxMouseOverplus");
            this.pictureBoxMouseOverplus.Name = "pictureBoxMouseOverplus";
            this.pictureBoxMouseOverplus.selected = false;
            this.pictureBoxMouseOverplus.TabStop = false;
            this.pictureBoxMouseOverplus.Tag = "+";
            this.pictureBoxMouseOverplus.Click += new System.EventHandler(this.pictureBox_Click);
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
            this.radialGradientBG1.BackColor = System.Drawing.Color.Black;
            this.radialGradientBG1.CenterColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(164)))), ((int)(((byte)(33)))));
            // 
            // 
            // 
            this.radialGradientBG1.Image.BackColor = System.Drawing.Color.Transparent;
            this.radialGradientBG1.Image.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radialGradientBG1.Image.ImeMode")));
            this.radialGradientBG1.Image.Location = ((System.Drawing.Point)(resources.GetObject("radialGradientBG1.Image.Location")));
            this.radialGradientBG1.Image.MaximumSize = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Image.MaximumSize")));
            this.radialGradientBG1.Image.MinimumSize = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Image.MinimumSize")));
            this.radialGradientBG1.Image.Name = "_Image";
            this.radialGradientBG1.Image.Size = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Image.Size")));
            this.radialGradientBG1.Image.TabIndex = ((int)(resources.GetObject("radialGradientBG1.Image.TabIndex")));
            this.radialGradientBG1.Image.TabStop = false;
            this.radialGradientBG1.Image.Visible = ((bool)(resources.GetObject("radialGradientBG1.Image.Visible")));
            // 
            // 
            // 
            this.radialGradientBG1.Label.AutoSize = ((bool)(resources.GetObject("radialGradientBG1.Label.AutoSize")));
            this.radialGradientBG1.Label.BackColor = System.Drawing.Color.Transparent;
            this.radialGradientBG1.Label.Font = ((System.Drawing.Font)(resources.GetObject("radialGradientBG1.Label.Font")));
            this.radialGradientBG1.Label.ForeColor = System.Drawing.Color.Black;
            this.radialGradientBG1.Label.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radialGradientBG1.Label.ImeMode")));
            this.radialGradientBG1.Label.Location = ((System.Drawing.Point)(resources.GetObject("radialGradientBG1.Label.Location")));
            this.radialGradientBG1.Label.Name = "_Label";
            this.radialGradientBG1.Label.Size = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Label.Size")));
            this.radialGradientBG1.Label.TabIndex = ((int)(resources.GetObject("radialGradientBG1.Label.TabIndex")));
            this.radialGradientBG1.Label.Text = resources.GetString("radialGradientBG1.Label.Text");
            resources.ApplyResources(this.radialGradientBG1, "radialGradientBG1");
            this.radialGradientBG1.Name = "radialGradientBG1";
            this.radialGradientBG1.OutsideColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(10)))));
            // 
            // _4FrameType
            // 
            
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.planebackground;
            this.Controls.Add(this.radialGradientBG1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "_4FrameType";
            resources.ApplyResources(this, "$this");
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOvertrap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverplus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Controls.GradientBG radialGradientBG1;
        private Label label2;
        private Controls.PictureBoxMouseOver pictureBoxMouseOverplus;
        private Controls.PictureBoxMouseOver pictureBoxMouseOverX;
        private Controls.PictureBoxMouseOver pictureBoxMouseOvertrap;
        private Controls.PictureBoxMouseOver pictureBoxMouseOverH;
        private Controls.PictureBoxMouseOver pictureBoxMouseOverY;
        private Label label3;

    }
}
