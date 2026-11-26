using SkiaSharp;

namespace System.Drawing
{
    public class Brushes
    {
        private static Brush _Transparent;
        public static Brush Transparent { get { return _Transparent ?? (_Transparent = new SolidBrush(0x00FFFFFF)); }}

        private static Brush _AliceBlue;
        public static Brush AliceBlue { get { return _AliceBlue ?? (_AliceBlue = new SolidBrush(0xFFF0F8FF)); }}

        private static Brush _AntiqueWhite;
        public static Brush AntiqueWhite { get { return _AntiqueWhite ?? (_AntiqueWhite = new SolidBrush(0xFFFAEBD7)); }}

        private static Brush _Aqua;
        public static Brush Aqua { get { return _Aqua ?? (_Aqua = new SolidBrush(0xFF00FFFF)); }}

        private static Brush _Aquamarine;
        public static Brush Aquamarine { get { return _Aquamarine ?? (_Aquamarine = new SolidBrush(0xFF7FFFD4)); }}

        private static Brush _Azure;
        public static Brush Azure { get { return _Azure ?? (_Azure = new SolidBrush(0xFFF0FFFF)); }}

        private static Brush _Beige;
        public static Brush Beige { get { return _Beige ?? (_Beige = new SolidBrush(0xFFF5F5DC)); }}

        private static Brush _Bisque;
        public static Brush Bisque { get { return _Bisque ?? (_Bisque = new SolidBrush(0xFFFFE4C4)); }}

        private static Brush _Black;
        public static Brush Black { get { return _Black ?? (_Black = new SolidBrush(0xFF000000)); }}

        private static Brush _BlanchedAlmond;
        public static Brush BlanchedAlmond { get { return _BlanchedAlmond ?? (_BlanchedAlmond = new SolidBrush(0xFFFFEBCD)); }}

        private static Brush _Blue;
        public static Brush Blue { get { return _Blue ?? (_Blue = new SolidBrush(0xFF0000FF)); }}

        private static Brush _BlueViolet;
        public static Brush BlueViolet { get { return _BlueViolet ?? (_BlueViolet = new SolidBrush(0xFF8A2BE2)); }}

        private static Brush _Brown;
        public static Brush Brown { get { return _Brown ?? (_Brown = new SolidBrush(0xFFA52A2A)); }}

        private static Brush _BurlyWood;
        public static Brush BurlyWood { get { return _BurlyWood ?? (_BurlyWood = new SolidBrush(0xFFDEB887)); }}

        private static Brush _CadetBlue;
        public static Brush CadetBlue { get { return _CadetBlue ?? (_CadetBlue = new SolidBrush(0xFF5F9EA0)); }}

        private static Brush _Chartreuse;
        public static Brush Chartreuse { get { return _Chartreuse ?? (_Chartreuse = new SolidBrush(0xFF7FFF00)); }}

        private static Brush _Chocolate;
        public static Brush Chocolate { get { return _Chocolate ?? (_Chocolate = new SolidBrush(0xFFD2691E)); }}

        private static Brush _Coral;
        public static Brush Coral { get { return _Coral ?? (_Coral = new SolidBrush(0xFFFF7F50)); }}

        private static Brush _CornflowerBlue;
        public static Brush CornflowerBlue { get { return _CornflowerBlue ?? (_CornflowerBlue = new SolidBrush(0xFF6495ED)); }}

        private static Brush _Cornsilk;
        public static Brush Cornsilk { get { return _Cornsilk ?? (_Cornsilk = new SolidBrush(0xFFFFF8DC)); }}

        private static Brush _Crimson;
        public static Brush Crimson { get { return _Crimson ?? (_Crimson = new SolidBrush(0xFFDC143C)); }}

        private static Brush _Cyan;
        public static Brush Cyan { get { return _Cyan ?? (_Cyan = new SolidBrush(0xFF00FFFF)); }}

        private static Brush _DarkBlue;
        public static Brush DarkBlue { get { return _DarkBlue ?? (_DarkBlue = new SolidBrush(0xFF00008B)); }}

        private static Brush _DarkCyan;
        public static Brush DarkCyan { get { return _DarkCyan ?? (_DarkCyan = new SolidBrush(0xFF008B8B)); }}

        private static Brush _DarkGoldenrod;
        public static Brush DarkGoldenrod { get { return _DarkGoldenrod ?? (_DarkGoldenrod = new SolidBrush(0xFFB8860B)); }}

