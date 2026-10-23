using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// A column header that is itself a checkbox. Clicking it toggles <see cref="Checked"/> and raises
    /// <see cref="CheckedChanged"/>; the grid's owner decides what that means for the rows (the Plan
    /// page ticks or clears every row of the column). <see cref="SetChecked"/> updates the box without
    /// raising the event, for keeping it in step with the rows.
    /// </summary>
    public class CheckBoxHeaderCell : DataGridViewColumnHeaderCell
    {
        public bool Checked { get; private set; }

        public event EventHandler CheckedChanged;

        public void SetChecked(bool value)
        {
            if (Checked == value)
                return;
            Checked = value;
            DataGridView?.InvalidateCell(this);
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates dataGridViewElementState, object value, object formattedValue,
            string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue,
                errorText, cellStyle, advancedBorderStyle,
                paintParts & ~DataGridViewPaintParts.ContentForeground); // the box, not the header text

            // the box on the left, the header text (for example "All") beside it
            var state = Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
            var size = CheckBoxRenderer.GetGlyphSize(graphics, state);
            var at = new Point(cellBounds.X + 4, cellBounds.Y + (cellBounds.Height - size.Height) / 2);
            CheckBoxRenderer.DrawCheckBox(graphics, at, state);

            var text = formattedValue?.ToString();
            if (!string.IsNullOrEmpty(text) && cellStyle != null)
            {
                var textBounds = new Rectangle(at.X + size.Width + 3, cellBounds.Y,
                    cellBounds.Right - (at.X + size.Width + 3), cellBounds.Height);
                TextRenderer.DrawText(graphics, text, cellStyle.Font, textBounds, cellStyle.ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button != MouseButtons.Left)
                return;

            Checked = !Checked;
            DataGridView?.InvalidateCell(this);
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
