namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigCubeID
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
            this.but_upfw = new MissionPlanner.Controls.MyButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mavnumtimeout = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavpasscombo = new MissionPlanner.Controls.MavlinkComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mavnumtimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // but_upfw
            // 
            this.but_upfw.Location = new System.Drawing.Point(141, 218);
            this.but_upfw.Name = "but_upfw";
            this.but_upfw.Size = new System.Drawing.Size(120, 23);
            this.but_upfw.TabIndex = 0;
            this.but_upfw.Text = "Upload Firmware";
            this.but_upfw.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_upfw.UseVisualStyleBackColor = true;
            this.but_upfw.Click += new System.EventHandler(this.but_upfw_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(346, 52);
            this.label1.TabIndex = 1;
            this.label1.Text = "To use this feature\r\nPlease enable serial passthough to the port the CubeID is co" +
    "nnected to.\r\nSet param SERIAL_PASSTIMO to 0\r\nSet param SERIAL_PASS2 to the telem" +
    " port\r\n";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "SERIAL_PASS2 ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "SERIAL_PASSTIMO";
            // 
            // mavnumtimeout
            // 
            this.mavnumtimeout.Enabled = false;
            this.mavnumtimeout.Location = new System.Drawing.Point(141, 104);
            this.mavnumtimeout.Max = 1F;
            this.mavnumtimeout.Min = 0F;
            this.mavnumtimeout.Name = "mavnumtimeout";
            this.mavnumtimeout.ParamName = null;
            this.mavnumtimeout.Size = new System.Drawing.Size(120, 20);
            this.mavnumtimeout.TabIndex = 3;
            // 
            // mavpasscombo
            // 
            this.mavpasscombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mavpasscombo.Enabled = false;
            this.mavpasscombo.FormattingEnabled = true;
            this.mavpasscombo.Location = new System.Drawing.Point(141, 130);
            this.mavpasscombo.Name = "mavpasscombo";
            this.mavpasscombo.ParamName = null;
            this.mavpasscombo.Size = new System.Drawing.Size(120, 21);
            this.mavpasscombo.SubControl = null;
            this.mavpasscombo.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 180);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(314, 26);
            this.label4.TabIndex = 6;
            this.label4.Text = "Select the ODID device from the dropdown in the top right corner\r\nthen click the " +
    "Update Firmware button";
            // 
            // ConfigCubeID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mavnumtimeout);
            this.Controls.Add(this.mavpasscombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.but_upfw);
            this.Controls.Add(this.label4);
            this.Name = "ConfigCubeID";
            this.Size = new System.Drawing.Size(525, 340);
            ((System.ComponentModel.ISupportInitialize)(this.mavnumtimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyButton but_upfw;
        private System.Windows.Forms.Label label1;
        private Controls.MavlinkComboBox mavpasscombo;
        private Controls.MavlinkNumericUpDown mavnumtimeout;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}