        private static Brush _DarkGray;
        public static Brush DarkGray { get { return _DarkGray ?? (_DarkGray = new SolidBrush(0xFFA9A9A9)); }}

        private static Brush _DarkGreen;
        public static Brush DarkGreen { get { return _DarkGreen ?? (_DarkGreen = new SolidBrush(0xFF006400)); }}

        private static Brush _DarkKhaki;
        public static Brush DarkKhaki { get { return _DarkKhaki ?? (_DarkKhaki = new SolidBrush(0xFFBDB76B)); }}

        private static Brush _DarkMagenta;
        public static Brush DarkMagenta { get { return _DarkMagenta ?? (_DarkMagenta = new SolidBrush(0xFF8B008B)); }}

        private static Brush _DarkOliveGreen;
        public static Brush DarkOliveGreen { get { return _DarkOliveGreen ?? (_DarkOliveGreen = new SolidBrush(0xFF556B2F)); }}

        private static Brush _DarkOrange;
        public static Brush DarkOrange { get { return _DarkOrange ?? (_DarkOrange = new SolidBrush(0xFFFF8C00)); }}

        private static Brush _DarkOrchid;
        public static Brush DarkOrchid { get { return _DarkOrchid ?? (_DarkOrchid = new SolidBrush(0xFF9932CC)); }}

        private static Brush _DarkRed;
        public static Brush DarkRed { get { return _DarkRed ?? (_DarkRed = new SolidBrush(0xFF8B0000)); }}

        private static Brush _DarkSalmon;
        public static Brush DarkSalmon { get { return _DarkSalmon ?? (_DarkSalmon = new SolidBrush(0xFFE9967A)); }}

        private static Brush _DarkSeaGreen;
        public static Brush DarkSeaGreen { get { return _DarkSeaGreen ?? (_DarkSeaGreen = new SolidBrush(0xFF8FBC8B)); }}

        private static Brush _DarkSlateBlue;
        public static Brush DarkSlateBlue { get { return _DarkSlateBlue ?? (_DarkSlateBlue = new SolidBrush(0xFF483D8B)); }}

        private static Brush _DarkSlateGray;
        public static Brush DarkSlateGray { get { return _DarkSlateGray ?? (_DarkSlateGray = new SolidBrush(0xFF2F4F4F)); }}

        private static Brush _DarkTurquoise;
        public static Brush DarkTurquoise { get { return _DarkTurquoise ?? (_DarkTurquoise = new SolidBrush(0xFF00CED1)); }}

        private static Brush _DarkViolet;
        public static Brush DarkViolet { get { return _DarkViolet ?? (_DarkViolet = new SolidBrush(0xFF9400D3)); }}

        private static Brush _DeepPink;
        public static Brush DeepPink { get { return _DeepPink ?? (_DeepPink = new SolidBrush(0xFFFF1493)); }}

        private static Brush _DeepSkyBlue;
        public static Brush DeepSkyBlue { get { return _DeepSkyBlue ?? (_DeepSkyBlue = new SolidBrush(0xFF00BFFF)); }}

        private static Brush _DimGray;
        public static Brush DimGray { get { return _DimGray ?? (_DimGray = new SolidBrush(0xFF696969)); }}

        private static Brush _DodgerBlue;
        public static Brush DodgerBlue { get { return _DodgerBlue ?? (_DodgerBlue = new SolidBrush(0xFF1E90FF)); }}

        private static Brush _Firebrick;
        public static Brush Firebrick { get { return _Firebrick ?? (_Firebrick = new SolidBrush(0xFFB22222)); }}

        private static Brush _FloralWhite;
        public static Brush FloralWhite { get { return _FloralWhite ?? (_FloralWhite = new SolidBrush(0xFFFFFAF0)); }}

        private static Brush _ForestGreen;
        public static Brush ForestGreen { get { return _ForestGreen ?? (_ForestGreen = new SolidBrush(0xFF228B22)); }}

        private static Brush _Fuchsia;
        public static Brush Fuchsia { get { return _Fuchsia ?? (_Fuchsia = new SolidBrush(0xFFFF00FF)); }}

        private static Brush _Gainsboro;
        public static Brush Gainsboro { get { return _Gainsboro ?? (_Gainsboro = new SolidBrush(0xFFDCDCDC)); }}

