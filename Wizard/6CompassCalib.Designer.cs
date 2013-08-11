using System.Windows.Forms;
namespace ArdupilotMega.Wizard
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
            this.BUT_MagCalibrationLive = new ArdupilotMega.Controls.MyButton();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radialGradientBG1 = new ArdupilotMega.Controls.GradientBG();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.BUT_compassorient = new ArdupilotMega.Controls.MyButton();
            this.pictureBoxMouseOver3 = new ArdupilotMega.Controls.PictureBoxMouseOver();
            this.pictureBoxMouseOver2 = new ArdupilotMega.Controls.PictureBoxMouseOver();
            this.pictureBoxMouseOver1 = new ArdupilotMega.Controls.PictureBoxMouseOver();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label6 = new System.Windows.Forms.Label();
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
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.BUT_MagCalibrationLive);
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(30, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(740, 158);
            this.panel1.TabIndex = 1;
            // 
            // BUT_MagCalibrationLive
            // 
            this.BUT_MagCalibrationLive.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_MagCalibrationLive.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_MagCalibrationLive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_MagCalibrationLive.Location = new System.Drawing.Point(115, 106);
            this.BUT_MagCalibrationLive.Name = "BUT_MagCalibrationLive";
            this.BUT_MagCalibrationLive.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_MagCalibrationLive.Size = new System.Drawing.Size(66, 27);
            this.BUT_MagCalibrationLive.TabIndex = 86;
            this.BUT_MagCalibrationLive.Text = "Live Calibration";
            this.BUT_MagCalibrationLive.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_MagCalibrationLive.UseVisualStyleBackColor = true;
            this.BUT_MagCalibrationLive.Click += new System.EventHandler(this.BUT_MagCalibration_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.linkLabel1.LinkColor = System.Drawing.Color.White;
            this.linkLabel1.Location = new System.Drawing.Point(208, 113);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(90, 13);
            this.linkLabel1.TabIndex = 85;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Youtube Example";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(539, 59);
            this.label2.TabIndex = 3;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(241, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "COMPASS CALIBRATION";
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
            this.radialGradientBG1.Label.Size = new System.Drawing.Size(288, 29);
            this.radialGradientBG1.Label.TabIndex = 1;
            this.radialGradientBG1.Label.Text = "Calibrate your Compass";
            this.radialGradientBG1.Location = new System.Drawing.Point(0, 0);
            this.radialGradientBG1.Name = "radialGradientBG1";
            this.radialGradientBG1.OutsideColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(10)))));
            this.radialGradientBG1.Size = new System.Drawing.Size(800, 41);
            this.radialGradientBG1.TabIndex = 4;
            // 
            // panel2
            // 
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
            this.panel2.Location = new System.Drawing.Point(30, 249);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(740, 219);
            this.panel2.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(210, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Press Start";
            // 
            // BUT_compassorient
            // 
            this.BUT_compassorient.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_compassorient.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_compassorient.Location = new System.Drawing.Point(115, 138);
            this.BUT_compassorient.Name = "BUT_compassorient";
            this.BUT_compassorient.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_compassorient.Size = new System.Drawing.Size(75, 23);
            this.BUT_compassorient.TabIndex = 7;
            this.BUT_compassorient.Text = "Start";
            this.BUT_compassorient.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_compassorient.UseVisualStyleBackColor = true;
            this.BUT_compassorient.Click += new System.EventHandler(this.BUT_compassorient_Click);
            // 
            // pictureBoxMouseOver3
            // 
            this.pictureBoxMouseOver3.Image = global::MissionPlanner.Properties.Resources.apmp2;
            this.pictureBoxMouseOver3.ImageNormal = global::MissionPlanner.Properties.Resources.apmp2;
            this.pictureBoxMouseOver3.ImageOver = global::MissionPlanner.Properties.Resources.apmp2;
            this.pictureBoxMouseOver3.Location = new System.Drawing.Point(588, 138);
            this.pictureBoxMouseOver3.Name = "pictureBoxMouseOver3";
            this.pictureBoxMouseOver3.selected = false;
            this.pictureBoxMouseOver3.Size = new System.Drawing.Size(133, 67);
            this.pictureBoxMouseOver3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMouseOver3.TabIndex = 6;
            this.pictureBoxMouseOver3.TabStop = false;
            this.pictureBoxMouseOver3.Tag = "apm2";
            this.pictureBoxMouseOver3.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // pictureBoxMouseOver2
            // 
            this.pictureBoxMouseOver2.Image = global::MissionPlanner.Properties.Resources.px4;
            this.pictureBoxMouseOver2.ImageNormal = global::MissionPlanner.Properties.Resources.px4;
            this.pictureBoxMouseOver2.ImageOver = global::MissionPlanner.Properties.Resources.px4;
            this.pictureBoxMouseOver2.Location = new System.Drawing.Point(588, 4);
            this.pictureBoxMouseOver2.Name = "pictureBoxMouseOver2";
            this.pictureBoxMouseOver2.selected = false;
            this.pictureBoxMouseOver2.Size = new System.Drawing.Size(133, 61);
            this.pictureBoxMouseOver2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMouseOver2.TabIndex = 5;
            this.pictureBoxMouseOver2.TabStop = false;
            this.pictureBoxMouseOver2.Tag = "px4";
            this.pictureBoxMouseOver2.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // pictureBoxMouseOver1
            // 
            this.pictureBoxMouseOver1.Image = global::MissionPlanner.Properties.Resources.maggps;
            this.pictureBoxMouseOver1.ImageNormal = global::MissionPlanner.Properties.Resources.maggps;
            this.pictureBoxMouseOver1.ImageOver = global::MissionPlanner.Properties.Resources.maggps;
            this.pictureBoxMouseOver1.Location = new System.Drawing.Point(588, 71);
            this.pictureBoxMouseOver1.Name = "pictureBoxMouseOver1";
            this.pictureBoxMouseOver1.selected = false;
            this.pictureBoxMouseOver1.Size = new System.Drawing.Size(133, 61);
            this.pictureBoxMouseOver1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMouseOver1.TabIndex = 4;
            this.pictureBoxMouseOver1.TabStop = false;
            this.pictureBoxMouseOver1.Tag = "external";
            this.pictureBoxMouseOver1.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(539, 67);
            this.label3.TabIndex = 3;
            this.label3.Text = "Depending on your hardware, the compass orientation needs to be changed. Please f" +
    "ollow the instructions bellow. Make sure to keep the autopilot flat during the o" +
    "rientation.\r\n\r\n\r\n";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(4, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(241, 22);
            this.label4.TabIndex = 1;
            this.label4.Text = "COMPASS ORIENTATION";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(210, 170);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = ".";
            // 
            // _6CompassCalib
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.planebackground;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.radialGradientBG1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "_6CompassCalib";
            this.Size = new System.Drawing.Size(800, 500);
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
