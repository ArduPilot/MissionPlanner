namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigSecureAP
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.but_bootloader = new MissionPlanner.Controls.MyButton();
            this.but_firmware = new MissionPlanner.Controls.MyButton();
            this.but_privkey = new MissionPlanner.Controls.MyButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txt_fwapj = new System.Windows.Forms.TextBox();
            this.txt_bl = new System.Windows.Forms.TextBox();
            this.txt_pubkey = new System.Windows.Forms.TextBox();
            this.but_generatekey = new MissionPlanner.Controls.MyButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Supported Files|*.bin;*.pem;*.apj";
            // 
            // but_bootloader
            // 
            this.but_bootloader.Location = new System.Drawing.Point(6, 48);
            this.but_bootloader.Name = "but_bootloader";
            this.but_bootloader.Size = new System.Drawing.Size(75, 23);
            this.but_bootloader.TabIndex = 0;
            this.but_bootloader.Text = "BootLoader";
            this.but_bootloader.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_bootloader.UseVisualStyleBackColor = true;
            this.but_bootloader.Click += new System.EventHandler(this.but_bootloader_Click);
            // 
            // but_firmware
            // 
            this.but_firmware.Location = new System.Drawing.Point(6, 77);
            this.but_firmware.Name = "but_firmware";
            this.but_firmware.Size = new System.Drawing.Size(75, 23);
            this.but_firmware.TabIndex = 1;
            this.but_firmware.Text = "Firmware";
            this.but_firmware.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_firmware.UseVisualStyleBackColor = true;
            this.but_firmware.Click += new System.EventHandler(this.but_firmware_Click);
            // 
            // but_privkey
            // 
            this.but_privkey.Location = new System.Drawing.Point(6, 19);
            this.but_privkey.Name = "but_privkey";
            this.but_privkey.Size = new System.Drawing.Size(75, 23);
            this.but_privkey.TabIndex = 2;
            this.but_privkey.Text = "Private Key";
            this.but_privkey.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_privkey.UseVisualStyleBackColor = true;
            this.but_privkey.Click += new System.EventHandler(this.but_privkey_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txt_fwapj);
            this.groupBox4.Controls.Add(this.txt_bl);
            this.groupBox4.Controls.Add(this.txt_pubkey);
            this.groupBox4.Controls.Add(this.but_privkey);
            this.groupBox4.Controls.Add(this.but_bootloader);
            this.groupBox4.Controls.Add(this.but_firmware);
            this.groupBox4.Location = new System.Drawing.Point(59, 64);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(335, 120);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Files";
            // 
            // txt_fwapj
            // 
            this.txt_fwapj.Location = new System.Drawing.Point(87, 80);
            this.txt_fwapj.Name = "txt_fwapj";
            this.txt_fwapj.Size = new System.Drawing.Size(230, 20);
            this.txt_fwapj.TabIndex = 5;
            // 
            // txt_bl
            // 
            this.txt_bl.Location = new System.Drawing.Point(88, 51);
            this.txt_bl.Name = "txt_bl";
            this.txt_bl.Size = new System.Drawing.Size(230, 20);
            this.txt_bl.TabIndex = 4;
            // 
            // txt_pubkey
            // 
            this.txt_pubkey.Location = new System.Drawing.Point(88, 21);
            this.txt_pubkey.Name = "txt_pubkey";
            this.txt_pubkey.Size = new System.Drawing.Size(230, 20);
            this.txt_pubkey.TabIndex = 3;
            // 
            // but_generatekey
            // 
            this.but_generatekey.Location = new System.Drawing.Point(6, 19);
            this.but_generatekey.Name = "but_generatekey";
            this.but_generatekey.Size = new System.Drawing.Size(75, 23);
            this.but_generatekey.TabIndex = 6;
            this.but_generatekey.Text = "Generate Key";
            this.but_generatekey.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_generatekey.UseVisualStyleBackColor = true;
            this.but_generatekey.Click += new System.EventHandler(this.but_generatekey_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.but_generatekey);
            this.groupBox5.Location = new System.Drawing.Point(59, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(335, 55);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Do Only Once";
            // 
            // ConfigSecureAP
            // 
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Name = "ConfigSecureAP";
            this.Size = new System.Drawing.Size(511, 224);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.TextBox textBox1;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private Controls.MyButton but_bootloader;
        private Controls.MyButton but_firmware;
        private Controls.MyButton but_privkey;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txt_fwapj;
        private System.Windows.Forms.TextBox txt_bl;
        private System.Windows.Forms.TextBox txt_pubkey;
        private Controls.MyButton but_generatekey;
        private System.Windows.Forms.GroupBox groupBox5;
    }
}
