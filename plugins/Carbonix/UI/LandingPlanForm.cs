using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner;
using MissionPlanner.Utilities;


namespace Carbonix
{
    public partial class LandingPlanForm : Form
    {
        const double rad2deg = (180 / Math.PI);
        const double deg2rad = (1.0 / rad2deg);

        readonly private CarbonixPlugin plugin;

        readonly GMapOverlay layer_route;
        readonly GMapOverlay layer_small_markers;

        // Prevent control change handlers from running when we are updating the UI
        private bool freeze_handlers = true;

        private readonly AircraftSettings aircraft_settings;
        private double loiter_radius;
        private List<AircraftSettings.Point> approach_points;
        private double wind_direction;
        private double loit_exit_dist;
        private double min_exit_dist;
        private double loit_exit_alt;
        private double alt_offset = 0;
        private double grade;

        PointLatLngAlt land_point;
        PointLatLngAlt loiter_point;
        GMapMarker land_marker;
        GMapMarker loiter_marker;
        GMapMarkerRect loiter_circle;
        public LandingPlanForm(CarbonixPlugin plugin, AircraftSettings aircraft_settings)
        {
            this.plugin = plugin;

            InitializeComponent();

            // We keep another copy of the wind_direction as a double to 
            // keep the click and drag from feeling too "choppy"
            wind_direction = (double)num_winddir.Value;

            this.aircraft_settings = aircraft_settings;

            map.MapProvider = plugin.Host.FDMapType;

            layer_route = new GMapOverlay("strip");
            layer_small_markers = new GMapOverlay("small_markers");
            map.Overlays.Add(layer_route);
            map.Overlays.Add(layer_small_markers);

            // Map Events
            map.OnMarkerEnter += new MarkerEnter(map_OnMarkerEnter);
            map.OnMarkerLeave += new MarkerLeave(map_OnMarkerLeave);
            map.MouseUp += new MouseEventHandler(map_MouseUp);
            map.MouseDown += new MouseEventHandler(this.map_MouseDown);
            map.MouseMove += new MouseEventHandler(this.map_MouseMove);

            // Unit settings
            lbl_alt_unit.Text = CurrentState.AltUnit;
            lbl_alt_unit2.Text = CurrentState.AltUnit;
            lbl_alt_unit3.Text = CurrentState.AltUnit;
            lbl_dist_unit.Text = CurrentState.DistanceUnit;
            num_vtolalt.Increment = (CurrentState.AltUnit == "ft") ? 10 : 5;
            num_loitrad.Increment = (CurrentState.DistanceUnit == "ft") ? 50 : 10;
            num_exitalt.Increment = (CurrentState.AltUnit == "ft") ? 50 : 10;
            num_transit_alt.Increment = (CurrentState.AltUnit == "ft") ? 100 : 50;
        }

        void loadsettings()
        {
            // Get min/max VTOL altitude
            num_vtolalt.Minimum = (decimal)Math.Round(CurrentState.toAltDisplayUnit(aircraft_settings.min_vtol_altitude), 0);
            num_vtolalt.Maximum = (decimal)Math.Round(CurrentState.toAltDisplayUnit(aircraft_settings.max_vtol_altitude), 0);

            // Get minimum loiter radius
            num_loitrad.Minimum = (decimal)Math.Round(CurrentState.toDistDisplayUnit(aircraft_settings.loitradius_min), 0);
            // Set maximum loiter radius to 2550m (ArduPilot limit)
            num_loitrad.Maximum = (decimal)Math.Round(CurrentState.toDistDisplayUnit(Math.Min(aircraft_settings.loitradius_max, 2550)), 0);

            // Get default transit altitude
            decimal default_alt = Math.Round(decimal.Parse(plugin.Host.MainForm.FlightPlanner.TXT_DefaultAlt.Text, CultureInfo.InvariantCulture), 0);
            num_transit_alt.Value = Math.Max(default_alt, num_transit_alt.Minimum);

            // Get default loiter time
            num_loitertimemin.Value = aircraft_settings.landing_hold_minutes;

            // This will load the default approach settings into the relevant
            // controls and then unfreeze the handlers for the first time
            load_default_approach();

        }

