using MissionPlanner.Controls;
namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigTradHeli
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigTradHeli));
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fbl_modeFBL = new MissionPlanner.Controls.MavlinkCheckBox();
            this.H_SWASH_TYPE = new System.Windows.Forms.RadioButton();
            this.CCPM = new System.Windows.Forms.RadioButton();
            this.label41 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.mavlinkNumericUpDowntailspeed = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.mavlinkComboBoxTailType = new MissionPlanner.Controls.MavlinkComboBox();
            this.label46 = new System.Windows.Forms.Label();
            this.H_GYR_GAIN = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this.H_COL_MIN = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.H_COL_MID = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.H_COL_MAX = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label24 = new System.Windows.Forms.Label();
            this.HS4_MIN = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.HS4_MAX = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label40 = new System.Windows.Forms.Label();
            this.HS4_REV = new MissionPlanner.Controls.MavlinkCheckBox();
            this.label43 = new System.Windows.Forms.Label();
            this.HS4_TRIM = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label44 = new System.Windows.Forms.Label();
            this.HS3_TRIM = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.HS2_TRIM = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.HS1_TRIM = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.H_SV3_POS = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.H_SV2_POS = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.H_SV1_POS = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.HS3_REV = new MissionPlanner.Controls.MavlinkCheckBox();
            this.HS2_REV = new MissionPlanner.Controls.MavlinkCheckBox();
            this.HS1_REV = new MissionPlanner.Controls.MavlinkCheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.H_COLYAW = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownH_RSC_CRITICAL = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownrunuptime = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.mavlinkudH_RSC_RATE = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.mavlinkudH_RSC_SETPOINT = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.H_RSC_MODE = new MissionPlanner.Controls.MavlinkComboBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.mavlinkCheckBoxatc_piro_comp = new MissionPlanner.Controls.MavlinkCheckBox();
            this.mavlinkNumericUpDownh_sv_test = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavlinkNumericUpDownatc_hovr_rol_trm = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavlinkNumericUpDownh_cyc_max = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownland_col_min = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.HS4 = new MissionPlanner.Controls.HorizontalProgressBar2();
            this.currentStateBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.HS3 = new MissionPlanner.Controls.VerticalProgressBar2();
            this.Gservoloc = new AGaugeApp.AGauge();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.myButtonH_SV_MANtest = new MissionPlanner.Controls.MyButton();
            this.myButtonH_SV_MANmin = new MissionPlanner.Controls.MyButton();
            this.myButtonH_SV_MANzero = new MissionPlanner.Controls.MyButton();
            this.myButtonH_SV_MANmax = new MissionPlanner.Controls.MyButton();
            this.myButtonH_SV_MANmanual = new MissionPlanner.Controls.MyButton();
            this.myButtonH_SV_MAN = new MissionPlanner.Controls.MyButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.h_rsc_rev = new MissionPlanner.Controls.MavlinkCheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownh_rsc_max = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownh_rsc_min = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownh_rsc_power_high = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownh_rsc_power_low = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownh_rsc_idle = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.label31 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownim_acro_col_exp = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label30 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownim_stab_col_3 = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavlinkNumericUpDownim_stab_col_4 = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownim_stab_col_2 = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.label29 = new System.Windows.Forms.Label();
            this.mavlinkNumericUpDownim_stab_col_1 = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.mavlinkNumericUpDownH_PHANG = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.groupBox5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDowntailspeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_GYR_GAIN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_COL_MIN)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.H_COL_MID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_COL_MAX)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HS4_MIN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HS4_MAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HS4_TRIM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HS3_TRIM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HS2_TRIM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HS1_TRIM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_SV3_POS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_SV2_POS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_SV1_POS)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.H_COLYAW)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownH_RSC_CRITICAL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownrunuptime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkudH_RSC_RATE)).BeginInit();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkudH_RSC_SETPOINT)).BeginInit();
            this.groupBox9.SuspendLayout();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_sv_test)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownatc_hovr_rol_trm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_cyc_max)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownland_col_min)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentStateBindingSource)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_rsc_max)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_rsc_min)).BeginInit();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_rsc_power_high)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_rsc_power_low)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_rsc_idle)).BeginInit();
            this.groupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownim_acro_col_exp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownim_stab_col_3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownim_stab_col_4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownim_stab_col_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownim_stab_col_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownH_PHANG)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.panel1);
            this.groupBox5.Controls.Add(this.H_SWASH_TYPE);
            this.groupBox5.Controls.Add(this.CCPM);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fbl_modeFBL);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // fbl_modeFBL
            // 
            resources.ApplyResources(this.fbl_modeFBL, "fbl_modeFBL");
            this.fbl_modeFBL.Name = "fbl_modeFBL";
            this.fbl_modeFBL.OffValue = 0D;
            this.fbl_modeFBL.OnValue = 1D;
            this.fbl_modeFBL.ParamName = null;
            this.fbl_modeFBL.UseVisualStyleBackColor = true;
            // 
            // H_SWASH_TYPE
            // 
            resources.ApplyResources(this.H_SWASH_TYPE, "H_SWASH_TYPE");
            this.H_SWASH_TYPE.Name = "H_SWASH_TYPE";
            this.H_SWASH_TYPE.TabStop = true;
            this.H_SWASH_TYPE.UseVisualStyleBackColor = true;
            this.H_SWASH_TYPE.CheckedChanged += new System.EventHandler(this.H_SWASH_TYPE_CheckedChanged);
            // 
            // CCPM
            // 
            resources.ApplyResources(this.CCPM, "CCPM");
            this.CCPM.Name = "CCPM";
            this.CCPM.TabStop = true;
            this.CCPM.UseVisualStyleBackColor = true;
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.Name = "label41";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.mavlinkNumericUpDowntailspeed);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.mavlinkComboBoxTailType);
            this.groupBox3.Controls.Add(this.label46);
            this.groupBox3.Controls.Add(this.H_GYR_GAIN);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // mavlinkNumericUpDowntailspeed
            // 
            resources.ApplyResources(this.mavlinkNumericUpDowntailspeed, "mavlinkNumericUpDowntailspeed");
            this.mavlinkNumericUpDowntailspeed.Max = 1F;
            this.mavlinkNumericUpDowntailspeed.Min = 0F;
            this.mavlinkNumericUpDowntailspeed.Name = "mavlinkNumericUpDowntailspeed";
            this.mavlinkNumericUpDowntailspeed.ParamName = null;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // mavlinkComboBoxTailType
            // 
            this.mavlinkComboBoxTailType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.mavlinkComboBoxTailType, "mavlinkComboBoxTailType");
            this.mavlinkComboBoxTailType.FormattingEnabled = true;
            this.mavlinkComboBoxTailType.Name = "mavlinkComboBoxTailType";
            this.mavlinkComboBoxTailType.ParamName = null;
            this.mavlinkComboBoxTailType.SubControl = null;
            // 
            // label46
            // 
            resources.ApplyResources(this.label46, "label46");
            this.label46.Name = "label46";
            // 
            // H_GYR_GAIN
            // 
            resources.ApplyResources(this.H_GYR_GAIN, "H_GYR_GAIN");
            this.H_GYR_GAIN.Max = 1F;
            this.H_GYR_GAIN.Min = 0F;
            this.H_GYR_GAIN.Name = "H_GYR_GAIN";
            this.H_GYR_GAIN.ParamName = null;
            this.H_GYR_GAIN.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.H_GYR_GAIN.Validating += new System.ComponentModel.CancelEventHandler(this.GYR_GAIN__Validating);
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // H_COL_MIN
            // 
            resources.ApplyResources(this.H_COL_MIN, "H_COL_MIN");
            this.H_COL_MIN.Max = 1F;
            this.H_COL_MIN.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.H_COL_MIN.Min = 0F;
            this.H_COL_MIN.Name = "H_COL_MIN";
            this.H_COL_MIN.ParamName = null;
            this.H_COL_MIN.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.H_COL_MIN.Validating += new System.ComponentModel.CancelEventHandler(this.PWM_Validating);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label41);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.H_COL_MIN);
            this.groupBox1.Controls.Add(this.H_COL_MID);
            this.groupBox1.Controls.Add(this.H_COL_MAX);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // H_COL_MID
            // 
            resources.ApplyResources(this.H_COL_MID, "H_COL_MID");
            this.H_COL_MID.Max = 1F;
            this.H_COL_MID.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.H_COL_MID.Min = 0F;
            this.H_COL_MID.Name = "H_COL_MID";
            this.H_COL_MID.ParamName = null;
            this.H_COL_MID.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.H_COL_MID.Validating += new System.ComponentModel.CancelEventHandler(this.PWM_Validating);
            // 
            // H_COL_MAX
            // 
            resources.ApplyResources(this.H_COL_MAX, "H_COL_MAX");
            this.H_COL_MAX.Max = 1F;
            this.H_COL_MAX.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.H_COL_MAX.Min = 0F;
            this.H_COL_MAX.Name = "H_COL_MAX";
            this.H_COL_MAX.ParamName = null;
            this.H_COL_MAX.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.H_COL_MAX.Enter += new System.EventHandler(this.COL_MAX__Enter);
            this.H_COL_MAX.Leave += new System.EventHandler(this.COL_MAX__Leave);
            this.H_COL_MAX.Validating += new System.ComponentModel.CancelEventHandler(this.PWM_Validating);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Controls.Add(this.HS4_MIN);
            this.groupBox2.Controls.Add(this.HS4_MAX);
            this.groupBox2.Controls.Add(this.label40);
            this.groupBox2.Controls.Add(this.HS4_REV);
            this.groupBox2.Controls.Add(this.label43);
            this.groupBox2.Controls.Add(this.HS4_TRIM);
            this.groupBox2.Controls.Add(this.label44);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // HS4_MIN
            // 
            resources.ApplyResources(this.HS4_MIN, "HS4_MIN");
            this.HS4_MIN.Max = 1F;
            this.HS4_MIN.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.HS4_MIN.Min = 0F;
            this.HS4_MIN.Name = "HS4_MIN";
            this.HS4_MIN.ParamName = null;
            this.HS4_MIN.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.HS4_MIN.Validating += new System.ComponentModel.CancelEventHandler(this.PWM_Validating);
            // 
            // HS4_MAX
            // 
            resources.ApplyResources(this.HS4_MAX, "HS4_MAX");
            this.HS4_MAX.Max = 1F;
            this.HS4_MAX.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.HS4_MAX.Min = 0F;
            this.HS4_MAX.Name = "HS4_MAX";
            this.HS4_MAX.ParamName = null;
            this.HS4_MAX.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.HS4_MAX.Enter += new System.EventHandler(this.HS4_MAX_Enter);
            this.HS4_MAX.Leave += new System.EventHandler(this.HS4_MAX_Leave);
            this.HS4_MAX.Validating += new System.ComponentModel.CancelEventHandler(this.PWM_Validating);
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.Name = "label40";
            // 
            // HS4_REV
            // 
            resources.ApplyResources(this.HS4_REV, "HS4_REV");
            this.HS4_REV.Name = "HS4_REV";
            this.HS4_REV.OffValue = 0D;
            this.HS4_REV.OnValue = 1D;
            this.HS4_REV.ParamName = null;
            this.HS4_REV.UseVisualStyleBackColor = true;
            // 
            // label43
            // 
            resources.ApplyResources(this.label43, "label43");
            this.label43.Name = "label43";
            // 
            // HS4_TRIM
            // 
            resources.ApplyResources(this.HS4_TRIM, "HS4_TRIM");
            this.HS4_TRIM.Max = 1F;
            this.HS4_TRIM.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.HS4_TRIM.Min = 0F;
            this.HS4_TRIM.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.HS4_TRIM.Name = "HS4_TRIM";
            this.HS4_TRIM.ParamName = null;
            this.HS4_TRIM.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            // 
            // label44
            // 
            resources.ApplyResources(this.label44, "label44");
            this.label44.Name = "label44";
            // 
            // HS3_TRIM
            // 
            resources.ApplyResources(this.HS3_TRIM, "HS3_TRIM");
            this.HS3_TRIM.Max = 1F;
            this.HS3_TRIM.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.HS3_TRIM.Min = 0F;
            this.HS3_TRIM.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.HS3_TRIM.Name = "HS3_TRIM";
            this.HS3_TRIM.ParamName = null;
            this.HS3_TRIM.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            // 
            // HS2_TRIM
            // 
            resources.ApplyResources(this.HS2_TRIM, "HS2_TRIM");
            this.HS2_TRIM.Max = 1F;
            this.HS2_TRIM.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.HS2_TRIM.Min = 0F;
            this.HS2_TRIM.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.HS2_TRIM.Name = "HS2_TRIM";
            this.HS2_TRIM.ParamName = null;
            this.HS2_TRIM.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            // 
            // HS1_TRIM
            // 
            resources.ApplyResources(this.HS1_TRIM, "HS1_TRIM");
            this.HS1_TRIM.Max = 1F;
            this.HS1_TRIM.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.HS1_TRIM.Min = 0F;
            this.HS1_TRIM.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.HS1_TRIM.Name = "HS1_TRIM";
            this.HS1_TRIM.ParamName = null;
            this.HS1_TRIM.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Name = "label39";
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.Name = "label38";
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.Name = "label37";
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.Name = "label36";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // H_SV3_POS
            // 
            resources.ApplyResources(this.H_SV3_POS, "H_SV3_POS");
            this.H_SV3_POS.Max = 1F;
            this.H_SV3_POS.Min = 0F;
            this.H_SV3_POS.Name = "H_SV3_POS";
            this.H_SV3_POS.ParamName = null;
            this.H_SV3_POS.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.H_SV3_POS.Validating += new System.ComponentModel.CancelEventHandler(this.TXT_srvpos3_Validating);
            // 
            // H_SV2_POS
            // 
            resources.ApplyResources(this.H_SV2_POS, "H_SV2_POS");
            this.H_SV2_POS.Max = 1F;
            this.H_SV2_POS.Min = 0F;
            this.H_SV2_POS.Name = "H_SV2_POS";
            this.H_SV2_POS.ParamName = null;
            this.H_SV2_POS.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.H_SV2_POS.Validating += new System.ComponentModel.CancelEventHandler(this.TXT_srvpos2_Validating);
            // 
            // H_SV1_POS
            // 
            resources.ApplyResources(this.H_SV1_POS, "H_SV1_POS");
            this.H_SV1_POS.Max = 1F;
            this.H_SV1_POS.Min = 0F;
            this.H_SV1_POS.Name = "H_SV1_POS";
            this.H_SV1_POS.ParamName = null;
            this.H_SV1_POS.Validating += new System.ComponentModel.CancelEventHandler(this.TXT_srvpos1_Validating);
            // 
            // HS3_REV
            // 
            resources.ApplyResources(this.HS3_REV, "HS3_REV");
            this.HS3_REV.Name = "HS3_REV";
            this.HS3_REV.OffValue = 0D;
            this.HS3_REV.OnValue = 1D;
            this.HS3_REV.ParamName = null;
            this.HS3_REV.UseVisualStyleBackColor = true;
            // 
            // HS2_REV
            // 
            resources.ApplyResources(this.HS2_REV, "HS2_REV");
            this.HS2_REV.Name = "HS2_REV";
            this.HS2_REV.OffValue = 0D;
            this.HS2_REV.OnValue = 1D;
            this.HS2_REV.ParamName = null;
            this.HS2_REV.UseVisualStyleBackColor = true;
            // 
            // HS1_REV
            // 
            resources.ApplyResources(this.HS1_REV, "HS1_REV");
            this.HS1_REV.Name = "HS1_REV";
            this.HS1_REV.OffValue = 0D;
            this.HS1_REV.OnValue = 1D;
            this.HS1_REV.ParamName = null;
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.H_COLYAW);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // H_COLYAW
            // 
            this.H_COLYAW.DecimalPlaces = 2;
            resources.ApplyResources(this.H_COLYAW, "H_COLYAW");
            this.H_COLYAW.Max = 1F;
            this.H_COLYAW.Min = 0F;
            this.H_COLYAW.Name = "H_COLYAW";
            this.H_COLYAW.ParamName = null;
            this.H_COLYAW.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Controls.Add(this.mavlinkNumericUpDownH_RSC_CRITICAL);
            this.groupBox7.Controls.Add(this.label7);
            this.groupBox7.Controls.Add(this.mavlinkNumericUpDownrunuptime);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Controls.Add(this.mavlinkudH_RSC_RATE);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // mavlinkNumericUpDownH_RSC_CRITICAL
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownH_RSC_CRITICAL, "mavlinkNumericUpDownH_RSC_CRITICAL");
            this.mavlinkNumericUpDownH_RSC_CRITICAL.Max = 1F;
            this.mavlinkNumericUpDownH_RSC_CRITICAL.Min = 0F;
            this.mavlinkNumericUpDownH_RSC_CRITICAL.Name = "mavlinkNumericUpDownH_RSC_CRITICAL";
            this.mavlinkNumericUpDownH_RSC_CRITICAL.ParamName = null;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // mavlinkNumericUpDownrunuptime
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownrunuptime, "mavlinkNumericUpDownrunuptime");
            this.mavlinkNumericUpDownrunuptime.Max = 1F;
            this.mavlinkNumericUpDownrunuptime.Min = 0F;
            this.mavlinkNumericUpDownrunuptime.Name = "mavlinkNumericUpDownrunuptime";
            this.mavlinkNumericUpDownrunuptime.ParamName = null;
            this.mavlinkNumericUpDownrunuptime.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // mavlinkudH_RSC_RATE
            // 
            resources.ApplyResources(this.mavlinkudH_RSC_RATE, "mavlinkudH_RSC_RATE");
            this.mavlinkudH_RSC_RATE.Max = 1F;
            this.mavlinkudH_RSC_RATE.Min = 0F;
            this.mavlinkudH_RSC_RATE.Name = "mavlinkudH_RSC_RATE";
            this.mavlinkudH_RSC_RATE.ParamName = null;
            this.mavlinkudH_RSC_RATE.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.mavlinkudH_RSC_SETPOINT);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // mavlinkudH_RSC_SETPOINT
            // 
            resources.ApplyResources(this.mavlinkudH_RSC_SETPOINT, "mavlinkudH_RSC_SETPOINT");
            this.mavlinkudH_RSC_SETPOINT.Max = 1F;
            this.mavlinkudH_RSC_SETPOINT.Min = 0F;
            this.mavlinkudH_RSC_SETPOINT.Name = "mavlinkudH_RSC_SETPOINT";
            this.mavlinkudH_RSC_SETPOINT.ParamName = null;
            this.mavlinkudH_RSC_SETPOINT.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.H_RSC_MODE);
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // H_RSC_MODE
            // 
            this.H_RSC_MODE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.H_RSC_MODE, "H_RSC_MODE");
            this.H_RSC_MODE.FormattingEnabled = true;
            this.H_RSC_MODE.Name = "H_RSC_MODE";
            this.H_RSC_MODE.ParamName = null;
            this.H_RSC_MODE.SubControl = null;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.label11);
            this.groupBox10.Controls.Add(this.label10);
            this.groupBox10.Controls.Add(this.label9);
            this.groupBox10.Controls.Add(this.label8);
            this.groupBox10.Controls.Add(this.mavlinkCheckBoxatc_piro_comp);
            this.groupBox10.Controls.Add(this.mavlinkNumericUpDownh_sv_test);
            this.groupBox10.Controls.Add(this.mavlinkNumericUpDownatc_hovr_rol_trm);
            this.groupBox10.Controls.Add(this.mavlinkNumericUpDownh_cyc_max);
            this.groupBox10.Controls.Add(this.label4);
            this.groupBox10.Controls.Add(this.mavlinkNumericUpDownland_col_min);
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // mavlinkCheckBoxatc_piro_comp
            // 
            resources.ApplyResources(this.mavlinkCheckBoxatc_piro_comp, "mavlinkCheckBoxatc_piro_comp");
            this.mavlinkCheckBoxatc_piro_comp.Name = "mavlinkCheckBoxatc_piro_comp";
            this.mavlinkCheckBoxatc_piro_comp.OffValue = 0D;
            this.mavlinkCheckBoxatc_piro_comp.OnValue = 1D;
            this.mavlinkCheckBoxatc_piro_comp.ParamName = null;
            this.mavlinkCheckBoxatc_piro_comp.UseVisualStyleBackColor = true;
            // 
            // mavlinkNumericUpDownh_sv_test
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownh_sv_test, "mavlinkNumericUpDownh_sv_test");
            this.mavlinkNumericUpDownh_sv_test.Max = 1F;
            this.mavlinkNumericUpDownh_sv_test.Min = 0F;
            this.mavlinkNumericUpDownh_sv_test.Name = "mavlinkNumericUpDownh_sv_test";
            this.mavlinkNumericUpDownh_sv_test.ParamName = null;
            this.mavlinkNumericUpDownh_sv_test.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // mavlinkNumericUpDownatc_hovr_rol_trm
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownatc_hovr_rol_trm, "mavlinkNumericUpDownatc_hovr_rol_trm");
            this.mavlinkNumericUpDownatc_hovr_rol_trm.Max = 1F;
            this.mavlinkNumericUpDownatc_hovr_rol_trm.Min = 0F;
            this.mavlinkNumericUpDownatc_hovr_rol_trm.Name = "mavlinkNumericUpDownatc_hovr_rol_trm";
            this.mavlinkNumericUpDownatc_hovr_rol_trm.ParamName = null;
            this.mavlinkNumericUpDownatc_hovr_rol_trm.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // mavlinkNumericUpDownh_cyc_max
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownh_cyc_max, "mavlinkNumericUpDownh_cyc_max");
            this.mavlinkNumericUpDownh_cyc_max.Max = 1F;
            this.mavlinkNumericUpDownh_cyc_max.Min = 0F;
            this.mavlinkNumericUpDownh_cyc_max.Name = "mavlinkNumericUpDownh_cyc_max";
            this.mavlinkNumericUpDownh_cyc_max.ParamName = null;
            this.mavlinkNumericUpDownh_cyc_max.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // mavlinkNumericUpDownland_col_min
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownland_col_min, "mavlinkNumericUpDownland_col_min");
            this.mavlinkNumericUpDownland_col_min.Max = 1F;
            this.mavlinkNumericUpDownland_col_min.Min = 0F;
            this.mavlinkNumericUpDownland_col_min.Name = "mavlinkNumericUpDownland_col_min";
            this.mavlinkNumericUpDownland_col_min.ParamName = null;
            this.mavlinkNumericUpDownland_col_min.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // HS4
            // 
            this.HS4.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.HS4.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.HS4.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.currentStateBindingSource, "ch4in", true));
            this.HS4.DisplayScale = 1F;
            this.HS4.DrawLabel = true;
            this.HS4.Label = "Rudder";
            resources.ApplyResources(this.HS4, "HS4");
            this.HS4.Maximum = 2200;
            this.HS4.maxline = 0;
            this.HS4.Minimum = 800;
            this.HS4.minline = 0;
            this.HS4.Name = "HS4";
            this.HS4.Value = 1500;
            this.HS4.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.HS4.Paint += new System.Windows.Forms.PaintEventHandler(this.HS4_Paint);
            // 
            // currentStateBindingSource
            // 
            this.currentStateBindingSource.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // HS3
            // 
            this.HS3.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(68)))), ((int)(((byte)(69)))));
            this.HS3.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.HS3.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.currentStateBindingSource, "ch3in", true));
            this.HS3.DisplayScale = 1F;
            this.HS3.DrawLabel = true;
            this.HS3.Label = "Collective";
            resources.ApplyResources(this.HS3, "HS3");
            this.HS3.Maximum = 2200;
            this.HS3.maxline = 0;
            this.HS3.Minimum = 800;
            this.HS3.minline = 0;
            this.HS3.Name = "HS3";
            this.HS3.Value = 1500;
            this.HS3.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.HS3.Paint += new System.Windows.Forms.PaintEventHandler(this.HS3_Paint);
            // 
            // Gservoloc
            // 
            this.Gservoloc.BackColor = System.Drawing.Color.Transparent;
            this.Gservoloc.BackgroundImage = global::MissionPlanner.Properties.Resources.Gaugebg;
            resources.ApplyResources(this.Gservoloc, "Gservoloc");
            this.Gservoloc.BaseArcColor = System.Drawing.Color.Transparent;
            this.Gservoloc.BaseArcRadius = 60;
            this.Gservoloc.BaseArcStart = 90;
            this.Gservoloc.BaseArcSweep = 360;
            this.Gservoloc.BaseArcWidth = 2;
            this.Gservoloc.Cap_Idx = ((byte)(0));
            this.Gservoloc.CapColor = System.Drawing.Color.White;
            this.Gservoloc.CapColors = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.Gservoloc.CapPosition = new System.Drawing.Point(55, 85);
            this.Gservoloc.CapsPosition = new System.Drawing.Point[] {
        new System.Drawing.Point(55, 85),
        new System.Drawing.Point(40, 67),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10),
        new System.Drawing.Point(10, 10)};
            this.Gservoloc.CapsText = new string[] {
        "Position",
        "",
        "",
        "",
        ""};
            this.Gservoloc.CapText = "Position";
            this.Gservoloc.Center = new System.Drawing.Point(75, 75);
            this.Gservoloc.MaxValue = 180F;
            this.Gservoloc.MinValue = -180F;
            this.Gservoloc.Name = "Gservoloc";
            this.Gservoloc.Need_Idx = ((byte)(3));
            this.Gservoloc.NeedleColor1 = AGaugeApp.AGauge.NeedleColorEnum.Gray;
            this.Gservoloc.NeedleColor2 = System.Drawing.Color.White;
            this.Gservoloc.NeedleEnabled = false;
            this.Gservoloc.NeedleRadius = 80;
            this.Gservoloc.NeedlesColor1 = new AGaugeApp.AGauge.NeedleColorEnum[] {
        AGaugeApp.AGauge.NeedleColorEnum.Gray,
        AGaugeApp.AGauge.NeedleColorEnum.Red,
        AGaugeApp.AGauge.NeedleColorEnum.Green,
        AGaugeApp.AGauge.NeedleColorEnum.Gray};
            this.Gservoloc.NeedlesColor2 = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White};
            this.Gservoloc.NeedlesEnabled = new bool[] {
        true,
        true,
        true,
        false};
            this.Gservoloc.NeedlesRadius = new int[] {
        60,
        60,
        60,
        80};
            this.Gservoloc.NeedlesType = new int[] {
        0,
        0,
        0,
        0};
            this.Gservoloc.NeedlesWidth = new int[] {
        2,
        2,
        2,
        2};
            this.Gservoloc.NeedleType = 0;
            this.Gservoloc.NeedleWidth = 2;
            this.Gservoloc.Range_Idx = ((byte)(0));
            this.Gservoloc.RangeColor = System.Drawing.Color.LightGreen;
            this.Gservoloc.RangeEnabled = false;
            this.Gservoloc.RangeEndValue = 360F;
            this.Gservoloc.RangeInnerRadius = 1;
            this.Gservoloc.RangeOuterRadius = 60;
            this.Gservoloc.RangesColor = new System.Drawing.Color[] {
        System.Drawing.Color.LightGreen,
        System.Drawing.Color.Red,
        System.Drawing.Color.Orange,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.Gservoloc.RangesEnabled = new bool[] {
        false,
        false,
        false,
        false,
        false};
            this.Gservoloc.RangesEndValue = new float[] {
        360F,
        200F,
        150F,
        0F,
        0F};
            this.Gservoloc.RangesInnerRadius = new int[] {
        1,
        1,
        1,
        70,
        70};
            this.Gservoloc.RangesOuterRadius = new int[] {
        60,
        60,
        60,
        80,
        80};
            this.Gservoloc.RangesStartValue = new float[] {
        0F,
        150F,
        75F,
        0F,
        0F};
            this.Gservoloc.RangeStartValue = 0F;
            this.Gservoloc.ScaleLinesInterColor = System.Drawing.Color.White;
            this.Gservoloc.ScaleLinesInterInnerRadius = 52;
            this.Gservoloc.ScaleLinesInterOuterRadius = 60;
            this.Gservoloc.ScaleLinesInterWidth = 1;
            this.Gservoloc.ScaleLinesMajorColor = System.Drawing.Color.White;
            this.Gservoloc.ScaleLinesMajorInnerRadius = 50;
            this.Gservoloc.ScaleLinesMajorOuterRadius = 60;
            this.Gservoloc.ScaleLinesMajorStepValue = 30F;
            this.Gservoloc.ScaleLinesMajorWidth = 2;
            this.Gservoloc.ScaleLinesMinorColor = System.Drawing.Color.White;
            this.Gservoloc.ScaleLinesMinorInnerRadius = 55;
            this.Gservoloc.ScaleLinesMinorNumOf = 2;
            this.Gservoloc.ScaleLinesMinorOuterRadius = 60;
            this.Gservoloc.ScaleLinesMinorWidth = 1;
            this.Gservoloc.ScaleNumbersColor = System.Drawing.Color.White;
            this.Gservoloc.ScaleNumbersFormat = null;
            this.Gservoloc.ScaleNumbersRadius = 44;
            this.Gservoloc.ScaleNumbersRotation = 45;
            this.Gservoloc.ScaleNumbersStartScaleLine = 2;
            this.Gservoloc.ScaleNumbersStepScaleLines = 1;
            this.Gservoloc.Value = 0F;
            this.Gservoloc.Value0 = -60F;
            this.Gservoloc.Value1 = 60F;
            this.Gservoloc.Value2 = 180F;
            this.Gservoloc.Value3 = 0F;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.myButtonH_SV_MANtest);
            this.groupBox6.Controls.Add(this.myButtonH_SV_MANmin);
            this.groupBox6.Controls.Add(this.myButtonH_SV_MANzero);
            this.groupBox6.Controls.Add(this.myButtonH_SV_MANmax);
            this.groupBox6.Controls.Add(this.myButtonH_SV_MANmanual);
            this.groupBox6.Controls.Add(this.myButtonH_SV_MAN);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // myButtonH_SV_MANtest
            // 
            resources.ApplyResources(this.myButtonH_SV_MANtest, "myButtonH_SV_MANtest");
            this.myButtonH_SV_MANtest.Name = "myButtonH_SV_MANtest";
            this.myButtonH_SV_MANtest.UseVisualStyleBackColor = true;
            this.myButtonH_SV_MANtest.Click += new System.EventHandler(this.myButtonH_SV_MANtest_Click);
            // 
            // myButtonH_SV_MANmin
            // 
            resources.ApplyResources(this.myButtonH_SV_MANmin, "myButtonH_SV_MANmin");
            this.myButtonH_SV_MANmin.Name = "myButtonH_SV_MANmin";
            this.myButtonH_SV_MANmin.UseVisualStyleBackColor = true;
            this.myButtonH_SV_MANmin.Click += new System.EventHandler(this.myButtonH_SV_MANmin_Click);
            // 
            // myButtonH_SV_MANzero
            // 
            resources.ApplyResources(this.myButtonH_SV_MANzero, "myButtonH_SV_MANzero");
            this.myButtonH_SV_MANzero.Name = "myButtonH_SV_MANzero";
            this.myButtonH_SV_MANzero.UseVisualStyleBackColor = true;
            this.myButtonH_SV_MANzero.Click += new System.EventHandler(this.myButtonH_SV_MANzero_Click);
            // 
            // myButtonH_SV_MANmax
            // 
            resources.ApplyResources(this.myButtonH_SV_MANmax, "myButtonH_SV_MANmax");
            this.myButtonH_SV_MANmax.Name = "myButtonH_SV_MANmax";
            this.myButtonH_SV_MANmax.UseVisualStyleBackColor = true;
            this.myButtonH_SV_MANmax.Click += new System.EventHandler(this.myButtonH_SV_MANmax_Click);
            // 
            // myButtonH_SV_MANmanual
            // 
            resources.ApplyResources(this.myButtonH_SV_MANmanual, "myButtonH_SV_MANmanual");
            this.myButtonH_SV_MANmanual.Name = "myButtonH_SV_MANmanual";
            this.myButtonH_SV_MANmanual.UseVisualStyleBackColor = true;
            this.myButtonH_SV_MANmanual.Click += new System.EventHandler(this.myButtonH_SV_MANmanual_Click);
            // 
            // myButtonH_SV_MAN
            // 
            resources.ApplyResources(this.myButtonH_SV_MAN, "myButtonH_SV_MAN");
            this.myButtonH_SV_MAN.Name = "myButtonH_SV_MAN";
            this.myButtonH_SV_MAN.UseVisualStyleBackColor = true;
            this.myButtonH_SV_MAN.Click += new System.EventHandler(this.myButtonH_SV_MAN_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.h_rsc_rev);
            this.groupBox11.Controls.Add(this.label13);
            this.groupBox11.Controls.Add(this.label14);
            this.groupBox11.Controls.Add(this.mavlinkNumericUpDownh_rsc_max);
            this.groupBox11.Controls.Add(this.label15);
            this.groupBox11.Controls.Add(this.mavlinkNumericUpDownh_rsc_min);
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            // 
            // h_rsc_rev
            // 
            resources.ApplyResources(this.h_rsc_rev, "h_rsc_rev");
            this.h_rsc_rev.Name = "h_rsc_rev";
            this.h_rsc_rev.OffValue = 0D;
            this.h_rsc_rev.OnValue = 1D;
            this.h_rsc_rev.ParamName = null;
            this.h_rsc_rev.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // mavlinkNumericUpDownh_rsc_max
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownh_rsc_max, "mavlinkNumericUpDownh_rsc_max");
            this.mavlinkNumericUpDownh_rsc_max.Max = 1F;
            this.mavlinkNumericUpDownh_rsc_max.Min = 0F;
            this.mavlinkNumericUpDownh_rsc_max.Name = "mavlinkNumericUpDownh_rsc_max";
            this.mavlinkNumericUpDownh_rsc_max.ParamName = null;
            this.mavlinkNumericUpDownh_rsc_max.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // mavlinkNumericUpDownh_rsc_min
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownh_rsc_min, "mavlinkNumericUpDownh_rsc_min");
            this.mavlinkNumericUpDownh_rsc_min.Max = 1F;
            this.mavlinkNumericUpDownh_rsc_min.Min = 0F;
            this.mavlinkNumericUpDownh_rsc_min.Name = "mavlinkNumericUpDownh_rsc_min";
            this.mavlinkNumericUpDownh_rsc_min.ParamName = null;
            this.mavlinkNumericUpDownh_rsc_min.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.label16);
            this.groupBox12.Controls.Add(this.mavlinkNumericUpDownh_rsc_power_high);
            this.groupBox12.Controls.Add(this.label25);
            this.groupBox12.Controls.Add(this.mavlinkNumericUpDownh_rsc_power_low);
            this.groupBox12.Controls.Add(this.label26);
            this.groupBox12.Controls.Add(this.mavlinkNumericUpDownh_rsc_idle);
            resources.ApplyResources(this.groupBox12, "groupBox12");
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.TabStop = false;
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // mavlinkNumericUpDownh_rsc_power_high
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownh_rsc_power_high, "mavlinkNumericUpDownh_rsc_power_high");
            this.mavlinkNumericUpDownh_rsc_power_high.Max = 1F;
            this.mavlinkNumericUpDownh_rsc_power_high.Min = 0F;
            this.mavlinkNumericUpDownh_rsc_power_high.Name = "mavlinkNumericUpDownh_rsc_power_high";
            this.mavlinkNumericUpDownh_rsc_power_high.ParamName = null;
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // mavlinkNumericUpDownh_rsc_power_low
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownh_rsc_power_low, "mavlinkNumericUpDownh_rsc_power_low");
            this.mavlinkNumericUpDownh_rsc_power_low.Max = 1F;
            this.mavlinkNumericUpDownh_rsc_power_low.Min = 0F;
            this.mavlinkNumericUpDownh_rsc_power_low.Name = "mavlinkNumericUpDownh_rsc_power_low";
            this.mavlinkNumericUpDownh_rsc_power_low.ParamName = null;
            this.mavlinkNumericUpDownh_rsc_power_low.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // mavlinkNumericUpDownh_rsc_idle
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownh_rsc_idle, "mavlinkNumericUpDownh_rsc_idle");
            this.mavlinkNumericUpDownh_rsc_idle.Max = 1F;
            this.mavlinkNumericUpDownh_rsc_idle.Min = 0F;
            this.mavlinkNumericUpDownh_rsc_idle.Name = "mavlinkNumericUpDownh_rsc_idle";
            this.mavlinkNumericUpDownh_rsc_idle.ParamName = null;
            this.mavlinkNumericUpDownh_rsc_idle.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.label31);
            this.groupBox13.Controls.Add(this.mavlinkNumericUpDownim_acro_col_exp);
            this.groupBox13.Controls.Add(this.label30);
            this.groupBox13.Controls.Add(this.mavlinkNumericUpDownim_stab_col_3);
            this.groupBox13.Controls.Add(this.mavlinkNumericUpDownim_stab_col_4);
            this.groupBox13.Controls.Add(this.label27);
            this.groupBox13.Controls.Add(this.label28);
            this.groupBox13.Controls.Add(this.mavlinkNumericUpDownim_stab_col_2);
            this.groupBox13.Controls.Add(this.label29);
            this.groupBox13.Controls.Add(this.mavlinkNumericUpDownim_stab_col_1);
            resources.ApplyResources(this.groupBox13, "groupBox13");
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.TabStop = false;
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.Name = "label31";
            // 
            // mavlinkNumericUpDownim_acro_col_exp
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownim_acro_col_exp, "mavlinkNumericUpDownim_acro_col_exp");
            this.mavlinkNumericUpDownim_acro_col_exp.Max = 1F;
            this.mavlinkNumericUpDownim_acro_col_exp.Min = 0F;
            this.mavlinkNumericUpDownim_acro_col_exp.Name = "mavlinkNumericUpDownim_acro_col_exp";
            this.mavlinkNumericUpDownim_acro_col_exp.ParamName = null;
            this.mavlinkNumericUpDownim_acro_col_exp.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // mavlinkNumericUpDownim_stab_col_3
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownim_stab_col_3, "mavlinkNumericUpDownim_stab_col_3");
            this.mavlinkNumericUpDownim_stab_col_3.Max = 1F;
            this.mavlinkNumericUpDownim_stab_col_3.Min = 0F;
            this.mavlinkNumericUpDownim_stab_col_3.Name = "mavlinkNumericUpDownim_stab_col_3";
            this.mavlinkNumericUpDownim_stab_col_3.ParamName = null;
            this.mavlinkNumericUpDownim_stab_col_3.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // mavlinkNumericUpDownim_stab_col_4
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownim_stab_col_4, "mavlinkNumericUpDownim_stab_col_4");
            this.mavlinkNumericUpDownim_stab_col_4.Max = 1F;
            this.mavlinkNumericUpDownim_stab_col_4.Min = 0F;
            this.mavlinkNumericUpDownim_stab_col_4.Name = "mavlinkNumericUpDownim_stab_col_4";
            this.mavlinkNumericUpDownim_stab_col_4.ParamName = null;
            this.mavlinkNumericUpDownim_stab_col_4.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // mavlinkNumericUpDownim_stab_col_2
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownim_stab_col_2, "mavlinkNumericUpDownim_stab_col_2");
            this.mavlinkNumericUpDownim_stab_col_2.Max = 1F;
            this.mavlinkNumericUpDownim_stab_col_2.Min = 0F;
            this.mavlinkNumericUpDownim_stab_col_2.Name = "mavlinkNumericUpDownim_stab_col_2";
            this.mavlinkNumericUpDownim_stab_col_2.ParamName = null;
            this.mavlinkNumericUpDownim_stab_col_2.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            // 
            // mavlinkNumericUpDownim_stab_col_1
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownim_stab_col_1, "mavlinkNumericUpDownim_stab_col_1");
            this.mavlinkNumericUpDownim_stab_col_1.Max = 1F;
            this.mavlinkNumericUpDownim_stab_col_1.Min = 0F;
            this.mavlinkNumericUpDownim_stab_col_1.Name = "mavlinkNumericUpDownim_stab_col_1";
            this.mavlinkNumericUpDownim_stab_col_1.ParamName = null;
            this.mavlinkNumericUpDownim_stab_col_1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // zedGraphControl1
            // 
            resources.ApplyResources(this.zedGraphControl1, "zedGraphControl1");
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            // 
            // mavlinkNumericUpDownH_PHANG
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownH_PHANG, "mavlinkNumericUpDownH_PHANG");
            this.mavlinkNumericUpDownH_PHANG.Max = 1F;
            this.mavlinkNumericUpDownH_PHANG.Min = 0F;
            this.mavlinkNumericUpDownH_PHANG.Name = "mavlinkNumericUpDownH_PHANG";
            this.mavlinkNumericUpDownH_PHANG.ParamName = null;
            // 
            // ConfigTradHeli
            // 
            
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox13);
            this.Controls.Add(this.zedGraphControl1);
            this.Controls.Add(this.groupBox12);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mavlinkNumericUpDownH_PHANG);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.HS3_TRIM);
            this.Controls.Add(this.HS2_TRIM);
            this.Controls.Add(this.HS1_TRIM);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.H_SV3_POS);
            this.Controls.Add(this.H_SV2_POS);
            this.Controls.Add(this.H_SV1_POS);
            this.Controls.Add(this.HS3_REV);
            this.Controls.Add(this.HS2_REV);
            this.Controls.Add(this.HS1_REV);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.HS4);
            this.Controls.Add(this.HS3);
            this.Controls.Add(this.Gservoloc);
            this.Name = "ConfigTradHeli";
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDowntailspeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_GYR_GAIN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_COL_MIN)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.H_COL_MID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_COL_MAX)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HS4_MIN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HS4_MAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HS4_TRIM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HS3_TRIM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HS2_TRIM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HS1_TRIM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_SV3_POS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_SV2_POS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.H_SV1_POS)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.H_COLYAW)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownH_RSC_CRITICAL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownrunuptime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkudH_RSC_RATE)).EndInit();
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkudH_RSC_SETPOINT)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_sv_test)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownatc_hovr_rol_trm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_cyc_max)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownland_col_min)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentStateBindingSource)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_rsc_max)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_rsc_min)).EndInit();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_rsc_power_high)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_rsc_power_low)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownh_rsc_idle)).EndInit();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownim_acro_col_exp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownim_stab_col_3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownim_stab_col_4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownim_stab_col_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownim_stab_col_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownH_PHANG)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton H_SWASH_TYPE;
        private System.Windows.Forms.RadioButton CCPM;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label46;
        private MavlinkNumericUpDown H_GYR_GAIN;
        private System.Windows.Forms.Label label21;
        private MavlinkNumericUpDown H_COL_MIN;
        private System.Windows.Forms.GroupBox groupBox1;
        private MavlinkNumericUpDown H_COL_MID;
        private MavlinkNumericUpDown H_COL_MAX;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label24;
        private MavlinkNumericUpDown HS4_MIN;
        private MavlinkNumericUpDown HS4_MAX;
        private System.Windows.Forms.Label label40;
        private MavlinkNumericUpDown HS3_TRIM;
        private MavlinkNumericUpDown HS2_TRIM;
        private MavlinkNumericUpDown HS1_TRIM;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private MavlinkNumericUpDown H_SV3_POS;
        private MavlinkNumericUpDown H_SV2_POS;
        private MavlinkNumericUpDown H_SV1_POS;
        private MavlinkCheckBox HS3_REV;
        private MavlinkCheckBox HS2_REV;
        private MavlinkCheckBox HS1_REV;
        private System.Windows.Forms.Label label17;
        private HorizontalProgressBar2 HS4;
        private VerticalProgressBar2 HS3;
        private AGaugeApp.AGauge Gservoloc;
        private System.Windows.Forms.BindingSource currentStateBindingSource;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label43;
        private MavlinkNumericUpDown HS4_TRIM;
        private MavlinkCheckBox HS4_REV;
        private MavlinkCheckBox fbl_modeFBL;
        private System.Windows.Forms.GroupBox groupBox4;
        private Controls.MavlinkNumericUpDown H_COLYAW;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox7;
        private Controls.MavlinkNumericUpDown mavlinkudH_RSC_RATE;
        private System.Windows.Forms.GroupBox groupBox8;
        private Controls.MavlinkNumericUpDown mavlinkudH_RSC_SETPOINT;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label label5;
        private Controls.MavlinkComboBox H_RSC_MODE;
        private MavlinkComboBox mavlinkComboBoxTailType;
        private MavlinkNumericUpDown mavlinkNumericUpDowntailspeed;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox10;
        private MavlinkNumericUpDown mavlinkNumericUpDownland_col_min;
        private System.Windows.Forms.Label label7;
        private MavlinkNumericUpDown mavlinkNumericUpDownrunuptime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox6;
        private MyButton myButtonH_SV_MANtest;
        private MyButton myButtonH_SV_MANmin;
        private MyButton myButtonH_SV_MANzero;
        private MyButton myButtonH_SV_MANmax;
        private MyButton myButtonH_SV_MANmanual;
        private MyButton myButtonH_SV_MAN;
        private System.Windows.Forms.Label label2;
        private MavlinkNumericUpDown mavlinkNumericUpDownH_PHANG;
        private System.Windows.Forms.Label label3;
        private MavlinkCheckBox mavlinkCheckBoxatc_piro_comp;
        private MavlinkNumericUpDown mavlinkNumericUpDownh_sv_test;
        private MavlinkNumericUpDown mavlinkNumericUpDownatc_hovr_rol_trm;
        private MavlinkNumericUpDown mavlinkNumericUpDownh_cyc_max;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private MavlinkNumericUpDown mavlinkNumericUpDownH_RSC_CRITICAL;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private MavlinkNumericUpDown mavlinkNumericUpDownh_rsc_max;
        private System.Windows.Forms.Label label15;
        private MavlinkNumericUpDown mavlinkNumericUpDownh_rsc_min;
        private MavlinkCheckBox h_rsc_rev;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Label label16;
        private MavlinkNumericUpDown mavlinkNumericUpDownh_rsc_power_high;
        private System.Windows.Forms.Label label25;
        private MavlinkNumericUpDown mavlinkNumericUpDownh_rsc_power_low;
        private System.Windows.Forms.Label label26;
        private MavlinkNumericUpDown mavlinkNumericUpDownh_rsc_idle;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.Label label30;
        private MavlinkNumericUpDown mavlinkNumericUpDownim_stab_col_3;
        private MavlinkNumericUpDown mavlinkNumericUpDownim_stab_col_4;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private MavlinkNumericUpDown mavlinkNumericUpDownim_stab_col_2;
        private System.Windows.Forms.Label label29;
        private MavlinkNumericUpDown mavlinkNumericUpDownim_stab_col_1;
        private System.Windows.Forms.Label label31;
        private MavlinkNumericUpDown mavlinkNumericUpDownim_acro_col_exp;
    }
}
