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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RedundantLinkManager));
            this.but_addLink = new MissionPlanner.Controls.MyButton();
            this.but_connect = new MissionPlanner.Controls.MyButton();
            this.grid_links = new MissionPlanner.Controls.MyDataGridView();
            this.Priority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Enabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Host = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Port = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LinkName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Up = new System.Windows.Forms.DataGridViewImageColumn();
            this.Down = new System.Windows.Forms.DataGridViewImageColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grid_links)).BeginInit();
            this.SuspendLayout();
            // 
            // but_addLink
            // 
            this.but_addLink.Location = new System.Drawing.Point(12, 12);
            this.but_addLink.Name = "but_addLink";
            this.but_addLink.Size = new System.Drawing.Size(75, 23);
            this.but_addLink.TabIndex = 2;
            this.but_addLink.Text = "Add Link";
            this.but_addLink.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_addLink.UseVisualStyleBackColor = true;
            this.but_addLink.Click += new System.EventHandler(this.but_addLink_Click);
            // 
            // but_connect
            // 
            this.but_connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.but_connect.Location = new System.Drawing.Point(728, 12);
            this.but_connect.Name = "but_connect";
            this.but_connect.Size = new System.Drawing.Size(75, 23);
            this.but_connect.TabIndex = 3;
            this.but_connect.Text = "Connect";
            this.but_connect.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_connect.UseVisualStyleBackColor = true;
            // 
            // grid_links
            // 
            this.grid_links.AllowUserToAddRows = false;
            this.grid_links.AllowUserToDeleteRows = false;
            this.grid_links.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid_links.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_links.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Priority,
            this.Enabled,
            this.Type,
            this.Host,
            this.Port,
            this.LinkName,
            this.Up,
            this.Down});
            this.grid_links.Location = new System.Drawing.Point(12, 42);
            this.grid_links.Margin = new System.Windows.Forms.Padding(4);
            this.grid_links.MultiSelect = false;
            this.grid_links.Name = "grid_links";
            this.grid_links.RowHeadersWidth = 20;
            this.grid_links.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid_links.Size = new System.Drawing.Size(790, 420);
            this.grid_links.TabIndex = 1;
            this.grid_links.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.grid_links_CellFormatting);
            this.grid_links.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.myDataGridView1_RowPostPaint);
            // 
            // Priority
            // 
            this.Priority.HeaderText = "Priority";
            this.Priority.Name = "Priority";
            this.Priority.Width = 50;
            // 
            // Enabled
            // 
            this.Enabled.DataPropertyName = "Enabled";
            this.Enabled.HeaderText = "Enabled";
            this.Enabled.Name = "Enabled";
            this.Enabled.Width = 50;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Items.AddRange(new object[] {
            "COM",
            "TCP",
            "UDP",
            "UDPCl",
            "WS"});
            this.Type.Name = "Type";
            this.Type.Width = 50;
            // 
            // Host
            // 
            this.Host.HeaderText = "COM/Host";
            this.Host.Name = "Host";
            // 
            // Port
            // 
            this.Port.HeaderText = "Baud/Port";
            this.Port.Name = "Port";
            // 
            // LinkName
            // 
            this.LinkName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LinkName.HeaderText = "Name";
            this.LinkName.Name = "LinkName";
            // 
            // Up
            // 
            this.Up.HeaderText = "▲";
            this.Up.Image = ((System.Drawing.Image)(resources.GetObject("Up.Image")));
            this.Up.Name = "Up";
            this.Up.Width = 40;
            // 
            // Down
            // 
            this.Down.HeaderText = "▼";
            this.Down.Image = ((System.Drawing.Image)(resources.GetObject("Down.Image")));
            this.Down.Name = "Down";
            this.Down.Width = 40;
            // 
            // RedundantLinkManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 475);
            this.Controls.Add(this.but_connect);
            this.Controls.Add(this.but_addLink);
            this.Controls.Add(this.grid_links);
            this.Name = "RedundantLinkManager";
            this.Text = "RedundantLinkManager";
            ((System.ComponentModel.ISupportInitialize)(this.grid_links)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MissionPlanner.Controls.MyDataGridView grid_links;
        private MissionPlanner.Controls.MyButton but_addLink;
        private MissionPlanner.Controls.MyButton but_connect;
        private System.Windows.Forms.DataGridViewTextBoxColumn Priority;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Enabled;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Host;
        private System.Windows.Forms.DataGridViewTextBoxColumn Port;
        private System.Windows.Forms.DataGridViewTextBoxColumn LinkName;
        private System.Windows.Forms.DataGridViewImageColumn Up;
        private System.Windows.Forms.DataGridViewImageColumn Down;
    }
}