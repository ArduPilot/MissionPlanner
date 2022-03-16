using System;

namespace GMap.NET.MapProviders
{
    /// <summary>
    ///     CzechTuristMap provider, http://www.mapy.cz/
    /// </summary>
    public class CzechTuristMapProvider : CzechMapProviderBase
    {
        public static readonly CzechTuristMapProvider Instance;

        CzechTuristMapProvider()
        {
        }

        static CzechTuristMapProvider()
        {
            Instance = new CzechTuristMapProvider();
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get;
        } = new Guid("102A54BE-3894-439B-9C1F-CA6FF2EA1FE9");

        public override string Name
        {
            get;
        } = "CzechTuristMap";

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom, LanguageStr);

            return GetTileImageUsingHttp(url);
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            // http://m3.mapserver.mapy.cz/wtourist-m/14-8802-5528

            return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, pos.X, pos.Y);
        }

        static readonly string UrlFormat = "https://mapserver.mapy.cz/turist-m/{1}-{2}-{3}";
    }
}
