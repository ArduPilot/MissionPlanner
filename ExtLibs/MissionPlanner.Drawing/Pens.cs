using SkiaSharp;

namespace System.Drawing
{
    public class Pens
    {
        // each property used to return a new Pen (two native paints) per access; build each once.
        // System pens are shared, so Dispose() on them does nothing, as GDI+ doesn't allow it either.
        private static Pen SystemPen(uint color)
        {
            return new Pen() {nativePen = new SKPaint() {Color = new SKColor(color)}, isSystemPen = true};
        }

        private static Pen _Transparent;
        public static Pen Transparent
        {
            get { return _Transparent ?? (_Transparent = SystemPen(0x00FFFFFF)); }
        }

        private static Pen _AliceBlue;
        public static Pen AliceBlue
        {
            get { return _AliceBlue ?? (_AliceBlue = SystemPen(0xFFF0F8FF)); }
        }

        private static Pen _AntiqueWhite;
        public static Pen AntiqueWhite
        {
            get { return _AntiqueWhite ?? (_AntiqueWhite = SystemPen(0xFFFAEBD7)); }
        }

        private static Pen _Aqua;
        public static Pen Aqua
        {
            get { return _Aqua ?? (_Aqua = SystemPen(0xFF00FFFF)); }
        }

        private static Pen _Aquamarine;
        public static Pen Aquamarine
        {
            get { return _Aquamarine ?? (_Aquamarine = SystemPen(0xFF7FFFD4)); }
        }

        private static Pen _Azure;
        public static Pen Azure
        {
            get { return _Azure ?? (_Azure = SystemPen(0xFFF0FFFF)); }
        }

        private static Pen _Beige;
        public static Pen Beige
        {
            get { return _Beige ?? (_Beige = SystemPen(0xFFF5F5DC)); }
        }

        private static Pen _Bisque;
        public static Pen Bisque
        {
            get { return _Bisque ?? (_Bisque = SystemPen(0xFFFFE4C4)); }
        }

        private static Pen _Black;
        public static Pen Black
        {
            get { return _Black ?? (_Black = SystemPen(0xFF000000)); }
        }

        private static Pen _BlanchedAlmond;
        public static Pen BlanchedAlmond
        {
            get { return _BlanchedAlmond ?? (_BlanchedAlmond = SystemPen(0xFFFFEBCD)); }
        }

        private static Pen _Blue;
        public static Pen Blue
        {
            get { return _Blue ?? (_Blue = SystemPen(0xFF0000FF)); }
        }

        private static Pen _BlueViolet;
        public static Pen BlueViolet
        {
            get { return _BlueViolet ?? (_BlueViolet = SystemPen(0xFF8A2BE2)); }
        }

        private static Pen _Brown;
        public static Pen Brown
        {
            get { return _Brown ?? (_Brown = SystemPen(0xFFA52A2A)); }
        }

        private static Pen _BurlyWood;
        public static Pen BurlyWood
        {
            get { return _BurlyWood ?? (_BurlyWood = SystemPen(0xFFDEB887)); }
        }

        private static Pen _CadetBlue;
        public static Pen CadetBlue
        {
            get { return _CadetBlue ?? (_CadetBlue = SystemPen(0xFF5F9EA0)); }
        }

        private static Pen _Chartreuse;
        public static Pen Chartreuse
        {
            get { return _Chartreuse ?? (_Chartreuse = SystemPen(0xFF7FFF00)); }
        }

        private static Pen _Chocolate;
        public static Pen Chocolate
        {
            get { return _Chocolate ?? (_Chocolate = SystemPen(0xFFD2691E)); }
        }

        private static Pen _Coral;
        public static Pen Coral
        {
            get { return _Coral ?? (_Coral = SystemPen(0xFFFF7F50)); }
        }

        private static Pen _CornflowerBlue;
        public static Pen CornflowerBlue
        {
            get { return _CornflowerBlue ?? (_CornflowerBlue = SystemPen(0xFF6495ED)); }
        }

        private static Pen _Cornsilk;
        public static Pen Cornsilk
        {
            get { return _Cornsilk ?? (_Cornsilk = SystemPen(0xFFFFF8DC)); }
        }

