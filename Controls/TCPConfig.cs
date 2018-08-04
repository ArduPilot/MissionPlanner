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
    public partial class TCPConfig : UserControl, IConnectionConfig
    {
        private Label label1;
        public Label Title;
        private TextBox textBoxHost;
        private TextBox textBoxPort;
        private CheckBox checkBoxAutoReconnect;
        private Label label3;
        private NumericUpDown upDownConnectionTimeout;
        private Label label2;

        public TCPConfig()
        {
            InitializeComponent();
        }

        public ConnectionType ConnectionType { get { return ConnectionType.TCP; } }
        public CheckState AutoReconnect
        {
            get { return checkBoxAutoReconnect.CheckState; }
            set { checkBoxAutoReconnect.CheckState = value; }
        }

        public decimal AutoReconnectTimeout
        {
            get { return upDownConnectionTimeout.Value; }
            set { upDownConnectionTimeout.Value = value; }
        }

        public string Host
        {
            get { return textBoxHost.Text; }
            set { textBoxHost.Text = value; }
        }

        public string Port
        {
            get { return textBoxPort.Text; }
            set { textBoxPort.Text = value; }
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Title = new System.Windows.Forms.Label();
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.checkBoxAutoReconnect = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.upDownConnectionTimeout = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.upDownConnectionTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(10, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter host name/ip";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(10, 100);
            this.label2.Margin = new System.Windows.Forms.Padding(10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Enter remote port";
            // 
            // Title
            // 
            this.Title.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Title.AutoSize = true;
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.ForeColor = System.Drawing.SystemColors.Control;
            this.Title.Location = new System.Drawing.Point(10, 10);
            this.Title.Margin = new System.Windows.Forms.Padding(10);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(28, 13);
            this.Title.TabIndex = 2;
            this.Title.Tag = "TCP";
            this.Title.Text = "TCP";
            // 
            // textBoxHost
            // 
            this.textBoxHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHost.Location = new System.Drawing.Point(13, 60);
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.Size = new System.Drawing.Size(277, 20);
            this.textBoxHost.TabIndex = 3;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPort.Location = new System.Drawing.Point(13, 120);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(277, 20);
            this.textBoxPort.TabIndex = 4;
            // 
            // checkBoxAutoReconnect
            // 
            this.checkBoxAutoReconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxAutoReconnect.AutoSize = true;
            this.checkBoxAutoReconnect.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxAutoReconnect.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.checkBoxAutoReconnect.Checked = true;
            this.checkBoxAutoReconnect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoReconnect.ForeColor = System.Drawing.SystemColors.Control;
            this.checkBoxAutoReconnect.Location = new System.Drawing.Point(13, 154);
            this.checkBoxAutoReconnect.Name = "checkBoxAutoReconnect";
            this.checkBoxAutoReconnect.Size = new System.Drawing.Size(99, 17);
            this.checkBoxAutoReconnect.TabIndex = 9;
            this.checkBoxAutoReconnect.Text = "Auto reconnect";
            this.checkBoxAutoReconnect.UseVisualStyleBackColor = false;
            this.checkBoxAutoReconnect.CheckStateChanged += new System.EventHandler(this.checkBoxAutoReconnect_CheckStateChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(10, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Timeout:";
            // 
            // upDownConnectionTimeout
            // 
            this.upDownConnectionTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.upDownConnectionTimeout.Location = new System.Drawing.Point(64, 177);
            this.upDownConnectionTimeout.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.upDownConnectionTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownConnectionTimeout.Name = "upDownConnectionTimeout";
            this.upDownConnectionTimeout.ReadOnly = true;
            this.upDownConnectionTimeout.Size = new System.Drawing.Size(60, 20);
            this.upDownConnectionTimeout.TabIndex = 12;
            this.upDownConnectionTimeout.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // TCPConfig
            // 
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.upDownConnectionTimeout);
            this.Controls.Add(this.checkBoxAutoReconnect);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.textBoxHost);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(10);
            this.Name = "TCPConfig";
            this.Size = new System.Drawing.Size(300, 200);
            ((System.ComponentModel.ISupportInitialize)(this.upDownConnectionTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void checkBoxAutoReconnect_CheckStateChanged(object sender, EventArgs e)
        {
            if (AutoReconnect == CheckState.Checked)
            {
                upDownConnectionTimeout.Enabled = true;
            }
            else
            {
                upDownConnectionTimeout.Enabled = false;
            }
        }
    }
}