        private static Brush _GhostWhite;
        public static Brush GhostWhite { get { return _GhostWhite ?? (_GhostWhite = new SolidBrush(0xFFF8F8FF)); }}

        private static Brush _Gold;
        public static Brush Gold { get { return _Gold ?? (_Gold = new SolidBrush(0xFFFFD700)); }}

        private static Brush _Goldenrod;
        public static Brush Goldenrod { get { return _Goldenrod ?? (_Goldenrod = new SolidBrush(0xFFDAA520)); }}

        private static Brush _Gray;
        public static Brush Gray { get { return _Gray ?? (_Gray = new SolidBrush(0xFF808080)); }}

        private static Brush _Green;
        public static Brush Green { get { return _Green ?? (_Green = new SolidBrush(0xFF008000)); }}

        private static Brush _GreenYellow;
        public static Brush GreenYellow { get { return _GreenYellow ?? (_GreenYellow = new SolidBrush(0xFFADFF2F)); }}

        private static Brush _Honeydew;
        public static Brush Honeydew { get { return _Honeydew ?? (_Honeydew = new SolidBrush(0xFFF0FFF0)); }}

        private static Brush _HotPink;
        public static Brush HotPink { get { return _HotPink ?? (_HotPink = new SolidBrush(0xFFFF69B4)); }}

        private static Brush _IndianRed;
        public static Brush IndianRed { get { return _IndianRed ?? (_IndianRed = new SolidBrush(0xFFCD5C5C)); }}

        private static Brush _Indigo;
        public static Brush Indigo { get { return _Indigo ?? (_Indigo = new SolidBrush(0xFF4B0082)); }}

        private static Brush _Ivory;
        public static Brush Ivory { get { return _Ivory ?? (_Ivory = new SolidBrush(0xFFFFFFF0)); }}

        private static Brush _Khaki;
        public static Brush Khaki { get { return _Khaki ?? (_Khaki = new SolidBrush(0xFFF0E68C)); }}

        private static Brush _Lavender;
        public static Brush Lavender { get { return _Lavender ?? (_Lavender = new SolidBrush(0xFFE6E6FA)); }}

        private static Brush _LavenderBlush;
        public static Brush LavenderBlush { get { return _LavenderBlush ?? (_LavenderBlush = new SolidBrush(0xFFFFF0F5)); }}

        private static Brush _LawnGreen;
        public static Brush LawnGreen { get { return _LawnGreen ?? (_LawnGreen = new SolidBrush(0xFF7CFC00)); }}

        private static Brush _LemonChiffon;
        public static Brush LemonChiffon { get { return _LemonChiffon ?? (_LemonChiffon = new SolidBrush(0xFFFFFACD)); }}

        private static Brush _LightBlue;
        public static Brush LightBlue { get { return _LightBlue ?? (_LightBlue = new SolidBrush(0xFFADD8E6)); }}

        private static Brush _LightCoral;
        public static Brush LightCoral { get { return _LightCoral ?? (_LightCoral = new SolidBrush(0xFFF08080)); }}

        private static Brush _LightCyan;
        public static Brush LightCyan { get { return _LightCyan ?? (_LightCyan = new SolidBrush(0xFFE0FFFF)); }}

        private static Brush _LightGoldenrodYellow;
        public static Brush LightGoldenrodYellow { get { return _LightGoldenrodYellow ?? (_LightGoldenrodYellow = new SolidBrush(0xFFFAFAD2)); }}

        private static Brush _LightGreen;
        public static Brush LightGreen { get { return _LightGreen ?? (_LightGreen = new SolidBrush(0xFF90EE90)); }}

        private static Brush _LightGray;
        public static Brush LightGray { get { return _LightGray ?? (_LightGray = new SolidBrush(0xFFD3D3D3)); }}

        private static Brush _LightPink;
        public static Brush LightPink { get { return _LightPink ?? (_LightPink = new SolidBrush(0xFFFFB6C1)); }}

        private static Brush _LightSalmon;
        public static Brush LightSalmon { get { return _LightSalmon ?? (_LightSalmon = new SolidBrush(0xFFFFA07A)); }}

        private static Brush _LightSeaGreen;
        public static Brush LightSeaGreen { get { return _LightSeaGreen ?? (_LightSeaGreen = new SolidBrush(0xFF20B2AA)); }}

        private static Brush _LightSkyBlue;
        public static Brush LightSkyBlue { get { return _LightSkyBlue ?? (_LightSkyBlue = new SolidBrush(0xFF87CEFA)); }}

