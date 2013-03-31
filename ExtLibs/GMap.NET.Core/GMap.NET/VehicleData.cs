
namespace GMap.NET
{
   public struct VehicleData
   {
      public int Id;
      public double Lat;
      public double Lng;
      public string Line;
      public string LastStop;
      public string TrackType;
      public string AreaName;
      public string StreetName;
      public string Time;
      public double? Bearing;
   }

   public enum TransportType
   {
      Bus,
      TrolleyBus,
   }
}
