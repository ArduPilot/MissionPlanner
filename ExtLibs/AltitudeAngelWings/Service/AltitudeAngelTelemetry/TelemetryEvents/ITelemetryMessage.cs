using Newtonsoft.Json;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// Telemetry information
    /// </summary>
    public interface ITelemetryMessage
    {
        /// <summary>
        /// Message version
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Message type
        /// </summary>
        [JsonIgnore]
        string MessageType { get; }
    }
}
