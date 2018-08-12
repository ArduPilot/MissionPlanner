using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
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
            this.pictureBoxPlus = new MissionPlanner.Controls.PictureBoxWithPseudoOpacity();
            this.pictureBoxX = new MissionPlanner.Controls.PictureBoxWithPseudoOpacity();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButton_V = new System.Windows.Forms.RadioButton();
            this.pictureBoxV = new MissionPlanner.Controls.PictureBoxWithPseudoOpacity();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButton_H = new System.Windows.Forms.RadioButton();
            this.pictureBoxH = new MissionPlanner.Controls.PictureBoxWithPseudoOpacity();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButton_Y = new System.Windows.Forms.RadioButton();
            this.pictureBoxY = new MissionPlanner.Controls.PictureBoxWithPseudoOpacity();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.configDefaultSettings1 = new MissionPlanner.Controls.DefaultSettings();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.radioButton_VTail = new System.Windows.Forms.RadioButton();
            this.pictureBoxVTail = new MissionPlanner.Controls.PictureBoxWithPseudoOpacity();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxY)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVTail)).BeginInit();
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
            resources.ApplyResources(this.pictureBoxPlus, "pictureBoxPlus");
            this.pictureBoxPlus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxPlus.Image = global::MissionPlanner.Properties.Resources.frames_plus;
            this.pictureBoxPlus.Name = "pictureBoxPlus";
            this.pictureBoxPlus.TabStop = false;
            this.pictureBoxPlus.Click += new System.EventHandler(this.pictureBoxPlus_Click);
            // 
            // pictureBoxX
            // 
            resources.ApplyResources(this.pictureBoxX, "pictureBoxX");
            this.pictureBoxX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxX.Image = global::MissionPlanner.Properties.Resources.frames_x;
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
            resources.ApplyResources(this.pictureBoxV, "pictureBoxV");
            this.pictureBoxV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxV.Image = global::MissionPlanner.Properties.Resources.new_3DR_04;
            this.pictureBoxV.Name = "pictureBoxV";
            this.pictureBoxV.TabStop = false;
            this.pictureBoxV.Click += new System.EventHandler(this.pictureBoxV_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // radioButton_H
            // 
            resources.ApplyResources(this.radioButton_H, "radioButton_H");
            this.radioButton_H.Name = "radioButton_H";
            this.radioButton_H.TabStop = true;
            this.radioButton_H.UseVisualStyleBackColor = true;
            this.radioButton_H.CheckedChanged += new System.EventHandler(this.radioButton_H_CheckedChanged);
            // 
            // pictureBoxH
            // 
            resources.ApplyResources(this.pictureBoxH, "pictureBoxH");
            this.pictureBoxH.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxH.Image = global::MissionPlanner.Properties.Resources.frames_h;
            this.pictureBoxH.Name = "pictureBoxH";
            this.pictureBoxH.TabStop = false;
            this.pictureBoxH.Click += new System.EventHandler(this.pictureBoxH_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // radioButton_Y
            // 
            resources.ApplyResources(this.radioButton_Y, "radioButton_Y");
            this.radioButton_Y.Name = "radioButton_Y";
            this.radioButton_Y.TabStop = true;
            this.radioButton_Y.UseVisualStyleBackColor = true;
            this.radioButton_Y.CheckedChanged += new System.EventHandler(this.radioButton_Y_CheckedChanged);
            // 
            // pictureBoxY
            // 
            resources.ApplyResources(this.pictureBoxY, "pictureBoxY");
            this.pictureBoxY.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxY.Image = global::MissionPlanner.Properties.Resources.y6b;
            this.pictureBoxY.Name = "pictureBoxY";
            this.pictureBoxY.TabStop = false;
            this.pictureBoxY.Click += new System.EventHandler(this.pictureBoxY_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.configDefaultSettings1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // configDefaultSettings1
            // 
            resources.ApplyResources(this.configDefaultSettings1, "configDefaultSettings1");
            this.configDefaultSettings1.Name = "configDefaultSettings1";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.radioButton_VTail);
            this.groupBox2.Controls.Add(this.pictureBoxVTail);
            this.groupBox2.Controls.Add(this.pictureBoxPlus);
            this.groupBox2.Controls.Add(this.pictureBoxX);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.radioButton_Plus);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.radioButton_X);
            this.groupBox2.Controls.Add(this.radioButton_Y);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.pictureBoxY);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.pictureBoxV);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.radioButton_V);
            this.groupBox2.Controls.Add(this.radioButton_H);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.pictureBoxH);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // radioButton_VTail
            // 
            resources.ApplyResources(this.radioButton_VTail, "radioButton_VTail");
            this.radioButton_VTail.Name = "radioButton_VTail";
            this.radioButton_VTail.TabStop = true;
            this.radioButton_VTail.UseVisualStyleBackColor = true;
            this.radioButton_VTail.CheckedChanged += new System.EventHandler(this.radioButton_VTail_CheckedChanged);
            // 
            // pictureBoxVTail
            // 
            resources.ApplyResources(this.pictureBoxVTail, "pictureBoxVTail");
            this.pictureBoxVTail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxVTail.Name = "pictureBoxVTail";
            this.pictureBoxVTail.TabStop = false;
            this.pictureBoxVTail.Click += new System.EventHandler(this.pictureBoxVTail_Click);
            // 
            // ConfigFrameType
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ConfigFrameType";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxY)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVTail)).EndInit();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButton_H;
        private PictureBoxWithPseudoOpacity pictureBoxH;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioButton_Y;
        private PictureBoxWithPseudoOpacity pictureBoxY;
        private System.Windows.Forms.Label label7;
        private DefaultSettings configDefaultSettings1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton radioButton_VTail;
        private PictureBoxWithPseudoOpacity pictureBoxVTail;
    }
}
