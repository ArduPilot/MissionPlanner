using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.ApiClient.Client.FlightClient;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration;
using AltitudeAngelWings.ApiClient.Models.Strategic;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.Messaging;
using AltitudeAngelWings.Service.OutboundNotifications;
using Newtonsoft.Json.Linq;
using NodaTime;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using NetTopologySuite.Algorithm;

namespace AltitudeAngelWings.Service.FlightService
{
    public class FlightService : IFlightService
    {
        private readonly IMessagesService _messagesService;
        private readonly IMissionPlannerState _missionPlannerState;
        private readonly CompositeDisposable _disposer = new CompositeDisposable();
        private readonly IFlightClient _flightClient;
        private readonly IAuthClient _authClient;
        private readonly ISettings _settings;
        private readonly IOutboundNotificationsService _notificationsService;

        public FlightService(
            IMessagesService messagesService,
            IMissionPlannerState missionPlannerState,
            ISettings settings,
            IFlightDataService flightDataService,
            IFlightClient flightClient,
            IAuthClient authClient,
            IOutboundNotificationsService notificationsService)
        {
            _messagesService = messagesService;
            _missionPlannerState = missionPlannerState;
            _settings = settings;
            _flightClient = flightClient;
            _authClient = authClient;
            _notificationsService = notificationsService;
            _settings.CurrentFlightPlanId = null;
            _settings.CurrentFlightId = null;

            _disposer.Add(flightDataService.FlightArmed
                .SubscribeWithAsync(async (i, ct) => await StartTelemetryFlight(ct)));
            _disposer.Add(flightDataService.FlightDisarmed
                .SubscribeWithAsync((i, ct) => CompleteFlight(ct)));
        }

        private async Task StartTelemetryFlight(CancellationToken cancellationToken)
        {
            if (!(_settings.UseFlightPlans && _settings.UseFlights && _settings.TokenResponse.HasScopes(Scopes.StrategicCrs, Scopes.TacticalCrs)))
            {
                return;
            }

            var flightPlan = GetFlightPlan();
            if (flightPlan == null)
            {
                return;
            }
            if (_settings.CurrentFlightId != null)
            {
                // Complete flight if starting one before the previous ends.
                await CompleteFlight(cancellationToken);
            }

            // TODO somehow prevent Arming of UAV until the following try statement has been completed, so telemetry isn't sent late. PBI 8490
            Guid? flightPlanId = null;
            try
            {
                if (_settings.UseExistingFlightPlanId)
                {
                    flightPlanId = _settings.ExistingFlightPlanId;
                }
                else
                {
                    await _messagesService.AddMessageAsync(Message.ForInfo("FlightPlan", "Creating flight plan."));
                    var profile = await _authClient.GetUserProfile(_settings.TokenResponse.AccessToken, cancellationToken);
                    var flightPlanRequest = ConstructFlightPlanRequest(flightPlan, profile);
                    var createPlanResponse = await _flightClient.CreateFlightPlan(flightPlanRequest, cancellationToken);
                    if (createPlanResponse.Outcome == StrategicSeverity.DirectConflict)
                    {
                        await _messagesService.AddMessageAsync(Message.ForInfo("FlightPlan", "Flight plan conflict detected; flight cancelled.", TimeSpan.FromSeconds(10)));
                        return;
                    }

                    flightPlanId = createPlanResponse.FlightPlanId;
                }
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(Message.ForError("FlightPlan", "Flight plan create failed.", ex));
            }

            try
            {
                // Check flight plan id is valid
                if (flightPlanId == null || flightPlanId == Guid.Empty)
                {
                    await _messagesService.AddMessageAsync(Message.ForInfo("Flight", "Flight plan not available; flight cancelled.", TimeSpan.FromSeconds(10)));
                    return;
                }
                _settings.CurrentFlightPlanId = flightPlanId.ToString();

                // Flight being rejected will throw, and cause a disarm
                await _messagesService.AddMessageAsync(Message.ForInfo("Flight", $"Starting flight on flight plan {flightPlanId}.", TimeSpan.FromSeconds(10)));
                var startFlightResponse = await _flightClient.StartFlight(flightPlanId.Value.ToString("D"), cancellationToken);

                _settings.CurrentFlightId = startFlightResponse.Id;
                var tacticalSettings = startFlightResponse.ServiceResponses.First();

                var notificationSettings = (WebsocketNotificationProtocolConfiguration)tacticalSettings.Properties.NotificationProtocols.First();
                _settings.OutboundNotificationsUrl = notificationSettings.Properties.Endpoints.First();

                var telemetrySettings = (UdpTelemetryProtocolConfiguration)tacticalSettings.Properties.TelemetryProtocols.First();
                _settings.CurrentTelemetryId = telemetrySettings.Id;
                _settings.EncryptionKey = telemetrySettings.Properties.EncryptionKey;

                var telemetryEndPoint = telemetrySettings.Properties.Endpoints.First();
                _settings.TelemetryHostName = telemetryEndPoint.Split(':')[0];
                _settings.TelemetryPortNumber = int.Parse(telemetryEndPoint.Split(':')[1]);
                _settings.TransmissionRateInMilliseconds = telemetrySettings.Properties.TransmissionRateInMilliseconds;

                var task = _notificationsService.StartWebSocket();

                await _messagesService.AddMessageAsync(Message.ForInfo("Flight", $"Flight {startFlightResponse.Id} approved and underway.", TimeSpan.FromSeconds(10)));
                await task;
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(Message.ForError("Flight", "Flight create failed.", ex));
            }
        }

