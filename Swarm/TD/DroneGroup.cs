using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Joystick;
using MissionPlanner.Utilities;

namespace MissionPlanner.Swarm.TD
{
    public class DroneGroup
    {
        public enum Mode
        {
            idle,
            takeoff,
            guided,
            LandAlt,
            Land
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        Mode _currentMode = Mode.idle;

        public Mode CurrentMode
        {
            get { return _currentMode; }
            set
            {
                _currentMode = value;
                Console.WriteLine("Mode Change " + value.ToString());
            }
        }

        public List<Drone> Drones = new List<Drone>();

        public List<PointLatLngAlt> Fence = new List<PointLatLngAlt>();

        public double FenceMinAlt = 2;
        public double FenceMaxAlt = 30;

        public void UpdatePositions()
        {
            // get current positions and velocitys
            foreach (var drone in Drones)
            {
                if (drone.Location == null)
                    drone.Location = new PointLatLngAlt();
                drone.Location.Lat = drone.MavState.cs.lat;
                drone.Location.Lng = drone.MavState.cs.lng;
                drone.Location.Alt = drone.MavState.cs.alt;
                if (drone.Velocity == null)
                    drone.Velocity = new Vector3();
                drone.Velocity.x = drone.MavState.cs.vx;
                drone.Velocity.y = drone.MavState.cs.vy;
                drone.Velocity.z = drone.MavState.cs.vz;
            }

            // collision check
            foreach (var drone1 in Drones)
            {
                if (!drone1.MavState.cs.armed)
                    continue;

                // our base speed
                drone1.speed = 5;

                foreach (var drone2 in Drones)
                {
                    if (drone1 == drone2)
                        continue;

                    if (!drone2.MavState.cs.armed)
                        continue;

                    if (CurrentMode <= Mode.takeoff)
                        continue;

                    var distanceseperation = drone2.bubblerad * 2;

                    var distancecurrent = drone1.Location.GetDistance(drone2.Location);
                    var distanceprojected = drone1.ProjectedLocation.GetDistance(drone2.ProjectedLocation);
                    var distanceprojected2 = drone1.ProjectedLocation2.GetDistance(drone2.ProjectedLocation2);

                    var altseperation = Math.Abs(drone1.MavState.cs.altasl - drone2.MavState.cs.altasl);

                    var speed = map(distancecurrent, distanceseperation, 8, 0, 5);

                    if (distanceprojected2 < distanceseperation)
                    {
                        //slow down
                        if (altseperation < distanceseperation)
                        {
                            drone1.speed = Math.Min(drone1.speed, speed);
                        }
                    }
                    else
                    {
                        var bearing = drone1.Location.GetBearing(drone2.Location);
                        var delta = wrap_180(drone1.MavState.cs.yaw - bearing);
                        if (Math.Abs(delta) > 45)
                        {

                        }
                        else
                        {
                            drone1.speed = Math.Min(drone1.speed, speed);
                        }
                    }

                    if (distancecurrent < distanceseperation)
                    {
                        if (altseperation < distanceseperation)
                        {
                            var bearing = drone1.Location.GetBearing(drone2.Location);
                            var delta = wrap_180(drone1.MavState.cs.yaw - bearing);
                            if (Math.Abs(delta) < 45)
                            {
                                drone1.commandsent = true;
                                drone1.speed = 0.1;
                            }
                            else if (Math.Abs(delta) > 90)
                            {
                                drone1.speed = 1;
                            }
                        }
                    }
                }
            }

            switch (CurrentMode)
            {
                case Mode.idle:
                    CurrentMode = Mode.takeoff;
                    // request positon at 10hz
                    foreach (var drone in Drones)
                    {
                        var MAV = drone.MavState;

                        MAV.parent.requestDatastream(MAVLink.MAV_DATA_STREAM.POSITION, 10, MAV.sysid, MAV.compid);
                        MAV.cs.rateposition = 10;

                        drone.takeoffdone = false;


                        try
                        {
                            // get param
                            MAV.parent.GetParam(MAV.sysid, MAV.compid, "RTL_ALT");
                            // set param
                            MAV.parent.setParam(MAV.sysid, MAV.compid, "RTL_ALT", 0); // cms - rtl at current alt
                        }
                        catch
                        {
                        }
                        try
                        {
                            // get param
                            MAV.parent.GetParam(MAV.sysid, MAV.compid, "WPNAV_ACCEL");
                            // set param to default 100cm/s
                            MAV.parent.setParam(MAV.sysid, MAV.compid, "WPNAV_ACCEL", drone.speed * 100.0);
                        }
                        catch
                        {
                        }
                    }
                    break;
                case Mode.takeoff:
                    var takeoffalt = (float)FenceMinAlt;
                    // change mode
                    foreach (var drone in Drones)
                    {
                        try
                        {
                            // guided mode
                            if (!drone.MavState.cs.mode.ToLower().Equals("guided"))
                                drone.MavState.parent.setMode(drone.MavState.sysid, drone.MavState.compid, "GUIDED");
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            Loading.ShowLoading("Communication with one of the drones is failing\n" + ex);

                            return;
                        }
                    }
                    // arm
                    foreach (var drone in Drones)
                    {
                        try
                        {
                            // arm
                            if (!drone.MavState.cs.armed)
                            {
                                drone.takeoffdone = false;
                                if (!drone.MavState.parent.doARM(drone.MavState.sysid, drone.MavState.compid, true, true))
                                    return;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            Loading.ShowLoading("Communication with one of the drones is failing\n" + ex);

                            return;
                        }
                    }

                    foreach (var drone in Drones)
                    {
                        // set drone target position
                        drone.TargetLocation = drone.Location;
                        drone.TargetLocation.Alt = takeoffalt;

                        try
                        {
                            // takeoff
                            if (drone.MavState.cs.alt < (takeoffalt - 0.5) && !drone.takeoffdone)
                                if (drone.MavState.parent.doCommand(drone.MavState.sysid, drone.MavState.compid, MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0,
                                    0, takeoffalt))
                                    drone.takeoffdone = true;
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            Loading.ShowLoading("Communication with one of the drones is failing\n" + ex);

                            return;
                        }

                        drone.MavState.GuidedMode.x = 0;
                        drone.MavState.GuidedMode.y = 0;
                        drone.MavState.GuidedMode.z = (float)drone.TargetLocation.Alt;

                        // wait for takeoff
                        if (drone.MavState.cs.alt < (takeoffalt - 0.5))
                        {
                            Thread.Sleep(100);
                            // check we are still armed
                            if (!drone.MavState.cs.armed)
                            {
                                drone.takeoffdone = false;
                                return;
                            }

                            // move on to next drone
                            continue;
                        }

                        // we should only get here once takeoff alt has been archived by this drone.

                        // position control
                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        drone.MavState.GuidedMode.x = (float)drone.TargetLocation.Lat;
                        drone.MavState.GuidedMode.y = (float)drone.TargetLocation.Lng;
                        drone.MavState.GuidedMode.z = (float)drone.TargetLocation.Alt;
                    }
                    // wait for all to get within seperation distance
                    foreach (var drone in Drones)
                    {
                        var MAV = drone.MavState;
                        // check how far off target we are
                        if (drone.TargetLocation.GetDistance(drone.Location) > drone.bubblerad)
                        {
                            // position control
                            drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                            //if we are off target, we have already sent the command to this drone
                            return;
                        }

                        // wait for takeoff
                        if (MAV.cs.alt < (takeoffalt - 0.5))
                        {
                            Thread.Sleep(100);
                            // check we are still armed
                            if (!MAV.cs.armed)
                                return;

                            // reloop to force takeoff 
                            drone.takeoffdone = false;
                            return;
                        }
                    }
                    CurrentMode = Mode.guided;
                    break;
                case Mode.guided:
                    foreach (var drone in Drones)
                    {
                        // get drone target position
                        var newpos = GetUserPosition(drone);

                        // check for zero
                        if (newpos == PointLatLngAlt.Zero)
                            continue;

                        // set the new pos
                        drone.TargetLocation = newpos;

                        if (drone.commandsent)
                        {
                            drone.TargetLocation = drone.Location;
                            drone.TargetVelocity.X = -0.1;
                            //drone.SendYaw(drone.MavState.cs.yaw + 5);
                            drone.commandsent = false;
                            //continue;
                        }
                        else
                        {
                            drone.TargetVelocity.X = 0;
                        }

                        // position control
                        drone.SendPositionVelocityYaw(drone.TargetLocation, drone.TargetVelocity,
                            (float) drone.Location.GetBearing(drone.TargetLocation));

                        drone.MavState.GuidedMode.x = (float) drone.TargetLocation.Lat;
                        drone.MavState.GuidedMode.y = (float) drone.TargetLocation.Lng;
                        drone.MavState.GuidedMode.z = (float) drone.TargetLocation.Alt;
                    }

                    break;
                case Mode.LandAlt:
                    var e = 0;
                    foreach (var drone in Drones)
                    {
                        drone.TargetLocation.Alt = 3 + e;

                        try
                        {
                            var MAV = drone.MavState;
                            // set param to default 100cm/s
                            if (MAV.param["WPNAV_ACCEL"].Value != 100)
                                MAV.parent.setParam(MAV.sysid, MAV.compid, "WPNAV_ACCEL", 100);
                        }
                        catch
                        {
                        }

                        // position control
                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        drone.MavState.GuidedMode.z = (float) drone.TargetLocation.Alt;

                        Thread.Sleep(200);

                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        e++;
                    }
                    // check status
                    foreach (var drone in Drones)
                    {
                        // wait for alt hit
                        while (Math.Abs(drone.MavState.cs.alt - drone.TargetLocation.Alt) > 0.5)
                        {
                            if (!drone.MavState.cs.armed)
                                break;
                            Thread.Sleep(200);
                            drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);
                        }

                        Thread.Sleep(200);

                        // set mode rtl
                        drone.MavState.parent.setMode(drone.MavState.sysid, drone.MavState.compid, "RTL");
                    }
                    CurrentMode = Mode.Land;
                    break;
                case Mode.Land:
                    foreach (var drone in Drones)
                        if (!drone.MavState.cs.armed)
                            continue;
                    break;
            }
        }

