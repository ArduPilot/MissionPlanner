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
    /// Japan Sea
    /// </summary>
    public class Japan_Sea : GMapProvider
    {
        public static readonly Japan_Sea Instance;

        Japan_Sea ()
        {
            MaxZoom = 22;
            MinZoom = 2;
            Copyright = string.Format("©Geospatial Information Authority of Japan.", DateTime.Today.Year);
        }

        static Japan_Sea()
        {
            Instance = new Japan_Sea();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("7a3a64d0-98f7-42d3-b7ee-31fac87b4c6c");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "Japan_Sea";

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

            if (zoom < 14 || zoom > 16)
            {
                ret = string.Format(CultureInfo.InvariantCulture, CustomURLstd, zoom, pos.X, pos.Y);
            }
            else
            {
                ret = string.Format(CultureInfo.InvariantCulture, CustomURLccm, zoom, pos.X, pos.Y);
            }

            return ret;
        }
        public readonly string CustomURLstd = 
            "https://cyberjapandata.gsi.go.jp/xyz/std/{0}/{1}/{2}.png";
            
        public readonly string CustomURLccm = 
            "https://cyberjapandata.gsi.go.jp/xyz/ccm1/{0}/{1}/{2}.png";
    }
}