        private static Pen _Crimson;
        public static Pen Crimson
        {
            get { return _Crimson ?? (_Crimson = SystemPen(0xFFDC143C)); }
        }

        private static Pen _Cyan;
        public static Pen Cyan
        {
            get { return _Cyan ?? (_Cyan = SystemPen(0xFF00FFFF)); }
        }

        private static Pen _DarkBlue;
        public static Pen DarkBlue
        {
            get { return _DarkBlue ?? (_DarkBlue = SystemPen(0xFF00008B)); }
        }

        private static Pen _DarkCyan;
        public static Pen DarkCyan
        {
            get { return _DarkCyan ?? (_DarkCyan = SystemPen(0xFF008B8B)); }
        }

        private static Pen _DarkGoldenrod;
        public static Pen DarkGoldenrod
        {
            get { return _DarkGoldenrod ?? (_DarkGoldenrod = SystemPen(0xFFB8860B)); }
        }

        private static Pen _DarkGray;
        public static Pen DarkGray
        {
            get { return _DarkGray ?? (_DarkGray = SystemPen(0xFFA9A9A9)); }
        }

        private static Pen _DarkGreen;
        public static Pen DarkGreen
        {
            get { return _DarkGreen ?? (_DarkGreen = SystemPen(0xFF006400)); }
        }

        private static Pen _DarkKhaki;
        public static Pen DarkKhaki
        {
            get { return _DarkKhaki ?? (_DarkKhaki = SystemPen(0xFFBDB76B)); }
        }

        private static Pen _DarkMagenta;
        public static Pen DarkMagenta
        {
            get { return _DarkMagenta ?? (_DarkMagenta = SystemPen(0xFF8B008B)); }
        }

        private static Pen _DarkOliveGreen;
        public static Pen DarkOliveGreen
        {
            get { return _DarkOliveGreen ?? (_DarkOliveGreen = SystemPen(0xFF556B2F)); }
        }

        private static Pen _DarkOrange;
        public static Pen DarkOrange
        {
            get { return _DarkOrange ?? (_DarkOrange = SystemPen(0xFFFF8C00)); }
        }

        private static Pen _DarkOrchid;
        public static Pen DarkOrchid
        {
            get { return _DarkOrchid ?? (_DarkOrchid = SystemPen(0xFF9932CC)); }
        }

        private static Pen _DarkRed;
        public static Pen DarkRed
        {
            get { return _DarkRed ?? (_DarkRed = SystemPen(0xFF8B0000)); }
        }

        private static Pen _DarkSalmon;
        public static Pen DarkSalmon
        {
            get { return _DarkSalmon ?? (_DarkSalmon = SystemPen(0xFFE9967A)); }
        }

        private static Pen _DarkSeaGreen;
        public static Pen DarkSeaGreen
        {
            get { return _DarkSeaGreen ?? (_DarkSeaGreen = SystemPen(0xFF8FBC8B)); }
        }

        private static Pen _DarkSlateBlue;
        public static Pen DarkSlateBlue
        {
            get { return _DarkSlateBlue ?? (_DarkSlateBlue = SystemPen(0xFF483D8B)); }
        }

        private static Pen _DarkSlateGray;
        public static Pen DarkSlateGray
        {
            get { return _DarkSlateGray ?? (_DarkSlateGray = SystemPen(0xFF2F4F4F)); }
        }

        private static Pen _DarkTurquoise;
        public static Pen DarkTurquoise
        {
            get { return _DarkTurquoise ?? (_DarkTurquoise = SystemPen(0xFF00CED1)); }
        }

        private static Pen _DarkViolet;
        public static Pen DarkViolet
        {
            get { return _DarkViolet ?? (_DarkViolet = SystemPen(0xFF9400D3)); }
        }

        private static Pen _DeepPink;
        public static Pen DeepPink
        {
            get { return _DeepPink ?? (_DeepPink = SystemPen(0xFFFF1493)); }
        }

        private static Pen _DeepSkyBlue;
        public static Pen DeepSkyBlue
        {
            get { return _DeepSkyBlue ?? (_DeepSkyBlue = SystemPen(0xFF00BFFF)); }
        }

