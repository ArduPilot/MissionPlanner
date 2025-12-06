using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigMotorTest : MyUserControl, IActivate
    {
        // Motor function values in ArduPilot (33 = Motor1, 34 = Motor2, etc.)
        private const int MOTOR_FUNCTION_BASE = 33;
        private const int MAX_SERVO_CHANNELS = 16;

        // Frame type SVG URLs from ArduPilot documentation
        private static readonly Dictionary<(int frameClass, int frameType), string> FrameSvgUrls = new Dictionary<(int, int), string>
        {
            // QUAD (Class 1)
            {(1, 0), "http://ardupilot.org/copter/_images/m_01_00_quad_plus.svg"},
            {(1, 1), "http://ardupilot.org/copter/_images/m_01_01_quad_x.svg"},
            {(1, 2), "http://ardupilot.org/copter/_images/m_01_02_quad_v.svg"},
            {(1, 3), "http://ardupilot.org/copter/_images/m_01_03_quad_h.svg"},
            {(1, 4), "http://ardupilot.org/copter/_images/m_01_04_quad_v_tail.svg"},
            {(1, 5), "http://ardupilot.org/copter/_images/m_01_05_quad_a_tail.svg"},
            {(1, 6), "http://ardupilot.org/copter/_images/m_01_06_quad_plus_rev.svg"},
            {(1, 12), "http://ardupilot.org/copter/_images/m_01_12_quad_x_bf.svg"},
            {(1, 13), "http://ardupilot.org/copter/_images/m_01_13_quad_x_dji.svg"},
            {(1, 14), "http://ardupilot.org/copter/_images/m_01_14_quad_x_cw.svg"},
            {(1, 16), "http://ardupilot.org/copter/_images/m_01_16_quad_plus_nyt.svg"},
            {(1, 17), "http://ardupilot.org/copter/_images/m_01_17_quad_x_nyt.svg"},
            {(1, 18), "http://ardupilot.org/copter/_images/m_01_18_quad_x_bf_rev.svg"},
            {(1, 19), "http://ardupilot.org/copter/_images/m_01_19_quad_y4a.svg"},
            // HEXA (Class 2)
            {(2, 0), "http://ardupilot.org/copter/_images/m_02_00_hexa_plus.svg"},
            {(2, 1), "http://ardupilot.org/copter/_images/m_02_01_hexa_x.svg"},
            {(2, 3), "http://ardupilot.org/copter/_images/m_02_03_hexa_h.svg"},
            {(2, 13), "http://ardupilot.org/copter/_images/m_02_13_hexa_x_dji.svg"},
            {(2, 14), "http://ardupilot.org/copter/_images/m_02_14_hexa_x_cw.svg"},
            // OCTA (Class 3)
            {(3, 0), "http://ardupilot.org/copter/_images/m_03_00_octo_plus.svg"},
            {(3, 1), "http://ardupilot.org/copter/_images/m_03_01_octo_x.svg"},
            {(3, 2), "http://ardupilot.org/copter/_images/m_03_02_octo_v.svg"},
            {(3, 3), "http://ardupilot.org/copter/_images/m_03_03_octo_h.svg"},
            {(3, 13), "http://ardupilot.org/copter/_images/m_03_13_octo_x_dji.svg"},
            {(3, 14), "http://ardupilot.org/copter/_images/m_03_14_octo_x_cw.svg"},
            {(3, 15), "http://ardupilot.org/copter/_images/m_03_15_octo_i.svg"},
            // OCTA QUAD (Class 4)
            {(4, 0), "http://ardupilot.org/copter/_images/m_04_00_octo_quad_plus.svg"},
            {(4, 1), "http://ardupilot.org/copter/_images/m_04_01_octo_quad_x.svg"},
            {(4, 2), "http://ardupilot.org/copter/_images/m_04_02_octo_quad_v.svg"},
            {(4, 3), "http://ardupilot.org/copter/_images/m_04_03_octo_quad_h.svg"},
            {(4, 12), "http://ardupilot.org/copter/_images/m_04_12_octo_quad_x_bf.svg"},
            {(4, 14), "http://ardupilot.org/copter/_images/m_04_14_octo_quad_x_cw.svg"},
            {(4, 18), "http://ardupilot.org/copter/_images/m_04_18_octo_quad_x_bf_rev.svg"},
            // Y6 (Class 5)
            {(5, 0), "http://ardupilot.org/copter/_images/m_05_00_y6_a.svg"},
            {(5, 10), "http://ardupilot.org/copter/_images/m_05_10_y6_b.svg"},
            {(5, 11), "http://ardupilot.org/copter/_images/m_05_11_y6_f.svg"},
            // TRI (Class 7)
            {(7, 0), "http://ardupilot.org/copter/_images/m_07_00_tricopter.svg"},
            {(7, 6), "http://ardupilot.org/copter/_images/m_07_06_tricopter_pitch_rev.svg"},
            // BICOPTER (Class 10)
            {(10, 0), "http://ardupilot.org/copter/_images/m_10_00_bicopter.svg"},
            // DODECAHEXA (Class 12)
            {(12, 0), "http://ardupilot.org/copter/_images/m_12_00_dodecahexa_plus.svg"},
            {(12, 1), "http://ardupilot.org/copter/_images/m_12_01_dodecahexa_x.svg"},
            // DECA (Class 14)
            {(14, 0), "http://ardupilot.org/copter/_images/m_14_00_deca_plus.svg"},
            {(14, 1), "http://ardupilot.org/copter/_images/m_14_01_deca_x_and_cw_x.svg"}
        };

        public ConfigMotorTest()
        {
            InitializeComponent();
        }

        private int motormax = 0;
        private List<MotorOutputPanel> motorPanels = new List<MotorOutputPanel>();

        private struct _motors
        {
            public int Number { get; set; }
            public int TestOrder { get; set; }
            public string Rotation { get; set; }
            public float Roll { get; set; }
            public float Pitch { get; set; }
        }
        struct _layouts
         {
            public int Class { get; set; }
            public int Type { get; set; }
            public _motors[] motors { get; set; }
        }
        private struct JSON_motors
        {
            public string Version { get; set; }
            public _layouts[] layouts { get; set; }
        }
        private _layouts motor_layout;

        private class MotorOutputPanel
        {
            public GroupBox GroupBox { get; set; }
            public Label ServoLabel { get; set; }
            public ComboBox MotorSelector { get; set; }
            public MyButton SpinButton { get; set; }
            public int MotorIndex { get; set; }  // 1-based motor index (A=1, B=2, etc.)
            public int ServoChannel { get; set; }  // 1-based servo channel
        }

        public async void Activate()
        {
            // Initialize WebView2
            try
            {
                await webViewFrame.EnsureCoreWebView2Async(null);
                webViewFrame.DefaultBackgroundColor = System.Drawing.Color.Transparent;
                webViewFrame.Visible = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WebView2 initialization error: " + ex.Message);
            }

            motormax = this.get_motormax();

            // Clear previous dynamic controls
            ClearDynamicControls();

            // Create motor output panels
            CreateMotorOutputPanels();

            // Load MOT_SPIN_ARM and MOT_SPIN_MIN parameters
            NUM_mot_spin_arm.ValueChanged -= NUM_mot_spin_arm_ValueChanged;
            NUM_mot_spin_min.ValueChanged -= NUM_mot_spin_min_ValueChanged;

            if (MainV2.comPort.MAV.param.ContainsKey("MOT_SPIN_ARM"))
            {
                NUM_mot_spin_arm.Value = (decimal)MainV2.comPort.MAV.param["MOT_SPIN_ARM"].Value;
            }
            if (MainV2.comPort.MAV.param.ContainsKey("MOT_SPIN_MIN"))
            {
                NUM_mot_spin_min.Value = (decimal)MainV2.comPort.MAV.param["MOT_SPIN_MIN"].Value;
            }

            NUM_mot_spin_arm.ValueChanged += NUM_mot_spin_arm_ValueChanged;
            NUM_mot_spin_min.ValueChanged += NUM_mot_spin_min_ValueChanged;

            Utilities.ThemeManager.ApplyThemeTo(this);
        }

        private void ClearDynamicControls()
        {
            foreach (var panel in motorPanels)
            {
                if (panel.GroupBox != null && tableLayoutPanelMotors.Controls.Contains(panel.GroupBox))
                {
                    tableLayoutPanelMotors.Controls.Remove(panel.GroupBox);
                    panel.GroupBox.Dispose();
                }
            }
            motorPanels.Clear();
        }

        private void CreateMotorOutputPanels()
        {
            // Suspend layout to prevent flickering during dynamic control creation
            tableLayoutPanelMotors.SuspendLayout();
            panel1.SuspendLayout();
            this.SuspendLayout();

            try
            {
                // Find which servo outputs are assigned to motors
                var motorAssignments = GetMotorAssignments();

                // Clear and reconfigure TableLayoutPanel
                tableLayoutPanelMotors.Controls.Clear();
                tableLayoutPanelMotors.RowStyles.Clear();
                tableLayoutPanelMotors.ColumnStyles.Clear();

                // Configure columns: 3 equal columns for the button row
                tableLayoutPanelMotors.ColumnCount = 3;
                tableLayoutPanelMotors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
                tableLayoutPanelMotors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
                tableLayoutPanelMotors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

                // Rows: one for each motor GroupBox (spanning all 3 columns) + one row for buttons
                tableLayoutPanelMotors.RowCount = motormax + 1;
                for (int i = 0; i < motormax; i++)
                {
                    tableLayoutPanelMotors.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
                }
                tableLayoutPanelMotors.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F)); // Button row

                // Add motor GroupBoxes (each spans all 3 columns)
                for (int motorIdx = 1; motorIdx <= motormax; motorIdx++)
                {
                    var motorPanel = CreateMotorPanel(motorIdx, motorAssignments);
                    motorPanels.Add(motorPanel);
                    motorPanel.GroupBox.Dock = DockStyle.Fill;
                    tableLayoutPanelMotors.Controls.Add(motorPanel.GroupBox, 0, motorIdx - 1);
                    tableLayoutPanelMotors.SetColumnSpan(motorPanel.GroupBox, 3);
                }

                // Add buttons to the last row
                btnTestAll.Dock = DockStyle.Fill;
                btnStopAll.Dock = DockStyle.Fill;
                btnTestSequence.Dock = DockStyle.Fill;
                tableLayoutPanelMotors.Controls.Add(btnTestAll, 0, motormax);
                tableLayoutPanelMotors.Controls.Add(btnStopAll, 1, motormax);
                tableLayoutPanelMotors.Controls.Add(btnTestSequence, 2, motormax);

                // Position WebView to the right of the table with 32px spacing
                PositionWebView();
                tableLayoutPanelMotors.SizeChanged -= TableLayoutPanelMotors_SizeChanged;
                tableLayoutPanelMotors.SizeChanged += TableLayoutPanelMotors_SizeChanged;
            }
            finally
            {
                // Resume layout
                this.ResumeLayout(false);
                panel1.ResumeLayout(false);
                tableLayoutPanelMotors.ResumeLayout(true);

                // Ensure bottom row (buttons) is exactly 50px after layout
                if (tableLayoutPanelMotors.RowStyles.Count > motormax)
                {
                    tableLayoutPanelMotors.RowStyles[motormax] = new RowStyle(SizeType.Absolute, 50F);
                }
            }
        }

        private void TableLayoutPanelMotors_SizeChanged(object sender, EventArgs e)
        {
            PositionWebView();
        }

        private void PositionWebView()
        {
            // Position WebView 32px to the right of the table, aligned to table top
            webViewFrame.Location = new Point(tableLayoutPanelMotors.Right + 12, tableLayoutPanelMotors.Top);
        }

        private Dictionary<int, int> GetMotorAssignments()
        {
            // Returns a dictionary mapping motor test order (1-12) to servo channel (1-16)
            // Uses the motor_layout to map test order -> motor number -> servo channel
            var assignments = new Dictionary<int, int>();

            // First, build a map of motor number -> servo channel
            var motorToServo = new Dictionary<int, int>();
            for (int servo = 1; servo <= MAX_SERVO_CHANNELS; servo++)
            {
                string paramName = $"SERVO{servo}_FUNCTION";
                if (MainV2.comPort.MAV.param.ContainsKey(paramName))
                {
                    int funcValue = (int)MainV2.comPort.MAV.param[paramName].Value;
                    // Motor1 = 33, Motor2 = 34, etc.
                    if (funcValue >= MOTOR_FUNCTION_BASE && funcValue < MOTOR_FUNCTION_BASE + 12)
                    {
                        int motorNum = funcValue - MOTOR_FUNCTION_BASE + 1;
                        motorToServo[motorNum] = servo;
                    }
                }
            }

            // Now map test order -> servo channel using motor_layout
            if (motor_layout.motors != null)
            {
                foreach (var motor in motor_layout.motors)
                {
                    if (motorToServo.ContainsKey(motor.Number))
                    {
                        assignments[motor.TestOrder] = motorToServo[motor.Number];
                    }
                }
            }
            else
            {
                // Fallback: assume test order = motor number
                assignments = motorToServo;
            }

            return assignments;
        }

        private MotorOutputPanel CreateMotorPanel(int motorIdx, Dictionary<int, int> motorAssignments)
        {
            char motorLetter = (char)('A' + motorIdx - 1);
            int servoChannel = motorAssignments.ContainsKey(motorIdx) ? motorAssignments[motorIdx] : 0;

            // Get motor number and rotation info from motor layout
            int motorNumber = motorIdx; // Default fallback
            string rotationInfo = "";
            if (motor_layout.motors != null)
            {
                foreach (var motor in motor_layout.motors)
                {
                    if (motor.TestOrder == motorIdx)
                    {
                        motorNumber = motor.Number;
                        if (motor.Rotation != "?")
                        {
                            rotationInfo = motor.Rotation;
                        }
                        break;
                    }
                }
            }

            string groupBoxTitle = string.IsNullOrEmpty(rotationInfo)
                ? $"Motor {motorLetter}"
                : $"Motor {motorLetter} ({rotationInfo})";

            var groupBox = new GroupBox
            {
                Text = groupBoxTitle,
                Height = 50,
                Margin = new Padding(3, 3, 3, 3)
            };

            // Create TableLayoutPanel inside GroupBox for proper scaling
            var innerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0),
                Padding = new Padding(3, 0, 3, 0)
            };
            innerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55F));  // Motor number label
            innerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // Servo selector (stretches)
            innerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));  // Smart Assign button
            innerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));  // Spin button
            innerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Motor number label
            var motorLabel = new Label
            {
                Text = $"Motor {motorNumber}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Servo output selector
            var servoSelector = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            // Prevent mouse wheel from changing selection
            servoSelector.MouseWheel += (s, ev) => ((HandledMouseEventArgs)ev).Handled = true;

            // Populate servo selector with Servo 1-16 options
            servoSelector.Items.Add(new ComboBoxItem { Text = "Not assigned", Value = 0 });
            for (int i = 1; i <= MAX_SERVO_CHANNELS; i++)
            {
                servoSelector.Items.Add(new ComboBoxItem { Text = $"Servo {i}", Value = i });
            }
            servoSelector.DisplayMember = "Text";

            // Select current servo channel
            SelectServoChannel(servoSelector, servoChannel);

            servoSelector.Tag = new ServoSelectorTag { MotorIndex = motorIdx, MotorNumber = motorNumber };
            servoSelector.SelectedIndexChanged += ServoSelector_SelectedIndexChanged;

            // Smart Assign button
            var smartAssignButton = new MyButton
            {
                Text = "?",
                Dock = DockStyle.Fill
            };
            smartAssignButton.Tag = new SmartAssignTag { MotorIndex = motorIdx, ServoSelector = servoSelector };
            smartAssignButton.Click += SmartAssignButton_Click;

            // Spin button
            var spinButton = new MyButton
            {
                Text = "Spin",
                Dock = DockStyle.Fill
            };
            spinButton.Tag = motorIdx;
            spinButton.Click += SpinButton_Click;

            innerLayout.Controls.Add(motorLabel, 0, 0);
            innerLayout.Controls.Add(servoSelector, 1, 0);
            innerLayout.Controls.Add(smartAssignButton, 2, 0);
            innerLayout.Controls.Add(spinButton, 3, 0);
            groupBox.Controls.Add(innerLayout);

            return new MotorOutputPanel
            {
                GroupBox = groupBox,
                ServoLabel = motorLabel,
                MotorSelector = servoSelector,
                SpinButton = spinButton,
                MotorIndex = motorIdx,
                ServoChannel = servoChannel
            };
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }
            public override string ToString() => Text;
        }

        private class MotorSelectorTag
        {
            public int MotorIndex { get; set; }
            public int ServoChannel { get; set; }
        }

        private class ServoSelectorTag
        {
            public int MotorIndex { get; set; }
            public int MotorNumber { get; set; }
        }

        private class SmartAssignTag
        {
            public int MotorIndex { get; set; }
            public ComboBox ServoSelector { get; set; }
        }

        // Flag to prevent recursive updates when programmatically changing dropdowns
        private bool _updatingServoSelectors = false;

        private void SelectMotorFunction(ComboBox combo, int functionValue)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                var item = combo.Items[i] as ComboBoxItem;
                if (item != null && item.Value == functionValue)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
            combo.SelectedIndex = 0;
        }

        private void SelectServoChannel(ComboBox combo, int servoChannel)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                var item = combo.Items[i] as ComboBoxItem;
                if (item != null && item.Value == servoChannel)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
            combo.SelectedIndex = 0;
        }

        private async void ServoSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Prevent recursive updates
            if (_updatingServoSelectors) return;

            var combo = sender as ComboBox;
            if (combo == null) return;

            var tag = combo.Tag as ServoSelectorTag;
            if (tag == null) return;

            var selectedItem = combo.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            int newServoChannel = selectedItem.Value;
            int motorNumber = tag.MotorNumber;
            int motorFunction = MOTOR_FUNCTION_BASE + motorNumber - 1;

            // Find current servo assigned to this motor
            int currentServoForThisMotor = 0;
            for (int servo = 1; servo <= MAX_SERVO_CHANNELS; servo++)
            {
                string paramName = $"SERVO{servo}_FUNCTION";
                if (MainV2.comPort.MAV.param.ContainsKey(paramName))
                {
                    int funcValue = (int)MainV2.comPort.MAV.param[paramName].Value;
                    if (funcValue == motorFunction)
                    {
                        currentServoForThisMotor = servo;
                        break;
                    }
                }
            }

            if (newServoChannel <= 0)
            {
                // Setting to "Not assigned" - clear the current servo if any
                if (currentServoForThisMotor > 0)
                {
                    string paramName = $"SERVO{currentServoForThisMotor}_FUNCTION";
                    try
                    {
                        await MainV2.comPort.setParamAsync(
                            (byte)MainV2.comPort.sysidcurrent,
                            (byte)MainV2.comPort.compidcurrent,
                            paramName,
                            0).ConfigureAwait(true);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show($"Failed to clear {paramName}: " + ex.Message, Strings.ERROR);
                    }
                }
                return;
            }

            string newParamName = $"SERVO{newServoChannel}_FUNCTION";

            try
            {
                // Check what function is currently on the target servo
                int existingFunctionOnTargetServo = 0;
                if (MainV2.comPort.MAV.param.ContainsKey(newParamName))
                {
                    existingFunctionOnTargetServo = (int)MainV2.comPort.MAV.param[newParamName].Value;
                }

                // Check if target servo is already assigned to another motor
                bool targetHasMotorFunction = existingFunctionOnTargetServo >= MOTOR_FUNCTION_BASE &&
                                               existingFunctionOnTargetServo < MOTOR_FUNCTION_BASE + 12;

                _updatingServoSelectors = true;
                try
                {
                    if (targetHasMotorFunction && currentServoForThisMotor > 0)
                    {
                        // SWAP: Target servo has a motor, and this motor has a servo
                        // Set the other motor's function to our current servo
                        string oldServoParam = $"SERVO{currentServoForThisMotor}_FUNCTION";
                        await MainV2.comPort.setParamAsync(
                            (byte)MainV2.comPort.sysidcurrent,
                            (byte)MainV2.comPort.compidcurrent,
                            oldServoParam,
                            existingFunctionOnTargetServo).ConfigureAwait(true);

                        // Update the UI for the swapped motor
                        int otherMotorNumber = existingFunctionOnTargetServo - MOTOR_FUNCTION_BASE + 1;
                        UpdateServoSelectorForMotor(otherMotorNumber, currentServoForThisMotor);
                    }
                    else if (targetHasMotorFunction)
                    {
                        // Target servo has a motor, but this motor has no servo assigned
                        // Set the other motor to "Not assigned" in UI
                        int otherMotorNumber = existingFunctionOnTargetServo - MOTOR_FUNCTION_BASE + 1;
                        UpdateServoSelectorForMotor(otherMotorNumber, 0);
                    }
                    else if (currentServoForThisMotor > 0)
                    {
                        // Just clear old servo assignment (target has no motor function)
                        string oldServoParam = $"SERVO{currentServoForThisMotor}_FUNCTION";
                        await MainV2.comPort.setParamAsync(
                            (byte)MainV2.comPort.sysidcurrent,
                            (byte)MainV2.comPort.compidcurrent,
                            oldServoParam,
                            0).ConfigureAwait(true);
                    }

                    // Set the new servo to this motor function
                    await MainV2.comPort.setParamAsync(
                        (byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent,
                        newParamName,
                        motorFunction).ConfigureAwait(true);
                }
                finally
                {
                    _updatingServoSelectors = false;
                }
            }
            catch (Exception ex)
            {
                _updatingServoSelectors = false;
                CustomMessageBox.Show($"Failed to set {newParamName}: " + ex.Message, Strings.ERROR);
            }
        }

        private void UpdateServoSelectorForMotor(int motorNumber, int newServoChannel)
        {
            // Find the motor panel that has this motor number and update its dropdown
            // Note: _updatingServoSelectors flag should already be set by caller
            foreach (var panel in motorPanels)
            {
                var tag = panel.MotorSelector?.Tag as ServoSelectorTag;
                if (tag != null && tag.MotorNumber == motorNumber)
                {
                    SelectServoChannel(panel.MotorSelector, newServoChannel);
                    break;
                }
            }
        }

        private async void MotorSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo == null) return;

            var tag = combo.Tag as MotorSelectorTag;
            if (tag == null) return;

            var selectedItem = combo.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            int newFunction = selectedItem.Value;
            int servoChannel = tag.ServoChannel;

            if (servoChannel <= 0)
            {
                CustomMessageBox.Show($"Motor {(char)('A' + tag.MotorIndex - 1)} is not assigned to a servo channel.", Strings.ERROR);
                return;
            }

            string paramName = $"SERVO{servoChannel}_FUNCTION";

            try
            {
                await MainV2.comPort.setParamAsync(
                    (byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    paramName,
                    newFunction).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Failed to set {paramName}: " + ex.Message, Strings.ERROR);
            }
        }

        private void SpinButton_Click(object sender, EventArgs e)
        {
            var button = sender as MyButton;
            if (button == null) return;

            int motorIdx = (int)button.Tag;
            int speed = (int)NUM_thr_percent.Value;
            int time = (int)NUM_duration.Value;

            try
            {
                testMotor(motorIdx, speed, time);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to test motor\n" + ex);
            }
        }

        private void SmartAssignButton_Click(object sender, EventArgs e)
        {
            var button = sender as MyButton;
            if (button == null) return;

            var tag = button.Tag as SmartAssignTag;
            if (tag == null) return;

            int motorIdx = tag.MotorIndex;
            var servoSelector = tag.ServoSelector;

            // Get current servo channel for this motor
            var selectedItem = servoSelector.SelectedItem as ComboBoxItem;
            int currentServoChannel = selectedItem?.Value ?? 0;

            if (currentServoChannel == 0)
            {
                CustomMessageBox.Show("Please assign a servo output first before using Smart Assign.", "No Servo Assigned");
                return;
            }

            // Spin the motor
            int speed = (int)NUM_thr_percent.Value;
            int time = (int)NUM_duration.Value;

            try
            {
                testMotor(motorIdx, speed, time);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to test motor\n" + ex);
                return;
            }

            // Show dialog asking which motor actually spun
            using (var dialog = new Form())
            {
                dialog.Text = "Smart Assign";
                dialog.Size = new Size(350, 180);
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                var label = new Label
                {
                    Text = $"Servo {currentServoChannel} was activated.\nWhich motor actually spun?",
                    Location = new Point(20, 20),
                    Size = new Size(300, 40)
                };

                var comboBox = new ComboBox
                {
                    Location = new Point(20, 70),
                    Size = new Size(290, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                // Populate with all motors in test order (Motor A, Motor B, etc.)
                for (int i = 1; i <= motormax; i++)
                {
                    char letter = (char)('A' + i - 1);
                    int motorNumber = i; // Default

                    // Get actual motor number from layout
                    if (motor_layout.motors != null)
                    {
                        foreach (var motor in motor_layout.motors)
                        {
                            if (motor.TestOrder == i)
                            {
                                motorNumber = motor.Number;
                                break;
                            }
                        }
                    }

                    comboBox.Items.Add(new ComboBoxItem
                    {
                        Text = $"Motor {letter} (Motor {motorNumber})",
                        Value = i
                    });
                }
                comboBox.DisplayMember = "Text";
                comboBox.SelectedIndex = 0;

                var okButton = new MyButton
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(130, 105),
                    Size = new Size(80, 30)
                };

                dialog.Controls.Add(label);
                dialog.Controls.Add(comboBox);
                dialog.Controls.Add(okButton);
                dialog.AcceptButton = okButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var selectedMotor = comboBox.SelectedItem as ComboBoxItem;
                    if (selectedMotor != null)
                    {
                        int targetMotorIdx = selectedMotor.Value;

                        // If user selected the same motor, no changes needed
                        if (targetMotorIdx == motorIdx)
                        {
                            return;
                        }

                        // Find the source motor's panel (the one we clicked "?" on)
                        var sourcePanel = motorPanels.FirstOrDefault(p => p.MotorIndex == motorIdx);

                        // Find the target motor's panel
                        var targetPanel = motorPanels.FirstOrDefault(p => p.MotorIndex == targetMotorIdx);

                        if (targetPanel != null)
                        {
                            var targetTag = targetPanel.MotorSelector.Tag as ServoSelectorTag;
                            if (targetTag != null)
                            {
                                int targetMotorNumber = targetTag.MotorNumber;
                                int targetFunctionValue = MOTOR_FUNCTION_BASE + targetMotorNumber - 1;
                                string paramName = $"SERVO{currentServoChannel}_FUNCTION";

                                _updatingServoSelectors = true;
                                try
                                {
                                    // Check if target motor already has a different servo assigned
                                    var targetCurrentItem = targetPanel.MotorSelector.SelectedItem as ComboBoxItem;
                                    int targetCurrentServo = targetCurrentItem?.Value ?? 0;

                                    if (targetCurrentServo > 0 && targetCurrentServo != currentServoChannel)
                                    {
                                        // Target motor has a different servo - clear it first
                                        string oldTargetParam = $"SERVO{targetCurrentServo}_FUNCTION";
                                        MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent,
                                            (byte)MainV2.comPort.compidcurrent, oldTargetParam, 0);
                                    }

                                    // Set the servo to the target motor's function
                                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent,
                                        (byte)MainV2.comPort.compidcurrent, paramName, targetFunctionValue);

                                    // Update target motor's dropdown to show the servo
                                    SelectServoChannel(targetPanel.MotorSelector, currentServoChannel);

                                    // Clear source motor's dropdown (it no longer has this servo)
                                    if (sourcePanel != null)
                                    {
                                        SelectServoChannel(sourcePanel.MotorSelector, 0);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CustomMessageBox.Show($"Failed to set {paramName}: " + ex.Message, Strings.ERROR);
                                }
                                finally
                                {
                                    _updatingServoSelectors = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private int get_motormax()
        {
            var motormax = 8;

            if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.GROUND_ROVER || MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.SURFACE_BOAT)
            {
                return 4;
            }

            var enable = MainV2.comPort.MAV.param.ContainsKey("FRAME") || MainV2.comPort.MAV.param.ContainsKey("Q_FRAME_TYPE") || MainV2.comPort.MAV.param.ContainsKey("FRAME_TYPE");

            if (!enable)
            {
                Enabled = false;
                return motormax;
            }

            if (set_frame_class_and_type("FRAME_CLASS", "FRAME_TYPE") ||
                set_frame_class_and_type("Q_FRAME_CLASS", "Q_FRAME_TYPE"))
            {
                if (motor_layout.motors != null)
                {
                    return motor_layout.motors.Length;
                }
            }

            MAVLink.MAV_TYPE type = MAVLink.MAV_TYPE.QUADROTOR;

            if (MainV2.comPort.MAV.param.ContainsKey("Q_FRAME_CLASS"))
            {
                var value = (int)MainV2.comPort.MAV.param["Q_FRAME_CLASS"].Value;
                switch (value)
                {
                    case 0:
                    case 1:
                        type = MAVLink.MAV_TYPE.QUADROTOR;
                        break;
                    case 2:
                    case 5:
                        type = MAVLink.MAV_TYPE.HEXAROTOR;
                        break;
                    case 3:
                    case 4:
                        type = MAVLink.MAV_TYPE.OCTOROTOR;
                        break;
                    case 6:
                        type = MAVLink.MAV_TYPE.HELICOPTER;
                        break;
                    case 7:
                        type = MAVLink.MAV_TYPE.TRICOPTER;
                        break;
                }

            }
            else if (MainV2.comPort.MAV.param.ContainsKey("FRAME"))
            {
                type = MainV2.comPort.MAV.aptype;
            }
            else if (MainV2.comPort.MAV.param.ContainsKey("FRAME_TYPE"))
            {
                type = MainV2.comPort.MAV.aptype;
            }

            if (type == MAVLink.MAV_TYPE.TRICOPTER)
            {
                motormax = 4;
            }
            else if (type == MAVLink.MAV_TYPE.QUADROTOR)
            {
                motormax = 4;
            }
            else if (type == MAVLink.MAV_TYPE.HEXAROTOR)
            {
                motormax = 6;
            }
            else if (type == MAVLink.MAV_TYPE.OCTOROTOR)
            {
                motormax = 8;
            }
            else if (type == MAVLink.MAV_TYPE.HELICOPTER)
            {
                motormax = 0;
            }
            else if (type == MAVLink.MAV_TYPE.DODECAROTOR)
            {
                motormax = 12;
            }

            return motormax;
        }

        private bool set_frame_class_and_type(string class_param_name, string type_param_name)
        {
            if (!MainV2.comPort.MAV.param.ContainsKey(class_param_name) || !MainV2.comPort.MAV.param.ContainsKey(type_param_name))
            {
                return false;
            }
            var frame_class = (int)MainV2.comPort.MAV.param[class_param_name].Value;
            var frame_type = (int)MainV2.comPort.MAV.param[type_param_name].Value;

            string className = "unknown";
            string typeName = "unknown";

            var class_list = ParameterMetaDataRepository.GetParameterOptionsInt(class_param_name, MainV2.comPort.MAV.cs.firmware.ToString());
            foreach (var item in class_list)
            {
                if (item.Key == Convert.ToInt32(frame_class))
                {
                    className = item.Value;
                    break;
                }
            }

            var type_list = ParameterMetaDataRepository.GetParameterOptionsInt(type_param_name, MainV2.comPort.MAV.cs.firmware.ToString());
            foreach (var item in type_list)
            {
                if (item.Key == Convert.ToInt32(frame_type))
                {
                    typeName = item.Value;
                    break;
                }
            }

            FrameClass.Text = $"Class: {className}, Type: {typeName}";

            lookup_frame_layout(frame_class, frame_type);
            UpdateFrameImage(frame_class, frame_type);

            return true;
        }

        private void UpdateFrameImage(int frame_class, int frame_type)
        {
            try
            {
                if (webViewFrame.CoreWebView2 == null)
                {
                    Debug.WriteLine("WebView2 CoreWebView2 not initialized");
                    return;
                }

                // Lookup SVG URL based on frame class and type
                if (FrameSvgUrls.TryGetValue((frame_class, frame_type), out string svgUrl))
                {
                    // Create HTML to display SVG with proper sizing
                    string html = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                body {{
                                    margin: 0;
                                    padding: 0;
                                    display: flex;
                                    align-items: center;
                                    justify-content: center;
                                    width: 400px;
                                    height: 400px;
                                    overflow: hidden;
                                }}
                                img {{
                                    max-width: 100%;
                                    max-height: 100%;
                                    object-fit: contain;
                                }}
                            </style>
                        </head>
                        <body>
                            <img src='{svgUrl}' alt='Frame diagram' />
                        </body>
                        </html>";

                    webViewFrame.CoreWebView2.NavigateToString(html);
                    webViewFrame.Visible = true;
                }
                else
                {
                    Debug.WriteLine($"No SVG found for frame class {frame_class}, type {frame_type}");
                    webViewFrame.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error updating frame image: " + ex.Message);
                webViewFrame.Visible = false;
            }
        }

        private void lookup_frame_layout(int frame_class, int frame_type)
        {
            try
            {
                string file = Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location)) + Path.DirectorySeparatorChar + "APMotorLayout.json";
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    var all_layouts = JsonConvert.DeserializeObject<JSON_motors>(json);
                    if (all_layouts.Version == "AP_Motors library test ver 1.2")
                    {
                        foreach (var layout in all_layouts.layouts)
                        {
                            if ((layout.Class == frame_class) && (layout.Type == frame_type))
                            {
                                motor_layout = layout;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void but_TestAll(object sender, EventArgs e)
        {
            int speed = (int)NUM_thr_percent.Value;
            int time = (int)NUM_duration.Value;

            for (int i = 1; i <= motormax; i++)
            {
                testMotor(i, speed, time);
            }
        }

        private void but_TestAllSeq(object sender, EventArgs e)
        {
            int speed = (int)NUM_thr_percent.Value;
            int time = (int)NUM_duration.Value;

            testMotor(1, speed, time, motormax);
        }

        private void but_StopAll(object sender, EventArgs e)
        {
            for (int i = 1; i <= motormax; i++)
            {
                testMotor(i, 0, 0);
            }
        }

        private void testMotor(int motor, int speed, int time, int motorcount = 0)
        {
            try
            {
                if (!MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent,
                        MAVLink.MAV_CMD.DO_MOTOR_TEST,
                        (float)motor,
                        (float)(byte)MAVLink.MOTOR_TEST_THROTTLE_TYPE.MOTOR_TEST_THROTTLE_PERCENT,
                        (float)speed,
                        (float)time,
                        (float)motorcount,
                        0,
                        0))
                {
                    CustomMessageBox.Show("Command was denied by the autopilot");
                }
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorCommunicating + "\nMotor: " + motor, Strings.ERROR);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("https://ardupilot.org/copter/docs/connect-escs-and-motors.html#motor-order-diagrams");
            }
            catch
            {
                CustomMessageBox.Show("Bad default system association", Strings.ERROR);
            }
        }

        private async void NUM_mot_spin_arm_ValueChanged(object sender, EventArgs e)
        {
            if (!MainV2.comPort.MAV.param.ContainsKey("MOT_SPIN_ARM"))
            {
                return;
            }

            try
            {
                await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent, "MOT_SPIN_ARM",
                    (float)NUM_mot_spin_arm.Value).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to set MOT_SPIN_ARM: " + ex.Message, Strings.ERROR);
            }
        }

        private async void NUM_mot_spin_min_ValueChanged(object sender, EventArgs e)
        {
            if (!MainV2.comPort.MAV.param.ContainsKey("MOT_SPIN_MIN"))
            {
                return;
            }

            try
            {
                await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent, "MOT_SPIN_MIN",
                    (float)NUM_mot_spin_min.Value).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to set MOT_SPIN_MIN: " + ex.Message, Strings.ERROR);
            }
        }
    }
}