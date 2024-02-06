using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.HIL;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;

namespace Carbonix
{
    public partial class ActionsControl : UserControl
    {
        readonly PluginHost Host;

        readonly private string velz_unit;
        readonly private double multipliervelz;

        readonly private Dictionary<string, decimal> value_backups = new Dictionary<string, decimal>();

        // Used to prevent change handlers from running with programatic updates
        private bool freeze_handlers = true;

        public ActionsControl(PluginHost Host, GeneralSettings settings, AircraftSettings aircraft_settings)
        {
            this.Host = Host;
            
            InitializeComponent();
            
            // Bind event handler for mav parameter changes
            Host.comPort.ParamListChanged += ParamListChanged;
            Host.comPort.CommsClose += CommsClose;

            // Set up guided altitude control
            NUM_guidedalt.Increment = CurrentState.AltUnit == "m" ? 10 : 50;
            NUM_guidedalt.Minimum = (decimal)CurrentState.toAltDisplayUnit(aircraft_settings.guidedalt_min);
            NUM_guidedalt.Maximum = (decimal)CurrentState.toAltDisplayUnit(aircraft_settings.guidedalt_max);
            // Round to nearest 10 increments
            NUM_guidedalt.Minimum = Math.Round(NUM_guidedalt.Minimum / NUM_guidedalt.Increment / 10) * NUM_guidedalt.Increment * 10;
            NUM_guidedalt.Maximum = Math.Round(NUM_guidedalt.Maximum / NUM_guidedalt.Increment / 10) * NUM_guidedalt.Increment * 10;
            decimal guidedalt = decimal.Parse(Host.config["guided_alt", "100"], CultureInfo.InvariantCulture);
            guidedalt = Math.Max(guidedalt, NUM_guidedalt.Minimum);
            guidedalt = Math.Min(guidedalt, NUM_guidedalt.Maximum);
            NUM_guidedalt.Value = guidedalt;
            value_backups[NUM_guidedalt.Name] = NUM_guidedalt.Value;
            NUM_guidedalt.Enabled = true; // This one is not param-based, we can change it while disconnected
            LBL_altunits.Text = CurrentState.AltUnit;

            // Set up loiter radius control
            NUM_loitradius.Increment = CurrentState.AltUnit == "m" ? 25 : 100;
            NUM_loitradius.Minimum = (decimal)CurrentState.toDistDisplayUnit(aircraft_settings.loitradius_min);
            NUM_loitradius.Maximum = (decimal)CurrentState.toDistDisplayUnit(aircraft_settings.loitradius_max);
            // Round to nearest increment, ceiling rounding the minimum
            NUM_loitradius.Minimum = Math.Ceiling(NUM_loitradius.Minimum / NUM_loitradius.Increment) * NUM_loitradius.Increment;
            NUM_loitradius.Maximum = Math.Round(NUM_loitradius.Maximum / NUM_loitradius.Increment) * NUM_loitradius.Increment;
            NUM_loitradius.Value = NUM_loitradius.Minimum;
            NUM_loitradius.Enabled = false;
            CHK_loitdirection.Enabled = false;
            LBL_loitradiusunits.Text = CurrentState.DistanceUnit;

            // Set up climb rate control
            switch(settings.velz_unit)
            {
            case VelZUnits.feet_per_minute:
                multipliervelz = 25000.0 / 127.0; // Exact conversion
                velz_unit = "ft/min";
                NUM_climbrate.Increment = 20;
                NUM_climbrate.DecimalPlaces = 0;
                break;
            case VelZUnits.meters_per_second:
                multipliervelz = 1.0;
                velz_unit = "m/s";
                NUM_climbrate.Increment = 0.1m;
                NUM_climbrate.DecimalPlaces = 1;
                break;
            default:
                throw new Exception("Unknown VelZUnits: " + settings.velz_unit.ToString());
            }
            NUM_climbrate.Minimum = (decimal)ToVelZDisplayUnit(aircraft_settings.climbrate_min);
            NUM_climbrate.Maximum = (decimal)ToVelZDisplayUnit(aircraft_settings.climbrate_max);
            NUM_climbrate.Value = NUM_climbrate.Maximum;
            NUM_climbrate.Enabled = false;
            LBL_climbunits.Text = velz_unit;

            // Set up the airspeed control
            NUM_airspeed.Increment = CurrentState.SpeedUnit == "m/s" ? 0.5m : 1;
            // Airspeed min/max will be determined by params, so skip those
            NUM_airspeed.Enabled = false;
            LBL_airspeedunits.Text = CurrentState.SpeedUnit;

            freeze_handlers = false;
        }

