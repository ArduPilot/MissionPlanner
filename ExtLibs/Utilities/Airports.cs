using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using log4net;
using System.Reflection;

namespace MissionPlanner.Utilities
{
    public class Airports
    {
        // Unlocode
        //http://www.unece.org/cefact/locode/welcome.html

        //http://www.partow.net/miscellaneous/airportdatabase/index.html

        //http://ourairports.com/data/

        //http://openflights.org/data.html

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static List<PointLatLngAlt> airports = new List<PointLatLngAlt>();

        public static int GetAirportCount
        {
            get { lock (locker) return airports.Count; }
        }

        static PointLatLngAlt currentcenter = PointLatLngAlt.Zero;

        static object locker = new object();

        /// <summary>
        /// the cache zone radius in m, based on the centerpoint provided
        /// </summary>
        public static int proximity = 100000;

        /// <summary>
        /// used to track is more airports have been loaded, and the cache needs refreshing
        /// </summary>
        static bool newairports = false;

        public static bool checkdups = false;

        static List<PointLatLngAlt> cache = new List<PointLatLngAlt>();

        public static List<PointLatLngAlt> getAirports(PointLatLngAlt centerpoint)
        {
            lock (locker)
            {
                DateTime start = DateTime.Now;

                //log.Info("getAirports " + centerpoint);

                // check if we have moved 66% from our last cache center point
                if (currentcenter.GetDistance(centerpoint) < ((proximity/3)*2))
                {
                    if (!newairports)
                        return cache;
                }

                newairports = false;

                log.Info("getAirports - regen list");

                // generate a new list
                currentcenter = centerpoint;

                cache.Clear();

                foreach (PointLatLngAlt item in airports)
                {
                    if (item.GetDistance(centerpoint) < proximity)
                    {
                        cache.Add(item);
                    }
                }

                log.Info("getAirports done " + (DateTime.Now - start).TotalSeconds + " sec");

                return cache;
            }
        }

        public static void AddAirport(PointLatLngAlt plla)
        {
            lock (locker)
            {
                if (checkdups)
                {
                    foreach (PointLatLngAlt item in airports)
                    {
                        if (item.GetDistance(plla) < 1000) // 1000m
                        {
                            return;
                        }
                    }
                }

                airports.Add(plla);

                newairports = true;
            }
        }

        public static void ReadOpenflights(string fn)
        {
            string[] lines = File.ReadAllLines(fn);

            foreach (var line in lines)
            {
                string[] items = line.Split(',');

                if (items.Length == 0)
                    continue;

                try
                {
                    string name = items[1];
                    int latOffset = 0;
                    while (name[0] == '"' && name[name.Length - 1] != '"')
                    {
                        latOffset += 1;
                        name = name + "," + items[2 + latOffset];
                    }
                    name = name.Trim('"');

                    if (items[5 + latOffset].Length != 6)
                        continue;

                    double lat = double.Parse(items[6 + latOffset].Trim('"'), CultureInfo.InvariantCulture);
                    double lng = double.Parse(items[7 + latOffset].Trim('"'), CultureInfo.InvariantCulture);
                    double alt = 0;

                    //double alt = double.Parse(items[8 + latOffset].Trim('"')) * 0.3048;

                    var newap = new PointLatLngAlt(lat, lng, alt, name);

                    AddAirport(newap);
                    //Console.WriteLine(newap);
                }
                catch
                {
                }
            }
        }

