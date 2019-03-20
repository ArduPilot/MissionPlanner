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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthKeys));
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
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FName,
            this.Use,
            this.Key});
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            this.dataGridView1.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_UserDeletedRow);
            // 
            // FName
            // 
            resources.ApplyResources(this.FName, "FName");
            this.FName.Name = "FName";
            // 
            // Use
            // 
            resources.ApplyResources(this.Use, "Use");
            this.Use.Name = "Use";
            this.Use.ReadOnly = true;
            // 
            // Key
            // 
            this.Key.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.Key, "Key");
            this.Key.Name = "Key";
            this.Key.ReadOnly = true;
            // 
            // but_save
            // 
            resources.ApplyResources(this.but_save, "but_save");
            this.but_save.Name = "but_save";
            this.but_save.UseVisualStyleBackColor = true;
            this.but_save.Click += new System.EventHandler(this.but_save_Click);
            // 
            // but_add
            // 
            resources.ApplyResources(this.but_add, "but_add");
            this.but_add.Name = "but_add";
            this.but_add.UseVisualStyleBackColor = true;
            this.but_add.Click += new System.EventHandler(this.but_add_Click);
            // 
            // but_disablesigning
            // 
            resources.ApplyResources(this.but_disablesigning, "but_disablesigning");
            this.but_disablesigning.Name = "but_disablesigning";
            this.but_disablesigning.UseVisualStyleBackColor = true;
            this.but_disablesigning.Click += new System.EventHandler(this.but_disablesigning_Click);
            // 
            // AuthKeys
            // 
            
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.but_disablesigning);
            this.Controls.Add(this.but_add);
            this.Controls.Add(this.but_save);
            this.Controls.Add(this.dataGridView1);
            this.Name = "AuthKeys";
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