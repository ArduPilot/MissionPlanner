namespace Carbonix
{
    partial class LandingPlanForm
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
            this.map = new MissionPlanner.Controls.myGMAP();
            this.rad_loitccw = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.num_loitertimemin = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.lbl_alt_unit2 = new System.Windows.Forms.Label();
            this.num_exitalt = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.lbl_alt_unit = new System.Windows.Forms.Label();
            this.num_vtolalt = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.num_winddir = new System.Windows.Forms.NumericUpDown();
            this.rad_loitcw = new System.Windows.Forms.RadioButton();
            this.but_accept = new MissionPlanner.Controls.MyButton();
            this.num_transit_alt = new System.Windows.Forms.NumericUpDown();
            this.lbl_alt_unit3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.num_loitrad = new System.Windows.Forms.NumericUpDown();
            this.lbl_dist_unit = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.chk_land_home = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chk_showalt = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.num_loitertimemin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_exitalt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_vtolalt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_winddir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_transit_alt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_loitrad)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // map
            // 
            this.map.Bearing = 0F;
            this.map.CanDragMap = true;
            this.map.Dock = System.Windows.Forms.DockStyle.Fill;
            this.map.EmptyTileColor = System.Drawing.Color.Gray;
            this.map.GrayScaleMode = false;
            this.map.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.map.HoldInvalidation = false;
            this.map.LevelsKeepInMemmory = 5;
            this.map.Location = new System.Drawing.Point(0, 0);
            this.map.MarkersEnabled = true;
            this.map.MaxZoom = 24;
            this.map.MinZoom = 2;
            this.map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            this.map.Name = "map";
            this.map.NegativeMode = false;
            this.map.PolygonsEnabled = true;
            this.map.RetryLoadTile = 0;
            this.map.RoutesEnabled = true;
            this.map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Fractional;
            this.map.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.map.ShowTileGridLines = false;
            this.map.Size = new System.Drawing.Size(763, 537);
            this.map.TabIndex = 20;
            this.map.Zoom = 3D;
            // 
            // rad_loitccw
            // 
            this.rad_loitccw.AutoSize = true;
            this.rad_loitccw.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rad_loitccw.Location = new System.Drawing.Point(0, 17);
            this.rad_loitccw.Margin = new System.Windows.Forms.Padding(0);
            this.rad_loitccw.Name = "rad_loitccw";
            this.rad_loitccw.Size = new System.Drawing.Size(50, 17);
            this.rad_loitccw.TabIndex = 6;
            this.rad_loitccw.Text = "CCW";
            this.rad_loitccw.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(147, 124);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(23, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "min";
            // 
            // num_loitertimemin
            // 
            this.num_loitertimemin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_loitertimemin.Location = new System.Drawing.Point(89, 121);
            this.num_loitertimemin.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.num_loitertimemin.Name = "num_loitertimemin";
            this.num_loitertimemin.Size = new System.Drawing.Size(52, 20);
            this.num_loitertimemin.TabIndex = 27;
            this.num_loitertimemin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.num_loitertimemin.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(28, 124);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Hold Time";
            // 
            // lbl_alt_unit2
            // 
            this.lbl_alt_unit2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_alt_unit2.AutoSize = true;
            this.lbl_alt_unit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_alt_unit2.Location = new System.Drawing.Point(147, 58);
            this.lbl_alt_unit2.Name = "lbl_alt_unit2";
            this.lbl_alt_unit2.Size = new System.Drawing.Size(13, 13);
            this.lbl_alt_unit2.TabIndex = 25;
            this.lbl_alt_unit2.Text = "ft";
            // 
            // num_exitalt
            // 
            this.num_exitalt.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_exitalt.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.num_exitalt.Location = new System.Drawing.Point(89, 55);
            this.num_exitalt.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.num_exitalt.Minimum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.num_exitalt.Name = "num_exitalt";
            this.num_exitalt.Size = new System.Drawing.Size(52, 20);
            this.num_exitalt.TabIndex = 24;
            this.num_exitalt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.num_exitalt.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.num_exitalt.ValueChanged += new System.EventHandler(this.num_exitalt_ValueChanged);
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(18, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "Exit Altitude:";
            // 
            // lbl_alt_unit
            // 
            this.lbl_alt_unit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_alt_unit.AutoSize = true;
            this.lbl_alt_unit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_alt_unit.Location = new System.Drawing.Point(147, 32);
            this.lbl_alt_unit.Name = "lbl_alt_unit";
            this.lbl_alt_unit.Size = new System.Drawing.Size(13, 13);
            this.lbl_alt_unit.TabIndex = 22;
            this.lbl_alt_unit.Text = "ft";
            // 
            // num_vtolalt
            // 
            this.num_vtolalt.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_vtolalt.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.num_vtolalt.Location = new System.Drawing.Point(89, 29);
            this.num_vtolalt.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.num_vtolalt.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.num_vtolalt.Name = "num_vtolalt";
            this.num_vtolalt.Size = new System.Drawing.Size(52, 20);
            this.num_vtolalt.TabIndex = 21;
            this.num_vtolalt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.num_vtolalt.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.num_vtolalt.ValueChanged += new System.EventHandler(this.num_vtolalt_ValueChanged);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(7, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "VTOL Altitude:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(3, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Show Altitudes:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Wind Direction:";
            // 
            // num_winddir
            // 
            this.num_winddir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_winddir.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.num_winddir.Location = new System.Drawing.Point(89, 3);
            this.num_winddir.Maximum = new decimal(new int[] {
            375,
            0,
            0,
            0});
            this.num_winddir.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            -2147483648});
            this.num_winddir.Name = "num_winddir";
            this.num_winddir.Size = new System.Drawing.Size(52, 20);
            this.num_winddir.TabIndex = 3;
            this.num_winddir.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.num_winddir.Value = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.num_winddir.ValueChanged += new System.EventHandler(this.num_winddir_ValueChanged);
            // 
            // rad_loitcw
            // 
            this.rad_loitcw.AutoSize = true;
            this.rad_loitcw.Checked = true;
            this.rad_loitcw.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rad_loitcw.Location = new System.Drawing.Point(0, 0);
            this.rad_loitcw.Margin = new System.Windows.Forms.Padding(0);
            this.rad_loitcw.Name = "rad_loitcw";
            this.rad_loitcw.Size = new System.Drawing.Size(43, 17);
            this.rad_loitcw.TabIndex = 5;
            this.rad_loitcw.TabStop = true;
            this.rad_loitcw.Text = "CW";
            this.rad_loitcw.UseVisualStyleBackColor = true;
            this.rad_loitcw.CheckedChanged += new System.EventHandler(this.rad_loitcw_CheckedChanged);
            // 
            // but_accept
            // 
            this.but_accept.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.but_accept.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.but_accept.Location = new System.Drawing.Point(176, 479);
            this.but_accept.Name = "but_accept";
            this.but_accept.Size = new System.Drawing.Size(52, 23);
            this.but_accept.TabIndex = 17;
            this.but_accept.Text = "Accept";
            this.but_accept.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_accept.UseVisualStyleBackColor = true;
            this.but_accept.Click += new System.EventHandler(this.but_accept_Click);
            // 
            // num_transit_alt
            // 
            this.num_transit_alt.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_transit_alt.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.num_transit_alt.Location = new System.Drawing.Point(89, 480);
            this.num_transit_alt.Maximum = new decimal(new int[] {
            15000,
            0,
            0,
            0});
            this.num_transit_alt.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.num_transit_alt.Name = "num_transit_alt";
            this.num_transit_alt.Size = new System.Drawing.Size(52, 20);
            this.num_transit_alt.TabIndex = 13;
            this.num_transit_alt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.num_transit_alt.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.num_transit_alt.ValueChanged += new System.EventHandler(this.num_transit_alt_ValueChanged);
            // 
            // lbl_alt_unit3
            // 
            this.lbl_alt_unit3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_alt_unit3.AutoSize = true;
            this.lbl_alt_unit3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_alt_unit3.Location = new System.Drawing.Point(147, 484);
            this.lbl_alt_unit3.Name = "lbl_alt_unit3";
            this.lbl_alt_unit3.Size = new System.Drawing.Size(13, 13);
            this.lbl_alt_unit3.TabIndex = 14;
            this.lbl_alt_unit3.Text = "ft";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(11, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Loiter Radius:";
            // 
            // num_loitrad
            // 
            this.num_loitrad.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_loitrad.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.num_loitrad.Location = new System.Drawing.Point(89, 88);
            this.num_loitrad.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_loitrad.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.num_loitrad.Name = "num_loitrad";
            this.num_loitrad.Size = new System.Drawing.Size(52, 20);
            this.num_loitrad.TabIndex = 8;
            this.num_loitrad.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.num_loitrad.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.num_loitrad.ValueChanged += new System.EventHandler(this.num_loitrad_ValueChanged);
            // 
            // lbl_dist_unit
            // 
            this.lbl_dist_unit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_dist_unit.AutoSize = true;
            this.lbl_dist_unit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_dist_unit.Location = new System.Drawing.Point(147, 91);
            this.lbl_dist_unit.Name = "lbl_dist_unit";
            this.lbl_dist_unit.Size = new System.Drawing.Size(13, 13);
            this.lbl_dist_unit.TabIndex = 11;
            this.lbl_dist_unit.Text = "ft";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.rad_loitcw);
            this.flowLayoutPanel1.Controls.Add(this.rad_loitccw);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(176, 81);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(50, 34);
            this.flowLayoutPanel1.TabIndex = 10;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabOptions);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tabControl1.Location = new System.Drawing.Point(763, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(245, 537);
            this.tabControl1.TabIndex = 21;
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.tableLayoutPanel1);
            this.tabOptions.Location = new System.Drawing.Point(4, 22);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabOptions.Size = new System.Drawing.Size(237, 511);
            this.tabOptions.TabIndex = 3;
            this.tabOptions.Text = "Options";
            this.tabOptions.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label9, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.num_loitertimemin, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lbl_alt_unit2, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.num_exitalt, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lbl_alt_unit, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.num_vtolalt, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.num_winddir, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.but_accept, 3, 8);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.num_transit_alt, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.lbl_alt_unit3, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.num_loitrad, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbl_dist_unit, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.chk_land_home, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.chk_showalt, 1, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(231, 505);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(3, 484);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Transit Altitude:";
            // 
            // chk_land_home
            // 
            this.chk_land_home.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chk_land_home.AutoSize = true;
            this.chk_land_home.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_land_home.Location = new System.Drawing.Point(89, 147);
            this.chk_land_home.Name = "chk_land_home";
            this.chk_land_home.Size = new System.Drawing.Size(15, 14);
            this.chk_land_home.TabIndex = 16;
            this.chk_land_home.UseVisualStyleBackColor = true;
            this.chk_land_home.CheckedChanged += new System.EventHandler(this.chk_land_home_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 147);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Land at Home:";
            // 
            // chk_showalt
            // 
            this.chk_showalt.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chk_showalt.AutoSize = true;
            this.chk_showalt.Checked = true;
            this.chk_showalt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_showalt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_showalt.Location = new System.Drawing.Point(89, 167);
            this.chk_showalt.Name = "chk_showalt";
            this.chk_showalt.Size = new System.Drawing.Size(15, 14);
            this.chk_showalt.TabIndex = 19;
            this.chk_showalt.UseVisualStyleBackColor = true;
            this.chk_showalt.CheckedChanged += new System.EventHandler(this.chk_showalt_CheckedChanged);
            // 
            // LandingPlanForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 537);
            this.Controls.Add(this.map);
            this.Controls.Add(this.tabControl1);
            this.Name = "LandingPlanForm";
            this.Text = "Landing Sequence Planner";
            this.Load += new System.EventHandler(this.LandingPlanUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.num_loitertimemin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_exitalt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_vtolalt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_winddir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_transit_alt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_loitrad)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabOptions.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MissionPlanner.Controls.myGMAP map;
        private System.Windows.Forms.RadioButton rad_loitccw;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown num_loitertimemin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbl_alt_unit2;
        private System.Windows.Forms.NumericUpDown num_exitalt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lbl_alt_unit;
        private System.Windows.Forms.NumericUpDown num_vtolalt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown num_winddir;
        private System.Windows.Forms.RadioButton rad_loitcw;
        private MissionPlanner.Controls.MyButton but_accept;
        private System.Windows.Forms.NumericUpDown num_transit_alt;
        private System.Windows.Forms.Label lbl_alt_unit3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown num_loitrad;
        private System.Windows.Forms.Label lbl_dist_unit;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chk_land_home;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chk_showalt;
    }
}