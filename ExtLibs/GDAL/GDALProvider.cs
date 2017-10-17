using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GDAL
{
    using GMap.NET;
    using GMap.NET.MapProviders;
    using GMap.NET.Projections;
    using System;
    using System.Reflection;

    /// <summary>
    /// GDAL Custom
    /// </summary>
    public class GDALProvider : GMapProvider
    {
        public static readonly GDALProvider Instance;

        GDALProvider()
        {
            MaxZoom = 24;
        }

        static GDALProvider()
        {
            Instance = new GDALProvider();

            Type mytype = typeof (GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>) field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);

            //GMap.NET.MapProviders.GMapProviders.List.Add(Instance);
        }

        readonly Guid id = new Guid("4574218D-B552-4CAF-89AE-F20941BBDB2B");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "GDAL Custom";

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
                    overlays = new GMapProvider[] { GoogleSatelliteMapProvider.Instance, this };
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
            var px1 = GDALProvider.Instance.Projection.FromTileXYToPixel(pos);
            var px2 = px1;

            px1.Offset(0, 0);
            PointLatLng p1 = GDALProvider.Instance.Projection.FromPixelToLatLng(px1, zoom);

            px2.Offset(GDALProvider.Instance.Projection.TileSize.Width, -GDALProvider.Instance.Projection.TileSize.Height);
            PointLatLng p2 = GDALProvider.Instance.Projection.FromPixelToLatLng(px2, zoom);

            var bmp = GDAL.GetBitmap(p1.Lng, p1.Lat, p2.Lng, p2.Lat, GDALProvider.Instance.Projection.TileSize.Width, GDALProvider.Instance.Projection.TileSize.Height);

            if (bmp == null)
            {
                return null;
            }

            var ms = new MemoryStream();

            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            return new GMapImage() { Img = bmp, Data = ms };
        }
    }

    public class GMapImage : PureImage
    {
        public Image Img { get; set; }

        public override void Dispose()
        {
            Img.Dispose();
            Data.Dispose();
        }
    }
}