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
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TXT_ParamA = new System.Windows.Forms.TextBox();
            this.TXT_ParamB = new System.Windows.Forms.TextBox();
            this.TXT_ParamC = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TXT_ParamD = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Btn_SaveChanges = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Parameter a";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(237, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Parameter b";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Parameter";
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
            this.label6.Location = new System.Drawing.Point(3, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "v(degree):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(64, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(302, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Notes: Energyconsumption depends on climb angle and speed";
            // 
            // TXT_ParamA
            // 
            this.TXT_ParamA.Location = new System.Drawing.Point(131, 32);
            this.TXT_ParamA.Name = "TXT_ParamA";
            this.TXT_ParamA.Size = new System.Drawing.Size(100, 20);
            this.TXT_ParamA.TabIndex = 8;
            this.TXT_ParamA.TextChanged += new System.EventHandler(this.TXT_ParamA_TextChanged);
            // 
            // TXT_ParamB
            // 
            this.TXT_ParamB.Location = new System.Drawing.Point(307, 32);
            this.TXT_ParamB.Name = "TXT_ParamB";
            this.TXT_ParamB.Size = new System.Drawing.Size(100, 20);
            this.TXT_ParamB.TabIndex = 9;
            // 
            // TXT_ParamC
            // 
            this.TXT_ParamC.Location = new System.Drawing.Point(131, 58);
            this.TXT_ParamC.Name = "TXT_ParamC";
            this.TXT_ParamC.Size = new System.Drawing.Size(100, 20);
            this.TXT_ParamC.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(61, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Parameter c";
            // 
            // TXT_ParamD
            // 
            this.TXT_ParamD.Location = new System.Drawing.Point(307, 58);
            this.TXT_ParamD.Name = "TXT_ParamD";
            this.TXT_ParamD.Size = new System.Drawing.Size(100, 20);
            this.TXT_ParamD.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(237, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Parameter d";
            // 
            // Btn_SaveChanges
            // 
            this.Btn_SaveChanges.Location = new System.Drawing.Point(3, 77);
            this.Btn_SaveChanges.Name = "Btn_SaveChanges";
            this.Btn_SaveChanges.Size = new System.Drawing.Size(68, 32);
            this.Btn_SaveChanges.TabIndex = 14;
            this.Btn_SaveChanges.Text = "Save Changes";
            this.Btn_SaveChanges.UseVisualStyleBackColor = true;
            this.Btn_SaveChanges.Click += new System.EventHandler(this.Btn_SaveChanges_Click);
            // 
            // ConfigEnergyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Btn_SaveChanges);
            this.Controls.Add(this.TXT_ParamD);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.TXT_ParamC);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.TXT_ParamB);
            this.Controls.Add(this.TXT_ParamA);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ConfigEnergyProfile";
            this.Size = new System.Drawing.Size(802, 273);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TXT_ParamA;
        private System.Windows.Forms.TextBox TXT_ParamB;
        private System.Windows.Forms.TextBox TXT_ParamC;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TXT_ParamD;
        private System.Windows.Forms.Label label8;
        private Controls.MyButton Btn_SaveChanges;
    }
}
