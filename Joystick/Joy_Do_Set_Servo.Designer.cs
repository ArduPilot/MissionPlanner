namespace MissionPlanner.Joystick
{
    partial class Joy_Do_Set_Servo
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.numericUpDownservono = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownpwm = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownservono)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownpwm)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownservono
            // 
            this.numericUpDownservono.Location = new System.Drawing.Point(77, 7);
            this.numericUpDownservono.Name = "numericUpDownservono";
            this.numericUpDownservono.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownservono.TabIndex = 0;
            this.numericUpDownservono.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Servo No#";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(130, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "PWM";
            // 
            // numericUpDownpwm
            // 
            this.numericUpDownpwm.Location = new System.Drawing.Point(171, 7);
            this.numericUpDownpwm.Maximum = new decimal(new int[] {
            2100,
            0,
            0,
            0});
            this.numericUpDownpwm.Minimum = new decimal(new int[] {
            900,
            0,
            0,
            0});
            this.numericUpDownpwm.Name = "numericUpDownpwm";
            this.numericUpDownpwm.Size = new System.Drawing.Size(78, 20);
            this.numericUpDownpwm.TabIndex = 3;
            this.numericUpDownpwm.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.numericUpDownpwm.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // Joy_Do_Set_Servo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            
            this.ClientSize = new System.Drawing.Size(258, 36);
            this.Controls.Add(this.numericUpDownpwm);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownservono);
            this.Name = "Joy_Do_Set_Servo";
            this.Text = "Joy_Do_Set_Servo";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownservono)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownpwm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownservono;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownpwm;
    }
}