        // Load the map control, load settings, set initial values and draw the landing
        private void LandingPlanUI_Load(object sender, EventArgs e)
        {
            // Center the map on the FlightPlanner map
            map.Position = plugin.Host.FPGMapControl.Position;

            // Create landing marker
            land_point = map.Position;
            land_marker = new GMarkerGoogle(land_point, GMarkerGoogleType.green_dot)
            {
                ToolTipMode = MarkerTooltipMode.OnMouseOver,
                ToolTipText = "Landing Point",
                Tag = "Land"
            };
            // Create loiter marker
            loiter_point = land_point.newpos(185, 3000);
            loiter_marker = new GMarkerGoogle(loiter_point, GMarkerGoogleType.green_dot)
            {
                ToolTipMode = MarkerTooltipMode.OnMouseOver,
                ToolTipText = "Loiter Point",
                Tag = "Loiter"
            };
            loiter_circle = new GMapMarkerRect(loiter_point)
            {
                InnerMarker = loiter_marker,
                wprad = loiter_radius
            };

            layer_route.Markers.Add(land_marker);
            layer_route.Markers.Add(loiter_marker);
            layer_route.Markers.Add(loiter_circle);

            loadsettings();

            // Check if home point is within current map bounds, if so check chk_land_home
            // (This will trigger the handler and move the landing points we just set).
            chk_land_home.Checked = plugin.Host.FPGMapControl.ViewArea.Contains(plugin.Host.cs.PlannedHomeLocation);

            RecalculateMarkers();
            RecenterMap();

        }

        // Calculate new marker positions for setting changes
        void RecalculateMarkers()
        {
            // Recalculate loiter point location to keep same exit point
            var dist = rad_loitcw.Checked ? loiter_radius : -loiter_radius;
            loiter_point = land_point.newpos(wind_direction, -loit_exit_dist).newpos(wind_direction + 90, dist);

            DrawLanding();
        }

        // Calculate approach parameters from controls and map marker locations and draw landing approach on map.
        // This is called when the form is loaded, when a point is dragged, or when a control value is changed.
        void DrawLanding()
        {
            // Prevent handlers from running while we are updating values
            freeze_handlers = true;

            // Calculate distance and bearing from loiter point to land point
            double dist = loiter_point.GetDistance(land_point);
            double bearing = loiter_point.GetBearing(land_point);

            // Calculate wind direction, but only if dragging
            if (isMouseDraging)
            {
                // Calculate difference between bearing of loiter-to-landing and wind angle
                double dtheta = Math.Atan2(loiter_radius, loit_exit_dist) * rad2deg;
                // If the turn is clockwise, we add dtheta to loiter-to-landing bearing to get wind angle,
                // otherwise we subtract it
                if (rad_loitccw.Checked) dtheta *= -1;
                wind_direction = bearing + dtheta;
                // Wrap wind_direction to 0, 360
                wind_direction = Wrap360(wind_direction);

                // Update the wind direction control
                // Wrapping still needed below to handle when wind_direction rounds to 0
                num_winddir.Value = (decimal)Wrap360(Math.Round(wind_direction, num_winddir.DecimalPlaces));
            }

            var N = approach_points.Count;

            // Recalculate the exit point parameters
            // Only recalculate the exit dist if we are dragging. (this prevents numerical 
            // error from addingup when repeatedly chaning the loiter radius control)
            if (isMouseDraging)
            {
                // Calculate exit point parameters
                loit_exit_dist = Math.Sqrt(dist * dist - loiter_radius * loiter_radius);

                // Prevent moving the loiter circle too close
                if (double.IsNaN(loit_exit_dist) || loit_exit_dist < min_exit_dist)
                {
                    loit_exit_dist = min_exit_dist;
                    // recalculate position of loiter_point
                    var radius = rad_loitcw.Checked ? loiter_radius : -loiter_radius;
                    loiter_point = land_point.newpos(wind_direction, -loit_exit_dist).newpos(wind_direction + 90, radius);
                }
            }
            // Snap next WP to loiter exit if it would be within 110% of loiter radius
            // (ArduPilot skips heading check if next WP is within 105%)
            // (the latter check is equivalent to checking if the normalized distance
            //  between exit and next WP is less than ~0.46)
            if ((loit_exit_dist - min_exit_dist) / loiter_radius < 0.46)
            {
                approach_points[N - 2] = new AircraftSettings.Point() { dist = loit_exit_dist, alt = approach_points[N - 2].alt };
            }
            else
            {
                approach_points[N - 2] = new AircraftSettings.Point() { dist = min_exit_dist, alt = approach_points[N - 2].alt };
            }

            // Calculate loit_exit_alt from grade
            loit_exit_alt = (loit_exit_dist - approach_points[N - 2].dist) * grade + approach_points[N - 2].alt;

            // Update last approach point
            approach_points[N - 1] = new AircraftSettings.Point() { dist = loit_exit_dist, alt = loit_exit_alt };

            // Update the loiter altitude control
            double loit_exit_max_alt = (loit_exit_dist - approach_points[N - 2].dist) * aircraft_settings.max_descent_grade + approach_points[N - 2].alt;
            num_exitalt.Maximum = (decimal)Math.Round(CurrentState.toAltDisplayUnit(loit_exit_max_alt + alt_offset));
            num_exitalt.Minimum = (decimal)Math.Round(CurrentState.toAltDisplayUnit(approach_points[N - 2].alt + alt_offset));
            num_exitalt.Value = (decimal)Math.Round(CurrentState.toAltDisplayUnit(loit_exit_alt + alt_offset));
            num_exitalt.Enabled = (num_exitalt.Minimum != num_exitalt.Maximum); // Disable if can't change

            // Draw landing strip route on the map
            layer_route.Routes.Clear();
            var exit_pt = land_point.newpos(wind_direction, -loit_exit_dist);
            List<PointLatLng> list = new List<PointLatLng>
            {
                land_point,
                exit_pt
            };
            GMapRoute poly = new GMapRoute(list, "landing strip")
            {
                Stroke = new Pen(Color.GreenYellow, 2),
            };
            layer_route.Routes.Add(poly);

            // Add small markers the altitudes along the final approach
            layer_small_markers.Markers.Clear();

            // Draw tooltip over invisible marker on circle to indicate transit altitude
            var angle = wind_direction + 90;
            if (rad_loitcw.Checked) angle += 180;
            var pt_lla = loiter_point.newpos(angle, -loiter_radius);
            pt_lla.Alt = (float)num_transit_alt.Value / CurrentState.multiplieralt;
            AddApproachMarker(pt_lla, angle, false);

            // Draw small markers and tooltips for each approach point
            angle += 180; // Flip the tooltips the other way for the others
            foreach (var pt in approach_points)
            {
                pt_lla = land_point.newpos(wind_direction, -pt.dist);
                pt_lla.Alt = pt.alt + alt_offset;
                AddApproachMarker(pt_lla, angle);
            }

            // Update markers
            loiter_marker.Position = loiter_point;
            loiter_circle.Position = loiter_point;
            loiter_circle.wprad = (float)loiter_radius;
            land_marker.Position = land_point;

            // Free the handlers to run again
            freeze_handlers = false;
        }