        static double wrap_180(double angle)
        {
            var res = wrap_360(angle);
            if (res > 180.0)
            {
                res -= 360.0;
            }
            return res;
        }

        static double wrap_360(double angle)
        {
            double ang_360 = 360.0;
            double res = angle % ang_360;
            if (res < 0)
            {
                res += ang_360;
            }
            return res;
        }

        static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private PointLatLngAlt GetUserPosition(Drone drone)
        {
            PointLatLngAlt newpos;
            double yawtarget = drone.MavState.cs.yaw;

            var idx = Drones.IndexOf(drone);

            //if(true)
            if (idx < Controller.Joysticks.Count && Controller.Joysticks[idx] != null)
            {
                Controller.Joysticks[idx].Poll();

                var state = Controller.Joysticks[idx].CurrentJoystickState();

                var x = map(state.X, ushort.MinValue, ushort.MaxValue, short.MinValue, short.MaxValue);
                var y = map(state.Y, ushort.MinValue, ushort.MaxValue, short.MinValue, short.MaxValue);
                var z = map(state.Z, ushort.MinValue, ushort.MaxValue, 1, -1);
                var yaw = map(state.Rz, ushort.MinValue, ushort.MaxValue, short.MinValue, short.MaxValue);
                /*
                // forwards
                var x = 0;
                var y = short.MaxValue;
                var z = 0;
                var yaw = 0;*/

                // matrix with our current copter yaw
                var Matrix = new Matrix3();

                Matrix.from_euler(0, 0, drone.MavState.cs.yaw * MathHelper.deg2rad);

                // z deadzone
                if (Math.Abs(z) < 0.1)
                    z = 0;

                // rotated vector based on heading.
                var vector = new Vector3(x, y, 0);
                vector = Matrix * vector;

                var vectorbase = new Vector3(short.MaxValue, short.MaxValue, 0);

                var lengthscale = vector.length() / vectorbase.length();

                var newvector = vector.normalized() * (drone.speed * vector.length());

               var direction = Math.Atan2(newvector.x, -newvector.y) * (180 / Math.PI);

                // yaw deadzone
                if (Math.Abs(x) > 5000)
                    yawtarget = direction;

                newpos = drone.Location.newpos(yawtarget, drone.speed * lengthscale);

                newpos.Alt += z;

                drone.TargetVelocity =  Vector3.Zero;
                //new Vector3(Math.Cos(yawtarget * MathHelper.deg2rad) *
                //drone.speed * lengthscale, Math.Sin(yawtarget * MathHelper.deg2rad) *
                //drone.speed * lengthscale, 0);

                //drone.TargetVelocity = newvector;
                //Console.WriteLine("{0} {1} {2} {3} {4} {5}", vector.ToString(), yaw, lengthscale, newvector.ToString(), direction, newpos);
            }
            else
            {
                return PointLatLngAlt.Zero;
            }

            // can still move around, but alt limited
            if (newpos.Alt > FenceMaxAlt)
                newpos.Alt = FenceMaxAlt;

            // cant move around, as alt is the issue
            if (newpos.Alt < FenceMinAlt + 0.5)
            {
                newpos.Alt = FenceMinAlt + 0.5;
            }

            // close the loop if needed
            if (!Fence.Last().Equals(Fence.First()))
            {
                Fence.Add(Fence.First());
            }

            if (!PointInPolygon(newpos, Fence))
            {
                var bear = drone.Location.GetBearing(Centroid(Fence));
                newpos = drone.Location.newpos(bear, 1);
                drone.TargetVelocity = Vector3.Zero;
            }

            //Console.WriteLine("{0} {1} {2}",drone.MavState.sysid, newpos, drone.TargetVelocity.ToString());

            return newpos;
        }

