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
        private Panel panel1;
        private Panel panel2;
        private Button buttonOk;

        public static void Show(string title, string details)
        {
            AutoReconnectDetailsForm form = new AutoReconnectDetailsForm(title, details);
            form.ShowDialog();
        }

        public AutoReconnectDetailsForm(string title, string details)
        {
            InitializeComponent();
            for(int i = 0; i < details.Length; i++)
            {
                if(i%100 == 0)
                {
                    string left = details.Substring(0, i);
                    string right = details.Substring(i);
                    details = left + " - " + Environment.NewLine + right;
                }
            }
            this.Text = title;
            this.labelDetails.Text = details;
        }

        private void InitializeComponent()
        {
            this.buttonOk = new System.Windows.Forms.Button();
            this.labelDetails = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(206, 3);
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
            this.labelDetails.Location = new System.Drawing.Point(12, 9);
            this.labelDetails.Name = "labelDetails";
            this.labelDetails.Size = new System.Drawing.Size(35, 13);
            this.labelDetails.TabIndex = 1;
            this.labelDetails.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.labelDetails);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(284, 49);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.buttonOk);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 49);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(284, 29);
            this.panel2.TabIndex = 3;
            // 
            // AutoReconnectDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.ClientSize = new System.Drawing.Size(284, 78);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoReconnectDetailsForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoReconnectDetailsForm";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
