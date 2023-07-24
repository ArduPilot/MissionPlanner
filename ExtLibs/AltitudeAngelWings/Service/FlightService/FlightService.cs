using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.ApiClient.Client.FlightClient;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration;
using AltitudeAngelWings.ApiClient.Models.Strategic;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.Messaging;
using AltitudeAngelWings.Service.OutboundNotifications;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using NodaTime;

namespace AltitudeAngelWings.Service.FlightService
{
    public class FlightService : IFlightService
    {
        private readonly IMessagesService _messagesService;
        private readonly IMissionPlannerState _missionPlannerState;
        private readonly CompositeDisposable _disposer = new CompositeDisposable();
        private readonly IFlightClient _flightClient;
        private readonly ISettings _settings;
        private readonly IAltitudeAngelClient _client;
        private readonly IOutboundNotificationsService _notificationsService;

        public FlightService(
            IMessagesService messagesService,
            IMissionPlannerState missionPlannerState,
            ISettings settings,
            IFlightDataService flightDataService,
            IAltitudeAngelClient client,
            IFlightClient flightClient,
            IOutboundNotificationsService notificationsService)
        {
            _messagesService = messagesService;
            _missionPlannerState = missionPlannerState;
            _settings = settings;
            _client = client;
            _flightClient = flightClient;
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
            try
            {
                Guid? flightPlanId;
                if (_settings.UseExistingFlightPlanId)
                {
                    flightPlanId = _settings.ExistingFlightPlanId;
                }
                else
                {
                    await _messagesService.AddMessageAsync(Message.ForInfo("Creating flight plan."));
                    var profile = await _client.GetUserProfile(cancellationToken);
                    var createPlanResponse = await _flightClient.CreateFlightPlan(flightPlan, profile, cancellationToken);
                    if (createPlanResponse.Outcome == StrategicSeverity.DirectConflict)
                    {
                        await _messagesService.AddMessageAsync(Message.ForInfo("Conflict detected; flight cancelled.", TimeSpan.FromSeconds(10)));
                        //await _missionPlanner.Disarm();
                        return;
                    }

                    flightPlanId = createPlanResponse.FlightPlanId;
                }

                // Check flight plan id is valid
                if (flightPlanId == null || flightPlanId == Guid.Empty)
                {
                    await _messagesService.AddMessageAsync(Message.ForInfo("Flight plan not available; flight cancelled.", TimeSpan.FromSeconds(10)));
                    //await _missionPlanner.Disarm();
                    return;
                }
                _settings.CurrentFlightPlanId = flightPlanId.ToString();
                await _messagesService.AddMessageAsync(Message.ForInfo($"Flight plan {flightPlanId} in use.", TimeSpan.FromSeconds(10)));

                // Flight being rejected will throw, and cause a disarm
                await _messagesService.AddMessageAsync(Message.ForInfo("Starting flight.", TimeSpan.FromSeconds(10)));
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

                await _messagesService.AddMessageAsync(Message.ForInfo($"Flight {startFlightResponse.Id} approved and underway.", TimeSpan.FromSeconds(10)));
                await task;
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(Message.ForError("Flight create failed.", ex));
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
                await _messagesService.AddMessageAsync(Message.ForInfo($"Flight {_settings.CurrentFlightId} completed.", TimeSpan.FromSeconds(10)));

                //await _flightClient.CompleteFlightPlan(_settings.CurrentFlightPlanId, cancellationToken);
                await _messagesService.AddMessageAsync(Message.ForInfo($"Flight plan {_settings.CurrentFlightPlanId} completed.", TimeSpan.FromSeconds(10)));
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(Message.ForError($"ERROR: Failed to complete flight {_settings.CurrentFlightId} and plan {_settings.CurrentFlightPlanId}.", ex));
            }
            finally
            {
                await _notificationsService.StopWebSocket();
                _settings.CurrentFlightId = null;
                _settings.CurrentFlightPlanId = null;
            }
        }

        private Task StartSurveillanceFlight(FlightPlan flightPlan, CancellationToken cancellationToken)
        {
            // TODO : Implement 8962   
            return Task.Run(() => new NotImplementedException("Please implement the 8962"), cancellationToken);
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
            var envelope = GeometryFactory.Default.CreateMultiPoint(
                    waypoints
                        .Select(l => new Point(l.Longitude, l.Latitude))
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
    }
}
