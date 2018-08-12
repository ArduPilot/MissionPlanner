namespace MissionPlanner.GCSViews
{
    partial class Help
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Help));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.CHK_showconsole = new System.Windows.Forms.CheckBox();
            this.BUT_updatecheck = new MissionPlanner.Controls.MyButton();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.PIC_wizard = new System.Windows.Forms.PictureBox();
            this.BUT_betaupdate = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.PIC_wizard)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBox1.DetectUrls = false;
            this.richTextBox1.Name = "richTextBox1";
            // 
            // CHK_showconsole
            // 
            resources.ApplyResources(this.CHK_showconsole, "CHK_showconsole");
            this.CHK_showconsole.Name = "CHK_showconsole";
            this.CHK_showconsole.UseVisualStyleBackColor = true;
            this.CHK_showconsole.CheckedChanged += new System.EventHandler(this.CHK_showconsole_CheckedChanged);
            // 
            // BUT_updatecheck
            // 
            resources.ApplyResources(this.BUT_updatecheck, "BUT_updatecheck");
            this.BUT_updatecheck.Name = "BUT_updatecheck";
            this.BUT_updatecheck.UseVisualStyleBackColor = true;
            this.BUT_updatecheck.Click += new System.EventHandler(this.BUT_updatecheck_Click);
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // PIC_wizard
            // 
            resources.ApplyResources(this.PIC_wizard, "PIC_wizard");
            this.PIC_wizard.Image = global::MissionPlanner.Properties.Resources.wizardicon;
            this.PIC_wizard.Name = "PIC_wizard";
            this.PIC_wizard.TabStop = false;
            this.PIC_wizard.Click += new System.EventHandler(this.PIC_wizard_Click);
            // 
            // BUT_betaupdate
            // 
            resources.ApplyResources(this.BUT_betaupdate, "BUT_betaupdate");
            this.BUT_betaupdate.Name = "BUT_betaupdate";
            this.BUT_betaupdate.UseVisualStyleBackColor = true;
            this.BUT_betaupdate.Click += new System.EventHandler(this.BUT_betaupdate_Click);
            // 
            // Help
            // 
            
            this.Controls.Add(this.BUT_betaupdate);
            this.Controls.Add(this.PIC_wizard);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.CHK_showconsole);
            this.Controls.Add(this.BUT_updatecheck);
            this.Controls.Add(this.richTextBox1);
            resources.ApplyResources(this, "$this");
            this.Name = "Help";
            this.Load += new System.EventHandler(this.Help_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PIC_wizard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private Controls.MyButton BUT_updatecheck;
        private System.Windows.Forms.CheckBox CHK_showconsole;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.PictureBox PIC_wizard;
        private Controls.MyButton BUT_betaupdate;

    }
}
