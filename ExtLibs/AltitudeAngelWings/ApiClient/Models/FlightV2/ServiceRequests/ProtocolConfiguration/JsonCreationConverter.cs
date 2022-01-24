using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.ApiClient.Models.FlightV2.ServiceRequests.ProtocolConfiguration
{
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => typeof(T).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            // Load JObject from stream.
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject.
            T target = this.Create(objectType, jObject);

            // Populate the object properties.
            if (target != null)
            {
                serializer.Populate(jObject.CreateReader(), target);
            }

            return target;
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer) => throw new NotImplementedException();

        protected abstract T Create(Type objectType, JObject jObject);
    }
}
