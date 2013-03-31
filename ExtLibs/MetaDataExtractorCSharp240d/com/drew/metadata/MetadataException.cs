using System;
using com.drew.lang;

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
namespace com.drew.metadata
{
	/// <summary>
	/// This class represents a Metadata exception
	/// </summary>
	public class MetadataException : CompoundException 
	{
		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="aMessage">The error aMessage</param>
		public MetadataException(string aMessage) : base(aMessage)
		{
		}

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="aMessage">The error aMessage</param>
		/// <param name="aCause">The aCause of the exception</param>
		public MetadataException(string aMessage, Exception aCause) : base(aMessage, aCause)
		{
		}

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="aCause">The aCause of the exception</param>
        public MetadataException(Exception aCause)
            : base(aCause.Message, aCause)
		{
		}
	}
}
