using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;

namespace MissionPlanner.Maps
{
    using System;
    using GMap.NET.Projections;
    using System.Globalization;
    using GMap.NET.MapProviders;
    using GMap.NET;
    using System.Reflection;

    /// <summary>
    /// EarthBuilder Custom
    /// </summary>
    public class MapboxUser : GMapProvider
    {
        public static readonly MapboxUser Instance;

        MapboxUser()
        {
        }

        static MapboxUser()
        {
            Instance = new MapboxUser();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("d83d5ba2-7ff4-49b0-9840-0a4bba402402");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "MapBox User";

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

            ret = string.Format(CultureInfo.InvariantCulture, CustomURL, pos.X, pos.Y, zoom, StyleId, UserName, MapKey);

            return ret;
        }

        public static string CustomURL = "https://api.mapbox.com/styles/v1/{4}/{3}/tiles/256/{2}/{0}/{1}?access_token={5}";

        public object UserName = Settings.Instance["MapBoxUserName", ""];
        public object StyleId = Settings.Instance["MapBoxStyleID", ""];
        public object MapKey = Settings.Instance["MapBoxMapKey", ""];
    }
}