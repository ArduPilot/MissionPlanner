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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BUT_Accept = new ArdupilotMega.Controls.MyButton();
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
            this.map = new ArdupilotMega.Controls.myGMAP();
            this.label7 = new System.Windows.Forms.Label();
            this.NUM_overshoot2 = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_overshoot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_angle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_spacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_Distance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_overshoot2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.NUM_overshoot2);
            this.groupBox1.Controls.Add(this.BUT_Accept);
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
            this.groupBox1.Text = "Grid Options";
            // 
            // BUT_Accept
            // 
            this.BUT_Accept.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_Accept.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_Accept.Location = new System.Drawing.Point(137, 338);
            this.BUT_Accept.Name = "BUT_Accept";
            this.BUT_Accept.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_Accept.Size = new System.Drawing.Size(75, 23);
            this.BUT_Accept.TabIndex = 12;
            this.BUT_Accept.Text = "Accept";
            this.BUT_Accept.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Accept.UseVisualStyleBackColor = true;
            this.BUT_Accept.Click += new System.EventHandler(this.BUT_Accept_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 179);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "StartFrom";
            // 
            // CMB_startfrom
            // 
            this.CMB_startfrom.FormattingEnabled = true;
            this.CMB_startfrom.Location = new System.Drawing.Point(121, 176);
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
            this.NUM_overshoot.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
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
            this.NUM_angle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
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
            this.NUM_spacing.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
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
            this.NUM_Distance.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.NUM_Distance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
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
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 152);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "OverShoot";
            // 
            // NUM_overshoot2
            // 
            this.NUM_overshoot2.Location = new System.Drawing.Point(121, 150);
            this.NUM_overshoot2.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.NUM_overshoot2.Name = "NUM_overshoot2";
            this.NUM_overshoot2.Size = new System.Drawing.Size(91, 20);
            this.NUM_overshoot2.TabIndex = 13;
            this.NUM_overshoot2.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
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
            this.Resize += new System.EventHandler(this.GridUI_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_overshoot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_angle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_spacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_Distance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_overshoot2)).EndInit();
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
        private ArdupilotMega.Controls.MyButton BUT_Accept;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown NUM_overshoot2;
    }
}