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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.BUT_compare = new MissionPlanner.Controls.MyButton();
            this.BUT_rerequestparams = new MissionPlanner.Controls.MyButton();
            this.BUT_writePIDS = new MissionPlanner.Controls.MyButton();
            this.BUT_save = new MissionPlanner.Controls.MyButton();
            this.BUT_load = new MissionPlanner.Controls.MyButton();
            this.Params = new System.Windows.Forms.DataGridView();
            this.Command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Units = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Options = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.BUT_find = new MissionPlanner.Controls.MyButton();
            this.BUT_paramfileload = new MissionPlanner.Controls.MyButton();
            this.CMB_paramfiles = new System.Windows.Forms.ComboBox();
            this.BUT_reset_params = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.Params)).BeginInit();
            this.SuspendLayout();
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
            // Params
            // 
            this.Params.AllowUserToAddRows = false;
            this.Params.AllowUserToDeleteRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.Black;
            this.Params.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.Params, "Params");
            this.Params.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Params.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.Params.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Params.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Command,
            this.Value,
            this.Units,
            this.Options,
            this.Desc});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Params.DefaultCellStyle = dataGridViewCellStyle9;
            this.Params.Name = "Params";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Params.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.Params.RowHeadersVisible = false;
            this.Params.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.Params.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Params_CellValueChanged);
            // 
            // Command
            // 
            resources.ApplyResources(this.Command, "Command");
            this.Command.Name = "Command";
            this.Command.ReadOnly = true;
            // 
            // Value
            // 
            resources.ApplyResources(this.Value, "Value");
            this.Value.Name = "Value";
            // 
            // Units
            // 
            this.Units.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            resources.ApplyResources(this.Units, "Units");
            this.Units.Name = "Units";
            this.Units.ReadOnly = true;
            // 
            // Options
            // 
            this.Options.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.Options, "Options");
            this.Options.Name = "Options";
            this.Options.ReadOnly = true;
            // 
            // Desc
            // 
            this.Desc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Desc.DefaultCellStyle = dataGridViewCellStyle8;
            resources.ApplyResources(this.Desc, "Desc");
            this.Desc.Name = "Desc";
            this.Desc.ReadOnly = true;
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
            // BUT_find
            // 
            resources.ApplyResources(this.BUT_find, "BUT_find");
            this.BUT_find.Name = "BUT_find";
            this.BUT_find.UseVisualStyleBackColor = true;
            this.BUT_find.Click += new System.EventHandler(this.BUT_find_Click);
            // 
            // BUT_paramfileload
            // 
            resources.ApplyResources(this.BUT_paramfileload, "BUT_paramfileload");
            this.BUT_paramfileload.Name = "BUT_paramfileload";
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
            this.BUT_reset_params.UseVisualStyleBackColor = true;
            this.BUT_reset_params.Click += new System.EventHandler(this.BUT_reset_params_Click);
            // 
            // ConfigRawParams
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.BUT_reset_params);
            this.Controls.Add(this.BUT_paramfileload);
            this.Controls.Add(this.CMB_paramfiles);
            this.Controls.Add(this.BUT_find);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BUT_compare);
            this.Controls.Add(this.BUT_rerequestparams);
            this.Controls.Add(this.BUT_writePIDS);
            this.Controls.Add(this.BUT_save);
            this.Controls.Add(this.BUT_load);
            this.Controls.Add(this.Params);
            this.Name = "ConfigRawParams";
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
        private System.Windows.Forms.DataGridView Params;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private Controls.MyButton BUT_find;
        private Controls.MyButton BUT_paramfileload;
        private System.Windows.Forms.ComboBox CMB_paramfiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn Units;
        private System.Windows.Forms.DataGridViewTextBoxColumn Options;
        private System.Windows.Forms.DataGridViewTextBoxColumn Desc;
        private Controls.MyButton BUT_reset_params;
    }
}
