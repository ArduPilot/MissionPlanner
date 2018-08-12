namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigHWOptFlow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigHWOptFlow));
            this.CHK_enableoptflow = new Controls.MavlinkCheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // CHK_enableoptflow
            // 
            resources.ApplyResources(this.CHK_enableoptflow, "CHK_enableoptflow");
            this.CHK_enableoptflow.Name = "CHK_enableoptflow";
            this.CHK_enableoptflow.OffValue = 0F;
            this.CHK_enableoptflow.OnValue = 1F;
            
            this.CHK_enableoptflow.ParamName = null;
            this.CHK_enableoptflow.UseVisualStyleBackColor = true;
            this.CHK_enableoptflow.CheckedChanged += new System.EventHandler(this.CHK_enableoptflow_CheckedChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.BackgroundImage = global::MissionPlanner.Properties.Resources.opticalflow;
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
            // ConfigHWOptFlow
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.CHK_enableoptflow);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.pictureBox2);
            this.Name = "ConfigHWOptFlow";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.MavlinkCheckBox CHK_enableoptflow;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}