        private static Brush _LightSlateGray;
        public static Brush LightSlateGray { get { return _LightSlateGray ?? (_LightSlateGray = new SolidBrush(0xFF778899)); }}

        private static Brush _LightSteelBlue;
        public static Brush LightSteelBlue { get { return _LightSteelBlue ?? (_LightSteelBlue = new SolidBrush(0xFFB0C4DE)); }}

        private static Brush _LightYellow;
        public static Brush LightYellow { get { return _LightYellow ?? (_LightYellow = new SolidBrush(0xFFFFFFE0)); }}

        private static Brush _Lime;
        public static Brush Lime { get { return _Lime ?? (_Lime = new SolidBrush(0xFF00FF00)); }}

        private static Brush _LimeGreen;
        public static Brush LimeGreen { get { return _LimeGreen ?? (_LimeGreen = new SolidBrush(0xFF32CD32)); }}

        private static Brush _Linen;
        public static Brush Linen { get { return _Linen ?? (_Linen = new SolidBrush(0xFFFAF0E6)); }}

        private static Brush _Magenta;
        public static Brush Magenta { get { return _Magenta ?? (_Magenta = new SolidBrush(0xFFFF00FF)); }}

        private static Brush _Maroon;
        public static Brush Maroon { get { return _Maroon ?? (_Maroon = new SolidBrush(0xFF800000)); }}

        private static Brush _MediumAquamarine;
        public static Brush MediumAquamarine { get { return _MediumAquamarine ?? (_MediumAquamarine = new SolidBrush(0xFF66CDAA)); }}

        private static Brush _MediumBlue;
        public static Brush MediumBlue { get { return _MediumBlue ?? (_MediumBlue = new SolidBrush(0xFF0000CD)); }}

        private static Brush _MediumOrchid;
        public static Brush MediumOrchid { get { return _MediumOrchid ?? (_MediumOrchid = new SolidBrush(0xFFBA55D3)); }}

        private static Brush _MediumPurple;
        public static Brush MediumPurple { get { return _MediumPurple ?? (_MediumPurple = new SolidBrush(0xFF9370DB)); }}

        private static Brush _MediumSeaGreen;
        public static Brush MediumSeaGreen { get { return _MediumSeaGreen ?? (_MediumSeaGreen = new SolidBrush(0xFF3CB371)); }}

        private static Brush _MediumSlateBlue;
        public static Brush MediumSlateBlue { get { return _MediumSlateBlue ?? (_MediumSlateBlue = new SolidBrush(0xFF7B68EE)); }}

        private static Brush _MediumSpringGreen;
        public static Brush MediumSpringGreen { get { return _MediumSpringGreen ?? (_MediumSpringGreen = new SolidBrush(0xFF00FA9A)); }}

        private static Brush _MediumTurquoise;
        public static Brush MediumTurquoise { get { return _MediumTurquoise ?? (_MediumTurquoise = new SolidBrush(0xFF48D1CC)); }}

        private static Brush _MediumVioletRed;
        public static Brush MediumVioletRed { get { return _MediumVioletRed ?? (_MediumVioletRed = new SolidBrush(0xFFC71585)); }}

        private static Brush _MidnightBlue;
        public static Brush MidnightBlue { get { return _MidnightBlue ?? (_MidnightBlue = new SolidBrush(0xFF191970)); }}

        private static Brush _MintCream;
        public static Brush MintCream { get { return _MintCream ?? (_MintCream = new SolidBrush(0xFFF5FFFA)); }}

        private static Brush _MistyRose;
        public static Brush MistyRose { get { return _MistyRose ?? (_MistyRose = new SolidBrush(0xFFFFE4E1)); }}

        private static Brush _Moccasin;
        public static Brush Moccasin { get { return _Moccasin ?? (_Moccasin = new SolidBrush(0xFFFFE4B5)); }}

        private static Brush _NavajoWhite;
        public static Brush NavajoWhite { get { return _NavajoWhite ?? (_NavajoWhite = new SolidBrush(0xFFFFDEAD)); }}

        private static Brush _Navy;
        public static Brush Navy { get { return _Navy ?? (_Navy = new SolidBrush(0xFF000080)); }}

        private static Brush _OldLace;
        public static Brush OldLace { get { return _OldLace ?? (_OldLace = new SolidBrush(0xFFFDF5E6)); }}

