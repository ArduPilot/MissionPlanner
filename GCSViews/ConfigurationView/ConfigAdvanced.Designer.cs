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
            this.but_warningmanager = new MissionPlanner.Controls.MyButton();
            this.but_proximity = new MissionPlanner.Controls.MyButton();
            this.but_signkey = new MissionPlanner.Controls.MyButton();
            this.BUT_outputMavlink = new MissionPlanner.Controls.MyButton();
            this.BUT_outputnmea = new MissionPlanner.Controls.MyButton();
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
            this.but_mavinspector.Click += new System.EventHandler(this.but_mavinspector_Click);
            // 
            // but_warningmanager
            // 
            resources.ApplyResources(this.but_warningmanager, "but_warningmanager");
            this.but_warningmanager.Name = "but_warningmanager";
            this.but_warningmanager.UseVisualStyleBackColor = true;
            this.but_warningmanager.Click += new System.EventHandler(this.but_warningmanager_Click);
            // 
            // but_proximity
            // 
            resources.ApplyResources(this.but_proximity, "but_proximity");
            this.but_proximity.Name = "but_proximity";
            this.but_proximity.UseVisualStyleBackColor = true;
            this.but_proximity.Click += new System.EventHandler(this.but_proximity_Click);
            // 
            // but_signkey
            // 
            resources.ApplyResources(this.but_signkey, "but_signkey");
            this.but_signkey.Name = "but_signkey";
            this.but_signkey.UseVisualStyleBackColor = true;
            this.but_signkey.Click += new System.EventHandler(this.but_signkey_Click);
            // 
            // BUT_outputMavlink
            // 
            resources.ApplyResources(this.BUT_outputMavlink, "BUT_outputMavlink");
            this.BUT_outputMavlink.Name = "BUT_outputMavlink";
            this.BUT_outputMavlink.UseVisualStyleBackColor = true;
            this.BUT_outputMavlink.Click += new System.EventHandler(this.BUT_outputMavlink_Click);
            // 
            // BUT_outputnmea
            // 
            resources.ApplyResources(this.BUT_outputnmea, "BUT_outputnmea");
            this.BUT_outputnmea.Name = "BUT_outputnmea";
            this.BUT_outputnmea.UseVisualStyleBackColor = true;
            this.BUT_outputnmea.Click += new System.EventHandler(this.BUT_outputnmea_Click);
            // 
            // ConfigAdvanced
            // 
            this.Controls.Add(this.BUT_outputnmea);
            this.Controls.Add(this.BUT_outputMavlink);
            this.Controls.Add(this.but_signkey);
            this.Controls.Add(this.but_proximity);
            this.Controls.Add(this.but_warningmanager);
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
        private Controls.MyButton but_warningmanager;
        private Controls.MyButton but_proximity;
        private Controls.MyButton but_signkey;
        private Controls.MyButton BUT_outputMavlink;
        private Controls.MyButton BUT_outputnmea;
    }
}
