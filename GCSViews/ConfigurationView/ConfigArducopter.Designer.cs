using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigArducopter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigArducopter));
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label25 = new System.Windows.Forms.Label();
            this.CHK_lockrollpitch = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label22 = new System.Windows.Forms.Label();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.groupBox20 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label77 = new System.Windows.Forms.Label();
            this.label82 = new System.Windows.Forms.Label();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label84 = new System.Windows.Forms.Label();
            this.label86 = new System.Windows.Forms.Label();
            this.label87 = new System.Windows.Forms.Label();
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.label28 = new System.Windows.Forms.Label();
            this.P_FLTD = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label88 = new System.Windows.Forms.Label();
            this.label90 = new System.Windows.Forms.Label();
            this.label91 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BUT_rerequestparams = new MissionPlanner.Controls.MyButton();
            this.BUT_writePIDS = new MissionPlanner.Controls.MyButton();
            this.myLabel3 = new System.Windows.Forms.Label();
            this.myLabel2 = new System.Windows.Forms.Label();
            this.myLabel1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.BUT_refreshpart = new MissionPlanner.Controls.MyButton();
            this.myLabel4 = new System.Windows.Forms.Label();
            this.myLabel5 = new System.Windows.Forms.Label();
            this.myLabel6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label51 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label40 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.INS_NOTCH_ENABLE = new MissionPlanner.Controls.MavlinkComboBox();
            this.INS_NOTCH_ATT = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_NOTCH_BW = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_NOTCH_FREQ = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_HNTCH_HMNCS = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_HNTCH_OPTS = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_HNTCH_BW = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_HNTCH_ATT = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_HNTCH_FREQ = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_HNTCH_REF = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_HNTCH_MODE = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.CH6_OPTION = new MissionPlanner.Controls.MavlinkComboBox();
            this.INS_LOG_BAT_MASK = new MissionPlanner.Controls.MavlinkComboBox();
            this.INS_ACCEL_FILTER = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_GYRO_FILTER = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.CH10_OPTION = new MissionPlanner.Controls.MavlinkComboBox();
            this.CH9_OPTION = new MissionPlanner.Controls.MavlinkComboBox();
            this.CH8_OPTION = new MissionPlanner.Controls.MavlinkComboBox();
            this.THR_ACCEL_D = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.THR_ACCEL_IMAX = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.THR_ACCEL_I = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.THR_ACCEL_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.LOITER_LAT_D = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.LOITER_LAT_IMAX = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.LOITER_LAT_I = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.LOITER_LAT_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.TUNE_LOW = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.TUNE_HIGH = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.TUNE = new MissionPlanner.Controls.MavlinkComboBox();
            this.CH7_OPTION = new MissionPlanner.Controls.MavlinkComboBox();
            this.THR_RATE_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.WPNAV_SPEED_UP = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.WPNAV_LOIT_SPEED = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.WPNAV_SPEED_DN = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.WPNAV_RADIUS = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.WPNAV_SPEED = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.THR_ALT_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavlinkNumericUpDownatc_input_tc = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.HLD_LAT_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavlinkNumericUpDownatc_accel_y_max = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.STB_YAW_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavlinkNumericUpDownatc_accel_p_max = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.STB_PIT_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavlinkNumericUpDownatc_accel_r_max = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.STB_RLL_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.ATC_RAT_YAW_FLTT = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.ATC_RAT_YAW_FLTD = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_YAW_FILT = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_YAW_D = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_YAW_IMAX = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_YAW_I = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_YAW_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.ATC_RAT_RLL_FLTT = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.ATC_RAT_RLL_FLTD = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_PIT_FILT = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_PIT_D = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_PIT_IMAX = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_PIT_I = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_PIT_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.ATC_RAT_PIT_FLTT = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.ATC_RAT_PIT_FLTD = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_RLL_FILT = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_RLL_D = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_RLL_IMAX = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_RLL_I = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.RATE_RLL_P = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_LOG_BAT_OPT = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.INS_HNTCH_ENABLE = new MissionPlanner.Controls.MavlinkComboBox();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox19.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.groupBox25.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.INS_NOTCH_ATT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_NOTCH_BW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_NOTCH_FREQ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_HMNCS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_OPTS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_BW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_ATT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_FREQ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_REF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_MODE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_ACCEL_FILTER)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_GYRO_FILTER)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_ACCEL_D)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_ACCEL_IMAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_ACCEL_I)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_ACCEL_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LOITER_LAT_D)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LOITER_LAT_IMAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LOITER_LAT_I)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LOITER_LAT_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TUNE_LOW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TUNE_HIGH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_RATE_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WPNAV_SPEED_UP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WPNAV_LOIT_SPEED)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WPNAV_SPEED_DN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WPNAV_RADIUS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WPNAV_SPEED)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_ALT_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownatc_input_tc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HLD_LAT_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownatc_accel_y_max)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.STB_YAW_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownatc_accel_p_max)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.STB_PIT_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownatc_accel_r_max)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.STB_RLL_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_YAW_FLTT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_YAW_FLTD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_YAW_FILT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_YAW_D)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_YAW_IMAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_YAW_I)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_YAW_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_RLL_FLTT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_RLL_FLTD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_PIT_FILT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_PIT_D)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_PIT_IMAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_PIT_I)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_PIT_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_PIT_FLTT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_PIT_FLTD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_RLL_FILT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_RLL_D)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_RLL_IMAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_RLL_I)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_RLL_P)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_LOG_BAT_OPT)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.THR_RATE_P);
            this.groupBox5.Controls.Add(this.label25);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // CHK_lockrollpitch
            // 
            resources.ApplyResources(this.CHK_lockrollpitch, "CHK_lockrollpitch");
            this.CHK_lockrollpitch.Checked = true;
            this.CHK_lockrollpitch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_lockrollpitch.Name = "CHK_lockrollpitch";
            this.CHK_lockrollpitch.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.WPNAV_SPEED_UP);
            this.groupBox4.Controls.Add(this.label27);
            this.groupBox4.Controls.Add(this.WPNAV_LOIT_SPEED);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.WPNAV_SPEED_DN);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.WPNAV_RADIUS);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.WPNAV_SPEED);
            this.groupBox4.Controls.Add(this.label16);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.THR_ALT_P);
            this.groupBox7.Controls.Add(this.label22);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.mavlinkNumericUpDownatc_input_tc);
            this.groupBox19.Controls.Add(this.label23);
            this.groupBox19.Controls.Add(this.HLD_LAT_P);
            this.groupBox19.Controls.Add(this.label31);
            resources.ApplyResources(this.groupBox19, "groupBox19");
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.TabStop = false;
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.Name = "label31";
            // 
            // groupBox20
            // 
            this.groupBox20.Controls.Add(this.mavlinkNumericUpDownatc_accel_y_max);
            this.groupBox20.Controls.Add(this.label21);
            this.groupBox20.Controls.Add(this.STB_YAW_P);
            this.groupBox20.Controls.Add(this.label35);
            resources.ApplyResources(this.groupBox20, "groupBox20");
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.TabStop = false;
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.mavlinkNumericUpDownatc_accel_p_max);
            this.groupBox21.Controls.Add(this.label20);
            this.groupBox21.Controls.Add(this.STB_PIT_P);
            this.groupBox21.Controls.Add(this.label42);
            resources.ApplyResources(this.groupBox21, "groupBox21");
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.TabStop = false;
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // label42
            // 
            resources.ApplyResources(this.label42, "label42");
            this.label42.Name = "label42";
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.mavlinkNumericUpDownatc_accel_r_max);
            this.groupBox22.Controls.Add(this.label19);
            this.groupBox22.Controls.Add(this.STB_RLL_P);
            this.groupBox22.Controls.Add(this.label46);
            resources.ApplyResources(this.groupBox22, "groupBox22");
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.TabStop = false;
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // label46
            // 
            resources.ApplyResources(this.label46, "label46");
            this.label46.Name = "label46";
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.ATC_RAT_YAW_FLTT);
            this.groupBox23.Controls.Add(this.label33);
            this.groupBox23.Controls.Add(this.ATC_RAT_YAW_FLTD);
            this.groupBox23.Controls.Add(this.label32);
            this.groupBox23.Controls.Add(this.RATE_YAW_FILT);
            this.groupBox23.Controls.Add(this.label18);
            this.groupBox23.Controls.Add(this.RATE_YAW_D);
            this.groupBox23.Controls.Add(this.label10);
            this.groupBox23.Controls.Add(this.RATE_YAW_IMAX);
            this.groupBox23.Controls.Add(this.label47);
            this.groupBox23.Controls.Add(this.RATE_YAW_I);
            this.groupBox23.Controls.Add(this.label77);
            this.groupBox23.Controls.Add(this.RATE_YAW_P);
            this.groupBox23.Controls.Add(this.label82);
            resources.ApplyResources(this.groupBox23, "groupBox23");
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.TabStop = false;
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.Name = "label32";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label47
            // 
            resources.ApplyResources(this.label47, "label47");
            this.label47.Name = "label47";
            // 
            // label77
            // 
            resources.ApplyResources(this.label77, "label77");
            this.label77.Name = "label77";
            // 
            // label82
            // 
            resources.ApplyResources(this.label82, "label82");
            this.label82.Name = "label82";
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.ATC_RAT_RLL_FLTT);
            this.groupBox24.Controls.Add(this.label30);
            this.groupBox24.Controls.Add(this.ATC_RAT_RLL_FLTD);
            this.groupBox24.Controls.Add(this.label29);
            this.groupBox24.Controls.Add(this.RATE_PIT_FILT);
            this.groupBox24.Controls.Add(this.label14);
            this.groupBox24.Controls.Add(this.RATE_PIT_D);
            this.groupBox24.Controls.Add(this.label11);
            this.groupBox24.Controls.Add(this.RATE_PIT_IMAX);
            this.groupBox24.Controls.Add(this.label84);
            this.groupBox24.Controls.Add(this.RATE_PIT_I);
            this.groupBox24.Controls.Add(this.label86);
            this.groupBox24.Controls.Add(this.RATE_PIT_P);
            this.groupBox24.Controls.Add(this.label87);
            resources.ApplyResources(this.groupBox24, "groupBox24");
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.TabStop = false;
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label84
            // 
            resources.ApplyResources(this.label84, "label84");
            this.label84.Name = "label84";
            // 
            // label86
            // 
            resources.ApplyResources(this.label86, "label86");
            this.label86.Name = "label86";
            // 
            // label87
            // 
            resources.ApplyResources(this.label87, "label87");
            this.label87.Name = "label87";
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.ATC_RAT_PIT_FLTT);
            this.groupBox25.Controls.Add(this.label28);
            this.groupBox25.Controls.Add(this.ATC_RAT_PIT_FLTD);
            this.groupBox25.Controls.Add(this.P_FLTD);
            this.groupBox25.Controls.Add(this.RATE_RLL_FILT);
            this.groupBox25.Controls.Add(this.label12);
            this.groupBox25.Controls.Add(this.RATE_RLL_D);
            this.groupBox25.Controls.Add(this.label17);
            this.groupBox25.Controls.Add(this.RATE_RLL_IMAX);
            this.groupBox25.Controls.Add(this.label88);
            this.groupBox25.Controls.Add(this.RATE_RLL_I);
            this.groupBox25.Controls.Add(this.label90);
            this.groupBox25.Controls.Add(this.RATE_RLL_P);
            this.groupBox25.Controls.Add(this.label91);
            resources.ApplyResources(this.groupBox25, "groupBox25");
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.TabStop = false;
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // P_FLTD
            // 
            resources.ApplyResources(this.P_FLTD, "P_FLTD");
            this.P_FLTD.Name = "P_FLTD";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // label88
            // 
            resources.ApplyResources(this.label88, "label88");
            this.label88.Name = "label88";
            // 
            // label90
            // 
            resources.ApplyResources(this.label90, "label90");
            this.label90.Name = "label90";
            // 
            // label91
            // 
            resources.ApplyResources(this.label91, "label91");
            this.label91.Name = "label91";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 20000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LOITER_LAT_D);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.LOITER_LAT_IMAX);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.LOITER_LAT_I);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.LOITER_LAT_P);
            this.groupBox1.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // BUT_rerequestparams
            // 
            resources.ApplyResources(this.BUT_rerequestparams, "BUT_rerequestparams");
            this.BUT_rerequestparams.Name = "BUT_rerequestparams";
            this.BUT_rerequestparams.UseVisualStyleBackColor = true;
            this.BUT_rerequestparams.Click += new System.EventHandler(this.BUT_rerequestparams_Click);
            // 
            // BUT_writePIDS
            // 
            resources.ApplyResources(this.BUT_writePIDS, "BUT_writePIDS");
            this.BUT_writePIDS.Name = "BUT_writePIDS";
            this.BUT_writePIDS.UseVisualStyleBackColor = true;
            this.BUT_writePIDS.Click += new System.EventHandler(this.BUT_writePIDS_Click);
            // 
            // myLabel3
            // 
            resources.ApplyResources(this.myLabel3, "myLabel3");
            this.myLabel3.Name = "myLabel3";
            // 
            // myLabel2
            // 
            resources.ApplyResources(this.myLabel2, "myLabel2");
            this.myLabel2.Name = "myLabel2";
            // 
            // myLabel1
            // 
            resources.ApplyResources(this.myLabel1, "myLabel1");
            this.myLabel1.Name = "myLabel1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.THR_ACCEL_D);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.THR_ACCEL_IMAX);
            this.groupBox2.Controls.Add(this.THR_ACCEL_I);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.THR_ACCEL_P);
            this.groupBox2.Controls.Add(this.label8);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // BUT_refreshpart
            // 
            resources.ApplyResources(this.BUT_refreshpart, "BUT_refreshpart");
            this.BUT_refreshpart.Name = "BUT_refreshpart";
            this.BUT_refreshpart.UseVisualStyleBackColor = true;
            this.BUT_refreshpart.Click += new System.EventHandler(this.BUT_refreshpart_Click);
            // 
            // myLabel4
            // 
            resources.ApplyResources(this.myLabel4, "myLabel4");
            this.myLabel4.Name = "myLabel4";
            // 
            // myLabel5
            // 
            resources.ApplyResources(this.myLabel5, "myLabel5");
            this.myLabel5.Name = "myLabel5";
            // 
            // myLabel6
            // 
            resources.ApplyResources(this.myLabel6, "myLabel6");
            this.myLabel6.Name = "myLabel6";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.INS_ACCEL_FILTER);
            this.groupBox3.Controls.Add(this.label26);
            this.groupBox3.Controls.Add(this.INS_GYRO_FILTER);
            this.groupBox3.Controls.Add(this.label24);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.INS_HNTCH_ENABLE);
            this.groupBox9.Controls.Add(this.INS_HNTCH_HMNCS);
            this.groupBox9.Controls.Add(this.label51);
            this.groupBox9.Controls.Add(this.INS_HNTCH_OPTS);
            this.groupBox9.Controls.Add(this.label50);
            this.groupBox9.Controls.Add(this.INS_HNTCH_BW);
            this.groupBox9.Controls.Add(this.label49);
            this.groupBox9.Controls.Add(this.INS_HNTCH_ATT);
            this.groupBox9.Controls.Add(this.label48);
            this.groupBox9.Controls.Add(this.INS_HNTCH_FREQ);
            this.groupBox9.Controls.Add(this.label41);
            this.groupBox9.Controls.Add(this.INS_HNTCH_REF);
            this.groupBox9.Controls.Add(this.label43);
            this.groupBox9.Controls.Add(this.INS_HNTCH_MODE);
            this.groupBox9.Controls.Add(this.label44);
            this.groupBox9.Controls.Add(this.label45);
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // label51
            // 
            resources.ApplyResources(this.label51, "label51");
            this.label51.Name = "label51";
            // 
            // label50
            // 
            resources.ApplyResources(this.label50, "label50");
            this.label50.Name = "label50";
            // 
            // label49
            // 
            resources.ApplyResources(this.label49, "label49");
            this.label49.Name = "label49";
            // 
            // label48
            // 
            resources.ApplyResources(this.label48, "label48");
            this.label48.Name = "label48";
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.Name = "label41";
            // 
            // label43
            // 
            resources.ApplyResources(this.label43, "label43");
            this.label43.Name = "label43";
            // 
            // label44
            // 
            resources.ApplyResources(this.label44, "label44");
            this.label44.Name = "label44";
            // 
            // label45
            // 
            resources.ApplyResources(this.label45, "label45");
            this.label45.Name = "label45";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.INS_NOTCH_ENABLE);
            this.groupBox8.Controls.Add(this.INS_NOTCH_ATT);
            this.groupBox8.Controls.Add(this.label40);
            this.groupBox8.Controls.Add(this.INS_NOTCH_BW);
            this.groupBox8.Controls.Add(this.label39);
            this.groupBox8.Controls.Add(this.INS_NOTCH_FREQ);
            this.groupBox8.Controls.Add(this.label38);
            this.groupBox8.Controls.Add(this.label37);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.Name = "label40";
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
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.INS_LOG_BAT_OPT);
            this.groupBox6.Controls.Add(this.INS_LOG_BAT_MASK);
            this.groupBox6.Controls.Add(this.label36);
            this.groupBox6.Controls.Add(this.label34);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.Name = "label36";
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // label52
            // 
            resources.ApplyResources(this.label52, "label52");
            this.label52.Name = "label52";
            // 
            // INS_NOTCH_ENABLE
            // 
            this.INS_NOTCH_ENABLE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.INS_NOTCH_ENABLE.DropDownWidth = 170;
            resources.ApplyResources(this.INS_NOTCH_ENABLE, "INS_NOTCH_ENABLE");
            this.INS_NOTCH_ENABLE.FormattingEnabled = true;
            this.INS_NOTCH_ENABLE.Name = "INS_NOTCH_ENABLE";
            this.INS_NOTCH_ENABLE.ParamName = null;
            this.INS_NOTCH_ENABLE.SubControl = null;
            this.INS_NOTCH_ENABLE.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_NOTCH_ATT
            // 
            resources.ApplyResources(this.INS_NOTCH_ATT, "INS_NOTCH_ATT");
            this.INS_NOTCH_ATT.Max = 1F;
            this.INS_NOTCH_ATT.Min = 0F;
            this.INS_NOTCH_ATT.Name = "INS_NOTCH_ATT";
            this.INS_NOTCH_ATT.ParamName = null;
            this.INS_NOTCH_ATT.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_NOTCH_BW
            // 
            resources.ApplyResources(this.INS_NOTCH_BW, "INS_NOTCH_BW");
            this.INS_NOTCH_BW.Max = 1F;
            this.INS_NOTCH_BW.Min = 0F;
            this.INS_NOTCH_BW.Name = "INS_NOTCH_BW";
            this.INS_NOTCH_BW.ParamName = null;
            this.INS_NOTCH_BW.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_NOTCH_FREQ
            // 
            resources.ApplyResources(this.INS_NOTCH_FREQ, "INS_NOTCH_FREQ");
            this.INS_NOTCH_FREQ.Max = 1F;
            this.INS_NOTCH_FREQ.Min = 0F;
            this.INS_NOTCH_FREQ.Name = "INS_NOTCH_FREQ";
            this.INS_NOTCH_FREQ.ParamName = null;
            this.INS_NOTCH_FREQ.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_HNTCH_HMNCS
            // 
            resources.ApplyResources(this.INS_HNTCH_HMNCS, "INS_HNTCH_HMNCS");
            this.INS_HNTCH_HMNCS.Max = 1F;
            this.INS_HNTCH_HMNCS.Min = 0F;
            this.INS_HNTCH_HMNCS.Name = "INS_HNTCH_HMNCS";
            this.INS_HNTCH_HMNCS.ParamName = null;
            this.INS_HNTCH_HMNCS.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_HNTCH_OPTS
            // 
            resources.ApplyResources(this.INS_HNTCH_OPTS, "INS_HNTCH_OPTS");
            this.INS_HNTCH_OPTS.Max = 1F;
            this.INS_HNTCH_OPTS.Min = 0F;
            this.INS_HNTCH_OPTS.Name = "INS_HNTCH_OPTS";
            this.INS_HNTCH_OPTS.ParamName = null;
            this.INS_HNTCH_OPTS.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_HNTCH_BW
            // 
            resources.ApplyResources(this.INS_HNTCH_BW, "INS_HNTCH_BW");
            this.INS_HNTCH_BW.Max = 1F;
            this.INS_HNTCH_BW.Min = 0F;
            this.INS_HNTCH_BW.Name = "INS_HNTCH_BW";
            this.INS_HNTCH_BW.ParamName = null;
            this.INS_HNTCH_BW.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_HNTCH_ATT
            // 
            resources.ApplyResources(this.INS_HNTCH_ATT, "INS_HNTCH_ATT");
            this.INS_HNTCH_ATT.Max = 1F;
            this.INS_HNTCH_ATT.Min = 0F;
            this.INS_HNTCH_ATT.Name = "INS_HNTCH_ATT";
            this.INS_HNTCH_ATT.ParamName = null;
            this.INS_HNTCH_ATT.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_HNTCH_FREQ
            // 
            resources.ApplyResources(this.INS_HNTCH_FREQ, "INS_HNTCH_FREQ");
            this.INS_HNTCH_FREQ.Max = 1F;
            this.INS_HNTCH_FREQ.Min = 0F;
            this.INS_HNTCH_FREQ.Name = "INS_HNTCH_FREQ";
            this.INS_HNTCH_FREQ.ParamName = null;
            this.INS_HNTCH_FREQ.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_HNTCH_REF
            // 
            resources.ApplyResources(this.INS_HNTCH_REF, "INS_HNTCH_REF");
            this.INS_HNTCH_REF.Max = 1F;
            this.INS_HNTCH_REF.Min = 0F;
            this.INS_HNTCH_REF.Name = "INS_HNTCH_REF";
            this.INS_HNTCH_REF.ParamName = null;
            this.INS_HNTCH_REF.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_HNTCH_MODE
            // 
            resources.ApplyResources(this.INS_HNTCH_MODE, "INS_HNTCH_MODE");
            this.INS_HNTCH_MODE.Max = 1F;
            this.INS_HNTCH_MODE.Min = 0F;
            this.INS_HNTCH_MODE.Name = "INS_HNTCH_MODE";
            this.INS_HNTCH_MODE.ParamName = null;
            this.INS_HNTCH_MODE.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // CH6_OPTION
            // 
            this.CH6_OPTION.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CH6_OPTION.DropDownWidth = 170;
            resources.ApplyResources(this.CH6_OPTION, "CH6_OPTION");
            this.CH6_OPTION.FormattingEnabled = true;
            this.CH6_OPTION.Name = "CH6_OPTION";
            this.CH6_OPTION.ParamName = null;
            this.CH6_OPTION.SubControl = null;
            this.CH6_OPTION.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_LOG_BAT_MASK
            // 
            this.INS_LOG_BAT_MASK.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.INS_LOG_BAT_MASK.DropDownWidth = 170;
            resources.ApplyResources(this.INS_LOG_BAT_MASK, "INS_LOG_BAT_MASK");
            this.INS_LOG_BAT_MASK.FormattingEnabled = true;
            this.INS_LOG_BAT_MASK.Name = "INS_LOG_BAT_MASK";
            this.INS_LOG_BAT_MASK.ParamName = null;
            this.INS_LOG_BAT_MASK.SubControl = null;
            this.INS_LOG_BAT_MASK.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_ACCEL_FILTER
            // 
            resources.ApplyResources(this.INS_ACCEL_FILTER, "INS_ACCEL_FILTER");
            this.INS_ACCEL_FILTER.Max = 1F;
            this.INS_ACCEL_FILTER.Min = 0F;
            this.INS_ACCEL_FILTER.Name = "INS_ACCEL_FILTER";
            this.INS_ACCEL_FILTER.ParamName = null;
            this.INS_ACCEL_FILTER.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_GYRO_FILTER
            // 
            resources.ApplyResources(this.INS_GYRO_FILTER, "INS_GYRO_FILTER");
            this.INS_GYRO_FILTER.Max = 1F;
            this.INS_GYRO_FILTER.Min = 0F;
            this.INS_GYRO_FILTER.Name = "INS_GYRO_FILTER";
            this.INS_GYRO_FILTER.ParamName = null;
            this.INS_GYRO_FILTER.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // CH10_OPTION
            // 
            this.CH10_OPTION.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CH10_OPTION.DropDownWidth = 170;
            resources.ApplyResources(this.CH10_OPTION, "CH10_OPTION");
            this.CH10_OPTION.FormattingEnabled = true;
            this.CH10_OPTION.Name = "CH10_OPTION";
            this.CH10_OPTION.ParamName = null;
            this.CH10_OPTION.SubControl = null;
            // 
            // CH9_OPTION
            // 
            this.CH9_OPTION.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CH9_OPTION.DropDownWidth = 170;
            resources.ApplyResources(this.CH9_OPTION, "CH9_OPTION");
            this.CH9_OPTION.FormattingEnabled = true;
            this.CH9_OPTION.Name = "CH9_OPTION";
            this.CH9_OPTION.ParamName = null;
            this.CH9_OPTION.SubControl = null;
            // 
            // CH8_OPTION
            // 
            this.CH8_OPTION.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CH8_OPTION.DropDownWidth = 170;
            resources.ApplyResources(this.CH8_OPTION, "CH8_OPTION");
            this.CH8_OPTION.FormattingEnabled = true;
            this.CH8_OPTION.Name = "CH8_OPTION";
            this.CH8_OPTION.ParamName = null;
            this.CH8_OPTION.SubControl = null;
            this.CH8_OPTION.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // THR_ACCEL_D
            // 
            resources.ApplyResources(this.THR_ACCEL_D, "THR_ACCEL_D");
            this.THR_ACCEL_D.Max = 1F;
            this.THR_ACCEL_D.Min = 0F;
            this.THR_ACCEL_D.Name = "THR_ACCEL_D";
            this.THR_ACCEL_D.ParamName = null;
            this.THR_ACCEL_D.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // THR_ACCEL_IMAX
            // 
            resources.ApplyResources(this.THR_ACCEL_IMAX, "THR_ACCEL_IMAX");
            this.THR_ACCEL_IMAX.Max = 1F;
            this.THR_ACCEL_IMAX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.THR_ACCEL_IMAX.Min = 0F;
            this.THR_ACCEL_IMAX.Name = "THR_ACCEL_IMAX";
            this.THR_ACCEL_IMAX.ParamName = null;
            this.THR_ACCEL_IMAX.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // THR_ACCEL_I
            // 
            resources.ApplyResources(this.THR_ACCEL_I, "THR_ACCEL_I");
            this.THR_ACCEL_I.Max = 1F;
            this.THR_ACCEL_I.Min = 0F;
            this.THR_ACCEL_I.Name = "THR_ACCEL_I";
            this.THR_ACCEL_I.ParamName = null;
            this.THR_ACCEL_I.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // THR_ACCEL_P
            // 
            resources.ApplyResources(this.THR_ACCEL_P, "THR_ACCEL_P");
            this.THR_ACCEL_P.Max = 1F;
            this.THR_ACCEL_P.Min = 0F;
            this.THR_ACCEL_P.Name = "THR_ACCEL_P";
            this.THR_ACCEL_P.ParamName = null;
            this.THR_ACCEL_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // LOITER_LAT_D
            // 
            resources.ApplyResources(this.LOITER_LAT_D, "LOITER_LAT_D");
            this.LOITER_LAT_D.Max = 1F;
            this.LOITER_LAT_D.Min = 0F;
            this.LOITER_LAT_D.Name = "LOITER_LAT_D";
            this.LOITER_LAT_D.ParamName = null;
            this.LOITER_LAT_D.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // LOITER_LAT_IMAX
            // 
            resources.ApplyResources(this.LOITER_LAT_IMAX, "LOITER_LAT_IMAX");
            this.LOITER_LAT_IMAX.Max = 1F;
            this.LOITER_LAT_IMAX.Min = 0F;
            this.LOITER_LAT_IMAX.Name = "LOITER_LAT_IMAX";
            this.LOITER_LAT_IMAX.ParamName = null;
            this.LOITER_LAT_IMAX.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // LOITER_LAT_I
            // 
            resources.ApplyResources(this.LOITER_LAT_I, "LOITER_LAT_I");
            this.LOITER_LAT_I.Max = 1F;
            this.LOITER_LAT_I.Min = 0F;
            this.LOITER_LAT_I.Name = "LOITER_LAT_I";
            this.LOITER_LAT_I.ParamName = null;
            this.LOITER_LAT_I.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // LOITER_LAT_P
            // 
            resources.ApplyResources(this.LOITER_LAT_P, "LOITER_LAT_P");
            this.LOITER_LAT_P.Max = 1F;
            this.LOITER_LAT_P.Min = 0F;
            this.LOITER_LAT_P.Name = "LOITER_LAT_P";
            this.LOITER_LAT_P.ParamName = null;
            this.LOITER_LAT_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // TUNE_LOW
            // 
            resources.ApplyResources(this.TUNE_LOW, "TUNE_LOW");
            this.TUNE_LOW.Max = 1F;
            this.TUNE_LOW.Min = 0F;
            this.TUNE_LOW.Name = "TUNE_LOW";
            this.TUNE_LOW.ParamName = null;
            this.TUNE_LOW.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // TUNE_HIGH
            // 
            resources.ApplyResources(this.TUNE_HIGH, "TUNE_HIGH");
            this.TUNE_HIGH.Max = 1F;
            this.TUNE_HIGH.Min = 0F;
            this.TUNE_HIGH.Name = "TUNE_HIGH";
            this.TUNE_HIGH.ParamName = null;
            this.TUNE_HIGH.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // TUNE
            // 
            this.TUNE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TUNE.DropDownWidth = 170;
            resources.ApplyResources(this.TUNE, "TUNE");
            this.TUNE.FormattingEnabled = true;
            this.TUNE.Name = "TUNE";
            this.TUNE.ParamName = null;
            this.TUNE.SubControl = null;
            this.TUNE.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // CH7_OPTION
            // 
            this.CH7_OPTION.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CH7_OPTION.DropDownWidth = 170;
            resources.ApplyResources(this.CH7_OPTION, "CH7_OPTION");
            this.CH7_OPTION.FormattingEnabled = true;
            this.CH7_OPTION.Name = "CH7_OPTION";
            this.CH7_OPTION.ParamName = null;
            this.CH7_OPTION.SubControl = null;
            this.CH7_OPTION.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // THR_RATE_P
            // 
            resources.ApplyResources(this.THR_RATE_P, "THR_RATE_P");
            this.THR_RATE_P.Max = 1F;
            this.THR_RATE_P.Min = 0F;
            this.THR_RATE_P.Name = "THR_RATE_P";
            this.THR_RATE_P.ParamName = null;
            this.THR_RATE_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // WPNAV_SPEED_UP
            // 
            resources.ApplyResources(this.WPNAV_SPEED_UP, "WPNAV_SPEED_UP");
            this.WPNAV_SPEED_UP.Max = 1F;
            this.WPNAV_SPEED_UP.Min = 0F;
            this.WPNAV_SPEED_UP.Name = "WPNAV_SPEED_UP";
            this.WPNAV_SPEED_UP.ParamName = null;
            this.WPNAV_SPEED_UP.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // WPNAV_LOIT_SPEED
            // 
            resources.ApplyResources(this.WPNAV_LOIT_SPEED, "WPNAV_LOIT_SPEED");
            this.WPNAV_LOIT_SPEED.Max = 1F;
            this.WPNAV_LOIT_SPEED.Min = 0F;
            this.WPNAV_LOIT_SPEED.Name = "WPNAV_LOIT_SPEED";
            this.WPNAV_LOIT_SPEED.ParamName = null;
            this.WPNAV_LOIT_SPEED.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // WPNAV_SPEED_DN
            // 
            resources.ApplyResources(this.WPNAV_SPEED_DN, "WPNAV_SPEED_DN");
            this.WPNAV_SPEED_DN.Max = 1F;
            this.WPNAV_SPEED_DN.Min = 0F;
            this.WPNAV_SPEED_DN.Name = "WPNAV_SPEED_DN";
            this.WPNAV_SPEED_DN.ParamName = null;
            this.WPNAV_SPEED_DN.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // WPNAV_RADIUS
            // 
            resources.ApplyResources(this.WPNAV_RADIUS, "WPNAV_RADIUS");
            this.WPNAV_RADIUS.Max = 1F;
            this.WPNAV_RADIUS.Min = 0F;
            this.WPNAV_RADIUS.Name = "WPNAV_RADIUS";
            this.WPNAV_RADIUS.ParamName = null;
            this.WPNAV_RADIUS.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // WPNAV_SPEED
            // 
            resources.ApplyResources(this.WPNAV_SPEED, "WPNAV_SPEED");
            this.WPNAV_SPEED.Max = 1F;
            this.WPNAV_SPEED.Min = 0F;
            this.WPNAV_SPEED.Name = "WPNAV_SPEED";
            this.WPNAV_SPEED.ParamName = null;
            this.WPNAV_SPEED.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // THR_ALT_P
            // 
            resources.ApplyResources(this.THR_ALT_P, "THR_ALT_P");
            this.THR_ALT_P.Max = 1F;
            this.THR_ALT_P.Min = 0F;
            this.THR_ALT_P.Name = "THR_ALT_P";
            this.THR_ALT_P.ParamName = null;
            this.THR_ALT_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // mavlinkNumericUpDownatc_input_tc
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownatc_input_tc, "mavlinkNumericUpDownatc_input_tc");
            this.mavlinkNumericUpDownatc_input_tc.Max = 1F;
            this.mavlinkNumericUpDownatc_input_tc.Min = 0F;
            this.mavlinkNumericUpDownatc_input_tc.Name = "mavlinkNumericUpDownatc_input_tc";
            this.mavlinkNumericUpDownatc_input_tc.ParamName = null;
            this.mavlinkNumericUpDownatc_input_tc.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // HLD_LAT_P
            // 
            resources.ApplyResources(this.HLD_LAT_P, "HLD_LAT_P");
            this.HLD_LAT_P.Max = 1F;
            this.HLD_LAT_P.Min = 0F;
            this.HLD_LAT_P.Name = "HLD_LAT_P";
            this.HLD_LAT_P.ParamName = null;
            this.HLD_LAT_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // mavlinkNumericUpDownatc_accel_y_max
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownatc_accel_y_max, "mavlinkNumericUpDownatc_accel_y_max");
            this.mavlinkNumericUpDownatc_accel_y_max.Max = 1F;
            this.mavlinkNumericUpDownatc_accel_y_max.Min = 0F;
            this.mavlinkNumericUpDownatc_accel_y_max.Name = "mavlinkNumericUpDownatc_accel_y_max";
            this.mavlinkNumericUpDownatc_accel_y_max.ParamName = null;
            this.mavlinkNumericUpDownatc_accel_y_max.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // STB_YAW_P
            // 
            resources.ApplyResources(this.STB_YAW_P, "STB_YAW_P");
            this.STB_YAW_P.Max = 1F;
            this.STB_YAW_P.Min = 0F;
            this.STB_YAW_P.Name = "STB_YAW_P";
            this.STB_YAW_P.ParamName = null;
            this.STB_YAW_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // mavlinkNumericUpDownatc_accel_p_max
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownatc_accel_p_max, "mavlinkNumericUpDownatc_accel_p_max");
            this.mavlinkNumericUpDownatc_accel_p_max.Max = 1F;
            this.mavlinkNumericUpDownatc_accel_p_max.Min = 0F;
            this.mavlinkNumericUpDownatc_accel_p_max.Name = "mavlinkNumericUpDownatc_accel_p_max";
            this.mavlinkNumericUpDownatc_accel_p_max.ParamName = null;
            this.mavlinkNumericUpDownatc_accel_p_max.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // STB_PIT_P
            // 
            resources.ApplyResources(this.STB_PIT_P, "STB_PIT_P");
            this.STB_PIT_P.Max = 1F;
            this.STB_PIT_P.Min = 0F;
            this.STB_PIT_P.Name = "STB_PIT_P";
            this.STB_PIT_P.ParamName = null;
            this.STB_PIT_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // mavlinkNumericUpDownatc_accel_r_max
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownatc_accel_r_max, "mavlinkNumericUpDownatc_accel_r_max");
            this.mavlinkNumericUpDownatc_accel_r_max.Max = 1F;
            this.mavlinkNumericUpDownatc_accel_r_max.Min = 0F;
            this.mavlinkNumericUpDownatc_accel_r_max.Name = "mavlinkNumericUpDownatc_accel_r_max";
            this.mavlinkNumericUpDownatc_accel_r_max.ParamName = null;
            this.mavlinkNumericUpDownatc_accel_r_max.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // STB_RLL_P
            // 
            resources.ApplyResources(this.STB_RLL_P, "STB_RLL_P");
            this.STB_RLL_P.Max = 1F;
            this.STB_RLL_P.Min = 0F;
            this.STB_RLL_P.Name = "STB_RLL_P";
            this.STB_RLL_P.ParamName = null;
            this.STB_RLL_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // ATC_RAT_YAW_FLTT
            // 
            resources.ApplyResources(this.ATC_RAT_YAW_FLTT, "ATC_RAT_YAW_FLTT");
            this.ATC_RAT_YAW_FLTT.Max = 1F;
            this.ATC_RAT_YAW_FLTT.Min = 0F;
            this.ATC_RAT_YAW_FLTT.Name = "ATC_RAT_YAW_FLTT";
            this.ATC_RAT_YAW_FLTT.ParamName = null;
            this.ATC_RAT_YAW_FLTT.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // ATC_RAT_YAW_FLTD
            // 
            resources.ApplyResources(this.ATC_RAT_YAW_FLTD, "ATC_RAT_YAW_FLTD");
            this.ATC_RAT_YAW_FLTD.Max = 1F;
            this.ATC_RAT_YAW_FLTD.Min = 0F;
            this.ATC_RAT_YAW_FLTD.Name = "ATC_RAT_YAW_FLTD";
            this.ATC_RAT_YAW_FLTD.ParamName = null;
            this.ATC_RAT_YAW_FLTD.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_YAW_FILT
            // 
            resources.ApplyResources(this.RATE_YAW_FILT, "RATE_YAW_FILT");
            this.RATE_YAW_FILT.Max = 1F;
            this.RATE_YAW_FILT.Min = 0F;
            this.RATE_YAW_FILT.Name = "RATE_YAW_FILT";
            this.RATE_YAW_FILT.ParamName = null;
            this.RATE_YAW_FILT.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_YAW_D
            // 
            resources.ApplyResources(this.RATE_YAW_D, "RATE_YAW_D");
            this.RATE_YAW_D.Max = 1F;
            this.RATE_YAW_D.Min = 0F;
            this.RATE_YAW_D.Name = "RATE_YAW_D";
            this.RATE_YAW_D.ParamName = null;
            this.RATE_YAW_D.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_YAW_IMAX
            // 
            resources.ApplyResources(this.RATE_YAW_IMAX, "RATE_YAW_IMAX");
            this.RATE_YAW_IMAX.Max = 1F;
            this.RATE_YAW_IMAX.Min = 0F;
            this.RATE_YAW_IMAX.Name = "RATE_YAW_IMAX";
            this.RATE_YAW_IMAX.ParamName = null;
            this.RATE_YAW_IMAX.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_YAW_I
            // 
            resources.ApplyResources(this.RATE_YAW_I, "RATE_YAW_I");
            this.RATE_YAW_I.Max = 1F;
            this.RATE_YAW_I.Min = 0F;
            this.RATE_YAW_I.Name = "RATE_YAW_I";
            this.RATE_YAW_I.ParamName = null;
            this.RATE_YAW_I.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_YAW_P
            // 
            resources.ApplyResources(this.RATE_YAW_P, "RATE_YAW_P");
            this.RATE_YAW_P.Max = 1F;
            this.RATE_YAW_P.Min = 0F;
            this.RATE_YAW_P.Name = "RATE_YAW_P";
            this.RATE_YAW_P.ParamName = null;
            this.RATE_YAW_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // ATC_RAT_RLL_FLTT
            // 
            resources.ApplyResources(this.ATC_RAT_RLL_FLTT, "ATC_RAT_RLL_FLTT");
            this.ATC_RAT_RLL_FLTT.Max = 1F;
            this.ATC_RAT_RLL_FLTT.Min = 0F;
            this.ATC_RAT_RLL_FLTT.Name = "ATC_RAT_RLL_FLTT";
            this.ATC_RAT_RLL_FLTT.ParamName = null;
            this.ATC_RAT_RLL_FLTT.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // ATC_RAT_RLL_FLTD
            // 
            resources.ApplyResources(this.ATC_RAT_RLL_FLTD, "ATC_RAT_RLL_FLTD");
            this.ATC_RAT_RLL_FLTD.Max = 1F;
            this.ATC_RAT_RLL_FLTD.Min = 0F;
            this.ATC_RAT_RLL_FLTD.Name = "ATC_RAT_RLL_FLTD";
            this.ATC_RAT_RLL_FLTD.ParamName = null;
            this.ATC_RAT_RLL_FLTD.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_PIT_FILT
            // 
            resources.ApplyResources(this.RATE_PIT_FILT, "RATE_PIT_FILT");
            this.RATE_PIT_FILT.Max = 1F;
            this.RATE_PIT_FILT.Min = 0F;
            this.RATE_PIT_FILT.Name = "RATE_PIT_FILT";
            this.RATE_PIT_FILT.ParamName = null;
            this.RATE_PIT_FILT.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_PIT_D
            // 
            resources.ApplyResources(this.RATE_PIT_D, "RATE_PIT_D");
            this.RATE_PIT_D.Max = 1F;
            this.RATE_PIT_D.Min = 0F;
            this.RATE_PIT_D.Name = "RATE_PIT_D";
            this.RATE_PIT_D.ParamName = null;
            this.RATE_PIT_D.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_PIT_IMAX
            // 
            resources.ApplyResources(this.RATE_PIT_IMAX, "RATE_PIT_IMAX");
            this.RATE_PIT_IMAX.Max = 1F;
            this.RATE_PIT_IMAX.Min = 0F;
            this.RATE_PIT_IMAX.Name = "RATE_PIT_IMAX";
            this.RATE_PIT_IMAX.ParamName = null;
            this.RATE_PIT_IMAX.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_PIT_I
            // 
            resources.ApplyResources(this.RATE_PIT_I, "RATE_PIT_I");
            this.RATE_PIT_I.Max = 1F;
            this.RATE_PIT_I.Min = 0F;
            this.RATE_PIT_I.Name = "RATE_PIT_I";
            this.RATE_PIT_I.ParamName = null;
            this.RATE_PIT_I.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_PIT_P
            // 
            resources.ApplyResources(this.RATE_PIT_P, "RATE_PIT_P");
            this.RATE_PIT_P.Max = 1F;
            this.RATE_PIT_P.Min = 0F;
            this.RATE_PIT_P.Name = "RATE_PIT_P";
            this.RATE_PIT_P.ParamName = null;
            this.RATE_PIT_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // ATC_RAT_PIT_FLTT
            // 
            resources.ApplyResources(this.ATC_RAT_PIT_FLTT, "ATC_RAT_PIT_FLTT");
            this.ATC_RAT_PIT_FLTT.Max = 1F;
            this.ATC_RAT_PIT_FLTT.Min = 0F;
            this.ATC_RAT_PIT_FLTT.Name = "ATC_RAT_PIT_FLTT";
            this.ATC_RAT_PIT_FLTT.ParamName = null;
            this.ATC_RAT_PIT_FLTT.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // ATC_RAT_PIT_FLTD
            // 
            resources.ApplyResources(this.ATC_RAT_PIT_FLTD, "ATC_RAT_PIT_FLTD");
            this.ATC_RAT_PIT_FLTD.Max = 1F;
            this.ATC_RAT_PIT_FLTD.Min = 0F;
            this.ATC_RAT_PIT_FLTD.Name = "ATC_RAT_PIT_FLTD";
            this.ATC_RAT_PIT_FLTD.ParamName = null;
            this.ATC_RAT_PIT_FLTD.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_RLL_FILT
            // 
            resources.ApplyResources(this.RATE_RLL_FILT, "RATE_RLL_FILT");
            this.RATE_RLL_FILT.Max = 1F;
            this.RATE_RLL_FILT.Min = 0F;
            this.RATE_RLL_FILT.Name = "RATE_RLL_FILT";
            this.RATE_RLL_FILT.ParamName = null;
            this.RATE_RLL_FILT.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_RLL_D
            // 
            resources.ApplyResources(this.RATE_RLL_D, "RATE_RLL_D");
            this.RATE_RLL_D.Max = 1F;
            this.RATE_RLL_D.Min = 0F;
            this.RATE_RLL_D.Name = "RATE_RLL_D";
            this.RATE_RLL_D.ParamName = null;
            this.RATE_RLL_D.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_RLL_IMAX
            // 
            resources.ApplyResources(this.RATE_RLL_IMAX, "RATE_RLL_IMAX");
            this.RATE_RLL_IMAX.Max = 1F;
            this.RATE_RLL_IMAX.Min = 0F;
            this.RATE_RLL_IMAX.Name = "RATE_RLL_IMAX";
            this.RATE_RLL_IMAX.ParamName = null;
            this.RATE_RLL_IMAX.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_RLL_I
            // 
            resources.ApplyResources(this.RATE_RLL_I, "RATE_RLL_I");
            this.RATE_RLL_I.Max = 1F;
            this.RATE_RLL_I.Min = 0F;
            this.RATE_RLL_I.Name = "RATE_RLL_I";
            this.RATE_RLL_I.ParamName = null;
            this.RATE_RLL_I.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // RATE_RLL_P
            // 
            resources.ApplyResources(this.RATE_RLL_P, "RATE_RLL_P");
            this.RATE_RLL_P.Max = 1F;
            this.RATE_RLL_P.Min = 0F;
            this.RATE_RLL_P.Name = "RATE_RLL_P";
            this.RATE_RLL_P.ParamName = null;
            this.RATE_RLL_P.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_LOG_BAT_OPT
            // 
            resources.ApplyResources(this.INS_LOG_BAT_OPT, "INS_LOG_BAT_OPT");
            this.INS_LOG_BAT_OPT.Max = 1F;
            this.INS_LOG_BAT_OPT.Min = 0F;
            this.INS_LOG_BAT_OPT.Name = "INS_LOG_BAT_OPT";
            this.INS_LOG_BAT_OPT.ParamName = null;
            this.INS_LOG_BAT_OPT.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // INS_HNTCH_ENABLE
            // 
            this.INS_HNTCH_ENABLE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.INS_HNTCH_ENABLE.DropDownWidth = 170;
            resources.ApplyResources(this.INS_HNTCH_ENABLE, "INS_HNTCH_ENABLE");
            this.INS_HNTCH_ENABLE.FormattingEnabled = true;
            this.INS_HNTCH_ENABLE.Name = "INS_HNTCH_ENABLE";
            this.INS_HNTCH_ENABLE.ParamName = null;
            this.INS_HNTCH_ENABLE.SubControl = null;
            this.INS_HNTCH_ENABLE.ValueUpdated += new System.EventHandler(this.numeric_ValueUpdated);
            // 
            // ConfigArducopter
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.label52);
            this.Controls.Add(this.CH6_OPTION);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.myLabel6);
            this.Controls.Add(this.CH10_OPTION);
            this.Controls.Add(this.myLabel5);
            this.Controls.Add(this.CH9_OPTION);
            this.Controls.Add(this.myLabel4);
            this.Controls.Add(this.CH8_OPTION);
            this.Controls.Add(this.BUT_refreshpart);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BUT_rerequestparams);
            this.Controls.Add(this.BUT_writePIDS);
            this.Controls.Add(this.myLabel3);
            this.Controls.Add(this.TUNE_LOW);
            this.Controls.Add(this.TUNE_HIGH);
            this.Controls.Add(this.myLabel2);
            this.Controls.Add(this.TUNE);
            this.Controls.Add(this.myLabel1);
            this.Controls.Add(this.CH7_OPTION);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.CHK_lockrollpitch);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox19);
            this.Controls.Add(this.groupBox20);
            this.Controls.Add(this.groupBox21);
            this.Controls.Add(this.groupBox22);
            this.Controls.Add(this.groupBox23);
            this.Controls.Add(this.groupBox24);
            this.Controls.Add(this.groupBox25);
            this.Name = "ConfigArducopter";
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox19.ResumeLayout(false);
            this.groupBox20.ResumeLayout(false);
            this.groupBox21.ResumeLayout(false);
            this.groupBox22.ResumeLayout(false);
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox24.ResumeLayout(false);
            this.groupBox24.PerformLayout();
            this.groupBox25.ResumeLayout(false);
            this.groupBox25.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.INS_NOTCH_ATT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_NOTCH_BW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_NOTCH_FREQ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_HMNCS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_OPTS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_BW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_ATT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_FREQ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_REF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_HNTCH_MODE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_ACCEL_FILTER)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_GYRO_FILTER)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_ACCEL_D)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_ACCEL_IMAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_ACCEL_I)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_ACCEL_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LOITER_LAT_D)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LOITER_LAT_IMAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LOITER_LAT_I)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LOITER_LAT_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TUNE_LOW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TUNE_HIGH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_RATE_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WPNAV_SPEED_UP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WPNAV_LOIT_SPEED)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WPNAV_SPEED_DN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WPNAV_RADIUS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WPNAV_SPEED)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.THR_ALT_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownatc_input_tc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HLD_LAT_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownatc_accel_y_max)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.STB_YAW_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownatc_accel_p_max)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.STB_PIT_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownatc_accel_r_max)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.STB_RLL_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_YAW_FLTT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_YAW_FLTD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_YAW_FILT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_YAW_D)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_YAW_IMAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_YAW_I)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_YAW_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_RLL_FLTT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_RLL_FLTD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_PIT_FILT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_PIT_D)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_PIT_IMAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_PIT_I)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_PIT_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_PIT_FLTT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ATC_RAT_PIT_FLTD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_RLL_FILT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_RLL_D)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_RLL_IMAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_RLL_I)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RATE_RLL_P)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.INS_LOG_BAT_OPT)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

  

        #endregion

        private Label myLabel3;
        private Controls.MavlinkNumericUpDown TUNE_LOW;
        private Controls.MavlinkNumericUpDown TUNE_HIGH;
        private Label myLabel2;
        private Controls.MavlinkComboBox  TUNE;
        private Label myLabel1;
        private Controls.MavlinkComboBox CH7_OPTION;
        private System.Windows.Forms.GroupBox groupBox5;
        private Controls.MavlinkNumericUpDown THR_RATE_P;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.CheckBox CHK_lockrollpitch;
        private System.Windows.Forms.GroupBox groupBox4;
        private Controls.MavlinkNumericUpDown WPNAV_SPEED_UP;
        private System.Windows.Forms.Label label27;
        private Controls.MavlinkNumericUpDown WPNAV_LOIT_SPEED;
        private System.Windows.Forms.Label label9;
        private Controls.MavlinkNumericUpDown WPNAV_SPEED_DN;
        private System.Windows.Forms.Label label13;
        private Controls.MavlinkNumericUpDown WPNAV_RADIUS;
        private System.Windows.Forms.Label label15;
        private Controls.MavlinkNumericUpDown WPNAV_SPEED;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox7;
        private Controls.MavlinkNumericUpDown THR_ALT_P;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.GroupBox groupBox19;
        private Controls.MavlinkNumericUpDown HLD_LAT_P;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.GroupBox groupBox20;
        private Controls.MavlinkNumericUpDown STB_YAW_P;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.GroupBox groupBox21;
        private Controls.MavlinkNumericUpDown STB_PIT_P;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.GroupBox groupBox22;
        private Controls.MavlinkNumericUpDown STB_RLL_P;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.GroupBox groupBox23;
        private Controls.MavlinkNumericUpDown RATE_YAW_D;
        private System.Windows.Forms.Label label10;
        private Controls.MavlinkNumericUpDown RATE_YAW_IMAX;
        private System.Windows.Forms.Label label47;
        private Controls.MavlinkNumericUpDown RATE_YAW_I;
        private System.Windows.Forms.Label label77;
        private Controls.MavlinkNumericUpDown RATE_YAW_P;
        private System.Windows.Forms.Label label82;
        private System.Windows.Forms.GroupBox groupBox24;
        private Controls.MavlinkNumericUpDown RATE_PIT_D;
        private System.Windows.Forms.Label label11;
        private Controls.MavlinkNumericUpDown RATE_PIT_IMAX;
        private System.Windows.Forms.Label label84;
        private Controls.MavlinkNumericUpDown RATE_PIT_I;
        private System.Windows.Forms.Label label86;
        private Controls.MavlinkNumericUpDown RATE_PIT_P;
        private System.Windows.Forms.Label label87;
        private System.Windows.Forms.GroupBox groupBox25;
        private Controls.MavlinkNumericUpDown RATE_RLL_D;
        private System.Windows.Forms.Label label17;
        private Controls.MavlinkNumericUpDown RATE_RLL_IMAX;
        private System.Windows.Forms.Label label88;
        private Controls.MavlinkNumericUpDown RATE_RLL_I;
        private System.Windows.Forms.Label label90;
        private Controls.MavlinkNumericUpDown RATE_RLL_P;
        private System.Windows.Forms.Label label91;
        private System.Windows.Forms.ToolTip toolTip1;
        private Controls.MyButton BUT_writePIDS;
        private Controls.MyButton BUT_rerequestparams;
        private System.Windows.Forms.GroupBox groupBox1;
        private Controls.MavlinkNumericUpDown LOITER_LAT_D;
        private System.Windows.Forms.Label label1;
        private Controls.MavlinkNumericUpDown LOITER_LAT_IMAX;
        private System.Windows.Forms.Label label2;
        private Controls.MavlinkNumericUpDown LOITER_LAT_I;
        private System.Windows.Forms.Label label3;
        private Controls.MavlinkNumericUpDown LOITER_LAT_P;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private Controls.MavlinkNumericUpDown THR_ACCEL_D;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private Controls.MavlinkNumericUpDown THR_ACCEL_IMAX;
        private Controls.MavlinkNumericUpDown THR_ACCEL_I;
        private System.Windows.Forms.Label label7;
        private Controls.MavlinkNumericUpDown THR_ACCEL_P;
        private System.Windows.Forms.Label label8;
        private Controls.MyButton BUT_refreshpart;
        private Label myLabel4;
        private Controls.MavlinkComboBox CH8_OPTION;
        private Controls.MavlinkNumericUpDown RATE_YAW_FILT;
        private System.Windows.Forms.Label label18;
        private Controls.MavlinkNumericUpDown RATE_PIT_FILT;
        private System.Windows.Forms.Label label14;
        private Controls.MavlinkNumericUpDown RATE_RLL_FILT;
        private System.Windows.Forms.Label label12;
        private Label myLabel5;
        private Controls.MavlinkComboBox CH9_OPTION;
        private Label myLabel6;
        private Controls.MavlinkComboBox CH10_OPTION;
        private Controls.MavlinkNumericUpDown mavlinkNumericUpDownatc_input_tc;
        private Label label23;
        private Controls.MavlinkNumericUpDown mavlinkNumericUpDownatc_accel_y_max;
        private Label label21;
        private Controls.MavlinkNumericUpDown mavlinkNumericUpDownatc_accel_p_max;
        private Label label20;
        private Controls.MavlinkNumericUpDown mavlinkNumericUpDownatc_accel_r_max;
        private Label label19;
        private GroupBox groupBox3;
        private Controls.MavlinkNumericUpDown INS_GYRO_FILTER;
        private Label label24;
        private Controls.MavlinkNumericUpDown INS_ACCEL_FILTER;
        private Label label26;
        private GroupBox groupBox6;
        private Label label36;
        private Label label34;
        private GroupBox groupBox9;
        private GroupBox groupBox8;
        private Label label37;
        private Controls.MavlinkNumericUpDown INS_NOTCH_ATT;
        private Label label40;
        private Controls.MavlinkNumericUpDown INS_NOTCH_BW;
        private Label label39;
        private Controls.MavlinkNumericUpDown INS_NOTCH_FREQ;
        private Label label38;
        private Controls.MavlinkNumericUpDown INS_HNTCH_FREQ;
        private Label label41;
        private Controls.MavlinkNumericUpDown INS_HNTCH_REF;
        private Label label43;
        private Controls.MavlinkNumericUpDown INS_HNTCH_MODE;
        private Label label44;
        private Label label45;
        private Controls.MavlinkNumericUpDown INS_HNTCH_HMNCS;
        private Label label51;
        private Controls.MavlinkNumericUpDown INS_HNTCH_OPTS;
        private Label label50;
        private Controls.MavlinkNumericUpDown INS_HNTCH_BW;
        private Label label49;
        private Controls.MavlinkNumericUpDown INS_HNTCH_ATT;
        private Label label48;
        private Label label52;
        private Controls.MavlinkComboBox CH6_OPTION;
        private Controls.MavlinkNumericUpDown ATC_RAT_PIT_FLTT;
        private Label label28;
        private Controls.MavlinkNumericUpDown ATC_RAT_PIT_FLTD;
        private Label P_FLTD;
        private Controls.MavlinkNumericUpDown ATC_RAT_YAW_FLTT;
        private Label label33;
        private Controls.MavlinkNumericUpDown ATC_RAT_YAW_FLTD;
        private Label label32;
        private Controls.MavlinkNumericUpDown ATC_RAT_RLL_FLTT;
        private Label label30;
        private Controls.MavlinkNumericUpDown ATC_RAT_RLL_FLTD;
        private Label label29;
        private Controls.MavlinkComboBox INS_LOG_BAT_MASK;
        private Controls.MavlinkComboBox INS_NOTCH_ENABLE;
        private Controls.MavlinkComboBox INS_HNTCH_ENABLE;
        private Controls.MavlinkNumericUpDown INS_LOG_BAT_OPT;
    }
}
