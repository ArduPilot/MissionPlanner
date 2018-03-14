using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AltitudeAngelWings.Models;

namespace MissionPlanner.Controls
{
    public class PropagationSettings : Form
    {
        private CheckBox chk_terrain;
        private CheckBox chk_rf;
        private CheckBox chk_dronedist;
        private CheckBox chk_homedist;
        private GroupBox groupBox1;
        private Label label91;
        private Label label89;
        private NumericUpDown Clearance;
        private Label label100;
        private Label label113;
        private Label label109;
        private ComboBox CMB_Rotational;
        private Label label90;
        private ComboBox CMB_Resolution;
        private Label label110;
        private ComboBox CMB_Angular;
        private Label label111;
        private NumericUpDown NUM_range;
        private Label label112;
        private NumericUpDown NUM_height;
        private Label label114;
        private NumericUpDown Tolerance;
        private CheckBox chk_ele;

        public PropagationSettings()
        {
            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            Clearance.Value = (decimal) Settings.Instance.GetFloat("Propagation_Clearance", 5);
            CMB_Resolution.Text = Settings.Instance.GetInt32("Propagation_Resolution", 4).ToString();
            CMB_Rotational.Text = Settings.Instance.GetInt32("Propagation_Rotational", 1).ToString();
            CMB_Angular.Text = Settings.Instance.GetInt32("Propagation_Converge", 1).ToString();
            NUM_range.Value = (decimal) Settings.Instance.GetFloat("Propagation_Range", 2.0f);
            NUM_height.Value = (decimal) Settings.Instance.GetFloat("Propagation_Height", 2.0f);
            Tolerance.Value = (decimal) Settings.Instance.GetFloat("Propagation_Tolerance", 0.8f);

            chk_ele.Checked = Maps.Propagation.ele_run;
            chk_terrain.Checked = Maps.Propagation.ter_run;
            chk_rf.Checked = Maps.Propagation.rf_run;
            chk_homedist.Checked = Maps.Propagation.home_kmleft;
            chk_dronedist.Checked = Maps.Propagation.drone_kmleft;
        }

