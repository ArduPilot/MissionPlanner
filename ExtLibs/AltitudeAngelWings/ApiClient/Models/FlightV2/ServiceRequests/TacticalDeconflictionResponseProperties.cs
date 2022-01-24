using System.Collections.Generic;
using AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests
{
    public class TacticalDeconflictionResponseProperties
    {
        /// <summary>
        /// Conflict resolution scope under which this flight will be operating
        /// </summary>
        public string Scope {
            get;
            set;
        }

        /// <summary>
        /// Whether or not surveillance-based conflict resolution is enabled
        /// </summary>
        public bool SurveillanceResolution {
            get;
            set;
        }

        /// <summary>
        /// The types of guidance which will be issued to the aircraft
        /// </summary>
        public List<string> Guidance {
            get;
            set;
        }

        /// <summary>
        /// The configuration for the requested telemetry protocols to be used for this flight
        /// </summary>
        public List<BaseTelemetryProtocolConfiguration> TelemetryProtocols {
            get; set;
        }

        /// <summary>
        /// The configuration for the requested notification protocols to be used for this flight
        /// </summary>
        public List<BaseNotificationProtocolConfiguration> NotificationProtocols {
            get; set;
        }

    }
}
