using MissionPlanner.Utilities;
using System;
using System.Windows.Forms;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropagationSettings));
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
            resources.ApplyResources(this.chk_ele, "chk_ele");
            this.chk_ele.Checked = true;
            this.chk_ele.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chk_ele.Name = "chk_ele";
            this.chk_ele.UseVisualStyleBackColor = true;
            this.chk_ele.CheckedChanged += new System.EventHandler(this.chk_ele_CheckedChanged);
            // 
            // chk_terrain
            // 
            resources.ApplyResources(this.chk_terrain, "chk_terrain");
            this.chk_terrain.Checked = true;
            this.chk_terrain.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chk_terrain.Name = "chk_terrain";
            this.chk_terrain.UseVisualStyleBackColor = true;
            this.chk_terrain.CheckedChanged += new System.EventHandler(this.chk_terrain_CheckedChanged);
            // 
            // chk_rf
            // 
            resources.ApplyResources(this.chk_rf, "chk_rf");
            this.chk_rf.Checked = true;
            this.chk_rf.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chk_rf.Name = "chk_rf";
            this.chk_rf.UseVisualStyleBackColor = true;
            this.chk_rf.CheckedChanged += new System.EventHandler(this.chk_rf_CheckedChanged);
            // 
            // chk_dronedist
            // 
            resources.ApplyResources(this.chk_dronedist, "chk_dronedist");
            this.chk_dronedist.Checked = true;
            this.chk_dronedist.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chk_dronedist.Name = "chk_dronedist";
            this.chk_dronedist.UseVisualStyleBackColor = true;
            this.chk_dronedist.CheckedChanged += new System.EventHandler(this.chk_dronedist_CheckedChanged);
            // 
            // chk_homedist
            // 
            resources.ApplyResources(this.chk_homedist, "chk_homedist");
            this.chk_homedist.Checked = true;
            this.chk_homedist.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chk_homedist.Name = "chk_homedist";
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
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label91
            // 
            resources.ApplyResources(this.label91, "label91");
            this.label91.Name = "label91";
            // 
            // label89
            // 
            resources.ApplyResources(this.label89, "label89");
            this.label89.Name = "label89";
            // 
            // Clearance
            // 
            this.Clearance.DecimalPlaces = 1;
            this.Clearance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.Clearance, "Clearance");
            this.Clearance.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Clearance.Name = "Clearance";
            this.Clearance.ValueChanged += new System.EventHandler(this.Clearance_ValueChanged);
            // 
            // label100
            // 
            resources.ApplyResources(this.label100, "label100");
            this.label100.Name = "label100";
            // 
            // label113
            // 
            resources.ApplyResources(this.label113, "label113");
            this.label113.Name = "label113";
            // 
            // label109
            // 
            resources.ApplyResources(this.label109, "label109");
            this.label109.Name = "label109";
            // 
            // CMB_Rotational
            // 
            this.CMB_Rotational.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_Rotational.FormattingEnabled = true;
            this.CMB_Rotational.Items.AddRange(new object[] {
            resources.GetString("CMB_Rotational.Items"),
            resources.GetString("CMB_Rotational.Items1"),
            resources.GetString("CMB_Rotational.Items2"),
            resources.GetString("CMB_Rotational.Items3"),
            resources.GetString("CMB_Rotational.Items4")});
            resources.ApplyResources(this.CMB_Rotational, "CMB_Rotational");
            this.CMB_Rotational.Name = "CMB_Rotational";
            this.CMB_Rotational.SelectedIndexChanged += new System.EventHandler(this.CMB_Rotational_SelectedIndexChanged);
            // 
            // label90
            // 
            resources.ApplyResources(this.label90, "label90");
            this.label90.Name = "label90";
            // 
            // CMB_Resolution
            // 
            this.CMB_Resolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_Resolution.FormattingEnabled = true;
            this.CMB_Resolution.Items.AddRange(new object[] {
            resources.GetString("CMB_Resolution.Items"),
            resources.GetString("CMB_Resolution.Items1"),
            resources.GetString("CMB_Resolution.Items2"),
            resources.GetString("CMB_Resolution.Items3"),
            resources.GetString("CMB_Resolution.Items4")});
            resources.ApplyResources(this.CMB_Resolution, "CMB_Resolution");
            this.CMB_Resolution.Name = "CMB_Resolution";
            this.CMB_Resolution.SelectedIndexChanged += new System.EventHandler(this.CMB_Resolution_SelectedIndexChanged);
            // 
            // label110
            // 
            resources.ApplyResources(this.label110, "label110");
            this.label110.Name = "label110";
            // 
            // CMB_Angular
            // 
            this.CMB_Angular.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_Angular.FormattingEnabled = true;
            this.CMB_Angular.Items.AddRange(new object[] {
            resources.GetString("CMB_Angular.Items"),
            resources.GetString("CMB_Angular.Items1"),
            resources.GetString("CMB_Angular.Items2"),
            resources.GetString("CMB_Angular.Items3"),
            resources.GetString("CMB_Angular.Items4")});
            resources.ApplyResources(this.CMB_Angular, "CMB_Angular");
            this.CMB_Angular.Name = "CMB_Angular";
            this.CMB_Angular.SelectedIndexChanged += new System.EventHandler(this.CMB_Angular_SelectedIndexChanged);
            // 
            // label111
            // 
            resources.ApplyResources(this.label111, "label111");
            this.label111.Name = "label111";
            // 
            // NUM_range
            // 
            this.NUM_range.DecimalPlaces = 1;
            this.NUM_range.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NUM_range, "NUM_range");
            this.NUM_range.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.NUM_range.Name = "NUM_range";
            this.NUM_range.ValueChanged += new System.EventHandler(this.NUM_range_ValueChanged);
            // 
            // label112
            // 
            resources.ApplyResources(this.label112, "label112");
            this.label112.Name = "label112";
            // 
            // NUM_height
            // 
            this.NUM_height.DecimalPlaces = 1;
            this.NUM_height.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NUM_height, "NUM_height");
            this.NUM_height.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.NUM_height.Name = "NUM_height";
            this.NUM_height.ValueChanged += new System.EventHandler(this.NUM_height_ValueChanged);
            // 
            // label114
            // 
            resources.ApplyResources(this.label114, "label114");
            this.label114.Name = "label114";
            // 
            // Tolerance
            // 
            this.Tolerance.DecimalPlaces = 1;
            this.Tolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.Tolerance, "Tolerance");
            this.Tolerance.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Tolerance.Name = "Tolerance";
            this.Tolerance.ValueChanged += new System.EventHandler(this.Tolerance_ValueChanged);
            // 
            // PropagationSettings
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chk_homedist);
            this.Controls.Add(this.chk_dronedist);
            this.Controls.Add(this.chk_rf);
            this.Controls.Add(this.chk_terrain);
            this.Controls.Add(this.chk_ele);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PropagationSettings";
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