        private async Task CompleteFlight(CancellationToken cancellationToken)
        {
            if (!(_settings.UseFlightPlans && _settings.UseFlights && _settings.TokenResponse.HasScopes(Scopes.StrategicCrs, Scopes.TacticalCrs)))
            {
                return;
            }
            if (string.IsNullOrEmpty(_settings.CurrentFlightId) || string.IsNullOrEmpty(_settings.CurrentFlightPlanId))
            {
                return;
            }
            try
            {
                await _flightClient.CompleteFlight(_settings.CurrentFlightId, cancellationToken);
                await _messagesService.AddMessageAsync(Message.ForInfo("FlightComplete", $"Flight {_settings.CurrentFlightId} completed.", TimeSpan.FromSeconds(10)));

                //await _flightClient.CompleteFlightPlan(_settings.CurrentFlightPlanId, cancellationToken);
                //await _messagesService.AddMessageAsync(Message.ForInfo($"Flight plan {_settings.CurrentFlightPlanId} completed.", TimeSpan.FromSeconds(10)));
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(Message.ForError("FlightComplete", $"Failed to complete flight {_settings.CurrentFlightId}.", ex));
            }
            finally
            {
                await _notificationsService.StopWebSocket();
                _settings.CurrentFlightId = null;
                _settings.CurrentFlightPlanId = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _disposer?.Dispose();
            }
        }

        public FlightPlan GetFlightPlan()
        {
            var waypoints = _missionPlannerState.Waypoints;
            if (waypoints.Count == 0)
            {
                return null;
            }

            waypoints.Insert(0, _missionPlannerState.HomeLocation);
            var envelope = NetTopologySuite.Geometries.GeometryFactory.Default.CreateMultiPoint(
                    waypoints
                        .Select(l => new NetTopologySuite.Geometries.Point(l.Longitude, l.Latitude))
                        .ToArray())
                .Envelope;
            var center = envelope.Centroid;
            var minimumBoundingCircle = new MinimumBoundingCircle(envelope);
            return new FlightPlan(waypoints)
            {
                CenterLongitude = center.X,
                CenterLatitude = center.Y,
                BoundingRadius = (int)Math.Ceiling(minimumBoundingCircle.GetRadius()),
                FlightCapability = _missionPlannerState.FlightCapability,
                Summary =  _settings.FlightPlanName,
                Description = _settings.FlightPlanDescription,
                Duration = Duration.FromTimeSpan(_settings.FlightPlanTimeSpan),
                UseLocalConflictScope = false,
                AllowSmsContact = false,
                SmsPhoneNumber = "",
                DroneSerialNumber = "",
                FlightOperationMode = FlightOperationMode.BVLOS
            };
        }

        private CreateStrategicPlanRequest ConstructFlightPlanRequest(FlightPlan flightPlan, UserProfileInfo profile)
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

            return new CreateStrategicPlanRequest
            {
                ConflictResolutionScope = flightPlan.UseLocalConflictScope ? StrategicConflictResolutionScope.Local : StrategicConflictResolutionScope.Global,
                Summary = flightPlan.Summary,
                Description = flightPlan.Description,
                Parts = sParts,
                PointOfContact = new StrategicContactDetails
                {
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
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
