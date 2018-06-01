using System;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using com.drew.metadata;
using com.drew.metadata.iptc;
using com.drew.imaging.jpg;

/// <summary>
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com
{
    /// <summary>
    /// This class is a simple example of how to use the classes inside this project.
    /// </summary>
    public sealed class SimpleRun
    {
        /// <summary>
        /// Shows all metadata and all tag for one file.
        /// </summary>
        /// <param name="aFileName">the image file name (ex: c:/temp/a.jpg)</param>
        /// <returns>The information about the image as a string</returns>
        public static String ShowOneFileAllMetaDataAllTag(string aFileName)
        {
            Metadata lcMetadata = null;
            try
            {
                FileInfo lcImgFile = new FileInfo(aFileName);
                // Loading all meta data
                lcMetadata = JpegMetadataReader.ReadMetadata(lcImgFile);
            }
            catch (JpegProcessingException e)
            {
                Console.Error.WriteLine(e.Message);
                return "Error";
            }

            // Now try to print them
            StringBuilder lcBuff = new StringBuilder(1024);
            lcBuff.Append("---> ").Append(aFileName).Append(" <---").AppendLine();
            // We want all directory, so we iterate on each
            foreach(AbstractDirectory lcDirectory in lcMetadata)
            {
                // We look for potential error
                if (lcDirectory.HasError)
                {
                    Console.Error.WriteLine("Some errors were found, activate trace using /d:TRACE option with the compiler");
                }
                lcBuff.Append("---+ ").Append(lcDirectory.GetName()).AppendLine();
                // Then we want all tags, so we iterate on the current directory
                foreach(Tag lcTag in lcDirectory) {
                    string lcTagDescription = null;
                    try
                    {
                        lcTagDescription = lcTag.GetDescription();
                    }
                    catch (MetadataException e)
                    {
                        Console.Error.WriteLine(e.Message);
                    }
                    string lcTagName = lcTag.GetTagName();
                    lcBuff.Append(lcTagName).Append('=').Append(lcTagDescription).AppendLine();

                    lcTagDescription = null;
                    lcTagName = null;
                }
            }
            lcMetadata = null;

            return lcBuff.ToString();
        }

        /// <summary>
        /// Shows only IPTC directory and all of its tag for one file.
        /// </summary>
        /// <param name="aFileName">the image file name (ex: c:/temp/a.jpg)</param>
        /// <returns>The information about IPTC for this image as a string</returns>
        public static String ShowOneFileOnlyIptcAllTag(string aFileName)
        {
            Metadata lcMetadata = null;
            try
            {
                FileInfo lcImgFile = new FileInfo(aFileName);
                // Loading all meta data
                lcMetadata = JpegMetadataReader.ReadMetadata(lcImgFile);
            }
            catch (JpegProcessingException e)
            {
                Console.Error.WriteLine(e.Message);
                return "Error";
            }

            // Now try to print them
            StringBuilder lcBuff = new StringBuilder(1024);
            lcBuff.Append("---> ").Append(aFileName).Append(" <---").AppendLine();
            // We want anly IPCT directory
            IptcDirectory lcIptDirectory = (IptcDirectory)lcMetadata.GetDirectory("com.drew.metadata.iptc.IptcDirectory");
            if (lcIptDirectory == null)
            {
                lcBuff.Append("No Iptc for this image.!").AppendLine();
                return lcBuff.ToString();
            }

            // We look for potential error
            if (lcIptDirectory.HasError)
            {
                Console.Error.WriteLine("Some errors were found, activate trace using /d:TRACE option with the compiler");
            }

            // Then we want all tags, so we iterate on the Iptc directory
            foreach(Tag lcTag in lcIptDirectory) {
                string lcTagDescription = null;
                try
                {
                    lcTagDescription = lcTag.GetDescription();
                }
                catch (MetadataException e)
                {
                    Console.Error.WriteLine(e.Message);
                }
                string lcTagName = lcTag.GetTagName();
                lcBuff.Append(lcTagName).Append('=').Append(lcTagDescription).AppendLine();

                lcTagDescription = null;
                lcTagName = null;
            }

            return lcBuff.ToString();
        }

        /// <summary>
        /// Shows only IPTC directory and only the TAG_HEADLINE value for one file.
        /// </summary>
        /// <param name="aFileName">the image file name (ex: c:/temp/a.jpg)</param>
        /// <returns>The information about IPTC for this image but only the TAG_HEADLINE tag as a string</returns>
        public static string ShowOneFileOnlyIptcOnlyTagTAG_HEADLINE(string aFileName)
        {
            Metadata lcMetadata = null;
            try
            {
                FileInfo lcImgFile = new FileInfo(aFileName);
                // Loading all meta data
                lcMetadata = JpegMetadataReader.ReadMetadata(lcImgFile);
            }
            catch (JpegProcessingException e)
            {
                Console.Error.WriteLine(e.Message);
                return "Error";
            }

            // Now try to print them
            StringBuilder lcBuff = new StringBuilder(1024);
            lcBuff.Append("---> ").Append(aFileName).Append(" <---").AppendLine();
            // We want anly IPCT directory
            IptcDirectory lcIptDirectory = (IptcDirectory)lcMetadata.GetDirectory("com.drew.metadata.iptc.IptcDirectory");
            if (lcIptDirectory == null)
            {
                lcBuff.Append("No Iptc for this image.!").AppendLine();
                return lcBuff.ToString();
            }

            // We look for potential error
            if (lcIptDirectory.HasError)
            {
                Console.Error.WriteLine("Some errors were found, activate trace using /d:TRACE option with the compiler");
            }

            // Then we want only the TAG_HEADLINE tag
            if (!lcIptDirectory.ContainsTag(IptcDirectory.TAG_HEADLINE))
            {
                lcBuff.Append("No TAG_HEADLINE for this image.!").AppendLine();
                return lcBuff.ToString();
            }
            string lcTagDescription = null;
            try
            {
                lcTagDescription = lcIptDirectory.GetDescription(IptcDirectory.TAG_HEADLINE);
            }
            catch (MetadataException e)
            {
                Console.Error.WriteLine(e.Message);
            }
            string lcTagName = lcIptDirectory.GetTagName(IptcDirectory.TAG_HEADLINE);
            lcBuff.Append(lcTagName).Append('=').Append(lcTagDescription).AppendLine();

            return lcBuff.ToString();
        }

        /*
        [STAThread]
        public static void Main(string[] someArgs)
        {
            string lcFileName = "c:/temp/a.jpg";
            Console.WriteLine(ShowOneFileAllMetaDataAllTag(lcFileName));
            Console.ReadLine();
            Console.WriteLine(ShowOneFileOnlyIptcAllTag(lcFileName));
            Console.ReadLine();
            Console.WriteLine(ShowOneFileOnlyIptcOnlyTagTAG_HEADLINE(lcFileName));
            Console.ReadLine();
        }
         */
    }
}