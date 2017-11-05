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
    /// Eniro_Topo Custom
    /// </summary>
    public class Eniro_Topo : GMapProvider
    {
        public static readonly Eniro_Topo Instance;

        Eniro_Topo()
        {
        }

        static Eniro_Topo()
        {
            Instance = new Eniro_Topo();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("9056b8f1-977d-4def-b814-616c0415941b");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "Eniro_Topo";

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

            this.RefererUrl = "http://eniro.se/";

            ret = string.Format(CultureInfo.InvariantCulture, CustomURL, zoom, pos.X, (1 << zoom) - 1 - pos.Y);

            return ret;
        }

        //http://map.eniro.com/geowebcache/service/tms1.0.0/map/%1/%2/%3.png             

        public static string CustomURL =
            "http://map.eniro.com/geowebcache/service/tms1.0.0/map/{0}/{1}/{2}.png";
    }
}