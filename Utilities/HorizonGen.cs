using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET;
using MissionPlanner.Controls;

namespace MissionPlanner.Utilities
{
    public class HorizonGen
    {
        PointLatLngAlt gelocs = new PointLatLngAlt();
        List<PointLatLngAlt> srtmlocs = new List<PointLatLngAlt>();
        List<PointLatLngAlt> pointends = new List<PointLatLngAlt>();
        PointLatLng point = new PointLatLng();

        double homealt;

        bool carryon = true;
        double range;
        double base_height;

        public HorizonGen(PointLatLng location, List<PointLatLng> pointslist, double alt)
        {
            double distance = 0; //km
            double latend = 0;
            double lngend = 0;
            double latradians = location.Lat * Math.PI / 180;
            double lngradians = location.Lng * Math.PI / 180;

            homealt = alt;
            range = Settings.Instance.GetInt32("NUM_range");
            base_height = Settings.Instance.GetInt32("NUM_height");

            for (float angle = 0; angle <= 2 * (float)Math.PI; angle += (float)Math.PI / 180* Settings.Instance.GetInt32("Rotational"))
            {
                carryon = true;
                float triangle = 85 * (float)Math.PI / 180 ;

                while (carryon && triangle >= 0)
                {
                    pointends.Clear();
                    pointends.Add(location);

                    distance = (range * Math.Cos(triangle)) / 6371;

                    latend = Math.Asin(Math.Sin(latradians) * Math.Cos(distance) + Math.Cos(latradians) * Math.Sin(distance) * Math.Cos(angle));
                    if (Math.Cos(latend) == 0)
                        lngend = lngradians;      // endpoint a pole
                    else
                        lngend = mod(lngradians - Math.Asin(Math.Sin(angle) * Math.Sin(distance) / Math.Cos(latend)) + Math.PI, 2 * Math.PI) - Math.PI;

                    double newlatend = latend * 180 / Math.PI;
                    double newlngend = lngend * 180 / Math.PI;

                    pointends.Add(new PointLatLngAlt(newlatend, newlngend, 0, (1).ToString()));
                    point = getSRTMAltPath(pointends, triangle); //DEM data

                    triangle -= ((float)Math.PI / 180) * Settings.Instance.GetInt32("Angular");
                }

                pointslist.Add(point);
            }
            pointslist.Add(pointslist[0]);
        }

        double mod(double a, double n)
        {
            double result = a % n;
            if ((a < 0 && n > 0) || (a > 0 && n < 0))
                result += n;
            return result;
        }

        PointLatLngAlt getSRTMAltPath(List<PointLatLngAlt> list, float triangle)
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
            while (a <= points && elev < homealt + base_height + height)
            {
                double lat = last.Lat - steplat * a;
                double lng = last.Lng - steplng * a;

                var newpoint = new PointLatLngAlt(lat, lng, srtm.getAltitude(lat, lng).alt, "");

                double subdist = lastpnt.GetDistance(newpoint);

                disttotal += subdist;

                height = disttotal * Math.Tan(triangle);

                // srtm alts
                answer = newpoint;
                elev = newpoint.Alt;
                lastpnt = newpoint;
                a++;
            }

            if (a <= points)
            {
                carryon = false;
            }

            return answer;
        }
    }
}