        private double ToVelZDisplayUnit(double velz)
        {
            return velz * multipliervelz;
        }

        private double FromVelZDisplayUnit(double velz)
        {
            return velz / multipliervelz;
        }
        
        // Updates certain control values based on mavlink parameters
        private void ParamListChanged(object sender, EventArgs e)
        {
            if (Host.comPort.BaseStream == null || !Host.comPort.BaseStream.IsOpen)
            {
                return;
            }

            freeze_handlers = true;

            // Update the guided alt control
            if(Host.comPort.MAV.GuidedMode.z == 0)
            {
                Host.comPort.MAV.GuidedMode.z = (float)NUM_guidedalt.Value / CurrentState.multiplieralt;
                // Set this to something slightly non-zero if it was intentially set to zero
                if(Host.comPort.MAV.GuidedMode.z == 0)
                {
                    Host.comPort.MAV.GuidedMode.z = 0.001f;
                }
            }
            else
            {
                NUM_guidedalt.Value = (decimal)CurrentState.toAltDisplayUnit(Host.comPort.MAV.GuidedMode.z);
            }

            // Get param for loiter radius
            if (Host.comPort.MAV.param["WP_LOITER_RAD"] != null)
            {
                decimal loitradius = (decimal)CurrentState.toDistDisplayUnit((float)Host.comPort.MAV.param["WP_LOITER_RAD"]);
                CHK_loitdirection.Checked = loitradius < 0;
                loitradius = Math.Abs(loitradius);
                // This should not happen, but if this is outside the bounds of the control, we will change the bounds
                if (loitradius > NUM_loitradius.Maximum) NUM_loitradius.Maximum = Math.Ceiling(loitradius * NUM_loitradius.Increment) / NUM_loitradius.Increment;
                if(loitradius < NUM_loitradius.Minimum) NUM_loitradius.Minimum = Math.Floor(loitradius * NUM_loitradius.Increment) / NUM_loitradius.Increment;
                NUM_loitradius.Value = Math.Abs(loitradius);
                NUM_loitradius.Enabled = true;
                CHK_loitdirection.Enabled = true;
            }
            else
            {
                NUM_loitradius.Enabled = false;
            }
            value_backups[CHK_loitdirection.Name] = CHK_loitdirection.Checked ? -1 : 1;
            value_backups[NUM_loitradius.Name] = NUM_loitradius.Value;

            // Get param for climb rate
            if (Host.comPort.MAV.param["TECS_CLMB_MAX"] != null)
            {
                decimal climbrate = (decimal)ToVelZDisplayUnit((float)Host.comPort.MAV.param["TECS_CLMB_MAX"]);
                // This should not happen, but if this is outside the bounds of the control, we will change the bounds
                if (climbrate > NUM_climbrate.Maximum) NUM_climbrate.Maximum = Math.Ceiling(climbrate * NUM_climbrate.Increment) / NUM_climbrate.Increment;
                if (climbrate < NUM_climbrate.Minimum) NUM_climbrate.Minimum = Math.Floor(climbrate * NUM_climbrate.Increment) / NUM_climbrate.Increment;
                NUM_climbrate.Value = climbrate;
                NUM_climbrate.Enabled = true;
            }
            else
            {
                NUM_climbrate.Enabled = false;
            }
            value_backups[NUM_climbrate.Name] = NUM_climbrate.Value;

            // Get params for airspeed
            if (Host.comPort.MAV.param["TRIM_ARSPD_CM"] != null &&
                Host.comPort.MAV.param["ARSPD_FBW_MIN"] != null &&
                Host.comPort.MAV.param["ARSPD_FBW_MAX"] != null)
            {
                NUM_airspeed.Minimum = (decimal)CurrentState.toSpeedDisplayUnit((float)Host.comPort.MAV.param["ARSPD_FBW_MIN"]);
                NUM_airspeed.Maximum = (decimal)CurrentState.toSpeedDisplayUnit((float)Host.comPort.MAV.param["ARSPD_FBW_MAX"]);
                NUM_airspeed.Value = (decimal)CurrentState.toSpeedDisplayUnit((float)Host.comPort.MAV.param["TRIM_ARSPD_CM"] / 100.0);
                NUM_airspeed.Enabled = true;
            }
            else
            {
                NUM_airspeed.Enabled = false;
            }
            value_backups[NUM_airspeed.Name] = NUM_airspeed.Value;

            freeze_handlers = false;
        }

