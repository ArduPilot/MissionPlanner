namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigHWPX4Flow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigHWPX4Flow));
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.imagebox = new System.Windows.Forms.PictureBox();
            this.but_focusmode = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imagebox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.BackgroundImage = global::MissionPlanner.Properties.Resources.PX4FLOW1;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // imagebox
            // 
            resources.ApplyResources(this.imagebox, "imagebox");
            this.imagebox.Name = "imagebox";
            this.imagebox.TabStop = false;
            // 
            // but_focusmode
            // 
            resources.ApplyResources(this.but_focusmode, "but_focusmode");
            this.but_focusmode.Name = "but_focusmode";
            this.but_focusmode.UseVisualStyleBackColor = true;
            this.but_focusmode.Click += new System.EventHandler(this.but_focusmode_Click);
            // 
            // ConfigHWPX4Flow
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.but_focusmode);
            this.Controls.Add(this.imagebox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.pictureBox2);
            this.Name = "ConfigHWPX4Flow";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imagebox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.PictureBox imagebox;
        private Controls.MyButton but_focusmode;
    }
}
