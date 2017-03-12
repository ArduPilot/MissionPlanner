﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigArdurover : UserControl, IActivate
    {
        // from http://stackoverflow.com/questions/2512781/winforms-big-paragraph-tooltip/2512895#2512895
        private const int maximumSingleLineTooltipLength = 50;
        private static readonly Hashtable tooltips = new Hashtable();
        private readonly Hashtable changes = new Hashtable();
        internal bool startup = true;

        public ConfigArdurover()
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
            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover)
            {
                Enabled = true;
            }
            else
            {
                Enabled = false;
                return;
            }

            startup = true;

            FS_THR_VALUE.setup(0, 0, 1, 1, "FS_THR_VALUE", MainV2.comPort.MAV.param);
            THR_MAX.setup(0, 0, 1, 1, "THR_MAX", MainV2.comPort.MAV.param);
            THR_MIN.setup(0, 0, 1, 1, "THR_MIN", MainV2.comPort.MAV.param);
            CRUISE_THROTTLE.setup(0, 0, 1, 1, "CRUISE_THROTTLE", MainV2.comPort.MAV.param);
            SPEED2THR_IMAX.setup(0, 0, 1, 1, "SPEED2THR_IMAX", MainV2.comPort.MAV.param);
            SPEED2THR_D.setup(0, 0, 1, 1, "SPEED2THR_D", MainV2.comPort.MAV.param);
            SPEED2THR_I.setup(0, 0, 1, 1, "SPEED2THR_I", MainV2.comPort.MAV.param);
            SPEED2THR_P.setup(0, 0, 1, 1, "SPEED2THR_P", MainV2.comPort.MAV.param);
            SPEED_TURN_DIST.setup(0, 0, 1, 1, "SPEED_TURN_DIST", MainV2.comPort.MAV.param);
            SPEED_TURN_GAIN.setup(0, 0, 1, 1, "SPEED_TURN_GAIN", MainV2.comPort.MAV.param);
            CRUISE_SPEED.setup(0, 0, 1, 1, "CRUISE_SPEED", MainV2.comPort.MAV.param);
            STEER2SRV_IMAX.setup(0, 0, 1, 1, "STEER2SRV_IMAX", MainV2.comPort.MAV.param);
            STEER2SRV_D.setup(0, 0, 1, 1, "STEER2SRV_D", MainV2.comPort.MAV.param);
            STEER2SRV_I.setup(0, 0, 1, 1, "STEER2SRV_I", MainV2.comPort.MAV.param);
            STEER2SRV_P.setup(0, 0, 1, 1, "STEER2SRV_P", MainV2.comPort.MAV.param);
            SONAR_DEBOUNCE.setup(0, 0, 1, 1, "SONAR_DEBOUNCE", MainV2.comPort.MAV.param);
            SONAR_TURN_TIME.setup(0, 0, 1, 1, "SONAR_TURN_TIME", MainV2.comPort.MAV.param);
            SONAR_TURN_ANGLE.setup(0, 0, 1, 1, "SONAR_TURN_ANGLE", MainV2.comPort.MAV.param);
            SONAR_TRIGGER_CM.setup(0, 0, 1, 1, "SONAR_TRIGGER_CM", MainV2.comPort.MAV.param);
            WP_RADIUS.setup(0, 0, 1, 1, "WP_RADIUS", MainV2.comPort.MAV.param);
            NAVL1_DAMPING.setup(0, 0, 1, 1, "NAVL1_DAMPING", MainV2.comPort.MAV.param);
            NAVL1_PERIOD.setup(0, 0, 1, 1, "NAVL1_PERIOD", MainV2.comPort.MAV.param);

            changes.Clear();

            // add tooltips to all controls
            foreach (Control control1 in Controls)
            {
                foreach (Control control2 in control1.Controls)
                {
                    if (control2 is MavlinkNumericUpDown)
                    {
                        var ParamName = ((MavlinkNumericUpDown)control2).ParamName;
                        toolTip1.SetToolTip(control2,
                            ParameterMetaDataRepository.GetParameterMetaData(ParamName,
                                ParameterMetaDataConstants.Description, MainV2.comPort.MAV.cs.firmware.ToString()));
                    }
                    if (control2 is MavlinkComboBox)
                    {
                        var ParamName = ((MavlinkComboBox)control2).ParamName;
                        toolTip1.SetToolTip(control2,
                            ParameterMetaDataRepository.GetParameterMetaData(ParamName,
                                ParameterMetaDataConstants.Description, MainV2.comPort.MAV.cs.firmware.ToString()));
                    }
                }
            }

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
            var lineLength = (int) Math.Sqrt(text.Length)*2;
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

        private void ComboBox_Validated(object sender, EventArgs e)
        {
            EEPROM_View_float_TextChanged(sender, e);
        }

        private void Configuration_Validating(object sender, CancelEventArgs e)
        {
            EEPROM_View_float_TextChanged(sender, e);
        }

        internal void EEPROM_View_float_TextChanged(object sender, EventArgs e)
        {
            float value = 0;
            var name = ((Control) sender).Name;

            // do domainupdown state check
            try
            {
                if (sender.GetType() == typeof (NumericUpDown))
                {
                    value = (float) ((NumericUpDown) sender).Value;
                    MAVLinkInterface.modifyParamForDisplay(false, ((Control) sender).Name, ref value);
                    changes[name] = value;
                }
                else if (sender.GetType() == typeof (ComboBox))
                {
                    value = ((ComboBox) sender).SelectedIndex;
                    changes[name] = value;
                }
                ((Control) sender).BackColor = Color.Green;
            }
            catch (Exception)
            {
                ((Control) sender).BackColor = Color.Red;
            }
        }

        private void BUT_writePIDS_Click(object sender, EventArgs e)
        {
            var temp = (Hashtable) changes.Clone();

            foreach (string value in temp.Keys)
            {
                try
                {
                    if ((float) changes[value] > (float) MainV2.comPort.MAV.param[value]*2.0f)
                        if (
                            CustomMessageBox.Show(value + " has more than doubled the last input. Are you sure?",
                                "Large Value", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;

                    MainV2.comPort.setParam(value, (float) changes[value]);

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

            ((Control) sender).Enabled = false;

            try
            {
                MainV2.comPort.getParamList();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ErrorReceivingParams + ex, Strings.ERROR);
            }


            ((Control) sender).Enabled = true;

            Activate();
        }

        private void BUT_refreshpart_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            ((Control) sender).Enabled = false;


            updateparam(this);

            ((Control) sender).Enabled = true;


            Activate();
        }

        private void updateparam(Control parentctl)
        {
            foreach (Control ctl in parentctl.Controls)
            {
                if (typeof (NumericUpDown) == ctl.GetType() || typeof (ComboBox) == ctl.GetType())
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

        public struct paramsettings // hk's
        {
            public string desc;
            public float maxvalue;
            public float minvalue;
            public string name;
            public float normalvalue;
            public float scale;
        }
    }
}