        private static Pen _DimGray;
        public static Pen DimGray
        {
            get { return _DimGray ?? (_DimGray = SystemPen(0xFF696969)); }
        }

        private static Pen _DodgerBlue;
        public static Pen DodgerBlue
        {
            get { return _DodgerBlue ?? (_DodgerBlue = SystemPen(0xFF1E90FF)); }
        }

        private static Pen _Firebrick;
        public static Pen Firebrick
        {
            get { return _Firebrick ?? (_Firebrick = SystemPen(0xFFB22222)); }
        }

        private static Pen _FloralWhite;
        public static Pen FloralWhite
        {
            get { return _FloralWhite ?? (_FloralWhite = SystemPen(0xFFFFFAF0)); }
        }

        private static Pen _ForestGreen;
        public static Pen ForestGreen
        {
            get { return _ForestGreen ?? (_ForestGreen = SystemPen(0xFF228B22)); }
        }

        private static Pen _Fuchsia;
        public static Pen Fuchsia
        {
            get { return _Fuchsia ?? (_Fuchsia = SystemPen(0xFFFF00FF)); }
        }

        private static Pen _Gainsboro;
        public static Pen Gainsboro
        {
            get { return _Gainsboro ?? (_Gainsboro = SystemPen(0xFFDCDCDC)); }
        }

        private static Pen _GhostWhite;
        public static Pen GhostWhite
        {
            get { return _GhostWhite ?? (_GhostWhite = SystemPen(0xFFF8F8FF)); }
        }

        private static Pen _Gold;
        public static Pen Gold
        {
            get { return _Gold ?? (_Gold = SystemPen(0xFFFFD700)); }
        }

        private static Pen _Goldenrod;
        public static Pen Goldenrod
        {
            get { return _Goldenrod ?? (_Goldenrod = SystemPen(0xFFDAA520)); }
        }

        private static Pen _Gray;
        public static Pen Gray
        {
            get { return _Gray ?? (_Gray = SystemPen(0xFF808080)); }
        }

        private static Pen _Green;
        public static Pen Green
        {
            get { return _Green ?? (_Green = SystemPen(0xFF008000)); }
        }

        private static Pen _GreenYellow;
        public static Pen GreenYellow
        {
            get { return _GreenYellow ?? (_GreenYellow = SystemPen(0xFFADFF2F)); }
        }

        private static Pen _Honeydew;
        public static Pen Honeydew
        {
            get { return _Honeydew ?? (_Honeydew = SystemPen(0xFFF0FFF0)); }
        }

        private static Pen _HotPink;
        public static Pen HotPink
        {
            get { return _HotPink ?? (_HotPink = SystemPen(0xFFFF69B4)); }
        }

        private static Pen _IndianRed;
        public static Pen IndianRed
        {
            get { return _IndianRed ?? (_IndianRed = SystemPen(0xFFCD5C5C)); }
        }

        private static Pen _Indigo;
        public static Pen Indigo
        {
            get { return _Indigo ?? (_Indigo = SystemPen(0xFF4B0082)); }
        }

        private static Pen _Ivory;
        public static Pen Ivory
        {
            get { return _Ivory ?? (_Ivory = SystemPen(0xFFFFFFF0)); }
        }

        private static Pen _Khaki;
        public static Pen Khaki
        {
            get { return _Khaki ?? (_Khaki = SystemPen(0xFFF0E68C)); }
        }

        private static Pen _Lavender;
        public static Pen Lavender
        {
            get { return _Lavender ?? (_Lavender = SystemPen(0xFFE6E6FA)); }
        }

        private static Pen _LavenderBlush;
        public static Pen LavenderBlush
        {
            get { return _LavenderBlush ?? (_LavenderBlush = SystemPen(0xFFFFF0F5)); }
        }

        private static Pen _LawnGreen;
        public static Pen LawnGreen
        {
            get { return _LawnGreen ?? (_LawnGreen = SystemPen(0xFF7CFC00)); }
        }

