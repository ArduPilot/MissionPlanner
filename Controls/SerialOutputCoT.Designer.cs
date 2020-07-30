namespace MissionPlanner.Controls
{
    partial class SerialOutputCoT
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
            this.CMB_updaterate = new System.Windows.Forms.ComboBox();
            this.CMB_baudrate = new System.Windows.Forms.ComboBox();
            this.BUT_connect = new MissionPlanner.Controls.MyButton();
            this.CMB_serialport = new System.Windows.Forms.ComboBox();
            this.TB_output = new System.Windows.Forms.TextBox();
            this.TB_xml_uid = new System.Windows.Forms.TextBox();
            this.label_uid = new System.Windows.Forms.Label();
            this.BTN_clear_TB = new System.Windows.Forms.Button();
            this.GB_connection = new System.Windows.Forms.GroupBox();
            this.GB_override = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CB_overrides_enable = new System.Windows.Forms.CheckBox();
            this.TB_override_speed = new System.Windows.Forms.TextBox();
            this.CB_override_speed = new System.Windows.Forms.CheckBox();
            this.TB_override_heading = new System.Windows.Forms.TextBox();
            this.CB_override_heading = new System.Windows.Forms.CheckBox();
            this.TB_override_alt = new System.Windows.Forms.TextBox();
            this.CB_override_alt = new System.Windows.Forms.CheckBox();
            this.TB_override_lng = new System.Windows.Forms.TextBox();
            this.CB_override_lng = new System.Windows.Forms.CheckBox();
            this.TB_override_lat = new System.Windows.Forms.TextBox();
            this.CB_override_lat = new System.Windows.Forms.CheckBox();
            this.label_type = new System.Windows.Forms.Label();
            this.TB_xml_type = new System.Windows.Forms.TextBox();
            this.GB_connection.SuspendLayout();
            this.GB_override.SuspendLayout();
            this.SuspendLayout();
            // 
            // CMB_updaterate
            // 
            this.CMB_updaterate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_updaterate.FormattingEnabled = true;
            this.CMB_updaterate.Items.AddRange(new object[] {
            "10hz",
            "5hz",
            "2hz",
            "1hz",
            "0.5hz",
            "0.25hz"});
            this.CMB_updaterate.Location = new System.Drawing.Point(136, 46);
            this.CMB_updaterate.Name = "CMB_updaterate";
            this.CMB_updaterate.Size = new System.Drawing.Size(75, 21);
            this.CMB_updaterate.TabIndex = 11;
            this.CMB_updaterate.SelectedIndexChanged += new System.EventHandler(this.CMB_updaterate_SelectedIndexChanged);
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
            this.CMB_baudrate.Location = new System.Drawing.Point(8, 46);
            this.CMB_baudrate.Name = "CMB_baudrate";
            this.CMB_baudrate.Size = new System.Drawing.Size(121, 21);
            this.CMB_baudrate.TabIndex = 10;
            // 
            // BUT_connect
            // 
            this.BUT_connect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_connect.Location = new System.Drawing.Point(135, 19);
            this.BUT_connect.Name = "BUT_connect";
            this.BUT_connect.Size = new System.Drawing.Size(75, 23);
            this.BUT_connect.TabIndex = 9;
            this.BUT_connect.Text = "Connect";
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
            // 
            // CMB_serialport
            // 
            this.CMB_serialport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_serialport.FormattingEnabled = true;
            this.CMB_serialport.Location = new System.Drawing.Point(8, 19);
            this.CMB_serialport.Name = "CMB_serialport";
            this.CMB_serialport.Size = new System.Drawing.Size(121, 21);
            this.CMB_serialport.TabIndex = 8;
            // 
            // TB_output
            // 
            this.TB_output.Location = new System.Drawing.Point(12, 172);
            this.TB_output.Multiline = true;
            this.TB_output.Name = "TB_output";
            this.TB_output.ReadOnly = true;
            this.TB_output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TB_output.Size = new System.Drawing.Size(486, 262);
            this.TB_output.TabIndex = 12;
            this.TB_output.WordWrap = false;
            // 
            // TB_xml_uid
            // 
            this.TB_xml_uid.Location = new System.Drawing.Point(60, 110);
            this.TB_xml_uid.Name = "TB_xml_uid";
            this.TB_xml_uid.Size = new System.Drawing.Size(139, 20);
            this.TB_xml_uid.TabIndex = 13;
            this.TB_xml_uid.Text = "K1000ULE";
            // 
            // label_uid
            // 
            this.label_uid.AutoSize = true;
            this.label_uid.Location = new System.Drawing.Point(28, 112);
            this.label_uid.Name = "label_uid";
            this.label_uid.Size = new System.Drawing.Size(26, 13);
            this.label_uid.TabIndex = 15;
            this.label_uid.Text = "UID";
            // 
            // BTN_clear_TB
            // 
            this.BTN_clear_TB.Location = new System.Drawing.Point(423, 440);
            this.BTN_clear_TB.Name = "BTN_clear_TB";
            this.BTN_clear_TB.Size = new System.Drawing.Size(75, 23);
            this.BTN_clear_TB.TabIndex = 16;
            this.BTN_clear_TB.Text = "Clear";
            this.BTN_clear_TB.UseVisualStyleBackColor = true;
            // 
            // GB_connection
            // 
            this.GB_connection.Controls.Add(this.CMB_serialport);
            this.GB_connection.Controls.Add(this.BUT_connect);
            this.GB_connection.Controls.Add(this.CMB_updaterate);
            this.GB_connection.Controls.Add(this.CMB_baudrate);
            this.GB_connection.Location = new System.Drawing.Point(12, 12);
            this.GB_connection.Name = "GB_connection";
            this.GB_connection.Size = new System.Drawing.Size(217, 81);
            this.GB_connection.TabIndex = 18;
            this.GB_connection.TabStop = false;
            this.GB_connection.Text = "Connection";
            // 
            // GB_override
            // 
            this.GB_override.Controls.Add(this.label3);
            this.GB_override.Controls.Add(this.label2);
            this.GB_override.Controls.Add(this.label1);
            this.GB_override.Controls.Add(this.CB_overrides_enable);
            this.GB_override.Controls.Add(this.TB_override_speed);
            this.GB_override.Controls.Add(this.CB_override_speed);
            this.GB_override.Controls.Add(this.TB_override_heading);
            this.GB_override.Controls.Add(this.CB_override_heading);
            this.GB_override.Controls.Add(this.TB_override_alt);
            this.GB_override.Controls.Add(this.CB_override_alt);
            this.GB_override.Controls.Add(this.TB_override_lng);
            this.GB_override.Controls.Add(this.CB_override_lng);
            this.GB_override.Controls.Add(this.TB_override_lat);
            this.GB_override.Controls.Add(this.CB_override_lat);
            this.GB_override.Location = new System.Drawing.Point(253, 21);
            this.GB_override.Name = "GB_override";
            this.GB_override.Size = new System.Drawing.Size(245, 145);
            this.GB_override.TabIndex = 19;
            this.GB_override.TabStop = false;
            this.GB_override.Text = "       Override Values";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(204, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "m/s";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(204, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "0-359";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(204, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "m";
            // 
            // CB_overrides_enable
            // 
            this.CB_overrides_enable.AutoSize = true;
            this.CB_overrides_enable.Location = new System.Drawing.Point(11, 0);
            this.CB_overrides_enable.Name = "CB_overrides_enable";
            this.CB_overrides_enable.Size = new System.Drawing.Size(15, 14);
            this.CB_overrides_enable.TabIndex = 23;
            this.CB_overrides_enable.UseVisualStyleBackColor = true;
            this.CB_overrides_enable.CheckedChanged += new System.EventHandler(this.CB_overrides_enable_CheckedChanged);
            // 
            // TB_override_speed
            // 
            this.TB_override_speed.Location = new System.Drawing.Point(77, 116);
            this.TB_override_speed.Name = "TB_override_speed";
            this.TB_override_speed.Size = new System.Drawing.Size(126, 20);
            this.TB_override_speed.TabIndex = 22;
            // 
            // CB_override_speed
            // 
            this.CB_override_speed.AutoSize = true;
            this.CB_override_speed.Location = new System.Drawing.Point(7, 118);
            this.CB_override_speed.Name = "CB_override_speed";
            this.CB_override_speed.Size = new System.Drawing.Size(57, 17);
            this.CB_override_speed.TabIndex = 21;
            this.CB_override_speed.Text = "Speed";
            this.CB_override_speed.UseVisualStyleBackColor = true;
            // 
            // TB_override_heading
            // 
            this.TB_override_heading.Location = new System.Drawing.Point(77, 93);
            this.TB_override_heading.Name = "TB_override_heading";
            this.TB_override_heading.Size = new System.Drawing.Size(126, 20);
            this.TB_override_heading.TabIndex = 20;
            // 
            // CB_override_heading
            // 
            this.CB_override_heading.AutoSize = true;
            this.CB_override_heading.Location = new System.Drawing.Point(7, 95);
            this.CB_override_heading.Name = "CB_override_heading";
            this.CB_override_heading.Size = new System.Drawing.Size(66, 17);
            this.CB_override_heading.TabIndex = 19;
            this.CB_override_heading.Text = "Heading";
            this.CB_override_heading.UseVisualStyleBackColor = true;
            // 
            // TB_override_alt
            // 
            this.TB_override_alt.Location = new System.Drawing.Point(77, 70);
            this.TB_override_alt.Name = "TB_override_alt";
            this.TB_override_alt.Size = new System.Drawing.Size(126, 20);
            this.TB_override_alt.TabIndex = 18;
            // 
            // CB_override_alt
            // 
            this.CB_override_alt.AutoSize = true;
            this.CB_override_alt.Location = new System.Drawing.Point(7, 72);
            this.CB_override_alt.Name = "CB_override_alt";
            this.CB_override_alt.Size = new System.Drawing.Size(61, 17);
            this.CB_override_alt.TabIndex = 17;
            this.CB_override_alt.Text = "Altitude";
            this.CB_override_alt.UseVisualStyleBackColor = true;
            // 
            // TB_override_lng
            // 
            this.TB_override_lng.Location = new System.Drawing.Point(77, 47);
            this.TB_override_lng.Name = "TB_override_lng";
            this.TB_override_lng.Size = new System.Drawing.Size(126, 20);
            this.TB_override_lng.TabIndex = 16;
            // 
            // CB_override_lng
            // 
            this.CB_override_lng.AutoSize = true;
            this.CB_override_lng.Location = new System.Drawing.Point(7, 49);
            this.CB_override_lng.Name = "CB_override_lng";
            this.CB_override_lng.Size = new System.Drawing.Size(73, 17);
            this.CB_override_lng.TabIndex = 15;
            this.CB_override_lng.Text = "Longitude";
            this.CB_override_lng.UseVisualStyleBackColor = true;
            // 
            // TB_override_lat
            // 
            this.TB_override_lat.Location = new System.Drawing.Point(77, 24);
            this.TB_override_lat.Name = "TB_override_lat";
            this.TB_override_lat.Size = new System.Drawing.Size(126, 20);
            this.TB_override_lat.TabIndex = 14;
            // 
            // CB_override_lat
            // 
            this.CB_override_lat.AutoSize = true;
            this.CB_override_lat.Location = new System.Drawing.Point(7, 26);
            this.CB_override_lat.Name = "CB_override_lat";
            this.CB_override_lat.Size = new System.Drawing.Size(64, 17);
            this.CB_override_lat.TabIndex = 0;
            this.CB_override_lat.Text = "Latitude";
            this.CB_override_lat.UseVisualStyleBackColor = true;
            // 
            // label_type
            // 
            this.label_type.AutoSize = true;
            this.label_type.Location = new System.Drawing.Point(28, 138);
            this.label_type.Name = "label_type";
            this.label_type.Size = new System.Drawing.Size(31, 13);
            this.label_type.TabIndex = 21;
            this.label_type.Text = "Type";
            // 
            // TB_xml_type
            // 
            this.TB_xml_type.Location = new System.Drawing.Point(60, 136);
            this.TB_xml_type.Name = "TB_xml_type";
            this.TB_xml_type.Size = new System.Drawing.Size(139, 20);
            this.TB_xml_type.TabIndex = 20;
            this.TB_xml_type.Text = "a-f-A-M-F-Q";
            // 
            // SerialOutputCoT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 475);
            this.Controls.Add(this.label_type);
            this.Controls.Add(this.TB_xml_type);
            this.Controls.Add(this.GB_override);
            this.Controls.Add(this.GB_connection);
            this.Controls.Add(this.BTN_clear_TB);
            this.Controls.Add(this.label_uid);
            this.Controls.Add(this.TB_xml_uid);
            this.Controls.Add(this.TB_output);
            this.Name = "SerialOutputCoT";
            this.Text = "Cursor on Target";
            this.GB_connection.ResumeLayout(false);
            this.GB_override.ResumeLayout(false);
            this.GB_override.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_updaterate;
        private System.Windows.Forms.ComboBox CMB_baudrate;
        private MyButton BUT_connect;
        private System.Windows.Forms.ComboBox CMB_serialport;
        private System.Windows.Forms.TextBox TB_output;
        private System.Windows.Forms.TextBox TB_xml_uid;
        private System.Windows.Forms.Label label_uid;
        private System.Windows.Forms.Button BTN_clear_TB;
        private System.Windows.Forms.GroupBox GB_connection;
        private System.Windows.Forms.GroupBox GB_override;
        private System.Windows.Forms.CheckBox CB_overrides_enable;
        private System.Windows.Forms.TextBox TB_override_speed;
        private System.Windows.Forms.CheckBox CB_override_speed;
        private System.Windows.Forms.TextBox TB_override_heading;
        private System.Windows.Forms.CheckBox CB_override_heading;
        private System.Windows.Forms.TextBox TB_override_alt;
        private System.Windows.Forms.CheckBox CB_override_alt;
        private System.Windows.Forms.TextBox TB_override_lng;
        private System.Windows.Forms.CheckBox CB_override_lng;
        private System.Windows.Forms.TextBox TB_override_lat;
        private System.Windows.Forms.CheckBox CB_override_lat;
        private System.Windows.Forms.Label label_type;
        private System.Windows.Forms.TextBox TB_xml_type;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}