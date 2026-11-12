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
    /// NASA GIBS Arctic (EPSG:3413) - Blue Marble Shaded Relief Bathymetry
    /// Tile source: https://gibs.earthdata.nasa.gov/wmts/epsg3413/best/
    /// </summary>
    public class GIBSArctic : GMapProvider
    {
        public static readonly GIBSArctic Instance;

        GIBSArctic()
        {
            MaxZoom = 4;
            MinZoom = 0;
            Copyright = "NASA GIBS";
            RefererUrl = "https://gibs.earthdata.nasa.gov";
        }

        static GIBSArctic()
        {
            Instance = new GIBSArctic();

            Type mytype = typeof(GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>)field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("A5A300B8-2B3C-4E6F-9A10-7C3A8D4F1E2B");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "GIBS Arctic";

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
            get { return PolarStereographicNorthProjection.Instance; }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom);

            return GetTileImageUsingHttp(url);
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom)
        {
            // GIBS WMTS: {z}/{row}/{col} where row=Y, col=X
            return string.Format(CultureInfo.InvariantCulture, CustomURL,
                zoom, pos.Y, pos.X);
        }

        // Blue Marble Shaded Relief Bathymetry, 500m TileMatrixSet (zoom 0-4)
        // Change to a 250m layer URL for zoom 5 support
        public static string CustomURL =
            "https://gibs.earthdata.nasa.gov/wmts/epsg3413/best/" +
            "BlueMarble_ShadedRelief_Bathymetry/default/2004-08/" +
            "500m/{0}/{1}/{2}.jpeg";
    }
}
