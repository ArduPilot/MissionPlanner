namespace MissionPlanner.Controls
{
    partial class MissionStyleEditor
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
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.stylePresetPanel = new System.Windows.Forms.TableLayoutPanel();
            this.previewButton = new MissionPlanner.Controls.MyButton();
            this.styleLabel = new System.Windows.Forms.Label();
            this.saveAsButton = new MissionPlanner.Controls.MyButton();
            this.styleBox = new System.Windows.Forms.ComboBox();
            this.saveButton = new MissionPlanner.Controls.MyButton();
            this.editorSplit = new System.Windows.Forms.SplitContainer();
            this.ruleEditorPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ruleTabs = new System.Windows.Forms.TabControl();
            this.markerRuleTab = new System.Windows.Forms.TabPage();
            this.markerRuleListBox = new System.Windows.Forms.ListBox();
            this.segmentRuleTab = new System.Windows.Forms.TabPage();
            this.segmentRuleListBox = new System.Windows.Forms.ListBox();
            this.ruleAddButton = new MissionPlanner.Controls.MyButton();
            this.ruleDuplicateButton = new MissionPlanner.Controls.MyButton();
            this.ruleDeleteButton = new MissionPlanner.Controls.MyButton();
            this.ruleMoveUpButton = new MissionPlanner.Controls.MyButton();
            this.ruleMoveDownButton = new MissionPlanner.Controls.MyButton();
            this.rulePropertyEditor = new System.Windows.Forms.PropertyGrid();
            this.dialogButtons = new System.Windows.Forms.TableLayoutPanel();
            this.okButton = new MissionPlanner.Controls.MyButton();
            this.cancelButton = new MissionPlanner.Controls.MyButton();
            this.mainPanel.SuspendLayout();
            this.stylePresetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorSplit)).BeginInit();
            this.editorSplit.Panel1.SuspendLayout();
            this.editorSplit.Panel2.SuspendLayout();
            this.editorSplit.SuspendLayout();
            this.ruleEditorPanel.SuspendLayout();
            this.ruleTabs.SuspendLayout();
            this.markerRuleTab.SuspendLayout();
            this.segmentRuleTab.SuspendLayout();
            this.dialogButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Controls.Add(this.stylePresetPanel, 0, 0);
            this.mainPanel.Controls.Add(this.editorSplit, 0, 1);
            this.mainPanel.Controls.Add(this.dialogButtons, 0, 2);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 3;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.Size = new System.Drawing.Size(800, 450);
            this.mainPanel.TabIndex = 0;
            // 
            // stylePresetPanel
            // 
            this.stylePresetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stylePresetPanel.AutoSize = true;
            this.stylePresetPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stylePresetPanel.ColumnCount = 5;
            this.stylePresetPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.stylePresetPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.stylePresetPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.stylePresetPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.stylePresetPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.stylePresetPanel.Controls.Add(this.previewButton, 4, 0);
            this.stylePresetPanel.Controls.Add(this.styleLabel, 0, 0);
            this.stylePresetPanel.Controls.Add(this.saveAsButton, 3, 0);
            this.stylePresetPanel.Controls.Add(this.styleBox, 1, 0);
            this.stylePresetPanel.Controls.Add(this.saveButton, 2, 0);
            this.stylePresetPanel.Location = new System.Drawing.Point(3, 3);
            this.stylePresetPanel.Name = "stylePresetPanel";
            this.stylePresetPanel.RowCount = 1;
            this.stylePresetPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.stylePresetPanel.Size = new System.Drawing.Size(794, 29);
            this.stylePresetPanel.TabIndex = 1;
            // 
            // previewButton
            // 
            this.previewButton.Location = new System.Drawing.Point(726, 3);
            this.previewButton.Name = "previewButton";
            this.previewButton.Size = new System.Drawing.Size(65, 23);
            this.previewButton.TabIndex = 2;
            this.previewButton.Text = "Preview";
            this.previewButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.previewButton.UseVisualStyleBackColor = true;
            this.previewButton.Click += new System.EventHandler(this.previewButton_Click);
            // 
            // styleLabel
            // 
            this.styleLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.styleLabel.AutoSize = true;
            this.styleLabel.Location = new System.Drawing.Point(3, 8);
            this.styleLabel.Name = "styleLabel";
            this.styleLabel.Size = new System.Drawing.Size(36, 13);
            this.styleLabel.TabIndex = 0;
            this.styleLabel.Text = "Style: ";
            // 
            // saveAsButton
            // 
            this.saveAsButton.Location = new System.Drawing.Point(273, 3);
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.Size = new System.Drawing.Size(65, 23);
            this.saveAsButton.TabIndex = 1;
            this.saveAsButton.Text = "Save As";
            this.saveAsButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.saveAsButton.UseVisualStyleBackColor = true;
            this.saveAsButton.Click += new System.EventHandler(this.saveAsButton_Click);
            // 
            // styleBox
            // 
            this.styleBox.FormattingEnabled = true;
            this.styleBox.Location = new System.Drawing.Point(45, 3);
            this.styleBox.Name = "styleBox";
            this.styleBox.Size = new System.Drawing.Size(151, 21);
            this.styleBox.TabIndex = 1;
            this.styleBox.SelectedIndexChanged += new System.EventHandler(this.styleBox_SelectedIndexChanged);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(202, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(65, 23);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save";
            this.saveButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // editorSplit
            // 
            this.editorSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorSplit.Location = new System.Drawing.Point(3, 38);
            this.editorSplit.Name = "editorSplit";
            // 
            // editorSplit.Panel1
            // 
            this.editorSplit.Panel1.Controls.Add(this.ruleEditorPanel);
            // 
            // editorSplit.Panel2
            // 
            this.editorSplit.Panel2.Controls.Add(this.rulePropertyEditor);
            this.editorSplit.Size = new System.Drawing.Size(794, 374);
            this.editorSplit.SplitterDistance = 258;
            this.editorSplit.TabIndex = 0;
            // 
            // ruleEditorPanel
            // 
            this.ruleEditorPanel.ColumnCount = 4;
            this.ruleEditorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.99999F));
            this.ruleEditorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ruleEditorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ruleEditorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ruleEditorPanel.Controls.Add(this.ruleTabs, 0, 0);
            this.ruleEditorPanel.Controls.Add(this.ruleAddButton, 0, 2);
            this.ruleEditorPanel.Controls.Add(this.ruleDuplicateButton, 1, 2);
            this.ruleEditorPanel.Controls.Add(this.ruleDeleteButton, 2, 2);
            this.ruleEditorPanel.Controls.Add(this.ruleMoveUpButton, 3, 0);
            this.ruleEditorPanel.Controls.Add(this.ruleMoveDownButton, 3, 1);
            this.ruleEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ruleEditorPanel.Location = new System.Drawing.Point(0, 0);
            this.ruleEditorPanel.Name = "ruleEditorPanel";
            this.ruleEditorPanel.RowCount = 3;
            this.ruleEditorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.ruleEditorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ruleEditorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ruleEditorPanel.Size = new System.Drawing.Size(258, 374);
            this.ruleEditorPanel.TabIndex = 0;
            // 
            // ruleTabs
            // 
            this.ruleEditorPanel.SetColumnSpan(this.ruleTabs, 3);
            this.ruleTabs.Controls.Add(this.markerRuleTab);
            this.ruleTabs.Controls.Add(this.segmentRuleTab);
            this.ruleTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ruleTabs.Location = new System.Drawing.Point(3, 3);
            this.ruleTabs.Name = "ruleTabs";
            this.ruleEditorPanel.SetRowSpan(this.ruleTabs, 2);
            this.ruleTabs.SelectedIndex = 0;
            this.ruleTabs.Size = new System.Drawing.Size(220, 339);
            this.ruleTabs.TabIndex = 0;
            // 
            // markerRuleTab
            // 
            this.markerRuleTab.Controls.Add(this.markerRuleListBox);
            this.markerRuleTab.Location = new System.Drawing.Point(4, 22);
            this.markerRuleTab.Name = "markerRuleTab";
            this.markerRuleTab.Padding = new System.Windows.Forms.Padding(3);
            this.markerRuleTab.Size = new System.Drawing.Size(212, 313);
            this.markerRuleTab.TabIndex = 0;
            this.markerRuleTab.Text = "Markers";
            this.markerRuleTab.UseVisualStyleBackColor = true;
            // 
            // markerRuleListBox
            // 
            this.markerRuleListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.markerRuleListBox.FormattingEnabled = true;
            this.markerRuleListBox.Location = new System.Drawing.Point(3, 3);
            this.markerRuleListBox.Name = "markerRuleListBox";
            this.markerRuleListBox.Size = new System.Drawing.Size(206, 307);
            this.markerRuleListBox.TabIndex = 0;
            this.markerRuleListBox.SelectedIndexChanged += new System.EventHandler(this.ruleListBox_SelectedIndexChanged);
            // 
            // segmentRuleTab
            // 
            this.segmentRuleTab.Controls.Add(this.segmentRuleListBox);
            this.segmentRuleTab.Location = new System.Drawing.Point(4, 22);
            this.segmentRuleTab.Name = "segmentRuleTab";
            this.segmentRuleTab.Padding = new System.Windows.Forms.Padding(3);
            this.segmentRuleTab.Size = new System.Drawing.Size(212, 313);
            this.segmentRuleTab.TabIndex = 1;
            this.segmentRuleTab.Text = "Segments";
            this.segmentRuleTab.UseVisualStyleBackColor = true;
            // 
            // segmentRuleListBox
            // 
            this.segmentRuleListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.segmentRuleListBox.FormattingEnabled = true;
            this.segmentRuleListBox.Location = new System.Drawing.Point(3, 3);
            this.segmentRuleListBox.Name = "segmentRuleListBox";
            this.segmentRuleListBox.Size = new System.Drawing.Size(206, 307);
            this.segmentRuleListBox.TabIndex = 0;
            this.segmentRuleListBox.SelectedIndexChanged += new System.EventHandler(this.ruleListBox_SelectedIndexChanged);
            // 
            // ruleAddButton
            // 
            this.ruleAddButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ruleAddButton.Location = new System.Drawing.Point(9, 348);
            this.ruleAddButton.Name = "ruleAddButton";
            this.ruleAddButton.Size = new System.Drawing.Size(65, 23);
            this.ruleAddButton.TabIndex = 1;
            this.ruleAddButton.Text = "Add";
            this.ruleAddButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.ruleAddButton.UseVisualStyleBackColor = true;
            this.ruleAddButton.Click += new System.EventHandler(this.ruleAddButton_Click);
            // 
            // ruleDuplicateButton
            // 
            this.ruleDuplicateButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ruleDuplicateButton.Location = new System.Drawing.Point(80, 348);
            this.ruleDuplicateButton.Name = "ruleDuplicateButton";
            this.ruleDuplicateButton.Size = new System.Drawing.Size(65, 23);
            this.ruleDuplicateButton.TabIndex = 2;
            this.ruleDuplicateButton.Text = "Duplicate";
            this.ruleDuplicateButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.ruleDuplicateButton.UseVisualStyleBackColor = true;
            this.ruleDuplicateButton.Click += new System.EventHandler(this.ruleDuplicateButton_Click);
            // 
            // ruleDeleteButton
            // 
            this.ruleDeleteButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ruleDeleteButton.Location = new System.Drawing.Point(151, 348);
            this.ruleDeleteButton.Name = "ruleDeleteButton";
            this.ruleDeleteButton.Size = new System.Drawing.Size(65, 23);
            this.ruleDeleteButton.TabIndex = 3;
            this.ruleDeleteButton.Text = "Delete";
            this.ruleDeleteButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.ruleDeleteButton.UseVisualStyleBackColor = true;
            this.ruleDeleteButton.Click += new System.EventHandler(this.ruleDeleteButton_Click);
            // 
            // ruleMoveUpButton
            // 
            this.ruleMoveUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ruleMoveUpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ruleMoveUpButton.Location = new System.Drawing.Point(229, 32);
            this.ruleMoveUpButton.Name = "ruleMoveUpButton";
            this.ruleMoveUpButton.Size = new System.Drawing.Size(25, 25);
            this.ruleMoveUpButton.TabIndex = 4;
            this.ruleMoveUpButton.Text = "⬆";
            this.ruleMoveUpButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.ruleMoveUpButton.UseVisualStyleBackColor = true;
            this.ruleMoveUpButton.Click += new System.EventHandler(this.ruleMoveUpButton_Click);
            // 
            // ruleMoveDownButton
            // 
            this.ruleMoveDownButton.Location = new System.Drawing.Point(229, 63);
            this.ruleMoveDownButton.Name = "ruleMoveDownButton";
            this.ruleMoveDownButton.Size = new System.Drawing.Size(25, 25);
            this.ruleMoveDownButton.TabIndex = 5;
            this.ruleMoveDownButton.Text = "⬇";
            this.ruleMoveDownButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.ruleMoveDownButton.UseVisualStyleBackColor = true;
            this.ruleMoveDownButton.Click += new System.EventHandler(this.ruleMoveDownButton_Click);
            // 
            // rulePropertyEditor
            // 
            this.rulePropertyEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rulePropertyEditor.Location = new System.Drawing.Point(0, 0);
            this.rulePropertyEditor.Name = "rulePropertyEditor";
            this.rulePropertyEditor.Size = new System.Drawing.Size(532, 374);
            this.rulePropertyEditor.TabIndex = 0;
            this.rulePropertyEditor.ToolbarVisible = false;
            // 
            // dialogButtons
            // 
            this.dialogButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dialogButtons.AutoSize = true;
            this.dialogButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dialogButtons.ColumnCount = 3;
            this.dialogButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dialogButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.dialogButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.dialogButtons.Controls.Add(this.okButton, 1, 0);
            this.dialogButtons.Controls.Add(this.cancelButton, 2, 0);
            this.dialogButtons.Location = new System.Drawing.Point(3, 418);
            this.dialogButtons.Name = "dialogButtons";
            this.dialogButtons.RowCount = 1;
            this.dialogButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dialogButtons.Size = new System.Drawing.Size(794, 29);
            this.dialogButtons.TabIndex = 2;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(655, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(65, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(726, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(65, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // MissionStyleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainPanel);
            this.Name = "MissionStyleEditor";
            this.Text = "Mission Style Editor";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.stylePresetPanel.ResumeLayout(false);
            this.stylePresetPanel.PerformLayout();
            this.editorSplit.Panel1.ResumeLayout(false);
            this.editorSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.editorSplit)).EndInit();
            this.editorSplit.ResumeLayout(false);
            this.ruleEditorPanel.ResumeLayout(false);
            this.ruleTabs.ResumeLayout(false);
            this.markerRuleTab.ResumeLayout(false);
            this.segmentRuleTab.ResumeLayout(false);
            this.dialogButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.SplitContainer editorSplit;
        private System.Windows.Forms.PropertyGrid rulePropertyEditor;
        private System.Windows.Forms.TableLayoutPanel stylePresetPanel;
        private System.Windows.Forms.ComboBox styleBox;
        private MyButton previewButton;
        private MyButton saveAsButton;
        private MyButton saveButton;
        private System.Windows.Forms.TableLayoutPanel dialogButtons;
        private MyButton okButton;
        private MyButton cancelButton;
        private System.Windows.Forms.TableLayoutPanel ruleEditorPanel;
        private System.Windows.Forms.TabControl ruleTabs;
        private System.Windows.Forms.TabPage markerRuleTab;
        private System.Windows.Forms.TabPage segmentRuleTab;
        private MyButton ruleAddButton;
        private MyButton ruleDuplicateButton;
        private MyButton ruleDeleteButton;
        private MyButton ruleMoveUpButton;
        private MyButton ruleMoveDownButton;
        private System.Windows.Forms.ListBox markerRuleListBox;
        private System.Windows.Forms.ListBox segmentRuleListBox;
        private System.Windows.Forms.Label styleLabel;
    }
}