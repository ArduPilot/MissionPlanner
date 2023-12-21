using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class ContactDetails
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("allowSmsContact")]
        public bool AllowSmsContact { get; set; }

        [JsonProperty("registrationIds", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> RegistrationIds { get; set; }

        [JsonProperty("additionalInfo", NullValueHandling = NullValueHandling.Ignore)]
        public JObject AdditionalInfo { get; set; }
    }
}