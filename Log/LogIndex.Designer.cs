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
            this.objectListView1 = new BrightIdeasSoftware.FastObjectListView();
            this.olvColumnImage = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumndir = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnFrame = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSysid = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnduration = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnHome = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnTimeInAir = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnDistTraveled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.BUT_changedir = new MissionPlanner.Controls.MyButton();
            this.olvColumnCamMSG = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumnImage);
            this.objectListView1.AllColumns.Add(this.olvColumnDate);
            this.objectListView1.AllColumns.Add(this.olvColumndir);
            this.objectListView1.AllColumns.Add(this.olvColumnFrame);
            this.objectListView1.AllColumns.Add(this.olvColumnSysid);
            this.objectListView1.AllColumns.Add(this.olvColumnduration);
            this.objectListView1.AllColumns.Add(this.olvColumnName);
            this.objectListView1.AllColumns.Add(this.olvColumnSize);
            this.objectListView1.AllColumns.Add(this.olvColumnHome);
            this.objectListView1.AllColumns.Add(this.olvColumnTimeInAir);
            this.objectListView1.AllColumns.Add(this.olvColumnDistTraveled);
            this.objectListView1.AllColumns.Add(this.olvColumnCamMSG);
            this.objectListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnImage,
            this.olvColumnDate,
            this.olvColumndir,
            this.olvColumnFrame,
            this.olvColumnSysid,
            this.olvColumnduration,
            this.olvColumnName,
            this.olvColumnSize,
            this.olvColumnHome,
            this.olvColumnTimeInAir,
            this.olvColumnDistTraveled,
            this.olvColumnCamMSG});
            this.objectListView1.Location = new System.Drawing.Point(12, 42);
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.RowHeight = 150;
            this.objectListView1.Size = new System.Drawing.Size(1153, 460);
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
            // olvColumnDate
            // 
            this.olvColumnDate.AspectName = "Date";
            this.olvColumnDate.AspectToStringFormat = "{0:yyyy/MM/dd hh\\:mm}";
            this.olvColumnDate.CellPadding = null;
            this.olvColumnDate.Text = "Date";
            this.olvColumnDate.Width = 100;
            // 
            // olvColumndir
            // 
            this.olvColumndir.AspectName = "Directory";
            this.olvColumndir.CellPadding = null;
            this.olvColumndir.Text = "Directory";
            this.olvColumndir.Width = 258;
            // 
            // olvColumnFrame
            // 
            this.olvColumnFrame.AspectName = "Frame";
            this.olvColumnFrame.CellPadding = null;
            this.olvColumnFrame.Text = "Frame";
            // 
            // olvColumnSysid
            // 
            this.olvColumnSysid.AspectName = "Aircraft";
            this.olvColumnSysid.CellPadding = null;
            this.olvColumnSysid.Text = "Aircraft";
            // 
            // olvColumnduration
            // 
            this.olvColumnduration.AspectName = "Duration";
            this.olvColumnduration.AspectToStringFormat = "{0:hh\\:mm\\:ss}";
            this.olvColumnduration.CellPadding = null;
            this.olvColumnduration.Text = "Duration";
            this.olvColumnduration.Width = 100;
            // 
            // olvColumnName
            // 
            this.olvColumnName.AspectName = "Name";
            this.olvColumnName.CellPadding = null;
            this.olvColumnName.Text = "FileName";
            this.olvColumnName.Width = 178;
            // 
            // olvColumnSize
            // 
            this.olvColumnSize.AspectName = "Size";
            this.olvColumnSize.CellPadding = null;
            this.olvColumnSize.Text = "Size";
            // 
            // olvColumnHome
            // 
            this.olvColumnHome.AspectName = "Home";
            this.olvColumnHome.CellPadding = null;
            this.olvColumnHome.Text = "Home";
            // 
            // olvColumnTimeInAir
            // 
            this.olvColumnTimeInAir.AspectName = "TimeInAir";
            this.olvColumnTimeInAir.CellPadding = null;
            this.olvColumnTimeInAir.Text = "TimeInAir";
            // 
            // olvColumnDistTraveled
            // 
            this.olvColumnDistTraveled.AspectName = "DistTraveled";
            this.olvColumnDistTraveled.CellPadding = null;
            this.olvColumnDistTraveled.Text = "DistTraveled";
            // 
            // BUT_changedir
            // 
            this.BUT_changedir.AutoSize = true;
            this.BUT_changedir.Location = new System.Drawing.Point(13, 13);
            this.BUT_changedir.Name = "BUT_changedir";
            this.BUT_changedir.Size = new System.Drawing.Size(99, 23);
            this.BUT_changedir.TabIndex = 1;
            this.BUT_changedir.Text = "Change Directory";
            this.BUT_changedir.UseVisualStyleBackColor = true;
            this.BUT_changedir.Click += new System.EventHandler(this.BUT_changedir_Click);
            // 
            // olvColumnCamMSG
            // 
            this.olvColumnCamMSG.AspectName = "CamMSG";
            this.olvColumnCamMSG.CellPadding = null;
            this.olvColumnCamMSG.Text = "CamMSG";
            // 
            // LogIndex
            // 
            
            this.ClientSize = new System.Drawing.Size(1177, 514);
            this.Controls.Add(this.BUT_changedir);
            this.Controls.Add(this.objectListView1);
            this.Name = "LogIndex";
            this.Text = "LogIndex";
            this.Load += new System.EventHandler(this.LogIndex_Load);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView objectListView1;
        private BrightIdeasSoftware.OLVColumn olvColumnImage;
        private BrightIdeasSoftware.OLVColumn olvColumnName;
        private BrightIdeasSoftware.OLVColumn olvColumnduration;
        private BrightIdeasSoftware.OLVColumn olvColumndir;
        private Controls.MyButton BUT_changedir;
        private BrightIdeasSoftware.OLVColumn olvColumnDate;
        private BrightIdeasSoftware.OLVColumn olvColumnSysid;
        private BrightIdeasSoftware.OLVColumn olvColumnSize;
        private BrightIdeasSoftware.OLVColumn olvColumnHome;
        private BrightIdeasSoftware.OLVColumn olvColumnTimeInAir;
        private BrightIdeasSoftware.OLVColumn olvColumnFrame;
        private BrightIdeasSoftware.OLVColumn olvColumnDistTraveled;
        private BrightIdeasSoftware.OLVColumn olvColumnCamMSG;
    }
}