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
    /// NASA GIBS Antarctic (EPSG:3031) - Blue Marble Shaded Relief
    /// Tile source: https://gibs.earthdata.nasa.gov/wmts/epsg3031/best/
    /// </summary>
    public class GIBSAntarctic : GMapProvider
    {
        public static readonly GIBSAntarctic Instance;

        GIBSAntarctic()
        {
            MaxZoom = 4;
            MinZoom = 0;
            Copyright = "NASA GIBS";
            RefererUrl = "https://gibs.earthdata.nasa.gov";
        }

        static GIBSAntarctic()
        {
            Instance = new GIBSAntarctic();

            Type mytype = typeof(GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>)field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("B6C201F4-7D8A-4E3B-A5F9-2C1D6E8F0A3B");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "GIBS Antarctic";

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
            get { return PolarStereographicSouthProjection.Instance; }
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

        // Blue Marble Shaded Relief, 500m TileMatrixSet (zoom 0-4)
        public static string CustomURL =
            "https://gibs.earthdata.nasa.gov/wmts/epsg3031/best/" +
            "BlueMarble_ShadedRelief/default/2004-08/" +
            "500m/{0}/{1}/{2}.jpeg";
    }
}
