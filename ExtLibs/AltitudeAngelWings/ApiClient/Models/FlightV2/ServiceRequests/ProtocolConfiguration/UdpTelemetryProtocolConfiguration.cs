using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration
{
    public class UdpTelemetryProtocolConfiguration : BaseTelemetryProtocolConfiguration
    {
        public UdpTelemetryProtocolConfiguration() : base(TelemetryProtocolType.Udp) { }

        public UdpTelemetryProtocolConfigurationProperties Properties { get; set; }

        public override string Description => "UDP";

        public override object UserProperties => null; // Currently does not have any user-set properties
        public override int UserPropertiesHash => 0;
    }

    public class UdpTelemetryProtocolConfigurationProperties
    {
        /// <summary>
        /// Gets a base 64 encoded encryption key.
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// Gets the collection of endpoints
        /// </summary>
        public IEnumerable<string> Endpoints { get; set; }

        /// <summary>
        /// Gets the rate of transmission in milliseconds.
        /// </summary>
        public int TransmissionRateInMilliseconds { get; set; }
    }
}