        private static Brush _Olive;
        public static Brush Olive { get { return _Olive ?? (_Olive = new SolidBrush(0xFF808000)); }}

        private static Brush _OliveDrab;
        public static Brush OliveDrab { get { return _OliveDrab ?? (_OliveDrab = new SolidBrush(0xFF6B8E23)); }}

        private static Brush _Orange;
        public static Brush Orange { get { return _Orange ?? (_Orange = new SolidBrush(0xFFFFA500)); }}

        private static Brush _OrangeRed;
        public static Brush OrangeRed { get { return _OrangeRed ?? (_OrangeRed = new SolidBrush(0xFFFF4500)); }}

        private static Brush _Orchid;
        public static Brush Orchid { get { return _Orchid ?? (_Orchid = new SolidBrush(0xFFDA70D6)); }}

        private static Brush _PaleGoldenrod;
        public static Brush PaleGoldenrod { get { return _PaleGoldenrod ?? (_PaleGoldenrod = new SolidBrush(0xFFEEE8AA)); }}

        private static Brush _PaleGreen;
        public static Brush PaleGreen { get { return _PaleGreen ?? (_PaleGreen = new SolidBrush(0xFF98FB98)); }}

        private static Brush _PaleTurquoise;
        public static Brush PaleTurquoise { get { return _PaleTurquoise ?? (_PaleTurquoise = new SolidBrush(0xFFAFEEEE)); }}

        private static Brush _PaleVioletRed;
        public static Brush PaleVioletRed { get { return _PaleVioletRed ?? (_PaleVioletRed = new SolidBrush(0xFFDB7093)); }}

        private static Brush _PapayaWhip;
        public static Brush PapayaWhip { get { return _PapayaWhip ?? (_PapayaWhip = new SolidBrush(0xFFFFEFD5)); }}

        private static Brush _PeachPuff;
        public static Brush PeachPuff { get { return _PeachPuff ?? (_PeachPuff = new SolidBrush(0xFFFFDAB9)); }}

        private static Brush _Peru;
        public static Brush Peru { get { return _Peru ?? (_Peru = new SolidBrush(0xFFCD853F)); }}

        private static Brush _Pink;
        public static Brush Pink { get { return _Pink ?? (_Pink = new SolidBrush(0xFFFFC0CB)); }}

        private static Brush _Plum;
        public static Brush Plum { get { return _Plum ?? (_Plum = new SolidBrush(0xFFDDA0DD)); }}

        private static Brush _PowderBlue;
        public static Brush PowderBlue { get { return _PowderBlue ?? (_PowderBlue = new SolidBrush(0xFFB0E0E6)); }}

        private static Brush _Purple;
        public static Brush Purple { get { return _Purple ?? (_Purple = new SolidBrush(0xFF800080)); }}

        private static Brush _Red;
        public static Brush Red { get { return _Red ?? (_Red = new SolidBrush(0xFFFF0000)); }}

        private static Brush _RosyBrown;
        public static Brush RosyBrown { get { return _RosyBrown ?? (_RosyBrown = new SolidBrush(0xFFBC8F8F)); }}

        private static Brush _RoyalBlue;
        public static Brush RoyalBlue { get { return _RoyalBlue ?? (_RoyalBlue = new SolidBrush(0xFF4169E1)); }}

        private static Brush _SaddleBrown;
        public static Brush SaddleBrown { get { return _SaddleBrown ?? (_SaddleBrown = new SolidBrush(0xFF8B4513)); }}

        private static Brush _Salmon;
        public static Brush Salmon { get { return _Salmon ?? (_Salmon = new SolidBrush(0xFFFA8072)); }}

        private static Brush _SandyBrown;
        public static Brush SandyBrown { get { return _SandyBrown ?? (_SandyBrown = new SolidBrush(0xFFF4A460)); }}

        private static Brush _SeaGreen;
        public static Brush SeaGreen { get { return _SeaGreen ?? (_SeaGreen = new SolidBrush(0xFF2E8B57)); }}

        private static Brush _SeaShell;
        public static Brush SeaShell { get { return _SeaShell ?? (_SeaShell = new SolidBrush(0xFFFFF5EE)); }}

        private static Brush _Sienna;
        public static Brush Sienna { get { return _Sienna ?? (_Sienna = new SolidBrush(0xFFA0522D)); }}

