extern alias SystemDrawing;

using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Drawing = SystemDrawing::System.Drawing;

namespace MissionPlanner.AIWaypointPlanner
{
    public enum PluginButtonStyle
    {
        Secondary,
        Primary,
        Danger
    }

    /// <summary>
    /// Fully paints the tab strip so the native WinForms background cannot
    /// reintroduce a bright system-colored band inside the dark plugin.
    /// </summary>
    public class PluginTabControl : TabControl
    {
        public PluginTabControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            PluginTheme.PaintTabControl(this, e.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            PluginTheme.PaintTabControlBackground(this, e.Graphics);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Invalidate();
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
    }

    /// <summary>
    /// High-contrast, plugin-owned colors and WinForms theming helpers.
    /// Mission Planner 1.3.83 themes controls by exact runtime type, so derived
    /// layout controls and controls created after the host theme pass need a
    /// deterministic second pass owned by the plugin.
    /// </summary>
    public static class PluginTheme
    {
        public static readonly Drawing.Color WindowBackground = Drawing.Color.FromArgb(17, 19, 24);
        public static readonly Drawing.Color Surface = Drawing.Color.FromArgb(27, 30, 36);
        public static readonly Drawing.Color RaisedSurface = Drawing.Color.FromArgb(36, 40, 50);
        public static readonly Drawing.Color InputBackground = Drawing.Color.FromArgb(23, 26, 32);
        public static readonly Drawing.Color Border = Drawing.Color.FromArgb(112, 122, 142);
        public static readonly Drawing.Color PrimaryText = Drawing.Color.FromArgb(245, 247, 250);
        public static readonly Drawing.Color SecondaryText = Drawing.Color.FromArgb(184, 192, 204);
        public static readonly Drawing.Color Accent = Drawing.Color.FromArgb(29, 78, 216);
        public static readonly Drawing.Color SelectionBackground = Drawing.Color.FromArgb(57, 112, 200);
        public static readonly Drawing.Color SelectionOutline = Drawing.Color.FromArgb(104, 151, 255);
        public static readonly Drawing.Color Danger = Drawing.Color.FromArgb(180, 35, 47);
        public static readonly Drawing.Color UserMessage = Drawing.Color.FromArgb(32, 57, 88);
        public static readonly Drawing.Color AssistantMessage = Drawing.Color.FromArgb(36, 40, 50);
        public static readonly Drawing.Color ErrorMessage = Drawing.Color.FromArgb(58, 23, 28);
        public static readonly Drawing.Color ErrorText = Drawing.Color.FromArgb(255, 205, 210);
        public static readonly Drawing.Color SafetyBanner = Drawing.Color.FromArgb(176, 0, 32);

        private static readonly Drawing.Color AccentHover = Drawing.Color.FromArgb(37, 99, 235);
        private static readonly Drawing.Color AccentPressed = Drawing.Color.FromArgb(30, 64, 175);
        private static readonly Drawing.Color DangerHover = Drawing.Color.FromArgb(204, 45, 58);
        private static readonly Drawing.Color DangerPressed = Drawing.Color.FromArgb(132, 25, 35);
        private static readonly Drawing.Color DisabledBackground = Drawing.Color.FromArgb(38, 42, 50);
        private static readonly Drawing.Color DisabledText = Drawing.Color.FromArgb(166, 174, 186);

        private sealed class ButtonThemeState
        {
            public PluginButtonStyle Style;
        }

        private sealed class ThemeMarker
        {
            public bool RefreshQueued;
        }

        private static readonly ConditionalWeakTable<Button, ButtonThemeState> ButtonStates =
            new ConditionalWeakTable<Button, ButtonThemeState>();
        private static readonly ConditionalWeakTable<TabControl, ThemeMarker> ThemedTabs =
            new ConditionalWeakTable<TabControl, ThemeMarker>();
        private static readonly ConditionalWeakTable<ComboBox, ThemeMarker> ThemedComboBoxes =
            new ConditionalWeakTable<ComboBox, ThemeMarker>();

        public static void Apply(Control control)
        {
            if (control == null || control.IsDisposed)
                return;

            ApplyControl(control);
            foreach (Control child in control.Controls)
                Apply(child);

            ConversationMessageControl message = control as ConversationMessageControl;
            if (message != null)
                message.ApplyPluginThemeColors();
        }

