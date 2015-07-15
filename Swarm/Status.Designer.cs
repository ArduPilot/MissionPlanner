namespace MissionPlanner.Swarm
{
    partial class Status
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
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_gps = new System.Windows.Forms.Label();
            this.lbl_armed = new System.Windows.Forms.Label();
            this.lbl_mode = new System.Windows.Forms.Label();
            this.lbl_mav = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_guided = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "GPS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Armed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Mode";
            // 
            // lbl_gps
            // 
            this.lbl_gps.AutoSize = true;
            this.lbl_gps.Location = new System.Drawing.Point(46, 39);
            this.lbl_gps.Name = "lbl_gps";
            this.lbl_gps.Size = new System.Drawing.Size(29, 13);
            this.lbl_gps.TabIndex = 3;
            this.lbl_gps.Text = "GPS";
            // 
            // lbl_armed
            // 
            this.lbl_armed.AutoSize = true;
            this.lbl_armed.Location = new System.Drawing.Point(46, 13);
            this.lbl_armed.Name = "lbl_armed";
            this.lbl_armed.Size = new System.Drawing.Size(37, 13);
            this.lbl_armed.TabIndex = 4;
            this.lbl_armed.Text = "Armed";
            // 
            // lbl_mode
            // 
            this.lbl_mode.AutoSize = true;
            this.lbl_mode.Location = new System.Drawing.Point(46, 26);
            this.lbl_mode.Name = "lbl_mode";
            this.lbl_mode.Size = new System.Drawing.Size(34, 13);
            this.lbl_mode.TabIndex = 5;
            this.lbl_mode.Text = "Mode";
            // 
            // lbl_mav
            // 
            this.lbl_mav.AutoSize = true;
            this.lbl_mav.Location = new System.Drawing.Point(3, 0);
            this.lbl_mav.Name = "lbl_mav";
            this.lbl_mav.Size = new System.Drawing.Size(29, 13);
            this.lbl_mav.TabIndex = 7;
            this.lbl_mav.Text = "GPS";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Guided";
            // 
            // lbl_guided
            // 
            this.lbl_guided.Location = new System.Drawing.Point(46, 52);
            this.lbl_guided.Name = "lbl_guided";
            this.lbl_guided.Size = new System.Drawing.Size(67, 30);
            this.lbl_guided.TabIndex = 9;
            this.lbl_guided.Text = "0.00000,0.00000,0.00";
            // 
            // Status
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl_guided);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbl_mav);
            this.Controls.Add(this.lbl_mode);
            this.Controls.Add(this.lbl_armed);
            this.Controls.Add(this.lbl_gps);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Status";
            this.Size = new System.Drawing.Size(113, 82);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_gps;
        private System.Windows.Forms.Label lbl_armed;
        private System.Windows.Forms.Label lbl_mode;
        private System.Windows.Forms.Label lbl_mav;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbl_guided;

    }
}
