using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MissionPlanner.Controls
{
    public class ProgressReporterSphere : ProgressReporterDialogue
    {
        private System.Windows.Forms.CheckBox CHK_rotate;
        public Sphere sphere2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chk_auto;
        public Sphere sphere1;
        public Sphere sphere3;

        public bool autoaccept = true;

        public ProgressReporterSphere()
        {
            InitializeComponent();

            chk_auto.Checked = autoaccept;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressReporterSphere));
            this.sphere1 = new MissionPlanner.Controls.Sphere();
            this.CHK_rotate = new System.Windows.Forms.CheckBox();
            this.sphere2 = new MissionPlanner.Controls.Sphere();
            this.label1 = new System.Windows.Forms.Label();
            this.chk_auto = new System.Windows.Forms.CheckBox();
            this.sphere3 = new MissionPlanner.Controls.Sphere();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            // 
            // sphere1
            // 
            resources.ApplyResources(this.sphere1, "sphere1");
            this.sphere1.BackColor = System.Drawing.Color.Black;
            this.sphere1.Name = "sphere1";
            this.sphere1.rotatewithdata = true;
            this.sphere1.VSync = false;
            // 
            // CHK_rotate
            // 
            resources.ApplyResources(this.CHK_rotate, "CHK_rotate");
            this.CHK_rotate.Checked = true;
            this.CHK_rotate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_rotate.Name = "CHK_rotate";
            this.CHK_rotate.UseVisualStyleBackColor = true;
            this.CHK_rotate.CheckedChanged += new System.EventHandler(this.CHK_rotate_CheckedChanged);
            // 
            // sphere2
            // 
            resources.ApplyResources(this.sphere2, "sphere2");
            this.sphere2.BackColor = System.Drawing.Color.Black;
            this.sphere2.Name = "sphere2";
            this.sphere2.rotatewithdata = true;
            this.sphere2.VSync = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // chk_auto
            // 
            resources.ApplyResources(this.chk_auto, "chk_auto");
            this.chk_auto.Checked = true;
            this.chk_auto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_auto.Name = "chk_auto";
            this.chk_auto.UseVisualStyleBackColor = true;
            this.chk_auto.CheckedChanged += new System.EventHandler(this.chk_auto_CheckedChanged);
            // 
            // sphere3
            // 
            resources.ApplyResources(this.sphere3, "sphere3");
            this.sphere3.BackColor = System.Drawing.Color.Black;
            this.sphere3.Name = "sphere3";
            this.sphere3.rotatewithdata = true;
            this.sphere3.VSync = false;
            // 
            // ProgressReporterSphere
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.sphere3);
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
            this.Controls.SetChildIndex(this.sphere3, 0);
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
            autoaccept = chk_auto.Checked;
        }
    }
}
