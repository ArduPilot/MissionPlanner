namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigFirmwareDisabled
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigFirmwareDisabled));
            this.label1 = new System.Windows.Forms.Label();
            this.but_bootloaderupdate = new MissionPlanner.Controls.MyButton();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // but_bootloaderupdate
            // 
            resources.ApplyResources(this.but_bootloaderupdate, "but_bootloaderupdate");
            this.but_bootloaderupdate.Name = "but_bootloaderupdate";
            this.but_bootloaderupdate.UseVisualStyleBackColor = true;
            this.but_bootloaderupdate.Click += new System.EventHandler(this.but_bootloaderupdate_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ConfigFirmwareDisabled
            // 
            this.Controls.Add(this.label2);
            this.Controls.Add(this.but_bootloaderupdate);
            this.Controls.Add(this.label1);
            this.Name = "ConfigFirmwareDisabled";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private MissionPlanner.Controls.MyButton but_bootloaderupdate;
        private System.Windows.Forms.Label label2;
    }
}
