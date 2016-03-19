namespace MissionPlanner
{
    partial class GridUIv2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridUIv2));
            this.BUT_Accept = new MissionPlanner.Controls.MyButton();
            this.label4 = new System.Windows.Forms.Label();
            this.NUM_angle = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.NUM_altitude = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.TBAR_zoom = new System.Windows.Forms.TrackBar();
            this.chk_includeland = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            this.numericUpDownFlySpeed = new System.Windows.Forms.NumericUpDown();
            this.CHK_includetakeoff = new System.Windows.Forms.CheckBox();
            this.CMB_camera = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chk_footprints = new System.Windows.Forms.CheckBox();
            this.chk_internals = new System.Windows.Forms.CheckBox();
            this.chk_grid = new System.Windows.Forms.CheckBox();
            this.chk_markers = new System.Windows.Forms.CheckBox();
            this.chk_boundary = new System.Windows.Forms.CheckBox();
            this.map = new MissionPlanner.Controls.myGMAP();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.lbl_distbetweenlines = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.lbl_footprint = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.lbl_strips = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.lbl_spacing = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.lbl_distance = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.quickViewimagecount = new MissionPlanner.Controls.QuickView();
            this.quickViewgroundres = new MissionPlanner.Controls.QuickView();
            this.quickViewflighttime = new MissionPlanner.Controls.QuickView();
            this.quickViewarea = new MissionPlanner.Controls.QuickView();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.RAD_camdirectionport = new System.Windows.Forms.RadioButton();
            this.RAD_camdirectionland = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NUM_maxflttime = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.NUM_maxspd = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NUM_minspd = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.CMB_aircraft = new System.Windows.Forms.ComboBox();
            this.LBL_topdock = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonpan = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonbox = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonmovebox = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtoneditbox = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_angle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altitude)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TBAR_zoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFlySpeed)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_maxflttime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_maxspd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_minspd)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BUT_Accept
            // 
            resources.ApplyResources(this.BUT_Accept, "BUT_Accept");
            this.BUT_Accept.Name = "BUT_Accept";
            this.BUT_Accept.UseVisualStyleBackColor = true;
            this.BUT_Accept.Click += new System.EventHandler(this.BUT_Accept_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // NUM_angle
            // 
            resources.ApplyResources(this.NUM_angle, "NUM_angle");
            this.NUM_angle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.NUM_angle.Name = "NUM_angle";
            this.NUM_angle.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // NUM_altitude
            // 
            this.NUM_altitude.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NUM_altitude, "NUM_altitude");
            this.NUM_altitude.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUM_altitude.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NUM_altitude.Name = "NUM_altitude";
            this.NUM_altitude.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NUM_altitude.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.TBAR_zoom);
            this.groupBox6.Controls.Add(this.chk_includeland);
            this.groupBox6.Controls.Add(this.label24);
            this.groupBox6.Controls.Add(this.numericUpDownFlySpeed);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.CHK_includetakeoff);
            this.groupBox6.Controls.Add(this.NUM_angle);
            this.groupBox6.Controls.Add(this.NUM_altitude);
            this.groupBox6.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // TBAR_zoom
            // 
            resources.ApplyResources(this.TBAR_zoom, "TBAR_zoom");
            this.TBAR_zoom.Maximum = 400;
            this.TBAR_zoom.Minimum = 10;
            this.TBAR_zoom.Name = "TBAR_zoom";
            this.TBAR_zoom.TickFrequency = 5;
            this.TBAR_zoom.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.TBAR_zoom.Value = 100;
            this.TBAR_zoom.Scroll += new System.EventHandler(this.TBAR_zoom_Scroll);
            // 
            // chk_includeland
            // 
            resources.ApplyResources(this.chk_includeland, "chk_includeland");
            this.chk_includeland.Checked = true;
            this.chk_includeland.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_includeland.Name = "chk_includeland";
            this.chk_includeland.UseVisualStyleBackColor = true;
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // numericUpDownFlySpeed
            // 
            resources.ApplyResources(this.numericUpDownFlySpeed, "numericUpDownFlySpeed");
            this.numericUpDownFlySpeed.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericUpDownFlySpeed.Name = "numericUpDownFlySpeed";
            this.numericUpDownFlySpeed.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownFlySpeed.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // CHK_includetakeoff
            // 
            resources.ApplyResources(this.CHK_includetakeoff, "CHK_includetakeoff");
            this.CHK_includetakeoff.Checked = true;
            this.CHK_includetakeoff.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_includetakeoff.Name = "CHK_includetakeoff";
            this.CHK_includetakeoff.UseVisualStyleBackColor = true;
            // 
            // CMB_camera
            // 
            this.CMB_camera.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_camera, "CMB_camera");
            this.CMB_camera.Name = "CMB_camera";
            this.CMB_camera.SelectedIndexChanged += new System.EventHandler(this.CMB_camera_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.chk_footprints);
            this.groupBox4.Controls.Add(this.chk_internals);
            this.groupBox4.Controls.Add(this.chk_grid);
            this.groupBox4.Controls.Add(this.chk_markers);
            this.groupBox4.Controls.Add(this.chk_boundary);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // chk_footprints
            // 
            resources.ApplyResources(this.chk_footprints, "chk_footprints");
            this.chk_footprints.Name = "chk_footprints";
            this.chk_footprints.UseVisualStyleBackColor = true;
            this.chk_footprints.CheckedChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // chk_internals
            // 
            resources.ApplyResources(this.chk_internals, "chk_internals");
            this.chk_internals.Name = "chk_internals";
            this.chk_internals.UseVisualStyleBackColor = true;
            this.chk_internals.CheckedChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // chk_grid
            // 
            resources.ApplyResources(this.chk_grid, "chk_grid");
            this.chk_grid.Checked = true;
            this.chk_grid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_grid.Name = "chk_grid";
            this.chk_grid.UseVisualStyleBackColor = true;
            this.chk_grid.CheckedChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // chk_markers
            // 
            resources.ApplyResources(this.chk_markers, "chk_markers");
            this.chk_markers.Checked = true;
            this.chk_markers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_markers.Name = "chk_markers";
            this.chk_markers.UseVisualStyleBackColor = true;
            this.chk_markers.CheckedChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // chk_boundary
            // 
            resources.ApplyResources(this.chk_boundary, "chk_boundary");
            this.chk_boundary.Checked = true;
            this.chk_boundary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_boundary.Name = "chk_boundary";
            this.chk_boundary.UseVisualStyleBackColor = true;
            this.chk_boundary.CheckedChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // map
            // 
            this.map.Bearing = 0F;
            this.map.CanDragMap = true;
            resources.ApplyResources(this.map, "map");
            this.map.EmptyTileColor = System.Drawing.Color.Gray;
            this.map.GrayScaleMode = false;
            this.map.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.map.LevelsKeepInMemmory = 5;
            this.map.MarkersEnabled = true;
            this.map.MaxZoom = 19;
            this.map.MinZoom = 2;
            this.map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.map.Name = "map";
            this.map.NegativeMode = false;
            this.map.PolygonsEnabled = true;
            this.map.RetryLoadTile = 0;
            this.map.RoutesEnabled = true;
            this.map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Fractional;
            this.map.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.map.ShowTileGridLines = false;
            this.map.Zoom = 3D;
            this.map.OnPolygonClick += new GMap.NET.WindowsForms.PolygonClick(this.map_OnPolygonClick);
            this.map.OnPolygonEnter += new GMap.NET.WindowsForms.PolygonEnter(this.map_OnPolygonEnter);
            this.map.OnPolygonLeave += new GMap.NET.WindowsForms.PolygonLeave(this.map_OnPolygonLeave);
            this.map.MouseDown += new System.Windows.Forms.MouseEventHandler(this.map_MouseDown);
            this.map.MouseMove += new System.Windows.Forms.MouseEventHandler(this.map_MouseMove);
            this.map.MouseUp += new System.Windows.Forms.MouseEventHandler(this.map_MouseUp);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.groupBox4);
            this.groupBox5.Controls.Add(this.label32);
            this.groupBox5.Controls.Add(this.label35);
            this.groupBox5.Controls.Add(this.label28);
            this.groupBox5.Controls.Add(this.label31);
            this.groupBox5.Controls.Add(this.lbl_distbetweenlines);
            this.groupBox5.Controls.Add(this.label25);
            this.groupBox5.Controls.Add(this.lbl_footprint);
            this.groupBox5.Controls.Add(this.label30);
            this.groupBox5.Controls.Add(this.lbl_strips);
            this.groupBox5.Controls.Add(this.label33);
            this.groupBox5.Controls.Add(this.lbl_spacing);
            this.groupBox5.Controls.Add(this.label27);
            this.groupBox5.Controls.Add(this.lbl_distance);
            this.groupBox5.Controls.Add(this.label23);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.Name = "label32";
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.Name = "label31";
            // 
            // lbl_distbetweenlines
            // 
            resources.ApplyResources(this.lbl_distbetweenlines, "lbl_distbetweenlines");
            this.lbl_distbetweenlines.Name = "lbl_distbetweenlines";
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // lbl_footprint
            // 
            resources.ApplyResources(this.lbl_footprint, "lbl_footprint");
            this.lbl_footprint.Name = "lbl_footprint";
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // lbl_strips
            // 
            resources.ApplyResources(this.lbl_strips, "lbl_strips");
            this.lbl_strips.Name = "lbl_strips";
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // lbl_spacing
            // 
            resources.ApplyResources(this.lbl_spacing, "lbl_spacing");
            this.lbl_spacing.Name = "lbl_spacing";
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // lbl_distance
            // 
            resources.ApplyResources(this.lbl_distance, "lbl_distance");
            this.lbl_distance.Name = "lbl_distance";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.quickViewimagecount);
            this.panel1.Controls.Add(this.quickViewgroundres);
            this.panel1.Controls.Add(this.quickViewflighttime);
            this.panel1.Controls.Add(this.quickViewarea);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // quickViewimagecount
            // 
            this.quickViewimagecount.desc = "Number of images";
            resources.ApplyResources(this.quickViewimagecount, "quickViewimagecount");
            this.quickViewimagecount.Name = "quickViewimagecount";
            this.quickViewimagecount.number = -9999D;
            this.quickViewimagecount.numberColor = System.Drawing.Color.LimeGreen;
            this.quickViewimagecount.numberformat = "0";
            // 
            // quickViewgroundres
            // 
            this.quickViewgroundres.desc = "Ground resolution (cm/pixel)";
            resources.ApplyResources(this.quickViewgroundres, "quickViewgroundres");
            this.quickViewgroundres.Name = "quickViewgroundres";
            this.quickViewgroundres.number = -9999D;
            this.quickViewgroundres.numberColor = System.Drawing.Color.Chocolate;
            this.quickViewgroundres.numberformat = "0.00";
            // 
            // quickViewflighttime
            // 
            this.quickViewflighttime.desc = "Flight time (min)";
            resources.ApplyResources(this.quickViewflighttime, "quickViewflighttime");
            this.quickViewflighttime.Name = "quickViewflighttime";
            this.quickViewflighttime.number = -9999D;
            this.quickViewflighttime.numberColor = System.Drawing.SystemColors.Highlight;
            this.quickViewflighttime.numberformat = "0";
            // 
            // quickViewarea
            // 
            this.quickViewarea.desc = "Area (km2)";
            resources.ApplyResources(this.quickViewarea, "quickViewarea");
            this.quickViewarea.Name = "quickViewarea";
            this.quickViewarea.number = -9999D;
            this.quickViewarea.numberColor = System.Drawing.Color.Red;
            this.quickViewarea.numberformat = "0.00";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.RAD_camdirectionport);
            this.groupBox7.Controls.Add(this.RAD_camdirectionland);
            this.groupBox7.Controls.Add(this.CMB_camera);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // RAD_camdirectionport
            // 
            resources.ApplyResources(this.RAD_camdirectionport, "RAD_camdirectionport");
            this.RAD_camdirectionport.Checked = true;
            this.RAD_camdirectionport.Name = "RAD_camdirectionport";
            this.RAD_camdirectionport.TabStop = true;
            this.RAD_camdirectionport.UseVisualStyleBackColor = true;
            this.RAD_camdirectionport.CheckedChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // RAD_camdirectionland
            // 
            resources.ApplyResources(this.RAD_camdirectionland, "RAD_camdirectionland");
            this.RAD_camdirectionland.Name = "RAD_camdirectionland";
            this.RAD_camdirectionland.UseVisualStyleBackColor = true;
            this.RAD_camdirectionland.CheckedChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.BUT_Accept);
            this.panel2.Controls.Add(this.groupBox6);
            this.panel2.Controls.Add(this.groupBox7);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.NUM_maxflttime);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.NUM_maxspd);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.NUM_minspd);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.CMB_aircraft);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // NUM_maxflttime
            // 
            this.NUM_maxflttime.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NUM_maxflttime, "NUM_maxflttime");
            this.NUM_maxflttime.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUM_maxflttime.Name = "NUM_maxflttime";
            this.NUM_maxflttime.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUM_maxflttime.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // NUM_maxspd
            // 
            this.NUM_maxspd.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NUM_maxspd, "NUM_maxspd");
            this.NUM_maxspd.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUM_maxspd.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUM_maxspd.Name = "NUM_maxspd";
            this.NUM_maxspd.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.NUM_maxspd.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // NUM_minspd
            // 
            this.NUM_minspd.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NUM_minspd, "NUM_minspd");
            this.NUM_minspd.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUM_minspd.Name = "NUM_minspd";
            this.NUM_minspd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUM_minspd.ValueChanged += new System.EventHandler(this.domainUpDown1_ValueChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // CMB_aircraft
            // 
            this.CMB_aircraft.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_aircraft, "CMB_aircraft");
            this.CMB_aircraft.Name = "CMB_aircraft";
            this.CMB_aircraft.SelectedIndexChanged += new System.EventHandler(this.CMB_aircraft_SelectedIndexChanged);
            // 
            // LBL_topdock
            // 
            resources.ApplyResources(this.LBL_topdock, "LBL_topdock");
            this.LBL_topdock.Name = "LBL_topdock";
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonpan,
            this.toolStripButtonbox,
            this.toolStripButtonmovebox,
            this.toolStripButtoneditbox});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonpan
            // 
            resources.ApplyResources(this.toolStripButtonpan, "toolStripButtonpan");
            this.toolStripButtonpan.Name = "toolStripButtonpan";
            this.toolStripButtonpan.Click += new System.EventHandler(this.toolStripButtonpan_Click);
            // 
            // toolStripButtonbox
            // 
            resources.ApplyResources(this.toolStripButtonbox, "toolStripButtonbox");
            this.toolStripButtonbox.Name = "toolStripButtonbox";
            this.toolStripButtonbox.Click += new System.EventHandler(this.toolStripButtonbox_Click);
            // 
            // toolStripButtonmovebox
            // 
            resources.ApplyResources(this.toolStripButtonmovebox, "toolStripButtonmovebox");
            this.toolStripButtonmovebox.Name = "toolStripButtonmovebox";
            this.toolStripButtonmovebox.Click += new System.EventHandler(this.toolStripButtonmovebox_Click);
            // 
            // toolStripButtoneditbox
            // 
            resources.ApplyResources(this.toolStripButtoneditbox, "toolStripButtoneditbox");
            this.toolStripButtoneditbox.Name = "toolStripButtoneditbox";
            this.toolStripButtoneditbox.Click += new System.EventHandler(this.toolStripButtoneditbox_Click);
            // 
            // GridUIv2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.map);
            this.Controls.Add(this.LBL_topdock);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.panel2);
            this.Name = "GridUIv2";
            this.Load += new System.EventHandler(this.GridUI_Load);
            this.Resize += new System.EventHandler(this.GridUI_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.NUM_angle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altitude)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TBAR_zoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFlySpeed)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_maxflttime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_maxspd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_minspd)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.myGMAP map;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NUM_altitude;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown NUM_angle;
        private Controls.MyButton BUT_Accept;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chk_markers;
        private System.Windows.Forms.CheckBox chk_boundary;
        private System.Windows.Forms.CheckBox chk_footprints;
        private System.Windows.Forms.CheckBox chk_internals;
        private System.Windows.Forms.CheckBox chk_grid;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label lbl_distance;
        private System.Windows.Forms.ComboBox CMB_camera;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lbl_spacing;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label lbl_distbetweenlines;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label lbl_footprint;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label lbl_strips;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.NumericUpDown numericUpDownFlySpeed;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.CheckBox CHK_includetakeoff;
        private System.Windows.Forms.Panel panel1;
        private Controls.QuickView quickViewimagecount;
        private Controls.QuickView quickViewgroundres;
        private Controls.QuickView quickViewflighttime;
        private Controls.QuickView quickViewarea;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton RAD_camdirectionport;
        private System.Windows.Forms.RadioButton RAD_camdirectionland;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox CMB_aircraft;
        private System.Windows.Forms.CheckBox chk_includeland;
        private System.Windows.Forms.NumericUpDown NUM_maxflttime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown NUM_maxspd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown NUM_minspd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar TBAR_zoom;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label LBL_topdock;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonpan;
        private System.Windows.Forms.ToolStripButton toolStripButtonbox;
        private System.Windows.Forms.ToolStripButton toolStripButtoneditbox;
        private System.Windows.Forms.ToolStripButton toolStripButtonmovebox;
    }
}