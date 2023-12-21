using System;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.Services;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;

namespace AltitudeAngelWings.Clients.Telemetry
{
    public class TelemetryClient: ITelemetryClient
    {
        private IAutpService _autpService;

        public TelemetryClient(IAutpService autpService)
        {
            _autpService = autpService;
        }

        public void SendTelemetry(ITelemetryEvent dataStructure, string portName, int portNumber, string encryptionKey)
        {
            byte[] bytes = _autpService.WriteTelemetry(dataStructure, Convert.FromBase64String(encryptionKey));
            IDatagramSender datagramSender = new DatagramSender(portName, portNumber);
            datagramSender.SendToServer(bytes);
        }
    }
}
