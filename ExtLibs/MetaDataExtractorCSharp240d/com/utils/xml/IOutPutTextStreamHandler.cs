using System;
using System.Collections.Generic;
using System.Text;
using com.drew.metadata;

namespace com.utils.xml
{
    /// <summary>
    /// This class handles output text format for metadata.
    /// </summary>
    public interface IOutPutTextStreamHandler
    {
        /// <summary>
        /// Get/set the unknown option
        /// </summary>
        bool DoUnknown
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the metdata attribute
        /// </summary>
        Metadata Metadata
        {
            get;
            set;
        }

        /// <summary>
        /// Start out put stream
        /// </summary>
        /// <param name="aBuff">where to put informations</param>
        /// <param name="someParam">Can be used for anything</param>
        void StartTextStream(StringBuilder aBuff, string[] someParam);

        /// <summary>
        /// Finish out put stream
        /// </summary>
        /// <param name="aBuff">where to put informations</param>
        /// <param name="someParam">Can be used for anything</param>
        void EndTextStream(StringBuilder aBuff, string[] someParam);


        /// <summary>
        /// Transform the Metadata object into a text stream.
        /// </summary>
        /// <returns>The Metadata object as a text stream</returns>
        string AsText();

        /// <summary>
        /// Normalize a value into the text stream
        /// </summary>
        /// <param name="aBuff">where to put normalized value</param>
        /// <param name="aValue">the value to normalize</param>
        /// <param name="useSpecific">if true will use specific stream, if false will replace FORBIDEN chars by their normal value</param>
        void Normalize(StringBuilder aBuff, string aValue, bool useSpecific);
    }
}
