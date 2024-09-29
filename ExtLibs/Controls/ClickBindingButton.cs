using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace MissionPlanner.Controls
{
    /// <summary>
    /// Custom button control for binding a mouse click (with optional modifier keys) to a function.
    /// </summary>
    public class ClickBindingButton : MyButton
    {
        // Indicates if the button is currently waiting for an input
        private bool isListening = false;

        // Stores the mouse button that is currently bound to this function
        private MouseButtons _clickBinding = MouseButtons.None;

        // Stores the optional modifier keys that are bound to this function
        private Keys _modifiers = Keys.None;

        // Backup of the outline color so we can paint it red when listening for a key
        private Color _OutlineBackup;

        // The prompt text that is displayed when listening for input and nothing has been pressed yet
        public string Prompt = "Click a mouse button... (Esc to cancel)";

        /// <summary>
        /// Event that is raised when the click binding is changed.
        /// </summary>
        public event EventHandler ClickBindingChanged;

        /// <summary>
        /// The key/button combination that is bound to this function.
        /// </summary>
        [Category("Click Binding")]
        [Description("The mouse button and modifier keys that are bound to this function.")]
        public (Keys, MouseButtons) ClickBinding
        {
            get { return (_modifiers, _clickBinding); }
            set
            {
                _modifiers = value.Item1;
                _clickBinding = value.Item2;
                Text = GetClickBindingString(_modifiers, _clickBinding);
                ClickBindingChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public ClickBindingButton()
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
        /// Start listening for a mouse click, or bind the clicked button if already listening.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            if (!isListening)
            {
                // Start listening
                _OutlineBackup = _Outline;
                _Outline = Color.Red;
                isListening = true;
                Text = Prompt;
            }
            else
            {
                // Handle the click binding
                if (e is MouseEventArgs mouseEvent)
                {
                    _modifiers = ModifierKeys;
                    _clickBinding = mouseEvent.Button;
                    CancelClickAssignment();
                }
            }

            base.OnClick(e);
        }

        /// <summary>
        /// Cancel click assignment. Prevents multiple buttons from listening at the same time.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLeave(EventArgs e)
        {
            if (isListening)
            {
                CancelClickAssignment();
            }

            base.OnLeave(e);
        }

        /// <summary>
        /// Update the displayed key combination
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (isListening)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    CancelClickAssignment();
                    return;
                }

                _modifiers = ModifierKeys;
                Text = GetModifiersString(_modifiers);
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
                    Text = Prompt;
                }
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Cancel click assignment, restore outline color, and display the current key combination.
        /// </summary>
        private void CancelClickAssignment()
        {
            Text = GetClickBindingString(_modifiers, _clickBinding);
            _Outline = _OutlineBackup;
            isListening = false;
        }

        /// <summary>
        /// Get the string representation of the current key/button combination.
        /// </summary>
        /// <param name="modifiers">Keys enum representing held modifier keys</param>
        /// <param name="clickButton">MouseButtons enum representing the clicked mouse button</param>
        /// <returns>Key/button combination represented as a string</returns>
        private string GetClickBindingString(Keys modifiers, MouseButtons clickButton)
        {
            if (clickButton == MouseButtons.None)
            {
                return "None";
            }

            string bindingString = GetModifiersString(modifiers);

            if (clickButton != MouseButtons.None)
            {
                bindingString += $"{clickButton} Click";
            }

            return bindingString;
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
