namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigSimplePids
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigSimplePids));
            this.TXT_info = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // TXT_info
            // 
            resources.ApplyResources(this.TXT_info, "TXT_info");
            this.TXT_info.Name = "TXT_info";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // ConfigSimplePids
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.TXT_info);
            this.Name = "ConfigSimplePids";
            this.Load += new System.EventHandler(this.ConfigSimplePids_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TXT_info;
        private System.Windows.Forms.Panel panel1;


    }
}
