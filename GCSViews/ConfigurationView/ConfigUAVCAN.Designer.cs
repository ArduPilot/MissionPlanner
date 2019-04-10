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
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.but_slcanmode1 = new MissionPlanner.Controls.MyButton();
            this.myDataGridView1 = new MissionPlanner.Controls.MyDataGridView();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.healthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uptimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hardwareVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Parameter = new System.Windows.Forms.DataGridViewButtonColumn();
            this.uAVCANModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.but_slcanmode2 = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uAVCANModelBindingSource)).BeginInit();
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
            this.groupBox5.Size = new System.Drawing.Size(793, 5);
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
            this.myDataGridView1.AllowUserToOrderColumns = true;
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
            this.updateDataGridViewTextBoxColumn,
            this.Parameter});
            this.myDataGridView1.DataSource = this.uAVCANModelBindingSource;
            this.myDataGridView1.Location = new System.Drawing.Point(7, 58);
            this.myDataGridView1.Name = "myDataGridView1";
            this.myDataGridView1.ReadOnly = true;
            this.myDataGridView1.Size = new System.Drawing.Size(785, 320);
            this.myDataGridView1.TabIndex = 1;
            this.myDataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.myDataGridView1_CellClick);
            this.myDataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.myDataGridView1_RowsAdded);
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            this.iDDataGridViewTextBoxColumn.Width = 43;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.Width = 60;
            // 
            // modeDataGridViewTextBoxColumn
            // 
            this.modeDataGridViewTextBoxColumn.DataPropertyName = "Mode";
            this.modeDataGridViewTextBoxColumn.HeaderText = "Mode";
            this.modeDataGridViewTextBoxColumn.Name = "modeDataGridViewTextBoxColumn";
            this.modeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // healthDataGridViewTextBoxColumn
            // 
            this.healthDataGridViewTextBoxColumn.DataPropertyName = "Health";
            this.healthDataGridViewTextBoxColumn.HeaderText = "Health";
            this.healthDataGridViewTextBoxColumn.Name = "healthDataGridViewTextBoxColumn";
            this.healthDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // uptimeDataGridViewTextBoxColumn
            // 
            this.uptimeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.uptimeDataGridViewTextBoxColumn.DataPropertyName = "Uptime";
            this.uptimeDataGridViewTextBoxColumn.HeaderText = "Uptime";
            this.uptimeDataGridViewTextBoxColumn.Name = "uptimeDataGridViewTextBoxColumn";
            this.uptimeDataGridViewTextBoxColumn.ReadOnly = true;
            this.uptimeDataGridViewTextBoxColumn.Width = 65;
            // 
            // hardwareVersionDataGridViewTextBoxColumn
            // 
            this.hardwareVersionDataGridViewTextBoxColumn.DataPropertyName = "HardwareVersion";
            this.hardwareVersionDataGridViewTextBoxColumn.HeaderText = "HardwareVersion";
            this.hardwareVersionDataGridViewTextBoxColumn.Name = "hardwareVersionDataGridViewTextBoxColumn";
            this.hardwareVersionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // updateDataGridViewTextBoxColumn
            // 
            this.updateDataGridViewTextBoxColumn.HeaderText = "Update Firmware";
            this.updateDataGridViewTextBoxColumn.Name = "updateDataGridViewTextBoxColumn";
            this.updateDataGridViewTextBoxColumn.ReadOnly = true;
            this.updateDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.updateDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.updateDataGridViewTextBoxColumn.Text = "Update Firmware";
            // 
            // Parameter
            // 
            this.Parameter.HeaderText = "Parameter";
            this.Parameter.Name = "Parameter";
            this.Parameter.ReadOnly = true;
            this.Parameter.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // uAVCANModelBindingSource
            // 
            this.uAVCANModelBindingSource.DataSource = typeof(MissionPlanner.GCSViews.ConfigurationView.UAVCANModel);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(269, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(359, 26);
            this.label1.TabIndex = 83;
            this.label1.Text = "After enabling SLCAN, you will no longer be able to connect via MAVLINK.\r\nYou mus" +
    "t reboot the flight controller to return to a normal mode\r\n";
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
            // ConfigUAVCAN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.but_slcanmode2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.but_slcanmode1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.myDataGridView1);
            this.Name = "ConfigUAVCAN";
            this.Size = new System.Drawing.Size(795, 381);
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uAVCANModelBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyDataGridView myDataGridView1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        public System.Windows.Forms.BindingSource uAVCANModelBindingSource;
        private Controls.MyButton but_slcanmode1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn modeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn healthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn uptimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hardwareVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn updateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn Parameter;
        private Controls.MyButton but_slcanmode2;
    }
}
