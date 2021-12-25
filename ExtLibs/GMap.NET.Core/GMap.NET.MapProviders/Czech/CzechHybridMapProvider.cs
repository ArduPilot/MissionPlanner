using System;

namespace GMap.NET.MapProviders
{
    /// <summary>
    ///     CzechHybridMap provider, http://www.mapy.cz/
    /// </summary>
    public class CzechHybridMapProvider : CzechMapProviderBase
    {
        public static readonly CzechHybridMapProvider Instance;

        CzechHybridMapProvider()
        {
            MaxZoom = 20;
        }

        static CzechHybridMapProvider()
        {
            Instance = new CzechHybridMapProvider();
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get;
        } = new Guid("7540CE5B-F634-41E9-B23E-A6E0A97526FD");

        public override string Name
        {
            get;
        } = "CzechHybridMap";

        GMapProvider[] _overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (_overlays == null)
                {
                    _overlays = new GMapProvider[] {CzechSatelliteMapProvider.Instance, this};
                }

                return _overlays;
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
            // http://m3.mapserver.mapy.cz/hybrid-m/14-8802-5528

            return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, pos.X, pos.Y);
        }

        static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/hybrid-m/{1}-{2}-{3}";
    }
}
