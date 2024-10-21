namespace MissionPlanner.Controls
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
            this.pointDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pointHomeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yawLockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.takePictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startRecordingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopRecordingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ControlInfoTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.UITimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).BeginInit();
            this.VideoBoxContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // VideoBox
            // 
            this.VideoBox.ContextMenuStrip = this.VideoBoxContextMenu;
            this.VideoBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VideoBox.ErrorImage = global::MissionPlanner.Properties.Resources.no_video;
            this.VideoBox.Image = global::MissionPlanner.Properties.Resources.no_video;
            this.VideoBox.InitialImage = null;
            this.VideoBox.Location = new System.Drawing.Point(0, 0);
            this.VideoBox.Margin = new System.Windows.Forms.Padding(0);
            this.VideoBox.Name = "VideoBox";
            this.VideoBox.Size = new System.Drawing.Size(640, 480);
            this.VideoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.VideoBox.TabIndex = 0;
            this.VideoBox.TabStop = false;
            this.VideoBox.Click += new System.EventHandler(this.VideoBox_Click);
            this.VideoBox.MouseLeave += new System.EventHandler(this.VideoBox_MouseLeave);
            this.VideoBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VideoBox_MouseMove);
            // 
            // VideoBoxContextMenu
            // 
            this.VideoBoxContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.videoStreamToolStripMenuItem,
            this.toolStripMenuItem1,
            this.retractToolStripMenuItem,
            this.neutralToolStripMenuItem,
            this.pointDownToolStripMenuItem,
            this.pointHomeToolStripMenuItem,
            this.yawLockToolStripMenuItem,
            this.toolStripSeparator1,
            this.takePictureToolStripMenuItem,
            this.startRecordingToolStripMenuItem,
            this.stopRecordingToolStripMenuItem,
            this.toolStripMenuItem2,
            this.settingsToolStripMenuItem});
            this.VideoBoxContextMenu.Name = "VideoBoxContextMenu";
            this.VideoBoxContextMenu.Size = new System.Drawing.Size(156, 242);
            // 
            // videoStreamToolStripMenuItem
            // 
            this.videoStreamToolStripMenuItem.Name = "videoStreamToolStripMenuItem";
            this.videoStreamToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.videoStreamToolStripMenuItem.Text = "Video Stream";
            this.videoStreamToolStripMenuItem.Click += new System.EventHandler(this.videoStreamToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // retractToolStripMenuItem
            // 
            this.retractToolStripMenuItem.Name = "retractToolStripMenuItem";
            this.retractToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.retractToolStripMenuItem.Text = "Retract";
            this.retractToolStripMenuItem.Click += new System.EventHandler(this.retractToolStripMenuItem_Click);
            // 
            // neutralToolStripMenuItem
            // 
            this.neutralToolStripMenuItem.Name = "neutralToolStripMenuItem";
            this.neutralToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.neutralToolStripMenuItem.Text = "Neutral";
            this.neutralToolStripMenuItem.Click += new System.EventHandler(this.neutralToolStripMenuItem_Click);
            // 
            // pointDownToolStripMenuItem
            // 
            this.pointDownToolStripMenuItem.Name = "pointDownToolStripMenuItem";
            this.pointDownToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pointDownToolStripMenuItem.Text = "Point Down";
            this.pointDownToolStripMenuItem.Click += new System.EventHandler(this.pointDownToolStripMenuItem_Click);
            // 
            // pointHomeToolStripMenuItem
            // 
            this.pointHomeToolStripMenuItem.Name = "pointHomeToolStripMenuItem";
            this.pointHomeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pointHomeToolStripMenuItem.Text = "Point Home";
            this.pointHomeToolStripMenuItem.Click += new System.EventHandler(this.pointHomeToolStripMenuItem_Click);
            // 
            // yawLockToolStripMenuItem
            // 
            this.yawLockToolStripMenuItem.Name = "yawLockToolStripMenuItem";
            this.yawLockToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.yawLockToolStripMenuItem.Text = "Yaw Lock";
            this.yawLockToolStripMenuItem.Click += new System.EventHandler(this.yawLockToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // takePictureToolStripMenuItem
            // 
            this.takePictureToolStripMenuItem.Name = "takePictureToolStripMenuItem";
            this.takePictureToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.takePictureToolStripMenuItem.Text = "Take Picture";
            this.takePictureToolStripMenuItem.Click += new System.EventHandler(this.takePictureToolStripMenuItem_Click);
            // 
            // startRecordingToolStripMenuItem
            // 
            this.startRecordingToolStripMenuItem.Name = "startRecordingToolStripMenuItem";
            this.startRecordingToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.startRecordingToolStripMenuItem.Text = "Start Recording";
            this.startRecordingToolStripMenuItem.Click += new System.EventHandler(this.startRecordingToolStripMenuItem_Click);
            // 
            // stopRecordingToolStripMenuItem
            // 
            this.stopRecordingToolStripMenuItem.Name = "stopRecordingToolStripMenuItem";
            this.stopRecordingToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.stopRecordingToolStripMenuItem.Text = "Stop Recording";
            this.stopRecordingToolStripMenuItem.Click += new System.EventHandler(this.stopRecordingToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // UITimer
            // 
            this.UITimer.Enabled = true;
            this.UITimer.Interval = 500;
            this.UITimer.Tick += new System.EventHandler(this.UITimer_Tick);
            // 
            // GimbalVideoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.VideoBox);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "GimbalVideoControl";
            this.Size = new System.Drawing.Size(640, 480);
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).EndInit();
            this.VideoBoxContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip ControlInfoTooltip;
        private System.Windows.Forms.ToolStripMenuItem videoStreamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem retractToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neutralToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pointDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pointHomeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yawLockToolStripMenuItem;
        private System.Windows.Forms.Timer UITimer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem takePictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startRecordingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopRecordingToolStripMenuItem;
        public System.Windows.Forms.ContextMenuStrip VideoBoxContextMenu;
        public System.Windows.Forms.PictureBox VideoBox;
    }
}
