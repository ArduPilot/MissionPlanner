namespace GMap.NET.MapProviders
{
    using System;
    using GMap.NET.Projections;
    using System.Globalization;

    /// <summary>
    /// JapanLakeMap provider,https://cyberjapandata.gsi.go.jp/
    /// </summary>
    public class JapanLakeMapProvider : GMapProvider
    {
      public static readonly JapanLakeMapProvider Instance;

      JapanLakeMapProvider()
      {
            MaxZoom = 22;
            MinZoom = 2;
            Copyright = string.Format("©Geospatial Information Authority of Japan.", DateTime.Today.Year);
      }

      static JapanLakeMapProvider()
      {
         Instance = new JapanLakeMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("8b8accbd-9529-4814-9eda-18c0a588bcb5");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "JapanLakeMap";
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
            if (zoom < 16 || zoom > 17)
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
      static readonly string UrlFormat = "https://cyberjapandata.gsi.go.jp/xyz/lake1/{0}/{1}/{2}.png";

    }
}