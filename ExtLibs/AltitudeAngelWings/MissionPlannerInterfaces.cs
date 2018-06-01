using System;

namespace AltitudeAngel.IsolatedPlugin.Common
{
    [Serializable]
    public class MissionPlannerInterfaces
    {
        public IMissionPlanner MissionPlanner { get; set; }
        //public IFlightComms FlightComms { get; set; }
        public IMissionPlannerState CurrentState { get; set; }
    }
}