        private void CommsClose(object sender, EventArgs e)
        {
            NUM_loitradius.Enabled = false;
            CHK_loitdirection.Enabled = false;
            NUM_climbrate.Enabled = false;
            NUM_airspeed.Enabled = false;
        }

        private void UpdateParameter(NumericUpDown num, string param_name, double value, string fail_message)
        {
            // Update the specified parameter in the aircraft
            byte sysid = (byte)Host.comPort.sysidcurrent;
            byte compid = (byte)Host.comPort.compidcurrent;
            bool result = Host.comPort.setParam(sysid, compid, param_name, value);
            if (result)
            {
                // Update the backup value
                value_backups[num.Name] = num.Value;
                num.BackColor = ThemeManager.ControlBGColor;
                toolTip1.SetToolTip((Control)num, null);
            }
            else
            {
                CustomMessageBox.Show(fail_message);
            }
        }

        // This flag is used to detect if the user has changed the value of a control by typing instead of with the mouse
        private bool _num_changed_manually = false;
        
        private void NUM_KeyDown(object sender, KeyEventArgs e)
        {
            // Check for escape key and reset value to backup
            if (e.KeyCode == Keys.Escape)
            {
                NumericUpDown num = (NumericUpDown)sender;
                // Handler will trigger and handle the bg color change
                num.Value = value_backups[num.Name];
            }

            _num_changed_manually = true;
        }

        private void NUM_ValueChanged(object sender, EventArgs e)
        {
            if (freeze_handlers) return;
            
            NumericUpDown control = (NumericUpDown)sender;
            // If the value changed from the mouse wheel or arrow buttons, round to nearest increment
            if (!_num_changed_manually)
            {
                // Round to increment
                decimal rounded = Math.Round(control.Value / control.Increment) * control.Increment;

                // clamp to the min/max
                if (rounded > control.Maximum) rounded = control.Maximum;
                if (rounded < control.Minimum) rounded = control.Minimum;

                // Set the value
                freeze_handlers = true;
                control.Value = rounded;
                freeze_handlers = false;
                
            }

            // Clear the change-type flag
            _num_changed_manually = false;

            // If the value is different than the backup value, set the background color to green
            if(Math.Round(control.Value, control.DecimalPlaces) != Math.Round(value_backups[control.Name], control.DecimalPlaces))
            {
                // Use a color that contrasts with the text color
                control.BackColor = ThemeManager.ControlBGColor.GetBrightness() < 0.5 ? Color.DarkGreen : Color.LightGreen;
                // Display tooltip explaining how to discard changes
                toolTip1.SetToolTip(control, "Hit escape to cancel changes");
            }
            else
            {
                control.BackColor = ThemeManager.ControlBGColor;
                toolTip1.SetToolTip(control, null);
            }
        }

        private void ActionsControl_VisibleChanged(object sender, EventArgs e)
        {
            ParamListChanged(null, null);
        }

