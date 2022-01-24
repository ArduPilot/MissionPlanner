using System.Collections.Generic;
using System.Linq;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration
{
    public class WebsocketNotificationProtocolConfiguration : BaseNotificationProtocolConfiguration
    {
        public WebsocketNotificationProtocolConfiguration() : base(NotificationProtocolType.Websocket) { }

        public WebsocketNotificationProtocolConfigurationProperties Properties { get; set; }

        public override object UserProperties => null;
        public override int UserPropertiesHash => 0;

        public override string Description => $"Websocket with these endpoints {string.Join(",", this.Properties?.Endpoints ?? Enumerable.Empty<string>())}";
    }

    public class WebsocketNotificationProtocolConfigurationProperties
    {
        public IEnumerable<string> Endpoints { get; set; }
    }
}
