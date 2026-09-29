using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Small modal used by the Flight Planner when the mission type dropdown is set to ALL.
    /// Lets the user pick which of Mission / Fence / Rally should be read from or written to
    /// the vehicle. Everything is ticked by default so "all" is a single click.
    /// </summary>
    public class MissionTypePicker : Form
    {
        private static readonly MAVLink.MAV_MISSION_TYPE[] order =
        {
            MAVLink.MAV_MISSION_TYPE.MISSION,
            MAVLink.MAV_MISSION_TYPE.FENCE,
            MAVLink.MAV_MISSION_TYPE.RALLY
        };

        private readonly Dictionary<MAVLink.MAV_MISSION_TYPE, CheckBox> boxes =
            new Dictionary<MAVLink.MAV_MISSION_TYPE, CheckBox>();

        private readonly MyButton butOk;

        /// <summary>
        /// Show the picker.
        /// </summary>
        /// <param name="owner">owning window</param>
        /// <param name="write">true when writing to the vehicle, false when reading from it
        /// (sets the title, prompt and OK button)</param>
        /// <param name="counts">optional item count per type, shown next to each checkbox</param>
        /// <returns>selected types in upload order (mission, fence, rally), or null if cancelled</returns>
        public static List<MAVLink.MAV_MISSION_TYPE> Show(IWin32Window owner, bool write,
            IDictionary<MAVLink.MAV_MISSION_TYPE, int> counts = null)
        {
            using (var dlg = new MissionTypePicker(write, counts))
            {
                if (dlg.ShowDialog(owner) != DialogResult.OK)
                    return null;

                return order.Where(t => dlg.boxes[t].Checked).ToList();
            }
        }

        private MissionTypePicker(bool write, IDictionary<MAVLink.MAV_MISSION_TYPE, int> counts)
        {
            Text = write ? Strings.WriteWhichItems : Strings.ReadWhichItems;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(12);

            var layout = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Dock = DockStyle.Fill
            };

            var label = new Label
            {
                AutoSize = true,
                Text = write ? Strings.WriteItemTypesPrompt : Strings.ReadItemTypesPrompt,
                Margin = new Padding(0, 0, 0, 8)
            };
            layout.Controls.Add(label);

            foreach (var type in order)
            {
                var text = TypeName(type);
                if (counts != null && counts.TryGetValue(type, out var n))
                    text += " (" + (n == 1 ? Strings.ItemCountOne : string.Format(Strings.ItemCountMany, n)) + ")";

                var cb = new CheckBox
                {
                    AutoSize = true,
                    Checked = true,
                    Text = text,
                    Margin = new Padding(8, 2, 0, 2)
                };
                cb.CheckedChanged += (s, e) => UpdateOkState();
                boxes[type] = cb;
                layout.Controls.Add(cb);
            }

            var buttons = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 12, 0, 0)
            };

            var butCancel = new MyButton {Text = Strings.Cancel, DialogResult = DialogResult.Cancel, Width = 75};
            butOk = new MyButton
            {
                Text = write ? Strings.Write : Strings.Read, DialogResult = DialogResult.OK, Width = 75
            };

            buttons.Controls.Add(butCancel);
            buttons.Controls.Add(butOk);
            layout.Controls.Add(buttons);

            Controls.Add(layout);

            AcceptButton = butOk;
            CancelButton = butCancel;

            ThemeManager.ApplyThemeTo(this);
        }

        private void UpdateOkState()
        {
            butOk.Enabled = boxes.Values.Any(b => b.Checked);
        }

        /// <summary>
        /// The localized name of an item type, as shown in the picker.
        /// </summary>
        public static string TypeName(MAVLink.MAV_MISSION_TYPE type)
        {
            switch (type)
            {
                case MAVLink.MAV_MISSION_TYPE.MISSION:
                    return Strings.MissionTypeMission;
                case MAVLink.MAV_MISSION_TYPE.FENCE:
                    return Strings.MissionTypeFence;
                case MAVLink.MAV_MISSION_TYPE.RALLY:
                    return Strings.MissionTypeRally;
                default:
                    return type.ToString();
            }
        }
    }
}
