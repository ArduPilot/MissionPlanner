using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigAteryx : UserControl, IActivate
    {
        // from http://stackoverflow.com/questions/2512781/winforms-big-paragraph-tooltip/2512895#2512895
        private const int maximumSingleLineTooltipLength = 50;

        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Hashtable tooltips = new Hashtable();
        private readonly Hashtable changes = new Hashtable();
        internal bool startup = true;

        public ConfigAteryx()
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
            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx)
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

            processToScreen();

            startup = false;
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

        internal void processToScreen()
        {
            toolTip1.RemoveAll();

            disableNumericUpDownControls(this);

            // process hashdefines and update display
            foreach (string value in MainV2.comPort.MAV.param.Keys)
            {
                if (value == null || value == "")
                    continue;

                var name = value;
                var text = Controls.Find(name, true);
                foreach (var ctl in text)
                {
                    try
                    {
                        if (ctl.GetType() == typeof (NumericUpDown))
                        {
                            var thisctl = ((NumericUpDown) ctl);
                            thisctl.Maximum = 9000;
                            thisctl.Minimum = -9000;
                            thisctl.Value = (decimal) (float) MainV2.comPort.MAV.param[value];
                            thisctl.Increment = (decimal) 0.001;
                            if (thisctl.Name.EndsWith("_P") || thisctl.Name.EndsWith("_I") ||
                                thisctl.Name.EndsWith("_D")
                                || thisctl.Name.EndsWith("_LOW") || thisctl.Name.EndsWith("_HIGH") || thisctl.Value == 0
                                || thisctl.Value.ToString("0.###", new CultureInfo("en-US")).Contains("."))
                            {
                                thisctl.DecimalPlaces = 3;
                            }
                            else
                            {
                                thisctl.Increment = 1;
                                thisctl.DecimalPlaces = 1;
                            }

                            if (thisctl.Name.EndsWith("_IMAX"))
                            {
                                thisctl.Maximum = 180;
                                thisctl.Minimum = -180;
                            }

                            thisctl.Enabled = true;

                            thisctl.BackColor = Color.FromArgb(0x43, 0x44, 0x45);
                            thisctl.Validated += null;
                            if (tooltips[value] != null)
                            {
                                try
                                {
                                    toolTip1.SetToolTip(ctl, ((paramsettings) tooltips[value]).desc);
                                }
                                catch
                                {
                                }
                            }
                            thisctl.Validated += EEPROM_View_float_TextChanged;
                        }
                        else if (ctl.GetType() == typeof (ComboBox))
                        {
                            var thisctl = ((ComboBox) ctl);

                            thisctl.SelectedIndex = (int) (float) MainV2.comPort.MAV.param[value];

                            thisctl.Validated += ComboBox_Validated;
                        }
                    }
                    catch
                    {
                    }
                }
                if (text.Length == 0)
                {
                    //Console.WriteLine(name + " not found");
                }
            }
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
                    value = float.Parse(((Control) sender).Text);
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

        private void groupBox7_Enter(object sender, EventArgs e)
        {
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
        }

        private void numericUpDown15_ValueChanged(object sender, EventArgs e)
        {
        }

        private void BUT_write_flash_Click(object sender, EventArgs e)
        {
            try
            {
                ((Button) sender).Enabled = false;

                if ((MainV2.comPort.MAV.cs.airspeed > 7.0) || (MainV2.comPort.MAV.cs.groundspeed > 10.0))
                {
                    MessageBox.Show("Unable - UAV airborne");
                    ((Button) sender).Enabled = true;
                    return;
                }

                //MainV2.comPort.doCommand((MAVLink.MAV_CMD)Enum.Parse(typeof(MAVLink.MAV_CMD), "MAV_CMD_PREFLIGHT_STORAGE"));
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_STORAGE, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
            }
            catch
            {
                MessageBox.Show("The Command failed to execute");
            }
            ((Button) sender).Enabled = true;
        }

        private void BUT_read_flash_Click(object sender, EventArgs e)
        {
            try
            {
                var dr = MessageBox.Show("Reset Flash to Factory Defaults?", "Continue", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    if ((MainV2.comPort.MAV.cs.airspeed > 7.0) || (MainV2.comPort.MAV.cs.groundspeed > 7.0))
                    {
                        MessageBox.Show("Unable - UAV airborne");
                        ((Button) sender).Enabled = true;
                        return;
                    }
                    //MainV2.comPort.doCommand((MAVLink.MAV_CMD)Enum.Parse(typeof(MAVLink.MAV_CMD), "MAV_CMD_PREFLIGHT_STORAGE"));
                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_STORAGE, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
                }
            }
            catch
            {
                MessageBox.Show("The Command failed to execute");
            }
            ((Button) sender).Enabled = true;
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