using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using GMap.NET;
using MissionPlanner.Controls;

namespace MissionPlanner.Utilities
{
    public class SightGen
    {
        //
        // RF Propagation
        //
        double homealt;
        double drone_alt;
        double clearance;
        double range;
        double base_height;

        /// <summary>
        /// True when a bearing stopped early because its terrain is not loaded yet (the tile is
        /// still downloading, or the map is offline). The outline should be worked out again later.
        /// </summary>
        public bool MissingTerrain { get; private set; }

        /// <summary>
        /// Degrees between bearings, 1 when unset. The setting holds the text of the settings
        /// dropdown item ("0.5"), so it is read the same way in every locale.
        /// </summary>
        public static double AzimuthStep
        {
            get
            {
                double step;
                if (double.TryParse(Settings.Instance["Propagation_Rotational"], NumberStyles.Float,
                        CultureInfo.InvariantCulture, out step) && step > 0)
                    return step;
                return 1;
            }
        }

        public SightGen(PointLatLng location, List<PointLatLng> pointslist, double alt, PointLatLng aircraftlocation, double aircraft_alt)
        {
            range = Settings.Instance.GetFloat("Propagation_Range", 2.0f);
            clearance = Settings.Instance.GetFloat("Propagation_Clearance", 5);
            base_height = Settings.Instance.GetFloat("Propagation_Height", 2.0f);
            var rotational = AzimuthStep;

            drone_alt = aircraft_alt;

            // enable rf when there is no drone connected
            // use home loc + max(base height , clearance) + 0.1;
            if (drone_alt == 0)
                drone_alt = alt + Math.Max(base_height, clearance) + 0.1;

            homealt = alt;

            // The obstacles come from the terrain model, so stand the antenna and the aircraft on it
            // too: keep the aircraft's height above home, but measure it from the terrain at home.
            // The home altitude is a GPS or typed value and can be metres off the terrain model,
            // enough to put the antenna underground so that every bearing ends at home.
            var ground = srtm.getAltitude(location.Lat, location.Lng);
            if (ground.currenttype != srtm.tiletype.invalid)
            {
                drone_alt += ground.alt - homealt;
                homealt = ground.alt;
            }

            SortedDictionary<double,PointLatLngAlt> sortedlist = new SortedDictionary<double, PointLatLngAlt>();

            Parallel.ForEach(Extensions.SteppedRange(0, 360, rotational), bearing =>  //Terrain intercept scan full 360
            {
                var point = getSRTMAltPath(location, bearing);

                lock(sortedlist)
                    sortedlist.Add(bearing, point);
            });

            sortedlist.Values.ForEach(a => pointslist.Add(a));

            pointslist.Add(pointslist[0]);

            pointslist.DeDupOrderedList();
        }

        /// <summary>
        /// Walk out along one bearing from the antenna (base height above the terrain at home) and
        /// return the last point where a target at the aircraft altitude minus the clearance is
        /// still in sight.
        /// The walk keeps the steepest slope from the antenna to the terrain passed so far; the
        /// target is hidden once its own slope from the antenna is below that, or once the terrain
        /// under it rises to the target altitude.
        /// </summary>
        PointLatLngAlt getSRTMAltPath(PointLatLng location, double bearing)
        {
            var start = new PointLatLngAlt(location);
            var end = start.newpos(bearing, range * 1000);

            var antenna = homealt + base_height;
            var target = drone_alt - clearance;

            double dist = start.GetDistance(end);

            int points = (int)(dist / 10) + 1;

            double steplat = (end.Lat - start.Lat) / points;
            double steplng = (end.Lng - start.Lng) / points;

            // steepest rise over distance from the antenna to the terrain walked so far
            double horizon = double.NegativeInfinity;

            PointLatLngAlt answer = start;

            for (int a = 1; a <= points; a++)
            {
                double lat = start.Lat + steplat * a;
                double lng = start.Lng + steplng * a;
                double d = dist * a / points;

                var altresponce = srtm.getAltitude(lat, lng);

                if (altresponce.currenttype == srtm.tiletype.invalid)
                {
                    // no terrain here yet, stop at the last point we know about
                    MissingTerrain = true;
                    break;
                }

                // terrain up to the target altitude, or nearer terrain hides the target
                if (altresponce.alt >= target || (target - antenna) / d < horizon)
                    break;

                horizon = Math.Max(horizon, (altresponce.alt - antenna) / d);

                answer = new PointLatLngAlt(lat, lng, altresponce.alt, "");
            }

            return answer;
        }
    }
}
