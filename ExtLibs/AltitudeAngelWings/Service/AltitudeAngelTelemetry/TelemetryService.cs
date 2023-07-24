using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.Messaging;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients;
using AltitudeAngelWings.Clients.Auth.Model;
using AltitudeAngelWings.Clients.Telemetry;
using AltitudeAngelWings.Model;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry
{
    public class TelemetryService : ITelemetryService
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

            _disposer.Add(flightDataService.ArmedFlightData
                .SubscribeWithAsync((i, ct) => SendTelemetry(i)));

            _disposer.Add(flightDataService.FlightDisarmed
                .SubscribeWithAsync((i, ct) => FlightDisarmed(i)));
        }

        private Task FlightDisarmed(Model.FlightData flightData)
        {
            _sequenceNumber = 0;
            return Task.CompletedTask;
        }

        private async Task SendTelemetry(Model.FlightData flightData)
        {
            if (!(_settings.UseFlightPlans && _settings.UseFlights && _settings.SendFlightTelemetry > FlightTelemetry.None && _settings.TokenResponse.HasScopes(Scopes.TacticalCrs)))
            {
                return;
            }

            if (_settings.CurrentFlightId == null)
            {
                await _messagesService.AddMessageAsync(Message.ForInfo("Telemetry", "Not sending telemetry as no current flight ID is set."));
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
                await _messagesService.AddMessageAsync(Message.ForInfo("Telemetry", message));

                _client.SendTelemetry(telemetry, _settings.TelemetryHostName, _settings.TelemetryPortNumber, _settings.EncryptionKey);
                _sequenceNumber += 1;
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(Message.ForError("Sending telemetry failed.", ex));
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
