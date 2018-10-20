namespace GMap.NET.MapProviders
{
    using System;
    using GMap.NET.Projections;
    using System.Globalization;

    /// <summary>
    /// JapanSeaMap provider, http://cyberjapandata.gsi.go.jp/
    /// </summary>
    public class JapanSeaMapProvider : GMapProvider
    {
      public static readonly JapanSeaMapProvider Instance;

      JapanSeaMapProvider()
      {
            MaxZoom = 22;
            MinZoom = 2;
            Copyright = string.Format("©Geospatial Information Authority of Japan.", DateTime.Today.Year);
      }

      static JapanSeaMapProvider()
      {
         Instance = new JapanSeaMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("5fd1b829-7f34-482b-8d50-ab06a993f743");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "JapanSeaMap";
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
            if (zoom < 14 || zoom > 16)
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
      static readonly string UrlFormat = "https://cyberjapandata.gsi.go.jp/xyz/ccm1/{0}/{1}/{2}.png";

    }
}