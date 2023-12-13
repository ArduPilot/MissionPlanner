namespace RedundantLinkManager
{
    partial class RedundantLinkManager
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
            this.grid_links = new MissionPlanner.Controls.MyDataGridView();
            this.LinkEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.LinkName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Port = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Host = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Up = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Down = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.cmb_presets = new System.Windows.Forms.ComboBox();
            this.but_save = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.grid_links)).BeginInit();
            this.SuspendLayout();
            // 
            // grid_links
            // 
            this.grid_links.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid_links.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_links.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LinkEnabled,
            this.Type,
            this.LinkName,
            this.Port,
            this.Host,
            this.Up,
            this.Down,
            this.Delete});
            this.grid_links.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.grid_links.Location = new System.Drawing.Point(10, 35);
            this.grid_links.Name = "grid_links";
            this.grid_links.RowHeadersWidth = 20;
            this.grid_links.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.grid_links.Size = new System.Drawing.Size(547, 325);
            this.grid_links.TabIndex = 1;
            this.grid_links.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.grid_links_CellBeginEdit);
            this.grid_links.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_links_CellContentClick);
            this.grid_links.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.grid_links_CellValidating);
            this.grid_links.CurrentCellDirtyStateChanged += new System.EventHandler(this.grid_links_CurrentCellDirtyStateChanged);
            this.grid_links.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.grid_links_UserAddedRow);
            // 
            // LinkEnabled
            // 
            this.LinkEnabled.DataPropertyName = "Enabled";
            this.LinkEnabled.HeaderText = "Enabled";
            this.LinkEnabled.MinimumWidth = 6;
            this.LinkEnabled.Name = "LinkEnabled";
            this.LinkEnabled.Width = 52;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "Type";
            this.Type.HeaderText = "Type";
            this.Type.Items.AddRange(new object[] {
            "Serial",
            "TCP",
            "UDP",
            "UDPCl"});
            this.Type.MinimumWidth = 6;
            this.Type.Name = "Type";
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Type.Width = 70;
            // 
            // LinkName
            // 
            this.LinkName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LinkName.DataPropertyName = "Name";
            this.LinkName.FillWeight = 50F;
            this.LinkName.HeaderText = "Name";
            this.LinkName.MinimumWidth = 6;
            this.LinkName.Name = "LinkName";
            // 
            // Port
            // 
            this.Port.DataPropertyName = "PortOrBaud";
            this.Port.HeaderText = "Port/Baud";
            this.Port.MinimumWidth = 6;
            this.Port.Name = "Port";
            this.Port.Width = 63;
            // 
            // Host
            // 
            this.Host.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Host.DataPropertyName = "HostOrCom";
            this.Host.HeaderText = "Host/COM";
            this.Host.MinimumWidth = 6;
            this.Host.Name = "Host";
            // 
            // Up
            // 
            this.Up.HeaderText = "▲";
            this.Up.MinimumWidth = 6;
            this.Up.Name = "Up";
            this.Up.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Up.Text = "▲";
            this.Up.UseColumnTextForButtonValue = true;
            this.Up.Width = 25;
            // 
            // Down
            // 
            this.Down.HeaderText = "▼";
            this.Down.MinimumWidth = 6;
            this.Down.Name = "Down";
            this.Down.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Down.Text = "▼";
            this.Down.UseColumnTextForButtonValue = true;
            this.Down.Width = 25;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "❌";
            this.Delete.MinimumWidth = 6;
            this.Delete.Name = "Delete";
            this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Delete.Text = "❌";
            this.Delete.UseColumnTextForButtonValue = true;
            this.Delete.Width = 25;
            // 
            // cmb_presets
            // 
            this.cmb_presets.FormattingEnabled = true;
            this.cmb_presets.Location = new System.Drawing.Point(12, 8);
            this.cmb_presets.Name = "cmb_presets";
            this.cmb_presets.Size = new System.Drawing.Size(161, 21);
            this.cmb_presets.TabIndex = 4;
            this.cmb_presets.SelectedIndexChanged += new System.EventHandler(this.cmb_presets_SelectedIndexChanged);
            // 
            // but_save
            // 
            this.but_save.Location = new System.Drawing.Point(178, 8);
            this.but_save.Margin = new System.Windows.Forms.Padding(2);
            this.but_save.Name = "but_save";
            this.but_save.Size = new System.Drawing.Size(56, 21);
            this.but_save.TabIndex = 5;
            this.but_save.Text = "Save";
            this.but_save.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_save.UseVisualStyleBackColor = true;
            this.but_save.Click += new System.EventHandler(this.but_save_Click);
            // 
            // RedundantLinkManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 370);
            this.Controls.Add(this.but_save);
            this.Controls.Add(this.cmb_presets);
            this.Controls.Add(this.grid_links);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RedundantLinkManager";
            this.Text = "Redundant Link Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RedundantLinkManager_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.grid_links)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MissionPlanner.Controls.MyDataGridView grid_links;
        private System.Windows.Forms.ComboBox cmb_presets;
        private MissionPlanner.Controls.MyButton but_save;
        private System.Windows.Forms.DataGridViewCheckBoxColumn LinkEnabled;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn LinkName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Port;
        private System.Windows.Forms.DataGridViewTextBoxColumn Host;
        private System.Windows.Forms.DataGridViewButtonColumn Up;
        private System.Windows.Forms.DataGridViewButtonColumn Down;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
    }
}