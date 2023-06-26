using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Models;

namespace AltitudeAngelWings.Service.FlightData.Providers
{
    public class MissionPlannerFlightDataProvider : IFlightDataProvider
    {
        public MissionPlannerFlightDataProvider(IMissionPlannerState missionPlannerState)
        {
            _missionPlannerState = missionPlannerState;
        }

        public Models.FlightData GetCurrentFlightData()
        {
            return new Models.FlightData
            {
                Armed = _missionPlannerState.IsArmed,
                CurrentPosition = new FlightDataPosition
                {
                    Longitude = _missionPlannerState.Longitude,
                    Latitude = _missionPlannerState.Latitude,
                    Altitude = (int) _missionPlannerState.Altitude,
                    Course = _missionPlannerState.GroundCourse,
                    Speed = _missionPlannerState.GroundSpeed,
                    VerticalSpeed = _missionPlannerState.VerticalSpeed
                }
            };
        }

        private readonly IMissionPlannerState _missionPlannerState;
    }
}