        private static Brush _Silver;
        public static Brush Silver { get { return _Silver ?? (_Silver = new SolidBrush(0xFFC0C0C0)); }}

        private static Brush _SkyBlue;
        public static Brush SkyBlue { get { return _SkyBlue ?? (_SkyBlue = new SolidBrush(0xFF87CEEB)); }}

        private static Brush _SlateBlue;
        public static Brush SlateBlue { get { return _SlateBlue ?? (_SlateBlue = new SolidBrush(0xFF6A5ACD)); }}

        private static Brush _SlateGray;
        public static Brush SlateGray { get { return _SlateGray ?? (_SlateGray = new SolidBrush(0xFF708090)); }}

        private static Brush _Snow;
        public static Brush Snow { get { return _Snow ?? (_Snow = new SolidBrush(0xFFFFFAFA)); }}

        private static Brush _SpringGreen;
        public static Brush SpringGreen { get { return _SpringGreen ?? (_SpringGreen = new SolidBrush(0xFF00FF7F)); }}

        private static Brush _SteelBlue;
        public static Brush SteelBlue { get { return _SteelBlue ?? (_SteelBlue = new SolidBrush(0xFF4682B4)); }}

        private static Brush _Tan;
        public static Brush Tan { get { return _Tan ?? (_Tan = new SolidBrush(0xFFD2B48C)); }}

        private static Brush _Teal;
        public static Brush Teal { get { return _Teal ?? (_Teal = new SolidBrush(0xFF008080)); }}

        private static Brush _Thistle;
        public static Brush Thistle { get { return _Thistle ?? (_Thistle = new SolidBrush(0xFFD8BFD8)); }}

        private static Brush _Tomato;
        public static Brush Tomato { get { return _Tomato ?? (_Tomato = new SolidBrush(0xFFFF6347)); }}

        private static Brush _Turquoise;
        public static Brush Turquoise { get { return _Turquoise ?? (_Turquoise = new SolidBrush(0xFF40E0D0)); }}

        private static Brush _Violet;
        public static Brush Violet { get { return _Violet ?? (_Violet = new SolidBrush(0xFFEE82EE)); }}

        private static Brush _Wheat;
        public static Brush Wheat { get { return _Wheat ?? (_Wheat = new SolidBrush(0xFFF5DEB3)); }}

        private static Brush _White;
        public static Brush White { get { return _White ?? (_White = new SolidBrush(0xFFFFFFFF)); }}

        private static Brush _WhiteSmoke;
        public static Brush WhiteSmoke { get { return _WhiteSmoke ?? (_WhiteSmoke = new SolidBrush(0xFFF5F5F5)); }}

        private static Brush _Yellow;
        public static Brush Yellow { get { return _Yellow ?? (_Yellow = new SolidBrush(0xFFFFFF00)); }}

        private static Brush _YellowGreen;
        public static Brush YellowGreen { get { return _YellowGreen ?? (_YellowGreen = new SolidBrush(0xFF9ACD32)); }}

        private static Brush _ActiveBorder;
        public static Brush ActiveBorder { get { return _ActiveBorder ?? (_ActiveBorder = new SolidBrush(0xFFB4B4B4)); }}

        private static Brush _ActiveCaption;
        public static Brush ActiveCaption { get { return _ActiveCaption ?? (_ActiveCaption = new SolidBrush(0xFF99B4D1)); }}

        private static Brush _ActiveCaptionText;
        public static Brush ActiveCaptionText { get { return _ActiveCaptionText ?? (_ActiveCaptionText = new SolidBrush(0xFF000000)); }}

        private static Brush _AppWorkspace;
        public static Brush AppWorkspace { get { return _AppWorkspace ?? (_AppWorkspace = new SolidBrush(0xFFABABAB)); }}

        private static Brush _ButtonFace;
        public static Brush ButtonFace { get { return _ButtonFace ?? (_ButtonFace = new SolidBrush(0xFFF0F0F0)); }}

        private static Brush _ButtonHighlight;
        public static Brush ButtonHighlight { get { return _ButtonHighlight ?? (_ButtonHighlight = new SolidBrush(0xFFFFFFFF)); }}

        private static Brush _ButtonShadow;
        public static Brush ButtonShadow { get { return _ButtonShadow ?? (_ButtonShadow = new SolidBrush(0xFFA0A0A0)); }}

        private static Brush _Control;
        public static Brush Control { get { return _Control ?? (_Control = new SolidBrush(0xFFF0F0F0)); }}

