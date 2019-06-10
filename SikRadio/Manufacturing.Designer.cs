namespace RFD900Tools
{
    partial class Manufacturing
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnLockdownAU = new System.Windows.Forms.Button();
            this.btnLockdownNZ = new System.Windows.Forms.Button();
            this.btnLockdownUS = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.btnQueryLockStatus = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lstLog, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(757, 470);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.btnLockdownAU, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnLockdownNZ, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnLockdownUS, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnQueryLockStatus, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 43);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(144, 424);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // btnLockdownAU
            // 
            this.btnLockdownAU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLockdownAU.Location = new System.Drawing.Point(3, 43);
            this.btnLockdownAU.Name = "btnLockdownAU";
            this.btnLockdownAU.Size = new System.Drawing.Size(138, 34);
            this.btnLockdownAU.TabIndex = 0;
            this.btnLockdownAU.Text = "Lockdown AU";
            this.btnLockdownAU.UseVisualStyleBackColor = true;
            this.btnLockdownAU.Click += new System.EventHandler(this.btnLockdownAU_Click);
            // 
            // btnLockdownNZ
            // 
            this.btnLockdownNZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLockdownNZ.Location = new System.Drawing.Point(3, 83);
            this.btnLockdownNZ.Name = "btnLockdownNZ";
            this.btnLockdownNZ.Size = new System.Drawing.Size(138, 34);
            this.btnLockdownNZ.TabIndex = 1;
            this.btnLockdownNZ.Text = "Lockdown NZ";
            this.btnLockdownNZ.UseVisualStyleBackColor = true;
            this.btnLockdownNZ.Click += new System.EventHandler(this.btnLockdownNZ_Click);
            // 
            // btnLockdownUS
            // 
            this.btnLockdownUS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLockdownUS.Location = new System.Drawing.Point(3, 123);
            this.btnLockdownUS.Name = "btnLockdownUS";
            this.btnLockdownUS.Size = new System.Drawing.Size(138, 34);
            this.btnLockdownUS.TabIndex = 2;
            this.btnLockdownUS.Text = "Lockdown US";
            this.btnLockdownUS.UseVisualStyleBackColor = true;
            this.btnLockdownUS.Click += new System.EventHandler(this.btnLockdownUS_Click);
            // 
            // lstLog
            // 
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.ItemHeight = 16;
            this.lstLog.Location = new System.Drawing.Point(153, 43);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(601, 424);
            this.lstLog.TabIndex = 1;
            // 
            // btnQueryLockStatus
            // 
            this.btnQueryLockStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnQueryLockStatus.Location = new System.Drawing.Point(3, 3);
            this.btnQueryLockStatus.Name = "btnQueryLockStatus";
            this.btnQueryLockStatus.Size = new System.Drawing.Size(138, 34);
            this.btnQueryLockStatus.TabIndex = 3;
            this.btnQueryLockStatus.Text = "Query Lockdown";
            this.btnQueryLockStatus.UseVisualStyleBackColor = true;
            this.btnQueryLockStatus.Click += new System.EventHandler(this.btnQueryLockdown_Click);
            // 
            // Manufacturing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Manufacturing";
            this.Size = new System.Drawing.Size(757, 470);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button btnLockdownAU;
        private System.Windows.Forms.Button btnLockdownNZ;
        private System.Windows.Forms.Button btnLockdownUS;
        private System.Windows.Forms.Button btnQueryLockStatus;
    }
}
