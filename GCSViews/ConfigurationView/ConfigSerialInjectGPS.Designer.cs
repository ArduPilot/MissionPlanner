namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigSerialInjectGPS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigSerialInjectGPS));
            this.CMB_serialport = new System.Windows.Forms.ComboBox();
            this.CMB_baudrate = new System.Windows.Forms.ComboBox();
            this.lbl_status1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chk_rtcmmsg = new System.Windows.Forms.CheckBox();
            this.lbl_svin = new System.Windows.Forms.Label();
            this.chk_autoconfig = new System.Windows.Forms.CheckBox();
            this.groupBox_autoconfig = new System.Windows.Forms.GroupBox();
            this.panel_um982 = new System.Windows.Forms.Panel();
            this.panel_septentrio = new System.Windows.Forms.Panel();
            this.label21 = new System.Windows.Forms.Label();
            this.chk_septentriogalileo = new System.Windows.Forms.CheckBox();
            this.chk_septentriobeidou = new System.Windows.Forms.CheckBox();
            this.chk_septentrioglonass = new System.Windows.Forms.CheckBox();
            this.chk_septentriogps = new System.Windows.Forms.CheckBox();
            this.input_septentriortcminterval = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.cmb_septentriortcmamount = new System.Windows.Forms.ComboBox();
            this.input_septentriofixedlongitude = new System.Windows.Forms.TextBox();
            this.chk_septentriofixedposition = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.input_septentriofixedatitude = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.input_septentriofixedaltitude = new System.Windows.Forms.TextBox();
            this.panel_ubloxoptions = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chk_m8p_130p = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_surveyinAcc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_surveyinDur = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chk_sendgga = new System.Windows.Forms.CheckBox();
            this.check_sendntripv1 = new System.Windows.Forms.CheckBox();
            this.lbl_status2 = new System.Windows.Forms.Label();
            this.lbl_status3 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelmsgseen = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelGall = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label14BDS = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.labelglonass = new System.Windows.Forms.Label();
            this.labelgps = new System.Windows.Forms.Label();
            this.labelbase = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBoxConfigType = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.button_septentriortcminterval = new MissionPlanner.Controls.MyButton();
            this.button_septentriosetposition = new MissionPlanner.Controls.MyButton();
            this.but_restartsvin = new MissionPlanner.Controls.MyButton();
            this.but_save_basepos = new MissionPlanner.Controls.MyButton();
            this.BUT_connect = new MissionPlanner.Controls.MyButton();
            this.myGMAP1 = new MissionPlanner.Controls.myGMAP();
            this.dg_basepos = new MissionPlanner.Controls.MyDataGridView();
            this.Lat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Long = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Alt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BaseName1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Use = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBox_autoconfig.SuspendLayout();
            this.panel_um982.SuspendLayout();
            this.panel_septentrio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.input_septentriortcminterval)).BeginInit();
            this.panel_ubloxoptions.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            resources.GetString("CMB_baudrate.Items10"),
            resources.GetString("CMB_baudrate.Items11"),
            resources.GetString("CMB_baudrate.Items12")});
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
            // chk_autoconfig
            // 
            resources.ApplyResources(this.chk_autoconfig, "chk_autoconfig");
            this.chk_autoconfig.Name = "chk_autoconfig";
            this.toolTip1.SetToolTip(this.chk_autoconfig, resources.GetString("chk_autoconfig.ToolTip"));
            this.chk_autoconfig.UseVisualStyleBackColor = true;
            this.chk_autoconfig.CheckedChanged += new System.EventHandler(this.chk_autoconfig_CheckedChanged);
            // 
            // groupBox_autoconfig
            // 
            this.groupBox_autoconfig.Controls.Add(this.panel_um982);
            this.groupBox_autoconfig.Controls.Add(this.panel_septentrio);
            this.groupBox_autoconfig.Controls.Add(this.panel_ubloxoptions);
            resources.ApplyResources(this.groupBox_autoconfig, "groupBox_autoconfig");
            this.groupBox_autoconfig.Name = "groupBox_autoconfig";
            this.groupBox_autoconfig.TabStop = false;
            // 
            // panel_um982
            // 
            this.panel_um982.Controls.Add(this.label22);
            resources.ApplyResources(this.panel_um982, "panel_um982");
            this.panel_um982.Name = "panel_um982";
            // 
            // panel_septentrio
            // 
            this.panel_septentrio.Controls.Add(this.label21);
            this.panel_septentrio.Controls.Add(this.chk_septentriogalileo);
            this.panel_septentrio.Controls.Add(this.chk_septentriobeidou);
            this.panel_septentrio.Controls.Add(this.chk_septentrioglonass);
            this.panel_septentrio.Controls.Add(this.chk_septentriogps);
            this.panel_septentrio.Controls.Add(this.input_septentriortcminterval);
            this.panel_septentrio.Controls.Add(this.button_septentriortcminterval);
            this.panel_septentrio.Controls.Add(this.label20);
            this.panel_septentrio.Controls.Add(this.label19);
            this.panel_septentrio.Controls.Add(this.cmb_septentriortcmamount);
            this.panel_septentrio.Controls.Add(this.input_septentriofixedlongitude);
            this.panel_septentrio.Controls.Add(this.chk_septentriofixedposition);
            this.panel_septentrio.Controls.Add(this.label14);
            this.panel_septentrio.Controls.Add(this.input_septentriofixedatitude);
            this.panel_septentrio.Controls.Add(this.label17);
            this.panel_septentrio.Controls.Add(this.label18);
            this.panel_septentrio.Controls.Add(this.button_septentriosetposition);
            this.panel_septentrio.Controls.Add(this.input_septentriofixedaltitude);
            resources.ApplyResources(this.panel_septentrio, "panel_septentrio");
            this.panel_septentrio.Name = "panel_septentrio";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            this.toolTip1.SetToolTip(this.label21, resources.GetString("label21.ToolTip"));
            // 
            // chk_septentriogalileo
            // 
            resources.ApplyResources(this.chk_septentriogalileo, "chk_septentriogalileo");
            this.chk_septentriogalileo.Checked = true;
            this.chk_septentriogalileo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_septentriogalileo.Name = "chk_septentriogalileo";
            this.toolTip1.SetToolTip(this.chk_septentriogalileo, resources.GetString("chk_septentriogalileo.ToolTip"));
            this.chk_septentriogalileo.UseVisualStyleBackColor = true;
            this.chk_septentriogalileo.Click += new System.EventHandler(this.chk_septentrioconstellation_Click);
            // 
            // chk_septentriobeidou
            // 
            resources.ApplyResources(this.chk_septentriobeidou, "chk_septentriobeidou");
            this.chk_septentriobeidou.Checked = true;
            this.chk_septentriobeidou.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_septentriobeidou.Name = "chk_septentriobeidou";
            this.toolTip1.SetToolTip(this.chk_septentriobeidou, resources.GetString("chk_septentriobeidou.ToolTip"));
            this.chk_septentriobeidou.UseVisualStyleBackColor = true;
            this.chk_septentriobeidou.Click += new System.EventHandler(this.chk_septentrioconstellation_Click);
            // 
            // chk_septentrioglonass
            // 
            resources.ApplyResources(this.chk_septentrioglonass, "chk_septentrioglonass");
            this.chk_septentrioglonass.Checked = true;
            this.chk_septentrioglonass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_septentrioglonass.Name = "chk_septentrioglonass";
            this.toolTip1.SetToolTip(this.chk_septentrioglonass, resources.GetString("chk_septentrioglonass.ToolTip"));
            this.chk_septentrioglonass.UseVisualStyleBackColor = true;
            this.chk_septentrioglonass.Click += new System.EventHandler(this.chk_septentrioconstellation_Click);
            // 
            // chk_septentriogps
            // 
            resources.ApplyResources(this.chk_septentriogps, "chk_septentriogps");
            this.chk_septentriogps.Checked = true;
            this.chk_septentriogps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_septentriogps.Name = "chk_septentriogps";
            this.toolTip1.SetToolTip(this.chk_septentriogps, resources.GetString("chk_septentriogps.ToolTip"));
            this.chk_septentriogps.UseVisualStyleBackColor = true;
            this.chk_septentriogps.Click += new System.EventHandler(this.chk_septentrioconstellation_Click);
            // 
            // input_septentriortcminterval
            // 
            this.input_septentriortcminterval.DecimalPlaces = 1;
            this.input_septentriortcminterval.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.input_septentriortcminterval, "input_septentriortcminterval");
            this.input_septentriortcminterval.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.input_septentriortcminterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.input_septentriortcminterval.Name = "input_septentriortcminterval";
            this.input_septentriortcminterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            this.toolTip1.SetToolTip(this.label20, resources.GetString("label20.ToolTip"));
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            this.toolTip1.SetToolTip(this.label19, resources.GetString("label19.ToolTip"));
            // 
            // cmb_septentriortcmamount
            // 
            this.cmb_septentriortcmamount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_septentriortcmamount.FormattingEnabled = true;
            this.cmb_septentriortcmamount.Items.AddRange(new object[] {
            resources.GetString("cmb_septentriortcmamount.Items"),
            resources.GetString("cmb_septentriortcmamount.Items1"),
            resources.GetString("cmb_septentriortcmamount.Items2")});
            resources.ApplyResources(this.cmb_septentriortcmamount, "cmb_septentriortcmamount");
            this.cmb_septentriortcmamount.Name = "cmb_septentriortcmamount";
            this.cmb_septentriortcmamount.SelectedIndexChanged += new System.EventHandler(this.cmb_septentriortcmamount_SelectedIndexChanged);
            // 
            // input_septentriofixedlongitude
            // 
            resources.ApplyResources(this.input_septentriofixedlongitude, "input_septentriofixedlongitude");
            this.input_septentriofixedlongitude.Name = "input_septentriofixedlongitude";
            // 
            // chk_septentriofixedposition
            // 
            resources.ApplyResources(this.chk_septentriofixedposition, "chk_septentriofixedposition");
            this.chk_septentriofixedposition.Name = "chk_septentriofixedposition";
            this.toolTip1.SetToolTip(this.chk_septentriofixedposition, resources.GetString("chk_septentriofixedposition.ToolTip"));
            this.chk_septentriofixedposition.UseVisualStyleBackColor = true;
            this.chk_septentriofixedposition.Click += new System.EventHandler(this.chk_septentriofixedposition_Click);
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // input_septentriofixedatitude
            // 
            resources.ApplyResources(this.input_septentriofixedatitude, "input_septentriofixedatitude");
            this.input_septentriofixedatitude.Name = "input_septentriofixedatitude";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // input_septentriofixedaltitude
            // 
            resources.ApplyResources(this.input_septentriofixedaltitude, "input_septentriofixedaltitude");
            this.input_septentriofixedaltitude.Name = "input_septentriofixedaltitude";
            // 
            // panel_ubloxoptions
            // 
            this.panel_ubloxoptions.Controls.Add(this.panel2);
            this.panel_ubloxoptions.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.panel_ubloxoptions, "panel_ubloxoptions");
            this.panel_ubloxoptions.Name = "panel_ubloxoptions";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.but_restartsvin);
            this.panel2.Controls.Add(this.chk_m8p_130p);
            this.panel2.Controls.Add(this.but_save_basepos);
            this.panel2.Controls.Add(this.dg_basepos);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txt_surveyinAcc);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txt_surveyinDur);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // chk_m8p_130p
            // 
            resources.ApplyResources(this.chk_m8p_130p, "chk_m8p_130p");
            this.chk_m8p_130p.Name = "chk_m8p_130p";
            this.toolTip1.SetToolTip(this.chk_m8p_130p, resources.GetString("chk_m8p_130p.ToolTip"));
            this.chk_m8p_130p.UseVisualStyleBackColor = true;
            this.chk_m8p_130p.CheckedChanged += new System.EventHandler(this.chk_m8p_130p_CheckedChanged);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_svin);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            resources.ApplyResources(this.groupBox1, "groupBox1");
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
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // chk_sendgga
            // 
            resources.ApplyResources(this.chk_sendgga, "chk_sendgga");
            this.chk_sendgga.Checked = true;
            this.chk_sendgga.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_sendgga.Name = "chk_sendgga";
            this.toolTip1.SetToolTip(this.chk_sendgga, resources.GetString("chk_sendgga.ToolTip"));
            this.chk_sendgga.UseVisualStyleBackColor = true;
            // 
            // check_sendntripv1
            // 
            resources.ApplyResources(this.check_sendntripv1, "check_sendntripv1");
            this.check_sendntripv1.Name = "check_sendntripv1";
            this.toolTip1.SetToolTip(this.check_sendntripv1, resources.GetString("check_sendntripv1.ToolTip"));
            this.check_sendntripv1.UseVisualStyleBackColor = true;
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
            this.labelmsgseen.Click += new System.EventHandler(this.labelmsgseen_Click);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.groupBox_autoconfig);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelGall);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label14BDS);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.labelglonass);
            this.groupBox2.Controls.Add(this.labelgps);
            this.groupBox2.Controls.Add(this.labelbase);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.lbl_status3);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // labelGall
            // 
            this.labelGall.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.labelGall, "labelGall");
            this.labelGall.Name = "labelGall";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // label14BDS
            // 
            this.label14BDS.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.label14BDS, "label14BDS");
            this.label14BDS.Name = "label14BDS";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // labelglonass
            // 
            this.labelglonass.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.labelglonass, "labelglonass");
            this.labelglonass.Name = "labelglonass";
            // 
            // labelgps
            // 
            this.labelgps.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.labelgps, "labelgps");
            this.labelgps.Name = "labelgps";
            // 
            // labelbase
            // 
            this.labelbase.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.labelbase, "labelbase");
            this.labelbase.Name = "labelbase";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.lbl_status1);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.lbl_status2);
            this.groupBox3.Controls.Add(this.labelmsgseen);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // comboBoxConfigType
            // 
            this.comboBoxConfigType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxConfigType.FormattingEnabled = true;
            this.comboBoxConfigType.Items.AddRange(new object[] {
            resources.GetString("comboBoxConfigType.Items"),
            resources.GetString("comboBoxConfigType.Items1"),
            resources.GetString("comboBoxConfigType.Items2")});
            resources.ApplyResources(this.comboBoxConfigType, "comboBoxConfigType");
            this.comboBoxConfigType.Name = "comboBoxConfigType";
            this.comboBoxConfigType.SelectedIndexChanged += new System.EventHandler(this.comboBoxConfigType_SelectedIndexChanged);
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // button_septentriortcminterval
            // 
            resources.ApplyResources(this.button_septentriortcminterval, "button_septentriortcminterval");
            this.button_septentriortcminterval.Name = "button_septentriortcminterval";
            this.button_septentriortcminterval.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.button_septentriortcminterval.UseVisualStyleBackColor = true;
            this.button_septentriortcminterval.Click += new System.EventHandler(this.button_septentriortcminterval_Click);
            // 
            // button_septentriosetposition
            // 
            resources.ApplyResources(this.button_septentriosetposition, "button_septentriosetposition");
            this.button_septentriosetposition.Name = "button_septentriosetposition";
            this.button_septentriosetposition.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.button_septentriosetposition.UseVisualStyleBackColor = true;
            this.button_septentriosetposition.Click += new System.EventHandler(this.button_septentriosetposition_Click);
            // 
            // but_restartsvin
            // 
            resources.ApplyResources(this.but_restartsvin, "but_restartsvin");
            this.but_restartsvin.Name = "but_restartsvin";
            this.but_restartsvin.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.but_restartsvin, resources.GetString("but_restartsvin.ToolTip"));
            this.but_restartsvin.UseVisualStyleBackColor = true;
            this.but_restartsvin.Click += new System.EventHandler(this.but_restartsvin_Click);
            // 
            // but_save_basepos
            // 
            resources.ApplyResources(this.but_save_basepos, "but_save_basepos");
            this.but_save_basepos.Name = "but_save_basepos";
            this.but_save_basepos.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.but_save_basepos, resources.GetString("but_save_basepos.ToolTip"));
            this.but_save_basepos.UseVisualStyleBackColor = true;
            this.but_save_basepos.Click += new System.EventHandler(this.but_save_basepos_Click);
            // 
            // BUT_connect
            // 
            resources.ApplyResources(this.BUT_connect, "BUT_connect");
            this.BUT_connect.Name = "BUT_connect";
            this.BUT_connect.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
            // 
            // myGMAP1
            // 
            resources.ApplyResources(this.myGMAP1, "myGMAP1");
            this.myGMAP1.Bearing = 0F;
            this.myGMAP1.CanDragMap = true;
            this.myGMAP1.EmptyTileColor = System.Drawing.Color.Navy;
            this.myGMAP1.GrayScaleMode = false;
            this.myGMAP1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.myGMAP1.HoldInvalidation = false;
            this.myGMAP1.LevelsKeepInMemmory = 5;
            this.myGMAP1.MarkersEnabled = true;
            this.myGMAP1.MaxZoom = 2;
            this.myGMAP1.MinZoom = 2;
            this.myGMAP1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.myGMAP1.Name = "myGMAP1";
            this.myGMAP1.NegativeMode = false;
            this.myGMAP1.PolygonsEnabled = true;
            this.myGMAP1.RetryLoadTile = 0;
            this.myGMAP1.RoutesEnabled = true;
            this.myGMAP1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.myGMAP1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.myGMAP1.ShowTileGridLines = false;
            this.myGMAP1.Zoom = 0D;
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
            // ConfigSerialInjectGPS
            // 
            this.Controls.Add(this.comboBoxConfigType);
            this.Controls.Add(this.check_sendntripv1);
            this.Controls.Add(this.chk_sendgga);
            this.Controls.Add(this.myGMAP1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.chk_autoconfig);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.chk_rtcmmsg);
            this.Controls.Add(this.CMB_baudrate);
            this.Controls.Add(this.BUT_connect);
            this.Controls.Add(this.CMB_serialport);
            this.Name = "ConfigSerialInjectGPS";
            resources.ApplyResources(this, "$this");
            this.groupBox_autoconfig.ResumeLayout(false);
            this.panel_um982.ResumeLayout(false);
            this.panel_um982.PerformLayout();
            this.panel_septentrio.ResumeLayout(false);
            this.panel_septentrio.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.input_septentriortcminterval)).EndInit();
            this.panel_ubloxoptions.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_basepos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox CMB_serialport;
        private Controls.MyButton BUT_connect;
        private System.Windows.Forms.ComboBox CMB_baudrate;
        private System.Windows.Forms.Label lbl_status1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox chk_rtcmmsg;
        private System.Windows.Forms.Label lbl_svin;
        private System.Windows.Forms.CheckBox chk_autoconfig;
        private System.Windows.Forms.GroupBox groupBox_autoconfig;
        private System.Windows.Forms.TextBox txt_surveyinAcc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_surveyinDur;
        private Controls.MyButton but_save_basepos;
        private Controls.MyDataGridView dg_basepos;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chk_m8p_130p;
        private System.Windows.Forms.ToolTip toolTip1;
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
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelglonass;
        private System.Windows.Forms.Label labelgps;
        private System.Windows.Forms.Label labelbase;
        private System.Windows.Forms.Label label14BDS;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Lat;
        private System.Windows.Forms.DataGridViewTextBoxColumn Long;
        private System.Windows.Forms.DataGridViewTextBoxColumn Alt;
        private System.Windows.Forms.DataGridViewTextBoxColumn BaseName1;
        private System.Windows.Forms.DataGridViewButtonColumn Use;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
        private System.Windows.Forms.Label labelGall;
        private System.Windows.Forms.Label label16;
        private Controls.myGMAP myGMAP1;
        private System.Windows.Forms.CheckBox chk_sendgga;
        private System.Windows.Forms.CheckBox check_sendntripv1;
        private System.Windows.Forms.ComboBox comboBoxConfigType;
        private System.Windows.Forms.Panel panel_ubloxoptions;
        private System.Windows.Forms.Panel panel_septentrio;
        private System.Windows.Forms.TextBox input_septentriofixedatitude;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox input_septentriofixedaltitude;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox input_septentriofixedlongitude;
        private System.Windows.Forms.CheckBox chk_septentriofixedposition;
        private Controls.MyButton button_septentriosetposition;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.ComboBox cmb_septentriortcmamount;
        private Controls.MyButton button_septentriortcminterval;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.NumericUpDown input_septentriortcminterval;
        private System.Windows.Forms.CheckBox chk_septentriogalileo;
        private System.Windows.Forms.CheckBox chk_septentriobeidou;
        private System.Windows.Forms.CheckBox chk_septentrioglonass;
        private System.Windows.Forms.CheckBox chk_septentriogps;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Panel panel_um982;
        private System.Windows.Forms.Label label22;
    }
}