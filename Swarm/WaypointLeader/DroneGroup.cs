using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using ZedGraph;

namespace MissionPlanner.Swarm.WaypointLeader
{
    public class DroneGroup
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<Drone> Drones = new List<Drone>();
        public MAVState airmaster;
        public MAVState groundmaster;
        public Drone AirMasterDrone { get { return Drones.First(i => i.MavState == airmaster); } }
        public Drone GroundMasterDrone { get { return Drones.First(i => i.MavState == groundmaster); } }

        public double Seperation { get; set; }
        public double Lead { get; set; }
        public double OffPathTrigger { get; set; }
        public bool V { get; set; }
        public double Takeoff_Land_alt_sep { get; set; }
        public bool AltInterleave { get; set; }
        public decimal WPNAV_ACCEL { get; internal set; }

        public PointPairList path_to_fly = new PointPairList();

        internal int pathcount = 0;

        public Mode CurrentMode = Mode.idle;

        public enum Mode
        {
            idle,
            takeoff,
            flytouser,
            followuser,
            RTH,
            LandAlt,
            Land
        }

        public DroneGroup()
        {
            Seperation = 5;
            Lead = 20;
            OffPathTrigger = 10;
            Takeoff_Land_alt_sep = 2;
            AltInterleave = false;
            WPNAV_ACCEL = 1;
        }

        public void UpdatePositions()
        {
            if (airmaster == null || groundmaster == null)
                return;

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

                // set default target as ground reference
                drone.TargetVelocity = GroundMasterDrone.Velocity;


            }


            // generate a new list including the airmaster first
            List<Drone> newlist = new List<Drone>();
            newlist.Add(AirMasterDrone);
            // get list to remove airmaster and groundmaster
            List<Drone> newlist2 = new List<Drone>();
            newlist2.AddRange(Drones);
            newlist2.Remove(AirMasterDrone);
            newlist2.Remove(GroundMasterDrone);
            // add modified list back
            newlist.AddRange(newlist2);

            // collision check
            foreach (var drone1 in newlist)
            {
                // ground drone is not part of the check
                if (drone1 == GroundMasterDrone)
                    continue;

                if (!drone1.MavState.cs.armed)
                    continue;

                foreach (var drone2 in newlist)
                {
                    if(drone1 == drone2)
                        continue;

                    if (drone2 == GroundMasterDrone)
                        continue;

                    if(!drone2.MavState.cs.armed)
                        continue;

                    // check how close they are based on current position
                    if (drone1.Location.GetDistance(drone2.Location) < Seperation / 2)
                    {
                        // check if the alt seperation is less than 1m
                        if (Math.Abs(drone1.Location.Alt - drone2.Location.Alt) < 1)
                        {
                            // check which is higher already, and seperate further
                            if (drone1.Location.Alt > drone2.Location.Alt)
                            {
                                drone1.SendPositionVelocity(new PointLatLngAlt(drone1.Location) { Alt = drone1.Location.Alt + Takeoff_Land_alt_sep }, Vector3.Zero);
                                return;
                            }
                            else
                            {
                                drone2.SendPositionVelocity(new PointLatLngAlt(drone2.Location) { Alt = drone2.Location.Alt + Takeoff_Land_alt_sep }, Vector3.Zero);
                                return;
                            }
                        }
                    }

                        // check how close they are based on a 1 second projection
                        if (drone1.ProjectedLocation.GetDistance(drone2.ProjectedLocation) < Seperation/2)
                    {
                        // check if they are heading the same direction
                        if (((Math.Abs(drone1.Heading - drone2.Heading)+360)%360) < 45 && drone1.MavState.cs.groundspeed > 0.5)
                        {
                            // check if the alt seperation is less than 1m
                            if (Math.Abs(drone1.Location.Alt - drone2.Location.Alt) < 1)
                            {
                                // they are heading within 45 degrees of each other
                                // return here to let them settle themselfs, as the target position will be correct
                                // ie the ground refrence is moving faster than the drones can maintain
                                Console.WriteLine("1 drone, to close");
                                drone1.SendPositionVelocity(drone1.Location, Vector3.Zero);
                                return;
                            }
                        }

                        // check if the are heading are at each other
                        if (((Math.Abs(drone1.Heading - drone2.Heading)+360)%360) > 135 )
                        {
                            // check if the alt seperation is less than 1m
                            if (Math.Abs(drone1.Location.Alt - drone2.Location.Alt) < 1)
                            {
                                // stop the drones
                                drone1.SendPositionVelocity(new PointLatLngAlt(drone1.Location) { Alt = drone1.Location.Alt + Takeoff_Land_alt_sep }, Vector3.Zero);
                                drone2.SendPositionVelocity(drone2.Location, Vector3.Zero);
                                Console.WriteLine("2 stopping drone, to close and heading towards each other");
                                return;
                            }
                        }
                    }
                }
            }

