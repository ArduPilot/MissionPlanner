using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public class PlaceholderTextBox : TextBox
    {
        private string _placeholderText = "";
        private Color _placeholderColor = Color.Gray;

        // P/Invoke for centering text in multiline textbox
        private const int EM_SETMARGINS = 0xD3;
        private const int EC_LEFTMARGIN = 0x1;
        private const int EC_RIGHTMARGIN = 0x2;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [Category("Appearance")]
        [Description("The placeholder text to display when the textbox is empty.")]
        public string PlaceholderText
        {
            get { return _placeholderText; }
            set
            {
                _placeholderText = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("The color of the placeholder text.")]
        public Color PlaceholderColor
        {
            get { return _placeholderColor; }
            set
            {
                _placeholderColor = value;
                Invalidate();
            }
        }

        public PlaceholderTextBox()
        {
            // Enable multiline to allow custom height
            this.Multiline = true;

            // Set default height
            this.Height = 24;

            this.SetStyle(ControlStyles.UserPaint, false);

            // Apply theme colors
            ApplyTheme();
        }

        /// <summary>
        /// Sets up autocomplete with a custom list of suggestions.
        /// Note: Autocomplete requires Multiline to be false.
        /// </summary>
        public void SetAutoCompleteSource(IEnumerable<string> items)
        {
            // Autocomplete doesn't work with multiline textboxes
            // We need to disable multiline but preserve the height
            int currentHeight = this.Height;
            this.Multiline = false;
            this.Height = currentHeight;

            var collection = new AutoCompleteStringCollection();
            collection.AddRange(items.ToArray());

            this.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.AutoCompleteCustomSource = collection;
        }

        /// <summary>
        /// Disables autocomplete
        /// </summary>
        public void DisableAutoComplete()
        {
            this.AutoCompleteMode = AutoCompleteMode.None;
            this.AutoCompleteSource = AutoCompleteSource.None;
        }

        /// <summary>
        /// Applies theme colors to the textbox
        /// </summary>
        public void ApplyTheme()
        {
            this.BackColor = ThemeManager.ControlBGColor;
            this.ForeColor = ThemeManager.TextColor;
            _placeholderColor = Color.FromArgb(128, ThemeManager.TextColor);
            Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Set minimal margins to align with placeholder
            SendMessage(Handle, EM_SETMARGINS, (IntPtr)(EC_LEFTMARGIN | EC_RIGHTMARGIN), (IntPtr)0x00020002);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Prevent Enter key from creating new lines (keep single-line behavior)
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            base.OnKeyDown(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // WM_PAINT
            if (m.Msg == 0xF)
            {
                if (!Focused && string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(_placeholderText))
                {
                    using (Graphics g = Graphics.FromHwnd(Handle))
                    {
                        // Use the same text rendering as the control for consistent appearance
                        TextFormatFlags flags = TextFormatFlags.VerticalCenter |
                                                TextFormatFlags.Left |
                                                TextFormatFlags.EndEllipsis |
                                                TextFormatFlags.TextBoxControl |
                                                TextFormatFlags.NoPadding;

                        Rectangle rect = ClientRectangle;

                        // Adjust for border to match internal text area
                        switch (BorderStyle)
                        {
                            case BorderStyle.Fixed3D:
                                rect.Inflate(-2, -2);
                                break;
                            case BorderStyle.FixedSingle:
                                rect.Inflate(-1, -1);
                                break;
                        }

                        // Match the exact margins set by EM_SETMARGINS
                        rect.X += 2;
                        rect.Width -= 4;

                        // Vertical centering - account for text baseline
                        // For multiline textboxes, text starts slightly lower
                        rect.Y += 1;
                        rect.Height -= 2;

                        // Draw with the exact same font as the textbox
                        TextRenderer.DrawText(g, _placeholderText, this.Font, rect, _placeholderColor,
                                            BackColor, flags);
                    }
                }
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();

            // Remove any newlines that might have been pasted
            if (Text.Contains("\r") || Text.Contains("\n"))
            {
                int selectionStart = SelectionStart;
                Text = Text.Replace("\r", "").Replace("\n", "");
                SelectionStart = Math.Min(selectionStart, Text.Length);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        // Override to ensure text is vertically centered
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AdjustHeight();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

        private void AdjustHeight()
        {
            // Calculate appropriate height based on font
            if (Font != null && Height < Font.Height + 8)
            {
                Height = Font.Height + 8;
            }
        }
    }
}
