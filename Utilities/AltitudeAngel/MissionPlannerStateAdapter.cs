using System;
using AltitudeAngelWings.Extra;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    internal class MissionPlannerStateAdapter : IMissionPlannerState
    {
        private readonly Func<CurrentState> _getCurrentState;

        public MissionPlannerStateAdapter(Func<CurrentState> getCurrentState)
        {
            _getCurrentState = getCurrentState;
        }

        public bool IsArmed => _getCurrentState().armed;
        public double Longitude => _getCurrentState().lng;
        public double Latitude => _getCurrentState().lat;
        public float Altitude => _getCurrentState().alt;
        public float GroundSpeed => _getCurrentState().groundspeed;
        public float MagneticDeclination => _getCurrentState().mag_declination;
        public bool IsConnected => _getCurrentState().connected;
    }
}