        // Custom tooltip for altitude markers. Allows for more control over placement
        private class MyToolTip : GMapToolTip
        {
            public MyToolTip(GMapMarker marker) : base(marker)
            {
                Stroke.Width = 2;
                Stroke.Color = Color.FromArgb(150, Stroke.Color.R, Stroke.Color.G, Stroke.Color.B);
            }
            public override void OnRender(IGraphics g)
            {
                Size st = g.MeasureString(Marker.ToolTipText, Font).ToSize();
                Rectangle rect = new Rectangle(Marker.ToolTipPosition.X, Marker.ToolTipPosition.Y - st.Height, st.Width + TextPadding.Width, st.Height + TextPadding.Height);
                rect.Offset(Offset.X, Offset.Y);
                g.DrawLine(Stroke, Marker.ToolTipPosition.X, Marker.ToolTipPosition.Y, rect.X, rect.Y + rect.Height / 2);

                if (Marker.ToolTip.Offset.X < 0)
                {
                    rect.Offset(-rect.Width, 0);
                }

                g.FillRectangle(Fill, rect);
                g.DrawRectangle(Stroke, rect);

#if PocketPC
                rect.Offset(0, (rect.Height - st.Height) / 2);
#endif

                g.DrawString(Marker.ToolTipText, Font, Foreground, rect, Format);
            }
        }

        // Add a small marker with a custom tooltip for displaying altitudes of important points
        // Parameters:
        // point: the lat/lng location to add the marker 
        // angle: the angle at which to draw the tooltip
        // visible: thether the marker should be visible
        private void AddApproachMarker(PointLatLngAlt point, double angle, bool visible = true)
        {
            GMarkerGoogle marker;

            if (visible)
            {
                marker = new GMarkerGoogle(point, GMarkerGoogleType.white_small);
            }
            else
            {
                Bitmap empty = new Bitmap(1, 1);
                empty.MakeTransparent();
                marker = new GMarkerGoogle(point, empty);
            }
            marker.ToolTipText = CurrentState.toAltDisplayUnit(point.Alt).ToString("0") + CurrentState.AltUnit;
            marker.ToolTipMode = chk_showalt.Checked ? MarkerTooltipMode.Always : MarkerTooltipMode.OnMouseOver;

            // Place the marker tooltips 30 pixels away at `angle`
            var tooltip_offset = new Point((int)(-30 * Math.Sin(angle * deg2rad)), (int)(30 * Math.Cos(angle * deg2rad)));
            marker.ToolTip = new MyToolTip(marker.ToolTip.Marker)
            {
                Offset = tooltip_offset
            };
            layer_small_markers.Markers.Add(marker);
        }

