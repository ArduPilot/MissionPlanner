using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Utilities;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace MissionPlanner
{
    public class Grid
    {
        public static MissionPlanner.Plugin.PluginHost Host2;

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public struct linelatlng
        {
            // start of line
            public utmpos p1;
            // end of line
            public utmpos p2;
            // used as a base for grid along line (initial setout)
            public utmpos basepnt;
        }

        public enum StartPosition
        {
            Home = 0,
            BottomLeft = 1,
            TopLeft = 2,
            BottomRight = 3,
            TopRight = 4
        }

        static void addtomap(linelatlng pos)        //need to figure out what should I do with this...
        {
            return;
            //List<PointLatLng> list = new List<PointLatLng>();
            //list.Add(pos.p1.ToLLA());
            //list.Add(pos.p2.ToLLA());

         //   polygons.Routes.Add(new GMapRoute(list, "test") { Stroke = new System.Drawing.Pen(System.Drawing.Color.Yellow,4) });
            
            //.Markers.Add(new GMapMarkerGoogleRed(pnt));

            //map.ZoomAndCenterRoutes("polygons");

           // map.Invalidate();
        }


        /// <summary>
        /// this is a debug function
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="tag"></param>
        static void addtomap(utmpos pos, string tag)
        {
            //tag = (no++).ToString();
            //polygons.Markers.Add(new GMapMarkerGoogleRed(pos.ToLLA()));// { ToolTipText = tag, ToolTipMode = MarkerTooltipMode.Always } );

            //map.ZoomAndCenterMarkers("polygons");

            //map.Invalidate();
        }

        public static List<PointLatLngAlt> CreateGrid(List<PointLatLngAlt> polygon, double altitude, 
            double distance, double spacing, double angle, double overshoot1,double overshoot2, 
            StartPosition startpos, bool shutter, float minLaneSeparation, float leadin = 0)
        {
            if (spacing < 10 && spacing != 0)
                spacing = 10;

            if (distance < 5)
                distance = 5;

            if (polygon.Count == 0)     //I think this returns a list of nothing if there are no WPs to make a polygon
                return new List<PointLatLngAlt>();

            
            // Make a non round number in case of corner cases      //need to figure out why we needed to do this
            if (minLaneSeparation != 0)
                minLaneSeparation += 0.5F;
            // Lane Separation in meters
            double minLaneSeparationINMeters = minLaneSeparation * distance;

            List<PointLatLngAlt> ans = new List<PointLatLngAlt>();      //what was this used for...?

            // utm zone distance calcs will be done in
            int utmzone = polygon[0].GetUTMZone();

            // utm position list
            List<utmpos> utmpositions = utmpos.ToList(PointLatLngAlt.ToUTM(utmzone, polygon), utmzone);

            // close the loop if its not already
            if (utmpositions[0] != utmpositions[utmpositions.Count - 1])
                utmpositions.Add(utmpositions[0]); // make a full loop

            // get mins/maxs of coverage area
            Rect area = getPolyMinMax(utmpositions);

            // get initial grid

            // used to determine the size of the outer grid area
            double diagdist = area.DiagDistance();

            // somewhere to store our generated lines
            List<linelatlng> grid = new List<linelatlng>();
            // number of lines we need
            int lines = 0;

            // get start point middle
            double x = area.MidWidth;
            double y = area.MidHeight;

            //addtomap(new utmpos(x, y, utmzone),"Base");  //this function is all commented out...? so I commented this line out

            // get left extent
            double xb1 = x;
            double yb1 = y;
            // to the left
            newpos(ref xb1, ref yb1, angle - 90, diagdist / 2 + distance);
            // backwards
            newpos(ref xb1, ref yb1, angle + 180, diagdist / 2 + distance);

            utmpos left = new utmpos(xb1, yb1, utmzone);

            //addtomap(left, "left");   //function doesn't do anything either

            // get right extent
            double xb2 = x;
            double yb2 = y;
            // to the right
            newpos(ref xb2, ref yb2, angle + 90, diagdist / 2 + distance);
            // backwards
            newpos(ref xb2, ref yb2, angle + 180, diagdist / 2 + distance);

            utmpos right = new utmpos(xb2, yb2, utmzone);

            //addtomap(right,"right");  //this also does nothing

            // set start point to left hand side
            x = xb1;
            y = yb1;

            // draw the outergrid, this is a grid that cover the entire area of the rectangle plus more.
            //this while loop is actually really important, it allows for the grid lines to be stored to output later on
            while (lines < ((diagdist + distance * 2) / distance))
            {
                // copy the start point to generate the end point
                double nx = x;
                double ny = y;
                newpos(ref nx, ref ny, angle, diagdist + distance*2);

                linelatlng line = new linelatlng();
                line.p1 = new utmpos(x, y, utmzone);
                line.p2 = new utmpos(nx, ny, utmzone);
                line.basepnt = new utmpos(x, y, utmzone);
                grid.Add(line);       //this code line is adding the lines produced above to the line list<T>

               //addtomap(line);      //not being used either

                newpos(ref x, ref y, angle + 90, distance);
                lines++;              //incrementing the counter for the number of lines that was initialized above
            }

            // find intersections with our polygon

            // store lines that dont have any intersections
            //takes only the important lines and doesn't put them in the remove list because we will need them
            //the remove list are the lines that were created as extras that are to be removed from the full list later
            List<linelatlng> remove = new List<linelatlng>();

            int gridno = grid.Count;    //the number of grid (lines~?) in the list<T>

            // cycle through our grid
            for (int a = 0; a < gridno; a++)        //going through the whole grid lines
            {
                double closestdistance = double.MaxValue;
                double farestdistance = double.MinValue;

                utmpos closestpoint = utmpos.Zero;
                utmpos farestpoint = utmpos.Zero;

                // somewhere to store our intersections
                List<utmpos> matchs = new List<utmpos>();

                int b = -1;         //why initialize to a -1??
                int crosses = 0;
                utmpos newutmpos = utmpos.Zero;
                foreach (utmpos pnt in utmpositions)        //how does this work exactly...?
                {
                    b++;
                    if (b == 0)     //it will always be 0 the first time around...? So why do we need this
                    {
                        continue;   //goes back to the beginning of the for loop and this only runs once
                    }
                    //zero would be returned into the newutmpos below if the lines were parallel
                    newutmpos = FindLineIntersection(utmpositions[b - 1], utmpositions[b], grid[a].p1, grid[a].p2);
                    if (!newutmpos.IsZero)      //basically means if newutmpos is not 0, i.e.: if (newutmpos != 0)
                    {                           //so intersection was found if this line runs
                        crosses++;              //increment counter for the number of line internsections
                        matchs.Add(newutmpos);  //store intersection into the intersection list matchs
                                                //now try to find if you can update the closer and farther variables
                        if (closestdistance > grid[a].p1.GetDistance(newutmpos))  //find a closer closestdistance
                        {
                            closestpoint.y = newutmpos.y;
                            closestpoint.x = newutmpos.x;
                            closestpoint.zone = newutmpos.zone;
                            closestdistance = grid[a].p1.GetDistance(newutmpos);
                        }
                        if (farestdistance < grid[a].p1.GetDistance(newutmpos))   //find a farther farthest distance
                        {
                            farestpoint.y = newutmpos.y;
                            farestpoint.x = newutmpos.x;
                            farestpoint.zone = newutmpos.zone;
                            farestdistance = grid[a].p1.GetDistance(newutmpos);
                        }
                    }
                }
                if (crosses == 0) // outside our polygon    
                {                 //all of the lines created are outside of our polygon (made by the waypoints~?)
                    if (!PointInPolygon(grid[a].p1, utmpositions) && !PointInPolygon(grid[a].p2, utmpositions))
                        remove.Add(grid[a]);    //I think this adds the lines with no intersections to the remove list<T>
                }                               //which stores lines that don't have any intersections
                else if (crosses == 1) // bad - shouldnt happen
                {   //the line crosses one part of the polygon which means its a random line barely touching the polygon area
                    //should nothing be done is crosses = 1 ?
                }
                else if (crosses == 2) // simple start and finish
                {                      //there is just a straight line from point A to point B and then the flight is done
                    linelatlng line = grid[a];
                    line.p1 = closestpoint;     //set p1 (start) to the closer of the two crosses
                    line.p2 = farestpoint;      //set p2 (end) to the farther of the two crosses
                    grid[a] = line;
                }
                else // multiple intersections
                {    //means we have to go back and forth through the polygon multiple times to cover all of the space/area
                    linelatlng line = grid[a];      //grid has the generated lines and a is at the first index at the beginning
                    remove.Add(line);

                    while (matchs.Count > 1)        //which there are still more than 1 matches
                    {
                        linelatlng newline = new linelatlng();      //is this for storing the lines to be used?

                        closestpoint = findClosestPoint(closestpoint, matchs);
                        newline.p1 = closestpoint;
                        matchs.Remove(closestpoint);

                        closestpoint = findClosestPoint(closestpoint, matchs);      //why is this done twice?
                        newline.p2 = closestpoint;
                        matchs.Remove(closestpoint);

                        newline.basepnt = line.basepnt;     //basepnt is used for a base for the grid along the line

                        grid.Add(newline);    //adding this newline with a new base to the grid list of generated lines (why?)
                    }
                }
            }

            // cleanup and keep only lines that pass though our polygon
            foreach (linelatlng line in remove)     //removes all the lines in the remove line list<T> from the grid list<T>
            {
                grid.Remove(line);
            }

            // debug
            foreach (linelatlng line in grid)       //adds all of the lines that are good to the map
            {
                addtomap(line);                     //however, this function doesn't do anything...
            }

            if (grid.Count == 0)                    //what is this...?
                return ans;

            utmpos startposutm;

            //setting a new start position depending on where the user chose for it to be
            switch (startpos)
            {
                default:
                case StartPosition.Home:
                    startposutm = new utmpos(Host2.cs.HomeLocation);
                    break;
                case StartPosition.BottomLeft:
                    startposutm = new utmpos(area.Left, area.Bottom, utmzone);
                    break;
                case StartPosition.BottomRight:
                    startposutm = new utmpos(area.Right, area.Bottom, utmzone);
                    break;
                case StartPosition.TopLeft:
                    startposutm = new utmpos(area.Left, area.Top, utmzone);
                    break;
                case StartPosition.TopRight:
                    startposutm = new utmpos(area.Right, area.Top, utmzone);
                    break;

            }

            // find closest line point to startpos
            linelatlng closest = findClosestLine(startposutm, grid, 0 /*Lane separation does not apply to starting point*/, angle);

            utmpos lastpnt;

            // get the closes point from the line we picked
            if (closest.p1.GetDistance(startposutm) < closest.p2.GetDistance(startposutm))
            {
                lastpnt = closest.p1;
            }
            else
            {
                lastpnt = closest.p2;
            }

            while (grid.Count > 0)
            {
                // for each line, check which end of the line is the next closest
                if (closest.p1.GetDistance(lastpnt) < closest.p2.GetDistance(lastpnt))  //is the closer point of the line is already p1
                {
                    utmpos newstart = newpos(closest.p1, angle, -leadin);

                    addtomap(newstart, "S");
                    ans.Add(newstart);

                    if (spacing > 0)        //need to look and figure out what this all does later though
                    {
                        for (int d = (int)(spacing - ((closest.basepnt.GetDistance(closest.p1)) % spacing));
                            d < (closest.p1.GetDistance(closest.p2));
                            d += (int)spacing)
                        {
                            double ax = closest.p1.x;
                            double ay = closest.p1.y;

                            newpos(ref ax, ref ay, angle, d);
                            addtomap(new utmpos(ax,ay,utmzone),"M");
                            ans.Add((new utmpos(ax, ay, utmzone) { Tag = "M" }));
                        }
                    }


                    utmpos newend = newpos(closest.p2, angle, overshoot1);
                    addtomap(newend, "E");
                    ans.Add(newend);

                    lastpnt = closest.p2;

                    grid.Remove(closest);
                    if (grid.Count == 0)        //all lines have been added once the count reaches zero
                        break;

                    closest = findClosestLine(newend, grid, minLaneSeparationINMeters, angle);      //gets next closest line
                }
                else        //probably for when you need to change a line or switch the start and end of a line to match the previous one
                {
                    utmpos newstart = newpos(closest.p2, angle, leadin);
                    addtomap(newstart, "E");
                    ans.Add(newstart);

                    if (spacing > 0)
                    {
                        for (int d = (int)((closest.basepnt.GetDistance(closest.p2)) % spacing);
                            d < (closest.p1.GetDistance(closest.p2));
                            d += (int)spacing)
                        {
                            double ax = closest.p2.x;
                            double ay = closest.p2.y;

                            newpos(ref ax, ref ay, angle, -d);
                            addtomap(new utmpos(ax, ay, utmzone), "M");
                            ans.Add((new utmpos(ax, ay, utmzone) { Tag = "M" }));
                        }
                    }

                    utmpos newend = newpos(closest.p1, angle, -overshoot2);
                 //   if (overshoot2 > 0)
                 //       ans.Add(new utmpos(closest.p1) { Tag = "M" });
                    addtomap(newend, "E");
                    ans.Add(newend);

                    lastpnt = closest.p1;

                    grid.Remove(closest);
                    if (grid.Count == 0)
                        break;
                    closest = findClosestLine(newend, grid, minLaneSeparationINMeters, angle);
                }
            }

            // set the altitude on all points
            ans.ForEach(plla => { plla.Alt = altitude; });

            return ans;
        }

        static Rect getPolyMinMax(List<utmpos> utmpos)
        {
            if (utmpos.Count == 0)
                return new Rect();

            double minx, miny, maxx, maxy;

            minx = maxx = utmpos[0].x;
            miny = maxy = utmpos[0].y;

            foreach (utmpos pnt in utmpos)
            {
                minx = Math.Min(minx, pnt.x);
                maxx = Math.Max(maxx, pnt.x);

                miny = Math.Min(miny, pnt.y);
                maxy = Math.Max(maxy, pnt.y);
            }

            return new Rect(minx, maxy, maxx - minx,miny - maxy);
        }

        // polar to rectangular
        static void newpos(ref double x, ref double y, double bearing, double distance)
        {
            double degN = 90 - bearing;
            if (degN < 0)
                degN += 360;
            x = x + distance * Math.Cos(degN * deg2rad);
            y = y + distance * Math.Sin(degN * deg2rad);
        }

        // polar to rectangular
        static utmpos newpos(utmpos input, double bearing, double distance)
        {
            double degN = 90 - bearing;
            if (degN < 0)
                degN += 360;
            double x = input.x + distance * Math.Cos(degN * deg2rad);
            double y = input.y + distance * Math.Sin(degN * deg2rad);

            return new utmpos(x, y, input.zone);
        }

        /// <summary>
        /// from http://stackoverflow.com/questions/1119451/how-to-tell-if-a-line-intersects-a-polygon-in-c
        /// </summary>
        /// <param name="start1"></param>
        /// <param name="end1"></param>
        /// <param name="start2"></param>
        /// <param name="end2"></param>
        /// <returns></returns>
        public static utmpos FindLineIntersection(utmpos start1, utmpos end1, utmpos start2, utmpos end2)
        {
            double denom = ((end1.x - start1.x) * (end2.y - start2.y)) - ((end1.y - start1.y) * (end2.x - start2.x));
            //  AB & CD are parallel         
            if (denom == 0)
                return utmpos.Zero;
            double numer = ((start1.y - start2.y) * (end2.x - start2.x)) - ((start1.x - start2.x) * (end2.y - start2.y));
            double r = numer / denom;
            double numer2 = ((start1.y - start2.y) * (end1.x - start1.x)) - ((start1.x - start2.x) * (end1.y - start1.y));
            double s = numer2 / denom;
            if ((r < 0 || r > 1) || (s < 0 || s > 1))
                return utmpos.Zero;
            // Find intersection point      
            utmpos result = new utmpos();
            result.x = start1.x + (r * (end1.x - start1.x));
            result.y = start1.y + (r * (end1.y - start1.y));
            result.zone = start1.zone;
            return result;
        }

        static utmpos findClosestPoint(utmpos start, List<utmpos> list)
        {
            utmpos answer = utmpos.Zero;
            double currentbest = double.MaxValue;

            foreach (utmpos pnt in list)
            {
                double dist1 = start.GetDistance(pnt);

                if (dist1 < currentbest)
                {
                    answer = pnt;
                    currentbest = dist1;
                }
            }

            return answer;
        }

        // Add an angle while normalizing output in the range 0...360
        static double AddAngle(double angle, double degrees)
        {
            angle += degrees;

            angle = angle % 360;

            while (angle < 0)
            {
                angle += 360;
            }
            return angle;
        }

        static linelatlng findClosestLine(utmpos start, List<linelatlng> list, double minDistance, double angle)
        {
            // By now, just add 5.000 km to our lines so they are long enough to allow intersection
            double METERS_TO_EXTEND = 5000000;

            double perperndicularOrientation = AddAngle(angle, 90);

            // Calculation of a perpendicular line to the grid lines containing the "start" point
            /*
             *  --------------------------------------|------------------------------------------
             *  --------------------------------------|------------------------------------------
             *  -------------------------------------start---------------------------------------
             *  --------------------------------------|------------------------------------------
             *  --------------------------------------|------------------------------------------
             *  --------------------------------------|------------------------------------------
             *  --------------------------------------|------------------------------------------
             *  --------------------------------------|------------------------------------------
             */
            utmpos start_perpendicular_line = newpos(start, perperndicularOrientation, -METERS_TO_EXTEND);
            utmpos stop_perpendicular_line = newpos(start, perperndicularOrientation, METERS_TO_EXTEND);

            // Store one intersection point per grid line
            Dictionary<utmpos, linelatlng> intersectedPoints = new Dictionary<utmpos, linelatlng>();
            // lets order distances from every intersected point per line with the "start" point
            Dictionary<double, utmpos> ordered_min_to_max = new Dictionary<double, utmpos>();

            foreach (linelatlng line in list)
            {
                // Extend line at both ends so it intersecs for sure with our perpendicular line
                utmpos extended_line_start = newpos(line.p1, angle, -METERS_TO_EXTEND);
                utmpos extended_line_stop = newpos(line.p2, angle, METERS_TO_EXTEND);
                // Calculate intersection point
                utmpos p = FindLineIntersection(extended_line_start, extended_line_stop, start_perpendicular_line, stop_perpendicular_line);
                
                // Store it
                intersectedPoints[p] = line;

                // Calculate distances between interesected point and "start" (i.e. line and start)
                double distance_p = start.GetDistance(p);
                if (!ordered_min_to_max.ContainsKey(distance_p))
                    ordered_min_to_max.Add(distance_p, p);
            }
     
            // Acquire keys and sort them.
            List<double> ordered_keys = ordered_min_to_max.Keys.ToList();
            ordered_keys.Sort();

            // Lets select a line that is the closest to "start" point but "mindistance" away at least.
            // If we have only one line, return that line whatever the minDistance says
            double key = double.MaxValue;
            int i = 0;
            while (key == double.MaxValue && i < ordered_keys.Count)
            {
                if (ordered_keys[i] >= minDistance)
                    key = ordered_keys[i];
                i++;
            }

            // If no line is selected (because all of them are closer than minDistance, then get the farest one
            if (key == double.MaxValue)
                key = ordered_keys[ordered_keys.Count-1];

            // return line
            return intersectedPoints[ordered_min_to_max[key]];

        }

        static bool PointInPolygon(utmpos p, List<utmpos> poly)
        {
            utmpos p1, p2;
            bool inside = false;

            if (poly.Count < 3)
            {
                return inside;
            }
            utmpos oldPoint = new utmpos(poly[poly.Count - 1]);

            for (int i = 0; i < poly.Count; i++)
            {

                utmpos newPoint = new utmpos(poly[i]);

                if (newPoint.y > oldPoint.y)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.y < p.y) == (p.y <= oldPoint.y)
                    && ((double)p.x - (double)p1.x) * (double)(p2.y - p1.y)
                    < ((double)p2.x - (double)p1.x) * (double)(p.y - p1.y))
                {
                    inside = !inside;
                }
                oldPoint = newPoint;
            }
            return inside;
        }


    }
}
