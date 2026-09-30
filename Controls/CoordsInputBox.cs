using GeoUtility.GeoSystem;
using MissionPlanner.Utilities;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Asks the user for a location, letting them pick the coordinate frame (GEO lat/long,
    /// UTM or MGRS) and an optional altitude. Also hosts the shared text conversions used by the
    /// Plan page home location box.
    /// </summary>
    public class CoordsInputBox : Form
    {
        private readonly ComboBox cmbSystem;
        private readonly Label lblHint;
        private readonly TextBox txtCoords;
        private readonly TextBox txtAlt;
        private readonly MyButton butOk;

        private PointLatLngAlt result;
        private bool resultHasAlt;

        /// <summary>
        /// Show the dialog.
        /// </summary>
        /// <param name="owner">owning window</param>
        /// <param name="title">window title</param>
        /// <param name="defaultSystem">Coords.CoordsSystems name to preselect (GEO, UTM, MGRS)</param>
        /// <param name="hasAlt">true when the user typed an altitude</param>
        /// <returns>the location (Alt = typed altitude or 0), or null when cancelled</returns>
        public static PointLatLngAlt Show(IWin32Window owner, string title, string defaultSystem, out bool hasAlt)
        {
            using (var dlg = new CoordsInputBox(title, defaultSystem))
            {
                if (dlg.ShowDialog(owner) != DialogResult.OK)
                {
                    hasAlt = false;
                    return null;
                }

                hasAlt = dlg.resultHasAlt;
                return dlg.result;
            }
        }

        private CoordsInputBox(string title, string defaultSystem)
        {
            Text = title;
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
                ColumnCount = 2,
                Dock = DockStyle.Fill
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // row 0: coordinate system
            layout.Controls.Add(new Label {Text = "Coordinate system", AutoSize = true, Anchor = AnchorStyles.Left}, 0, 0);
            cmbSystem = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            };
            cmbSystem.Items.AddRange(Enum.GetNames(typeof(Coords.CoordsSystems)).Cast<object>().ToArray());
            cmbSystem.SelectedItem = cmbSystem.Items.Contains(defaultSystem ?? "")
                ? defaultSystem
                : Coords.CoordsSystems.GEO.ToString();
            cmbSystem.SelectedIndexChanged += (s, e) => UpdateHint();
            layout.Controls.Add(cmbSystem, 1, 0);

            // row 1: hint
            lblHint = new Label {AutoSize = true, Margin = new Padding(3, 6, 3, 6)};
            layout.Controls.Add(lblHint, 0, 1);
            layout.SetColumnSpan(lblHint, 2);

            // row 2: coordinates
            layout.Controls.Add(new Label {Text = "Coordinates", AutoSize = true, Anchor = AnchorStyles.Left}, 0, 2);
            txtCoords = new TextBox {Width = 260};
            layout.Controls.Add(txtCoords, 1, 2);

            // row 3: altitude
            layout.Controls.Add(
                new Label
                {
                    Text = "Altitude (optional, " + CurrentState.AltUnit + ")", AutoSize = true,
                    Anchor = AnchorStyles.Left
                }, 0, 3);
            txtAlt = new TextBox {Width = 100};
            layout.Controls.Add(txtAlt, 1, 3);

            // row 4: buttons
            var buttons = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 12, 0, 0)
            };
            var butCancel = new MyButton {Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 75};
            butOk = new MyButton {Text = "OK", Width = 75};
            butOk.Click += ButOk_Click;
            buttons.Controls.Add(butCancel);
            buttons.Controls.Add(butOk);
            layout.Controls.Add(buttons, 0, 4);
            layout.SetColumnSpan(buttons, 2);

            Controls.Add(layout);

            AcceptButton = butOk;
            CancelButton = butCancel;

            UpdateHint();

            ThemeManager.ApplyThemeTo(this);
        }

        private string SelectedSystem => cmbSystem.SelectedItem?.ToString() ?? Coords.CoordsSystems.GEO.ToString();

        private void UpdateHint()
        {
            if (SelectedSystem == Coords.CoordsSystems.UTM.ToString())
                lblHint.Text = "Zone+band, easting and northing in metres, eg 29U 540660 5854629";
            else if (SelectedSystem == Coords.CoordsSystems.MGRS.ToString())
                lblHint.Text = "MGRS grid reference, eg 29UPU0406654629 (spaces allowed)";
            else
                lblHint.Text = "Decimal degrees 'lat;long' or 'lat;long;alt', eg 52.829;-7.470";
        }

        private void ButOk_Click(object sender, EventArgs e)
        {
            if (!TryParse(SelectedSystem, txtCoords.Text, out var lat, out var lng, out var inlineAlt))
            {
                CustomMessageBox.Show("Invalid " + SelectedSystem + " coordinate: " + txtCoords.Text.Trim(),
                    Strings.ERROR);
                return;
            }

            double? alt = inlineAlt;
            if (txtAlt.Text.Trim() != "")
            {
                if (!double.TryParse(txtAlt.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture,
                        out var typedAlt))
                {
                    CustomMessageBox.Show("Invalid altitude: " + txtAlt.Text.Trim(), Strings.ERROR);
                    return;
                }

                alt = typedAlt;
            }

            result = new PointLatLngAlt(lat, lng, alt ?? 0);
            resultHasAlt = alt.HasValue;
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// A location as text in the given coordinate system. Empty when the location is not
        /// valid, is outside the UTM/MGRS grid, or the system is unknown.
        /// </summary>
        /// <param name="system">a Coords.CoordsSystems name</param>
        public static string Format(string system, double lat, double lng)
        {
            if (lat == 0 && lng == 0)
                return "";

            if (system == Coords.CoordsSystems.GEO.ToString())
                return lat.ToString("0.0000000", CultureInfo.InvariantCulture) + ";" +
                       lng.ToString("0.0000000", CultureInfo.InvariantCulture);

            // outside the UTM/MGRS grid
            if (lat > 84 || lat < -80 || lng >= 180 || lng <= -180)
                return "";

            try
            {
                var point = new Geographic(lng, lat);

                if (system == Coords.CoordsSystems.MGRS.ToString())
                {
                    var mgrs = (MGRS) point;
                    mgrs.Precision = 5;
                    return mgrs.ToString();
                }

                if (system == Coords.CoordsSystems.UTM.ToString())
                {
                    var utm = (UTM) point;
                    // whole metres, same as the mouse position display
                    return utm.Zoneband + " " + Math.Round(utm.East) + " " + Math.Round(utm.North);
                }
            }
            catch
            {
            }

            return "";
        }

        /// <summary>
        /// Parse a location typed in the given coordinate system.
        /// GEO: "lat;long" or "lat;long;alt" (also comma or space separated).
        /// UTM: "zone+band east north", eg "29U 540660 5854629", optional trailing alt.
        /// MGRS: compact grid reference, spaces ignored, eg "29UPU0406654629".
        /// </summary>
        /// <param name="alt">altitude typed after the coordinates, if any</param>
        public static bool TryParse(string system, string text, out double lat, out double lng, out double? alt)
        {
            lat = 0;
            lng = 0;
            alt = null;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            try
            {
                var parts = text.Trim().ToUpperInvariant()
                    .Split(new[] {' ', ';', ',', '\t'}, StringSplitOptions.RemoveEmptyEntries);

                if (system == Coords.CoordsSystems.GEO.ToString())
                {
                    if (parts.Length != 2 && parts.Length != 3)
                        return false;

                    lat = double.Parse(parts[0], CultureInfo.InvariantCulture);
                    lng = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    if (parts.Length == 3)
                        alt = double.Parse(parts[2], CultureInfo.InvariantCulture);

                    return lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180;
                }

                Geographic geo = null;

                if (system == Coords.CoordsSystems.MGRS.ToString())
                {
                    // MGRS itself never contains spaces, so everything is one token
                    geo = (Geographic) new MGRS(string.Concat(parts));
                }
                else if (system == Coords.CoordsSystems.UTM.ToString())
                {
                    if (parts.Length != 3 && parts.Length != 4)
                        return false;

                    var zoneband = Regex.Match(parts[0], @"^(\d{1,2})([A-Z])$");
                    if (!zoneband.Success)
                        return false;

                    // GeoUtility parses whole metres only
                    var east = Math.Round(double.Parse(parts[1], CultureInfo.InvariantCulture))
                        .ToString(CultureInfo.InvariantCulture);
                    var north = Math.Round(double.Parse(parts[2], CultureInfo.InvariantCulture))
                        .ToString(CultureInfo.InvariantCulture);

                    if (!UTM.TryParse(zoneband.Groups[1].Value, zoneband.Groups[2].Value, east, north,
                            out UTM utm, out string error, out var valid))
                        return false;

                    geo = (Geographic) utm;

                    if (parts.Length == 4)
                        alt = double.Parse(parts[3], CultureInfo.InvariantCulture);
                }

                if (geo == null)
                    return false;

                lat = geo.Latitude;
                lng = geo.Longitude;
                return !(lat == 0 && lng == 0);
            }
            catch
            {
                return false;
            }
        }
    }
}
