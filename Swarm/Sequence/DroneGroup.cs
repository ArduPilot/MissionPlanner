using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.Swarm.Sequence
{
    public class DroneGroup
    {
        public enum Mode
        {
            idle,
            running
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

        public ConcurrentBag<Drone> Drones = new ConcurrentBag<Drone>();
        
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

                  
                }
            }

            switch (CurrentMode)
            {
                case Mode.idle:
                    
                    // request positon at 10hz
                    foreach (var drone in Drones)
                    {
                        var MAV = drone.MavState;

                        MAV.parent.requestDatastream(MAVLink.MAV_DATA_STREAM.POSITION, 10, MAV.sysid, MAV.compid);
                        MAV.cs.rateposition = 10;
                        
                    }

                    CurrentMode = Mode.running;
                    break;

                case Mode.running:

                    break;
            }
        }

        static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }
}