using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http.Configuration;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests;
using AltitudeAngelWings.ApiClient.Models.FlightV2;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration;
using AltitudeAngelWings.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using AltitudeAngelWings.ApiClient.Models.Strategic;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Models;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Threading;

namespace AltitudeAngelWings.ApiClient.Client.FlightClient
{
    public class FlightClient : IFlightClient
    {
        private readonly ISettings _settings;
        private readonly FlurlClient _client;

        public FlightClient(ISettings settings, IHttpClientFactory clientFactory)
        {
            _settings = settings;
            _client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = clientFactory,
                }
            };
        }

        public Task<CreateStrategicPlanResponse> CreateFlightPlan(FlightPlan flightPlan, UserProfileInfo currentUser, CancellationToken cancellationToken = default)
        {
            var parts = CreateFlightPartsFromWaypoints(flightPlan.Waypoints, flightPlan.Duration);
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

            return _settings.FlightServiceUrl
                .AppendPathSegments("v1", "conflict-resolution", "strategic", "flight-plans")
                .WithClient(_client)
                .ConfigureRequest(settings =>
                {
                    settings.JsonSerializer = CreateNewtonsoftJsonSerializer();
                })
                .PostJsonAsync(req, cancellationToken)
                .ReceiveJson<CreateStrategicPlanResponse>();
        }

        public Task<StartFlightResponse> StartFlight(string flightPlanId, CancellationToken cancellationToken = default)
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

            return _settings.FlightServiceUrl
                .AppendPathSegments("flight", "v2", "flights")
                .WithClient(_client)
                .ConfigureRequest(settings =>
                {
                    settings.JsonSerializer = CreateNewtonsoftJsonSerializer();
                })
                .PostJsonAsync(startFlightRequest, cancellationToken)
                .ReceiveJson<StartFlightResponse>();
        }

        public Task CompleteFlight(string flightId, CancellationToken cancellationToken = default)
            => _settings.FlightServiceUrl
                .AppendPathSegments("flight", "v2", "flights", flightId)
                .WithClient(_client)
                .DeleteAsync(cancellationToken);

        public Task CompleteFlightPlan(string flightPlanId, CancellationToken cancellationToken = default)
            => _settings.FlightServiceUrl
                .AppendPathSegments("flightapprovals", flightPlanId, "complete")
                .WithClient(_client)
                .PostAsync(cancellationToken: cancellationToken);

        public Task CancelFlightPlan(string flightPlanId, CancellationToken cancellationToken = default)
            => _settings.FlightServiceUrl
                .AppendPathSegments("flightapprovals", flightPlanId, "cancel")
                .WithClient(_client)
                .PostAsync(cancellationToken: cancellationToken);

        public Task AcceptInstruction(string instructionId, CancellationToken cancellationToken = default) => ProcessInstruction(instructionId, true, cancellationToken);

        public Task RejectInstruction(string instructionId, CancellationToken cancellationToken = default) => ProcessInstruction(instructionId, false, cancellationToken);

        private Task ProcessInstruction(string instructionId, bool accept, CancellationToken cancellationToken = default)
            => _settings.FlightServiceUrl
                .AppendPathSegments("flight", "v2", "instructions", instructionId, accept ? "accept" : "reject")
                .WithClient(_client)
                .PutAsync(cancellationToken: cancellationToken);

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

        private static CreateFlightPartRequest CreateFlightPartRequest(
            int id,
            double highestAltitude,
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

        private static (Instant start, Instant end) GetFlightPlanStartEndInstants(Duration duration)
        {
            var startInstant = SystemClock.Instance.GetCurrentInstant() + Duration.FromSeconds(30);
            var endInstant = startInstant + duration;
            return (startInstant, endInstant);
        }

        private static IGeometryObject ConvertWaypointsToLineString(FlightPlanWaypoint startWayPoint, FlightPlanWaypoint endWayPoint)
            => new LineString(new List<IPosition> {
                new Position(startWayPoint.Latitude, startWayPoint.Longitude),
                new Position(endWayPoint.Latitude, endWayPoint.Longitude)
            });
    }
}
