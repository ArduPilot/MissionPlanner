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

    /// <summary>
    /// WMS Custom
    /// </summary>
    public class WMSProvider : GMapProvider
    {
        public static readonly WMSProvider Instance;

        WMSProvider()
        {
            MaxZoom = 22;
        }

        static WMSProvider()
        {
            Instance = new WMSProvider();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("4574218D-B552-4CAF-89AE-F20951BBDB2B");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "WMS Custom";

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
            var px1 = Projection.FromTileXYToPixel(pos);
            var px2 = px1;

            px1.Offset(0, Projection.TileSize.Height);
            PointLatLng p1 = Projection.FromPixelToLatLng(px1, zoom);

            px2.Offset(Projection.TileSize.Width, 0);
            PointLatLng p2 = Projection.FromPixelToLatLng(px2, zoom);

            string ret;

            string extra = "?";

            if (CustomWMSURL.Contains("?"))
                extra = "&";

            //if there is a layer, use it  
            if (szWmsLayer != "")
            {
                ret = string.Format(CultureInfo.InvariantCulture,
                    CustomWMSURL + extra + "VERSION=1.1.1&REQUEST=GetMap&SERVICE=WMS&layers=" + szWmsLayer +
                    "&styles=&bbox={0},{1},{2},{3}&width={4}&height={5}&srs=EPSG:4326&format=image/png", p1.Lng, p1.Lat,
                    p2.Lng, p2.Lat, Projection.TileSize.Width, Projection.TileSize.Height);
            }
            else
            {
                ret = string.Format(CultureInfo.InvariantCulture,
                    CustomWMSURL + extra +
                    "VERSION=1.1.1&REQUEST=GetMap&SERVICE=WMS&styles=&bbox={0},{1},{2},{3}&width={4}&height={5}&srs=EPSG:4326&format=image/png",
                    p1.Lng, p1.Lat, p2.Lng, p2.Lat, Projection.TileSize.Width, Projection.TileSize.Height);
            }

            return ret;
        }

        public static string szWmsLayer = "";

        public static string CustomWMSURL =
            "http://mapbender.wheregroup.com/cgi-bin/mapserv?map=/data/umn/osm/osm_basic.map";
    }
}