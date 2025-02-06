using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class GimbalControlSettingsForm : Form
    {
        public GimbalControlSettings preferences;

        public GimbalControlSettingsForm(GimbalControlSettings preferences)
        {
            InitializeComponent();

            this.preferences = new GimbalControlSettings(preferences);
            LoadPreferences();

            ThemeManager.ApplyThemeTo(this);
        }

        private void LoadPreferences()
        {
            var properties = preferences.GetType().GetProperties();

            // Delete all rows in the table layout panel
            SettingsTablePanel.Controls.Clear();
            SettingsTablePanel.RowStyles.Clear();
            int row = -1;
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(PreferencesAttribute), true);
                if (attributes.Length == 0)
                {
                    continue;
                }

                row++;
                SettingsTablePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                var attribute = (PreferencesAttribute)attributes[0];

                var label = new Label
                {
                    Width = 200,
                    Text = attribute.LabelText,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Anchor = AnchorStyles.Left,
                };

                SettingsTablePanel.Controls.Add(label, 0, row);

                switch (attribute.ControlType)
                {
                case ControlType.KeyBindingButton:
                    var keyBindingButton = new KeyBindingButton
                    {
                        KeyBinding = (Keys)property.GetValue(preferences),
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    };
                    keyBindingButton.KeyBindingChanged += (sender, e) =>
                    {
                        HandleKeyBindingChanged(keyBindingButton, property);
                    };
                    SettingsTablePanel.Controls.Add(keyBindingButton, 1, row);
                    var clearKeyButton = new MyButton
                    {
                        Text = "❌",
                        Width = keyBindingButton.Height,
                        Height = keyBindingButton.Height,
                    };
                    clearKeyButton.Click += (sender, e) =>
                    {
                        keyBindingButton.KeyBinding = Keys.None;
                    };
                    SettingsTablePanel.Controls.Add(clearKeyButton, 2, row);
                    break;
                case ControlType.ClickBindingButton:
                    var clickBindingButton = new ClickBindingButton
                    {
                        ClickBinding = ((Keys, MouseButtons))property.GetValue(preferences),
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    };
                    clickBindingButton.ClickBindingChanged += (sender, e) =>
                    {
                        HandleClickBindingChanged(clickBindingButton, property);
                    };
                    SettingsTablePanel.Controls.Add(clickBindingButton, 1, row);
                    var clearClickButton = new MyButton
                    {
                        Text = "❌",
                        Width = clickBindingButton.Height,
                        Height = clickBindingButton.Height,
                    };
                    clearClickButton.Click += (sender, e) =>
                    {
                        clickBindingButton.ClickBinding = (Keys.None, MouseButtons.None);
                    };
                    SettingsTablePanel.Controls.Add(clearClickButton, 2, row);
                    break;
                case ControlType.ModifierBinding:
                    var modifierBinding = new KeyBindingButton
                    {
                        ModifiersOnly = true,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    };
                    modifierBinding.KeyBinding = (Keys)property.GetValue(preferences);
                    SettingsTablePanel.Controls.Add(modifierBinding, 1, row);
                    modifierBinding.KeyBindingChanged += (sender, e) =>
                    {
                        HandleKeyBindingChanged(modifierBinding, property);
                    };
                    var clearModifierButton = new MyButton
                    {
                        Text = "❌",
                        Width = modifierBinding.Height,
                        Height = modifierBinding.Height,
                    };
                    clearModifierButton.Click += (sender, e) =>
                    {
                        modifierBinding.KeyBinding = Keys.None;
                    };
                    SettingsTablePanel.Controls.Add(clearModifierButton, 2, row);
                    break;
                case ControlType.DecimalUpDown:
                    var value = (decimal)property.GetValue(preferences);
                    var decimalUpDown = new NumericUpDown
                    {
                        Value = value,
                        Minimum = Math.Min((decimal)attribute.Min, value),
                        Maximum = Math.Max((decimal)attribute.Max, value),
                        Increment = (decimal)attribute.Increment,
                        DecimalPlaces = attribute.DecimalPlaces
                    };
                    decimalUpDown.ValueChanged += (sender, e) =>
                    {
                        property.SetValue(preferences, decimalUpDown.Value);
                    };
                    SettingsTablePanel.Controls.Add(decimalUpDown, 1, row);
                    break;
                case ControlType.Checkbox:
                    var checkbox = new CheckBox
                    {
                        Checked = (bool)property.GetValue(preferences)
                    };
                    checkbox.CheckedChanged += (sender, e) =>
                    {
                        property.SetValue(preferences, checkbox.Checked);
                    };
                    SettingsTablePanel.Controls.Add(checkbox, 1, row);
                    break;
                }
            }

            // Recalculate column widths
            SettingsTablePanel.Invalidate();
        }

        private void but_save_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void but_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        // Flag to suppress handlers to prevent re-entry when reverting to the previous value
        private bool suppressHandlers = false;

        /// <summary>
        /// After a new key binding has been assigned, check for clashes and update the preferences object or revert if needed.
        /// </summary>
        /// <param name="keyBindingButton">Button that triggered this event</param>
        /// <param name="property">Property that the button is controlling</param>
        private void HandleKeyBindingChanged(KeyBindingButton keyBindingButton, PropertyInfo property)
        {
            if (suppressHandlers)
            {
                return;
            }
            if (keyBindingButton.KeyBinding == Keys.None ||
                GetClashes(keyBindingButton.KeyBinding, property) == (int)DialogResult.Yes)
            {
                property.SetValue(preferences, keyBindingButton.KeyBinding);
            }
            else
            {
                suppressHandlers = true;
                keyBindingButton.KeyBinding = (Keys)property.GetValue(preferences);
                suppressHandlers = false;
            }
        }

        /// <summary>
        /// After a new click binding has been assigned, check for clashes and update the preferences object or revert if needed.
        /// </summary>
        /// <param name="clickBindingButton">Button that triggered this event</param>
        /// <param name="property">Property that the button is controlling</param>
        private void HandleClickBindingChanged(ClickBindingButton clickBindingButton, PropertyInfo property)
        {
            if (suppressHandlers)
            {
                return;
            }
            if (clickBindingButton.ClickBinding == (Keys.None, MouseButtons.None) ||
                GetClashes(clickBindingButton.ClickBinding, property) == (int)DialogResult.Yes)
            {
                property.SetValue(preferences, clickBindingButton.ClickBinding);
            }
            else
            {
                suppressHandlers = true;
                clickBindingButton.ClickBinding = ((Keys, MouseButtons))property.GetValue(preferences);
                suppressHandlers = false;
            }
        }

        /// <summary>
        /// Scan through all properties of the preferences object and check for clashes with the new binding. If any are found, show a warning message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binding">New binding that we are attempting to set</param>
        /// <param name="property">Property that stores the keybinding</param>
        /// <returns>Dialog result of warning pop-up</returns>
        private int GetClashes<T>(T binding, PropertyInfo property)
        {
            var properties = preferences.GetType().GetProperties();
            var clashList = new List<PreferencesAttribute>();
            foreach (var otherProperty in properties)
            {
                if (otherProperty == property)
                {
                    continue;
                }
                if (otherProperty.PropertyType != property.PropertyType)
                {
                    continue;
                }
                var attributes = otherProperty.GetCustomAttributes(typeof(PreferencesAttribute), true);
                if (attributes.Length == 0)
                {
                    continue;
                }
                var attribute = (PreferencesAttribute)attributes[0];
                if (object.Equals(otherProperty.GetValue(preferences), binding))
                {
                    clashList.Add(attribute);
                }
            }

            if (clashList.Count > 0)
            {
                var clashMessage = new StringBuilder();
                clashMessage.AppendLine("This binding is already used by the following:");
                clashMessage.AppendLine();
                foreach (var clash in clashList)
                {
                    clashMessage.AppendLine($"- {clash.LabelText}");
                }
                clashMessage.AppendLine();
                clashMessage.AppendLine("Are you sure you want to use this binding?");
                return CustomMessageBox.Show(clashMessage.ToString(), "Key Binding Clash", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }

            return (int)DialogResult.Yes;
        }
    }

    public enum ControlType
    {
        KeyBindingButton,
        ClickBindingButton,
        ModifierBinding,
        DecimalUpDown,
        Checkbox
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PreferencesAttribute : Attribute
    {
        public string LabelText { get; }
        public ControlType ControlType { get; }

        // Additional properties for DecimalUpDown control
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 100;
        public double Increment { get; set; } = 1;
        public int DecimalPlaces { get; set; } = 2;

        public PreferencesAttribute(string labelText, ControlType controlType)
        {
            LabelText = labelText;
            ControlType = controlType;
        }
    }

    public class GimbalControlSettings
    {
        // Keybindings for various actions
        [Preferences("Slew Left", ControlType.KeyBindingButton)]
        public Keys SlewLeft { get; set; }
        [Preferences("Slew Right", ControlType.KeyBindingButton)]
        public Keys SlewRight { get; set; }
        [Preferences("Slew Up", ControlType.KeyBindingButton)]
        public Keys SlewUp { get; set; }
        [Preferences("Slew Down", ControlType.KeyBindingButton)]
        public Keys SlewDown { get; set; }
        [Preferences("Zoom In", ControlType.KeyBindingButton)]
        public Keys ZoomIn { get; set; }
        [Preferences("Zoom Out", ControlType.KeyBindingButton)]
        public Keys ZoomOut { get; set; }

        [Preferences("Slew Fast Modifier", ControlType.ModifierBinding)]
        public Keys SlewFastModifier { get; set; }
        [Preferences("Slew Slow Modifier", ControlType.ModifierBinding)]
        public Keys SlewSlowModifier { get; set; }

        [Preferences("Take Picture", ControlType.KeyBindingButton)]
        public Keys TakePicture { get; set; }
        [Preferences("Toggle Recording", ControlType.KeyBindingButton)]
        public Keys ToggleRecording { get; set; }
        [Preferences("Start Recording", ControlType.KeyBindingButton)]
        public Keys StartRecording { get; set; }
        [Preferences("Stop Recording", ControlType.KeyBindingButton)]
        public Keys StopRecording { get; set; }

        [Preferences("Toggle Lock/Follow", ControlType.KeyBindingButton)]
        public Keys ToggleLockFollow { get; set; }
        [Preferences("Set Lock", ControlType.KeyBindingButton)]
        public Keys SetLock { get; set; }
        [Preferences("Set Follow", ControlType.KeyBindingButton)]
        public Keys SetFollow { get; set; }
        [Preferences("Retract", ControlType.KeyBindingButton)]
        public Keys Retract { get; set; }
        [Preferences("Neutral", ControlType.KeyBindingButton)]
        public Keys Neutral { get; set; }
        [Preferences("Point Down", ControlType.KeyBindingButton)]
        public Keys PointDown { get; set; }
        [Preferences("Home", ControlType.KeyBindingButton)]
        public Keys Home { get; set; }


        [Preferences("Click Pan/Tilt", ControlType.ClickBindingButton)]
        public (Keys, MouseButtons) MoveCameraToMouseLocation { get; set; }
        [Preferences("Click Point-of-Interest", ControlType.ClickBindingButton)]
        public (Keys, MouseButtons) MoveCameraPOIToMouseLocation { get; set; }
        [Preferences("Click Track Object", ControlType.ClickBindingButton)]
        public (Keys, MouseButtons) TrackObjectUnderMouse { get; set; }


        // Speed settings
        [Preferences("Slew Speed Slow (deg/s)", ControlType.DecimalUpDown, Min = 0.1, Max = 360, Increment = 1, DecimalPlaces = 1)]
        public decimal SlewSpeedSlow { get; set; }
        [Preferences("Slew Speed Normal (deg/s)", ControlType.DecimalUpDown, Min = 0.1, Max = 360, Increment = 1, DecimalPlaces = 1)]
        public decimal SlewSpeedNormal { get; set; }
        [Preferences("Slew Speed Fast (deg/s)", ControlType.DecimalUpDown, Min = 0.1, Max = 360, Increment = 1, DecimalPlaces = 1)]
        public decimal SlewSpeedFast { get; set; }

        [Preferences("Zoom Speed (unitless)", ControlType.DecimalUpDown, Min = 0.01, Max = 1, Increment = 0.1, DecimalPlaces = 2)]
        public decimal ZoomSpeed { get; set; }
        [Preferences("Camera Horizontal FOV (deg)", ControlType.DecimalUpDown, Min = 0.01, Max = 180, Increment = 1, DecimalPlaces = 2)]
        public decimal CameraHFOV { get; set; }
        [Preferences("Camera Vertical FOV (deg)", ControlType.DecimalUpDown, Min = 0.01, Max = 180, Increment = 1, DecimalPlaces = 2)]
        public decimal CameraVFOV { get; set; }

        // Boolean options
        [Preferences("Use FOV Reported by Camera", ControlType.Checkbox)]
        public bool UseFOVReportedByCamera { get; set; }
        [Preferences("Default Locked Mode", ControlType.Checkbox)]
        public bool DefaultLockedMode { get; set; }

        public GimbalControlSettings()
        {
            SlewLeft = Keys.A;
            SlewRight = Keys.D;
            SlewUp = Keys.W;
            SlewDown = Keys.S;
            ZoomIn = Keys.E;
            ZoomOut = Keys.Q;

            SlewSlowModifier = Keys.Control;
            SlewFastModifier = Keys.Shift;

            TakePicture = Keys.Alt | Keys.F;
            ToggleRecording = Keys.Alt | Keys.R;
            StartRecording = Keys.None;
            StopRecording = Keys.None;

            ToggleLockFollow = Keys.L;
            SetLock = Keys.None;
            SetFollow = Keys.None;
            Retract = Keys.None;
            Neutral = Keys.N;
            PointDown = Keys.None;
            Home = Keys.H;

            MoveCameraToMouseLocation = (Keys.None, MouseButtons.Left);
            MoveCameraPOIToMouseLocation = (Keys.Control, MouseButtons.Left);
            TrackObjectUnderMouse = (Keys.Alt, MouseButtons.Left);

            // Default speed settings
            SlewSpeedSlow = 1m; // deg/sec
            SlewSpeedNormal = 5m; // deg/sec
            SlewSpeedFast = 25m; // deg/sec
            ZoomSpeed = 1.0m; // unitless [0, 1]
            CameraHFOV = 40.0m; // horizontal, degrees
            CameraVFOV = 30.0m; // vertical, degrees

            // Default boolean options
            UseFOVReportedByCamera = true;
            DefaultLockedMode = false;
        }

        public GimbalControlSettings(GimbalControlSettings other)
        {
            foreach (var property in other.GetType().GetProperties())
            {
                property.SetValue(this, property.GetValue(other));
            }
        }
    }
}
