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
    /// This class is a bundle class.<br/>
    /// 
    /// Used for internationalisation (multi-language).<br/>
    /// 
    /// Allow the use of messages with holes.<br/>
    /// 
    /// Example:
    /// <pre>
    /// KEY1=Hello
    /// KEY2=Hello {0}
    /// KEY3=Hello {0} with an age of {1}
    /// Then you will use :
    /// myBundle["KEY1"];
    /// myBundle["KEY2", "Jhon"];
    /// myBundle["KEY3", "Jhon", 32.ToString()];
    /// myBundle["KEY3", new string[] {"Jhon", 32.ToString()}];
    /// </pre>
    /// </summary>
    sealed class ResourceBundle : AbstractResourceBundle
    {
        private IDictionary<string, string> resourceManager;
        public override IDictionary<string, string> Entries
        {
            get { return this.resourceManager; }
        }


        /// <summary>
        /// Indexator on a aMessage with many holes {0}, {1}, {2] ... in it.
        /// </summary>
        /// <param name="aKey">the referenced key</param>
        /// <param name="fillGapWith">what to put in holes. fillGapWith[0] used for {0}, fillGapWith[1] used for {1} ...</param>
        /// <returns>the aMessage attached to this key, or launch a MissingResourceException if none found</returns>
        public override string this[string aKey, string[] fillGapWith]
        {
            get
            {
                string resu = this.resourceManager[aKey];
                if (resu == null)
                {
                    throw new MissingResourceException("\"" + aKey + "\" Not found");
                }
                return replace(resu, fillGapWith);
            }
        }

        /// <summary>
        /// Constructor of the object.
        /// 
        /// Keep private, use the other one.
        /// </summary>
        private ResourceBundle()
            : base()
        {
        }

        /// <summary>
        /// Constructor of the object. <br/>
        /// Will use default resources by <i>default</i>.
        /// </summary>
        /// <param name="aPropertyFileName">The resource file where to find keys. Do not add the extension and do not forget to add your resource file into the assembly.</param>
        public ResourceBundle(string aPropertyFileName)
            : this(aPropertyFileName, null)
        {
        }

        /// <summary>
        /// Constructor of the object.
        /// </summary>
        /// <param name="aPropertyFileName">The resource file where to find keys. Do not add the extension and do not forget to add your resource file into the assembly.</param>
        /// <param name="aCultureInfo">The culture info. Can be null</param>
        public ResourceBundle(string aPropertyFileName, CultureInfo aCultureInfo)
            : base()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();              
     
            StreamReader reader = null;
            string cultureInfo = "";
            if (aCultureInfo != null && !aCultureInfo.IsNeutralCulture)
            {
                cultureInfo = aCultureInfo.TwoLetterISOLanguageName + ".";
            }
            string rsFile = assembly.GetName().Name +".resources." + cultureInfo + aPropertyFileName + ".txt";
            string defaultFile = assembly.GetName().Name + ".resources." + aPropertyFileName + ".txt";
            Stream strm = null;
            Encoding useEncoding = Encoding.UTF8;
            try
            {

                strm = assembly.GetManifestResourceStream(rsFile);
                reader = new StreamReader(strm, useEncoding);
                this.resourceManager = this.LoadFromFile(reader);
            }
            catch (Exception)
            {                
                // Console.Error.WriteLine("Caution : Resource file '" + rsFile + "' was not found ! (" + e.Message + "). Will use default file '" + defaultFile + "'");
                try
                {

                    strm = assembly.GetManifestResourceStream(defaultFile);
                    reader = new StreamReader(strm, useEncoding);
                    this.resourceManager = this.LoadFromFile(reader);
                }
                catch (Exception e2)
                {
                    Console.Error.WriteLine("Caution : Default Resource file '" + defaultFile + "' was not found too ! (" + e2.Message + ").");
                    this.resourceManager = new Dictionary<string, string>(0);
                }

            }
            finally
            {
                if (strm != null)
                {
                    strm.Close();
                    strm.Dispose();
                }

                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            this.Name = aPropertyFileName;
            this.Fullname = rsFile;
        }

        /// <summary>
        /// Reads a stream and take out the <i>bundle</i>. <br/>
        /// This method was created beacause resources file is a pain in the ace to handle for consol application.
        /// </summary>
        /// <param name="aStream">A stream. Caution : you are responsible for opening and closing this stream.</param>
        /// <returns>A dictionnary with all info stored as key=value</returns>
        private IDictionary<string, string> LoadFromFile(StreamReader aStream)
        {
            string line = null;
            IDictionary<string, string> bundle = new Dictionary<string, string>();
            while (!aStream.EndOfStream)
            {
                line = aStream.ReadLine();
                if (line != null && !line.StartsWith("#") && line.Length > 0)
                {
                    int id = line.IndexOf("=");
                    if (id > 0)
                    {
                        string key = line.Substring(0, id);
                        string valueFk = line.Substring(id + 1);
                        bundle.Add(key, valueFk);
                    }
                }
            }
            return bundle;
        }
    }
}