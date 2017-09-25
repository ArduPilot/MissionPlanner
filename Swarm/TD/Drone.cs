using System;
using MissionPlanner.HIL;
using MissionPlanner.Utilities;

namespace MissionPlanner.Swarm.TD
{
    public class Drone : DroneBase
    {
        public double bubblerad = 2.0;

        public double speed = 2.0;

        public bool takeoffdone = false;
    }
}