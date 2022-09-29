namespace MissionPlanner
{
    partial class NMEA_GPS_Connection
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
            this.CB_auto_connect = new System.Windows.Forms.CheckBox();
            this.CMB_baudrate = new System.Windows.Forms.ComboBox();
            this.CMB_serialport = new System.Windows.Forms.ComboBox();
            this.BUT_connect = new MissionPlanner.Controls.MyButton();
            this.LBL_gpsStatus = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CB_auto_connect
            // 
            this.CB_auto_connect.AutoSize = true;
            this.CB_auto_connect.Location = new System.Drawing.Point(303, 42);
            this.CB_auto_connect.Name = "CB_auto_connect";
            this.CB_auto_connect.Size = new System.Drawing.Size(48, 17);
            this.CB_auto_connect.TabIndex = 46;
            this.CB_auto_connect.Text = "Auto";
            this.CB_auto_connect.UseVisualStyleBackColor = true;
            this.CB_auto_connect.CheckedChanged += new System.EventHandler(this.CB_auto_connect_CheckedChanged);
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
            this.CMB_baudrate.Location = new System.Drawing.Point(133, 18);
            this.CMB_baudrate.Name = "CMB_baudrate";
            this.CMB_baudrate.Size = new System.Drawing.Size(97, 21);
            this.CMB_baudrate.TabIndex = 45;
            // 
            // CMB_serialport
            // 
            this.CMB_serialport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_serialport.FormattingEnabled = true;
            this.CMB_serialport.Location = new System.Drawing.Point(6, 19);
            this.CMB_serialport.Name = "CMB_serialport";
            this.CMB_serialport.Size = new System.Drawing.Size(121, 21);
            this.CMB_serialport.TabIndex = 44;
            this.CMB_serialport.Enter += new System.EventHandler(this.CMB_serialport_Enter);
            // 
            // BUT_connect
            // 
            this.BUT_connect.Location = new System.Drawing.Point(236, 19);
            this.BUT_connect.Name = "BUT_connect";
            this.BUT_connect.Size = new System.Drawing.Size(115, 20);
            this.BUT_connect.TabIndex = 43;
            this.BUT_connect.Text = "Connect Base GPS";
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
            // 
            // LBL_gpsStatus
            // 
            this.LBL_gpsStatus.AutoSize = true;
            this.LBL_gpsStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_gpsStatus.Location = new System.Drawing.Point(3, 43);
            this.LBL_gpsStatus.Name = "LBL_gpsStatus";
            this.LBL_gpsStatus.Size = new System.Drawing.Size(368, 13);
            this.LBL_gpsStatus.TabIndex = 42;
            this.LBL_gpsStatus.Text = "Not Yet Started                                                                  " +
    "                              ";
            this.LBL_gpsStatus.DoubleClick += new System.EventHandler(this.LBL_gpsStatus_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BUT_connect);
            this.groupBox1.Controls.Add(this.CB_auto_connect);
            this.groupBox1.Controls.Add(this.LBL_gpsStatus);
            this.groupBox1.Controls.Add(this.CMB_baudrate);
            this.groupBox1.Controls.Add(this.CMB_serialport);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(360, 80);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "GCS GPS ";
            // 
            // NMEA_GPS_Connection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "NMEA_GPS_Connection";
            this.Size = new System.Drawing.Size(369, 88);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox CB_auto_connect;
        private System.Windows.Forms.ComboBox CMB_baudrate;
        private System.Windows.Forms.ComboBox CMB_serialport;
        private Controls.MyButton BUT_connect;
        private System.Windows.Forms.Label LBL_gpsStatus;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
