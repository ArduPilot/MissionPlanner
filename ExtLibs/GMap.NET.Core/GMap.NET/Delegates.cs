
namespace GMap.NET
{
   public delegate void CurrentPositionChanged(PointLatLng point);
   internal delegate void NeedInvalidation();

   public delegate void TileLoadComplete(long ElapsedMilliseconds);
   public delegate void TileLoadStart();
  
   public delegate void MapDrag();
   public delegate void MapZoomChanged();
   public delegate void MapTypeChanged(MapType type);

   public delegate void EmptyTileError(int zoom, GPoint pos);
}
