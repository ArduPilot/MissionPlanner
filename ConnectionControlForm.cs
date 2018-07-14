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
    public class ConnectionControlForm: Form
    {
        private Button buttonCancel;
        private Button buttonConnect;
        private Panel panelConfig;
        private Controls.TCPConfig tcpConfig;
        private Controls.UDPConfig udpConfig;
        private Controls.UDPClConfig udpclConfig;
        private Controls.COMConfig comConfig;
        private Controls.AUTOConfig autoConfig;
        public Controls.ConnectionControl ConnectionControl;

        private enum ConnectionType
        {
            TCP,
            UDP,
            UDPCl,
            AUTO,
            COM
        }

        public ConnectionControlForm()
        {
            InitializeComponent();
            panelConfig.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;

            tcpConfig = new Controls.TCPConfig();
            udpConfig = new Controls.UDPConfig();
            udpclConfig = new Controls.UDPClConfig();
            autoConfig = new Controls.AUTOConfig();
            comConfig = new Controls.COMConfig();

            tcpConfig.Tag = ConnectionType.TCP;
            udpConfig.Tag = ConnectionType.UDP;
            udpclConfig.Tag = ConnectionType.UDPCl;
            autoConfig.Tag = ConnectionType.AUTO;
            comConfig.Tag = ConnectionType.COM;

            tcpConfig.Dock = DockStyle.Fill;
            udpConfig.Dock = DockStyle.Fill;
            udpclConfig.Dock = DockStyle.Fill;
            autoConfig.Dock = DockStyle.Fill;
            comConfig.Dock = DockStyle.Fill;

            panelConfig.Controls.Add(tcpConfig);
            panelConfig.Controls.Add(udpConfig);
            panelConfig.Controls.Add(udpclConfig);
            panelConfig.Controls.Add(autoConfig);
            panelConfig.Controls.Add(comConfig);

            tcpConfig.Hide();
            udpConfig.Hide();
            udpclConfig.Hide();
            autoConfig.Hide();
            comConfig.Hide();

            currentPort = GetConnectionType(ConnectionControl.CMB_serialport.Text);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(ConnectionControlForm));
            buttonCancel = new System.Windows.Forms.Button();
            buttonConnect = new System.Windows.Forms.Button();
            ConnectionControl = new Controls.ConnectionControl();
            panelConfig = new System.Windows.Forms.Panel();
            SuspendLayout();
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(12, 303);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonConnect
            // 
            buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            buttonConnect.Location = new System.Drawing.Point(237, 303);
            buttonConnect.Name = "buttonConnect";
            buttonConnect.Size = new System.Drawing.Size(75, 23);
            buttonConnect.TabIndex = 2;
            buttonConnect.Text = "Connect";
            buttonConnect.UseVisualStyleBackColor = true;
            buttonConnect.Click += new System.EventHandler(buttonConnect_Click);
            // 
            // ConnectionControl
            // 
            ConnectionControl.AutoSize = true;
            ConnectionControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ConnectionControl.BackgroundImage")));
            ConnectionControl.Dock = System.Windows.Forms.DockStyle.Top;
            ConnectionControl.Location = new System.Drawing.Point(12, 12);
            ConnectionControl.MaximumSize = new System.Drawing.Size(300, 85);
            ConnectionControl.MinimumSize = new System.Drawing.Size(300, 85);
            ConnectionControl.Name = "ConnectionControl";
            ConnectionControl.Size = new System.Drawing.Size(300, 85);
            ConnectionControl.TabIndex = 0;
            ConnectionControl.Paint += new System.Windows.Forms.PaintEventHandler(ConnectionControl_Paint);
            // 
            // panelConfig
            // 
            panelConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            panelConfig.BackColor = System.Drawing.Color.Transparent;
            panelConfig.Location = new System.Drawing.Point(12, 97);
            panelConfig.Margin = new System.Windows.Forms.Padding(0);
            panelConfig.Name = "panelConfig";
            panelConfig.Size = new System.Drawing.Size(300, 200);
            panelConfig.TabIndex = 3;
            // 
            // ConnectionControlForm
            // 
            AcceptButton = buttonConnect;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(324, 341);
            Controls.Add(buttonCancel);
            Controls.Add(buttonConnect);
            Controls.Add(panelConfig);
            Controls.Add(ConnectionControl);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConnectionControlForm";
            Padding = new System.Windows.Forms.Padding(12);
            ShowIcon = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Connection Control";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private ConnectionType currentPort;
        private void ConnectionControl_Paint(object sender, PaintEventArgs e)
        {
            if (!currentPort.Equals(ConnectionControl.CMB_serialport.Text))
            {
                currentPort = GetConnectionType(ConnectionControl.CMB_serialport.Text);

                foreach (Control control in panelConfig.Controls)
                {
                    if (control.Tag.Equals(ConnectionType.COM))
                    {
                        Controls.COMConfig comControl = (Controls.COMConfig)control;
                        comControl.Title.Text = ConnectionControl.CMB_serialport.Text;
                        control.Show();
                    }
                    else if (control.Tag.Equals(currentPort))       control.Show();
                    else                                            control.Hide();
                }
            }
        }

        private ConnectionType GetConnectionType(string connectionType)
        {
            if (connectionType.Equals("TCP"))           return ConnectionType.TCP;
            else if (connectionType.Equals("UDP"))      return ConnectionType.UDP;
            else if (connectionType.Equals("UDPCl"))    return ConnectionType.UDPCl;
            else if (connectionType.Equals("AUTO"))     return ConnectionType.AUTO;
            else                                        return ConnectionType.COM;
        }
    }
}
