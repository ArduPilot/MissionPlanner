namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigUAVCAN
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.but_slcanmode1 = new MissionPlanner.Controls.MyButton();
            this.myDataGridView1 = new MissionPlanner.Controls.MyDataGridView();
            this.uAVCANModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.but_slcanmode2 = new MissionPlanner.Controls.MyButton();
            this.but_uavcaninspector = new MissionPlanner.Controls.MyButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox3 = new System.Windows.Forms.TextBox();
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
            this.Text = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chk_canonclose = new System.Windows.Forms.CheckBox();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.healthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uptimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hardwareVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoftwareVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoftwareCRC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Parameter = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Restart = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uAVCANModelBindingSource)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGDebug)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 20);
            this.label6.TabIndex = 80;
            this.label6.Text = "UAVCAN";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Location = new System.Drawing.Point(-1, 18);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(765, 5);
            this.groupBox5.TabIndex = 79;
            this.groupBox5.TabStop = false;
            // 
            // but_slcanmode1
            // 
            this.but_slcanmode1.Location = new System.Drawing.Point(7, 29);
            this.but_slcanmode1.Name = "but_slcanmode1";
            this.but_slcanmode1.Size = new System.Drawing.Size(125, 23);
            this.but_slcanmode1.TabIndex = 82;
            this.but_slcanmode1.Text = "SLCan Mode CAN1";
            this.but_slcanmode1.UseVisualStyleBackColor = true;
            this.but_slcanmode1.Click += new System.EventHandler(this.but_slcanmode_Click);
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
            this.updateDataGridViewTextBoxColumn,
            this.Parameter,
            this.Restart});
            this.myDataGridView1.DataSource = this.uAVCANModelBindingSource;
            this.myDataGridView1.Location = new System.Drawing.Point(7, 58);
            this.myDataGridView1.Name = "myDataGridView1";
            this.myDataGridView1.ReadOnly = true;
            this.myDataGridView1.Size = new System.Drawing.Size(757, 253);
            this.myDataGridView1.TabIndex = 1;
            this.myDataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.myDataGridView1_CellClick);
            this.myDataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.myDataGridView1_RowEnter);
            this.myDataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.myDataGridView1_RowsAdded);
            // 
            // uAVCANModelBindingSource
            // 
            this.uAVCANModelBindingSource.AllowNew = true;
            this.uAVCANModelBindingSource.DataSource = typeof(MissionPlanner.GCSViews.ConfigurationView.UAVCANModel);
            this.uAVCANModelBindingSource.CurrentChanged += new System.EventHandler(this.uAVCANModelBindingSource_CurrentChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(350, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(359, 26);
            this.label1.TabIndex = 83;
            this.label1.Text = "After enabling SLCAN, you will no longer be able to connect via MAVLINK.\r\nYou mus" +
    "t leave this screen and wait 2 seconds before connecting again\r\n";
            // 
            // but_slcanmode2
            // 
            this.but_slcanmode2.Location = new System.Drawing.Point(138, 29);
            this.but_slcanmode2.Name = "but_slcanmode2";
            this.but_slcanmode2.Size = new System.Drawing.Size(125, 23);
            this.but_slcanmode2.TabIndex = 84;
            this.but_slcanmode2.Text = "SLCan Mode CAN2";
            this.but_slcanmode2.UseVisualStyleBackColor = true;
            this.but_slcanmode2.Click += new System.EventHandler(this.but_slcanmode2_Click);
            // 
            // but_uavcaninspector
            // 
            this.but_uavcaninspector.Location = new System.Drawing.Point(269, 29);
            this.but_uavcaninspector.Name = "but_uavcaninspector";
            this.but_uavcaninspector.Size = new System.Drawing.Size(75, 23);
            this.but_uavcaninspector.TabIndex = 85;
            this.but_uavcaninspector.Text = "Inspector";
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 317);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(757, 137);
            this.tableLayoutPanel1.TabIndex = 86;
            // 
            // textBox3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox3, 2);
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "HardwareUID", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "X"));
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.Location = new System.Drawing.Point(331, 111);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(423, 20);
            this.textBox3.TabIndex = 18;
            // 
            // textBox13
            // 
            this.textBox13.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "HardwareVersion", true));
            this.textBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox13.Location = new System.Drawing.Point(137, 111);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(188, 20);
            this.textBox13.TabIndex = 17;
            // 
            // textBox11
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox11, 2);
            this.textBox11.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "SoftwareCRC", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "X"));
            this.textBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox11.Location = new System.Drawing.Point(331, 84);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(423, 20);
            this.textBox11.TabIndex = 15;
            // 
            // textBox10
            // 
            this.textBox10.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "SoftwareVersion", true));
            this.textBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox10.Location = new System.Drawing.Point(137, 84);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(188, 20);
            this.textBox10.TabIndex = 14;
            // 
            // textBox9
            // 
            this.textBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox9.Location = new System.Drawing.Point(545, 57);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(209, 20);
            this.textBox9.TabIndex = 13;
            // 
            // textBox7
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox7, 2);
            this.textBox7.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "VSC", true));
            this.textBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox7.Location = new System.Drawing.Point(137, 57);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(402, 20);
            this.textBox7.TabIndex = 11;
            // 
            // textBox6
            // 
            this.textBox6.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "Uptime", true));
            this.textBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox6.Location = new System.Drawing.Point(545, 30);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(209, 20);
            this.textBox6.TabIndex = 10;
            // 
            // textBox5
            // 
            this.textBox5.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "Health", true));
            this.textBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox5.Location = new System.Drawing.Point(331, 30);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(208, 20);
            this.textBox5.TabIndex = 9;
            // 
            // textBox4
            // 
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "Mode", true));
            this.textBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox4.Location = new System.Drawing.Point(137, 30);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(188, 20);
            this.textBox4.TabIndex = 8;
            // 
            // textBox2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox2, 2);
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "Name", true));
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(331, 3);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(423, 20);
            this.textBox2.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 27);
            this.label2.TabIndex = 0;
            this.label2.Text = "Node ID / Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 27);
            this.label3.TabIndex = 1;
            this.label3.Text = "Mode / Health / Uptime";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 27);
            this.label4.TabIndex = 2;
            this.label4.Text = "Vendor-specific code";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 27);
            this.label5.TabIndex = 3;
            this.label5.Text = "Software version/CRC64";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 29);
            this.label7.TabIndex = 4;
            this.label7.Text = "Hardware version/UID";
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uAVCANModelBindingSource, "ID", true));
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(137, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(188, 20);
            this.textBox1.TabIndex = 5;
            // 
            // chk_log
            // 
            this.chk_log.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chk_log.AutoSize = true;
            this.chk_log.Location = new System.Drawing.Point(720, 3);
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
            this.Text});
            this.DGDebug.Location = new System.Drawing.Point(7, 458);
            this.DGDebug.Name = "DGDebug";
            this.DGDebug.Size = new System.Drawing.Size(757, 144);
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
            // Text
            // 
            this.Text.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Text.HeaderText = "Text";
            this.Text.Name = "Text";
            this.Text.ReadOnly = true;
            // 
            // chk_canonclose
            // 
            this.chk_canonclose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chk_canonclose.AutoSize = true;
            this.chk_canonclose.Checked = true;
            this.chk_canonclose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_canonclose.Location = new System.Drawing.Point(583, 3);
            this.chk_canonclose.Name = "chk_canonclose";
            this.chk_canonclose.Size = new System.Drawing.Size(131, 17);
            this.chk_canonclose.TabIndex = 90;
            this.chk_canonclose.Text = "Exit SLCAN on leave?";
            this.chk_canonclose.UseVisualStyleBackColor = true;
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
            dataGridViewCellStyle2.Format = "X";
            dataGridViewCellStyle2.NullValue = null;
            this.SoftwareCRC.DefaultCellStyle = dataGridViewCellStyle2;
            this.SoftwareCRC.HeaderText = "SW CRC";
            this.SoftwareCRC.Name = "SoftwareCRC";
            this.SoftwareCRC.ReadOnly = true;
            this.SoftwareCRC.Width = 110;
            // 
            // updateDataGridViewTextBoxColumn
            // 
            this.updateDataGridViewTextBoxColumn.HeaderText = "Update Firmware";
            this.updateDataGridViewTextBoxColumn.Name = "updateDataGridViewTextBoxColumn";
            this.updateDataGridViewTextBoxColumn.ReadOnly = true;
            this.updateDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.updateDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.updateDataGridViewTextBoxColumn.Text = "Update Firmware";
            this.updateDataGridViewTextBoxColumn.Width = 60;
            // 
            // Parameter
            // 
            this.Parameter.HeaderText = "Parameter";
            this.Parameter.Name = "Parameter";
            this.Parameter.ReadOnly = true;
            this.Parameter.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Parameter.Text = "Parameter";
            this.Parameter.Width = 65;
            // 
            // Restart
            // 
            this.Restart.HeaderText = "Restart";
            this.Restart.Name = "Restart";
            this.Restart.ReadOnly = true;
            this.Restart.Width = 50;
            // 
            // ConfigUAVCAN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chk_canonclose);
            this.Controls.Add(this.DGDebug);
            this.Controls.Add(this.chk_log);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.but_uavcaninspector);
            this.Controls.Add(this.but_slcanmode2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.but_slcanmode1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.myDataGridView1);
            this.Name = "ConfigUAVCAN";
            this.Size = new System.Drawing.Size(767, 605);
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uAVCANModelBindingSource)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGDebug)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyDataGridView myDataGridView1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private Controls.MyButton but_slcanmode1;
        private System.Windows.Forms.Label label1;
        private Controls.MyButton but_slcanmode2;
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
        private System.Windows.Forms.DataGridViewTextBoxColumn Node;
        private System.Windows.Forms.DataGridViewTextBoxColumn Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewTextBoxColumn Text;
        private System.Windows.Forms.CheckBox chk_canonclose;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn modeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn healthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn uptimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hardwareVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoftwareVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoftwareCRC;
        private System.Windows.Forms.DataGridViewButtonColumn updateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn Parameter;
        private System.Windows.Forms.DataGridViewButtonColumn Restart;
    }
}
