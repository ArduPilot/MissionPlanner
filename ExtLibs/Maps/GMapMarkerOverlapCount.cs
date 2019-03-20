using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using SvgNet.SvgGdi;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerOverlapCount : GMapMarker
    {
        List<GMapPolygon> footprintpolys = new List<GMapPolygon>();

        RectLatLng area;

        readonly Dictionary<PointLatLng, int> overlapCount = new Dictionary<PointLatLng, int>();

        public GMapMarkerOverlapCount(PointLatLng p)
            : base(p)
        {
            area = new RectLatLng(p, SizeLatLng.Empty);

        }

        static Color[] color = new[]
        {
            Color.Purple,
            Color.Blue,
            Color.Aqua,
            Color.Green,
            Color.Yellow,
            Color.Orange,
            Color.Red,
            Color.DarkRed
        };

        static object[] colorbrushs;

        public override void OnRender(IGraphics g)
        {
            if (colorbrushs == null)
            {
                colorbrushs = new SolidBrush[color.Length];
                int a = 0;
                foreach (var color1 in color)
                {
                    colorbrushs[a] = new SolidBrush(Color.FromArgb(140, color1.R, color1.G, color1.B));
                    a++;
                }
            }

            DateTime start = DateTime.Now;
            var pos = Overlay.Control.FromLatLngToLocal(Position);

            pos.Offset(-LocalPosition.X, -LocalPosition.Y);

            double width =
                (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                    Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0))*1000.0);
            double m2pixelwidth = Overlay.Control.Width/width;

            Rect screenRect = new Rect(Overlay.Control.Width/-2, Overlay.Control.Height/-2, Overlay.Control.Width,
                Overlay.Control.Height);

            int skipped = 0;

            foreach (var pg in overlapCount)
            {
                GPoint p = Overlay.Control.FromLatLngToLocal(pg.Key);

                p.Offset(-pos.X, -pos.Y);

                if (p.X < screenRect.Left || p.X > screenRect.Right ||
                    p.Y < screenRect.Top || p.Y > screenRect.Bottom)
                {
                    skipped++;
                    continue;
                }

                var col = Math.Min(pg.Value - 1, 7);

                var coloruse = colorbrushs[col] as SolidBrush;

                var widthc = 5*m2pixelwidth;

                var halfwidthc = widthc/2.0f;

                g.FillPie(coloruse, (float) (p.X - halfwidthc), (float) (p.Y - halfwidthc), (float) (widthc),
                    (float) (widthc), 0, 360);
            }

            drawLegend(g);
            Console.WriteLine("OnRender "+(DateTime.Now - start));
        }

        public void Add(GMapPolygon footprint)
        {
            DateTime start = DateTime.Now;
            // if the same name footprint exists exit
            if (footprintpolys.Any(p => p.Name == footprint.Name))
            {
                return;
            }

            // if this is the first entry reset area
            if (footprintpolys.Count == 0)
            {
                Position = footprint.Points[0];
                area = new RectLatLng(footprint.Points[0], new SizeLatLng(0, 0));
            }

            // add the new footprint
            footprintpolys.Add(footprint);

            // recalc the area
            foreach (var point in footprint.Points)
            {
                if (!area.Contains(point))
                {
                    double tllat = Math.Max(area.Lat, point.Lat);
                    double tllng = Math.Min(area.Lng, point.Lng);
                    double brlat = Math.Min(area.Bottom, point.Lat);
                    double brlng = Math.Max(area.Right, point.Lng);
                    // enlarge the area
                    area = RectLatLng.FromLTRB(tllng, tllat, brlng, brlat);
                }
            }

            generateCoverageFP(footprint);

            Console.WriteLine("Add "+(DateTime.Now - start));
        }

        public void drawLegend(IGraphics g)
        {
            // top left corner
            var tl = Overlay.Control.FromLocalToLatLng(0, 0);
            // top right corner
            var tr = Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0);
            // width in m
            double width = Overlay.Control.MapProvider.Projection.GetDistance(tl, tr)*1000.0;
            // meters per pixel
            double m2pixelwidth = Overlay.Control.Width/width;

            var widthc = 20;

            var halfwidthc = widthc/2.0f;

            var pos = Overlay.Control.FromLatLngToLocal(Position);

            pos.Offset(-LocalPosition.X, -LocalPosition.Y);

            using (StringFormat stringFormat = new StringFormat())
            {
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                int a = 0;
                foreach (Brush brush in colorbrushs)
                {
                    GPoint p = new GPoint(20, (long) (100 + a*(widthc + 5)));

                    p.Offset(-pos.X, -pos.Y);

                    g.FillPie(brush, (float) (p.X - halfwidthc), (float) (p.Y - halfwidthc), (float) (widthc),
                        (float) (widthc), 0, 360);

                    g.DrawString((a + 1).ToString(), SystemFonts.DefaultFont, Brushes.White,
                        new RectangleF((float) (p.X - halfwidthc), (float) (p.Y - halfwidthc), (float) widthc,
                            (float) widthc), stringFormat);
                    a++;
                }
            }
        }

        public void generateCoverageAll()
        {
            overlapCount.Clear();

            if (area.WidthLng > 1 || area.HeightLat > 1)
                return;

            for (double lat = Math.Round(area.Lat, 4); lat >= area.Bottom; lat -= 0.0001)
            {
                for (double lng = Math.Round(area.Lng, 4); lng <= area.Right; lng += 0.0001)
                {
                    var p = new PointLatLng(Math.Round(lat, 4), Math.Round(lng, 4));

                    foreach (var footprintpoly in footprintpolys)
                    {
                        if (footprintpoly.IsInside(p))
                        {
                            if (overlapCount.ContainsKey(p))
                            {
                                overlapCount[p]++;
                            }
                            else
                            {
                                overlapCount[p] = 1;
                            }
                        }
                    }
                }
            }
        }

        public void generateCoverageFP(GMapPolygon footprintpoly)
        {
            if (area.WidthLng > 1 || area.HeightLat > 1)
                return;

            for (double lat = Math.Round(area.Lat, 4); lat >= area.Bottom; lat -= 0.0001)
            {
                for (double lng = Math.Round(area.Lng, 4); lng <= area.Right; lng += 0.0001)
                {
                    var p = new PointLatLng(Math.Round(lat, 4), Math.Round(lng, 4));

                    if (footprintpoly.IsInside(p))
                    {
                        if (overlapCount.ContainsKey(p))
                        {
                            overlapCount[p]++;
                        }
                        else
                        {
                            overlapCount[p] = 1;
                        }
                    }
                }

            }
        }

        public void Clear()
        {
            footprintpolys.Clear();
            overlapCount.Clear();
        }
    }
}