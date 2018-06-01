using System;
using System.Collections;
using System.Text;
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
	/// Tag descriptor for almost every images
	/// </summary>
	public class ExifInteropDescriptor : AbstractTagDescriptor 
	{
		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="base.directory">a base.directory</param>
		public ExifInteropDescriptor(AbstractDirectory aDirectory) : base(aDirectory)
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
			switch(tagType) 
			{
				case ExifInteropDirectory.TAG_INTEROP_INDEX:
					return GetInteropIndexDescription();
				case ExifInteropDirectory.TAG_INTEROP_VERSION:
					return GetInteropVersionDescription();
				default:
					return base.directory.GetString(tagType);
			}
		}

		/// <summary>
		/// Returns the Interop Version Description. 
		/// </summary>
		/// <returns>the Interop Version Description.</returns>
		private string GetInteropVersionDescription()  
		{
			if (!base.directory.ContainsTag(ExifInteropDirectory.TAG_INTEROP_VERSION))
				return null;
			int[] ints =
				base.directory.GetIntArray(ExifInteropDirectory.TAG_INTEROP_VERSION);
			return ExifDescriptor.ConvertBytesToVersionString(ints);
		}

		/// <summary>
		/// Returns the Interop index Description. 
		/// </summary>
		/// <returns>the Interop index Description.</returns>
		private string GetInteropIndexDescription() 
		{
			if (!base.directory.ContainsTag(ExifInteropDirectory.TAG_INTEROP_INDEX))
				return null;
			string interopIndex =
				base.directory.GetString(ExifInteropDirectory.TAG_INTEROP_INDEX).Trim();
			if ("R98".Equals(interopIndex.ToUpper())) 
			{
				return BUNDLE["RECOMMENDED_EXIF_INTEROPERABILITY"];
			} 
			else 
			{
				return BUNDLE["UNKNOWN", interopIndex.ToString()];
			}
		}
	}
}
