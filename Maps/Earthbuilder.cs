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
    /// EarthBuilder Custom
    /// </summary>
    public class Earthbuilder : GMapProvider
    {
        public static readonly Earthbuilder Instance;

        Earthbuilder()
        {
        }

        static Earthbuilder()
        {
            Instance = new Earthbuilder();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("a6571151-1ef0-43d3-a454-92d6fd64a843");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "Earthbuilder Custom";

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

            ret = string.Format(CultureInfo.InvariantCulture, CustomURL + "&x={0}&y={1}&z={2}&s=", pos.X, pos.Y, zoom);

            return ret;
        }

        public static string CustomURL =
            "https://earthbuilder.googleapis.com/09372590152434720789-00913315481290556980-4/10/maptile/maps?v=84&authToken=CggK0cFX5-FgiRDSgKGXBQ==";
    }
}