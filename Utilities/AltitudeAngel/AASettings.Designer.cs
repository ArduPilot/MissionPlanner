using MissionPlanner.Controls;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    internal partial class AASettings
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
            this.txt_FlightReportName = new System.Windows.Forms.TextBox();
            this.lbl_FlightReportName = new System.Windows.Forms.Label();
            this.txt_FlightReportDuration = new System.Windows.Forms.TextBox();
            this.lbl_FlightReportDuration = new System.Windows.Forms.Label();
            this.chk_FlightReportCommercial = new System.Windows.Forms.CheckBox();
            this.chk_FlightReportEnable = new System.Windows.Forms.CheckBox();
            this.lbl_FlightReportWhat = new System.Windows.Forms.LinkLabel();
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
            this.chk_airdata.Location = new System.Drawing.Point(13, 163);
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
            this.chk_grounddata.Location = new System.Drawing.Point(94, 163);
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
            this.chklb_layers.Location = new System.Drawing.Point(13, 187);
            this.chklb_layers.Name = "chklb_layers";
            this.chklb_layers.Size = new System.Drawing.Size(172, 186);
            this.chklb_layers.TabIndex = 7;
            this.chklb_layers.SelectedIndexChanged += new System.EventHandler(this.chklb_layers_SelectedIndexChanged);
            // 
            // txt_FlightReportName
            // 
            this.txt_FlightReportName.Location = new System.Drawing.Point(13, 90);
            this.txt_FlightReportName.Name = "txt_FlightReportName";
            this.txt_FlightReportName.Size = new System.Drawing.Size(172, 31);
            this.txt_FlightReportName.TabIndex = 3;
            this.txt_FlightReportName.TextChanged += new System.EventHandler(this.txt_FlightReportName_TextChanged);
            // 
            // lbl_FlightReportName
            // 
            this.lbl_FlightReportName.AutoSize = true;
            this.lbl_FlightReportName.Location = new System.Drawing.Point(8, 74);
            this.lbl_FlightReportName.Name = "lbl_FlightReportName";
            this.lbl_FlightReportName.Size = new System.Drawing.Size(197, 25);
            this.lbl_FlightReportName.TabIndex = 8;
            this.lbl_FlightReportName.Text = "Flight Report Name";
            // 
            // txt_FlightReportDuration
            // 
            this.txt_FlightReportDuration.Location = new System.Drawing.Point(13, 141);
            this.txt_FlightReportDuration.Name = "txt_FlightReportDuration";
            this.txt_FlightReportDuration.Size = new System.Drawing.Size(172, 31);
            this.txt_FlightReportDuration.TabIndex = 4;
            this.txt_FlightReportDuration.TextChanged += new System.EventHandler(this.txt_FlightReportDuration_TextChanged);
            // 
            // lbl_FlightReportDuration
            // 
            this.lbl_FlightReportDuration.AutoSize = true;
            this.lbl_FlightReportDuration.Location = new System.Drawing.Point(8, 125);
            this.lbl_FlightReportDuration.Name = "lbl_FlightReportDuration";
            this.lbl_FlightReportDuration.Size = new System.Drawing.Size(287, 25);
            this.lbl_FlightReportDuration.TabIndex = 9;
            this.lbl_FlightReportDuration.Text = "Flight Report Duration (mins)";
            // 
            // chk_FlightReportCommercial
            // 
            this.chk_FlightReportCommercial.AutoSize = true;
            this.chk_FlightReportCommercial.Location = new System.Drawing.Point(12, 107);
            this.chk_FlightReportCommercial.Name = "chk_FlightReportCommercial";
            this.chk_FlightReportCommercial.Size = new System.Drawing.Size(216, 29);
            this.chk_FlightReportCommercial.TabIndex = 3;
            this.chk_FlightReportCommercial.Text = "Commercial Flight";
            this.chk_FlightReportCommercial.UseVisualStyleBackColor = true;
            this.chk_FlightReportCommercial.CheckedChanged += new System.EventHandler(this.chk_FlightReportCommercial_CheckedChanged);
            // 
            // chk_FlightReportEnable
            // 
            this.chk_FlightReportEnable.AutoSize = true;
            this.chk_FlightReportEnable.Location = new System.Drawing.Point(12, 39);
            this.chk_FlightReportEnable.Name = "chk_FlightReportEnable";
            this.chk_FlightReportEnable.Size = new System.Drawing.Size(269, 29);
            this.chk_FlightReportEnable.TabIndex = 2;
            this.chk_FlightReportEnable.Text = "Enable Flight Reporting";
            this.chk_FlightReportEnable.UseVisualStyleBackColor = true;
            this.chk_FlightReportEnable.CheckedChanged += new System.EventHandler(this.chk_FlightReportEnable_CheckedChanged);
            // 
            // lbl_FlightReportWhat
            // 
            this.lbl_FlightReportWhat.AutoSize = true;
            this.lbl_FlightReportWhat.Location = new System.Drawing.Point(12, 57);
            this.lbl_FlightReportWhat.Name = "lbl_FlightReportWhat";
            this.lbl_FlightReportWhat.Size = new System.Drawing.Size(136, 25);
            this.lbl_FlightReportWhat.TabIndex = 10;
            this.lbl_FlightReportWhat.TabStop = true;
            this.lbl_FlightReportWhat.Text = "What is this?";
            this.lbl_FlightReportWhat.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_FlightReportWhat_LinkClicked);
            // 
            // AASettings
            // 
            
            this.ClientSize = new System.Drawing.Size(198, 379);
            this.Controls.Add(this.lbl_FlightReportWhat);
            this.Controls.Add(this.chk_FlightReportEnable);
            this.Controls.Add(this.chk_FlightReportCommercial);
            this.Controls.Add(this.lbl_FlightReportDuration);
            this.Controls.Add(this.txt_FlightReportDuration);
            this.Controls.Add(this.lbl_FlightReportName);
            this.Controls.Add(this.txt_FlightReportName);
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
        private System.Windows.Forms.TextBox txt_FlightReportName;
        private System.Windows.Forms.Label lbl_FlightReportName;
        private System.Windows.Forms.TextBox txt_FlightReportDuration;
        private System.Windows.Forms.Label lbl_FlightReportDuration;
        private System.Windows.Forms.CheckBox chk_FlightReportCommercial;
        private System.Windows.Forms.CheckBox chk_FlightReportEnable;
        private System.Windows.Forms.LinkLabel lbl_FlightReportWhat;
    }
}