        public static void ApplyButton(Button button, PluginButtonStyle style)
        {
            if (button == null || button.IsDisposed)
                return;

            ButtonThemeState state;
            if (!ButtonStates.TryGetValue(button, out state))
            {
                state = new ButtonThemeState();
                ButtonStates.Add(button, state);
                button.EnabledChanged += ThemedButtonEnabledChanged;
            }

            state.Style = style;
            ApplyButtonColors(button, style);
        }

        public static void ApplyMutedText(Control control)
        {
            if (control == null || control.IsDisposed)
                return;
            control.ForeColor = SecondaryText;
        }

        public static void ApplyRaised(Control control)
        {
            if (control == null || control.IsDisposed)
                return;
            control.BackColor = RaisedSurface;
            control.ForeColor = PrimaryText;
        }

        public static double GetContrastRatio(Drawing.Color first, Drawing.Color second)
        {
            double firstLuminance = GetRelativeLuminance(first);
            double secondLuminance = GetRelativeLuminance(second);
            double lighter = Math.Max(firstLuminance, secondLuminance);
            double darker = Math.Min(firstLuminance, secondLuminance);
            return (lighter + 0.05D) / (darker + 0.05D);
        }

        public static double GetRelativeLuminance(Drawing.Color color)
        {
            double red = Linearize(color.R / 255D);
            double green = Linearize(color.G / 255D);
            double blue = Linearize(color.B / 255D);
            return (0.2126D * red) + (0.7152D * green) + (0.0722D * blue);
        }

        private static double Linearize(double component)
        {
            return component <= 0.04045D
                ? component / 12.92D
                : Math.Pow((component + 0.055D) / 1.055D, 2.4D);
        }

        private static void ApplyControl(Control control)
        {
            Form form = control as Form;
            if (form != null)
            {
                form.BackColor = WindowBackground;
                form.ForeColor = PrimaryText;
                return;
            }

            TabPage tabPage = control as TabPage;
            if (tabPage != null)
            {
                tabPage.BackColor = WindowBackground;
                tabPage.ForeColor = PrimaryText;
                tabPage.BorderStyle = BorderStyle.None;
                return;
            }

            TabControl tabControl = control as TabControl;
            if (tabControl != null)
            {
                ApplyTabControl(tabControl);
                return;
            }

            DataGridView grid = control as DataGridView;
            if (grid != null)
            {
                ApplyDataGridView(grid);
                return;
            }

            Button button = control as Button;
            if (button != null)
            {
                ButtonThemeState state;
                if (ButtonStates.TryGetValue(button, out state))
                    ApplyButton(button, state.Style);
                else
                    ApplyButton(button, PluginButtonStyle.Secondary);
                return;
            }

            ComboBox comboBox = control as ComboBox;
            if (comboBox != null)
            {
                ApplyComboBox(comboBox);
                return;
            }

            TextBoxBase textBox = control as TextBoxBase;
            if (textBox != null)
            {
                textBox.BackColor = InputBackground;
                textBox.ForeColor = PrimaryText;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                return;
            }

            LinkLabel linkLabel = control as LinkLabel;
            if (linkLabel != null)
            {
                linkLabel.ForeColor = PrimaryText;
                linkLabel.LinkColor = Drawing.Color.FromArgb(132, 174, 255);
                linkLabel.ActiveLinkColor = Drawing.Color.FromArgb(179, 205, 255);
                linkLabel.VisitedLinkColor = Drawing.Color.FromArgb(180, 162, 255);
                return;
            }

            Label label = control as Label;
            if (label != null)
            {
                label.ForeColor = PrimaryText;
                return;
            }

            CheckBox checkBox = control as CheckBox;
            if (checkBox != null)
            {
                checkBox.BackColor = Drawing.Color.Transparent;
                checkBox.ForeColor = PrimaryText;
                return;
            }

            RadioButton radioButton = control as RadioButton;
            if (radioButton != null)
            {
                radioButton.BackColor = Drawing.Color.Transparent;
                radioButton.ForeColor = PrimaryText;
                return;
            }

            NumericUpDown numericUpDown = control as NumericUpDown;
            if (numericUpDown != null)
            {
                numericUpDown.BackColor = InputBackground;
                numericUpDown.ForeColor = PrimaryText;
                numericUpDown.BorderStyle = BorderStyle.FixedSingle;
                return;
            }

            ListBox listBox = control as ListBox;
            if (listBox != null)
            {
                listBox.BackColor = InputBackground;
                listBox.ForeColor = PrimaryText;
                listBox.BorderStyle = BorderStyle.FixedSingle;
                return;
            }

            TreeView treeView = control as TreeView;
            if (treeView != null)
            {
                treeView.BackColor = InputBackground;
                treeView.ForeColor = PrimaryText;
                treeView.LineColor = Border;
                treeView.BorderStyle = BorderStyle.FixedSingle;
                return;
            }

            GroupBox groupBox = control as GroupBox;
            if (groupBox != null)
            {
                if (!IsPluginSurface(groupBox.BackColor))
                    groupBox.BackColor = GetInheritedSurface(groupBox, WindowBackground);
                groupBox.ForeColor = PrimaryText;
                return;
            }

            TableLayoutPanel tableLayout = control as TableLayoutPanel;
            if (tableLayout != null)
            {
                if (!IsPluginSurface(tableLayout.BackColor))
                    tableLayout.BackColor = Drawing.Color.Transparent;
                tableLayout.ForeColor = PrimaryText;
                return;
            }

            FlowLayoutPanel flowLayout = control as FlowLayoutPanel;
            if (flowLayout != null)
            {
                if (!IsPluginSurface(flowLayout.BackColor))
                    flowLayout.BackColor = GetInheritedSurface(flowLayout, WindowBackground);
                flowLayout.ForeColor = PrimaryText;
                return;
            }

            Panel panel = control as Panel;
            if (panel != null)
            {
                if (!IsPluginSurface(panel.BackColor))
                    panel.BackColor = GetInheritedSurface(panel, Surface);
                panel.ForeColor = PrimaryText;
                return;
            }

            control.ForeColor = PrimaryText;
        }

