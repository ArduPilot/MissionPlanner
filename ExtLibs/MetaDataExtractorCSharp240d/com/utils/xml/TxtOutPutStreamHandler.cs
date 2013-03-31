using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.drew.metadata;

namespace com.utils.xml
{
    /// <summary>
    /// This class will handle text for a metatdata class
    /// </summary>
    public class TxtOutPutStreamHandler : IOutPutTextStreamHandler
    {
        private static Dictionary<string, string> FORBIDEN_CHAR = TxtOutPutStreamHandler.BuildForbidenChar();

        private Metadata metadata;
        public Metadata Metadata
        {
            get
            {
                return this.metadata;
            }
            set
            {
                this.metadata = value;
            }
        }

        /// <summary>
        /// Get/set the unknown option
        /// </summary>
        private bool doUnknown;
        public bool DoUnknown
        {
            get
            {
                return this.doUnknown;
            }
            set
            {
                this.doUnknown = value;
            }
        }

        /// <summary>
        /// Constructor of the object.
        /// </summary>
        public TxtOutPutStreamHandler()
            : this(null)
        {
        }

        /// <summary>
        /// Constructor of the object.
        /// </summary>
        /// <param name="aMetadata">the metadata that shoud be transformed into txt</param>
        public TxtOutPutStreamHandler(Metadata aMetadata)
            : base()
        {
            this.Metadata = aMetadata;
        }

        /// <summary>
        /// Gives all forbiden letter in txt standard and their correspondance.
        /// </summary>
        /// <returns>All forbiden chars and their txt correspondance</returns>
        private static Dictionary<string, string> BuildForbidenChar()
        {
            // Dos consol hates french and no US language ;-)
            Dictionary<string, string> lcResu = new Dictionary<string, string>(11);
            // Usion Unicode for better behavior
            lcResu.Add("\xE9", "e");//e2
            lcResu.Add("\xE8", "e");//e4
            lcResu.Add("\xEA", "e");//e3
            lcResu.Add("\xF9", "u");//u4
            lcResu.Add("\xE2", "a");//a3
            lcResu.Add("\xE0", "a");//a4
            lcResu.Add("\xE4", "a");//a5
            lcResu.Add("\xEE", "i");//i3
            lcResu.Add("\xEF", "i");//i5
            lcResu.Add("\xF4", "o");//o3
            lcResu.Add("\xF6", "o");//o5
            return lcResu;
        }

        /// <summary>
        /// Normalize a value into Txt
        /// </summary>
        /// <param name="aBuff">where to put new XML value</param>
        /// <param name="aValue">the value to normalize</param>
        /// <param name="useCdata">if false will replace FORBIDEN chars by their normal value, else will do nothing</param>
        public virtual void Normalize(StringBuilder aBuff, string aValue, bool useCdata)
        {
            if (aValue != null)
            {
                if (useCdata)
                {
                    aBuff.Append(aValue);
                }
                else
                {
                    // check if value contains strange char and replace them if needed
                    foreach(KeyValuePair<string, string> lcPair in FORBIDEN_CHAR)
                    {
                        aValue = aValue.Replace(lcPair.Key, lcPair.Value);
                    }
                    aBuff.Append(aValue);
                }
            }
        }

        /// <summary>
        /// Creates an TXT tag using the Tag object info.
        /// </summary>
        /// <param name="aBuff">where to put tag</param>
        /// <param name="aTag">the tag</param>
        protected virtual void CreateTag(StringBuilder aBuff, Tag aTag)
        {
            if (aTag != null)
            {
                string lcDescription = null;
                try
                {
                    lcDescription = aTag.GetDescription();
                }
                catch (MetadataException)
                {
                    // Does not care here
                }
                string lcName = aTag.GetTagName();
                if (!this.DoUnknown && (lcName.ToLower().StartsWith("unknown") || lcDescription.ToLower().StartsWith("unknown")))
                {
                    // No unKnown and is unKnown so do nothing
                    return;
                }
                Normalize(aBuff, lcName, false); 
                aBuff.Append('=');
                Normalize(aBuff, lcDescription, false);
                aBuff.AppendLine();
            }
        }

        /// <summary>
        /// Creates a directory tag.
        /// </summary>
        /// <param name="aBuff">where to put info</param>
        /// <param name="aDirectory">the information to add</param>
        protected virtual void CreateDirectory(StringBuilder aBuff, AbstractDirectory aDirectory)
        {
            if (aDirectory != null)
            {
                aBuff.Append("--| ").Append(aDirectory.GetName()).Append(" |--");
                aBuff.AppendLine();
                foreach(Tag lcTag in aDirectory) {
                    CreateTag(aBuff, lcTag);
                }
            }
        }

        /// <summary>
        /// Transform the metatdat object into an TXT stream.
        /// </summary>
        /// <returns>The metadata object as TXT stream</returns>
        public virtual string AsText()
        {
            StringBuilder lcBuff = new StringBuilder();
			foreach(AbstractDirectory lcDirectory in this.Metadata)
			{
                CreateDirectory(lcBuff, lcDirectory);
            }
            return lcBuff.ToString();
        }

        /// <summary>
        /// Start out put stream. Does nothing.
        /// </summary>
        /// <param name="aBuff">where to put informations</param>
        /// <param name="someParam">Can be used for anything</param>
        public void StartTextStream(StringBuilder aBuff, string[] someParam)
        {
            // Does nothing
        }

        /// <summary>
        /// Finish out put stream. Does nothing.
        /// </summary>
        /// <param name="aBuff">where to put informations</param>
        /// <param name="someParam">Can be used for anything</param>
        public void EndTextStream(StringBuilder aBuff, string[] someParam)
        {
            // Does nothing
        }
    }
}
