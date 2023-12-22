using System;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients.Api.Model;

namespace AltitudeAngelWings.Clients.Api
{
    public interface IApiClient : IDisposable
    {
        Task<MapFeatureCollection> GetMapData(BoundingLatLong latLongBounds, CancellationToken cancellationToken = default);
        Task<RateCardDetail> GetRateCard(string rateCardId, CancellationToken cancellationToken = default);
        Task<WeatherInfo> GetWeather(LatLong latLong);
    }
}