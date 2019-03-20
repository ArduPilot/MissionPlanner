using System;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Models;
using DotNetOpenAuth.OAuth2;
using Flurl;
using Flurl.Http;
using GMap.NET;
using Newtonsoft.Json.Linq;
using TimeZoneConverter;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class AltitudeAngelClient : IAltitudeAngelClient
    {
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffff";

        private readonly string _apiUrl;
        private readonly string _authUrl;
        private readonly FlurlClient _client;
        private readonly AltitudeAngelHttpHandlerFactory _handlerFactory;

        public IAuthorizationState AuthorizationState => _handlerFactory.AuthorizationState;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="authUrl">The base auth URL (scheme and host) for the service.</param>
        /// <param name="apiUrl">The base API URL (scheme and host) for the service.</param>
        /// <param name="existingState">Any existing state from a previous session.</param>
        /// <param name="handlerFactory">A delegate to create the http handler factory.</param>
        public AltitudeAngelClient(
            string authUrl,
            string apiUrl,
            IAuthorizationState existingState,
            Func<string, IAuthorizationState, AltitudeAngelHttpHandlerFactory> handlerFactory)
        {
            _authUrl = authUrl;
            _apiUrl = apiUrl;
            _handlerFactory = handlerFactory(authUrl, existingState);
            _client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = _handlerFactory
                }
            };
        }

        /// <summary>
        ///     Disconnect the client from AA. Will force logon on the next request if required.
        /// </summary>
        public void Disconnect()
        {
            _client.Dispose();
            _handlerFactory.ClearAuthState();
        }

        /// <summary>
        ///     Get the map data from the AA api service.
        ///     Required scopes: query_mapdata query_mapairdata
        /// </summary>
        /// <param name="latLongBounds">The bounds of the request.</param>
        /// <returns>A collection of GeoJSON features.</returns>
        public Task<AAFeatureCollection> GetMapData(RectLatLng latLongBounds)
        {
            return _apiUrl
                .AppendPathSegments("v2", "mapdata", "geojson")
                .SetQueryParams(new
                {
                    n = latLongBounds.Top,
                    e = latLongBounds.Right,
                    s = latLongBounds.Bottom,
                    w = latLongBounds.Left,
                    isCompact = false,
                    include = "flight_report"
                })
                .WithClient(_client)
                .GetJsonAsync<AAFeatureCollection>();
        }

        /// <summary>
        ///     Get the weather for the specified location. Do not call this method often, typically only once per session.
        ///     Required scopes: talk_tower
        /// </summary>
        /// <param name="latLong">The location.</param>
        /// <returns>The weather info for the current conditions.</returns>
        public async Task<WeatherInfo> GetWeather(PointLatLng latLong)
        {
            var flightInfo = new FlightInfo
            {
                Position = new LatLong(latLong)
            };

            var aircraftInfo = new AircraftInfo
            {
                Id = "Id"
            };

            // Load this as a JObject as reports are extensible
            JObject reportResponse = await _apiUrl
                .AppendPathSegments("ops", "tower", "report")
                .WithClient(_client)
                .PostJsonAsync(new ReportRequest(aircraftInfo, flightInfo, "weather"))
                .ReceiveJson<JObject>();

            // Grab the weather for now if there is one and process it
            var currentWeather = reportResponse.SelectToken("weather.forecast.current")?.ToObject<WeatherInfo>();

            return currentWeather;
        }

        /// <summary>
        ///     Get the user profile for the current user. Must be using user auth. Required scopes: query_userinfo
        /// </summary>
        /// <returns>The user profile.</returns>
        public Task<UserProfileInfo> GetUserProfile()
        {
            return _authUrl
                .AppendPathSegment("userProfile")
                .WithClient(_client)
                .GetJsonAsync<UserProfileInfo>();
        }

        public async Task<string> CreateFlightReport(string flightReportName, bool isCommerial, DateTime localStartTime, DateTime localEndTime, PointLatLng location, int radius)
        {
            var response = await _apiUrl
                .AppendPathSegments("flightReport")
                .WithClient(_client)
                .PutJsonAsync(new
                {
                    name = flightReportName,
                    flight_type = isCommerial ? "com" : "rec",
                    timezone = TZConvert.WindowsToIana(TimeZoneInfo.Local.Id),
                    start = localStartTime.ToString(DateTimeFormat),
                    end = localEndTime.ToString(DateTimeFormat),
                    radius_meters = radius,
                    loc = new
                    {
                        lat = location.Lat,
                        lng = location.Lng
                    }
                })
                .ReceiveJson<JObject>();
            return response.SelectToken("flightId").ToString();
        }

        public Task CompleteFlightReport(string flightId)
        {
            return _apiUrl
                .AppendPathSegments("flightReport", flightId)
                .WithClient(_client)
                .DeleteAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
