// This example program uses Mission Planner's on the fly compiled plugin system
// 
// Initial parameters calculation based on Leonard Hall's excellent Tuning Guide 
// And it's excel sheet implementation by Shawn aka xfacta on Ardupilot Discuss forum.
//
// Copy this file as it is to the Mission Planner/Plugins directory
// When connected to an Arduopter press ALT+A to start calculator

using System;
using System.Collections.Generic;
using System.Linq;
using MissionPlanner.Utilities;
using System.Reactive.Linq;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.InitialParamCalc
{
    public class InitialParamPlugin : MissionPlanner.Plugin.Plugin
    {
        //Variables for calculating initial paramateres

        static double prop_size;
        static double batt_cells;
        static double batt_cell_max_voltage;
        static double batt_cell_min_voltage;


        static double acro_yaw_p;
        static double atc_accel_p_max;
        static double atc_accel_r_max;
        static double atc_accel_y_max;
        static double atc_rat_pit_fltd;
        static double atc_rat_pit_flte;
        static double atc_rat_pit_fltt;
        static double atc_rat_rll_fltd;
        static double atc_rat_rll_flte;
        static double atc_rat_rll_fltt;
        static double atc_rat_yaw_fltd;
        static double atc_rat_yaw_flte;
        static double atc_rat_yaw_fltt;
        static double atc_thr_mix_man;
        static double ins_accel_filter;
        static double ins_gyro_filter;
        static double mot_thst_expo;
        static double mot_thst_hover;

        static double batt_arm_volt;
        static double batt_crt_volt;
        static double batt_low_volt;
        static double mot_bat_volt_max;
        static double mot_bat_volt_min;


        //Additional variables 
        MissionPlanner.Controls.MyButton button;

        public override string Name
        {
            get { return "Initial Parameters"; }
        }

        public override string Version
        {
            get { return "1.1"; }
        }

        public override string Author
        {
            get { return "EosBandi"; }
        }

        public override bool Init()
        {
            //Capture keyboard presses from Main Instance
            MainV2.instance.ProcessCmdKeyCallback += this.Instance_ProcessCmdKeyCallback;
            return true;
        }
        public override bool Loaded()
        {
            return true;
        }

        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }

        private bool Instance_ProcessCmdKeyCallback(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {

            //Add our shortcut

            if (keyData == (Keys.Alt | Keys.A))
            {
                do_parameters(this, null);
                return true;
            }
            return false;
        }

        // Main 
        void do_parameters(object sender, EventArgs e)
        {
            var new_params = new Dictionary<string, double>();


            double prop = 9;
            double cellcount = 4;
            double cellmax = 4.2;
            double cellmin = 3.3;


            //Add intro and some warnings
            CustomMessageBox.Show("This plugin will calculate some initial parameters based on battery and prop size for a new copter setup.\r\n\r\n" +
                                  "Please make sure that before running this plugin and updating calculated parameters:\r\n" +
                                  "ALL INITIAL SETUPS ARE DONE (Calibrations, frame settings, motor tests)\r\n" +
                                  "BATTERY VOLTAGE MONITORING IS SET AND WORKING\r\n\r\n" +
                                  "Note: INS_GYRO_FILTER with a value other than 20 is optional and probably only for small frames/props " +
                                  "At first you can keep it at 20\r\n", "Initial Parameter Calculator");

            //Check environment

            if (!Host.cs.connected)
            {
                CustomMessageBox.Show("Please connect first!", "Initial paremeter calculator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Host.cs.firmware != ArduPilot.Firmwares.ArduCopter2)
            {
                CustomMessageBox.Show("Initial parameter calculation works with Arducopter only!", "Initial paremeter calculator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // Get input parameters
            if (MissionPlanner.Controls.InputBox.Show("Initial parameter calculator", "Enter airscrew size in inch", ref prop) != DialogResult.OK) return;
            if (MissionPlanner.Controls.InputBox.Show("Initial parameter calculator", "Enter battery cellcount", ref cellcount) != DialogResult.OK) return;
            if (MissionPlanner.Controls.InputBox.Show("Initial parameter calculator", "Enter battery cell fully charged voltage\r\nLiPo - 4.2, LipoHV - 4.35, LiIon - 4.1 or 4.2", ref cellmax) != DialogResult.OK) return;
            if (MissionPlanner.Controls.InputBox.Show("Initial parameter calculator", "Enter battery cell fully discharged voltage\r\nLiPo/LipoHV - 3.3, LiIon - 2.8", ref cellmin) != DialogResult.OK) return;



            //Ok we have all input and we are connected to an ArduCopter

            //Convert for sanity check
            prop_size = prop.ConvertToDouble();
            batt_cells = cellcount.ConvertToDouble();
            batt_cell_max_voltage = cellmax.ConvertToDouble();
            batt_cell_min_voltage = cellmin.ConvertToDouble();


            if (prop_size <= 0 || batt_cells < 1)
            {
                CustomMessageBox.Show("Invalid input data!", "Initial paremeter calculator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            //Do calculation
            calc_values();

            //Fill up the list of params to change
            new_params.Add("ACRO_YAW_P", acro_yaw_p);
            new_params.Add("ATC_ACCEL_P_MAX", atc_accel_p_max);
            new_params.Add("ATC_ACCEL_R_MAX", atc_accel_r_max);
            new_params.Add("ATC_ACCEL_Y_MAX", atc_accel_y_max);

            //Filters has different name in 4.x and in 3.x
            if (Host.cs.version.Major == 4)
            {

                new_params.Add("ATC_RAT_PIT_FLTD", atc_rat_pit_fltd);
                new_params.Add("ATC_RAT_PIT_FLTE", atc_rat_pit_flte);
                new_params.Add("ATC_RAT_PIT_FLTT", atc_rat_pit_fltt);
                new_params.Add("ATC_RAT_RLL_FLTD", atc_rat_rll_fltd);
                new_params.Add("ATC_RAT_RLL_FLTE", atc_rat_rll_flte);
                new_params.Add("ATC_RAT_RLL_FLTT", atc_rat_rll_fltt);
                new_params.Add("ATC_RAT_YAW_FLTD", atc_rat_yaw_fltd);
                new_params.Add("ATC_RAT_YAW_FLTE", atc_rat_yaw_flte);
                new_params.Add("ATC_RAT_YAW_FLTT", atc_rat_yaw_fltt);
            }
            else
            {
                new_params.Add("ATC_RAT_PIT_FILT", atc_rat_pit_fltd);
                new_params.Add("ATC_RAT_RLL_FILT", atc_rat_rll_fltd);
                new_params.Add("ATC_RAT_YAW_FILT", atc_rat_yaw_flte);

            }

            new_params.Add("ATC_THR_MIX_MAN", atc_thr_mix_man);
            new_params.Add("INS_ACCEL_FILTER", ins_accel_filter);
            new_params.Add("INS_GYRO_FILTER", ins_gyro_filter);
            new_params.Add("MOT_THST_EXPO", mot_thst_expo);
            new_params.Add("MOT_THST_HOVER", mot_thst_hover);
            new_params.Add("BATT_ARM_VOLT", batt_arm_volt);
            new_params.Add("BATT_CRT_VOLT", batt_crt_volt);
            new_params.Add("BATT_LOW_VOLT", batt_low_volt);
            new_params.Add("MOT_BAT_VOLT_MAX", mot_bat_volt_max);
            new_params.Add("MOT_BAT_VOLT_MIN", mot_bat_volt_min);

            Form paramCompareForm = new ParamCompare(null, MainV2.comPort.MAV.param, new_params);
            ThemeManager.ApplyThemeTo(paramCompareForm);
            button = paramCompareForm.Controls.Find("BUT_save", true).FirstOrDefault() as MissionPlanner.Controls.MyButton;
            button.Text = "Write to FC";

            paramCompareForm.ShowDialog();
            CustomMessageBox.Show("Initial Parameters succesfully updated.\r\nCheck parameters before flight!\r\n\r\nAfter test flight :\r\n\tSet ATC_THR_MIX_MAN to 0.5\r\n\tSet PSC_ACCZ_P to MOT_THST_HOVER\r\n\tSet PSC_ACCZ_I to 2*MOT_THST_HOVER\r\n\r\nHappy flying!", "Initial parameter calculator");

        }

        static double RoundTo(double value, int precision)
		{
			if (precision < -4 && precision > 15)
				throw new ArgumentOutOfRangeException("precision", "Must be and integer between -4 and 15");
	
			if (precision >= 0) return Math.Round(value, precision);
			else
			{
				precision = (int)Math.Pow(10, Math.Abs(precision));
				value = value + (5 * precision / 10);
				return Math.Round(value - (value % precision), 0);
			}
		}

        // Do calculations
        static void calc_values()
        {
            atc_accel_y_max = Math.Max(8000,RoundTo(-900 * prop_size + 36000,-2));
			
            acro_yaw_p = 0.5 * atc_accel_y_max / 4500;

            atc_accel_p_max = Math.Max(10000,RoundTo(-2.613267*Math.Pow(prop_size,3)+343.39216*Math.Pow(prop_size,2)-15083.7121*prop_size+235771,-2));
            atc_accel_r_max = atc_accel_p_max;

            ins_gyro_filter = Math.Max(20,Math.Round((289.22 * Math.Pow(prop_size, -0.838)), 0));

            atc_rat_pit_fltd = Math.Max(10,ins_gyro_filter / 2);
            atc_rat_pit_flte = 0;
            atc_rat_pit_fltt = Math.Max(10,ins_gyro_filter / 2);
            atc_rat_rll_fltd = Math.Max(10,ins_gyro_filter / 2);
            atc_rat_rll_flte = 0;
            atc_rat_rll_fltt = Math.Max(10,ins_gyro_filter / 2);
            atc_rat_yaw_fltd = 0;
            atc_rat_yaw_flte = 2;
            atc_rat_yaw_fltt = Math.Max(10,ins_gyro_filter / 2);

            atc_thr_mix_man = 0.1;
            ins_accel_filter = 20;
            mot_thst_expo = Math.Round(0.1405 * Math.Log(prop_size) + 0.3254, 2);
            mot_thst_hover = 0.2;

            batt_arm_volt = (batt_cells - 1) * 0.1 + 3.6 * batt_cells;
            batt_crt_volt = (batt_cell_min_voltage + 0.2) * batt_cells;
            batt_low_volt = (batt_cell_min_voltage + 0.3) * batt_cells;
            mot_bat_volt_max = batt_cell_max_voltage * batt_cells;
            mot_bat_volt_min = batt_cell_min_voltage * batt_cells;

        }
    }
}

