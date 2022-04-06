using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public interface IAutpService
    {
        byte[] WriteTelemetry(ITelemetryEvent telemetryEvent, byte[] encryptionKey = null);
    }
}
