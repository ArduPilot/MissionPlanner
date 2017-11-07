using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Core.Utils
{
    public static class ColorUtils
    {
        public static string ColorToHashHTML(Color col) {
            Color tmpc2 = Color.FromArgb(col.ToArgb());
            return ColorTranslator.ToHtml(tmpc2);
        }

        /// <summary>
        /// Used to get an alpha blend of 2 colors. 
        /// </summary>
        /// <param name="aToB">A number from 0 -> 1 (100% Color A -> 100% Color B) </param>
        /// <returns>A new color that represents a mix of the other 2.</returns>
        public static Color GetInbetweenColor(Color a, Color b, double aToB) {
            aToB = Math.Min(1, aToB);
            int redD = b.R - a.R;
            int greenD = b.G - a.G;
            int blueD = b.B - a.B;

            int red = Convert.ToInt32((double)a.R + (double)redD * aToB);
            int green = Convert.ToInt32((double)a.G + (double)greenD * aToB);
            int blue = Convert.ToInt32((double)a.B + (double)blueD * aToB);

            return Color.FromArgb(red, green, blue);
        }

        /*--07/10/06---KnownColor enum should be used instead 
         * public static Color[] GetStandardColors()
        {
            Type tColor = typeof(Color);
            PropertyInfo[] pis = tColor.GetProperties(BindingFlags.Static | BindingFlags.Public);
            List<Color> colors = new List<Color>();
            foreach (PropertyInfo pi in pis) {
                if (pi.PropertyType.Equals(tColor)) {
                    colors.Add((Color)pi.GetValue(null, null));
                }
            }
            return colors.ToArray();
        }*/

        private static Dictionary<Color, CIELAB> cielabLookup = new Dictionary<Color, CIELAB>();

        public struct CIELAB
        {
            public double L;
            public double a;
            public double b;

            public double DistanceTo(CIELAB c) {
                double dL = L - c.L;
                double da = a - c.a;
                double db = b - c.b;
                return Math.Sqrt(0.5 * dL * dL + da * da + db * db);
            }

            public double DistanceTo(Color clr) {
                return DistanceTo(ConvertToCIELAB(clr));
            }
        }

        public static CIELAB ConvertToCIELAB(Color c) {
            if (cielabLookup.ContainsKey(c)) {
                return cielabLookup[c];
            }
            double rf = ConvertChannel(c.R) * 100;
            double gf = ConvertChannel(c.G) * 100;
            double bf = ConvertChannel(c.B) * 100;

            double var_X = rf * 0.4124 + gf * 0.3576 + bf * 0.1805;
            double var_Y = rf * 0.2126 + gf * 0.7152 + bf * 0.0722;
            double var_Z = rf * 0.0193 + gf * 0.1192 + bf * 0.9505;

            var_X = var_X / 95.047;          //Observer = 2°, Illuminant = D65
            var_Y = var_Y / 100.000;
            var_Z = var_Z / 108.883;

            double toPow = 1.0 / 3.0;
            if (var_X > 0.008856) {
                var_X = Math.Pow(var_X, toPow);
            } else {
                var_X = (7.787 * var_X) + (16 / 116);
            }
            if (var_Y > 0.008856) {
                var_Y = Math.Pow(var_Y, toPow);
            } else {
                var_Y = (7.787 * var_Y) + (16 / 116);
            }
            if (var_Z > 0.008856) {
                var_Z = Math.Pow(var_Z, toPow);
            } else {
                var_Z = (7.787 * var_Z) + (16 / 116);
            }

            CIELAB ans = new CIELAB();
            ans.L = (116 * var_Y) - 16;
            ans.a = 500 * (var_X - var_Y);
            ans.b = 200 * (var_Y - var_Z);

            cielabLookup[c] = ans;
            return ans;
        }

        private static double ConvertChannel(byte channel) {
            double f = (double)channel / 255;
            if (f > 0.04045) {
                return Math.Pow((f + 0.055) / 1.055, 2.4);
            }
            return f / 12.92;
        }

        /*
         procedure RGBToCIELab(R,G,B: Byte; var CIEL,CIEa,CIEb: Single);
var
  rf,gf,bf: Single;
  var_X,var_Y,var_Z: Single;
  X,Y,Z: Single;
begin
  //Observer. = 2°, Illuminant = D65
  rf = ( R / 255 );        //R = From 0 to 255
  gf = ( G / 255 );        //G = From 0 to 255
  bf = ( B / 255 );        //B = From 0 to 255

  if ( rf > 0.04045 )then
    rf = Power(( ( rf + 0.055 ) / 1.055 ),2.4)
  else
    rf = rf / 12.92;

  if ( gf > 0.04045 ) then
    gf = Power(( ( gf + 0.055 ) / 1.055 ),2.4)
  else
    gf = gf / 12.92;

  if ( bf > 0.04045 ) then
    bf = Power(( ( bf + 0.055 ) / 1.055 ),2.4)
  else
    bf = bf / 12.92;

  rf = rf * 100;
  gf = gf * 100;
  bf = bf * 100;

  //Observer. = 2°, Illuminant = D65
  X = rf * 0.4124 + gf * 0.3576 + bf * 0.1805;
  Y = rf * 0.2126 + gf * 0.7152 + bf * 0.0722;
  Z = rf * 0.0193 + gf * 0.1192 + bf * 0.9505;

  var_X = X / 95.047;          //Observer = 2°, Illuminant = D65
  var_Y = Y / 100.000;
  var_Z = Z / 108.883;

  if ( var_X > 0.008856 )then
    var_X = Power(var_X ,( 1/3 ))
  else
    var_X = ( 7.787 * var_X ) + ( 16 / 116 );

  if ( var_Y > 0.008856 ) then
    var_Y = Power(var_Y ,( 1/3 ))
  else
    var_Y = ( 7.787 * var_Y ) + ( 16 / 116 );

  if ( var_Z > 0.008856 ) then
    var_Z = Power(var_Z ,( 1/3 ))
  else
    var_Z = ( 7.787 * var_Z ) + ( 16 / 116 );

  CIEL = ( 116 * var_Y ) - 16;
  CIEa = 500 * ( var_X - var_Y );
  CIEb = 200 * ( var_Y - var_Z );
end;
         */
         /*
        public static Color GetRandomNamedColor(Random r) {
            Color clr = Color.FromKnownColor(KnownColor.Control);
            Array clrs = Enum.GetValues(typeof(KnownColor));
            while (clr.IsSystemColor) {
                clr = Color.FromKnownColor((KnownColor)clrs.GetValue(r.Next(clrs.Length)));
            }
            return clr;
        }
        */
        /*public static Color Invert(Color aColor)
        {
            int r = 255 - Convert.ToInt32(aColor.R);
            int g = 255 - Convert.ToInt32(aColor.G);
            int b = 255 - Convert.ToInt32(aColor.B);
            return Color.FromArgb(aColor.A, r, g, b);
        }*/

        public static Color Invert(Color aColor) {
            int r = Convert.ToInt32(aColor.R);
            int g = Convert.ToInt32(aColor.G);
            int b = Convert.ToInt32(aColor.B);
            return ((r + g + b) > 255) ? Color.Black : Color.White;
        }
        /*
        public static List<Color> GetNamedColors() {
            List<Color> colors = new List<Color>();
            foreach (string n in Enum.GetNames(typeof(KnownColor))) {
                Color clr = Color.FromKnownColor(MiscUtils.ParseEnum<KnownColor>(n));
                if (!clr.IsSystemColor) {
                    colors.Add(clr);
                }
            }
            return colors;
        }
        */
        /// <summary>
        /// Gets a list of colors that are theoretically all distinguishable from each other. Colors at the beginning of the list are easily distinguished from each other.
        /// </summary>
        public static List<Color> GetDistinctColors() {
            List<Color> colors = new List<Color>();
            //--
            colors.Add(ColorTranslator.FromHtml("#BEE117"));
            colors.Add(ColorTranslator.FromHtml("#FFFF00"));
            colors.Add(ColorTranslator.FromHtml("#D8011D"));
            colors.Add(ColorTranslator.FromHtml("#0066FF"));
            colors.Add(ColorTranslator.FromHtml("#F16D9F"));
            colors.Add(ColorTranslator.FromHtml("#AC4CDC"));
            colors.Add(ColorTranslator.FromHtml("#FF9400"));
            colors.Add(ColorTranslator.FromHtml("#52A400"));
            //--
            colors.Add(ColorTranslator.FromHtml("#FEBABA"));
            colors.Add(ColorTranslator.FromHtml("#80D50B"));
            colors.Add(ColorTranslator.FromHtml("#FFCE00"));
            colors.Add(ColorTranslator.FromHtml("#CBFF84"));
            colors.Add(ColorTranslator.FromHtml("#00A6FF"));
            colors.Add(ColorTranslator.FromHtml("#AD5701"));
            colors.Add(ColorTranslator.FromHtml("#990000"));
            colors.Add(ColorTranslator.FromHtml("#0000CC"));
            //--
            colors.Add(ColorTranslator.FromHtml("#CC99CC"));
            colors.Add(ColorTranslator.FromHtml("#0D9D93"));
            colors.Add(ColorTranslator.FromHtml("#66CCFF"));
            colors.Add(ColorTranslator.FromHtml("#CC9966"));
            colors.Add(ColorTranslator.FromHtml("#CC6699"));
            colors.Add(ColorTranslator.FromHtml("#FDE6C1"));
            colors.Add(ColorTranslator.FromHtml("#FFE300"));
            colors.Add(ColorTranslator.FromHtml("#009933"));
            colors.Add(ColorTranslator.FromHtml("#C4E1FF"));
            colors.Add(ColorTranslator.FromHtml("#9999FF"));
            colors.Add(ColorTranslator.FromHtml("#E0FF00"));
            //--
            colors.Add(ColorTranslator.FromHtml("#B3EA5F"));
            colors.Add(ColorTranslator.FromHtml("#DCDC78"));
            colors.Add(ColorTranslator.FromHtml("#FF6600"));
            colors.Add(ColorTranslator.FromHtml("#CCCCFF"));
            colors.Add(ColorTranslator.FromHtml("#843C3C"));
            colors.Add(ColorTranslator.FromHtml("#416529"));
            colors.Add(ColorTranslator.FromHtml("#00CC99"));
            colors.Add(ColorTranslator.FromHtml("#71437C"));
            //--
            colors.Add(ColorTranslator.FromHtml("#66FF00"));
            colors.Add(ColorTranslator.FromHtml("#DBEF9A"));
            colors.Add(ColorTranslator.FromHtml("#429392"));
            colors.Add(ColorTranslator.FromHtml("#654229"));
            colors.Add(ColorTranslator.FromHtml("#FF6060"));
            colors.Add(ColorTranslator.FromHtml("#91FDA3"));
            colors.Add(ColorTranslator.FromHtml("#ED8787"));
            colors.Add(ColorTranslator.FromHtml("#8DC0AF"));
            colors.Add(ColorTranslator.FromHtml("#DDF4CA"));
            colors.Add(ColorTranslator.FromHtml("#FFD9FF"));
            colors.Add(ColorTranslator.FromHtml("#598A5F"));
            colors.Add(ColorTranslator.FromHtml("#FFFF99"));
            colors.Add(ColorTranslator.FromHtml("#CCFFFF"));
            colors.Add(ColorTranslator.FromHtml("#BE00CC"));
            colors.Add(ColorTranslator.FromHtml("#1A3D51"));
            colors.Add(ColorTranslator.FromHtml("#993366"));
            colors.Add(ColorTranslator.FromHtml("#996633"));
            colors.Add(ColorTranslator.FromHtml("#FF0000"));
            colors.Add(ColorTranslator.FromHtml("#33FFFF"));
            colors.Add(ColorTranslator.FromHtml("#006699"));
            colors.Add(ColorTranslator.FromHtml("#66FF99"));
            colors.Add(ColorTranslator.FromHtml("#006633"));
            colors.Add(ColorTranslator.FromHtml("#E639FF"));

            colors.Add(Color.Black);
            colors.Add(Color.Silver);
            colors.Add(Color.Gainsboro);
            colors.Add(Color.White);
            return colors;
        }
        
        public static bool ColorsMatch(Color a, Color b, double distance) {
            CIELAB aLab = ConvertToCIELAB(a);
            return aLab.DistanceTo(b) < distance;
        }

        public static bool ColorsMatch(Color a, Color b, int tol) {
            if (Math.Abs(a.R - b.R) > tol) {
                return false;
            }
            if (Math.Abs(a.G - b.G) > tol) {
                return false;
            }
            if (Math.Abs(a.B - b.B) > tol) {
                return false;
            }
            return true;
        }

        public static bool AlphaTest(Color col, float threshold) {
            return (col.A > threshold * 255);
        }
    }
}
