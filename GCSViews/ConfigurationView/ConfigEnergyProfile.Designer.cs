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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, "22,18");
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 22D);
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint3 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 22D);
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint4 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(-90D, 18D);
            this.CB_EnableEnergyProfile = new System.Windows.Forms.CheckBox();
            this.panelCurrentConfiguration = new System.Windows.Forms.Panel();
            this.LabelDeviationCB = new System.Windows.Forms.Label();
            this.ComboBoxDeviation = new System.Windows.Forms.ComboBox();
            this.BtnPlot = new MissionPlanner.Controls.MyButton();
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
            this.energyProfileModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.currentStateBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panelCurrentConfiguration.SuspendLayout();
            this.CrntTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartI)).BeginInit();
            this.panelHover.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelExpImp.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.energyProfileModelBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentStateBindingSource)).BeginInit();
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
            this.panelCurrentConfiguration.Controls.Add(this.LabelDeviationCB);
            this.panelCurrentConfiguration.Controls.Add(this.ComboBoxDeviation);
            this.panelCurrentConfiguration.Controls.Add(this.BtnPlot);
            this.panelCurrentConfiguration.Controls.Add(this.LbInfoCurrentMove);
            this.panelCurrentConfiguration.Controls.Add(this.CrntTable);
            this.panelCurrentConfiguration.Controls.Add(this.ChartI);
            this.panelCurrentConfiguration.Controls.Add(this.lbCurrentInfo2);
            this.panelCurrentConfiguration.Controls.Add(this.lblTitleCurrent);
            this.panelCurrentConfiguration.Enabled = false;
            this.panelCurrentConfiguration.Location = new System.Drawing.Point(17, 141);
            this.panelCurrentConfiguration.Name = "panelCurrentConfiguration";
            this.panelCurrentConfiguration.Size = new System.Drawing.Size(818, 502);
            this.panelCurrentConfiguration.TabIndex = 40;
            // 
            // LabelDeviationCB
            // 
            this.LabelDeviationCB.AutoSize = true;
            this.LabelDeviationCB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelDeviationCB.Location = new System.Drawing.Point(653, 144);
            this.LabelDeviationCB.Name = "LabelDeviationCB";
            this.LabelDeviationCB.Size = new System.Drawing.Size(146, 26);
            this.LabelDeviationCB.TabIndex = 68;
            this.LabelDeviationCB.Text = "Fix threshold value from \r\naverrage Current:";
            // 
            // ComboBoxDeviation
            // 
            this.ComboBoxDeviation.DisplayMember = "0";
            this.ComboBoxDeviation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxDeviation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboBoxDeviation.Location = new System.Drawing.Point(656, 186);
            this.ComboBoxDeviation.MaxDropDownItems = 5;
            this.ComboBoxDeviation.Name = "ComboBoxDeviation";
            this.ComboBoxDeviation.Size = new System.Drawing.Size(49, 21);
            this.ComboBoxDeviation.TabIndex = 67;
            this.ComboBoxDeviation.SelectedIndexChanged += new System.EventHandler(this.ComboBoxDeviation_SelectedIndexChanged);
            // 
            // BtnPlot
            // 
            this.BtnPlot.Location = new System.Drawing.Point(715, 310);
            this.BtnPlot.Name = "BtnPlot";
            this.BtnPlot.Size = new System.Drawing.Size(84, 26);
            this.BtnPlot.TabIndex = 52;
            this.BtnPlot.Text = "Plot";
            this.BtnPlot.UseVisualStyleBackColor = true;
            this.BtnPlot.Click += new System.EventHandler(this.BtnPlot_Click);
            // 
            // LbInfoCurrentMove
            // 
            this.LbInfoCurrentMove.AutoSize = true;
            this.LbInfoCurrentMove.Location = new System.Drawing.Point(309, 3);
            this.LbInfoCurrentMove.Name = "LbInfoCurrentMove";
            this.LbInfoCurrentMove.Size = new System.Drawing.Size(504, 39);
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
            this.CrntTblRowLbl04.Text = "Stand.\r\nDev.";
            this.CrntTblRowLbl04.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            legend1.AutoFitMinFontSize = 8;
            legend1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            legend1.ForeColor = System.Drawing.SystemColors.Control;
            legend1.IsTextAutoFit = false;
            legend1.Name = "Legend1";
            this.ChartI.Legends.Add(legend1);
            this.ChartI.Location = new System.Drawing.Point(3, 52);
            this.ChartI.Name = "ChartI";
            series1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineRange;
            series1.Color = System.Drawing.Color.Wheat;
            series1.Legend = "Legend1";
            series1.Name = "Range";
            series1.Points.Add(dataPoint1);
            series1.YValuesPerPoint = 2;
            series2.BorderWidth = 3;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.LawnGreen;
            series2.Legend = "Legend1";
            series2.Name = "AverageCurrent";
            series2.Points.Add(dataPoint2);
            series3.BorderWidth = 3;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series3.Color = System.Drawing.Color.OrangeRed;
            series3.Legend = "Legend1";
            series3.MarkerColor = System.Drawing.Color.White;
            series3.Name = "MaxCurrent";
            series3.Points.Add(dataPoint3);
            series4.BorderWidth = 3;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series4.Color = System.Drawing.Color.Cyan;
            series4.Legend = "Legend1";
            series4.Name = "MinCurrent";
            series4.Points.Add(dataPoint4);
            this.ChartI.Series.Add(series1);
            this.ChartI.Series.Add(series2);
            this.ChartI.Series.Add(series3);
            this.ChartI.Series.Add(series4);
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
            this.lbCurrentHover.Text = "Current [A]";
            this.lbCurrentHover.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            // energyProfileModelBindingSource
            // 
            this.energyProfileModelBindingSource.DataSource = typeof(MissionPlanner.Utilities.EnergyProfileModel);
            // 
            // currentStateBindingSource
            // 
            this.currentStateBindingSource.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // ConfigEnergyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panelExpImp);
            this.Controls.Add(this.panelHover);
            this.Controls.Add(this.panelCurrentConfiguration);
            this.Controls.Add(this.CB_EnableEnergyProfile);
            this.Controls.Add(this.label1);
            this.Name = "ConfigEnergyProfile";
            this.Size = new System.Drawing.Size(864, 669);
            this.Leave += new System.EventHandler(this.ConfigEnergyProfile_Leave);
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
            ((System.ComponentModel.ISupportInitialize)(this.energyProfileModelBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentStateBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private Controls.MyButton BtnPlot;
        private Label LbInfoCurrentMove;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox HoverDevTB;
        private Label lbCurrentHover;
        private Label lbVarianceCurrentHover;
        private TextBox HoverCrntTB;
        private ComboBox ComboBoxDeviation;
        private Label LabelDeviationCB;
        private BindingSource energyProfileModelBindingSource;
        private BindingSource currentStateBindingSource;
    }
}
