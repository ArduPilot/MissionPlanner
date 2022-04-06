using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    /// A telemetry event
    /// </summary>
    public interface ITelemetryEvent
    {
        /// <summary>
        /// Event structure version
        /// </summary>
        int Version { get; }


        /// <summary>
        /// Timestamp event was received in the server
        /// </summary>
        DateTime ReceivedTimestamp
        {
            get;
            set;
        }
    }
}
