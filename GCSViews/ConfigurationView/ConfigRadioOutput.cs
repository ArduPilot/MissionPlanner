using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigRadioOutput : MyUserControl, IActivate, IDeactivate
    {
        public ConfigRadioOutput()
        {
            InitializeComponent();

            bindingSource1.DataSource = typeof(CurrentState);

            var num_servos = 16;

            // See if 32 servo support is enabled
            if (MainV2.comPort.MAV.param.ContainsKey("SERVO_32_ENABLE") &&
                    (MainV2.comPort.MAV.param["SERVO_32_ENABLE"].Value > 0))
            {
                num_servos = 32;
            }

            SuspendLayout();
            foreach (var i in Enumerable.Range(1, num_servos))
            {
                setup(i);
            }

            ResumeLayout(true);
        }

        private void setup(int servono)
        {
            var servo = String.Format("SERVO{0}", servono);
            var initializing = true;

            var label = new Label()
                {Text = servono.ToString(), AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleCenter};
            var bAR1 = new HorizontalProgressBar2()
            {
                Minimum = 800, Maximum = 2200, Value = 1500, DrawLabel = true, Name = "BAR" + servono,
                Dock = DockStyle.Fill
            };
            var rev1 = new MissionPlanner.Controls.MavlinkCheckBox()
                {Enabled = false, Dock = DockStyle.Fill, AutoSize = true};
            var func1 = new MavlinkComboBox()
            { Enabled = false, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
            var min1 = new MavlinkNumericUpDown() { Minimum = 800, Maximum = 2200, Value = 1500, Enabled = false, Width = 50 };
            var trim1 = new MavlinkNumericUpDown() { Minimum = 800, Maximum = 2200, Value = 1500, Enabled = false, Width = 50 };
            var max1 = new MavlinkNumericUpDown() { Minimum = 800, Maximum = 2200, Value = 1500, Enabled = false, Width = 50 };
            var minMaxFromTrim1 = new NumericUpDown() { Minimum = 0, Maximum = 700, Value = 0, Width = 50, Visible = false, Increment = 10 };
            var equidistantCheck = new CheckBox() { Text = "", AutoSize = true };
            var equidistantPanel = new FlowLayoutPanel()
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Dock = DockStyle.Fill
            };
            equidistantPanel.Controls.Add(equidistantCheck);
            equidistantPanel.Controls.Add(minMaxFromTrim1);

            this.tableLayoutPanel1.Controls.Add(label, 0, servono);
            this.tableLayoutPanel1.Controls.Add(bAR1, 1, servono);
            this.tableLayoutPanel1.Controls.Add(rev1, 2, servono);
            this.tableLayoutPanel1.Controls.Add(func1, 3, servono);
            this.tableLayoutPanel1.Controls.Add(min1, 4, servono);
            this.tableLayoutPanel1.Controls.Add(trim1, 5, servono);
            this.tableLayoutPanel1.Controls.Add(max1, 6, servono);
            this.tableLayoutPanel1.Controls.Add(equidistantPanel, 7, servono);

            bAR1.DataBindings.Add("Value", bindingSource1, "ch" + servono + "out");
            rev1.setup(1, 0, servo + "_REVERSED", MainV2.comPort.MAV.param);
            func1.setup(ParameterMetaDataRepository.GetParameterOptionsInt(servo + "_FUNCTION",
                    MainV2.comPort.MAV.cs.firmware.ToString()), servo + "_FUNCTION", MainV2.comPort.MAV.param);
            min1.setup(800, 2200, 1, 1, servo + "_MIN", MainV2.comPort.MAV.param);
            trim1.setup(800, 2200, 1, 1, servo + "_TRIM", MainV2.comPort.MAV.param);
            max1.setup(800, 2200, 1, 1, servo + "_MAX", MainV2.comPort.MAV.param);

            equidistantCheck.CheckedChanged += (sender, e) =>
            {
                if (initializing)
                    return;

                var checkedState = equidistantCheck.Checked;
                min1.Enabled = !checkedState;
                max1.Enabled = !checkedState;
                minMaxFromTrim1.Visible = checkedState;

                if (checkedState)
                {
                    ApplyEquidistantRange(servo, trim1, min1, max1, minMaxFromTrim1);
                }
            };

            // Add event handler for min/max from trim control
            minMaxFromTrim1.ValueChanged += (sender, e) =>
            {
                if (initializing)
                    return;

                if (!equidistantCheck.Checked)
                    return;

                ApplyEquidistantRange(servo, trim1, min1, max1, minMaxFromTrim1);
            };

            if (IsEquidistant(min1.Value, trim1.Value, max1.Value, out var offset))
            {
                minMaxFromTrim1.Value = Math.Min(minMaxFromTrim1.Maximum, Math.Max(minMaxFromTrim1.Minimum, offset));
                equidistantCheck.Checked = true;
            }

            initializing = false;

            if (equidistantCheck.Checked)
            {
                min1.Enabled = false;
                max1.Enabled = false;
                minMaxFromTrim1.Visible = true;
            }
        }

        private static void ApplyEquidistantRange(string servo, NumericUpDown trim, NumericUpDown min, NumericUpDown max,
            NumericUpDown offset)
        {
            var trimValue = trim.Value;
            var newMin = ClampServoValue(trimValue - offset.Value);
            var newMax = ClampServoValue(trimValue + offset.Value);

            min.Value = newMin;
            max.Value = newMax;

            // Explicitly write the new limits so the change is not UI-only
            WriteServoRange(servo, newMin, newMax);
        }

        private static decimal ClampServoValue(decimal value)
        {
            if (value < 800) return 800;
            if (value > 2200) return 2200;
            return value;
        }

        private static bool IsEquidistant(decimal min, decimal trim, decimal max, out decimal offset)
        {
            offset = 0;
            if (trim < min || trim > max)
                return false;

            var diffLow = trim - min;
            var diffHigh = max - trim;

            if (diffLow == diffHigh)
            {
                offset = diffLow;
                return true;
            }

            return false;
        }

        private static void WriteServoRange(string servo, decimal newMin, decimal newMax)
        {
            if (MainV2.comPort?.BaseStream == null || !MainV2.comPort.BaseStream.IsOpen)
                return;

            var minParam = servo + "_MIN";
            var maxParam = servo + "_MAX";

            if (MainV2.comPort.MAV?.param == null)
                return;

            try
            {
                if (MainV2.comPort.MAV.param.ContainsKey(minParam))
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                        minParam, (double)newMin);

                if (MainV2.comPort.MAV.param.ContainsKey(maxParam))
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                        maxParam, (double)newMax);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set {servo} min/max from trim: {ex}");
            }
        }

        public void Activate()
        {
            timer1.Start();
        }

        public void Deactivate()
        {
            timer1.Stop();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSource1.UpdateDataSource(MainV2.comPort.MAV.cs));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
