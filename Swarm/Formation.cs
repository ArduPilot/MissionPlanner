using System;
using System.Collections.Generic;

using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using MissionPlanner.Utilities;
using MissionPlanner.ArduPilot;
using Vector3 = MissionPlanner.Utilities.Vector3;

namespace MissionPlanner.Swarm
{
    /// <summary>
    /// Follow the leader
    /// </summary>
    class Formation : Swarm
    {
        Dictionary<MAVState, Vector3> offsets = new Dictionary<MAVState, Vector3>();

        PointLatLngAlt masterpos = new PointLatLngAlt();

        public void setOffsets(MAVState mav, double x, double y, double z)
        {
            offsets[mav] = new Vector3(x, y, z);
            log.Info(mav.ToString() + " " + offsets[mav].ToString());
        }

        public Vector3 getOffsets(MAVState mav)
        {
            if (offsets.ContainsKey(mav))
            {
                return offsets[mav];
            }

            return new Vector3(offsets.Count, 0, 0);
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

                        var x = ((Vector3) offsets[mav]).x;
                        var y = ((Vector3) offsets[mav]).y;

                        // add offsets to utm
                        p1[0] += x*Math.Cos(heading*MathHelper.deg2rad) - y*Math.Sin(heading*MathHelper.deg2rad);
                        p1[1] += x*Math.Sin(heading*MathHelper.deg2rad) + y*Math.Cos(heading*MathHelper.deg2rad);

                        if (mav.cs.firmware == Firmwares.ArduPlane)
                        {
                            // project the point forwards gs
                            var gs = mav.cs.groundspeed;

                            //p1[1] += gs*Math.Cos((-heading)*MathHelper.deg2rad);
                            //p1[0] += gs*Math.Sin((-heading)*MathHelper.deg2rad);
                        }
                        // convert back to wgs84
                        IMathTransform inversedTransform = trans.MathTransform.Inverse();
                        double[] point = inversedTransform.Transform(p1);

                        target.Lat = point[1];
                        target.Lng = point[0];
                        target.Alt += ((Vector3) offsets[mav]).z;

                        if (mav.cs.firmware == Firmwares.ArduPlane)
                        {
                            // display update
                            mav.GuidedMode.x = (float)target.Lat;
                            mav.GuidedMode.y = (float)target.Lng;
                            mav.GuidedMode.z = (float)target.Alt;

                            // get distance from target position
                            var dist = target.GetDistance(mav.cs.Location);

                            // get bearing to target
                            var targyaw = mav.cs.Location.GetBearing(target);

                            if (Math.Abs(mav.cs.yaw - targyaw) > 135)
                            {
                                dist *= -1;
                            }

                            targyaw = mav.cs.Location.GetBearing(target.newpos(Leader.cs.yaw,
                                2 + Math.Min(Math.Abs(dist), 100)));

                            MAVLink.mavlink_set_attitude_target_t att_target = new MAVLink.mavlink_set_attitude_target_t();
                            att_target.target_system = mav.sysid;
                            att_target.target_component = mav.compid;
                            att_target.type_mask = 0xff;

                            var extraroll = 0d;
                            var newpitch = 0d;

                            if (Math.Abs(Leader.cs.altasl - mav.cs.altasl) > 5)
                            {
                                var altdelta = Leader.cs.altasl - mav.cs.altasl;
                                newpitch = altdelta;
                                att_target.type_mask -= 0b00000010;
                            }

                            if (true)
                            {
                                var leaderturnrad = Leader.cs.radius;
                                var mavturnradius = leaderturnrad - x;

                                // tr = gs2 / (9.8 * x)
                                // (9.8 * x) * tr = gs2
                                // 9.8 * x = gs2 / tr
                                // (gs2/tr)/9.8 = x

                                var angle = ((mav.cs.groundspeed * mav.cs.groundspeed) / mavturnradius) / 9.8;

                                extraroll = Leader.cs.roll - angle * MathHelper.rad2deg;
                            }

                            // do speed
                            if (true)
                            {
                                att_target.thrust = (float) MathHelper.map(dist, 0, 40, 0, 1);
                                att_target.type_mask -= 0b01000000;
                            }

                            Quaternion q = new Quaternion();
                            q.from_vector312((Leader.cs.roll + extraroll) * MathHelper.deg2rad, (Leader.cs.pitch + newpitch) * MathHelper.deg2rad, (targyaw - mav.cs.yaw) * MathHelper.deg2rad);

                            att_target.q = new float[4];
                            att_target.q[0] = (float) q.q1;
                            att_target.q[1] = (float) q.q2;
                            att_target.q[2] = (float) q.q3;
                            att_target.q[3] = (float) q.q4;
                   
                             //0b0= rpy
                            att_target.type_mask -= 0b10000101;

                            Console.WriteLine("sysid {0} - {1} dist {2}", mav.sysid, att_target.thrust, dist);

                            port.sendPacket(att_target, mav.sysid, mav.compid);

                            //port.setParam(mav.sysid, mav.compid, "TRIM_ARSPD_CM", newspeed*100.0f);

                            // send position
                            /*port.setGuidedModeWP(mav.sysid, mav.compid, new Locationwp()
                            {
                                alt = (float) target.Alt,
                                lat = target.Lat,
                                lng = target.Lng,
                                id = (ushort) MAVLink.MAV_CMD.WAYPOINT
                            });*/
                        }
                        else
                        {
                            Vector3 vel = new Vector3(Leader.cs.vx, Leader.cs.vy, Leader.cs.vz);

                            // do pos/vel
                            port.setPositionTargetGlobalInt(mav.sysid, mav.compid, true,
                                true, false, false,
                                MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT, target.Lat, target.Lng, target.Alt, vel.x,
                                vel.y, vel.z, 0, 0);

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