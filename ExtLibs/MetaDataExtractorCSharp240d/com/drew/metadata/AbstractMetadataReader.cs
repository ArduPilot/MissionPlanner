using System;
using System.Collections;
using System.IO;
using com.drew.lang;
using com.drew.metadata;
using com.drew.imaging.jpg;
using com.utils;
using System.Diagnostics;

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
namespace com.drew.metadata
{
    /// <summary>
    /// An abstract reader class
    /// </summary>
    public abstract class AbstractMetadataReader : IMetadataReader
    {
        /// <summary>
        /// The data segment
        /// </summary>
        protected readonly byte[] data;

        /// <summary>
        /// Creates a new Reader for the specified file.
        /// </summary>
        /// <param name="aFile">where to read</param>
        protected AbstractMetadataReader(FileInfo aFile, byte aSegment)
            : this(
            new JpegSegmentReader(aFile).ReadSegment(
            aSegment))
        {
        }

        /// <summary>
        /// Constructor of the object
        /// </summary>
        /// <param name="data">the data to read</param>
        protected AbstractMetadataReader(byte[] aData)
        {
            this.data = aData;
        }

        /// <summary>
        /// Performs the data extraction, returning a new instance of Metadata. 
        /// </summary>
        /// <returns>a new instance of Metadata</returns>
        public Metadata Extract()
        {
            return Extract(new Metadata());
        }

        /// <summary>
        /// Extracts aMetadata
        /// </summary>
        /// <param name="aMetadata">where to add aMetadata</param>
        /// <returns>the aMetadata found</returns>
        public abstract Metadata Extract(Metadata metadata);

        /// <summary>
        /// Returns an int calculated from two bytes of data at the specified lcOffset (MSB, LSB).
        /// </summary>
        /// <param name="anOffset">position within the data buffer to read first byte</param>
        /// <returns>the 32 bit int value, between 0x0000 and 0xFFFF</returns>
        protected virtual int Get32Bits(int anOffset)
        {
            if (anOffset >= this.data.Length)
            {
                throw new MetadataException("Attempt to read bytes from outside Iptc data buffer");
            }
            return ((this.data[anOffset] & 255) << 8) | (this.data[anOffset + 1] & 255);
        }

        /// <summary>
        /// Returns an int calculated from one byte of data at the specified lcOffset.
        /// </summary>
        /// <param name="anOffset">position within the data buffer to read byte</param>
        /// <returns>the 16 bit int value, between 0x00 and 0xFF</returns>
        protected virtual int Get16Bits(int anOffset)
        {
            if (anOffset >= this.data.Length)
            {
                throw new MetadataException("Attempt to read bytes from outside Jpeg segment data buffer");
            }

            return (this.data[anOffset] & 255);
        }
   }
}