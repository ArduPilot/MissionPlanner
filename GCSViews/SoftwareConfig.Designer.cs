namespace ArdupilotMega.GCSViews
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
            this.backstageView = new ArdupilotMega.Controls.BackstageView.BackstageView();
            this.SuspendLayout();
            // 
            // backstageView
            // 
            this.backstageView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backstageView.HighlightColor1 = System.Drawing.SystemColors.Highlight;
            this.backstageView.HighlightColor2 = System.Drawing.SystemColors.MenuHighlight;
            this.backstageView.Location = new System.Drawing.Point(0, 0);
            this.backstageView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.backstageView.Name = "backstageView";
            this.backstageView.Size = new System.Drawing.Size(1000, 450);
            this.backstageView.TabIndex = 1;
            this.backstageView.WidthMenu = 172;
            // 
            // SoftwareConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.backstageView);
            this.MinimumSize = new System.Drawing.Size(1000, 450);
            this.Name = "SoftwareConfig";
            this.Size = new System.Drawing.Size(1000, 450);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SoftwareConfig_FormClosing);
            this.Load += new System.EventHandler(this.SoftwareConfig_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.BackstageView.BackstageView backstageView;
    }
}
