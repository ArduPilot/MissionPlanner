using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.ApiClient.Models.FlightV2;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration;
using AltitudeAngelWings.ApiClient.Models.Strategic;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Polly;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class AltitudeAngelClient : IAltitudeAngelClient
    {
        private FlurlClient _client;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAsyncPolicy _asyncPolicy;
        private readonly ISettings _settings;

        public AltitudeAngelClient(
            ISettings settings,
            IHttpClientFactory clientFactory,
            IAsyncPolicy asyncPolicy)
        {
            _settings = settings;
            _clientFactory = clientFactory;
            _asyncPolicy = asyncPolicy;
        }

        protected FlurlClient Client
            => _client ?? (_client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = _clientFactory
                }
            });

        /// <summary>
        ///     Disconnect the client from AA. Will force logon on the next request if required.
        /// </summary>
        public void Disconnect(bool resetAuth = false)
        {
            _client?.Dispose();
            _client = null;
            if (resetAuth)
            {
                _settings.TokenResponse = null;
            }
        }

        public Task<AAFeatureCollection> GetMapData(BoundingLatLong latLongBounds, CancellationToken cancellationToken)
            => _asyncPolicy.ExecuteAsync(() => _settings.ApiUrl
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
                    .WithClient(Client)
                    .GetJsonAsync<AAFeatureCollection>(cancellationToken));

        /// <summary>
        ///     Get the weather for the specified location. Do not call this method often, typically only once per session.
        ///     Required scopes: talk_tower
        /// </summary>
        /// <param name="latLong">The location.</param>
        /// <returns>The weather info for the current conditions.</returns>
        public async Task<WeatherInfo> GetWeather(LatLong latLong)
        {
            var flightInfo = new FlightInfo
            {
                Position = new LatLong(latLong.Latitude, latLong.Longitude)
            };

            var aircraftInfo = new AircraftInfo
            {
                Id = "Id"
            };

            // Load this as a JObject as reports are extensible
            var reportResponse = await _asyncPolicy.ExecuteAsync(async () => await _settings.ApiUrl
                .AppendPathSegments("ops", "tower", "report")
                .WithClient(Client)
                .PostJsonAsync(new ReportRequest(aircraftInfo, flightInfo, "weather"))
                .ReceiveJson<JObject>());

            // Grab the weather for now if there is one and process it
            var currentWeather = reportResponse.SelectToken("weather.forecast.current")?.ToObject<WeatherInfo>();

            return currentWeather;
        }

        /// <summary>
        ///     Get the user profile for the current user. Must be using user auth. Required scopes: query_userinfo
        /// </summary>
        /// <returns>The user profile.</returns>
        public Task<UserProfileInfo> GetUserProfile()
            =>_asyncPolicy.ExecuteAsync(() => _settings.AuthenticationUrl
                .AppendPathSegment("userProfile")
                .WithClient(Client)
                .GetJsonAsync<UserProfileInfo>());

        /// <summary>
        /// Creates a flight plan via FlightService API
        /// </summary>
        /// <param name="flightPlan"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public Task<CreateStrategicPlanResponse> CreateFlightPlan(FlightPlan flightPlan, UserProfileInfo currentUser)
        {
            var parts = CreateFlightPartsFromWaypoints(flightPlan.Waypoints, flightPlan.Duration);
            return CreateFlightPlan(flightPlan, parts, currentUser);
        }

        public Task<StartFlightResponse> StartFlight(string flightPlanId)
        {
            var startFlightRequest = new StartFlightRequest
            {
                FlightPlanId = flightPlanId,
                ServiceRequests = new List<IFlightServiceRequest>
                {
                    new TacticalDeconflictionFlightServiceRequest {Properties = new TacticalDeconflictionRequestProperties
                    {
                        Guidance = new List<string> {"vector"},
                        NotificationProtocols = new List<object> {new {type = "Websocket"}},
                        TelemetryProtocols = new List<object> {new {type = "Udp"}},
                        Scope = "global",
                        SurveillanceResolution = true
                    }}
                }
            };

            return _asyncPolicy.ExecuteAsync(() => _settings.FlightServiceUrl
                .AppendPathSegments("flight", "v2", "flights")
                .WithClient(Client)
                .ConfigureRequest(settings =>
                {
                    settings.JsonSerializer = CreateNewtonsoftJsonSerializer();
                })
                .PostJsonAsync(startFlightRequest)
                .ReceiveJson<StartFlightResponse>());
        }

        /// <summary>
        /// Completes a flight via FlightService API
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns></returns>
        public Task CompleteFlight(string flightId)
            => _asyncPolicy.ExecuteAsync(() => _settings.FlightServiceUrl
                .AppendPathSegments("flight", "v2", "flights", flightId)
                .WithClient(Client)
                .DeleteAsync());

        public Task CancelFlightPlan(string flightPlanId)
            => _asyncPolicy.ExecuteAsync(() => _settings.FlightServiceUrl
                .AppendPathSegments("v1", "conflict-resolution", "strategic", "flight-plans", flightPlanId, "cancel")
                .WithClient(Client)
                .PostAsync());

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
                _client = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static NewtonsoftJsonSerializer CreateNewtonsoftJsonSerializer() => new NewtonsoftJsonSerializer(CreateJsonSerializerSettings());

        private static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None
            };

            jsonSerializerSettings.Converters.Add(new BaseNotificationProtocolConfigurationConverter());
            jsonSerializerSettings.Converters.Add(new BaseTelemetryProtocolConfigurationConverter());
            jsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return jsonSerializerSettings;
        }
        
        private static (Instant start, Instant end) GetFlightPlanStartEndInstants(Duration duration)
        {
            var startInstant = SystemClock.Instance.GetCurrentInstant() + Duration.FromSeconds(30);
            var endInstant = startInstant + duration;
            return (startInstant, endInstant);
        }

        private static CreateFlightPartRequest CreateFlightPartRequest(
            int id,
            int highestAltitude,
            Instant start,
            Instant end,
            Feature geography)
            => new CreateFlightPartRequest
            {
                Id = id.ToString(),
                Geography = geography,
                Start = start,
                End = end,
                TimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault(),
                MaxAltitude = new Altitude { Meters = highestAltitude, Datum = AltitudeDatum.Agl }
            };

        /// <summary>
        /// Creates a flight plan via FlightService API
        /// </summary>
        /// <param name="flightPlan"></param>
        /// <param name="parts"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        private async Task<CreateStrategicPlanResponse> CreateFlightPlan(
            FlightPlan flightPlan,
            IEnumerable<CreateFlightPartRequest> parts,
            UserProfileInfo currentUser)
        {
            var sParts = parts.Select(p => new CreateStrategicPlanPartRequest
            {
                Id = p.Id,
                MaxAltitude = p.MaxAltitude,
                Start = p.Start,
                End = p.End,
                Geography = FeatureToGeometryJson(p.Geography)
            }).ToList();

            var identifiers = new Dictionary<string, string>();
            if (_settings.FlightIdentifierIcao)
            {
                identifiers.Add("icao", _settings.FlightIdentifierIcaoAddress);
            }
            if (_settings.FlightIdentifierSerial)
            {
                identifiers.Add("serialNumber", _settings.FlightIdentifierSerialNumber);
            }
            if (identifiers.Count == 0)
            {
                identifiers = null;
            }

            var req = new CreateStrategicPlanRequest
            {
                ConflictResolutionScope = flightPlan.UseLocalConflictScope ? StrategicConflictResolutionScope.Local : StrategicConflictResolutionScope.Global,
                Summary = flightPlan.Summary,
                Description = flightPlan.Description,
                Parts = sParts,
                PointOfContact = new StrategicContactDetails
                {
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    PhoneNumber = _settings.FlightPhoneNumber,
                    AllowSmsContact = _settings.FlightAllowSms
                },
                Identifiers = identifiers,
                DroneDetails = new StrategicDroneDetails
                {
                    AirFrame = MapToAirFrame(flightPlan.FlightCapability),
                    MaxWeight = 1,
                    Weight = 1
                }
            };

            return await _asyncPolicy.ExecuteAsync(async () => await _settings.FlightServiceUrl
                .AppendPathSegments("v1", "conflict-resolution", "strategic", "flight-plans")
                .WithClient(Client)
                .ConfigureRequest(settings =>
                {
                    settings.JsonSerializer = CreateNewtonsoftJsonSerializer();
                })
                .PostJsonAsync(req)
                .ReceiveJson<CreateStrategicPlanResponse>());
        }

        private static StrategicAirframeType MapToAirFrame(FlightCapability flightCapability)
        {
            switch (flightCapability)
            {
                case FlightCapability.FixedWing:
                    return StrategicAirframeType.FixedWing;
                case FlightCapability.Rotary:
                    return StrategicAirframeType.Rotary;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flightCapability), flightCapability, "Failed to map FlightCapability to StrategicAirframeType.");
            }
        }

        private static JObject FeatureToGeometryJson(Feature feature) 
            // Getting an IGeometry converter is a massive pain due to dependency and framework conflicts. Taking the geometry
            // out of the feature that has working converters works best.
            => (JObject)JObject.FromObject(feature)["geometry"];

        private static IEnumerable<CreateFlightPartRequest> CreateFlightPartsFromWaypoints(IList<FlightPlanWaypoint> waypoints, Duration duration)
        {
            var flightPartRequests = new List<CreateFlightPartRequest>();
            var highestAltitude = waypoints.Max(wp => wp.Altitude);
            var (startInstant, endInstant) = GetFlightPlanStartEndInstants(duration);

            for (var pos = 1; pos < waypoints.Count; pos++)
            {
                if (Math.Abs(waypoints[pos - 1].Latitude - waypoints[pos].Latitude) < 0.0000001 &&
                    Math.Abs(waypoints[pos - 1].Longitude - waypoints[pos].Longitude) < 0.0000001) continue;
                var geography = new Feature(ConvertWaypointsToLineString(waypoints[pos - 1], waypoints[pos]));
                var createFlightPartRequest = CreateFlightPartRequest(pos, highestAltitude, startInstant, endInstant, geography);
                flightPartRequests.Add(createFlightPartRequest);
            }

            return flightPartRequests;
        }

        private static IGeometryObject ConvertWaypointsToLineString(FlightPlanWaypoint startWayPoint, FlightPlanWaypoint endWayPoint)
            => new LineString(new List<IPosition> {
                new Position(startWayPoint.Latitude, startWayPoint.Longitude),
                new Position(endWayPoint.Latitude, endWayPoint.Longitude)
            });
    }
}
