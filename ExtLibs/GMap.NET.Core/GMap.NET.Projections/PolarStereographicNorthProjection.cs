
namespace GMap.NET.Projections
{
   using System;

   /// <summary>
   /// EPSG:3413 - WGS 84 / NSIDC Sea Ice Polar Stereographic North
   /// Standard parallel: 70N, Central meridian: -45
   /// Used by NASA GIBS Arctic tile service (512x512 tiles)
   /// </summary>
   public class PolarStereographicNorthProjection : PureProjection
   {
      public static readonly PolarStereographicNorthProjection Instance = new PolarStereographicNorthProjection();

      // WGS84 ellipsoid
      const double SemiMajorAxis = 6378137.0;
      const double InverseFlattening = 298.257223563;
      const double flat = 1.0 / InverseFlattening;
      static readonly double e2 = 2 * flat - flat * flat;
      static readonly double ecc = Math.Sqrt(e2);

      // EPSG:3413 parameters
      static readonly double phi_c = 70.0 * Math.PI / 180.0; // standard parallel
      static readonly double lambda_0 = -45.0 * Math.PI / 180.0; // central meridian

      // GIBS tile matrix
      const double FullExtent = 8388608.0; // meters, total extent per axis
      const double OriginX = -4194304.0;   // top-left X in meters
      const double OriginY = 4194304.0;    // top-left Y in meters

      // Geographic bounds
      static readonly double MinLatitude = 30.0;
      static readonly double MaxLatitude = 90.0;
      static readonly double MinLongitude = -180.0;
      static readonly double MaxLongitude = 180.0;

      // Pre-computed values for the standard parallel
      static readonly double sin_phi_c;
      static readonly double m_c;
      static readonly double t_c;

      static PolarStereographicNorthProjection()
      {
         sin_phi_c = Math.Sin(phi_c);
         m_c = Math.Cos(phi_c) / Math.Sqrt(1.0 - e2 * sin_phi_c * sin_phi_c);
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

         double phi = DegreesToRadians(lat);
         double lambda = DegreesToRadians(lng);

         // Forward polar stereographic: lat/lng -> projected meters
         double t = ComputeT(phi);
         double rho = SemiMajorAxis * m_c * t / t_c;
         double x = rho * Math.Sin(lambda - lambda_0);
         double y = -rho * Math.Cos(lambda - lambda_0);

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
         // GIBS EPSG:3413 uses 2^(z+1) tiles per axis
         long xy = 1L << (zoom + 1);
         return new GSize(xy - 1, xy - 1);
      }

      public override double GetGroundResolution(int zoom, double latitude)
      {
         // Meters per pixel (true at standard parallel 70N)
         long totalPixels = (1L << (zoom + 1)) * 512;
         return FullExtent / totalPixels;
      }
   }
}
