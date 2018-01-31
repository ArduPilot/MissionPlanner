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
    public class SightGen
    {
        PointLatLngAlt gelocs = new PointLatLngAlt();
        List<PointLatLngAlt> srtmlocs = new List<PointLatLngAlt>();
        List<PointLatLngAlt> pointends = new List<PointLatLngAlt>();

        double homealt;

        public SightGen(PointLatLng location, List<PointLatLng> pointslist, double alt)
        {
            double distance = 0.99 / 6371; //km
            double latend = 0;
            double lngend = 0;
            double latradians = location.Lat * Math.PI / 180;
            double lngradians = location.Lng * Math.PI / 180;

            homealt = alt;

            for (float angle = 0; angle <= 2 * (float)Math.PI; angle += (float)Math.PI / 180)
            {
                pointends.Clear();
                pointends.Add(location);

                latend = Math.Asin(Math.Sin(latradians) * Math.Cos(distance) + Math.Cos(latradians) * Math.Sin(distance) * Math.Cos(angle));
                if (Math.Cos(latradians) == 0)
                    lngend = lngradians;      // endpoint a pole
                else
                    lngend = mod(lngradians - Math.Asin(Math.Sin(angle) * Math.Sin(distance) / Math.Cos(latend)) + Math.PI, 2 * Math.PI) - Math.PI;

                double newlatend = latend * 180 / Math.PI;
                double newlngend = lngend * 180 / Math.PI;

                pointends.Add(new PointLatLngAlt(newlatend, newlngend, 0, (1).ToString()));
                pointslist.Add(getSRTMAltPath(pointends)); //DEM data
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

        PointLatLngAlt getSRTMAltPath(List<PointLatLngAlt> list)
        {
            PointLatLngAlt answer = new PointLatLngAlt();

            PointLatLngAlt last = list[0];
            PointLatLngAlt loc = list[1];

            double disttotal = 0;
            int a = 0;
            double elev = 0;


            double dist = last.GetDistance(loc);

            int points = (int)(dist / 10) + 1;

            double deltalat = (last.Lat - loc.Lat);
            double deltalng = (last.Lng - loc.Lng);

            double steplat = deltalat / points;
            double steplng = deltalng / points;

            PointLatLngAlt lastpnt = last;
            while (a <= points && elev < homealt+20)
            {
                double lat = last.Lat - steplat * a;
                double lng = last.Lng - steplng * a;

                var newpoint = new PointLatLngAlt(lat, lng, srtm.getAltitude(lat, lng).alt, "");

                double subdist = lastpnt.GetDistance(newpoint);

                disttotal += subdist;

                // srtm alts
                answer = newpoint;
                elev = newpoint.Alt;
                lastpnt = newpoint;
                a++;   
            }

            return answer;
        }
    }
}
