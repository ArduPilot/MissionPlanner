using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.Clients.OutboundNotifications.Model
{
    /// <summary>
    /// A notification message issued to an <see cref="AA.OutboundNotifications.Api.Services.INotificationClient"/>
    /// </summary>
    public class NotificationMessage
    {
        /// <summary>
        /// Notification Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Notification must be acknowledged
        /// </summary>
        public bool Acknowledge { get; set; }

        /// <summary>
        /// Type of notification
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Notification properties
        /// </summary>
        public JObject Properties { get; set; }

        /// <summary>
        /// Human-readable version of the message
        /// </summary>
        public string FriendlyText { get; set; }

        /// <summary>
        /// The ID of the flight this message targets.
        /// </summary>
        public string FlightId { get; set; }
    }
}
