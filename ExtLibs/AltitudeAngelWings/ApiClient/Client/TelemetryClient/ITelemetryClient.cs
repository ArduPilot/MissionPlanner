using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;

namespace AltitudeAngelWings.ApiClient.Client.TelemetryClient
{
    public interface ITelemetryClient
    {
        void SendTelemetry(ITelemetryEvent dataStructure, string portName, int portNumber, string encryptionKey);
    }
}
