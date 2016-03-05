using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using ProjNet.Converters;
using MissionPlanner.Utilities;
using MissionPlanner;

namespace MissionPlanner.Swarm
{
    /// <summary>
    /// Follow the leader
    /// </summary>
    class Formation : Swarm
    {
        Dictionary<MAVLinkInterface, HIL.Vector3> offsets = new Dictionary<MAVLinkInterface, HIL.Vector3>();

        PointLatLngAlt masterpos = new PointLatLngAlt();

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public void setOffsets(MAVLinkInterface mav, double x, double y, double z)
        {
            offsets[mav] = new HIL.Vector3(x, y, z);
            //log.Info(mav.ToString() + " " + offsets[mav].ToString());
        }

        public HIL.Vector3 getOffsets(MAVLinkInterface mav)
        {
            if (offsets.ContainsKey(mav))
            {
                return offsets[mav];
            }

            return new HIL.Vector3(offsets.Count, 0, 0);
        }

        public override void Update()
        {
            if (MainV2.comPort.MAV.cs.lat == 0 || MainV2.comPort.MAV.cs.lng == 0)
                return;

            if (Leader == null)
                Leader = MainV2.comPort;

            masterpos = new PointLatLngAlt(Leader.MAV.cs.lat, Leader.MAV.cs.lng, Leader.MAV.cs.alt, "");
        }

        public override void SendCommand()
        {
            if (masterpos.Lat == 0 || masterpos.Lng == 0)
                return;

            Console.WriteLine(DateTime.Now);
            Console.WriteLine("Leader {0} {1} {2}", masterpos.Lat, masterpos.Lng, masterpos.Alt);

            int a = 0;
            foreach (var port in MainV2.Comports)
            {
                if (port == Leader)
                    continue;

                PointLatLngAlt target = new PointLatLngAlt(masterpos);

                try
                {
                    //convert Wgs84ConversionInfo to utm
                    CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();

                    GeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

                    int utmzone = (int) ((masterpos.Lng - -186.0)/6.0);

                    IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmzone,
                        masterpos.Lat < 0 ? false : true);

                    ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84, utm);

                    double[] pll1 = {target.Lng, target.Lat};

                    double[] p1 = trans.MathTransform.Transform(pll1);

                    double heading = -Leader.MAV.cs.yaw;

                    double length = offsets[port].length();

                    var x = ((HIL.Vector3)offsets[port]).x;
                    var y = ((HIL.Vector3)offsets[port]).y;

                    // add offsets to utm
                    p1[0] += x*Math.Cos(heading*deg2rad) - y*Math.Sin(heading*deg2rad);
                    p1[1] += x*Math.Sin(heading*deg2rad) + y*Math.Cos(heading*deg2rad);

                    if (port.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
                    {
                        // project the point forwards gs*5
                        var gs = port.MAV.cs.groundspeed;

                        p1[1] += gs*5*Math.Cos((-heading)*deg2rad);
                        p1[0] += gs*5*Math.Sin((-heading)*deg2rad);
                    }
                    // convert back to wgs84
                    IMathTransform inversedTransform = trans.MathTransform.Inverse();
                    double[] point = inversedTransform.Transform(p1);

                    target.Lat = point[1];
                    target.Lng = point[0];
                    target.Alt += ((HIL.Vector3) offsets[port]).z;

                    if (port.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
                    {
                        var dist = target.GetDistance(new PointLatLngAlt(port.MAV.cs.lat, port.MAV.cs.lng, port.MAV.cs.alt));

                        dist -= port.MAV.cs.groundspeed*5;

                        var leadergs = Leader.MAV.cs.groundspeed;

                        var newspeed = (leadergs + (float) (dist/10));

                        if (newspeed < 5)
                            newspeed = 5;

                        port.setParam("TRIM_ARSPD_CM", newspeed*100.0f);
                    }

                    port.setGuidedModeWP(new Locationwp()
                    {
                        alt = (float) target.Alt,
                        lat = target.Lat,
                        lng = target.Lng,
                        id = (byte) MAVLink.MAV_CMD.WAYPOINT
                    });

                    Console.WriteLine("{0} {1} {2} {3}", port.ToString(), target.Lat, target.Lng, target.Alt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send command " + port.ToString() + "\n" + ex.ToString());
                }

                a++;
            }
        }
    }
}