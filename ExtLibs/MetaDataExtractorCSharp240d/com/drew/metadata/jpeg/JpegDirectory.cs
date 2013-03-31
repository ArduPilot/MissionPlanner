using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using com.drew.metadata;
using com.utils.bundle;

/// <summary>
/// This class was first written by Drew Noakes in Java.
///
/// This is public domain software - that is, you can do whatever you want
/// with it, and include it software that is licensed under the GNU or the
/// BSD license, or whatever other licence you choose, including proprietary
/// closed source licenses.  I do ask that you leave this lcHeader in tact.
///
/// If you make modifications to this code that you think would benefit the
/// wider community, please send me a copy and I'll post it on my site.
///
/// If you make use of this code, Drew Noakes will appreciate hearing 
/// about it: <a href="mailto:drew@drewnoakes.com">drew@drewnoakes.com</a>
///
/// Latest Java version of this software kept at 
/// <a href="http://drewnoakes.com">http://drewnoakes.com/</a>
///
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com.drew.metadata.jpeg
{
	/// <summary>
	/// The Jpeg Directory class
	/// </summary>
	public class JpegDirectory : AbstractDirectory 
	{
		/// <summary>
		/// This is in bits/sample, usually 8 (12 and 16 not supported by most software).
		/// </summary>
		public const int TAG_JPEG_DATA_PRECISION = 0;

		/// <summary>
		/// The image'str height.  Necessary for decoding the image, so it should always be there.
		/// </summary>
		public const int TAG_JPEG_IMAGE_HEIGHT = 1;

		/// <summary>
		/// The image'str width.  Necessary for decoding the image, so it should always be there.
		/// </summary>
		public const int TAG_JPEG_IMAGE_WIDTH = 3;

		/// <summary>
		/// Usually 1 = grey scaled, 3 = color YcbCr or YIQ, 4 = color CMYK Each component TAG_COMPONENT_DATA_[1-4], 
		/// has the following meaning: component Id(1byte)(1 = Y, 2 = Cb, 3 = Cr, 4 = I, 5 = Q), 
		/// sampling factors (1byte) (bit 0-3 vertical., 4-7 horizontal.), 
		/// quantization table number (1 byte).
		/// This info is from http://www.funducode.com/freec/Fileformats/format3/format3b.htm
		/// </summary>
		public const int TAG_JPEG_NUMBER_OF_COMPONENTS = 5;

		// NOTE!  Component tag type int values must increment in steps of 1

		/// <summary>
		/// the first of a possible 4 color components.  Number of components specified in TAG_JPEG_NUMBER_OF_COMPONENTS.
		/// </summary>
		public const int TAG_JPEG_COMPONENT_DATA_1 = 6;

		/// <summary>
		/// the second of a possible 4 color components.  Number of components specified in TAG_JPEG_NUMBER_OF_COMPONENTS.
		/// </summary>
		public const int TAG_JPEG_COMPONENT_DATA_2 = 7;

		/// <summary>
		/// the third of a possible 4 color components.  Number of components specified in TAG_JPEG_NUMBER_OF_COMPONENTS.
		/// </summary>
		public const int TAG_JPEG_COMPONENT_DATA_3 = 8;

		/// <summary>
		/// the fourth of a possible 4 color components.  Number of components specified in TAG_JPEG_NUMBER_OF_COMPONENTS.
		/// </summary>
		public const int TAG_JPEG_COMPONENT_DATA_4 = 9;

		/// <summary>
		/// Constructor of the object.
		/// </summary>
        public JpegDirectory()
            : base("JpegMarkernote")
		{
			this.SetDescriptor(new JpegDescriptor(this));
		}

		/// <summary>
		/// Gets the component
		/// </summary>
		/// <param name="componentNumber">The zero-based index of the component.  This number is normally between 0 and 3. Use GetNumberOfComponents for bounds-checking.</param>
		/// <returns>the JpegComponent</returns>
		public JpegComponent GetComponent(int componentNumber) 
		{
			int tagType = JpegDirectory.TAG_JPEG_COMPONENT_DATA_1 + componentNumber;

			JpegComponent component = (JpegComponent) GetObject(tagType);

			return component;
		}

		/// <summary>
		/// Gets image width
		/// </summary>
		/// <returns>image width</returns>
		public int GetImageWidth() 
		{
			return GetInt(JpegDirectory.TAG_JPEG_IMAGE_WIDTH);
		}

		/// <summary>
		/// Gets image height
		/// </summary>
		/// <returns>image height</returns>
		public int GetImageHeight() 
		{
			return GetInt(JpegDirectory.TAG_JPEG_IMAGE_HEIGHT);
		}

		/// <summary>
		/// Gets the Number Of Components
		/// </summary>
		/// <returns>the Number Of Components</returns>
		public int GetNumberOfComponents() 
		{
			return GetInt(JpegDirectory.TAG_JPEG_NUMBER_OF_COMPONENTS);
		}
	}
}