            // convert wp path to 0.1m path increments
            var path = Path.GeneratePath(AirMasterDrone.MavState);

            if (path.Count == 0)
                return;

            // update the graph to show location along path
            if (pathcount != path.Count)
            {
                path_to_fly.Clear();
                double inc = 0;
                path.ForEach(i =>
                {
                    path_to_fly.Add(inc, i.Alt);
                    inc += 0.1;
                });
            }
            pathcount = path.Count;

            foreach (var drone in Drones)
            {
                var locs = GetLocations(path, drone.Location, 0, 0);
                if (locs.Count == 0)
                    continue;
                drone.PathIndex = path.IndexOf(locs[0]);
            }
            

            switch (CurrentMode)
            {
                case Mode.idle:
                    CurrentMode = Mode.takeoff;
                    // request positon at 5hz
                    foreach (var drone in Drones)
                    {
                        var MAV = drone.MavState;

                        MAV.parent.requestDatastream(MAVLink.MAV_DATA_STREAM.POSITION, 5, MAV.sysid, MAV.compid);
                        MAV.cs.rateposition = 5;

                        drone.takeoffdone = false;

                        if (drone != GroundMasterDrone)
                        {
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
                                MAV.parent.setParam(MAV.sysid, MAV.compid, "WPNAV_ACCEL", 100);
                            }
                            catch
                            {
                            }
                        }
                    }
                    break;
                case Mode.takeoff:
                    // newposition index
                    int a = 0;
                    // calc new positions based on lead and seperation
                    var lead = Drones.Count * Seperation;
                    // get locations based on position of the airmaster with a negative lead
                    var newpositions = GetLocations(path, AirMasterDrone.MavState.cs.HomeLocation, lead, Seperation);
                    if (newpositions.Count == 0)
                        return;

                    // from here airmaster will be first, all other drones will be in order they will fly in (could sort based on sysid?)
                    foreach (var drone in newlist)
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
                            Loading.ShowLoading("Communication with one of the drones is failing\n"+ex.ToString());

