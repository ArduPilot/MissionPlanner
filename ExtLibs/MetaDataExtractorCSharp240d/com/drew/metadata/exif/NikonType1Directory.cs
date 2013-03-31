using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using com.drew.metadata;
using com.drew.lang;

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
namespace com.drew.metadata.exif
{
	public class NikonType1Directory : AbstractNikonTypeDirectory 
	{
		// TYPE1 is for E-Series cameras prior to (not including) E990
		public const int TAG_NIKON_TYPE1_UNKNOWN_1 = 0x0002;
		public const int TAG_NIKON_TYPE1_QUALITY = 0x0003;
		public const int TAG_NIKON_TYPE1_COLOR_MODE = 0x0004;
		public const int TAG_NIKON_TYPE1_IMAGE_ADJUSTMENT = 0x0005;
		public const int TAG_NIKON_TYPE1_CCD_SENSITIVITY = 0x0006;
		public const int TAG_NIKON_TYPE1_WHITE_BALANCE = 0x0007;
		public const int TAG_NIKON_TYPE1_FOCUS = 0x0008;
		public const int TAG_NIKON_TYPE1_UNKNOWN_2 = 0x0009;
		public const int TAG_NIKON_TYPE1_DIGITAL_ZOOM = 0x000A;
		public const int TAG_NIKON_TYPE1_CONVERTER = 0x000B;
		public const int TAG_NIKON_TYPE1_UNKNOWN_3 = 0x0F00;

		/// <summary>
		/// Constructor of the object.
		/// </summary>
        public NikonType1Directory()
            : base("NikonTypeMarkernote")
		{
			this.SetDescriptor(new NikonType1Descriptor(this));
		}
	}
}