        private static Pen _LemonChiffon;
        public static Pen LemonChiffon
        {
            get { return _LemonChiffon ?? (_LemonChiffon = SystemPen(0xFFFFFACD)); }
        }

        private static Pen _LightBlue;
        public static Pen LightBlue
        {
            get { return _LightBlue ?? (_LightBlue = SystemPen(0xFFADD8E6)); }
        }

        private static Pen _LightCoral;
        public static Pen LightCoral
        {
            get { return _LightCoral ?? (_LightCoral = SystemPen(0xFFF08080)); }
        }

        private static Pen _LightCyan;
        public static Pen LightCyan
        {
            get { return _LightCyan ?? (_LightCyan = SystemPen(0xFFE0FFFF)); }
        }

        private static Pen _LightGoldenrodYellow;
        public static Pen LightGoldenrodYellow
        {
            get { return _LightGoldenrodYellow ?? (_LightGoldenrodYellow = SystemPen(0xFFFAFAD2)); }
        }

        private static Pen _LightGreen;
        public static Pen LightGreen
        {
            get { return _LightGreen ?? (_LightGreen = SystemPen(0xFF90EE90)); }
        }

        private static Pen _LightGray;
        public static Pen LightGray
        {
            get { return _LightGray ?? (_LightGray = SystemPen(0xFFD3D3D3)); }
        }

        private static Pen _LightPink;
        public static Pen LightPink
        {
            get { return _LightPink ?? (_LightPink = SystemPen(0xFFFFB6C1)); }
        }

        private static Pen _LightSalmon;
        public static Pen LightSalmon
        {
            get { return _LightSalmon ?? (_LightSalmon = SystemPen(0xFFFFA07A)); }
        }

        private static Pen _LightSeaGreen;
        public static Pen LightSeaGreen
        {
            get { return _LightSeaGreen ?? (_LightSeaGreen = SystemPen(0xFF20B2AA)); }
        }

        private static Pen _LightSkyBlue;
        public static Pen LightSkyBlue
        {
            get { return _LightSkyBlue ?? (_LightSkyBlue = SystemPen(0xFF87CEFA)); }
        }

        private static Pen _LightSlateGray;
        public static Pen LightSlateGray
        {
            get { return _LightSlateGray ?? (_LightSlateGray = SystemPen(0xFF778899)); }
        }

        private static Pen _LightSteelBlue;
        public static Pen LightSteelBlue
        {
            get { return _LightSteelBlue ?? (_LightSteelBlue = SystemPen(0xFFB0C4DE)); }
        }

        private static Pen _LightYellow;
        public static Pen LightYellow
        {
            get { return _LightYellow ?? (_LightYellow = SystemPen(0xFFFFFFE0)); }
        }

        private static Pen _Lime;
        public static Pen Lime
        {
            get { return _Lime ?? (_Lime = SystemPen(0xFF00FF00)); }
        }

        private static Pen _LimeGreen;
        public static Pen LimeGreen
        {
            get { return _LimeGreen ?? (_LimeGreen = SystemPen(0xFF32CD32)); }
        }

        private static Pen _Linen;
        public static Pen Linen
        {
            get { return _Linen ?? (_Linen = SystemPen(0xFFFAF0E6)); }
        }

        private static Pen _Magenta;
        public static Pen Magenta
        {
            get { return _Magenta ?? (_Magenta = SystemPen(0xFFFF00FF)); }
        }

        private static Pen _Maroon;
        public static Pen Maroon
        {
            get { return _Maroon ?? (_Maroon = SystemPen(0xFF800000)); }
        }

        private static Pen _MediumAquamarine;
        public static Pen MediumAquamarine
        {
            get { return _MediumAquamarine ?? (_MediumAquamarine = SystemPen(0xFF66CDAA)); }
        }

        private static Pen _MediumBlue;
        public static Pen MediumBlue
        {
            get { return _MediumBlue ?? (_MediumBlue = SystemPen(0xFF0000CD)); }
        }

        private static Pen _MediumOrchid;
        public static Pen MediumOrchid
        {
            get { return _MediumOrchid ?? (_MediumOrchid = SystemPen(0xFFBA55D3)); }
        }