        private static bool IsPluginSurface(Drawing.Color color)
        {
            int value = color.ToArgb();
            return value == WindowBackground.ToArgb() ||
                   value == Surface.ToArgb() ||
                   value == RaisedSurface.ToArgb() ||
                   value == InputBackground.ToArgb() ||
                   value == UserMessage.ToArgb() ||
                   value == AssistantMessage.ToArgb() ||
                   value == ErrorMessage.ToArgb() ||
                   value == SafetyBanner.ToArgb();
        }

        private static Drawing.Color GetInheritedSurface(Control control, Drawing.Color fallback)
        {
            Control parent = control == null ? null : control.Parent;
            while (parent != null)
            {
                if (IsPluginSurface(parent.BackColor))
                    return parent.BackColor;
                parent = parent.Parent;
            }
            return fallback;
        }

        private static void ApplyButtonColors(Button button, PluginButtonStyle style)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.UseVisualStyleBackColor = false;
            button.FlatAppearance.BorderSize = 1;

            if (!button.Enabled)
            {
                button.BackColor = DisabledBackground;
                button.ForeColor = DisabledText;
                button.FlatAppearance.BorderColor = Border;
                button.FlatAppearance.MouseOverBackColor = DisabledBackground;
                button.FlatAppearance.MouseDownBackColor = DisabledBackground;
                button.Invalidate();
                return;
            }

            if (style == PluginButtonStyle.Primary)
            {
                button.BackColor = Accent;
                button.ForeColor = PrimaryText;
                button.FlatAppearance.BorderColor = SelectionOutline;
                button.FlatAppearance.MouseOverBackColor = AccentHover;
                button.FlatAppearance.MouseDownBackColor = AccentPressed;
            }
            else if (style == PluginButtonStyle.Danger)
            {
                button.BackColor = Danger;
                button.ForeColor = PrimaryText;
                button.FlatAppearance.BorderColor = Drawing.Color.FromArgb(232, 105, 116);
                button.FlatAppearance.MouseOverBackColor = DangerHover;
                button.FlatAppearance.MouseDownBackColor = DangerPressed;
            }
            else
            {
                button.BackColor = RaisedSurface;
                button.ForeColor = PrimaryText;
                button.FlatAppearance.BorderColor = Border;
                button.FlatAppearance.MouseOverBackColor = Drawing.Color.FromArgb(51, 57, 70);
                button.FlatAppearance.MouseDownBackColor = Drawing.Color.FromArgb(28, 31, 39);
            }
            button.Invalidate();
        }

        private static void ThemedButtonEnabledChanged(object sender, EventArgs e)
        {
            Button button = sender as Button;
            ButtonThemeState state;
            if (button != null && ButtonStates.TryGetValue(button, out state))
                ApplyButtonColors(button, state.Style);
        }

