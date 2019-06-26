namespace MissionPlanner.Controls
{
    partial class RelayOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RelayOptions));
            this.BUT_Low = new MissionPlanner.Controls.MyButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BUT_High = new MissionPlanner.Controls.MyButton();
            this.BUT_Repeat = new MissionPlanner.Controls.MyButton();
            this.TXT_rcchannel = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BUT_Low
            // 
            this.BUT_Low.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.BUT_Low, "BUT_Low");
            this.BUT_Low.Name = "BUT_Low";
            this.BUT_Low.UseVisualStyleBackColor = true;
            this.BUT_Low.Click += new System.EventHandler(this.BUT_Low_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            resources.ApplyResources(this.renameToolStripMenuItem, "renameToolStripMenuItem");
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // BUT_High
            // 
            this.BUT_High.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.BUT_High, "BUT_High");
            this.BUT_High.Name = "BUT_High";
            this.BUT_High.UseVisualStyleBackColor = true;
            this.BUT_High.Click += new System.EventHandler(this.BUT_High_Click);
            // 
            // BUT_Repeat
            // 
            resources.ApplyResources(this.BUT_Repeat, "BUT_Repeat");
            this.BUT_Repeat.Name = "BUT_Repeat";
            this.BUT_Repeat.UseVisualStyleBackColor = true;
            this.BUT_Repeat.Click += new System.EventHandler(this.BUT_Repeat_Click);
            // 
            // TXT_rcchannel
            // 
            this.TXT_rcchannel.BackColor = System.Drawing.Color.Red;
            this.TXT_rcchannel.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.TXT_rcchannel, "TXT_rcchannel");
            this.TXT_rcchannel.Name = "TXT_rcchannel";
            // 
            // RelayOptions
            // 
            this.Controls.Add(this.TXT_rcchannel);
            this.Controls.Add(this.BUT_Repeat);
            this.Controls.Add(this.BUT_High);
            this.Controls.Add(this.BUT_Low);
            this.Name = "RelayOptions";
            resources.ApplyResources(this, "$this");
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MyButton BUT_Low;
        private MyButton BUT_High;
        private MyButton BUT_Repeat;
        private System.Windows.Forms.Label TXT_rcchannel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
    }
}
