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
    /// Japan
    /// </summary>
    public class Japan : GMapProvider
    {
        public static readonly Japan Instance;

        Japan ()
        {
            MaxZoom = 22;
            MinZoom = 2;
            Copyright = string.Format("©Geospatial Information Authority of Japan.", DateTime.Today.Year);
        }

        static Japan()
        {
            Instance = new Japan();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("99fe9e6b-719c-42d1-99e8-7fa2f55d4295");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "Japan";

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
            string ret;

            ret = string.Format(CultureInfo.InvariantCulture, CustomURL, zoom, pos.X, pos.Y);

            return ret;
        }

        public static string CustomURL =
            "https://cyberjapandata.gsi.go.jp/xyz/std/{0}/{1}/{2}.png";
    }
}
