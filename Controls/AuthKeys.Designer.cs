namespace MissionPlanner.Controls
{
    partial class AuthKeys
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.FName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Use = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.but_save = new MissionPlanner.Controls.MyButton();
            this.but_add = new MissionPlanner.Controls.MyButton();
            this.but_disablesigning = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FName,
            this.Use,
            this.Key});
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(598, 167);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            this.dataGridView1.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_UserDeletedRow);
            // 
            // FName
            // 
            this.FName.HeaderText = "Friendly Name";
            this.FName.Name = "FName";
            // 
            // Use
            // 
            this.Use.HeaderText = "Use";
            this.Use.Name = "Use";
            this.Use.ReadOnly = true;
            this.Use.Width = 50;
            // 
            // Key
            // 
            this.Key.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Key.HeaderText = "Key";
            this.Key.Name = "Key";
            this.Key.ReadOnly = true;
            // 
            // but_save
            // 
            this.but_save.Location = new System.Drawing.Point(535, 185);
            this.but_save.Name = "but_save";
            this.but_save.Size = new System.Drawing.Size(75, 23);
            this.but_save.TabIndex = 1;
            this.but_save.Text = "Save";
            this.but_save.UseVisualStyleBackColor = true;
            this.but_save.Click += new System.EventHandler(this.but_save_Click);
            // 
            // but_add
            // 
            this.but_add.Location = new System.Drawing.Point(12, 185);
            this.but_add.Name = "but_add";
            this.but_add.Size = new System.Drawing.Size(75, 23);
            this.but_add.TabIndex = 2;
            this.but_add.Text = "Add";
            this.but_add.UseVisualStyleBackColor = true;
            this.but_add.Click += new System.EventHandler(this.but_add_Click);
            // 
            // but_disablesigning
            // 
            this.but_disablesigning.Location = new System.Drawing.Point(93, 185);
            this.but_disablesigning.Name = "but_disablesigning";
            this.but_disablesigning.Size = new System.Drawing.Size(75, 23);
            this.but_disablesigning.TabIndex = 3;
            this.but_disablesigning.Text = "Disable Signing";
            this.but_disablesigning.UseVisualStyleBackColor = true;
            this.but_disablesigning.Click += new System.EventHandler(this.but_disablesigning_Click);
            // 
            // AuthKeys
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(622, 227);
            this.Controls.Add(this.but_disablesigning);
            this.Controls.Add(this.but_add);
            this.Controls.Add(this.but_save);
            this.Controls.Add(this.dataGridView1);
            this.Name = "AuthKeys";
            this.Text = "AuthKeys";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private MyButton but_save;
        private MyButton but_add;
        private System.Windows.Forms.DataGridViewTextBoxColumn FName;
        private System.Windows.Forms.DataGridViewButtonColumn Use;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private MyButton but_disablesigning;
    }
}