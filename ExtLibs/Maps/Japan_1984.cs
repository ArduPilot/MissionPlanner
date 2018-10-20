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
    /// Japan 1984
    /// </summary>
    public class Japan_1984 : GMapProvider
    {
        public static readonly Japan_1984 Instance;

        Japan_1984 ()
        {
            MaxZoom = 22;
            MinZoom = 2;
            Copyright = string.Format("©Geospatial Information Authority of Japan.", DateTime.Today.Year);
        }

        static Japan_1984()
        {
            Instance = new Japan_1984();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("9f0c3381-3662-488c-8689-d3bcc2e3cbb4");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "Japan_1984";

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

            if (zoom < 10 || zoom > 17)
            {
                ret = string.Format(CultureInfo.InvariantCulture, CustomURLstd, zoom, pos.X, pos.Y);
            }
            else
            {
                ret = string.Format(CultureInfo.InvariantCulture, CustomURLgazo, zoom, pos.X, pos.Y);
            }

            return ret;
        }
        public readonly string CustomURLstd = 
            "https://cyberjapandata.gsi.go.jp/xyz/std/{0}/{1}/{2}.png";
            
        public readonly string CustomURLgazo = 
            "https://cyberjapandata.gsi.go.jp/xyz/gazo3/{0}/{1}/{2}.jpg";
    }
}