        public static void ReadOurairports(string fn)
        {
            string[] lines = File.ReadAllLines(fn);

            foreach (var line in lines)
            {
                string[] items = line.Split(',');

                if (items.Length == 0)
                    continue;

                try
                {
                    if (items[0] == "\"id\"")
                        continue;

                    if (items[1].Length != 6) // "xxxx"
                        continue;

                    if (items[2].Contains("small_airport") || items[2].Contains("seaplane_base") ||
                        items[2].Contains("heliport") || items[2].Contains("closed"))
                        continue;


                    string name = items[3];
                    int latOffset = 0;
                    while (name[0] == '"' && name[name.Length - 1] != '"')
                    {
                        latOffset += 1;
                        name = name + "," + items[3 + latOffset];
                    }
                    name = name.Trim('"');
                    double lat = double.Parse(items[4 + latOffset].Trim('"'), CultureInfo.InvariantCulture);
                    double lng = double.Parse(items[5 + latOffset].Trim('"'), CultureInfo.InvariantCulture);
                    double alt = 0;

                    //double alt = double.Parse(items[6 + latOffset].Trim('"')) * 0.3048;

                    var newap = new PointLatLngAlt(lat, lng, alt, name);

                    AddAirport(newap);
                    //Console.WriteLine(newap);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Field 01 - ICAO Code: 4 character ICAO code 
        /// Field 02 - IATA Code: 3 character IATA code 
        /// Field 03 - Airport Name: string of varying length 
        /// Field 04 - City,Town or Suburb: string of varying length 
        /// Field 05 - Country: string of varying length 
        /// Field 06 - Latitude Degrees: 2 ASCII characters representing one numeric value 
        /// Field 07 - Latitude Minutes: 2 ASCII characters representing one numeric value 
        /// Field 08 - Latitude Seconds: 2 ASCII characters representing one numeric value 
        /// Field 09 - Latitude Direction: 1 ASCII character either N or S representing compass direction 
        /// Field 10 - Longitude Degrees: 2 ASCII characters representing one numeric value 
        /// Field 11 - Longitude Minutes: 2 ASCII characters representing one numeric value 
        /// Field 12 - Longitude Seconds: 2 ASCII characters representing one numeric value 
        /// Field 13 - Longitude Direction: 1 ASCII character either E or W representing compass direction 
        /// Field 14 - Altitude: varying sequence of ASCII characters representing a numeric value corresponding to the airport's altitude from mean sea level (ie: "123" or "-123") 
        /// </summary>
        /// <param name="fn"></param>
        public static void ReadPartow(string fn)
        {
            string[] lines = File.ReadAllLines(fn);

            foreach (var line in lines)
            {
                string[] items = line.Split(':');

                string name = items[2].Trim('"');

                double lat = double.Parse(items[5], CultureInfo.InvariantCulture) +
                             double.Parse(items[6], CultureInfo.InvariantCulture)/60 +
                             double.Parse(items[7], CultureInfo.InvariantCulture)/3600;
                double lng = double.Parse(items[9], CultureInfo.InvariantCulture) +
                             double.Parse(items[10], CultureInfo.InvariantCulture)/60 +
                             double.Parse(items[11], CultureInfo.InvariantCulture)/3600;

                if (items[8] == "S")
                    lat *= -1;

                if (items[12] == "W")
                    lng *= -1;

                var newap = new PointLatLngAlt(lat, lng, 0, name);

                AddAirport(newap);
                Console.WriteLine(newap);
            }
        }

        public static void ReadUNLOCODE(string fn)
        {
            string[] lines = File.ReadAllLines(fn);

            foreach (var line in lines)
            {
                string[] items = line.Split(',');

                string name = items[3].Trim('"');
                string function = items[6].Trim('"');
                string coords = items[10].Trim('"');

                if (name == "")
                    continue;
                if (coords == "")
                    continue;
                if (!function.Contains("4"))
                    continue;

                string[] coordssplit = coords.Split(' ');

                if (coordssplit.Length != 2)
                    continue;

                double northing = double.Parse(coordssplit[0].Substring(0, 4), CultureInfo.InvariantCulture)/100.0;
                double easting = double.Parse(coordssplit[1].Substring(0, 5), CultureInfo.InvariantCulture)/100.0;

                if (coordssplit[0].Contains("S"))
                    northing *= -1;

                if (coordssplit[1].Contains("W"))
                    easting *= -1;

                var newap = new PointLatLngAlt(northing, easting, 0, name);

                AddAirport(newap);
                Console.WriteLine(newap);
            }
        }
    }
}