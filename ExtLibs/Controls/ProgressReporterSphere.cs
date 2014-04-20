using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Controls
{
    public class ProgressReporterSphere : ProgressReporterDialogue
    {
        private System.Windows.Forms.CheckBox CHK_rotate;
        public Sphere sphere1;

        public ProgressReporterSphere()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.sphere1 = new MissionPlanner.Controls.Sphere();
            this.CHK_rotate = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(199, 411);
            // 
            // sphere1
            // 
            this.sphere1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sphere1.BackColor = System.Drawing.Color.Black;
            this.sphere1.Location = new System.Drawing.Point(13, 141);
            this.sphere1.Name = "sphere1";
            this.sphere1.rotatewithdata = true;
            this.sphere1.Size = new System.Drawing.Size(263, 263);
            this.sphere1.TabIndex = 6;
            this.sphere1.VSync = false;
            // 
            // CHK_rotate
            // 
            this.CHK_rotate.AutoSize = true;
            this.CHK_rotate.Checked = true;
            this.CHK_rotate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_rotate.Location = new System.Drawing.Point(13, 411);
            this.CHK_rotate.Name = "CHK_rotate";
            this.CHK_rotate.Size = new System.Drawing.Size(157, 17);
            this.CHK_rotate.TabIndex = 7;
            this.CHK_rotate.Text = "Rotate with each data point";
            this.CHK_rotate.UseVisualStyleBackColor = true;
            this.CHK_rotate.CheckedChanged += new System.EventHandler(this.CHK_rotate_CheckedChanged);
            // 
            // ProgressReporterSphere
            // 
            this.ClientSize = new System.Drawing.Size(292, 446);
            this.Controls.Add(this.CHK_rotate);
            this.Controls.Add(this.sphere1);
            this.Name = "ProgressReporterSphere";
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.sphere1, 0);
            this.Controls.SetChildIndex(this.CHK_rotate, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void CHK_rotate_CheckedChanged(object sender, EventArgs e)
        {
            sphere1.rotatewithdata = CHK_rotate.Checked;
        }
    }
}
