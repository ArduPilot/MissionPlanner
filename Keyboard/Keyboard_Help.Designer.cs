namespace MissionPlanner.Keyboard
{
    partial class Keyboard_Help
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LBL_controls = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.LBL_factors = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Controls";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label2.Location = new System.Drawing.Point(9, 288);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Factors";
            // 
            // LBL_controls
            // 
            this.LBL_controls.AutoSize = true;
            this.LBL_controls.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.LBL_controls.Location = new System.Drawing.Point(296, 37);
            this.LBL_controls.MaximumSize = new System.Drawing.Size(200, 0);
            this.LBL_controls.Name = "LBL_controls";
            this.LBL_controls.Size = new System.Drawing.Size(38, 15);
            this.LBL_controls.TabIndex = 3;
            this.LBL_controls.Text = "Label";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MissionPlanner.Properties.Resources.Controls;
            this.pictureBox1.Location = new System.Drawing.Point(8, 37);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(282, 240);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::MissionPlanner.Properties.Resources.Factors;
            this.pictureBox2.Location = new System.Drawing.Point(15, 316);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(200, 150);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // LBL_factors
            // 
            this.LBL_factors.AutoSize = true;
            this.LBL_factors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.LBL_factors.Location = new System.Drawing.Point(221, 316);
            this.LBL_factors.MaximumSize = new System.Drawing.Size(200, 0);
            this.LBL_factors.Name = "LBL_factors";
            this.LBL_factors.Size = new System.Drawing.Size(38, 15);
            this.LBL_factors.TabIndex = 6;
            this.LBL_factors.Text = "Label";
            // 
            // Keyboard_Help
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(506, 503);
            this.Controls.Add(this.LBL_factors);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.LBL_controls);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Keyboard_Help";
            this.Text = "Keyboard Controller Help";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LBL_controls;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label LBL_factors;
    }
}