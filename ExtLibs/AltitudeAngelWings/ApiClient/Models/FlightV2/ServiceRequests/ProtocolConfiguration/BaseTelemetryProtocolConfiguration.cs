using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration
{
    [JsonConverter(typeof(BaseTelemetryProtocolConfigurationConverter))]
    public abstract class BaseTelemetryProtocolConfiguration
    {
        protected BaseTelemetryProtocolConfiguration(
            TelemetryProtocolType type)
        {
            Type = type;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public TelemetryProtocolType Type {
            get;
        }

        [JsonIgnore]
        public abstract string Description {
            get;
        }

        /// <summary>
        /// Gets properties of the <see cref="BaseTelemetryProtocolConfiguration"/> that are set by the user/flight creator.
        /// </summary>
        /// <returns>An object containing the user-set properties.</returns>
        [JsonIgnore]
        public abstract object UserProperties {
            get;
        }

        /// <summary>
        /// Gets the hash code of the properties of the <see cref="BaseTelemetryProtocolConfiguration"/> that are set by the user/flight creator.
        /// </summary>
        [JsonIgnore]
        public abstract int UserPropertiesHash {
            get;
        }

        public virtual bool IsEquivalentTo(BaseTelemetryProtocolConfiguration configuration) => this.Type == configuration.Type;
    }

    public class BaseTelemetryProtocolConfigurationConverter : JsonCreationConverter<BaseTelemetryProtocolConfiguration>
    {
        protected override BaseTelemetryProtocolConfiguration Create(Type objectType, JObject jObject)
        {
            string typeString = jObject.GetStringIgnoreCase("type");

            if (typeString != null && Enum.TryParse(typeString, true, out TelemetryProtocolType type))
            {
                switch (type)
                {
                    case TelemetryProtocolType.Udp:
                        return new UdpTelemetryProtocolConfiguration();
                }
            }

            return null;
        }
    }

    public static class JObjectExtensions
    {
        public static string GetStringIgnoreCase(this JObject jObject, string propertyName)
        {
            return jObject.GetJTokenIgnoreCase(propertyName)?.Value<string>();
        }

        public static JToken GetJTokenIgnoreCase(this JObject jObject, string propertyName)
        {
            return jObject.GetValue(propertyName, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public enum TelemetryProtocolType
    {
        Udp,
        Adsb
    }
}