        private static Pen _MediumPurple;
        public static Pen MediumPurple
        {
            get { return _MediumPurple ?? (_MediumPurple = SystemPen(0xFF9370DB)); }
        }

        private static Pen _MediumSeaGreen;
        public static Pen MediumSeaGreen
        {
            get { return _MediumSeaGreen ?? (_MediumSeaGreen = SystemPen(0xFF3CB371)); }
        }

        private static Pen _MediumSlateBlue;
        public static Pen MediumSlateBlue
        {
            get { return _MediumSlateBlue ?? (_MediumSlateBlue = SystemPen(0xFF7B68EE)); }
        }

        private static Pen _MediumSpringGreen;
        public static Pen MediumSpringGreen
        {
            get { return _MediumSpringGreen ?? (_MediumSpringGreen = SystemPen(0xFF00FA9A)); }
        }

        private static Pen _MediumTurquoise;
        public static Pen MediumTurquoise
        {
            get { return _MediumTurquoise ?? (_MediumTurquoise = SystemPen(0xFF48D1CC)); }
        }

        private static Pen _MediumVioletRed;
        public static Pen MediumVioletRed
        {
            get { return _MediumVioletRed ?? (_MediumVioletRed = SystemPen(0xFFC71585)); }
        }

        private static Pen _MidnightBlue;
        public static Pen MidnightBlue
        {
            get { return _MidnightBlue ?? (_MidnightBlue = SystemPen(0xFF191970)); }
        }

        private static Pen _MintCream;
        public static Pen MintCream
        {
            get { return _MintCream ?? (_MintCream = SystemPen(0xFFF5FFFA)); }
        }

        private static Pen _MistyRose;
        public static Pen MistyRose
        {
            get { return _MistyRose ?? (_MistyRose = SystemPen(0xFFFFE4E1)); }
        }

        private static Pen _Moccasin;
        public static Pen Moccasin
        {
            get { return _Moccasin ?? (_Moccasin = SystemPen(0xFFFFE4B5)); }
        }

        private static Pen _NavajoWhite;
        public static Pen NavajoWhite
        {
            get { return _NavajoWhite ?? (_NavajoWhite = SystemPen(0xFFFFDEAD)); }
        }

        private static Pen _Navy;
        public static Pen Navy
        {
            get { return _Navy ?? (_Navy = SystemPen(0xFF000080)); }
        }

        private static Pen _OldLace;
        public static Pen OldLace
        {
            get { return _OldLace ?? (_OldLace = SystemPen(0xFFFDF5E6)); }
        }

        private static Pen _Olive;
        public static Pen Olive
        {
            get { return _Olive ?? (_Olive = SystemPen(0xFF808000)); }
        }

        private static Pen _OliveDrab;
        public static Pen OliveDrab
        {
            get { return _OliveDrab ?? (_OliveDrab = SystemPen(0xFF6B8E23)); }
        }

        private static Pen _Orange;
        public static Pen Orange
        {
            get { return _Orange ?? (_Orange = SystemPen(0xFFFFA500)); }
        }

        private static Pen _OrangeRed;
        public static Pen OrangeRed
        {
            get { return _OrangeRed ?? (_OrangeRed = SystemPen(0xFFFF4500)); }
        }

        private static Pen _Orchid;
        public static Pen Orchid
        {
            get { return _Orchid ?? (_Orchid = SystemPen(0xFFDA70D6)); }
        }

        private static Pen _PaleGoldenrod;
        public static Pen PaleGoldenrod
        {
            get { return _PaleGoldenrod ?? (_PaleGoldenrod = SystemPen(0xFFEEE8AA)); }
        }

        private static Pen _PaleGreen;
        public static Pen PaleGreen
        {
            get { return _PaleGreen ?? (_PaleGreen = SystemPen(0xFF98FB98)); }
        }

        private static Pen _PaleTurquoise;
        public static Pen PaleTurquoise
        {
            get { return _PaleTurquoise ?? (_PaleTurquoise = SystemPen(0xFFAFEEEE)); }
        }

