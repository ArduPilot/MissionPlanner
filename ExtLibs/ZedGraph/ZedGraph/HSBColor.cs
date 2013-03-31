using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ZedGraph
{
	/// <summary>
	/// Hue-Saturation-Brightness Color class to store a color value, and to manage conversions
	/// to and from RGB colors in the <see cref="Color" /> struct.
	/// </summary>
	/// <remarks>
	/// This class is based on code from http://www.cs.rit.edu/~ncs/color/ by Eugene Vishnevsky.
	/// This struct stores the hue, saturation, brightness, and alpha values internally as
	/// <see cref="byte" /> values from 0 to 255.  The hue represents a fraction of the 360 degrees
	/// of color space available. The saturation is the color intensity, where 0 represents gray scale
	/// and 255 is the most colored.  For the brightness, 0 represents black and 255
	/// represents white.
	/// </remarks>
	[Serializable]
	public struct HSBColor
	{
		/// <summary>
		/// The color hue value, ranging from 0 to 255.
		/// </summary>
		/// <remarks>
		/// This property is actually a rescaling of the 360 degrees on the color wheel to 255
		/// possible values.  Therefore, every 42.5 units is a new sector, with the following
		/// convention:  red=0, yellow=42.5, green=85, cyan=127.5, blue=170, magenta=212.5
		/// </remarks>
		public byte H;
		/// <summary>
		/// The color saturation (intensity) value, ranging from 0 (gray scale) to 255 (most colored).
		/// </summary>
		public byte S;
		/// <summary>
		/// The brightness value, ranging from 0 (black) to 255 (white).
		/// </summary>
		public byte B;
		/// <summary>
		/// The alpha value (opacity), ranging from 0 (transparent) to 255 (opaque).
		/// </summary>
		public byte A;

		/// <summary>
		/// Constructor to load an <see cref="HSBColor" /> struct from hue, saturation and
		/// brightness values
		/// </summary>
		/// <param name="h">The color hue value, ranging from 0 to 255</param>
		/// <param name="s">The color saturation (intensity) value, ranging from 0 (gray scale)
		/// to 255 (most colored)</param>
		/// <param name="b">The brightness value, ranging from 0 (black) to 255 (white)</param>
		public HSBColor( int h, int s, int b )
		{
			this.H = (byte)h;
			this.S = (byte)s;
			this.B = (byte)b;
			this.A = 255;
		}

		/// <summary>
		/// Constructor to load an <see cref="HSBColor" /> struct from hue, saturation,
		/// brightness, and alpha values
		/// </summary>
		/// <param name="h">The color hue value, ranging from 0 to 255</param>
		/// <param name="s">The color saturation (intensity) value, ranging from 0 (gray scale)
		/// to 255 (most colored)</param>
		/// <param name="b">The brightness value, ranging from 0 (black) to 255 (white)</param>
		/// <param name="a">The alpha value (opacity), ranging from 0 (transparent) to
		/// 255 (opaque)</param>
		public HSBColor( int a, int h, int s, int b )
			: this( h, s, b )
		{
			this.A = (byte)a;
		}

		/// <summary>
		/// Constructor to load an <see cref="HSBColor" /> struct from a system
		/// <see cref="Color" /> struct.
		/// </summary>
		/// <param name="color">An rgb <see cref="Color" /> struct containing the equivalent
		/// color you want to generate</param>
		public HSBColor( Color color )
		{
			this = FromRGB( color );
		}


		/// <summary>
		/// Implicit conversion operator to convert directly from an <see cref="HSBColor" /> to
		/// a <see cref="Color" /> struct.
		/// </summary>
		/// <param name="hsbColor">The <see cref="HSBColor" /> struct to be converted</param>
		/// <returns>An equivalent <see cref="Color" /> struct that can be used in the GDI+
		/// graphics library</returns>
		public static implicit operator Color( HSBColor hsbColor )
		{
			return ToRGB( hsbColor );
		}

		/// <summary>
		/// Convert an <see cref="HSBColor" /> value to an equivalent <see cref="Color" /> value.
		/// </summary>
		/// <remarks>
		/// This method is based on code from http://www.cs.rit.edu/~ncs/color/ by Eugene Vishnevsky.
		/// </remarks>
		/// <param name="hsbColor">The <see cref="HSBColor" /> struct to be converted</param>
		/// <returns>An equivalent <see cref="Color" /> struct, compatible with the GDI+ library</returns>
		public static Color ToRGB( HSBColor hsbColor )
		{
			Color rgbColor = Color.Black;

			// Determine which sector of the color wheel contains this hue
			// hsbColor.H ranges from 0 to 255, and there are 6 sectors, so 42.5 per sector
			int sector = (int) Math.Floor( (double) hsbColor.H / 42.5 );
			// Calculate where the hue lies within the sector for interpolation purpose
			double fraction = (double) hsbColor.H / 42.5 - (double) sector;

			double sFrac = (double) hsbColor.S / 255.0;
			byte p = (byte) (( (double) hsbColor.B * ( 1.0 - sFrac ) ) + 0.5);
			byte q = (byte) (( (double) hsbColor.B * ( 1.0 - sFrac * fraction ) ) + 0.5);
			byte t = (byte) (( (double) hsbColor.B * ( 1.0 - sFrac * ( 1.0 - fraction ) ) ) + 0.5);


			switch( sector )
			{
				case 0:			// red - yellow
					rgbColor = Color.FromArgb( hsbColor.A, hsbColor.B, t, p );
					break;
				case 1:			// yellow - green
					rgbColor = Color.FromArgb( hsbColor.A, q, hsbColor.B, p );
					break;
				case 2:			// green - cyan
					rgbColor = Color.FromArgb( hsbColor.A, p, hsbColor.B, t );
					break;
				case 3:			// cyan - blue
					rgbColor = Color.FromArgb( hsbColor.A, p, q, hsbColor.B );
					break;
				case 4:			// blue - magenta
					rgbColor = Color.FromArgb( hsbColor.A, t, p, hsbColor.B );
					break;
				case 5:
				default:		// magenta - red
					rgbColor = Color.FromArgb( hsbColor.A, hsbColor.B, p, q );
					break;
			}

			return rgbColor;
		}

		/// <summary>
		/// Convert this <see cref="HSBColor" /> value to an equivalent <see cref="Color" /> value.
		/// </summary>
		/// <remarks>
		/// This method is based on code from http://www.cs.rit.edu/~ncs/color/ by Eugene Vishnevsky.
		/// </remarks>
		/// <returns>An equivalent <see cref="Color" /> struct, compatible with the GDI+ library</returns>
		public Color ToRGB()
		{
			return ToRGB( this );
		}
			
		/// <summary>
		/// Convert a <see cref="Color" /> value to an equivalent <see cref="HSBColor" /> value.
		/// </summary>
		/// <remarks>
		/// This method is based on code from http://www.cs.rit.edu/~ncs/color/ by Eugene Vishnevsky.
		/// </remarks>
		/// <returns>An equivalent <see cref="HSBColor" /> struct</returns>
		public HSBColor FromRGB()
		{
			return FromRGB( this );
		}

		/// <summary>
		/// Convert a <see cref="Color" /> value to an equivalent <see cref="HSBColor" /> value.
		/// </summary>
		/// <remarks>
		/// This method is based on code from http://www.cs.rit.edu/~ncs/color/ by Eugene Vishnevsky.
		/// </remarks>
		/// <param name="rgbColor">The <see cref="Color" /> struct to be converted</param>
		/// <returns>An equivalent <see cref="HSBColor" /> struct</returns>
		public static HSBColor FromRGB( Color rgbColor )
		{
			double r = (double) rgbColor.R / 255.0;
			double g = (double) rgbColor.G / 255.0;
			double b = (double) rgbColor.B / 255.0;

			double min = Math.Min( Math.Min( r, g ), b );
			double max = Math.Max( Math.Max( r, g ), b );

			HSBColor hsbColor = new HSBColor( rgbColor.A, 0, 0, 0 );

			hsbColor.B = (byte) ( max * 255.0 + 0.5 );

			double delta = max - min;

			if ( max != 0.0 )
			{
				hsbColor.S = (byte) ( delta / max * 255.0 + 0.5 );
			}
			else
			{
				hsbColor.S = 0;
				hsbColor.H = 0;
				return hsbColor;
			}

			double h;
			if ( r == max )
				h = ( g - b ) / delta;		// between yellow & magenta
			else if ( g == max )
				h = 2 + ( b - r ) / delta;	// between cyan & yellow
			else
				h = 4 + ( r - g ) / delta;	// between magenta & cyan

			hsbColor.H = (byte) ( h * 42.5 );	
			if ( hsbColor.H < 0 )
				hsbColor.H += 255;

			return hsbColor;
		}
	
	}
}
