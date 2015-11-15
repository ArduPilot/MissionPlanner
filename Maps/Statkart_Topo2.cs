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
    public class Statkart_Topo2 : GMapProvider
    {
        public static readonly Statkart_Topo2 Instance;

        Statkart_Topo2()
        {
        }

        static Statkart_Topo2()
        {
            Instance = new Statkart_Topo2();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("24855c2f-7f75-4074-a30b-ac3ce3287237");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "Statkart_Topo2";

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

            this.RefererUrl = "http://www.norgeskart.no/";

            ret = string.Format(CultureInfo.InvariantCulture, CustomURL, zoom, pos.X, pos.Y);

            return ret;
        }

        //http://opencache.statkart.no/gatekeeper/gk/gk.open_gmaps?layers=topo2&zoom=%1&x=%2&y=%3              

        public static string CustomURL =
            "http://opencache.statkart.no/gatekeeper/gk/gk.open_gmaps?layers=topo2&zoom={0}&x={1}&y={2}";
    }
}