        private void InitializeComponent()
        {
            this.chk_ele = new System.Windows.Forms.CheckBox();
            this.chk_terrain = new System.Windows.Forms.CheckBox();
            this.chk_rf = new System.Windows.Forms.CheckBox();
            this.chk_dronedist = new System.Windows.Forms.CheckBox();
            this.chk_homedist = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label91 = new System.Windows.Forms.Label();
            this.label89 = new System.Windows.Forms.Label();
            this.Clearance = new System.Windows.Forms.NumericUpDown();
            this.label100 = new System.Windows.Forms.Label();
            this.label113 = new System.Windows.Forms.Label();
            this.label109 = new System.Windows.Forms.Label();
            this.CMB_Rotational = new System.Windows.Forms.ComboBox();
            this.label90 = new System.Windows.Forms.Label();
            this.CMB_Resolution = new System.Windows.Forms.ComboBox();
            this.label110 = new System.Windows.Forms.Label();
            this.CMB_Angular = new System.Windows.Forms.ComboBox();
            this.label111 = new System.Windows.Forms.Label();
            this.NUM_range = new System.Windows.Forms.NumericUpDown();
            this.label112 = new System.Windows.Forms.Label();
            this.NUM_height = new System.Windows.Forms.NumericUpDown();
            this.label114 = new System.Windows.Forms.Label();
            this.Tolerance = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Clearance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_range)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tolerance)).BeginInit();
            this.SuspendLayout();
            // 
            // chk_ele
            // 
            this.chk_ele.AutoSize = true;
            this.chk_ele.Checked = true;
            this.chk_ele.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chk_ele.Location = new System.Drawing.Point(13, 13);
            this.chk_ele.Name = "chk_ele";
            this.chk_ele.Size = new System.Drawing.Size(70, 17);
            this.chk_ele.TabIndex = 0;
            this.chk_ele.Text = "Elevation";
            this.chk_ele.UseVisualStyleBackColor = true;
            this.chk_ele.CheckedChanged += new System.EventHandler(this.chk_ele_CheckedChanged);
            // 
            // chk_terrain
            // 
            this.chk_terrain.AutoSize = true;
            this.chk_terrain.Checked = true;
            this.chk_terrain.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chk_terrain.Location = new System.Drawing.Point(13, 36);
            this.chk_terrain.Name = "chk_terrain";
            this.chk_terrain.Size = new System.Drawing.Size(59, 17);
            this.chk_terrain.TabIndex = 1;
            this.chk_terrain.Text = "Terrain";
            this.chk_terrain.UseVisualStyleBackColor = true;
            this.chk_terrain.CheckedChanged += new System.EventHandler(this.chk_terrain_CheckedChanged);
            // 
            // chk_rf
            // 
            this.chk_rf.AutoSize = true;
            this.chk_rf.Checked = true;
            this.chk_rf.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chk_rf.Location = new System.Drawing.Point(13, 59);
            this.chk_rf.Name = "chk_rf";
            this.chk_rf.Size = new System.Drawing.Size(64, 17);
            this.chk_rf.TabIndex = 2;
            this.chk_rf.Text = "RF Map";
            this.chk_rf.UseVisualStyleBackColor = true;
            this.chk_rf.CheckedChanged += new System.EventHandler(this.chk_rf_CheckedChanged);
            // 
            // chk_dronedist
            // 
            this.chk_dronedist.AutoSize = true;
            this.chk_dronedist.Checked = true;
            this.chk_dronedist.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chk_dronedist.Location = new System.Drawing.Point(107, 36);
            this.chk_dronedist.Name = "chk_dronedist";
            this.chk_dronedist.Size = new System.Drawing.Size(97, 17);
            this.chk_dronedist.TabIndex = 3;
            this.chk_dronedist.Text = "Drone Dist Left";
            this.chk_dronedist.UseVisualStyleBackColor = true;
            this.chk_dronedist.CheckedChanged += new System.EventHandler(this.chk_dronedist_CheckedChanged);
            // 
            // chk_homedist
            // 
            this.chk_homedist.AutoSize = true;
            this.chk_homedist.Checked = true;
            this.chk_homedist.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chk_homedist.Location = new System.Drawing.Point(107, 13);
            this.chk_homedist.Name = "chk_homedist";
            this.chk_homedist.Size = new System.Drawing.Size(96, 17);
            this.chk_homedist.TabIndex = 4;
            this.chk_homedist.Text = "Home Dist Left";
            this.chk_homedist.UseVisualStyleBackColor = true;
            this.chk_homedist.CheckedChanged += new System.EventHandler(this.chk_homedist_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label91);
            this.groupBox1.Controls.Add(this.label89);
            this.groupBox1.Controls.Add(this.Clearance);
            this.groupBox1.Controls.Add(this.label100);
            this.groupBox1.Controls.Add(this.label113);
            this.groupBox1.Controls.Add(this.label109);
            this.groupBox1.Controls.Add(this.CMB_Rotational);
            this.groupBox1.Controls.Add(this.label90);
            this.groupBox1.Controls.Add(this.CMB_Resolution);
            this.groupBox1.Controls.Add(this.label110);
            this.groupBox1.Controls.Add(this.CMB_Angular);
            this.groupBox1.Controls.Add(this.label111);
            this.groupBox1.Controls.Add(this.NUM_range);
            this.groupBox1.Controls.Add(this.label112);
            this.groupBox1.Controls.Add(this.NUM_height);
            this.groupBox1.Controls.Add(this.label114);
            this.groupBox1.Controls.Add(this.Tolerance);
            this.groupBox1.Location = new System.Drawing.Point(13, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(674, 141);
            this.groupBox1.TabIndex = 130;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map Overlay";
            // 
            // label91
            // 
            this.label91.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label91.Location = new System.Drawing.Point(16, 51);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(66, 23);
            this.label91.TabIndex = 117;
            this.label91.Text = "Elevation";
            // 
            // label89
            // 
            this.label89.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label89.Location = new System.Drawing.Point(97, 22);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(76, 17);
            this.label89.TabIndex = 121;
            this.label89.Text = "Clearance [m]";
            // 
            // Clearance
            // 
            this.Clearance.DecimalPlaces = 1;
            this.Clearance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Clearance.Location = new System.Drawing.Point(174, 19);
            this.Clearance.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Clearance.Name = "Clearance";
            this.Clearance.Size = new System.Drawing.Size(56, 20);
            this.Clearance.TabIndex = 120;
            this.Clearance.ValueChanged += new System.EventHandler(this.Clearance_ValueChanged);
            // 
            // label100
            // 
            this.label100.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label100.Location = new System.Drawing.Point(16, 75);
            this.label100.Name = "label100";
            this.label100.Size = new System.Drawing.Size(69, 23);
            this.label100.TabIndex = 122;
            this.label100.Text = "Propagation";
            // 
            // label113
            // 
            this.label113.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label113.Location = new System.Drawing.Point(18, 101);
            this.label113.Name = "label113";
            this.label113.Size = new System.Drawing.Size(57, 23);
            this.label113.TabIndex = 127;
            this.label113.Text = "Kmleft";
            // 
            // label109
            // 
            this.label109.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label109.Location = new System.Drawing.Point(97, 75);
            this.label109.Name = "label109";
            this.label109.Size = new System.Drawing.Size(71, 23);
            this.label109.TabIndex = 123;
            this.label109.Text = "Azimuth Step";
            // 
            // CMB_Rotational
            // 
            this.CMB_Rotational.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_Rotational.FormattingEnabled = true;
            this.CMB_Rotational.Items.AddRange(new object[] {
            "0.5",
            "1",
            "2",
            "5",
            "10"});
            this.CMB_Rotational.Location = new System.Drawing.Point(174, 72);
            this.CMB_Rotational.Name = "CMB_Rotational";
            this.CMB_Rotational.Size = new System.Drawing.Size(40, 21);
            this.CMB_Rotational.TabIndex = 119;
            this.CMB_Rotational.SelectedIndexChanged += new System.EventHandler(this.CMB_Rotational_SelectedIndexChanged);
            // 
            // label90
            // 
            this.label90.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label90.Location = new System.Drawing.Point(97, 48);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(63, 17);
            this.label90.TabIndex = 119;
            this.label90.Text = "Resolution";
            // 
            // CMB_Resolution
            // 
            this.CMB_Resolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_Resolution.FormattingEnabled = true;
            this.CMB_Resolution.Items.AddRange(new object[] {
            "2",
            "4",
            "6",
            "8",
            "10"});
            this.CMB_Resolution.Location = new System.Drawing.Point(174, 45);
            this.CMB_Resolution.Name = "CMB_Resolution";
            this.CMB_Resolution.Size = new System.Drawing.Size(40, 21);
            this.CMB_Resolution.TabIndex = 118;
            this.CMB_Resolution.SelectedIndexChanged += new System.EventHandler(this.CMB_Resolution_SelectedIndexChanged);
            // 
            // label110
            // 
            this.label110.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label110.Location = new System.Drawing.Point(236, 75);
            this.label110.Name = "label110";
            this.label110.Size = new System.Drawing.Size(75, 23);
            this.label110.TabIndex = 124;
            this.label110.Text = "Convergance";
            // 
            // CMB_Angular
            // 
            this.CMB_Angular.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_Angular.FormattingEnabled = true;
            this.CMB_Angular.Items.AddRange(new object[] {
            "0",
            "1",
            "5",
            "10",
            "15"});
            this.CMB_Angular.Location = new System.Drawing.Point(317, 72);
            this.CMB_Angular.Name = "CMB_Angular";
            this.CMB_Angular.Size = new System.Drawing.Size(40, 21);
            this.CMB_Angular.TabIndex = 0;
            this.CMB_Angular.SelectedIndexChanged += new System.EventHandler(this.CMB_Angular_SelectedIndexChanged);
            // 
            // label111
            // 
            this.label111.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label111.Location = new System.Drawing.Point(372, 75);
            this.label111.Name = "label111";
            this.label111.Size = new System.Drawing.Size(64, 23);
            this.label111.TabIndex = 125;
            this.label111.Text = "Range [Km]";
            // 
            // NUM_range
            // 
            this.NUM_range.DecimalPlaces = 1;
            this.NUM_range.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUM_range.Location = new System.Drawing.Point(442, 73);
            this.NUM_range.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.NUM_range.Name = "NUM_range";
            this.NUM_range.Size = new System.Drawing.Size(56, 20);
            this.NUM_range.TabIndex = 121;
            this.NUM_range.ValueChanged += new System.EventHandler(this.NUM_range_ValueChanged);
            // 
            // label112
            // 
            this.label112.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label112.Location = new System.Drawing.Point(511, 75);
            this.label112.Name = "label112";
            this.label112.Size = new System.Drawing.Size(84, 23);
            this.label112.TabIndex = 126;
            this.label112.Text = "Base Height [m]";
            // 
            // NUM_height
            // 
            this.NUM_height.DecimalPlaces = 1;
            this.NUM_height.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUM_height.Location = new System.Drawing.Point(601, 73);
            this.NUM_height.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.NUM_height.Name = "NUM_height";
            this.NUM_height.Size = new System.Drawing.Size(46, 20);
            this.NUM_height.TabIndex = 122;
            this.NUM_height.ValueChanged += new System.EventHandler(this.NUM_height_ValueChanged);
            // 
            // label114
            // 
            this.label114.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label114.Location = new System.Drawing.Point(97, 101);
            this.label114.Name = "label114";
            this.label114.Size = new System.Drawing.Size(63, 23);
            this.label114.TabIndex = 128;
            this.label114.Text = "Tolerance";
            // 
            // Tolerance
            // 
            this.Tolerance.DecimalPlaces = 1;
            this.Tolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Tolerance.Location = new System.Drawing.Point(174, 99);
            this.Tolerance.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Tolerance.Name = "Tolerance";
            this.Tolerance.Size = new System.Drawing.Size(56, 20);
            this.Tolerance.TabIndex = 123;
            this.Tolerance.ValueChanged += new System.EventHandler(this.Tolerance_ValueChanged);
            // 
            // PropagationSettings
            // 
            this.ClientSize = new System.Drawing.Size(694, 230);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chk_homedist);
            this.Controls.Add(this.chk_dronedist);
            this.Controls.Add(this.chk_rf);
            this.Controls.Add(this.chk_terrain);
            this.Controls.Add(this.chk_ele);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PropagationSettings";
            this.Text = "Propagation Settings";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Clearance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_range)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tolerance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void chk_ele_CheckedChanged(object sender, EventArgs e)
        {
            Maps.Propagation.ele_run = chk_ele.Checked;
        }

        private void chk_terrain_CheckedChanged(object sender, EventArgs e)
        {
            Maps.Propagation.ter_run = chk_terrain.Checked;
        }

        private void chk_rf_CheckedChanged(object sender, EventArgs e)
        {
            Maps.Propagation.rf_run = chk_rf.Checked;
        }

        private void chk_homedist_CheckedChanged(object sender, EventArgs e)
        {
            Maps.Propagation.home_kmleft = chk_homedist.Checked;
        }

        private void chk_dronedist_CheckedChanged(object sender, EventArgs e)
        {
            Maps.Propagation.drone_kmleft = chk_dronedist.Checked;
        }

        private void Clearance_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Clearance"] = Clearance.Value.ToString();
        }

        private void CMB_Resolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Resolution"] = CMB_Resolution.Text;
        }

        private void CMB_Rotational_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Rotational"] = CMB_Rotational.Text;
        }

        private void CMB_Angular_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Converge"] = CMB_Angular.Text;
        }

        private void NUM_range_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Range"] = NUM_range.Value.ToString();
        }

        private void NUM_height_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Height"] = NUM_height.Value.ToString();
        }

        private void Tolerance_ValueChanged(object sender, EventArgs e)
        {
            Settings.Instance["Propagation_Tolerance"] = Tolerance.Value.ToString();
        }
    }
}