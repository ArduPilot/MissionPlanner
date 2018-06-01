using System;
using System.IO;
using com.codec.jpeg;
using com.drew.metadata;
using com.drew.metadata.jpeg;
using com.drew.metadata.iptc;
using com.drew.metadata.exif;

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
namespace com.drew.imaging.tiff
{
	/// <summary>
	/// This class will extract MetaData from a picture.
	/// </summary>
    public class TiffMetadataReader
    {
        /// <summary>
        /// Constructor of the object.
        /// </summary>
        private TiffMetadataReader()
            : base()
        {
            throw new Exception("Do not use");
        }

        /// <summary>
        /// Constructor of the object.
        /// </summary>
        /// <param name="aFile">Where to read metadata from</param>
        /// <returns>a meta data</returns>
        public static Metadata ReadMetadata(FileInfo aFile)
        {
            Stream lcStream = null;
            Metadata lcMetadata = null;
            try
            {
                lcStream = aFile.OpenRead();
                lcMetadata = ReadMetadata(lcStream);

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (lcStream != null)
                {
                    lcStream.Close();
                    lcStream.Dispose();
                }
            }
            return lcMetadata;
        }

        /// <summary>
        /// Constructor of the object.
        /// </summary>
        /// <param name="aStream">Where to read information from. Caution, you are responsible for closing this stream.</param>
        /// <returns>a meta data object</returns>
        public static Metadata ReadMetadata(Stream aStream)
        {
            Metadata metadata = new Metadata();
            try
            {
                byte[] buffer = new byte[(int)aStream.Length];
                aStream.Read(buffer, 0, buffer.Length);

                new ExifReader(buffer).ExtractTiff(metadata);
            }
            catch (MetadataException e)
            {
                throw new TiffProcessingException(e);
            }
            return metadata;
        }
    }
}
