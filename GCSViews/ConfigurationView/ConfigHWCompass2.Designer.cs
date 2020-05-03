namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigHWCompass2
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
            this.myDataGridView1 = new MissionPlanner.Controls.MyDataGridView();
            this.devIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.busTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.busDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.devTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Up = new System.Windows.Forms.DataGridViewImageColumn();
            this.Down = new System.Windows.Forms.DataGridViewImageColumn();
            this.compassInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBoxonboardcalib = new System.Windows.Forms.GroupBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.mavlinkComboBoxfitness = new MissionPlanner.Controls.MavlinkComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.horizontalProgressBar3 = new MissionPlanner.Controls.HorizontalProgressBar();
            this.horizontalProgressBar2 = new MissionPlanner.Controls.HorizontalProgressBar();
            this.horizontalProgressBar1 = new MissionPlanner.Controls.HorizontalProgressBar();
            this.lbl_obmagresult = new System.Windows.Forms.TextBox();
            this.BUT_OBmagcalaccept = new MissionPlanner.Controls.MyButton();
            this.BUT_OBmagcalcancel = new MissionPlanner.Controls.MyButton();
            this.BUT_OBmagcalstart = new MissionPlanner.Controls.MyButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.mavlinkCheckBoxUseCompass1 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.mavlinkCheckBoxUseCompass3 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.mavlinkCheckBoxUseCompass2 = new MissionPlanner.Controls.MavlinkCheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.but_largemagcal = new MissionPlanner.Controls.MyButton();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.compassInfoBindingSource)).BeginInit();
            this.groupBoxonboardcalib.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // myDataGridView1
            // 
            this.myDataGridView1.AllowUserToAddRows = false;
            this.myDataGridView1.AllowUserToDeleteRows = false;
            this.myDataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myDataGridView1.AutoGenerateColumns = false;
            this.myDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.myDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.devIDDataGridViewTextBoxColumn,
            this.busTypeDataGridViewTextBoxColumn,
            this.busDataGridViewTextBoxColumn,
            this.addressDataGridViewTextBoxColumn,
            this.devTypeDataGridViewTextBoxColumn,
            this.Up,
            this.Down});
            this.myDataGridView1.DataSource = this.compassInfoBindingSource;
            this.myDataGridView1.Location = new System.Drawing.Point(3, 49);
            this.myDataGridView1.MultiSelect = false;
            this.myDataGridView1.Name = "myDataGridView1";
            this.myDataGridView1.ReadOnly = true;
            this.myDataGridView1.RowHeadersWidth = 20;
            this.myDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.myDataGridView1.Size = new System.Drawing.Size(600, 209);
            this.myDataGridView1.TabIndex = 0;
            this.myDataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.myDataGridView1_CellContentClick);
            this.myDataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.myDataGridView1_RowPostPaint);
            // 
            // devIDDataGridViewTextBoxColumn
            // 
            this.devIDDataGridViewTextBoxColumn.DataPropertyName = "DevID";
            this.devIDDataGridViewTextBoxColumn.HeaderText = "DevID";
            this.devIDDataGridViewTextBoxColumn.Name = "devIDDataGridViewTextBoxColumn";
            this.devIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.devIDDataGridViewTextBoxColumn.Width = 50;
            // 
            // busTypeDataGridViewTextBoxColumn
            // 
            this.busTypeDataGridViewTextBoxColumn.DataPropertyName = "BusType";
            this.busTypeDataGridViewTextBoxColumn.HeaderText = "BusType";
            this.busTypeDataGridViewTextBoxColumn.Name = "busTypeDataGridViewTextBoxColumn";
            this.busTypeDataGridViewTextBoxColumn.ReadOnly = true;
            this.busTypeDataGridViewTextBoxColumn.Width = 150;
            // 
            // busDataGridViewTextBoxColumn
            // 
            this.busDataGridViewTextBoxColumn.DataPropertyName = "Bus";
            this.busDataGridViewTextBoxColumn.HeaderText = "Bus";
            this.busDataGridViewTextBoxColumn.Name = "busDataGridViewTextBoxColumn";
            this.busDataGridViewTextBoxColumn.ReadOnly = true;
            this.busDataGridViewTextBoxColumn.Width = 50;
            // 
            // addressDataGridViewTextBoxColumn
            // 
            this.addressDataGridViewTextBoxColumn.DataPropertyName = "Address";
            this.addressDataGridViewTextBoxColumn.HeaderText = "Address";
            this.addressDataGridViewTextBoxColumn.Name = "addressDataGridViewTextBoxColumn";
            this.addressDataGridViewTextBoxColumn.ReadOnly = true;
            this.addressDataGridViewTextBoxColumn.Width = 50;
            // 
            // devTypeDataGridViewTextBoxColumn
            // 
            this.devTypeDataGridViewTextBoxColumn.DataPropertyName = "DevType";
            this.devTypeDataGridViewTextBoxColumn.HeaderText = "DevType";
            this.devTypeDataGridViewTextBoxColumn.Name = "devTypeDataGridViewTextBoxColumn";
            this.devTypeDataGridViewTextBoxColumn.ReadOnly = true;
            this.devTypeDataGridViewTextBoxColumn.Width = 150;
            // 
            // Up
            // 
            this.Up.HeaderText = "Up";
            this.Up.Image = global::MissionPlanner.Properties.Resources.up;
            this.Up.Name = "Up";
            this.Up.ReadOnly = true;
            this.Up.Width = 40;
            // 
            // Down
            // 
            this.Down.HeaderText = "Down";
            this.Down.Image = global::MissionPlanner.Properties.Resources.down;
            this.Down.Name = "Down";
            this.Down.ReadOnly = true;
            this.Down.Width = 40;
            // 
            // compassInfoBindingSource
            // 
            this.compassInfoBindingSource.DataSource = typeof(MissionPlanner.GCSViews.ConfigurationView.DeviceInfo);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(438, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Set the Compass Priority by reordering the compasses in the table below (Highest " +
    "at the top)\r\n";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(583, 22);
            this.label6.TabIndex = 80;
            this.label6.Text = "Compass Priority";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Location = new System.Drawing.Point(-1, 18);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1125, 10);
            this.groupBox5.TabIndex = 79;
            this.groupBox5.TabStop = false;
            // 
            // groupBoxonboardcalib
            // 
            this.groupBoxonboardcalib.Controls.Add(this.pictureBox3);
            this.groupBoxonboardcalib.Controls.Add(this.pictureBox2);
            this.groupBoxonboardcalib.Controls.Add(this.pictureBox1);
            this.groupBoxonboardcalib.Controls.Add(this.label2);
            this.groupBoxonboardcalib.Controls.Add(this.label10);
            this.groupBoxonboardcalib.Controls.Add(this.mavlinkComboBoxfitness);
            this.groupBoxonboardcalib.Controls.Add(this.label9);
            this.groupBoxonboardcalib.Controls.Add(this.label8);
            this.groupBoxonboardcalib.Controls.Add(this.label7);
            this.groupBoxonboardcalib.Controls.Add(this.horizontalProgressBar3);
            this.groupBoxonboardcalib.Controls.Add(this.horizontalProgressBar2);
            this.groupBoxonboardcalib.Controls.Add(this.horizontalProgressBar1);
            this.groupBoxonboardcalib.Controls.Add(this.lbl_obmagresult);
            this.groupBoxonboardcalib.Controls.Add(this.BUT_OBmagcalaccept);
            this.groupBoxonboardcalib.Controls.Add(this.BUT_OBmagcalcancel);
            this.groupBoxonboardcalib.Controls.Add(this.BUT_OBmagcalstart);
            this.groupBoxonboardcalib.Location = new System.Drawing.Point(3, 313);
            this.groupBoxonboardcalib.Name = "groupBoxonboardcalib";
            this.groupBoxonboardcalib.Size = new System.Drawing.Size(600, 162);
            this.groupBoxonboardcalib.TabIndex = 81;
            this.groupBoxonboardcalib.TabStop = false;
            this.groupBoxonboardcalib.Text = "Onboard Mag Calibration";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(321, 107);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(23, 23);
            this.pictureBox3.TabIndex = 20;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(321, 78);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(23, 23);
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(321, 49);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(23, 23);
            this.pictureBox1.TabIndex = 18;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(203, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Relax fitness if calibration fails";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(7, 139);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Fitness";
            // 
            // mavlinkComboBoxfitness
            // 
            this.mavlinkComboBoxfitness.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mavlinkComboBoxfitness.Enabled = false;
            this.mavlinkComboBoxfitness.FormattingEnabled = true;
            this.mavlinkComboBoxfitness.Location = new System.Drawing.Point(57, 135);
            this.mavlinkComboBoxfitness.Name = "mavlinkComboBoxfitness";
            this.mavlinkComboBoxfitness.ParamName = null;
            this.mavlinkComboBoxfitness.Size = new System.Drawing.Size(140, 21);
            this.mavlinkComboBoxfitness.SubControl = null;
            this.mavlinkComboBoxfitness.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(6, 117);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Mag 3";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(6, 88);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Mag 2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(6, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Mag 1";
            // 
            // horizontalProgressBar3
            // 
            this.horizontalProgressBar3.DrawLabel = true;
            this.horizontalProgressBar3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.horizontalProgressBar3.Label = null;
            this.horizontalProgressBar3.Location = new System.Drawing.Point(57, 107);
            this.horizontalProgressBar3.maxline = 0;
            this.horizontalProgressBar3.minline = 0;
            this.horizontalProgressBar3.Name = "horizontalProgressBar3";
            this.horizontalProgressBar3.Size = new System.Drawing.Size(258, 23);
            this.horizontalProgressBar3.TabIndex = 6;
            // 
            // horizontalProgressBar2
            // 
            this.horizontalProgressBar2.DrawLabel = true;
            this.horizontalProgressBar2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.horizontalProgressBar2.Label = null;
            this.horizontalProgressBar2.Location = new System.Drawing.Point(57, 78);
            this.horizontalProgressBar2.maxline = 0;
            this.horizontalProgressBar2.minline = 0;
            this.horizontalProgressBar2.Name = "horizontalProgressBar2";
            this.horizontalProgressBar2.Size = new System.Drawing.Size(258, 23);
            this.horizontalProgressBar2.TabIndex = 5;
            // 
            // horizontalProgressBar1
            // 
            this.horizontalProgressBar1.DrawLabel = true;
            this.horizontalProgressBar1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.horizontalProgressBar1.Label = null;
            this.horizontalProgressBar1.Location = new System.Drawing.Point(57, 49);
            this.horizontalProgressBar1.maxline = 0;
            this.horizontalProgressBar1.minline = 0;
            this.horizontalProgressBar1.Name = "horizontalProgressBar1";
            this.horizontalProgressBar1.Size = new System.Drawing.Size(258, 23);
            this.horizontalProgressBar1.TabIndex = 4;
            // 
            // lbl_obmagresult
            // 
            this.lbl_obmagresult.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_obmagresult.Location = new System.Drawing.Point(350, 20);
            this.lbl_obmagresult.Multiline = true;
            this.lbl_obmagresult.Name = "lbl_obmagresult";
            this.lbl_obmagresult.ReadOnly = true;
            this.lbl_obmagresult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lbl_obmagresult.Size = new System.Drawing.Size(244, 110);
            this.lbl_obmagresult.TabIndex = 3;
            // 
            // BUT_OBmagcalaccept
            // 
            this.BUT_OBmagcalaccept.Enabled = false;
            this.BUT_OBmagcalaccept.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_OBmagcalaccept.Location = new System.Drawing.Point(125, 20);
            this.BUT_OBmagcalaccept.Name = "BUT_OBmagcalaccept";
            this.BUT_OBmagcalaccept.Size = new System.Drawing.Size(75, 23);
            this.BUT_OBmagcalaccept.TabIndex = 1;
            this.BUT_OBmagcalaccept.Text = "Accept";
            this.BUT_OBmagcalaccept.UseVisualStyleBackColor = true;
            this.BUT_OBmagcalaccept.Click += new System.EventHandler(this.BUT_OBmagcalaccept_Click);
            // 
            // BUT_OBmagcalcancel
            // 
            this.BUT_OBmagcalcancel.Enabled = false;
            this.BUT_OBmagcalcancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_OBmagcalcancel.Location = new System.Drawing.Point(206, 20);
            this.BUT_OBmagcalcancel.Name = "BUT_OBmagcalcancel";
            this.BUT_OBmagcalcancel.Size = new System.Drawing.Size(75, 23);
            this.BUT_OBmagcalcancel.TabIndex = 2;
            this.BUT_OBmagcalcancel.Text = "Cancel";
            this.BUT_OBmagcalcancel.UseVisualStyleBackColor = true;
            this.BUT_OBmagcalcancel.Click += new System.EventHandler(this.BUT_OBmagcalcancel_Click);
            // 
            // BUT_OBmagcalstart
            // 
            this.BUT_OBmagcalstart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_OBmagcalstart.Location = new System.Drawing.Point(44, 20);
            this.BUT_OBmagcalstart.Name = "BUT_OBmagcalstart";
            this.BUT_OBmagcalstart.Size = new System.Drawing.Size(75, 23);
            this.BUT_OBmagcalstart.TabIndex = 0;
            this.BUT_OBmagcalstart.Text = "Start";
            this.BUT_OBmagcalstart.UseVisualStyleBackColor = true;
            this.BUT_OBmagcalstart.Click += new System.EventHandler(this.BUT_OBmagcalstart_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // mavlinkCheckBoxUseCompass1
            // 
            this.mavlinkCheckBoxUseCompass1.AutoSize = true;
            this.mavlinkCheckBoxUseCompass1.Enabled = false;
            this.mavlinkCheckBoxUseCompass1.Location = new System.Drawing.Point(3, 277);
            this.mavlinkCheckBoxUseCompass1.Name = "mavlinkCheckBoxUseCompass1";
            this.mavlinkCheckBoxUseCompass1.OffValue = 0D;
            this.mavlinkCheckBoxUseCompass1.OnValue = 1D;
            this.mavlinkCheckBoxUseCompass1.ParamName = null;
            this.mavlinkCheckBoxUseCompass1.Size = new System.Drawing.Size(100, 17);
            this.mavlinkCheckBoxUseCompass1.TabIndex = 82;
            this.mavlinkCheckBoxUseCompass1.Text = "Use Compass 1";
            this.mavlinkCheckBoxUseCompass1.UseVisualStyleBackColor = true;
            // 
            // mavlinkCheckBoxUseCompass3
            // 
            this.mavlinkCheckBoxUseCompass3.AutoSize = true;
            this.mavlinkCheckBoxUseCompass3.Enabled = false;
            this.mavlinkCheckBoxUseCompass3.Location = new System.Drawing.Point(248, 277);
            this.mavlinkCheckBoxUseCompass3.Name = "mavlinkCheckBoxUseCompass3";
            this.mavlinkCheckBoxUseCompass3.OffValue = 0D;
            this.mavlinkCheckBoxUseCompass3.OnValue = 1D;
            this.mavlinkCheckBoxUseCompass3.ParamName = null;
            this.mavlinkCheckBoxUseCompass3.Size = new System.Drawing.Size(100, 17);
            this.mavlinkCheckBoxUseCompass3.TabIndex = 83;
            this.mavlinkCheckBoxUseCompass3.Text = "Use Compass 3";
            this.mavlinkCheckBoxUseCompass3.UseVisualStyleBackColor = true;
            // 
            // mavlinkCheckBoxUseCompass2
            // 
            this.mavlinkCheckBoxUseCompass2.AutoSize = true;
            this.mavlinkCheckBoxUseCompass2.Enabled = false;
            this.mavlinkCheckBoxUseCompass2.Location = new System.Drawing.Point(128, 277);
            this.mavlinkCheckBoxUseCompass2.Name = "mavlinkCheckBoxUseCompass2";
            this.mavlinkCheckBoxUseCompass2.OffValue = 0D;
            this.mavlinkCheckBoxUseCompass2.OnValue = 1D;
            this.mavlinkCheckBoxUseCompass2.ParamName = null;
            this.mavlinkCheckBoxUseCompass2.Size = new System.Drawing.Size(100, 17);
            this.mavlinkCheckBoxUseCompass2.TabIndex = 84;
            this.mavlinkCheckBoxUseCompass2.Text = "Use Compass 2";
            this.mavlinkCheckBoxUseCompass2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 261);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(255, 13);
            this.label3.TabIndex = 85;
            this.label3.Text = "Do you want to disable any of the first 3 compasses?";
            // 
            // but_largemagcal
            // 
            this.but_largemagcal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.but_largemagcal.Location = new System.Drawing.Point(3, 481);
            this.but_largemagcal.Name = "but_largemagcal";
            this.but_largemagcal.Size = new System.Drawing.Size(75, 23);
            this.but_largemagcal.TabIndex = 21;
            this.but_largemagcal.Text = "Large Vehicle MagCal";
            this.but_largemagcal.UseVisualStyleBackColor = true;
            this.but_largemagcal.Click += new System.EventHandler(this.but_largemagcal_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 297);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(281, 13);
            this.label4.TabIndex = 86;
            this.label4.Text = "A mag calibration is required to remap the above changes.";
            // 
            // ConfigHWCompass2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.but_largemagcal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mavlinkCheckBoxUseCompass2);
            this.Controls.Add(this.mavlinkCheckBoxUseCompass3);
            this.Controls.Add(this.mavlinkCheckBoxUseCompass1);
            this.Controls.Add(this.groupBoxonboardcalib);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.myDataGridView1);
            this.Name = "ConfigHWCompass2";
            this.Size = new System.Drawing.Size(606, 753);
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.compassInfoBindingSource)).EndInit();
            this.groupBoxonboardcalib.ResumeLayout(false);
            this.groupBoxonboardcalib.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyDataGridView myDataGridView1;
        private System.Windows.Forms.BindingSource compassInfoBindingSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBoxonboardcalib;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label10;
        private Controls.MavlinkComboBox mavlinkComboBoxfitness;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private Controls.HorizontalProgressBar horizontalProgressBar3;
        private Controls.HorizontalProgressBar horizontalProgressBar2;
        private Controls.HorizontalProgressBar horizontalProgressBar1;
        private System.Windows.Forms.TextBox lbl_obmagresult;
        private Controls.MyButton BUT_OBmagcalaccept;
        private Controls.MyButton BUT_OBmagcalcancel;
        private Controls.MyButton BUT_OBmagcalstart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn devIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn busTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn busDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn addressDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn devTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewImageColumn Up;
        private System.Windows.Forms.DataGridViewImageColumn Down;
        private Controls.MavlinkCheckBox mavlinkCheckBoxUseCompass1;
        private Controls.MavlinkCheckBox mavlinkCheckBoxUseCompass3;
        private Controls.MavlinkCheckBox mavlinkCheckBoxUseCompass2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Controls.MyButton but_largemagcal;
        private System.Windows.Forms.Label label4;
    }
}
