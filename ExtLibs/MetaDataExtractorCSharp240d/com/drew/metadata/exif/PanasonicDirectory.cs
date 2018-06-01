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
    /// The panasonic directory class.
    /// </summary>
	public class PanasonicDirectory : AbstractDirectory 
	{
        public const int TAG_PANASONIC_QUALITY_MODE = 0x0001;
        public const int TAG_PANASONIC_VERSION = 0x0002;

        /// <summary>
        ///  1 = On
        ///  2 = Off
        /// </summary>
        public const int TAG_PANASONIC_MACRO_MODE = 0x001C;

        /// <summary>
        ///  1 = Normal
        ///  2 = Portrait
        ///  9 = Macro 
        /// </summary>
        public const int TAG_PANASONIC_RECORD_MODE = 0x001F;
        public const int TAG_PANASONIC_PRINT_IMAGE_MATCHING_INFO = 0x0E00;

		/// <summary>
		/// Constructor of the object.
		/// </summary>
        public PanasonicDirectory()
            : base("PanasonicMarkernote")
		{
			this.SetDescriptor(new PanasonicDescriptor(this));
		}
	}
}