        private static Pen _PaleVioletRed;
        public static Pen PaleVioletRed
        {
            get { return _PaleVioletRed ?? (_PaleVioletRed = SystemPen(0xFFDB7093)); }
        }

        private static Pen _PapayaWhip;
        public static Pen PapayaWhip
        {
            get { return _PapayaWhip ?? (_PapayaWhip = SystemPen(0xFFFFEFD5)); }
        }

        private static Pen _PeachPuff;
        public static Pen PeachPuff
        {
            get { return _PeachPuff ?? (_PeachPuff = SystemPen(0xFFFFDAB9)); }
        }

        private static Pen _Peru;
        public static Pen Peru
        {
            get { return _Peru ?? (_Peru = SystemPen(0xFFCD853F)); }
        }

        private static Pen _Pink;
        public static Pen Pink
        {
            get { return _Pink ?? (_Pink = SystemPen(0xFFFFC0CB)); }
        }

        private static Pen _Plum;
        public static Pen Plum
        {
            get { return _Plum ?? (_Plum = SystemPen(0xFFDDA0DD)); }
        }

        private static Pen _PowderBlue;
        public static Pen PowderBlue
        {
            get { return _PowderBlue ?? (_PowderBlue = SystemPen(0xFFB0E0E6)); }
        }

        private static Pen _Purple;
        public static Pen Purple
        {
            get { return _Purple ?? (_Purple = SystemPen(0xFF800080)); }
        }

        private static Pen _Red;
        public static Pen Red
        {
            get { return _Red ?? (_Red = SystemPen(0xFFFF0000)); }
        }

        private static Pen _RosyBrown;
        public static Pen RosyBrown
        {
            get { return _RosyBrown ?? (_RosyBrown = SystemPen(0xFFBC8F8F)); }
        }

        private static Pen _RoyalBlue;
        public static Pen RoyalBlue
        {
            get { return _RoyalBlue ?? (_RoyalBlue = SystemPen(0xFF4169E1)); }
        }

        private static Pen _SaddleBrown;
        public static Pen SaddleBrown
        {
            get { return _SaddleBrown ?? (_SaddleBrown = SystemPen(0xFF8B4513)); }
        }

        private static Pen _Salmon;
        public static Pen Salmon
        {
            get { return _Salmon ?? (_Salmon = SystemPen(0xFFFA8072)); }
        }

        private static Pen _SandyBrown;
        public static Pen SandyBrown
        {
            get { return _SandyBrown ?? (_SandyBrown = SystemPen(0xFFF4A460)); }
        }

        private static Pen _SeaGreen;
        public static Pen SeaGreen
        {
            get { return _SeaGreen ?? (_SeaGreen = SystemPen(0xFF2E8B57)); }
        }

        private static Pen _SeaShell;
        public static Pen SeaShell
        {
            get { return _SeaShell ?? (_SeaShell = SystemPen(0xFFFFF5EE)); }
        }

        private static Pen _Sienna;
        public static Pen Sienna
        {
            get { return _Sienna ?? (_Sienna = SystemPen(0xFFA0522D)); }
        }

        private static Pen _Silver;
        public static Pen Silver
        {
            get { return _Silver ?? (_Silver = SystemPen(0xFFC0C0C0)); }
        }

        private static Pen _SkyBlue;
        public static Pen SkyBlue
        {
            get { return _SkyBlue ?? (_SkyBlue = SystemPen(0xFF87CEEB)); }
        }

        private static Pen _SlateBlue;
        public static Pen SlateBlue
        {
            get { return _SlateBlue ?? (_SlateBlue = SystemPen(0xFF6A5ACD)); }
        }

        private static Pen _SlateGray;
        public static Pen SlateGray
        {
            get { return _SlateGray ?? (_SlateGray = SystemPen(0xFF708090)); }
        }

        private static Pen _Snow;
        public static Pen Snow
        {
            get { return _Snow ?? (_Snow = SystemPen(0xFFFFFAFA)); }
        }

        private static Pen _SpringGreen;
        public static Pen SpringGreen
        {
            get { return _SpringGreen ?? (_SpringGreen = SystemPen(0xFF00FF7F)); }
        }

