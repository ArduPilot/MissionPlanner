using MissionPlanner.Controls;

namespace MissionPlanner.Wizard
{
    partial class Wizard
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressStep1 = new Controls.ProgressStep();
            this.BUT_Next = new Controls.MyButton();
            this.BUT_Back = new Controls.MyButton();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(799, 500);
            this.panel1.TabIndex = 2;
            // 
            // progressStep1
            // 
            this.progressStep1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressStep1.BackColor = System.Drawing.Color.Transparent;
            this.progressStep1.Location = new System.Drawing.Point(30, 505);
            this.progressStep1.Maximum = 0;
            this.progressStep1.Name = "progressStep1";
            this.progressStep1.Size = new System.Drawing.Size(576, 51);
            this.progressStep1.Step = 0;
            this.progressStep1.TabIndex = 3;
            // 
            // BUT_Next
            // 
            this.BUT_Next.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BUT_Next.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_Next.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_Next.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BUT_Next.Location = new System.Drawing.Point(693, 505);
            this.BUT_Next.Name = "BUT_Next";
            this.BUT_Next.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_Next.Size = new System.Drawing.Size(75, 24);
            this.BUT_Next.TabIndex = 1;
            this.BUT_Next.Text = "NEXT >>";
            this.BUT_Next.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Next.UseVisualStyleBackColor = true;
            this.BUT_Next.Click += new System.EventHandler(this.BUT_Next_Click);
            // 
            // BUT_Back
            // 
            this.BUT_Back.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BUT_Back.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_Back.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_Back.Enabled = false;
            this.BUT_Back.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BUT_Back.Location = new System.Drawing.Point(612, 505);
            this.BUT_Back.Name = "BUT_Back";
            this.BUT_Back.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_Back.Size = new System.Drawing.Size(75, 24);
            this.BUT_Back.TabIndex = 0;
            this.BUT_Back.Text = "<< BACK";
            this.BUT_Back.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Back.UseVisualStyleBackColor = true;
            this.BUT_Back.Click += new System.EventHandler(this.BUT_Back_Click);
            // 
            // Wizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(799, 562);
            this.Controls.Add(this.progressStep1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.BUT_Next);
            this.Controls.Add(this.BUT_Back);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Wizard";
            this.Text = "Wizard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Wizard_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private MyButton BUT_Back;
        private MyButton BUT_Next;
        private System.Windows.Forms.Panel panel1;
        private ProgressStep progressStep1;
    }
}