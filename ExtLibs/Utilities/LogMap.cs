using GMap.NET;
using GMap.NET.MapProviders;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MissionPlanner.Comms;
using MissionPlanner.Utilities.Drawing;
using SkiaSharp;

namespace MissionPlanner.Log
{
    public class LogMap
    {
        public static void MapLogs(string[] logs)
        {
            Parallel.ForEach(logs, logfile => { ProcessFile(logfile); });
        }

        public static void ProcessFile(string logfile)
        {
            if (File.Exists(logfile + ".jpg"))
                return;

            double minx = 99999;
            double maxx = -99999;
            double miny = 99999;
            double maxy = -99999;

            bool sitl = false;

            Dictionary<int, List<PointLatLngAlt>> loc_list = new Dictionary<int, List<PointLatLngAlt>>();

            try
            {
                if (logfile.ToLower().EndsWith(".tlog"))
                {
                    Comms.CommsFile cf = new CommsFile();
                    cf.Open(logfile);

                    using (CommsStream cs = new CommsStream(cf, cf.BytesToRead))
                    {
                        MAVLink.MavlinkParse parse = new MAVLink.MavlinkParse(true);

                        while (cs.Position < cs.Length)
                        {
                            MAVLink.MAVLinkMessage packet = parse.ReadPacket(cs);

                            if (packet == null || packet.Length < 5)
                                continue;

                            if (packet.msgid == (byte)MAVLink.MAVLINK_MSG_ID.SIM_STATE ||
                                packet.msgid == (byte)MAVLink.MAVLINK_MSG_ID.SIMSTATE)
                            {
                                sitl = true;
                            }

                            if (packet.msgid == (byte)MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT)
                            {
                                var loc = packet.ToStructure<MAVLink.mavlink_global_position_int_t>();

                                if (loc.lat == 0 || loc.lon == 0)
                                    continue;

                                var id = packet.sysid * 256 + packet.compid;

                                if (!loc_list.ContainsKey(id))
                                    loc_list[id] = new List<PointLatLngAlt>();

                                loc_list[id].Add(new PointLatLngAlt(loc.lat / 10000000.0f, loc.lon / 10000000.0f));

                                minx = Math.Min(minx, loc.lon / 10000000.0f);
                                maxx = Math.Max(maxx, loc.lon / 10000000.0f);
                                miny = Math.Min(miny, loc.lat / 10000000.0f);
                                maxy = Math.Max(maxy, loc.lat / 10000000.0f);
                            }
                        }
                    }
                    cf.Close();
                }
                else if (logfile.ToLower().EndsWith(".bin") || logfile.ToLower().EndsWith(".log"))
                {
                    using (
                        CollectionBuffer colbuf =
                            new CollectionBuffer(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        )
                    {
                        loc_list[0] = new List<PointLatLngAlt>();

                        foreach (var item in colbuf.GetEnumeratorType("GPS"))
                        {
                            if (item.msgtype.StartsWith("GPS"))
                            {
                                if (!colbuf.dflog.logformat.ContainsKey("GPS"))
                                    continue;

                                var status =
                                    double.Parse(item.items[colbuf.dflog.FindMessageOffset(item.msgtype, "Status")]);
                                var lat = double.Parse(item.items[colbuf.dflog.FindMessageOffset(item.msgtype, "Lat")]);
                                var lon = double.Parse(item.items[colbuf.dflog.FindMessageOffset(item.msgtype, "Lng")]);

                                if (lat == 0 || lon == 0 || status < 3)
                                    continue;

                                loc_list[0].Add(new PointLatLngAlt(lat, lon));

                                minx = Math.Min(minx, lon);
                                maxx = Math.Max(maxx, lon);
                                miny = Math.Min(miny, lat);
                                maxy = Math.Max(maxy, lat);
                            }

                        }
                    }
                }

                if (loc_list.Count > 0 && loc_list.First().Value.Count > 10)
                {
                    // add a bit of buffer
                    var area = RectLatLng.FromLTRB(minx - 0.001, maxy + 0.001, maxx + 0.001, miny - 0.001);
                    using (var map = GetMap(area))
                    using (var grap = Graphics.FromImage(map))
                    {
                        if (sitl)
                        {
                            AddTextToMap(grap, "SITL");
                        }

                        Color[] colours =
                        {
                            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo,
                            Color.Violet, Color.Pink
                        };

                        int a = 0;
                        foreach (var locs in loc_list.Values)
                        {
                            PointF lastpoint = new PointF();
                            var pen = new Pen(colours[a % (colours.Length - 1)]);

                            foreach (var loc in locs)
                            {
                                PointF newpoint = GetPixel(area, loc, map.Size);

                                if (!lastpoint.IsEmpty)
                                    grap.DrawLine(pen, lastpoint, newpoint);

                                lastpoint = newpoint;
                            }

                            a++;
                        }

                        map.Save(logfile + ".jpg", SKEncodedImageFormat.Jpeg);

                        File.SetLastWriteTime(logfile + ".jpg", new FileInfo(logfile).LastWriteTime);
                    }
                }
                else
                {
                    DoTextMap(logfile + ".jpg", "No gps data");

                    File.SetLastWriteTime(logfile + ".jpg", new FileInfo(logfile).LastWriteTime);
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Mavlink 0.9"))
                    DoTextMap(logfile + ".jpg", "Old log\nMavlink 0.9");
            }
        }

        static void DoTextMap(string jpgname, string text)
        {
            var map = new Bitmap(100, 100);

            var grap = Graphics.FromImage(map);

            AddTextToMap(grap, text);

            map.Save(jpgname, SKEncodedImageFormat.Jpeg);

            map.Dispose();

            map = null;
        }

        static void AddTextToMap(Graphics grap, string text)
        {
            grap.DrawString(text, SystemFonts.DefaultFont, Brushes.Red, 0, 0, StringFormat.GenericDefault);
        }

        static PointF GetPixel(RectLatLng area, PointLatLngAlt loc, Size size)
        {
            double lon = loc.Lng;
            double lat = loc.Lat;

            double lonscale = (lon - area.Left)*(size.Width - 0)/(area.Right - area.Left) + 0;

            double latscale = (lat - area.Top)*(size.Height - 0)/(area.Bottom - area.Top) + 0;

            return new PointF((float) lonscale, (float) latscale);
        }

        static Bitmap GetMap(RectLatLng area)
        {
            GMapProvider type = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            PureProjection prj = type.Projection;

            int zoom = 16;

            GPoint topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
            GPoint rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
            GPoint pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);

            // zoom based on pixel density
            while (pxDelta.X > 2000)
            {
                zoom--;

                // current area
                topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
                rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
                pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);
            }

            // get type list at new zoom level
            List<GPoint> tileArea = prj.GetAreaTileList(area, zoom, 0);

            int padding = 10;

            Bitmap bmpDestination = new Bitmap((int) pxDelta.X + padding*2, (int) pxDelta.Y + padding*2);

            {
                using (Graphics gfx = Graphics.FromImage(bmpDestination))
                {
                    gfx.CompositingMode = CompositingMode.SourceOver;

                    // get tiles & combine into one
                    foreach (var p in tileArea)
                    {
                        Console.WriteLine("Downloading[" + p + "]: " + tileArea.IndexOf(p) + " of " + tileArea.Count);

                        foreach (var tp in type.Overlays)
                        {
                            Exception ex;
                            using (var tile = GMaps.Instance.GetImageFrom(tp, p, zoom, out ex))
                            {
                                if (tile != null)
                                {
                                    using (tile)
                                    {
                                        long x = p.X*prj.TileSize.Width - topLeftPx.X + padding;
                                        long y = p.Y*prj.TileSize.Width - topLeftPx.Y + padding;
                                        {
                                            gfx.DrawImage(Image.FromStream(tile.Data), x, y, prj.TileSize.Width, prj.TileSize.Height);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return bmpDestination;
            }
        }
    }
}