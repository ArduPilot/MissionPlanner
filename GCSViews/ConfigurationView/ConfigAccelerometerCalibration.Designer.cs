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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxAccelCal = new System.Windows.Forms.GroupBox();
            this.pictureBoxOrientation = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.BUT_calib_accell = new MissionPlanner.Controls.MyButton();
            this.lbl_Accel_user = new System.Windows.Forms.Label();
            this.groupBoxLevel = new System.Windows.Forms.GroupBox();
            this.BUT_level = new MissionPlanner.Controls.MyButton();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxSimple = new System.Windows.Forms.GroupBox();
            this.BUT_simpleAccelCal = new MissionPlanner.Controls.MyButton();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxAccelCal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOrientation)).BeginInit();
            this.groupBoxLevel.SuspendLayout();
            this.groupBoxSimple.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label5.Name = "label5";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.groupBoxAccelCal, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxLevel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxSimple, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBoxAccelCal
            // 
            resources.ApplyResources(this.groupBoxAccelCal, "groupBoxAccelCal");
            this.groupBoxAccelCal.Controls.Add(this.pictureBoxOrientation);
            this.groupBoxAccelCal.Controls.Add(this.label4);
            this.groupBoxAccelCal.Controls.Add(this.BUT_calib_accell);
            this.groupBoxAccelCal.Controls.Add(this.lbl_Accel_user);
            this.groupBoxAccelCal.Name = "groupBoxAccelCal";
            this.groupBoxAccelCal.TabStop = false;
            // 
            // pictureBoxOrientation
            // 
            resources.ApplyResources(this.pictureBoxOrientation, "pictureBoxOrientation");
            this.pictureBoxOrientation.Name = "pictureBoxOrientation";
            this.pictureBoxOrientation.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // BUT_calib_accell
            // 
            resources.ApplyResources(this.BUT_calib_accell, "BUT_calib_accell");
            this.BUT_calib_accell.Name = "BUT_calib_accell";
            this.BUT_calib_accell.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_calib_accell.UseVisualStyleBackColor = true;
            this.BUT_calib_accell.Click += new System.EventHandler(this.BUT_calib_accell_Click);
            // 
            // lbl_Accel_user
            // 
            resources.ApplyResources(this.lbl_Accel_user, "lbl_Accel_user");
            this.lbl_Accel_user.Name = "lbl_Accel_user";
            // 
            // groupBoxLevel
            // 
            resources.ApplyResources(this.groupBoxLevel, "groupBoxLevel");
            this.groupBoxLevel.Controls.Add(this.BUT_level);
            this.groupBoxLevel.Controls.Add(this.label1);
            this.groupBoxLevel.Name = "groupBoxLevel";
            this.groupBoxLevel.TabStop = false;
            // 
            // BUT_level
            // 
            resources.ApplyResources(this.BUT_level, "BUT_level");
            this.BUT_level.Name = "BUT_level";
            this.BUT_level.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_level.UseVisualStyleBackColor = true;
            this.BUT_level.Click += new System.EventHandler(this.BUT_level_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBoxSimple
            // 
            resources.ApplyResources(this.groupBoxSimple, "groupBoxSimple");
            this.groupBoxSimple.Controls.Add(this.BUT_simpleAccelCal);
            this.groupBoxSimple.Controls.Add(this.label2);
            this.groupBoxSimple.Name = "groupBoxSimple";
            this.groupBoxSimple.TabStop = false;
            // 
            // BUT_simpleAccelCal
            // 
            resources.ApplyResources(this.BUT_simpleAccelCal, "BUT_simpleAccelCal");
            this.BUT_simpleAccelCal.Name = "BUT_simpleAccelCal";
            this.BUT_simpleAccelCal.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_simpleAccelCal.UseVisualStyleBackColor = true;
            this.BUT_simpleAccelCal.Click += new System.EventHandler(this.BUT_simpleAccelCal_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ConfigAccelerometerCalibration
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label5);
            this.Name = "ConfigAccelerometerCalibration";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxAccelCal.ResumeLayout(false);
            this.groupBoxAccelCal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOrientation)).EndInit();
            this.groupBoxLevel.ResumeLayout(false);
            this.groupBoxLevel.PerformLayout();
            this.groupBoxSimple.ResumeLayout(false);
            this.groupBoxSimple.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBoxAccelCal;
        private System.Windows.Forms.PictureBox pictureBoxOrientation;
        private System.Windows.Forms.Label lbl_Accel_user;
        private MyButton BUT_calib_accell;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBoxLevel;
        private MyButton BUT_level;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxSimple;
        private MyButton BUT_simpleAccelCal;
        private System.Windows.Forms.Label label2;
    }
}
