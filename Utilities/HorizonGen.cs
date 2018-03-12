using System;
using System.Collections.Generic;
using GMap.NET;

namespace MissionPlanner.Utilities
{
    public class HorizonGen
    {
        private readonly double base_height;

        private bool carryon = true;

        private bool down;
        //
        //Horizon Propagation
        //


        private readonly double homealt;
        private readonly PointLatLng point;
        private readonly List<PointLatLngAlt> pointends = new List<PointLatLngAlt>();

        private readonly double range;

        private bool up;

        public HorizonGen(PointLatLng location, List<PointLatLng> pointslist, double alt)
        {
            double distance = 0; //km
            double latend = 0;
            double lngend = 0;
            var latradians = location.Lat * Math.PI / 180;
            var lngradians = location.Lng * Math.PI / 180;

            var Max = 90 * (float) Math.PI / 180; //radians
            float Min = 0; //radians


            homealt = alt;
            range = Settings.Instance.GetFloat("Propagation_Range");
            base_height = Settings.Instance.GetFloat("Propagation_Height");

            for (float angle = 0;
                angle <= 2 * (float) Math.PI;
                angle += (float) Math.PI / 180 *
                         Settings.Instance.GetInt32("Propagation_Rotational")) //Terrain intercept scan full 360
            {
                carryon = true;
                var triangle = 45 * (float) Math.PI / 180;
                up = false;
                down = false;
                Max = 90 * (float) Math.PI / 180;
                Min = 0;

                while (carryon && Min + Settings.Instance.GetInt32("Propagation_Converge") * (float) Math.PI / 180 < Max
                ) //Breaks if terrain intercept found or convergence occurs
                {
                    pointends.Clear();
                    pointends.Add(location);

                    distance = range * Math.Cos(triangle) / 6371;

                    latend = Math.Asin(Math.Sin(latradians) * Math.Cos(distance) +
                                       Math.Cos(latradians) * Math.Sin(distance) * Math.Cos(angle));
                    if (Math.Cos(latend) == 0)
                        lngend = lngradians; // endpoint a pole
                    else
                        lngend =
                            mod(
                                lngradians - Math.Asin(Math.Sin(angle) * Math.Sin(distance) / Math.Cos(latend)) +
                                Math.PI, 2 * Math.PI) - Math.PI;

                    var newlatend = latend * 180 / Math.PI;
                    var newlngend = lngend * 180 / Math.PI;

                    pointends.Add(new PointLatLngAlt(newlatend, newlngend, 0, 1.ToString()));
                    point = getSRTMAltPath(pointends, triangle); //DEM data

                    if (up)
                        Min = triangle;

                    else if (down) Max = triangle;
                    triangle = (Max + Min) / 2;
                }

                pointslist.Add(point);
            }

            pointslist.Add(pointslist[0]);
        }

        private double mod(double a, double n)
        {
            var result = a % n;
            if (a < 0 && n > 0 || a > 0 && n < 0)
                result += n;
            return result;
        }

        private PointLatLngAlt getSRTMAltPath(List<PointLatLngAlt> list, float triangle)
        {
            var answer = new PointLatLngAlt();

            var last = list[0];
            var loc = list[1];

            double disttotal = 0;
            var a = 0;
            double elev = 0;
            double height = 0;

            var dist = last.GetDistance(loc);

            var points = (int) (dist / 10) + 1;

            var deltalat = last.Lat - loc.Lat;
            var deltalng = last.Lng - loc.Lng;

            var steplat = deltalat / points;
            var steplng = deltalng / points;

            var lastpnt = last;
            while (a <= points && elev < homealt + base_height + height)
            {
                var lat = last.Lat - steplat * a;
                var lng = last.Lng - steplng * a;

                var newpoint = new PointLatLngAlt(lat, lng, srtm.getAltitude(lat, lng).alt, "");

                var subdist = lastpnt.GetDistance(newpoint);

                disttotal += subdist;

                height = disttotal * Math.Tan(triangle);

                // srtm alts
                answer = newpoint;
                elev = newpoint.Alt;
                lastpnt = newpoint;
                a++;
            }

            if (elev < homealt + base_height + height + 0.1 && elev > homealt + base_height + height - 0.1
            ) //Terrain intercept found
            {
                carryon = false;
            }

            else
            {
                if (elev > homealt + base_height + height)
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