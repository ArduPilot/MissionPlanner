using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Models;

namespace AltitudeAngelWings.ApiClient.Client
{
    public interface IApiClient
    {
        Task<MapFeatureCollection> GetMapData(BoundingLatLong latLongBounds, CancellationToken cancellationToken = default);
        Task<RateCardDetail> GetRateCard(string rateCardId, CancellationToken cancellationToken = default);
        Task<WeatherInfo> GetWeather(LatLong latLong);
    }
}