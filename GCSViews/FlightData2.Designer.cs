namespace MissionPlanner.GCSViews
{
    partial class FlightData2
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.quickView3 = new MissionPlanner.Controls.QuickView();
            this.quickView2 = new MissionPlanner.Controls.QuickView();
            this.quickView1 = new MissionPlanner.Controls.QuickView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.quickView7 = new MissionPlanner.Controls.QuickView();
            this.quickView6 = new MissionPlanner.Controls.QuickView();
            this.quickView5 = new MissionPlanner.Controls.QuickView();
            this.quickView4 = new MissionPlanner.Controls.QuickView();
            this.hud1 = new MissionPlanner.Controls.HUD();
            this.panel3 = new System.Windows.Forms.Panel();
            this.coords1 = new MissionPlanner.Controls.Coords();
            this.myGMAP1 = new MissionPlanner.Controls.myGMAP();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.quickView3);
            this.panel1.Controls.Add(this.quickView2);
            this.panel1.Controls.Add(this.quickView1);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(155, 507);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.menuStrip1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 129);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(155, 216);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(30, 30);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.menuStrip1.Location = new System.Drawing.Point(3, 16);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(149, 142);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::MissionPlanner.Properties.Resources.dark_flightdata_icon;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(142, 34);
            this.toolStripMenuItem1.Text = "Flight";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = global::MissionPlanner.Properties.Resources.dark_flightplan_icon;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(142, 34);
            this.toolStripMenuItem2.Text = "Plan";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Image = global::MissionPlanner.Properties.Resources.dark_initialsetup_icon;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(142, 34);
            this.toolStripMenuItem3.Text = "Setup";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Image = global::MissionPlanner.Properties.Resources.dark_tuningconfig_icon;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(142, 34);
            this.toolStripMenuItem4.Text = "Tune";
            // 
            // quickView3
            // 
            this.quickView3.desc = "Altitude:";
            this.quickView3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.quickView3.Location = new System.Drawing.Point(0, 345);
            this.quickView3.Name = "quickView3";
            this.quickView3.number = -9999D;
            this.quickView3.numberColor = System.Drawing.Color.White;
            this.quickView3.numberformat = "0.00";
            this.quickView3.Size = new System.Drawing.Size(155, 54);
            this.quickView3.TabIndex = 2;
            // 
            // quickView2
            // 
            this.quickView2.desc = "Altitude:";
            this.quickView2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.quickView2.Location = new System.Drawing.Point(0, 399);
            this.quickView2.Name = "quickView2";
            this.quickView2.number = -9999D;
            this.quickView2.numberColor = System.Drawing.Color.White;
            this.quickView2.numberformat = "0.00";
            this.quickView2.Size = new System.Drawing.Size(155, 54);
            this.quickView2.TabIndex = 1;
            // 
            // quickView1
            // 
            this.quickView1.desc = "Altitude:";
            this.quickView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.quickView1.Location = new System.Drawing.Point(0, 453);
            this.quickView1.Name = "quickView1";
            this.quickView1.number = -9999D;
            this.quickView1.numberColor = System.Drawing.Color.White;
            this.quickView1.numberformat = "0.00";
            this.quickView1.Size = new System.Drawing.Size(155, 54);
            this.quickView1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(155, 129);
            this.panel4.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.InfoText;
            this.panel2.Controls.Add(this.quickView7);
            this.panel2.Controls.Add(this.quickView6);
            this.panel2.Controls.Add(this.quickView5);
            this.panel2.Controls.Add(this.quickView4);
            this.panel2.Controls.Add(this.hud1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(155, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(328, 507);
            this.panel2.TabIndex = 1;
            // 
            // quickView7
            // 
            this.quickView7.desc = "Altitude:";
            this.quickView7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.quickView7.Location = new System.Drawing.Point(0, 291);
            this.quickView7.Name = "quickView7";
            this.quickView7.number = -9999D;
            this.quickView7.numberColor = System.Drawing.Color.White;
            this.quickView7.numberformat = "0.00";
            this.quickView7.Size = new System.Drawing.Size(328, 54);
            this.quickView7.TabIndex = 6;
            // 
            // quickView6
            // 
            this.quickView6.desc = "Altitude:";
            this.quickView6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.quickView6.Location = new System.Drawing.Point(0, 345);
            this.quickView6.Name = "quickView6";
            this.quickView6.number = -9999D;
            this.quickView6.numberColor = System.Drawing.Color.White;
            this.quickView6.numberformat = "0.00";
            this.quickView6.Size = new System.Drawing.Size(328, 54);
            this.quickView6.TabIndex = 5;
            // 
            // quickView5
            // 
            this.quickView5.desc = "Altitude:";
            this.quickView5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.quickView5.Location = new System.Drawing.Point(0, 399);
            this.quickView5.Name = "quickView5";
            this.quickView5.number = -9999D;
            this.quickView5.numberColor = System.Drawing.Color.White;
            this.quickView5.numberformat = "0.00";
            this.quickView5.Size = new System.Drawing.Size(328, 54);
            this.quickView5.TabIndex = 4;
            // 
            // quickView4
            // 
            this.quickView4.desc = "Altitude:";
            this.quickView4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.quickView4.Location = new System.Drawing.Point(0, 453);
            this.quickView4.Name = "quickView4";
            this.quickView4.number = -9999D;
            this.quickView4.numberColor = System.Drawing.Color.White;
            this.quickView4.numberformat = "0.00";
            this.quickView4.Size = new System.Drawing.Size(328, 54);
            this.quickView4.TabIndex = 3;
            // 
            // hud1
            // 
            this.hud1.airspeed = 0F;
            this.hud1.alt = 0F;
            this.hud1.BackColor = System.Drawing.Color.Black;
            this.hud1.batterylevel = 0F;
            this.hud1.batteryremaining = 0F;
            this.hud1.connected = false;
            this.hud1.current = 0F;
            this.hud1.datetime = new System.DateTime(((long)(0)));
            this.hud1.disttowp = 0F;
            this.hud1.Dock = System.Windows.Forms.DockStyle.Top;
            this.hud1.failsafe = false;
            this.hud1.gpsfix = 0F;
            this.hud1.gpshdop = 0F;
            this.hud1.groundalt = 0F;
            this.hud1.groundcourse = 0F;
            this.hud1.groundspeed = 0F;
            this.hud1.heading = 0F;
            this.hud1.hudcolor = System.Drawing.Color.White;
            this.hud1.linkqualitygcs = 0F;
            this.hud1.Location = new System.Drawing.Point(0, 0);
            this.hud1.lowairspeed = false;
            this.hud1.lowgroundspeed = false;
            this.hud1.lowvoltagealert = false;
            this.hud1.message = "";
            this.hud1.messagetime = new System.DateTime(((long)(0)));
            this.hud1.mode = "Unknown";
            this.hud1.Name = "hud1";
            this.hud1.navpitch = 0F;
            this.hud1.navroll = 0F;
            this.hud1.opengl = true;
            this.hud1.pitch = 0F;
            this.hud1.roll = 0F;
            this.hud1.Russian = false;
            this.hud1.Size = new System.Drawing.Size(328, 267);
            this.hud1.status = false;
            this.hud1.streamjpg = null;
            this.hud1.TabIndex = 2;
            this.hud1.targetalt = 0F;
            this.hud1.targetheading = 0F;
            this.hud1.targetspeed = 0F;
            this.hud1.turnrate = 0F;
            this.hud1.UseOpenGL = true;
            this.hud1.verticalspeed = 0F;
            this.hud1.VSync = false;
            this.hud1.wpno = 0;
            this.hud1.xtrack_error = 0F;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.coords1);
            this.panel3.Controls.Add(this.myGMAP1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(483, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(539, 507);
            this.panel3.TabIndex = 2;
            // 
            // coords1
            // 
            this.coords1.Alt = 0D;
            this.coords1.BackColor = System.Drawing.Color.Transparent;
            this.coords1.Lat = 0D;
            this.coords1.Lng = 0D;
            this.coords1.Location = new System.Drawing.Point(7, 4);
            this.coords1.Name = "coords1";
            this.coords1.Size = new System.Drawing.Size(200, 21);
            this.coords1.TabIndex = 1;
            this.coords1.Vertical = false;
            // 
            // myGMAP1
            // 
            this.myGMAP1.Bearing = 0F;
            this.myGMAP1.CanDragMap = true;
            this.myGMAP1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myGMAP1.EmptyTileColor = System.Drawing.Color.Navy;
            this.myGMAP1.GrayScaleMode = false;
            this.myGMAP1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.myGMAP1.LevelsKeepInMemmory = 5;
            this.myGMAP1.Location = new System.Drawing.Point(0, 0);
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
            this.myGMAP1.Size = new System.Drawing.Size(539, 507);
            this.myGMAP1.TabIndex = 0;
            this.myGMAP1.Zoom = 0D;
            // 
            // FlightData2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "FlightData2";
            this.Size = new System.Drawing.Size(1022, 507);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private Controls.Coords coords1;
        private Controls.myGMAP myGMAP1;
        private Controls.QuickView quickView3;
        private Controls.QuickView quickView2;
        private Controls.QuickView quickView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private Controls.HUD hud1;
        private System.Windows.Forms.Panel panel4;
        private Controls.QuickView quickView7;
        private Controls.QuickView quickView6;
        private Controls.QuickView quickView5;
        private Controls.QuickView quickView4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
    }
}