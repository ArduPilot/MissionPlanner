using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Maps
{
    using System;
    using GMap.NET.Projections;
    using System.Globalization;
    using GMap.NET.MapProviders;
    using GMap.NET;
    using System.Reflection;
    using System.Xml;
    using System.IO;
    using BruTile.Wmts;
    using BruTile.Web;

    /// <summary>
    /// WMTS Custom
    /// </summary>
    public class WMTSProvider
        : GMapProvider
    {
        public static readonly WMTSProvider Instance;

        WMTSProvider()
        {
            MaxZoom = 24;
        }

        static WMTSProvider()
        {
            Instance = new WMTSProvider();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("e55c464a-d87f-4538-a899-165eed86ac46");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "WMTS Custom";

        public override string Name
        {
            get { return name; }
        }

        GMapProvider[] overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider[] {this};
                }
                return overlays;
            }
        }

        public override PureProjection Projection
        {
            get { return MercatorProjection.Instance; }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom, LanguageStr);

            return GetTileImageUsingHttp(url);
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            var set = tilesource.First(a=>a.Name == LayerName);

            var ret = set.GetUri(new BruTile.TileInfo() { Index = new BruTile.TileIndex((int)pos.X,(int)pos.Y,zoom)});

            return ret.AbsoluteUri;
        }

        public static string LayerName = "";

        private static IEnumerable<HttpTileSource> tilesource;
        private static string _url;

        public static string[] Layers { get
            {
                return tilesource.Select(a=>a.Name).ToArray();
            } }

        public static string CustomWMTSURL {
            get{ return _url; }
            set{
                //1.0.0/WMTSCapabilities.xml
                _url = value;
                var content = Instance.GetContentUsingHttp(value);

                tilesource = BruTile.Wmts.WmtsParser.Parse(new MemoryStream(content.Select(a=>(byte)a).ToArray()));
                
                } }
        //https://maps.wien.gv.at/basemap/1.0.0/WMTSCapabilities.xml
    }
}