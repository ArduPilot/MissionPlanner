using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using MissionPlanner.Controls;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner;
using System.Drawing;
using System.Threading.Tasks;
using GMap.NET.WindowsForms;
using MissionPlanner.ArduPilot;
using MissionPlanner.GCSViews;
using MissionPlanner.Maps;


namespace FenceDist
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        ToolStripMenuItem but;
        static GMapMarkerFill marker;

        public override string Name
        {
            get { return "FenceDist"; }
        }

        public override string Version
        {
            get { return "0.10"; }
        }

        public override string Author
        {
            get { return "Michael Oborne"; }
        }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            but = new ToolStripMenuItem("Draw Fence Dist");
            but.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            col.Add(but);
            return true;
        }

        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }



        float testCode(MAVState MAV, PointLatLngAlt Location)
        {
            // this is GeoFenceDist from currentstate
            try
            {
                float disttotal = 99999;
                var R = 6371e3;
                var currenthash = MAV.fencepoints.GetHashCode();
                lock(this)
                    if (currenthash != listhash)
                    {
                        list = MAV.fencepoints
                            .Where(a => a.Value.command != (ushort) MAVLink.MAV_CMD.FENCE_RETURN_POINT)
                            .ChunkByField((a, b, count) =>
                            {
                                // these fields types stand alone
                                if (a.Value.command == (ushort) MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION ||
                                    a.Value.command == (ushort) MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION)
                                    return false;

                                if (count >= b.Value.param1)
                                    return false;

                                return a.Value.command == b.Value.command;
                            }).ToList();
                        listhash = currenthash;
                    }

                // check all sublists
                foreach (var sublist in list)
                {
                    // process circles
                    if (sublist.Count() == 1)
                    {
                        var item = sublist.First().Value;
                        if (item.command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION)
                        {
                            var lla = new PointLatLngAlt(item.x / 1e7,
                                item.y / 1e7);
                            var dist = lla.GetDistance(Location);
                            if (dist < item.param1)
                                return 0;
                            disttotal = (float)Math.Min(dist - item.param1, disttotal);
                        }
                        else if (item.command == (ushort)MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION)
                        {
                            var lla = new PointLatLngAlt(item.x / 1e7,
                                item.y / 1e7);

                            var dist = lla.GetDistance(Location);
                            if (dist > item.param1)
                                return 0;
                            disttotal = (float) Math.Min(item.param1 - dist, disttotal);
                        }
                    }

                    if (sublist == null || sublist.Count() < 3)
                        continue;

                    if (PolygonTools.isInside(
                        sublist.CloseLoop().Select(a => new PolygonTools.Point(a.Value.y / 1e7, a.Value.x / 1e7)).ToList(),
                        new PolygonTools.Point(Location.Lng, Location.Lat)))
                    {
                        if (sublist.First().Value.command == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION)
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        if (sublist.First().Value.command == (ushort)MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION)
                        {
                            return 0;
                        }
                    }

                    PointLatLngAlt lineStartLatLngAlt = null;
                    // check all segments
                    foreach (var mavlinkFencePointT in sublist.CloseLoop())
                    {
                        if (lineStartLatLngAlt == null)
                        {
                            lineStartLatLngAlt = new PointLatLngAlt(mavlinkFencePointT.Value.x / 1e7,
                                mavlinkFencePointT.Value.y / 1e7);
                            continue;
                        }

                        // crosstrack distance
                        var lineEndLatLngAlt = new PointLatLngAlt(mavlinkFencePointT.Value.x / 1e7,
                            mavlinkFencePointT.Value.y / 1e7);

                        var lineDist = lineStartLatLngAlt.GetDistance2(lineEndLatLngAlt);

                        var distToLocation = lineStartLatLngAlt.GetDistance2(Location);
                        var bearToLocation = lineStartLatLngAlt.GetBearing(Location);
                        var lineBear = lineStartLatLngAlt.GetBearing(lineEndLatLngAlt);

                        var angle = bearToLocation - lineBear;
                        if (angle < 0)
                            angle += 360;

                        var alongline = Math.Cos(angle * MathHelper.deg2rad) * distToLocation;

                        // check to see if our point is still within the line length
                        if (alongline < 0 || alongline > lineDist)
                        {
                            lineStartLatLngAlt = lineEndLatLngAlt;
                            continue;
                        }

                        var dXt2 = Math.Sin(angle * MathHelper.deg2rad) * distToLocation;

                        var dXt = Math.Asin(Math.Sin(distToLocation / R) * Math.Sin(angle * MathHelper.deg2rad)) * R;

                        disttotal = (float) Math.Min(disttotal, Math.Abs(dXt2));

                        lineStartLatLngAlt = lineEndLatLngAlt;
                    }
                }

                // check also distance from the points - because if we are outside the polygon, we may be on a corner segment
                foreach (var sublist in list)
                foreach (var mavlinkFencePointT in sublist)
                {
                    if (mavlinkFencePointT.Value.command == (ushort) MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION)
                        continue;
                    var pathpoint = new PointLatLngAlt(mavlinkFencePointT.Value.x / 1e7,
                        mavlinkFencePointT.Value.y / 1e7);
                    var dXt2 = pathpoint.GetDistance(Location);
                    disttotal = (float) Math.Min(disttotal, Math.Abs(dXt2));
                }

                return disttotal;
            }
            catch
            {
                return 0;
            }
        }


        byte[] colors = { 220, 226, 232, 233, 244, 250, 214, 142, 106 };
        static GMapOverlay overlay;
        private IEnumerable<IEnumerable<KeyValuePair<int, MAVLink.mavlink_mission_item_int_t>>> list;
        private int listhash;

        void but_Click(object sender, EventArgs e)
        {
            if (overlay == null)
            {
                overlay = new GMapOverlay();
                overlay.IsVisibile = false;
                Host.FDGMapControl.Overlays.Add(overlay);
            }

            overlay.IsVisibile = !overlay.IsVisibile;
            
            var size = 400;

            byte[,] bitmap = new byte[size, size];

            var va = FlightData.instance.gMapControl1.ViewArea;
            var spacingw = va.WidthLng / size;
            var spacingh = va.HeightLat / size;
            var loc = MainV2.comPort.MAV.cs.Location;

            Parallel.For(0, size, (x) =>
            {
                for (int y = 0; y < size; y++)
                {
                    var colindex = (int) MathHelper.mapConstrained(testCode(MainV2.comPort.MAV,
                        new PointLatLngAlt(va.Bottom + spacingh * y,
                            va.Left + spacingw * x)), 0, 10, 1, 255);
                    bitmap[x, size - y - 1] = (byte) colindex;
                }
            });

            marker = new GMapMarkerFill(bitmap, va, loc);

            overlay.Markers.Clear();
            overlay.Markers.Add(marker);

        }
    }
}