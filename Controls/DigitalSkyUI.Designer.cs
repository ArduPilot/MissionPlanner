namespace MissionPlanner.Controls
{
    partial class DigitalSkyUI
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
            this.cmb_drones = new System.Windows.Forms.ComboBox();
            this.cmb_applications = new System.Windows.Forms.ComboBox();
            this.lbl_approvedstatus = new System.Windows.Forms.Label();
            this.but_dlartifact = new MissionPlanner.Controls.MyButton();
            this.but_login = new MissionPlanner.Controls.MyButton();
            this.myGMAP1 = new MissionPlanner.Controls.myGMAP();
            this.but_uploadflightlog = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // cmb_drones
            // 
            this.cmb_drones.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmb_drones.FormattingEnabled = true;
            this.cmb_drones.Location = new System.Drawing.Point(84, 393);
            this.cmb_drones.Name = "cmb_drones";
            this.cmb_drones.Size = new System.Drawing.Size(121, 21);
            this.cmb_drones.TabIndex = 1;
            this.cmb_drones.SelectedIndexChanged += new System.EventHandler(this.Cmb_drones_SelectedIndexChanged);
            // 
            // cmb_applications
            // 
            this.cmb_applications.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmb_applications.FormattingEnabled = true;
            this.cmb_applications.Location = new System.Drawing.Point(211, 393);
            this.cmb_applications.Name = "cmb_applications";
            this.cmb_applications.Size = new System.Drawing.Size(197, 21);
            this.cmb_applications.TabIndex = 3;
            this.cmb_applications.SelectedIndexChanged += new System.EventHandler(this.Cmb_applications_SelectedIndexChanged);
            // 
            // lbl_approvedstatus
            // 
            this.lbl_approvedstatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_approvedstatus.AutoSize = true;
            this.lbl_approvedstatus.Location = new System.Drawing.Point(414, 396);
            this.lbl_approvedstatus.Name = "lbl_approvedstatus";
            this.lbl_approvedstatus.Size = new System.Drawing.Size(16, 13);
            this.lbl_approvedstatus.TabIndex = 4;
            this.lbl_approvedstatus.Text = "...";
            // 
            // but_dlartifact
            // 
            this.but_dlartifact.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.but_dlartifact.Location = new System.Drawing.Point(506, 391);
            this.but_dlartifact.Name = "but_dlartifact";
            this.but_dlartifact.Size = new System.Drawing.Size(75, 23);
            this.but_dlartifact.TabIndex = 5;
            this.but_dlartifact.Text = "Permission Artifact";
            this.but_dlartifact.UseVisualStyleBackColor = true;
            this.but_dlartifact.Click += new System.EventHandler(this.But_dlartifact_Click);
            // 
            // but_login
            // 
            this.but_login.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.but_login.Location = new System.Drawing.Point(3, 391);
            this.but_login.Name = "but_login";
            this.but_login.Size = new System.Drawing.Size(75, 23);
            this.but_login.TabIndex = 2;
            this.but_login.Text = "Login";
            this.but_login.UseVisualStyleBackColor = true;
            this.but_login.Click += new System.EventHandler(this.But_login_Click);
            // 
            // myGMAP1
            // 
            this.myGMAP1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myGMAP1.Bearing = 0F;
            this.myGMAP1.CanDragMap = true;
            this.myGMAP1.EmptyTileColor = System.Drawing.Color.Navy;
            this.myGMAP1.GrayScaleMode = false;
            this.myGMAP1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.myGMAP1.HoldInvalidation = false;
            this.myGMAP1.LevelsKeepInMemmory = 5;
            this.myGMAP1.Location = new System.Drawing.Point(3, 3);
            this.myGMAP1.MarkersEnabled = true;
            this.myGMAP1.MaxZoom = 2;
            this.myGMAP1.MinZoom = 2;
            this.myGMAP1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.myGMAP1.Name = "myGMAP1";
            this.myGMAP1.NegativeMode = false;
            this.myGMAP1.PolygonsEnabled = true;
            this.myGMAP1.RetryLoadTile = 0;
            this.myGMAP1.RoutesEnabled = true;
            this.myGMAP1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.myGMAP1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.myGMAP1.ShowTileGridLines = false;
            this.myGMAP1.Size = new System.Drawing.Size(949, 382);
            this.myGMAP1.TabIndex = 0;
            this.myGMAP1.Zoom = 0D;
            // 
            // but_uploadflightlog
            // 
            this.but_uploadflightlog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.but_uploadflightlog.Location = new System.Drawing.Point(587, 391);
            this.but_uploadflightlog.Name = "but_uploadflightlog";
            this.but_uploadflightlog.Size = new System.Drawing.Size(75, 23);
            this.but_uploadflightlog.TabIndex = 6;
            this.but_uploadflightlog.Text = "Upload Flight Log";
            this.but_uploadflightlog.UseVisualStyleBackColor = true;
            this.but_uploadflightlog.Click += new System.EventHandler(this.But_uploadflightlog_Click);
            // 
            // DigitalSkyUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.but_uploadflightlog);
            this.Controls.Add(this.but_dlartifact);
            this.Controls.Add(this.lbl_approvedstatus);
            this.Controls.Add(this.cmb_applications);
            this.Controls.Add(this.but_login);
            this.Controls.Add(this.cmb_drones);
            this.Controls.Add(this.myGMAP1);
            this.Name = "DigitalSkyUI";
            this.Size = new System.Drawing.Size(955, 454);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private myGMAP myGMAP1;
        private System.Windows.Forms.ComboBox cmb_drones;
        private MyButton but_login;
        private System.Windows.Forms.ComboBox cmb_applications;
        private System.Windows.Forms.Label lbl_approvedstatus;
        private MyButton but_dlartifact;
        private MyButton but_uploadflightlog;
    }
}
