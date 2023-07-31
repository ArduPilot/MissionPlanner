using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients;
using AltitudeAngelWings.Clients.Auth;
using AltitudeAngelWings.Clients.Auth.Model;
using AltitudeAngelWings.Clients.Flight;
using AltitudeAngelWings.Clients.Flight.Model;
using AltitudeAngelWings.Clients.Flight.Model.ServiceRequests;
using AltitudeAngelWings.Clients.Flight.Model.ServiceRequests.ProtocolConfiguration;
using AltitudeAngelWings.Model;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.Messaging;
using AltitudeAngelWings.Service.OutboundNotifications;
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
            if (!(_settings.UseFlightPlans && _settings.UseFlights && _settings.TokenResponse.HasScopes(Scopes.ManageFlightReports)))
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
                    await _messagesService.AddMessageAsync(Message.ForInfo("FlightPlan", $"Flight plan created. Approval status is {createPlanResponse.Status.State}.", TimeSpan.FromSeconds(30)));
                    flightPlanId = createPlanResponse.Id;
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
                    await _messagesService.AddMessageAsync(Message.ForInfo("Flight", "Flight plan not available.", TimeSpan.FromSeconds(10)));
                    return;
                }
                _settings.CurrentFlightPlanId = flightPlanId.ToString();

                // Flight being rejected will throw, and cause a disarm
                await _messagesService.AddMessageAsync(Message.ForInfo("Flight", $"Starting flight on flight plan {flightPlanId}.", TimeSpan.FromSeconds(10)));
                var startFlightRequest = new StartFlightRequest
                {
                    FlightPlanId = flightPlanId.ToString()
                };
                if (_settings.TokenResponse.HasScopes(Scopes.TacticalCrs))
                {
                    startFlightRequest.ServiceRequests.Add(new TacticalDeconflictionFlightServiceRequest
                    {
                        Properties = new TacticalDeconflictionRequestProperties
                        {
                            Guidance = new List<string> {"vector"},
                            NotificationProtocols = new List<object> {new {type = "Websocket"}},
                            TelemetryProtocols = new List<object> {new {type = "Udp"}},
                            Scope = "global",
                            SurveillanceResolution = true
                        }
                    });
                }

                var startFlightResponse = await _flightClient.StartFlight(startFlightRequest, cancellationToken);

                _settings.CurrentFlightId = startFlightResponse.Id;
                var tacticalSettings = startFlightResponse.ServiceResponses.OfType<TacticalDeconflictionFlightServiceResponse>().FirstOrDefault();
                if (tacticalSettings != null)
                {
                    var notificationSettings =
                        (WebsocketNotificationProtocolConfiguration)tacticalSettings.Properties.NotificationProtocols
                            .First();
                    _settings.OutboundNotificationsUrl = notificationSettings.Properties.Endpoints.First();

                    var telemetrySettings =
                        (UdpTelemetryProtocolConfiguration)tacticalSettings.Properties.TelemetryProtocols.First();
                    _settings.CurrentTelemetryId = telemetrySettings.Id;
                    _settings.EncryptionKey = telemetrySettings.Properties.EncryptionKey;

                    var telemetryEndPoint = telemetrySettings.Properties.Endpoints.First();
                    _settings.TelemetryHostName = telemetryEndPoint.Split(':')[0];
                    _settings.TelemetryPortNumber = int.Parse(telemetryEndPoint.Split(':')[1]);
                    _settings.TransmissionRateInMilliseconds =
                        telemetrySettings.Properties.TransmissionRateInMilliseconds;

                    await _notificationsService.StartWebSocket(cancellationToken);
                }

                await _messagesService.AddMessageAsync(Message.ForInfo("Flight", $"Flight {startFlightResponse.Id} underway.", TimeSpan.FromSeconds(10)));
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(Message.ForError("Flight", "Flight create failed.", ex));
            }
        }

        private async Task CompleteFlight(CancellationToken cancellationToken)
        {
            if (!(_settings.UseFlightPlans && _settings.UseFlights && _settings.TokenResponse.HasScopes(Scopes.ManageFlightReports)))
            {
                return;
            }
            if (string.IsNullOrEmpty(_settings.CurrentFlightId) || string.IsNullOrEmpty(_settings.CurrentFlightPlanId))
            {
                return;
            }
            try
            {
                try
                {
                    await _flightClient.CompleteFlight(_settings.CurrentFlightId, cancellationToken);
                    await _messagesService.AddMessageAsync(Message.ForInfo("FlightComplete", $"Flight {_settings.CurrentFlightId} completed.", TimeSpan.FromSeconds(10)));
                }
                catch (Exception ex)
                {
                    await _messagesService.AddMessageAsync(Message.ForError("FlightComplete", $"Failed to complete flight {_settings.CurrentFlightId}.", ex));
                }

                try
                {
                    await _flightClient.CompleteFlightPlan(_settings.CurrentFlightPlanId, cancellationToken);
                    await _messagesService.AddMessageAsync(Message.ForInfo("FlightPlanComplete", $"Flight plan {_settings.CurrentFlightPlanId} completed.", TimeSpan.FromSeconds(10)));
                }
                catch (Exception ex)
                {
                    await _messagesService.AddMessageAsync(Message.ForError("FlightPlanComplete", $"Failed to complete flight plan {_settings.CurrentFlightPlanId}.", ex));
                }
            }
            finally
            {
                await _notificationsService.StopWebSocket(cancellationToken);
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

        private CreateFlightPlanRequest ConstructFlightPlanRequest(FlightPlan flightPlan, UserProfileInfo profile)
        {
            var parts = CreateFlightPartsFromWaypoints(flightPlan.Waypoints, flightPlan.Duration);
            var sParts = parts.Select(p => new CreateFlightPartRequest
            {
                Id = p.Id,
                MaxAltitude = p.MaxAltitude,
                Start = p.Start,
                End = p.End,
                TimeZone = p.TimeZone,
                Geography = p.Geography
            }).ToList();

            var flightPlanRequest = new CreateFlightPlanRequest
            {
                Summary = flightPlan.Summary,
                Description = flightPlan.Description,
                Parts = sParts,
                PointOfContact = new ContactDetails
                {
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    PhoneNumber = _settings.FlightPhoneNumber,
                    AllowSmsContact = _settings.FlightAllowSms
                },
                DroneDetails = new CreateFlightPlanRequestDroneDetails
                {
                    AirFrame = MapToAirFrame(flightPlan.FlightCapability),
                    MaxWeight = 1
                }
            };
            if (_settings.FlightIdentifierIcao)
            {
                flightPlanRequest.IcaoAddress = _settings.FlightIdentifierIcaoAddress;
            }
            if (_settings.FlightIdentifierSerial)
            {
                flightPlanRequest.DroneSerialNumber = _settings.FlightIdentifierSerialNumber;
            }
            return flightPlanRequest;
        }

                private static AirFrameType MapToAirFrame(FlightCapability flightCapability)
        {
            switch (flightCapability)
            {
                case FlightCapability.FixedWing:
                    return AirFrameType.FixedWing;
                case FlightCapability.Rotary:
                    return AirFrameType.Rotary;
                case FlightCapability.VTOL:
                    return AirFrameType.VTOL;
                case FlightCapability.Tethered:
                    return AirFrameType.Tethered;
                case FlightCapability.Unspecified:
                default:
                    throw new ArgumentOutOfRangeException(nameof(flightCapability), flightCapability, "Failed to map FlightCapability to AirFrameType.");
            }
        }

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
