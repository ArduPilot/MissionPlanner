using System.Collections.Generic;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class Contact
    {
        [JsonProperty("phoneNumbers")]
        public IList<PhoneNumber> PhoneNumbers { get; set; }
    }
}