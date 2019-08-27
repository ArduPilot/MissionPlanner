using MissionPlanner.ArduPilot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MissionPlanner.Utilities
{
    public class FenceCircle
    {
        public PointLatLngAlt Center { get; set; }
        public float Radius { get; set; }

        public PolyType Mode { get; set; }

        public enum PolyType
        {
            Inclusive = MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION,
            Exclusive = MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION
        }
    }

    public class FencePolygon
    {
        public List<PointLatLngAlt> Points { get; set; } = new List<PointLatLngAlt>();

        public PolyType Mode { get; set; }

        public enum PolyType
        {
            Inclusive = MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION,
            Exclusive = MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION
        }
    }

    public class FenceReturn
    {
        public PointLatLngAlt Return { get; set; }
    }

    public class Fence
    {
        /// <summary>
        /// Fence
        /// </summary>
        public List<object> Fences = new List<object>();

        public void DownloadFence(MAVLinkInterface port, Action<int, string> progress = null)
        {
            var list = mav_mission.download(port, MAVLink.MAV_MISSION_TYPE.FENCE, progress);

            FencePolygon pol = new FencePolygon();

            foreach (var locationwp in list)
            {
                var plla = new PointLatLngAlt(locationwp);
                switch (locationwp.id)
                {
                    case (ushort) MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_EXCLUSION:
                    {
                        pol.Mode = FencePolygon.PolyType.Exclusive;
                        pol.Points.Add(plla);

                        if (pol.Points.Count == locationwp.p1)
                        {
                            Fences.Add(pol);
                            pol = new FencePolygon();
                        }
                    }
                        break;
                    case (ushort) MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION:
                    {
                        pol.Mode = FencePolygon.PolyType.Inclusive;
                        pol.Points.Add(plla);

                        if (pol.Points.Count == locationwp.p1)
                        {
                            Fences.Add(pol);
                            pol = new FencePolygon();
                        }
                    }
                        break;
                    case (ushort) MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION:
                    {
                        var cir = new FenceCircle()
                        {
                            Mode = FenceCircle.PolyType.Exclusive,
                            Center = plla,
                            Radius = locationwp.p1
                        };
                        Fences.Add(cir);
                    }
                        break;
                    case (ushort) MAVLink.MAV_CMD.FENCE_CIRCLE_INCLUSION:
                    {
                        var cir = new FenceCircle()
                        {
                            Mode = FenceCircle.PolyType.Inclusive,
                            Center = plla,
                            Radius = locationwp.p1
                        };
                        Fences.Add(cir);
                    }
                        break;
                    case (ushort) MAVLink.MAV_CMD.FENCE_RETURN_POINT:
                        Fences.Add(new FenceReturn() {Return = plla});
                        break;
                    default:
                        break;
                }
            }
        }

        public void UploadFence(MAVLinkInterface port, Action<int, string> progress = null)
        {
            var fence = Fences.SelectMany((o, i) =>
            {
                var ans = new List<Locationwp>();
                if (o is FenceCircle)
                {
                    var b = o as FenceCircle;
                    ans.Add(new Locationwp()
                    {
                        frame = (byte) MAVLink.MAV_FRAME.GLOBAL, id = (ushort) b.Mode, lat = b.Center.Lat,
                        lng = b.Center.Lng, p1 = b.Radius
                    });
                }
                else if (o is FencePolygon)
                {
                    var b = o as FencePolygon;
                    b.Points.ForEach(pt=>
                    {
                        ans.Add(new Locationwp()
                        {
                            frame = (byte) MAVLink.MAV_FRAME.GLOBAL,
                            id = (ushort) b.Mode,
                            lat = pt.Lat,
                            lng = pt.Lng,
                            p1 = b.Points.Count
                        });
                    });
                }
                else if (o is FenceReturn)
                {
                    var b = o as FenceReturn;
                    ans.Add(new Locationwp()
                    {
                        frame = (byte)MAVLink.MAV_FRAME.GLOBAL,
                        id = (ushort)MAVLink.MAV_CMD.FENCE_RETURN_POINT,
                        lat = b.Return.Lat,
                        lng = b.Return.Lng
                    });
                }

                return ans;
            }).ToList();

            mav_mission.upload(port, MAVLink.MAV_MISSION_TYPE.FENCE, fence, progress);
        }
    }
}