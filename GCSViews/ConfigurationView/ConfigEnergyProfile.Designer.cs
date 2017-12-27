using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Windows.Forms;
using DotSpatial.Data;
using MissionPlanner.Utilities;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigEnergyProfile));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint9 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, "22,18");
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint10 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 22D);
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint11 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 18D);
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint12 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 22D);
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series13 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint13 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, "22,18");
            System.Windows.Forms.DataVisualization.Charting.Series series14 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint14 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 22D);
            System.Windows.Forms.DataVisualization.Charting.Series series15 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint15 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 18D);
            System.Windows.Forms.DataVisualization.Charting.Series series16 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint16 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 22D);
            this.CB_EnableEnergyProfile = new System.Windows.Forms.CheckBox();
            this.panelCurrentConfiguration = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.CB_Interp_Curr = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.LabelDeviationCB = new System.Windows.Forms.Label();
            this.ComboBoxCrntDeviation = new System.Windows.Forms.ComboBox();
            this.BtnPlotCrnt = new MissionPlanner.Controls.MyButton();
            this.LbInfoCurrentMove = new System.Windows.Forms.Label();
            this.CrntTable = new System.Windows.Forms.TableLayoutPanel();
            this.CrntTblTb411 = new System.Windows.Forms.TextBox();
            this.CrntTblTb410 = new System.Windows.Forms.TextBox();
            this.CrntTblTb409 = new System.Windows.Forms.TextBox();
            this.CrntTblTb408 = new System.Windows.Forms.TextBox();
            this.CrntTblTb407 = new System.Windows.Forms.TextBox();
            this.CrntTblTb406 = new System.Windows.Forms.TextBox();
            this.CrntTblTb405 = new System.Windows.Forms.TextBox();
            this.CrntTblTb404 = new System.Windows.Forms.TextBox();
            this.CrntTblTb403 = new System.Windows.Forms.TextBox();
            this.CrntTblTb402 = new System.Windows.Forms.TextBox();
            this.CrntTblTb401 = new System.Windows.Forms.TextBox();
            this.CrntTblTb311 = new System.Windows.Forms.TextBox();
            this.CrntTblTb310 = new System.Windows.Forms.TextBox();
            this.CrntTblTb309 = new System.Windows.Forms.TextBox();
            this.CrntTblTb308 = new System.Windows.Forms.TextBox();
            this.CrntTblTb307 = new System.Windows.Forms.TextBox();
            this.CrntTblTb306 = new System.Windows.Forms.TextBox();
            this.CrntTblTb305 = new System.Windows.Forms.TextBox();
            this.CrntTblTb304 = new System.Windows.Forms.TextBox();
            this.CrntTblTb303 = new System.Windows.Forms.TextBox();
            this.CrntTblTb302 = new System.Windows.Forms.TextBox();
            this.CrntTblTb301 = new System.Windows.Forms.TextBox();
            this.CrntTblTb211 = new System.Windows.Forms.TextBox();
            this.CrntTblTb210 = new System.Windows.Forms.TextBox();
            this.CrntTblTb209 = new System.Windows.Forms.TextBox();
            this.CrntTblTb208 = new System.Windows.Forms.TextBox();
            this.CrntTblTb207 = new System.Windows.Forms.TextBox();
            this.CrntTblTb206 = new System.Windows.Forms.TextBox();
            this.CrntTblTb205 = new System.Windows.Forms.TextBox();
            this.CrntTblTb204 = new System.Windows.Forms.TextBox();
            this.CrntTblTb203 = new System.Windows.Forms.TextBox();
            this.CrntTblTb202 = new System.Windows.Forms.TextBox();
            this.CrntTblRowLbl02 = new System.Windows.Forms.Label();
            this.CrntTblRowLbl01 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl01 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl02 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl03 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl04 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl05 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl06 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl07 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl08 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl09 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl10 = new System.Windows.Forms.Label();
            this.CrntTblClmLbl11 = new System.Windows.Forms.Label();
            this.CrntTblRowLbl03 = new System.Windows.Forms.Label();
            this.CrntTblRowLbl04 = new System.Windows.Forms.Label();
            this.CrntTblTb201 = new System.Windows.Forms.TextBox();
            this.ChartI = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lbCurrentInfo2 = new System.Windows.Forms.Label();
            this.lblTitleCurrent = new System.Windows.Forms.Label();
            this.panelHover = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.HoverDevTB = new System.Windows.Forms.TextBox();
            this.lbCurrentHover = new System.Windows.Forms.Label();
            this.lbVarianceCurrentHover = new System.Windows.Forms.Label();
            this.HoverCrntTB = new System.Windows.Forms.TextBox();
            this.lbCurrentInfo1 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbCopterID = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.panelExpImp = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnImport = new MissionPlanner.Controls.MyButton();
            this.BtnExport = new MissionPlanner.Controls.MyButton();
            this.label29 = new System.Windows.Forms.Label();
            this.panelVelocityConfiguration = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.CB_Interp_Vel = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ComboBoxVelDeviation = new System.Windows.Forms.ComboBox();
            this.BtnPlotVelocity = new MissionPlanner.Controls.MyButton();
            this.LbVelocityInfo = new System.Windows.Forms.Label();
            this.VelocityTable = new System.Windows.Forms.TableLayoutPanel();
            this.VelTblTb411 = new System.Windows.Forms.TextBox();
            this.VelTblTb410 = new System.Windows.Forms.TextBox();
            this.VelTblTb409 = new System.Windows.Forms.TextBox();
            this.VelTblTb408 = new System.Windows.Forms.TextBox();
            this.VelTblTb407 = new System.Windows.Forms.TextBox();
            this.VelTblTb406 = new System.Windows.Forms.TextBox();
            this.VelTblTb405 = new System.Windows.Forms.TextBox();
            this.VelTblTb404 = new System.Windows.Forms.TextBox();
            this.VelTblTb403 = new System.Windows.Forms.TextBox();
            this.VelTblTb402 = new System.Windows.Forms.TextBox();
            this.VelTblTb401 = new System.Windows.Forms.TextBox();
            this.VelTblTb311 = new System.Windows.Forms.TextBox();
            this.VelTblTb310 = new System.Windows.Forms.TextBox();
            this.VelTblTb309 = new System.Windows.Forms.TextBox();
            this.VelTblTb308 = new System.Windows.Forms.TextBox();
            this.VelTblTb307 = new System.Windows.Forms.TextBox();
            this.VelTblTb306 = new System.Windows.Forms.TextBox();
            this.VelTblTb305 = new System.Windows.Forms.TextBox();
            this.VelTblTb304 = new System.Windows.Forms.TextBox();
            this.VelTblTb303 = new System.Windows.Forms.TextBox();
            this.VelTblTb302 = new System.Windows.Forms.TextBox();
            this.VelTblTb301 = new System.Windows.Forms.TextBox();
            this.VelTblTb211 = new System.Windows.Forms.TextBox();
            this.VelTblTb210 = new System.Windows.Forms.TextBox();
            this.VelTblTb209 = new System.Windows.Forms.TextBox();
            this.VelTblTb208 = new System.Windows.Forms.TextBox();
            this.VelTblTb207 = new System.Windows.Forms.TextBox();
            this.VelTblTb206 = new System.Windows.Forms.TextBox();
            this.VelTblTb205 = new System.Windows.Forms.TextBox();
            this.VelTblTb204 = new System.Windows.Forms.TextBox();
            this.VelTblTb203 = new System.Windows.Forms.TextBox();
            this.VelTblTb202 = new System.Windows.Forms.TextBox();
            this.VelTblRowLbl02 = new System.Windows.Forms.Label();
            this.VelTblRowLbl01 = new System.Windows.Forms.Label();
            this.VelTblClmLbl01 = new System.Windows.Forms.Label();
            this.VelTblClmLbl02 = new System.Windows.Forms.Label();
            this.VelTblClmLbl03 = new System.Windows.Forms.Label();
            this.VelTblClmLbl04 = new System.Windows.Forms.Label();
            this.VelTblClmLbl05 = new System.Windows.Forms.Label();
            this.VelTblClmLbl06 = new System.Windows.Forms.Label();
            this.VelTblClmLbl08 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.VelTblClmLbl09 = new System.Windows.Forms.Label();
            this.VelTblClmLbl10 = new System.Windows.Forms.Label();
            this.VelTblClmLbl11 = new System.Windows.Forms.Label();
            this.VelTblRowLbl03 = new System.Windows.Forms.Label();
            this.VelTblRowLbl04 = new System.Windows.Forms.Label();
            this.VelTblTb201 = new System.Windows.Forms.TextBox();
            this.ChartV = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label19 = new System.Windows.Forms.Label();
            this.LbTitleVelocity = new System.Windows.Forms.Label();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.Btn_Analyze = new MissionPlanner.Controls.MyButton();
            this.Btn_LoadLogfile = new MissionPlanner.Controls.MyButton();
            this.Btn_DeleteLogfile = new MissionPlanner.Controls.MyButton();
            this.lb_transtime = new System.Windows.Forms.Label();
            this.lb_minvalues = new System.Windows.Forms.Label();
            this.lbl_cmdflighttime = new System.Windows.Forms.Label();
            this.ChB_SpeedTransition = new System.Windows.Forms.CheckBox();
            this.ChB_CurrentTransition = new System.Windows.Forms.CheckBox();
            this.Panel_LogAnalyzer = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tb_cmdflighttime = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tb_transtime = new System.Windows.Forms.TextBox();
            this.tb_minval = new System.Windows.Forms.TextBox();
            this.Lb_LogAnalyzer = new System.Windows.Forms.ListBox();
            this.lb_Infotext_LogfileAnalyzer = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.Lbl_Head_LogfileAnalyzer = new System.Windows.Forms.Label();
            this.panelCurrentConfiguration.SuspendLayout();
            this.CrntTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartI)).BeginInit();
            this.panelHover.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelExpImp.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelVelocityConfiguration.SuspendLayout();
            this.VelocityTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartV)).BeginInit();
            this.Panel_LogAnalyzer.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // CB_EnableEnergyProfile
            // 
            this.CB_EnableEnergyProfile.AutoSize = true;
            this.CB_EnableEnergyProfile.Location = new System.Drawing.Point(16, 3);
            this.CB_EnableEnergyProfile.Name = "CB_EnableEnergyProfile";
            this.CB_EnableEnergyProfile.Size = new System.Drawing.Size(191, 17);
            this.CB_EnableEnergyProfile.TabIndex = 38;
            this.CB_EnableEnergyProfile.Text = "Enable Energy Consumption Profile";
            this.CB_EnableEnergyProfile.UseVisualStyleBackColor = true;
            this.CB_EnableEnergyProfile.CheckStateChanged += new System.EventHandler(this.CB_EnableEnergyProfile_CheckStateChanged);
            // 
            // panelCurrentConfiguration
            // 
            this.panelCurrentConfiguration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCurrentConfiguration.Controls.Add(this.label3);
            this.panelCurrentConfiguration.Controls.Add(this.CB_Interp_Curr);
            this.panelCurrentConfiguration.Controls.Add(this.checkBox1);
            this.panelCurrentConfiguration.Controls.Add(this.LabelDeviationCB);
            this.panelCurrentConfiguration.Controls.Add(this.ComboBoxCrntDeviation);
            this.panelCurrentConfiguration.Controls.Add(this.BtnPlotCrnt);
            this.panelCurrentConfiguration.Controls.Add(this.LbInfoCurrentMove);
            this.panelCurrentConfiguration.Controls.Add(this.CrntTable);
            this.panelCurrentConfiguration.Controls.Add(this.ChartI);
            this.panelCurrentConfiguration.Controls.Add(this.lbCurrentInfo2);
            this.panelCurrentConfiguration.Controls.Add(this.lblTitleCurrent);
            this.panelCurrentConfiguration.Enabled = false;
            this.panelCurrentConfiguration.Location = new System.Drawing.Point(17, 141);
            this.panelCurrentConfiguration.Name = "panelCurrentConfiguration";
            this.panelCurrentConfiguration.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.panelCurrentConfiguration.Size = new System.Drawing.Size(818, 505);
            this.panelCurrentConfiguration.TabIndex = 40;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(653, 237);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 13);
            this.label3.TabIndex = 71;
            this.label3.Text = "Interpolation Mode [%]:";
            // 
            // CB_Interp_Curr
            // 
            this.CB_Interp_Curr.DisplayMember = "0";
            this.CB_Interp_Curr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Interp_Curr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_Interp_Curr.Items.AddRange(new object[] {
            "Linear",
            "Cubic Spline"});
            this.CB_Interp_Curr.Location = new System.Drawing.Point(656, 263);
            this.CB_Interp_Curr.MaxDropDownItems = 5;
            this.CB_Interp_Curr.Name = "CB_Interp_Curr";
            this.CB_Interp_Curr.Size = new System.Drawing.Size(135, 21);
            this.CB_Interp_Curr.TabIndex = 70;
            this.ToolTip.SetToolTip(this.CB_Interp_Curr, "Set a fix deviation for all current values.");
            this.CB_Interp_Curr.SelectedIndexChanged += new System.EventHandler(this.CB_Interp_Curr_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(656, 208);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(109, 17);
            this.checkBox1.TabIndex = 69;
            this.checkBox1.Text = "Flexible Deviation";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            // 
            // LabelDeviationCB
            // 
            this.LabelDeviationCB.AutoSize = true;
            this.LabelDeviationCB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelDeviationCB.Location = new System.Drawing.Point(653, 144);
            this.LabelDeviationCB.Name = "LabelDeviationCB";
            this.LabelDeviationCB.Size = new System.Drawing.Size(141, 13);
            this.LabelDeviationCB.TabIndex = 68;
            this.LabelDeviationCB.Text = "Expected deviation [%]:";
            // 
            // ComboBoxCrntDeviation
            // 
            this.ComboBoxCrntDeviation.DisplayMember = "0";
            this.ComboBoxCrntDeviation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxCrntDeviation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboBoxCrntDeviation.Location = new System.Drawing.Point(656, 169);
            this.ComboBoxCrntDeviation.MaxDropDownItems = 5;
            this.ComboBoxCrntDeviation.Name = "ComboBoxCrntDeviation";
            this.ComboBoxCrntDeviation.Size = new System.Drawing.Size(49, 21);
            this.ComboBoxCrntDeviation.TabIndex = 67;
            this.ToolTip.SetToolTip(this.ComboBoxCrntDeviation, "Set a fix deviation for all current values.");
            this.ComboBoxCrntDeviation.SelectedIndexChanged += new System.EventHandler(this.ComboBoxDeviation_SelectedIndexChanged);
            // 
            // BtnPlotCrnt
            // 
            this.BtnPlotCrnt.Location = new System.Drawing.Point(722, 310);
            this.BtnPlotCrnt.Name = "BtnPlotCrnt";
            this.BtnPlotCrnt.Size = new System.Drawing.Size(84, 26);
            this.BtnPlotCrnt.TabIndex = 52;
            this.BtnPlotCrnt.Text = "Plot";
            this.ToolTip.SetToolTip(this.BtnPlotCrnt, "Plot the energy consumption during flight.");
            this.BtnPlotCrnt.UseVisualStyleBackColor = true;
            this.BtnPlotCrnt.Click += new System.EventHandler(this.BtnCrntPlot_Click);
            // 
            // LbInfoCurrentMove
            // 
            this.LbInfoCurrentMove.AutoSize = true;
            this.LbInfoCurrentMove.Location = new System.Drawing.Point(309, 3);
            this.LbInfoCurrentMove.Name = "LbInfoCurrentMove";
            this.LbInfoCurrentMove.Size = new System.Drawing.Size(505, 39);
            this.LbInfoCurrentMove.TabIndex = 66;
            this.LbInfoCurrentMove.Text = resources.GetString("LbInfoCurrentMove.Text");
            // 
            // CrntTable
            // 
            this.CrntTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.CrntTable.ColumnCount = 12;
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.CrntTable.Controls.Add(this.CrntTblTb411, 11, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb410, 10, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb409, 9, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb408, 8, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb407, 7, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb406, 6, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb405, 5, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb404, 4, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb403, 3, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb402, 2, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb401, 1, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb311, 11, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb310, 10, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb309, 9, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb308, 8, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb307, 7, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb306, 6, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb305, 5, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb304, 4, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb303, 3, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb302, 2, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb301, 1, 2);
            this.CrntTable.Controls.Add(this.CrntTblTb211, 11, 1);
            this.CrntTable.Controls.Add(this.CrntTblTb210, 10, 1);
            this.CrntTable.Controls.Add(this.CrntTblTb209, 9, 1);
            this.CrntTable.Controls.Add(this.CrntTblTb208, 8, 1);
            this.CrntTable.Controls.Add(this.CrntTblTb207, 7, 1);
            this.CrntTable.Controls.Add(this.CrntTblTb206, 6, 1);
            this.CrntTable.Controls.Add(this.CrntTblTb205, 5, 1);
            this.CrntTable.Controls.Add(this.CrntTblTb204, 4, 1);
            this.CrntTable.Controls.Add(this.CrntTblTb203, 3, 1);
            this.CrntTable.Controls.Add(this.CrntTblTb202, 2, 1);
            this.CrntTable.Controls.Add(this.CrntTblRowLbl02, 0, 1);
            this.CrntTable.Controls.Add(this.CrntTblRowLbl01, 0, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl01, 1, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl02, 2, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl03, 3, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl04, 4, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl05, 5, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl06, 6, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl07, 7, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl08, 8, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl09, 9, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl10, 10, 0);
            this.CrntTable.Controls.Add(this.CrntTblClmLbl11, 11, 0);
            this.CrntTable.Controls.Add(this.CrntTblRowLbl03, 0, 2);
            this.CrntTable.Controls.Add(this.CrntTblRowLbl04, 0, 3);
            this.CrntTable.Controls.Add(this.CrntTblTb201, 1, 1);
            this.CrntTable.Location = new System.Drawing.Point(3, 351);
            this.CrntTable.Name = "CrntTable";
            this.CrntTable.RowCount = 4;
            this.CrntTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.CrntTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.CrntTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.CrntTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.CrntTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.CrntTable.Size = new System.Drawing.Size(810, 146);
            this.CrntTable.TabIndex = 64;
            // 
            // CrntTblTb411
            // 
            this.CrntTblTb411.Enabled = false;
            this.CrntTblTb411.Location = new System.Drawing.Point(746, 117);
            this.CrntTblTb411.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb411.Name = "CrntTblTb411";
            this.CrntTblTb411.ReadOnly = true;
            this.CrntTblTb411.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb411.TabIndex = 100;
            this.CrntTblTb411.Text = "0,00";
            // 
            // CrntTblTb410
            // 
            this.CrntTblTb410.Enabled = false;
            this.CrntTblTb410.Location = new System.Drawing.Point(679, 117);
            this.CrntTblTb410.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb410.Name = "CrntTblTb410";
            this.CrntTblTb410.ReadOnly = true;
            this.CrntTblTb410.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb410.TabIndex = 99;
            this.CrntTblTb410.Text = "0,00";
            // 
            // CrntTblTb409
            // 
            this.CrntTblTb409.Enabled = false;
            this.CrntTblTb409.Location = new System.Drawing.Point(612, 117);
            this.CrntTblTb409.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb409.Name = "CrntTblTb409";
            this.CrntTblTb409.ReadOnly = true;
            this.CrntTblTb409.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb409.TabIndex = 98;
            this.CrntTblTb409.Text = "0,00";
            // 
            // CrntTblTb408
            // 
            this.CrntTblTb408.Enabled = false;
            this.CrntTblTb408.Location = new System.Drawing.Point(545, 117);
            this.CrntTblTb408.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb408.Name = "CrntTblTb408";
            this.CrntTblTb408.ReadOnly = true;
            this.CrntTblTb408.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb408.TabIndex = 97;
            this.CrntTblTb408.Text = "0,00";
            // 
            // CrntTblTb407
            // 
            this.CrntTblTb407.Enabled = false;
            this.CrntTblTb407.Location = new System.Drawing.Point(478, 117);
            this.CrntTblTb407.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb407.Name = "CrntTblTb407";
            this.CrntTblTb407.ReadOnly = true;
            this.CrntTblTb407.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb407.TabIndex = 96;
            this.CrntTblTb407.Text = "0,00";
            // 
            // CrntTblTb406
            // 
            this.CrntTblTb406.Enabled = false;
            this.CrntTblTb406.Location = new System.Drawing.Point(411, 117);
            this.CrntTblTb406.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb406.Name = "CrntTblTb406";
            this.CrntTblTb406.ReadOnly = true;
            this.CrntTblTb406.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb406.TabIndex = 95;
            this.CrntTblTb406.Text = "0,00";
            // 
            // CrntTblTb405
            // 
            this.CrntTblTb405.Enabled = false;
            this.CrntTblTb405.Location = new System.Drawing.Point(344, 117);
            this.CrntTblTb405.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb405.Name = "CrntTblTb405";
            this.CrntTblTb405.ReadOnly = true;
            this.CrntTblTb405.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb405.TabIndex = 94;
            this.CrntTblTb405.Text = "0,00";
            // 
            // CrntTblTb404
            // 
            this.CrntTblTb404.Enabled = false;
            this.CrntTblTb404.Location = new System.Drawing.Point(277, 117);
            this.CrntTblTb404.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb404.Name = "CrntTblTb404";
            this.CrntTblTb404.ReadOnly = true;
            this.CrntTblTb404.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb404.TabIndex = 93;
            this.CrntTblTb404.Text = "0,00";
            // 
            // CrntTblTb403
            // 
            this.CrntTblTb403.Enabled = false;
            this.CrntTblTb403.Location = new System.Drawing.Point(210, 117);
            this.CrntTblTb403.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb403.Name = "CrntTblTb403";
            this.CrntTblTb403.ReadOnly = true;
            this.CrntTblTb403.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb403.TabIndex = 92;
            this.CrntTblTb403.Text = "0,00";
            // 
            // CrntTblTb402
            // 
            this.CrntTblTb402.Enabled = false;
            this.CrntTblTb402.Location = new System.Drawing.Point(143, 117);
            this.CrntTblTb402.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb402.Name = "CrntTblTb402";
            this.CrntTblTb402.ReadOnly = true;
            this.CrntTblTb402.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb402.TabIndex = 91;
            this.CrntTblTb402.Text = "0,00";
            // 
            // CrntTblTb401
            // 
            this.CrntTblTb401.Enabled = false;
            this.CrntTblTb401.Location = new System.Drawing.Point(76, 117);
            this.CrntTblTb401.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb401.Name = "CrntTblTb401";
            this.CrntTblTb401.ReadOnly = true;
            this.CrntTblTb401.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb401.TabIndex = 90;
            this.CrntTblTb401.Text = "0,00";
            // 
            // CrntTblTb311
            // 
            this.CrntTblTb311.Location = new System.Drawing.Point(746, 81);
            this.CrntTblTb311.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb311.Name = "CrntTblTb311";
            this.CrntTblTb311.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb311.TabIndex = 89;
            this.CrntTblTb311.Text = "0,00";
            // 
            // CrntTblTb310
            // 
            this.CrntTblTb310.Location = new System.Drawing.Point(679, 81);
            this.CrntTblTb310.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb310.Name = "CrntTblTb310";
            this.CrntTblTb310.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb310.TabIndex = 88;
            this.CrntTblTb310.Text = "0,00";
            // 
            // CrntTblTb309
            // 
            this.CrntTblTb309.Location = new System.Drawing.Point(612, 81);
            this.CrntTblTb309.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb309.Name = "CrntTblTb309";
            this.CrntTblTb309.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb309.TabIndex = 87;
            this.CrntTblTb309.Text = "0,00";
            // 
            // CrntTblTb308
            // 
            this.CrntTblTb308.Location = new System.Drawing.Point(545, 81);
            this.CrntTblTb308.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb308.Name = "CrntTblTb308";
            this.CrntTblTb308.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb308.TabIndex = 86;
            this.CrntTblTb308.Text = "0,00";
            // 
            // CrntTblTb307
            // 
            this.CrntTblTb307.Location = new System.Drawing.Point(478, 81);
            this.CrntTblTb307.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb307.Name = "CrntTblTb307";
            this.CrntTblTb307.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb307.TabIndex = 85;
            this.CrntTblTb307.Text = "0,00";
            // 
            // CrntTblTb306
            // 
            this.CrntTblTb306.Location = new System.Drawing.Point(411, 81);
            this.CrntTblTb306.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb306.Name = "CrntTblTb306";
            this.CrntTblTb306.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb306.TabIndex = 84;
            this.CrntTblTb306.Text = "0,00";
            // 
            // CrntTblTb305
            // 
            this.CrntTblTb305.Location = new System.Drawing.Point(344, 81);
            this.CrntTblTb305.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb305.Name = "CrntTblTb305";
            this.CrntTblTb305.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb305.TabIndex = 83;
            this.CrntTblTb305.Text = "0,00";
            // 
            // CrntTblTb304
            // 
            this.CrntTblTb304.Location = new System.Drawing.Point(277, 81);
            this.CrntTblTb304.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb304.Name = "CrntTblTb304";
            this.CrntTblTb304.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb304.TabIndex = 82;
            this.CrntTblTb304.Text = "0,00";
            // 
            // CrntTblTb303
            // 
            this.CrntTblTb303.Location = new System.Drawing.Point(210, 81);
            this.CrntTblTb303.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb303.Name = "CrntTblTb303";
            this.CrntTblTb303.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb303.TabIndex = 81;
            this.CrntTblTb303.Text = "0,00";
            // 
            // CrntTblTb302
            // 
            this.CrntTblTb302.Location = new System.Drawing.Point(143, 81);
            this.CrntTblTb302.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb302.Name = "CrntTblTb302";
            this.CrntTblTb302.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb302.TabIndex = 80;
            this.CrntTblTb302.Text = "0,00";
            // 
            // CrntTblTb301
            // 
            this.CrntTblTb301.Location = new System.Drawing.Point(76, 81);
            this.CrntTblTb301.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb301.Name = "CrntTblTb301";
            this.CrntTblTb301.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb301.TabIndex = 79;
            this.CrntTblTb301.Text = "0,00";
            // 
            // CrntTblTb211
            // 
            this.CrntTblTb211.Location = new System.Drawing.Point(746, 45);
            this.CrntTblTb211.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb211.Name = "CrntTblTb211";
            this.CrntTblTb211.ReadOnly = true;
            this.CrntTblTb211.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb211.TabIndex = 78;
            this.CrntTblTb211.Text = "90";
            // 
            // CrntTblTb210
            // 
            this.CrntTblTb210.Location = new System.Drawing.Point(679, 45);
            this.CrntTblTb210.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb210.Name = "CrntTblTb210";
            this.CrntTblTb210.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb210.TabIndex = 77;
            this.CrntTblTb210.Text = "0";
            // 
            // CrntTblTb209
            // 
            this.CrntTblTb209.Location = new System.Drawing.Point(612, 45);
            this.CrntTblTb209.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb209.Name = "CrntTblTb209";
            this.CrntTblTb209.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb209.TabIndex = 76;
            this.CrntTblTb209.Text = "0";
            // 
            // CrntTblTb208
            // 
            this.CrntTblTb208.Location = new System.Drawing.Point(545, 45);
            this.CrntTblTb208.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb208.Name = "CrntTblTb208";
            this.CrntTblTb208.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb208.TabIndex = 75;
            this.CrntTblTb208.Text = "0";
            // 
            // CrntTblTb207
            // 
            this.CrntTblTb207.Location = new System.Drawing.Point(478, 45);
            this.CrntTblTb207.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb207.Name = "CrntTblTb207";
            this.CrntTblTb207.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb207.TabIndex = 74;
            this.CrntTblTb207.Text = "0";
            // 
            // CrntTblTb206
            // 
            this.CrntTblTb206.Location = new System.Drawing.Point(411, 45);
            this.CrntTblTb206.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb206.Name = "CrntTblTb206";
            this.CrntTblTb206.ReadOnly = true;
            this.CrntTblTb206.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb206.TabIndex = 73;
            this.CrntTblTb206.Text = "0";
            // 
            // CrntTblTb205
            // 
            this.CrntTblTb205.Location = new System.Drawing.Point(344, 45);
            this.CrntTblTb205.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb205.Name = "CrntTblTb205";
            this.CrntTblTb205.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb205.TabIndex = 72;
            this.CrntTblTb205.Text = "0";
            // 
            // CrntTblTb204
            // 
            this.CrntTblTb204.Location = new System.Drawing.Point(277, 45);
            this.CrntTblTb204.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb204.Name = "CrntTblTb204";
            this.CrntTblTb204.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb204.TabIndex = 71;
            this.CrntTblTb204.Text = "0";
            // 
            // CrntTblTb203
            // 
            this.CrntTblTb203.Location = new System.Drawing.Point(210, 45);
            this.CrntTblTb203.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb203.Name = "CrntTblTb203";
            this.CrntTblTb203.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb203.TabIndex = 70;
            this.CrntTblTb203.Text = "0";
            // 
            // CrntTblTb202
            // 
            this.CrntTblTb202.Location = new System.Drawing.Point(143, 45);
            this.CrntTblTb202.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb202.Name = "CrntTblTb202";
            this.CrntTblTb202.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb202.TabIndex = 69;
            this.CrntTblTb202.Text = "0";
            // 
            // CrntTblRowLbl02
            // 
            this.CrntTblRowLbl02.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblRowLbl02.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblRowLbl02.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblRowLbl02.Location = new System.Drawing.Point(4, 40);
            this.CrntTblRowLbl02.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblRowLbl02.Name = "CrntTblRowLbl02";
            this.CrntTblRowLbl02.Size = new System.Drawing.Size(60, 29);
            this.CrntTblRowLbl02.TabIndex = 65;
            this.CrntTblRowLbl02.Text = "Angle";
            this.CrntTblRowLbl02.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.CrntTblRowLbl02, "This is the climb angle.");
            // 
            // CrntTblRowLbl01
            // 
            this.CrntTblRowLbl01.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblRowLbl01.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblRowLbl01.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblRowLbl01.Location = new System.Drawing.Point(4, 4);
            this.CrntTblRowLbl01.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblRowLbl01.Name = "CrntTblRowLbl01";
            this.CrntTblRowLbl01.Size = new System.Drawing.Size(60, 29);
            this.CrntTblRowLbl01.TabIndex = 64;
            this.CrntTblRowLbl01.Text = "Value";
            this.CrntTblRowLbl01.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl01
            // 
            this.CrntTblClmLbl01.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl01.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl01.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl01.Location = new System.Drawing.Point(71, 4);
            this.CrntTblClmLbl01.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl01.Name = "CrntTblClmLbl01";
            this.CrntTblClmLbl01.Size = new System.Drawing.Size(60, 29);
            this.CrntTblClmLbl01.TabIndex = 2;
            this.CrntTblClmLbl01.Text = "01";
            this.CrntTblClmLbl01.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl02
            // 
            this.CrntTblClmLbl02.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl02.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl02.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl02.Location = new System.Drawing.Point(138, 4);
            this.CrntTblClmLbl02.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl02.Name = "CrntTblClmLbl02";
            this.CrntTblClmLbl02.Size = new System.Drawing.Size(60, 29);
            this.CrntTblClmLbl02.TabIndex = 3;
            this.CrntTblClmLbl02.Text = "02";
            this.CrntTblClmLbl02.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl03
            // 
            this.CrntTblClmLbl03.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl03.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl03.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl03.Location = new System.Drawing.Point(205, 4);
            this.CrntTblClmLbl03.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl03.Name = "CrntTblClmLbl03";
            this.CrntTblClmLbl03.Size = new System.Drawing.Size(60, 29);
            this.CrntTblClmLbl03.TabIndex = 4;
            this.CrntTblClmLbl03.Text = "03";
            this.CrntTblClmLbl03.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl04
            // 
            this.CrntTblClmLbl04.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl04.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl04.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl04.Location = new System.Drawing.Point(272, 4);
            this.CrntTblClmLbl04.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl04.Name = "CrntTblClmLbl04";
            this.CrntTblClmLbl04.Size = new System.Drawing.Size(60, 29);
            this.CrntTblClmLbl04.TabIndex = 5;
            this.CrntTblClmLbl04.Text = "04";
            this.CrntTblClmLbl04.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl05
            // 
            this.CrntTblClmLbl05.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl05.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl05.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl05.Location = new System.Drawing.Point(339, 4);
            this.CrntTblClmLbl05.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl05.Name = "CrntTblClmLbl05";
            this.CrntTblClmLbl05.Size = new System.Drawing.Size(60, 29);
            this.CrntTblClmLbl05.TabIndex = 6;
            this.CrntTblClmLbl05.Text = "05";
            this.CrntTblClmLbl05.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl06
            // 
            this.CrntTblClmLbl06.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl06.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl06.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl06.Location = new System.Drawing.Point(406, 4);
            this.CrntTblClmLbl06.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl06.Name = "CrntTblClmLbl06";
            this.CrntTblClmLbl06.Size = new System.Drawing.Size(60, 29);
            this.CrntTblClmLbl06.TabIndex = 7;
            this.CrntTblClmLbl06.Text = "06";
            this.CrntTblClmLbl06.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl07
            // 
            this.CrntTblClmLbl07.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl07.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl07.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl07.Location = new System.Drawing.Point(473, 4);
            this.CrntTblClmLbl07.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl07.Name = "CrntTblClmLbl07";
            this.CrntTblClmLbl07.Size = new System.Drawing.Size(60, 29);
            this.CrntTblClmLbl07.TabIndex = 8;
            this.CrntTblClmLbl07.Text = "07";
            this.CrntTblClmLbl07.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl08
            // 
            this.CrntTblClmLbl08.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl08.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl08.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl08.Location = new System.Drawing.Point(540, 4);
            this.CrntTblClmLbl08.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl08.Name = "CrntTblClmLbl08";
            this.CrntTblClmLbl08.Size = new System.Drawing.Size(60, 29);
            this.CrntTblClmLbl08.TabIndex = 9;
            this.CrntTblClmLbl08.Text = "08";
            this.CrntTblClmLbl08.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl09
            // 
            this.CrntTblClmLbl09.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl09.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl09.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl09.Location = new System.Drawing.Point(607, 4);
            this.CrntTblClmLbl09.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl09.Name = "CrntTblClmLbl09";
            this.CrntTblClmLbl09.Size = new System.Drawing.Size(60, 29);
            this.CrntTblClmLbl09.TabIndex = 10;
            this.CrntTblClmLbl09.Text = "09";
            this.CrntTblClmLbl09.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl10
            // 
            this.CrntTblClmLbl10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl10.Location = new System.Drawing.Point(674, 4);
            this.CrntTblClmLbl10.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl10.Name = "CrntTblClmLbl10";
            this.CrntTblClmLbl10.Size = new System.Drawing.Size(60, 29);
            this.CrntTblClmLbl10.TabIndex = 11;
            this.CrntTblClmLbl10.Text = "10";
            this.CrntTblClmLbl10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblClmLbl11
            // 
            this.CrntTblClmLbl11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblClmLbl11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblClmLbl11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblClmLbl11.Location = new System.Drawing.Point(741, 4);
            this.CrntTblClmLbl11.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblClmLbl11.Name = "CrntTblClmLbl11";
            this.CrntTblClmLbl11.Size = new System.Drawing.Size(65, 29);
            this.CrntTblClmLbl11.TabIndex = 12;
            this.CrntTblClmLbl11.Text = "11";
            this.CrntTblClmLbl11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CrntTblRowLbl03
            // 
            this.CrntTblRowLbl03.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblRowLbl03.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblRowLbl03.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblRowLbl03.Location = new System.Drawing.Point(4, 76);
            this.CrntTblRowLbl03.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblRowLbl03.Name = "CrntTblRowLbl03";
            this.CrntTblRowLbl03.Size = new System.Drawing.Size(60, 29);
            this.CrntTblRowLbl03.TabIndex = 66;
            this.CrntTblRowLbl03.Text = "Ø\r\nCurrent";
            this.CrntTblRowLbl03.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.CrntTblRowLbl03, "This is the mean current.");
            // 
            // CrntTblRowLbl04
            // 
            this.CrntTblRowLbl04.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CrntTblRowLbl04.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrntTblRowLbl04.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CrntTblRowLbl04.Location = new System.Drawing.Point(4, 112);
            this.CrntTblRowLbl04.Margin = new System.Windows.Forms.Padding(3);
            this.CrntTblRowLbl04.Name = "CrntTblRowLbl04";
            this.CrntTblRowLbl04.Size = new System.Drawing.Size(60, 30);
            this.CrntTblRowLbl04.TabIndex = 67;
            this.CrntTblRowLbl04.Text = "Dev.";
            this.CrntTblRowLbl04.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.CrntTblRowLbl04, "This is the deviation from mean current.");
            // 
            // CrntTblTb201
            // 
            this.CrntTblTb201.Location = new System.Drawing.Point(76, 45);
            this.CrntTblTb201.Margin = new System.Windows.Forms.Padding(8);
            this.CrntTblTb201.Name = "CrntTblTb201";
            this.CrntTblTb201.ReadOnly = true;
            this.CrntTblTb201.Size = new System.Drawing.Size(50, 20);
            this.CrntTblTb201.TabIndex = 68;
            this.CrntTblTb201.Text = "-90";
            // 
            // ChartI
            // 
            this.ChartI.BackColor = System.Drawing.Color.Transparent;
            this.ChartI.BorderlineColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ChartI.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea3.AxisX.Interval = 22.5D;
            chartArea3.AxisX.IsLabelAutoFit = false;
            chartArea3.AxisX.LabelAutoFitMaxFontSize = 8;
            chartArea3.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            chartArea3.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea3.AxisX.Maximum = 90D;
            chartArea3.AxisX.Minimum = -90D;
            chartArea3.AxisX.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            chartArea3.AxisX.Title = "Angle [Degree]";
            chartArea3.AxisX.TitleForeColor = System.Drawing.Color.White;
            chartArea3.AxisY.IsLabelAutoFit = false;
            chartArea3.AxisY.LabelAutoFitMaxFontSize = 7;
            chartArea3.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            chartArea3.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.DimGray;
            chartArea3.AxisY.Title = "Current [A]";
            chartArea3.AxisY.TitleForeColor = System.Drawing.Color.White;
            chartArea3.BackColor = System.Drawing.Color.Transparent;
            chartArea3.Name = "ChartArea1";
            this.ChartI.ChartAreas.Add(chartArea3);
            legend4.AutoFitMinFontSize = 8;
            legend4.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            legend4.ForeColor = System.Drawing.SystemColors.Control;
            legend4.IsTextAutoFit = false;
            legend4.Name = "Legend1";
            this.ChartI.Legends.Add(legend4);
            this.ChartI.Location = new System.Drawing.Point(3, 52);
            this.ChartI.Name = "ChartI";
            series9.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineRange;
            series9.Color = System.Drawing.Color.Wheat;
            series9.Legend = "Legend1";
            series9.Name = "Range";
            series9.Points.Add(dataPoint9);
            series9.YValuesPerPoint = 2;
            series10.BorderWidth = 3;
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series10.Color = System.Drawing.Color.OrangeRed;
            series10.Legend = "Legend1";
            series10.MarkerColor = System.Drawing.Color.White;
            series10.Name = "MaxCurrent";
            series10.Points.Add(dataPoint10);
            series11.BorderWidth = 3;
            series11.ChartArea = "ChartArea1";
            series11.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series11.Color = System.Drawing.Color.Cyan;
            series11.Legend = "Legend1";
            series11.Name = "MinCurrent";
            series11.Points.Add(dataPoint11);
            series12.BorderWidth = 3;
            series12.ChartArea = "ChartArea1";
            series12.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series12.Color = System.Drawing.Color.LawnGreen;
            series12.Legend = "Legend1";
            series12.Name = "AverageCurrent";
            series12.Points.Add(dataPoint12);
            this.ChartI.Series.Add(series9);
            this.ChartI.Series.Add(series10);
            this.ChartI.Series.Add(series11);
            this.ChartI.Series.Add(series12);
            this.ChartI.Size = new System.Drawing.Size(810, 293);
            this.ChartI.TabIndex = 54;
            this.ChartI.Text = "chart1";
            // 
            // lbCurrentInfo2
            // 
            this.lbCurrentInfo2.AutoSize = true;
            this.lbCurrentInfo2.Location = new System.Drawing.Point(273, 3);
            this.lbCurrentInfo2.Name = "lbCurrentInfo2";
            this.lbCurrentInfo2.Size = new System.Drawing.Size(0, 13);
            this.lbCurrentInfo2.TabIndex = 47;
            // 
            // lblTitleCurrent
            // 
            this.lblTitleCurrent.AutoSize = true;
            this.lblTitleCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleCurrent.Location = new System.Drawing.Point(3, 3);
            this.lblTitleCurrent.Name = "lblTitleCurrent";
            this.lblTitleCurrent.Size = new System.Drawing.Size(196, 26);
            this.lblTitleCurrent.TabIndex = 47;
            this.lblTitleCurrent.Text = "Energy Consumption during Flight\r\n(Movement)";
            // 
            // panelHover
            // 
            this.panelHover.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelHover.Controls.Add(this.tableLayoutPanel1);
            this.panelHover.Controls.Add(this.lbCurrentInfo1);
            this.panelHover.Controls.Add(this.label30);
            this.panelHover.Enabled = false;
            this.panelHover.Location = new System.Drawing.Point(16, 61);
            this.panelHover.Name = "panelHover";
            this.panelHover.Size = new System.Drawing.Size(450, 78);
            this.panelHover.TabIndex = 53;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.HoverDevTB, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbCurrentHover, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbVarianceCurrentHover, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.HoverCrntTB, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 32);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(442, 41);
            this.tableLayoutPanel1.TabIndex = 58;
            // 
            // HoverDevTB
            // 
            this.HoverDevTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HoverDevTB.Enabled = false;
            this.HoverDevTB.Location = new System.Drawing.Point(341, 11);
            this.HoverDevTB.Margin = new System.Windows.Forms.Padding(10);
            this.HoverDevTB.Name = "HoverDevTB";
            this.HoverDevTB.Size = new System.Drawing.Size(90, 20);
            this.HoverDevTB.TabIndex = 52;
            this.HoverDevTB.Text = "0";
            // 
            // lbCurrentHover
            // 
            this.lbCurrentHover.AutoSize = true;
            this.lbCurrentHover.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbCurrentHover.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCurrentHover.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrentHover.Location = new System.Drawing.Point(4, 4);
            this.lbCurrentHover.Margin = new System.Windows.Forms.Padding(3);
            this.lbCurrentHover.Name = "lbCurrentHover";
            this.lbCurrentHover.Size = new System.Drawing.Size(103, 34);
            this.lbCurrentHover.TabIndex = 57;
            this.lbCurrentHover.Text = "Current [A]:";
            this.lbCurrentHover.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.lbCurrentHover, "The average current in hover mode.");
            // 
            // lbVarianceCurrentHover
            // 
            this.lbVarianceCurrentHover.AutoSize = true;
            this.lbVarianceCurrentHover.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbVarianceCurrentHover.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbVarianceCurrentHover.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVarianceCurrentHover.Location = new System.Drawing.Point(224, 4);
            this.lbVarianceCurrentHover.Margin = new System.Windows.Forms.Padding(3);
            this.lbVarianceCurrentHover.Name = "lbVarianceCurrentHover";
            this.lbVarianceCurrentHover.Size = new System.Drawing.Size(103, 34);
            this.lbVarianceCurrentHover.TabIndex = 52;
            this.lbVarianceCurrentHover.Text = "Deviation";
            this.lbVarianceCurrentHover.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.lbVarianceCurrentHover, "The deviation from average current in hover mode.");
            // 
            // HoverCrntTB
            // 
            this.HoverCrntTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HoverCrntTB.Location = new System.Drawing.Point(121, 11);
            this.HoverCrntTB.Margin = new System.Windows.Forms.Padding(10);
            this.HoverCrntTB.Name = "HoverCrntTB";
            this.HoverCrntTB.Size = new System.Drawing.Size(89, 20);
            this.HoverCrntTB.TabIndex = 56;
            this.HoverCrntTB.Text = "0";
            // 
            // lbCurrentInfo1
            // 
            this.lbCurrentInfo1.AutoSize = true;
            this.lbCurrentInfo1.Location = new System.Drawing.Point(213, 2);
            this.lbCurrentInfo1.Name = "lbCurrentInfo1";
            this.lbCurrentInfo1.Size = new System.Drawing.Size(232, 26);
            this.lbCurrentInfo1.TabIndex = 64;
            this.lbCurrentInfo1.Text = "These settings characterize the flowing current\r\nduring a hover in air / a hold p" +
    "osition maneuver.";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(5, 2);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(199, 26);
            this.label30.TabIndex = 58;
            this.label30.Text = "Energy Consumption during Hover\r\n(No Movement)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 41;
            // 
            // tbCopterID
            // 
            this.tbCopterID.Location = new System.Drawing.Point(64, 10);
            this.tbCopterID.Name = "tbCopterID";
            this.tbCopterID.ReadOnly = true;
            this.tbCopterID.Size = new System.Drawing.Size(49, 20);
            this.tbCopterID.TabIndex = 52;
            this.tbCopterID.Text = "0";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(3, 10);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(52, 13);
            this.label24.TabIndex = 61;
            this.label24.Text = "Copter ID";
            // 
            // panelExpImp
            // 
            this.panelExpImp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelExpImp.Controls.Add(this.panel1);
            this.panelExpImp.Controls.Add(this.label29);
            this.panelExpImp.Enabled = false;
            this.panelExpImp.Location = new System.Drawing.Point(468, 61);
            this.panelExpImp.Name = "panelExpImp";
            this.panelExpImp.Size = new System.Drawing.Size(367, 78);
            this.panelExpImp.TabIndex = 63;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label24);
            this.panel1.Controls.Add(this.BtnImport);
            this.panel1.Controls.Add(this.BtnExport);
            this.panel1.Controls.Add(this.tbCopterID);
            this.panel1.Location = new System.Drawing.Point(3, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(359, 38);
            this.panel1.TabIndex = 66;
            // 
            // BtnImport
            // 
            this.BtnImport.Location = new System.Drawing.Point(267, 4);
            this.BtnImport.Name = "BtnImport";
            this.BtnImport.Size = new System.Drawing.Size(84, 26);
            this.BtnImport.TabIndex = 64;
            this.BtnImport.Text = "Import Profile";
            this.ToolTip.SetToolTip(this.BtnImport, "Import a energy profile from XML.");
            this.BtnImport.UseVisualStyleBackColor = true;
            this.BtnImport.Click += new System.EventHandler(this.BtnImport_Click);
            // 
            // BtnExport
            // 
            this.BtnExport.Location = new System.Drawing.Point(177, 4);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(84, 26);
            this.BtnExport.TabIndex = 60;
            this.BtnExport.Text = "Export Profile";
            this.ToolTip.SetToolTip(this.BtnExport, "Export a energy profile to XML.");
            this.BtnExport.UseVisualStyleBackColor = true;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(3, 2);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(167, 13);
            this.label29.TabIndex = 64;
            this.label29.Text = "Export/Import Energy-Profile";
            // 
            // panelVelocityConfiguration
            // 
            this.panelVelocityConfiguration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelVelocityConfiguration.Controls.Add(this.label4);
            this.panelVelocityConfiguration.Controls.Add(this.CB_Interp_Vel);
            this.panelVelocityConfiguration.Controls.Add(this.label2);
            this.panelVelocityConfiguration.Controls.Add(this.ComboBoxVelDeviation);
            this.panelVelocityConfiguration.Controls.Add(this.BtnPlotVelocity);
            this.panelVelocityConfiguration.Controls.Add(this.LbVelocityInfo);
            this.panelVelocityConfiguration.Controls.Add(this.VelocityTable);
            this.panelVelocityConfiguration.Controls.Add(this.ChartV);
            this.panelVelocityConfiguration.Controls.Add(this.label19);
            this.panelVelocityConfiguration.Controls.Add(this.LbTitleVelocity);
            this.panelVelocityConfiguration.Enabled = false;
            this.panelVelocityConfiguration.Location = new System.Drawing.Point(17, 648);
            this.panelVelocityConfiguration.Name = "panelVelocityConfiguration";
            this.panelVelocityConfiguration.Size = new System.Drawing.Size(818, 502);
            this.panelVelocityConfiguration.TabIndex = 64;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(653, 221);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(138, 13);
            this.label4.TabIndex = 72;
            this.label4.Text = "Interpolation Mode [%]:";
            // 
            // CB_Interp_Vel
            // 
            this.CB_Interp_Vel.DisplayMember = "0";
            this.CB_Interp_Vel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Interp_Vel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_Interp_Vel.Items.AddRange(new object[] {
            "Linear",
            "Cubic Spline"});
            this.CB_Interp_Vel.Location = new System.Drawing.Point(656, 248);
            this.CB_Interp_Vel.MaxDropDownItems = 5;
            this.CB_Interp_Vel.Name = "CB_Interp_Vel";
            this.CB_Interp_Vel.Size = new System.Drawing.Size(135, 21);
            this.CB_Interp_Vel.TabIndex = 71;
            this.ToolTip.SetToolTip(this.CB_Interp_Vel, "Set a fix deviation for all current values.");
            this.CB_Interp_Vel.SelectedIndexChanged += new System.EventHandler(this.CB_Interp_Vel_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(653, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 13);
            this.label2.TabIndex = 68;
            this.label2.Text = "Expected deviation [%]:";
            this.label2.Visible = false;
            // 
            // ComboBoxVelDeviation
            // 
            this.ComboBoxVelDeviation.DisplayMember = "0";
            this.ComboBoxVelDeviation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxVelDeviation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboBoxVelDeviation.Location = new System.Drawing.Point(656, 169);
            this.ComboBoxVelDeviation.MaxDropDownItems = 5;
            this.ComboBoxVelDeviation.Name = "ComboBoxVelDeviation";
            this.ComboBoxVelDeviation.Size = new System.Drawing.Size(49, 21);
            this.ComboBoxVelDeviation.TabIndex = 67;
            this.ComboBoxVelDeviation.Visible = false;
            // 
            // BtnPlotVelocity
            // 
            this.BtnPlotVelocity.Location = new System.Drawing.Point(722, 310);
            this.BtnPlotVelocity.Name = "BtnPlotVelocity";
            this.BtnPlotVelocity.Size = new System.Drawing.Size(84, 26);
            this.BtnPlotVelocity.TabIndex = 52;
            this.BtnPlotVelocity.Text = "Plot";
            this.ToolTip.SetToolTip(this.BtnPlotVelocity, "Plot the Velocity in flight.");
            this.BtnPlotVelocity.UseVisualStyleBackColor = true;
            this.BtnPlotVelocity.Click += new System.EventHandler(this.BtnPlotVelocity_Click);
            // 
            // LbVelocityInfo
            // 
            this.LbVelocityInfo.AutoSize = true;
            this.LbVelocityInfo.Location = new System.Drawing.Point(359, 0);
            this.LbVelocityInfo.Name = "LbVelocityInfo";
            this.LbVelocityInfo.Size = new System.Drawing.Size(455, 39);
            this.LbVelocityInfo.TabIndex = 66;
            this.LbVelocityInfo.Text = resources.GetString("LbVelocityInfo.Text");
            // 
            // VelocityTable
            // 
            this.VelocityTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.VelocityTable.ColumnCount = 12;
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.VelocityTable.Controls.Add(this.VelTblTb411, 11, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb410, 10, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb409, 9, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb408, 8, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb407, 7, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb406, 6, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb405, 5, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb404, 4, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb403, 3, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb402, 2, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb401, 1, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb311, 11, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb310, 10, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb309, 9, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb308, 8, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb307, 7, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb306, 6, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb305, 5, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb304, 4, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb303, 3, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb302, 2, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb301, 1, 2);
            this.VelocityTable.Controls.Add(this.VelTblTb211, 11, 1);
            this.VelocityTable.Controls.Add(this.VelTblTb210, 10, 1);
            this.VelocityTable.Controls.Add(this.VelTblTb209, 9, 1);
            this.VelocityTable.Controls.Add(this.VelTblTb208, 8, 1);
            this.VelocityTable.Controls.Add(this.VelTblTb207, 7, 1);
            this.VelocityTable.Controls.Add(this.VelTblTb206, 6, 1);
            this.VelocityTable.Controls.Add(this.VelTblTb205, 5, 1);
            this.VelocityTable.Controls.Add(this.VelTblTb204, 4, 1);
            this.VelocityTable.Controls.Add(this.VelTblTb203, 3, 1);
            this.VelocityTable.Controls.Add(this.VelTblTb202, 2, 1);
            this.VelocityTable.Controls.Add(this.VelTblRowLbl02, 0, 1);
            this.VelocityTable.Controls.Add(this.VelTblRowLbl01, 0, 0);
            this.VelocityTable.Controls.Add(this.VelTblClmLbl01, 1, 0);
            this.VelocityTable.Controls.Add(this.VelTblClmLbl02, 2, 0);
            this.VelocityTable.Controls.Add(this.VelTblClmLbl03, 3, 0);
            this.VelocityTable.Controls.Add(this.VelTblClmLbl04, 4, 0);
            this.VelocityTable.Controls.Add(this.VelTblClmLbl05, 5, 0);
            this.VelocityTable.Controls.Add(this.VelTblClmLbl06, 6, 0);
            this.VelocityTable.Controls.Add(this.VelTblClmLbl08, 7, 0);
            this.VelocityTable.Controls.Add(this.label13, 8, 0);
            this.VelocityTable.Controls.Add(this.VelTblClmLbl09, 9, 0);
            this.VelocityTable.Controls.Add(this.VelTblClmLbl10, 10, 0);
            this.VelocityTable.Controls.Add(this.VelTblClmLbl11, 11, 0);
            this.VelocityTable.Controls.Add(this.VelTblRowLbl03, 0, 2);
            this.VelocityTable.Controls.Add(this.VelTblRowLbl04, 0, 3);
            this.VelocityTable.Controls.Add(this.VelTblTb201, 1, 1);
            this.VelocityTable.Location = new System.Drawing.Point(3, 351);
            this.VelocityTable.Name = "VelocityTable";
            this.VelocityTable.RowCount = 4;
            this.VelocityTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.VelocityTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.VelocityTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.VelocityTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.VelocityTable.Size = new System.Drawing.Size(810, 146);
            this.VelocityTable.TabIndex = 64;
            // 
            // VelTblTb411
            // 
            this.VelTblTb411.Enabled = false;
            this.VelTblTb411.Location = new System.Drawing.Point(746, 117);
            this.VelTblTb411.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb411.Name = "VelTblTb411";
            this.VelTblTb411.ReadOnly = true;
            this.VelTblTb411.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb411.TabIndex = 100;
            this.VelTblTb411.Text = "0,00";
            // 
            // VelTblTb410
            // 
            this.VelTblTb410.Enabled = false;
            this.VelTblTb410.Location = new System.Drawing.Point(679, 117);
            this.VelTblTb410.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb410.Name = "VelTblTb410";
            this.VelTblTb410.ReadOnly = true;
            this.VelTblTb410.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb410.TabIndex = 99;
            this.VelTblTb410.Text = "0,00";
            // 
            // VelTblTb409
            // 
            this.VelTblTb409.Enabled = false;
            this.VelTblTb409.Location = new System.Drawing.Point(612, 117);
            this.VelTblTb409.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb409.Name = "VelTblTb409";
            this.VelTblTb409.ReadOnly = true;
            this.VelTblTb409.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb409.TabIndex = 98;
            this.VelTblTb409.Text = "0,00";
            // 
            // VelTblTb408
            // 
            this.VelTblTb408.Enabled = false;
            this.VelTblTb408.Location = new System.Drawing.Point(545, 117);
            this.VelTblTb408.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb408.Name = "VelTblTb408";
            this.VelTblTb408.ReadOnly = true;
            this.VelTblTb408.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb408.TabIndex = 97;
            this.VelTblTb408.Text = "0,00";
            // 
            // VelTblTb407
            // 
            this.VelTblTb407.Enabled = false;
            this.VelTblTb407.Location = new System.Drawing.Point(478, 117);
            this.VelTblTb407.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb407.Name = "VelTblTb407";
            this.VelTblTb407.ReadOnly = true;
            this.VelTblTb407.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb407.TabIndex = 96;
            this.VelTblTb407.Text = "0,00";
            // 
            // VelTblTb406
            // 
            this.VelTblTb406.Enabled = false;
            this.VelTblTb406.Location = new System.Drawing.Point(411, 117);
            this.VelTblTb406.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb406.Name = "VelTblTb406";
            this.VelTblTb406.ReadOnly = true;
            this.VelTblTb406.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb406.TabIndex = 95;
            this.VelTblTb406.Text = "0,00";
            // 
            // VelTblTb405
            // 
            this.VelTblTb405.Enabled = false;
            this.VelTblTb405.Location = new System.Drawing.Point(344, 117);
            this.VelTblTb405.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb405.Name = "VelTblTb405";
            this.VelTblTb405.ReadOnly = true;
            this.VelTblTb405.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb405.TabIndex = 94;
            this.VelTblTb405.Text = "0,00";
            // 
            // VelTblTb404
            // 
            this.VelTblTb404.Enabled = false;
            this.VelTblTb404.Location = new System.Drawing.Point(277, 117);
            this.VelTblTb404.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb404.Name = "VelTblTb404";
            this.VelTblTb404.ReadOnly = true;
            this.VelTblTb404.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb404.TabIndex = 93;
            this.VelTblTb404.Text = "0,00";
            // 
            // VelTblTb403
            // 
            this.VelTblTb403.Enabled = false;
            this.VelTblTb403.Location = new System.Drawing.Point(210, 117);
            this.VelTblTb403.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb403.Name = "VelTblTb403";
            this.VelTblTb403.ReadOnly = true;
            this.VelTblTb403.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb403.TabIndex = 92;
            this.VelTblTb403.Text = "0,00";
            // 
            // VelTblTb402
            // 
            this.VelTblTb402.Enabled = false;
            this.VelTblTb402.Location = new System.Drawing.Point(143, 117);
            this.VelTblTb402.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb402.Name = "VelTblTb402";
            this.VelTblTb402.ReadOnly = true;
            this.VelTblTb402.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb402.TabIndex = 91;
            this.VelTblTb402.Text = "0,00";
            // 
            // VelTblTb401
            // 
            this.VelTblTb401.Enabled = false;
            this.VelTblTb401.Location = new System.Drawing.Point(76, 117);
            this.VelTblTb401.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb401.Name = "VelTblTb401";
            this.VelTblTb401.ReadOnly = true;
            this.VelTblTb401.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb401.TabIndex = 90;
            this.VelTblTb401.Text = "0,00";
            // 
            // VelTblTb311
            // 
            this.VelTblTb311.Location = new System.Drawing.Point(746, 81);
            this.VelTblTb311.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb311.Name = "VelTblTb311";
            this.VelTblTb311.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb311.TabIndex = 89;
            this.VelTblTb311.Text = "0,00";
            // 
            // VelTblTb310
            // 
            this.VelTblTb310.Location = new System.Drawing.Point(679, 81);
            this.VelTblTb310.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb310.Name = "VelTblTb310";
            this.VelTblTb310.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb310.TabIndex = 88;
            this.VelTblTb310.Text = "0,00";
            // 
            // VelTblTb309
            // 
            this.VelTblTb309.Location = new System.Drawing.Point(612, 81);
            this.VelTblTb309.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb309.Name = "VelTblTb309";
            this.VelTblTb309.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb309.TabIndex = 87;
            this.VelTblTb309.Text = "0,00";
            // 
            // VelTblTb308
            // 
            this.VelTblTb308.Location = new System.Drawing.Point(545, 81);
            this.VelTblTb308.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb308.Name = "VelTblTb308";
            this.VelTblTb308.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb308.TabIndex = 86;
            this.VelTblTb308.Text = "0,00";
            // 
            // VelTblTb307
            // 
            this.VelTblTb307.Location = new System.Drawing.Point(478, 81);
            this.VelTblTb307.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb307.Name = "VelTblTb307";
            this.VelTblTb307.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb307.TabIndex = 85;
            this.VelTblTb307.Text = "0,00";
            // 
            // VelTblTb306
            // 
            this.VelTblTb306.Location = new System.Drawing.Point(411, 81);
            this.VelTblTb306.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb306.Name = "VelTblTb306";
            this.VelTblTb306.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb306.TabIndex = 84;
            this.VelTblTb306.Text = "0,00";
            // 
            // VelTblTb305
            // 
            this.VelTblTb305.Location = new System.Drawing.Point(344, 81);
            this.VelTblTb305.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb305.Name = "VelTblTb305";
            this.VelTblTb305.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb305.TabIndex = 83;
            this.VelTblTb305.Text = "0,00";
            // 
            // VelTblTb304
            // 
            this.VelTblTb304.Location = new System.Drawing.Point(277, 81);
            this.VelTblTb304.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb304.Name = "VelTblTb304";
            this.VelTblTb304.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb304.TabIndex = 82;
            this.VelTblTb304.Text = "0,00";
            // 
            // VelTblTb303
            // 
            this.VelTblTb303.Location = new System.Drawing.Point(210, 81);
            this.VelTblTb303.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb303.Name = "VelTblTb303";
            this.VelTblTb303.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb303.TabIndex = 81;
            this.VelTblTb303.Text = "0,00";
            // 
            // VelTblTb302
            // 
            this.VelTblTb302.Location = new System.Drawing.Point(143, 81);
            this.VelTblTb302.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb302.Name = "VelTblTb302";
            this.VelTblTb302.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb302.TabIndex = 80;
            this.VelTblTb302.Text = "0,00";
            // 
            // VelTblTb301
            // 
            this.VelTblTb301.Location = new System.Drawing.Point(76, 81);
            this.VelTblTb301.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb301.Name = "VelTblTb301";
            this.VelTblTb301.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb301.TabIndex = 79;
            this.VelTblTb301.Text = "0,00";
            // 
            // VelTblTb211
            // 
            this.VelTblTb211.Location = new System.Drawing.Point(746, 45);
            this.VelTblTb211.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb211.Name = "VelTblTb211";
            this.VelTblTb211.ReadOnly = true;
            this.VelTblTb211.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb211.TabIndex = 78;
            this.VelTblTb211.Text = "90";
            // 
            // VelTblTb210
            // 
            this.VelTblTb210.Location = new System.Drawing.Point(679, 45);
            this.VelTblTb210.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb210.Name = "VelTblTb210";
            this.VelTblTb210.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb210.TabIndex = 77;
            this.VelTblTb210.Text = "0";
            // 
            // VelTblTb209
            // 
            this.VelTblTb209.Location = new System.Drawing.Point(612, 45);
            this.VelTblTb209.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb209.Name = "VelTblTb209";
            this.VelTblTb209.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb209.TabIndex = 76;
            this.VelTblTb209.Text = "0";
            // 
            // VelTblTb208
            // 
            this.VelTblTb208.Location = new System.Drawing.Point(545, 45);
            this.VelTblTb208.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb208.Name = "VelTblTb208";
            this.VelTblTb208.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb208.TabIndex = 75;
            this.VelTblTb208.Text = "0";
            // 
            // VelTblTb207
            // 
            this.VelTblTb207.Location = new System.Drawing.Point(478, 45);
            this.VelTblTb207.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb207.Name = "VelTblTb207";
            this.VelTblTb207.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb207.TabIndex = 74;
            this.VelTblTb207.Text = "0";
            // 
            // VelTblTb206
            // 
            this.VelTblTb206.Location = new System.Drawing.Point(411, 45);
            this.VelTblTb206.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb206.Name = "VelTblTb206";
            this.VelTblTb206.ReadOnly = true;
            this.VelTblTb206.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb206.TabIndex = 73;
            this.VelTblTb206.Text = "0";
            // 
            // VelTblTb205
            // 
            this.VelTblTb205.Location = new System.Drawing.Point(344, 45);
            this.VelTblTb205.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb205.Name = "VelTblTb205";
            this.VelTblTb205.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb205.TabIndex = 72;
            this.VelTblTb205.Text = "0";
            // 
            // VelTblTb204
            // 
            this.VelTblTb204.Location = new System.Drawing.Point(277, 45);
            this.VelTblTb204.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb204.Name = "VelTblTb204";
            this.VelTblTb204.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb204.TabIndex = 71;
            this.VelTblTb204.Text = "0";
            // 
            // VelTblTb203
            // 
            this.VelTblTb203.Location = new System.Drawing.Point(210, 45);
            this.VelTblTb203.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb203.Name = "VelTblTb203";
            this.VelTblTb203.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb203.TabIndex = 70;
            this.VelTblTb203.Text = "0";
            // 
            // VelTblTb202
            // 
            this.VelTblTb202.Location = new System.Drawing.Point(143, 45);
            this.VelTblTb202.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb202.Name = "VelTblTb202";
            this.VelTblTb202.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb202.TabIndex = 69;
            this.VelTblTb202.Text = "0";
            // 
            // VelTblRowLbl02
            // 
            this.VelTblRowLbl02.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblRowLbl02.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblRowLbl02.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblRowLbl02.Location = new System.Drawing.Point(4, 40);
            this.VelTblRowLbl02.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblRowLbl02.Name = "VelTblRowLbl02";
            this.VelTblRowLbl02.Size = new System.Drawing.Size(60, 29);
            this.VelTblRowLbl02.TabIndex = 65;
            this.VelTblRowLbl02.Text = "Angle";
            this.VelTblRowLbl02.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.VelTblRowLbl02, "This is the climb angle.");
            // 
            // VelTblRowLbl01
            // 
            this.VelTblRowLbl01.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblRowLbl01.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblRowLbl01.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblRowLbl01.Location = new System.Drawing.Point(4, 4);
            this.VelTblRowLbl01.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblRowLbl01.Name = "VelTblRowLbl01";
            this.VelTblRowLbl01.Size = new System.Drawing.Size(60, 29);
            this.VelTblRowLbl01.TabIndex = 64;
            this.VelTblRowLbl01.Text = "Value";
            this.VelTblRowLbl01.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblClmLbl01
            // 
            this.VelTblClmLbl01.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblClmLbl01.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblClmLbl01.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblClmLbl01.Location = new System.Drawing.Point(71, 4);
            this.VelTblClmLbl01.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblClmLbl01.Name = "VelTblClmLbl01";
            this.VelTblClmLbl01.Size = new System.Drawing.Size(60, 29);
            this.VelTblClmLbl01.TabIndex = 2;
            this.VelTblClmLbl01.Text = "01";
            this.VelTblClmLbl01.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblClmLbl02
            // 
            this.VelTblClmLbl02.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblClmLbl02.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblClmLbl02.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblClmLbl02.Location = new System.Drawing.Point(138, 4);
            this.VelTblClmLbl02.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblClmLbl02.Name = "VelTblClmLbl02";
            this.VelTblClmLbl02.Size = new System.Drawing.Size(60, 29);
            this.VelTblClmLbl02.TabIndex = 3;
            this.VelTblClmLbl02.Text = "02";
            this.VelTblClmLbl02.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblClmLbl03
            // 
            this.VelTblClmLbl03.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblClmLbl03.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblClmLbl03.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblClmLbl03.Location = new System.Drawing.Point(205, 4);
            this.VelTblClmLbl03.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblClmLbl03.Name = "VelTblClmLbl03";
            this.VelTblClmLbl03.Size = new System.Drawing.Size(60, 29);
            this.VelTblClmLbl03.TabIndex = 4;
            this.VelTblClmLbl03.Text = "03";
            this.VelTblClmLbl03.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblClmLbl04
            // 
            this.VelTblClmLbl04.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblClmLbl04.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblClmLbl04.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblClmLbl04.Location = new System.Drawing.Point(272, 4);
            this.VelTblClmLbl04.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblClmLbl04.Name = "VelTblClmLbl04";
            this.VelTblClmLbl04.Size = new System.Drawing.Size(60, 29);
            this.VelTblClmLbl04.TabIndex = 5;
            this.VelTblClmLbl04.Text = "04";
            this.VelTblClmLbl04.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblClmLbl05
            // 
            this.VelTblClmLbl05.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblClmLbl05.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblClmLbl05.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblClmLbl05.Location = new System.Drawing.Point(339, 4);
            this.VelTblClmLbl05.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblClmLbl05.Name = "VelTblClmLbl05";
            this.VelTblClmLbl05.Size = new System.Drawing.Size(60, 29);
            this.VelTblClmLbl05.TabIndex = 6;
            this.VelTblClmLbl05.Text = "05";
            this.VelTblClmLbl05.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblClmLbl06
            // 
            this.VelTblClmLbl06.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblClmLbl06.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblClmLbl06.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblClmLbl06.Location = new System.Drawing.Point(406, 4);
            this.VelTblClmLbl06.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblClmLbl06.Name = "VelTblClmLbl06";
            this.VelTblClmLbl06.Size = new System.Drawing.Size(60, 29);
            this.VelTblClmLbl06.TabIndex = 7;
            this.VelTblClmLbl06.Text = "06";
            this.VelTblClmLbl06.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblClmLbl08
            // 
            this.VelTblClmLbl08.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblClmLbl08.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblClmLbl08.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblClmLbl08.Location = new System.Drawing.Point(473, 4);
            this.VelTblClmLbl08.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblClmLbl08.Name = "VelTblClmLbl08";
            this.VelTblClmLbl08.Size = new System.Drawing.Size(60, 29);
            this.VelTblClmLbl08.TabIndex = 8;
            this.VelTblClmLbl08.Text = "07";
            this.VelTblClmLbl08.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(540, 4);
            this.label13.Margin = new System.Windows.Forms.Padding(3);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 29);
            this.label13.TabIndex = 9;
            this.label13.Text = "08";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblClmLbl09
            // 
            this.VelTblClmLbl09.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblClmLbl09.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblClmLbl09.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblClmLbl09.Location = new System.Drawing.Point(607, 4);
            this.VelTblClmLbl09.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblClmLbl09.Name = "VelTblClmLbl09";
            this.VelTblClmLbl09.Size = new System.Drawing.Size(60, 29);
            this.VelTblClmLbl09.TabIndex = 10;
            this.VelTblClmLbl09.Text = "09";
            this.VelTblClmLbl09.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblClmLbl10
            // 
            this.VelTblClmLbl10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblClmLbl10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblClmLbl10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblClmLbl10.Location = new System.Drawing.Point(674, 4);
            this.VelTblClmLbl10.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblClmLbl10.Name = "VelTblClmLbl10";
            this.VelTblClmLbl10.Size = new System.Drawing.Size(60, 29);
            this.VelTblClmLbl10.TabIndex = 11;
            this.VelTblClmLbl10.Text = "10";
            this.VelTblClmLbl10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblClmLbl11
            // 
            this.VelTblClmLbl11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblClmLbl11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblClmLbl11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblClmLbl11.Location = new System.Drawing.Point(741, 4);
            this.VelTblClmLbl11.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblClmLbl11.Name = "VelTblClmLbl11";
            this.VelTblClmLbl11.Size = new System.Drawing.Size(65, 29);
            this.VelTblClmLbl11.TabIndex = 12;
            this.VelTblClmLbl11.Text = "11";
            this.VelTblClmLbl11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VelTblRowLbl03
            // 
            this.VelTblRowLbl03.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblRowLbl03.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblRowLbl03.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblRowLbl03.Location = new System.Drawing.Point(4, 76);
            this.VelTblRowLbl03.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblRowLbl03.Name = "VelTblRowLbl03";
            this.VelTblRowLbl03.Size = new System.Drawing.Size(60, 29);
            this.VelTblRowLbl03.TabIndex = 66;
            this.VelTblRowLbl03.Text = "Ø\r\nVelocity";
            this.VelTblRowLbl03.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.VelTblRowLbl03, "This is the mean velocity.");
            // 
            // VelTblRowLbl04
            // 
            this.VelTblRowLbl04.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VelTblRowLbl04.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VelTblRowLbl04.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VelTblRowLbl04.Location = new System.Drawing.Point(4, 112);
            this.VelTblRowLbl04.Margin = new System.Windows.Forms.Padding(3);
            this.VelTblRowLbl04.Name = "VelTblRowLbl04";
            this.VelTblRowLbl04.Size = new System.Drawing.Size(60, 30);
            this.VelTblRowLbl04.TabIndex = 67;
            this.VelTblRowLbl04.Text = "Dev.";
            this.VelTblRowLbl04.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip.SetToolTip(this.VelTblRowLbl04, "This is the deviation from mean velocity.");
            // 
            // VelTblTb201
            // 
            this.VelTblTb201.Location = new System.Drawing.Point(76, 45);
            this.VelTblTb201.Margin = new System.Windows.Forms.Padding(8);
            this.VelTblTb201.Name = "VelTblTb201";
            this.VelTblTb201.ReadOnly = true;
            this.VelTblTb201.Size = new System.Drawing.Size(50, 20);
            this.VelTblTb201.TabIndex = 68;
            this.VelTblTb201.Text = "-90";
            // 
            // ChartV
            // 
            this.ChartV.BackColor = System.Drawing.Color.Transparent;
            this.ChartV.BorderlineColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ChartV.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea4.AxisX.Interval = 22.5D;
            chartArea4.AxisX.IsLabelAutoFit = false;
            chartArea4.AxisX.LabelAutoFitMaxFontSize = 8;
            chartArea4.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            chartArea4.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea4.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea4.AxisX.Maximum = 90D;
            chartArea4.AxisX.Minimum = -90D;
            chartArea4.AxisX.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            chartArea4.AxisX.Title = "Angle [Degree]";
            chartArea4.AxisX.TitleForeColor = System.Drawing.Color.White;
            chartArea4.AxisY.IsLabelAutoFit = false;
            chartArea4.AxisY.LabelAutoFitMaxFontSize = 7;
            chartArea4.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F);
            chartArea4.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea4.AxisY.MajorGrid.LineColor = System.Drawing.Color.DimGray;
            chartArea4.AxisY.Title = "Velocity [m/s]";
            chartArea4.AxisY.TitleForeColor = System.Drawing.Color.White;
            chartArea4.BackColor = System.Drawing.Color.Transparent;
            chartArea4.Name = "ChartArea1";
            this.ChartV.ChartAreas.Add(chartArea4);
            legend5.AutoFitMinFontSize = 8;
            legend5.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            legend5.ForeColor = System.Drawing.SystemColors.Control;
            legend5.IsTextAutoFit = false;
            legend5.Name = "Legend1";
            legend6.Enabled = false;
            legend6.Name = "Legend2";
            this.ChartV.Legends.Add(legend5);
            this.ChartV.Legends.Add(legend6);
            this.ChartV.Location = new System.Drawing.Point(3, 52);
            this.ChartV.Name = "ChartV";
            series13.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            series13.ChartArea = "ChartArea1";
            series13.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineRange;
            series13.Color = System.Drawing.Color.Wheat;
            series13.Legend = "Legend2";
            series13.Name = "Range";
            series13.Points.Add(dataPoint13);
            series13.YValuesPerPoint = 2;
            series14.BorderWidth = 3;
            series14.ChartArea = "ChartArea1";
            series14.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series14.Color = System.Drawing.Color.OrangeRed;
            series14.Legend = "Legend2";
            series14.MarkerColor = System.Drawing.Color.White;
            series14.Name = "MaxVelocity";
            series14.Points.Add(dataPoint14);
            series15.BorderWidth = 3;
            series15.ChartArea = "ChartArea1";
            series15.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series15.Color = System.Drawing.Color.Cyan;
            series15.Legend = "Legend2";
            series15.Name = "MinVelocity";
            series15.Points.Add(dataPoint15);
            series16.BorderWidth = 3;
            series16.ChartArea = "ChartArea1";
            series16.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series16.Color = System.Drawing.Color.LawnGreen;
            series16.Legend = "Legend1";
            series16.Name = "AverageVelocity";
            series16.Points.Add(dataPoint16);
            this.ChartV.Series.Add(series13);
            this.ChartV.Series.Add(series14);
            this.ChartV.Series.Add(series15);
            this.ChartV.Series.Add(series16);
            this.ChartV.Size = new System.Drawing.Size(810, 293);
            this.ChartV.TabIndex = 54;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(273, 3);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(0, 13);
            this.label19.TabIndex = 47;
            // 
            // LbTitleVelocity
            // 
            this.LbTitleVelocity.AutoSize = true;
            this.LbTitleVelocity.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbTitleVelocity.Location = new System.Drawing.Point(3, 3);
            this.LbTitleVelocity.Name = "LbTitleVelocity";
            this.LbTitleVelocity.Size = new System.Drawing.Size(180, 26);
            this.LbTitleVelocity.TabIndex = 47;
            this.LbTitleVelocity.Text = "Speed to Flight Angle Relation\r\n(Movement)";
            // 
            // Btn_Analyze
            // 
            this.Btn_Analyze.Enabled = false;
            this.Btn_Analyze.Location = new System.Drawing.Point(726, 231);
            this.Btn_Analyze.Name = "Btn_Analyze";
            this.Btn_Analyze.Size = new System.Drawing.Size(84, 26);
            this.Btn_Analyze.TabIndex = 52;
            this.Btn_Analyze.Text = "Analyze";
            this.ToolTip.SetToolTip(this.Btn_Analyze, "Analyze all logfiles in listbox.");
            this.Btn_Analyze.UseVisualStyleBackColor = true;
            this.Btn_Analyze.Click += new System.EventHandler(this.Btn_Analyze_Click);
            // 
            // Btn_LoadLogfile
            // 
            this.Btn_LoadLogfile.Location = new System.Drawing.Point(726, 43);
            this.Btn_LoadLogfile.Name = "Btn_LoadLogfile";
            this.Btn_LoadLogfile.Size = new System.Drawing.Size(84, 26);
            this.Btn_LoadLogfile.TabIndex = 68;
            this.Btn_LoadLogfile.Text = "Add Logfile";
            this.ToolTip.SetToolTip(this.Btn_LoadLogfile, "Add logfile to listbox.");
            this.Btn_LoadLogfile.UseVisualStyleBackColor = true;
            this.Btn_LoadLogfile.Click += new System.EventHandler(this.Btn_LoadLogfile_Click);
            // 
            // Btn_DeleteLogfile
            // 
            this.Btn_DeleteLogfile.Location = new System.Drawing.Point(726, 75);
            this.Btn_DeleteLogfile.Name = "Btn_DeleteLogfile";
            this.Btn_DeleteLogfile.Size = new System.Drawing.Size(84, 26);
            this.Btn_DeleteLogfile.TabIndex = 69;
            this.Btn_DeleteLogfile.Text = "Delete Logfile";
            this.ToolTip.SetToolTip(this.Btn_DeleteLogfile, "Delete marked lofiles from listbox.");
            this.Btn_DeleteLogfile.UseVisualStyleBackColor = true;
            this.Btn_DeleteLogfile.Click += new System.EventHandler(this.Btn_DeleteLogfile_Click);
            // 
            // lb_transtime
            // 
            this.lb_transtime.AutoSize = true;
            this.lb_transtime.Location = new System.Drawing.Point(3, 41);
            this.lb_transtime.Name = "lb_transtime";
            this.lb_transtime.Size = new System.Drawing.Size(82, 13);
            this.lb_transtime.TabIndex = 72;
            this.lb_transtime.Text = "Transition Time:";
            this.ToolTip.SetToolTip(this.lb_transtime, "This is the time before or after a waypoint passes for a valid value.");
            // 
            // lb_minvalues
            // 
            this.lb_minvalues.AutoSize = true;
            this.lb_minvalues.Location = new System.Drawing.Point(3, 10);
            this.lb_minvalues.Name = "lb_minvalues";
            this.lb_minvalues.Size = new System.Drawing.Size(91, 13);
            this.lb_minvalues.TabIndex = 61;
            this.lb_minvalues.Text = "Min. Valid Values:";
            this.ToolTip.SetToolTip(this.lb_minvalues, "This is the minimum number of sample-points for an valid mean_value.");
            // 
            // lbl_cmdflighttime
            // 
            this.lbl_cmdflighttime.AutoSize = true;
            this.lbl_cmdflighttime.Location = new System.Drawing.Point(14, 10);
            this.lbl_cmdflighttime.Name = "lbl_cmdflighttime";
            this.lbl_cmdflighttime.Size = new System.Drawing.Size(101, 13);
            this.lbl_cmdflighttime.TabIndex = 73;
            this.lbl_cmdflighttime.Text = "minimum FlightTime:";
            this.ToolTip.SetToolTip(this.lbl_cmdflighttime, "This is the minimum time for a valid flight between 2 commands.");
            // 
            // ChB_SpeedTransition
            // 
            this.ChB_SpeedTransition.AutoSize = true;
            this.ChB_SpeedTransition.Checked = true;
            this.ChB_SpeedTransition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChB_SpeedTransition.Location = new System.Drawing.Point(132, 40);
            this.ChB_SpeedTransition.Name = "ChB_SpeedTransition";
            this.ChB_SpeedTransition.Size = new System.Drawing.Size(103, 17);
            this.ChB_SpeedTransition.TabIndex = 1;
            this.ChB_SpeedTransition.Text = "SpeedTransition";
            this.ToolTip.SetToolTip(this.ChB_SpeedTransition, "Set the transition time for velocity values.");
            this.ChB_SpeedTransition.UseVisualStyleBackColor = true;
            // 
            // ChB_CurrentTransition
            // 
            this.ChB_CurrentTransition.AutoSize = true;
            this.ChB_CurrentTransition.Checked = true;
            this.ChB_CurrentTransition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChB_CurrentTransition.Location = new System.Drawing.Point(17, 40);
            this.ChB_CurrentTransition.Name = "ChB_CurrentTransition";
            this.ChB_CurrentTransition.Size = new System.Drawing.Size(104, 17);
            this.ChB_CurrentTransition.TabIndex = 0;
            this.ChB_CurrentTransition.Text = "CurrentTransiton";
            this.ToolTip.SetToolTip(this.ChB_CurrentTransition, "Set the transition time for current values.");
            this.ChB_CurrentTransition.UseVisualStyleBackColor = true;
            // 
            // Panel_LogAnalyzer
            // 
            this.Panel_LogAnalyzer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_LogAnalyzer.Controls.Add(this.panel3);
            this.Panel_LogAnalyzer.Controls.Add(this.panel2);
            this.Panel_LogAnalyzer.Controls.Add(this.Btn_DeleteLogfile);
            this.Panel_LogAnalyzer.Controls.Add(this.Btn_LoadLogfile);
            this.Panel_LogAnalyzer.Controls.Add(this.Lb_LogAnalyzer);
            this.Panel_LogAnalyzer.Controls.Add(this.Btn_Analyze);
            this.Panel_LogAnalyzer.Controls.Add(this.lb_Infotext_LogfileAnalyzer);
            this.Panel_LogAnalyzer.Controls.Add(this.label22);
            this.Panel_LogAnalyzer.Controls.Add(this.Lbl_Head_LogfileAnalyzer);
            this.Panel_LogAnalyzer.Enabled = false;
            this.Panel_LogAnalyzer.Location = new System.Drawing.Point(17, 1152);
            this.Panel_LogAnalyzer.Name = "Panel_LogAnalyzer";
            this.Panel_LogAnalyzer.Size = new System.Drawing.Size(818, 279);
            this.Panel_LogAnalyzer.TabIndex = 65;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.lbl_cmdflighttime);
            this.panel3.Controls.Add(this.tb_cmdflighttime);
            this.panel3.Controls.Add(this.ChB_SpeedTransition);
            this.panel3.Controls.Add(this.ChB_CurrentTransition);
            this.panel3.Location = new System.Drawing.Point(208, 208);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(256, 66);
            this.panel3.TabIndex = 73;
            this.panel3.Visible = false;
            // 
            // tb_cmdflighttime
            // 
            this.tb_cmdflighttime.Location = new System.Drawing.Point(121, 7);
            this.tb_cmdflighttime.Margin = new System.Windows.Forms.Padding(10);
            this.tb_cmdflighttime.Name = "tb_cmdflighttime";
            this.tb_cmdflighttime.Size = new System.Drawing.Size(89, 20);
            this.tb_cmdflighttime.TabIndex = 72;
            this.tb_cmdflighttime.Text = "3";
            this.tb_cmdflighttime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lb_transtime);
            this.panel2.Controls.Add(this.lb_minvalues);
            this.panel2.Controls.Add(this.tb_transtime);
            this.panel2.Controls.Add(this.tb_minval);
            this.panel2.Location = new System.Drawing.Point(3, 208);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(204, 66);
            this.panel2.TabIndex = 72;
            // 
            // tb_transtime
            // 
            this.tb_transtime.Location = new System.Drawing.Point(108, 38);
            this.tb_transtime.Margin = new System.Windows.Forms.Padding(10);
            this.tb_transtime.Name = "tb_transtime";
            this.tb_transtime.Size = new System.Drawing.Size(89, 20);
            this.tb_transtime.TabIndex = 71;
            this.tb_transtime.Text = "3000";
            this.tb_transtime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tb_minval
            // 
            this.tb_minval.Location = new System.Drawing.Point(108, 7);
            this.tb_minval.Margin = new System.Windows.Forms.Padding(10);
            this.tb_minval.Name = "tb_minval";
            this.tb_minval.Size = new System.Drawing.Size(89, 20);
            this.tb_minval.TabIndex = 70;
            this.tb_minval.Text = "30";
            this.tb_minval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Lb_LogAnalyzer
            // 
            this.Lb_LogAnalyzer.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.Lb_LogAnalyzer.FormattingEnabled = true;
            this.Lb_LogAnalyzer.Location = new System.Drawing.Point(3, 43);
            this.Lb_LogAnalyzer.Name = "Lb_LogAnalyzer";
            this.Lb_LogAnalyzer.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.Lb_LogAnalyzer.Size = new System.Drawing.Size(702, 160);
            this.Lb_LogAnalyzer.TabIndex = 67;
            // 
            // lb_Infotext_LogfileAnalyzer
            // 
            this.lb_Infotext_LogfileAnalyzer.AutoSize = true;
            this.lb_Infotext_LogfileAnalyzer.Location = new System.Drawing.Point(298, 3);
            this.lb_Infotext_LogfileAnalyzer.Name = "lb_Infotext_LogfileAnalyzer";
            this.lb_Infotext_LogfileAnalyzer.Size = new System.Drawing.Size(516, 26);
            this.lb_Infotext_LogfileAnalyzer.TabIndex = 66;
            this.lb_Infotext_LogfileAnalyzer.Text = "Here it is possible to create an individualized energy profile from a number of l" +
    "ogfiles. \r\nThese should come from a suitable benchmark flight. Further informati" +
    "on can be found in the documentation.";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(273, 3);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(0, 13);
            this.label22.TabIndex = 47;
            // 
            // Lbl_Head_LogfileAnalyzer
            // 
            this.Lbl_Head_LogfileAnalyzer.AutoSize = true;
            this.Lbl_Head_LogfileAnalyzer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Head_LogfileAnalyzer.Location = new System.Drawing.Point(3, 3);
            this.Lbl_Head_LogfileAnalyzer.Name = "Lbl_Head_LogfileAnalyzer";
            this.Lbl_Head_LogfileAnalyzer.Size = new System.Drawing.Size(97, 13);
            this.Lbl_Head_LogfileAnalyzer.TabIndex = 47;
            this.Lbl_Head_LogfileAnalyzer.Text = "Logfile Analyzer";
            // 
            // ConfigEnergyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.Panel_LogAnalyzer);
            this.Controls.Add(this.panelVelocityConfiguration);
            this.Controls.Add(this.panelExpImp);
            this.Controls.Add(this.panelHover);
            this.Controls.Add(this.panelCurrentConfiguration);
            this.Controls.Add(this.CB_EnableEnergyProfile);
            this.Controls.Add(this.label1);
            this.Name = "ConfigEnergyProfile";
            this.Size = new System.Drawing.Size(836, 685);
            this.panelCurrentConfiguration.ResumeLayout(false);
            this.panelCurrentConfiguration.PerformLayout();
            this.CrntTable.ResumeLayout(false);
            this.CrntTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartI)).EndInit();
            this.panelHover.ResumeLayout(false);
            this.panelHover.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelExpImp.ResumeLayout(false);
            this.panelExpImp.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelVelocityConfiguration.ResumeLayout(false);
            this.panelVelocityConfiguration.PerformLayout();
            this.VelocityTable.ResumeLayout(false);
            this.VelocityTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartV)).EndInit();
            this.Panel_LogAnalyzer.ResumeLayout(false);
            this.Panel_LogAnalyzer.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.CheckBox CB_EnableEnergyProfile;
        private System.Windows.Forms.Panel panelCurrentConfiguration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTitleCurrent;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartI;
        private System.Windows.Forms.Label lbCurrentInfo2;
        private Controls.MyButton BtnExport;
        private System.Windows.Forms.TextBox tbCopterID;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Panel panelExpImp;
        private System.Windows.Forms.Label label29;
        private Controls.MyButton BtnImport;
        private System.Windows.Forms.Panel panelHover;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label lbCurrentInfo1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel CrntTable;
        private System.Windows.Forms.Label CrntTblClmLbl01;
        private System.Windows.Forms.Label CrntTblClmLbl02;
        private System.Windows.Forms.Label CrntTblClmLbl03;
        private System.Windows.Forms.Label CrntTblClmLbl04;
        private System.Windows.Forms.Label CrntTblClmLbl05;
        private System.Windows.Forms.Label CrntTblClmLbl06;
        private System.Windows.Forms.Label CrntTblClmLbl07;
        private System.Windows.Forms.Label CrntTblClmLbl08;
        private System.Windows.Forms.Label CrntTblClmLbl09;
        private System.Windows.Forms.Label CrntTblClmLbl10;
        private System.Windows.Forms.Label CrntTblClmLbl11;
        private System.Windows.Forms.Label CrntTblRowLbl02;
        private System.Windows.Forms.Label CrntTblRowLbl01;
        private System.Windows.Forms.Label CrntTblRowLbl03;
        private System.Windows.Forms.Label CrntTblRowLbl04;
        private System.Windows.Forms.TextBox CrntTblTb201;
        private System.Windows.Forms.TextBox CrntTblTb410;
        private System.Windows.Forms.TextBox CrntTblTb409;
        private System.Windows.Forms.TextBox CrntTblTb408;
        private System.Windows.Forms.TextBox CrntTblTb407;
        private System.Windows.Forms.TextBox CrntTblTb406;
        private System.Windows.Forms.TextBox CrntTblTb405;
        private System.Windows.Forms.TextBox CrntTblTb404;
        private System.Windows.Forms.TextBox CrntTblTb403;
        private System.Windows.Forms.TextBox CrntTblTb402;
        private System.Windows.Forms.TextBox CrntTblTb401;
        private System.Windows.Forms.TextBox CrntTblTb311;
        private System.Windows.Forms.TextBox CrntTblTb310;
        private System.Windows.Forms.TextBox CrntTblTb309;
        private System.Windows.Forms.TextBox CrntTblTb308;
        private System.Windows.Forms.TextBox CrntTblTb307;
        private System.Windows.Forms.TextBox CrntTblTb306;
        private System.Windows.Forms.TextBox CrntTblTb305;
        private System.Windows.Forms.TextBox CrntTblTb304;
        private System.Windows.Forms.TextBox CrntTblTb303;
        private System.Windows.Forms.TextBox CrntTblTb302;
        private System.Windows.Forms.TextBox CrntTblTb301;
        private System.Windows.Forms.TextBox CrntTblTb211;
        private System.Windows.Forms.TextBox CrntTblTb210;
        private System.Windows.Forms.TextBox CrntTblTb209;
        private System.Windows.Forms.TextBox CrntTblTb208;
        private System.Windows.Forms.TextBox CrntTblTb207;
        private System.Windows.Forms.TextBox CrntTblTb206;
        private System.Windows.Forms.TextBox CrntTblTb205;
        private System.Windows.Forms.TextBox CrntTblTb204;
        private System.Windows.Forms.TextBox CrntTblTb203;
        private System.Windows.Forms.TextBox CrntTblTb202;
        private System.Windows.Forms.TextBox CrntTblTb411;
        private Controls.MyButton BtnPlotCrnt;
        private Label LbInfoCurrentMove;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox HoverDevTB;
        private Label lbCurrentHover;
        private Label lbVarianceCurrentHover;
        private TextBox HoverCrntTB;
        private ComboBox ComboBoxCrntDeviation;
        private Label LabelDeviationCB;
        private Panel panelVelocityConfiguration;
        private Label label2;
        private ComboBox ComboBoxVelDeviation;
        private Controls.MyButton BtnPlotVelocity;
        private Label LbVelocityInfo;
        private TableLayoutPanel VelocityTable;
        private TextBox VelTblTb411;
        private TextBox VelTblTb410;
        private TextBox VelTblTb409;
        private TextBox VelTblTb408;
        private TextBox VelTblTb407;
        private TextBox VelTblTb406;
        private TextBox VelTblTb405;
        private TextBox VelTblTb404;
        private TextBox VelTblTb403;
        private TextBox VelTblTb402;
        private TextBox VelTblTb401;
        private TextBox VelTblTb311;
        private TextBox VelTblTb310;
        private TextBox VelTblTb309;
        private TextBox VelTblTb308;
        private TextBox VelTblTb307;
        private TextBox VelTblTb306;
        private TextBox VelTblTb305;
        private TextBox VelTblTb304;
        private TextBox VelTblTb303;
        private TextBox VelTblTb302;
        private TextBox VelTblTb301;
        private TextBox VelTblTb211;
        private TextBox VelTblTb210;
        private TextBox VelTblTb209;
        private TextBox VelTblTb208;
        private TextBox VelTblTb207;
        private TextBox VelTblTb206;
        private TextBox VelTblTb205;
        private TextBox VelTblTb204;
        private TextBox VelTblTb203;
        private TextBox VelTblTb202;
        private Label VelTblRowLbl02;
        private Label VelTblRowLbl01;
        private Label VelTblClmLbl01;
        private Label VelTblClmLbl02;
        private Label VelTblClmLbl03;
        private Label VelTblClmLbl04;
        private Label VelTblClmLbl05;
        private Label VelTblClmLbl06;
        private Label VelTblClmLbl08;
        private Label label13;
        private Label VelTblClmLbl09;
        private Label VelTblClmLbl10;
        private Label VelTblClmLbl11;
        private Label VelTblRowLbl03;
        private Label VelTblRowLbl04;
        private TextBox VelTblTb201;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartV;
        private Label label19;
        private Label LbTitleVelocity;
        private CheckBox checkBox1;
        private Panel Panel_LogAnalyzer;
        private Controls.MyButton Btn_DeleteLogfile;
        private Controls.MyButton Btn_LoadLogfile;
        private ListBox Lb_LogAnalyzer;
        private Controls.MyButton Btn_Analyze;
        private Label lb_Infotext_LogfileAnalyzer;
        private Label label22;
        private Label Lbl_Head_LogfileAnalyzer;
        private Panel panel2;
        private Label lb_transtime;
        private Label lb_minvalues;
        private TextBox tb_transtime;
        private TextBox tb_minval;
        private Panel panel3;
        private CheckBox ChB_SpeedTransition;
        private CheckBox ChB_CurrentTransition;
        private Label lbl_cmdflighttime;
        private TextBox tb_cmdflighttime;
        private Label label3;
        private ComboBox CB_Interp_Curr;
        private Label label4;
        private ComboBox CB_Interp_Vel;
    }
}
