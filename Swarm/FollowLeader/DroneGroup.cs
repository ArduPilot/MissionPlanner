using System;
using System.Collections.Generic;
using System.Linq;
using MissionPlanner.Utilities;

namespace MissionPlanner.Swarm.FollowLeader
{
    public class DroneGroup
    {
        List<PointLatLngAlt> trail = new List<PointLatLngAlt>();

        public List<Drone> Drones = new List<Drone>();
        public MAVState airmaster;
        public MAVState groundmaster;
        public Drone AirMasterDrone { get { return Drones.First(i => i.MavState == airmaster); } }
        public Drone GroundMasterDrone { get { return Drones.First(i => i.MavState == groundmaster); } }

        public double Seperation { get; set; }
        public double Lead { get; set; }
        public double Altitude { get; set; }

        public DroneGroup()
        {
            Seperation = 5;
            Lead = 20;
            Altitude = 10;
        }

        public void UpdatePositions()
        {
            // add new point to trail
            trail.Add(new PointLatLngAlt(GroundMasterDrone.MavState.cs.lat, GroundMasterDrone.MavState.cs.lng, GroundMasterDrone.MavState.cs.alt,""));

            while (trail.Count > 1000)
                trail.RemoveAt(0);

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

                drone.TargetVelocity = GroundMasterDrone.Velocity;
            }

            var targetbearing = GroundMasterDrone.Heading;

            //
            if (GroundMasterDrone.MavState.cs.wp_dist < Seperation*1.5)
            {
                var headingtowp = (int) GroundMasterDrone.MavState.cs.wpno;
                var nextwp = headingtowp + 1;

                try
                {
                    PointLatLngAlt targetwp = new PointLatLngAlt(GroundMasterDrone.MavState.wps[headingtowp]);
                    //PointLatLngAlt targetwp = GroundMasterDrone.Location;
                    PointLatLngAlt nexttargetwp = new PointLatLngAlt(GroundMasterDrone.MavState.wps[nextwp]);

                    var bearing = targetwp.GetBearing(nexttargetwp);

                    // point on the wp line for target
                    var targetpnt = targetwp.newpos(bearing, Seperation);

                    targetbearing = GroundMasterDrone.Location.GetBearing(targetpnt);

                    if (Math.Abs(targetbearing - bearing) > 20)
                    {
                        //targetbearing = bearing;
                    }

                    AirMasterDrone.TargetVelocity.x = Math.Cos(targetbearing*MathHelper.deg2rad)*
                                                      GroundMasterDrone.MavState.cs.groundspeed;
                    AirMasterDrone.TargetVelocity.y = Math.Sin(targetbearing*MathHelper.deg2rad)*
                                                      GroundMasterDrone.MavState.cs.groundspeed;
                }
                catch
                {
                }
            }
            else
            {
                
            }

            // calc airmaster position
            AirMasterDrone.TargetLocation = GroundMasterDrone.Location.newpos(targetbearing, Seperation);
            AirMasterDrone.TargetLocation.Alt = Altitude;

            // send new position to airmaster
            AirMasterDrone.SendPositionVelocity(AirMasterDrone.TargetLocation, AirMasterDrone.TargetVelocity * 0.6);

            AirMasterDrone.MavState.GuidedMode.x = (float)AirMasterDrone.TargetLocation.Lat;
            AirMasterDrone.MavState.GuidedMode.y = (float)AirMasterDrone.TargetLocation.Lng;
            AirMasterDrone.MavState.GuidedMode.z = (float)AirMasterDrone.TargetLocation.Alt;

            // get the path
            List<PointLatLngAlt> newpositions = PlanMove();

            List<PointLatLngAlt> newlist = new List<PointLatLngAlt>();
            newlist.Add(GroundMasterDrone.Location);
            newlist.AddRange(newpositions);

            newpositions = newlist;

            int a = 0;
            // send position and velocity
            foreach (var drone in Drones)
            {
                if(drone.MavState == airmaster)
                    continue;

                if (drone.MavState == groundmaster)
                    continue;

                if (a > (newpositions.Count - 1))
                    break;

                newpositions[a].Alt += Altitude;

                // spline control
                drone.SendPositionVelocity(newpositions[a], drone.TargetVelocity/2);

                drone.MavState.GuidedMode.x = (float)newpositions[a].Lat;
                drone.MavState.GuidedMode.y = (float)newpositions[a].Lng;
                drone.MavState.GuidedMode.z = (float) newpositions[a].Alt;

                // vel only
                //drone.SendVelocity(drone.TargetVelocity);

                a++;
            }
        }

        List<PointLatLngAlt> PlanMove()
        {
            List<PointLatLngAlt> currentpos = new List<PointLatLngAlt>();

            // get current pos
            foreach (var drone in Drones)
            {
                currentpos.Add(drone.Location);
            }

            // check they are not to close already

            foreach (var lla in currentpos)
            {
                foreach (var lla2 in currentpos)
                {
                    double dist = lla.GetDistance(lla2);

                    if (dist < (Seperation / 2))
                    {
                        // do nothing yet
                        //Stop();
                        //return;
                    }
                }
            }

            // get planned pos.
            List<PointLatLngAlt> pathwithseperation = new List<PointLatLngAlt>();

            PointLatLngAlt current = trail.Last();

            // generate path with distance gaps
            for (int a = 0; a < 20; a++)
            {
                PointLatLngAlt target = FindTrailPnt(current);
                if (target != null)
                {
                    pathwithseperation.Add(target);
                    current = target;
                }
            }

            return pathwithseperation;

            // find closest MAV


            // check intersect
        }

        PointLatLngAlt FindTrailPnt(PointLatLngAlt from)
        {
            // get the start point for the distance
            int start = trail.IndexOf(from);

            for (int i = start; i > 0; i--)
            {
                double dist = from.GetDistance(trail[i]); // 2d distance
                if (dist > Seperation)
                {
                    return trail[i];
                }
            }

            return null;
        }
    }
}
