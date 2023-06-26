using System;

namespace AltitudeAngelWings.ApiClient.Models
{
    public static class BoundingLatLongExtensions
    {
        public static BoundingLatLong RoundExpand(this BoundingLatLong bound, int digits)
        {
            bound.NorthEast.Latitude = Math.Round(bound.NorthEast.Latitude, digits, MidpointRounding.AwayFromZero);
            bound.NorthEast.Longitude = Math.Round(bound.NorthEast.Longitude, digits, MidpointRounding.AwayFromZero);
            bound.SouthWest.Latitude = Math.Round(bound.SouthWest.Latitude, digits, MidpointRounding.AwayFromZero);
            bound.SouthWest.Longitude = Math.Round(bound.SouthWest.Longitude, digits, MidpointRounding.AwayFromZero);
            return bound;
        }
    }
}