using System;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.ApiClient.Models.FlightV2;
using AltitudeAngelWings.ApiClient.Models.Strategic;
using AltitudeAngelWings.Models;

namespace AltitudeAngelWings.ApiClient.Client
{
    public interface IAltitudeAngelClient : IDisposable
    {
        void Disconnect(bool resetAuth = false);
        Task<AAFeatureCollection> GetMapData(BoundingLatLong latLongBounds, CancellationToken cancellationToken);
        Task<WeatherInfo> GetWeather(LatLong latLong);
        Task<UserProfileInfo> GetUserProfile();
        Task CompleteFlight(string flightId);
        Task<CreateStrategicPlanResponse> CreateFlightPlan(FlightPlan flightPlan, UserProfileInfo currentUser);
        Task<StartFlightResponse> StartFlight(string flightPlanId);
        Task CancelFlightPlan(string flightPlanId);
    }
}