using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using com.drew.metadata;
using com.drew.lang;
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
namespace com.drew.metadata.exif
{
	/// <summary>
	/// The Fuji Film Makernote Directory
	/// </summary>
	public class FujifilmDirectory : AbstractDirectory 
	{
		public const int TAG_FUJIFILM_MAKERNOTE_VERSION = 0x0000;
		public const int TAG_FUJIFILM_QUALITY = 0x1000;
		public const int TAG_FUJIFILM_SHARPNESS = 0x1001;
		public const int TAG_FUJIFILM_WHITE_BALANCE = 0x1002;
		public const int TAG_FUJIFILM_COLOR = 0x1003;
		public const int TAG_FUJIFILM_TONE = 0x1004;
		public const int TAG_FUJIFILM_FLASH_MODE = 0x1010;
		public const int TAG_FUJIFILM_FLASH_STRENGTH = 0x1011;
		public const int TAG_FUJIFILM_MACRO = 0x1020;
		public const int TAG_FUJIFILM_FOCUS_MODE = 0x1021;
		public const int TAG_FUJIFILM_SLOW_SYNCHRO = 0x1030;
		public const int TAG_FUJIFILM_PICTURE_MODE = 0x1031;
		public const int TAG_FUJIFILM_UNKNOWN_1 = 0x1032;
		public const int TAG_FUJIFILM_CONTINUOUS_TAKING_OR_AUTO_BRACKETTING =	0x1100;
		public const int TAG_FUJIFILM_UNKNOWN_2 = 0x1200;
		public const int TAG_FUJIFILM_BLUR_WARNING = 0x1300;
		public const int TAG_FUJIFILM_FOCUS_WARNING = 0x1301;
		public const int TAG_FUJIFILM_AE_WARNING = 0x1302;

		/// <summary>
		/// Constructor of the object.
		/// </summary>
        public FujifilmDirectory()
            : base("FujiFilmMarkernote")
		{
			this.SetDescriptor(new FujifilmDescriptor(this));
		}

	}
}