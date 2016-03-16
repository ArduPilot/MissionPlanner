using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigArducopter : UserControl, IActivate
    {
        // from http://stackoverflow.com/questions/2512781/winforms-big-paragraph-tooltip/2512895#2512895
        private const int maximumSingleLineTooltipLength = 50;
        private static Hashtable tooltips = new Hashtable();
        private readonly Hashtable changes = new Hashtable();
        internal bool startup = true;

        public ConfigArducopter()
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
            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
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

            // ensure the fields are populated before setting them
            TUNE.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("TUNE", MainV2.comPort.MAV.cs.firmware.ToString())
                    .ToList(), "TUNE", MainV2.comPort.MAV.param);
            CH7_OPT.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("CH7_OPT", MainV2.comPort.MAV.cs.firmware.ToString())
                    .ToList(), "CH7_OPT", MainV2.comPort.MAV.param);
            CH8_OPT.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("CH8_OPT", MainV2.comPort.MAV.cs.firmware.ToString())
                    .ToList(), "CH8_OPT", MainV2.comPort.MAV.param);

            TUNE_LOW.setup(0, 10000, 1000, 0.01f, "TUNE_LOW", MainV2.comPort.MAV.param);
            TUNE_HIGH.setup(0, 10000, 1000, 0.01f, "TUNE_HIGH", MainV2.comPort.MAV.param);

            HLD_LAT_P.setup(0, 0, 1, 0.001f, new[] {"HLD_LAT_P", "POS_XY_P"}, MainV2.comPort.MAV.param);
            LOITER_LAT_D.setup(0, 0, 1, 0.001f, "LOITER_LAT_D", MainV2.comPort.MAV.param);
            LOITER_LAT_I.setup(0, 0, 1, 0.001f, new[] {"LOITER_LAT_I", "VEL_XY_I"}, MainV2.comPort.MAV.param);
            LOITER_LAT_IMAX.setup(0, 0, 10, 1f, new[] {"LOITER_LAT_IMAX", "VEL_XY_IMAX"}, MainV2.comPort.MAV.param);
            LOITER_LAT_P.setup(0, 0, 1, 0.001f, new[] {"LOITER_LAT_P", "VEL_XY_P"}, MainV2.comPort.MAV.param);
            RATE_PIT_FF.setup(0, 0, 1, 0.001f, "RATE_PIT_VFF", MainV2.comPort.MAV.param);
            RATE_PIT_D.setup(0, 0, 1, 0.001f, "RATE_PIT_D", MainV2.comPort.MAV.param);
            RATE_PIT_I.setup(0, 0, 1, 0.001f, "RATE_PIT_I", MainV2.comPort.MAV.param);
            RATE_PIT_IMAX.setup(0, 0, 10, 1f, "RATE_PIT_IMAX", MainV2.comPort.MAV.param);
            RATE_PIT_P.setup(0, 0, 1, 0.001f, "RATE_PIT_P", MainV2.comPort.MAV.param);
            RATE_RLL_D.setup(0, 0, 1, 0.001f, "RATE_RLL_D", MainV2.comPort.MAV.param);
            RATE_RLL_I.setup(0, 0, 1, 0.001f, "RATE_RLL_I", MainV2.comPort.MAV.param);
            RATE_RLL_IMAX.setup(0, 0, 10, 1f, "RATE_RLL_IMAX", MainV2.comPort.MAV.param);
            RATE_RLL_P.setup(0, 0, 1, 0.001f, "RATE_RLL_P", MainV2.comPort.MAV.param);
            RATE_RLL_FF.setup(0, 0, 1, 0.001f, "RATE_RLL_VFF", MainV2.comPort.MAV.param);
            RATE_YAW_D.setup(0, 0, 1, 0.001f, "RATE_YAW_D", MainV2.comPort.MAV.param);
            RATE_YAW_FF.setup(0, 0, 1, 0.001f, "RATE_YAW_VFF", MainV2.comPort.MAV.param);
            RATE_YAW_I.setup(0, 0, 1, 0.001f, "RATE_YAW_I", MainV2.comPort.MAV.param);
            RATE_YAW_IMAX.setup(0, 0, 10, 1f, "RATE_YAW_IMAX", MainV2.comPort.MAV.param);
            RATE_YAW_P.setup(0, 0, 1, 0.001f, "RATE_YAW_P", MainV2.comPort.MAV.param);
            STB_PIT_P.setup(0, 0, 1, 0.001f, "STB_PIT_P", MainV2.comPort.MAV.param);
            STB_RLL_P.setup(0, 0, 1, 0.001f, "STB_RLL_P", MainV2.comPort.MAV.param);
            STB_YAW_P.setup(0, 0, 1, 0.001f, "STB_YAW_P", MainV2.comPort.MAV.param);
            THR_ACCEL_D.setup(0, 0, 1, 0.001f, new[] {"THR_ACCEL_D", "ACCEL_Z_D"}, MainV2.comPort.MAV.param);
            THR_ACCEL_I.setup(0, 0, 1, 0.001f, new[] {"THR_ACCEL_I", "ACCEL_Z_I"}, MainV2.comPort.MAV.param);
            THR_ACCEL_IMAX.setup(0, 0, 10, 1f, new[] {"THR_ACCEL_IMAX", "ACCEL_Z_IMAX"}, MainV2.comPort.MAV.param);
            THR_ACCEL_P.setup(0, 0, 1, 0.001f, new[] {"THR_ACCEL_P", "ACCEL_Z_P"}, MainV2.comPort.MAV.param);
            THR_ALT_P.setup(0, 0, 1, 0.001f, new[] {"THR_ALT_P", "POS_Z_P"}, MainV2.comPort.MAV.param);
            THR_RATE_P.setup(0, 0, 1, 0.001f, new[] {"THR_RATE_P", "VEL_Z_P"}, MainV2.comPort.MAV.param);
            WPNAV_LOIT_SPEED.setup(0, 0, 1, 0.001f, "WPNAV_LOIT_SPEED", MainV2.comPort.MAV.param);
            WPNAV_RADIUS.setup(0, 0, 1, 0.001f, "WPNAV_RADIUS", MainV2.comPort.MAV.param);
            WPNAV_SPEED.setup(0, 0, 1, 0.001f, "WPNAV_SPEED", MainV2.comPort.MAV.param);
            WPNAV_SPEED_DN.setup(0, 0, 1, 0.001f, "WPNAV_SPEED_DN", MainV2.comPort.MAV.param);
            WPNAV_SPEED_UP.setup(0, 0, 1, 0.001f, "WPNAV_SPEED_UP", MainV2.comPort.MAV.param);

            // unlock entries if they differ
            if (RATE_RLL_P.Value != RATE_PIT_P.Value || RATE_RLL_I.Value != RATE_PIT_I.Value
                || RATE_RLL_D.Value != RATE_PIT_D.Value || RATE_RLL_IMAX.Value != RATE_PIT_IMAX.Value)
            {
                CHK_lockrollpitch.Checked = false;
            }

            if (MainV2.comPort.MAV.param["H_SWASH_TYPE"] != null)
            {
                CHK_lockrollpitch.Checked = false;
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

        private void disableNumericUpDownControls(Control inctl)
        {
            foreach (Control ctl in inctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                {
                    disableNumericUpDownControls(ctl);
                }
                if (ctl.GetType() == typeof (NumericUpDown))
                {
                    ctl.Enabled = false;
                }
            }
        }

        internal void EEPROM_View_float_TextChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            float value = 0;
            var name = ((Control) sender).Name;

            // do domainupdown state check
            try
            {
                if (sender.GetType() == typeof (MavlinkNumericUpDown))
                {
                    value = ((MAVLinkParamChanged) e).value;
                    changes[name] = value;
                }
                else if (sender.GetType() == typeof (MavlinkComboBox))
                {
                    value = ((MAVLinkParamChanged) e).value;
                    changes[name] = value;
                }
                ((Control) sender).BackColor = Color.Green;
            }
            catch (Exception)
            {
                ((Control) sender).BackColor = Color.Red;
            }

            try
            {
                // enable roll and pitch pairing for ac2
                if (CHK_lockrollpitch.Checked)
                {
                    if (name.StartsWith("RATE_") || name.StartsWith("STB_") || name.StartsWith("ACRO_"))
                    {
                        if (name.Contains("_RLL_"))
                        {
                            var newname = name.Replace("_RLL_", "_PIT_");
                            var arr = Controls.Find(newname, true);
                            changes[newname] = value;

                            if (arr.Length > 0)
                            {
                                arr[0].Text = ((Control) sender).Text;
                                arr[0].BackColor = Color.Green;
                            }
                        }
                        else if (name.Contains("_PIT_"))
                        {
                            var newname = name.Replace("_PIT_", "_RLL_");
                            var arr = Controls.Find(newname, true);
                            changes[newname] = value;

                            if (arr.Length > 0)
                            {
                                arr[0].Text = ((Control) sender).Text;
                                arr[0].BackColor = Color.Green;
                            }
                        }
                    }
                }
                // keep nav_lat and nav_lon paired
                if (name.Contains("NAV_LAT_"))
                {
                    var newname = name.Replace("NAV_LAT_", "NAV_LON_");
                    var arr = Controls.Find(newname, true);
                    changes[newname] = value;

                    if (arr.Length > 0)
                    {
                        arr[0].Text = ((Control) sender).Text;
                        arr[0].BackColor = Color.Green;
                    }
                }
                // keep loiter_lat and loiter_lon paired
                if (name.Contains("LOITER_LAT_"))
                {
                    var newname = name.Replace("LOITER_LAT_", "LOITER_LON_");
                    var arr = Controls.Find(newname, true);
                    changes[newname] = value;

                    if (arr.Length > 0)
                    {
                        arr[0].Text = ((Control) sender).Text;
                        arr[0].BackColor = Color.Green;
                    }
                }
            }
            catch
            {
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

                    changes.Remove(value);

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
                if (typeof (MavlinkNumericUpDown) == ctl.GetType() || typeof (ComboBox) == ctl.GetType())
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

        private void numeric_ValueUpdated(object sender, EventArgs e)
        {
            EEPROM_View_float_TextChanged(sender, e);
        }
    }
}