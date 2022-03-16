namespace MissionPlanner.Controls
{
    partial class ThemeEditor
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
            this.listboxThemeItems = new System.Windows.Forms.ListBox();
            this.lblItemName = new System.Windows.Forms.Label();
            this.lblColorName = new System.Windows.Forms.Label();
            this.colorPatch = new System.Windows.Forms.Panel();
            this.btnCopy = new MissionPlanner.Controls.MyButton();
            this.btnPreview = new MissionPlanner.Controls.MyButton();
            this.colorSelectDialog = new System.Windows.Forms.ColorDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.lblThemeName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new MissionPlanner.Controls.MyButton();
            this.btnSaveApply = new MissionPlanner.Controls.MyButton();
            this.btnRestore = new MissionPlanner.Controls.MyButton();
            this.cbIconSet = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listboxThemeItems
            // 
            this.listboxThemeItems.FormattingEnabled = true;
            this.listboxThemeItems.Location = new System.Drawing.Point(16, 20);
            this.listboxThemeItems.Margin = new System.Windows.Forms.Padding(2);
            this.listboxThemeItems.Name = "listboxThemeItems";
            this.listboxThemeItems.Size = new System.Drawing.Size(134, 368);
            this.listboxThemeItems.TabIndex = 1;
            this.listboxThemeItems.SelectedIndexChanged += new System.EventHandler(this.listboxThemeItems_SelectedIndexChanged);
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Location = new System.Drawing.Point(6, 16);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(55, 13);
            this.lblItemName.TabIndex = 3;
            this.lblItemName.Text = "ItemName";
            // 
            // lblColorName
            // 
            this.lblColorName.AutoSize = true;
            this.lblColorName.Location = new System.Drawing.Point(6, 42);
            this.lblColorName.Name = "lblColorName";
            this.lblColorName.Size = new System.Drawing.Size(59, 13);
            this.lblColorName.TabIndex = 5;
            this.lblColorName.Text = "ColorName";
            // 
            // colorPatch
            // 
            this.colorPatch.BackColor = System.Drawing.Color.Red;
            this.colorPatch.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorPatch.Location = new System.Drawing.Point(50, 75);
            this.colorPatch.Name = "colorPatch";
            this.colorPatch.Size = new System.Drawing.Size(117, 96);
            this.colorPatch.TabIndex = 7;
            this.colorPatch.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.colorPatch_dblclick);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(168, 291);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(92, 23);
            this.btnCopy.TabIndex = 6;
            this.btnCopy.Text = "Create Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(285, 291);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(92, 23);
            this.btnPreview.TabIndex = 8;
            this.btnPreview.Text = "Preview Colors";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // colorSelectDialog
            // 
            this.colorSelectDialog.AnyColor = true;
            this.colorSelectDialog.ShowHelp = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(167, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Theme Name:";
            // 
            // lblThemeName
            // 
            this.lblThemeName.AutoSize = true;
            this.lblThemeName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThemeName.Location = new System.Drawing.Point(241, 20);
            this.lblThemeName.Name = "lblThemeName";
            this.lblThemeName.Size = new System.Drawing.Size(0, 13);
            this.lblThemeName.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.colorPatch);
            this.groupBox1.Controls.Add(this.lblColorName);
            this.groupBox1.Controls.Add(this.lblItemName);
            this.groupBox1.Location = new System.Drawing.Point(168, 75);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 201);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selected Item";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(285, 365);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSaveApply
            // 
            this.btnSaveApply.Location = new System.Drawing.Point(168, 365);
            this.btnSaveApply.Name = "btnSaveApply";
            this.btnSaveApply.Size = new System.Drawing.Size(92, 23);
            this.btnSaveApply.TabIndex = 13;
            this.btnSaveApply.Text = "Save && Apply";
            this.btnSaveApply.UseVisualStyleBackColor = true;
            this.btnSaveApply.Click += new System.EventHandler(this.btnSaveApply_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(168, 331);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(92, 23);
            this.btnRestore.TabIndex = 14;
            this.btnRestore.Text = "Restore Theme";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // cbIconSet
            // 
            this.cbIconSet.AutoSize = true;
            this.cbIconSet.Location = new System.Drawing.Point(168, 52);
            this.cbIconSet.Name = "cbIconSet";
            this.cbIconSet.Size = new System.Drawing.Size(103, 17);
            this.cbIconSet.TabIndex = 15;
            this.cbIconSet.Text = "Dark menuicons";
            this.cbIconSet.UseVisualStyleBackColor = true;
            this.cbIconSet.CheckedChanged += new System.EventHandler(this.cbIconSet_CheckedChanged);
            // 
            // ThemeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 412);
            this.Controls.Add(this.cbIconSet);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnSaveApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblThemeName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.listboxThemeItems);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ThemeEditor";
            this.Text = "ThemeEditor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox listboxThemeItems;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label lblColorName;
        private MyButton btnCopy;
        private System.Windows.Forms.Panel colorPatch;
        private MyButton btnPreview;
        private System.Windows.Forms.ColorDialog colorSelectDialog;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblThemeName;
        private System.Windows.Forms.GroupBox groupBox1;
        private MyButton btnCancel;
        private MyButton btnSaveApply;
        private MyButton btnRestore;
        private System.Windows.Forms.CheckBox cbIconSet;
    }
}