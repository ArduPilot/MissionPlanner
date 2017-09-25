using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.HIL;
using MissionPlanner.Utilities;
using ZedGraph;

namespace MissionPlanner.Swarm.WaypointLeader
{
    public class Drone : DroneBase
    {
        public int PathIndex = 0;

        public bool takeoffdone = false;
    }
}
