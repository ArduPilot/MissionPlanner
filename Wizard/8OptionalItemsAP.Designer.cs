using System.Windows.Forms;
namespace MissionPlanner.Wizard
{
    partial class _8OptionalItemsAP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_8OptionalItemsAP));
            this.panel1 = new System.Windows.Forms.Panel();
            this.CHK_airspeeduse = new MissionPlanner.Controls.MavlinkCheckBox();
            this.LBL_airspeed = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CHK_enableairspeed = new MissionPlanner.Controls.MavlinkCheckBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radialGradientBG1 = new MissionPlanner.Controls.GradientBG();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.CHK_airspeeduse);
            this.panel1.Controls.Add(this.LBL_airspeed);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.CHK_enableairspeed);
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // CHK_airspeeduse
            // 
            resources.ApplyResources(this.CHK_airspeeduse, "CHK_airspeeduse");
            this.CHK_airspeeduse.Name = "CHK_airspeeduse";
            this.CHK_airspeeduse.OffValue = 0F;
            this.CHK_airspeeduse.OnValue = 1F;
            this.CHK_airspeeduse.param = null;
            this.CHK_airspeeduse.ParamName = null;
            this.CHK_airspeeduse.UseVisualStyleBackColor = true;
            // 
            // LBL_airspeed
            // 
            resources.ApplyResources(this.LBL_airspeed, "LBL_airspeed");
            this.LBL_airspeed.Name = "LBL_airspeed";
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
            // CHK_enableairspeed
            // 
            resources.ApplyResources(this.CHK_enableairspeed, "CHK_enableairspeed");
            this.CHK_enableairspeed.Name = "CHK_enableairspeed";
            this.CHK_enableairspeed.OffValue = 0F;
            this.CHK_enableairspeed.OnValue = 1F;
            this.CHK_enableairspeed.param = null;
            this.CHK_enableairspeed.ParamName = null;
            this.CHK_enableairspeed.UseVisualStyleBackColor = true;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.White;
            this.pictureBox3.BackgroundImage = global::MissionPlanner.Properties.Resources.airspeed;
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
            // radialGradientBG1
            // 
            this.radialGradientBG1.BackColor = System.Drawing.Color.Black;
            this.radialGradientBG1.CenterColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(164)))), ((int)(((byte)(33)))));
            // 
            // 
            // 
            this.radialGradientBG1.Image.BackColor = System.Drawing.Color.Transparent;
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
            this.radialGradientBG1.Label.Location = ((System.Drawing.Point)(resources.GetObject("radialGradientBG1.Label.Location")));
            this.radialGradientBG1.Label.Name = "_Label";
            this.radialGradientBG1.Label.Size = ((System.Drawing.Size)(resources.GetObject("radialGradientBG1.Label.Size")));
            this.radialGradientBG1.Label.TabIndex = ((int)(resources.GetObject("radialGradientBG1.Label.TabIndex")));
            this.radialGradientBG1.Label.Text = resources.GetString("radialGradientBG1.Label.Text");
            resources.ApplyResources(this.radialGradientBG1, "radialGradientBG1");
            this.radialGradientBG1.Name = "radialGradientBG1";
            this.radialGradientBG1.OutsideColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(10)))));
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // _8OptionalItemsAP
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.planebackground;
            this.Controls.Add(this.radialGradientBG1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "_8OptionalItemsAP";
            resources.ApplyResources(this, "$this");
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radialGradientBG1.Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Controls.GradientBG radialGradientBG1;
        private PictureBox pictureBox3;
        private Controls.MavlinkCheckBox CHK_enableairspeed;
        private Label label3;
        private Timer timer1;
        private Label label4;
        private Label LBL_airspeed;
        private Controls.MavlinkCheckBox CHK_airspeeduse;

    }
}