        private static Brush _ControlLightLight;
        public static Brush ControlLightLight { get { return _ControlLightLight ?? (_ControlLightLight = new SolidBrush(0xFFFFFFFF)); }}

        private static Brush _ControlLight;
        public static Brush ControlLight { get { return _ControlLight ?? (_ControlLight = new SolidBrush(0xFFE3E3E3)); }}

        private static Brush _ControlDark;
        public static Brush ControlDark { get { return _ControlDark ?? (_ControlDark = new SolidBrush(0xFFA0A0A0)); }}

        private static Brush _ControlDarkDark;
        public static Brush ControlDarkDark { get { return _ControlDarkDark ?? (_ControlDarkDark = new SolidBrush(0xFF696969)); }}

        private static Brush _ControlText;
        public static Brush ControlText { get { return _ControlText ?? (_ControlText = new SolidBrush(0xFF000000)); }}

        private static Brush _Desktop;
        public static Brush Desktop { get { return _Desktop ?? (_Desktop = new SolidBrush(0xFF000000)); }}

        private static Brush _GradientActiveCaption;
        public static Brush GradientActiveCaption { get { return _GradientActiveCaption ?? (_GradientActiveCaption = new SolidBrush(0xFFB9D1EA)); }}

        private static Brush _GradientInactiveCaption;
        public static Brush GradientInactiveCaption { get { return _GradientInactiveCaption ?? (_GradientInactiveCaption = new SolidBrush(0xFFD7E4F2)); }}

        private static Brush _GrayText;
        public static Brush GrayText { get { return _GrayText ?? (_GrayText = new SolidBrush(0xFF6D6D6D)); }}

        private static Brush _Highlight;
        public static Brush Highlight { get { return _Highlight ?? (_Highlight = new SolidBrush(0xFF0078D7)); }}

        private static Brush _HighlightText;
        public static Brush HighlightText { get { return _HighlightText ?? (_HighlightText = new SolidBrush(0xFFFFFFFF)); }}

        private static Brush _HotTrack;
        public static Brush HotTrack { get { return _HotTrack ?? (_HotTrack = new SolidBrush(0xFF0066CC)); }}

        private static Brush _InactiveCaption;
        public static Brush InactiveCaption { get { return _InactiveCaption ?? (_InactiveCaption = new SolidBrush(0xFFBFCDDB)); }}

        private static Brush _InactiveBorder;
        public static Brush InactiveBorder { get { return _InactiveBorder ?? (_InactiveBorder = new SolidBrush(0xFFF4F7FC)); }}

        private static Brush _InactiveCaptionText;
        public static Brush InactiveCaptionText { get { return _InactiveCaptionText ?? (_InactiveCaptionText = new SolidBrush(0xFF000000)); }}

        private static Brush _Info;
        public static Brush Info { get { return _Info ?? (_Info = new SolidBrush(0xFFFFFFE1)); }}

        private static Brush _InfoText;
        public static Brush InfoText { get { return _InfoText ?? (_InfoText = new SolidBrush(0xFF000000)); }}

        private static Brush _Menu;
        public static Brush Menu { get { return _Menu ?? (_Menu = new SolidBrush(0xFFF0F0F0)); }}

        private static Brush _MenuBar;
        public static Brush MenuBar { get { return _MenuBar ?? (_MenuBar = new SolidBrush(0xFFF0F0F0)); }}

        private static Brush _MenuHighlight;
        public static Brush MenuHighlight { get { return _MenuHighlight ?? (_MenuHighlight = new SolidBrush(0xFF3399FF)); }}

        private static Brush _MenuText;
        public static Brush MenuText { get { return _MenuText ?? (_MenuText = new SolidBrush(0xFF000000)); }}

        private static Brush _ScrollBar;
        public static Brush ScrollBar { get { return _ScrollBar ?? (_ScrollBar = new SolidBrush(0xFFC8C8C8)); }}

        private static Brush _Window;
        public static Brush Window { get { return _Window ?? (_Window = new SolidBrush(0xFFFFFFFF)); }}

        private static Brush _WindowFrame;
        public static Brush WindowFrame { get { return _WindowFrame ?? (_WindowFrame = new SolidBrush(0xFF646464)); }}

        private static Brush _WindowText;
        public static Brush WindowText { get { return _WindowText ?? (_WindowText = new SolidBrush(0xFF000000)); }}

    }
}