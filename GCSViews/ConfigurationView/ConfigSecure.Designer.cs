namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigSecure
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
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.txt_sha = new System.Windows.Forms.TextBox();
            this.but_firmware = new MissionPlanner.Controls.MyButton();
            this.but_dfu = new MissionPlanner.Controls.MyButton();
            this.but_bootloader = new MissionPlanner.Controls.MyButton();
            this.but_login = new MissionPlanner.Controls.MyButton();
            this.but_getsn = new MissionPlanner.Controls.MyButton();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(253, 134);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(193, 20);
            this.textBox1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(431, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "CubeOrange Only - DO NOT USE  UNLESS YOU UNDERSTAND THE CONSEQUENCE";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(25, 242);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(428, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 268);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "....";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.but_login);
            this.groupBox1.Controls.Add(this.but_getsn);
            this.groupBox1.Location = new System.Drawing.Point(25, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(143, 83);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Always";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.but_dfu);
            this.groupBox2.Controls.Add(this.but_bootloader);
            this.groupBox2.Location = new System.Drawing.Point(253, 45);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(142, 83);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "One Time";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.but_firmware);
            this.groupBox3.Location = new System.Drawing.Point(25, 134);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(141, 53);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Firmware";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(188, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Device SN";
            // 
            // timer1
            // 
            this.timer1.Interval = 4000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 226);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "....";
            // 
            // txt_sha
            // 
            this.txt_sha.Location = new System.Drawing.Point(253, 160);
            this.txt_sha.Multiline = true;
            this.txt_sha.Name = "txt_sha";
            this.txt_sha.ReadOnly = true;
            this.txt_sha.Size = new System.Drawing.Size(193, 43);
            this.txt_sha.TabIndex = 14;
            // 
            // but_firmware
            // 
            this.but_firmware.Enabled = false;
            this.but_firmware.Location = new System.Drawing.Point(6, 19);
            this.but_firmware.Name = "but_firmware";
            this.but_firmware.Size = new System.Drawing.Size(122, 23);
            this.but_firmware.TabIndex = 5;
            this.but_firmware.Text = "Get Firmware";
            this.but_firmware.UseVisualStyleBackColor = true;
            this.but_firmware.Click += new System.EventHandler(this.but_firmware_Click);
            // 
            // but_dfu
            // 
            this.but_dfu.Enabled = false;
            this.but_dfu.Location = new System.Drawing.Point(6, 19);
            this.but_dfu.Name = "but_dfu";
            this.but_dfu.Size = new System.Drawing.Size(122, 23);
            this.but_dfu.TabIndex = 4;
            this.but_dfu.Text = "Enter DFU Mode";
            this.but_dfu.UseVisualStyleBackColor = true;
            this.but_dfu.Click += new System.EventHandler(this.but_dfu_Click);
            // 
            // but_bootloader
            // 
            this.but_bootloader.Enabled = false;
            this.but_bootloader.Location = new System.Drawing.Point(6, 48);
            this.but_bootloader.Name = "but_bootloader";
            this.but_bootloader.Size = new System.Drawing.Size(122, 23);
            this.but_bootloader.TabIndex = 2;
            this.but_bootloader.Text = "Get Bootloader";
            this.but_bootloader.UseVisualStyleBackColor = true;
            this.but_bootloader.Click += new System.EventHandler(this.but_bootloader_Click);
            // 
            // but_login
            // 
            this.but_login.Location = new System.Drawing.Point(6, 19);
            this.but_login.Name = "but_login";
            this.but_login.Size = new System.Drawing.Size(122, 23);
            this.but_login.TabIndex = 0;
            this.but_login.Text = "Login";
            this.but_login.UseVisualStyleBackColor = true;
            this.but_login.Click += new System.EventHandler(this.but_login_Click);
            // 
            // but_getsn
            // 
            this.but_getsn.Enabled = false;
            this.but_getsn.Location = new System.Drawing.Point(6, 48);
            this.but_getsn.Name = "but_getsn";
            this.but_getsn.Size = new System.Drawing.Size(122, 23);
            this.but_getsn.TabIndex = 1;
            this.but_getsn.Text = "Enter Bootloader Mode";
            this.but_getsn.UseVisualStyleBackColor = true;
            this.but_getsn.Click += new System.EventHandler(this.but_getsn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(188, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Fw SHA";
            // 
            // ConfigSecure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt_sha);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "ConfigSecure";
            this.Size = new System.Drawing.Size(477, 285);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyButton but_login;
        private Controls.MyButton but_getsn;
        private Controls.MyButton but_bootloader;
        private System.Windows.Forms.TextBox textBox1;
        private Controls.MyButton but_dfu;
        private Controls.MyButton but_firmware;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_sha;
        private System.Windows.Forms.Label label5;
    }
}
