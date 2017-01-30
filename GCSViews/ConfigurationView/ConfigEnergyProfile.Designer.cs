namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigEnergyProfile
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
            this.label2 = new System.Windows.Forms.Label();
            this.CB_EnableEnergyProfile = new System.Windows.Forms.CheckBox();
            this.Btn_SaveChanges = new MissionPlanner.Controls.MyButton();
            this.P_EnergyProfileConfiguration = new System.Windows.Forms.Panel();
            this.P_EnergyProfileConfiguration.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 13);
            this.label2.TabIndex = 37;
            this.label2.Text = "Approximation of Energyconsumption";
            // 
            // CB_EnableEnergyProfile
            // 
            this.CB_EnableEnergyProfile.AutoSize = true;
            this.CB_EnableEnergyProfile.Location = new System.Drawing.Point(16, 11);
            this.CB_EnableEnergyProfile.Name = "CB_EnableEnergyProfile";
            this.CB_EnableEnergyProfile.Size = new System.Drawing.Size(124, 17);
            this.CB_EnableEnergyProfile.TabIndex = 38;
            this.CB_EnableEnergyProfile.Text = "Enable EnergyProfile";
            this.CB_EnableEnergyProfile.UseVisualStyleBackColor = true;
            this.CB_EnableEnergyProfile.CheckStateChanged += new System.EventHandler(this.CB_EnableEnergyProfile_CheckStateChanged);
            // 
            // Btn_SaveChanges
            // 
            this.Btn_SaveChanges.Location = new System.Drawing.Point(20, 277);
            this.Btn_SaveChanges.Name = "Btn_SaveChanges";
            this.Btn_SaveChanges.Size = new System.Drawing.Size(68, 32);
            this.Btn_SaveChanges.TabIndex = 14;
            this.Btn_SaveChanges.Text = "Save Changes";
            this.Btn_SaveChanges.UseVisualStyleBackColor = true;
            this.Btn_SaveChanges.Click += new System.EventHandler(this.Btn_SaveChanges_Click);
            // 
            // P_EnergyProfileConfiguration
            // 
            this.P_EnergyProfileConfiguration.Controls.Add(this.Btn_SaveChanges);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label2);
            this.P_EnergyProfileConfiguration.Enabled = false;
            this.P_EnergyProfileConfiguration.Location = new System.Drawing.Point(16, 34);
            this.P_EnergyProfileConfiguration.Name = "P_EnergyProfileConfiguration";
            this.P_EnergyProfileConfiguration.Size = new System.Drawing.Size(612, 390);
            this.P_EnergyProfileConfiguration.TabIndex = 40;
            // 
            // ConfigEnergyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.P_EnergyProfileConfiguration);
            this.Controls.Add(this.CB_EnableEnergyProfile);
            this.Name = "ConfigEnergyProfile";
            this.Size = new System.Drawing.Size(837, 458);
            this.P_EnergyProfileConfiguration.ResumeLayout(false);
            this.P_EnergyProfileConfiguration.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Controls.MyButton Btn_SaveChanges;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox CB_EnableEnergyProfile;
        private System.Windows.Forms.Panel P_EnergyProfileConfiguration;
    }
}
