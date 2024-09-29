using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace MissionPlanner.Controls
{
    /// <summary>
    /// Custom button control for binding a key to a function.
    /// </summary>
    public class KeyBindingButton : MyButton
    {
        // Indicates if the button is currently waiting for a key input
        private bool isListening = false;

        // Stores key combination that is currently bound to this function
        private Keys _keyBinding = Keys.None;

        // Backup of the outline color so we can paint it red when listening for a key
        private Color _OutlineBackup;

        // The prompt text that is displayed when listening for a key and nothing has been pressed yet
        public string Prompt = "Press a key... (Esc to cancel)";

        /// <summary>
        /// Event that is raised when the key binding is changed.
        /// </summary>
        public event EventHandler KeyBindingChanged;

        /// <summary>
        /// The key combination that is bound to this function.
        /// </summary>
        [Category("Key Binding")]
        [Description("The key combination that is bound to this function.")]
        public Keys KeyBinding
        {
            get { return _keyBinding; }
            set
            {
                _keyBinding = value;
                Text = GetKeyString(_keyBinding);
                KeyBindingChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _modifiersOnly = false;
        /// <summary>
        /// True if this button should only be used to assign a combination of modifier keys.
        /// </summary>
        [Category("Key Binding")]
        [Description("Only allow modifier keys to be assigned.")]
        public bool ModifiersOnly
        {
            get
            {
                return _modifiersOnly;
            }
            set
            {
                Prompt = value ?
                    "Press modifier keys + Enter..." :
                    "Press a key... (Esc to cancel)";
                _modifiersOnly = value;
            }
        }

        public KeyBindingButton()
        {
            _BGGradTop = Color.FromArgb(0x94, 0xc1, 0x1f);
            _BGGradBot = Color.FromArgb(0xcd, 0xe2, 0x96);
            _TextColor = Color.FromArgb(0x40, 0x57, 0x04);
            _Outline = Color.FromArgb(0x79, 0x94, 0x29);
            _ColorNotEnabled = Color.FromArgb(73, 0x2b, 0x3a, 0x03);
            _ColorMouseOver = Color.FromArgb(73, 0x2b, 0x3a, 0x03);
            _ColorMouseDown = Color.FromArgb(150, 0x2b, 0x3a, 0x03);
        }

        /// <summary>
        /// Start listening for a key press.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!isListening)
            {
                // Make the outline red to indicate that we are listening for a key
                _OutlineBackup = _Outline;
                _Outline = Color.Red;

                isListening = true;
                Text = Prompt;
            }

            base.OnMouseClick(e);
        }

        /// <summary>
        /// Cancel key assignment. Prevents multiple buttons from listening at the same time.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLeave(EventArgs e)
        {
            if (isListening)
            {
                CancelKeyAssignment();
            }

            base.OnLeave(e);
        }

        /// <summary>
        /// Prevents arrow keys and space/enter from being ignored.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
            base.OnPreviewKeyDown(e);
        }

        /// <summary>
        /// Update the displayed key combination, and stop listening if we have a valid key combination.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (isListening)
            {
                // Cancel key assignment if the user presses escape
                if (e.KeyCode == Keys.Escape)
                {
                    CancelKeyAssignment();
                    return;
                }

                // Display the modifier keys as they are pressed
                Text = GetModifiersString(e.KeyData);

                // Modifier keys are not allowed as the main key
                if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu)
                {
                    return;
                }

                if (_modifiersOnly)
                {
                    // If only modifier keys are allowed, we wait for the user to press Enter
                    if (e.KeyCode == Keys.Enter)
                    {
                        KeyBinding = e.Modifiers;
                        CancelKeyAssignment();
                    }
                    return;
                }

                // If any non-modifier key is pressed, assign it (and any modifiers) to the key binding
                KeyBinding = e.KeyData;
                CancelKeyAssignment();
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Update the displayed key combination when a modifier key is released.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (isListening)
            {
                if (e.Modifiers != Keys.None)
                {
                    Text = GetModifiersString(e.KeyData);
                }
                else
                {
                    // No keys are pressed, display the prompt text again
                    Text = Prompt;
                }
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Cancel key assignment, restore outline color, and display the current key combination.
        /// </summary>
        private void CancelKeyAssignment()
        {
            Text = GetKeyString(KeyBinding);
            _Outline = _OutlineBackup;
            isListening = false;
            this.Invalidate();
        }

        /// <summary>
        /// Get a string representation of a key combination.
        /// </summary>
        /// <param name="key">Keys enum representing a key combination</param>
        /// <returns>Keys represented as a string</returns>
        private string GetKeyString(Keys key)
        {
            string modString = GetModifiersString(key);

            if (_modifiersOnly)
            {
                return modString;
            }

            // Remove the modifiers to isolate the main key
            key &= ~Keys.Shift;
            key &= ~Keys.Control;
            key &= ~Keys.Alt;

            if (key == Keys.None)
            {
                return "None";
            }
            return modString + key.ToString();
        }

        /// <summary>
        /// Get a string representation of just the modifier keys in a key combination.
        /// </summary>
        /// <param name="key">Keys enum representing a key combination</param>
        /// <returns>Modifier keys represented as a string</returns>
        private string GetModifiersString(Keys key)
        {
            string modifiers = "";
            if ((key & Keys.Shift) != 0)
            {
                modifiers += "Shift + ";
            }
            if ((key & Keys.Control) != 0)
            {
                modifiers += "Ctrl + ";
            }
            if ((key & Keys.Alt) != 0)
            {
                modifiers += "Alt + ";
            }
            return modifiers;
        }
    }
}
