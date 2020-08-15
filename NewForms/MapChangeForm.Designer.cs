namespace MissionPlanner.NewForms
{
    partial class MapChangeForm
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
            this.panel3 = new System.Windows.Forms.Panel();
            this.chk_grid = new System.Windows.Forms.CheckBox();
            this.lbl_status = new System.Windows.Forms.Label();
            this.comboBoxMapType = new System.Windows.Forms.ComboBox();
            this.lnk_kml = new System.Windows.Forms.LinkLabel();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.chk_grid);
            this.panel3.Controls.Add(this.lbl_status);
            this.panel3.Controls.Add(this.comboBoxMapType);
            this.panel3.Controls.Add(this.lnk_kml);
            this.panel3.Location = new System.Drawing.Point(12, 28);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(214, 79);
            this.panel3.TabIndex = 54;
            // 
            // chk_grid
            // 
            this.chk_grid.AutoSize = true;
            this.chk_grid.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_grid.Location = new System.Drawing.Point(3, 3);
            this.chk_grid.Name = "chk_grid";
            this.chk_grid.Size = new System.Drawing.Size(45, 17);
            this.chk_grid.TabIndex = 44;
            this.chk_grid.Text = "Grid";
            this.chk_grid.UseVisualStyleBackColor = true;
            // 
            // lbl_status
            // 
            this.lbl_status.AutoSize = true;
            this.lbl_status.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_status.Location = new System.Drawing.Point(4, 46);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(37, 13);
            this.lbl_status.TabIndex = 43;
            this.lbl_status.Text = "Status";
            // 
            // comboBoxMapType
            // 
            this.comboBoxMapType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMapType.FormattingEnabled = true;
            this.comboBoxMapType.Location = new System.Drawing.Point(3, 22);
            this.comboBoxMapType.Name = "comboBoxMapType";
            this.comboBoxMapType.Size = new System.Drawing.Size(208, 21);
            this.comboBoxMapType.TabIndex = 42;
            // 
            // lnk_kml
            // 
            this.lnk_kml.AutoSize = true;
            this.lnk_kml.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lnk_kml.Location = new System.Drawing.Point(156, 7);
            this.lnk_kml.Name = "lnk_kml";
            this.lnk_kml.Size = new System.Drawing.Size(55, 13);
            this.lnk_kml.TabIndex = 45;
            this.lnk_kml.TabStop = true;
            this.lnk_kml.Text = "View KML";
            // 
            // MapChangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(238, 123);
            this.Controls.Add(this.panel3);
            this.Name = "MapChangeForm";
            this.Text = "MapChangeForm";
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.CheckBox chk_grid;
        public System.Windows.Forms.Label lbl_status;
        public System.Windows.Forms.ComboBox comboBoxMapType;
        public System.Windows.Forms.LinkLabel lnk_kml;
    }
}