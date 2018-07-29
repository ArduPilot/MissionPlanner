using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner
{
    public class ConnectionControlForm: Form
    {
        private Button buttonCancel;
        private Button buttonOk;
        private Panel panelConfig;
        private Controls.TCPConfig tcpConfig;
        private Controls.UDPConfig udpConfig;
        private Controls.UDPClConfig udpclConfig;
        private Controls.COMConfig comConfig;
        private Controls.AUTOConfig autoConfig;
        public Controls.ConnectionControl ConnectionControl;

        public ConnectionControlForm Copy()
        {
            ConnectionControlForm form = new ConnectionControlForm();

            foreach (object item in this.ConnectionControl.cmb_sysid.Items)
            {
                form.ConnectionControl.cmb_sysid.Items.Add(item);
            }
            foreach (object item in this.ConnectionControl.CMB_serialport.Items)
            {
                form.ConnectionControl.CMB_serialport.Items.Add(item);
            }
            foreach (object item in this.ConnectionControl.TOOL_APMFirmware.Items)
            {
                form.ConnectionControl.TOOL_APMFirmware.Items.Add(item);
            }

            form.ConnectionControl.cmb_sysid.SelectedIndex = this.ConnectionControl.cmb_sysid.SelectedIndex;
            form.ConnectionControl.CMB_baudrate.SelectedIndex = this.ConnectionControl.CMB_baudrate.SelectedIndex;
            form.ConnectionControl.CMB_serialport.SelectedIndex = this.ConnectionControl.CMB_serialport.SelectedIndex;
            form.ConnectionControl.TOOL_APMFirmware.SelectedIndex = this.ConnectionControl.TOOL_APMFirmware.SelectedIndex;

            form.tcpConfig.IPAddress = this.tcpConfig.IPAddress;
            form.tcpConfig.Port = this.tcpConfig.Port;
            form.tcpConfig.AutoReconnect = this.tcpConfig.AutoReconnect;
            form.tcpConfig.AutoReconnectTimeout = this.tcpConfig.AutoReconnectTimeout;

            form.udpclConfig.IPAddress = this.udpclConfig.IPAddress;
            form.udpclConfig.Port = this.udpclConfig.Port;
            form.udpclConfig.AutoReconnect = this.udpclConfig.AutoReconnect;
            form.udpclConfig.AutoReconnectTimeout = this.udpclConfig.AutoReconnectTimeout;

            form.udpConfig.Port = this.udpConfig.Port;
            form.udpConfig.AutoReconnect = this.udpConfig.AutoReconnect;
            form.udpConfig.AutoReconnectTimeout = this.udpConfig.AutoReconnectTimeout;

            form.comConfig.AutoReconnect = this.comConfig.AutoReconnect;
            form.comConfig.AutoReconnectTimeout = this.comConfig.AutoReconnectTimeout;

            return form;
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

            tcpConfig.Tag = ConnectionTypes.TCP;
            udpConfig.Tag = ConnectionTypes.UDP;
            udpclConfig.Tag = ConnectionTypes.UDPCl;
            autoConfig.Tag = ConnectionTypes.AUTO;
            comConfig.Tag = ConnectionTypes.COM;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionControlForm));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.ConnectionControl = new MissionPlanner.Controls.ConnectionControl();
            this.panelConfig = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(12, 303);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(237, 303);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "Save";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonConnect_Click);
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
            this.ConnectionControl.Paint += new System.Windows.Forms.PaintEventHandler(this.ConnectionControl_Paint);
            // 
            // panelConfig
            // 
            this.panelConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelConfig.BackColor = System.Drawing.Color.Transparent;
            this.panelConfig.Location = new System.Drawing.Point(12, 97);
            this.panelConfig.Margin = new System.Windows.Forms.Padding(0);
            this.panelConfig.Name = "panelConfig";
            this.panelConfig.Size = new System.Drawing.Size(300, 200);
            this.panelConfig.TabIndex = 3;
            // 
            // ConnectionControlForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(324, 341);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
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

        private ConnectionTypes currentPort;
        private void ConnectionControl_Paint(object sender, PaintEventArgs e)
        {
            if (!currentPort.Equals(ConnectionControl.CMB_serialport.Text))
            {
                currentPort = GetConnectionType(ConnectionControl.CMB_serialport.Text);

                foreach (Control control in panelConfig.Controls)
                {
                    if (control.Tag.Equals(ConnectionTypes.COM))
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

        public IConnectionConfig ConnectionType
        {
            get
            {
                switch (currentPort)
                {
                    case ConnectionTypes.COM:   return comConfig;
                    case ConnectionTypes.TCP:   return tcpConfig;
                    case ConnectionTypes.UDP:   return udpConfig;
                    case ConnectionTypes.UDPCl: return udpclConfig;
                    default:                    return autoConfig;
                }
            }
        }

        private ConnectionTypes GetConnectionType(string connectionType)
        {
            if (connectionType.Equals("TCP"))           return ConnectionTypes.TCP;
            else if (connectionType.Equals("UDP"))      return ConnectionTypes.UDP;
            else if (connectionType.Equals("UDPCl"))    return ConnectionTypes.UDPCl;
            else if (connectionType.Equals("AUTO"))     return ConnectionTypes.AUTO;
            else                                        return ConnectionTypes.COM;
        }
    }
}
