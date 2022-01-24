using System.Collections.Generic;
using NodaTime;

namespace AltitudeAngelWings.Models
{
    public class FlightPlan
    {
        public FlightPlan(IEnumerable<FlightPlanWaypoint> waypoints)
        {
            Waypoints = new List<FlightPlanWaypoint>(waypoints);
        }

        public IList<FlightPlanWaypoint> Waypoints { get; }
        public FlightCapability FlightCapability { get; set; }
        public double CenterLatitude { get; set; }
        public double CenterLongitude { get; set; }
        public int BoundingRadius { get; set; }
        public FlightOperationMode FlightOperationMode { get; set; }

        /// <summary>
        /// The serial number of the drone that will be flying to this flight plan, if applicable
        /// </summary>
        public string DroneSerialNumber { get; set; }

        public string Summary { get; set; }
        public string Description { get; set; }
        public bool AllowSmsContact { get; set; }
        public string SmsPhoneNumber { get; set; }
        public Duration Duration { get; set; }
        public bool UseLocalConflictScope { get; set; }
    }
}