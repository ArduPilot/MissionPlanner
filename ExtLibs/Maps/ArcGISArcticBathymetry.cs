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
    /// ArcGIS Arctic Bathymetry Basemap (EPSG:3995)
    /// IBCAO v4 + GEBCO_2020 bathymetry/topography with Natural Earth land cover
    /// Zoom 0-10, ~66m/pixel at max zoom
    /// </summary>
    public class ArcGISArcticBathymetry : GMapProvider
    {
        public static readonly ArcGISArcticBathymetry Instance;

        ArcGISArcticBathymetry()
        {
            MaxZoom = 10;
            MinZoom = 0;
            Copyright = "Esri, IBCAO, GEBCO, Natural Earth";
        }

        static ArcGISArcticBathymetry()
        {
            Instance = new ArcGISArcticBathymetry();

            Type mytype = typeof(GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>)field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("D2E8F1A3-5B6C-4D9E-8A7F-3C2B1D0E4F5A");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "ArcGIS Arctic Bathymetry";

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
            get { return EPSG3995Projection.Instance; }
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
            "https://tiles.arcgis.com/tiles/C8EMgrsFcRFL6LrL/arcgis/rest/services/" +
            "Arctic_Bathymetry_Basemap/MapServer/tile/{0}/{1}/{2}";
    }
}
