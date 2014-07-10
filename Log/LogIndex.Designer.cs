namespace MissionPlanner.Log
{
    partial class LogIndex
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
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnImage = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumndir = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnduration = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.BUT_changedir = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumnImage);
            this.objectListView1.AllColumns.Add(this.olvColumndir);
            this.objectListView1.AllColumns.Add(this.olvColumnName);
            this.objectListView1.AllColumns.Add(this.olvColumnduration);
            this.objectListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnImage,
            this.olvColumndir,
            this.olvColumnName,
            this.olvColumnduration});
            this.objectListView1.Location = new System.Drawing.Point(12, 42);
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.RowHeight = 150;
            this.objectListView1.Size = new System.Drawing.Size(982, 460);
            this.objectListView1.TabIndex = 0;
            this.objectListView1.UseCellFormatEvents = true;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            this.objectListView1.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.objectListView1_FormatCell);
            // 
            // olvColumnImage
            // 
            this.olvColumnImage.AspectName = "";
            this.olvColumnImage.CellPadding = null;
            this.olvColumnImage.Text = "";
            this.olvColumnImage.Width = 150;
            // 
            // olvColumndir
            // 
            this.olvColumndir.AspectName = "Directory";
            this.olvColumndir.CellPadding = null;
            this.olvColumndir.Text = "Directory";
            // 
            // olvColumnName
            // 
            this.olvColumnName.AspectName = "Name";
            this.olvColumnName.CellPadding = null;
            this.olvColumnName.Text = "FileName";
            this.olvColumnName.Width = 120;
            // 
            // olvColumnduration
            // 
            this.olvColumnduration.AspectName = "Duration";
            this.olvColumnduration.AspectToStringFormat = "{0:hh\\:mm\\:ss}";
            this.olvColumnduration.CellPadding = null;
            this.olvColumnduration.Text = "Duration";
            // 
            // BUT_changedir
            // 
            this.BUT_changedir.Location = new System.Drawing.Point(13, 13);
            this.BUT_changedir.Name = "BUT_changedir";
            this.BUT_changedir.Size = new System.Drawing.Size(75, 23);
            this.BUT_changedir.TabIndex = 1;
            this.BUT_changedir.Text = "Change Directory";
            this.BUT_changedir.UseVisualStyleBackColor = true;
            this.BUT_changedir.Click += new System.EventHandler(this.BUT_changedir_Click);
            // 
            // LogIndex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 514);
            this.Controls.Add(this.BUT_changedir);
            this.Controls.Add(this.objectListView1);
            this.Name = "LogIndex";
            this.Text = "LogIndex";
            this.Load += new System.EventHandler(this.LogIndex_Load);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView objectListView1;
        private BrightIdeasSoftware.OLVColumn olvColumnImage;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnduration;
        private BrightIdeasSoftware.OLVColumn olvColumndir;
        private Controls.MyButton BUT_changedir;

    }
}