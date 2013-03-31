namespace ArdupilotMega.Controls
{
    partial class ServoOptions
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
            this.BUT_Low = new ArdupilotMega.Controls.MyButton();
            this.TXT_rcchannel = new System.Windows.Forms.TextBox();
            this.BUT_High = new ArdupilotMega.Controls.MyButton();
            this.TXT_pwm_low = new System.Windows.Forms.TextBox();
            this.TXT_pwm_high = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BUT_Repeat = new ArdupilotMega.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // BUT_Low
            // 
            this.BUT_Low.Location = new System.Drawing.Point(0, 1);
            this.BUT_Low.Name = "BUT_Low";
            this.BUT_Low.Size = new System.Drawing.Size(38, 23);
            this.BUT_Low.TabIndex = 0;
            this.BUT_Low.Text = "Low";
            this.BUT_Low.UseVisualStyleBackColor = true;
            this.BUT_Low.Click += new System.EventHandler(this.BUT_Low_Click);
            // 
            // TXT_rcchannel
            // 
            this.TXT_rcchannel.Location = new System.Drawing.Point(170, 3);
            this.TXT_rcchannel.Name = "TXT_rcchannel";
            this.TXT_rcchannel.ReadOnly = true;
            this.TXT_rcchannel.Size = new System.Drawing.Size(21, 20);
            this.TXT_rcchannel.TabIndex = 11;
            this.TXT_rcchannel.TabStop = false;
            this.TXT_rcchannel.Text = "5";
            // 
            // BUT_High
            // 
            this.BUT_High.Location = new System.Drawing.Point(44, 1);
            this.BUT_High.Name = "BUT_High";
            this.BUT_High.Size = new System.Drawing.Size(38, 23);
            this.BUT_High.TabIndex = 1;
            this.BUT_High.Text = "High";
            this.BUT_High.UseVisualStyleBackColor = true;
            this.BUT_High.Click += new System.EventHandler(this.BUT_High_Click);
            // 
            // TXT_pwm_low
            // 
            this.TXT_pwm_low.Location = new System.Drawing.Point(197, 3);
            this.TXT_pwm_low.Name = "TXT_pwm_low";
            this.TXT_pwm_low.Size = new System.Drawing.Size(35, 20);
            this.TXT_pwm_low.TabIndex = 3;
            this.TXT_pwm_low.Text = "1100";
            this.TXT_pwm_low.TextChanged += new System.EventHandler(this.TXT_pwm_low_TextChanged);
            // 
            // TXT_pwm_high
            // 
            this.TXT_pwm_high.Location = new System.Drawing.Point(238, 3);
            this.TXT_pwm_high.Name = "TXT_pwm_high";
            this.TXT_pwm_high.Size = new System.Drawing.Size(35, 20);
            this.TXT_pwm_high.TabIndex = 4;
            this.TXT_pwm_high.Text = "1900";
            this.TXT_pwm_high.TextChanged += new System.EventHandler(this.TXT_pwm_high_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Red;
            this.pictureBox1.Location = new System.Drawing.Point(140, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 23);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // BUT_Repeat
            // 
            this.BUT_Repeat.Location = new System.Drawing.Point(88, 1);
            this.BUT_Repeat.Name = "BUT_Repeat";
            this.BUT_Repeat.Size = new System.Drawing.Size(46, 23);
            this.BUT_Repeat.TabIndex = 2;
            this.BUT_Repeat.Text = "Toggle";
            this.BUT_Repeat.UseVisualStyleBackColor = true;
            this.BUT_Repeat.Click += new System.EventHandler(this.BUT_Repeat_Click);
            // 
            // ServoOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BUT_Repeat);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.TXT_pwm_high);
            this.Controls.Add(this.TXT_pwm_low);
            this.Controls.Add(this.BUT_High);
            this.Controls.Add(this.TXT_rcchannel);
            this.Controls.Add(this.BUT_Low);
            this.Name = "ServoOptions";
            this.Size = new System.Drawing.Size(278, 24);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyButton BUT_Low;
        private System.Windows.Forms.TextBox TXT_rcchannel;
        private MyButton BUT_High;
        private System.Windows.Forms.TextBox TXT_pwm_low;
        private System.Windows.Forms.TextBox TXT_pwm_high;
        private System.Windows.Forms.PictureBox pictureBox1;
        private MyButton BUT_Repeat;
    }
}
