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
        //
        // RF Propagation
        //

        PointLatLngAlt gelocs = new PointLatLngAlt();
        List<PointLatLngAlt> srtmlocs = new List<PointLatLngAlt>();
        List<PointLatLngAlt> pointends = new List<PointLatLngAlt>();
        PointLatLng point = new PointLatLng();

        double homealt;
        double drone_alt;
        double clearance;

        bool carryon = true;
        bool up = false;
        bool down = false;

        double range;
        double base_height;

        public SightGen(PointLatLng location, List<PointLatLng> pointslist, double alt, double aircraft_alt)
        {
            double distance = 0; //km
            double latend = 0;
            double lngend = 0;
            double latradians = location.Lat * Math.PI / 180;
            double lngradians = location.Lng * Math.PI / 180;

            float Max = 90 * (float)Math.PI / 180;  //radians
            float Min = 0;                          //radians


            range = Settings.Instance.GetFloat("NUM_range");
            clearance = Settings.Instance.GetFloat("Clearance");
            base_height = Settings.Instance.GetFloat("NUM_height");

            drone_alt = aircraft_alt;

            homealt = alt;

            for (float angle = 0; angle <= 2 * (float)Math.PI; angle += (float)Math.PI / 180 * Settings.Instance.GetInt32("Rotational")) //Terrain intercept scan full 360
            {
                carryon = true;
                
                up = false;
                down = false;

                float triangle = 45 * (float)Math.PI / 180; //radians
                Max = 90 * (float)Math.PI / 180;            //radians
                Min = 0;                                    //radians


                while (carryon && Min + Settings.Instance.GetInt32("Converge") * (float)Math.PI/180 < Max) //Breaks if terrain intercept found or convergence occurs
                {
                    pointends.Clear();
                    pointends.Add(location);

                    distance = (range * Math.Cos(triangle)) / 6371; //Km

                    latend = Math.Asin(Math.Sin(latradians) * Math.Cos(distance) + Math.Cos(latradians) * Math.Sin(distance) * Math.Cos(angle));
                    if (Math.Cos(latend) == 0)
                        lngend = lngradians;      // endpoint a pole
                    else
                        lngend = mod(lngradians - Math.Asin(Math.Sin(angle) * Math.Sin(distance) / Math.Cos(latend)) + Math.PI, 2 * Math.PI) - Math.PI;

                    double newlatend = latend * 180 / Math.PI;
                    double newlngend = lngend * 180 / Math.PI;

                    pointends.Add(new PointLatLngAlt(newlatend, newlngend, 0, (1).ToString()));
                    point = getSRTMAltPath(pointends,triangle); //DEM data

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

        PointLatLngAlt getSRTMAltPath(List<PointLatLngAlt> list,float triangle)
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
            while (a <= points && elev < homealt+ base_height + height && elev < homealt + drone_alt-clearance)
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
                && elev < homealt + drone_alt - clearance + 0.1 && elev > homealt + drone_alt - clearance -0.1) //Terrain intercept found
            {
                carryon = false;
            }

            else
            {
                if (elev > homealt + base_height + height && elev > homealt+drone_alt-clearance)
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
