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
            this.TXT_info = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // TXT_info
            // 
            this.TXT_info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_info.Location = new System.Drawing.Point(4, 226);
            this.TXT_info.Multiline = true;
            this.TXT_info.Name = "TXT_info";
            this.TXT_info.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TXT_info.Size = new System.Drawing.Size(556, 82);
            this.TXT_info.TabIndex = 1;
            this.TXT_info.Text = "NOTE: using this interface may reset some off your custom pids.";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(556, 216);
            this.panel1.TabIndex = 2;
            // 
            // ConfigSimplePids
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.TXT_info);
            this.Name = "ConfigSimplePids";
            this.Size = new System.Drawing.Size(563, 311);
            this.Load += new System.EventHandler(this.ConfigSimplePids_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TXT_info;
        private System.Windows.Forms.Panel panel1;


    }
}