        private static void ApplyTabControl(TabControl tabs)
        {
            tabs.BackColor = WindowBackground;
            tabs.ForeColor = PrimaryText;

            // A native TabControl can keep the compact comctl32 default height
            // when the host applies its theme before the plugin window handle is
            // created. Give every language enough vertical room on the first
            // frame; the width remains automatically calculated from each title.
            Drawing.Size itemSize = tabs.ItemSize;
            int minimumHeight = tabs.Font.Height + 8;
            if (itemSize.Height < minimumHeight)
                tabs.ItemSize = new Drawing.Size(Math.Max(1, itemSize.Width), minimumHeight);

            ThemeMarker marker;
            if (!ThemedTabs.TryGetValue(tabs, out marker))
            {
                marker = new ThemeMarker();
                ThemedTabs.Add(tabs, marker);
                tabs.DrawItem += DrawTabItem;
                tabs.HandleCreated += ThemedTabControlHandleCreated;
            }

            tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
            QueueTabHeaderRefresh(tabs, marker);
        }

        private static void ThemedTabControlHandleCreated(object sender, EventArgs e)
        {
            TabControl tabs = sender as TabControl;
            ThemeMarker marker;
            if (tabs != null && ThemedTabs.TryGetValue(tabs, out marker))
                QueueTabHeaderRefresh(tabs, marker);
        }

        private static void QueueTabHeaderRefresh(TabControl tabs, ThemeMarker marker)
        {
            if (tabs == null || marker == null || tabs.IsDisposed || tabs.Disposing ||
                !tabs.IsHandleCreated || marker.RefreshQueued)
                return;

            marker.RefreshQueued = true;
            try
            {
                tabs.BeginInvoke((MethodInvoker)delegate
                {
                    marker.RefreshQueued = false;
                    if (tabs.IsDisposed || tabs.Disposing || !tabs.IsHandleCreated)
                        return;

                    tabs.PerformLayout();
                    tabs.Invalidate(true);
                    tabs.Update();
                });
            }
            catch (InvalidOperationException)
            {
                marker.RefreshQueued = false;
            }
        }

        private static void DrawTabItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabs = sender as TabControl;
            if (tabs == null || e.Index < 0 || e.Index >= tabs.TabPages.Count)
                return;

            PaintTabItem(tabs, e.Graphics, e.Index, e.Bounds);
        }

        internal static void PaintTabControl(TabControl tabs, Drawing.Graphics graphics)
        {
            if (tabs == null || graphics == null || tabs.IsDisposed)
                return;

            PaintTabControlBackground(tabs, graphics);
            for (int index = 0; index < tabs.TabPages.Count; index++)
            {
                Drawing.Rectangle bounds = tabs.GetTabRect(index);
                if (bounds.Width > 0 && bounds.Height > 0)
                    PaintTabItem(tabs, graphics, index, bounds);
            }
        }

        internal static void PaintTabControlBackground(
            TabControl tabs, Drawing.Graphics graphics)
        {
            if (tabs == null || graphics == null || tabs.IsDisposed)
                return;

            Drawing.Rectangle client = tabs.ClientRectangle;
            if (client.Width <= 0 || client.Height <= 0)
                return;

            Drawing.Rectangle page = tabs.DisplayRectangle;
            int topHeight = Math.Max(tabs.Font.Height + 8, page.Top);
            for (int index = 0; index < tabs.TabPages.Count; index++)
                topHeight = Math.Max(topHeight, tabs.GetTabRect(index).Bottom);
            topHeight = Math.Min(client.Height, Math.Max(0, topHeight));

            using (var backgroundBrush = new Drawing.SolidBrush(WindowBackground))
            {
                if (topHeight > 0)
                    graphics.FillRectangle(backgroundBrush,
                        new Drawing.Rectangle(0, 0, client.Width, topHeight));

                int pageTop = Math.Max(topHeight, page.Top);
                int pageBottom = Math.Min(client.Height, page.Bottom);
                if (page.Left > 0 && pageBottom > pageTop)
                    graphics.FillRectangle(backgroundBrush,
                        new Drawing.Rectangle(0, pageTop, page.Left, pageBottom - pageTop));
                if (page.Right < client.Width && pageBottom > pageTop)
                    graphics.FillRectangle(backgroundBrush,
                        new Drawing.Rectangle(page.Right, pageTop,
                            client.Width - page.Right, pageBottom - pageTop));
                if (page.Bottom < client.Height)
                    graphics.FillRectangle(backgroundBrush,
                        new Drawing.Rectangle(0, page.Bottom,
                            client.Width, client.Height - page.Bottom));
            }
        }

