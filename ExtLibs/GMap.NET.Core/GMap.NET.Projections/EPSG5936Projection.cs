
namespace GMap.NET.Projections
{
   using System;

   /// <summary>
   /// EPSG:5936 - WGS 84 / EPSG Alaska Polar Stereographic
   /// Polar Stereographic (Variant A): natural origin at North Pole
   /// Scale factor: 0.994, Central meridian: -150
   /// False easting/northing: 2,000,000
   /// Tile grid matches Esri Polar Arctic services (256x256 tiles)
   /// </summary>
   public class EPSG5936Projection : PureProjection
   {
      public static readonly EPSG5936Projection Instance = new EPSG5936Projection();

      // WGS84 ellipsoid
      const double SemiMajorAxis = 6378137.0;
      const double InverseFlattening = 298.257223563;
      const double flat = 1.0 / InverseFlattening;
      static readonly double e2 = 2 * flat - flat * flat;
      static readonly double ecc = Math.Sqrt(e2);

      // EPSG:5936 parameters (Variant A - scale factor at pole)
      const double k0 = 0.994;
      static readonly double lambda_0 = -150.0 * Math.PI / 180.0;
      const double FalseEasting = 2000000.0;
      const double FalseNorthing = 2000000.0;

      // Pre-computed factor F = sqrt((1+e)^(1+e) * (1-e)^(1-e))
      static readonly double F;

      // Esri Polar tiling scheme
      const double OriginX = -28567784.109255;
      const double OriginY = 32567784.109255;
      const double BaseResolution = 238810.81335399998;

      // Geographic bounds
      static readonly double MinLatitude = 50.0;
      static readonly double MaxLatitude = 90.0;
      static readonly double MinLongitude = -180.0;
      static readonly double MaxLongitude = 180.0;

      static EPSG5936Projection()
      {
         F = Math.Sqrt(Math.Pow(1.0 + ecc, 1.0 + ecc) * Math.Pow(1.0 - ecc, 1.0 - ecc));
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

         // Forward polar stereographic (Variant A at pole)
         double t = ComputeT(phi);
         double rho = 2.0 * SemiMajorAxis * k0 * t / F;

         double E = FalseEasting + rho * Math.Sin(lambda - lambda_0);
         double N = FalseNorthing - rho * Math.Cos(lambda - lambda_0);

         // Projected meters -> pixels using Esri tile grid
         double resolution = BaseResolution / (1 << zoom);
         double pixelX = (E - OriginX) / resolution;
         double pixelY = (OriginY - N) / resolution;

         long totalPixels = (1L << zoom) * 256;
         ret.X = (long)Clip(pixelX + 0.5, 0, totalPixels - 1);
         ret.Y = (long)Clip(pixelY + 0.5, 0, totalPixels - 1);

         return ret;
      }

      public override PointLatLng FromPixelToLatLng(long x, long y, int zoom)
      {
         PointLatLng ret = PointLatLng.Empty;

         // Pixels -> projected meters
         double resolution = BaseResolution / (1 << zoom);
         double E = x * resolution + OriginX;
         double N = OriginY - y * resolution;

         // Inverse polar stereographic
         double dx = E - FalseEasting;
         double dy = FalseNorthing - N;
         double rho = Math.Sqrt(dx * dx + dy * dy);

         if (rho < 1.0)
         {
            ret.Lat = 90.0;
            ret.Lng = 0.0;
            return ret;
         }

         double t = rho * F / (2.0 * SemiMajorAxis * k0);

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
         double lambda = lambda_0 + Math.Atan2(dx, dy);

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
         long xy = 1L << zoom;
         return new GSize(xy - 1, xy - 1);
      }

      public override double GetGroundResolution(int zoom, double latitude)
      {
         return BaseResolution / (1 << zoom);
      }
   }
}
