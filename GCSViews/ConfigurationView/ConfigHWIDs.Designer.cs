namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigHWIDs
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
            this.deviceInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.paramNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.devIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.busTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.busDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.devTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceInfoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // myDataGridView1
            // 
            this.myDataGridView1.AllowUserToAddRows = false;
            this.myDataGridView1.AllowUserToDeleteRows = false;
            this.myDataGridView1.AllowUserToOrderColumns = true;
            this.myDataGridView1.AutoGenerateColumns = false;
            this.myDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.myDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.paramNameDataGridViewTextBoxColumn,
            this.devIDDataGridViewTextBoxColumn,
            this.busTypeDataGridViewTextBoxColumn,
            this.busDataGridViewTextBoxColumn,
            this.addressDataGridViewTextBoxColumn,
            this.devTypeDataGridViewTextBoxColumn});
            this.myDataGridView1.DataSource = this.deviceInfoBindingSource;
            this.myDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myDataGridView1.Location = new System.Drawing.Point(0, 0);
            this.myDataGridView1.Name = "myDataGridView1";
            this.myDataGridView1.ReadOnly = true;
            this.myDataGridView1.Size = new System.Drawing.Size(856, 496);
            this.myDataGridView1.TabIndex = 0;
            // 
            // deviceInfoBindingSource
            // 
            this.deviceInfoBindingSource.DataSource = typeof(MissionPlanner.GCSViews.ConfigurationView.DeviceInfo);
            // 
            // paramNameDataGridViewTextBoxColumn
            // 
            this.paramNameDataGridViewTextBoxColumn.DataPropertyName = "ParamName";
            this.paramNameDataGridViewTextBoxColumn.HeaderText = "ParamName";
            this.paramNameDataGridViewTextBoxColumn.Name = "paramNameDataGridViewTextBoxColumn";
            this.paramNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // devIDDataGridViewTextBoxColumn
            // 
            this.devIDDataGridViewTextBoxColumn.DataPropertyName = "DevID";
            this.devIDDataGridViewTextBoxColumn.HeaderText = "DevID";
            this.devIDDataGridViewTextBoxColumn.Name = "devIDDataGridViewTextBoxColumn";
            this.devIDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // busTypeDataGridViewTextBoxColumn
            // 
            this.busTypeDataGridViewTextBoxColumn.DataPropertyName = "BusType";
            this.busTypeDataGridViewTextBoxColumn.HeaderText = "BusType";
            this.busTypeDataGridViewTextBoxColumn.Name = "busTypeDataGridViewTextBoxColumn";
            this.busTypeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // busDataGridViewTextBoxColumn
            // 
            this.busDataGridViewTextBoxColumn.DataPropertyName = "Bus";
            this.busDataGridViewTextBoxColumn.HeaderText = "Bus";
            this.busDataGridViewTextBoxColumn.Name = "busDataGridViewTextBoxColumn";
            this.busDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // addressDataGridViewTextBoxColumn
            // 
            this.addressDataGridViewTextBoxColumn.DataPropertyName = "Address";
            this.addressDataGridViewTextBoxColumn.HeaderText = "Address";
            this.addressDataGridViewTextBoxColumn.Name = "addressDataGridViewTextBoxColumn";
            this.addressDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // devTypeDataGridViewTextBoxColumn
            // 
            this.devTypeDataGridViewTextBoxColumn.DataPropertyName = "DevType";
            this.devTypeDataGridViewTextBoxColumn.HeaderText = "DevType";
            this.devTypeDataGridViewTextBoxColumn.Name = "devTypeDataGridViewTextBoxColumn";
            this.devTypeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ConfigHWIDs
            // 
            this.Controls.Add(this.myDataGridView1);
            this.Name = "ConfigHWIDs";
            this.Size = new System.Drawing.Size(856, 496);
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceInfoBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.MyDataGridView myDataGridView1;
        private System.Windows.Forms.BindingSource deviceInfoBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn devIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn busTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn busDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn addressDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn devTypeDataGridViewTextBoxColumn;
    }
}
