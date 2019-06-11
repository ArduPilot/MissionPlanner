using MissionPlanner.Controls;

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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportVisibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BUT_Graphit = new MissionPlanner.Controls.MyButton();
            this.BUT_cleargraph = new MissionPlanner.Controls.MyButton();
            this.BUT_loadlog = new MissionPlanner.Controls.MyButton();
            this.splitContainerZgGrid = new System.Windows.Forms.SplitContainer();
            this.splitContainerZgMap = new System.Windows.Forms.SplitContainer();
            this.zg1 = new ZedGraph.ZedGraphControl();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.myGMAP1 = new MissionPlanner.Controls.myGMAP();
            this.splitContainerButGrid = new System.Windows.Forms.SplitContainer();
            this.chk_events = new System.Windows.Forms.CheckBox();
            this.chk_datagrid = new System.Windows.Forms.CheckBox();
            this.chk_msg = new System.Windows.Forms.CheckBox();
            this.chk_errors = new System.Windows.Forms.CheckBox();
            this.chk_mode = new System.Windows.Forms.CheckBox();
            this.BUT_Graphit_R = new MissionPlanner.Controls.MyButton();
            this.chk_time = new System.Windows.Forms.CheckBox();
            this.CHK_map = new System.Windows.Forms.CheckBox();
            this.CMB_preselect = new System.Windows.Forms.ComboBox();
            this.BUT_removeitem = new MissionPlanner.Controls.MyButton();
            this.dataGridView1 = new MissionPlanner.Controls.MyDataGridView();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainerAllTree = new System.Windows.Forms.SplitContainer();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerZgGrid)).BeginInit();
            this.splitContainerZgGrid.Panel1.SuspendLayout();
            this.splitContainerZgGrid.Panel2.SuspendLayout();
            this.splitContainerZgGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerZgMap)).BeginInit();
            this.splitContainerZgMap.Panel1.SuspendLayout();
            this.splitContainerZgMap.Panel2.SuspendLayout();
            this.splitContainerZgMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerButGrid)).BeginInit();
            this.splitContainerButGrid.Panel1.SuspendLayout();
            this.splitContainerButGrid.Panel2.SuspendLayout();
            this.splitContainerButGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAllTree)).BeginInit();
            this.splitContainerAllTree.Panel1.SuspendLayout();
            this.splitContainerAllTree.Panel2.SuspendLayout();
            this.splitContainerAllTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportVisibleToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // exportVisibleToolStripMenuItem
            // 
            this.exportVisibleToolStripMenuItem.Name = "exportVisibleToolStripMenuItem";
            resources.ApplyResources(this.exportVisibleToolStripMenuItem, "exportVisibleToolStripMenuItem");
            this.exportVisibleToolStripMenuItem.Click += new System.EventHandler(this.exportVisibleToolStripMenuItem_Click);
            // 
            // BUT_Graphit
            // 
            this.BUT_Graphit.DialogResult = System.Windows.Forms.DialogResult.None;
            resources.ApplyResources(this.BUT_Graphit, "BUT_Graphit");
            this.BUT_Graphit.Name = "BUT_Graphit";
            this.BUT_Graphit.UseVisualStyleBackColor = true;
            this.BUT_Graphit.Click += new System.EventHandler(this.Graphit_Click);
            // 
            // BUT_cleargraph
            // 
            this.BUT_cleargraph.DialogResult = System.Windows.Forms.DialogResult.None;
            resources.ApplyResources(this.BUT_cleargraph, "BUT_cleargraph");
            this.BUT_cleargraph.Name = "BUT_cleargraph";
            this.BUT_cleargraph.UseVisualStyleBackColor = true;
            this.BUT_cleargraph.Click += new System.EventHandler(this.BUT_cleargraph_Click);
            // 
            // BUT_loadlog
            // 
            this.BUT_loadlog.DialogResult = System.Windows.Forms.DialogResult.None;
            resources.ApplyResources(this.BUT_loadlog, "BUT_loadlog");
            this.BUT_loadlog.Name = "BUT_loadlog";
            this.BUT_loadlog.UseVisualStyleBackColor = true;
            this.BUT_loadlog.Click += new System.EventHandler(this.BUT_loadlog_Click);
            // 
            // splitContainerZgGrid
            // 
            resources.ApplyResources(this.splitContainerZgGrid, "splitContainerZgGrid");
            this.splitContainerZgGrid.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerZgGrid.Name = "splitContainerZgGrid";
            // 
            // splitContainerZgGrid.Panel1
            // 
            this.splitContainerZgGrid.Panel1.Controls.Add(this.splitContainerZgMap);
            // 
            // splitContainerZgGrid.Panel2
            // 
            this.splitContainerZgGrid.Panel2.Controls.Add(this.splitContainerButGrid);
            this.splitContainerZgGrid.Resize += new System.EventHandler(this.splitContainer1_Resize);
            // 
            // splitContainerZgMap
            // 
            resources.ApplyResources(this.splitContainerZgMap, "splitContainerZgMap");
            this.splitContainerZgMap.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerZgMap.Name = "splitContainerZgMap";
            // 
            // splitContainerZgMap.Panel1
            // 
            this.splitContainerZgMap.Panel1.Controls.Add(this.zg1);
            // 
            // splitContainerZgMap.Panel2
            // 
            this.splitContainerZgMap.Panel2.Controls.Add(this.label4);
            this.splitContainerZgMap.Panel2.Controls.Add(this.label3);
            this.splitContainerZgMap.Panel2.Controls.Add(this.label2);
            this.splitContainerZgMap.Panel2.Controls.Add(this.label1);
            this.splitContainerZgMap.Panel2.Controls.Add(this.myGMAP1);
            this.splitContainerZgMap.Panel2Collapsed = true;
            this.splitContainerZgMap.Resize += new System.EventHandler(this.splitContainer2_Resize);
            // 
            // zg1
            // 
            resources.ApplyResources(this.zg1, "zg1");
            this.zg1.IsSynchronizeYAxes = true;
            this.zg1.Name = "zg1";
            this.zg1.ScrollGrace = 0D;
            this.zg1.ScrollMaxX = 0D;
            this.zg1.ScrollMaxY = 0D;
            this.zg1.ScrollMaxY2 = 0D;
            this.zg1.ScrollMinX = 0D;
            this.zg1.ScrollMinY = 0D;
            this.zg1.ScrollMinY2 = 0D;
            this.zg1.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zg1_ZoomEvent);
            this.zg1.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zg1_MouseMoveEvent);
            this.zg1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.zg1_MouseDoubleClick);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.Yellow;
            this.label4.Name = "label4";
            this.label4.Tag = "custom";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Name = "label3";
            this.label3.Tag = "custom";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.Green;
            this.label2.Name = "label2";
            this.label2.Tag = "custom";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Name = "label1";
            this.label1.Tag = "custom";
            // 
            // myGMAP1
            // 
            this.myGMAP1.Bearing = 0F;
            this.myGMAP1.CanDragMap = true;
            resources.ApplyResources(this.myGMAP1, "myGMAP1");
            this.myGMAP1.EmptyTileColor = System.Drawing.Color.Navy;
            this.myGMAP1.GrayScaleMode = false;
            this.myGMAP1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.myGMAP1.HoldInvalidation = false;
            this.myGMAP1.LevelsKeepInMemmory = 5;
            this.myGMAP1.MarkersEnabled = true;
            this.myGMAP1.MaxZoom = 21;
            this.myGMAP1.MinZoom = 2;
            this.myGMAP1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
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
            this.myGMAP1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.myGMAP1_MouseDown);
            this.myGMAP1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.myGMAP1_MouseMove);
            this.myGMAP1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.myGMAP1_MouseUp);
            // 
            // splitContainerButGrid
            // 
            resources.ApplyResources(this.splitContainerButGrid, "splitContainerButGrid");
            this.splitContainerButGrid.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerButGrid.Name = "splitContainerButGrid";
            // 
            // splitContainerButGrid.Panel1
            // 
            this.splitContainerButGrid.Panel1.Controls.Add(this.chk_events);
            this.splitContainerButGrid.Panel1.Controls.Add(this.chk_datagrid);
            this.splitContainerButGrid.Panel1.Controls.Add(this.BUT_Graphit);
            this.splitContainerButGrid.Panel1.Controls.Add(this.chk_msg);
            this.splitContainerButGrid.Panel1.Controls.Add(this.BUT_cleargraph);
            this.splitContainerButGrid.Panel1.Controls.Add(this.chk_errors);
            this.splitContainerButGrid.Panel1.Controls.Add(this.BUT_loadlog);
            this.splitContainerButGrid.Panel1.Controls.Add(this.chk_mode);
            this.splitContainerButGrid.Panel1.Controls.Add(this.BUT_Graphit_R);
            this.splitContainerButGrid.Panel1.Controls.Add(this.chk_time);
            this.splitContainerButGrid.Panel1.Controls.Add(this.CHK_map);
            this.splitContainerButGrid.Panel1.Controls.Add(this.CMB_preselect);
            this.splitContainerButGrid.Panel1.Controls.Add(this.BUT_removeitem);
            // 
            // splitContainerButGrid.Panel2
            // 
            this.splitContainerButGrid.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainerButGrid.Panel2Collapsed = true;
            // 
            // chk_events
            // 
            resources.ApplyResources(this.chk_events, "chk_events");
            this.chk_events.Checked = true;
            this.chk_events.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_events.Name = "chk_events";
            this.chk_events.UseVisualStyleBackColor = true;
            this.chk_events.CheckedChanged += new System.EventHandler(this.chk_events_CheckedChanged);
            // 
            // chk_datagrid
            // 
            resources.ApplyResources(this.chk_datagrid, "chk_datagrid");
            this.chk_datagrid.Name = "chk_datagrid";
            this.chk_datagrid.UseVisualStyleBackColor = true;
            this.chk_datagrid.CheckedChanged += new System.EventHandler(this.chk_datagrid_CheckedChanged);
            // 
            // chk_msg
            // 
            resources.ApplyResources(this.chk_msg, "chk_msg");
            this.chk_msg.Checked = true;
            this.chk_msg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_msg.Name = "chk_msg";
            this.chk_msg.UseVisualStyleBackColor = true;
            this.chk_msg.CheckedChanged += new System.EventHandler(this.chk_msg_CheckedChanged);
            // 
            // chk_errors
            // 
            resources.ApplyResources(this.chk_errors, "chk_errors");
            this.chk_errors.Checked = true;
            this.chk_errors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_errors.Name = "chk_errors";
            this.chk_errors.UseVisualStyleBackColor = true;
            this.chk_errors.CheckedChanged += new System.EventHandler(this.chk_errors_CheckedChanged);
            // 
            // chk_mode
            // 
            resources.ApplyResources(this.chk_mode, "chk_mode");
            this.chk_mode.Checked = true;
            this.chk_mode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_mode.Name = "chk_mode";
            this.chk_mode.UseVisualStyleBackColor = true;
            this.chk_mode.CheckedChanged += new System.EventHandler(this.chk_mode_CheckedChanged);
            // 
            // BUT_Graphit_R
            // 
            this.BUT_Graphit_R.DialogResult = System.Windows.Forms.DialogResult.None;
            resources.ApplyResources(this.BUT_Graphit_R, "BUT_Graphit_R");
            this.BUT_Graphit_R.Name = "BUT_Graphit_R";
            this.BUT_Graphit_R.UseVisualStyleBackColor = true;
            this.BUT_Graphit_R.Click += new System.EventHandler(this.BUT_Graphit_R_Click);
            // 
            // chk_time
            // 
            resources.ApplyResources(this.chk_time, "chk_time");
            this.chk_time.Checked = true;
            this.chk_time.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_time.Name = "chk_time";
            this.chk_time.UseVisualStyleBackColor = true;
            this.chk_time.CheckedChanged += new System.EventHandler(this.chk_time_CheckedChanged);
            // 
            // CHK_map
            // 
            resources.ApplyResources(this.CHK_map, "CHK_map");
            this.CHK_map.Name = "CHK_map";
            this.CHK_map.UseVisualStyleBackColor = true;
            this.CHK_map.CheckedChanged += new System.EventHandler(this.CHK_map_CheckedChanged);
            // 
            // CMB_preselect
            // 
            this.CMB_preselect.DropDownWidth = 300;
            this.CMB_preselect.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_preselect, "CMB_preselect");
            this.CMB_preselect.Name = "CMB_preselect";
            this.CMB_preselect.SelectedIndexChanged += new System.EventHandler(this.CMB_preselect_SelectedIndexChanged);
            // 
            // BUT_removeitem
            // 
            this.BUT_removeitem.DialogResult = System.Windows.Forms.DialogResult.None;
            resources.ApplyResources(this.BUT_removeitem, "BUT_removeitem");
            this.BUT_removeitem.Name = "BUT_removeitem";
            this.BUT_removeitem.UseVisualStyleBackColor = true;
            this.BUT_removeitem.Click += new System.EventHandler(this.BUT_removeitem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
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
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView1.Name = "treeView1";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeView1.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeView1.Nodes1")))});
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // splitContainerAllTree
            // 
            resources.ApplyResources(this.splitContainerAllTree, "splitContainerAllTree");
            this.splitContainerAllTree.Name = "splitContainerAllTree";
            // 
            // splitContainerAllTree.Panel1
            // 
            this.splitContainerAllTree.Panel1.Controls.Add(this.splitContainerZgGrid);
            // 
            // splitContainerAllTree.Panel2
            // 
            this.splitContainerAllTree.Panel2.Controls.Add(this.treeView1);
            // 
            // LogBrowse
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.splitContainerAllTree);
            this.Name = "LogBrowse";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogBrowse_FormClosed);
            this.Load += new System.EventHandler(this.LogBrowse_Load);
            this.Resize += new System.EventHandler(this.LogBrowse_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainerZgGrid.Panel1.ResumeLayout(false);
            this.splitContainerZgGrid.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerZgGrid)).EndInit();
            this.splitContainerZgGrid.ResumeLayout(false);
            this.splitContainerZgMap.Panel1.ResumeLayout(false);
            this.splitContainerZgMap.Panel2.ResumeLayout(false);
            this.splitContainerZgMap.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerZgMap)).EndInit();
            this.splitContainerZgMap.ResumeLayout(false);
            this.splitContainerButGrid.Panel1.ResumeLayout(false);
            this.splitContainerButGrid.Panel1.PerformLayout();
            this.splitContainerButGrid.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerButGrid)).EndInit();
            this.splitContainerButGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainerAllTree.Panel1.ResumeLayout(false);
            this.splitContainerAllTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAllTree)).EndInit();
            this.splitContainerAllTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.MyDataGridView dataGridView1;
        private Controls.MyButton BUT_Graphit;
        private Controls.MyButton BUT_cleargraph;
        private Controls.MyButton BUT_loadlog;
        private System.Windows.Forms.SplitContainer splitContainerZgGrid;
        private Controls.MyButton BUT_Graphit_R;
        private System.Windows.Forms.SplitContainer splitContainerZgMap;
        private ZedGraph.ZedGraphControl zg1;
        private Controls.myGMAP myGMAP1;
        private System.Windows.Forms.CheckBox CHK_map;
        private Controls.MyButton BUT_removeitem;
        private System.Windows.Forms.ComboBox CMB_preselect;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.CheckBox chk_time;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportVisibleToolStripMenuItem;
        private System.Windows.Forms.CheckBox chk_mode;
        private System.Windows.Forms.CheckBox chk_msg;
        private System.Windows.Forms.CheckBox chk_errors;
        private System.Windows.Forms.SplitContainer splitContainerAllTree;
        private System.Windows.Forms.SplitContainer splitContainerButGrid;
        private System.Windows.Forms.CheckBox chk_datagrid;
        private System.Windows.Forms.CheckBox chk_events;
    }
}

