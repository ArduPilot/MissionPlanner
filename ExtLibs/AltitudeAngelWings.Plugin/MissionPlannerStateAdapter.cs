using System;
using System.Collections.Generic;
using AltitudeAngelWings.Clients.Flight.Model;
using AltitudeAngelWings.Model;
using MissionPlanner;

namespace AltitudeAngelWings.Plugin
{
    internal class MissionPlannerStateAdapter : IMissionPlannerState
    {
        private readonly Func<CurrentState> _getCurrentState;
        private readonly Func<IList<FlightPlanWaypoint>> _getCurrentWaypoints;
        private readonly Func<FlightCapability> _getFlightCapability;

        public MissionPlannerStateAdapter(Func<CurrentState> getCurrentState, Func<IList<FlightPlanWaypoint>> getCurrentWaypoints, Func<FlightCapability> getFlightCapability)
        {
            _getCurrentState = getCurrentState;
            _getCurrentWaypoints = getCurrentWaypoints;
            _getFlightCapability = getFlightCapability;
        }

        public bool IsArmed => _getCurrentState().armed;
        public double Longitude => _getCurrentState().lng;
        public double Latitude => _getCurrentState().lat;
        public float Altitude => _getCurrentState().alt;
        public float GroundSpeed => _getCurrentState().groundspeed;
        public float GroundCourse => _getCurrentState().groundcourse;
        public bool IsConnected => _getCurrentState().connected;
        public float VerticalSpeed => _getCurrentState().verticalspeed;
        public FlightCapability FlightCapability => _getFlightCapability();
        public IList<FlightPlanWaypoint> Waypoints => _getCurrentWaypoints();
        public FlightPlanWaypoint HomeLocation
        {
            get
            {
                var home = _getCurrentState().PlannedHomeLocation;
                return new FlightPlanWaypoint
                {
                    Latitude = home.Lat,
                    Longitude = home.Lng,
                    Altitude = home.Alt
                };
            }
        }
    }
}