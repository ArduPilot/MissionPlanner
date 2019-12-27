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
            this.compassInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.configHWCompass2BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.devIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.busTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.busDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.devTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Up = new System.Windows.Forms.DataGridViewImageColumn();
            this.Down = new System.Windows.Forms.DataGridViewImageColumn();
            this.Use = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.compassInfoBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configHWCompass2BindingSource)).BeginInit();
            this.SuspendLayout();
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
            this.devIDDataGridViewTextBoxColumn,
            this.busTypeDataGridViewTextBoxColumn,
            this.busDataGridViewTextBoxColumn,
            this.addressDataGridViewTextBoxColumn,
            this.devTypeDataGridViewTextBoxColumn,
            this.Up,
            this.Down,
            this.Use});
            this.myDataGridView1.DataSource = this.compassInfoBindingSource;
            this.myDataGridView1.Location = new System.Drawing.Point(3, 68);
            this.myDataGridView1.Name = "myDataGridView1";
            this.myDataGridView1.ReadOnly = true;
            this.myDataGridView1.RowHeadersWidth = 20;
            this.myDataGridView1.Size = new System.Drawing.Size(583, 314);
            this.myDataGridView1.TabIndex = 0;
            this.myDataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.myDataGridView1_CellContentClick);
            // 
            // compassInfoBindingSource
            // 
            this.compassInfoBindingSource.DataSource = typeof(MissionPlanner.GCSViews.ConfigurationView.CompassInfo);
            // 
            // configHWCompass2BindingSource
            // 
            this.configHWCompass2BindingSource.DataSource = typeof(MissionPlanner.GCSViews.ConfigurationView.ConfigHWCompass2);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(364, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Set the Compass Priority by moving around the compass\'s in the table below\r\n";
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
            this.groupBox5.Size = new System.Drawing.Size(1108, 10);
            this.groupBox5.TabIndex = 79;
            this.groupBox5.TabStop = false;
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
            // Use
            // 
            this.Use.HeaderText = "Use";
            this.Use.Name = "Use";
            this.Use.ReadOnly = true;
            // 
            // ConfigHWCompass2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.myDataGridView1);
            this.Name = "ConfigHWCompass2";
            this.Size = new System.Drawing.Size(589, 385);
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.compassInfoBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configHWCompass2BindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyDataGridView myDataGridView1;
        private System.Windows.Forms.BindingSource configHWCompass2BindingSource;
        private System.Windows.Forms.BindingSource compassInfoBindingSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridViewTextBoxColumn devIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn busTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn busDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn addressDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn devTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewImageColumn Up;
        private System.Windows.Forms.DataGridViewImageColumn Down;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Use;
    }
}
