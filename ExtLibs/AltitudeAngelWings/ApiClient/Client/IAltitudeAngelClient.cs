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
        Task<MapFeatureCollection> GetMapData(BoundingLatLong latLongBounds, CancellationToken cancellationToken = default);
        Task<WeatherInfo> GetWeather(LatLong latLong);
        Task<UserProfileInfo> GetUserProfile(CancellationToken cancellationToken = default);
        Task CompleteFlight(string flightId);
        Task<CreateStrategicPlanResponse> CreateFlightPlan(FlightPlan flightPlan, UserProfileInfo currentUser);
        Task<StartFlightResponse> StartFlight(string flightPlanId);
        Task CancelFlightPlan(string flightPlanId);
        Task<RateCardDetail> GetRateCard(string rateCardId, CancellationToken cancellationToken = default);
    }
}