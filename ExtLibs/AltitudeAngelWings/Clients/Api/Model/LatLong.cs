using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public class LatLong
    {
        public LatLong()
        {
        }

        public LatLong(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        [JsonProperty(PropertyName = "lat")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public double Longitude { get; set; }

    }
}
