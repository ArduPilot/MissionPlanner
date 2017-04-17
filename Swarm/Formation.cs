using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using ProjNet.Converters;
using MissionPlanner.Utilities;
using MissionPlanner;
using MissionPlanner.HIL;

namespace MissionPlanner.Swarm
{
    /// <summary>
    /// Follow the leader
    /// </summary>
    class Formation : Swarm
    {
        Dictionary<MAVState, HIL.Vector3> offsets = new Dictionary<MAVState, HIL.Vector3>();

        PointLatLngAlt masterpos = new PointLatLngAlt();

        public void setOffsets(MAVState mav, double x, double y, double z)
        {
            offsets[mav] = new HIL.Vector3(x, y, z);
            log.Info(mav.ToString() + " " + offsets[mav].ToString());
        }

        public HIL.Vector3 getOffsets(MAVState mav)
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
                Leader = MainV2.comPort.MAV;

            masterpos = new PointLatLngAlt(Leader.cs.lat, Leader.cs.lng, Leader.cs.alt, "");
        }

        //convert Wgs84ConversionInfo to utm
        CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();

        GeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

        public override void SendCommand()
        {
            if (masterpos.Lat == 0 || masterpos.Lng == 0)
                return;

            //Console.WriteLine(DateTime.Now);
            //Console.WriteLine("Leader {0} {1} {2}", masterpos.Lat, masterpos.Lng, masterpos.Alt);

            int a = 0;
            foreach (var port in MainV2.Comports.ToArray())
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav == Leader)
                        continue;

                    PointLatLngAlt target = new PointLatLngAlt(masterpos);

                    try
                    {
                        int utmzone = (int) ((masterpos.Lng - -186.0)/6.0);

                        IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmzone,
                            masterpos.Lat < 0 ? false : true);

                        ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84, utm);

                        double[] pll1 = {target.Lng, target.Lat};

                        double[] p1 = trans.MathTransform.Transform(pll1);

                        double heading = -Leader.cs.yaw;

                        double length = offsets[mav].length();

                        var x = ((HIL.Vector3) offsets[mav]).x;
                        var y = ((HIL.Vector3) offsets[mav]).y;

                        // add offsets to utm
                        p1[0] += x*Math.Cos(heading*MathHelper.deg2rad) - y*Math.Sin(heading*MathHelper.deg2rad);
                        p1[1] += x*Math.Sin(heading*MathHelper.deg2rad) + y*Math.Cos(heading*MathHelper.deg2rad);

                        if (mav.cs.firmware == MainV2.Firmwares.ArduPlane)
                        {
                            // project the point forwards gs*5
                            var gs = mav.cs.groundspeed*5;

                            p1[1] += gs*Math.Cos((-heading)*MathHelper.deg2rad);
                            p1[0] += gs*Math.Sin((-heading)*MathHelper.deg2rad);
                        }
                        // convert back to wgs84
                        IMathTransform inversedTransform = trans.MathTransform.Inverse();
                        double[] point = inversedTransform.Transform(p1);

                        target.Lat = point[1];
                        target.Lng = point[0];
                        target.Alt += ((HIL.Vector3) offsets[mav]).z;

                        if (mav.cs.firmware == MainV2.Firmwares.ArduPlane)
                        {
                            var dist =
                                target.GetDistance(new PointLatLngAlt(mav.cs.lat, mav.cs.lng, mav.cs.alt));

                            dist -= mav.cs.groundspeed*5;

                            var leadergs = Leader.cs.groundspeed;

                            var newspeed = (leadergs + (float) (dist/10));

                            if (newspeed < 5)
                                newspeed = 5;

                            port.setParam(mav.sysid, mav.compid, "TRIM_ARSPD_CM", newspeed*100.0f);

                            // send position
                            port.setGuidedModeWP(mav.sysid, mav.compid, new Locationwp()
                            {
                                alt = (float) target.Alt,
                                lat = target.Lat,
                                lng = target.Lng,
                                id = (ushort) MAVLink.MAV_CMD.WAYPOINT
                            });
                        }
                        else
                        {
                            Vector3 vel = new Vector3(Leader.cs.vx, Leader.cs.vy, Leader.cs.vz);

                            // do pos/vel
                            port.setPositionTargetGlobalInt(mav.sysid, mav.compid, true,
                                true, false,
                                MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT, target.Lat, target.Lng, target.Alt, vel.x,
                                vel.y, -vel.z);

                            // do yaw
                            if (!gimbal)
                            {
                                // within 3 degrees dont send
                                if (Math.Abs(mav.cs.yaw - Leader.cs.yaw) > 3)
                                    port.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.CONDITION_YAW, Leader.cs.yaw,
                                        100.0f, 0, 0, 0, 0, 0, false);
                            }
                            else
                            {
                                // gimbal direction
                                if (Math.Abs(mav.cs.yaw - Leader.cs.yaw) > 3)
                                    port.setMountControl(mav.sysid, mav.compid, 45, 0, Leader.cs.yaw, false);
                            }
                        }

                        //Console.WriteLine("{0} {1} {2} {3}", port.ToString(), target.Lat, target.Lng, target.Alt);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to send command " + mav.ToString() + "\n" + ex.ToString());
                    }

                    a++;
                }
            }
        }

        public bool gimbal { get; set; }
    }
}