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
    public class AutoReconnectDetailsForm: Form
    {
        private Label labelDetails;
        private Button buttonOk;

        public static void Show(string title, string details)
        {
            AutoReconnectDetailsForm form = new AutoReconnectDetailsForm(title, details);
            form.ShowDialog();
        }

        public AutoReconnectDetailsForm(string title, string details)
        {
            InitializeComponent();
            this.Text = title;
            this.labelDetails.Text = details;
            int x = Width / 2;
            int y = Height / 2;
            this.labelDetails.Location = new Point(x - (this.labelDetails.Location.X / 2), y - (this.labelDetails.Location.Y / 2));
        }

        private void InitializeComponent()
        {
            this.buttonOk = new System.Windows.Forms.Button();
            this.labelDetails = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(197, 226);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // labelDetails
            // 
            this.labelDetails.AutoSize = true;
            this.labelDetails.BackColor = System.Drawing.Color.Transparent;
            this.labelDetails.ForeColor = System.Drawing.SystemColors.Control;
            this.labelDetails.Location = new System.Drawing.Point(125, 107);
            this.labelDetails.Name = "labelDetails";
            this.labelDetails.Size = new System.Drawing.Size(35, 13);
            this.labelDetails.TabIndex = 1;
            this.labelDetails.Text = "label1";
            // 
            // AutoReconnectDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.ControlBox = false;
            this.Controls.Add(this.labelDetails);
            this.Controls.Add(this.buttonOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoReconnectDetailsForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoReconnectDetailsForm";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
