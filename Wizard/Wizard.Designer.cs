namespace ArdupilotMega.Controls.Wizard
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
            this.BUT_Next = new ArdupilotMega.Controls.MyButton();
            this.BUT_Back = new ArdupilotMega.Controls.MyButton();
            this.progressStep1 = new ArdupilotMega.Controls.ProgressStep();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(983, 483);
            this.panel1.TabIndex = 2;
            // 
            // BUT_Next
            // 
            this.BUT_Next.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BUT_Next.Location = new System.Drawing.Point(921, 502);
            this.BUT_Next.Name = "BUT_Next";
            this.BUT_Next.Size = new System.Drawing.Size(75, 23);
            this.BUT_Next.TabIndex = 1;
            this.BUT_Next.Text = "Next";
            this.BUT_Next.UseVisualStyleBackColor = true;
            this.BUT_Next.Click += new System.EventHandler(this.BUT_Next_Click);
            // 
            // BUT_Back
            // 
            this.BUT_Back.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BUT_Back.Enabled = false;
            this.BUT_Back.Location = new System.Drawing.Point(840, 502);
            this.BUT_Back.Name = "BUT_Back";
            this.BUT_Back.Size = new System.Drawing.Size(75, 23);
            this.BUT_Back.TabIndex = 0;
            this.BUT_Back.Text = "Back";
            this.BUT_Back.UseVisualStyleBackColor = true;
            this.BUT_Back.Click += new System.EventHandler(this.BUT_Back_Click);
            // 
            // progressStep1
            // 
            this.progressStep1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressStep1.Location = new System.Drawing.Point(231, 502);
            this.progressStep1.Name = "progressStep1";
            this.progressStep1.Size = new System.Drawing.Size(384, 26);
            this.progressStep1.TabIndex = 3;
            // 
            // Wizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 537);
            this.Controls.Add(this.progressStep1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.BUT_Next);
            this.Controls.Add(this.BUT_Back);
            this.Name = "Wizard";
            this.Text = "Wizard";
            this.ResumeLayout(false);

        }

        #endregion

        private MyButton BUT_Back;
        private MyButton BUT_Next;
        private System.Windows.Forms.Panel panel1;
        private ProgressStep progressStep1;
    }
}