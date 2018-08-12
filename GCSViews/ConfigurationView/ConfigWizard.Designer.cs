namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigWizard));
            this.PIC_wizard = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PIC_wizard)).BeginInit();
            this.SuspendLayout();
            // 
            // PIC_wizard
            // 
            resources.ApplyResources(this.PIC_wizard, "PIC_wizard");
            this.PIC_wizard.Image = global::MissionPlanner.Properties.Resources.wizardicon;
            this.PIC_wizard.Name = "PIC_wizard";
            this.PIC_wizard.TabStop = false;
            this.PIC_wizard.Click += new System.EventHandler(this.PIC_wizard_Click);
            // 
            // ConfigWizard
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.PIC_wizard);
            this.Name = "ConfigWizard";
            ((System.ComponentModel.ISupportInitialize)(this.PIC_wizard)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PIC_wizard;

    }
}
