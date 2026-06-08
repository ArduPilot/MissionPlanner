
namespace GMap.NET.Projections
{
   using System;

   /// <summary>
   /// EPSG:3995 - WGS 84 / Arctic Polar Stereographic
   /// Standard parallel: 71N, Central meridian: 0
   /// Tile grid matches ArcGIS Arctic Bathymetry Basemap (256x256 tiles)
   /// </summary>
   public class EPSG3995Projection : PureProjection
   {
      public static readonly EPSG3995Projection Instance = new EPSG3995Projection();

      // WGS84 ellipsoid
      const double SemiMajorAxis = 6378137.0;
      const double InverseFlattening = 298.257223563;
      const double flat = 1.0 / InverseFlattening;
      static readonly double e2 = 2 * flat - flat * flat;
      static readonly double ecc = Math.Sqrt(e2);

      // EPSG:3995 parameters
      static readonly double phi_c = 71.0 * Math.PI / 180.0; // standard parallel
      const double lambda_0 = 0.0; // central meridian in radians

      // ArcGIS tile grid parameters
      const double OriginX = -30636100.0;
      const double OriginY = 30636100.0;
      const double BaseResolution = 67733.46880027094; // meters/pixel at zoom 0

      // Geographic bounds
      static readonly double MinLatitude = 30.0;
      static readonly double MaxLatitude = 90.0;
      static readonly double MinLongitude = -180.0;
      static readonly double MaxLongitude = 180.0;

      // Pre-computed values for the standard parallel
      static readonly double m_c;
      static readonly double t_c;

      static EPSG3995Projection()
      {
         double sinPhiC = Math.Sin(phi_c);
         m_c = Math.Cos(phi_c) / Math.Sqrt(1.0 - e2 * sinPhiC * sinPhiC);
         t_c = ComputeT(phi_c);
      }

      static double ComputeT(double phi)
      {
         double sinPhi = Math.Sin(phi);
         return Math.Tan(Math.PI / 4.0 - phi / 2.0) /
                Math.Pow((1.0 - ecc * sinPhi) / (1.0 + ecc * sinPhi), ecc / 2.0);
      }

      public override RectLatLng Bounds
      {
         get
         {
            return RectLatLng.FromLTRB(MinLongitude, MaxLatitude, MaxLongitude, MinLatitude);
         }
      }

      readonly GSize tileSize = new GSize(256, 256);
      public override GSize TileSize
      {
         get
         {
            return tileSize;
         }
      }

      public override double Axis
      {
         get
         {
            return SemiMajorAxis;
         }
      }

      public override double Flattening
      {
         get
         {
            return flat;
         }
      }

      public override GPoint FromLatLngToPixel(double lat, double lng, int zoom)
      {
         GPoint ret = GPoint.Empty;

         lat = Clip(lat, MinLatitude, MaxLatitude);
         lng = Clip(lng, MinLongitude, MaxLongitude);

         double phi = DegreesToRadians(lat);
         double lambda = DegreesToRadians(lng);

         // Forward polar stereographic: lat/lng -> projected meters
         double t = ComputeT(phi);
         double rho = SemiMajorAxis * m_c * t / t_c;
         double x = rho * Math.Sin(lambda - lambda_0);
         double y = -rho * Math.Cos(lambda - lambda_0);

         // Meters -> pixels using ArcGIS tile grid
         double resolution = BaseResolution / (1 << zoom);
         double pixelX = (x - OriginX) / resolution;
         double pixelY = (OriginY - y) / resolution;

         long totalPixels = 3L * (1 << zoom) * 256;
         ret.X = (long)Clip(pixelX + 0.5, 0, totalPixels - 1);
         ret.Y = (long)Clip(pixelY + 0.5, 0, totalPixels - 1);

         return ret;
      }

      public override PointLatLng FromPixelToLatLng(long x, long y, int zoom)
      {
         PointLatLng ret = PointLatLng.Empty;

         // Pixels -> meters
         double resolution = BaseResolution / (1 << zoom);
         double mx = x * resolution + OriginX;
         double my = OriginY - y * resolution;

         // Inverse polar stereographic: meters -> lat/lng
         double rho = Math.Sqrt(mx * mx + my * my);

         if (rho < 1.0)
         {
            ret.Lat = 90.0;
            ret.Lng = 0.0;
            return ret;
         }

         double t = rho * t_c / (SemiMajorAxis * m_c);

         // Iterative inverse for latitude
         double phi = HALF_PI - 2.0 * Math.Atan(t);
         for (int i = 0; i < 15; i++)
         {
            double sinPhi = Math.Sin(phi);
            double newPhi = HALF_PI - 2.0 * Math.Atan(
               t * Math.Pow((1.0 - ecc * sinPhi) / (1.0 + ecc * sinPhi), ecc / 2.0));
            if (Math.Abs(newPhi - phi) < 1e-12)
               break;
            phi = newPhi;
         }

         // Longitude
         double lambda = lambda_0 + Math.Atan2(mx, -my);

         ret.Lat = Clip(RadiansToDegrees(phi), MinLatitude, MaxLatitude);
         ret.Lng = RadiansToDegrees(lambda);

         // Normalize longitude to [-180, 180]
         while (ret.Lng > 180) ret.Lng -= 360;
         while (ret.Lng < -180) ret.Lng += 360;

         return ret;
      }

      public override GSize GetTileMatrixMinXY(int zoom)
      {
         return new GSize(0, 0);
      }

      public override GSize GetTileMatrixMaxXY(int zoom)
      {
         // 3 * 2^zoom tiles per axis covers the data extent with margin
         long xy = 3L * (1 << zoom);
         return new GSize(xy - 1, xy - 1);
      }

      public override double GetGroundResolution(int zoom, double latitude)
      {
         return BaseResolution / (1 << zoom);
      }
   }
}