        private static Pen _SteelBlue;
        public static Pen SteelBlue
        {
            get { return _SteelBlue ?? (_SteelBlue = SystemPen(0xFF4682B4)); }
        }

        private static Pen _Tan;
        public static Pen Tan
        {
            get { return _Tan ?? (_Tan = SystemPen(0xFFD2B48C)); }
        }

        private static Pen _Teal;
        public static Pen Teal
        {
            get { return _Teal ?? (_Teal = SystemPen(0xFF008080)); }
        }

        private static Pen _Thistle;
        public static Pen Thistle
        {
            get { return _Thistle ?? (_Thistle = SystemPen(0xFFD8BFD8)); }
        }

        private static Pen _Tomato;
        public static Pen Tomato
        {
            get { return _Tomato ?? (_Tomato = SystemPen(0xFFFF6347)); }
        }

        private static Pen _Turquoise;
        public static Pen Turquoise
        {
            get { return _Turquoise ?? (_Turquoise = SystemPen(0xFF40E0D0)); }
        }

        private static Pen _Violet;
        public static Pen Violet
        {
            get { return _Violet ?? (_Violet = SystemPen(0xFFEE82EE)); }
        }

        private static Pen _Wheat;
        public static Pen Wheat
        {
            get { return _Wheat ?? (_Wheat = SystemPen(0xFFF5DEB3)); }
        }

        private static Pen _White;
        public static Pen White
        {
            get { return _White ?? (_White = SystemPen(0xFFFFFFFF)); }
        }

        private static Pen _WhiteSmoke;
        public static Pen WhiteSmoke
        {
            get { return _WhiteSmoke ?? (_WhiteSmoke = SystemPen(0xFFF5F5F5)); }
        }

        private static Pen _Yellow;
        public static Pen Yellow
        {
            get { return _Yellow ?? (_Yellow = SystemPen(0xFFFFFF00)); }
        }

        private static Pen _YellowGreen;
        public static Pen YellowGreen
        {
            get { return _YellowGreen ?? (_YellowGreen = SystemPen(0xFF9ACD32)); }
        }

        private static Pen _ActiveBorder;
        public static Pen ActiveBorder
        {
            get { return _ActiveBorder ?? (_ActiveBorder = SystemPen(0xFFB4B4B4)); }
        }

        private static Pen _ActiveCaption;
        public static Pen ActiveCaption
        {
            get { return _ActiveCaption ?? (_ActiveCaption = SystemPen(0xFF99B4D1)); }
        }

        private static Pen _ActiveCaptionText;
        public static Pen ActiveCaptionText
        {
            get { return _ActiveCaptionText ?? (_ActiveCaptionText = SystemPen(0xFF000000)); }
        }

        private static Pen _AppWorkspace;
        public static Pen AppWorkspace
        {
            get { return _AppWorkspace ?? (_AppWorkspace = SystemPen(0xFFABABAB)); }
        }

        private static Pen _ButtonFace;
        public static Pen ButtonFace
        {
            get { return _ButtonFace ?? (_ButtonFace = SystemPen(0xFFF0F0F0)); }
        }

        private static Pen _ButtonHighlight;
        public static Pen ButtonHighlight
        {
            get { return _ButtonHighlight ?? (_ButtonHighlight = SystemPen(0xFFFFFFFF)); }
        }

        private static Pen _ButtonShadow;
        public static Pen ButtonShadow
        {
            get { return _ButtonShadow ?? (_ButtonShadow = SystemPen(0xFFA0A0A0)); }
        }

        private static Pen _Control;
        public static Pen Control
        {
            get { return _Control ?? (_Control = SystemPen(0xFFF0F0F0)); }
        }

        private static Pen _ControlLightLight;
        public static Pen ControlLightLight
        {
            get { return _ControlLightLight ?? (_ControlLightLight = SystemPen(0xFFFFFFFF)); }
        }

        private static Pen _ControlLight;
        public static Pen ControlLight
        {
            get { return _ControlLight ?? (_ControlLight = SystemPen(0xFFE3E3E3)); }
        }

        private static Pen _ControlDark;
        public static Pen ControlDark
        {
            get { return _ControlDark ?? (_ControlDark = SystemPen(0xFFA0A0A0)); }
        }

