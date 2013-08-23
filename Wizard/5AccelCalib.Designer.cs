using System.Windows.Forms;
namespace ArdupilotMega.Wizard
{
    partial class _5AccelCalib
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
            this.BUT_continue = new ArdupilotMega.Controls.MyButton();
            this.imageLabel1 = new ArdupilotMega.Controls.ImageLabel();
            this.BUT_start = new ArdupilotMega.Controls.MyButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radialGradientBG1 = new ArdupilotMega.Controls.GradientBG();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.BUT_continue);
            this.panel1.Controls.Add(this.imageLabel1);
            this.panel1.Controls.Add(this.BUT_start);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(30, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(740, 410);
            this.panel1.TabIndex = 1;
            // 
            // BUT_continue
            // 
            this.BUT_continue.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_continue.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_continue.Enabled = false;
            this.BUT_continue.Location = new System.Drawing.Point(95, 308);
            this.BUT_continue.Name = "BUT_continue";
            this.BUT_continue.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_continue.Size = new System.Drawing.Size(75, 23);
            this.BUT_continue.TabIndex = 6;
            this.BUT_continue.Text = "Continue";
            this.BUT_continue.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_continue.UseVisualStyleBackColor = true;
            this.BUT_continue.Click += new System.EventHandler(this.BUT_continue_Click);
            // 
            // imageLabel1
            // 
            this.imageLabel1.Image = global::MissionPlanner.Properties.Resources.calibration01;
            this.imageLabel1.Location = new System.Drawing.Point(262, 106);
            this.imageLabel1.Name = "imageLabel1";
            this.imageLabel1.Size = new System.Drawing.Size(415, 290);
            this.imageLabel1.TabIndex = 5;
            // 
            // BUT_start
            // 
            this.BUT_start.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_start.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_start.Location = new System.Drawing.Point(95, 144);
            this.BUT_start.Name = "BUT_start";
            this.BUT_start.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_start.Size = new System.Drawing.Size(75, 23);
            this.BUT_start.TabIndex = 4;
            this.BUT_start.Text = "Start";
            this.BUT_start.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_start.UseVisualStyleBackColor = true;
            this.BUT_start.Click += new System.EventHandler(this.BUT_start_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(445, 59);
            this.label2.TabIndex = 3;
            this.label2.Text = "Here we will calibrate the minimum and maximum values seen by your accelerometer." +
    " Please click start and follow the pictures indicated.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "ACCELEROMETER CALIBRATION";
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
            this.radialGradientBG1.Label.Size = new System.Drawing.Size(344, 29);
            this.radialGradientBG1.Label.TabIndex = 1;
            this.radialGradientBG1.Label.Text = "Calibrate your Accelerometer";
            this.radialGradientBG1.Location = new System.Drawing.Point(0, 0);
            this.radialGradientBG1.Name = "radialGradientBG1";
            this.radialGradientBG1.OutsideColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(10)))));
            this.radialGradientBG1.Size = new System.Drawing.Size(800, 41);
            this.radialGradientBG1.TabIndex = 4;
            // 
            // _5AccelCalib
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.planebackground;
            this.Controls.Add(this.radialGradientBG1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "_5AccelCalib";
            this.Size = new System.Drawing.Size(800, 500);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Controls.GradientBG radialGradientBG1;
        private Label label2;
        private Controls.ImageLabel imageLabel1;
        private Controls.MyButton BUT_start;
        private Controls.MyButton BUT_continue;

    }
}
