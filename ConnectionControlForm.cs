using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner
{
    public partial class ConnectionControlForm: Form
    {
        private Button buttonCancel;
        private Button buttonConnect;
        private Panel panelConfig;
        public Controls.ConnectionControl ConnectionControl;

        public ConnectionControlForm()
        {
            InitializeComponent();
            panelConfig.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionControlForm));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.ConnectionControl = new MissionPlanner.Controls.ConnectionControl();
            this.panelConfig = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(12, 283);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConnect.Location = new System.Drawing.Point(237, 283);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 2;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // ConnectionControl
            // 
            this.ConnectionControl.AutoSize = true;
            this.ConnectionControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ConnectionControl.BackgroundImage")));
            this.ConnectionControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.ConnectionControl.Location = new System.Drawing.Point(12, 12);
            this.ConnectionControl.MaximumSize = new System.Drawing.Size(300, 85);
            this.ConnectionControl.MinimumSize = new System.Drawing.Size(300, 85);
            this.ConnectionControl.Name = "ConnectionControl";
            this.ConnectionControl.Size = new System.Drawing.Size(300, 85);
            this.ConnectionControl.TabIndex = 0;
            // 
            // panelConfig
            // 
            this.panelConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelConfig.BackColor = System.Drawing.Color.Transparent;
            this.panelConfig.Location = new System.Drawing.Point(12, 97);
            this.panelConfig.Name = "panelConfig";
            this.panelConfig.Size = new System.Drawing.Size(300, 180);
            this.panelConfig.TabIndex = 3;
            // 
            // ConnectionControlForm
            // 
            this.AcceptButton = this.buttonConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(324, 321);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.panelConfig);
            this.Controls.Add(this.ConnectionControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionControlForm";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connection Control";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
