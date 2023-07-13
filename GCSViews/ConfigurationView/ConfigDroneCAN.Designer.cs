﻿namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigDroneCAN
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.but_slcanmode1 = new MissionPlanner.Controls.MyButton();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menu_parameters = new System.Windows.Forms.MenuItem();
            this.menu_restart = new System.Windows.Forms.MenuItem();
            this.menu_update = new System.Windows.Forms.MenuItem();
            this.menu_updatebeta = new System.Windows.Forms.MenuItem();
            this.menu_passthrough = new System.Windows.Forms.MenuItem();
            this.menu_passthrough4 = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.but_mavlinkcanmode2 = new MissionPlanner.Controls.MyButton();
            this.but_uavcaninspector = new MissionPlanner.Controls.MyButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.uAVCANModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.chk_log = new System.Windows.Forms.CheckBox();
            this.DGDebug = new System.Windows.Forms.DataGridView();
            this.Node = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UAVText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chk_canonclose = new System.Windows.Forms.CheckBox();
            this.but_mavlinkcanmode2_2 = new MissionPlanner.Controls.MyButton();
            this.but_filter = new MissionPlanner.Controls.MyButton();
            this.myDataGridView1 = new MissionPlanner.Controls.MyDataGridView();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.healthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uptimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hardwareVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoftwareVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoftwareCRC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Menu = new System.Windows.Forms.DataGridViewButtonColumn();
            this.but_stats = new MissionPlanner.Controls.MyButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uAVCANModelBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGDebug)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 20);
            this.label6.TabIndex = 80;
            this.label6.Text = "DroneCAN/UAVCAN";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Location = new System.Drawing.Point(0, 26);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(796, 5);
            this.groupBox5.TabIndex = 79;
            this.groupBox5.TabStop = false;
            // 
            // but_slcanmode1
            // 
            this.but_slcanmode1.Location = new System.Drawing.Point(7, 35);
            this.but_slcanmode1.Name = "but_slcanmode1";
            this.but_slcanmode1.Size = new System.Drawing.Size(83, 23);
            this.but_slcanmode1.TabIndex = 82;
            this.but_slcanmode1.Text = "SLCan Direct";
            this.but_slcanmode1.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_slcanmode1.UseVisualStyleBackColor = true;
            this.but_slcanmode1.Click += new System.EventHandler(this.but_slcandirect_Click);
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_parameters,
            this.menu_restart,
            this.menu_update,
            this.menu_updatebeta,
            this.menu_passthrough,
            this.menu_passthrough4});
            // 
            // menu_parameters
            // 
            this.menu_parameters.Index = 0;
            this.menu_parameters.Text = "Parameters";
            this.menu_parameters.Click += new System.EventHandler(this.menu_parameters_Click);
            // 
            // menu_restart
            // 
            this.menu_restart.Index = 1;
            this.menu_restart.Text = "Restart";
            this.menu_restart.Click += new System.EventHandler(this.menu_restart_Click);
            // 
            // menu_update
            // 
            this.menu_update.Index = 2;
            this.menu_update.Text = "Update";
            this.menu_update.Click += new System.EventHandler(this.menu_update_Click);
            // 
            // menu_updatebeta
            // 
            this.menu_updatebeta.Index = 3;
            this.menu_updatebeta.Text = "Update Beta";
            this.menu_updatebeta.Click += new System.EventHandler(this.menu_updatebeta_Click);
            // 
            // menu_passthrough
            // 
            this.menu_passthrough.Index = 4;
            this.menu_passthrough.RadioCheck = true;
            this.menu_passthrough.Text = "CANPassThrough Here3";
            this.menu_passthrough.Click += new System.EventHandler(this.menu_passthrough_Click);
            // 
            // menu_passthrough4
            // 
            this.menu_passthrough4.Index = 5;
            this.menu_passthrough4.Text = "CANPassThough Here3+/4";
            this.menu_passthrough4.Click += new System.EventHandler(this.menu_passthrough4_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(401, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(359, 26);
            this.label1.TabIndex = 83;
            this.label1.Text = "After enabling SLCAN, you will no longer be able to connect via MAVLINK.\r\nYou mus" +
    "t leave this screen and wait 2 seconds before connecting again\r\n";
            // 
            // but_mavlinkcanmode2
            // 
            this.but_mavlinkcanmode2.Location = new System.Drawing.Point(96, 35);
            this.but_mavlinkcanmode2.Name = "but_mavlinkcanmode2";
            this.but_mavlinkcanmode2.Size = new System.Drawing.Size(91, 23);
            this.but_mavlinkcanmode2.TabIndex = 84;
            this.but_mavlinkcanmode2.Text = "MAVlink-CAN1";
            this.but_mavlinkcanmode2.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_mavlinkcanmode2.UseVisualStyleBackColor = true;
            this.but_mavlinkcanmode2.Click += new System.EventHandler(this.but_slcanmavlink_Click);
            // 
            // but_uavcaninspector
            // 
            this.but_uavcaninspector.Location = new System.Drawing.Point(338, 35);
            this.but_uavcaninspector.Name = "but_uavcaninspector";
            this.but_uavcaninspector.Size = new System.Drawing.Size(57, 23);
            this.but_uavcaninspector.TabIndex = 85;
            this.but_uavcaninspector.Text = "Inspector";
            this.but_uavcaninspector.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_uavcaninspector.UseVisualStyleBackColor = true;
            this.but_uavcaninspector.Click += new System.EventHandler(this.But_uavcaninspector_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.70701F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.60509F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.24511F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.24511F));
            this.tableLayoutPanel1.Controls.Add(this.textBox3, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBox13, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBox11, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox10, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox9, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox7, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox6, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox5, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox4, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 324);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(788, 136);
            this.tableLayoutPanel1.TabIndex = 86;
            // 
            // textBox3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox3, 2);
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "HardwareUID", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "X"));
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.Location = new System.Drawing.Point(344, 111);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(441, 20);
            this.textBox3.TabIndex = 18;
            // 
            // uAVCANModelBindingSource
            // 
            this.uAVCANModelBindingSource.AllowNew = true;
            this.uAVCANModelBindingSource.DataSource = typeof(MissionPlanner.GCSViews.ConfigurationView.DroneCANModel);
            // 
            // textBox13
            // 
            this.textBox13.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "HardwareVersion", true));
            this.textBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox13.Location = new System.Drawing.Point(142, 111);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(196, 20);
            this.textBox13.TabIndex = 17;
            // 
            // textBox11
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox11, 2);
            this.textBox11.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "SoftwareCRC", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "X"));
            this.textBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox11.Location = new System.Drawing.Point(344, 84);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(441, 20);
            this.textBox11.TabIndex = 15;
            // 
            // textBox10
            // 
            this.textBox10.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "SoftwareVersion", true));
            this.textBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox10.Location = new System.Drawing.Point(142, 84);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(196, 20);
            this.textBox10.TabIndex = 14;
            // 
            // textBox9
            // 
            this.textBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox9.Location = new System.Drawing.Point(567, 57);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(218, 20);
            this.textBox9.TabIndex = 13;
            // 
            // textBox7
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox7, 2);
            this.textBox7.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "VSC", true));
            this.textBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox7.Location = new System.Drawing.Point(142, 57);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(419, 20);
            this.textBox7.TabIndex = 11;
            // 
            // textBox6
            // 
            this.textBox6.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "Uptime", true));
            this.textBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox6.Location = new System.Drawing.Point(567, 30);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(218, 20);
            this.textBox6.TabIndex = 10;
            // 
            // textBox5
            // 
            this.textBox5.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "Health", true));
            this.textBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox5.Location = new System.Drawing.Point(344, 30);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(217, 20);
            this.textBox5.TabIndex = 9;
            // 
            // textBox4
            // 
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "Mode", true));
            this.textBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox4.Location = new System.Drawing.Point(142, 30);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(196, 20);
            this.textBox4.TabIndex = 8;
            // 
            // textBox2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox2, 2);
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "Name", true));
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(344, 3);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(441, 20);
            this.textBox2.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 27);
            this.label2.TabIndex = 0;
            this.label2.Text = "Node ID / Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 27);
            this.label3.TabIndex = 1;
            this.label3.Text = "Mode / Health / Uptime";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 27);
            this.label4.TabIndex = 2;
            this.label4.Text = "Vendor-specific code";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(133, 27);
            this.label5.TabIndex = 3;
            this.label5.Text = "Software version/CRC64";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(133, 28);
            this.label7.TabIndex = 4;
            this.label7.Text = "Hardware version/UID";
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "ID", true));
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(142, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(196, 20);
            this.textBox1.TabIndex = 5;
            // 
            // chk_log
            // 
            this.chk_log.AutoSize = true;
            this.chk_log.Location = new System.Drawing.Point(753, 3);
            this.chk_log.Name = "chk_log";
            this.chk_log.Size = new System.Drawing.Size(44, 17);
            this.chk_log.TabIndex = 87;
            this.chk_log.Text = "Log";
            this.chk_log.UseVisualStyleBackColor = true;
            // 
            // DGDebug
            // 
            this.DGDebug.AllowUserToAddRows = false;
            this.DGDebug.AllowUserToDeleteRows = false;
            this.DGDebug.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGDebug.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGDebug.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Node,
            this.Level,
            this.Source,
            this.UAVText});
            this.DGDebug.Location = new System.Drawing.Point(7, 465);
            this.DGDebug.Name = "DGDebug";
            this.DGDebug.Size = new System.Drawing.Size(788, 144);
            this.DGDebug.TabIndex = 89;
            // 
            // Node
            // 
            this.Node.HeaderText = "Node";
            this.Node.Name = "Node";
            this.Node.ReadOnly = true;
            this.Node.Width = 40;
            // 
            // Level
            // 
            this.Level.HeaderText = "Level";
            this.Level.Name = "Level";
            this.Level.ReadOnly = true;
            this.Level.Width = 40;
            // 
            // Source
            // 
            this.Source.HeaderText = "Source";
            this.Source.Name = "Source";
            this.Source.ReadOnly = true;
            this.Source.Width = 50;
            // 
            // UAVText
            // 
            this.UAVText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.UAVText.HeaderText = "Text";
            this.UAVText.Name = "UAVText";
            this.UAVText.ReadOnly = true;
            // 
            // chk_canonclose
            // 
            this.chk_canonclose.AutoSize = true;
            this.chk_canonclose.Checked = true;
            this.chk_canonclose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_canonclose.Location = new System.Drawing.Point(608, 3);
            this.chk_canonclose.Name = "chk_canonclose";
            this.chk_canonclose.Size = new System.Drawing.Size(131, 17);
            this.chk_canonclose.TabIndex = 90;
            this.chk_canonclose.Text = "Exit SLCAN on leave?";
            this.chk_canonclose.UseVisualStyleBackColor = true;
            // 
            // but_mavlinkcanmode2_2
            // 
            this.but_mavlinkcanmode2_2.Location = new System.Drawing.Point(193, 35);
            this.but_mavlinkcanmode2_2.Name = "but_mavlinkcanmode2_2";
            this.but_mavlinkcanmode2_2.Size = new System.Drawing.Size(91, 23);
            this.but_mavlinkcanmode2_2.TabIndex = 91;
            this.but_mavlinkcanmode2_2.Text = "MAVlink-CAN2";
            this.but_mavlinkcanmode2_2.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_mavlinkcanmode2_2.UseVisualStyleBackColor = true;
            this.but_mavlinkcanmode2_2.Click += new System.EventHandler(this.but_slcanmode2_2_Click);
            // 
            // but_filter
            // 
            this.but_filter.Location = new System.Drawing.Point(290, 35);
            this.but_filter.Name = "but_filter";
            this.but_filter.Size = new System.Drawing.Size(42, 23);
            this.but_filter.TabIndex = 92;
            this.but_filter.Text = "Filter";
            this.but_filter.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_filter.UseVisualStyleBackColor = true;
            this.but_filter.Click += new System.EventHandler(this.but_filter_Click);
            // 
            // myDataGridView1
            // 
            this.myDataGridView1.AllowUserToAddRows = false;
            this.myDataGridView1.AllowUserToDeleteRows = false;
            this.myDataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myDataGridView1.AutoGenerateColumns = false;
            this.myDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.myDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.modeDataGridViewTextBoxColumn,
            this.healthDataGridViewTextBoxColumn,
            this.uptimeDataGridViewTextBoxColumn,
            this.hardwareVersionDataGridViewTextBoxColumn,
            this.SoftwareVersion,
            this.SoftwareCRC,
            this.Menu});
            this.myDataGridView1.ContextMenu = this.contextMenu1;
            this.myDataGridView1.DataSource = this.uAVCANModelBindingSource;
            this.myDataGridView1.Location = new System.Drawing.Point(7, 89);
            this.myDataGridView1.Name = "myDataGridView1";
            this.myDataGridView1.ReadOnly = true;
            this.myDataGridView1.Size = new System.Drawing.Size(788, 229);
            this.myDataGridView1.TabIndex = 1;
            this.myDataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.myDataGridView1_CellClick);
            this.myDataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.myDataGridView1_RowEnter);
            this.myDataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.myDataGridView1_RowsAdded);
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            this.iDDataGridViewTextBoxColumn.Width = 40;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.Width = 110;
            // 
            // modeDataGridViewTextBoxColumn
            // 
            this.modeDataGridViewTextBoxColumn.DataPropertyName = "Mode";
            this.modeDataGridViewTextBoxColumn.HeaderText = "Mode";
            this.modeDataGridViewTextBoxColumn.Name = "modeDataGridViewTextBoxColumn";
            this.modeDataGridViewTextBoxColumn.ReadOnly = true;
            this.modeDataGridViewTextBoxColumn.Width = 90;
            // 
            // healthDataGridViewTextBoxColumn
            // 
            this.healthDataGridViewTextBoxColumn.DataPropertyName = "Health";
            this.healthDataGridViewTextBoxColumn.HeaderText = "Health";
            this.healthDataGridViewTextBoxColumn.Name = "healthDataGridViewTextBoxColumn";
            this.healthDataGridViewTextBoxColumn.ReadOnly = true;
            this.healthDataGridViewTextBoxColumn.Width = 43;
            // 
            // uptimeDataGridViewTextBoxColumn
            // 
            this.uptimeDataGridViewTextBoxColumn.DataPropertyName = "Uptime";
            this.uptimeDataGridViewTextBoxColumn.HeaderText = "Uptime";
            this.uptimeDataGridViewTextBoxColumn.Name = "uptimeDataGridViewTextBoxColumn";
            this.uptimeDataGridViewTextBoxColumn.ReadOnly = true;
            this.uptimeDataGridViewTextBoxColumn.Width = 60;
            // 
            // hardwareVersionDataGridViewTextBoxColumn
            // 
            this.hardwareVersionDataGridViewTextBoxColumn.DataPropertyName = "HardwareVersion";
            this.hardwareVersionDataGridViewTextBoxColumn.HeaderText = "HW Version";
            this.hardwareVersionDataGridViewTextBoxColumn.Name = "hardwareVersionDataGridViewTextBoxColumn";
            this.hardwareVersionDataGridViewTextBoxColumn.ReadOnly = true;
            this.hardwareVersionDataGridViewTextBoxColumn.Width = 50;
            // 
            // SoftwareVersion
            // 
            this.SoftwareVersion.DataPropertyName = "SoftwareVersion";
            this.SoftwareVersion.HeaderText = "SW Version";
            this.SoftwareVersion.Name = "SoftwareVersion";
            this.SoftwareVersion.ReadOnly = true;
            this.SoftwareVersion.Width = 80;
            // 
            // SoftwareCRC
            // 
            this.SoftwareCRC.DataPropertyName = "SoftwareCRC";
            dataGridViewCellStyle1.Format = "X";
            dataGridViewCellStyle1.NullValue = null;
            this.SoftwareCRC.DefaultCellStyle = dataGridViewCellStyle1;
            this.SoftwareCRC.HeaderText = "SW CRC";
            this.SoftwareCRC.Name = "SoftwareCRC";
            this.SoftwareCRC.ReadOnly = true;
            this.SoftwareCRC.Width = 110;
            // 
            // Menu
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "Menu";
            this.Menu.DefaultCellStyle = dataGridViewCellStyle2;
            this.Menu.HeaderText = "Menu";
            this.Menu.Name = "Menu";
            this.Menu.ReadOnly = true;
            this.Menu.Width = 50;
            // 
            // but_stats
            // 
            this.but_stats.Location = new System.Drawing.Point(290, 60);
            this.but_stats.Name = "but_stats";
            this.but_stats.Size = new System.Drawing.Size(42, 23);
            this.but_stats.TabIndex = 93;
            this.but_stats.Text = "Stats";
            this.but_stats.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_stats.UseVisualStyleBackColor = true;
            this.but_stats.Click += new System.EventHandler(this.but_stats_Click);
            // 
            // ConfigDroneCAN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.but_stats);
            this.Controls.Add(this.but_filter);
            this.Controls.Add(this.but_mavlinkcanmode2_2);
            this.Controls.Add(this.chk_canonclose);
            this.Controls.Add(this.DGDebug);
            this.Controls.Add(this.chk_log);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.but_uavcaninspector);
            this.Controls.Add(this.but_mavlinkcanmode2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.but_slcanmode1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.myDataGridView1);
            this.Name = "ConfigDroneCAN";
            this.Size = new System.Drawing.Size(798, 612);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uAVCANModelBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGDebug)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyDataGridView myDataGridView1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private Controls.MyButton but_slcanmode1;
        private System.Windows.Forms.Label label1;
        private Controls.MyButton but_mavlinkcanmode2;
        private Controls.MyButton but_uavcaninspector;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox13;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.CheckBox chk_log;
        public System.Windows.Forms.BindingSource uAVCANModelBindingSource;
        private System.Windows.Forms.DataGridView DGDebug;
        private System.Windows.Forms.CheckBox chk_canonclose;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menu_passthrough;
        private System.Windows.Forms.MenuItem menu_update;
        private System.Windows.Forms.MenuItem menu_parameters;
        private System.Windows.Forms.MenuItem menu_restart;
        private System.Windows.Forms.MenuItem menu_updatebeta;
        private System.Windows.Forms.DataGridViewTextBoxColumn Node;
        private System.Windows.Forms.DataGridViewTextBoxColumn Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewTextBoxColumn UAVText;
        private Controls.MyButton but_mavlinkcanmode2_2;
        private Controls.MyButton but_filter;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn modeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn healthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn uptimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hardwareVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoftwareVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoftwareCRC;
        private System.Windows.Forms.DataGridViewButtonColumn Menu;
        private System.Windows.Forms.MenuItem menu_passthrough4;
        private Controls.MyButton but_stats;
    }
}
