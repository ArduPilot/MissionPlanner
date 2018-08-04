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
    public partial class UDPConfig : UserControl, IConnectionConfig
    {
        public Label Title;
        private TextBox textBoxPort;
        private CheckBox checkBoxAutoReconnect;
        private Label label1;
        private NumericUpDown upDownConnectionTimeout;
        private Label label2;

        public UDPConfig()
        {
            InitializeComponent();
        }

        public ConnectionType ConnectionType { get { return ConnectionType.UDP; } }
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

        public string Port
        {
            get { return textBoxPort.Text; }
            set { textBoxPort.Text = value; }
        }

        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.Title = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.checkBoxAutoReconnect = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.upDownConnectionTimeout = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.upDownConnectionTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(10, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Enter local port";
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
            this.Title.Margin = new System.Windows.Forms.Padding(20);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(30, 13);
            this.Title.TabIndex = 2;
            this.Title.Tag = "UDP";
            this.Title.Text = "UDP";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPort.Location = new System.Drawing.Point(13, 60);
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
            this.checkBoxAutoReconnect.TabIndex = 8;
            this.checkBoxAutoReconnect.Text = "Auto reconnect";
            this.checkBoxAutoReconnect.UseVisualStyleBackColor = false;
            this.checkBoxAutoReconnect.CheckStateChanged += new System.EventHandler(this.checkBoxAutoReconnect_CheckStateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(10, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Timeout:";
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
            this.upDownConnectionTimeout.Size = new System.Drawing.Size(60, 20);
            this.upDownConnectionTimeout.TabIndex = 12;
            this.upDownConnectionTimeout.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // UDPConfig
            // 
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.upDownConnectionTimeout);
            this.Controls.Add(this.checkBoxAutoReconnect);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.label2);
            this.Margin = new System.Windows.Forms.Padding(10);
            this.Name = "UDPConfig";
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