using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Comms;
using System.Threading;

namespace MissionPlanner.Controls
{
    public class ConnectionControlPreview : UserControl
    {
        private Label label3;
        private Label labelConnectionType;
        private Label labelDevice;
        private Label label4;

        public ConnectionControlPreview()
        {
            InitializeComponent();
        }

        public string Device
        {
            get { return labelDevice.Text; }
            set { labelDevice.Text = value; }
        }

        public string ConnectionType
        {
            get { return labelConnectionType.Text; }
            set { labelConnectionType.Text = value; }
        }

        private void InitializeComponent()
        {
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelConnectionType = new System.Windows.Forms.Label();
            this.labelDevice = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(13, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Connection Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(13, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Device";
            // 
            // labelConnectionType
            // 
            this.labelConnectionType.AutoSize = true;
            this.labelConnectionType.BackColor = System.Drawing.Color.Transparent;
            this.labelConnectionType.ForeColor = System.Drawing.Color.White;
            this.labelConnectionType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelConnectionType.Location = new System.Drawing.Point(110, 37);
            this.labelConnectionType.Name = "labelConnectionType";
            this.labelConnectionType.Size = new System.Drawing.Size(88, 13);
            this.labelConnectionType.TabIndex = 10;
            this.labelConnectionType.Text = "Connection Type";
            // 
            // labelDevice
            // 
            this.labelDevice.AutoSize = true;
            this.labelDevice.BackColor = System.Drawing.Color.Transparent;
            this.labelDevice.ForeColor = System.Drawing.Color.White;
            this.labelDevice.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelDevice.Location = new System.Drawing.Point(110, 10);
            this.labelDevice.Name = "labelDevice";
            this.labelDevice.Size = new System.Drawing.Size(41, 13);
            this.labelDevice.TabIndex = 9;
            this.labelDevice.Text = "Device";
            // 
            // ConnectionControlPreview
            // 
            this.AutoSize = true;
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.Controls.Add(this.labelConnectionType);
            this.Controls.Add(this.labelDevice);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Margin = new System.Windows.Forms.Padding(10);
            this.Name = "ConnectionControlPreview";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(233, 60);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}