        private static Pen _ControlDarkDark;
        public static Pen ControlDarkDark
        {
            get { return _ControlDarkDark ?? (_ControlDarkDark = SystemPen(0xFF696969)); }
        }

        private static Pen _ControlText;
        public static Pen ControlText
        {
            get { return _ControlText ?? (_ControlText = SystemPen(0xFF000000)); }
        }

        private static Pen _Desktop;
        public static Pen Desktop
        {
            get { return _Desktop ?? (_Desktop = SystemPen(0xFF000000)); }
        }

        private static Pen _GradientActiveCaption;
        public static Pen GradientActiveCaption
        {
            get { return _GradientActiveCaption ?? (_GradientActiveCaption = SystemPen(0xFFB9D1EA)); }
        }

        private static Pen _GradientInactiveCaption;
        public static Pen GradientInactiveCaption
        {
            get { return _GradientInactiveCaption ?? (_GradientInactiveCaption = SystemPen(0xFFD7E4F2)); }
        }

        private static Pen _GrayText;
        public static Pen GrayText
        {
            get { return _GrayText ?? (_GrayText = SystemPen(0xFF6D6D6D)); }
        }

        private static Pen _Highlight;
        public static Pen Highlight
        {
            get { return _Highlight ?? (_Highlight = SystemPen(0xFF0078D7)); }
        }

        private static Pen _HighlightText;
        public static Pen HighlightText
        {
            get { return _HighlightText ?? (_HighlightText = SystemPen(0xFFFFFFFF)); }
        }

        private static Pen _HotTrack;
        public static Pen HotTrack
        {
            get { return _HotTrack ?? (_HotTrack = SystemPen(0xFF0066CC)); }
        }

        private static Pen _InactiveCaption;
        public static Pen InactiveCaption
        {
            get { return _InactiveCaption ?? (_InactiveCaption = SystemPen(0xFFBFCDDB)); }
        }

        private static Pen _InactiveBorder;
        public static Pen InactiveBorder
        {
            get { return _InactiveBorder ?? (_InactiveBorder = SystemPen(0xFFF4F7FC)); }
        }

        private static Pen _InactiveCaptionText;
        public static Pen InactiveCaptionText
        {
            get { return _InactiveCaptionText ?? (_InactiveCaptionText = SystemPen(0xFF000000)); }
        }

        private static Pen _Info;
        public static Pen Info
        {
            get { return _Info ?? (_Info = SystemPen(0xFFFFFFE1)); }
        }

        private static Pen _InfoText;
        public static Pen InfoText
        {
            get { return _InfoText ?? (_InfoText = SystemPen(0xFF000000)); }
        }

        private static Pen _Menu;
        public static Pen Menu
        {
            get { return _Menu ?? (_Menu = SystemPen(0xFFF0F0F0)); }
        }

        private static Pen _MenuBar;
        public static Pen MenuBar
        {
            get { return _MenuBar ?? (_MenuBar = SystemPen(0xFFF0F0F0)); }
        }

        private static Pen _MenuHighlight;
        public static Pen MenuHighlight
        {
            get { return _MenuHighlight ?? (_MenuHighlight = SystemPen(0xFF3399FF)); }
        }

        private static Pen _MenuText;
        public static Pen MenuText
        {
            get { return _MenuText ?? (_MenuText = SystemPen(0xFF000000)); }
        }

        private static Pen _ScrollBar;
        public static Pen ScrollBar
        {
            get { return _ScrollBar ?? (_ScrollBar = SystemPen(0xFFC8C8C8)); }
        }

        private static Pen _Window;
        public static Pen Window
        {
            get { return _Window ?? (_Window = SystemPen(0xFFFFFFFF)); }
        }

        private static Pen _WindowFrame;
        public static Pen WindowFrame
        {
            get { return _WindowFrame ?? (_WindowFrame = SystemPen(0xFF646464)); }
        }

        private static Pen _WindowText;
        public static Pen WindowText
        {
            get { return _WindowText ?? (_WindowText = SystemPen(0xFF000000)); }
        }
    }
}