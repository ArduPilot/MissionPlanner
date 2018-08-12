using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigGPSInject
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
            this.gps = new MissionPlanner.SerialInjectGPS();
            this.SuspendLayout();
            // 
            // gps
            // 
            this.gps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gps.Location = new System.Drawing.Point(0, 0);
            this.gps.Name = "gps";
            this.gps.Size = new System.Drawing.Size(505, 158);
            this.gps.TabIndex = 0;
            // 
            // ConfigGPSInject
            // 

            this.Controls.Add(this.gps);
            this.Name = "ConfigGPSInject";
            this.Size = new System.Drawing.Size(505, 158);
            this.ResumeLayout(false);

        }

        private SerialInjectGPS gps;

        #endregion
    }
}
