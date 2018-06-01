using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


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
namespace com.drew.imaging.jpg
{
    [Serializable]
    public class JpegSegmentData
    {

        /// <summary>
        /// A map of byte[], keyed by the segment marker.
        /// </summary>
        private IDictionary<byte, IList<byte[]>> segmentDataMap;

        /// <summary>
        /// Constructor of the object.
        /// </summary>
        public JpegSegmentData()
            : base()
        {
            this.segmentDataMap = new Dictionary<byte, IList<byte[]>>(10);
        }

        /// <summary>
        /// Adds a segment.
        /// </summary>
        /// <param name="aSegmentMarker">the marker</param>
        /// <param name="aSegmentBytes">the value of the segment</param>
        public void AddSegment(byte aSegmentMarker, byte[] aSegmentBytes)
        {
            IList<byte[]> lcSegmentList = this.GetOrCreateSegmentList(aSegmentMarker);
            lcSegmentList.Add(aSegmentBytes);
        }

        /// <summary>
        /// Gets a segment using its key.
        /// </summary>
        /// <param name="aSegmentMarker">the segment'str key</param>
        /// <returns>The segment found or null if none found</returns>
        public byte[] GetSegment(byte aSegmentMarker)
        {
            return this.GetSegment(aSegmentMarker, 0);
        }

        /// <summary>
        /// Gets a segment using its marker and occurence value.
        /// </summary>
        /// <param name="aSegmentMarker">the segment'str marker</param>
        /// <param name="anOccurrence">the segment'str occurence</param>
        /// <returns>the segment found at the given occurence, or null if none found</returns>
        public byte[] GetSegment(byte aSegmentMarker, int anOccurrence)
        {
            IList<byte[]> lcSegmentList = this.GetSegmentList(aSegmentMarker);

            if (lcSegmentList == null || lcSegmentList.Count <= anOccurrence)
            {
                return null;
            }
            return lcSegmentList[anOccurrence];
        }

        /// <summary>
        /// Gets a segment size.
        /// </summary>
        /// <param name="aSegmentMarker">the segment'str marker</param>
        /// <returns>the size of the marker, zero if none found</returns>
        public int GetSegmentCount(byte aSegmentMarker)
        {
            IList<byte[]> lcSegmentList = this.GetSegmentList(aSegmentMarker);
            if (lcSegmentList == null)
            {
                return 0;
            }
            return lcSegmentList.Count;
        }

        /// <summary>
        /// Removes a segment using its marker and occurence value.
        /// </summary>
        /// <param name="aSegmentMarker">the segment'str marker</param>
        /// <param name="anOccurrence">the segment'str occurence</param>
        public void RemoveSegmentOccurrence(byte aSegmentMarker, int anOccurrence)
        {
            IList<byte[]> lcSegmentList = this.GetSegmentList(aSegmentMarker);
            if (lcSegmentList != null)
            {
                lcSegmentList.RemoveAt(anOccurrence);
            }

        }

        /// <summary>
        /// Removes a segment using its marker and occurence value.
        /// </summary>
        /// <param name="aSegmentMarker">the segment'str marker</param>
        public void RemoveSegment(byte aSegmentMarker)
        {
            if (this.segmentDataMap.ContainsKey(aSegmentMarker))
            {
                this.segmentDataMap.Remove(aSegmentMarker);
            }
        }

        /// <summary>
        /// Gets the segment list of value.
        /// </summary>
        /// <param name="aSegmentMarker">the segment marker</param>
        /// <returns>the segemnt list of value, null if none found</returns>
        private IList<byte[]> GetSegmentList(byte aSegmentMarker)
        {
            if (this.segmentDataMap.ContainsKey(aSegmentMarker))
            {
                return this.segmentDataMap[aSegmentMarker];
            }
            return null;
        }

        /// <summary>
        /// Gets or creates the segment value with the given marker key.
        /// </summary>
        /// <param name="aSegmentMarker">the segment'str marker</param>
        /// <returns>the segment marker you were looking for, or a new one if none exist</returns>
        private IList<byte[]> GetOrCreateSegmentList(byte aSegmentMarker)
        {
            IList<byte[]> lcSegmentList = null;
            if (this.segmentDataMap.ContainsKey(aSegmentMarker))
            {
                lcSegmentList = this.segmentDataMap[aSegmentMarker];
            }
            else
            {
                lcSegmentList = new List<byte[]>();
                segmentDataMap.Add(aSegmentMarker, lcSegmentList);
            }
            return lcSegmentList;
        }

        /// <summary>
        /// Indicates if the segment is present or not.
        /// </summary>
        /// <param name="aSegmentMarker">the segment'str marker you are looking for</param>
        /// <returns>true if present false if not</returns>
        public bool ContainsSegment(byte aSegmentMarker)
        {
            return this.segmentDataMap.ContainsKey(aSegmentMarker);
        }

        /// <summary>
        /// Writes the aSegmentData to a aFile.
        /// </summary>
        /// <param name="aFileName">where to write the information</param>
        /// <param name="aSegmentData">what to write in the aFile</param>
        public static void ToFile(string aFileName, JpegSegmentData aSegmentData)
        {
            FileStream lcFileStream = null;
            try
            {
                lcFileStream = new FileStream(aFileName, FileMode.CreateNew);
                BinaryFormatter lcBinFor = new BinaryFormatter();
                lcBinFor.Serialize(lcFileStream, aSegmentData);
            }
            finally
            {
                if (lcFileStream != null)
                {
                    lcFileStream.Close();
                    lcFileStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Loads a jpegsegmentdata from a file.
        /// </summary>
        /// <param name="aFileName">where to find data</param>
        /// <returns>the jpegsegment asked</returns>
        public static JpegSegmentData FromFile(string aFileName)
        {
            FileStream lcFileStream = null;
            try
            {
                lcFileStream = new FileStream(aFileName, FileMode.Open);
                BinaryFormatter lcBinFor = new BinaryFormatter();
                return (JpegSegmentData)lcBinFor.Deserialize(lcFileStream);
            }
            finally
            {
                if (lcFileStream != null)
                {
                    lcFileStream.Close();
                    lcFileStream.Dispose();
                }
            }
        }
    }
}
