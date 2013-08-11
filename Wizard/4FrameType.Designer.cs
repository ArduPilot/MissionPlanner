using System.Windows.Forms;
namespace ArdupilotMega.Wizard
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBoxMouseOvertrap = new ArdupilotMega.Controls.PictureBoxMouseOver();
            this.pictureBoxMouseOverX = new ArdupilotMega.Controls.PictureBoxMouseOver();
            this.pictureBoxMouseOverplus = new ArdupilotMega.Controls.PictureBoxMouseOver();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radialGradientBG1 = new ArdupilotMega.Controls.GradientBG();
            this.pictureBoxMouseOverH = new ArdupilotMega.Controls.PictureBoxMouseOver();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOvertrap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverplus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverH)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBoxMouseOverH);
            this.panel1.Controls.Add(this.pictureBoxMouseOvertrap);
            this.panel1.Controls.Add(this.pictureBoxMouseOverX);
            this.panel1.Controls.Add(this.pictureBoxMouseOverplus);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(30, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(740, 410);
            this.panel1.TabIndex = 1;
            // 
            // pictureBoxMouseOvertrap
            // 
            this.pictureBoxMouseOvertrap.Image = global::MissionPlanner.Properties.Resources.quadframesnormal_05;
            this.pictureBoxMouseOvertrap.ImageNormal = global::MissionPlanner.Properties.Resources.quadframesnormal_05;
            this.pictureBoxMouseOvertrap.ImageOver = global::MissionPlanner.Properties.Resources.quadhover_05;
            this.pictureBoxMouseOvertrap.Location = new System.Drawing.Point(142, 252);
            this.pictureBoxMouseOvertrap.Name = "pictureBoxMouseOvertrap";
            this.pictureBoxMouseOvertrap.selected = false;
            this.pictureBoxMouseOvertrap.Size = new System.Drawing.Size(120, 120);
            this.pictureBoxMouseOvertrap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMouseOvertrap.TabIndex = 6;
            this.pictureBoxMouseOvertrap.TabStop = false;
            this.pictureBoxMouseOvertrap.Tag = "trap";
            this.pictureBoxMouseOvertrap.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // pictureBoxMouseOverX
            // 
            this.pictureBoxMouseOverX.Image = global::MissionPlanner.Properties.Resources.quadframesnormal_03;
            this.pictureBoxMouseOverX.ImageNormal = global::MissionPlanner.Properties.Resources.quadframesnormal_03;
            this.pictureBoxMouseOverX.ImageOver = global::MissionPlanner.Properties.Resources.quadhover_03;
            this.pictureBoxMouseOverX.Location = new System.Drawing.Point(142, 99);
            this.pictureBoxMouseOverX.Name = "pictureBoxMouseOverX";
            this.pictureBoxMouseOverX.selected = false;
            this.pictureBoxMouseOverX.Size = new System.Drawing.Size(120, 120);
            this.pictureBoxMouseOverX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMouseOverX.TabIndex = 5;
            this.pictureBoxMouseOverX.TabStop = false;
            this.pictureBoxMouseOverX.Tag = "x";
            this.pictureBoxMouseOverX.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // pictureBoxMouseOverplus
            // 
            this.pictureBoxMouseOverplus.Image = global::MissionPlanner.Properties.Resources.PlusFrames_06;
            this.pictureBoxMouseOverplus.ImageNormal = global::MissionPlanner.Properties.Resources.PlusFrames_06;
            this.pictureBoxMouseOverplus.ImageOver = global::MissionPlanner.Properties.Resources.PlusFramesGreen_06;
            this.pictureBoxMouseOverplus.Location = new System.Drawing.Point(468, 99);
            this.pictureBoxMouseOverplus.Name = "pictureBoxMouseOverplus";
            this.pictureBoxMouseOverplus.selected = false;
            this.pictureBoxMouseOverplus.Size = new System.Drawing.Size(120, 120);
            this.pictureBoxMouseOverplus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMouseOverplus.TabIndex = 4;
            this.pictureBoxMouseOverplus.TabStop = false;
            this.pictureBoxMouseOverplus.Tag = "+";
            this.pictureBoxMouseOverplus.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(307, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "Please select your frame layout from below:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "FRAME LAYOUT";
            // 
            // radialGradientBG1
            // 
            this.radialGradientBG1.BackColor = System.Drawing.Color.Black;
            this.radialGradientBG1.CenterColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(164)))), ((int)(((byte)(33)))));
            // 
            // 
            // 
            this.radialGradientBG1.Image.BackColor = System.Drawing.Color.Transparent;
            this.radialGradientBG1.Image.Location = new System.Drawing.Point(38, 10);
            this.radialGradientBG1.Image.MaximumSize = new System.Drawing.Size(288, 72);
            this.radialGradientBG1.Image.MinimumSize = new System.Drawing.Size(288, 72);
            this.radialGradientBG1.Image.Name = "_Image";
            this.radialGradientBG1.Image.Size = new System.Drawing.Size(288, 72);
            this.radialGradientBG1.Image.TabIndex = 0;
            this.radialGradientBG1.Image.TabStop = false;
            this.radialGradientBG1.Image.Visible = false;
            // 
            // 
            // 
            this.radialGradientBG1.Label.AutoSize = true;
            this.radialGradientBG1.Label.BackColor = System.Drawing.Color.Transparent;
            this.radialGradientBG1.Label.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radialGradientBG1.Label.ForeColor = System.Drawing.Color.Black;
            this.radialGradientBG1.Label.Location = new System.Drawing.Point(30, 5);
            this.radialGradientBG1.Label.Name = "_Label";
            this.radialGradientBG1.Label.Size = new System.Drawing.Size(291, 29);
            this.radialGradientBG1.Label.TabIndex = 1;
            this.radialGradientBG1.Label.Text = "Select your frame layout";
            this.radialGradientBG1.Location = new System.Drawing.Point(0, 0);
            this.radialGradientBG1.Name = "radialGradientBG1";
            this.radialGradientBG1.OutsideColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(10)))));
            this.radialGradientBG1.Size = new System.Drawing.Size(800, 41);
            this.radialGradientBG1.TabIndex = 4;
            // 
            // pictureBoxMouseOverH
            // 
            this.pictureBoxMouseOverH.Image = global::MissionPlanner.Properties.Resources.Hframelight;
            this.pictureBoxMouseOverH.ImageNormal = global::MissionPlanner.Properties.Resources.Hframelight;
            this.pictureBoxMouseOverH.ImageOver = global::MissionPlanner.Properties.Resources.Hframe;
            this.pictureBoxMouseOverH.Location = new System.Drawing.Point(468, 252);
            this.pictureBoxMouseOverH.Name = "pictureBoxMouseOverH";
            this.pictureBoxMouseOverH.selected = false;
            this.pictureBoxMouseOverH.Size = new System.Drawing.Size(120, 120);
            this.pictureBoxMouseOverH.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMouseOverH.TabIndex = 7;
            this.pictureBoxMouseOverH.TabStop = false;
            this.pictureBoxMouseOverH.Tag = "h";
            this.pictureBoxMouseOverH.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // _4FrameType
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.planebackground;
            this.Controls.Add(this.radialGradientBG1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "_4FrameType";
            this.Size = new System.Drawing.Size(800, 500);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOvertrap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverplus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOverH)).EndInit();
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

    }
}
