using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MissionPlanner
{
    public class AutoReconnectForm: Form
    {
        private Button buttonCancel;
        private Button buttonReconnect;
        private Label labelMessage;
        private LinkLabel linkLabelDetails;
        private Panel panel1;
        private Panel panel2;
        private PictureBox pictureBoxIconError;

        public static DialogResult Show(string title, string message, string details, int timeout)
        {
            AutoReconnectForm form = new AutoReconnectForm(title, message, details, timeout);
            return form.ShowDialog();
        }

        public static DialogResult Show(string title, string message, int timeout)
        {
            AutoReconnectForm form = new AutoReconnectForm(title, message, timeout);
            return form.ShowDialog();
        }

        public AutoReconnectForm(string title, string message, string details, int timeout)
        {
            InitializeComponent();
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.labelMessage.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.pictureBoxIconError.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.linkLabelDetails.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.panel1.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.panel2.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.buttonReconnect.Select();
            Title = title;
            Message = message;
            Details = details;
            Timeout = timeout;
        }

        public AutoReconnectForm(string title, string message, int timeout)
        {
            InitializeComponent();
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.labelMessage.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.pictureBoxIconError.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.linkLabelDetails.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.panel1.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.panel2.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.buttonReconnect.Select();
            Title = title;
            Message = message;
            Details = "";
            linkLabelDetails.Hide();
            Timeout = timeout;
        }

        public int Timeout { get; set; }
        public string Details { get; set; }

        public string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public string Message
        {
            get { return labelMessage.Text; }
            set
            {
                labelMessage.Text = value;
                pictureBoxIconError.Location = new Point(labelMessage.Location.X - 35, labelMessage.Location.Y + (labelMessage.Height/2 - 15));
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Thread timeoutClockThread = new Thread(new ThreadStart(TimeoutClock));
            timeoutClockThread.Start();
        }

        private void TimeoutClock()
        {
            if (Timeout < 0) return;
            while (Timeout > 0)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        buttonReconnect.Text = "Reconnect(" + Timeout.ToString() + ")";
                    });
                } catch { }
                Thread.Sleep(1000);
                Timeout--;
            }
            DialogResult = DialogResult.OK;
        }

        private void InitializeComponent()
        {
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonReconnect = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.pictureBoxIconError = new System.Windows.Forms.PictureBox();
            this.linkLabelDetails = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIconError)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(12, 21);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonReconnect
            // 
            this.buttonReconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReconnect.AutoSize = true;
            this.buttonReconnect.Location = new System.Drawing.Point(144, 21);
            this.buttonReconnect.Name = "buttonReconnect";
            this.buttonReconnect.Size = new System.Drawing.Size(75, 23);
            this.buttonReconnect.TabIndex = 1;
            this.buttonReconnect.Text = "Reconnect";
            this.buttonReconnect.UseVisualStyleBackColor = true;
            this.buttonReconnect.Click += new System.EventHandler(this.buttonReconnect_Click);
            // 
            // labelMessage
            // 
            this.labelMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMessage.AutoSize = true;
            this.labelMessage.BackColor = System.Drawing.Color.Transparent;
            this.labelMessage.ForeColor = System.Drawing.SystemColors.Control;
            this.labelMessage.Location = new System.Drawing.Point(48, 32);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(35, 13);
            this.labelMessage.TabIndex = 2;
            this.labelMessage.Text = "label1";
            // 
            // pictureBoxIconError
            // 
            this.pictureBoxIconError.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxIconError.Image = global::MissionPlanner.Properties.Resources.iconWarning32;
            this.pictureBoxIconError.Location = new System.Drawing.Point(12, 23);
            this.pictureBoxIconError.Name = "pictureBoxIconError";
            this.pictureBoxIconError.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxIconError.TabIndex = 4;
            this.pictureBoxIconError.TabStop = false;
            // 
            // linkLabelDetails
            // 
            this.linkLabelDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelDetails.AutoSize = true;
            this.linkLabelDetails.BackColor = System.Drawing.Color.Transparent;
            this.linkLabelDetails.Location = new System.Drawing.Point(180, 5);
            this.linkLabelDetails.Name = "linkLabelDetails";
            this.linkLabelDetails.Size = new System.Drawing.Size(39, 13);
            this.linkLabelDetails.TabIndex = 5;
            this.linkLabelDetails.TabStop = true;
            this.linkLabelDetails.Text = "Details";
            this.linkLabelDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelDetails_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.pictureBoxIconError);
            this.panel1.Controls.Add(this.labelMessage);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(231, 72);
            this.panel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Controls.Add(this.buttonReconnect);
            this.panel2.Controls.Add(this.linkLabelDetails);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 72);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(231, 56);
            this.panel2.TabIndex = 7;
            // 
            // AutoReconnectForm
            // 
            this.AcceptButton = this.buttonReconnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(231, 128);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoReconnectForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connection failed";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIconError)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void buttonReconnect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void linkLabelDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AutoReconnectDetailsForm.Show(Title, Details);
        }
    }
}
