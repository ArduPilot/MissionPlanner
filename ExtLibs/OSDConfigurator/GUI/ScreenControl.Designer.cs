namespace OSDConfigurator.GUI
{
    partial class ScreenControl
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
            this.panelItemList = new System.Windows.Forms.FlowLayoutPanel();
            this.groupScreenOptions = new System.Windows.Forms.GroupBox();
            this.tableRoot = new System.Windows.Forms.TableLayoutPanel();
            this.groupOptions = new System.Windows.Forms.GroupBox();
            this.tableRight = new System.Windows.Forms.TableLayoutPanel();
            this.tableLeft = new System.Windows.Forms.TableLayoutPanel();
            this.tableRoot.SuspendLayout();
            this.tableRight.SuspendLayout();
            this.tableLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelItemList
            // 
            this.panelItemList.AutoScroll = true;
            this.panelItemList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelItemList.Location = new System.Drawing.Point(110, 484);
            this.panelItemList.Name = "panelItemList";
            this.panelItemList.Size = new System.Drawing.Size(696, 110);
            this.panelItemList.TabIndex = 1;
            // 
            // groupScreenOptions
            // 
            this.groupScreenOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupScreenOptions.Location = new System.Drawing.Point(3, 3);
            this.groupScreenOptions.Name = "groupScreenOptions";
            this.groupScreenOptions.Size = new System.Drawing.Size(404, 364);
            this.groupScreenOptions.TabIndex = 1;
            this.groupScreenOptions.TabStop = false;
            this.groupScreenOptions.Text = "Screen options";
            // 
            // tableRoot
            // 
            this.tableRoot.ColumnCount = 2;
            this.tableRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableRoot.Controls.Add(this.tableRight, 1, 0);
            this.tableRoot.Controls.Add(this.tableLeft, 0, 0);
            this.tableRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableRoot.Location = new System.Drawing.Point(0, 0);
            this.tableRoot.Name = "tableRoot";
            this.tableRoot.RowCount = 1;
            this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableRoot.Size = new System.Drawing.Size(1124, 746);
            this.tableRoot.TabIndex = 5;
            // 
            // groupOptions
            // 
            this.groupOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupOptions.Location = new System.Drawing.Point(3, 373);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(404, 364);
            this.groupOptions.TabIndex = 0;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Item Options";
            // 
            // tableRight
            // 
            this.tableRight.ColumnCount = 1;
            this.tableRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableRight.Controls.Add(this.groupOptions, 0, 1);
            this.tableRight.Controls.Add(this.groupScreenOptions, 0, 0);
            this.tableRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableRight.Location = new System.Drawing.Point(711, 3);
            this.tableRight.Name = "tableRight";
            this.tableRight.RowCount = 2;
            this.tableRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableRight.Size = new System.Drawing.Size(410, 740);
            this.tableRight.TabIndex = 0;
            // 
            // tableLeft
            // 
            this.tableLeft.AutoSize = true;
            this.tableLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLeft.ColumnCount = 1;
            this.tableLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLeft.Controls.Add(this.panelItemList, 0, 1);
            this.tableLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLeft.Location = new System.Drawing.Point(3, 3);
            this.tableLeft.Name = "tableLeft";
            this.tableLeft.RowCount = 2;
            this.tableLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLeft.Size = new System.Drawing.Size(702, 740);
            this.tableLeft.TabIndex = 1;
            // 
            // ScreenControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableRoot);
            this.MinimumSize = new System.Drawing.Size(1077, 685);
            this.Name = "ScreenControl";
            this.Size = new System.Drawing.Size(1124, 746);
            this.tableRoot.ResumeLayout(false);
            this.tableRoot.PerformLayout();
            this.tableRight.ResumeLayout(false);
            this.tableLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel panelItemList;
        private System.Windows.Forms.GroupBox groupScreenOptions;
        private System.Windows.Forms.TableLayoutPanel tableRoot;
        private System.Windows.Forms.TableLayoutPanel tableRight;
        private System.Windows.Forms.GroupBox groupOptions;
        private System.Windows.Forms.TableLayoutPanel tableLeft;
    }
}
