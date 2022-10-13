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
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
            }
            catch
            {
                try
                {
                    if (disposing && (components != null))
                    {
                        components.Dispose();
                    }
                }
                catch { }
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
            this.BTN_clear_TB = new System.Windows.Forms.Button();
            this.GB_connection = new System.Windows.Forms.GroupBox();
            this.label_type = new System.Windows.Forms.Label();
            this.TB_xml_type = new System.Windows.Forms.TextBox();
            this.CB_advancedMode = new System.Windows.Forms.CheckBox();
            this.myDataGridView1 = new MissionPlanner.Controls.MyDataGridView();
            this.sysid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VMF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactCallsign = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactEndPointIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GB_connection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).BeginInit();
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
            "0.2hz",
            "0.1hz"});
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
            this.CMB_serialport.SelectedIndexChanged += new System.EventHandler(this.CMB_serialport_SelectedIndexChanged);
            // 
            // TB_output
            // 
            this.TB_output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_output.Location = new System.Drawing.Point(12, 102);
            this.TB_output.Multiline = true;
            this.TB_output.Name = "TB_output";
            this.TB_output.ReadOnly = true;
            this.TB_output.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.TB_output.Size = new System.Drawing.Size(687, 334);
            this.TB_output.TabIndex = 12;
            this.TB_output.WordWrap = false;
            // 
            // BTN_clear_TB
            // 
            this.BTN_clear_TB.Location = new System.Drawing.Point(264, 70);
            this.BTN_clear_TB.Name = "BTN_clear_TB";
            this.BTN_clear_TB.Size = new System.Drawing.Size(88, 23);
            this.BTN_clear_TB.TabIndex = 16;
            this.BTN_clear_TB.Text = "Clear Window";
            this.BTN_clear_TB.UseVisualStyleBackColor = true;
            this.BTN_clear_TB.Click += new System.EventHandler(this.BTN_clear_TB_Click);
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
            // label_type
            // 
            this.label_type.AutoSize = true;
            this.label_type.Location = new System.Drawing.Point(233, 49);
            this.label_type.Name = "label_type";
            this.label_type.Size = new System.Drawing.Size(31, 13);
            this.label_type.TabIndex = 21;
            this.label_type.Text = "Type";
            // 
            // TB_xml_type
            // 
            this.TB_xml_type.Location = new System.Drawing.Point(265, 47);
            this.TB_xml_type.Name = "TB_xml_type";
            this.TB_xml_type.Size = new System.Drawing.Size(88, 20);
            this.TB_xml_type.TabIndex = 20;
            this.TB_xml_type.Text = "a-f-A-M-F-Q";
            // 
            // CB_advancedMode
            // 
            this.CB_advancedMode.AutoSize = true;
            this.CB_advancedMode.Location = new System.Drawing.Point(247, 20);
            this.CB_advancedMode.Margin = new System.Windows.Forms.Padding(2);
            this.CB_advancedMode.Name = "CB_advancedMode";
            this.CB_advancedMode.Size = new System.Drawing.Size(107, 17);
            this.CB_advancedMode.TabIndex = 23;
            this.CB_advancedMode.Text = "Additional Details";
            this.CB_advancedMode.UseVisualStyleBackColor = true;
            this.CB_advancedMode.CheckedChanged += new System.EventHandler(this.CB_advancedMode_CheckedChanged);
            // 
            // myDataGridView1
            // 
            this.myDataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.myDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sysid,
            this.UID,
            this.VMF,
            this.ContactCallsign,
            this.ContactEndPointIP});
            this.myDataGridView1.Location = new System.Drawing.Point(12, 443);
            this.myDataGridView1.Name = "myDataGridView1";
            this.myDataGridView1.RowHeadersWidth = 62;
            this.myDataGridView1.Size = new System.Drawing.Size(687, 121);
            this.myDataGridView1.TabIndex = 22;
            this.myDataGridView1.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.myDataGridView1_RowValidated);
            // 
            // sysid
            // 
            this.sysid.HeaderText = "sysid";
            this.sysid.MinimumWidth = 8;
            this.sysid.Name = "sysid";
            this.sysid.Width = 150;
            // 
            // UID
            // 
            this.UID.HeaderText = "UID";
            this.UID.MinimumWidth = 8;
            this.UID.Name = "UID";
            this.UID.Width = 150;
            // 
            // VMF
            // 
            this.VMF.HeaderText = "VMF";
            this.VMF.MinimumWidth = 8;
            this.VMF.Name = "VMF";
            this.VMF.Width = 150;
            // 
            // ContactCallsign
            // 
            this.ContactCallsign.HeaderText = "ContactCallsign";
            this.ContactCallsign.MinimumWidth = 8;
            this.ContactCallsign.Name = "ContactCallsign";
            this.ContactCallsign.ToolTipText = "If you don\'t knwo wat this is, either leave it blank or use the UID.";
            this.ContactCallsign.Width = 150;
            // 
            // ContactEndPointIP
            // 
            this.ContactEndPointIP.HeaderText = "ContactEndPointIP";
            this.ContactEndPointIP.MinimumWidth = 8;
            this.ContactEndPointIP.Name = "ContactEndPointIP";
            this.ContactEndPointIP.ToolTipText = "If you don\'t know what this is then leave it blank.";
            this.ContactEndPointIP.Width = 150;
            // 
            // SerialOutputCoT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 575);
            this.Controls.Add(this.CB_advancedMode);
            this.Controls.Add(this.myDataGridView1);
            this.Controls.Add(this.label_type);
            this.Controls.Add(this.TB_xml_type);
            this.Controls.Add(this.GB_connection);
            this.Controls.Add(this.BTN_clear_TB);
            this.Controls.Add(this.TB_output);
            this.Name = "SerialOutputCoT";
            this.Text = "Output Cursor on Target";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SerialOutputCoT_FormClosing);
            this.Load += new System.EventHandler(this.SerialOutputCoT_Load);
            this.GB_connection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_updaterate;
        private System.Windows.Forms.ComboBox CMB_baudrate;
        private MyButton BUT_connect;
        private System.Windows.Forms.ComboBox CMB_serialport;
        private System.Windows.Forms.TextBox TB_output;
        private System.Windows.Forms.Button BTN_clear_TB;
        private System.Windows.Forms.GroupBox GB_connection;
        private System.Windows.Forms.Label label_type;
        private System.Windows.Forms.TextBox TB_xml_type;
        private MyDataGridView myDataGridView1;
        private System.Windows.Forms.CheckBox CB_advancedMode;
        private System.Windows.Forms.DataGridViewTextBoxColumn sysid;
        private System.Windows.Forms.DataGridViewTextBoxColumn UID;
        private System.Windows.Forms.DataGridViewTextBoxColumn VMF;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactCallsign;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactEndPointIP;
    }
}