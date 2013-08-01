using ArdupilotMega.Controls;

namespace ArdupilotMega.GCSViews.ConfigurationView
{
    partial class ConfigFrameType
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigFrameType));
            this.radioButton_Plus = new System.Windows.Forms.RadioButton();
            this.radioButton_X = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBoxPlus = new ArdupilotMega.Controls.PictureBoxWithPseudoOpacity();
            this.pictureBoxX = new ArdupilotMega.Controls.PictureBoxWithPseudoOpacity();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButton_V = new System.Windows.Forms.RadioButton();
            this.pictureBoxV = new ArdupilotMega.Controls.PictureBoxWithPseudoOpacity();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxV)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButton_Plus
            // 
            resources.ApplyResources(this.radioButton_Plus, "radioButton_Plus");
            this.radioButton_Plus.Name = "radioButton_Plus";
            this.radioButton_Plus.TabStop = true;
            this.radioButton_Plus.UseVisualStyleBackColor = true;
            this.radioButton_Plus.CheckedChanged += new System.EventHandler(this.radioButton_Plus_CheckedChanged);
            // 
            // radioButton_X
            // 
            resources.ApplyResources(this.radioButton_X, "radioButton_X");
            this.radioButton_X.Name = "radioButton_X";
            this.radioButton_X.TabStop = true;
            this.radioButton_X.UseVisualStyleBackColor = true;
            this.radioButton_X.CheckedChanged += new System.EventHandler(this.radioButton_X_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // pictureBoxPlus
            // 
            this.pictureBoxPlus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxPlus.Image = global::MissionPlanner.Properties.Resources.frames_plus;
            resources.ApplyResources(this.pictureBoxPlus, "pictureBoxPlus");
            this.pictureBoxPlus.Name = "pictureBoxPlus";
            this.pictureBoxPlus.TabStop = false;
            this.pictureBoxPlus.Click += new System.EventHandler(this.pictureBoxPlus_Click);
            // 
            // pictureBoxX
            // 
            this.pictureBoxX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxX.Image = global::MissionPlanner.Properties.Resources.frames_x;
            resources.ApplyResources(this.pictureBoxX, "pictureBoxX");
            this.pictureBoxX.Name = "pictureBoxX";
            this.pictureBoxX.TabStop = false;
            this.pictureBoxX.Click += new System.EventHandler(this.pictureBoxX_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // radioButton_V
            // 
            resources.ApplyResources(this.radioButton_V, "radioButton_V");
            this.radioButton_V.Name = "radioButton_V";
            this.radioButton_V.TabStop = true;
            this.radioButton_V.UseVisualStyleBackColor = true;
            this.radioButton_V.CheckedChanged += new System.EventHandler(this.radioButton_V_CheckedChanged);
            // 
            // pictureBoxV
            // 
            this.pictureBoxV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxV.Image = global::MissionPlanner.Properties.Resources.new_3DR_04;
            resources.ApplyResources(this.pictureBoxV, "pictureBoxV");
            this.pictureBoxV.Name = "pictureBoxV";
            this.pictureBoxV.TabStop = false;
            this.pictureBoxV.Click += new System.EventHandler(this.pictureBoxV_Click);
            // 
            // ConfigFrameType
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.radioButton_V);
            this.Controls.Add(this.pictureBoxV);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.radioButton_X);
            this.Controls.Add(this.radioButton_Plus);
            this.Controls.Add(this.pictureBoxPlus);
            this.Controls.Add(this.pictureBoxX);
            this.Name = "ConfigFrameType";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBoxWithPseudoOpacity pictureBoxX;
        private PictureBoxWithPseudoOpacity pictureBoxPlus;
        private System.Windows.Forms.RadioButton radioButton_Plus;
        private System.Windows.Forms.RadioButton radioButton_X;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton radioButton_V;
        private PictureBoxWithPseudoOpacity pictureBoxV;
    }
}
