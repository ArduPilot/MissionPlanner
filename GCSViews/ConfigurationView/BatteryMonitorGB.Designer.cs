using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class BatteryMonitorGB
    {
        private System.ComponentModel.IContainer components = null;
        private GroupBox groupBox1;
        private Panel contentHost;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.contentHost = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.contentHost);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 300);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Battery";
            // 
            // contentHost
            // 
            this.contentHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentHost.Location = new System.Drawing.Point(3, 16);
            this.contentHost.Name = "contentHost";
            this.contentHost.Padding = new System.Windows.Forms.Padding(0);
            this.contentHost.Size = new System.Drawing.Size(394, 281);
            this.contentHost.TabIndex = 0;
            // 
            // BatteryMonitorGB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "BatteryMonitorGB";
            this.Size = new System.Drawing.Size(400, 300);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
