using System;
using System.Collections;
using System.IO;
using com.drew.metadata;
using com.drew.imaging.jpg;
using com.utils;

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
	/// The Jpeg reader class
	/// </summary>
    public class JpegCommentReader : AbstractMetadataReader 
	{

        /// <summary>
		/// Creates a new JpegCommentReader for the specified Jpeg jpegFile.
		/// </summary>
        /// <param name="aFile">where to read</param>
		public JpegCommentReader(FileInfo aFile) : base(aFile, JpegSegmentReader.SEGMENT_COM)
		{
		}

        /// <summary>
        /// Constructor of the object
        /// </summary>
        /// <param name="data">the data to read</param>
        public JpegCommentReader(byte[] aData)
            : base(aData)
        {
        }

		/// <summary>
		/// Extracts aMetadata
		/// </summary>
		/// <param name="aMetadata">where to add aMetadata</param>
		/// <returns>the aMetadata found</returns>
		public override Metadata Extract(Metadata aMetadata) 
		{
			if (base.data == null) 
			{
				return aMetadata;
			}

			AbstractDirectory lcDirectory = aMetadata.GetDirectory("com.drew.metadata.jpeg.JpegCommentDirectory");
            string comment = Utils.Decode(base.data, true);
			lcDirectory.SetObject(JpegCommentDirectory.TAG_JPEG_COMMENT,comment);
			return aMetadata;
		}
	}
}