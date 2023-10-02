using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigRawParams
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigRawParams));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.BUT_compare = new MissionPlanner.Controls.MyButton();
            this.BUT_rerequestparams = new MissionPlanner.Controls.MyButton();
            this.BUT_writePIDS = new MissionPlanner.Controls.MyButton();
            this.BUT_save = new MissionPlanner.Controls.MyButton();
            this.BUT_load = new MissionPlanner.Controls.MyButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.BUT_paramfileload = new MissionPlanner.Controls.MyButton();
            this.CMB_paramfiles = new System.Windows.Forms.ComboBox();
            this.BUT_reset_params = new MissionPlanner.Controls.MyButton();
            this.txt_search = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BUT_commitToFlash = new MissionPlanner.Controls.MyButton();
            this.chk_modified = new System.Windows.Forms.CheckBox();
            this.BUT_refreshTable = new MissionPlanner.Controls.MyButton();
            this.chk_none_default = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.Params = new MissionPlanner.Controls.MyDataGridView();
            this.Command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Default_value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Units = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Options = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fav = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.but_collapse = new MissionPlanner.Controls.MyButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Params)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BUT_compare
            // 
            resources.ApplyResources(this.BUT_compare, "BUT_compare");
            this.BUT_compare.Name = "BUT_compare";
            this.BUT_compare.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_compare.UseVisualStyleBackColor = true;
            this.BUT_compare.Click += new System.EventHandler(this.BUT_compare_Click);
            // 
            // BUT_rerequestparams
            // 
            resources.ApplyResources(this.BUT_rerequestparams, "BUT_rerequestparams");
            this.BUT_rerequestparams.Name = "BUT_rerequestparams";
            this.BUT_rerequestparams.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_rerequestparams.UseVisualStyleBackColor = true;
            this.BUT_rerequestparams.Click += new System.EventHandler(this.BUT_rerequestparams_Click);
            // 
            // BUT_writePIDS
            // 
            resources.ApplyResources(this.BUT_writePIDS, "BUT_writePIDS");
            this.BUT_writePIDS.Name = "BUT_writePIDS";
            this.BUT_writePIDS.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_writePIDS.UseVisualStyleBackColor = true;
            this.BUT_writePIDS.Click += new System.EventHandler(this.BUT_writePIDS_Click);
            // 
            // BUT_save
            // 
            resources.ApplyResources(this.BUT_save, "BUT_save");
            this.BUT_save.Name = "BUT_save";
            this.BUT_save.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_save.UseVisualStyleBackColor = true;
            this.BUT_save.Click += new System.EventHandler(this.BUT_save_Click);
            // 
            // BUT_load
            // 
            resources.ApplyResources(this.BUT_load, "BUT_load");
            this.BUT_load.Name = "BUT_load";
            this.BUT_load.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_load.UseVisualStyleBackColor = true;
            this.BUT_load.Click += new System.EventHandler(this.BUT_load_Click);
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
            // BUT_paramfileload
            // 
            resources.ApplyResources(this.BUT_paramfileload, "BUT_paramfileload");
            this.BUT_paramfileload.Name = "BUT_paramfileload";
            this.BUT_paramfileload.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_paramfileload.UseVisualStyleBackColor = true;
            this.BUT_paramfileload.Click += new System.EventHandler(this.BUT_paramfileload_Click);
            // 
            // CMB_paramfiles
            // 
            resources.ApplyResources(this.CMB_paramfiles, "CMB_paramfiles");
            this.CMB_paramfiles.FormattingEnabled = true;
            this.CMB_paramfiles.Name = "CMB_paramfiles";
            this.CMB_paramfiles.SelectedIndexChanged += new System.EventHandler(this.CMB_paramfiles_SelectedIndexChanged);
            // 
            // BUT_reset_params
            // 
            resources.ApplyResources(this.BUT_reset_params, "BUT_reset_params");
            this.BUT_reset_params.Name = "BUT_reset_params";
            this.BUT_reset_params.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
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
            this.BUT_commitToFlash.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_commitToFlash.UseVisualStyleBackColor = true;
            this.BUT_commitToFlash.Click += new System.EventHandler(this.BUT_commitToFlash_Click);
            // 
            // chk_modified
            // 
            resources.ApplyResources(this.chk_modified, "chk_modified");
            this.chk_modified.Name = "chk_modified";
            this.chk_modified.UseVisualStyleBackColor = true;
            this.chk_modified.CheckedChanged += new System.EventHandler(this.chk_filter_CheckedChanged);
            // 
            // BUT_refreshTable
            // 
            resources.ApplyResources(this.BUT_refreshTable, "BUT_refreshTable");
            this.BUT_refreshTable.Name = "BUT_refreshTable";
            this.BUT_refreshTable.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_refreshTable.UseVisualStyleBackColor = true;
            this.BUT_refreshTable.Click += new System.EventHandler(this.BUT_refreshTable_Click);
            // 
            // chk_none_default
            // 
            resources.ApplyResources(this.chk_none_default, "chk_none_default");
            this.chk_none_default.Name = "chk_none_default";
            this.chk_none_default.UseVisualStyleBackColor = true;
            this.chk_none_default.CheckedChanged += new System.EventHandler(this.chk_filter_CheckedChanged);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.Params);
            this.splitContainer1.Panel2.Controls.Add(this.but_collapse);
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            // 
            // treeView1
            // 
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.Name = "treeView1";
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // Params
            // 
            this.Params.AllowUserToAddRows = false;
            this.Params.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Params.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Params.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Command,
            this.Value,
            this.Default_value,
            this.Units,
            this.Options,
            this.Desc,
            this.Fav});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Params.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.Params, "Params");
            this.Params.Name = "Params";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Params.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.Params.RowHeadersVisible = false;
            this.Params.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.Params.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.Params_CellBeginEdit);
            this.Params.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Params_CellContentClick);
            this.Params.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Params_CellValueChanged);
            this.Params.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.Params_ColumnWidthChanged);
            this.Params.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.Params_RowEnter);
            this.Params.RowHeightChanged += new System.Windows.Forms.DataGridViewRowEventHandler(this.Params_RowHeightChanged);
            this.Params.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Params_Scroll);
            // 
            // Command
            // 
            this.Command.FillWeight = 20F;
            resources.ApplyResources(this.Command, "Command");
            this.Command.Name = "Command";
            this.Command.ReadOnly = true;
            // 
            // Value
            // 
            this.Value.FillWeight = 11F;
            resources.ApplyResources(this.Value, "Value");
            this.Value.Name = "Value";
            // 
            // Default_value
            // 
            this.Default_value.FillWeight = 11F;
            resources.ApplyResources(this.Default_value, "Default_value");
            this.Default_value.Name = "Default_value";
            this.Default_value.ReadOnly = true;
            // 
            // Units
            // 
            this.Units.FillWeight = 9F;
            resources.ApplyResources(this.Units, "Units");
            this.Units.Name = "Units";
            this.Units.ReadOnly = true;
            // 
            // Options
            // 
            this.Options.FillWeight = 28F;
            resources.ApplyResources(this.Options, "Options");
            this.Options.Name = "Options";
            this.Options.ReadOnly = true;
            // 
            // Desc
            // 
            this.Desc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Desc.DefaultCellStyle = dataGridViewCellStyle2;
            this.Desc.FillWeight = 25F;
            resources.ApplyResources(this.Desc, "Desc");
            this.Desc.Name = "Desc";
            this.Desc.ReadOnly = true;
            // 
            // Fav
            // 
            this.Fav.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Fav.FillWeight = 4F;
            resources.ApplyResources(this.Fav, "Fav");
            this.Fav.Name = "Fav";
            this.Fav.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // but_collapse
            // 
            resources.ApplyResources(this.but_collapse, "but_collapse");
            this.but_collapse.Name = "but_collapse";
            this.but_collapse.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_collapse.UseVisualStyleBackColor = true;
            this.but_collapse.Click += new System.EventHandler(this.but_collapse_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.BUT_refreshTable, 0, 16);
            this.tableLayoutPanel1.Controls.Add(this.chk_none_default, 0, 14);
            this.tableLayoutPanel1.Controls.Add(this.txt_search, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.BUT_load, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.BUT_save, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.BUT_rerequestparams, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.BUT_commitToFlash, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.BUT_writePIDS, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.BUT_reset_params, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.BUT_compare, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.BUT_paramfileload, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.CMB_paramfiles, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.chk_modified, 0, 13);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // ConfigRawParams
            // 
            this.Controls.Add(this.splitContainer1);
            this.Name = "ConfigRawParams";
            resources.ApplyResources(this, "$this");
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Params)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.MyButton BUT_compare;
        private Controls.MyButton BUT_rerequestparams;
        private Controls.MyButton BUT_writePIDS;
        private Controls.MyButton BUT_save;
        private Controls.MyButton BUT_load;
        private Controls.MyDataGridView Params;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private Controls.MyButton BUT_paramfileload;
        private System.Windows.Forms.ComboBox CMB_paramfiles;
        private Controls.MyButton BUT_reset_params;
        private System.Windows.Forms.TextBox txt_search;
        private System.Windows.Forms.Label label2;
        private MyButton BUT_commitToFlash;
        private System.Windows.Forms.CheckBox chk_modified;
        private MyButton BUT_refreshTable;
        private System.Windows.Forms.CheckBox chk_none_default;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView treeView1;
        private MyButton but_collapse;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn Default_value;
        private System.Windows.Forms.DataGridViewTextBoxColumn Units;
        private System.Windows.Forms.DataGridViewTextBoxColumn Options;
        private System.Windows.Forms.DataGridViewTextBoxColumn Desc;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Fav;
    }
}
