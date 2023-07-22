using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Service;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class ApiClient : IApiClient
    {
        private readonly ISettings _settings;
        private readonly IFlurlClient _client;

        public ApiClient(ISettings settings, IHttpClientFactory clientFactory)
        {
            _settings = settings;
            _client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = clientFactory
                }
            };
        }

        public Task<MapFeatureCollection> GetMapData(BoundingLatLong latLongBounds, CancellationToken cancellationToken)
            => _settings.ApiUrl
                .AppendPathSegments("v2", "mapdata", "geojson")
                .SetQueryParams(new
                {
                    n = latLongBounds.NorthEast.Latitude,
                    e = latLongBounds.NorthEast.Longitude,
                    s = latLongBounds.SouthWest.Latitude,
                    w = latLongBounds.SouthWest.Longitude,
                    isCompact = false,
                    useNewFilters = true,
                    include = "flight_report,flight_restrictions"
                })
                .WithClient(_client)
                .GetJsonAsync<MapFeatureCollection>(cancellationToken);

        public Task<RateCardDetail> GetRateCard(string rateCardId, CancellationToken cancellationToken)
            => _settings.ApiUrl
                .AppendPathSegments("v2", "mapdata", "rate-cards", rateCardId)
                .WithClient(_client)
                .GetJsonAsync<RateCardDetail>(cancellationToken);

        public async Task<WeatherInfo> GetWeather(LatLong latLong)
        {
            var reportResponse = await _settings.ApiUrl
                .AppendPathSegments("ops", "tower", "report")
                .WithClient(_client)
                .PostJsonAsync(new ReportRequest(
                    new AircraftInfo
                    {
                        Id = "Id"
                    },
                    new FlightInfo
                    {
                        Position = new LatLong(latLong.Latitude, latLong.Longitude)
                    },
                    "weather"))
                .ReceiveJson<JObject>();
            return reportResponse.SelectToken("weather.forecast.current")?.ToObject<WeatherInfo>();
        }
    }
}