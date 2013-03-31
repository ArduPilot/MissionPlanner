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
    /// This class is an abstract bundle class.<br/>
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
    abstract class AbstractResourceBundle : IResourceBundle
    {
        private string name;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private string fullName;

        public string Fullname
        {
            get { return this.fullName; }
            set { this.fullName = value; }
        }

        public abstract IDictionary<string, string> Entries
        {
            get;
        }

        /// <summary>
        /// Indexator on a simple aMessage.
        /// </summary>
        /// <param name="aKey">the referenced key</param>
        /// <returns>the aMessage attached to this key, or launch a MissingResourceException if none found</returns>
        public string this[string aKey]
        {
            get
            {
                return this[aKey, new string[] { null }];
            }
        }

        /// <summary>
        /// Indexator on a aMessage with one hole {0} in it.
        /// </summary>
        /// <param name="aKey">the referenced key</param>
        /// <param name="fillGapWith">what to put in hole {0}</param>
        /// <returns>the aMessage attached to this key, or launch a MissingResourceException if none found</returns>
        public string this[string aKey, string fillGapWith]
        {
            get
            {
                return this[aKey, new string[] { fillGapWith }];
            }
        }

        /// <summary>
        /// Indexator on a aMessage with two holes {0} and {1} in it.
        /// </summary>
        /// <param name="aKey">the referenced key</param>
        /// <param name="fillGap0">what to put in hole {0}</param>
        /// <param name="fillGap1">what to put in hole {1}</param>
        /// <returns>the aMessage attached to this key, or launch a MissingResourceException if none found</returns>
        public string this[string aKey, string fillGap0, string fillGap1]
        {
            get
            {
                return this[aKey, new string[] { fillGap0, fillGap1 }];
            }
        }


        /// <summary>
        /// Indexator on a aMessage with many holes {0}, {1}, {2] ... in it.
        /// </summary>
        /// <param name="aKey">the referenced key</param>
        /// <param name="fillGapWith">what to put in holes. fillGapWith[0] used for {0}, fillGapWith[1] used for {1} ...</param>
        /// <returns>the aMessage attached to this key, or launch a MissingResourceException if none found</returns>
        public abstract string this[string aKey, string[] fillGapWith]
        {
            get;
        }

        /// <summary>
        /// Constructor of the object.
        /// 
        /// Keep private, use the other one.
        /// </summary>
        protected AbstractResourceBundle()
            : base()
        {
        }


        /// <summary>
        /// Fills the gap in a string.
        /// </summary>
        /// <param name="aLine">where to fill the gap. A gap is {0} or {1} ...</param>
        /// <param name="fillGapWith">what to put in the gap. fillGapWith[0] will go in {0} and so on</param>
        /// <returns></returns>
        protected string replace(string aLine, string[] fillGapWith)
        {
            for (int i = 0; i < fillGapWith.Length; i++)
            {
                if (fillGapWith[i] == null)
                {
                    fillGapWith[i] = "";
                }
                aLine = aLine.Replace("{" + i + "}", fillGapWith[i]);
            }
            return aLine;
        }
    }
}