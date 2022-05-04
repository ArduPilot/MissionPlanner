namespace MissionPlanner.Controls
{
    partial class fftui
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
            this.components = new System.ComponentModel.Container();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.zedGraphControl6 = new ZedGraph.ZedGraphControl();
            this.zedGraphControl5 = new ZedGraph.ZedGraphControl();
            this.zedGraphControl4 = new ZedGraph.ZedGraphControl();
            this.zedGraphControl3 = new ZedGraph.ZedGraphControl();
            this.zedGraphControl2 = new ZedGraph.ZedGraphControl();
            this.NUM_bins = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NUM_startfreq = new System.Windows.Forms.NumericUpDown();
            this.chk_mag = new System.Windows.Forms.CheckBox();
            this.but_ISBH = new MissionPlanner.Controls.MyButton();
            this.but_fftimu13 = new MissionPlanner.Controls.MyButton();
            this.BUT_accgyrall = new MissionPlanner.Controls.MyButton();
            this.but_accgyr1 = new MissionPlanner.Controls.MyButton();
            this.BUT_runwav = new MissionPlanner.Controls.MyButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_bins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_startfreq)).BeginInit();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphControl1.IsShowPointValues = true;
            this.zedGraphControl1.Location = new System.Drawing.Point(3, 3);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(255, 238);
            this.zedGraphControl1.TabIndex = 0;
            this.zedGraphControl1.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.zedGraphControl_PointValueEvent);
            this.zedGraphControl1.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl1_MouseMoveEvent);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.zedGraphControl6, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.zedGraphControl5, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.zedGraphControl4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.zedGraphControl3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.zedGraphControl2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.zedGraphControl1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(785, 489);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // zedGraphControl6
            // 
            this.zedGraphControl6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphControl6.IsShowPointValues = true;
            this.zedGraphControl6.Location = new System.Drawing.Point(525, 247);
            this.zedGraphControl6.Name = "zedGraphControl6";
            this.zedGraphControl6.ScrollGrace = 0D;
            this.zedGraphControl6.ScrollMaxX = 0D;
            this.zedGraphControl6.ScrollMaxY = 0D;
            this.zedGraphControl6.ScrollMaxY2 = 0D;
            this.zedGraphControl6.ScrollMinX = 0D;
            this.zedGraphControl6.ScrollMinY = 0D;
            this.zedGraphControl6.ScrollMinY2 = 0D;
            this.zedGraphControl6.Size = new System.Drawing.Size(257, 239);
            this.zedGraphControl6.TabIndex = 5;
            this.zedGraphControl6.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.zedGraphControl_PointValueEvent);
            this.zedGraphControl6.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl1_MouseMoveEvent);
            // 
            // zedGraphControl5
            // 
            this.zedGraphControl5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphControl5.IsShowPointValues = true;
            this.zedGraphControl5.Location = new System.Drawing.Point(264, 247);
            this.zedGraphControl5.Name = "zedGraphControl5";
            this.zedGraphControl5.ScrollGrace = 0D;
            this.zedGraphControl5.ScrollMaxX = 0D;
            this.zedGraphControl5.ScrollMaxY = 0D;
            this.zedGraphControl5.ScrollMaxY2 = 0D;
            this.zedGraphControl5.ScrollMinX = 0D;
            this.zedGraphControl5.ScrollMinY = 0D;
            this.zedGraphControl5.ScrollMinY2 = 0D;
            this.zedGraphControl5.Size = new System.Drawing.Size(255, 239);
            this.zedGraphControl5.TabIndex = 4;
            this.zedGraphControl5.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.zedGraphControl_PointValueEvent);
            this.zedGraphControl5.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl1_MouseMoveEvent);
            // 
            // zedGraphControl4
            // 
            this.zedGraphControl4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphControl4.IsShowPointValues = true;
            this.zedGraphControl4.Location = new System.Drawing.Point(3, 247);
            this.zedGraphControl4.Name = "zedGraphControl4";
            this.zedGraphControl4.ScrollGrace = 0D;
            this.zedGraphControl4.ScrollMaxX = 0D;
            this.zedGraphControl4.ScrollMaxY = 0D;
            this.zedGraphControl4.ScrollMaxY2 = 0D;
            this.zedGraphControl4.ScrollMinX = 0D;
            this.zedGraphControl4.ScrollMinY = 0D;
            this.zedGraphControl4.ScrollMinY2 = 0D;
            this.zedGraphControl4.Size = new System.Drawing.Size(255, 239);
            this.zedGraphControl4.TabIndex = 3;
            this.zedGraphControl4.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.zedGraphControl_PointValueEvent);
            this.zedGraphControl4.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl1_MouseMoveEvent);
            // 
            // zedGraphControl3
            // 
            this.zedGraphControl3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphControl3.IsShowPointValues = true;
            this.zedGraphControl3.Location = new System.Drawing.Point(525, 3);
            this.zedGraphControl3.Name = "zedGraphControl3";
            this.zedGraphControl3.ScrollGrace = 0D;
            this.zedGraphControl3.ScrollMaxX = 0D;
            this.zedGraphControl3.ScrollMaxY = 0D;
            this.zedGraphControl3.ScrollMaxY2 = 0D;
            this.zedGraphControl3.ScrollMinX = 0D;
            this.zedGraphControl3.ScrollMinY = 0D;
            this.zedGraphControl3.ScrollMinY2 = 0D;
            this.zedGraphControl3.Size = new System.Drawing.Size(257, 238);
            this.zedGraphControl3.TabIndex = 2;
            this.zedGraphControl3.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.zedGraphControl_PointValueEvent);
            this.zedGraphControl3.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl1_MouseMoveEvent);
            // 
            // zedGraphControl2
            // 
            this.zedGraphControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphControl2.IsShowPointValues = true;
            this.zedGraphControl2.Location = new System.Drawing.Point(264, 3);
            this.zedGraphControl2.Name = "zedGraphControl2";
            this.zedGraphControl2.ScrollGrace = 0D;
            this.zedGraphControl2.ScrollMaxX = 0D;
            this.zedGraphControl2.ScrollMaxY = 0D;
            this.zedGraphControl2.ScrollMaxY2 = 0D;
            this.zedGraphControl2.ScrollMinX = 0D;
            this.zedGraphControl2.ScrollMinY = 0D;
            this.zedGraphControl2.ScrollMinY2 = 0D;
            this.zedGraphControl2.Size = new System.Drawing.Size(255, 238);
            this.zedGraphControl2.TabIndex = 1;
            this.zedGraphControl2.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.zedGraphControl_PointValueEvent);
            this.zedGraphControl2.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl1_MouseMoveEvent);
            // 
            // NUM_bins
            // 
            this.NUM_bins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NUM_bins.Location = new System.Drawing.Point(45, 507);
            this.NUM_bins.Name = "NUM_bins";
            this.NUM_bins.Size = new System.Drawing.Size(42, 20);
            this.NUM_bins.TabIndex = 4;
            this.NUM_bins.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 508);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Bins";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 508);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Start Freq";
            // 
            // NUM_startfreq
            // 
            this.NUM_startfreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NUM_startfreq.Location = new System.Drawing.Point(152, 507);
            this.NUM_startfreq.Name = "NUM_startfreq";
            this.NUM_startfreq.Size = new System.Drawing.Size(42, 20);
            this.NUM_startfreq.TabIndex = 6;
            this.NUM_startfreq.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // chk_mag
            // 
            this.chk_mag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chk_mag.AutoSize = true;
            this.chk_mag.Location = new System.Drawing.Point(200, 510);
            this.chk_mag.Name = "chk_mag";
            this.chk_mag.Size = new System.Drawing.Size(76, 17);
            this.chk_mag.TabIndex = 11;
            this.chk_mag.Text = "Magnitude";
            this.chk_mag.UseVisualStyleBackColor = true;
            this.chk_mag.CheckedChanged += new System.EventHandler(this.chk_mag_CheckedChanged);
            // 
            // but_ISBH
            // 
            this.but_ISBH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.but_ISBH.Location = new System.Drawing.Point(362, 508);
            this.but_ISBH.Name = "but_ISBH";
            this.but_ISBH.Size = new System.Drawing.Size(75, 32);
            this.but_ISBH.TabIndex = 10;
            this.but_ISBH.Text = "IMU Batch Sample";
            this.but_ISBH.UseVisualStyleBackColor = true;
            this.but_ISBH.Click += new System.EventHandler(this.but_ISBH_Click);
            // 
            // but_fftimu13
            // 
            this.but_fftimu13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.but_fftimu13.Location = new System.Drawing.Point(443, 508);
            this.but_fftimu13.Name = "but_fftimu13";
            this.but_fftimu13.Size = new System.Drawing.Size(75, 32);
            this.but_fftimu13.TabIndex = 9;
            this.but_fftimu13.Text = "Run all imus - IMU1-3 MSG";
            this.but_fftimu13.UseVisualStyleBackColor = true;
            this.but_fftimu13.Click += new System.EventHandler(this.but_fftimu13_Click);
            // 
            // BUT_accgyrall
            // 
            this.BUT_accgyrall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BUT_accgyrall.Location = new System.Drawing.Point(524, 508);
            this.BUT_accgyrall.Name = "BUT_accgyrall";
            this.BUT_accgyrall.Size = new System.Drawing.Size(87, 32);
            this.BUT_accgyrall.TabIndex = 8;
            this.BUT_accgyrall.Text = "Run all imus - ACC GYR MSG";
            this.BUT_accgyrall.UseVisualStyleBackColor = true;
            this.BUT_accgyrall.Click += new System.EventHandler(this.BUT_accgyrall_Click);
            // 
            // but_accgyr1
            // 
            this.but_accgyr1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.but_accgyr1.Location = new System.Drawing.Point(617, 507);
            this.but_accgyr1.Name = "but_accgyr1";
            this.but_accgyr1.Size = new System.Drawing.Size(99, 33);
            this.but_accgyr1.TabIndex = 2;
            this.but_accgyr1.Text = "Run Log - imu1 ACC1 GYR1 MSG";
            this.but_accgyr1.UseVisualStyleBackColor = true;
            this.but_accgyr1.Click += new System.EventHandler(this.acc1gyr1myButton1_Click);
            // 
            // BUT_runwav
            // 
            this.BUT_runwav.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BUT_runwav.Location = new System.Drawing.Point(722, 507);
            this.BUT_runwav.Name = "BUT_runwav";
            this.BUT_runwav.Size = new System.Drawing.Size(75, 33);
            this.BUT_runwav.TabIndex = 1;
            this.BUT_runwav.Text = "Run 16bit Mono Wav";
            this.BUT_runwav.UseVisualStyleBackColor = true;
            this.BUT_runwav.Click += new System.EventHandler(this.BUT_runwav_Click);
            // 
            // fftui
            // 
            this.ClientSize = new System.Drawing.Size(809, 542);
            this.Controls.Add(this.chk_mag);
            this.Controls.Add(this.but_ISBH);
            this.Controls.Add(this.but_fftimu13);
            this.Controls.Add(this.BUT_accgyrall);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.NUM_startfreq);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NUM_bins);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.but_accgyr1);
            this.Controls.Add(this.BUT_runwav);
            this.Name = "fftui";
            this.Text = "fftui";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUM_bins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_startfreq)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphControl1;
        private Controls.MyButton BUT_runwav;
        private Controls.MyButton but_accgyr1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ZedGraph.ZedGraphControl zedGraphControl6;
        private ZedGraph.ZedGraphControl zedGraphControl5;
        private ZedGraph.ZedGraphControl zedGraphControl4;
        private ZedGraph.ZedGraphControl zedGraphControl3;
        private ZedGraph.ZedGraphControl zedGraphControl2;
        private System.Windows.Forms.NumericUpDown NUM_bins;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NUM_startfreq;
        private Controls.MyButton BUT_accgyrall;
        private Controls.MyButton but_fftimu13;
        private Controls.MyButton but_ISBH;
        private System.Windows.Forms.CheckBox chk_mag;
    }
}