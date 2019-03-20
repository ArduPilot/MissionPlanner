namespace MissionPlanner
{
    partial class GenOTP
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
            this.txtboardsn = new System.Windows.Forms.TextBox();
            this.myLabel3 = new MissionPlanner.Controls.MyLabel();
            this.fileBrowseOtpbin = new MissionPlanner.Controls.FileBrowse();
            this.BUT_makeotp = new MissionPlanner.Controls.MyButton();
            this.myLabel2 = new MissionPlanner.Controls.MyLabel();
            this.myLabel1 = new MissionPlanner.Controls.MyLabel();
            this.fileBrowsePrivateKey = new MissionPlanner.Controls.FileBrowse();
            this.myLabel4 = new MissionPlanner.Controls.MyLabel();
            this.txt_vid = new System.Windows.Forms.TextBox();
            this.myLabel5 = new MissionPlanner.Controls.MyLabel();
            this.txt_pid = new System.Windows.Forms.TextBox();
            this.myLabel6 = new MissionPlanner.Controls.MyLabel();
            this.txt_id = new System.Windows.Forms.TextBox();
            this.BUT_sn = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // txtboardsn
            // 
            this.txtboardsn.Location = new System.Drawing.Point(126, 60);
            this.txtboardsn.Name = "txtboardsn";
            this.txtboardsn.Size = new System.Drawing.Size(232, 20);
            this.txtboardsn.TabIndex = 2;
            this.txtboardsn.Text = "003D001F3231470C34303736";
            // 
            // myLabel3
            // 
            this.myLabel3.Location = new System.Drawing.Point(12, 109);
            this.myLabel3.Name = "myLabel3";
            this.myLabel3.resize = false;
            this.myLabel3.Size = new System.Drawing.Size(108, 20);
            this.myLabel3.TabIndex = 6;
            this.myLabel3.Text = "OTP File Output";
            // 
            // fileBrowseOtpbin
            // 
            this.fileBrowseOtpbin.filename = "otp.bin";
            this.fileBrowseOtpbin.Filter = "*.bin|*.bin";
            this.fileBrowseOtpbin.Location = new System.Drawing.Point(126, 109);
            this.fileBrowseOtpbin.Name = "fileBrowseOtpbin";
            this.fileBrowseOtpbin.OpenFile = false;
            this.fileBrowseOtpbin.Size = new System.Drawing.Size(436, 20);
            this.fileBrowseOtpbin.TabIndex = 5;
            // 
            // BUT_makeotp
            // 
            this.BUT_makeotp.Location = new System.Drawing.Point(254, 280);
            this.BUT_makeotp.Name = "BUT_makeotp";
            this.BUT_makeotp.Size = new System.Drawing.Size(75, 23);
            this.BUT_makeotp.TabIndex = 4;
            this.BUT_makeotp.Text = "Make OTP";
            this.BUT_makeotp.UseVisualStyleBackColor = true;
            this.BUT_makeotp.Click += new System.EventHandler(this.BUT_makeotp_Click);
            // 
            // myLabel2
            // 
            this.myLabel2.Location = new System.Drawing.Point(12, 60);
            this.myLabel2.Name = "myLabel2";
            this.myLabel2.resize = false;
            this.myLabel2.Size = new System.Drawing.Size(108, 20);
            this.myLabel2.TabIndex = 3;
            this.myLabel2.Text = "Board SN";
            // 
            // myLabel1
            // 
            this.myLabel1.Location = new System.Drawing.Point(12, 12);
            this.myLabel1.Name = "myLabel1";
            this.myLabel1.resize = false;
            this.myLabel1.Size = new System.Drawing.Size(108, 20);
            this.myLabel1.TabIndex = 1;
            this.myLabel1.Text = "Private Key Input";
            // 
            // fileBrowsePrivateKey
            // 
            this.fileBrowsePrivateKey.filename = "private.txt";
            this.fileBrowsePrivateKey.Filter = "*.txt|*.txt";
            this.fileBrowsePrivateKey.Location = new System.Drawing.Point(126, 12);
            this.fileBrowsePrivateKey.Name = "fileBrowsePrivateKey";
            this.fileBrowsePrivateKey.Size = new System.Drawing.Size(436, 20);
            this.fileBrowsePrivateKey.TabIndex = 0;
            // 
            // myLabel4
            // 
            this.myLabel4.Location = new System.Drawing.Point(12, 149);
            this.myLabel4.Name = "myLabel4";
            this.myLabel4.resize = false;
            this.myLabel4.Size = new System.Drawing.Size(108, 20);
            this.myLabel4.TabIndex = 8;
            this.myLabel4.Text = "Vendor ID (hex)";
            this.myLabel4.Visible = false;
            // 
            // txt_vid
            // 
            this.txt_vid.Location = new System.Drawing.Point(126, 149);
            this.txt_vid.Name = "txt_vid";
            this.txt_vid.Size = new System.Drawing.Size(232, 20);
            this.txt_vid.TabIndex = 7;
            this.txt_vid.Text = "26AC";
            this.txt_vid.Visible = false;
            // 
            // myLabel5
            // 
            this.myLabel5.Location = new System.Drawing.Point(12, 188);
            this.myLabel5.Name = "myLabel5";
            this.myLabel5.resize = false;
            this.myLabel5.Size = new System.Drawing.Size(108, 20);
            this.myLabel5.TabIndex = 10;
            this.myLabel5.Text = "Product ID (hex)";
            this.myLabel5.Visible = false;
            // 
            // txt_pid
            // 
            this.txt_pid.Location = new System.Drawing.Point(126, 188);
            this.txt_pid.Name = "txt_pid";
            this.txt_pid.Size = new System.Drawing.Size(232, 20);
            this.txt_pid.TabIndex = 9;
            this.txt_pid.Text = "10";
            this.txt_pid.Visible = false;
            // 
            // myLabel6
            // 
            this.myLabel6.Location = new System.Drawing.Point(12, 226);
            this.myLabel6.Name = "myLabel6";
            this.myLabel6.resize = false;
            this.myLabel6.Size = new System.Drawing.Size(108, 20);
            this.myLabel6.TabIndex = 12;
            this.myLabel6.Text = "Type (hex)";
            this.myLabel6.Visible = false;
            // 
            // txt_id
            // 
            this.txt_id.Location = new System.Drawing.Point(126, 226);
            this.txt_id.Name = "txt_id";
            this.txt_id.Size = new System.Drawing.Size(232, 20);
            this.txt_id.TabIndex = 11;
            this.txt_id.Text = "0";
            this.txt_id.Visible = false;
            // 
            // BUT_sn
            // 
            this.BUT_sn.Location = new System.Drawing.Point(487, 57);
            this.BUT_sn.Name = "BUT_sn";
            this.BUT_sn.Size = new System.Drawing.Size(75, 23);
            this.BUT_sn.TabIndex = 13;
            this.BUT_sn.Text = "Get SN";
            this.BUT_sn.UseVisualStyleBackColor = true;
            this.BUT_sn.Click += new System.EventHandler(this.BUT_sn_Click);
            // 
            // GenOTP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            
            this.ClientSize = new System.Drawing.Size(574, 315);
            this.Controls.Add(this.BUT_sn);
            this.Controls.Add(this.myLabel6);
            this.Controls.Add(this.txt_id);
            this.Controls.Add(this.myLabel5);
            this.Controls.Add(this.txt_pid);
            this.Controls.Add(this.myLabel4);
            this.Controls.Add(this.txt_vid);
            this.Controls.Add(this.myLabel3);
            this.Controls.Add(this.fileBrowseOtpbin);
            this.Controls.Add(this.BUT_makeotp);
            this.Controls.Add(this.myLabel2);
            this.Controls.Add(this.txtboardsn);
            this.Controls.Add(this.myLabel1);
            this.Controls.Add(this.fileBrowsePrivateKey);
            this.Name = "GenOTP";
            this.Text = "GenOTP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.FileBrowse fileBrowsePrivateKey;
        private Controls.MyLabel myLabel1;
        private System.Windows.Forms.TextBox txtboardsn;
        private Controls.MyLabel myLabel2;
        private Controls.MyButton BUT_makeotp;
        private Controls.FileBrowse fileBrowseOtpbin;
        private Controls.MyLabel myLabel3;
        private Controls.MyLabel myLabel4;
        private System.Windows.Forms.TextBox txt_vid;
        private Controls.MyLabel myLabel5;
        private System.Windows.Forms.TextBox txt_pid;
        private Controls.MyLabel myLabel6;
        private System.Windows.Forms.TextBox txt_id;
        private Controls.MyButton BUT_sn;
    }
}