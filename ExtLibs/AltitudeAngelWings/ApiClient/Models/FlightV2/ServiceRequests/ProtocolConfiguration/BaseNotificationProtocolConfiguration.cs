using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration
{
    [JsonConverter(typeof(BaseNotificationProtocolConfigurationConverter))]
    public abstract class BaseNotificationProtocolConfiguration
    {
        protected BaseNotificationProtocolConfiguration(
            NotificationProtocolType type)
        {
            this.Type = type;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public NotificationProtocolType Type { get; }

        [JsonIgnore]
        public abstract string Description { get; }

        public virtual bool IsEquivalentTo(BaseNotificationProtocolConfiguration configuration) => this.Type == configuration.Type;

        /// <summary>
        /// Gets properties of the <see cref="BaseNotificationProtocolConfiguration"/> that are set by the user/flight creator.
        /// </summary>
        /// <returns>An object containing the user-set properties.</returns>
        [JsonIgnore]
        public abstract object UserProperties {
            get;
        }

        /// <summary>
        /// Gets the hash code of properties of the <see cref="BaseNotificationProtocolConfiguration"/> that are set by the user/flight creator.
        /// </summary>
        [JsonIgnore]
        public abstract int UserPropertiesHash {
            get;
        }
    }

    public class BaseNotificationProtocolConfigurationConverter : JsonCreationConverter<BaseNotificationProtocolConfiguration>
    {
        protected override BaseNotificationProtocolConfiguration Create(Type objectType, JObject jObject)
        {
            string typeString = jObject.GetStringIgnoreCase("type");

            if (typeString != null && Enum.TryParse(typeString, true, out NotificationProtocolType type))
            {
                switch (type)
                {
                    case NotificationProtocolType.Websocket:
                        return new WebsocketNotificationProtocolConfiguration();
                }
            }

            return null;
        }
    }

    public enum NotificationProtocolType
    {
        Webhook = 1,
        Websocket = 2,
        Push = 3,
        Sms = 4
    }
}
