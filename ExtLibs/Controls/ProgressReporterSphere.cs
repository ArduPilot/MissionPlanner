using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Controls
{
    public class ProgressReporterSphere : ProgressReporterDialogue
    {
        private System.Windows.Forms.CheckBox CHK_rotate;
        public Sphere sphere2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chk_auto;
        public Sphere sphere1;

        public bool autoaccept = true;

        public ProgressReporterSphere()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressReporterSphere));
            this.sphere1 = new MissionPlanner.Controls.Sphere();
            this.CHK_rotate = new System.Windows.Forms.CheckBox();
            this.sphere2 = new MissionPlanner.Controls.Sphere();
            this.label1 = new System.Windows.Forms.Label();
            this.chk_auto = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(472, 411);
            // 
            // sphere1
            // 
            this.sphere1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sphere1.BackColor = System.Drawing.Color.Black;
            this.sphere1.Location = new System.Drawing.Point(16, 141);
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
            // sphere2
            // 
            this.sphere2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sphere2.BackColor = System.Drawing.Color.Black;
            this.sphere2.Location = new System.Drawing.Point(285, 141);
            this.sphere2.Name = "sphere2";
            this.sphere2.rotatewithdata = true;
            this.sphere2.Size = new System.Drawing.Size(263, 263);
            this.sphere2.TabIndex = 8;
            this.sphere2.VSync = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(282, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(254, 104);
            this.label1.TabIndex = 9;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // chk_auto
            // 
            this.chk_auto.AutoSize = true;
            this.chk_auto.Checked = true;
            this.chk_auto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_auto.Location = new System.Drawing.Point(176, 410);
            this.chk_auto.Name = "chk_auto";
            this.chk_auto.Size = new System.Drawing.Size(107, 17);
            this.chk_auto.TabIndex = 10;
            this.chk_auto.Text = "Use Auto Accept";
            this.chk_auto.UseVisualStyleBackColor = true;
            this.chk_auto.CheckedChanged += new System.EventHandler(this.chk_auto_CheckedChanged);
            // 
            // ProgressReporterSphere
            // 
            this.ClientSize = new System.Drawing.Size(565, 446);
            this.Controls.Add(this.chk_auto);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sphere2);
            this.Controls.Add(this.CHK_rotate);
            this.Controls.Add(this.sphere1);
            this.Name = "ProgressReporterSphere";
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.sphere1, 0);
            this.Controls.SetChildIndex(this.CHK_rotate, 0);
            this.Controls.SetChildIndex(this.sphere2, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.chk_auto, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void CHK_rotate_CheckedChanged(object sender, EventArgs e)
        {
            sphere1.rotatewithdata = CHK_rotate.Checked;
            sphere2.rotatewithdata = CHK_rotate.Checked;
        }

        private void chk_auto_CheckedChanged(object sender, EventArgs e)
        {
            autoaccept = !autoaccept;
        }
    }
}
