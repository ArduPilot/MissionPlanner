
#region GPL License
/*
Copyright (c) 2010 Miguel Angel Guirado López

This file is part of CsAssortedWidgets.

    Trixion3D is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CsAssortedWidgets is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with CsAssortedWidgets.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using AssortedWidgets.Math3D;

namespace AssortedWidgets.Graphics
{
    /// <summary>
    /// Represents a four compAlignment TColor value (red, green, blue, alpha).
    /// </summary>
    public class TColor
    {
        /// <summary>
        /// The red compAlignment.
        /// </summary>
        public float R;
        /// <summary>
        /// The green compAlignment.
        /// </summary>
        public float G;
        /// <summary>
        /// The blue compAlignment.
        /// </summary>
        public float B;
        /// <summary>
        /// The alpha compAlignment.
        /// </summary>
        public float A;

        public TColor()
            : this(System.Drawing.Color.Black)
        {
            
        }

        public TColor(System.Drawing.Color pTColor)
        {
            this.R = pTColor.R / 255;
            this.G = pTColor.G / 255;
            this.B = pTColor.B / 255;
            this.A = pTColor.A / 255;
        }

        /// <summary>
        /// Initialises a new instancia of the TColorValue struct.
        /// </summary>
        /// <param Name_="red"></param>
        /// <param Name_="green"></param>
        /// <param Name_="blue"></param>
        /// <param Name_="alpha"></param>
        public TColor(float red, float green, float blue, float alpha)
        {
            this.R = red;
            this.G = green;
            this.B = blue;
            this.A = alpha;
        }

        /// <summary>
        /// Initialises a new instancia of the TColorValue struct.
        /// </summary>
        public TColor(float[] vals)
        {
            this.R = vals[0];
            this.G = vals[1];
            this.B = vals[2];
            this.A = vals[3];
        }

        /// <summary>
        /// Returns a TColorValue that is interpolated between two TColorValues by the given amound (0.0-1.0).
        /// </summary>
        /// <param Name_="start"></param>
        /// <param Name_="end"></param>
        /// <param Name_="percent"></param>
        /// <returns></returns>
        public static TColor Interpolate( TColor start, TColor end, float percent )
        {
            TColor r = new TColor();
            r.R = start.R + (percent * (end.R - start.R));
            r.G = start.G + (percent * (end.G - start.G));
            r.B = start.B + (percent * (end.B - start.B));
            r.A = start.A + (percent * (end.A - start.A));
            return r;
        }

        public static TColor Transparent
        {
            get { return new TColor( 1f, 1f, 1f, 0f ); }
        }


        public static TColor AliceBlue
        {
            get { return new TColor( 0.9411765f, 0.972549f, 1f, 1f ); }
        }


        public static TColor AntiqueWhite
        {
            get { return new TColor( 0.9803922f, 0.9215686f, 0.8431373f, 1f ); }
        }


        public static TColor Aqua
        {
            get { return new TColor( 0f, 1f, 1f, 1f ); }
        }


        public static TColor Aquamarine
        {
            get { return new TColor( 0.4980392f, 1f, 0.8313726f, 1f ); }
        }


        public static TColor Azure
        {
            get { return new TColor( 0.9411765f, 1f, 1f, 1f ); }
        }


        public static TColor Beige
        {
            get { return new TColor( 0.9607843f, 0.9607843f, 0.8627451f, 1f ); }
        }


        public static TColor Bisque
        {
            get { return new TColor( 1f, 0.8941177f, 0.7686275f, 1f ); }
        }


        public static TColor Black
        {
            get { return new TColor( 0f, 0f, 0f, 1f ); }
        }


        public static TColor BlanchedAlmond
        {
            get { return new TColor( 1f, 0.9215686f, 0.8039216f, 1f ); }
        }


        public static TColor Blue
        {
            get { return new TColor( 0f, 0f, 1f, 1f ); }
        }


        public static TColor BlueViolet
        {
            get { return new TColor( 0.5411765f, 0.1686275f, 0.8862745f, 1f ); }
        }


        public static TColor Brown
        {
            get { return new TColor( 0.6470588f, 0.1647059f, 0.1647059f, 1f ); }
        }


        public static TColor BurlyWood
        {
            get { return new TColor( 0.8705882f, 0.7215686f, 0.5294118f, 1f ); }
        }


        public static TColor CadetBlue
        {
            get { return new TColor( 0.372549f, 0.6196079f, 0.627451f, 1f ); }
        }


        public static TColor Chartreuse
        {
            get { return new TColor( 0.4980392f, 1f, 0f, 1f ); }
        }


        public static TColor Chocolate
        {
            get { return new TColor( 0.8235294f, 0.4117647f, 0.1176471f, 1f ); }
        }


        public static TColor Coral
        {
            get { return new TColor( 1f, 0.4980392f, 0.3137255f, 1f ); }
        }


        public static TColor CornflowerBlue
        {
            get { return new TColor( 0.3921569f, 0.5843138f, 0.9294118f, 1f ); }
        }


        public static TColor Cornsilk
        {
            get { return new TColor( 1f, 0.972549f, 0.8627451f, 1f ); }
        }


        public static TColor Crimson
        {
            get { return new TColor( 0.8627451f, 0.07843138f, 0.2352941f, 1f ); }
        }


        public static TColor Cyan
        {
            get { return new TColor( 0f, 1f, 1f, 1f ); }
        }


        public static TColor DarkBlue
        {
            get { return new TColor( 0f, 0f, 0.5450981f, 1f ); }
        }


        public static TColor DarkCyan
        {
            get { return new TColor( 0f, 0.5450981f, 0.5450981f, 1f ); }
        }


        public static TColor DarkGoldenrod
        {
            get { return new TColor( 0.7215686f, 0.5254902f, 0.04313726f, 1f ); }
        }


        public static TColor DarkGray
        {
            get { return new TColor( 0.6627451f, 0.6627451f, 0.6627451f, 1f ); }
        }


        public static TColor DarkGreen
        {
            get { return new TColor( 0f, 0.3921569f, 0f, 1f ); }
        }


        public static TColor DarkKhaki
        {
            get { return new TColor( 0.7411765f, 0.7176471f, 0.4196078f, 1f ); }
        }


        public static TColor DarkMagenta
        {
            get { return new TColor( 0.5450981f, 0f, 0.5450981f, 1f ); }
        }


        public static TColor DarkOliveGreen
        {
            get { return new TColor( 0.3333333f, 0.4196078f, 0.1843137f, 1f ); }
        }


        public static TColor DarkOrange
        {
            get { return new TColor( 1f, 0.5490196f, 0f, 1f ); }
        }


        public static TColor DarkOrchid
        {
            get { return new TColor( 0.6f, 0.1960784f, 0.8f, 1f ); }
        }


        public static TColor DarkRed
        {
            get { return new TColor( 0.5450981f, 0f, 0f, 1f ); }
        }


        public static TColor DarkSalmon
        {
            get { return new TColor( 0.9137255f, 0.5882353f, 0.4784314f, 1f ); }
        }


        public static TColor DarkSeaGreen
        {
            get { return new TColor( 0.5607843f, 0.7372549f, 0.5450981f, 1f ); }
        }


        public static TColor DarkSlateBlue
        {
            get { return new TColor( 0.282353f, 0.2392157f, 0.5450981f, 1f ); }
        }


        public static TColor DarkSlateGray
        {
            get { return new TColor( 0.1843137f, 0.3098039f, 0.3098039f, 1f ); }
        }


        public static TColor DarkTurquoise
        {
            get { return new TColor( 0f, 0.8078431f, 0.8196079f, 1f ); }
        }


        public static TColor DarkViolet
        {
            get { return new TColor( 0.5803922f, 0f, 0.827451f, 1f ); }
        }


        public static TColor DeepPink
        {
            get { return new TColor( 1f, 0.07843138f, 0.5764706f, 1f ); }
        }


        public static TColor DeepSkyBlue
        {
            get { return new TColor( 0f, 0.7490196f, 1f, 1f ); }
        }


        public static TColor DimGray
        {
            get { return new TColor( 0.4117647f, 0.4117647f, 0.4117647f, 1f ); }
        }


        public static TColor DodgerBlue
        {
            get { return new TColor( 0.1176471f, 0.5647059f, 1f, 1f ); }
        }


        public static TColor Firebrick
        {
            get { return new TColor( 0.6980392f, 0.1333333f, 0.1333333f, 1f ); }
        }


        public static TColor FloralWhite
        {
            get { return new TColor( 1f, 0.9803922f, 0.9411765f, 1f ); }
        }


        public static TColor ForestGreen
        {
            get { return new TColor( 0.1333333f, 0.5450981f, 0.1333333f, 1f ); }
        }


        public static TColor Fuchsia
        {
            get { return new TColor( 1f, 0f, 1f, 1f ); }
        }


        public static TColor Gainsboro
        {
            get { return new TColor( 0.8627451f, 0.8627451f, 0.8627451f, 1f ); }
        }


        public static TColor GhostWhite
        {
            get { return new TColor( 0.972549f, 0.972549f, 1f, 1f ); }
        }


        public static TColor Gold
        {
            get { return new TColor( 1f, 0.8431373f, 0f, 1f ); }
        }


        public static TColor Goldenrod
        {
            get { return new TColor( 0.854902f, 0.6470588f, 0.1254902f, 1f ); }
        }


        public static TColor Gray
        {
            get { return new TColor( 0.5019608f, 0.5019608f, 0.5019608f, 1f ); }
        }


        public static TColor Green
        {
            get { return new TColor( 0f, 0.5019608f, 0f, 1f ); }
        }


        public static TColor GreenYellow
        {
            get { return new TColor( 0.6784314f, 1f, 0.1843137f, 1f ); }
        }


        public static TColor Honeydew
        {
            get { return new TColor( 0.9411765f, 1f, 0.9411765f, 1f ); }
        }


        public static TColor HotPink
        {
            get { return new TColor( 1f, 0.4117647f, 0.7058824f, 1f ); }
        }


        public static TColor IndianRed
        {
            get { return new TColor( 0.8039216f, 0.3607843f, 0.3607843f, 1f ); }
        }


        public static TColor Indigo
        {
            get { return new TColor( 0.2941177f, 0f, 0.509804f, 1f ); }
        }


        public static TColor Ivory
        {
            get { return new TColor( 1f, 1f, 0.9411765f, 1f ); }
        }


        public static TColor Khaki
        {
            get { return new TColor( 0.9411765f, 0.9019608f, 0.5490196f, 1f ); }
        }


        public static TColor Lavender
        {
            get { return new TColor( 0.9019608f, 0.9019608f, 0.9803922f, 1f ); }
        }


        public static TColor LavenderBlush
        {
            get { return new TColor( 1f, 0.9411765f, 0.9607843f, 1f ); }
        }


        public static TColor LawnGreen
        {
            get { return new TColor( 0.4862745f, 0.9882353f, 0f, 1f ); }
        }


        public static TColor LemonChiffon
        {
            get { return new TColor( 1f, 0.9803922f, 0.8039216f, 1f ); }
        }


        public static TColor LightBlue
        {
            get { return new TColor( 0.6784314f, 0.8470588f, 0.9019608f, 1f ); }
        }


        public static TColor LightCoral
        {
            get { return new TColor( 0.9411765f, 0.5019608f, 0.5019608f, 1f ); }
        }


        public static TColor LightCyan
        {
            get { return new TColor( 0.8784314f, 1f, 1f, 1f ); }
        }


        public static TColor LightGoldenrodYellow
        {
            get { return new TColor( 0.9803922f, 0.9803922f, 0.8235294f, 1f ); }
        }


        public static TColor LightGreen
        {
            get { return new TColor( 0.5647059f, 0.9333333f, 0.5647059f, 1f ); }
        }


        public static TColor LightGray
        {
            get { return new TColor( 0.827451f, 0.827451f, 0.827451f, 1f ); }
        }


        public static TColor LightPink
        {
            get { return new TColor( 1f, 0.7137255f, 0.7568628f, 1f ); }
        }


        public static TColor LightSalmon
        {
            get { return new TColor( 1f, 0.627451f, 0.4784314f, 1f ); }
        }


        public static TColor LightSeaGreen
        {
            get { return new TColor( 0.1254902f, 0.6980392f, 0.6666667f, 1f ); }
        }


        public static TColor LightSkyBlue
        {
            get { return new TColor( 0.5294118f, 0.8078431f, 0.9803922f, 1f ); }
        }


        public static TColor LightSlateGray
        {
            get { return new TColor( 0.4666667f, 0.5333334f, 0.6f, 1f ); }
        }


        public static TColor LightSteelBlue
        {
            get { return new TColor( 0.6901961f, 0.7686275f, 0.8705882f, 1f ); }
        }


        public static TColor LightYellow
        {
            get { return new TColor( 1f, 1f, 0.8784314f, 1f ); }
        }


        public static TColor Lime
        {
            get { return new TColor( 0f, 1f, 0f, 1f ); }
        }


        public static TColor LimeGreen
        {
            get { return new TColor( 0.1960784f, 0.8039216f, 0.1960784f, 1f ); }
        }


        public static TColor Linen
        {
            get { return new TColor( 0.9803922f, 0.9411765f, 0.9019608f, 1f ); }
        }


        public static TColor Magenta
        {
            get { return new TColor( 1f, 0f, 1f, 1f ); }
        }


        public static TColor Maroon
        {
            get { return new TColor( 0.5019608f, 0f, 0f, 1f ); }
        }


        public static TColor MediumAquamarine
        {
            get { return new TColor( 0.4f, 0.8039216f, 0.6666667f, 1f ); }
        }


        public static TColor MediumBlue
        {
            get { return new TColor( 0f, 0f, 0.8039216f, 1f ); }
        }


        public static TColor MediumOrchid
        {
            get { return new TColor( 0.7294118f, 0.3333333f, 0.827451f, 1f ); }
        }


        public static TColor MediumPurple
        {
            get { return new TColor( 0.5764706f, 0.4392157f, 0.8588235f, 1f ); }
        }


        public static TColor MediumSeaGreen
        {
            get { return new TColor( 0.2352941f, 0.7019608f, 0.4431373f, 1f ); }
        }


        public static TColor MediumSlateBlue
        {
            get { return new TColor( 0.4823529f, 0.4078431f, 0.9333333f, 1f ); }
        }


        public static TColor MediumSpringGreen
        {
            get { return new TColor( 0f, 0.9803922f, 0.6039216f, 1f ); }
        }


        public static TColor MediumTurquoise
        {
            get { return new TColor( 0.282353f, 0.8196079f, 0.8f, 1f ); }
        }


        public static TColor MediumVioletRed
        {
            get { return new TColor( 0.7803922f, 0.08235294f, 0.5215687f, 1f ); }
        }


        public static TColor MidnightBlue
        {
            get { return new TColor( 0.09803922f, 0.09803922f, 0.4392157f, 1f ); }
        }


        public static TColor MintCream
        {
            get { return new TColor( 0.9607843f, 1f, 0.9803922f, 1f ); }
        }


        public static TColor MistyRose
        {
            get { return new TColor( 1f, 0.8941177f, 0.8823529f, 1f ); }
        }


        public static TColor Moccasin
        {
            get { return new TColor( 1f, 0.8941177f, 0.7098039f, 1f ); }
        }


        public static TColor NavajoWhite
        {
            get { return new TColor( 1f, 0.8705882f, 0.6784314f, 1f ); }
        }


        public static TColor Navy
        {
            get { return new TColor( 0f, 0f, 0.5019608f, 1f ); }
        }


        public static TColor OldLace
        {
            get { return new TColor( 0.9921569f, 0.9607843f, 0.9019608f, 1f ); }
        }


        public static TColor Olive
        {
            get { return new TColor( 0.5019608f, 0.5019608f, 0f, 1f ); }
        }


        public static TColor OliveDrab
        {
            get { return new TColor( 0.4196078f, 0.5568628f, 0.1372549f, 1f ); }
        }


        public static TColor Orange
        {
            get { return new TColor( 1f, 0.6470588f, 0f, 1f ); }
        }


        public static TColor OrangeRed
        {
            get { return new TColor( 1f, 0.2705882f, 0f, 1f ); }
        }


        public static TColor Orchid
        {
            get { return new TColor( 0.854902f, 0.4392157f, 0.8392157f, 1f ); }
        }


        public static TColor PaleGoldenrod
        {
            get { return new TColor( 0.9333333f, 0.9098039f, 0.6666667f, 1f ); }
        }


        public static TColor PaleGreen
        {
            get { return new TColor( 0.5960785f, 0.9843137f, 0.5960785f, 1f ); }
        }


        public static TColor PaleTurquoise
        {
            get { return new TColor( 0.6862745f, 0.9333333f, 0.9333333f, 1f ); }
        }


        public static TColor PaleVioletRed
        {
            get { return new TColor( 0.8588235f, 0.4392157f, 0.5764706f, 1f ); }
        }


        public static TColor PapayaWhip
        {
            get { return new TColor( 1f, 0.9372549f, 0.8352941f, 1f ); }
        }


        public static TColor PeachPuff
        {
            get { return new TColor( 1f, 0.854902f, 0.7254902f, 1f ); }
        }


        public static TColor Peru
        {
            get { return new TColor( 0.8039216f, 0.5215687f, 0.2470588f, 1f ); }
        }


        public static TColor Pink
        {
            get { return new TColor( 1f, 0.7529412f, 0.7960784f, 1f ); }
        }


        public static TColor Plum
        {
            get { return new TColor( 0.8666667f, 0.627451f, 0.8666667f, 1f ); }
        }


        public static TColor PowderBlue
        {
            get { return new TColor( 0.6901961f, 0.8784314f, 0.9019608f, 1f ); }
        }


        public static TColor Purple
        {
            get { return new TColor( 0.5019608f, 0f, 0.5019608f, 1f ); }
        }


        public static TColor Red
        {
            get { return new TColor( 1f, 0f, 0f, 1f ); }
        }


        public static TColor RosyBrown
        {
            get { return new TColor( 0.7372549f, 0.5607843f, 0.5607843f, 1f ); }
        }


        public static TColor RoyalBlue
        {
            get { return new TColor( 0.254902f, 0.4117647f, 0.8823529f, 1f ); }
        }


        public static TColor SaddleBrown
        {
            get { return new TColor( 0.5450981f, 0.2705882f, 0.07450981f, 1f ); }
        }


        public static TColor Salmon
        {
            get { return new TColor( 0.9803922f, 0.5019608f, 0.4470588f, 1f ); }
        }


        public static TColor SandyBrown
        {
            get { return new TColor( 0.9568627f, 0.6431373f, 0.3764706f, 1f ); }
        }


        public static TColor SeaGreen
        {
            get { return new TColor( 0.1803922f, 0.5450981f, 0.3411765f, 1f ); }
        }


        public static TColor SeaShell
        {
            get { return new TColor( 1f, 0.9607843f, 0.9333333f, 1f ); }
        }


        public static TColor Sienna
        {
            get { return new TColor( 0.627451f, 0.3215686f, 0.1764706f, 1f ); }
        }


        public static TColor Silver
        {
            get { return new TColor( 0.7529412f, 0.7529412f, 0.7529412f, 1f ); }
        }


        public static TColor SkyBlue
        {
            get { return new TColor( 0.5294118f, 0.8078431f, 0.9215686f, 1f ); }
        }


        public static TColor SlateBlue
        {
            get { return new TColor( 0.4156863f, 0.3529412f, 0.8039216f, 1f ); }
        }


        public static TColor SlateGray
        {
            get { return new TColor( 0.4392157f, 0.5019608f, 0.5647059f, 1f ); }
        }


        public static TColor Snow
        {
            get { return new TColor( 1f, 0.9803922f, 0.9803922f, 1f ); }
        }


        public static TColor SpringGreen
        {
            get { return new TColor( 0f, 1f, 0.4980392f, 1f ); }
        }


        public static TColor SteelBlue
        {
            get { return new TColor( 0.2745098f, 0.509804f, 0.7058824f, 1f ); }
        }


        public static TColor Tan
        {
            get { return new TColor( 0.8235294f, 0.7058824f, 0.5490196f, 1f ); }
        }


        public static TColor Teal
        {
            get { return new TColor( 0f, 0.5019608f, 0.5019608f, 1f ); }
        }


        public static TColor Thistle
        {
            get { return new TColor( 0.8470588f, 0.7490196f, 0.8470588f, 1f ); }
        }


        public static TColor Tomato
        {
            get { return new TColor( 1f, 0.3882353f, 0.2784314f, 1f ); }
        }

        public override string ToString()
        {
            return "Red: " + this.R + " Green: " + this.G + " Blue: " + this.B + " Alpha: " + this.A;
        }
        
        public Vector4f ToVector4()
        {
            return new Vector4f( this.R, this.G, this.B, this.A );
        }

        public static TColor Turquoise
        {
            get { return new TColor( 0.2509804f, 0.8784314f, 0.8156863f, 1f ); }
        }


        public static TColor Violet
        {
            get { return new TColor( 0.9333333f, 0.509804f, 0.9333333f, 1f ); }
        }


        public static TColor Wheat
        {
            get { return new TColor( 0.9607843f, 0.8705882f, 0.7019608f, 1f ); }
        }


        public static TColor White
        {
            get { return new TColor( 1f, 1f, 1f, 1f ); }
        }


        public static TColor WhiteSmoke
        {
            get { return new TColor( 0.9607843f, 0.9607843f, 0.9607843f, 1f ); }
        }


        public static TColor Yellow
        {
            get { return new TColor( 1f, 1f, 0f, 1f ); }
        }


        public static TColor YellowGreen
        {
            get { return new TColor( 0.6039216f, 0.8039216f, 0.1960784f, 1f ); }
        }
    }
}