        private static void PaintTabItem(
            TabControl tabs,
            Drawing.Graphics graphics,
            int index,
            Drawing.Rectangle bounds)
        {
            if (tabs == null || graphics == null || index < 0 || index >= tabs.TabPages.Count)
                return;

            bool selected = index == tabs.SelectedIndex;
            Drawing.Color background = selected ? RaisedSurface : WindowBackground;
            Drawing.Color foreground = tabs.Enabled ? PrimaryText : DisabledText;
            using (var backgroundBrush = new Drawing.SolidBrush(background))
                graphics.FillRectangle(backgroundBrush, bounds);

            Drawing.Rectangle textBounds = new Drawing.Rectangle(
                bounds.X + 8, bounds.Y + 2, Math.Max(1, bounds.Width - 16), Math.Max(1, bounds.Height - 4));
            TextRenderer.DrawText(
                graphics,
                tabs.TabPages[index].Text,
                tabs.Font,
                textBounds,
                foreground,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            using (var borderPen = new Drawing.Pen(Border))
                graphics.DrawRectangle(borderPen, bounds.X, bounds.Y,
                    Math.Max(0, bounds.Width - 1), Math.Max(0, bounds.Height - 1));
            if (selected && bounds.Height >= 3)
            {
                using (var accentBrush = new Drawing.SolidBrush(SelectionOutline))
                    graphics.FillRectangle(accentBrush, bounds.X + 1,
                        bounds.Bottom - 3, Math.Max(1, bounds.Width - 2), 3);
            }
            if (selected && tabs.Focused && bounds.Width >= 6 && bounds.Height >= 6)
            {
                using (var focusPen = new Drawing.Pen(SelectionOutline))
                    graphics.DrawRectangle(focusPen, bounds.X + 2, bounds.Y + 2,
                        Math.Max(1, bounds.Width - 5), Math.Max(1, bounds.Height - 6));
            }
        }

        private static void ApplyComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = InputBackground;
            comboBox.ForeColor = PrimaryText;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox.ItemHeight = Math.Max(comboBox.ItemHeight, comboBox.Font.Height + 6);

            ThemeMarker marker;
            if (!ThemedComboBoxes.TryGetValue(comboBox, out marker))
            {
                ThemedComboBoxes.Add(comboBox, new ThemeMarker());
                comboBox.DrawItem += DrawComboBoxItem;
            }
            comboBox.Invalidate();
        }

        private static void DrawComboBoxItem(object sender, DrawItemEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null)
                return;

            bool editPortion = (e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit;
            bool selected = !editPortion &&
                (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Drawing.Color background = selected ? SelectionBackground : InputBackground;
            Drawing.Color foreground = comboBox.Enabled ? PrimaryText : DisabledText;
            using (var backgroundBrush = new Drawing.SolidBrush(background))
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

            int index = e.Index >= 0 ? e.Index : comboBox.SelectedIndex;
            string text = index >= 0 && index < comboBox.Items.Count
                ? comboBox.GetItemText(comboBox.Items[index])
                : comboBox.Text;
            Drawing.Rectangle textBounds = new Drawing.Rectangle(
                e.Bounds.X + 6, e.Bounds.Y, Math.Max(1, e.Bounds.Width - 10), e.Bounds.Height);
            TextRenderer.DrawText(
                e.Graphics,
                text,
                comboBox.Font,
                textBounds,
                foreground,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                e.DrawFocusRectangle();
        }

        private static void ApplyDataGridView(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = WindowBackground;
            grid.ForeColor = PrimaryText;
            grid.GridColor = Border;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            ApplyGridCellStyle(grid.DefaultCellStyle, Surface, PrimaryText, SelectionBackground, PrimaryText);
            ApplyGridCellStyle(grid.RowsDefaultCellStyle, Surface, PrimaryText, SelectionBackground, PrimaryText);
            ApplyGridCellStyle(grid.AlternatingRowsDefaultCellStyle, RaisedSurface, PrimaryText, SelectionBackground, PrimaryText);
            ApplyGridCellStyle(grid.ColumnHeadersDefaultCellStyle, RaisedSurface, PrimaryText,
                RaisedSurface, PrimaryText);
            ApplyGridCellStyle(grid.RowHeadersDefaultCellStyle, RaisedSurface, PrimaryText,
                RaisedSurface, PrimaryText);
            grid.Invalidate();
        }

        private static void ApplyGridCellStyle(
            DataGridViewCellStyle style,
            Drawing.Color background,
            Drawing.Color foreground,
            Drawing.Color selectionBackground,
            Drawing.Color selectionForeground)
        {
            style.BackColor = background;
            style.ForeColor = foreground;
            style.SelectionBackColor = selectionBackground;
            style.SelectionForeColor = selectionForeground;
        }
    }
}
