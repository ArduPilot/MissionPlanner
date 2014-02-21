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

namespace MissionPlanner.SimpleGrid
{
    public class Grid
    {
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

        static void addtomap(linelatlng pos)
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


        static void addtomap(utmpos pos, string tag)
        {
            return;
            //tag = (no++).ToString();
          //  polygons.Markers.Add(new GMapMarkerGoogleRed(pos.ToLLA()));// { ToolTipText = tag, ToolTipMode = MarkerTooltipMode.Always } );

            //map.ZoomAndCenterMarkers("polygons");

            //map.Invalidate();
        }

        public static List<PointLatLngAlt> CreateGrid(List<PointLatLngAlt> polygon, double altitude, double distance, double spacing, double angle, double overshoot1,double overshoot2, StartPosition startpos, bool shutter)
        {
            if (spacing < 10 && spacing != 0)
                spacing = 10;

            List<PointLatLngAlt> ans = new List<PointLatLngAlt>();

            int utmzone = polygon[0].GetUTMZone();

            List<utmpos> utmpositions = utmpos.ToList(PointLatLngAlt.ToUTM(utmzone, polygon), utmzone);

            if (utmpositions[0] != utmpositions[utmpositions.Count - 1])
                utmpositions.Add(utmpositions[0]); // make a full loop

            Rect area = getPolyMinMax(utmpositions);

            // get initial grid

            // used to determine the size of the outer grid area
            double diagdist = area.DiagDistance();

            // somewhere to store out generated lines
            List<linelatlng> grid = new List<linelatlng>();
            // number of lines we need
            int lines = 0;

            // get start point bottom left
            double x = area.MidWidth;
            double y = area.MidHeight;

            addtomap(new utmpos(x, y, utmzone),"Base");

            // get left extent
            double xb1 = x;
            double yb1 = y;
            // to the left
            newpos(ref xb1, ref yb1, angle - 90, diagdist / 2 + distance);
            // backwards
            newpos(ref xb1, ref yb1, angle + 180, diagdist / 2 + distance);

            utmpos left = new utmpos(xb1, yb1, utmzone);

            addtomap(left, "left");

            // get right extent
            double xb2 = x;
            double yb2 = y;
            // to the right
            newpos(ref xb2, ref yb2, angle + 90, diagdist / 2 + distance);
            // backwards
            newpos(ref xb2, ref yb2, angle + 180, diagdist / 2 + distance);

            utmpos right = new utmpos(xb2, yb2, utmzone);

            addtomap(right,"right");

            // set start point to left hand side
            x = xb1;
            y = yb1;

            // draw the outergrid, this is a grid that cover the entire area of the rectangle plus more.
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
                grid.Add(line);

               // addtomap(line);

                newpos(ref x, ref y, angle + 90, distance);
                lines++;
            }

            // find intersections with our polygon

            // store lines that dont have any intersections
            List<linelatlng> remove = new List<linelatlng>();

            int gridno = grid.Count;

            // cycle through our grid
            for (int a = 0; a < gridno; a++)
            {
                double closestdistance = double.MaxValue;
                double farestdistance = double.MinValue;

                utmpos closestpoint = utmpos.Zero;
                utmpos farestpoint = utmpos.Zero;

                // somewhere to store our intersections
                List<utmpos> matchs = new List<utmpos>();

                int b = -1;
                int crosses = 0;
                utmpos newutmpos = utmpos.Zero;
                foreach (utmpos pnt in utmpositions)
                {
                    b++;
                    if (b == 0)
                    {
                        continue;
                    }
                    newutmpos = FindLineIntersection(utmpositions[b - 1], utmpositions[b], grid[a].p1, grid[a].p2);
                    if (!newutmpos.IsZero)
                    {
                        crosses++;
                        matchs.Add(newutmpos);
                        if (closestdistance > grid[a].p1.GetDistance(newutmpos))
                        {
                            closestpoint.y = newutmpos.y;
                            closestpoint.x = newutmpos.x;
                            closestpoint.zone = newutmpos.zone;
                            closestdistance = grid[a].p1.GetDistance(newutmpos);
                        }
                        if (farestdistance < grid[a].p1.GetDistance(newutmpos))
                        {
                            farestpoint.y = newutmpos.y;
                            farestpoint.x = newutmpos.x;
                            farestpoint.zone = newutmpos.zone;
                            farestdistance = grid[a].p1.GetDistance(newutmpos);
                        }
                    }
                }
                if (crosses == 0) // outside our polygon
                {
                    if (!PointInPolygon(grid[a].p1, utmpositions) && !PointInPolygon(grid[a].p2, utmpositions))
                        remove.Add(grid[a]);
                }
                else if (crosses == 1) // bad - shouldnt happen
                {

                }
                else if (crosses == 2) // simple start and finish
                {
                    linelatlng line = grid[a];
                    line.p1 = closestpoint;
                    line.p2 = farestpoint;
                    grid[a] = line;
                }
                else // multiple intersections
                {
                    linelatlng line = grid[a];
                    remove.Add(line);

                    while (matchs.Count > 1)
                    {
                        linelatlng newline = new linelatlng();

                        closestpoint = findClosestPoint(closestpoint, matchs);
                        newline.p1 = closestpoint;
                        matchs.Remove(closestpoint);

                        closestpoint = findClosestPoint(closestpoint, matchs);
                        newline.p2 = closestpoint;
                        matchs.Remove(closestpoint);

                        newline.basepnt = line.basepnt;

                        grid.Add(newline);
                    }
                }
            }

