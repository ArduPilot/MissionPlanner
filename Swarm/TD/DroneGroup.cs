using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.HIL;
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

                foreach (var drone2 in Drones)
                {
                    if (drone1 == drone2)
                        continue;

                    if (!drone2.MavState.cs.armed)
                        continue;

                    // check how close they are based on current position
                    if (drone1.Location.GetDistance(drone2.Location) < drone2.bubblerad * 2)
                    {
                        if (Math.Abs(drone1.Location.Alt - drone2.Location.Alt) < drone2.bubblerad * 2)
                            if (drone1.Location.Alt > drone2.Location.Alt)
                            {
                                drone1.SendPositionVelocity(
                                    new PointLatLngAlt(drone1.Location)
                                    {
                                        Alt = drone2.Location.Alt + drone2.bubblerad * 2
                                    },
                                    Vector3.Zero);
                                continue;
                            }
                            else
                            {
                                drone2.SendPositionVelocity(
                                    new PointLatLngAlt(drone2.Location)
                                    {
                                        Alt = drone1.Location.Alt + drone1.bubblerad * 2
                                    },
                                    Vector3.Zero);
                                continue;
                            }
                    }

                    // check how close they are based on a 1 second projection
                    if (drone1.ProjectedLocation.GetDistance(drone2.ProjectedLocation) < drone2.bubblerad * 2)
                    {
                        // check if they are heading the same direction
                        if ((Math.Abs(drone1.Heading - drone2.Heading) + 360) % 360 < 45)
                            if (Math.Abs(drone1.Location.Alt - drone2.Location.Alt) < 1)
                            {
                                // they are heading within 45 degrees of each other
                                // return here to let them settle themselfs, as the target position will be correct
                                Console.WriteLine("1 drone, to close");
                                drone1.SendPositionVelocity(drone1.Location, Vector3.Zero);
                                continue;
                            }

                        // check if the are heading are at each other
                        if ((Math.Abs(drone1.Heading - drone2.Heading) + 360) % 360 > 135)
                            if (Math.Abs(drone1.Location.Alt - drone2.Location.Alt) < 1)
                            {
                                // stop the drones
                                drone1.SendPositionVelocity(
                                    new PointLatLngAlt(drone1.Location)
                                    {
                                        Alt = drone1.Location.Alt + drone1.bubblerad * 2
                                    }, Vector3.Zero);
                                drone2.SendPositionVelocity(drone2.Location, Vector3.Zero);
                                Console.WriteLine("2 stopping drone, to close and heading towards each other");
                                continue;
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
                    var takeoffalt = (float)4;
                    foreach (var drone in Drones)
                    {
                        var MAV = drone.MavState;
                        try
                        {
                            // guided mode
                            if (!MAV.cs.mode.ToLower().Equals("guided"))
                                MAV.parent.setMode(MAV.sysid, MAV.compid, "GUIDED");
                            // arm
                            if (!MAV.cs.armed)
                                if (!MAV.parent.doARM(MAV.sysid, MAV.compid, true))
                                    return;
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            Loading.ShowLoading("Communication with one of the drones is failing\n" + ex);

                            return;
                        }
                        // set drone target position
                        drone.TargetLocation = drone.Location;
                        drone.TargetLocation.Alt = takeoffalt;

                        try
                        {
                            // takeoff
                            if (MAV.cs.alt < takeoffalt - 0.5 && !drone.takeoffdone)
                                if (MAV.parent.doCommand(MAV.sysid, MAV.compid, MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0,
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
                        drone.MavState.GuidedMode.z = (float) drone.TargetLocation.Alt;

                        // wait for takeoff
                        if (MAV.cs.alt < takeoffalt - 0.5)
                        {
                            Thread.Sleep(100);
                            // check we are still armed
                            if (!MAV.cs.armed)
                                return;

                            // move on to next drone
                            continue;
                        }

                        // we should only get here once takeoff alt has been archived by this drone.

                        // position control
                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        drone.MavState.GuidedMode.x = (float) drone.TargetLocation.Lat;
                        drone.MavState.GuidedMode.y = (float) drone.TargetLocation.Lng;
                        drone.MavState.GuidedMode.z = (float) drone.TargetLocation.Alt;
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
                        if (MAV.cs.alt < takeoffalt - 0.5)
                        {
                            Thread.Sleep(100);
                            // check we are still armed
                            if (!MAV.cs.armed)
                                return;

                            // reloop to force takeoff 
                            return;
                        }
                    }
                    CurrentMode = Mode.guided;
                    break;
                case Mode.guided:
                    foreach (var drone in Drones)
                    {
                        // set drone target position
                        drone.TargetLocation = GetUserPosition(drone);

                        // position control
                        drone.SendPositionVelocity(drone.TargetLocation, drone.TargetVelocity);

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

        private PointLatLngAlt GetUserPosition(Drone drone)
        {
            // project on current heading 1 seconds ahead - testing
            var newpos = drone.Location.newpos(drone.Heading, drone.speed);

            drone.TargetVelocity = new Vector3(Math.Cos(drone.Heading * Math.PI / 180.0),
                                       Math.Sin(drone.Heading * Math.PI / 180.0), 0).normalized() * drone.speed;

            // can still move around, but alt limited
            if (newpos.Alt > FenceMaxAlt)
                newpos.Alt = FenceMaxAlt;

            // cant move around, as alt is the issue
            if (newpos.Alt < FenceMinAlt + 0.5)
            {
                newpos = drone.Location;
                newpos.Alt = FenceMinAlt + 0.5;
            }

            // close the loop if needed
            if (!Fence.Last().Equals(Fence.First()))
            {
                Fence.Add(Fence.First());
            }

            if (!PointInPolygon(newpos, Fence))
            {
                var bear = drone.Location.GetBearing(Fence.First());
                newpos = drone.Location.newpos(bear, 0.2);
                drone.TargetVelocity = Vector3.Zero;
            }

            Console.WriteLine("{0} {1} {2}",drone.MavState.sysid, newpos, drone.TargetVelocity.ToString());

            return newpos;
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