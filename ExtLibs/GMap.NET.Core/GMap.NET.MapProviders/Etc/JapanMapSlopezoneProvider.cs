namespace GMap.NET.MapProviders
{
    using System;
    using GMap.NET.Projections;
    using System.Globalization;

    /// <summary>
    /// JapanMapSlopezone provider,https://cyberjapandata.gsi.go.jp/
    /// </summary>
    public class JapanMapSlopezoneProvider : GMapProvider
    {
      public static readonly JapanMapSlopezoneProvider Instance;

      JapanMapSlopezoneProvider()
      {
            MaxZoom = 22;
            MinZoom = 2;
            Copyright = string.Format("©Geospatial Information Authority of Japan.", DateTime.Today.Year);
      }

      static JapanMapSlopezoneProvider()
      {
         Instance = new JapanMapSlopezoneProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("ebe37fbd-96e8-4aeb-879c-b0e4a0315147");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "JapanMapSlopezone";
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
      string url;
            if (zoom < 3 || zoom > 15)
            {
                url = string.Format(UrlLandFormat, zoom, pos.X, pos.Y);
            }
            else
            {
                url = string.Format(UrlFormat, zoom, pos.X, pos.Y);
            }
            return url;
      }
      static readonly string UrlLandFormat = "https://cyberjapandata.gsi.go.jp/xyz/std/{0}/{1}/{2}.png";
      static readonly string UrlFormat = "https://cyberjapandata.gsi.go.jp/xyz/slopezone1map/{0}/{1}/{2}.png";
    }
}
