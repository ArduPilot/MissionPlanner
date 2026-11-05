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

        /// <summary>
        /// Ask for a POI position and add it. POI altitudes are metres: a typed altitude is taken
        /// in the display unit, otherwise the POI gets the ground height at its own position.
        /// </summary>
        /// <param name="owner">owning window</param>
        /// <param name="defaultSystem">Coords.CoordsSystems name to preselect (GEO, UTM, MGRS)</param>
        public static void AddPOI(IWin32Window owner, string defaultSystem)
        {
            var point = Show(owner, Strings.EnterPOICoords, defaultSystem, out var hasAlt);
            if (point == null)
                return;

            if (hasAlt)
                point.Alt = point.Alt / CurrentState.multiplieralt;
            else
                point.Alt = srtm.getAltitude(point.Lat, point.Lng).alt;

            POI.POIAdd(point);
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
            layout.Controls.Add(new Label {Text = Strings.CoordinateSystem, AutoSize = true, Anchor = AnchorStyles.Left}, 0, 0);
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
            layout.Controls.Add(new Label {Text = Strings.Coordinates, AutoSize = true, Anchor = AnchorStyles.Left}, 0, 2);
            txtCoords = new TextBox {Width = 260};
            layout.Controls.Add(txtCoords, 1, 2);

            // row 3: altitude
            layout.Controls.Add(
                new Label
                {
                    Text = string.Format(Strings.AltitudeOptional, CurrentState.AltUnit), AutoSize = true,
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
            var butCancel = new MyButton {Text = Strings.Cancel, DialogResult = DialogResult.Cancel, Width = 75};
            butOk = new MyButton {Text = Strings.OK, Width = 75};
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
                lblHint.Text = Strings.CoordsHintUTM;
            else if (SelectedSystem == Coords.CoordsSystems.MGRS.ToString())
                lblHint.Text = Strings.CoordsHintMGRS;
            else
                lblHint.Text = Strings.CoordsHintGEO;
        }

        private void ButOk_Click(object sender, EventArgs e)
        {
            if (!TryParse(SelectedSystem, txtCoords.Text, out var lat, out var lng, out var inlineAlt))
            {
                CustomMessageBox.Show(string.Format(Strings.InvalidCoordinate, SelectedSystem, txtCoords.Text.Trim()),
                    Strings.ERROR);
                return;
            }

            double? alt = inlineAlt;
            if (txtAlt.Text.Trim() != "")
            {
                if (!double.TryParse(txtAlt.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture,
                        out var typedAlt))
                {
                    CustomMessageBox.Show(string.Format(Strings.InvalidAltitude, txtAlt.Text.Trim()), Strings.ERROR);
                    return;
                }

                alt = typedAlt;
            }

            result = new PointLatLngAlt(lat, lng, alt ?? 0);
            resultHasAlt = alt.HasValue;
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// A location as text in the given coordinate system. Empty when the location is outside
        /// the UTM/MGRS grid or the system is unknown. 0, 0 is formatted like any other point;
        /// callers that use it for "not set" check for it themselves.
        /// </summary>
        /// <param name="system">a Coords.CoordsSystems name</param>
        public static string Format(string system, double lat, double lng)
        {
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
        /// GEO: "lat;long" or "lat;long;alt" (also comma or space separated; with a ';' in the text a
        /// comma does not separate, so a decimal comma is refused rather than read as a separator).
        /// UTM: "zone+band east north", eg "29U 540660 5854629", optional trailing alt. The letter is
        /// the latitude band, and a band that does not hold the latitude the numbers give is refused.
        /// MGRS: compact grid reference, spaces ignored, eg "29UPU0406654629".
        /// Numbers use a decimal point and no thousands separators.
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
                var trimmed = text.Trim().ToUpperInvariant();
                // "52,5;7" is a decimal comma, not lat 52, long 5, alt 7
                var separators = trimmed.Contains(";") ? new[] {' ', ';', '\t'} : new[] {' ', ';', ',', '\t'};
                var parts = trimmed.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                if (system == Coords.CoordsSystems.GEO.ToString())
                {
                    if (parts.Length != 2 && parts.Length != 3)
                        return false;

                    lat = ParseNumber(parts[0]);
                    lng = ParseNumber(parts[1]);
                    if (parts.Length == 3)
                        alt = ParseNumber(parts[2]);

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
                    var east = Math.Round(ParseNumber(parts[1])).ToString(CultureInfo.InvariantCulture);
                    var north = Math.Round(ParseNumber(parts[2])).ToString(CultureInfo.InvariantCulture);

                    if (!UTM.TryParse(zoneband.Groups[1].Value, zoneband.Groups[2].Value, east, north,
                            out UTM utm, out string error, out var valid))
                        return false;

                    geo = (Geographic) utm;

                    // GeoUtility takes the hemisphere from the band letter alone (N and after is
                    // north) and accepts any northing there, so "56S 334000 6252000" typed with S for
                    // south lands near 56 degrees north instead of at Sydney. Refuse a band that does
                    // not hold the latitude the numbers give.
                    if (!BandHolds(zoneband.Groups[2].Value[0], geo.Latitude))
                        return false;

                    if (parts.Length == 4)
                        alt = ParseNumber(parts[3]);
                }

                if (geo == null)
                    return false;

                lat = geo.Latitude;
                lng = geo.Longitude;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// A number as typed: decimal point, no thousands separators, so "1,5" is refused rather
        /// than read as 15.
        /// </summary>
        private static double ParseNumber(string text)
        {
            return double.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Whether a UTM latitude band (C to X without I and O, 8 degrees each from 80 S, X 12
        /// degrees) holds the latitude, with a little slack at the band edges for a point rounded to
        /// whole metres.
        /// </summary>
        internal static bool BandHolds(char band, double lat)
        {
            const string bands = "CDEFGHJKLMNPQRSTUVWX";
            var i = bands.IndexOf(char.ToUpperInvariant(band));
            if (i < 0)
                return false;

            const double slack = 0.01;
            var south = -80 + i * 8.0;
            var north = band == 'X' ? 84 : south + 8;
            return lat >= south - slack && lat <= north + slack;
        }
    }
}
