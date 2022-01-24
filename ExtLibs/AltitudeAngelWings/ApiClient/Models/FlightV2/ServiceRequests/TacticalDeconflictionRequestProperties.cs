using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests
{
    public class TacticalDeconflictionRequestProperties
    {
        /// <summary>
        /// Conflict resolution scope. Specify local for organisation drones only, or global for all drones
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Enable or disable surveillance-based conflict resolution.
        /// If enabled any aircraft picked up via surveillance sources will be included when providing guidance to your aircraft.
        /// Defaults to true if <see cref="Scope"/> is <see cref="ConflictResolutionScope.Global"/>,
        /// false if it is <see cref="ConflictResolutionScope.Local"/>
        /// </summary>
        public bool? SurveillanceResolution { get; set; }

        /// <summary>
        /// Allows the caller to request specific types of guidance based on the capability of the aircraft.
        /// </summary>
        public List<string> Guidance { get; set; } = new List<string>();

        /// <summary>
        /// The Telemetry protocols to be used for this flight
        /// </summary>
        public List<object> TelemetryProtocols { get; set; }

        /// <summary>
        /// The Notification protocols to be used for this flight
        /// </summary>
        public List<object> NotificationProtocols { get; set; }
    }
}