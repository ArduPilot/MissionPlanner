#if !LIB
using System;

using NUnit.Core;
using NUnit.Framework;

namespace Org.BouncyCastle.Asn1.Tests
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
                TestSuite suite = new TestSuite("ASN.1 tests");
			    // TODO Add these tests to RegressionTest list
			    suite.Add(new Asn1SequenceParserTest());
			    suite.Add(new OctetStringTest());
			    suite.Add(new ParseTest());
			    suite.Add(new TimeTest());
			    return suite;
            }
        }
    }
}
#endif
