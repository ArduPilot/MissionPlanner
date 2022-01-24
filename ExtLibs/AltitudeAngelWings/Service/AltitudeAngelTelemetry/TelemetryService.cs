using AltitudeAngelWings.ApiClient.Client.TelemetryClient;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.Messaging;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry
{
    public class TelemetryService: ITelemetryService
    {
        private readonly IMessagesService _messagesService;
        private readonly CompositeDisposable _disposer = new CompositeDisposable();
        private readonly ITelemetryClient _client;
        private readonly ISettings _settings;

        private int _sequenceNumber;

        public TelemetryService(
            IMessagesService messagesService,
            ISettings settings,
            IFlightDataService flightDataService,
            ITelemetryClient client)
        {
            _messagesService = messagesService;
            _settings = settings;
            _client = client;

            if (_settings.DisableTelemetrySending) return;
            _disposer.Add(flightDataService.ArmedFlightData
                .SubscribeWithAsync((i, ct) => SendTelemetry(i)));

            _disposer.Add(flightDataService.FlightDisarmed
                .SubscribeWithAsync((i, ct) => FlightDisarmed(i)));
        }

        private Task FlightDisarmed(Models.FlightData flightData)
        {
            _sequenceNumber = 0;
            return Task.CompletedTask;
        }

        private async Task SendTelemetry(Models.FlightData flightData)
        {
            if (_settings.CurrentFlightId == null)
            {
                await _messagesService.AddMessageAsync(
                    new Message("Not sending telemetry as no current flight ID is set."));
                return;
            }
            try
            {
                var lat = (float)flightData.CurrentPosition.Latitude;
                var lon = (float)flightData.CurrentPosition.Longitude;
                var alt = (ushort)flightData.CurrentPosition.Altitude;
                var courseInRadians = flightData.CurrentPosition.Course / 180 * Math.PI;
                var xVel = (float)(Math.Sin(courseInRadians) * flightData.CurrentPosition.Speed);
                var yVel = (float)(Math.Cos(courseInRadians) * flightData.CurrentPosition.Speed);
                var zVel = flightData.CurrentPosition.VerticalSpeed;


                // Create dataStructure
                var udpMessage = new UavPositionReport
                {
                    GpsTimestamp = DateTime.UtcNow,
                    Pos = new Position(lat, lon, 0),
                    Alt = new Altitude(alt, AltitudeDatum.Agl, 1),
                    Velocity = new Velocity(xVel, yVel, zVel, 1),
                    IsAirborne = AirborneStatus.Airborne,
                    SatellitesVisible = 3
                };

                var telemetryId = Guid.Parse(_settings.CurrentTelemetryId);
                var telemetry = new TelemetryEvent<UavPositionReport>(telemetryId, udpMessage) { SequenceNumber = _sequenceNumber };

                var message = string.Format($"Sending Telemetry {flightData.CurrentPosition.Latitude}, {flightData.CurrentPosition.Longitude}, {flightData.CurrentPosition.Altitude}");
                await _messagesService.AddMessageAsync(new Message(message));

                _client.SendTelemetry(telemetry, _settings.TelemetryHostName, _settings.TelemetryPortNumber, _settings.EncryptionKey);
                _sequenceNumber += 1;
            }
            catch (Exception)
            {
                await _messagesService.AddMessageAsync(new Message("ERROR: Sending telemetry failed."));
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
    }
}
