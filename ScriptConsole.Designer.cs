namespace MissionPlanner
{
    partial class ScriptConsole
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptConsole));
            this.textOutput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.updateoutput = new System.Windows.Forms.Timer(this.components);
            this.autoscrollCheckbox = new System.Windows.Forms.CheckBox();
            this.BUT_clear = new Controls.MyButton();
            this.SuspendLayout();
            // 
            // textOutput
            // 
            resources.ApplyResources(this.textOutput, "textOutput");
            this.textOutput.Name = "textOutput";
            this.textOutput.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // updateoutput
            // 
            this.updateoutput.Interval = 250;
            this.updateoutput.Tick += new System.EventHandler(this.updateoutput_Tick);
            // 
            // autoscrollCheckbox
            // 
            resources.ApplyResources(this.autoscrollCheckbox, "autoscrollCheckbox");
            this.autoscrollCheckbox.Checked = true;
            this.autoscrollCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoscrollCheckbox.Name = "autoscrollCheckbox";
            this.autoscrollCheckbox.UseVisualStyleBackColor = true;
            // 
            // BUT_clear
            // 
            resources.ApplyResources(this.BUT_clear, "BUT_clear");
            this.BUT_clear.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_clear.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_clear.Name = "BUT_clear";
            this.BUT_clear.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_clear.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_clear.UseVisualStyleBackColor = true;
            this.BUT_clear.Click += new System.EventHandler(this.BUT_clear_Click);
            // 
            // ScriptConsole
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.BUT_clear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.autoscrollCheckbox);
            this.Controls.Add(this.textOutput);
            this.Name = "ScriptConsole";
            this.Load += new System.EventHandler(this.ScriptConsole_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textOutput;
        private System.Windows.Forms.Label label1;
        private Controls.MyButton BUT_clear;
        private System.Windows.Forms.Timer updateoutput;
        private System.Windows.Forms.CheckBox autoscrollCheckbox;
    }
}