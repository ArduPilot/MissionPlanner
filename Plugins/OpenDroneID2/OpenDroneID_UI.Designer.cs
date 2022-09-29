
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
            this.txt_UserID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TXT_UAS_ID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.LED_ArmedError = new Bulb.LedBulb();
            this.label13 = new System.Windows.Forms.Label();
            this.LED_RemoteID_Messages = new Bulb.LedBulb();
            this.LED_gps_valid = new Bulb.LedBulb();
            this.label14 = new System.Windows.Forms.Label();
            this.LED_UAS_ID = new Bulb.LedBulb();
            this.label16 = new System.Windows.Forms.Label();
            this.CMB_uas_id_type = new System.Windows.Forms.ComboBox();
            this.CMB_uas_type = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.CMB_self_id_type = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.TXT_self_id_TXT = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ODOD_tabs = new System.Windows.Forms.TabControl();
            this.tabStatus = new System.Windows.Forms.TabPage();
            this.LBL_version = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.TXT_RID_Status_Msg = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TXT_ODID_Status = new System.Windows.Forms.RichTextBox();
            this.tab_uid = new System.Windows.Forms.TabPage();
            this.tab_ops = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.CMB_op_id_type = new System.Windows.Forms.ComboBox();
            this.nmea_GPS_1 = new MissionPlanner.NMEA_GPS_Connection();
            this.myODID_Status = new MissionPlanner.Controls.OpenDroneID_Map_Status();
            this.groupBox2.SuspendLayout();
            this.ODOD_tabs.SuspendLayout();
            this.tabStatus.SuspendLayout();
            this.tab_uid.SuspendLayout();
            this.tab_ops.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_UserID
            // 
            this.txt_UserID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_UserID.Location = new System.Drawing.Point(100, 9);
            this.txt_UserID.MaxLength = 20;
            this.txt_UserID.Name = "txt_UserID";
            this.txt_UserID.Size = new System.Drawing.Size(195, 20);
            this.txt_UserID.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Operator ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "UAS ID";
            // 
            // TXT_UAS_ID
            // 
            this.TXT_UAS_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXT_UAS_ID.Location = new System.Drawing.Point(109, 17);
            this.TXT_UAS_ID.MaxLength = 20;
            this.TXT_UAS_ID.Name = "TXT_UAS_ID";
            this.TXT_UAS_ID.Size = new System.Drawing.Size(205, 20);
            this.TXT_UAS_ID.TabIndex = 8;
            this.TXT_UAS_ID.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "UAS ID Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "UA Type";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(112, 43);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 13);
            this.label12.TabIndex = 31;
            this.label12.Text = "ARM Status";
            // 
            // LED_ArmedError
            // 
            this.LED_ArmedError.Color = System.Drawing.Color.White;
            this.LED_ArmedError.Location = new System.Drawing.Point(92, 41);
            this.LED_ArmedError.Name = "LED_ArmedError";
            this.LED_ArmedError.On = true;
            this.LED_ArmedError.Size = new System.Drawing.Size(16, 16);
            this.LED_ArmedError.TabIndex = 32;
            this.LED_ArmedError.Text = "ledBulb1";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(24, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(63, 13);
            this.label13.TabIndex = 33;
            this.label13.Text = "RID Comms";
            // 
            // LED_RemoteID_Messages
            // 
            this.LED_RemoteID_Messages.Color = System.Drawing.Color.White;
            this.LED_RemoteID_Messages.Location = new System.Drawing.Point(7, 19);
            this.LED_RemoteID_Messages.Name = "LED_RemoteID_Messages";
            this.LED_RemoteID_Messages.On = true;
            this.LED_RemoteID_Messages.Size = new System.Drawing.Size(16, 16);
            this.LED_RemoteID_Messages.TabIndex = 34;
            this.LED_RemoteID_Messages.Text = "ledBulb1";
            // 
            // LED_gps_valid
            // 
            this.LED_gps_valid.Color = System.Drawing.Color.White;
            this.LED_gps_valid.Location = new System.Drawing.Point(8, 41);
            this.LED_gps_valid.Name = "LED_gps_valid";
            this.LED_gps_valid.On = true;
            this.LED_gps_valid.Size = new System.Drawing.Size(16, 16);
            this.LED_gps_valid.TabIndex = 36;
            this.LED_gps_valid.Text = "LED_gps_sbas";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(27, 43);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 13);
            this.label14.TabIndex = 35;
            this.label14.Text = "GCS GPS";
            // 
            // LED_UAS_ID
            // 
            this.LED_UAS_ID.Color = System.Drawing.Color.White;
            this.LED_UAS_ID.Location = new System.Drawing.Point(93, 20);
            this.LED_UAS_ID.Name = "LED_UAS_ID";
            this.LED_UAS_ID.On = true;
            this.LED_UAS_ID.Size = new System.Drawing.Size(16, 16);
            this.LED_UAS_ID.TabIndex = 40;
            this.LED_UAS_ID.Text = "ledBulb3";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(110, 22);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(43, 13);
            this.label16.TabIndex = 39;
            this.label16.Text = "UAS ID";
            // 
            // CMB_uas_id_type
            // 
            this.CMB_uas_id_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CMB_uas_id_type.FormattingEnabled = true;
            this.CMB_uas_id_type.Location = new System.Drawing.Point(109, 45);
            this.CMB_uas_id_type.Name = "CMB_uas_id_type";
            this.CMB_uas_id_type.Size = new System.Drawing.Size(205, 21);
            this.CMB_uas_id_type.TabIndex = 42;
            // 
            // CMB_uas_type
            // 
            this.CMB_uas_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CMB_uas_type.FormattingEnabled = true;
            this.CMB_uas_type.Location = new System.Drawing.Point(109, 76);
            this.CMB_uas_type.Name = "CMB_uas_type";
            this.CMB_uas_type.Size = new System.Drawing.Size(205, 21);
            this.CMB_uas_type.TabIndex = 43;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(7, 116);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(66, 13);
            this.label17.TabIndex = 44;
            this.label17.Text = "Self ID Type";
            // 
            // CMB_self_id_type
            // 
            this.CMB_self_id_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CMB_self_id_type.FormattingEnabled = true;
            this.CMB_self_id_type.Location = new System.Drawing.Point(100, 116);
            this.CMB_self_id_type.Name = "CMB_self_id_type";
            this.CMB_self_id_type.Size = new System.Drawing.Size(195, 21);
            this.CMB_self_id_type.TabIndex = 45;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(6, 88);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(67, 13);
            this.label18.TabIndex = 47;
            this.label18.Text = "Self ID Desc";
            // 
            // TXT_self_id_TXT
            // 
            this.TXT_self_id_TXT.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXT_self_id_TXT.Location = new System.Drawing.Point(100, 88);
            this.TXT_self_id_TXT.MaxLength = 23;
            this.TXT_self_id_TXT.Name = "TXT_self_id_TXT";
            this.TXT_self_id_TXT.Size = new System.Drawing.Size(195, 20);
            this.TXT_self_id_TXT.TabIndex = 46;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.LED_ArmedError);
            this.groupBox2.Controls.Add(this.LED_RemoteID_Messages);
            this.groupBox2.Controls.Add(this.LED_UAS_ID);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.LED_gps_valid);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(171, 89);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(197, 69);
            this.groupBox2.TabIndex = 53;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Remote ID Status";
            // 
            // ODOD_tabs
            // 
            this.ODOD_tabs.Controls.Add(this.tabStatus);
            this.ODOD_tabs.Controls.Add(this.tab_uid);
            this.ODOD_tabs.Controls.Add(this.tab_ops);
            this.ODOD_tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ODOD_tabs.Location = new System.Drawing.Point(6, 164);
            this.ODOD_tabs.Name = "ODOD_tabs";
            this.ODOD_tabs.SelectedIndex = 0;
            this.ODOD_tabs.Size = new System.Drawing.Size(362, 209);
            this.ODOD_tabs.TabIndex = 57;
            // 
            // tabStatus
            // 
            this.tabStatus.Controls.Add(this.LBL_version);
            this.tabStatus.Controls.Add(this.label8);
            this.tabStatus.Controls.Add(this.TXT_RID_Status_Msg);
            this.tabStatus.Controls.Add(this.label5);
            this.tabStatus.Controls.Add(this.TXT_ODID_Status);
            this.tabStatus.Location = new System.Drawing.Point(4, 22);
            this.tabStatus.Name = "tabStatus";
            this.tabStatus.Size = new System.Drawing.Size(354, 183);
            this.tabStatus.TabIndex = 2;
            this.tabStatus.Text = "Status";
            this.tabStatus.UseVisualStyleBackColor = true;
            // 
            // LBL_version
            // 
            this.LBL_version.AutoSize = true;
            this.LBL_version.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_version.Location = new System.Drawing.Point(278, 166);
            this.LBL_version.Name = "LBL_version";
            this.LBL_version.Size = new System.Drawing.Size(70, 13);
            this.LBL_version.TabIndex = 13;
            this.LBL_version.Text = "                     ";
            this.LBL_version.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.LBL_version.DoubleClick += new System.EventHandler(this.LBL_version_DoubleClick);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 145);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(273, 26);
            this.label8.TabIndex = 12;
            this.label8.Text = "This version of Open Drone ID Plugin is for Testing Only.\r\nDouble Click GPS Data " +
    "for RAW NMEA Viewer. ";
            // 
            // TXT_RID_Status_Msg
            // 
            this.TXT_RID_Status_Msg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXT_RID_Status_Msg.Location = new System.Drawing.Point(146, 4);
            this.TXT_RID_Status_Msg.Name = "TXT_RID_Status_Msg";
            this.TXT_RID_Status_Msg.Size = new System.Drawing.Size(205, 20);
            this.TXT_RID_Status_Msg.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "RID Armed Status";
            // 
            // TXT_ODID_Status
            // 
            this.TXT_ODID_Status.Location = new System.Drawing.Point(4, 30);
            this.TXT_ODID_Status.Name = "TXT_ODID_Status";
            this.TXT_ODID_Status.Size = new System.Drawing.Size(347, 112);
            this.TXT_ODID_Status.TabIndex = 0;
            this.TXT_ODID_Status.Text = "";
            // 
            // tab_uid
            // 
            this.tab_uid.Controls.Add(this.TXT_UAS_ID);
            this.tab_uid.Controls.Add(this.label2);
            this.tab_uid.Controls.Add(this.label4);
            this.tab_uid.Controls.Add(this.CMB_uas_id_type);
            this.tab_uid.Controls.Add(this.CMB_uas_type);
            this.tab_uid.Controls.Add(this.label3);
            this.tab_uid.Location = new System.Drawing.Point(4, 22);
            this.tab_uid.Name = "tab_uid";
            this.tab_uid.Padding = new System.Windows.Forms.Padding(3);
            this.tab_uid.Size = new System.Drawing.Size(354, 183);
            this.tab_uid.TabIndex = 0;
            this.tab_uid.Text = "UAS ID";
            this.tab_uid.UseVisualStyleBackColor = true;
            // 
            // tab_ops
            // 
            this.tab_ops.Controls.Add(this.label7);
            this.tab_ops.Controls.Add(this.label6);
            this.tab_ops.Controls.Add(this.CMB_op_id_type);
            this.tab_ops.Controls.Add(this.label17);
            this.tab_ops.Controls.Add(this.label1);
            this.tab_ops.Controls.Add(this.txt_UserID);
            this.tab_ops.Controls.Add(this.CMB_self_id_type);
            this.tab_ops.Controls.Add(this.label18);
            this.tab_ops.Controls.Add(this.TXT_self_id_TXT);
            this.tab_ops.Location = new System.Drawing.Point(4, 22);
            this.tab_ops.Name = "tab_ops";
            this.tab_ops.Padding = new System.Windows.Forms.Padding(3);
            this.tab_ops.Size = new System.Drawing.Size(354, 183);
            this.tab_ops.TabIndex = 1;
            this.tab_ops.Text = "Operations";
            this.tab_ops.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(7, 66);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 50;
            this.label7.Text = "Self ID: ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(7, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 48;
            this.label6.Text = "Oper. ID Type";
            // 
            // CMB_op_id_type
            // 
            this.CMB_op_id_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CMB_op_id_type.FormattingEnabled = true;
            this.CMB_op_id_type.Location = new System.Drawing.Point(100, 34);
            this.CMB_op_id_type.Name = "CMB_op_id_type";
            this.CMB_op_id_type.Size = new System.Drawing.Size(195, 21);
            this.CMB_op_id_type.TabIndex = 49;
            // 
            // nmea_GPS_1
            // 
            this.nmea_GPS_1.Location = new System.Drawing.Point(6, 3);
            this.nmea_GPS_1.Name = "nmea_GPS_1";
            this.nmea_GPS_1.Size = new System.Drawing.Size(369, 88);
            this.nmea_GPS_1.TabIndex = 56;
            // 
            // myODID_Status
            // 
            this.myODID_Status.Location = new System.Drawing.Point(6, 97);
            this.myODID_Status.Name = "myODID_Status";
            this.myODID_Status.Size = new System.Drawing.Size(140, 40);
            this.myODID_Status.TabIndex = 54;
            // 
            // OpenDroneID_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ODOD_tabs);
            this.Controls.Add(this.nmea_GPS_1);
            this.Controls.Add(this.myODID_Status);
            this.Controls.Add(this.groupBox2);
            this.Name = "OpenDroneID_UI";
            this.Size = new System.Drawing.Size(380, 380);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ODOD_tabs.ResumeLayout(false);
            this.tabStatus.ResumeLayout(false);
            this.tabStatus.PerformLayout();
            this.tab_uid.ResumeLayout(false);
            this.tab_uid.PerformLayout();
            this.tab_ops.ResumeLayout(false);
            this.tab_ops.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txt_UserID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TXT_UAS_ID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label12;
        private Bulb.LedBulb LED_ArmedError;
        private System.Windows.Forms.Label label13;
        private Bulb.LedBulb LED_RemoteID_Messages;
        private Bulb.LedBulb LED_gps_valid;
        private System.Windows.Forms.Label label14;
        private Bulb.LedBulb LED_UAS_ID;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox CMB_uas_id_type;
        private System.Windows.Forms.ComboBox CMB_uas_type;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox CMB_self_id_type;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox TXT_self_id_TXT;
        private System.Windows.Forms.GroupBox groupBox2;
        private OpenDroneID_Map_Status myODID_Status;
        private NMEA_GPS_Connection nmea_GPS_1;
        private System.Windows.Forms.TabControl ODOD_tabs;
        private System.Windows.Forms.TabPage tab_uid;
        private System.Windows.Forms.TabPage tab_ops;
        private System.Windows.Forms.TabPage tabStatus;
        private System.Windows.Forms.RichTextBox TXT_ODID_Status;
        private System.Windows.Forms.TextBox TXT_RID_Status_Msg;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox CMB_op_id_type;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label LBL_version;
    }
}
