namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigMotorTest
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
            this.NUM_thr_percent = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.NUM_duration = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_thr_percent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_duration)).BeginInit();
            this.SuspendLayout();
            // 
            // NUM_thr_percent
            // 
            this.NUM_thr_percent.Location = new System.Drawing.Point(111, 3);
            this.NUM_thr_percent.Name = "NUM_thr_percent";
            this.NUM_thr_percent.Size = new System.Drawing.Size(54, 20);
            this.NUM_thr_percent.TabIndex = 0;
            this.NUM_thr_percent.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Throttle %";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(324, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(214, 52);
            this.label2.TabIndex = 2;
            this.label2.Text = "NOTE: PLEASE HOLD DOWN YOUR UAV\r\nThis will test your motors are working.\r\nMotors " +
    "are tested in a clockwise rotation \r\nstarting at the front right.";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(324, 65);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(218, 26);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Please click here to see your motor numbers,\r\nscroll to the bottom of the page";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // NUM_duration
            // 
            this.NUM_duration.Location = new System.Drawing.Point(246, 3);
            this.NUM_duration.Name = "NUM_duration";
            this.NUM_duration.Size = new System.Drawing.Size(54, 20);
            this.NUM_duration.TabIndex = 4;
            this.NUM_duration.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(174, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Duration (s)";
            // 
            // ConfigMotorTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.NUM_duration);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NUM_thr_percent);
            this.Name = "ConfigMotorTest";
            this.Size = new System.Drawing.Size(563, 264);
            ((System.ComponentModel.ISupportInitialize)(this.NUM_thr_percent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_duration)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown NUM_thr_percent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.NumericUpDown NUM_duration;
        private System.Windows.Forms.Label label3;
    }
}
