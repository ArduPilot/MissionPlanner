using System.Collections.Generic;
using AltitudeAngelWings.Clients.Flight.Model;
using AltitudeAngelWings.Model;

namespace AltitudeAngelWings
{
    public interface IMissionPlannerState
    {
        bool IsArmed { get; }
        double Longitude { get; }
        double Latitude { get; }
        float Altitude { get; }
        float GroundSpeed { get; }
        float GroundCourse { get; }
        bool IsConnected { get; }
        float VerticalSpeed { get; }
        FlightCapability FlightCapability { get; }
        IList<FlightPlanWaypoint> Waypoints { get; }
        FlightPlanWaypoint HomeLocation { get; }
    }
}
