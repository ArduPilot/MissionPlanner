using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Carbonix
{
    public class GeneralSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public VelZUnits velz_unit;
        public Color hud_groundcolor1;
        public Color hud_groundcolor2;
        public List<string> fdmap_menu_allow;
        public List<string> fpmap_menu_allow;
        public List<string> fpmap_menu_autowp_allow;
        public List<string> fpmap_menu_maptool_allow;
        public List<string> pilot_locations;
        public string controller;
        public string weather_api_key;

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
            pilot_locations = new List<string>()
            {
                "Field",
                "ROC1"
            };
            controller = "vJoy Device";
            /*
            It is shockingly difficult to handle this API key the "right way" in
            C#. Getting an environment variable at compile-time is not
            supported. The best way seems to be to put in a placeholder string
            and then have CI do a find/replace in all files.

            We will instead do this the wrong way until it becomes a problem.
            This is a free API key with a simple daily limit, so if it leaks, we
            can simply get a new one and then use this old key as the
            placeholder for the find/replace.
            */
            weather_api_key = "5f88b0183ab94ee68cb15d418c";
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
        public List<string> payloads;
        public bool has_avionics_battery;
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
                loitradius_default = 150;
                climbrate_min = 0.5;
                climbrate_max = 3.1;
                min_vtol_altitude = 20;
                max_vtol_altitude = 90;
                max_descent_grade = 0.08;
                cruise_speed = 21.0;
                landing_hold_minutes = 0;

                approach_points = new List<Point>()
                {
                    new Point() { dist = 400, alt = 40 },
                    new Point() { dist = 550, alt = 50 },
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
                    "Darwish Ismail",
                    "Aidan Biggar",
                    "Matt Sturdy",
                };

                payloads = new List<string>()
                {
                    "None",
                    "YellowScan",
                    "CM62",
                    "Nighthawk"
                };

                has_avionics_battery = false;

                use_joystick = true;
                break;
            case Aircraft.Ottano:
                guidedalt_min = -600;
                guidedalt_max = 4500;
                loitradius_min = 150;
                loitradius_max = 3000;
                loitradius_default = 200;
                climbrate_min = 0.5;
                climbrate_max = 4.5;
                min_vtol_altitude = 20;
                max_vtol_altitude = 90;
                max_descent_grade = 0.08;
                cruise_speed = 24.0;
                landing_hold_minutes = 0;

                approach_points = new List<Point>()
                {
                    new Point() { dist = 560, alt = 40 },
                    new Point() { dist = 700, alt = 50 },
                };

                takeofftab_displays = new List<NumberViewSettings>()
                {
                    new NumberViewSettings() { variable = "efi_rpm", description = "RPM", numberformat = "0", charwidth = 5 },
                    new NumberViewSettings() { variable = "efi_fuelconsumed", description = "Fuel Consumed(g)", numberformat = "0", charwidth = 5 },
                    new NumberViewSettings() { variable = "efi_headtemp", description = "CHT1", numberformat = "0", charwidth = 5 },
                    new NumberViewSettings() { variable = "MAV_CHT2", description = "CHT2", numberformat = "0", charwidth = 5 },
                };
                
                pilots = new List<string>()
                {
                    "Lachlan Conn",
                    "Isaac Straatemeier",
                    "Darwish Ismail",
                    "Aidan Biggar",
                    "Matt Sturdy",
                };

                payloads = new List<string>()
                {
                    "None",
                    "Dual GS-120",
                    "Riegl and GS-120",
                    "Single GS-120",
                    "Riegl Vux-120",
                };

                has_avionics_battery = true;

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
