namespace MissionPlanner.GCSViews
{
    partial class SoftwareConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftwareConfig));
            this.backstageView = new MissionPlanner.Controls.BackstageView.BackstageView();
            this.SuspendLayout();
            // 
            // backstageView
            // 
            resources.ApplyResources(this.backstageView, "backstageView");
            this.backstageView.HighlightColor1 = System.Drawing.SystemColors.Highlight;
            this.backstageView.HighlightColor2 = System.Drawing.SystemColors.MenuHighlight;
            this.backstageView.Name = "backstageView";
            this.backstageView.WidthMenu = 172;
            // 
            // SoftwareConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.backstageView);
            this.MinimumSize = new System.Drawing.Size(1000, 450);
            this.Name = "SoftwareConfig";
            resources.ApplyResources(this, "$this");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SoftwareConfig_FormClosing);
            this.Load += new System.EventHandler(this.SoftwareConfig_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.BackstageView.BackstageView backstageView;
    }
}
