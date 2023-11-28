using System;
using System.Collections.Generic;
using System.Drawing;

namespace Carbonix
{
    public class GeneralSettings
    {
        public VelZUnits velz_unit;
        public Color hud_groundcolor1;
        public Color hud_groundcolor2;
        public List<string> fdmap_menu_allow;
        public List<string> fpmap_menu_allow;
        public List<string> fpmap_menu_autowp_allow;
        public List<string> fpmap_menu_maptool_allow;
        public string controller;

        public GeneralSettings()
        {
            velz_unit = VelZUnits.meters_per_second;
            
            hud_groundcolor1 = Color.FromArgb(179, 119, 44);
            hud_groundcolor2 = Color.FromArgb(128, 85, 31);

            fdmap_menu_allow = new List<string>()
            {
                "goHereToolStripMenuItem",
                "flyToCoordsToolStripMenuItem",
                "flightPlannerToolStripMenuItem",
            };  
            fpmap_menu_allow = new List<string>()
            {
                "deleteWPToolStripMenuItem",
                "insertWpToolStripMenuItem",
                "loiterToolStripMenuItem",
                "jumpToolStripMenuItem",
                "clearMissionToolStripMenuItem",
                "toolStripSeparator1",
                "autoWPToolStripMenuItem",
                "mapToolToolStripMenuItem",
                "modifyAltToolStripMenuItem",
            };
            fpmap_menu_autowp_allow = new List<string>()
            {
                "createWpCircleToolStripMenuItem",
                "surveyGridToolStripMenuItem",
            };
            fpmap_menu_maptool_allow = new List<string>()
            {
                "zoomToToolStripMenuItem",
                "prefetchToolStripMenuItem",
                "prefetchWPPathToolStripMenuItem",
                "kMLOverlayToolStripMenuItem",
                "elevationGraphToolStripMenuItem",
            };
            controller = "vJoy Device";
        }
    }

    public class AircraftSettings
    {
        public double guidedalt_min;
        public double guidedalt_max;
        public double loitradius_min;
        public double loitradius_max;
        public double loitradius_default;
        public double climbrate_min;
        public double climbrate_max;
        public double min_vtol_altitude;
        public double max_vtol_altitude;
        public double max_descent_grade;
        public double cruise_speed;
        public decimal landing_hold_minutes;

        public struct Point
        {
            public double dist;
            public double alt;
        }
        public List<Point> approach_points;

        public List<NumberViewSettings> takeofftab_displays;

        public List<string> pilots;

        public bool use_joystick;

        public AircraftSettings(Aircraft aircraft)
        {
            switch(aircraft)
            {
            case Aircraft.Volanti:
                guidedalt_min = -600;
                guidedalt_max = 4500;
                loitradius_min = 120;
                loitradius_max = 3000;
                loitradius_default = 120;
                climbrate_min = 0.5;
                climbrate_max = 3.1;
                min_vtol_altitude = 20;
                max_vtol_altitude = 90;
                max_descent_grade = 0.08;
                cruise_speed = 21.0;
                landing_hold_minutes = 0;

                approach_points = new List<Point>()
                {
                    new Point() { dist = 250, alt = 40 },
                    new Point() { dist = 600, alt = 60 },
                };

                takeofftab_displays = new List<NumberViewSettings>()
                {
                    new NumberViewSettings() { variable = "esc1_rpm", description = "RPM1", numberformat = "0", charwidth = 5 },
                    new NumberViewSettings() { variable = "esc2_rpm", description = "RPM2", numberformat = "0", charwidth = 5 },
                    new NumberViewSettings() { variable = "esc3_rpm", description = "RPM3", numberformat = "0", charwidth = 5 },
                    new NumberViewSettings() { variable = "esc4_rpm", description = "RPM4", numberformat = "0", charwidth = 5 },
                };

                pilots = new List<string>()
                {
                    "Lachlan Conn",
                    "Isaac Straatemeier",
                };

                use_joystick = true;
                break;
            case Aircraft.Ottano:
                guidedalt_min = -600;
                guidedalt_max = 4500;
                loitradius_min = 120;
                loitradius_max = 3000;
                loitradius_default = 120;
                climbrate_min = 0.5;
                climbrate_max = 3.1;
                min_vtol_altitude = 20;
                max_vtol_altitude = 90;
                max_descent_grade = 0.08;
                cruise_speed = 21.0;
                landing_hold_minutes = 0;

                approach_points = new List<Point>()
                {
                    new Point() { dist = 250, alt = 40 },
                    new Point() { dist = 600, alt = 60 },
                };

                takeofftab_displays = new List<NumberViewSettings>()
                {
                    new NumberViewSettings() { variable = "efi_rpm", description = "RPM", numberformat = "0", charwidth = 5 },
                    new NumberViewSettings() { variable = "efi_fuelpress", description = "Fuel Pressure (kPa)", numberformat = "0", charwidth = 5 },
                    new NumberViewSettings() { variable = "efi_headtemp", description = "CHT1", numberformat = "0", charwidth = 5 },
                    new NumberViewSettings() { variable = "MAV_CHT2", description = "CHT2", numberformat = "0", charwidth = 5 },
                };
                
                pilots = new List<string>()
                {
                    "Lachlan Conn",
                    "Isaac Straatemeier",
                };
                
                use_joystick = false;
                break;
            default:
                throw new Exception("Unknown aircraft: " + aircraft.ToString());
            }
        }
    }

    public class NumberViewSettings
    {
        public string variable;
        public string description;
        public string numberformat;
        public int charwidth;
    }

    public enum VelZUnits
    {
        meters_per_second,
        feet_per_minute
    }

    public enum Aircraft
    {
        Volanti,
        Ottano
    }
}                                                                                                                                                  
