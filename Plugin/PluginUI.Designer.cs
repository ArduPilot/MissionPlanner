﻿namespace MissionPlanner.Controls
{
    partial class PluginUI
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
            this.bSave = new MissionPlanner.Controls.MyButton();
            this.labelWarning = new System.Windows.Forms.Label();
            this.dgvPlugins = new MissionPlanner.Controls.MyDataGridView();
            this.pluginName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pluginAuthor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pluginVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pluginDll = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pluginEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlugins)).BeginInit();
            this.SuspendLayout();
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(11, 8);
            this.bSave.Margin = new System.Windows.Forms.Padding(2);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(73, 31);
            this.bSave.TabIndex = 1;
            this.bSave.Text = "Save && Close";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // labelWarning
            // 
            this.labelWarning.AutoSize = true;
            this.labelWarning.Location = new System.Drawing.Point(88, 8);
            this.labelWarning.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(224, 26);
            this.labelWarning.TabIndex = 4;
            this.labelWarning.Text = "Enable/Disable settings changed, till restart \rnot loaded but enabled plugins wil" +
    "l not shown!";
            this.labelWarning.Visible = false;
            // 
            // dgvPlugins
            // 
            this.dgvPlugins.AllowUserToAddRows = false;
            this.dgvPlugins.AllowUserToDeleteRows = false;
            this.dgvPlugins.AllowUserToResizeRows = false;
            this.dgvPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPlugins.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvPlugins.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPlugins.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.pluginName,
            this.pluginAuthor,
            this.pluginVersion,
            this.pluginDll,
            this.pluginEnabled});
            this.dgvPlugins.Location = new System.Drawing.Point(9, 51);
            this.dgvPlugins.Margin = new System.Windows.Forms.Padding(2);
            this.dgvPlugins.Name = "dgvPlugins";
            this.dgvPlugins.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dgvPlugins.RowTemplate.Height = 24;
            this.dgvPlugins.Size = new System.Drawing.Size(655, 303);
            this.dgvPlugins.TabIndex = 0;
            this.dgvPlugins.RowHeadersWidthChanged += new System.EventHandler(this.dgvPlugins_RowHeadersWidthChanged);
            this.dgvPlugins.SelectionChanged += new System.EventHandler(this.dgvPlugins_SelectionChanged);
            // 
            // pluginName
            // 
            this.pluginName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.pluginName.HeaderText = "Plugin Name";
            this.pluginName.MinimumWidth = 6;
            this.pluginName.Name = "pluginName";
            this.pluginName.ReadOnly = true;
            this.pluginName.Width = 92;
            // 
            // pluginAuthor
            // 
            this.pluginAuthor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.pluginAuthor.HeaderText = "Author";
            this.pluginAuthor.MinimumWidth = 6;
            this.pluginAuthor.Name = "pluginAuthor";
            this.pluginAuthor.ReadOnly = true;
            this.pluginAuthor.Width = 63;
            // 
            // pluginVersion
            // 
            this.pluginVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.pluginVersion.HeaderText = "Version";
            this.pluginVersion.MinimumWidth = 6;
            this.pluginVersion.Name = "pluginVersion";
            this.pluginVersion.ReadOnly = true;
            this.pluginVersion.Width = 67;
            // 
            // pluginDll
            // 
            this.pluginDll.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.pluginDll.HeaderText = "FileName";
            this.pluginDll.MinimumWidth = 6;
            this.pluginDll.Name = "pluginDll";
            this.pluginDll.ReadOnly = true;
            this.pluginDll.Width = 76;
            // 
            // pluginEnabled
            // 
            this.pluginEnabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.pluginEnabled.HeaderText = "Enabled";
            this.pluginEnabled.MinimumWidth = 6;
            this.pluginEnabled.Name = "pluginEnabled";
            this.pluginEnabled.Width = 52;
            // 
            // PluginUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 373);
            this.Controls.Add(this.labelWarning);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.dgvPlugins);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PluginUI";
            this.Text = "PluginManager";
            this.Shown += new System.EventHandler(this.PluginUI_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlugins)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyDataGridView dgvPlugins;
        private MyButton bSave;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.DataGridViewTextBoxColumn pluginName;
        private System.Windows.Forms.DataGridViewTextBoxColumn pluginAuthor;
        private System.Windows.Forms.DataGridViewTextBoxColumn pluginVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn pluginDll;
        private System.Windows.Forms.DataGridViewCheckBoxColumn pluginEnabled;
    }
}