namespace GMap.NET.MapProviders
{
    using System;
    using GMap.NET.Projections;
    using System.Globalization;

    /// <summary>
    /// JapanMapRelief provider,https://cyberjapandata.gsi.go.jp/
    /// </summary>
    public class JapanMapReliefProvider : GMapProvider
    {
      public static readonly JapanMapReliefProvider Instance;

      JapanMapReliefProvider()
      {
            MaxZoom = 22;
            MinZoom = 2;
            Copyright = string.Format("©Geospatial Information Authority of Japan.", DateTime.Today.Year);
      }

      static JapanMapReliefProvider()
      {
         Instance = new JapanMapReliefProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("9dea1753-9819-4f4e-9111-dc59bd623ac0");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "JapanMapRelief";
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
            if (zoom < 5 || zoom > 15)
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
      static readonly string UrlFormat = "https://cyberjapandata.gsi.go.jp/xyz/relief/{0}/{1}/{2}.png";
    }
}