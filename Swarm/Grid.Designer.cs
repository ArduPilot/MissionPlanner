namespace MissionPlanner.Swarm
{
    partial class Grid
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeAltToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CHK_vertical = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeAltToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 48);
            // 
            // changeAltToolStripMenuItem
            // 
            this.changeAltToolStripMenuItem.Name = "changeAltToolStripMenuItem";
            this.changeAltToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.changeAltToolStripMenuItem.Text = "Change Alt";
            this.changeAltToolStripMenuItem.Click += new System.EventHandler(this.changeAltToolStripMenuItem_Click);
            // 
            // CHK_vertical
            // 
            this.CHK_vertical.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CHK_vertical.AutoSize = true;
            this.CHK_vertical.Location = new System.Drawing.Point(350, 312);
            this.CHK_vertical.Name = "CHK_vertical";
            this.CHK_vertical.Size = new System.Drawing.Size(61, 17);
            this.CHK_vertical.TabIndex = 1;
            this.CHK_vertical.Text = "Vertical";
            this.CHK_vertical.UseVisualStyleBackColor = true;
            this.CHK_vertical.CheckedChanged += new System.EventHandler(this.CHK_vertical_CheckedChanged);
            // 
            // Grid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.CHK_vertical);
            this.DoubleBuffered = true;
            this.Name = "Grid";
            this.Size = new System.Drawing.Size(414, 329);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem changeAltToolStripMenuItem;
        private System.Windows.Forms.CheckBox CHK_vertical;
    }
}
