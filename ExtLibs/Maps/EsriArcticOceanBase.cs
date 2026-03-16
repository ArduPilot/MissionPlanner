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
    /// Esri Polar Arctic Ocean Base (EPSG:5936)
    /// Marine basemap with bathymetry, shaded relief, land cover, and roads
    /// GEBCO, NOAA, National Geographic, DeLorme, Esri
    /// Free, no authentication required
    /// </summary>
    public class EsriArcticOceanBase : GMapProvider
    {
        public static readonly EsriArcticOceanBase Instance;

        EsriArcticOceanBase()
        {
            MaxZoom = 14;
            MinZoom = 0;
            Copyright = "Esri, GEBCO, NOAA, National Geographic, DeLorme";
        }

        static EsriArcticOceanBase()
        {
            Instance = new EsriArcticOceanBase();

            Type mytype = typeof(GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>)field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("F5A8B3C2-9D4E-4F6A-8B7C-2E3F4A5B6D7E");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "Esri Arctic Ocean Base";

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
            "Polar/Arctic_Ocean_Base/MapServer/tile/{0}/{1}/{2}";
    }
}
