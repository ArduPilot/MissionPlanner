
namespace Dowding
{
    partial class DowdingUI
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
            this.chk_enable = new System.Windows.Forms.CheckBox();
            this.myLabel1 = new System.Windows.Forms.Label();
            this.txt_username = new System.Windows.Forms.TextBox();
            this.myLabel3 = new System.Windows.Forms.Label();
            this.txt_password = new System.Windows.Forms.TextBox();
            this.myLabel2 = new System.Windows.Forms.Label();
            this.cmb_server = new System.Windows.Forms.ComboBox();
            this.but_verify = new MissionPlanner.Controls.MyButton();
            this.label1 = new System.Windows.Forms.Label();
            this.but_token = new MissionPlanner.Controls.MyButton();
            this.but_start = new MissionPlanner.Controls.MyButton();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbl_header = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.CMB_baudrate = new System.Windows.Forms.ComboBox();
            this.BUT_connect = new MissionPlanner.Controls.MyButton();
            this.CMB_serialport = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chk_enable
            // 
            this.chk_enable.AutoSize = true;
            this.chk_enable.Location = new System.Drawing.Point(226, 23);
            this.chk_enable.Name = "chk_enable";
            this.chk_enable.Size = new System.Drawing.Size(96, 17);
            this.chk_enable.TabIndex = 8;
            this.chk_enable.Text = "Enable at Start";
            this.chk_enable.UseVisualStyleBackColor = true;
            this.chk_enable.CheckedChanged += new System.EventHandler(this.chk_enable_CheckedChanged);
            // 
            // myLabel1
            // 
            this.myLabel1.Location = new System.Drawing.Point(3, 0);
            this.myLabel1.Name = "myLabel1";
            this.myLabel1.Size = new System.Drawing.Size(75, 23);
            this.myLabel1.TabIndex = 0;
            this.myLabel1.Text = "Username";
            // 
            // txt_username
            // 
            this.txt_username.Location = new System.Drawing.Point(84, 3);
            this.txt_username.Name = "txt_username";
            this.txt_username.Size = new System.Drawing.Size(100, 20);
            this.txt_username.TabIndex = 3;
            // 
            // myLabel3
            // 
            this.myLabel3.Location = new System.Drawing.Point(3, 26);
            this.myLabel3.Name = "myLabel3";
            this.myLabel3.Size = new System.Drawing.Size(75, 23);
            this.myLabel3.TabIndex = 2;
            this.myLabel3.Text = "Password";
            // 
            // txt_password
            // 
            this.txt_password.Location = new System.Drawing.Point(84, 29);
            this.txt_password.Name = "txt_password";
            this.txt_password.PasswordChar = '*';
            this.txt_password.Size = new System.Drawing.Size(100, 20);
            this.txt_password.TabIndex = 4;
            this.txt_password.UseSystemPasswordChar = true;
            // 
            // myLabel2
            // 
            this.myLabel2.Location = new System.Drawing.Point(4, 23);
            this.myLabel2.Name = "myLabel2";
            this.myLabel2.Size = new System.Drawing.Size(75, 23);
            this.myLabel2.TabIndex = 1;
            this.myLabel2.Text = "Server";
            // 
            // cmb_server
            // 
            this.cmb_server.FormattingEnabled = true;
            this.cmb_server.Location = new System.Drawing.Point(63, 23);
            this.cmb_server.Name = "cmb_server";
            this.cmb_server.Size = new System.Drawing.Size(100, 21);
            this.cmb_server.TabIndex = 5;
            // 
            // but_verify
            // 
            this.but_verify.Location = new System.Drawing.Point(3, 55);
            this.but_verify.Name = "but_verify";
            this.but_verify.Size = new System.Drawing.Size(181, 23);
            this.but_verify.TabIndex = 6;
            this.but_verify.Text = "Verify";
            this.but_verify.UseVisualStyleBackColor = true;
            this.but_verify.Click += new System.EventHandler(this.but_verify_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(197, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 23);
            this.label1.TabIndex = 10;
            this.label1.Text = "Or";
            // 
            // but_token
            // 
            this.but_token.Location = new System.Drawing.Point(3, 3);
            this.but_token.Name = "but_token";
            this.but_token.Size = new System.Drawing.Size(181, 23);
            this.but_token.TabIndex = 9;
            this.but_token.Text = "Enter Token";
            this.but_token.UseVisualStyleBackColor = true;
            this.but_token.Click += new System.EventHandler(this.but_token_Click);
            // 
            // but_start
            // 
            this.but_start.Location = new System.Drawing.Point(7, 158);
            this.but_start.Name = "but_start";
            this.but_start.Size = new System.Drawing.Size(407, 23);
            this.but_start.TabIndex = 11;
            this.but_start.Text = "Start";
            this.but_start.UseVisualStyleBackColor = true;
            this.but_start.Click += new System.EventHandler(this.but_start_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.myLabel1);
            this.flowLayoutPanel2.Controls.Add(this.txt_username);
            this.flowLayoutPanel2.Controls.Add(this.myLabel3);
            this.flowLayoutPanel2.Controls.Add(this.txt_password);
            this.flowLayoutPanel2.Controls.Add(this.but_verify);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(7, 67);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(188, 85);
            this.flowLayoutPanel2.TabIndex = 2;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.but_token);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(226, 67);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(188, 33);
            this.flowLayoutPanel3.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.but_start);
            this.panel1.Controls.Add(this.chk_enable);
            this.panel1.Controls.Add(this.lbl_header);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.flowLayoutPanel2);
            this.panel1.Controls.Add(this.flowLayoutPanel3);
            this.panel1.Controls.Add(this.cmb_server);
            this.panel1.Controls.Add(this.myLabel2);
            this.panel1.Location = new System.Drawing.Point(30, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(420, 189);
            this.panel1.TabIndex = 2;
            // 
            // lbl_header
            // 
            this.lbl_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_header.Location = new System.Drawing.Point(3, 0);
            this.lbl_header.Name = "lbl_header";
            this.lbl_header.Size = new System.Drawing.Size(95, 23);
            this.lbl_header.TabIndex = 7;
            this.lbl_header.Text = "Dowding";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.CMB_baudrate);
            this.panel2.Controls.Add(this.BUT_connect);
            this.panel2.Controls.Add(this.CMB_serialport);
            this.panel2.Location = new System.Drawing.Point(30, 220);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(420, 88);
            this.panel2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 23);
            this.label2.TabIndex = 8;
            this.label2.Text = "Antenna Tracker";
            // 
            // CMB_baudrate
            // 
            this.CMB_baudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_baudrate.FormattingEnabled = true;
            this.CMB_baudrate.Items.AddRange(new object[] {
            "4800",
            "9600",
            "14400",
            "19200",
            "28800",
            "38400",
            "57600",
            "115200"});
            this.CMB_baudrate.Location = new System.Drawing.Point(7, 53);
            this.CMB_baudrate.Name = "CMB_baudrate";
            this.CMB_baudrate.Size = new System.Drawing.Size(121, 21);
            this.CMB_baudrate.TabIndex = 5;
            // 
            // BUT_connect
            // 
            this.BUT_connect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_connect.Location = new System.Drawing.Point(134, 26);
            this.BUT_connect.Name = "BUT_connect";
            this.BUT_connect.Size = new System.Drawing.Size(75, 23);
            this.BUT_connect.TabIndex = 4;
            this.BUT_connect.Text = "Connect";
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
            // 
            // CMB_serialport
            // 
            this.CMB_serialport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_serialport.FormattingEnabled = true;
            this.CMB_serialport.Location = new System.Drawing.Point(7, 26);
            this.CMB_serialport.Name = "CMB_serialport";
            this.CMB_serialport.Size = new System.Drawing.Size(121, 21);
            this.CMB_serialport.TabIndex = 3;
            // 
            // DowdingUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "DowdingUI";
            this.Size = new System.Drawing.Size(480, 319);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label myLabel1;
        private System.Windows.Forms.TextBox txt_username;
        private System.Windows.Forms.Label myLabel3;
        private System.Windows.Forms.TextBox txt_password;
        private System.Windows.Forms.Label myLabel2;
        private System.Windows.Forms.ComboBox cmb_server;
        private MissionPlanner.Controls.MyButton but_verify;
        private System.Windows.Forms.CheckBox chk_enable;
        private System.Windows.Forms.Label label1;
        private MissionPlanner.Controls.MyButton but_token;
        private MissionPlanner.Controls.MyButton but_start;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbl_header;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox CMB_baudrate;
        private MissionPlanner.Controls.MyButton BUT_connect;
        private System.Windows.Forms.ComboBox CMB_serialport;
    }
}
