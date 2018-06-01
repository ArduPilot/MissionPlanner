using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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
/// Created on 28 April 2002, 17:40
/// Modified 04 Aug 2002
/// - Adjusted javadoc
/// - Added
/// Modified 29 Oct 2002 (v1.2)
/// - Stored IFD directories in separate tag-spaces
/// - iterator() now returns an Iterator over a list of TagValue objects
/// - More get///Description() methods to detail GPS tags, among others
/// - Put spaces between words of tag name for presentation reasons (they had no  significance in compound form)
/// 
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com.drew.metadata
{
	[Serializable]
    public sealed class Metadata : IEnumerable<AbstractDirectory>
	{

        /// <summary>
        /// Creates an Iterator over the tag types set against this image, preserving the 
        /// order in which they were set.  Should the same tag have been set more than once, 
        /// it'str first position is maintained, even though the final value is used. 
        /// </summary>
        /// <returns>an Iterator of tag types set for this image</returns>
        IEnumerator<AbstractDirectory> IEnumerable<AbstractDirectory>.GetEnumerator()
        {
            return GetDirectoryIterator();
        }

        /// <summary>
        /// Creates an Iterator over the tag types set against this image, preserving the 
        /// order in which they were set.  Should the same tag have been set more than once, 
        /// it'str first position is maintained, even though the final value is used. 
        /// </summary>
        /// <returns>an Iterator of tag types set for this image</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetDirectoryIterator();
        }

        private IDictionary<Type, AbstractDirectory> directoryMap;

		/// <summary>
		/// Creates a new instance of Metadata. 
		/// </summary>
		public Metadata() : base()
		{
            this.directoryMap = new Dictionary<Type, AbstractDirectory>();
		}

		/// <summary>
		/// Creates an Iterator over the tag types set against this image, preserving the 
		/// order in which they were set.  Should the same tag have been set more than once, 
		/// it'str first position is maintained, even though the final value is used. 
		/// </summary>
		/// <returns>an Iterator of tag types set for this image</returns>
		public IEnumerator<AbstractDirectory> GetDirectoryIterator() 
		{
            return this.directoryMap.Values.GetEnumerator();
		}

		/// <summary>
		/// Gets a directory regarding its type
		/// </summary>
        /// <param name="aTypeStr">the type you are looking for</param>
		/// <returns>the directory found</returns>
		/// <exception cref="ArgumentException">if aType is not a Directory like class</exception>
		public AbstractDirectory GetDirectory(string aTypeStr) 
		{
            Type aType = Type.GetType(aTypeStr);
			if (!Type.GetType("com.drew.metadata.AbstractDirectory").IsAssignableFrom(aType)) 
			{
                throw new ArgumentException("Class type passed to GetDirectory must be an implementation of com.drew.metadata.AbstractDirectory");
			}

			// check if we've already issued this type of directory
            if (this.ContainsDirectory(aType)) 
			{
				return directoryMap[aType];
			}
            AbstractDirectory lcDirectory = null;
			try 
			{
				ConstructorInfo[] lcConstructor = aType.GetConstructors();
				lcDirectory = (AbstractDirectory) lcConstructor[0].Invoke(null);
			} 
			catch (Exception e) 
			{
				throw new SystemException(
					"Cannot instantiate provided Directory type: "
					+ aType, e);
			}
			// store the directory in case it'str requested later
			this.directoryMap.Add(aType, lcDirectory);
		
			return lcDirectory;
		}

		/// <summary>
		/// Indicates whether a given directory type has been created in this aMetadata repository.
		/// Directories are created by calling getDirectory(Class).
		/// </summary>
		/// <param name="aType">the Directory type</param>
		/// <returns>true if the aMetadata directory has been created</returns>
		public bool ContainsDirectory(Type aType) 
		{
			return this.directoryMap.ContainsKey(aType);
		}
	}
}