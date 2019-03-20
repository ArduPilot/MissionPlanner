using MissionPlanner.Controls;

namespace MissionPlanner.Controls
{
    partial class UAVCANParams
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UAVCANParams));
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
            this.txt_search = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BUT_commitToFlash = new MissionPlanner.Controls.MyButton();
            this.chk_modified = new System.Windows.Forms.CheckBox();
            this.BUT_reset_params = new MissionPlanner.Controls.MyButton();
            this.Params = new MissionPlanner.Controls.MyDataGridView();
            this.Command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Min = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Max = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Default = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fav = new System.Windows.Forms.DataGridViewCheckBoxColumn();
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
            // BUT_reset_params
            // 
            resources.ApplyResources(this.BUT_reset_params, "BUT_reset_params");
            this.BUT_reset_params.Name = "BUT_reset_params";
            this.BUT_reset_params.UseVisualStyleBackColor = true;
            this.BUT_reset_params.Click += new System.EventHandler(this.BUT_reset_params_Click);
            // 
            // Params
            // 
            this.Params.AllowUserToAddRows = false;
            this.Params.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.Params, "Params");
            this.Params.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
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
            this.Min,
            this.Max,
            this.Default,
            this.Fav});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Params.DefaultCellStyle = dataGridViewCellStyle3;
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
            this.Params.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Params_CellContentClick);
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
            // Min
            // 
            resources.ApplyResources(this.Min, "Min");
            this.Min.Name = "Min";
            this.Min.ReadOnly = true;
            // 
            // Max
            // 
            resources.ApplyResources(this.Max, "Max");
            this.Max.Name = "Max";
            this.Max.ReadOnly = true;
            // 
            // Default
            // 
            this.Default.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Default.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.Default, "Default");
            this.Default.Name = "Default";
            this.Default.ReadOnly = true;
            // 
            // Fav
            // 
            this.Fav.FillWeight = 1F;
            resources.ApplyResources(this.Fav, "Fav");
            this.Fav.Name = "Fav";
            // 
            // UAVCANParams
            // 
            this.Controls.Add(this.chk_modified);
            this.Controls.Add(this.BUT_commitToFlash);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_search);
            this.Controls.Add(this.BUT_reset_params);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BUT_compare);
            this.Controls.Add(this.BUT_rerequestparams);
            this.Controls.Add(this.BUT_writePIDS);
            this.Controls.Add(this.BUT_save);
            this.Controls.Add(this.BUT_load);
            this.Controls.Add(this.Params);
            this.Name = "UAVCANParams";
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
        private Controls.MyDataGridView Params;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_search;
        private System.Windows.Forms.Label label2;
        private MyButton BUT_commitToFlash;
        private System.Windows.Forms.CheckBox chk_modified;
        private MyButton BUT_reset_params;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn Min;
        private System.Windows.Forms.DataGridViewTextBoxColumn Max;
        private System.Windows.Forms.DataGridViewTextBoxColumn Default;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Fav;
    }
}
