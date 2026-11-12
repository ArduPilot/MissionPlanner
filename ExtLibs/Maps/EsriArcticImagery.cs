using System;
using System.Collections.Generic;

namespace MissionPlanner.Maps
{
    using GMap.NET.Projections;
    using System.Globalization;
    using GMap.NET.MapProviders;
    using GMap.NET;
    using System.Reflection;

    /// <summary>
    /// Esri Polar Arctic Imagery (EPSG:5936)
    /// 15m TerraColor satellite imagery covering 50N-90N
    /// Free, no authentication required
    /// </summary>
    public class EsriArcticImagery : GMapProvider
    {
        public static readonly EsriArcticImagery Instance;

        EsriArcticImagery()
        {
            MaxZoom = 14;
            MinZoom = 0;
            Copyright = "Esri, Earthstar Geographics";
        }

        static EsriArcticImagery()
        {
            Instance = new EsriArcticImagery();

            Type mytype = typeof(GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>)field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("E4F7A2B1-8C3D-4E5F-9A6B-1D2E3F4A5B6C");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "Esri Arctic Imagery";

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
                    overlays = new GMapProvider[] { this };
                }
                return overlays;
            }
        }

        public override PureProjection Projection
        {
            get { return EPSG5936Projection.Instance; }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom);

            return GetTileImageUsingHttp(url);
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom)
        {
            // ArcGIS tile URL: tile/{z}/{y}/{x}
            return string.Format(CultureInfo.InvariantCulture, CustomURL,
                zoom, pos.Y, pos.X);
        }

        public static string CustomURL =
            "https://services.arcgisonline.com/arcgis/rest/services/" +
            "Polar/Arctic_Imagery/MapServer/tile/{0}/{1}/{2}";
    }
}
