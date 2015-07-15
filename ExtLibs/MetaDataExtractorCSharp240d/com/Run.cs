using System;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

using com.drew.lang;
using com.drew.metadata;
using com.drew.metadata.exif;
using com.drew.imaging.jpg;
using com.drew.imaging.tiff;

using com.utils;
using com.utils.bundle;
using com.utils.xml;

/// <summary>
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com
{
    public sealed class Run
    {
        private static readonly string AS_XML = "asXml";
        private static readonly string AS_XML2 = "asXml2";
        private static readonly string NO_UNKNOWN = "noUnknown";
        private static readonly string DO_SUB = "doSub";

        private static byte asXml = 0;
        private static bool noUnknown = false;
        private static bool doSub = false;

        /// <summary>
        /// Search for the asXml parameter in the given args.
        /// </summary>
        /// <param name="someArgs">the given args</param>
        private static void FindAsXml(string[] someArgs)
        {
            for (int i = 0; i < someArgs.Length; i++)
            {
                if (AS_XML2.Equals(someArgs[i], StringComparison.OrdinalIgnoreCase))
                {
                    Run.asXml = (byte)2;
                    break;
                }

                if (AS_XML.Equals(someArgs[i], StringComparison.OrdinalIgnoreCase))
                {
                    Run.asXml = (byte)1;
                    break;
                }
            }
        }

        /// <summary>
        /// Search for the noUnknown parameter in the given args.
        /// </summary>
        /// <param name="someArgs">the given args</param>
        private static void FindNoUnknown(string[] someArgs)
        {
            for (int i = 0; i < someArgs.Length; i++)
            {
                if (NO_UNKNOWN.Equals(someArgs[i], StringComparison.OrdinalIgnoreCase))
                {
                    Run.noUnknown = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Search for the doSub parameter in the given args.
        /// </summary>
        /// <param name="someArgs">the given args</param>
        private static void FindDoSub(string[] someArgs)
        {
            for (int i = 0; i < someArgs.Length; i++)
            {
                if (DO_SUB.Equals(someArgs[i], StringComparison.OrdinalIgnoreCase))
                {
                    Run.doSub = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Search for file names in the given args.
        /// </summary>
        /// <param name="someArgs">the given args</param>
        /// <returns>a file name list</returns>
        private static List<string> FindFileNames(string[] someArgs)
        {
            List<string> lcResu = new List<string>(someArgs.Length);
            for (int i = 0; i < someArgs.Length; i++)
            {
                if (AS_XML.Equals(someArgs[i], StringComparison.OrdinalIgnoreCase) || 
                    NO_UNKNOWN.Equals(someArgs[i], StringComparison.OrdinalIgnoreCase) || 
                    DO_SUB.Equals(someArgs[i], StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                lcResu.AddRange(Utils.SearchAllFileIn(someArgs[i], Run.doSub, "*.jpg"));
                lcResu.AddRange(Utils.SearchAllFileIn(someArgs[i], Run.doSub, "*.raw"));
                lcResu.AddRange(Utils.SearchAllFileIn(someArgs[i], Run.doSub, "*.cr2"));
                lcResu.AddRange(Utils.SearchAllFileIn(someArgs[i], Run.doSub, "*.crw"));
            }
            return lcResu;
        }


        /// <summary>
        /// The example.
        /// </summary>
        /// <param name="someArgs">Arguments</param>
        [STAThread]
        public static void Main(string[] someArgs)
        {

            string aFileName = @"C:\Users\hog\Downloads\IMG_6528.JPG";

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
                return;
            }

			foreach(AbstractDirectory lcDirectory in lcMetadata)
			{

                if (lcDirectory.ContainsTag(0x9003))
                {
                    Console.WriteLine("does "+ lcDirectory.GetTagName(0x9003) +" " + lcDirectory.GetDate(0x9003) );
                }
                
            }
            Console.ReadLine();

            if (someArgs.Length == 0)
            {
                Console.Error.WriteLine("Use: MetaDataExtractor [FilePaths|DirectoryPaths] [noUnknown|asXml|asXml2|doSub]");
                Console.Error.WriteLine("     - noUnknown: will hide unknown metadata tag");
                Console.Error.WriteLine("     - asXml    : will generate an XML stream");
                Console.Error.WriteLine("     - asXml2   : will generate an XML stream with more information than asXml");
                Console.Error.WriteLine("     - doSub    : will search subdirectories for *.jpg, *.raw, *.cr2, *.crw");
                Console.Error.WriteLine("Examples:");
                Console.Error.WriteLine("     - Will show you MyImage.jpg info as text:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\MyImage.jpg");
                Console.Error.WriteLine(" or ");
                Console.Error.WriteLine("     - Will show you all *.jpg|*.raw|*.cr2|*.crw in c:\\ and img1.jpg and img2.jpg info as text:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\ d:\\img1.jpg e:\\img2.jpg");
                Console.Error.WriteLine("     - Will show you all *.jpg|*.raw|*.cr2|*.crw in c:\\ as text but with no unkown tags:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\ noUnknown");
                Console.Error.WriteLine("     - Will show you all *.jpg|*.raw|*.cr2|*.crw in c:\\ as XML:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\ asXml");
                Console.Error.WriteLine("     - Will show you all *.jpg|*.raw|*.cr2|*.crw in c:\\ as XML2 but with no unkown tags:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\ noUnknown asXml2");
                Console.Error.WriteLine("     - Will show you all *.jpg|*.raw|*.cr2|*.crw in c:\\Temp\\ and all its subdirectories as XML but with no unkown tags:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\Temp noUnknown asXml doSub");
                Console.Error.WriteLine("     - Will put in a file called sample.xml all c:\\Temp\\ *.jpg|*.raw|*.cr2|*.crw and all its subdirectories as XML but with no unkown tags:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\Temp noUnknown asXml doSub > sample.xml");
                Console.Error.WriteLine("Cautions:");
                Console.Error.WriteLine(" + Pointing on c:\\ with doSub option is a very bad idea ;-)");
                Console.ReadLine();
            }
            else
            {
                Run.FindAsXml(someArgs);
                Run.FindNoUnknown(someArgs);
                Run.FindDoSub(someArgs);

                StringBuilder lcGlobalBuff = new StringBuilder(1024);

                IOutPutTextStreamHandler lcXmlHandler = null;

                string dtdFile = null;
                if (Run.asXml == (byte)1)
                {
                    lcXmlHandler = new XmlOutPutStreamHandler();
                    dtdFile = "MetadataExtractor.dtd";
                }
                else if (Run.asXml == (byte)2)
                {
                    lcXmlHandler = new XmlNewOutPutStreamHandler();
                    dtdFile = "MetadataExtractorNew.dtd";
                }
                else
                {
                    lcXmlHandler = new TxtOutPutStreamHandler();
                }
                lcXmlHandler.DoUnknown = !Run.noUnknown;

                List<string> lcFileNameLst = Run.FindFileNames(someArgs);
                // Args for OutPutTextStream objects

                // Indicate your Xsl here
                string lcXslFileName = null; // For example: ="exif.xslt";
                // Indicate if you want to use CDDATA in your XML stream
                string useCDDATA = "false";
                string[] lcOutputParams = new string[] { "ISO-8859-1", lcXslFileName, lcFileNameLst.Count.ToString(), dtdFile, useCDDATA };

                lcXmlHandler.StartTextStream(lcGlobalBuff, lcOutputParams);
                foreach(string lcFileName in lcFileNameLst) 
                {
                    StringBuilder lcBuff = new StringBuilder(2048);
                    //Metadata lcMetadata = null;
                    try
                    {
                        FileInfo lcImgFileInfo = new FileInfo(lcFileName);
                        if (lcFileName.ToLower().EndsWith(".raw") || 
                            lcFileName.ToLower().EndsWith(".cr2") || 
                            lcFileName.ToLower().EndsWith(".crw"))
                        {
                            lcMetadata = TiffMetadataReader.ReadMetadata(lcImgFileInfo);
                        }
                        else
                        {
                            lcMetadata = JpegMetadataReader.ReadMetadata(lcImgFileInfo);
                        }
                        lcXmlHandler.Metadata = lcMetadata;
                    }
                    catch (JpegProcessingException e)
                    {
                        Console.Error.WriteLine("Could note analyse the file '" + lcFileName + "' error message is:" + e.Message);
                        break;
                    }

                    if (Run.asXml != (byte)0)
                    {
                        // First open file name tag
                        lcBuff.Append("<file name=\"");
                        lcXmlHandler.Normalize(lcBuff, lcFileName, false);
                        lcBuff.Append("\">").AppendLine();
                        // Then create all directory tag
                        lcBuff.Append(lcXmlHandler.AsText());
                        // Then close file tag
                        lcBuff.Append("</file>").AppendLine().AppendLine();
                    }
                    else
                    {
                        lcBuff.Append("-> ");
                        lcXmlHandler.Normalize(lcBuff, lcFileName, false);
                        lcBuff.Append(" <-").AppendLine();
                        // Then create all directory tag
                        lcBuff.Append(lcXmlHandler.AsText()).AppendLine();
                    }
                    lcMetadata = null;
                    // Adds result for this file to big buffer
                    lcGlobalBuff.Append(lcBuff);
                    lcGlobalBuff.AppendLine();
                }
                lcXmlHandler.EndTextStream(lcGlobalBuff, lcOutputParams);

                Console.Out.WriteLine(lcGlobalBuff.ToString());
            }
          
            // Uncomment if you are running under VisualStudio
             Console.In.ReadLine();
        }
    }
}