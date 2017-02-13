namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigEnergyProfile
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 22D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-70D, 15D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint3 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-50D, 20D);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigEnergyProfile));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint4 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 22D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint5 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-70D, 15D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint6 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-50D, 20D);
            this.CB_EnableEnergyProfile = new System.Windows.Forms.CheckBox();
            this.panelCurrentConfiguration = new System.Windows.Forms.Panel();
            this.btnPlotI = new MissionPlanner.Controls.MyButton();
            this.ChartI = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tbLimitI = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.tbDevIPos = new System.Windows.Forms.TextBox();
            this.tbAmpIPos = new System.Windows.Forms.TextBox();
            this.tbAngIPos = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblTitleCurrent = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.DGV_IValues = new System.Windows.Forms.DataGridView();
            this.colAngleI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tbDevINeg = new System.Windows.Forms.TextBox();
            this.tbAmpINeg = new System.Windows.Forms.TextBox();
            this.tbAngINeg = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.DGV_VValues = new System.Windows.Forms.DataGridView();
            this.colAngleV = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVelocity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.panelVelocityConfiguration = new System.Windows.Forms.Panel();
            this.btnPlotV = new MissionPlanner.Controls.MyButton();
            this.chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label15 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.tbLowerLimitV = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.tbDevV = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.tbAmpV = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.tbAngV = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.pictureBoxMouseOver1 = new MissionPlanner.Controls.PictureBoxMouseOver();
            this.panelCurrentConfiguration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartI)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_IValues)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_VValues)).BeginInit();
            this.panelVelocityConfiguration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).BeginInit();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOver1)).BeginInit();
            this.SuspendLayout();
            // 
            // CB_EnableEnergyProfile
            // 
            this.CB_EnableEnergyProfile.AutoSize = true;
            this.CB_EnableEnergyProfile.Location = new System.Drawing.Point(16, 11);
            this.CB_EnableEnergyProfile.Name = "CB_EnableEnergyProfile";
            this.CB_EnableEnergyProfile.Size = new System.Drawing.Size(124, 17);
            this.CB_EnableEnergyProfile.TabIndex = 38;
            this.CB_EnableEnergyProfile.Text = "Enable EnergyProfile";
            this.CB_EnableEnergyProfile.UseVisualStyleBackColor = true;
            this.CB_EnableEnergyProfile.CheckStateChanged += new System.EventHandler(this.CB_EnableEnergyProfile_CheckStateChanged);
            // 
            // panelCurrentConfiguration
            // 
            this.panelCurrentConfiguration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCurrentConfiguration.Controls.Add(this.btnPlotI);
            this.panelCurrentConfiguration.Controls.Add(this.ChartI);
            this.panelCurrentConfiguration.Controls.Add(this.tbLimitI);
            this.panelCurrentConfiguration.Controls.Add(this.label8);
            this.panelCurrentConfiguration.Controls.Add(this.panel5);
            this.panelCurrentConfiguration.Controls.Add(this.lblTitleCurrent);
            this.panelCurrentConfiguration.Controls.Add(this.label5);
            this.panelCurrentConfiguration.Controls.Add(this.DGV_IValues);
            this.panelCurrentConfiguration.Controls.Add(this.panel4);
            this.panelCurrentConfiguration.Enabled = false;
            this.panelCurrentConfiguration.Location = new System.Drawing.Point(16, 113);
            this.panelCurrentConfiguration.Name = "panelCurrentConfiguration";
            this.panelCurrentConfiguration.Size = new System.Drawing.Size(787, 175);
            this.panelCurrentConfiguration.TabIndex = 40;
            // 
            // btnPlotI
            // 
            this.btnPlotI.Location = new System.Drawing.Point(196, 142);
            this.btnPlotI.Name = "btnPlotI";
            this.btnPlotI.Size = new System.Drawing.Size(84, 23);
            this.btnPlotI.TabIndex = 55;
            this.btnPlotI.Text = "Plot Graph";
            this.btnPlotI.UseVisualStyleBackColor = true;
            this.btnPlotI.Click += new System.EventHandler(this.btnPlotI_Click);
            // 
            // ChartI
            // 
            this.ChartI.BackColor = System.Drawing.Color.Transparent;
            this.ChartI.BorderlineColor = System.Drawing.Color.Black;
            this.ChartI.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.AxisX.Interval = 22.5D;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelAutoFitMaxFontSize = 8;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea1.AxisX.Maximum = 90D;
            chartArea1.AxisX.Minimum = -90D;
            chartArea1.AxisX.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            chartArea1.AxisX.Title = "Angle [Degree]";
            chartArea1.AxisX.TitleForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelAutoFitMaxFontSize = 7;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.DimGray;
            chartArea1.AxisY.Title = "Current [A]";
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.White;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "ChartArea1";
            this.ChartI.ChartAreas.Add(chartArea1);
            this.ChartI.Location = new System.Drawing.Point(286, 21);
            this.ChartI.Name = "ChartI";
            series1.BorderWidth = 2;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = System.Drawing.Color.Orange;
            series1.Name = "Series1";
            series1.Points.Add(dataPoint1);
            series1.Points.Add(dataPoint2);
            series1.Points.Add(dataPoint3);
            this.ChartI.Series.Add(series1);
            this.ChartI.Size = new System.Drawing.Size(278, 144);
            this.ChartI.TabIndex = 54;
            this.ChartI.Text = "chart1";
            // 
            // tbLimitI
            // 
            this.tbLimitI.Location = new System.Drawing.Point(116, 145);
            this.tbLimitI.Name = "tbLimitI";
            this.tbLimitI.Size = new System.Drawing.Size(74, 20);
            this.tbLimitI.TabIndex = 52;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 148);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 13);
            this.label8.TabIndex = 52;
            this.label8.Text = "Lower limit of current:";
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.tbDevIPos);
            this.panel5.Controls.Add(this.tbAmpIPos);
            this.panel5.Controls.Add(this.tbAngIPos);
            this.panel5.Controls.Add(this.label10);
            this.panel5.Controls.Add(this.label11);
            this.panel5.Controls.Add(this.label12);
            this.panel5.Controls.Add(this.label13);
            this.panel5.Location = new System.Drawing.Point(147, 21);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(133, 114);
            this.panel5.TabIndex = 53;
            // 
            // tbDevIPos
            // 
            this.tbDevIPos.Location = new System.Drawing.Point(65, 85);
            this.tbDevIPos.Name = "tbDevIPos";
            this.tbDevIPos.Size = new System.Drawing.Size(59, 20);
            this.tbDevIPos.TabIndex = 51;
            // 
            // tbAmpIPos
            // 
            this.tbAmpIPos.Location = new System.Drawing.Point(65, 26);
            this.tbAmpIPos.Name = "tbAmpIPos";
            this.tbAmpIPos.Size = new System.Drawing.Size(59, 20);
            this.tbAmpIPos.TabIndex = 49;
            // 
            // tbAngIPos
            // 
            this.tbAngIPos.Location = new System.Drawing.Point(65, 56);
            this.tbAngIPos.Name = "tbAngIPos";
            this.tbAngIPos.Size = new System.Drawing.Size(59, 20);
            this.tbAngIPos.TabIndex = 50;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 4);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(107, 13);
            this.label10.TabIndex = 42;
            this.label10.Text = "Settings for 0° to 90°:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 52);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 26);
            this.label11.TabIndex = 44;
            this.label11.Text = "Angle of\r\nAmplitude";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 88);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 13);
            this.label12.TabIndex = 48;
            this.label12.Text = "Deviation";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 29);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 13);
            this.label13.TabIndex = 45;
            this.label13.Text = "Amplitude:";
            // 
            // lblTitleCurrent
            // 
            this.lblTitleCurrent.AutoSize = true;
            this.lblTitleCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleCurrent.Location = new System.Drawing.Point(3, 3);
            this.lblTitleCurrent.Name = "lblTitleCurrent";
            this.lblTitleCurrent.Size = new System.Drawing.Size(166, 13);
            this.lblTitleCurrent.TabIndex = 47;
            this.lblTitleCurrent.Text = "Settings for current (I in [A])";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(567, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "Estimated Values for I:";
            // 
            // DGV_IValues
            // 
            this.DGV_IValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_IValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAngleI,
            this.colCurrent});
            this.DGV_IValues.Location = new System.Drawing.Point(570, 21);
            this.DGV_IValues.MultiSelect = false;
            this.DGV_IValues.Name = "DGV_IValues";
            this.DGV_IValues.ReadOnly = true;
            this.DGV_IValues.RowHeadersVisible = false;
            this.DGV_IValues.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DGV_IValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV_IValues.Size = new System.Drawing.Size(203, 144);
            this.DGV_IValues.TabIndex = 38;
            // 
            // colAngleI
            // 
            this.colAngleI.HeaderText = "Angle";
            this.colAngleI.Name = "colAngleI";
            this.colAngleI.ReadOnly = true;
            // 
            // colCurrent
            // 
            this.colCurrent.HeaderText = "Current [A]";
            this.colCurrent.Name = "colCurrent";
            this.colCurrent.ReadOnly = true;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.tbDevINeg);
            this.panel4.Controls.Add(this.tbAmpINeg);
            this.panel4.Controls.Add(this.tbAngINeg);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Location = new System.Drawing.Point(6, 21);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(135, 114);
            this.panel4.TabIndex = 52;
            // 
            // tbDevINeg
            // 
            this.tbDevINeg.Location = new System.Drawing.Point(65, 85);
            this.tbDevINeg.Name = "tbDevINeg";
            this.tbDevINeg.Size = new System.Drawing.Size(59, 20);
            this.tbDevINeg.TabIndex = 51;
            // 
            // tbAmpINeg
            // 
            this.tbAmpINeg.Location = new System.Drawing.Point(65, 26);
            this.tbAmpINeg.Name = "tbAmpINeg";
            this.tbAmpINeg.Size = new System.Drawing.Size(59, 20);
            this.tbAmpINeg.TabIndex = 49;
            // 
            // tbAngINeg
            // 
            this.tbAngINeg.Location = new System.Drawing.Point(65, 56);
            this.tbAngINeg.Name = "tbAngINeg";
            this.tbAngINeg.Size = new System.Drawing.Size(59, 20);
            this.tbAngINeg.TabIndex = 50;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "Settings for -90° to 0°:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 26);
            this.label6.TabIndex = 44;
            this.label6.Text = "Angle of\r\nAmplitude";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 48;
            this.label9.Text = "Deviation";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 45;
            this.label7.Text = "Amplitude:";
            // 
            // DGV_VValues
            // 
            this.DGV_VValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_VValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAngleV,
            this.colVelocity});
            this.DGV_VValues.Location = new System.Drawing.Point(570, 20);
            this.DGV_VValues.Name = "DGV_VValues";
            this.DGV_VValues.ReadOnly = true;
            this.DGV_VValues.RowHeadersVisible = false;
            this.DGV_VValues.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DGV_VValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV_VValues.Size = new System.Drawing.Size(203, 139);
            this.DGV_VValues.TabIndex = 40;
            // 
            // colAngleV
            // 
            this.colAngleV.HeaderText = "Angle";
            this.colAngleV.Name = "colAngleV";
            this.colAngleV.ReadOnly = true;
            this.colAngleV.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // colVelocity
            // 
            this.colVelocity.HeaderText = "Velocity [m/s]";
            this.colVelocity.Name = "colVelocity";
            this.colVelocity.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(495, 26);
            this.label1.TabIndex = 41;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // panelVelocityConfiguration
            // 
            this.panelVelocityConfiguration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelVelocityConfiguration.Controls.Add(this.btnPlotV);
            this.panelVelocityConfiguration.Controls.Add(this.chart3);
            this.panelVelocityConfiguration.Controls.Add(this.label15);
            this.panelVelocityConfiguration.Controls.Add(this.label19);
            this.panelVelocityConfiguration.Controls.Add(this.tbLowerLimitV);
            this.panelVelocityConfiguration.Controls.Add(this.label14);
            this.panelVelocityConfiguration.Controls.Add(this.panel7);
            this.panelVelocityConfiguration.Controls.Add(this.DGV_VValues);
            this.panelVelocityConfiguration.Location = new System.Drawing.Point(16, 300);
            this.panelVelocityConfiguration.Name = "panelVelocityConfiguration";
            this.panelVelocityConfiguration.Size = new System.Drawing.Size(787, 175);
            this.panelVelocityConfiguration.TabIndex = 43;
            // 
            // btnPlotV
            // 
            this.btnPlotV.Location = new System.Drawing.Point(196, 136);
            this.btnPlotV.Name = "btnPlotV";
            this.btnPlotV.Size = new System.Drawing.Size(84, 23);
            this.btnPlotV.TabIndex = 56;
            this.btnPlotV.Text = "Plot Graph";
            this.btnPlotV.UseVisualStyleBackColor = true;
            // 
            // chart3
            // 
            this.chart3.BackColor = System.Drawing.Color.Transparent;
            this.chart3.BorderlineColor = System.Drawing.Color.Black;
            this.chart3.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea2.AxisX.Interval = 22.5D;
            chartArea2.AxisX.IsLabelAutoFit = false;
            chartArea2.AxisX.LabelAutoFitMaxFontSize = 8;
            chartArea2.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            chartArea2.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea2.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea2.AxisX.Maximum = 90D;
            chartArea2.AxisX.Minimum = -90D;
            chartArea2.AxisX.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            chartArea2.AxisX.Title = "Angle [Degree]";
            chartArea2.AxisX.TitleForeColor = System.Drawing.Color.White;
            chartArea2.AxisY.IsLabelAutoFit = false;
            chartArea2.AxisY.LabelAutoFitMaxFontSize = 7;
            chartArea2.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            chartArea2.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.DimGray;
            chartArea2.AxisY.Title = "Velocity [m/s]";
            chartArea2.AxisY.TitleForeColor = System.Drawing.Color.White;
            chartArea2.BackColor = System.Drawing.Color.Transparent;
            chartArea2.Name = "ChartArea1";
            this.chart3.ChartAreas.Add(chartArea2);
            this.chart3.Location = new System.Drawing.Point(286, 20);
            this.chart3.Name = "chart3";
            series2.BorderWidth = 2;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.Orange;
            series2.Name = "Series1";
            series2.Points.Add(dataPoint4);
            series2.Points.Add(dataPoint5);
            series2.Points.Add(dataPoint6);
            this.chart3.Series.Add(series2);
            this.chart3.Size = new System.Drawing.Size(278, 139);
            this.chart3.TabIndex = 58;
            this.chart3.Text = "chart3";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(567, 4);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(116, 13);
            this.label15.TabIndex = 54;
            this.label15.Text = "Estimated Values for V:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(4, 4);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(187, 13);
            this.label19.TabIndex = 54;
            this.label19.Text = "Settings for velocity (V in [m/s])";
            // 
            // tbLowerLimitV
            // 
            this.tbLowerLimitV.Location = new System.Drawing.Point(116, 138);
            this.tbLowerLimitV.Name = "tbLowerLimitV";
            this.tbLowerLimitV.Size = new System.Drawing.Size(74, 20);
            this.tbLowerLimitV.TabIndex = 54;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 141);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(110, 13);
            this.label14.TabIndex = 55;
            this.label14.Text = "Lower limit of velocity:";
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.textBox9);
            this.panel7.Controls.Add(this.tbDevV);
            this.panel7.Controls.Add(this.textBox10);
            this.panel7.Controls.Add(this.tbAmpV);
            this.panel7.Controls.Add(this.textBox11);
            this.panel7.Controls.Add(this.tbAngV);
            this.panel7.Controls.Add(this.label16);
            this.panel7.Controls.Add(this.label20);
            this.panel7.Controls.Add(this.label17);
            this.panel7.Controls.Add(this.label21);
            this.panel7.Controls.Add(this.label18);
            this.panel7.Controls.Add(this.label22);
            this.panel7.Location = new System.Drawing.Point(6, 20);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(274, 110);
            this.panel7.TabIndex = 56;
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(199, 70);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(59, 20);
            this.textBox9.TabIndex = 51;
            // 
            // tbDevV
            // 
            this.tbDevV.Location = new System.Drawing.Point(65, 70);
            this.tbDevV.Name = "tbDevV";
            this.tbDevV.Size = new System.Drawing.Size(59, 20);
            this.tbDevV.TabIndex = 51;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(199, 11);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(59, 20);
            this.textBox10.TabIndex = 49;
            // 
            // tbAmpV
            // 
            this.tbAmpV.Location = new System.Drawing.Point(65, 11);
            this.tbAmpV.Name = "tbAmpV";
            this.tbAmpV.Size = new System.Drawing.Size(59, 20);
            this.tbAmpV.TabIndex = 49;
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(199, 41);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(59, 20);
            this.textBox11.TabIndex = 50;
            // 
            // tbAngV
            // 
            this.tbAngV.Location = new System.Drawing.Point(65, 41);
            this.tbAngV.Name = "tbAngV";
            this.tbAngV.Size = new System.Drawing.Size(59, 20);
            this.tbAngV.TabIndex = 50;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(137, 37);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 26);
            this.label16.TabIndex = 44;
            this.label16.Text = "Angle of\r\nAmplitude";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 37);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(53, 26);
            this.label20.TabIndex = 44;
            this.label20.Text = "Angle of\r\nAmplitude";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(137, 73);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(52, 13);
            this.label17.TabIndex = 48;
            this.label17.Text = "Deviation";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 73);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(52, 13);
            this.label21.TabIndex = 48;
            this.label21.Text = "Deviation";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(137, 14);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(56, 13);
            this.label18.TabIndex = 45;
            this.label18.Text = "Amplitude:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 14);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(56, 13);
            this.label22.TabIndex = 45;
            this.label22.Text = "Amplitude:";
            // 
            // pictureBoxMouseOver1
            // 
            this.pictureBoxMouseOver1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxMouseOver1.Image")));
            this.pictureBoxMouseOver1.ImageNormal = null;
            this.pictureBoxMouseOver1.ImageOver = null;
            this.pictureBoxMouseOver1.Location = new System.Drawing.Point(608, 3);
            this.pictureBoxMouseOver1.Name = "pictureBoxMouseOver1";
            this.pictureBoxMouseOver1.selected = false;
            this.pictureBoxMouseOver1.Size = new System.Drawing.Size(195, 104);
            this.pictureBoxMouseOver1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMouseOver1.TabIndex = 42;
            this.pictureBoxMouseOver1.TabStop = false;
            // 
            // ConfigEnergyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panelVelocityConfiguration);
            this.Controls.Add(this.pictureBoxMouseOver1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelCurrentConfiguration);
            this.Controls.Add(this.CB_EnableEnergyProfile);
            this.Name = "ConfigEnergyProfile";
            this.Size = new System.Drawing.Size(824, 532);
            this.panelCurrentConfiguration.ResumeLayout(false);
            this.panelCurrentConfiguration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartI)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_IValues)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_VValues)).EndInit();
            this.panelVelocityConfiguration.ResumeLayout(false);
            this.panelVelocityConfiguration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMouseOver1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox CB_EnableEnergyProfile;
        private System.Windows.Forms.Panel panelCurrentConfiguration;
        private System.Windows.Forms.DataGridView DGV_IValues;
        private System.Windows.Forms.DataGridView DGV_VValues;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAngleV;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVelocity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAngleI;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurrent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private Controls.PictureBoxMouseOver pictureBoxMouseOver1;
        private System.Windows.Forms.Panel panelVelocityConfiguration;
        private System.Windows.Forms.Label lblTitleCurrent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbAngINeg;
        private System.Windows.Forms.TextBox tbAmpINeg;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox tbDevINeg;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox tbDevIPos;
        private System.Windows.Forms.TextBox tbAmpIPos;
        private System.Windows.Forms.TextBox tbAngIPos;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbLimitI;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tbLowerLimitV;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.TextBox tbDevV;
        private System.Windows.Forms.TextBox tbAmpV;
        private System.Windows.Forms.TextBox tbAngV;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartI;
        private Controls.MyButton btnPlotI;
        private Controls.MyButton btnPlotV;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart3;
    }
}
