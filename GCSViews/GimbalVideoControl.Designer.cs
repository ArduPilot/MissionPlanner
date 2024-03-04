namespace MissionPlanner
{
    partial class GimbalVideoControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.VideoBox = new System.Windows.Forms.PictureBox();
            this.VideoBoxContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.videoStreamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.retractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neutralToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.hideControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CameraLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.TakePictureButton = new MissionPlanner.Controls.MyButton();
            this.RecordVideoButton = new MissionPlanner.Controls.MyButton();
            this.ZoomTrackBar = new MissionPlanner.Controls.MyTrackBar();
            this.ZoomLabel = new System.Windows.Forms.Label();
            this.LockButton = new MissionPlanner.Controls.MyButton();
            this.ControlInfoTooltip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).BeginInit();
            this.VideoBoxContextMenu.SuspendLayout();
            this.MainLayoutPanel.SuspendLayout();
            this.CameraLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // VideoBox
            // 
            this.VideoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VideoBox.ContextMenuStrip = this.VideoBoxContextMenu;
            this.VideoBox.ErrorImage = global::MissionPlanner.Properties.Resources.no_video;
            this.VideoBox.Image = global::MissionPlanner.Properties.Resources.no_video;
            this.VideoBox.InitialImage = null;
            this.VideoBox.Location = new System.Drawing.Point(37, 0);
            this.VideoBox.Margin = new System.Windows.Forms.Padding(0);
            this.VideoBox.Name = "VideoBox";
            this.VideoBox.Size = new System.Drawing.Size(511, 382);
            this.VideoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.VideoBox.TabIndex = 0;
            this.VideoBox.TabStop = false;
            // 
            // VideoBoxContextMenu
            // 
            this.VideoBoxContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.videoStreamToolStripMenuItem,
            this.toolStripMenuItem1,
            this.retractToolStripMenuItem,
            this.neutralToolStripMenuItem,
            this.toolStripMenuItem2,
            this.hideControlsToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.VideoBoxContextMenu.Name = "VideoBoxContextMenu";
            this.VideoBoxContextMenu.Size = new System.Drawing.Size(148, 126);
            // 
            // videoStreamToolStripMenuItem
            // 
            this.videoStreamToolStripMenuItem.Name = "videoStreamToolStripMenuItem";
            this.videoStreamToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.videoStreamToolStripMenuItem.Text = "Video Stream";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(144, 6);
            // 
            // retractToolStripMenuItem
            // 
            this.retractToolStripMenuItem.Name = "retractToolStripMenuItem";
            this.retractToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.retractToolStripMenuItem.Text = "Retract";
            // 
            // neutralToolStripMenuItem
            // 
            this.neutralToolStripMenuItem.Name = "neutralToolStripMenuItem";
            this.neutralToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.neutralToolStripMenuItem.Text = "Neutral";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(144, 6);
            // 
            // hideControlsToolStripMenuItem
            // 
            this.hideControlsToolStripMenuItem.Name = "hideControlsToolStripMenuItem";
            this.hideControlsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.hideControlsToolStripMenuItem.Text = "Hide Controls";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // MainLayoutPanel
            // 
            this.MainLayoutPanel.ColumnCount = 2;
            this.MainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.MainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutPanel.Controls.Add(this.VideoBox, 1, 0);
            this.MainLayoutPanel.Controls.Add(this.CameraLayoutPanel, 0, 0);
            this.MainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainLayoutPanel.Margin = new System.Windows.Forms.Padding(1);
            this.MainLayoutPanel.Name = "MainLayoutPanel";
            this.MainLayoutPanel.RowCount = 1;
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutPanel.Size = new System.Drawing.Size(548, 382);
            this.MainLayoutPanel.TabIndex = 1;
            // 
            // CameraLayoutPanel
            // 
            this.CameraLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.CameraLayoutPanel.Controls.Add(this.TakePictureButton);
            this.CameraLayoutPanel.Controls.Add(this.RecordVideoButton);
            this.CameraLayoutPanel.Controls.Add(this.ZoomTrackBar);
            this.CameraLayoutPanel.Controls.Add(this.ZoomLabel);
            this.CameraLayoutPanel.Controls.Add(this.LockButton);
            this.CameraLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.CameraLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.CameraLayoutPanel.Name = "CameraLayoutPanel";
            this.CameraLayoutPanel.Size = new System.Drawing.Size(37, 382);
            this.CameraLayoutPanel.TabIndex = 1;
            // 
            // TakePictureButton
            // 
            this.TakePictureButton.Location = new System.Drawing.Point(2, 2);
            this.TakePictureButton.Margin = new System.Windows.Forms.Padding(2);
            this.TakePictureButton.Name = "TakePictureButton";
            this.TakePictureButton.Size = new System.Drawing.Size(33, 33);
            this.TakePictureButton.TabIndex = 0;
            this.TakePictureButton.Text = "Pic";
            this.TakePictureButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.ControlInfoTooltip.SetToolTip(this.TakePictureButton, "Take a picture");
            this.TakePictureButton.UseVisualStyleBackColor = true;
            // 
            // RecordVideoButton
            // 
            this.RecordVideoButton.Location = new System.Drawing.Point(2, 39);
            this.RecordVideoButton.Margin = new System.Windows.Forms.Padding(2);
            this.RecordVideoButton.Name = "RecordVideoButton";
            this.RecordVideoButton.Size = new System.Drawing.Size(33, 33);
            this.RecordVideoButton.TabIndex = 1;
            this.RecordVideoButton.Text = "Rec";
            this.RecordVideoButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.ControlInfoTooltip.SetToolTip(this.RecordVideoButton, "Start/stop recording");
            this.RecordVideoButton.UseVisualStyleBackColor = true;
            // 
            // ZoomTrackBar
            // 
            this.ZoomTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomTrackBar.AutoSize = false;
            this.ZoomTrackBar.LargeChange = 0.005F;
            this.ZoomTrackBar.Location = new System.Drawing.Point(2, 76);
            this.ZoomTrackBar.Margin = new System.Windows.Forms.Padding(2);
            this.ZoomTrackBar.Maximum = 0.01F;
            this.ZoomTrackBar.Minimum = 0F;
            this.ZoomTrackBar.Name = "ZoomTrackBar";
            this.ZoomTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.ZoomTrackBar.Size = new System.Drawing.Size(33, 81);
            this.ZoomTrackBar.SmallChange = 0.001F;
            this.ZoomTrackBar.TabIndex = 4;
            this.ZoomTrackBar.TickFrequency = 0.001F;
            this.ZoomTrackBar.Value = 0F;
            // 
            // ZoomLabel
            // 
            this.ZoomLabel.AutoSize = true;
            this.ZoomLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.ZoomLabel.Location = new System.Drawing.Point(1, 159);
            this.ZoomLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.ZoomLabel.Name = "ZoomLabel";
            this.ZoomLabel.Size = new System.Drawing.Size(33, 13);
            this.ZoomLabel.TabIndex = 5;
            this.ZoomLabel.Text = "Zoom";
            // 
            // LockButton
            // 
            this.LockButton.Location = new System.Drawing.Point(2, 174);
            this.LockButton.Margin = new System.Windows.Forms.Padding(2);
            this.LockButton.Name = "LockButton";
            this.LockButton.Size = new System.Drawing.Size(33, 33);
            this.LockButton.TabIndex = 8;
            this.LockButton.Text = "Loc";
            this.LockButton.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.ControlInfoTooltip.SetToolTip(this.LockButton, "Yaw frame lock/follow");
            this.LockButton.UseVisualStyleBackColor = true;
            // 
            // GimbalVideoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MainLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "GimbalVideoControl";
            this.Size = new System.Drawing.Size(548, 382);
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).EndInit();
            this.VideoBoxContextMenu.ResumeLayout(false);
            this.MainLayoutPanel.ResumeLayout(false);
            this.CameraLayoutPanel.ResumeLayout(false);
            this.CameraLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomTrackBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox VideoBox;
        private System.Windows.Forms.TableLayoutPanel MainLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel CameraLayoutPanel;
        private Controls.MyButton TakePictureButton;
        private Controls.MyButton RecordVideoButton;
        private Controls.MyTrackBar ZoomTrackBar;
        private System.Windows.Forms.Label ZoomLabel;
        private Controls.MyButton LockButton;
        private System.Windows.Forms.ToolTip ControlInfoTooltip;
        private System.Windows.Forms.ContextMenuStrip VideoBoxContextMenu;
        private System.Windows.Forms.ToolStripMenuItem videoStreamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem retractToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neutralToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem hideControlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
    }
}
