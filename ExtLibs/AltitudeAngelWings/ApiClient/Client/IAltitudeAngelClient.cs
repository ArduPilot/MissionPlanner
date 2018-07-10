using System;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Models;
using DotNetOpenAuth.OAuth2;
using GMap.NET;

namespace AltitudeAngelWings.ApiClient.Client
{
    public interface IAltitudeAngelClient : IDisposable
    {
        IAuthorizationState AuthorizationState { get; }
        void Disconnect();
        Task<AAFeatureCollection> GetMapData(RectLatLng latLongBounds);
        Task<WeatherInfo> GetWeather(PointLatLng latLong);
        Task<UserProfileInfo> GetUserProfile();
        Task<string> CreateFlightReport(string flightReportName, bool isCommerial, DateTime localStartTime, DateTime localEndTime, PointLatLng location, int radius);
        Task CompleteFlightReport(string flightId);
    }
}