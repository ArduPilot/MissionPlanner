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
            this.CMB_paramfiles = new System.Windows.Forms.ComboBox();
            this.BUT_paramfileload = new MissionPlanner.Controls.MyButton();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TXT_info
            // 
            this.TXT_info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_info.Location = new System.Drawing.Point(4, 246);
            this.TXT_info.Multiline = true;
            this.TXT_info.Name = "TXT_info";
            this.TXT_info.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TXT_info.Size = new System.Drawing.Size(556, 62);
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
            this.panel1.Size = new System.Drawing.Size(556, 208);
            this.panel1.TabIndex = 2;
            // 
            // CMB_paramfiles
            // 
            this.CMB_paramfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CMB_paramfiles.FormattingEnabled = true;
            this.CMB_paramfiles.Location = new System.Drawing.Point(56, 219);
            this.CMB_paramfiles.Name = "CMB_paramfiles";
            this.CMB_paramfiles.Size = new System.Drawing.Size(121, 21);
            this.CMB_paramfiles.TabIndex = 3;
            // 
            // BUT_paramfileload
            // 
            this.BUT_paramfileload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BUT_paramfileload.Location = new System.Drawing.Point(183, 218);
            this.BUT_paramfileload.Name = "BUT_paramfileload";
            this.BUT_paramfileload.Size = new System.Drawing.Size(75, 21);
            this.BUT_paramfileload.TabIndex = 4;
            this.BUT_paramfileload.Text = "Load Params";
            this.BUT_paramfileload.UseVisualStyleBackColor = true;
            this.BUT_paramfileload.Click += new System.EventHandler(this.BUT_paramfileload_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 218);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Defaults";
            // 
            // ConfigSimplePids
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BUT_paramfileload);
            this.Controls.Add(this.CMB_paramfiles);
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
        private System.Windows.Forms.ComboBox CMB_paramfiles;
        private Controls.MyButton BUT_paramfileload;
        private System.Windows.Forms.Label label1;


    }
}
