#if !LIB
using System;

using NUnit.Core;
using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Cms.Tests
{
    public class AllTests
    {
        public static void Main(string[] args)
        {
            Suite.Run(new NullListener(), NUnit.Core.TestFilter.Empty);
        }

        [Suite]
        public static TestSuite Suite
        {
            get
            {
                TestSuite suite = new TestSuite("CMS Tests");
                suite.Add(new CompressedDataTest());
                suite.Add(new CompressedDataStreamTest());
                suite.Add(new EnvelopedDataTest());
                suite.Add(new EnvelopedDataStreamTest());
                suite.Add(new Rfc4134Test());
                suite.Add(new SignedDataTest());
                suite.Add(new SignedDataStreamTest());
                return suite;
            }
        }
    }
}
#endif
