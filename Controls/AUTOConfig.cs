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
        public ListBox ListBoxCommsTypes;
        private Button buttonUp;
        private Button buttonDown;
        private CheckBox checkBoxAutoReconnect;
        private Label label2;
        private ConnectionControl ConnectionControl;

        public AUTOConfig()
        {
            InitializeComponent();
        }

        public ConnectionType ConnectionType { get { return ConnectionType.AUTO; } }
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
        
        public int CurrentCommsIndex { get; private set; } = 0;
        public ConnectionType ConnectionQueue
        {
            get
            {
                try
                {
                    return ConnectionMethods.GetConnectionType((string)ListBoxCommsTypes.Items[CurrentCommsIndex++]);
                }
                catch
                {
                    CurrentCommsIndex = 0;
                    try
                    {
                        return ConnectionMethods.GetConnectionType((string)ListBoxCommsTypes.Items[CurrentCommsIndex++]);
                    }
                    catch
                    {
                        return ConnectionType.AUTO;
                    }
                }
            }
        }

        public ConnectionType NextConnectionQueue
        {
            get
            {
                try
                {
                    return ConnectionMethods.GetConnectionType((string)ListBoxCommsTypes.Items[CurrentCommsIndex]);
                }
                catch
                {
                    CurrentCommsIndex = 0;
                    try
                    {
                        return ConnectionMethods.GetConnectionType((string)ListBoxCommsTypes.Items[CurrentCommsIndex]);
                    }
                    catch
                    {
                        return ConnectionType.AUTO;
                    }
                }
            }
        }

        public void ConnectionQueueReset()
        {
            CurrentCommsIndex = 0;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AUTOConfig));
            this.ConnectionControl = new MissionPlanner.Controls.ConnectionControl();
            this.Title = new System.Windows.Forms.Label();
            this.checkBoxAutoReconnect = new System.Windows.Forms.CheckBox();
            this.upDownConnectionTimeout = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.ListBoxCommsTypes = new System.Windows.Forms.ListBox();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.upDownConnectionTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // ConnectionControl
            // 
            this.ConnectionControl.AutoSize = true;
            this.ConnectionControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ConnectionControl.BackgroundImage")));
            this.ConnectionControl.Location = new System.Drawing.Point(0, 0);
            this.ConnectionControl.MaximumSize = new System.Drawing.Size(300, 85);
            this.ConnectionControl.MinimumSize = new System.Drawing.Size(300, 85);
            this.ConnectionControl.Name = "ConnectionControl";
            this.ConnectionControl.Size = new System.Drawing.Size(300, 85);
            this.ConnectionControl.TabIndex = 0;
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
            // ListBoxCommsTypes
            // 
            this.ListBoxCommsTypes.FormattingEnabled = true;
            this.ListBoxCommsTypes.Location = new System.Drawing.Point(13, 49);
            this.ListBoxCommsTypes.Name = "ListBoxCommsTypes";
            this.ListBoxCommsTypes.Size = new System.Drawing.Size(215, 95);
            this.ListBoxCommsTypes.TabIndex = 12;
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(234, 49);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(50, 45);
            this.buttonUp.TabIndex = 13;
            this.buttonUp.Text = "Up";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(234, 100);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(50, 45);
            this.buttonDown.TabIndex = 14;
            this.buttonDown.Text = "Down";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(10, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 15;
            this.label2.Tag = "AUTO";
            this.label2.Text = "Priority";
            // 
            // AUTOConfig
            // 
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.ListBoxCommsTypes);
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

        private void buttonUp_Click(object sender, EventArgs e)
        {
            try
            {
                object item = ListBoxCommsTypes.Items[ListBoxCommsTypes.SelectedIndex - 1];
                ListBoxCommsTypes.Items[ListBoxCommsTypes.SelectedIndex - 1] = ListBoxCommsTypes.Items[ListBoxCommsTypes.SelectedIndex];
                ListBoxCommsTypes.Items[ListBoxCommsTypes.SelectedIndex] = item;
                ListBoxCommsTypes.SelectedIndex--;
            }
            catch { }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            try
            {
                object item = ListBoxCommsTypes.Items[ListBoxCommsTypes.SelectedIndex + 1];
                ListBoxCommsTypes.Items[ListBoxCommsTypes.SelectedIndex + 1] = ListBoxCommsTypes.Items[ListBoxCommsTypes.SelectedIndex];
                ListBoxCommsTypes.Items[ListBoxCommsTypes.SelectedIndex] = item;
                ListBoxCommsTypes.SelectedIndex++;
            }
            catch { }
        }
    }
}