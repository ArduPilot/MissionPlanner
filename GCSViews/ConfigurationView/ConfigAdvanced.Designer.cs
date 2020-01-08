namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigAdvanced
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigAdvanced));
            this.label1 = new System.Windows.Forms.Label();
            this.but_mavinspector = new MissionPlanner.Controls.MyButton();
            this.button3 = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // but_mavinspector
            // 
            resources.ApplyResources(this.but_mavinspector, "but_mavinspector");
            this.but_mavinspector.Name = "but_mavinspector";
            this.but_mavinspector.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // ConfigAdvanced
            // 
            this.Controls.Add(this.button3);
            this.Controls.Add(this.but_mavinspector);
            this.Controls.Add(this.label1);
            this.Name = "ConfigAdvanced";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Controls.MyButton but_mavinspector;
        private Controls.MyButton button3;
    }
}
