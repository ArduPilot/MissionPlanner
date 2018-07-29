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
    public partial class AUTOConfig : UserControl, IConnectionConfig
    {
        public Label Title;
        private NumericUpDown upDownConnectionTimeout;
        private Label label1;
        private CheckBox checkBoxAutoReconnect;

        public AUTOConfig()
        {
            InitializeComponent();
        }

        public ConnectionTypes ConnectionType { get { return ConnectionTypes.AUTO; } }
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

        private void InitializeComponent()
        {
            this.Title = new System.Windows.Forms.Label();
            this.checkBoxAutoReconnect = new System.Windows.Forms.CheckBox();
            this.upDownConnectionTimeout = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.upDownConnectionTimeout)).BeginInit();
            this.SuspendLayout();
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
            this.Title.Size = new System.Drawing.Size(37, 13);
            this.Title.TabIndex = 2;
            this.Title.Tag = "AUTO";
            this.Title.Text = "AUTO";
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
            this.upDownConnectionTimeout.TabIndex = 10;
            this.upDownConnectionTimeout.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(10, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Timeout:";
            // 
            // AUTOConfig
            // 
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.upDownConnectionTimeout);
            this.Controls.Add(this.checkBoxAutoReconnect);
            this.Controls.Add(this.Title);
            this.Margin = new System.Windows.Forms.Padding(10);
            this.Name = "AUTOConfig";
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