//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2004  John Champion
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// The <see cref="FontSpec"/> class is a generic font class that maintains the font family,
	/// attributes, colors, border and fill modes, font size, and angle information.
	/// This class can render text with a variety of alignment options using the
	/// <see cref="AlignH"/> and <see cref="AlignV"/> parameters in the
	/// <see cref="Draw(Graphics,PaneBase,string,float,float,AlignH,AlignV,float)"/> method.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.24 $ $Date: 2007-01-25 07:56:08 $ </version>
	[Serializable]
	public class FontSpec : ICloneable, ISerializable
	{
	#region Fields
		/// <summary>
		/// Private field that stores the color of the font characters for this
		/// <see cref="FontSpec"/>.  Use the public property <see cref="FontColor"/>
		/// to access this value.
		/// </summary>
		/// <value>A system <see cref="System.Drawing.Color"/> reference.</value>
		private Color _fontColor;
		/// <summary>
		/// Private field that stores the font family name for this <see cref="FontSpec"/>.
		/// Use the public property <see cref="Family"/> to access this value.
		/// </summary>
		/// <value>A text string with the font family name, e.g., "Arial"</value>
		private string _family;
		/// <summary>
		/// Private field that determines whether this <see cref="FontSpec"/> is
		/// drawn with bold typeface.
		/// Use the public property <see cref="IsBold"/> to access this value.
		/// </summary>
		/// <value>A boolean value, true for bold, false for normal</value>
		private bool _isBold;
		/// <summary>
		/// Private field that determines whether this <see cref="FontSpec"/> is
		/// drawn with italic typeface.
		/// Use the public property <see cref="IsItalic"/> to access this value.
		/// </summary>
		/// <value>A boolean value, true for italic, false for normal</value>
		private bool _isItalic;
		/// <summary>
		/// Private field that determines whether this <see cref="FontSpec"/> is
		/// drawn with underlined typeface.
		/// Use the public property <see cref="IsUnderline"/> to access this value.
		/// </summary>
		/// <value>A boolean value, true for underline, false for normal</value>
		private bool _isUnderline;
		/// <summary>
		/// Private field that stores the <see cref="ZedGraph.Fill"/> data for this
		/// <see cref="FontSpec"/>.  Use the public property <see cref="Fill"/> to
		/// access this value.
		/// </summary>
		private Fill _fill;
		/// <summary>
		/// Private field that determines the properties of the border around the text.
		/// Use the public property <see cref="Border"/> to access this value.
		/// </summary>
		private Border _border;

		/// <summary>
		/// Private field that determines the angle at which this
		/// <see cref="FontSpec"/> object is drawn.  Use the public property
		/// <see cref="Angle"/> to access this value.
		/// </summary>
		/// <value>The angle of the font, measured in anti-clockwise degrees from
		/// horizontal.  Negative values are permitted.</value>
		private float _angle;

		/// <summary>
		/// Private field that determines the alignment with which this
		/// <see cref="FontSpec"/> object is drawn.  This alignment really only
		/// affects multi-line strings.  Use the public property
		/// <see cref="StringAlignment"/> to access this value.
		/// </summary>
		/// <value>A <see cref="StringAlignment"/> enumeration.</value>
		private StringAlignment _stringAlignment;

		/// <summary>
		/// Private field that determines the size of the font for this
		/// <see cref="FontSpec"/> object.  Use the public property
		/// <see cref="Size"/> to access this value.
		/// </summary>
		/// <value>The size of the font, measured in points (1/72 inch).</value>
		private float _size;

		/// <summary>
		/// Private field that stores a reference to the <see cref="Font"/>
		/// object for this <see cref="FontSpec"/>.  This font object will be at
		/// the actual drawn size <see cref="_scaledSize"/> according to the current
		/// size of the <see cref="GraphPane"/>.  Use the public method
		/// <see cref="GetFont"/> to access this font object.
		/// </summary>
		/// <value>A reference to a <see cref="Font"/> object</value>
		private Font _font;

		/// <summary>
		/// Private field that determines if the <see cref="FontSpec" /> will be
		/// displayed using anti-aliasing logic.
		/// Use the public property <see cref="IsAntiAlias" /> to access this value.
		/// </summary>
		private bool _isAntiAlias;
		/// <summary>
		/// Private field that determines if the <see cref="FontSpec" /> will be
		/// displayed with a drop shadow.
		/// Use the public property <see cref="IsDropShadow" /> to access this value.
		/// </summary>
		private bool _isDropShadow;
		/// <summary>
		/// Private field that determines the color of the dropshadow for this
		/// <see cref="FontSpec" />.
		/// Use the public property <see cref="DropShadowColor" /> to access this value.
		/// </summary>
		private Color _dropShadowColor;
		/// <summary>
		/// Private field that determines the offset angle of the dropshadow for this
		/// <see cref="FontSpec" />.
		/// Use the public property <see cref="DropShadowAngle" /> to access this value.
		/// </summary>
		private float _dropShadowAngle;
		/// <summary>
		/// Private field that determines the offset distance of the dropshadow for this
		/// <see cref="FontSpec" />.
		/// Use the public property <see cref="DropShadowOffset" /> to access this value.
		/// </summary>
		private float _dropShadowOffset;

		/// <summary>
		/// Private field that stores a reference to the <see cref="Font"/>
		/// object that will be used for superscripts.  This font object will be a
		/// fraction of the <see cref="FontSpec"/> <see cref="_scaledSize"/>,
		/// based on the value of <see cref="Default.SuperSize"/>.  This
		/// property is internal, and has no public access.
		/// </summary>
		/// <value>A reference to a <see cref="Font"/> object</value>
		private Font _superScriptFont;

		/// <summary>
		/// Private field that temporarily stores the scaled size of the font for this
		/// <see cref="FontSpec"/> object.  This represents the actual on-screen
		/// size, rather than the <see cref="Size"/> that represents the reference
		/// size for a "full-sized" <see cref="GraphPane"/>.
		/// </summary>
		/// <value>The size of the font, measured in points (1/72 inch).</value>
		private float _scaledSize;
	#endregion

	#region Defaults
		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="FontSpec"/> class.
		/// </summary>
		public struct Default
		{
			/// <summary>
			/// The default size fraction of the superscript font, expressed as a fraction
			/// of the size of the main font.
			/// </summary>
			public static float SuperSize = 0.85F;
			/// <summary>
			/// The default shift fraction of the superscript, expressed as a
			/// fraction of the superscripted character height.  This is the height
			/// above the main font (a zero shift means the main font and the superscript
			/// font have the tops aligned).
			/// </summary>
			public static float SuperShift = 0.4F;
			/// <summary>
			/// The default color for filling in the background of the text block
			/// (<see cref="ZedGraph.Fill.Color"/> property).
			/// </summary>
			public static Color FillColor = Color.White;
			/// <summary>
			/// The default custom brush for filling in this <see cref="FontSpec"/>
			/// (<see cref="ZedGraph.Fill.Brush"/> property).
			/// </summary>
			public static Brush FillBrush = null;
			/// <summary>
			/// The default fill mode for this <see cref="FontSpec"/>
			/// (<see cref="ZedGraph.Fill.Type"/> property).
			/// </summary>
			public static FillType FillType = FillType.Solid;
			/// <summary>
			/// Default value for the alignment with which this
			/// <see cref="FontSpec"/> object is drawn.  This alignment really only
			/// affects multi-line strings.
			/// </summary>
			/// <value>A <see cref="StringAlignment"/> enumeration.</value>
			public static StringAlignment StringAlignment = StringAlignment.Center;

			/// <summary>
			/// Default value for <see cref="FontSpec.IsDropShadow"/>, which determines
			/// if the drop shadow is displayed for this <see cref="FontSpec" />.
			/// </summary>
			public static bool IsDropShadow = false;
			/// <summary>
			/// Default value for <see cref="FontSpec.IsAntiAlias"/>, which determines
			/// if anti-aliasing logic is used for this <see cref="FontSpec" />.
			/// </summary>
			public static bool IsAntiAlias = false;
			/// <summary>
			/// Default value for <see cref="FontSpec.DropShadowColor"/>, which determines
			/// the color of the drop shadow for this <see cref="FontSpec" />.
			/// </summary>
			public static Color DropShadowColor = Color.Black;
			/// <summary>
			/// Default value for <see cref="FontSpec.DropShadowAngle"/>, which determines
			/// the offset angle of the drop shadow for this <see cref="FontSpec" />.
			/// </summary>
			public static float DropShadowAngle = 45f;
			/// <summary>
			/// Default value for <see cref="FontSpec.DropShadowOffset"/>, which determines
			/// the offset distance of the drop shadow for this <see cref="FontSpec" />.
			/// </summary>
			public static float DropShadowOffset = 0.05f;

		}
	#endregion

	#region Properties
		/// <summary>
		/// The color of the font characters for this <see cref="FontSpec"/>.
		/// Note that the border and background
		/// colors are set using the <see cref="ZedGraph.LineBase.Color"/> and
		/// <see cref="ZedGraph.Fill.Color"/> properties, respectively.
		/// </summary>
		/// <value>A system <see cref="System.Drawing.Color"/> reference.</value>
		public Color FontColor
		{
			get { return _fontColor; }
			set { _fontColor = value; }
		}
		/// <summary>
		/// The font family name for this <see cref="FontSpec"/>.
		/// </summary>
		/// <value>A text string with the font family name, e.g., "Arial"</value>
		public string Family
		{
			get { return _family; }
			set
			{
				if ( value != _family )
				{
					_family = value;
					Remake( _scaledSize / _size, this.Size, ref _scaledSize, ref _font );
				}
			}
		}
		/// <summary>
		/// Determines whether this <see cref="FontSpec"/> is
		/// drawn with bold typeface.
		/// </summary>
		/// <value>A boolean value, true for bold, false for normal</value>
		public bool IsBold
		{
			get { return _isBold; }
			set
			{
				if ( value != _isBold )
				{
					_isBold = value;
					Remake( _scaledSize / _size, this.Size, ref _scaledSize, ref _font );
				}
			}
		}
		/// <summary>
		/// Determines whether this <see cref="FontSpec"/> is
		/// drawn with italic typeface.
		/// </summary>
		/// <value>A boolean value, true for italic, false for normal</value>
		public bool IsItalic
		{
			get { return _isItalic; }
			set
			{
				if ( value != _isItalic )
				{
					_isItalic = value;
					Remake( _scaledSize / _size, this.Size, ref _scaledSize, ref _font );
				}
			}
		}
		/// <summary>
		/// Determines whether this <see cref="FontSpec"/> is
		/// drawn with underlined typeface.
		/// </summary>
		/// <value>A boolean value, true for underline, false for normal</value>
		public bool IsUnderline
		{
			get { return _isUnderline; }
			set
			{
				if ( value != _isUnderline )
				{
					_isUnderline = value;
					Remake( _scaledSize / _size, this.Size, ref _scaledSize, ref _font );
				}
			}
		}
		/// <summary>
		/// The angle at which this <see cref="FontSpec"/> object is drawn.
		/// </summary>
		/// <value>The angle of the font, measured in anti-clockwise degrees from
		/// horizontal.  Negative values are permitted.</value>
		public float Angle
		{
			get { return _angle; }
			set { _angle = value; }
		}

		/// <summary>
		/// Determines the alignment with which this
		/// <see cref="FontSpec"/> object is drawn.  This alignment really only
		/// affects multi-line strings.
		/// </summary>
		/// <value>A <see cref="StringAlignment"/> enumeration.</value>
		public StringAlignment StringAlignment
		{
			get { return _stringAlignment; }
			set { _stringAlignment = value; }
		}

		/// <summary>
		/// The size of the font for this <see cref="FontSpec"/> object.
		/// </summary>
		/// <value>The size of the font, measured in points (1/72 inch).</value>
		public float Size
		{
			get { return _size; }
			set
			{
				if ( value != _size )
				{
					Remake( _scaledSize / _size * value, _size, ref _scaledSize,
								ref _font );
					_size = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the <see cref="Border"/> class used to draw the border border
		/// around this text.
		/// </summary>
		public Border Border
		{
			get { return _border; }
			set { _border = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="ZedGraph.Fill"/> data for this
		/// <see cref="FontSpec"/>, which controls how the background
		/// behind the text is filled.
		/// </summary>
		public Fill Fill
		{
			get { return _fill; }
			set { _fill = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines if the <see cref="FontSpec" /> will be
		/// drawn using anti-aliasing logic within GDI+.
		/// </summary>
		/// <remarks>
		/// If this property is set to true, it will override the current setting of
		/// <see cref="Graphics.SmoothingMode" /> by setting the value temporarily to
		/// <see cref="SmoothingMode.HighQuality" />.  If this property is set to false,
		/// the the current setting of <see cref="Graphics.SmoothingMode" /> will be
		/// left as-is.
		/// </remarks>
		public bool IsAntiAlias
		{
			get { return _isAntiAlias; }
			set { _isAntiAlias = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines if the <see cref="FontSpec" /> will be
		/// displayed with a drop shadow.
		/// </summary>
		/// <seealso cref="DropShadowColor" />
		/// <seealso cref="DropShadowAngle" />
		/// <seealso cref="DropShadowOffset" />
		public bool IsDropShadow
		{
			get { return _isDropShadow; }
			set { _isDropShadow = value; }
		}
		/// <summary>
		/// Gets or sets the color of the drop shadow for this <see cref="FontSpec" />.
		/// </summary>
		/// <remarks>
		/// This value only applies if <see cref="IsDropShadow" /> is true.
		/// </remarks>
		/// <seealso cref="IsDropShadow" />
		/// <seealso cref="DropShadowAngle" />
		/// <seealso cref="DropShadowOffset" />
		public Color DropShadowColor
		{
			get { return _dropShadowColor; }
			set { _dropShadowColor = value; }
		}
		/// <summary>
		/// Gets or sets the offset angle of the drop shadow for this <see cref="FontSpec" />.
		/// </summary>
		/// <remarks>
		/// This value only applies if <see cref="IsDropShadow" /> is true.
		/// </remarks>
		/// <value>The angle, measured in anti-clockwise degrees from
		/// horizontal.  Negative values are permitted.</value>
		/// <seealso cref="IsDropShadow" />
		/// <seealso cref="DropShadowColor" />
		/// <seealso cref="DropShadowOffset" />
		public float DropShadowAngle
		{
			get { return _dropShadowAngle; }
			set { _dropShadowAngle = value; }
		}
		/// <summary>
		/// Gets or sets the offset distance of the drop shadow for this <see cref="FontSpec" />.
		/// </summary>
		/// <remarks>
		/// This value only applies if <see cref="IsDropShadow" /> is true.
		/// </remarks>
		/// <value>The offset distance, measured as a fraction of the scaled font height.</value>
		/// <seealso cref="IsDropShadow" />
		/// <seealso cref="DropShadowColor" />
		/// <seealso cref="DropShadowAngle" />
		public float DropShadowOffset
		{
			get { return _dropShadowOffset; }
			set { _dropShadowOffset = value; }
		}

	#endregion

	#region Constructors

		/// <summary>
		/// Construct a <see cref="FontSpec"/> object with default properties.
		/// </summary>
		public FontSpec()
			: this( "Arial", 12, Color.Black, false, false, false )
		{
		}

		/// <summary>
		/// Construct a <see cref="FontSpec"/> object with the given properties.  All other properties
		/// are defaulted according to the values specified in the <see cref="Default"/>
		/// default class.
		/// </summary>
		/// <param name="family">A text string representing the font family
		/// (default is "Arial")</param>
		/// <param name="size">A size of the font in points.  This size will be scaled
		/// based on the ratio of the <see cref="PaneBase.Rect"/> dimension to the
		/// <see cref="PaneBase.BaseDimension"/> of the <see cref="GraphPane"/> object. </param>
		/// <param name="color">The color with which to render the font</param>
		/// <param name="isBold">true for a bold typeface, false otherwise</param>
		/// <param name="isItalic">true for an italic typeface, false otherwise</param>
		/// <param name="isUnderline">true for an underlined font, false otherwise</param>
		public FontSpec( string family, float size, Color color, bool isBold,
			bool isItalic, bool isUnderline )
		{
			Init( family, size, color, isBold, isItalic, isUnderline,
					Default.FillColor, Default.FillBrush, Default.FillType );
		}

		/// <summary>
		/// Construct a <see cref="FontSpec"/> object with the given properties.  All other properties
		/// are defaulted according to the values specified in the <see cref="Default"/>
		/// default class.
		/// </summary>
		/// <param name="family">A text string representing the font family
		/// (default is "Arial")</param>
		/// <param name="size">A size of the font in points.  This size will be scaled
		/// based on the ratio of the <see cref="PaneBase.Rect"/> dimension to the
		/// <see cref="PaneBase.BaseDimension"/> of the <see cref="GraphPane"/> object. </param>
		/// <param name="color">The color with which to render the font</param>
		/// <param name="isBold">true for a bold typeface, false otherwise</param>
		/// <param name="isItalic">true for an italic typeface, false otherwise</param>
		/// <param name="isUnderline">true for an underlined font, false otherwise</param>
		/// <param name="fillColor">The <see cref="Color"/> to use for filling in the text background</param>
		/// <param name="fillBrush">The <see cref="Brush"/> to use for filling in the text background</param>
		/// <param name="fillType">The <see cref="ZedGraph.FillType"/> to use for the
		/// text background</param>
		public FontSpec( string family, float size, Color color, bool isBold,
							bool isItalic, bool isUnderline, Color fillColor, Brush fillBrush,
							FillType fillType )
		{
			Init( family, size, color, isBold, isItalic, isUnderline,
					fillColor, fillBrush, fillType );
		}

		private void Init( string family, float size, Color color, bool isBold,
			bool isItalic, bool isUnderline, Color fillColor, Brush fillBrush,
			FillType fillType )
		{
			_fontColor = color;
			_family = family;
			_isBold = isBold;
			_isItalic = isItalic;
			_isUnderline = isUnderline;
			_size = size;
			_angle = 0F;

			_isAntiAlias = Default.IsAntiAlias;
			_stringAlignment = Default.StringAlignment;
			_isDropShadow = Default.IsDropShadow;
			_dropShadowColor = Default.DropShadowColor;
			_dropShadowAngle = Default.DropShadowAngle;
			_dropShadowOffset = Default.DropShadowOffset;

			_fill = new Fill( fillColor, fillBrush, fillType );
			_border = new Border( true, Color.Black, 1.0F );

			_scaledSize = -1;
			Remake( 1.0F, _size, ref _scaledSize, ref _font );
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The FontSpec object from which to copy</param>
		public FontSpec( FontSpec rhs )
		{
			_fontColor = rhs.FontColor;
			_family = rhs.Family;
			_isBold = rhs.IsBold;
			_isItalic = rhs.IsItalic;
			_isUnderline = rhs.IsUnderline;
			_fill = rhs.Fill.Clone();
			_border = rhs.Border.Clone();
			_isAntiAlias = rhs._isAntiAlias;

			_stringAlignment = rhs.StringAlignment;
			_angle = rhs.Angle;
			_size = rhs.Size;

			_isDropShadow = rhs._isDropShadow;
			_dropShadowColor = rhs._dropShadowColor;
			_dropShadowAngle = rhs._dropShadowAngle;
			_dropShadowOffset = rhs._dropShadowOffset;

			_scaledSize = rhs._scaledSize;
			Remake( 1.0F, _size, ref _scaledSize, ref _font );
		}

		/// <summary>
		/// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
		/// calling the typed version of <see cref="Clone" />
		/// </summary>
		/// <returns>A deep copy of this object</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Typesafe, deep-copy clone method.
		/// </summary>
		/// <returns>A new, independent copy of this class</returns>
		public FontSpec Clone()
		{
			return new FontSpec( this );
		}

	#endregion

	#region Serialization
		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		// Change to 2 with addition of isDropShadow, dropShadowColor, dropShadowAngle, dropShadowOffset
		// changed to 10 with the version 5 refactor -- not backwards compatible
		public const int schema = 10;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected FontSpec( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_fontColor = (Color)info.GetValue( "fontColor", typeof( Color ) );
			_family = info.GetString( "family" );
			_isBold = info.GetBoolean( "isBold" );
			_isItalic = info.GetBoolean( "isItalic" );
			_isUnderline = info.GetBoolean( "isUnderline" );
			_isAntiAlias = info.GetBoolean( "isAntiAlias" );

			_fill = (Fill)info.GetValue( "fill", typeof( Fill ) );
			_border = (Border)info.GetValue( "border", typeof( Border ) );
			_angle = info.GetSingle( "angle" );
			_stringAlignment = (StringAlignment)info.GetValue( "stringAlignment", typeof( StringAlignment ) );
			_size = info.GetSingle( "size" );

			_isDropShadow = info.GetBoolean( "isDropShadow" );
			_dropShadowColor = (Color)info.GetValue( "dropShadowColor", typeof( Color ) );
			_dropShadowAngle = info.GetSingle( "dropShadowAngle" );
			_dropShadowOffset = info.GetSingle( "dropShadowOffset" );

			_scaledSize = -1;
			Remake( 1.0F, _size, ref _scaledSize, ref _font );
		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		[SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "schema", schema );
			info.AddValue( "fontColor", _fontColor );
			info.AddValue( "family", _family );
			info.AddValue( "isBold", _isBold );
			info.AddValue( "isItalic", _isItalic );
			info.AddValue( "isUnderline", _isUnderline );
			info.AddValue( "isAntiAlias", _isAntiAlias );

			info.AddValue( "fill", _fill );
			info.AddValue( "border", _border );
			info.AddValue( "angle", _angle );
			info.AddValue( "stringAlignment", _stringAlignment );
			info.AddValue( "size", _size );

			info.AddValue( "isDropShadow", _isDropShadow );
			info.AddValue( "dropShadowColor", _dropShadowColor );
			info.AddValue( "dropShadowAngle", _dropShadowAngle );
			info.AddValue( "dropShadowOffset", _dropShadowOffset );
		}
	#endregion

	#region Font Construction Methods
		/// <summary>
		/// Recreate the font based on a new scaled size.  The font
		/// will only be recreated if the scaled size has changed by
		/// at least 0.1 points.
		/// </summary>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="size">The unscaled size of the font, in points</param>
		/// <param name="scaledSize">The scaled size of the font, in points</param>
		/// <param name="font">A reference to the <see cref="Font"/> object</param>
		private void Remake( float scaleFactor, float size, ref float scaledSize, ref Font font )
		{
			float newSize = size * scaleFactor;

			float oldSize = ( font == null ) ? 0.0f : font.Size;

			// Regenerate the font only if the size has changed significantly
			if ( font == null ||
					Math.Abs( newSize - oldSize ) > 0.1 ||
					font.Name != this.Family ||
					font.Bold != _isBold ||
					font.Italic != _isItalic ||
					font.Underline != _isUnderline )
			{
				FontStyle style = FontStyle.Regular;
				style = ( _isBold ? FontStyle.Bold : style ) |
							( _isItalic ? FontStyle.Italic : style ) |
							 ( _isUnderline ? FontStyle.Underline : style );

				scaledSize = size * (float)scaleFactor;
				font = new Font( _family, scaledSize, style, GraphicsUnit.World );
			}
		}

		/// <summary>
		/// Get the <see cref="Font"/> class for the current scaled font.
		/// </summary>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>Returns a reference to a <see cref="Font"/> object
		/// with a size of <see cref="_scaledSize"/>, and font <see cref="Family"/>.
		/// </returns>
		public Font GetFont( float scaleFactor )
		{
			Remake( scaleFactor, this.Size, ref _scaledSize, ref _font );
			return _font;
		}
	#endregion

	#region Rendering Methods
		/// <summary>
		/// Render the specified <paramref name="text"/> to the specifed
		/// <see cref="Graphics"/> device.  The text, border, and fill options
		/// will be rendered as required.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="PaneBase"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="text">A string value containing the text to be
		/// displayed.  This can be multiple lines, separated by newline ('\n')
		/// characters</param>
		/// <param name="x">The X location to display the text, in screen
		/// coordinates, relative to the horizontal (<see cref="AlignH"/>)
		/// alignment parameter <paramref name="alignH"/></param>
		/// <param name="y">The Y location to display the text, in screen
		/// coordinates, relative to the vertical (<see cref="AlignV"/>
		/// alignment parameter <paramref name="alignV"/></param>
		/// <param name="alignH">A horizontal alignment parameter specified
		/// using the <see cref="AlignH"/> enum type</param>
		/// <param name="alignV">A vertical alignment parameter specified
		/// using the <see cref="AlignV"/> enum type</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		public void Draw( Graphics g, PaneBase pane, string text, float x,
			float y, AlignH alignH, AlignV alignV,
			float scaleFactor )
		{
			this.Draw( g, pane, text, x, y, alignH, alignV,
						scaleFactor, new SizeF() );
		}

		/// <summary>
		/// Render the specified <paramref name="text"/> to the specifed
		/// <see cref="Graphics"/> device.  The text, border, and fill options
		/// will be rendered as required.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="PaneBase"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="text">A string value containing the text to be
		/// displayed.  This can be multiple lines, separated by newline ('\n')
		/// characters</param>
		/// <param name="x">The X location to display the text, in screen
		/// coordinates, relative to the horizontal (<see cref="AlignH"/>)
		/// alignment parameter <paramref name="alignH"/></param>
		/// <param name="y">The Y location to display the text, in screen
		/// coordinates, relative to the vertical (<see cref="AlignV"/>
		/// alignment parameter <paramref name="alignV"/></param>
		/// <param name="alignH">A horizontal alignment parameter specified
		/// using the <see cref="AlignH"/> enum type</param>
		/// <param name="alignV">A vertical alignment parameter specified
		/// using the <see cref="AlignV"/> enum type</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="layoutArea">The limiting area (<see cref="SizeF"/>) into which the text
		/// must fit.  The actual rectangle may be smaller than this, but the text will be wrapped
		/// to accomodate the area.</param>
		public void Draw( Graphics g, PaneBase pane, string text, float x,
			float y, AlignH alignH, AlignV alignV,
			float scaleFactor, SizeF layoutArea )
		{
			// make sure the font size is properly scaled
			//Remake( scaleFactor, this.Size, ref this.scaledSize, ref this.font );

			SmoothingMode sModeSave = g.SmoothingMode;
			TextRenderingHint sHintSave = g.TextRenderingHint;
			if ( _isAntiAlias )
			{
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.TextRenderingHint = TextRenderingHint.AntiAlias;
			}

			SizeF sizeF;
			if ( layoutArea.IsEmpty )
				sizeF = MeasureString( g, text, scaleFactor );
			else
				sizeF = MeasureString( g, text, scaleFactor, layoutArea );

			// Save the old transform matrix for later restoration
			Matrix saveMatrix = g.Transform;
			g.Transform = SetupMatrix( g.Transform, x, y, sizeF, alignH, alignV, _angle );

			// Create a rectangle representing the border around the
			// text.  Note that, while the text is drawn based on the
			// TopCenter position, the rectangle is drawn based on
			// the TopLeft position.  Therefore, move the rectangle
			// width/2 to the left to align it properly
			RectangleF rectF = new RectangleF( -sizeF.Width / 2.0F, 0.0F,
								sizeF.Width, sizeF.Height );

			// If the background is to be filled, fill it
			_fill.Draw( g, rectF );

			// Draw the border around the text if required
			_border.Draw( g, pane, scaleFactor, rectF );

			// make a center justified StringFormat alignment
			// for drawing the text
			StringFormat strFormat = new StringFormat();
			strFormat.Alignment = _stringAlignment;
			//			if ( this.stringAlignment == StringAlignment.Far )
			//				g.TranslateTransform( sizeF.Width / 2.0F, 0F, MatrixOrder.Prepend );
			//			else if ( this.stringAlignment == StringAlignment.Near )
			//				g.TranslateTransform( -sizeF.Width / 2.0F, 0F, MatrixOrder.Prepend );


			// Draw the drop shadow text.  Note that the coordinate system
			// is set up such that 0,0 is at the location where the
			// CenterTop of the text needs to be.
			if ( _isDropShadow )
			{
				float xShift = (float)( Math.Cos( _dropShadowAngle ) *
							_dropShadowOffset * _font.Height );
				float yShift = (float)( Math.Sin( _dropShadowAngle ) *
							_dropShadowOffset * _font.Height );
				RectangleF rectD = rectF;
				rectD.Offset( xShift, yShift );
				// make a solid brush for rendering the font itself
				using ( SolidBrush brushD = new SolidBrush( _dropShadowColor ) )
					g.DrawString( text, _font, brushD, rectD, strFormat );
			}

			// make a solid brush for rendering the font itself
			using ( SolidBrush brush = new SolidBrush( _fontColor ) )
			{
				// Draw the actual text.  Note that the coordinate system
				// is set up such that 0,0 is at the location where the
				// CenterTop of the text needs to be.
				//RectangleF layoutArea = new RectangleF( 0.0F, 0.0F, sizeF.Width, sizeF.Height );
				g.DrawString( text, _font, brush, rectF, strFormat );
			}

			// Restore the transform matrix back to original
			g.Transform = saveMatrix;

			g.SmoothingMode = sModeSave;
			g.TextRenderingHint = sHintSave;
		}

		/// <summary>
		/// Render the specified <paramref name="text"/> to the specifed
		/// <see cref="Graphics"/> device.  The text, border, and fill options
		/// will be rendered as required.  This special case method will show the
		/// specified text as a power of 10, using the <see cref="Default.SuperSize"/>
		/// and <see cref="Default.SuperShift"/>.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="text">A string value containing the text to be
		/// displayed.  This can be multiple lines, separated by newline ('\n')
		/// characters</param>
		/// <param name="x">The X location to display the text, in screen
		/// coordinates, relative to the horizontal (<see cref="AlignH"/>)
		/// alignment parameter <paramref name="alignH"/></param>
		/// <param name="y">The Y location to display the text, in screen
		/// coordinates, relative to the vertical (<see cref="AlignV"/>
		/// alignment parameter <paramref name="alignV"/></param>
		/// <param name="alignH">A horizontal alignment parameter specified
		/// using the <see cref="AlignH"/> enum type</param>
		/// <param name="alignV">A vertical alignment parameter specified
		/// using the <see cref="AlignV"/> enum type</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		public void DrawTenPower( Graphics g, GraphPane pane, string text, float x,
			float y, AlignH alignH, AlignV alignV,
			float scaleFactor )
		{
			SmoothingMode sModeSave = g.SmoothingMode;
			TextRenderingHint sHintSave = g.TextRenderingHint;
			if ( _isAntiAlias )
			{
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.TextRenderingHint = TextRenderingHint.AntiAlias;
			}

			// make sure the font size is properly scaled
			Remake( scaleFactor, this.Size, ref _scaledSize, ref _font );
			float scaledSuperSize = _scaledSize * Default.SuperSize;
			Remake( scaleFactor, this.Size * Default.SuperSize, ref scaledSuperSize,
				ref _superScriptFont );

			// Get the width and height of the text
			SizeF size10 = g.MeasureString( "10", _font );
			SizeF sizeText = g.MeasureString( text, _superScriptFont );
			SizeF totSize = new SizeF( size10.Width + sizeText.Width,
									size10.Height + sizeText.Height * Default.SuperShift );
			float charWidth = g.MeasureString( "x", _superScriptFont ).Width;

			// Save the old transform matrix for later restoration
			Matrix saveMatrix = g.Transform;

			g.Transform = SetupMatrix( g.Transform, x, y, totSize, alignH, alignV, _angle );

			// make a center justified StringFormat alignment
			// for drawing the text
			StringFormat strFormat = new StringFormat();
			strFormat.Alignment = _stringAlignment;

			// Create a rectangle representing the border around the
			// text.  Note that, while the text is drawn based on the
			// TopCenter position, the rectangle is drawn based on
			// the TopLeft position.  Therefore, move the rectangle
			// width/2 to the left to align it properly
			RectangleF rectF = new RectangleF( -totSize.Width / 2.0F, 0.0F,
				totSize.Width, totSize.Height );

			// If the background is to be filled, fill it
			_fill.Draw( g, rectF );

			// Draw the border around the text if required
			_border.Draw( g, pane, scaleFactor, rectF );

			// make a solid brush for rendering the font itself
			using ( SolidBrush brush = new SolidBrush( _fontColor ) )
			{
				// Draw the actual text.  Note that the coordinate system
				// is set up such that 0,0 is at the location where the
				// CenterTop of the text needs to be.
				g.DrawString( "10", _font, brush,
								( -totSize.Width + size10.Width ) / 2.0F,
								sizeText.Height * Default.SuperShift, strFormat );
				g.DrawString( text, _superScriptFont, brush,
								( totSize.Width - sizeText.Width - charWidth ) / 2.0F,
								0.0F,
								strFormat );
			}

			// Restore the transform matrix back to original
			g.Transform = saveMatrix;

			g.SmoothingMode = sModeSave;
			g.TextRenderingHint = sHintSave;
		}
	#endregion

	#region Sizing Methods
		/// <summary>
		/// Get the height of the scaled font
		/// </summary>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>The scaled font height, in pixels</returns>
		public float GetHeight( float scaleFactor )
		{
			Remake( scaleFactor, this.Size, ref _scaledSize, ref _font );
			float height = _font.Height;
			if ( _isDropShadow )
				height += (float)( Math.Sin( _dropShadowAngle ) * _dropShadowOffset * _font.Height );
			return height;
		}
		/// <summary>
		/// Get the average character width of the scaled font.  The average width is
		/// based on the character 'x'
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>The scaled font width, in pixels</returns>
		public float GetWidth( Graphics g, float scaleFactor )
		{
			Remake( scaleFactor, this.Size, ref _scaledSize, ref _font );
			return g.MeasureString( "x", _font ).Width;
		}

		/// <summary>
		/// Get the total width of the specified text string
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="text">The text string for which the width is to be calculated
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>The scaled text width, in pixels</returns>
		public float GetWidth( Graphics g, string text, float scaleFactor )
		{
			Remake( scaleFactor, this.Size, ref _scaledSize, ref _font );
			float width = g.MeasureString( text, _font ).Width;
			if ( _isDropShadow )
				width += (float)( Math.Cos( _dropShadowAngle ) * _dropShadowOffset * _font.Height );
			return width;
		}
		/// <summary>
		/// Get a <see cref="SizeF"/> struct representing the width and height
		/// of the specified text string, based on the scaled font size
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="text">The text string for which the width is to be calculated
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>The scaled text dimensions, in pixels, in the form of
		/// a <see cref="SizeF"/> struct</returns>
		public SizeF MeasureString( Graphics g, string text, float scaleFactor )
		{
			Remake( scaleFactor, this.Size, ref _scaledSize, ref _font );
			SizeF size = g.MeasureString( text, _font );
			if ( _isDropShadow )
			{
				size.Width += (float)( Math.Cos( _dropShadowAngle ) *
								_dropShadowOffset * _font.Height );
				size.Height += (float)( Math.Sin( _dropShadowAngle ) *
								_dropShadowOffset * _font.Height );
			}
			return size;
		}

		/// <summary>
		/// Get a <see cref="SizeF"/> struct representing the width and height
		/// of the specified text string, based on the scaled font size, and using
		/// the specified <see cref="SizeF"/> as an outer limit.
		/// </summary>
		/// <remarks>
		/// This method will allow the text to wrap as necessary to fit the 
		/// <see paramref="layoutArea"/>.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="text">The text string for which the width is to be calculated
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="layoutArea">The limiting area (<see cref="SizeF"/>) into which the text
		/// must fit.  The actual rectangle may be smaller than this, but the text will be wrapped
		/// to accomodate the area.</param>
		/// <returns>The scaled text dimensions, in pixels, in the form of
		/// a <see cref="SizeF"/> struct</returns>
		public SizeF MeasureString( Graphics g, string text, float scaleFactor, SizeF layoutArea )
		{
			Remake( scaleFactor, this.Size, ref _scaledSize, ref _font );
			SizeF size = g.MeasureString( text, _font, layoutArea );
			if ( _isDropShadow )
			{
				size.Width += (float)( Math.Cos( _dropShadowAngle ) *
								_dropShadowOffset * _font.Height );
				size.Height += (float)( Math.Sin( _dropShadowAngle ) *
								_dropShadowOffset * _font.Height );
			}
			return size;
		}

		/// <summary>
		/// Get a <see cref="SizeF"/> struct representing the width and height
		/// of the bounding box for the specified text string, based on the scaled font size.
		/// </summary>
		/// <remarks>
		/// This routine differs from <see cref="MeasureString(Graphics,string,float)"/> in that it takes into
		/// account the rotation angle of the font, and gives the dimensions of the
		/// bounding box that encloses the text at the specified angle.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="text">The text string for which the width is to be calculated
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>The scaled text dimensions, in pixels, in the form of
		/// a <see cref="SizeF"/> struct</returns>
		public SizeF BoundingBox( Graphics g, string text, float scaleFactor )
		{
			return BoundingBox( g, text, scaleFactor, new SizeF() );
		}

		/// <summary>
		/// Get a <see cref="SizeF"/> struct representing the width and height
		/// of the bounding box for the specified text string, based on the scaled font size.
		/// </summary>
		/// <remarks>
		/// This routine differs from <see cref="MeasureString(Graphics,string,float)"/> in that it takes into
		/// account the rotation angle of the font, and gives the dimensions of the
		/// bounding box that encloses the text at the specified angle.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="text">The text string for which the width is to be calculated
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="layoutArea">The limiting area (<see cref="SizeF"/>) into which the text
		/// must fit.  The actual rectangle may be smaller than this, but the text will be wrapped
		/// to accomodate the area.</param>
		/// <returns>The scaled text dimensions, in pixels, in the form of
		/// a <see cref="SizeF"/> struct</returns>
		public SizeF BoundingBox( Graphics g, string text, float scaleFactor, SizeF layoutArea )
		{
			//Remake( scaleFactor, this.Size, ref this.scaledSize, ref this.font );
			SizeF s;
			if ( layoutArea.IsEmpty )
				s = MeasureString( g, text, scaleFactor );
			else
				s = MeasureString( g, text, scaleFactor, layoutArea );

			float cs = (float)Math.Abs( Math.Cos( _angle * Math.PI / 180.0 ) );
			float sn = (float)Math.Abs( Math.Sin( _angle * Math.PI / 180.0 ) );

			SizeF s2 = new SizeF( s.Width * cs + s.Height * sn,
				s.Width * sn + s.Height * cs );

			return s2;
		}

		/// <summary>
		/// Get a <see cref="SizeF"/> struct representing the width and height
		/// of the bounding box for the specified text string, based on the scaled font size.
		/// </summary>
		/// <remarks>
		/// This special case method will show the specified string as a power of 10,
		/// superscripted and downsized according to the
		/// <see cref="Default.SuperSize"/> and <see cref="Default.SuperShift"/>.
		/// This routine differs from <see cref="MeasureString(Graphics,string,float)"/> in that it takes into
		/// account the rotation angle of the font, and gives the dimensions of the
		/// bounding box that encloses the text at the specified angle.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="text">The text string for which the width is to be calculated
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>The scaled text dimensions, in pixels, in the form of
		/// a <see cref="SizeF"/> struct</returns>
		public SizeF BoundingBoxTenPower( Graphics g, string text, float scaleFactor )
		{
			//Remake( scaleFactor, this.Size, ref this.scaledSize, ref this.font );
			float scaledSuperSize = _scaledSize * Default.SuperSize;
			Remake( scaleFactor, this.Size * Default.SuperSize, ref scaledSuperSize,
				ref _superScriptFont );

			// Get the width and height of the text
			SizeF size10 = MeasureString( g, "10", scaleFactor );
			SizeF sizeText = g.MeasureString( text, _superScriptFont );

			if ( _isDropShadow )
			{
				sizeText.Width += (float)( Math.Cos( _dropShadowAngle ) *
							_dropShadowOffset * _superScriptFont.Height );
				sizeText.Height += (float)( Math.Sin( _dropShadowAngle ) *
							_dropShadowOffset * _superScriptFont.Height );
			}

			SizeF totSize = new SizeF( size10.Width + sizeText.Width,
				size10.Height + sizeText.Height * Default.SuperShift );


			float cs = (float)Math.Abs( Math.Cos( _angle * Math.PI / 180.0 ) );
			float sn = (float)Math.Abs( Math.Sin( _angle * Math.PI / 180.0 ) );

			SizeF s2 = new SizeF( totSize.Width * cs + totSize.Height * sn,
				totSize.Width * sn + totSize.Height * cs );

			return s2;
		}


		/// <summary>
		/// Determines if the specified screen point lies within the bounding box of
		/// the text, taking into account alignment and rotation parameters.
		/// </summary>
		/// <param name="pt">The screen point, in pixel units</param>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="text">A string value containing the text to be
		/// displayed.  This can be multiple lines, separated by newline ('\n')
		/// characters</param>
		/// <param name="x">The X location to display the text, in screen
		/// coordinates, relative to the horizontal (<see cref="AlignH"/>)
		/// alignment parameter <paramref name="alignH"/></param>
		/// <param name="y">The Y location to display the text, in screen
		/// coordinates, relative to the vertical (<see cref="AlignV"/>
		/// alignment parameter <paramref name="alignV"/></param>
		/// <param name="alignH">A horizontal alignment parameter specified
		/// using the <see cref="AlignH"/> enum type</param>
		/// <param name="alignV">A vertical alignment parameter specified
		/// using the <see cref="AlignV"/> enum type</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>true if the point lies within the bounding box, false otherwise</returns>
		public bool PointInBox( PointF pt, Graphics g, string text, float x,
			float y, AlignH alignH, AlignV alignV,
			float scaleFactor )
		{
			return PointInBox( pt, g, text, x, y, alignH, alignV, scaleFactor, new SizeF() );
		}

		/// <summary>
		/// Determines if the specified screen point lies within the bounding box of
		/// the text, taking into account alignment and rotation parameters.
		/// </summary>
		/// <param name="pt">The screen point, in pixel units</param>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="text">A string value containing the text to be
		/// displayed.  This can be multiple lines, separated by newline ('\n')
		/// characters</param>
		/// <param name="x">The X location to display the text, in screen
		/// coordinates, relative to the horizontal (<see cref="AlignH"/>)
		/// alignment parameter <paramref name="alignH"/></param>
		/// <param name="y">The Y location to display the text, in screen
		/// coordinates, relative to the vertical (<see cref="AlignV"/>
		/// alignment parameter <paramref name="alignV"/></param>
		/// <param name="alignH">A horizontal alignment parameter specified
		/// using the <see cref="AlignH"/> enum type</param>
		/// <param name="alignV">A vertical alignment parameter specified
		/// using the <see cref="AlignV"/> enum type</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="layoutArea">The limiting area (<see cref="SizeF"/>) into which the text
		/// must fit.  The actual rectangle may be smaller than this, but the text will be wrapped
		/// to accomodate the area.</param>
		/// <returns>true if the point lies within the bounding box, false otherwise</returns>
		public bool PointInBox( PointF pt, Graphics g, string text, float x,
			float y, AlignH alignH, AlignV alignV,
			float scaleFactor, SizeF layoutArea )
		{
			// make sure the font size is properly scaled
			Remake( scaleFactor, this.Size, ref _scaledSize, ref _font );

			// Get the width and height of the text
			SizeF sizeF;
			if ( layoutArea.IsEmpty )
				sizeF = g.MeasureString( text, _font );
			else
				sizeF = g.MeasureString( text, _font, layoutArea );

			// Create a bounding box rectangle for the text
			RectangleF rect = new RectangleF( new PointF( -sizeF.Width / 2.0F, 0.0F ), sizeF );

			// Build a transform matrix that inverts that drawing transform
			// in this manner, the point is brought back to the box, rather
			// than vice-versa.  This allows the container check to be a simple
			// RectangleF.Contains, since the rectangle won't be rotated.
			Matrix matrix = GetMatrix( x, y, sizeF, alignH, alignV, _angle );

			PointF[] pts = new PointF[1];
			pts[0] = pt;
			matrix.TransformPoints( pts );

			return rect.Contains( pts[0] );
		}

		private Matrix SetupMatrix( Matrix matrix, float x, float y, SizeF sizeF, AlignH alignH,
				AlignV alignV, float angle )
		{
			// Move the coordinate system to local coordinates
			// of this text object (that is, at the specified
			// x,y location)
			matrix.Translate( x, y, MatrixOrder.Prepend );

			// Rotate the coordinate system according to the 
			// specified angle of the FontSpec
			if ( _angle != 0.0F )
				matrix.Rotate( -angle, MatrixOrder.Prepend );

			// Since the text will be drawn by g.DrawString()
			// assuming the location is the TopCenter
			// (the Font is aligned using StringFormat to the
			// center so multi-line text is center justified),
			// shift the coordinate system so that we are
			// actually aligned per the caller specified position
			float xa, ya;
			if ( alignH == AlignH.Left )
				xa = sizeF.Width / 2.0F;
			else if ( alignH == AlignH.Right )
				xa = -sizeF.Width / 2.0F;
			else
				xa = 0.0F;

			if ( alignV == AlignV.Center )
				ya = -sizeF.Height / 2.0F;
			else if ( alignV == AlignV.Bottom )
				ya = -sizeF.Height;
			else
				ya = 0.0F;

			// Shift the coordinates to accomodate the alignment
			// parameters
			matrix.Translate( xa, ya, MatrixOrder.Prepend );

			return matrix;
		}

		private Matrix GetMatrix( float x, float y, SizeF sizeF, AlignH alignH, AlignV alignV,
							float angle )
		{
			// Build a transform matrix that inverts that drawing transform
			// in this manner, the point is brought back to the box, rather
			// than vice-versa.  This allows the container check to be a simple
			// RectangleF.Contains, since the rectangle won't be rotated.
			Matrix matrix = new Matrix();

			// In this case, the bounding box is anchored to the
			// top-left of the text box.  Handle the alignment
			// as needed.
			float xa, ya;
			if ( alignH == AlignH.Left )
				xa = sizeF.Width / 2.0F;
			else if ( alignH == AlignH.Right )
				xa = -sizeF.Width / 2.0F;
			else
				xa = 0.0F;

			if ( alignV == AlignV.Center )
				ya = -sizeF.Height / 2.0F;
			else if ( alignV == AlignV.Bottom )
				ya = -sizeF.Height;
			else
				ya = 0.0F;

			// Shift the coordinates to accomodate the alignment
			// parameters
			matrix.Translate( -xa, -ya, MatrixOrder.Prepend );

			// Rotate the coordinate system according to the 
			// specified angle of the FontSpec
			if ( angle != 0.0F )
				matrix.Rotate( angle, MatrixOrder.Prepend );

			// Move the coordinate system to local coordinates
			// of this text object (that is, at the specified
			// x,y location)
			matrix.Translate( -x, -y, MatrixOrder.Prepend );

			return matrix;
		}

		/// <summary>
		/// Returns a polygon that defines the bounding box of
		/// the text, taking into account alignment and rotation parameters.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="text">A string value containing the text to be
		/// displayed.  This can be multiple lines, separated by newline ('\n')
		/// characters</param>
		/// <param name="x">The X location to display the text, in screen
		/// coordinates, relative to the horizontal (<see cref="AlignH"/>)
		/// alignment parameter <paramref name="alignH"/></param>
		/// <param name="y">The Y location to display the text, in screen
		/// coordinates, relative to the vertical (<see cref="AlignV"/>
		/// alignment parameter <paramref name="alignV"/></param>
		/// <param name="alignH">A horizontal alignment parameter specified
		/// using the <see cref="AlignH"/> enum type</param>
		/// <param name="alignV">A vertical alignment parameter specified
		/// using the <see cref="AlignV"/> enum type</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="layoutArea">The limiting area (<see cref="SizeF"/>) into which the text
		/// must fit.  The actual rectangle may be smaller than this, but the text will be wrapped
		/// to accomodate the area.</param>
		/// <returns>A polygon of 4 points defining the area of this text</returns>
		public PointF[] GetBox( Graphics g, string text, float x,
				float y, AlignH alignH, AlignV alignV,
				float scaleFactor, SizeF layoutArea )
		{
			// make sure the font size is properly scaled
			Remake( scaleFactor, this.Size, ref _scaledSize, ref _font );

			// Get the width and height of the text
			SizeF sizeF;
			if ( layoutArea.IsEmpty )
				sizeF = g.MeasureString( text, _font );
			else
				sizeF = g.MeasureString( text, _font, layoutArea );

			// Create a bounding box rectangle for the text
			RectangleF rect = new RectangleF( new PointF( -sizeF.Width / 2.0F, 0.0F ), sizeF );

			Matrix matrix = new Matrix();
			SetupMatrix( matrix, x, y, sizeF, alignH, alignV, _angle );

			PointF[] pts = new PointF[4];
			pts[0] = new PointF( rect.Left, rect.Top );
			pts[1] = new PointF( rect.Right, rect.Top );
			pts[2] = new PointF( rect.Right, rect.Bottom );
			pts[3] = new PointF( rect.Left, rect.Bottom );
			matrix.TransformPoints( pts );

			return pts;
		}

	#endregion

	}
}
