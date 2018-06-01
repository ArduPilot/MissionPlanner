
namespace GMap.NET.MapProviders
{
   using System;
   using GMap.NET.Projections;
    using System.Globalization;

   /// <summary>
   /// SpainMap provider, http://sigpac.mapa.es/fega/visor/
   /// </summary>
   public class SpainMapProvider : GMapProvider
   {
      public static readonly SpainMapProvider Instance;

      SpainMapProvider()
      {
         Copyright = string.Format("©{0} SIGPAC", DateTime.Today.Year);
         MinZoom = 1;
         Area = new RectLatLng(43.8741381814747, -9.700927734375, 14.34814453125, 7.8605775962932);
      }

      static SpainMapProvider()
      {
         Instance = new SpainMapProvider();
      }

      readonly string[] levels =
      {
         "0", "1", "2", "3", "4", 
         "MTNSIGPAC", 
         "MTN2000", "MTN2000", "MTN2000", "MTN2000", "MTN2000", 
         "MTN200", "MTN200", "MTN200", 
         "MTN25", "MTN25",
         "ORTOFOTOS","ORTOFOTOS","ORTOFOTOS","ORTOFOTOS"
      };

      #region GMapProvider Members

      readonly Guid id = new Guid("7B70ABB0-1265-4D34-9442-F0788F4F689F");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "SpainMap";
      public override string Name
      {
         get
         {
            return name;
         }
      }

      GMapProvider[] overlays;
      public override GMapProvider[] Overlays
      {
         get
         {
            if(overlays == null)
            {
               overlays = new GMapProvider[] { this };
            }
            return overlays;
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return MercatorProjection.Instance;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         string url = MakeTileImageUrl(pos, zoom, LanguageStr);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
          var px1 = Projection.FromTileXYToPixel(pos);
          var px2 = px1;

          px1.Offset(0, Projection.TileSize.Height);
          PointLatLng p1 = Projection.FromPixelToLatLng(px1, zoom);

          px2.Offset(Projection.TileSize.Width, 0);
          PointLatLng p2 = Projection.FromPixelToLatLng(px2, zoom);

          var ret = string.Format(CultureInfo.InvariantCulture, UrlFormat, p1.Lng, p1.Lat, p2.Lng, p2.Lat, Projection.TileSize.Width, Projection.TileSize.Height, levels[zoom]);

          return ret;
      }

      static readonly string UrlFormat = "http://wms.magrama.es/wms/wms.aspx?VERSION=1.1.0&REQUEST=GetMap&SERVICE=WMS&LAYERS={6}&styles=&bbox={0},{1},{2},{3}&width={4}&height={5}&srs=EPSG:4326&format=image/jpeg";
   }
}