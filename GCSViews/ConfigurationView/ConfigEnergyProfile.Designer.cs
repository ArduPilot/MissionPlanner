namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigEnergyProfile
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.TXT_GradCurNeg = new System.Windows.Forms.TextBox();
            this.TXT_BaseHoverCons = new System.Windows.Forms.TextBox();
            this.TXT_SpeedLoss = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TXT_SpeedZeroDeg = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Btn_SaveChanges = new MissionPlanner.Controls.MyButton();
            this.myLabel1 = new MissionPlanner.Controls.MyLabel();
            this.TXT_GradCurPos = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gradient: Energy usage\r\n-90° to 0°";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "Base energy consumption\r\nat hover (0°)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "I(degree):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(259, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "v(degree):\r\n";
            // 
            // TXT_GradCurNeg
            // 
            this.TXT_GradCurNeg.Location = new System.Drawing.Point(139, 64);
            this.TXT_GradCurNeg.Name = "TXT_GradCurNeg";
            this.TXT_GradCurNeg.Size = new System.Drawing.Size(100, 20);
            this.TXT_GradCurNeg.TabIndex = 8;
            this.TXT_GradCurNeg.TextChanged += new System.EventHandler(this.TXT_ParamA_TextChanged);
            // 
            // TXT_BaseHoverCons
            // 
            this.TXT_BaseHoverCons.Location = new System.Drawing.Point(139, 127);
            this.TXT_BaseHoverCons.Name = "TXT_BaseHoverCons";
            this.TXT_BaseHoverCons.Size = new System.Drawing.Size(100, 20);
            this.TXT_BaseHoverCons.TabIndex = 9;
            // 
            // TXT_SpeedLoss
            // 
            this.TXT_SpeedLoss.Location = new System.Drawing.Point(384, 64);
            this.TXT_SpeedLoss.Name = "TXT_SpeedLoss";
            this.TXT_SpeedLoss.Size = new System.Drawing.Size(100, 20);
            this.TXT_SpeedLoss.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(259, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(119, 26);
            this.label7.TabIndex = 10;
            this.label7.Text = "Gradient:\r\nLoss of speed per 22.5°";
            // 
            // TXT_SpeedZeroDeg
            // 
            this.TXT_SpeedZeroDeg.Location = new System.Drawing.Point(384, 93);
            this.TXT_SpeedZeroDeg.Name = "TXT_SpeedZeroDeg";
            this.TXT_SpeedZeroDeg.Size = new System.Drawing.Size(100, 20);
            this.TXT_SpeedZeroDeg.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(259, 96);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Est. Speed at 0°";
            // 
            // Btn_SaveChanges
            // 
            this.Btn_SaveChanges.Location = new System.Drawing.Point(3, 167);
            this.Btn_SaveChanges.Name = "Btn_SaveChanges";
            this.Btn_SaveChanges.Size = new System.Drawing.Size(68, 32);
            this.Btn_SaveChanges.TabIndex = 14;
            this.Btn_SaveChanges.Text = "Save Changes";
            this.Btn_SaveChanges.UseVisualStyleBackColor = true;
            this.Btn_SaveChanges.Click += new System.EventHandler(this.Btn_SaveChanges_Click);
            // 
            // myLabel1
            // 
            this.myLabel1.Location = new System.Drawing.Point(7, 3);
            this.myLabel1.Name = "myLabel1";
            this.myLabel1.resize = false;
            this.myLabel1.Size = new System.Drawing.Size(75, 23);
            this.myLabel1.TabIndex = 15;
            this.myLabel1.Text = "Parameter";
            // 
            // TXT_GradCurPos
            // 
            this.TXT_GradCurPos.Location = new System.Drawing.Point(139, 96);
            this.TXT_GradCurPos.Name = "TXT_GradCurPos";
            this.TXT_GradCurPos.Size = new System.Drawing.Size(100, 20);
            this.TXT_GradCurPos.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 26);
            this.label3.TabIndex = 16;
            this.label3.Text = "Gradient: Energy usage\r\n0° to 90°";
            // 
            // ConfigEnergyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TXT_GradCurPos);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.myLabel1);
            this.Controls.Add(this.Btn_SaveChanges);
            this.Controls.Add(this.TXT_SpeedZeroDeg);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.TXT_SpeedLoss);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.TXT_BaseHoverCons);
            this.Controls.Add(this.TXT_GradCurNeg);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ConfigEnergyProfile";
            this.Size = new System.Drawing.Size(1076, 556);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TXT_GradCurNeg;
        private System.Windows.Forms.TextBox TXT_BaseHoverCons;
        private System.Windows.Forms.TextBox TXT_SpeedLoss;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TXT_SpeedZeroDeg;
        private System.Windows.Forms.Label label8;
        private Controls.MyButton Btn_SaveChanges;
        private Controls.MyLabel myLabel1;
        private System.Windows.Forms.TextBox TXT_GradCurPos;
        private System.Windows.Forms.Label label3;
    }
}
