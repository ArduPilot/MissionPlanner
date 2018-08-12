using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigAccelerometerCalibration
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigAccelerometerCalibration));
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_Accel_user = new System.Windows.Forms.Label();
            this.BUT_calib_accell = new MissionPlanner.Controls.MyButton();
            this.lineSeparator2 = new MissionPlanner.Controls.LineSeparator();
            this.BUT_level = new MissionPlanner.Controls.MyButton();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lbl_Accel_user
            // 
            resources.ApplyResources(this.lbl_Accel_user, "lbl_Accel_user");
            this.lbl_Accel_user.Name = "lbl_Accel_user";
            // 
            // BUT_calib_accell
            // 
            resources.ApplyResources(this.BUT_calib_accell, "BUT_calib_accell");
            this.BUT_calib_accell.Name = "BUT_calib_accell";
            this.BUT_calib_accell.UseVisualStyleBackColor = true;
            this.BUT_calib_accell.Click += new System.EventHandler(this.BUT_calib_accell_Click);
            // 
            // lineSeparator2
            // 
            resources.ApplyResources(this.lineSeparator2, "lineSeparator2");
            this.lineSeparator2.Name = "lineSeparator2";
            this.lineSeparator2.Opacity1 = 0.6F;
            this.lineSeparator2.Opacity2 = 0.7F;
            this.lineSeparator2.Opacity3 = 0.1F;
            this.lineSeparator2.PrimaryColor = System.Drawing.Color.Black;
            this.lineSeparator2.SecondaryColor = System.Drawing.Color.Gainsboro;
            // 
            // BUT_level
            // 
            resources.ApplyResources(this.BUT_level, "BUT_level");
            this.BUT_level.Name = "BUT_level";
            this.BUT_level.UseVisualStyleBackColor = true;
            this.BUT_level.Click += new System.EventHandler(this.BUT_level_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ConfigAccelerometerCalibration
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BUT_level);
            this.Controls.Add(this.lbl_Accel_user);
            this.Controls.Add(this.BUT_calib_accell);
            this.Controls.Add(this.lineSeparator2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Name = "ConfigAccelerometerCalibration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private LineSeparator lineSeparator2;
        private MyButton BUT_calib_accell;
        private System.Windows.Forms.Label lbl_Accel_user;
        private MyButton BUT_level;
        private System.Windows.Forms.Label label1;
    }
}
