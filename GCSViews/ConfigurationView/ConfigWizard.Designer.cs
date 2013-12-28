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
            this.PIC_wizard = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PIC_wizard)).BeginInit();
            this.SuspendLayout();
            // 
            // PIC_wizard
            // 
            this.PIC_wizard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PIC_wizard.Image = global::MissionPlanner.Properties.Resources.wizardicon;
            this.PIC_wizard.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.PIC_wizard.Location = new System.Drawing.Point(193, 93);
            this.PIC_wizard.Name = "PIC_wizard";
            this.PIC_wizard.Size = new System.Drawing.Size(100, 70);
            this.PIC_wizard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PIC_wizard.TabIndex = 5;
            this.PIC_wizard.TabStop = false;
            this.PIC_wizard.Click += new System.EventHandler(this.PIC_wizard_Click);
            // 
            // ConfigWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PIC_wizard);
            this.Name = "ConfigWizard";
            this.Size = new System.Drawing.Size(486, 256);
            ((System.ComponentModel.ISupportInitialize)(this.PIC_wizard)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PIC_wizard;

    }
}
