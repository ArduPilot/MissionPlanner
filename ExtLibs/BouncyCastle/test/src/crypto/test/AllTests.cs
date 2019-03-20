using System;

#if !LIB
using NUnit.Core;
#endif
using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	[TestFixture]
	public class AllTests
	{
#if !LIB
        public static void Main(string[] args)
        {
            Suite.Run(new NullListener(), NUnit.Core.TestFilter.Empty);
        }

        [Suite]
        public static TestSuite Suite
        {
            get
            {
                TestSuite suite = new TestSuite("Lightweight Crypto Tests");
                suite.Add(new AllTests());
                suite.Add(new GcmReorderTest());
                return suite;
            }
        }
#endif

        [Test]
		public void TestCrypto()
		{
			foreach (Org.BouncyCastle.Utilities.Test.ITest test in RegressionTest.tests)
			{
				SimpleTestResult result = (SimpleTestResult)test.Perform();

				if (!result.IsSuccessful())
				{
					Assert.Fail(result.ToString());
				}
			}
		}
	}
}
