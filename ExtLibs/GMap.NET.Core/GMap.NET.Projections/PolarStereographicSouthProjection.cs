
namespace GMap.NET.Projections
{
   using System;

   /// <summary>
   /// EPSG:3031 - WGS 84 / Antarctic Polar Stereographic
   /// Standard parallel: -71S, Central meridian: 0
   /// Used by NASA GIBS Antarctic tile service (512x512 tiles)
   /// </summary>
   public class PolarStereographicSouthProjection : PureProjection
   {
      public static readonly PolarStereographicSouthProjection Instance = new PolarStereographicSouthProjection();

      // WGS84 ellipsoid
      const double SemiMajorAxis = 6378137.0;
      const double InverseFlattening = 298.257223563;
      const double flat = 1.0 / InverseFlattening;
      static readonly double e2 = 2 * flat - flat * flat;
      static readonly double ecc = Math.Sqrt(e2);

      // EPSG:3031 parameters (absolute values used in computation)
      const double phi_c_abs = 71.0; // |standard parallel| in degrees
      static readonly double phi_c_rad = phi_c_abs * Math.PI / 180.0;
      const double lambda_0 = 0.0; // central meridian in degrees
      static readonly double lambda_0_rad = lambda_0 * Math.PI / 180.0;

      // GIBS tile matrix (same extent as EPSG:3413)
      const double FullExtent = 8388608.0;
      const double OriginX = -4194304.0;
      const double OriginY = 4194304.0;

      // Geographic bounds (southern hemisphere)
      static readonly double MinLatitude = -90.0;
      static readonly double MaxLatitude = -30.0;
      static readonly double MinLongitude = -180.0;
      static readonly double MaxLongitude = 180.0;

      // Pre-computed values for the standard parallel
      static readonly double m_c;
      static readonly double t_c;

      static PolarStereographicSouthProjection()
      {
         double sinPhiC = Math.Sin(phi_c_rad);
         m_c = Math.Cos(phi_c_rad) / Math.Sqrt(1.0 - e2 * sinPhiC * sinPhiC);
         t_c = ComputeT(phi_c_rad);
      }

      /// <summary>
      /// Conformal latitude parameter for north pole formulation.
      /// For south pole, we mirror the latitude to positive before calling this.
      /// </summary>
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

      readonly GSize tileSize = new GSize(512, 512);
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

         // Mirror latitude to positive for south pole computation
         double phiAbs = DegreesToRadians(-lat);
         double lambda = DegreesToRadians(lng);

         // Forward polar stereographic using mirrored latitude
         double t = ComputeT(phiAbs);
         double rho = SemiMajorAxis * m_c * t / t_c;

         // South pole: y = +rho*cos (not -rho*cos like north)
         double x = rho * Math.Sin(lambda - lambda_0_rad);
         double y = rho * Math.Cos(lambda - lambda_0_rad);

         // Meters -> pixels
         long totalPixels = (1L << (zoom + 1)) * 512;
         double pixelX = (x - OriginX) / FullExtent * totalPixels;
         double pixelY = (OriginY - y) / FullExtent * totalPixels;

         ret.X = (long)Clip(pixelX + 0.5, 0, totalPixels - 1);
         ret.Y = (long)Clip(pixelY + 0.5, 0, totalPixels - 1);

         return ret;
      }

      public override PointLatLng FromPixelToLatLng(long x, long y, int zoom)
      {
         PointLatLng ret = PointLatLng.Empty;

         long totalPixels = (1L << (zoom + 1)) * 512;

         // Pixels -> meters
         double mx = (double)x / totalPixels * FullExtent + OriginX;
         double my = OriginY - (double)y / totalPixels * FullExtent;

         // Inverse polar stereographic: meters -> lat/lng
         double rho = Math.Sqrt(mx * mx + my * my);

         if (rho < 1.0)
         {
            // At the south pole
            ret.Lat = -90.0;
            ret.Lng = 0.0;
            return ret;
         }

         double t = rho * t_c / (SemiMajorAxis * m_c);

         // Iterative inverse for mirrored (positive) latitude
         double phiAbs = HALF_PI - 2.0 * Math.Atan(t);
         for (int i = 0; i < 15; i++)
         {
            double sinPhi = Math.Sin(phiAbs);
            double newPhi = HALF_PI - 2.0 * Math.Atan(
               t * Math.Pow((1.0 - ecc * sinPhi) / (1.0 + ecc * sinPhi), ecc / 2.0));
            if (Math.Abs(newPhi - phiAbs) < 1e-12)
               break;
            phiAbs = newPhi;
         }

         // South pole longitude: atan2(x, y) not atan2(x, -y)
         double lambda = lambda_0_rad + Math.Atan2(mx, my);

         // Mirror latitude back to southern hemisphere
         ret.Lat = Clip(-RadiansToDegrees(phiAbs), MinLatitude, MaxLatitude);
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
         // GIBS EPSG:3031 uses 2^(z+1) tiles per axis
         long xy = 1L << (zoom + 1);
         return new GSize(xy - 1, xy - 1);
      }

      public override double GetGroundResolution(int zoom, double latitude)
      {
         // Meters per pixel (true at standard parallel 71S)
         long totalPixels = (1L << (zoom + 1)) * 512;
         return FullExtent / totalPixels;
      }
   }
}
