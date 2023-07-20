using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration;
using AltitudeAngelWings.ApiClient.Models.Strategic;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.Messaging;
using AltitudeAngelWings.Service.OutboundNotifs;

namespace AltitudeAngelWings.Service.FlightService
{
    public class FlightService : IFlightService
    {
        private readonly IMessagesService _messagesService;
        private readonly IMissionPlanner _missionPlanner;
        private readonly CompositeDisposable _disposer = new CompositeDisposable();
        private readonly IAltitudeAngelClient _client;
        private readonly ISettings _settings;
        private readonly IOutboundNotifsService _notificationsService;

        public FlightService(
            IMessagesService messagesService,
            IMissionPlanner missionPlanner,
            ISettings settings,
            IFlightDataService flightDataService,
            IAltitudeAngelClient client,
            IOutboundNotifsService notificationsService)
        {
            _messagesService = messagesService;
            _missionPlanner = missionPlanner;
            _settings = settings;
            _client = client;
            _notificationsService = notificationsService;
            _settings.CurrentFlightReportId = null;
            _settings.CurrentFlightId = null;

            if (_settings.UseFlightPlans && _settings.UseFlights)
            {
                _disposer.Add(flightDataService.FlightArmed
                    .SubscribeWithAsync(async (i, ct) => await StartTelemetryFlight(_missionPlanner.GetFlightPlan(), ct)));
                _disposer.Add(flightDataService.FlightDisarmed
                    .SubscribeWithAsync((i, ct) => CompleteFlight(ct)));
            }
        }

        private async Task StartTelemetryFlight(FlightPlan flightPlan, CancellationToken cancellationToken)
        {
            if (flightPlan == null)
            {
                return;
            }
            if (_settings.CurrentFlightId != null)
            {
                // Complete flight if starting one before the previous ends.
                await CompleteFlight(cancellationToken);
            }

            // TODO somehow prevent Arming of UAV until the following try statement has been completed, so telemetry isnt sent late. PBI 8490
            try
            {
                Guid? flightPlanId;
                if (_settings.UseExistingFlightPlanId)
                {
                    flightPlanId = _settings.ExistingFlightPlanId;
                }
                else
                {
                    await _messagesService.AddMessageAsync(new Message("Creating flight plan...")
                        { TimeToLive = TimeSpan.FromSeconds(10) });
                    var profile = await _client.GetUserProfile(cancellationToken);
                    var createPlanResponse = await _client.CreateFlightPlan(flightPlan, profile);
                    if (createPlanResponse.Outcome == StrategicSeverity.DirectConflict)
                    {
                        await _messagesService.AddMessageAsync(new Message("Conflict detected; flight cancelled.")
                            { TimeToLive = TimeSpan.FromSeconds(10) });
                        await _missionPlanner.Disarm();
                        return;
                    }

                    flightPlanId = createPlanResponse.FlightPlanId;
                }

                // Check flight plan id is valid
                if (flightPlanId == null || flightPlanId == Guid.Empty)
                {
                    await _messagesService.AddMessageAsync(new Message("Flight plan not available; flight cancelled.")
                        { TimeToLive = TimeSpan.FromSeconds(10) });
                    await _missionPlanner.Disarm();
                    return;
                }
                _settings.CurrentFlightReportId = flightPlanId.ToString();
                await _messagesService.AddMessageAsync(new Message($"Flight plan {flightPlanId} in use.")
                    { TimeToLive = TimeSpan.FromSeconds(10) });

                // Flight being rejected will throw, and cause a disarm
                await _messagesService.AddMessageAsync(new Message("Starting flight...")
                    { TimeToLive = TimeSpan.FromSeconds(10) });
                var startFlightResponse = await _client.StartFlight(flightPlanId.Value.ToString("D"));

                _settings.CurrentFlightId = startFlightResponse.Id;
                var tacticalSettings = startFlightResponse.ServiceResponses.First();

                var notificationSettings = (WebsocketNotificationProtocolConfiguration)tacticalSettings.Properties.NotificationProtocols.First();
                _settings.OutboundNotifsEndpointUrl = notificationSettings.Properties.Endpoints.First();

                var telemetrySettings = (UdpTelemetryProtocolConfiguration)tacticalSettings.Properties.TelemetryProtocols.First();
                _settings.CurrentTelemetryId = telemetrySettings.Id;
                _settings.EncryptionKey = telemetrySettings.Properties.EncryptionKey;

                var telemetryEndPoint = telemetrySettings.Properties.Endpoints.First();
                _settings.TelemetryHostName = telemetryEndPoint.Split(':')[0];
                _settings.TelemetryPortNumber = int.Parse(telemetryEndPoint.Split(':')[1]);
                _settings.TransmissionRateInMilliseconds = telemetrySettings.Properties.TransmissionRateInMilliseconds;

                var task = _notificationsService.StartWebSocket();

                await _messagesService.AddMessageAsync(new Message($"Flight {startFlightResponse.Id} approved and underway.")
                    { TimeToLive = TimeSpan.FromSeconds(10) });

                await task;
            }
            catch (Exception)
            {
                await _messagesService.AddMessageAsync(new Message("Flight create failed."));
            }
        }

        private async Task CompleteFlight(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_settings.CurrentFlightId) || string.IsNullOrEmpty(_settings.CurrentFlightReportId))
            {
                return;
            }
            try
            {
                await _client.CompleteFlight(_settings.CurrentFlightId);
                await _messagesService.AddMessageAsync(new Message($"Flight {_settings.CurrentFlightId} completed.")
                    { TimeToLive = TimeSpan.FromSeconds(10) });

                //await _client.CancelFlightPlan(_settings.CurrentFlightReportId);
                await _messagesService.AddMessageAsync(new Message($"Flight plan {_settings.CurrentFlightReportId} completed.")
                    { TimeToLive = TimeSpan.FromSeconds(10) });
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(new Message($"ERROR: Failed to complete flight {_settings.CurrentFlightId} and plan {_settings.CurrentFlightReportId}:, {ex}"));
            }
            finally
            {
                await _notificationsService.StopWebSocket();
                _settings.CurrentFlightId = null;
                _settings.CurrentFlightReportId = null;
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
    }
}
