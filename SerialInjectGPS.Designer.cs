namespace MissionPlanner
{
    partial class SerialInjectGPS
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SerialInjectGPS));
            this.CMB_serialport = new System.Windows.Forms.ComboBox();
            this.CMB_baudrate = new System.Windows.Forms.ComboBox();
            this.lbl_status1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chk_rtcmmsg = new System.Windows.Forms.CheckBox();
            this.lbl_svin = new System.Windows.Forms.Label();
            this.chk_m8pautoconfig = new System.Windows.Forms.CheckBox();
            this.groupBoxm8p = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.but_restartsvin = new MissionPlanner.Controls.MyButton();
            this.chk_m8p_130p = new System.Windows.Forms.CheckBox();
            this.but_save_basepos = new MissionPlanner.Controls.MyButton();
            this.dg_basepos = new MissionPlanner.Controls.MyDataGridView();
            this.Lat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Long = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Alt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BaseName1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Use = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_surveyinAcc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_surveyinDur = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BUT_connect = new MissionPlanner.Controls.MyButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lbl_status2 = new System.Windows.Forms.Label();
            this.lbl_status3 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelmsgseen = new System.Windows.Forms.Label();
            this.groupBoxm8p.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_basepos)).BeginInit();
            this.SuspendLayout();
            // 
            // CMB_serialport
            // 
            this.CMB_serialport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_serialport.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_serialport, "CMB_serialport");
            this.CMB_serialport.Name = "CMB_serialport";
            this.CMB_serialport.SelectedIndexChanged += new System.EventHandler(this.CMB_serialport_SelectedIndexChanged);
            // 
            // CMB_baudrate
            // 
            this.CMB_baudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_baudrate.FormattingEnabled = true;
            this.CMB_baudrate.Items.AddRange(new object[] {
            resources.GetString("CMB_baudrate.Items"),
            resources.GetString("CMB_baudrate.Items1"),
            resources.GetString("CMB_baudrate.Items2"),
            resources.GetString("CMB_baudrate.Items3"),
            resources.GetString("CMB_baudrate.Items4"),
            resources.GetString("CMB_baudrate.Items5"),
            resources.GetString("CMB_baudrate.Items6"),
            resources.GetString("CMB_baudrate.Items7"),
            resources.GetString("CMB_baudrate.Items8"),
            resources.GetString("CMB_baudrate.Items9"),
            resources.GetString("CMB_baudrate.Items10")});
            resources.ApplyResources(this.CMB_baudrate, "CMB_baudrate");
            this.CMB_baudrate.Name = "CMB_baudrate";
            // 
            // lbl_status1
            // 
            resources.ApplyResources(this.lbl_status1, "lbl_status1");
            this.lbl_status1.Name = "lbl_status1";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // chk_rtcmmsg
            // 
            resources.ApplyResources(this.chk_rtcmmsg, "chk_rtcmmsg");
            this.chk_rtcmmsg.Checked = true;
            this.chk_rtcmmsg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_rtcmmsg.Name = "chk_rtcmmsg";
            this.toolTip1.SetToolTip(this.chk_rtcmmsg, resources.GetString("chk_rtcmmsg.ToolTip"));
            this.chk_rtcmmsg.UseVisualStyleBackColor = true;
            this.chk_rtcmmsg.CheckedChanged += new System.EventHandler(this.chk_rtcmmsg_CheckedChanged);
            // 
            // lbl_svin
            // 
            resources.ApplyResources(this.lbl_svin, "lbl_svin");
            this.lbl_svin.Name = "lbl_svin";
            this.toolTip1.SetToolTip(this.lbl_svin, resources.GetString("lbl_svin.ToolTip"));
            // 
            // chk_m8pautoconfig
            // 
            resources.ApplyResources(this.chk_m8pautoconfig, "chk_m8pautoconfig");
            this.chk_m8pautoconfig.Checked = true;
            this.chk_m8pautoconfig.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_m8pautoconfig.Name = "chk_m8pautoconfig";
            this.toolTip1.SetToolTip(this.chk_m8pautoconfig, resources.GetString("chk_m8pautoconfig.ToolTip"));
            this.chk_m8pautoconfig.UseVisualStyleBackColor = true;
            this.chk_m8pautoconfig.CheckedChanged += new System.EventHandler(this.chk_m8pautoconfig_CheckedChanged);
            // 
            // groupBoxm8p
            // 
            resources.ApplyResources(this.groupBoxm8p, "groupBoxm8p");
            this.groupBoxm8p.Controls.Add(this.groupBox1);
            this.groupBoxm8p.Controls.Add(this.panel2);
            this.groupBoxm8p.Name = "groupBoxm8p";
            this.groupBoxm8p.TabStop = false;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.lbl_svin);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.toolTip1.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            this.toolTip1.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.toolTip1.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.chk_m8pautoconfig);
            this.panel2.Controls.Add(this.but_restartsvin);
            this.panel2.Controls.Add(this.chk_m8p_130p);
            this.panel2.Controls.Add(this.but_save_basepos);
            this.panel2.Controls.Add(this.dg_basepos);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txt_surveyinAcc);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txt_surveyinDur);
            this.panel2.Name = "panel2";
            // 
            // but_restartsvin
            // 
            resources.ApplyResources(this.but_restartsvin, "but_restartsvin");
            this.but_restartsvin.Name = "but_restartsvin";
            this.toolTip1.SetToolTip(this.but_restartsvin, resources.GetString("but_restartsvin.ToolTip"));
            this.but_restartsvin.UseVisualStyleBackColor = true;
            this.but_restartsvin.Click += new System.EventHandler(this.but_restartsvin_Click);
            // 
            // chk_m8p_130p
            // 
            resources.ApplyResources(this.chk_m8p_130p, "chk_m8p_130p");
            this.chk_m8p_130p.Name = "chk_m8p_130p";
            this.toolTip1.SetToolTip(this.chk_m8p_130p, resources.GetString("chk_m8p_130p.ToolTip"));
            this.chk_m8p_130p.UseVisualStyleBackColor = true;
            this.chk_m8p_130p.CheckedChanged += new System.EventHandler(this.chk_m8p_130p_CheckedChanged);
            // 
            // but_save_basepos
            // 
            resources.ApplyResources(this.but_save_basepos, "but_save_basepos");
            this.but_save_basepos.Name = "but_save_basepos";
            this.toolTip1.SetToolTip(this.but_save_basepos, resources.GetString("but_save_basepos.ToolTip"));
            this.but_save_basepos.UseVisualStyleBackColor = true;
            this.but_save_basepos.Click += new System.EventHandler(this.but_save_basepos_Click);
            // 
            // dg_basepos
            // 
            resources.ApplyResources(this.dg_basepos, "dg_basepos");
            this.dg_basepos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg_basepos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Lat,
            this.Long,
            this.Alt,
            this.BaseName1,
            this.Use,
            this.Delete});
            this.dg_basepos.Name = "dg_basepos";
            this.dg_basepos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg_basepos_CellContentClick);
            this.dg_basepos.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg_basepos_CellEndEdit);
            this.dg_basepos.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dg_basepos_DefaultValuesNeeded);
            this.dg_basepos.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dg_basepos_RowsRemoved);
            // 
            // Lat
            // 
            resources.ApplyResources(this.Lat, "Lat");
            this.Lat.Name = "Lat";
            // 
            // Long
            // 
            resources.ApplyResources(this.Long, "Long");
            this.Long.Name = "Long";
            // 
            // Alt
            // 
            resources.ApplyResources(this.Alt, "Alt");
            this.Alt.Name = "Alt";
            // 
            // BaseName1
            // 
            resources.ApplyResources(this.BaseName1, "BaseName1");
            this.BaseName1.Name = "BaseName1";
            // 
            // Use
            // 
            resources.ApplyResources(this.Use, "Use");
            this.Use.Name = "Use";
            this.Use.ReadOnly = true;
            this.Use.Text = "Use";
            // 
            // Delete
            // 
            resources.ApplyResources(this.Delete, "Delete");
            this.Delete.Name = "Delete";
            this.Delete.ReadOnly = true;
            this.Delete.Text = "Delete";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txt_surveyinAcc
            // 
            resources.ApplyResources(this.txt_surveyinAcc, "txt_surveyinAcc");
            this.txt_surveyinAcc.Name = "txt_surveyinAcc";
            this.toolTip1.SetToolTip(this.txt_surveyinAcc, resources.GetString("txt_surveyinAcc.ToolTip"));
            this.txt_surveyinAcc.TextChanged += new System.EventHandler(this.txt_surveyinAcc_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // txt_surveyinDur
            // 
            resources.ApplyResources(this.txt_surveyinDur, "txt_surveyinDur");
            this.txt_surveyinDur.Name = "txt_surveyinDur";
            this.toolTip1.SetToolTip(this.txt_surveyinDur, resources.GetString("txt_surveyinDur.ToolTip"));
            this.txt_surveyinDur.TextChanged += new System.EventHandler(this.txt_surveyinDur_TextChanged);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // BUT_connect
            // 
            resources.ApplyResources(this.BUT_connect, "BUT_connect");
            this.BUT_connect.Name = "BUT_connect";
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // lbl_status2
            // 
            resources.ApplyResources(this.lbl_status2, "lbl_status2");
            this.lbl_status2.Name = "lbl_status2";
            // 
            // lbl_status3
            // 
            resources.ApplyResources(this.lbl_status3, "lbl_status3");
            this.lbl_status3.Name = "lbl_status3";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // labelmsgseen
            // 
            resources.ApplyResources(this.labelmsgseen, "labelmsgseen");
            this.labelmsgseen.Name = "labelmsgseen";
            // 
            // SerialInjectGPS
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.labelmsgseen);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbl_status3);
            this.Controls.Add(this.lbl_status2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBoxm8p);
            this.Controls.Add(this.chk_rtcmmsg);
            this.Controls.Add(this.lbl_status1);
            this.Controls.Add(this.CMB_baudrate);
            this.Controls.Add(this.BUT_connect);
            this.Controls.Add(this.CMB_serialport);
            this.Name = "SerialInjectGPS";
            resources.ApplyResources(this, "$this");
            this.groupBoxm8p.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_basepos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_serialport;
        private Controls.MyButton BUT_connect;
        private System.Windows.Forms.ComboBox CMB_baudrate;
        private System.Windows.Forms.Label lbl_status1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox chk_rtcmmsg;
        private System.Windows.Forms.Label lbl_svin;
        private System.Windows.Forms.CheckBox chk_m8pautoconfig;
        private System.Windows.Forms.GroupBox groupBoxm8p;
        private System.Windows.Forms.TextBox txt_surveyinAcc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_surveyinDur;
        private Controls.MyButton but_save_basepos;
        private Controls.MyDataGridView dg_basepos;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chk_m8p_130p;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Lat;
        private System.Windows.Forms.DataGridViewTextBoxColumn Long;
        private System.Windows.Forms.DataGridViewTextBoxColumn Alt;
        private System.Windows.Forms.DataGridViewTextBoxColumn BaseName1;
        private System.Windows.Forms.DataGridViewButtonColumn Use;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
        private System.Windows.Forms.Label lbl_status2;
        private System.Windows.Forms.Label lbl_status3;
        private Controls.MyButton but_restartsvin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelmsgseen;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}