using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using System.Collections;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigAntennaTracker : UserControl, IActivate
    {
        Hashtable changes = new Hashtable();
        static Hashtable tooltips = new Hashtable();
        internal bool startup = true;


        public ConfigAntennaTracker()
        {
            InitializeComponent();
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

        public void Activate()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                this.Enabled = false;
                return;
            }
            else
            {
                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduTracker)
                {
                    this.Enabled = true;
                }
                else
                {
                    this.Enabled = false;
                    return;
                }
            }

            startup = true;

            changes.Clear();

            mavlinkComboBox1.DataSource = ParameterMetaDataRepository.GetParameterOptionsInt("AHRS_ORIENTATION").ToList();
            mavlinkComboBox1.DisplayMember = "Value";
            mavlinkComboBox1.ValueMember = "Key";

            mavlinkComboBox2.DataSource = ParameterMetaDataRepository.GetParameterOptionsInt("SERVO_TYPE").ToList();
            mavlinkComboBox2.DisplayMember = "Value";
            mavlinkComboBox2.ValueMember = "Key";

            mavlinkComboBox3.DataSource = ParameterMetaDataRepository.GetParameterOptionsInt("PROXY_MODE").ToList();
            mavlinkComboBox3.DisplayMember = "Value";
            mavlinkComboBox3.ValueMember = "Key";

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
            mavlinkNumericUpDown8.setup(0, 180, 1, 1, "PITCH_RANGE", MainV2.comPort.MAV.param);

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

            startup = false;
        }

        // from http://stackoverflow.com/questions/2512781/winforms-big-paragraph-tooltip/2512895#2512895
        private const int maximumSingleLineTooltipLength = 50;

        private static string AddNewLinesForTooltip(string text)
        {
            if (text.Length < maximumSingleLineTooltipLength)
                return text;
            int lineLength = (int)Math.Sqrt((double)text.Length) * 2;
            StringBuilder sb = new StringBuilder();
            int currentLinePosition = 0;
            for (int textIndex = 0; textIndex < text.Length; textIndex++)
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


               void ComboBox_Validated(object sender, EventArgs e)
        {
            EEPROM_View_float_TextChanged(sender, e);
        }

        void Configuration_Validating(object sender, CancelEventArgs e)
        {
            EEPROM_View_float_TextChanged(sender, e);
        }

        internal void EEPROM_View_float_TextChanged(object sender, EventArgs e)
        {
            if (startup == true)
                return;

            float value = 0;
            string name = ((Control)sender).Name;

            // do domainupdown state check
            try
            {
                if (sender.GetType() == typeof(NumericUpDown))
                {
                    value = (float)((NumericUpDown)sender).Value;
                    MAVLinkInterface.modifyParamForDisplay(false, ((Control)sender).Name, ref value);
                    changes[name] = value;
                }
                else if (sender.GetType() == typeof(ComboBox))
                {
                    value = (int)((ComboBox)sender).SelectedValue;
                    changes[name] = value;
                }
                ((Control)sender).BackColor = Color.Green;
            }
            catch (Exception)
            {
                ((Control)sender).BackColor = Color.Red;
            }
        }

        private void BUT_writePIDS_Click(object sender, EventArgs e)
        {
            var temp = (Hashtable)changes.Clone();

            foreach (string value in temp.Keys)
            {
                try
                {
                    if ((float)changes[value] > (float)MainV2.comPort.MAV.param[value] * 2.0f)
                        if (CustomMessageBox.Show(value +" has more than doubled the last input. Are you sure?", "Large Value", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;

                    MainV2.comPort.setParam(value, (float)changes[value]);

                    try
                    {
                        // set control as well
                        var textControls = this.Controls.Find(value, true);
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
                    CustomMessageBox.Show("Set " + value + " Failed", "Error");
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the BUT_rerequestparams control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
                CustomMessageBox.Show("Error: getting param list " + ex.ToString(), "Error");
            }


            ((Control)sender).Enabled = true;


            this.Activate();
        }

        private void BUT_refreshpart_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return;

            ((Control)sender).Enabled = false;


            updateparam(this);

            ((Control)sender).Enabled = true;


            this.Activate();
        }

        void updateparam(Control parentctl)
        {
            foreach (Control ctl in parentctl.Controls)
            {
                if (typeof(NumericUpDown) == ctl.GetType() || typeof(ComboBox) == ctl.GetType())
                {
                    try
                    {
                        MainV2.comPort.GetParam(ctl.Name);
                    }
                    catch { }
                }

                if (ctl.Controls.Count > 0)
                {
                    updateparam(ctl);
                }
            }
        }

        private void BUT_test_yaw_Click(object sender, EventArgs e)
        {
            MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, 1, myTrackBar1.Value,0,0,0,0,0);
        }

        private void BUT_test_pitch_Click(object sender, EventArgs e)
        {
            MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, 1, myTrackBar2.Value, 0, 0, 0, 0, 0);
        }
        
    }
}
