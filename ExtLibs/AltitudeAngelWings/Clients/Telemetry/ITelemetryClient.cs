using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;

namespace AltitudeAngelWings.Clients.Telemetry
{
    public interface ITelemetryClient
    {
        void SendTelemetry(ITelemetryEvent dataStructure, string portName, int portNumber, string encryptionKey);
    }
}