        // Wrap to (0, 360]
        private double Wrap360(double angle)
        {
            while (angle > 360) angle -= 360;
            while (angle <= 0) angle += 360;
            return angle;
        }

        // Center on the landing point, and zoom to a level that can see all potential wind angles
        private void RecenterMap()
        {
            // Calculate the zoom level that fits all potential wind angles. We do this from first principals
            // of the definition of Zoom because none of the built-in functions do exactly what we want.
            // Note, this is an approximation; the actual zoom level depends on map projection and "other factors"
            // We zoom out a little bit more as a buffer.
            var dlat = land_point.newpos(0, loit_exit_dist + loiter_radius).Lat - land_point.Lat;
            var zoom = Math.Log(Math.Min(map.Height, map.Width) / 256, 2) - Math.Log(dlat / 180, 2) - 0.5;
            if (zoom < 1) zoom = 1;
            map.Zoom = zoom;

            // Center on the landing point
            map.Position = land_point;
        }

        // --------------------------------------------------
        //                  Event handlers
        // --------------------------------------------------

        internal PointLatLng MouseDownStart = new PointLatLng();
        internal PointLatLng MouseDownEnd;
        GMapMarker CurrentMarker = null;
        bool isMouseDown = false;
        bool isMouseDraging = false;
        static public object thisLock = new object();

        // Clear the current moveable marker if we mouse away from one
        private void map_OnMarkerLeave(GMapMarker item)
        {
            if (!isMouseDown)
            {
                if (ReferenceEquals(item, land_marker) || ReferenceEquals(item, loiter_marker) || ReferenceEquals(item, loiter_circle))
                {
                    CurrentMarker = null;
                }
            }
        }

        // Set the current moveable marker if we mouse over one
        private void map_OnMarkerEnter(GMapMarker item)
        {
            if (!isMouseDown)
            {
                if (ReferenceEquals(item, land_marker) || ReferenceEquals(item, loiter_marker) || ReferenceEquals(item, loiter_circle))
                {
                    CurrentMarker = item;
                }
            }
        }

        // Stop the drag when we release the mouse
        private void map_MouseUp(object sender, MouseEventArgs e)
        {
            MouseDownEnd = map.FromLocalToLatLng(e.X, e.Y);

            if (e.Button != MouseButtons.Left) // ignore right clicks etc.
            {
                return;
            }

            if (isMouseDown) // mouse down on some other object and dragged to here.
            {
                if (e.Button == MouseButtons.Left)
                {
                    isMouseDown = false;
                }
            }
            isMouseDraging = false;
            CurrentMarker = null;
        }

