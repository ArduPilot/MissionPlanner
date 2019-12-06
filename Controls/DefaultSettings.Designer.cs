namespace MissionPlanner.Controls
{
    partial class DefaultSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefaultSettings));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.BUT_paramfileload = new MissionPlanner.Controls.MyButton();
            this.CMB_paramfiles = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // BUT_paramfileload
            // 
            resources.ApplyResources(this.BUT_paramfileload, "BUT_paramfileload");
            this.BUT_paramfileload.Name = "BUT_paramfileload";
            this.BUT_paramfileload.UseVisualStyleBackColor = true;
            this.BUT_paramfileload.Click += new System.EventHandler(this.BUT_paramfileload_Click);
            // 
            // CMB_paramfiles
            // 
            this.CMB_paramfiles.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_paramfiles, "CMB_paramfiles");
            this.CMB_paramfiles.Name = "CMB_paramfiles";
            // 
            // ConfigDefaultSettings
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.BUT_paramfileload);
            this.Controls.Add(this.CMB_paramfiles);
            this.Controls.Add(this.textBox1);
            this.Name = "ConfigDefaultSettings";
            this.Load += new System.EventHandler(this.ConfigDefaultSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private Controls.MyButton BUT_paramfileload;
        private System.Windows.Forms.ComboBox CMB_paramfiles;
    }
}