                            return;
                        }
                        // set drone target position
                        drone.TargetLocation = newpositions[a];
                        // setup seperation (0=0,1=1,2=2,3=0)
                        drone.TargetLocation.Alt += Takeoff_Land_alt_sep * (a % 3);

                        float takeoffalt = (float)drone.TargetLocation.Alt;

                        try
                        {
                            // takeoff
                            if (MAV.cs.alt < (takeoffalt - 0.5) && !drone.takeoffdone)
                                if (MAV.parent.doCommand(MAV.sysid, MAV.compid, MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0,
                                    0, takeoffalt))
                                {
                                    drone.takeoffdone = true;
                                }                            
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            Loading.ShowLoading("Communication with one of the drones is failing\n" + ex.ToString());

                            return;
                        }

                        drone.MavState.GuidedMode.x = 0;
                        drone.MavState.GuidedMode.y = 0;
                        drone.MavState.GuidedMode.z = (float)drone.TargetLocation.Alt;

                        // wait for takeoff
                        if (MAV.cs.alt < (takeoffalt - 0.5))
                        {
                            System.Threading.Thread.Sleep(100);
                            // check we are still armed
                            if (!MAV.cs.armed)
                                return;

                            a++;
                            // move on to next drone
                            continue;
                        }

                        // we should only get here once takeoff alt has been archived by this drone.

                        // position control
                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        drone.MavState.GuidedMode.x = (float)drone.TargetLocation.Lat;
                        drone.MavState.GuidedMode.y = (float)drone.TargetLocation.Lng;
                        drone.MavState.GuidedMode.z = (float)drone.TargetLocation.Alt;

                        // check how far off target we are
                        if (drone.TargetLocation.GetDistance(drone.Location) > Seperation)
                        {
                            // only return if this is the third drone without seperation
                            if (a%3 == 2)
                            {
                                //if we are off target, we have already sent the command to this drone,
                                //but skip the one behind it untill this one is within the seperation range 
                                return;
                            }
                        }

                        a++;
                    }
                    // wait for all to get within seperation distance
                    foreach (var drone in newlist)
                    {
                        // check how far off target we are
                        if (drone.TargetLocation.GetDistance(drone.Location) > Seperation)
                        {
                            // position control
                            drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                            //if we are off target, we have already sent the command to this drone,
                            //but skip the one behind it untill this one is within the seperation range 
                            return;
                        }
                    }
                    CurrentMode = Mode.flytouser;
                    break;
                case Mode.flytouser:
                    // get locations based on position of the airmaster
                    var newpositions2 = GetLocations(path, AirMasterDrone.Location, Seperation, Seperation);
                    if (newpositions2.Count == 0)
                    {
                        // check if we are within 5m of the end of our flightplan
                        if (path.Last().GetDistance(AirMasterDrone.Location) < Seperation)
                            CurrentMode = Mode.RTH;
                        return;
                    }

                    int b = 0;
                    foreach (var drone in newlist)
                    {
                        if (newpositions2.Count == b)
                            break;
                        // set drone target position
                        drone.TargetLocation = newpositions2[b];

                        if (AltInterleave)
                            drone.TargetLocation.Alt += Takeoff_Land_alt_sep * (b % 2);

                        // position control
                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        drone.MavState.GuidedMode.x = (float)drone.TargetLocation.Lat;
                        drone.MavState.GuidedMode.y = (float)drone.TargetLocation.Lng;
                        drone.MavState.GuidedMode.z = (float)drone.TargetLocation.Alt;

                        b++;
                    }

                    // this is the same as used in the next step, to prevent a jump
                    var newpositionsfollowuser = GetLocations(path, GroundMasterDrone.Location, Lead, Seperation);
                    // check how far off target we are
                    if (newpositionsfollowuser.Count > 0 && AirMasterDrone.Location.GetDistance(newpositionsfollowuser.First()) < Seperation)
                    {
                        // update speed as we are changing to a high dynamic mode
                        foreach (var drone in newlist)
                        {
                            if (drone == GroundMasterDrone)
                                continue;
                            var MAV = drone.MavState;
                            // update to faster speed
                            MAV.parent.setParam(MAV.sysid, MAV.compid, "WPNAV_ACCEL", (float)WPNAV_ACCEL*100.0f);

                            MAV.parent.setMode(MAV.sysid, MAV.compid, "GUIDED");
                        }
                        //if we are off target, we have already sent the command to this drone,
                        //but skip the one behind it untill this one is within the seperation range 
                        CurrentMode = Mode.followuser;
                        return;
                    }

                    break;
                case Mode.followuser:
                    // calc new positions based on lead and seperation
                    List<PointLatLngAlt> newpositions3 = new List<PointLatLngAlt>();
                    if (V)
                    {
                        newpositions3 = GetLocationsV(path, GroundMasterDrone.Location, Lead, Seperation);
                    }
                    else
                    {
                        newpositions3 = GetLocations(path, GroundMasterDrone.Location, Lead, Seperation);
                    }

                    if (newpositions3.Count == 0)
                        return;

                    //  check if the user is offpath
                    var wps = new List<PointLatLngAlt>();
                    AirMasterDrone.MavState.wps.Values.ForEach(i => { wps.Add(new PointLatLngAlt((Locationwp)i)); });

                    if (GetOffPathDistance(wps, GroundMasterDrone.Location) > OffPathTrigger)
                    {
                        CurrentMode = Mode.RTH;
                    }

                    int c = 0;
                    // send position and velocity
                    foreach (var drone in newlist)
                    {

                        if (drone.MavState == groundmaster)
                            continue;

                        if (c > (newpositions3.Count - 1))
                            break;

                        drone.TargetLocation = newpositions3[c];

                        if (AltInterleave)
                            drone.TargetLocation.Alt += Takeoff_Land_alt_sep * (c % 2);

                        // spline control
                        drone.SendPositionVelocity(drone.TargetLocation, drone.TargetVelocity / 3);

                        drone.MavState.GuidedMode.x = (float)drone.TargetLocation.Lat;
                        drone.MavState.GuidedMode.y = (float)drone.TargetLocation.Lng;
                        drone.MavState.GuidedMode.z = (float)drone.TargetLocation.Alt;

                        // vel only
                        //drone.SendVelocity(drone.TargetVelocity);

                        // check how far off target we are
                        if (drone.TargetLocation.GetDistance(drone.Location) > Seperation * 2)
                        {
                            //if we are off target, we have already sent the command to this drone,
                            //but skip the one behind it untill this one is within the seperation range 

                            //break;
                        }

                        c++;
                    }
                    break;
                case Mode.RTH:
                    // get locations based on position of the airmaster
                    var newpositions4 = GetLocations(path, AirMasterDrone.Location, Seperation, Seperation);
                    if (newpositions4.Count == 0)
                    {
                        if (AirMasterDrone.Location.GetDistance(path.Last()) < Seperation)
                            CurrentMode = Mode.LandAlt;
                        return;
                    }

                    int d = 0;
                    foreach (var drone in newlist)
                    {
                        if (d > (newpositions4.Count - 1))
                            break;

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

                        // set drone target position
                        drone.TargetLocation = newpositions4[d];

                        // used in next step
                        double prevalt = drone.TargetLocation.Alt;

                        if (AltInterleave)
                            drone.TargetLocation.Alt += Takeoff_Land_alt_sep * (d % 2);

                        // position control
                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        drone.MavState.GuidedMode.x = (float)drone.TargetLocation.Lat;
                        drone.MavState.GuidedMode.y = (float)drone.TargetLocation.Lng;
                        drone.MavState.GuidedMode.z = (float)drone.TargetLocation.Alt;

                        // used for next step
                        drone.TargetLocation.Alt = prevalt;

                        d++;
                    }
                    break;
                case Mode.LandAlt:
                    int e = 0;
                    foreach (var drone in newlist)
                    {
                        drone.TargetLocation.Alt += Takeoff_Land_alt_sep * e;

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

                        drone.MavState.GuidedMode.z = (float)drone.TargetLocation.Alt;

                        System.Threading.Thread.Sleep(200);

                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        e++;
                    }
                    // check status
                    foreach (var drone in newlist)
                    {
                        // wait for alt hit
                        while (drone.MavState.cs.alt < (drone.TargetLocation.Alt-0.5))
                        {
                            if (!drone.MavState.cs.armed)
                                break;
                            System.Threading.Thread.Sleep(200);
                            drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);
                        }

                        System.Threading.Thread.Sleep(200);

                        log.Info(drone.MavState.sysid + " " + drone.MavState.cs.alt + " at alt " + drone.TargetLocation.Alt);

                        // set mode rtl
                        drone.MavState.parent.setMode(drone.MavState.sysid, drone.MavState.compid, "RTL");
                    }
                    CurrentMode = Mode.Land;
                    break;
                case Mode.Land:
                    Drone closest = new Drone() {Location = PointLatLngAlt.Zero};
                    var lastpnt = path.Last();
                    foreach (var drone in newlist)
                    {
                        if(!drone.MavState.cs.armed)
                            continue;

                        // low flying filter, move onto next drone
                        if (drone.MavState.cs.alt < lastpnt.Alt-1)
                            continue;

                        if (AirMasterDrone.MavState.cs.HomeLocation.GetDistance(drone.Location) < AirMasterDrone.MavState.cs.HomeLocation.GetDistance(closest.Location))
                        {
                            closest = drone;
                        }
                    }

                    if(closest.MavState!=null && !closest.MavState.cs.mode.ToLower().Equals("rtl"))
                        closest.MavState.parent.setMode(closest.MavState.sysid, closest.MavState.compid, "RTL");
                    break;
            }
        }

        double GetOffPathDistance(List<PointLatLngAlt> path, PointLatLngAlt Location)
        {
            double disttotal=double.MaxValue;
            PointLatLngAlt lineStartLatLngAlt = null;
            // check all segments
            foreach (var pathpoint in path)
            {
                if (lineStartLatLngAlt == null)
                {
                    lineStartLatLngAlt = new PointLatLngAlt(pathpoint.Lat,
                        pathpoint.Lng);
                    continue;
                }

                // crosstrack distance
                var lineEndLatLngAlt = new PointLatLngAlt(pathpoint.Lat, pathpoint.Lng);

                var lineDist = lineStartLatLngAlt.GetDistance2(lineEndLatLngAlt);

                var distToLocation = lineStartLatLngAlt.GetDistance2(Location);
                var bearToLocation = lineStartLatLngAlt.GetBearing(Location);
                var lineBear = lineStartLatLngAlt.GetBearing(lineEndLatLngAlt);

                var angle = bearToLocation - lineBear;
                if (angle < 0)
                    angle += 360;

                var alongline = Math.Cos(angle * MathHelper.deg2rad) * distToLocation;

                // check to see if our point is still within the line length
                if (Math.Abs(alongline) > lineDist)
                {
                    lineStartLatLngAlt = lineEndLatLngAlt;
                    continue;
                }

                var dXt2 = Math.Sin(angle * MathHelper.deg2rad) * distToLocation;

                disttotal = Math.Min(disttotal, Math.Abs(dXt2));

                lineStartLatLngAlt = lineEndLatLngAlt;
            }

            // check also distance from the points - because if we are outside the polygon, we may be on a corner segment
            foreach (var pathpoint in path)
            {
                var dXt2 = pathpoint.GetDistance(Location);
                disttotal = Math.Min(disttotal, Math.Abs(dXt2));
            }

            return disttotal;
        }

        private List<PointLatLngAlt> GetLocationsV(List<PointLatLngAlt> path, PointLatLngAlt location, double lead,
            double seperation)
        {
           List<PointLatLngAlt> result = new List<PointLatLngAlt>();
            var list = GetLocations(path, location, lead, seperation);

            if (list.Count == 0)
                return result;

            var front = list.First();

            result.Add(front);

            int a = 1;

            foreach (var pointLatLngAlt in list)
            {
                if (pointLatLngAlt == front)
                    continue;

                var left = pointLatLngAlt.newpos(pointLatLngAlt.GetBearing(front) + 90, seperation/2 * a);
                var right = pointLatLngAlt.newpos(pointLatLngAlt.GetBearing(front) - 90, seperation/2 * a);

                result.Add(left);
                result.Add(right);

                a++;
            }

            return result;
        }

        private List<PointLatLngAlt> GetLocations(List<PointLatLngAlt> path, PointLatLngAlt location, double lead, double seperation)
        {
            List<PointLatLngAlt> result = new List<PointLatLngAlt>();

            // get the current location closest point
            PointLatLngAlt closestPointLatLngAlt = PointLatLngAlt.Zero;
            double mindist = 99999999;
            foreach (var pointLatLngAlt in path)
            {
                var distloc = location.GetDistance(pointLatLngAlt);
                if (distloc < mindist)
                {
                    mindist = distloc;
                    closestPointLatLngAlt = pointLatLngAlt;
                }
            }

            var start = path.IndexOf(closestPointLatLngAlt);
            var a = 0;
            for (int i = start; i < (path.Count - 1); i++)
            {
                var targetdistance = lead - a * seperation;

                if (targetdistance < 0)
                    i-=2;

                if (i < 0)
                    break;

                double dist = closestPointLatLngAlt.GetDistance(path[i]); // 2d distance
                if (dist >= Math.Abs(targetdistance))
                {
                    result.Add(path[i]);
                    i = start;

                    if (result.Count > 20)
                        break;
                    if (seperation == 0)
                        break;
                    a++;
                }
            }

            return result;
        }
    }
}
