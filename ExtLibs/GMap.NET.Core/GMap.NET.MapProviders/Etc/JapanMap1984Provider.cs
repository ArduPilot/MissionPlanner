namespace GMap.NET.MapProviders
{
    using System;
    using GMap.NET.Projections;
    using System.Globalization;

    /// <summary>
    /// JapanMap1984 provider,https://cyberjapandata.gsi.go.jp/
    /// </summary>
    public class JapanMap1984Provider : GMapProvider
    {
      public static readonly JapanMap1984Provider Instance;

      JapanMap1984Provider()
      {
            MaxZoom = 22;
            MinZoom = 2;
            Copyright = string.Format("©Geospatial Information Authority of Japan.", DateTime.Today.Year);
      }

      static JapanMap1984Provider()
      {
         Instance = new JapanMap1984Provider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("9c1f02b1-46fb-4dce-bc71-5afc5f8be69c");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "JapanMap1984";
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
            if (zoom < 10 || zoom > 17)
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
      static readonly string UrlFormat = "https://cyberjapandata.gsi.go.jp/xyz/gazo3/{0}/{1}/{2}.jpg";

    }
}