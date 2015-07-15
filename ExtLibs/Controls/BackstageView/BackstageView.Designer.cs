﻿namespace MissionPlanner.Controls.BackstageView
{
    partial class BackstageView
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
            this.pnlPages = new System.Windows.Forms.Panel();
            this.pnlMenu = new MissionPlanner.Controls.BackstageView.BackStageViewMenuPanel();
            this.SuspendLayout();
            // 
            // pnlPages
            // 
            this.pnlPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPages.AutoScroll = true;
            this.pnlPages.Location = new System.Drawing.Point(203, 0);
            this.pnlPages.Margin = new System.Windows.Forms.Padding(4);
            this.pnlPages.MinimumSize = new System.Drawing.Size(133, 0);
            this.pnlPages.Name = "pnlPages";
            this.pnlPages.Size = new System.Drawing.Size(448, 422);
            this.pnlPages.TabIndex = 0;
            // 
            // pnlMenu
            // 
            this.pnlMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMenu.Margin = new System.Windows.Forms.Padding(4);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(203, 422);
            this.pnlMenu.TabIndex = 1;
            // 
            // BackstageView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pnlMenu);
            this.Controls.Add(this.pnlPages);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "BackstageView";
            this.Size = new System.Drawing.Size(657, 422);
            this.Load += new System.EventHandler(this.BackstageView_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlPages;
        private BackStageViewMenuPanel pnlMenu;
    }
}
