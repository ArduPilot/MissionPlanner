using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.Utilities
{
    public class Fence
    {
        /// <summary>
        /// Fence
        /// </summary>
        public List<object> Fences = new List<object>();

        public void DownloadFence(MAVLinkInterface port, Action<int, string> progress = null)
        {
            var list = Task.Run(async () => await mav_mission
                .download(port, port.MAV.sysid, port.MAV.compid, MAVLink.MAV_MISSION_TYPE.FENCE, progress).ConfigureAwait(false)).Result;

            LocationToFence(list);
        }

        public List<object> LocationToFence(List<Locationwp> list)
        {
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

            return Fences;
        }

        public void UploadFence(MAVLinkInterface port, Action<int, string> progress = null)
        {
            var fence = FenceToLocation();

            mav_mission.upload(port, port.MAV.sysid, port.MAV.compid, MAVLink.MAV_MISSION_TYPE.FENCE, fence, progress).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public List<Locationwp> FenceToLocation()
        {
            return Fences.SelectMany((o, i) =>
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
        }
    }
}