using System;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using com.drew.metadata;
using com.drew.metadata.exif;
using com.drew.metadata.iptc;
using com.drew.metadata.jpeg;
using com.drew.imaging.jpg;

using com.utils;
using com.utils.bundle;
using com.utils.xml;

/// <summary>
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com.test.resources
{
    /// <summary>
    /// Test if all references to BUNDLE["xxx"] works.
    /// </summary>
    public sealed class TestAllKeyWords
    {
        ///// <summary>
        ///// Where are all cs file, asking for the top root folder (ex: c:/temp/com")
        ///// </summary>
        //private static string CS_ROOT_FOLDER = "C:/Documents and Settings/Renaud91/Mes documents/MetaDataExtractorCSharp/MetaDataExtractor/com";
        //private static IResourceBundle BUNDLE = ResourceBundleFactory.CreateDefaultBundle("Commons", null);

        //public static void SearchAndExecuteBundle(string aCsFileName)
        //{
        //    Console.WriteLine("Reading file '" + aCsFileName + "'");
        //    FileStream lcFileStream = File.Open(aCsFileName, FileMode.Open, FileAccess.Read);
        //    byte[] lcByteRead = new byte[lcFileStream.Length];
        //    lcFileStream.Read(lcByteRead, 0, (int)lcFileStream.Length);
        //    StringBuilder lcBuff = new StringBuilder((int)lcFileStream.Length);
        //    for (int i = 0; i < lcFileStream.Length; i++)
        //    {
        //        lcBuff.Append((char)lcByteRead[i]);
        //    }            
        //    // Search for BUNDLE["
        //    string lcStr = lcBuff.ToString();
        //    int lcStartIndex = 0;
        //    while (lcStartIndex >= 0 &&lcStartIndex < lcStr.Length)
        //    {
        //        int lcFoundIndex = lcStr.IndexOf("BUNDLE[\"", lcStartIndex);
        //        int lcEndIndex = -1;
        //        if (lcFoundIndex > 0)
        //        {
        //            lcFoundIndex += +"BUNDLE[\"".Length;
        //            lcEndIndex = lcStr.IndexOf("\"", lcFoundIndex);
        //            if (lcEndIndex != -1)
        //            {
        //                try
        //                {                            
        //                    string tmp = BUNDLE[lcStr.Substring(lcFoundIndex, lcEndIndex - lcFoundIndex)];
        //                }
        //                catch (MissingResourceException e)
        //                {
        //                    Console.Error.WriteLine(e.Message);
        //                    break;
        //                }
        //            }
        //        }
        //        lcStartIndex = lcEndIndex;
        //    }
        //}

        /// <summary>
        /// Test if all references to BUNDLE["xxx"] works.
        /// </summary>
        /// <param name="someArgs">Arguments</param>
        //[STAThread]
        //public static void Main(string[] someArgs)
        //{
        //    Console.WriteLine("-- Starting TestAllKeyWords class --");
        //    // First instanciate all Directory in order to fill the Dictionnary and checks key
        //    try
        //    {
        //        new CanonDirectory();
        //        new CasioType1Directory();
        //        new CasioType2Directory();
        //        new ExifDirectory();
        //        new ExifInteropDirectory();
        //        new FujifilmDirectory();
        //        new GpsDirectory();
        //        new KodakDirectory();
        //        new KyoceraDirectory();
        //        new NikonType1Directory();
        //        new NikonType2Directory();
        //        new OlympusDirectory();
        //        new PanasonicDirectory();
        //        new PentaxDirectory();
        //        new SonyDirectory();
        //        new IptcDirectory();
        //        new JpegCommentDirectory();
        //        new JpegDirectory();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    // Then look for all CS
        //    List<string> lcAllCs = Utils.SearchAllFileIn(CS_ROOT_FOLDER, true, "*.cs");
        //    IEnumerator<string> lcEnumCs = lcAllCs.GetEnumerator();
        //    while (lcEnumCs.MoveNext())
        //    {
        //        string lcCsFileName = lcEnumCs.Current;
        //        if (lcCsFileName.Contains("Descriptor"))
        //        {
        //            //But only for descriptor ones, we checks Commons.txt
        //            SearchAndExecuteBundle(lcCsFileName);
        //        }
        //    }
        //    Console.WriteLine("-- Ending TestAllKeyWords class --");
        //    Console.ReadLine();
        //}
    }
}