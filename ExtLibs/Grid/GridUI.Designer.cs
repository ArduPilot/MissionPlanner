namespace MissionPlanner
{
    partial class GridUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridUI));
            this.map = new ArdupilotMega.Controls.myGMAP();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.CMB_startfrom = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.NUM_overshoot = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.NUM_angle = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NUM_spacing = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.NUM_Distance = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.NUM_altitude = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_overshoot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_angle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_spacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_Distance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altitude)).BeginInit();
            this.SuspendLayout();
            // 
            // map
            // 
            this.map.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.map.Bearing = 0F;
            this.map.CanDragMap = true;
            this.map.GrayScaleMode = false;
            this.map.LevelsKeepInMemmory = 5;
            this.map.Location = new System.Drawing.Point(-19, 0);
            this.map.MapType = GMap.NET.MapType.GoogleSatellite;
            this.map.MarkersEnabled = true;
            this.map.MaxZoom = 19;
            this.map.MinZoom = 2;
            this.map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.map.Name = "map";
            this.map.NegativeMode = false;
            this.map.PolygonsEnabled = true;
            this.map.RetryLoadTile = 0;
            this.map.RoutesEnabled = true;
            this.map.ShowTileGridLines = false;
            this.map.Size = new System.Drawing.Size(424, 392);
            this.map.streamjpg = ((System.IO.MemoryStream)(resources.GetObject("map.streamjpg")));
            this.map.TabIndex = 0;
            this.map.Zoom = 3D;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.CMB_startfrom);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.NUM_overshoot);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.NUM_angle);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.NUM_spacing);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.NUM_Distance);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.NUM_altitude);
            this.groupBox1.Location = new System.Drawing.Point(411, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(219, 367);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "StartFrom";
            // 
            // CMB_startfrom
            // 
            this.CMB_startfrom.FormattingEnabled = true;
            this.CMB_startfrom.Location = new System.Drawing.Point(121, 151);
            this.CMB_startfrom.Name = "CMB_startfrom";
            this.CMB_startfrom.Size = new System.Drawing.Size(92, 21);
            this.CMB_startfrom.TabIndex = 10;
            this.CMB_startfrom.SelectedIndexChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "OverShoot";
            // 
            // NUM_overshoot
            // 
            this.NUM_overshoot.Location = new System.Drawing.Point(121, 124);
            this.NUM_overshoot.Name = "NUM_overshoot";
            this.NUM_overshoot.Size = new System.Drawing.Size(91, 20);
            this.NUM_overshoot.TabIndex = 8;
            this.NUM_overshoot.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.NUM_overshoot.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Angle";
            // 
            // NUM_angle
            // 
            this.NUM_angle.Location = new System.Drawing.Point(121, 98);
            this.NUM_angle.Name = "NUM_angle";
            this.NUM_angle.Size = new System.Drawing.Size(91, 20);
            this.NUM_angle.TabIndex = 6;
            this.NUM_angle.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Spacing";
            // 
            // NUM_spacing
            // 
            this.NUM_spacing.Location = new System.Drawing.Point(122, 72);
            this.NUM_spacing.Name = "NUM_spacing";
            this.NUM_spacing.Size = new System.Drawing.Size(91, 20);
            this.NUM_spacing.TabIndex = 4;
            this.NUM_spacing.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Distance between lines";
            // 
            // NUM_Distance
            // 
            this.NUM_Distance.Location = new System.Drawing.Point(122, 46);
            this.NUM_Distance.Name = "NUM_Distance";
            this.NUM_Distance.Size = new System.Drawing.Size(91, 20);
            this.NUM_Distance.TabIndex = 2;
            this.NUM_Distance.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.NUM_Distance.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Altitiude";
            // 
            // NUM_altitude
            // 
            this.NUM_altitude.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUM_altitude.Location = new System.Drawing.Point(121, 20);
            this.NUM_altitude.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.NUM_altitude.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NUM_altitude.Name = "NUM_altitude";
            this.NUM_altitude.Size = new System.Drawing.Size(91, 20);
            this.NUM_altitude.TabIndex = 0;
            this.NUM_altitude.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NUM_altitude.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // GridUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 392);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.map);
            this.Name = "GridUI";
            this.Text = "GridUI";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_overshoot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_angle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_spacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_Distance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altitude)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ArdupilotMega.Controls.myGMAP map;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NUM_altitude;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown NUM_overshoot;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown NUM_angle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown NUM_spacing;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NUM_Distance;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox CMB_startfrom;
    }
}