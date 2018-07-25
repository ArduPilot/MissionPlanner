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
        private PictureBox pictureBoxIconError;

        public AutoReconnectForm(string title, string message, uint timeout)
        {
            InitializeComponent();
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.labelMessage.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.pictureBoxIconError.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.buttonReconnect.Select();
            Title = title;
            Message = message;
            Timeout = Convert.ToInt16(timeout);
        }

        public AutoReconnectForm(string title, string message)
        {
            InitializeComponent();
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.labelMessage.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.pictureBoxIconError.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.buttonReconnect.Select();
            Title = title;
            Message = message;
            Timeout = -1;
        }

        public int Timeout { get; set; }

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
                labelMessage.Location = new Point(5 + this.Width/2 - labelMessage.Size.Width/2, labelMessage.Location.Y);
                pictureBoxIconError.Location = new Point(labelMessage.Location.X - 35, pictureBoxIconError.Location.Y);
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
            if (Timeout == -1) return;
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

        private void buttonReconnect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void InitializeComponent()
        {
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonReconnect = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.pictureBoxIconError = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIconError)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(12, 106);
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
            this.buttonReconnect.Location = new System.Drawing.Point(197, 106);
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
            this.labelMessage.Location = new System.Drawing.Point(145, 46);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(35, 13);
            this.labelMessage.TabIndex = 2;
            this.labelMessage.Text = "label1";
            // 
            // pictureBoxIconError
            // 
            this.pictureBoxIconError.Image = global::MissionPlanner.Properties.Resources.iconWarning32;
            this.pictureBoxIconError.Location = new System.Drawing.Point(109, 37);
            this.pictureBoxIconError.Name = "pictureBoxIconError";
            this.pictureBoxIconError.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxIconError.TabIndex = 4;
            this.pictureBoxIconError.TabStop = false;
            // 
            // AutoReconnectForm
            // 
            this.AcceptButton = this.buttonReconnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(284, 141);
            this.Controls.Add(this.pictureBoxIconError);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.buttonReconnect);
            this.Controls.Add(this.buttonCancel);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
