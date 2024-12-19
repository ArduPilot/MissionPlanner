namespace MissionPlanner.Controls
{
    partial class SerialOutputPass
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SerialOutputPass));
            this.CMB_serialport = new System.Windows.Forms.ComboBox();
            this.BUT_connect = new MissionPlanner.Controls.MyButton();
            this.CMB_baudrate = new System.Windows.Forms.ComboBox();
            this.chk_write = new System.Windows.Forms.CheckBox();
            this.myDataGridView1 = new MissionPlanner.Controls.MyDataGridView();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Direction = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Port = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Extra = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Write = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Go = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // CMB_serialport
            // 
            this.CMB_serialport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_serialport.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_serialport, "CMB_serialport");
            this.CMB_serialport.Name = "CMB_serialport";
            // 
            // BUT_connect
            // 
            resources.ApplyResources(this.BUT_connect, "BUT_connect");
            this.BUT_connect.Name = "BUT_connect";
            this.BUT_connect.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
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
            resources.GetString("CMB_baudrate.Items7")});
            resources.ApplyResources(this.CMB_baudrate, "CMB_baudrate");
            this.CMB_baudrate.Name = "CMB_baudrate";
            // 
            // chk_write
            // 
            resources.ApplyResources(this.chk_write, "chk_write");
            this.chk_write.Name = "chk_write";
            this.chk_write.UseVisualStyleBackColor = true;
            this.chk_write.CheckedChanged += new System.EventHandler(this.chk_write_CheckedChanged);
            // 
            // myDataGridView1
            // 
            resources.ApplyResources(this.myDataGridView1, "myDataGridView1");
            this.myDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.myDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Type,
            this.Direction,
            this.Port,
            this.Extra,
            this.Write,
            this.Go});
            this.myDataGridView1.Name = "myDataGridView1";
            this.myDataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.myDataGridView1_CellContentClick);
            this.myDataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.myDataGridView1_CellEndEdit);
            this.myDataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.myDataGridView1_DataError);
            // 
            // Type
            // 
            resources.ApplyResources(this.Type, "Type");
            this.Type.Items.AddRange(new object[] {
            "Serial",
            "TCP",
            "UDP"});
            this.Type.Name = "Type";
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Direction
            // 
            resources.ApplyResources(this.Direction, "Direction");
            this.Direction.Items.AddRange(new object[] {
            "Inbound",
            "Outbound"});
            this.Direction.Name = "Direction";
            // 
            // Port
            // 
            resources.ApplyResources(this.Port, "Port");
            this.Port.Name = "Port";
            // 
            // Extra
            // 
            resources.ApplyResources(this.Extra, "Extra");
            this.Extra.Name = "Extra";
            // 
            // Write
            // 
            resources.ApplyResources(this.Write, "Write");
            this.Write.Name = "Write";
            // 
            // Go
            // 
            resources.ApplyResources(this.Go, "Go");
            this.Go.Name = "Go";
            this.Go.Text = "Go";
            // 
            // SerialOutputPass
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.myDataGridView1);
            this.Controls.Add(this.chk_write);
            this.Controls.Add(this.CMB_baudrate);
            this.Controls.Add(this.BUT_connect);
            this.Controls.Add(this.CMB_serialport);
            this.Name = "SerialOutputPass";
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_serialport;
        private Controls.MyButton BUT_connect;
        private System.Windows.Forms.ComboBox CMB_baudrate;
        private System.Windows.Forms.CheckBox chk_write;
        private MyDataGridView myDataGridView1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewComboBoxColumn Direction;
        private System.Windows.Forms.DataGridViewTextBoxColumn Port;
        private System.Windows.Forms.DataGridViewTextBoxColumn Extra;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Write;
        private System.Windows.Forms.DataGridViewButtonColumn Go;
    }
}