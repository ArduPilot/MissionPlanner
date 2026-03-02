using System;
using System.Collections.Generic;
using System.Text;

namespace GMap.NET.Core.GMap.NET.Projections
{
    public class NoProjection : PureProjection
    {
        public static PureProjection Instance { get; set; } = new NoProjection();

        public override GSize TileSize => new GSize(256, 256);

        public override double Axis => 6378137;

        public override double Flattening => (1.0 / 298.257223563);


        public override GPoint FromLatLngToPixel(double lat, double lng, int zoom)
        
        {
            GPoint ret = GPoint.Empty;

            double x = (lng + 180) / 360;
            double y = (90 - lat) / 180;

            GSize s = GetTileMatrixSizePixel(zoom);
            long mapSizeX = s.Width;
            long mapSizeY = s.Height;

            ret.X = (long)Clip(x * mapSizeX + 0.5, 0, mapSizeX - 1);
            ret.Y = (long)Clip(y * mapSizeY + 0.5, 0, mapSizeY - 1);

            return ret;
        }

        public override PointLatLng FromPixelToLatLng(long x, long y, int zoom)
        {
            PointLatLng ret = PointLatLng.Empty;

            GSize s = GetTileMatrixSizePixel(zoom);
            double mapSizeX = s.Width;
            double mapSizeY = s.Height;

            double xx = (Clip(x, 0, mapSizeX - 1) / mapSizeX);
            double yy = (Clip(y, 0, mapSizeY - 1) / mapSizeY);

            ret.Lat = 90 - 180 * yy;
            ret.Lng = 360 * xx - 180;

            return ret;
        }

        public override GSize GetTileMatrixMaxXY(int zoom)
        {
            long xy = (1 << zoom);
            return new GSize(xy - 1, xy - 1);
        }

        public override GSize GetTileMatrixMinXY(int zoom)
        {
            return new GSize(0, 0);
        }
    }
}
