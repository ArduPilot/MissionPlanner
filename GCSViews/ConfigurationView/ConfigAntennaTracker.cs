﻿using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigAntennaTracker : MyUserControl, IActivate, IDeactivate
    {
        // from http://stackoverflow.com/questions/2512781/winforms-big-paragraph-tooltip/2512895#2512895
        private const int maximumSingleLineTooltipLength = 50;
        private static Hashtable tooltips = new Hashtable();
        private readonly Hashtable changes = new Hashtable();
        internal bool startup = true;

        public ConfigAntennaTracker()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                Enabled = false;
                return;
            }
            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduTracker)
            {
                Enabled = true;
            }
            else
            {
                Enabled = false;
                return;
            }

            startup = true;

            changes.Clear();

            mavlinkComboBox1.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("AHRS_ORIENTATION",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "AHRS_ORIENTATION", MainV2.comPort.MAV.param);
            mavlinkComboBoxservo_yaw_type.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("SERVO_YAW_TYPE",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "SERVO_YAW_TYPE", MainV2.comPort.MAV.param);
            mavlinkComboBoxservo_pitch_type.setup(
            ParameterMetaDataRepository.GetParameterOptionsInt("SERVO_PITCH_TYPE",
                MainV2.comPort.MAV.cs.firmware.ToString()), "SERVO_PITCH_TYPE", MainV2.comPort.MAV.param);

            mavlinkComboBoxalt_source.setup(
            ParameterMetaDataRepository.GetParameterOptionsInt("ALT_SOURCE",
                MainV2.comPort.MAV.cs.firmware.ToString()), "ALT_SOURCE", MainV2.comPort.MAV.param);


            // yaw
            mavlinkNumericUpDown1.setup(900, 2200, 1, 1, "RC1_MIN", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown2.setup(900, 2200, 1, 1, "RC1_MAX", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown3.setup(900, 2200, 1, 1, "RC1_TRIM", MainV2.comPort.MAV.param);
            mavlinkCheckBox1.setup(1, -1, "RC1_REV", MainV2.comPort.MAV.param);

            // pitch
            mavlinkNumericUpDown6.setup(900, 2200, 1, 1, "RC2_MIN", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown5.setup(900, 2200, 1, 1, "RC2_MAX", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown4.setup(900, 2200, 1, 1, "RC2_TRIM", MainV2.comPort.MAV.param);
            mavlinkCheckBox2.setup(-1, 1, "RC2_REV", MainV2.comPort.MAV.param);

            // ranges
            mavlinkNumericUpDown7.setup(0, 360, 1, 1, "YAW_RANGE", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown8.setup(-90, 90, 1, 1, "PITCH_MIN", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown19.setup(-90, 90, 1, 1, "PITCH_MAX", MainV2.comPort.MAV.param);

            // yaw gain
            mavlinkNumericUpDown9.setup(0, 100, 1, .1f, "YAW2SRV_P", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown10.setup(0, 100, 1, .1f, "YAW2SRV_I", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown11.setup(0, 100, 1, .1f, "YAW2SRV_D", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown12.setup(0, 100, 1, 1, "YAW2SRV_IMAX", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown13.setup(0, 100, 1, .1f, "YAW_SLEW_TIME", MainV2.comPort.MAV.param);

            // pitch gain
            mavlinkNumericUpDown14.setup(0, 100, 1, .1f, "PITCH2SRV_P", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown15.setup(0, 100, 1, .1f, "PITCH2SRV_I", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown16.setup(0, 100, 1, .1f, "PITCH2SRV_D", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown17.setup(0, 100, 1, 1, "PITCH2SRV_IMAX", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown18.setup(0, 100, 1, .1f, "PITCH_SLEW_TIME", MainV2.comPort.MAV.param);

            timer1.Start();

            startup = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                BUT_writePIDS_Click(null, null);
                return true;
            }

            return false;
        }

        private static string AddNewLinesForTooltip(string text)
        {
            if (text.Length < maximumSingleLineTooltipLength)
                return text;
            var lineLength = (int)Math.Sqrt(text.Length) * 2;
            var sb = new StringBuilder();
            var currentLinePosition = 0;
            for (var textIndex = 0; textIndex < text.Length; textIndex++)
            {
                // If we have reached the target line length and the next      
                // character is whitespace then begin a new line.   
                if (currentLinePosition >= lineLength &&
                    char.IsWhiteSpace(text[textIndex]))
                {
                    sb.Append(Environment.NewLine);
                    currentLinePosition = 0;
                }
                // If we have just started a new line, skip all the whitespace.    
                if (currentLinePosition == 0)
                    while (textIndex < text.Length && char.IsWhiteSpace(text[textIndex]))
                        textIndex++;
                // Append the next character.     
                if (textIndex < text.Length) sb.Append(text[textIndex]);
                currentLinePosition++;
            }
            return sb.ToString();
        }

        private void BUT_writePIDS_Click(object sender, EventArgs e)
        {
            var temp = (Hashtable)changes.Clone();

            foreach (string value in temp.Keys)
            {
                try
                {
                    if ((float)changes[value] > MainV2.comPort.MAV.param[value].Value * 2.0f)
                        if (
                            CustomMessageBox.Show(value + " has more than doubled the last input. Are you sure?",
                                "Large Value", MessageBoxButtons.YesNo) == (int)DialogResult.No)
                            return;

                    if (MainV2.comPort.BaseStream == null || !MainV2.comPort.BaseStream.IsOpen)
                    {
                        CustomMessageBox.Show("Your are not connected", Strings.ERROR);
                        return;
                    }

                    MainV2.comPort.setParam(value, (float)changes[value]);

                    try
                    {
                        // set control as well
                        var textControls = Controls.Find(value, true);
                        if (textControls.Length > 0)
                        {
                            textControls[0].BackColor = Color.FromArgb(0x43, 0x44, 0x45);
                        }
                    }
                    catch
                    {
                    }
                }
                catch
                {
                    CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, value), Strings.ERROR);
                }
            }
        }

        /// <summary>
        ///     Handles the Click event of the BUT_rerequestparams control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void BUT_rerequestparams_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            ((Control)sender).Enabled = false;

            try
            {
                MainV2.comPort.getParamList();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ErrorReceivingParams + ex, Strings.ERROR);
            }


            ((Control)sender).Enabled = true;


            Activate();
        }

        private void BUT_refreshpart_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            ((Control)sender).Enabled = false;


            updateparam(this);

            ((Control)sender).Enabled = true;


            Activate();
        }

        private void updateparam(Control parentctl)
        {
            foreach (Control ctl in parentctl.Controls)
            {
                if (typeof(NumericUpDown) == ctl.GetType() || typeof(ComboBox) == ctl.GetType())
                {
                    try
                    {
                        MainV2.comPort.GetParam(ctl.Name);
                    }
                    catch
                    {
                    }
                }

                if (ctl.Controls.Count > 0)
                {
                    updateparam(ctl);
                }
            }
        }

        private void BUT_test_yaw_Click(object sender, EventArgs e)
        {
            double output = 1500;

            if (!mavlinkCheckBox1.Checked)
            {
                output = map(myTrackBar1.Value, myTrackBar1.Maximum, myTrackBar1.Minimum,
                    (double)mavlinkNumericUpDown1.Value, (double)mavlinkNumericUpDown2.Value);
            }
            else
            {
                output = map(myTrackBar1.Value, myTrackBar1.Minimum, myTrackBar1.Maximum,
                    (double)mavlinkNumericUpDown1.Value, (double)mavlinkNumericUpDown2.Value);
            }

            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_SET_SERVO, 1, (float)output, 0, 0, 0, 0, 0);
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
            }
        }

        private void BUT_test_pitch_Click(object sender, EventArgs e)
        {
            double output = 1500;
            if (mavlinkCheckBox2.Checked)
            {
                output = map(myTrackBar2.Value, myTrackBar2.Maximum, myTrackBar2.Minimum,
                    (double)mavlinkNumericUpDown6.Value, (double)mavlinkNumericUpDown5.Value);
            }
            else
            {
                output = map(myTrackBar2.Value, myTrackBar2.Minimum, myTrackBar2.Maximum,
                    (double)mavlinkNumericUpDown6.Value, (double)mavlinkNumericUpDown5.Value);
            }

            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_SET_SERVO, 2, (float)output, 0, 0, 0, 0, 0);
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
            }
        }

        private double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbl_yawpwm.Text = MainV2.comPort.MAV.cs.ch1out.ToString();
            lbl_pitchpwm.Text = MainV2.comPort.MAV.cs.ch2out.ToString();
        }

        public void Deactivate()
        {
            timer1.Stop();
        }
    }
}