        private void CHK_loitdirection_CheckedChanged(object sender, EventArgs e)
        {
            if (freeze_handlers) return;

            if (CHK_loitdirection.Checked == (value_backups["CHK_loitdirection"] < 0))
            {
                CHK_loitdirection.BackColor = Color.Transparent;
            }
            else
            {
                CHK_loitdirection.BackColor = ThemeManager.BGColor.GetBrightness() < 0.5 ? Color.DarkGreen : Color.LightGreen;
            }
        }

        private void BUT_guidedalt_Click(object sender, EventArgs e)
        {
            Host.config["guided_alt"] = NUM_guidedalt.Value.ToString();

            value_backups[NUM_guidedalt.Name] = NUM_guidedalt.Value;
            NUM_guidedalt.BackColor = ThemeManager.ControlBGColor;

            Host.comPort.MAV.GuidedMode.z = (float)NUM_guidedalt.Value / CurrentState.multiplieralt;
            // Set this to something slightly non-zero if it was intentially set to zero
            if (Host.comPort.MAV.GuidedMode.z == 0)
            {
                Host.comPort.MAV.GuidedMode.z = 0.001f;
            }

            if (Host.comPort.MAV.cs.mode == "Guided")
            {
                Host.comPort.setGuidedModeWP(new Locationwp
                {
                    alt = Host.comPort.MAV.GuidedMode.z,
                    lat = Host.comPort.MAV.GuidedMode.x / 1e7,
                    lng = Host.comPort.MAV.GuidedMode.y / 1e7
                });
            }
        }

        private void BUT_loitradius_Click(object sender, EventArgs e)
        {
            int sign = CHK_loitdirection.Checked ? -1 : 1;
            double radius = sign * CurrentState.fromDistDisplayUnit((double)NUM_loitradius.Value);

            // Handle background color stuff for direction checkbox
            value_backups[CHK_loitdirection.Name] = CHK_loitdirection.Checked ? -1 : 1;
            CHK_loitdirection.BackColor = Color.Transparent;

            UpdateParameter(NUM_loitradius, "WP_LOITER_RAD", radius, "Failed to set loiter radius");

        }

        private void BUT_climb_Click(object sender, EventArgs e)
        {
            double climbrate = FromVelZDisplayUnit((double)NUM_climbrate.Value);

            UpdateParameter(NUM_climbrate, "TECS_CLMB_MAX", climbrate, "Failed to set climb rate");
        }

        private void BUT_airspeed_Click(object sender, EventArgs e)
        {
            double airspeed = CurrentState.fromSpeedDisplayUnit((double)NUM_airspeed.Value);

            UpdateParameter(NUM_airspeed, "TRIM_ARSPD_CM", airspeed * 100, "Failed to set airspeed");
        }

        private void BUT_mode_Click(object sender, EventArgs e)
        {
            byte sysid = (byte)Host.comPort.sysidcurrent;
            byte compid = (byte)Host.comPort.compidcurrent;
            try
            {
                ((Control)sender).Enabled = false;
                MainV2.comPort.setMode(sysid, compid, ((Control)sender).Text);
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }

            ((Control)sender).Enabled = true;
        }

        private void BUT_setwp_Click(object sender, EventArgs e)
        {
            byte sysid = (byte)Host.comPort.sysidcurrent;
            byte compid = (byte)Host.comPort.compidcurrent;
            
            // Get the number of waypoints in the currently loaded mission
            int wpCount = Host.comPort.MAV.wps.Count - 1;
            string wpno_str = Host.cs.wpno.ToString();

            // Launch selection dialog and get resulting selected waypoint number
            MissionPlanner.Controls.InputBox.Show("Enter waypoint number", "Set Waypoint", ref wpno_str);

            if(!ushort.TryParse(wpno_str, out ushort wpno))
            {
                CustomMessageBox.Show("Invalid number format");
                return;
            }

            if (wpno > wpCount)
            {
                CustomMessageBox.Show("Waypoint number is greater than the number of waypoints in the mission", Strings.ERROR);
                return;
            }

            // Send the command to the autopilot
            try
            {
                Host.comPort.setWPCurrent(sysid, compid, wpno);
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
        }
    }

}
