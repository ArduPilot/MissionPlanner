namespace MissionPlanner.Joystick
{
    partial class Joy_Button_axis
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
            this.numericUpDownpwmmin = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownpwmmax = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownpwmmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownpwmmax)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownpwmmin
            // 
            this.numericUpDownpwmmin.Location = new System.Drawing.Point(77, 7);
            this.numericUpDownpwmmin.Maximum = new decimal(new int[] {
            2200,
            0,
            0,
            0});
            this.numericUpDownpwmmin.Minimum = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.numericUpDownpwmmin.Name = "numericUpDownpwmmin";
            this.numericUpDownpwmmin.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownpwmmin.TabIndex = 0;
            this.numericUpDownpwmmin.Value = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.numericUpDownpwmmin.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "PWM 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(130, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "PWM 2";
            // 
            // numericUpDownpwmmax
            // 
            this.numericUpDownpwmmax.Location = new System.Drawing.Point(193, 7);
            this.numericUpDownpwmmax.Maximum = new decimal(new int[] {
            2200,
            0,
            0,
            0});
            this.numericUpDownpwmmax.Minimum = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.numericUpDownpwmmax.Name = "numericUpDownpwmmax";
            this.numericUpDownpwmmax.Size = new System.Drawing.Size(78, 20);
            this.numericUpDownpwmmax.TabIndex = 3;
            this.numericUpDownpwmmax.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.numericUpDownpwmmax.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // Joy_Button_axis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            
            this.ClientSize = new System.Drawing.Size(298, 36);
            this.Controls.Add(this.numericUpDownpwmmax);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownpwmmin);
            this.Name = "Joy_Button_axis";
            this.Text = "Joy_Button_axis";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownpwmmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownpwmmax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownpwmmin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownpwmmax;
    }
}