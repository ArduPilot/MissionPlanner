using System;
using System.Collections;
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
	/// <summary>
	/// Tag descriptor for Panasonic
	/// </summary>
	public class PanasonicDescriptor : AbstractTagDescriptor 
	{
		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="aDirectory">a base.directory</param>
        public PanasonicDescriptor(AbstractDirectory aDirectory)
            : base(aDirectory)
		{		
		}

		/// <summary>
		/// Returns a descriptive value of the the specified tag for this image. 
		/// Where possible, known values will be substituted here in place of the raw tokens actually 
		/// kept in the Exif segment. 
		/// If no substitution is available, the value provided by GetString(int) will be returned.
		/// This and GetString(int) are the only 'get' methods that won't throw an exception.
		/// </summary>
		/// <param name="aTagType">the tag to find a description for</param>
		/// <returns>a description of the image'str value for the specified tag, or null if the tag hasn't been defined.</returns>
		public override string GetDescription(int tagType)  
		{
            switch (tagType)
            {
                case PanasonicDirectory.TAG_PANASONIC_MACRO_MODE:
                    return GetMacroModeDescription();
                case PanasonicDirectory.TAG_PANASONIC_RECORD_MODE:
                    return GetRecordModeDescription();
                case PanasonicDirectory.TAG_PANASONIC_PRINT_IMAGE_MATCHING_INFO:
                    return GetPrintImageMatchingInfoDescription();
                default:
                    return base.directory.GetString(tagType);
            }
        }

        /// <summary>
        /// Returns the print image matching info Description. 
        /// </summary>
        /// <returns>the print image matching info Description.</returns>
        private string GetPrintImageMatchingInfoDescription()
        {
            if (!base.directory.ContainsTag(PanasonicDirectory.TAG_PANASONIC_PRINT_IMAGE_MATCHING_INFO))
            {
                return null;
            }
            byte[] bytes = base.directory.GetByteArray(PanasonicDirectory.TAG_PANASONIC_PRINT_IMAGE_MATCHING_INFO);
            return BUNDLE["BYTES", bytes.Length.ToString()];
        }

        /// <summary>
        /// Returns the macro mode Description. 
        /// </summary>
        /// <returns>the macro mode Description.</returns>
        private string GetMacroModeDescription()
        {
            if (!base.directory.ContainsTag(PanasonicDirectory.TAG_PANASONIC_MACRO_MODE))
            {
                return null;
            }
            int value = base.directory.GetInt(PanasonicDirectory.TAG_PANASONIC_MACRO_MODE);
            switch (value)
            {
                case 1:
                    return BUNDLE["ON"];
                case 2:
                    return BUNDLE["OFF"];
                default:
                    return BUNDLE["UNKNOWN", value.ToString()];
            }
        }

        /// <summary>
        /// Returns record mode Description. 
        /// </summary>
        /// <returns>the record mode Description.</returns>
        private string GetRecordModeDescription()
        {
            if (!base.directory.ContainsTag(PanasonicDirectory.TAG_PANASONIC_RECORD_MODE))
            {
                return null;
            }
            int value = base.directory.GetInt(PanasonicDirectory.TAG_PANASONIC_RECORD_MODE);
            switch (value)
            {
                case 1:
                    return BUNDLE["NORMAL"];
                case 2:
                    return BUNDLE["PORTRAIT"];
                case 9:
                    return BUNDLE["MACRO"];
                default:
                    return BUNDLE["UNKNOWN", value.ToString()];
            }
        }


	}
}