            foreach (linelatlng line in remove)
            {
                grid.Remove(line);
            }

            foreach (linelatlng line in grid)
            {
                addtomap(line);
            }

            if (grid.Count == 0)
                return ans;

            utmpos startposutm;

            switch (startpos)
            {
                default:
                case StartPosition.Home:
                    startposutm = new utmpos(GridPlugin.Host2.cs.HomeLocation);
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

            //return 
          //      FindPath(grid, startposutm);

            // find closest line point to home
            linelatlng closest = findClosestLine(startposutm, grid);

            utmpos lastpnt;

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
                if (closest.p1.GetDistance(lastpnt) < closest.p2.GetDistance(lastpnt))
                {

                    addtomap(closest.p1, "S");
                    ans.Add(closest.p1);

                    if (spacing > 0)
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

                          //  if (shutter.ToLower().StartsWith("y"))
                              //  AddDigicamControlPhoto();
                        }
                    }


                    utmpos newend = newpos(closest.p2, angle, overshoot1);
                  //  if (overshoot1 > 0)
                   //     ans.Add(new utmpos(closest.p2) { Tag = "M" });
                    addtomap(newend, "E");
                    ans.Add(newend);

                    lastpnt = closest.p2;

                    grid.Remove(closest);
                    if (grid.Count == 0)
                        break;
                    closest = findClosestLine(newend, grid);
                }
                else
                {
                    addtomap(closest.p2,"S");
                    ans.Add(closest.p2);

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

                           // if (shutter.ToLower().StartsWith("y"))
                            //    AddDigicamControlPhoto();
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
                    closest = findClosestLine(newend, grid);
                }
            }

            // set the altitude on all points
            ans.ForEach(plla => { plla.Alt = altitude; });

            return ans;
        }

        //http://en.wikipedia.org/wiki/Rapidly_exploring_random_tree



        // a*
        static List<PointLatLngAlt> FindPath(List<linelatlng> grid1, utmpos startposutm)
        {
            List<PointLatLngAlt> answer = new List<PointLatLngAlt>();

            List<linelatlng> closedset = new List<linelatlng>();
            List<linelatlng> openset = new List<linelatlng>(); // nodes to be travered
            Hashtable came_from = new Hashtable();
            List<linelatlng> grid = new List<linelatlng>();


            linelatlng start = new linelatlng() { p1 = startposutm, p2 = startposutm };

            grid.Add(start);
            grid.AddRange(grid1);
            openset.Add(start);

            Hashtable g_score = new Hashtable();
            Hashtable f_score = new Hashtable();
            g_score[start] = 0.0;
            f_score[start] = (double)g_score[start] + heuristic_cost_estimate(grid,0,start); // heuristic_cost_estimate(start, goal)

            linelatlng current = start;

            while (openset.Count > 0)
            {
                current = FindLowestFscore(g_score, openset); // lowest f_score
                openset.Remove(current);
                closedset.Add(current);
                foreach (var neighbor in neighbor_nodes(current, grid))
                {
                    double tentative_g_score = (double)g_score[current];

                    double dist1 = current.p1.GetDistance(neighbor.p1);
                    double dist2 = current.p1.GetDistance(neighbor.p2);
                    double dist3 = current.p2.GetDistance(neighbor.p1);
                    double dist4 = current.p2.GetDistance(neighbor.p2);

                    tentative_g_score += (dist1 + dist2 + dist3 + dist4) / 4;

                    tentative_g_score  += neighbor.p1.GetDistance(neighbor.p2);

                    //tentative_g_score += Math.Min(Math.Min(dist1, dist2), Math.Min(dist3, dist4));
                    //tentative_g_score += Math.Max(Math.Max(dist1, dist2), Math.Max(dist3, dist4));

                   // if (closedset.Contains(neighbor) && tentative_g_score >= (double)g_score[neighbor])
                   //     continue;

                    if (!closedset.Contains(neighbor) ||
                       tentative_g_score < (double)g_score[neighbor])
                    {
                        came_from[neighbor] = current;
                        g_score[neighbor] = tentative_g_score;
                        f_score[neighbor] = tentative_g_score + heuristic_cost_estimate(grid, tentative_g_score, neighbor);
                        Console.WriteLine("neighbor score: " + g_score[neighbor] + " " + f_score[neighbor]);
                        if (!openset.Contains(neighbor))
                            openset.Add(neighbor);
                    }
                }
               
            }

            // bad
            //linelatlng ans = FindLowestFscore(g_score, grid);

          //  foreach (var ans in grid)
            {
                List<linelatlng> list = reconstruct_path(came_from, current);
              //  list.Insert(0,current);
                //list.Remove(start);
                //list.Remove(start);
                Console.WriteLine("List " + list.Count + " " + g_score[current]);
               {
                   List<utmpos> temp = new List<utmpos>();
                   temp.Add(list[0].p1);
                   temp.Add(list[0].p2);
                   utmpos oldpos = findClosestPoint(startposutm, temp);

                   foreach (var item in list) 
                   {
                       double dist1 = oldpos.GetDistance(item.p1);
                       double dist2 = oldpos.GetDistance(item.p2);
                       if (dist1 < dist2)
                       {
                           answer.Add(new PointLatLngAlt(item.p1));
                           answer.Add(new PointLatLngAlt(item.p2));
                           oldpos = item.p2;
                       }
                       else
                       {
                           answer.Add(new PointLatLngAlt(item.p2));
                           answer.Add(new PointLatLngAlt(item.p1));
                           oldpos = item.p1;
                       }
                   }
                   //return answer;
               }
            }

            List<PointLatLng> list2 = new List<PointLatLng>();

            answer.ForEach(x => { list2.Add(x); });

            GMapPolygon wppoly = new GMapPolygon(list2, "Grid");

            Console.WriteLine("dist " + (wppoly.Distance));

            return answer;
        }

        static double heuristic_cost_estimate(List<linelatlng> grid, double sofar, linelatlng current_node)
        {
            double ans = 0;

            linelatlng lastx = grid[0];

            grid.ForEach(x => 
            { 
                ans += x.p1.GetDistance(x.p2);
                ans += x.p1.GetDistance(lastx.p1);
                lastx = x; 
            });



            return ans - sofar * 0.95;
        }

        static List<linelatlng> reconstruct_path(Hashtable came_from, linelatlng current_node)
        {
            List<linelatlng> ans = new List<linelatlng>();
            if (came_from.ContainsKey(current_node))
            {
                
                ans.AddRange(reconstruct_path(came_from, (linelatlng)came_from[current_node]));
                ans.Add((linelatlng)came_from[current_node]);
                return ans;
            }
            else
            {
                ans.Add(current_node);
                return ans;
            }
        }


        static private List<linelatlng> neighbor_nodes(linelatlng current, List<linelatlng> grid)
        {
            List<linelatlng> neighbors = new List<linelatlng>();

            foreach (var item in grid)
            {
               // if (item.Equals(current))
              //      continue;

                neighbors.Add(item);
            }

            return neighbors;
        }

        static private linelatlng FindLowestFscore(Hashtable f_score, List<linelatlng> openset)
        {
            linelatlng lowest = openset[0];
            int lowestint = int.MaxValue;

            foreach (linelatlng key in openset)
            {
                if (f_score.ContainsKey(key) && (double)f_score[key] < lowestint)
                {
                    lowestint = (int)(double)f_score[key];
                    lowest = key;
                }
            }

            Console.WriteLine("Lowest " + lowestint);

            return lowest;
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

        static linelatlng findClosestLine(utmpos start, List<linelatlng> list)
        {
            linelatlng answer = list[0];
            double shortest = double.MaxValue;

            foreach (linelatlng line in list)
            {
                double ans1 = start.GetDistance(line.p1);
                double ans2 = start.GetDistance(line.p2);
                utmpos shorterpnt = ans1 < ans2 ? line.p1 : line.p2;

                if (shortest > start.GetDistance(shorterpnt))
                {
                    answer = line;
                    shortest = start.GetDistance(shorterpnt);
                }
            }

            return answer;
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