        // Start the drag when we press the mouse
        private void map_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownStart = map.FromLocalToLatLng(e.X, e.Y);

            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                isMouseDraging = false;
            }
        }

        // Handle dragging and panning
        private void map_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng point = map.FromLocalToLatLng(e.X, e.Y);

            if (MouseDownStart == point)
                return;

            //draging
            if (e.Button == MouseButtons.Left && isMouseDown)
            {
                isMouseDraging = true;

                if (CurrentMarker != null)
                {
                    PointLatLng pnew = map.FromLocalToLatLng(e.X, e.Y);

                    if (ReferenceEquals(CurrentMarker, loiter_marker) || ReferenceEquals(CurrentMarker, loiter_circle))
                    {
                        loiter_point = pnew;
                    }
                    else if (ReferenceEquals(CurrentMarker, land_marker))
                    {
                        land_point = pnew;
                        chk_land_home.Checked = false;
                    }

                    DrawLanding();

                }
                else // left click pan
                {
                    double latdif = MouseDownStart.Lat - point.Lat;
                    double lngdif = MouseDownStart.Lng - point.Lng;

                    try
                    {
                        lock (thisLock)
                        {
                            map.Position = new PointLatLng(map.Position.Lat + latdif, map.Position.Lng + lngdif);
                        }
                    }
                    catch { }
                }
            }
        }

        private void load_default_approach()
        {
            // Freeze the handlers while we initialize the controls
            freeze_handlers = true;

            // Copy settings from this approach type into current approach
            loiter_radius = aircraft_settings.loitradius_default;
            approach_points = aircraft_settings.approach_points.ToList();

            var N = approach_points.Count;

            // The distance of the second-to-last point is the minimum exit distance
            min_exit_dist = approach_points[N - 2].dist;
            // Set the current distance to the last point in the approach points
            loit_exit_dist = approach_points.Last().dist;

            // Calculate grade from last two points
            grade = (approach_points[N - 1].alt - approach_points[N - 2].alt) / (approach_points[N - 1].dist - approach_points[N - 2].dist);
            // Set the loiter exit limits and value
            double loit_exit_max_alt = (approach_points[N - 1].dist - approach_points[N - 2].dist) * aircraft_settings.max_descent_grade + approach_points[N - 2].alt;
            num_exitalt.Maximum = (decimal)Math.Floor(CurrentState.toAltDisplayUnit(loit_exit_max_alt + alt_offset));
            num_exitalt.Minimum = (decimal)Math.Round(CurrentState.toAltDisplayUnit(approach_points[N - 2].alt + alt_offset), 0);
            num_exitalt.Value = (decimal)Math.Floor(CurrentState.toAltDisplayUnit(approach_points.Last().alt));

            // Set the default loiter radius
            num_loitrad.Value = (decimal)Math.Round(CurrentState.toDistDisplayUnit(loiter_radius), num_loitrad.DecimalPlaces);

            // Set the minimum transit altitude to final loiter altitude
            num_transit_alt.Minimum = num_exitalt.Value;

            // Set the vtol altitude to the first point in the approach
            alt_offset = 0;
            num_vtolalt.Value = (decimal)Math.Round(CurrentState.toAltDisplayUnit(approach_points.First().alt), num_vtolalt.DecimalPlaces);

            // Release the handlers
            freeze_handlers = false;

            // Update drawing
            RecalculateMarkers();
            RecenterMap();
        }
        private void rad_loitcw_CheckedChanged(object sender, EventArgs e)
        {
            if (freeze_handlers) return;

            RecalculateMarkers();
        }

        private void num_winddir_ValueChanged(object sender, EventArgs e)
        {
            if (freeze_handlers) return;

            // Wrap new value to (0, 360]
            freeze_handlers = true;
            num_winddir.Value = (decimal)Wrap360((double)num_winddir.Value);
            wind_direction = (double)num_winddir.Value;
            freeze_handlers = false;

            RecalculateMarkers();
        }

        private void num_loitrad_ValueChanged(object sender, EventArgs e)
        {
            if (freeze_handlers) return;

            // Round loiter radius to 10m (ArduPilot limitation)
            loiter_radius = Math.Round(CurrentState.fromDistDisplayUnit((double)num_loitrad.Value) / 10, 0) * 10;
            loiter_circle.wprad = loiter_radius;
            RecalculateMarkers();
        }

        private void chk_land_home_CheckedChanged(object sender, EventArgs e)
        {
            if (freeze_handlers) return;

            // If the user checks this box, set the land point to the home point
            if (chk_land_home.Checked)
            {
                // Move the land and loiter points by the same amount
                PointLatLng home = plugin.Host.cs.PlannedHomeLocation;
                double latdif = home.Lat - land_point.Lat;
                double lngdif = home.Lng - land_point.Lng;
                land_point = new PointLatLng(land_point.Lat + latdif, land_point.Lng + lngdif);
                loiter_point = new PointLatLng(loiter_point.Lat + latdif, loiter_point.Lng + lngdif);

                DrawLanding();
                RecenterMap();
            }
        }

        private void chk_showalt_CheckedChanged(object sender, EventArgs e)
        {
            if (freeze_handlers) return;

            DrawLanding();
        }

        private void num_transit_alt_ValueChanged(object sender, EventArgs e)
        {
            if (freeze_handlers) return;

            // Only bother redrawing if we have altitudes displayed
            if (chk_showalt.Checked)
            {
                DrawLanding();
            }
        }

        private void but_accept_Click(object sender, EventArgs e)
        {
            // Write the landing pattern to the mission
            var sign = rad_loitcw.Checked ? 1 : -1;
            plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_LAND_START, 0, 0, 0, 0, loiter_point.Lng, loiter_point.Lat, (double)num_transit_alt.Value);

            // If exit altitude and transit altitude are within 5m, don't bother with a loiter to altitude
            if (Math.Abs(num_exitalt.Value - num_transit_alt.Value) > (decimal)CurrentState.toDistDisplayUnit(5))
            {
                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, loiter_point.Lng, loiter_point.Lat, (double)num_transit_alt.Value);
                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.LOITER_TO_ALT, 0, sign * loiter_radius, 0, 0, loiter_point.Lng, loiter_point.Lat, Math.Round(CurrentState.toAltDisplayUnit(approach_points.Last().alt + alt_offset)));
            }

            if (num_loitertimemin.Value > 0)
            {
                double minutes_per_turn = (2 * Math.PI * loiter_radius) / aircraft_settings.cruise_speed / 60;
                int loiter_turns = (int)Math.Round((double)num_loitertimemin.Value / minutes_per_turn);
                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.LOITER_TURNS, loiter_turns, 0, sign * loiter_radius, 0, loiter_point.Lng, loiter_point.Lat, Math.Round(CurrentState.toAltDisplayUnit(approach_points.Last().alt + alt_offset)));
                // Add a DO_LAND_START with an absurd altitude. This guarantees it will never be selected as the closest
                // landing sequence, but still serves to mark that we are in a landing sequence. This will waypoint will
                // manualy jumped to when the operators command the aircraft onto its final approach.
                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.DO_LAND_START, 0, 0, 0, 0, loiter_point.Lng, loiter_point.Lat, 60000); // meters or feet, this is many times higher than we're capable of flying. 
                // Add 0-turn loiter. This makes sure that no matter when that button is clicked, it won't exit until
                // it's pointing the right way.
                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.LOITER_TURNS, 0, 0, sign * loiter_radius, 1, loiter_point.Lng, loiter_point.Lat, Math.Round(CurrentState.toAltDisplayUnit(approach_points.Last().alt + alt_offset)));
            }
            else // Otherwise throw in a single loiter turn
            {
                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.LOITER_TURNS, 1, 0, sign * loiter_radius, 1, loiter_point.Lng, loiter_point.Lat, Math.Round(CurrentState.toAltDisplayUnit(approach_points.Last().alt + alt_offset)));
            }

            // Loop backward through approach_points starting with second-to-last and add waypoints
            for (int i = approach_points.Count - 2; i >= 0; i--)
            {
                // Skip if this waypoint is on top of the last one (when the user drags the sequence to the minimum distance)
                if (Math.Abs(approach_points[i].dist - approach_points[i + 1].dist) < 1)
                {
                    continue;
                }
                var point = land_point.newpos(wind_direction, -approach_points[i].dist);
                point.Alt = approach_points[i].alt;
                plugin.Host.AddWPtoList(MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, point.Lng, point.Lat, Math.Round(CurrentState.toAltDisplayUnit(point.Alt + alt_offset)));
            }

            // Add the VTOL Land
            plugin.Host.AddWPtoList(MAVLink.MAV_CMD.VTOL_LAND, 0, 0, 0, 0, land_point.Lng, land_point.Lat, 0);

            // Close the window
            this.Close();
        }

        private void num_exitalt_ValueChanged(object sender, EventArgs e)
        {
            // Make sure the transit altitude is not lower than the exit altitude
            bool freeze_handlers_backup = freeze_handlers;
            freeze_handlers = true;
            num_transit_alt.Minimum = num_exitalt.Value;
            freeze_handlers = freeze_handlers_backup;

            if (freeze_handlers) return;

            // Recalculate grade
            var N = approach_points.Count;
            grade = ((double)num_exitalt.Value / CurrentState.multiplieralt - alt_offset - approach_points[N - 2].alt) / (approach_points[N - 1].dist - approach_points[N - 2].dist);
            // Clamp to max_grade (because of above rounding, it can slightly exceed)
            grade = Math.Min(grade, aircraft_settings.max_descent_grade);

            // Only bother redrawing if we have altitudes displayed
            if (chk_showalt.Checked)
            {
                DrawLanding();
            }
        }

        private void num_vtolalt_ValueChanged(object sender, EventArgs e)
        {
            if (freeze_handlers) return;

            alt_offset = (double)num_vtolalt.Value / CurrentState.multiplieralt - approach_points[0].alt;

            // Only bother redrawing if we have altitudes displayed
            if (chk_showalt.Checked)
            {
                DrawLanding();
            }
        }
    }
}

