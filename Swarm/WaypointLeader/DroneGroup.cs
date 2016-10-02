using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Core.ExtendedObjects;
using MissionPlanner.HIL;
using MissionPlanner.Utilities;

namespace MissionPlanner.Swarm.WaypointLeader
{
    public class DroneGroup
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public List<Drone> Drones = new List<Drone>();
        public MAVState airmaster;
        public MAVState groundmaster;
        public Drone AirMasterDrone { get { return Drones.First(i => i.MavState == airmaster); } }
        public Drone GroundMasterDrone { get { return Drones.First(i => i.MavState == groundmaster); } }

        public double Seperation { get; set; }
        public double Lead { get; set; }
        public double OffPathTrigger { get; set; }
        public bool V { get; set; }

        public Mode CurrentMode = Mode.idle;

        public enum Mode
        {
            idle,
            takeoff,
            flytouser,
            followuser,
            RTH,
            Land
        }

        public DroneGroup()
        {
            Seperation = 5;
            Lead = 20;
            OffPathTrigger = 10;
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
                drone.Velocity.x = Math.Cos(drone.MavState.cs.groundcourse*deg2rad)*drone.MavState.cs.groundspeed;
                drone.Velocity.y = Math.Sin(drone.MavState.cs.groundcourse*deg2rad)*drone.MavState.cs.groundspeed;
                drone.Velocity.z = drone.MavState.cs.verticalspeed;

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

                    // check how close they are based on a 1 second projection
                    if (drone1.ProjectedLocation.GetDistance(drone2.ProjectedLocation) < Seperation/2)
                    {
                        // check if they are heading the same direction
                        if (((Math.Abs(drone1.Heading - drone2.Heading)+360)%360) < 45 && drone1.MavState.cs.groundspeed > 0.5)
                        {
                            // they are heading within 45 degrees of each other
                            // return here to let them settle themselfs, as the target position will be correct
                            // ie the ground refrence is moving faster than the drones can maintain
                            Console.WriteLine("1 drone, to close");
                            return;
                        }

                        // check if the are heading are at each other
                        if (((Math.Abs(drone1.Heading - drone2.Heading)+360)%360) > 135 )
                        {
                            // stop the drones
                            drone1.SendPositionVelocity(drone1.Location, Vector3.Zero);
                            drone2.SendPositionVelocity(drone2.Location, Vector3.Zero);
                            Console.WriteLine("2 stopping drone, to close and heading towards each other");
                            return;
                        }
                    }
                }
            }

            // convert wp path to 0.1m path increments
            var path = Path.GeneratePath(AirMasterDrone.MavState);

            if (path.Count == 0)
                return;

            switch (CurrentMode)
            {
                case Mode.idle:
                    CurrentMode = Mode.takeoff;
                    // request positon at 5hz
                    foreach (var drone in Drones)
                    {
                        drone.MavState.parent.requestDatastream(MAVLink.MAV_DATA_STREAM.POSITION, 5, drone.MavState.sysid,drone.MavState.compid);
                        drone.MavState.cs.rateposition = 5;
                    }
                    return;
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
                        // guided mode
                        if (!MAV.cs.mode.ToLower().Equals("guided"))
                            MAV.parent.setMode(MAV.sysid, MAV.compid, "GUIDED");
                        // arm
                        if (!MAV.cs.armed)
                            if (!MAV.parent.doARM(MAV.sysid, MAV.compid, true))
                                return;
                        // takeoff
                        if (MAV.cs.alt < 3)
                            if (MAV.parent.doCommand(MAV.sysid, MAV.compid, MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 5))
                                return;

                        // wait for takeoff
                        while (MAV.cs.alt < 3)
                        {
                            System.Threading.Thread.Sleep(100);
                            // check we are still armed
                            if (!MAV.cs.armed)
                                return;
                        }

                        // set drone target position
                        drone.TargetLocation = newpositions[a];

                        // position control
                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        drone.MavState.GuidedMode.x = (float) newpositions[a].Lat;
                        drone.MavState.GuidedMode.y = (float) newpositions[a].Lng;
                        drone.MavState.GuidedMode.z = (float) newpositions[a].Alt;

                        // check how far off target we are
                        if (drone.TargetLocation.GetDistance(drone.Location) > Seperation)
                        {
                            //if we are off target, we have already sent the command to this drone,
                            //but skip the one behind it untill this one is within the seperation range 
                            return;
                        }

                        a++;
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

                        // position control
                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        drone.MavState.GuidedMode.x = (float) newpositions2[b].Lat;
                        drone.MavState.GuidedMode.y = (float) newpositions2[b].Lng;
                        drone.MavState.GuidedMode.z = (float) newpositions2[b].Alt;

                        b++;
                    }

                    // this is the same as used in the next step, to prevent a jump
                    var newpositionsfollowuser = GetLocations(path, GroundMasterDrone.Location, Lead, Seperation);
                    // check how far off target we are
                    if (newpositionsfollowuser.Count > 0 && AirMasterDrone.Location.GetDistance(newpositionsfollowuser.First()) < Seperation)
                    {
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

                    // calc airmaster position
                    AirMasterDrone.TargetLocation = newpositions3[0];

                    newpositions3.RemoveAt(0);

                    // send new position to airmaster
                    AirMasterDrone.SendPositionVelocity(AirMasterDrone.TargetLocation, GroundMasterDrone.TargetVelocity / 3);

                    // update for display
                    AirMasterDrone.MavState.GuidedMode.x = (float)AirMasterDrone.TargetLocation.Lat;
                    AirMasterDrone.MavState.GuidedMode.y = (float)AirMasterDrone.TargetLocation.Lng;
                    AirMasterDrone.MavState.GuidedMode.z = (float)AirMasterDrone.TargetLocation.Alt;

                    // check how far off target we are
                    if (AirMasterDrone.TargetLocation.GetDistance(AirMasterDrone.Location) > Seperation * 2)
                    {
                        //if we are off target, we have already sent the command to this drone,
                        //but skip the one behind it untill this one is within the seperation range 

                        return;
                    }

                    int c = 0;
                    // send position and velocity
                    foreach (var drone in newlist)
                    {
                        if (drone.MavState == airmaster)
                            continue;

                        if (drone.MavState == groundmaster)
                            continue;

                        if (c > (newpositions3.Count - 1))
                            break;

                        drone.TargetLocation = newpositions3[c];

                        // spline control
                        drone.SendPositionVelocity(drone.TargetLocation, drone.TargetVelocity / 3);

                        drone.MavState.GuidedMode.x = (float)newpositions3[c].Lat;
                        drone.MavState.GuidedMode.y = (float)newpositions3[c].Lng;
                        drone.MavState.GuidedMode.z = (float)newpositions3[c].Alt;

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
                            CurrentMode = Mode.Land;
                        return;
                    }

                    int d = 0;
                    foreach (var drone in newlist)
                    {
                        // set drone target position
                        drone.TargetLocation = newpositions4[d];

                        // position control
                        drone.SendPositionVelocity(drone.TargetLocation, Vector3.Zero);

                        drone.MavState.GuidedMode.x = (float)newpositions4[d].Lat;
                        drone.MavState.GuidedMode.y = (float)newpositions4[d].Lng;
                        drone.MavState.GuidedMode.z = (float)newpositions4[d].Alt;

                        d++;
                    }
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

                var alongline = Math.Cos(angle * deg2rad) * distToLocation;

                // check to see if our point is still within the line length
                if (Math.Abs(alongline) > lineDist)
                {
                    lineStartLatLngAlt = lineEndLatLngAlt;
                    continue;
                }

                var dXt2 = Math.Sin(angle * deg2rad) * distToLocation;

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
                    a++;
                }
            }

            return result;
        }
    }
}
