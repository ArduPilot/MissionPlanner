using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigInitialParams : MyUserControl, IActivate
    {

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



        public ConfigInitialParams()
        {
            InitializeComponent();
        }


        public void Activate()
        {
            double prop = 9;
            double cellcount = 4;



            t_prop.Text = prop.ToString();
            t_cellcount.Text = cellcount.ToString();
            cb_tmotor.Checked = false;
            cb_suggested.Checked = false;

            cmb_batterytype.SelectedIndex = 0;


        }


        //Check values;

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
            atc_accel_y_max = Math.Max(8000, RoundTo(-900 * prop_size + 36000, -2));

            acro_yaw_p = 0.5 * atc_accel_y_max / 4500;

            atc_accel_p_max = Math.Max(10000, RoundTo(-2.613267 * Math.Pow(prop_size, 3) + 343.39216 * Math.Pow(prop_size, 2) - 15083.7121 * prop_size + 235771, -2));
            atc_accel_r_max = atc_accel_p_max;

            ins_gyro_filter = Math.Max(20, Math.Round((289.22 * Math.Pow(prop_size, -0.838)), 0));

            atc_rat_pit_fltd = Math.Max(10, ins_gyro_filter / 2);
            atc_rat_pit_flte = 0;
            atc_rat_pit_fltt = Math.Max(10, ins_gyro_filter / 2);
            atc_rat_rll_fltd = Math.Max(10, ins_gyro_filter / 2);
            atc_rat_rll_flte = 0;
            atc_rat_rll_fltt = Math.Max(10, ins_gyro_filter / 2);
            atc_rat_yaw_fltd = 0;
            atc_rat_yaw_flte = 2;
            atc_rat_yaw_fltt = Math.Max(10, ins_gyro_filter / 2);

            atc_thr_mix_man = 0.1;
            ins_accel_filter = 20;
            mot_thst_expo = Math.Round(0.1405 * Math.Log(prop_size) + 0.3254, 2);
            mot_thst_hover = 0.2;

            batt_arm_volt = (batt_cells - 1) * 0.1 + (batt_cell_min_voltage+0.3) * batt_cells;
            batt_crt_volt = (batt_cell_min_voltage + 0.2) * batt_cells;
            batt_low_volt = (batt_cell_min_voltage + 0.3) * batt_cells;
            mot_bat_volt_max = batt_cell_max_voltage * batt_cells;
            mot_bat_volt_min = batt_cell_min_voltage * batt_cells;

        }


        private void btn_docalc_Click(object sender, EventArgs e)
        {

            //Convert for sanity check
            prop_size = t_prop.Text.ConvertToDouble();
            batt_cells = t_cellcount.Text.ConvertToDouble();
            batt_cell_max_voltage = t_cellmax.Text.ConvertToDouble();
            batt_cell_min_voltage = t_cellmin.Text.ConvertToDouble();

            if (prop_size <= 0 )
            {

                CustomMessageBox.Show("Prop size must be larger than zero.", "ERROR!");
                return;

            }

            if (batt_cells < 1)
            {
                CustomMessageBox.Show("Battery cell count must be at least 1.", "ERROR!");
                return;
            }


            calc_values();

            if (cb_tmotor.Checked) mot_thst_expo = 0.2;


            var atc_prefix = "ATC";
            var mot_prefix = "MOT";
            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane)
            {
                atc_prefix = "Q_A";
                mot_prefix = "Q_M";
            }

            var new_params = new Dictionary<string, double>();

            //Fill up the list of params to change
            new_params.Add("ACRO_YAW_P", acro_yaw_p);
            new_params.Add(atc_prefix + "_ACCEL_P_MAX", atc_accel_p_max);
            new_params.Add(atc_prefix + "_ACCEL_R_MAX", atc_accel_r_max);
            new_params.Add(atc_prefix + "_ACCEL_Y_MAX", atc_accel_y_max);

            //Filters has different name in 4.x and in 3.x
            if (MainV2.comPort.MAV.cs.version.Major == 4)
            {

                new_params.Add(atc_prefix + "_RAT_PIT_FLTD", atc_rat_pit_fltd);
                new_params.Add(atc_prefix + "_RAT_PIT_FLTE", atc_rat_pit_flte);
                new_params.Add(atc_prefix + "_RAT_PIT_FLTT", atc_rat_pit_fltt);
                new_params.Add(atc_prefix + "_RAT_RLL_FLTD", atc_rat_rll_fltd);
                new_params.Add(atc_prefix + "_RAT_RLL_FLTE", atc_rat_rll_flte);
                new_params.Add(atc_prefix + "_RAT_RLL_FLTT", atc_rat_rll_fltt);
                new_params.Add(atc_prefix + "_RAT_YAW_FLTD", atc_rat_yaw_fltd);
                new_params.Add(atc_prefix + "_RAT_YAW_FLTE", atc_rat_yaw_flte);
                new_params.Add(atc_prefix + "_RAT_YAW_FLTT", atc_rat_yaw_fltt);
            }
            else
            {
                new_params.Add(atc_prefix + "_RAT_PIT_FILT", atc_rat_pit_fltd);
                new_params.Add(atc_prefix + "_RAT_RLL_FILT", atc_rat_rll_fltd);
                new_params.Add(atc_prefix + "_RAT_YAW_FILT", atc_rat_yaw_flte);

            }

            new_params.Add(atc_prefix + "_THR_MIX_MAN", atc_thr_mix_man);
            new_params.Add("INS_ACCEL_FILTER", ins_accel_filter);
            new_params.Add("INS_GYRO_FILTER", ins_gyro_filter);
            new_params.Add(mot_prefix + "_THST_EXPO", mot_thst_expo);
            new_params.Add(mot_prefix + "_THST_HOVER", mot_thst_hover);
            new_params.Add("BATT_ARM_VOLT", batt_arm_volt);
            new_params.Add("BATT_CRT_VOLT", batt_crt_volt);
            new_params.Add("BATT_LOW_VOLT", batt_low_volt);
            new_params.Add(mot_prefix + "_BAT_VOLT_MAX", mot_bat_volt_max);
            new_params.Add(mot_prefix + "_BAT_VOLT_MIN", mot_bat_volt_min);

            if (cb_tmotor.Checked)
            {
                new_params.Add(mot_prefix + "_PWM_MIN", 1100);
                new_params.Add(mot_prefix + "_PWM_MAX", 1940);

            }

            if (cb_suggested.Checked && MainV2.comPort.MAV.cs.version.Major == 4 && (MainV2.comPort.MAV.cs.firmware != Firmwares.ArduPlane))
            {
                new_params.Add("BATT_FS_CRT_ACT", 1);
                new_params.Add("BATT_FS_LOW_ACT", 2);
                new_params.Add("FENCE_ACTION", 3);
                new_params.Add("FENCE_ALT_MAX", 120);
                new_params.Add("FENCE_ENABLE", 1);
                new_params.Add("FENCE_RADIUS", 150);
                new_params.Add("FENCE_TYPE", 7);

            }

            Form paramCompareForm = new ParamCompare(null, MainV2.comPort.MAV.param, new_params);
            ThemeManager.ApplyThemeTo(paramCompareForm);

            MissionPlanner.Controls.MyButton button = paramCompareForm.Controls.Find("BUT_save", true).FirstOrDefault() as MissionPlanner.Controls.MyButton;
            button.Text = "Write to FC";
            paramCompareForm.StartPosition = FormStartPosition.CenterParent;
            paramCompareForm.ShowDialog();

            if (paramCompareForm.DialogResult == DialogResult.OK)
            {
                CustomMessageBox.Show("Initial Parameters succesfully updated.\r\nCheck parameters before flight!\r\n\r\nAfter test flight :\r\n\tSet ATC_THR_MIX_MAN to 0.5\r\n\tSet PSC_ACCZ_P to MOT_THST_HOVER\r\n\tSet PSC_ACCZ_I to 2*MOT_THST_HOVER\r\n\r\nHappy flying!", "Initial parameter calculator");
            }

            }

        private void cmb_batterytype_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch(cmb_batterytype.SelectedItem)
            {

                case "LiPo":
                    t_cellmax.Text = (4.2).ToString();
                    t_cellmin.Text = (3.3).ToString();
                    break;

                case "LiPoHV":
                    t_cellmax.Text = (4.35).ToString();
                    t_cellmin.Text = (3.3).ToString();
                    break;

                case "LiIon":
                    t_cellmax.Text = (4.1).ToString();
                    t_cellmin.Text = (2.8).ToString();
                    break;
                default:
                    t_cellmax.Text = (4.2).ToString();
                    t_cellmin.Text = (3.3).ToString();
                    break;
            }



        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ardupilot.org/copter/docs/tuning-process-instructions.html");
        }
    }
}
