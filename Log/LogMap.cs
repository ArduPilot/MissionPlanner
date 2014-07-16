using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MissionPlanner.Log
{
    class LogMap
    {
        public static void MapLogs(string[] logs)
        {
            foreach (var logfile in logs)
            {
                if (File.Exists(logfile + ".jpg"))
                    continue;

                MAVLinkInterface mine = new MAVLinkInterface();

                List<MAVLink.mavlink_global_position_int_t> locs = new List<MAVLink.mavlink_global_position_int_t>();

                try
                {
                    double minx = 99999;
                    double maxx = -99999;
                    double miny = 99999;
                    double maxy = -99999;

                    using (mine.logplaybackfile = new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        mine.logreadmode = true;

                        CurrentState cs = new CurrentState();

                        while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                        {
                            byte[] packet = mine.readPacket();

                            //Console.Write((mine.logplaybackfile.BaseStream.Position / (double)mine.logplaybackfile.BaseStream.Length) +"\r");

                            if (packet.Length < 5)
                                continue;

                            try
                            {
                                if (MainV2.speechEngine != null)
                                    MainV2.speechEngine.SpeakAsyncCancelAll();
                            }
                            catch { }

                            if (packet[5] == (byte)MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT)
                            {
                                var loc = packet.ByteArrayToStructure<MAVLink.mavlink_global_position_int_t>(6);

                                if (loc.lat == 0 || loc.lon == 0)
                                    continue;

                                locs.Add(loc);

                                minx = Math.Min(minx, loc.lon / 10000000.0f);
                                maxx = Math.Max(maxx, loc.lon / 10000000.0f);
                                miny = Math.Min(miny, loc.lat / 10000000.0f);
                                maxy = Math.Max(maxy, loc.lat / 10000000.0f);

                            }
                        }
                    }

                    if (locs.Count > 10)
                    {
                        // add a bit of buffer
                        var area = RectLatLng.FromLTRB(minx - 0.001 , maxy  + 0.001, maxx + 0.001 , miny - 0.001 );
                        var map = GetMap(area);

                        var grap = Graphics.FromImage(map);

                        PointF lastpoint = new PointF();

                        foreach (var loc in locs)
                        {
                            PointF newpoint = GetPixel(area, loc, map.Size);

                            if (!lastpoint.IsEmpty)
                                grap.DrawLine(Pens.Red, lastpoint, newpoint);

                            lastpoint = newpoint;                            
                        }

                        map.Save(logfile + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                        map.Dispose();

                        map = null;
                    }
                    else
                    {
                        var map = new Bitmap(100, 100);

                        var grap = Graphics.FromImage(map);

                        grap.DrawString("NO MAP", SystemFonts.DefaultFont, Brushes.Red, 0, 0, StringFormat.GenericDefault);

                        map.Save(logfile + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                        map.Dispose();

                        map = null;
                    }
                }
                catch { continue; }
            }
        }

        static PointF GetPixel(RectLatLng area, MAVLink.mavlink_global_position_int_t loc, Size size) 
        {
            double lon =  loc.lon / 10000000.0f;
            double lat = loc.lat / 10000000.0f;

            double lonscale = (lon - area.Left) * (size.Width - 0) / (area.Right - area.Left) + 0;

            double latscale = (lat - area.Top) * (size.Height - 0) / (area.Bottom - area.Top) + 0;

            return new PointF((float)lonscale, (float)latscale);
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

            Bitmap bmpDestination = new Bitmap((int)pxDelta.X + padding * 2, (int)pxDelta.Y + padding * 2);

            {
                using (Graphics gfx = Graphics.FromImage(bmpDestination))
                {
                    gfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                    // get tiles & combine into one
                    foreach (var p in tileArea)
                    {
                        Console.WriteLine("Downloading[" + p + "]: " + tileArea.IndexOf(p) + " of " + tileArea.Count);

                        foreach (var tp in type.Overlays)
                        {
                            Exception ex;
                            GMapImage tile = GMaps.Instance.GetImageFrom(tp, p, zoom, out ex) as GMapImage;

                            if (tile != null)
                            {
                                using (tile)
                                {
                                    long x = p.X * prj.TileSize.Width - topLeftPx.X + padding;
                                    long y = p.Y * prj.TileSize.Width - topLeftPx.Y + padding;
                                    {
                                        gfx.DrawImage(tile.Img, x, y, prj.TileSize.Width, prj.TileSize.Height);
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