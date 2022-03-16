using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public SightGen(PointLatLng location, List<PointLatLng> pointslist, double alt, PointLatLng aircraftlocation, double aircraft_alt)
        {
            range = Settings.Instance.GetFloat("Propagation_Range");
            clearance = Settings.Instance.GetFloat("Propagation_Clearance");
            base_height = Settings.Instance.GetFloat("Propagation_Height");
            var converge = Settings.Instance.GetInt32("Propagation_Converge");
            var rotational = Settings.Instance.GetInt32("Propagation_Rotational");
            if (rotational == 0)
                rotational = 1;

            drone_alt = aircraft_alt;

            // enable rf when there is no drone connected
            // use home loc + max(base height , clearance) + 0.1;
            if (drone_alt == 0)
                drone_alt = alt + Math.Max(base_height, clearance) + 0.1;

            homealt = alt;

            var startlocation = new PointLatLngAlt(location) {Alt = homealt + base_height};
            var dronelocation = new PointLatLngAlt(aircraftlocation) {Alt = drone_alt - clearance};

            SortedDictionary<double,PointLatLngAlt> sortedlist = new SortedDictionary<double, PointLatLngAlt>();

            Parallel.ForEach(Extensions.SteppedRange(0, 2 * (float)Math.PI, Math.PI / 180 * rotational), angle =>  //Terrain intercept scan full 360
            {
                List<PointLatLngAlt> pointends = new List<PointLatLngAlt>();

                var carryon = true;

                var up = false;
                var down = false;

                float triangle = 45 * (float)Math.PI / 180; //radians
                var Max = 90 * (float)Math.PI / 180;  //radians
                float Min = 0;                          //radians

                // calc LOS vector and intersection with terrain
                //point = srtm.getIntersectionWithTerrain(startlocation, endlocation);
                PointLatLngAlt point = null;

                var newpos = new PointLatLngAlt(location).newpos(angle * MathHelper.rad2deg, range * 1000);
                pointends.Clear();
                pointends.Add(location);
                pointends.Add(newpos);

                while (carryon && Min + converge * (float)Math.PI / 180 < Max) //Breaks if terrain intercept found or convergence occurs
                {
                    point = getSRTMAltPath(pointends, triangle, ref carryon, ref up, ref down); //DEM data

                    if (up)
                    {
                        Min = triangle;
                    }

                    else if (down)
                    {
                        Max = triangle;
                    }
                    triangle = (Max + Min) / 2;
                }

                lock(sortedlist)
                    sortedlist.Add(angle, point);
            });

            sortedlist.Values.ForEach(a => pointslist.Add(a));

            pointslist.Add(pointslist[0]);

            pointslist.DeDupOrderedList();
        }

        PointLatLngAlt getSRTMAltPath(List<PointLatLngAlt> list, float triangle, ref bool carryon, ref bool up, ref bool down)
        {
            PointLatLngAlt answer = new PointLatLngAlt();

            PointLatLngAlt last = list[0];
            PointLatLngAlt loc = list[1];

            double disttotal = 0;
            int a = 0;
            double elev = 0;
            double height = 0;

            double dist = last.GetDistance(loc);

            int points = (int)(dist / 10) + 1;

            double deltalat = (last.Lat - loc.Lat);
            double deltalng = (last.Lng - loc.Lng);

            double steplat = deltalat / points;
            double steplng = deltalng / points;

            PointLatLngAlt lastpnt = last;
            while (a <= points && elev < homealt + base_height + height && elev < drone_alt - clearance)
            {
                double lat = last.Lat - steplat * a;
                double lng = last.Lng - steplng * a;

                var newpoint = new PointLatLngAlt(lat, lng, srtm.getAltitude(lat, lng).alt, "");

                double subdist = lastpnt.GetDistance(newpoint);

                disttotal += subdist;

                height = disttotal * Math.Tan(triangle); //Height of traingle formed at point with projected line

                // srtm alts
                answer = newpoint;
                elev = newpoint.Alt;
                lastpnt = newpoint;
                a++;
            }

            if (elev < homealt + base_height + height + 0.1 && elev > homealt + base_height + height - 0.1
                && elev < drone_alt - clearance + 0.1 && elev > drone_alt - clearance - 0.1) //Terrain intercept found
            {
                carryon = false;
            }

            else
            {
                if (elev > homealt + base_height + height && elev > drone_alt - clearance)
                {
                    up = true;
                    down = false;
                }

                else
                {
                    down = true;
                    up = false;
                }
            }

            return answer;
        }
    }
}
