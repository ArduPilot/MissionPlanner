using System.Collections.Generic;
using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class Contact
    {
        [JsonProperty("phoneNumbers")]
        public IList<PhoneNumber> PhoneNumbers { get; set; }
    }
}