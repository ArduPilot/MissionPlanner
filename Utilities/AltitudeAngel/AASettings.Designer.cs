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
            this.chk_airdata.Location = new System.Drawing.Point(13, 43);
            this.chk_airdata.Name = "chk_airdata";
            this.chk_airdata.Size = new System.Drawing.Size(64, 17);
            this.chk_airdata.TabIndex = 2;
            this.chk_airdata.Text = "Air Data";
            this.chk_airdata.UseVisualStyleBackColor = true;
            this.chk_airdata.CheckedChanged += new System.EventHandler(this.chk_airdata_CheckedChanged);
            // 
            // chk_grounddata
            // 
            this.chk_grounddata.AutoSize = true;
            this.chk_grounddata.Location = new System.Drawing.Point(94, 42);
            this.chk_grounddata.Name = "chk_grounddata";
            this.chk_grounddata.Size = new System.Drawing.Size(87, 17);
            this.chk_grounddata.TabIndex = 3;
            this.chk_grounddata.Text = "Ground Data";
            this.chk_grounddata.UseVisualStyleBackColor = true;
            this.chk_grounddata.CheckedChanged += new System.EventHandler(this.chk_grounddata_CheckedChanged);
            // 
            // chklb_layers
            // 
            this.chklb_layers.CheckOnClick = true;
            this.chklb_layers.FormattingEnabled = true;
            this.chklb_layers.Location = new System.Drawing.Point(12, 65);
            this.chklb_layers.Name = "chklb_layers";
            this.chklb_layers.Size = new System.Drawing.Size(159, 199);
            this.chklb_layers.TabIndex = 4;
            this.chklb_layers.SelectedIndexChanged += new System.EventHandler(this.chklb_layers_SelectedIndexChanged);
            // 
            // AASettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(183, 274);
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
    }
}