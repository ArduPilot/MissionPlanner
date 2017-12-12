namespace MissionPlanner.Controls
{
    partial class SITL
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SITL));
            this.myGMAP1 = new MissionPlanner.Controls.myGMAP();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBoxheli = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxquad = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxrover = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.pictureBoxplane = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NUM_heading = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_cmdline = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmb_model = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.num_simspeed = new System.Windows.Forms.NumericUpDown();
            this.chk_wipe = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxheli)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxquad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxrover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxplane)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_heading)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_simspeed)).BeginInit();
            this.SuspendLayout();
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
            this.myGMAP1.Location = new System.Drawing.Point(3, 16);
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
            this.myGMAP1.Size = new System.Drawing.Size(831, 303);
            this.myGMAP1.TabIndex = 0;
            this.myGMAP1.Zoom = 0D;
            this.myGMAP1.OnMarkerEnter += new GMap.NET.WindowsForms.MarkerEnter(this.myGMAP1_OnMarkerEnter);
            this.myGMAP1.OnMarkerLeave += new GMap.NET.WindowsForms.MarkerLeave(this.myGMAP1_OnMarkerLeave);
            this.myGMAP1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.myGMAP1_MouseDown);
            this.myGMAP1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.myGMAP1_MouseMove);
            this.myGMAP1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.myGMAP1_MouseUp);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.pictureBoxheli);
            this.panel1.Controls.Add(this.pictureBoxquad);
            this.panel1.Controls.Add(this.pictureBoxrover);
            this.panel1.Controls.Add(this.pictureBoxplane);
            this.panel1.Location = new System.Drawing.Point(60, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(716, 137);
            this.panel1.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(48, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Plane";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(217, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Rover";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(370, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Multirotor";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(569, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Helicopter";
            // 
            // pictureBoxheli
            // 
            this.pictureBoxheli.Image = global::MissionPlanner.Properties.Resources.light_06;
            this.pictureBoxheli.ImageNormal = global::MissionPlanner.Properties.Resources.light_06;
            this.pictureBoxheli.ImageOver = global::MissionPlanner.Properties.Resources._01_06;
            this.pictureBoxheli.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxheli.Location = new System.Drawing.Point(477, 3);
            this.pictureBoxheli.Name = "pictureBoxheli";
            this.pictureBoxheli.selected = false;
            this.pictureBoxheli.Size = new System.Drawing.Size(235, 112);
            this.pictureBoxheli.TabIndex = 13;
            this.pictureBoxheli.TabStop = false;
            this.pictureBoxheli.Tag = "heli";
            this.pictureBoxheli.Click += new System.EventHandler(this.pictureBoxheli_Click);
            // 
            // pictureBoxquad
            // 
            this.pictureBoxquad.Image = global::MissionPlanner.Properties.Resources.light_05;
            this.pictureBoxquad.ImageNormal = global::MissionPlanner.Properties.Resources.light_05;
            this.pictureBoxquad.ImageOver = global::MissionPlanner.Properties.Resources._01_05;
            this.pictureBoxquad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxquad.Location = new System.Drawing.Point(331, 3);
            this.pictureBoxquad.Name = "pictureBoxquad";
            this.pictureBoxquad.selected = false;
            this.pictureBoxquad.Size = new System.Drawing.Size(125, 112);
            this.pictureBoxquad.TabIndex = 12;
            this.pictureBoxquad.TabStop = false;
            this.pictureBoxquad.Tag = "copter";
            this.pictureBoxquad.Click += new System.EventHandler(this.pictureBoxquad_Click);
            // 
            // pictureBoxrover
            // 
            this.pictureBoxrover.Image = global::MissionPlanner.Properties.Resources.light_03;
            this.pictureBoxrover.ImageNormal = global::MissionPlanner.Properties.Resources.light_03;
            this.pictureBoxrover.ImageOver = global::MissionPlanner.Properties.Resources._01_03;
            this.pictureBoxrover.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxrover.Location = new System.Drawing.Point(159, 3);
            this.pictureBoxrover.Name = "pictureBoxrover";
            this.pictureBoxrover.selected = false;
            this.pictureBoxrover.Size = new System.Drawing.Size(150, 112);
            this.pictureBoxrover.TabIndex = 11;
            this.pictureBoxrover.TabStop = false;
            this.pictureBoxrover.Tag = "rover";
            this.pictureBoxrover.Click += new System.EventHandler(this.pictureBoxrover_Click);
            // 
            // pictureBoxplane
            // 
            this.pictureBoxplane.Image = global::MissionPlanner.Properties.Resources.light_01;
            this.pictureBoxplane.ImageNormal = global::MissionPlanner.Properties.Resources.light_01;
            this.pictureBoxplane.ImageOver = global::MissionPlanner.Properties.Resources._01_01;
            this.pictureBoxplane.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxplane.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxplane.Name = "pictureBoxplane";
            this.pictureBoxplane.selected = false;
            this.pictureBoxplane.Size = new System.Drawing.Size(131, 112);
            this.pictureBoxplane.TabIndex = 10;
            this.pictureBoxplane.TabStop = false;
            this.pictureBoxplane.Tag = "plane";
            this.pictureBoxplane.Click += new System.EventHandler(this.pictureBoxplane_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.myGMAP1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(837, 322);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Home Location - Drag Me";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Location = new System.Drawing.Point(13, 390);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(837, 152);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Please select a firmware to run";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.NUM_heading);
            this.groupBox3.Location = new System.Drawing.Point(13, 341);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(123, 43);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Options";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Heading";
            // 
            // NUM_heading
            // 
            this.NUM_heading.Location = new System.Drawing.Point(60, 18);
            this.NUM_heading.Name = "NUM_heading";
            this.NUM_heading.Size = new System.Drawing.Size(51, 20);
            this.NUM_heading.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.chk_wipe);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.txt_cmdline);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.cmb_model);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.num_simspeed);
            this.groupBox4.Location = new System.Drawing.Point(152, 341);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(698, 43);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Advanced users only";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(296, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(99, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Extra command line";
            // 
            // txt_cmdline
            // 
            this.txt_cmdline.Location = new System.Drawing.Point(401, 17);
            this.txt_cmdline.Name = "txt_cmdline";
            this.txt_cmdline.Size = new System.Drawing.Size(230, 20);
            this.txt_cmdline.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(127, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Model";
            // 
            // cmb_model
            // 
            this.cmb_model.FormattingEnabled = true;
            this.cmb_model.Items.AddRange(new object[] {
            "",
            "quadplane",
            "xplane",
            "xplane-heli",
            "firefly",
            "+",
            "quad",
            "copter",
            "x",
            "hexa",
            "octa",
            "tri",
            "y6",
            "heli",
            "heli-dual",
            "heli-compound",
            "singlecopter",
            "coaxcopter",
            "rover",
            "crrcsim",
            "jsbsim",
            "flightaxis",
            "gazebo",
            "last_letter",
            "tracker",
            "balloon",
            "plane",
            "calibration"});
            this.cmb_model.Location = new System.Drawing.Point(169, 17);
            this.cmb_model.Name = "cmb_model";
            this.cmb_model.Size = new System.Drawing.Size(121, 21);
            this.cmb_model.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Sim Speed";
            // 
            // num_simspeed
            // 
            this.num_simspeed.Location = new System.Drawing.Point(70, 18);
            this.num_simspeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_simspeed.Name = "num_simspeed";
            this.num_simspeed.Size = new System.Drawing.Size(51, 20);
            this.num_simspeed.TabIndex = 6;
            this.num_simspeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chk_wipe
            // 
            this.chk_wipe.AutoSize = true;
            this.chk_wipe.Location = new System.Drawing.Point(638, 17);
            this.chk_wipe.Name = "chk_wipe";
            this.chk_wipe.Size = new System.Drawing.Size(51, 17);
            this.chk_wipe.TabIndex = 12;
            this.chk_wipe.Text = "Wipe";
            this.chk_wipe.UseVisualStyleBackColor = true;
            // 
            // SITL
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "SITL";
            this.Size = new System.Drawing.Size(862, 544);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxheli)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxquad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxrover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxplane)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_heading)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_simspeed)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private myGMAP myGMAP1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private PictureBoxMouseOver pictureBoxheli;
        private PictureBoxMouseOver pictureBoxquad;
        private PictureBoxMouseOver pictureBoxrover;
        private PictureBoxMouseOver pictureBoxplane;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NUM_heading;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmb_model;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown num_simspeed;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_cmdline;
        private System.Windows.Forms.CheckBox chk_wipe;
    }
}