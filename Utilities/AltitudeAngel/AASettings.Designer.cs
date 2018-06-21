using MissionPlanner.Controls;

namespace MissionPlanner.Utilities.AltitudeAngel
{


    partial class AASettings
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.but_enable = new MissionPlanner.Controls.MyButton();
            this.but_disable = new MissionPlanner.Controls.MyButton();
            this.chk_airdata = new System.Windows.Forms.CheckBox();
            this.chk_grounddata = new System.Windows.Forms.CheckBox();
            this.chklb_layers = new System.Windows.Forms.CheckedListBox();
            this.txt_FlightPlanName = new System.Windows.Forms.TextBox();
            this.lbl_FlightPlanName = new System.Windows.Forms.Label();
            this.txt_FlightPlanDuration = new System.Windows.Forms.TextBox();
            this.lbl_FlightPlanDuration = new System.Windows.Forms.Label();
            this.chk_FlightPlanCommercial = new System.Windows.Forms.CheckBox();
            this.chk_FlightPlanEnable = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // but_enable
            // 
            this.but_enable.Location = new System.Drawing.Point(13, 13);
            this.but_enable.Name = "but_enable";
            this.but_enable.Size = new System.Drawing.Size(75, 23);
            this.but_enable.TabIndex = 0;
            this.but_enable.Text = "Signin";
            this.but_enable.UseVisualStyleBackColor = true;
            this.but_enable.Click += new System.EventHandler(this.but_enable_Click);
            // 
            // but_disable
            // 
            this.but_disable.Location = new System.Drawing.Point(94, 13);
            this.but_disable.Name = "but_disable";
            this.but_disable.Size = new System.Drawing.Size(75, 23);
            this.but_disable.TabIndex = 1;
            this.but_disable.Text = "Disable";
            this.but_disable.UseVisualStyleBackColor = true;
            this.but_disable.Click += new System.EventHandler(this.but_disable_Click);
            // 
            // chk_airdata
            // 
            this.chk_airdata.AutoSize = true;
            this.chk_airdata.Location = new System.Drawing.Point(13, 148);
            this.chk_airdata.Name = "chk_airdata";
            this.chk_airdata.Size = new System.Drawing.Size(121, 29);
            this.chk_airdata.TabIndex = 5;
            this.chk_airdata.Text = "Air Data";
            this.chk_airdata.UseVisualStyleBackColor = true;
            this.chk_airdata.CheckedChanged += new System.EventHandler(this.chk_airdata_CheckedChanged);
            // 
            // chk_grounddata
            // 
            this.chk_grounddata.AutoSize = true;
            this.chk_grounddata.Location = new System.Drawing.Point(94, 148);
            this.chk_grounddata.Name = "chk_grounddata";
            this.chk_grounddata.Size = new System.Drawing.Size(166, 29);
            this.chk_grounddata.TabIndex = 6;
            this.chk_grounddata.Text = "Ground Data";
            this.chk_grounddata.UseVisualStyleBackColor = true;
            this.chk_grounddata.CheckedChanged += new System.EventHandler(this.chk_grounddata_CheckedChanged);
            // 
            // chklb_layers
            // 
            this.chklb_layers.CheckOnClick = true;
            this.chklb_layers.FormattingEnabled = true;
            this.chklb_layers.Location = new System.Drawing.Point(13, 172);
            this.chklb_layers.Name = "chklb_layers";
            this.chklb_layers.Size = new System.Drawing.Size(172, 186);
            this.chklb_layers.TabIndex = 7;
            this.chklb_layers.SelectedIndexChanged += new System.EventHandler(this.chklb_layers_SelectedIndexChanged);
            // 
            // txt_FlightPlanName
            // 
            this.txt_FlightPlanName.Location = new System.Drawing.Point(13, 75);
            this.txt_FlightPlanName.Name = "txt_FlightPlanName";
            this.txt_FlightPlanName.Size = new System.Drawing.Size(172, 31);
            this.txt_FlightPlanName.TabIndex = 3;
            this.txt_FlightPlanName.TextChanged += new System.EventHandler(this.txt_FlightPlanName_TextChanged);
            // 
            // lbl_FlightPlanName
            // 
            this.lbl_FlightPlanName.AutoSize = true;
            this.lbl_FlightPlanName.Location = new System.Drawing.Point(8, 59);
            this.lbl_FlightPlanName.Name = "lbl_FlightPlanName";
            this.lbl_FlightPlanName.Size = new System.Drawing.Size(176, 25);
            this.lbl_FlightPlanName.TabIndex = 8;
            this.lbl_FlightPlanName.Text = "Flight Plan Name";
            // 
            // txt_FlightPlanDuration
            // 
            this.txt_FlightPlanDuration.Location = new System.Drawing.Point(13, 126);
            this.txt_FlightPlanDuration.Name = "txt_FlightPlanDuration";
            this.txt_FlightPlanDuration.Size = new System.Drawing.Size(172, 31);
            this.txt_FlightPlanDuration.TabIndex = 4;
            this.txt_FlightPlanDuration.TextChanged += new System.EventHandler(this.txt_FlightPlanDuration_TextChanged);
            // 
            // lbl_FlightPlanDuration
            // 
            this.lbl_FlightPlanDuration.AutoSize = true;
            this.lbl_FlightPlanDuration.Location = new System.Drawing.Point(8, 110);
            this.lbl_FlightPlanDuration.Name = "lbl_FlightPlanDuration";
            this.lbl_FlightPlanDuration.Size = new System.Drawing.Size(266, 25);
            this.lbl_FlightPlanDuration.TabIndex = 9;
            this.lbl_FlightPlanDuration.Text = "Flight Plan Duration (mins)";
            // 
            // chk_FlightPlanCommercial
            // 
            this.chk_FlightPlanCommercial.AutoSize = true;
            this.chk_FlightPlanCommercial.Location = new System.Drawing.Point(12, 92);
            this.chk_FlightPlanCommercial.Name = "chk_FlightPlanCommercial";
            this.chk_FlightPlanCommercial.Size = new System.Drawing.Size(216, 29);
            this.chk_FlightPlanCommercial.TabIndex = 3;
            this.chk_FlightPlanCommercial.Text = "Commercial Flight";
            this.chk_FlightPlanCommercial.UseVisualStyleBackColor = true;
            this.chk_FlightPlanCommercial.CheckedChanged += new System.EventHandler(this.chk_FlightPlanCommercial_CheckedChanged);
            // 
            // chk_FlightPlanEnable
            // 
            this.chk_FlightPlanEnable.AutoSize = true;
            this.chk_FlightPlanEnable.Location = new System.Drawing.Point(12, 39);
            this.chk_FlightPlanEnable.Name = "chk_FlightPlanEnable";
            this.chk_FlightPlanEnable.Size = new System.Drawing.Size(219, 29);
            this.chk_FlightPlanEnable.TabIndex = 2;
            this.chk_FlightPlanEnable.Text = "Enable Flight Plan";
            this.chk_FlightPlanEnable.UseVisualStyleBackColor = true;
            this.chk_FlightPlanEnable.CheckedChanged += new System.EventHandler(this.chk_FlightPlanEnable_CheckedChanged);
            // 
            // AASettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(198, 365);
            this.Controls.Add(this.chk_FlightPlanEnable);
            this.Controls.Add(this.chk_FlightPlanCommercial);
            this.Controls.Add(this.lbl_FlightPlanDuration);
            this.Controls.Add(this.txt_FlightPlanDuration);
            this.Controls.Add(this.lbl_FlightPlanName);
            this.Controls.Add(this.txt_FlightPlanName);
            this.Controls.Add(this.chklb_layers);
            this.Controls.Add(this.chk_grounddata);
            this.Controls.Add(this.chk_airdata);
            this.Controls.Add(this.but_disable);
            this.Controls.Add(this.but_enable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AASettings";
            this.Text = "Altitude Angel - Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chk_airdata;
        private System.Windows.Forms.CheckBox chk_grounddata;
        private MyButton but_enable;
        private MyButton but_disable;
        private System.Windows.Forms.CheckedListBox chklb_layers;
        private System.Windows.Forms.TextBox txt_FlightPlanName;
        private System.Windows.Forms.Label lbl_FlightPlanName;
        private System.Windows.Forms.TextBox txt_FlightPlanDuration;
        private System.Windows.Forms.Label lbl_FlightPlanDuration;
        private System.Windows.Forms.CheckBox chk_FlightPlanCommercial;
        private System.Windows.Forms.CheckBox chk_FlightPlanEnable;
    }
}