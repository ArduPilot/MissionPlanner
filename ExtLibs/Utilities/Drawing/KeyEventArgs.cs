using System;

namespace MissionPlanner.Utilities.Drawing
{
    public class KeyEventArgs : EventArgs
    {
        private readonly Keys keyData;

        private bool handled;

        private bool suppressKeyPress;

        /// <summary>Gets a value indicating whether the ALT key was pressed.</summary>
        /// <returns>
        ///     <see langword="true" /> if the ALT key was pressed; otherwise, <see langword="false" />.</returns>
        public virtual bool Alt => (keyData & Keys.Alt) == Keys.Alt;

        /// <summary>Gets a value indicating whether the CTRL key was pressed.</summary>
        /// <returns>
        ///     <see langword="true" /> if the CTRL key was pressed; otherwise, <see langword="false" />.</returns>
        public bool Control => (keyData & Keys.Control) == Keys.Control;

        /// <summary>Gets or sets a value indicating whether the event was handled.</summary>
        /// <returns>
        ///     <see langword="true" /> to bypass the control's default handling; otherwise, <see langword="false" /> to also pass the event along to the default control handler.</returns>
        public bool Handled
        {
            get
            {
                return handled;
            }
            set
            {
                handled = value;
            }
        }

        /// <summary>Gets the keyboard code for a <see cref="E:System.Windows.Forms.Control.KeyDown" /> or <see cref="E:System.Windows.Forms.Control.KeyUp" /> event.</summary>
        /// <returns>A <see cref="T:System.Windows.Forms.Keys" /> value that is the key code for the event.</returns>
        public Keys KeyCode
        {
            get
            {
                Keys keys = keyData & Keys.KeyCode;
                if (!Enum.IsDefined(typeof(Keys), (int)keys))
                {
                    return Keys.None;
                }
                return keys;
            }
        }

        /// <summary>Gets the keyboard value for a <see cref="E:System.Windows.Forms.Control.KeyDown" /> or <see cref="E:System.Windows.Forms.Control.KeyUp" /> event.</summary>
        /// <returns>The integer representation of the <see cref="P:System.Windows.Forms.KeyEventArgs.KeyCode" /> property.</returns>
        public int KeyValue => (int)(keyData & Keys.KeyCode);

        /// <summary>Gets the key data for a <see cref="E:System.Windows.Forms.Control.KeyDown" /> or <see cref="E:System.Windows.Forms.Control.KeyUp" /> event.</summary>
        /// <returns>A <see cref="T:System.Windows.Forms.Keys" /> representing the key code for the key that was pressed, combined with modifier flags that indicate which combination of CTRL, SHIFT, and ALT keys was pressed at the same time.</returns>
        public Keys KeyData => keyData;

        /// <summary>Gets the modifier flags for a <see cref="E:System.Windows.Forms.Control.KeyDown" /> or <see cref="E:System.Windows.Forms.Control.KeyUp" /> event. The flags indicate which combination of CTRL, SHIFT, and ALT keys was pressed.</summary>
        /// <returns>A <see cref="T:System.Windows.Forms.Keys" /> value representing one or more modifier flags.</returns>
        public Keys Modifiers => keyData & Keys.Modifiers;

        /// <summary>Gets a value indicating whether the SHIFT key was pressed.</summary>
        /// <returns>
        ///     <see langword="true" /> if the SHIFT key was pressed; otherwise, <see langword="false" />.</returns>
        public virtual bool Shift => (keyData & Keys.Shift) == Keys.Shift;

        /// <summary>Gets or sets a value indicating whether the key event should be passed on to the underlying control.</summary>
        /// <returns>
        ///     <see langword="true" /> if the key event should not be sent to the control; otherwise, <see langword="false" />.</returns>
        public bool SuppressKeyPress
        {
            get
            {
                return suppressKeyPress;
            }
            set
            {
                suppressKeyPress = value;
                handled = value;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.KeyEventArgs" /> class.</summary>
        /// <param name="keyData">A <see cref="T:System.Windows.Forms.Keys" /> representing the key that was pressed, combined with any modifier flags that indicate which CTRL, SHIFT, and ALT keys were pressed at the same time. Possible values are obtained be applying the bitwise OR (|) operator to constants from the <see cref="T:System.Windows.Forms.Keys" /> enumeration. </param>
        public KeyEventArgs(Keys keyData)
        {
            this.keyData = keyData;
        }
    }
}