        static PointLatLngAlt Centroid(List<PointLatLngAlt> poly)
        {
            double lat = 0;
            double lng = 0;
            double parts = poly.Count;

            poly.ForEach(a =>
            {
                lat += (a.Lat / parts);
                lng += (a.Lng / parts);
            });

            return new PointLatLngAlt(lat, lng);
        }

        static bool PointInPolygon(PointLatLngAlt p, List<PointLatLngAlt> poly)
        {
            PointLatLngAlt p1, p2;
            bool inside = false;

            if (poly.Count < 3)
            {
                return inside;
            }
            PointLatLngAlt oldPoint = new PointLatLngAlt(poly[poly.Count - 1]);

            for (int i = 0; i < poly.Count; i++)
            {

                PointLatLngAlt newPoint = new PointLatLngAlt(poly[i]);

                if (newPoint.Lat > oldPoint.Lat)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.Lat < p.Lat) == (p.Lat <= oldPoint.Lat)
                    && ((double) p.Lng - (double) p1.Lng) * (double) (p2.Lat - p1.Lat)
                    < ((double) p2.Lng - (double) p1.Lng) * (double) (p.Lat - p1.Lat))
                {
                    inside = !inside;
                }
                oldPoint = newPoint;
            }
            return inside;
        }
    }
}