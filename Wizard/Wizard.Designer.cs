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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wizard));
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressStep1 = new MissionPlanner.Controls.ProgressStep();
            this.BUT_Next = new MissionPlanner.Controls.MyButton();
            this.BUT_Back = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Name = "panel1";
            // 
            // progressStep1
            // 
            resources.ApplyResources(this.progressStep1, "progressStep1");
            this.progressStep1.BackColor = System.Drawing.Color.Transparent;
            this.progressStep1.Maximum = 0;
            this.progressStep1.Name = "progressStep1";
            this.progressStep1.Step = 0;
            // 
            // BUT_Next
            // 
            resources.ApplyResources(this.BUT_Next, "BUT_Next");
            this.BUT_Next.Name = "BUT_Next";
            this.BUT_Next.UseVisualStyleBackColor = true;
            this.BUT_Next.Click += new System.EventHandler(this.BUT_Next_Click);
            // 
            // BUT_Back
            // 
            resources.ApplyResources(this.BUT_Back, "BUT_Back");
            this.BUT_Back.Name = "BUT_Back";
            this.BUT_Back.UseVisualStyleBackColor = true;
            this.BUT_Back.Click += new System.EventHandler(this.BUT_Back_Click);
            // 
            // Wizard
            // 
            resources.ApplyResources(this, "$this");
            
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.progressStep1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.BUT_Next);
            this.Controls.Add(this.BUT_Back);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Wizard";
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