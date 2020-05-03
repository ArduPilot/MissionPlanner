using System;
using System.IO;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Utilities.Test
{
    /// <summary>
    /// This is a testing utility class to check the property that a <c>Stream</c> is never
    /// closed in some particular context - typically when wrapped by another <c>Stream</c> that
    /// should not be forwarding its <c>Stream.Close()</c> calls. Not needed in production code.
    /// </summary>
	public class UncloseableStream
		: FilterStream
	{
		public UncloseableStream(
			Stream s)
			: base(s)
		{
		}

#if PORTABLE
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
			    throw new Exception("UncloseableStream was disposed");
            }

            base.Dispose(disposing);
        }
#else
        public override void Close()
		{
			throw new Exception("Close() called on UncloseableStream");
		}
#endif
    }
}
