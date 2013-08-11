using System.Windows.Forms;
namespace ArdupilotMega.Wizard
{
    partial class _13GeoFence
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
            this.configAC_Fence1 = new ArdupilotMega.GCSViews.ConfigurationView.ConfigAC_Fence();
            this.label3 = new System.Windows.Forms.Label();
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
            this.panel1.Controls.Add(this.configAC_Fence1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(30, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(740, 424);
            this.panel1.TabIndex = 1;
            // 
            // configAC_Fence1
            // 
            this.configAC_Fence1.Location = new System.Drawing.Point(136, 114);
            this.configAC_Fence1.Name = "configAC_Fence1";
            this.configAC_Fence1.Size = new System.Drawing.Size(506, 239);
            this.configAC_Fence1.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(476, 36);
            this.label3.TabIndex = 6;
            this.label3.Text = "A GeoFence is a virtual fence that the autopilot will not fly outside of. \r\nThe s" +
    "ettings are based off the home position\r\n";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "GeoFence Settings";
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
            this.radialGradientBG1.Label.Size = new System.Drawing.Size(129, 29);
            this.radialGradientBG1.Label.TabIndex = 1;
            this.radialGradientBG1.Label.Text = "GeoFence";
            this.radialGradientBG1.Location = new System.Drawing.Point(0, 0);
            this.radialGradientBG1.Name = "radialGradientBG1";
            this.radialGradientBG1.OutsideColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(10)))));
            this.radialGradientBG1.Size = new System.Drawing.Size(800, 41);
            this.radialGradientBG1.TabIndex = 4;
            // 
            // _13GeoFence
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.planebackground;
            this.Controls.Add(this.radialGradientBG1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "_13GeoFence";
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
        private Label label3;
        private GCSViews.ConfigurationView.ConfigAC_Fence configAC_Fence1;

    }
}
