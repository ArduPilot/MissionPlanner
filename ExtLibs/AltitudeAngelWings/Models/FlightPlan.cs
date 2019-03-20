using System.Collections.Generic;

namespace AltitudeAngelWings.Models
{
    public class FlightPlan
    {
        public FlightPlan(IEnumerable<FlightPlanWaypoint> waypoints)
        {
            Waypoints = new List<FlightPlanWaypoint>(waypoints);
        }

        public IList<FlightPlanWaypoint> Waypoints { get; }
        public double CenterLatitude { get; set; }
        public double CenterLongitude { get; set; }
        public int BoundingRadius { get; set; }
    }
}