using GMap.NET;
using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models
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

        public LatLong(PointLatLng latLong)
        {
            Latitude = latLong.Lat;
            Longitude = latLong.Lng;
        }

        [JsonProperty(PropertyName = "lat")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public double Longitude { get; set; }

    }
}
