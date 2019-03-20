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
                    Heading = (int) _missionPlannerState.MagneticDeclination,
                    Speed = _missionPlannerState.GroundSpeed
                }
            };
        }

        private readonly IMissionPlannerState _missionPlannerState;
    }
}
