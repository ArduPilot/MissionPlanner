namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigRawParamsTree
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigRawParamsTree));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.CMB_paramfiles = new System.Windows.Forms.ComboBox();
            this.Params = new BrightIdeasSoftware.DataTreeListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.BUT_compare = new MissionPlanner.Controls.MyButton();
            this.BUT_rerequestparams = new MissionPlanner.Controls.MyButton();
            this.BUT_writePIDS = new MissionPlanner.Controls.MyButton();
            this.BUT_save = new MissionPlanner.Controls.MyButton();
            this.BUT_load = new MissionPlanner.Controls.MyButton();
            this.BUT_paramfileload = new MissionPlanner.Controls.MyButton();
            this.BUT_reset_params = new MissionPlanner.Controls.MyButton();
            this.txt_search = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BUT_commitToFlash = new MissionPlanner.Controls.MyButton();
            this.chk_modified = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Params)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 180000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // CMB_paramfiles
            // 
            resources.ApplyResources(this.CMB_paramfiles, "CMB_paramfiles");
            this.CMB_paramfiles.FormattingEnabled = true;
            this.CMB_paramfiles.Name = "CMB_paramfiles";
            // 
            // Params
            // 
            this.Params.AllColumns.Add(this.olvColumn1);
            this.Params.AllColumns.Add(this.olvColumn2);
            this.Params.AllColumns.Add(this.olvColumn3);
            this.Params.AllColumns.Add(this.olvColumn4);
            this.Params.AllColumns.Add(this.olvColumn5);
            resources.ApplyResources(this.Params, "Params");
            this.Params.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5});
            this.Params.DataSource = null;
            this.Params.ForeColor = System.Drawing.Color.White;
            this.Params.HideSelection = false;
            this.Params.Name = "Params";
            this.Params.OwnerDraw = true;
            this.Params.RootKeyValueString = "";
            this.Params.RowHeight = 26;
            this.Params.ShowGroups = false;
            this.Params.ShowItemToolTips = true;
            this.Params.UseAlternatingBackColors = true;
            this.Params.UseCompatibleStateImageBehavior = false;
            this.Params.View = System.Windows.Forms.View.Details;
            this.Params.VirtualMode = true;
            this.Params.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.Params_CellEditFinishing);
            this.Params.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.Params_CellClick);
            this.Params.CellOver += new System.EventHandler<BrightIdeasSoftware.CellOverEventArgs>(this.Params_CellOver);
            this.Params.CellToolTipShowing += new System.EventHandler<BrightIdeasSoftware.ToolTipShowingEventArgs>(this.Params_CellToolTipShowing);
            this.Params.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.Params_FormatRow);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "paramname";
            this.olvColumn1.CellPadding = null;
            this.olvColumn1.IsEditable = false;
            resources.ApplyResources(this.olvColumn1, "olvColumn1");
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Value";
            this.olvColumn2.AutoCompleteEditor = false;
            this.olvColumn2.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.olvColumn2.CellPadding = null;
            resources.ApplyResources(this.olvColumn2, "olvColumn2");
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "unit";
            this.olvColumn3.CellPadding = null;
            this.olvColumn3.IsEditable = false;
            resources.ApplyResources(this.olvColumn3, "olvColumn3");
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "range";
            this.olvColumn4.CellPadding = null;
            this.olvColumn4.IsEditable = false;
            resources.ApplyResources(this.olvColumn4, "olvColumn4");
            this.olvColumn4.WordWrap = true;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "desc";
            this.olvColumn5.CellPadding = null;
            this.olvColumn5.FillsFreeSpace = true;
            this.olvColumn5.IsEditable = false;
            resources.ApplyResources(this.olvColumn5, "olvColumn5");
            this.olvColumn5.WordWrap = true;
            // 
            // BUT_compare
            // 
            resources.ApplyResources(this.BUT_compare, "BUT_compare");
            this.BUT_compare.Name = "BUT_compare";
            this.BUT_compare.UseVisualStyleBackColor = true;
            this.BUT_compare.Click += new System.EventHandler(this.BUT_compare_Click);
            // 
            // BUT_rerequestparams
            // 
            resources.ApplyResources(this.BUT_rerequestparams, "BUT_rerequestparams");
            this.BUT_rerequestparams.Name = "BUT_rerequestparams";
            this.BUT_rerequestparams.UseVisualStyleBackColor = true;
            this.BUT_rerequestparams.Click += new System.EventHandler(this.BUT_rerequestparams_Click);
            // 
            // BUT_writePIDS
            // 
            resources.ApplyResources(this.BUT_writePIDS, "BUT_writePIDS");
            this.BUT_writePIDS.Name = "BUT_writePIDS";
            this.BUT_writePIDS.UseVisualStyleBackColor = true;
            this.BUT_writePIDS.Click += new System.EventHandler(this.BUT_writePIDS_Click);
            // 
            // BUT_save
            // 
            resources.ApplyResources(this.BUT_save, "BUT_save");
            this.BUT_save.Name = "BUT_save";
            this.BUT_save.UseVisualStyleBackColor = true;
            this.BUT_save.Click += new System.EventHandler(this.BUT_save_Click);
            // 
            // BUT_load
            // 
            resources.ApplyResources(this.BUT_load, "BUT_load");
            this.BUT_load.Name = "BUT_load";
            this.BUT_load.UseVisualStyleBackColor = true;
            this.BUT_load.Click += new System.EventHandler(this.BUT_load_Click);
            // 
            // BUT_paramfileload
            // 
            resources.ApplyResources(this.BUT_paramfileload, "BUT_paramfileload");
            this.BUT_paramfileload.Name = "BUT_paramfileload";
            this.BUT_paramfileload.UseVisualStyleBackColor = true;
            this.BUT_paramfileload.Click += new System.EventHandler(this.BUT_paramfileload_Click);
            // 
            // BUT_reset_params
            // 
            resources.ApplyResources(this.BUT_reset_params, "BUT_reset_params");
            this.BUT_reset_params.Name = "BUT_reset_params";
            this.BUT_reset_params.UseVisualStyleBackColor = true;
            this.BUT_reset_params.Click += new System.EventHandler(this.BUT_reset_params_Click);
            // 
            // txt_search
            // 
            resources.ApplyResources(this.txt_search, "txt_search");
            this.txt_search.Name = "txt_search";
            this.txt_search.TextChanged += new System.EventHandler(this.txt_search_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // BUT_commitToFlash
            // 
            resources.ApplyResources(this.BUT_commitToFlash, "BUT_commitToFlash");
            this.BUT_commitToFlash.Name = "BUT_commitToFlash";
            this.BUT_commitToFlash.UseVisualStyleBackColor = true;
            this.BUT_commitToFlash.Click += new System.EventHandler(this.BUT_commitToFlash_Click);
            // 
            // chk_modified
            // 
            resources.ApplyResources(this.chk_modified, "chk_modified");
            this.chk_modified.Name = "chk_modified";
            this.chk_modified.UseVisualStyleBackColor = true;
            this.chk_modified.CheckedChanged += new System.EventHandler(this.chk_modified_CheckedChanged);
            // 
            // ConfigRawParamsTree
            // 
            this.Controls.Add(this.chk_modified);
            this.Controls.Add(this.BUT_commitToFlash);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_search);
            this.Controls.Add(this.Params);
            this.Controls.Add(this.BUT_reset_params);
            this.Controls.Add(this.BUT_paramfileload);
            this.Controls.Add(this.CMB_paramfiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BUT_compare);
            this.Controls.Add(this.BUT_rerequestparams);
            this.Controls.Add(this.BUT_writePIDS);
            this.Controls.Add(this.BUT_save);
            this.Controls.Add(this.BUT_load);
            this.Name = "ConfigRawParamsTree";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.Params)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyButton BUT_compare;
        private Controls.MyButton BUT_rerequestparams;
        private Controls.MyButton BUT_writePIDS;
        private Controls.MyButton BUT_save;
        private Controls.MyButton BUT_load;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private Controls.MyButton BUT_paramfileload;
        private System.Windows.Forms.ComboBox CMB_paramfiles;
        private Controls.MyButton BUT_reset_params;
        private BrightIdeasSoftware.DataTreeListView Params;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private System.Windows.Forms.TextBox txt_search;
        private System.Windows.Forms.Label label2;
        private Controls.MyButton BUT_commitToFlash;
        private System.Windows.Forms.CheckBox chk_modified;
    }
}
