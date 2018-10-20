namespace GMap.NET.MapProviders
{
    using System;
    using GMap.NET.Projections;
    using System.Globalization;

    /// <summary>
    /// JapanMap provider, http://cyberjapandata.gsi.go.jp/
    /// </summary>
    public class JapanMapProvider : GMapProvider
    {
      public static readonly JapanMapProvider Instance;

      JapanMapProvider()
      {
            MaxZoom = 22;
            MinZoom = 2;
            Copyright = string.Format("©Geospatial Information Authority of Japan.", DateTime.Today.Year);
      }

      static JapanMapProvider()
      {
         Instance = new JapanMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("f6b48af7-de2d-4c8c-91ba-606450484d77");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "JapanMap";
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
      string url = string.Format(UrlFormat, zoom, pos.X, pos.Y);
      return url;
      }

      static readonly string UrlFormat = "http://cyberjapandata.gsi.go.jp/xyz/std/{0}/{1}/{2}.png";
   }
}