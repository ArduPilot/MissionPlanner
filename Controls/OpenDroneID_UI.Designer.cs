
namespace MissionPlanner.Controls
{
    partial class OpenDroneID_UI
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
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle1 = new BrightIdeasSoftware.HeaderStateStyle();
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle2 = new BrightIdeasSoftware.HeaderStateStyle();
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle3 = new BrightIdeasSoftware.HeaderStateStyle();
            this.txt_UserID = new System.Windows.Forms.TextBox();
            this.LBL_gpsStatus = new System.Windows.Forms.Label();
            this.BUT_connect = new MissionPlanner.Controls.MyButton();
            this.CMB_baudrate = new System.Windows.Forms.ComboBox();
            this.CMB_serialport = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lineSeparator1 = new MissionPlanner.Controls.LineSeparator();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TXT_UAS_ID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LBL_armed_invalid = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.LED_ArmedError = new Bulb.LedBulb();
            this.label13 = new System.Windows.Forms.Label();
            this.LED_RemoteID_Messages = new Bulb.LedBulb();
            this.LED_gps_valid = new Bulb.LedBulb();
            this.label14 = new System.Windows.Forms.Label();
            this.LED_UAS_ID = new Bulb.LedBulb();
            this.label16 = new System.Windows.Forms.Label();
            this.CB_auto_connect = new System.Windows.Forms.CheckBox();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.LBL_GCS_GPS_Invalid = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.headerFormatStyle1 = new BrightIdeasSoftware.HeaderFormatStyle();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.myODID_Status = new MissionPlanner.Controls.OpenDroneID_Map_Status();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_UserID
            // 
            this.txt_UserID.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_UserID.Location = new System.Drawing.Point(68, 19);
            this.txt_UserID.Name = "txt_UserID";
            this.txt_UserID.Size = new System.Drawing.Size(116, 18);
            this.txt_UserID.TabIndex = 0;
            // 
            // LBL_gpsStatus
            // 
            this.LBL_gpsStatus.AutoSize = true;
            this.LBL_gpsStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_gpsStatus.Location = new System.Drawing.Point(10, 31);
            this.LBL_gpsStatus.Name = "LBL_gpsStatus";
            this.LBL_gpsStatus.Size = new System.Drawing.Size(368, 13);
            this.LBL_gpsStatus.TabIndex = 1;
            this.LBL_gpsStatus.Text = "Not Yet Started                                                                  " +
    "                              ";
            // 
            // BUT_connect
            // 
            this.BUT_connect.Location = new System.Drawing.Point(243, 7);
            this.BUT_connect.Name = "BUT_connect";
            this.BUT_connect.Size = new System.Drawing.Size(115, 20);
            this.BUT_connect.TabIndex = 3;
            this.BUT_connect.Text = "Connect Base GPS";
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
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
            this.CMB_baudrate.Location = new System.Drawing.Point(140, 6);
            this.CMB_baudrate.Name = "CMB_baudrate";
            this.CMB_baudrate.Size = new System.Drawing.Size(97, 21);
            this.CMB_baudrate.TabIndex = 5;
            // 
            // CMB_serialport
            // 
            this.CMB_serialport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_serialport.FormattingEnabled = true;
            this.CMB_serialport.Location = new System.Drawing.Point(13, 7);
            this.CMB_serialport.Name = "CMB_serialport";
            this.CMB_serialport.Size = new System.Drawing.Size(121, 21);
            this.CMB_serialport.TabIndex = 4;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lineSeparator1
            // 
            this.lineSeparator1.Location = new System.Drawing.Point(13, 61);
            this.lineSeparator1.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator1.Name = "lineSeparator1";
            this.lineSeparator1.Size = new System.Drawing.Size(334, 2);
            this.lineSeparator1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(1, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "Operator ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "UAS ID";
            // 
            // TXT_UAS_ID
            // 
            this.TXT_UAS_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXT_UAS_ID.Location = new System.Drawing.Point(68, 19);
            this.TXT_UAS_ID.Name = "TXT_UAS_ID";
            this.TXT_UAS_ID.Size = new System.Drawing.Size(115, 18);
            this.TXT_UAS_ID.TabIndex = 8;
            this.TXT_UAS_ID.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "UAS ID Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "UA Type";
            // 
            // LBL_armed_invalid
            // 
            this.LBL_armed_invalid.AutoSize = true;
            this.LBL_armed_invalid.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_armed_invalid.ForeColor = System.Drawing.Color.Red;
            this.LBL_armed_invalid.Location = new System.Drawing.Point(30, 112);
            this.LBL_armed_invalid.Name = "LBL_armed_invalid";
            this.LBL_armed_invalid.Size = new System.Drawing.Size(109, 12);
            this.LBL_armed_invalid.TabIndex = 29;
            this.LBL_armed_invalid.Text = "{ not started }                         ";
            this.LBL_armed_invalid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(27, 100);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(91, 12);
            this.label12.TabIndex = 31;
            this.label12.Text = "Drone ID Arm Status";
            // 
            // LED_ArmedError
            // 
            this.LED_ArmedError.Color = System.Drawing.Color.Gray;
            this.LED_ArmedError.Location = new System.Drawing.Point(7, 98);
            this.LED_ArmedError.Name = "LED_ArmedError";
            this.LED_ArmedError.On = true;
            this.LED_ArmedError.Size = new System.Drawing.Size(16, 16);
            this.LED_ArmedError.TabIndex = 32;
            this.LED_ArmedError.Text = "ledBulb1";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(24, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 12);
            this.label13.TabIndex = 33;
            this.label13.Text = "Remote ID Live";
            // 
            // LED_RemoteID_Messages
            // 
            this.LED_RemoteID_Messages.Color = System.Drawing.Color.Gray;
            this.LED_RemoteID_Messages.Location = new System.Drawing.Point(7, 19);
            this.LED_RemoteID_Messages.Name = "LED_RemoteID_Messages";
            this.LED_RemoteID_Messages.On = true;
            this.LED_RemoteID_Messages.Size = new System.Drawing.Size(16, 16);
            this.LED_RemoteID_Messages.TabIndex = 34;
            this.LED_RemoteID_Messages.Text = "ledBulb1";
            // 
            // LED_gps_valid
            // 
            this.LED_gps_valid.Color = System.Drawing.Color.Gray;
            this.LED_gps_valid.Location = new System.Drawing.Point(7, 64);
            this.LED_gps_valid.Name = "LED_gps_valid";
            this.LED_gps_valid.On = true;
            this.LED_gps_valid.Size = new System.Drawing.Size(16, 16);
            this.LED_gps_valid.TabIndex = 36;
            this.LED_gps_valid.Text = "LED_gps_sbas";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(26, 66);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(99, 12);
            this.label14.TabIndex = 35;
            this.label14.Text = "GCS Geolocation Valid";
            // 
            // LED_UAS_ID
            // 
            this.LED_UAS_ID.Color = System.Drawing.Color.Gray;
            this.LED_UAS_ID.Location = new System.Drawing.Point(7, 41);
            this.LED_UAS_ID.Name = "LED_UAS_ID";
            this.LED_UAS_ID.On = true;
            this.LED_UAS_ID.Size = new System.Drawing.Size(16, 16);
            this.LED_UAS_ID.TabIndex = 40;
            this.LED_UAS_ID.Text = "ledBulb3";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(24, 43);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(69, 12);
            this.label16.TabIndex = 39;
            this.label16.Text = "UAS ID is Valid";
            // 
            // CB_auto_connect
            // 
            this.CB_auto_connect.AutoSize = true;
            this.CB_auto_connect.Location = new System.Drawing.Point(267, 30);
            this.CB_auto_connect.Name = "CB_auto_connect";
            this.CB_auto_connect.Size = new System.Drawing.Size(91, 17);
            this.CB_auto_connect.TabIndex = 41;
            this.CB_auto_connect.Text = "Auto Connect";
            this.CB_auto_connect.UseVisualStyleBackColor = true;
            this.CB_auto_connect.CheckedChanged += new System.EventHandler(this.CB_auto_connect_CheckedChanged);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(68, 40);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(115, 21);
            this.comboBox1.TabIndex = 42;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(68, 67);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(115, 21);
            this.comboBox2.TabIndex = 43;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(2, 42);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(50, 12);
            this.label17.TabIndex = 44;
            this.label17.Text = "Flight Type";
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(68, 42);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(116, 21);
            this.comboBox3.TabIndex = 45;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(1, 69);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(52, 12);
            this.label18.TabIndex = 47;
            this.label18.Text = "Flight Desc";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(68, 69);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(116, 18);
            this.textBox2.TabIndex = 46;
            // 
            // LBL_GCS_GPS_Invalid
            // 
            this.LBL_GCS_GPS_Invalid.AutoSize = true;
            this.LBL_GCS_GPS_Invalid.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_GCS_GPS_Invalid.ForeColor = System.Drawing.Color.Red;
            this.LBL_GCS_GPS_Invalid.Location = new System.Drawing.Point(31, 80);
            this.LBL_GCS_GPS_Invalid.Name = "LBL_GCS_GPS_Invalid";
            this.LBL_GCS_GPS_Invalid.Size = new System.Drawing.Size(111, 12);
            this.LBL_GCS_GPS_Invalid.TabIndex = 48;
            this.LBL_GCS_GPS_Invalid.Text = "{reason if not}                         ";
            this.LBL_GCS_GPS_Invalid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.comboBox3);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.txt_UserID);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 175);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(197, 104);
            this.groupBox1.TabIndex = 51;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operataions";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.LBL_armed_invalid);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.LBL_GCS_GPS_Invalid);
            this.groupBox2.Controls.Add(this.LED_ArmedError);
            this.groupBox2.Controls.Add(this.LED_RemoteID_Messages);
            this.groupBox2.Controls.Add(this.LED_UAS_ID);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.LED_gps_valid);
            this.groupBox2.Location = new System.Drawing.Point(206, 124);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(155, 155);
            this.groupBox2.TabIndex = 53;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Remote ID Status";
            // 
            // headerFormatStyle1
            // 
            this.headerFormatStyle1.Hot = headerStateStyle1;
            this.headerFormatStyle1.Normal = headerStateStyle2;
            this.headerFormatStyle1.Pressed = headerStateStyle3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TXT_UAS_ID);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.comboBox1);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.comboBox2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(3, 69);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(194, 100);
            this.groupBox3.TabIndex = 55;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "UAS ID/TYPE";
            // 
            // myODID_Status
            // 
            this.myODID_Status.Location = new System.Drawing.Point(210, 70);
            this.myODID_Status.Name = "myODID_Status";
            this.myODID_Status.Size = new System.Drawing.Size(150, 42);
            this.myODID_Status.TabIndex = 54;
            // 
            // OpenDroneID_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.myODID_Status);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.CB_auto_connect);
            this.Controls.Add(this.lineSeparator1);
            this.Controls.Add(this.CMB_baudrate);
            this.Controls.Add(this.CMB_serialport);
            this.Controls.Add(this.BUT_connect);
            this.Controls.Add(this.LBL_gpsStatus);
            this.Name = "OpenDroneID_UI";
            this.Size = new System.Drawing.Size(380, 334);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_UserID;
        private System.Windows.Forms.Label LBL_gpsStatus;
        private MyButton BUT_connect;
        private System.Windows.Forms.ComboBox CMB_baudrate;
        private System.Windows.Forms.ComboBox CMB_serialport;
        private System.Windows.Forms.Timer timer1;
        private LineSeparator lineSeparator1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TXT_UAS_ID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label LBL_armed_invalid;
        private System.Windows.Forms.Label label12;
        private Bulb.LedBulb LED_ArmedError;
        private System.Windows.Forms.Label label13;
        private Bulb.LedBulb LED_RemoteID_Messages;
        private Bulb.LedBulb LED_gps_valid;
        private System.Windows.Forms.Label label14;
        private Bulb.LedBulb LED_UAS_ID;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox CB_auto_connect;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label LBL_GCS_GPS_Invalid;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private BrightIdeasSoftware.HeaderFormatStyle headerFormatStyle1;
        private OpenDroneID_Map_Status myODID_Status;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}
