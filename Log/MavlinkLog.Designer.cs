namespace MissionPlanner.Log
{
    partial class MavlinkLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MavlinkLog));
            this.BUT_redokml = new MissionPlanner.Controls.MyButton();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.BUT_humanreadable = new MissionPlanner.Controls.MyButton();
            this.BUT_graphmavlog = new MissionPlanner.Controls.MyButton();
            this.zg1 = new ZedGraph.ZedGraphControl();
            this.BUT_convertcsv = new MissionPlanner.Controls.MyButton();
            this.BUT_paramsfromlog = new MissionPlanner.Controls.MyButton();
            this.BUT_getwpsfromlog = new MissionPlanner.Controls.MyButton();
            this.BUT_matlab = new MissionPlanner.Controls.MyButton();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BUT_redokml
            // 
            resources.ApplyResources(this.BUT_redokml, "BUT_redokml");
            this.BUT_redokml.Name = "BUT_redokml";
            this.BUT_redokml.UseVisualStyleBackColor = true;
            this.BUT_redokml.Click += new System.EventHandler(this.BUT_redokml_Click);
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // BUT_humanreadable
            // 
            resources.ApplyResources(this.BUT_humanreadable, "BUT_humanreadable");
            this.BUT_humanreadable.Name = "BUT_humanreadable";
            this.BUT_humanreadable.UseVisualStyleBackColor = true;
            this.BUT_humanreadable.Click += new System.EventHandler(this.BUT_humanreadable_Click);
            // 
            // BUT_graphmavlog
            // 
            resources.ApplyResources(this.BUT_graphmavlog, "BUT_graphmavlog");
            this.BUT_graphmavlog.Name = "BUT_graphmavlog";
            this.BUT_graphmavlog.UseVisualStyleBackColor = true;
            this.BUT_graphmavlog.Click += new System.EventHandler(this.BUT_graphmavlog_Click);
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
            // 
            // BUT_convertcsv
            // 
            resources.ApplyResources(this.BUT_convertcsv, "BUT_convertcsv");
            this.BUT_convertcsv.Name = "BUT_convertcsv";
            this.BUT_convertcsv.UseVisualStyleBackColor = true;
            this.BUT_convertcsv.Click += new System.EventHandler(this.BUT_convertcsv_Click);
            // 
            // BUT_paramsfromlog
            // 
            resources.ApplyResources(this.BUT_paramsfromlog, "BUT_paramsfromlog");
            this.BUT_paramsfromlog.Name = "BUT_paramsfromlog";
            this.BUT_paramsfromlog.UseVisualStyleBackColor = true;
            this.BUT_paramsfromlog.Click += new System.EventHandler(this.BUT_paramsfromlog_Click);
            // 
            // BUT_getwpsfromlog
            // 
            resources.ApplyResources(this.BUT_getwpsfromlog, "BUT_getwpsfromlog");
            this.BUT_getwpsfromlog.Name = "BUT_getwpsfromlog";
            this.BUT_getwpsfromlog.UseVisualStyleBackColor = true;
            this.BUT_getwpsfromlog.Click += new System.EventHandler(this.BUT_getwpsfromlog_Click);
            // 
            // BUT_matlab
            // 
            resources.ApplyResources(this.BUT_matlab, "BUT_matlab");
            this.BUT_matlab.Name = "BUT_matlab";
            this.BUT_matlab.UseVisualStyleBackColor = true;
            this.BUT_matlab.Click += new System.EventHandler(this.BUT_matlab_Click);
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.Name = "treeView1";
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.zg1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView1);
            // 
            // MavlinkLog
            // 
            
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.BUT_matlab);
            this.Controls.Add(this.BUT_getwpsfromlog);
            this.Controls.Add(this.BUT_paramsfromlog);
            this.Controls.Add(this.BUT_convertcsv);
            this.Controls.Add(this.BUT_graphmavlog);
            this.Controls.Add(this.BUT_humanreadable);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.BUT_redokml);
            this.Name = "MavlinkLog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Log_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.MyButton BUT_redokml;
        private System.Windows.Forms.ProgressBar progressBar1;
        private Controls.MyButton BUT_humanreadable;
        private Controls.MyButton BUT_graphmavlog;
        private ZedGraph.ZedGraphControl zg1;
        private Controls.MyButton BUT_convertcsv;
        private Controls.MyButton BUT_paramsfromlog;
        private Controls.MyButton BUT_getwpsfromlog;
        private Controls.MyButton BUT_matlab;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}