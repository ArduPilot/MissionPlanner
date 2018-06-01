using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Globalization;

/// <summary>
/// This is public domain software - that is, you can do whatever you want
/// with it, and include it software that is licensed under the GNU or the
/// BSD license, or whatever other licence you choose, including proprietary
/// closed source licenses.  I do ask that you leave this lcHeader in tact.
///
/// If you make modifications to this code that you think would benefit the
/// wider community, please send me a copy and I'll post it on my site.
/// 
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com.utils.bundle
{
    /// <summary>
    /// This class is a bundle factory class.<br/>
    /// 
    /// You can switch the implementation or ResourceBundle using this class.<br/>
    /// </pre>
    /// </summary>
    public sealed class SpecialResourceWriter
    {
        public SpecialResourceWriter()
        {
            // Load all bunlde
            IList<IResourceBundle> allBundle = new List<IResourceBundle>(20);
            allBundle.Add(ResourceBundleFactory.CreateBundle("CanonMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("CasioMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("Commons", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("ExifInteropMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("ExifMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("FujiFilmMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("GpsMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("IptcMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("JpegMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("KodakMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("KyoceraMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("NikonTypeMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("OlympusMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("PanasonicMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("PentaxMarkernote", null, ResourceBundleFactory.USE_TXTFILE));
            allBundle.Add(ResourceBundleFactory.CreateBundle("SonyMarkernote", null, ResourceBundleFactory.USE_TXTFILE));

            foreach(IResourceBundle bdl in allBundle)
            {
                ResourceWriter rw = new ResourceWriter(bdl.Fullname+".resources");
                IDictionary<string,string> idic = bdl.Entries;
                IDictionaryEnumerator enumDic =  (IDictionaryEnumerator)idic.GetEnumerator();
                while (enumDic.MoveNext())
                {
                    rw.AddResource((string)enumDic.Key, (string)enumDic.Value);
                }
                rw.Close();
                rw.Dispose();

            }
        }
    }

}