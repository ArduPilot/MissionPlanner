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
        private ToolStripPanel BottomToolStripPanel;
        private ToolStripPanel TopToolStripPanel;
        private ToolStripPanel RightToolStripPanel;
        private ToolStripPanel LeftToolStripPanel;
        private Label label4;
        private Label label3;
        private Label labelConnectionType;
        private Label labelDevice;
        private ToolStripContentPanel ContentPanel;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionControlPreview));
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelConnectionType = new System.Windows.Forms.Label();
            this.labelDevice = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BottomToolStripPanel.BackgroundImage")));
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("TopToolStripPanel.BackgroundImage")));
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("RightToolStripPanel.BackgroundImage")));
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("LeftToolStripPanel.BackgroundImage")));
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ContentPanel.BackgroundImage")));
            this.ContentPanel.Size = new System.Drawing.Size(150, 175);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(8, 8);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(100, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "Device";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(8, 34);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(100, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "Connection Type";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelConnectionType
            // 
            this.labelConnectionType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelConnectionType.AutoSize = true;
            this.labelConnectionType.BackColor = System.Drawing.Color.Transparent;
            this.labelConnectionType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelConnectionType.ForeColor = System.Drawing.Color.White;
            this.labelConnectionType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelConnectionType.Location = new System.Drawing.Point(114, 35);
            this.labelConnectionType.Margin = new System.Windows.Forms.Padding(3);
            this.labelConnectionType.MaximumSize = new System.Drawing.Size(100, 13);
            this.labelConnectionType.MinimumSize = new System.Drawing.Size(27, 13);
            this.labelConnectionType.Name = "labelConnectionType";
            this.labelConnectionType.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelConnectionType.Size = new System.Drawing.Size(30, 13);
            this.labelConnectionType.TabIndex = 14;
            this.labelConnectionType.Text = "N/A";
            this.labelConnectionType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDevice
            // 
            this.labelDevice.AutoSize = true;
            this.labelDevice.BackColor = System.Drawing.Color.Transparent;
            this.labelDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDevice.ForeColor = System.Drawing.Color.White;
            this.labelDevice.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelDevice.Location = new System.Drawing.Point(114, 9);
            this.labelDevice.Margin = new System.Windows.Forms.Padding(3);
            this.labelDevice.MaximumSize = new System.Drawing.Size(100, 13);
            this.labelDevice.MinimumSize = new System.Drawing.Size(27, 13);
            this.labelDevice.Name = "labelDevice";
            this.labelDevice.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelDevice.Size = new System.Drawing.Size(30, 13);
            this.labelDevice.TabIndex = 13;
            this.labelDevice.Text = "N/A";
            this.labelDevice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ConnectionControlPreview
            // 
            this.AutoSize = true;
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelDevice);
            this.Controls.Add(this.labelConnectionType);
            this.Margin = new System.Windows.Forms.Padding(10);
            this.MaximumSize = new System.Drawing.Size(300, 57);
            this.MinimumSize = new System.Drawing.Size(169, 57);
            this.Name = "ConnectionControlPreview";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(169, 57);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}