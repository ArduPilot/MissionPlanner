namespace MissionPlanner.Log
{
    partial class LogBrowse
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogBrowse));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.BUT_Graphit = new MissionPlanner.Controls.MyButton();
            this.BUT_cleargraph = new MissionPlanner.Controls.MyButton();
            this.BUT_loadlog = new MissionPlanner.Controls.MyButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.zg1 = new ZedGraph.ZedGraphControl();
            this.myGMAP1 = new MissionPlanner.Controls.myGMAP();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.CMB_preselect = new System.Windows.Forms.ComboBox();
            this.BUT_removeitem = new MissionPlanner.Controls.MyButton();
            this.CHK_map = new System.Windows.Forms.CheckBox();
            this.BUT_Graphit_R = new MissionPlanner.Controls.MyButton();
            this.chk_time = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView1_CellValueNeeded);
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            this.dataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView1_RowPostPaint);
            // 
            // BUT_Graphit
            // 
            resources.ApplyResources(this.BUT_Graphit, "BUT_Graphit");
            this.BUT_Graphit.Name = "BUT_Graphit";
            this.BUT_Graphit.UseVisualStyleBackColor = true;
            this.BUT_Graphit.Click += new System.EventHandler(this.Graphit_Click);
            // 
            // BUT_cleargraph
            // 
            resources.ApplyResources(this.BUT_cleargraph, "BUT_cleargraph");
            this.BUT_cleargraph.Name = "BUT_cleargraph";
            this.BUT_cleargraph.UseVisualStyleBackColor = true;
            this.BUT_cleargraph.Click += new System.EventHandler(this.BUT_cleargraph_Click);
            // 
            // BUT_loadlog
            // 
            resources.ApplyResources(this.BUT_loadlog, "BUT_loadlog");
            this.BUT_loadlog.Name = "BUT_loadlog";
            this.BUT_loadlog.UseVisualStyleBackColor = true;
            this.BUT_loadlog.Click += new System.EventHandler(this.BUT_loadlog_Click);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chk_time);
            this.splitContainer1.Panel2.Controls.Add(this.treeView1);
            this.splitContainer1.Panel2.Controls.Add(this.CMB_preselect);
            this.splitContainer1.Panel2.Controls.Add(this.BUT_removeitem);
            this.splitContainer1.Panel2.Controls.Add(this.CHK_map);
            this.splitContainer1.Panel2.Controls.Add(this.BUT_Graphit_R);
            this.splitContainer1.Panel2.Controls.Add(this.BUT_Graphit);
            this.splitContainer1.Panel2.Controls.Add(this.BUT_loadlog);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel2.Controls.Add(this.BUT_cleargraph);
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.zg1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.myGMAP1);
            this.splitContainer2.Panel2Collapsed = true;
            // 
            // zg1
            // 
            resources.ApplyResources(this.zg1, "zg1");
            this.zg1.Name = "zg1";
            this.zg1.ScrollGrace = 0D;
            this.zg1.ScrollMaxX = 0D;
            this.zg1.ScrollMaxY = 0D;
            this.zg1.ScrollMaxY2 = 0D;
            this.zg1.ScrollMinX = 0D;
            this.zg1.ScrollMinY = 0D;
            this.zg1.ScrollMinY2 = 0D;
            this.zg1.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zg1_ZoomEvent);
            this.zg1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.zg1_MouseDoubleClick);
            // 
            // myGMAP1
            // 
            this.myGMAP1.Bearing = 0F;
            this.myGMAP1.CanDragMap = true;
            resources.ApplyResources(this.myGMAP1, "myGMAP1");
            this.myGMAP1.EmptyTileColor = System.Drawing.Color.Navy;
            this.myGMAP1.GrayScaleMode = false;
            this.myGMAP1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.myGMAP1.LevelsKeepInMemmory = 5;
            this.myGMAP1.MarkersEnabled = true;
            this.myGMAP1.MaxZoom = 21;
            this.myGMAP1.MinZoom = 2;
            this.myGMAP1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.myGMAP1.Name = "myGMAP1";
            this.myGMAP1.NegativeMode = false;
            this.myGMAP1.PolygonsEnabled = true;
            this.myGMAP1.RetryLoadTile = 0;
            this.myGMAP1.RoutesEnabled = true;
            this.myGMAP1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.myGMAP1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.myGMAP1.ShowTileGridLines = false;
            this.myGMAP1.Zoom = 0D;
            this.myGMAP1.OnRouteClick += new GMap.NET.WindowsForms.RouteClick(this.myGMAP1_OnRouteClick);
            // 
            // treeView1
            // 
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.CheckBoxes = true;
            this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView1.Name = "treeView1";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeView1.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeView1.Nodes1")))});
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // CMB_preselect
            // 
            this.CMB_preselect.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_preselect, "CMB_preselect");
            this.CMB_preselect.Name = "CMB_preselect";
            this.CMB_preselect.SelectedIndexChanged += new System.EventHandler(this.CMB_preselect_SelectedIndexChanged);
            // 
            // BUT_removeitem
            // 
            resources.ApplyResources(this.BUT_removeitem, "BUT_removeitem");
            this.BUT_removeitem.Name = "BUT_removeitem";
            this.BUT_removeitem.UseVisualStyleBackColor = true;
            this.BUT_removeitem.Click += new System.EventHandler(this.BUT_removeitem_Click);
            // 
            // CHK_map
            // 
            resources.ApplyResources(this.CHK_map, "CHK_map");
            this.CHK_map.Name = "CHK_map";
            this.CHK_map.UseVisualStyleBackColor = true;
            this.CHK_map.CheckedChanged += new System.EventHandler(this.CHK_map_CheckedChanged);
            // 
            // BUT_Graphit_R
            // 
            resources.ApplyResources(this.BUT_Graphit_R, "BUT_Graphit_R");
            this.BUT_Graphit_R.Name = "BUT_Graphit_R";
            this.BUT_Graphit_R.UseVisualStyleBackColor = true;
            this.BUT_Graphit_R.Click += new System.EventHandler(this.BUT_Graphit_R_Click);
            // 
            // chk_time
            // 
            resources.ApplyResources(this.chk_time, "chk_time");
            this.chk_time.Name = "chk_time";
            this.chk_time.UseVisualStyleBackColor = true;
            this.chk_time.CheckedChanged += new System.EventHandler(this.chk_time_CheckedChanged);
            // 
            // LogBrowse
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "LogBrowse";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogBrowse_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private Controls.MyButton BUT_Graphit;
        private Controls.MyButton BUT_cleargraph;
        private Controls.MyButton BUT_loadlog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Controls.MyButton BUT_Graphit_R;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ZedGraph.ZedGraphControl zg1;
        private Controls.myGMAP myGMAP1;
        private System.Windows.Forms.CheckBox CHK_map;
        private Controls.MyButton BUT_removeitem;
        private System.Windows.Forms.ComboBox CMB_preselect;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.CheckBox chk_time;
    }
}

